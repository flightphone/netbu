using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using netbu.Models;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;



namespace netbu.Controllers
{

    public class CunoController : Controller
    {

        

        public string update(string cur, string mode)
        {
            string res = "OK";
            
            try
            {
                String sql = "p_cuno_update_usd";
                SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable head = new DataTable();
                da.Fill(head);

                sql = "select * from v_cuno_file_usd";
                da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                DataTable body = new DataTable();
                da.Fill(body);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < body.Rows.Count; i++)
                {
                    string txtRow = body.Rows[i][0].ToString();
                    for (int j = 1; j < body.Columns.Count; j++)
                    {
                        txtRow = txtRow + ";" + body.Rows[i][j].ToString();
                    }
                    sb.AppendLine(txtRow);
                }
                string resFile = sb.ToString();
                if (mode=="show")
                {
                    res = resFile;
                }
                else
                {
                    string path = Program.AppConfig["cuno_usd_files"] + @"\" 
                    + ((DateTime)head.Rows[0][0]).ToString("UTG_yyyyMMddHHmmss") + ".txt";
                    System.IO.File.WriteAllText(path, resFile, Encoding.GetEncoding(1251));
                }
            }
            catch (Exception e)
            { 
                res = "error:" + e.Message;
            }
            return res;
        }


    }
}