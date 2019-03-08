using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using netbu.Models;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Xml;

namespace netbu.Controllers
{
    public class StoreController : Controller
    {
        public ActionResult refer(string id)
		{
            int IdDeclare = int.Parse(id);
			var cnstr = Program.AppConfig["mscns"];
            string sql = "select D.*, P.ParamValue  from t_rpDeclare D(nolock) " 
                + " inner join t_sysParams P(nolock) on 'GridFind' + D.DecName = P.ParamName "
                + " where IdDeclare = @IdDeclare";
            DataTable decTab = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(sql, cnstr);
            da.SelectCommand.Parameters.AddWithValue("@IdDeclare", IdDeclare);
            da.Fill(decTab);
			
            t_rpDeclare dec = new t_rpDeclare();
            dec.rw = decTab.Rows[0];
            dec.fmtSaveFieldList = decTab.Rows[0]["SaveFieldList"].ToString();
            dec.fmtSaveFieldList = "\"" + dec.fmtSaveFieldList.Replace(",", "\",\"") + "\"";


            DataTable FieldMapTable = new DataTable();
            string smap = "select * from t_sysFieldMap where DecName = @DecName";
            da = new SqlDataAdapter(smap, cnstr);
            da.SelectCommand.Parameters.AddWithValue("@DecName", decTab.Rows[0]["DecName"]);
            da.Fill(FieldMapTable);

            string s = decTab.Rows[0]["ParamValue"].ToString();
            XmlDocument xm = new XmlDocument();
            XmlElement xRoot;
            xm.LoadXml(s);
            xRoot = xm.DocumentElement;
            dec.ListField = new Dictionary<string, Field>();
            foreach (XmlNode xNod in xRoot.SelectNodes("COLUMN"))
            {

                XmlElement xCol = (XmlElement)xNod;
                Field fld = new Field();
                fld.FieldName = xCol.Attributes["FieldName"].Value;
                fld.Title = xCol.Attributes["FieldCaption"].Value;
                fld.Vis = xCol.Attributes["Visible"].Value;
                dec.ListField.Add(fld.FieldName, fld);
                
            }    
            string DecSQL = decTab.Rows[0]["DecSQL"].ToString(); 
            DataTable resTab = new DataTable();
            da = new SqlDataAdapter(DecSQL, cnstr);
            da.Fill(resTab);

            foreach (DataColumn c in resTab.Columns)
            {
                if (dec.ListField.ContainsKey(c.ColumnName))
                    dec.ListField[c.ColumnName].Type = c.DataType.ToString();

            }
            
            DataRow[] ar = FieldMapTable.Select("KeyField = 1");
            for (int i = 0; i < ar.Length; i ++)
            if (dec.ListField.ContainsKey(ar[i]["dstField"].ToString()))
            {
                    dec.ListField[ar[i]["dstField"].ToString()].Type = "joinRow";
                    dec.ListField[ar[i]["dstField"].ToString()].GroupDec = ar[i]["GroupDec"].ToString();
            }        

            foreach(Field fld in dec.ListField.Values)
            if (fld.Type == "joinRow")
            {
                ar = FieldMapTable.Select("IdDeclare > 0 and GroupDec = '" + fld.GroupDec + "'");
                fld.IdDeclare = ar[0]["idDeclare"].ToString();
                fld.ClassName = ar[0]["ClassName"].ToString();
                ar = FieldMapTable.Select("dstField <> '" + fld.FieldName + "' and GroupDec = '" + fld.GroupDec + "'");
                fld.LookUp = new Dictionary<string, string>();
                for (int i=0; i < ar.Length; i++)
                {
                    fld.LookUp.Add(ar[i]["dstField"].ToString(), ar[i]["srcField"].ToString());
                }
                if (fld.ClassName=="Bureau.Finder")
                {
                    fld.Editor = "[Editor(typeof(joinRowFinderEditor), typeof(System.Drawing.Design.UITypeEditor))]";
                    fld.jointrue = "false";
                }
                else
                {
                    fld.Editor = "[Editor(typeof(joinRowStatusEditor), typeof(System.Drawing.Design.UITypeEditor))]";
                    fld.jointrue = "true";
                }
            }

			return View(dec);
		}
    }
}