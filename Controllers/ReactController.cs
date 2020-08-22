using Microsoft.AspNetCore.Mvc;
using WpfBu.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace netbu.Controllers
{
    //[Authorize]
    public class ReactController : Controller
    {
        public JsonResult exec(string EditProc, string SQLParams, string KeyF)
        {
            Dictionary<string, object> WorkRow = JsonConvert.DeserializeObject<Dictionary<string, object>>(SQLParams);
            var vals = new List<string>();
            var Param = new Dictionary<string, object>();
            foreach (string fname in WorkRow.Keys)
            {
                string pname = "@_" + fname;
                string pval = WorkRow[fname].ToString();
                DateTime dval;
                if (DateTime.TryParse(pval, out dval))
                {
                    Param.Add(pname, dval);
                }
                else
                {
                    if (string.IsNullOrEmpty(WorkRow[fname].ToString()))
                    {
                        Param.Add(pname, DBNull.Value);
                    }
                    else
                    {
                        Param.Add(pname, WorkRow[fname]);
                    }
                }
                string val = "";
                if (MainObj.IsPostgres)
                    val = $"_{fname} => {pname}";
                else
                    val = $"@{fname} = {pname}";
                vals.Add(val);
            }
            string sqlpar = string.Join(",", vals);
            string sql = "";
            if (MainObj.IsPostgres)
                sql = $"select * from {EditProc}({sqlpar})";
            else
                sql = $"exec {EditProc} {sqlpar}";

            DataTable data;
            string message = "OK";
            try
            {
                data = MainObj.Dbutil.Runsql(sql, Param);
                List<Dictionary<string, object>> MainTab = MainObj.Dbutil.DataToJson(data);
                List<string> ColumnTab = MainObj.Dbutil.DataColumn(data);
                return Json(new
                {
                    message = message,
                    MainTab = MainTab,
                    ColumnTab = ColumnTab
                });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new
                {
                    message = message
                });
            }

        }

        public JsonResult FinderStart(string id, string mode, string page, string Fc, string TextParams, string SQLParams)
        {
            var F = new Finder();
            //F.Account = User.Identity.Name;
            F.Account = "malkin";

            F.nrows = 30;
            if (!string.IsNullOrEmpty(mode))
                F.Mode = mode;
            else
                F.Mode = "new";

            if (!string.IsNullOrEmpty(page))
                F.page = int.Parse(page);

            if (!string.IsNullOrEmpty(Fc))
            {
                F.Fcols = JsonConvert.DeserializeObject<List<FinderField>>(Fc);
            }

            if (!string.IsNullOrEmpty(TextParams))
            {
                F.TextParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(TextParams);
            }

            if (!string.IsNullOrEmpty(SQLParams))
            {
                F.SQLParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(SQLParams);
            }


            try
            {
                F.start(id);
                return Json(F);
            }
            catch (Exception e)
            {
                return Json(new { Error = e.Message });
            }
        }

        public ActionResult CSV(string id, string Fc, string TextParams, string SQLParams)
        {
            var F = new Finder();
            //F.Account = User.Identity.Name;
            F.Account = "malkin";

            F.Mode = "csv";
            if (!string.IsNullOrEmpty(Fc))
            {
                List<FinderField> Fcols = JsonConvert.DeserializeObject<List<FinderField>>(Fc);
                F.Fcols = Fcols;
            }

            if (!string.IsNullOrEmpty(TextParams))
            {
                F.TextParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(TextParams);
            }

            if (!string.IsNullOrEmpty(SQLParams))
            {
                F.SQLParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(SQLParams);
            }


            F.start(id);
            string s = F.ExportCSV();
            string ctype = "application/octet-stream";
            byte[] buf = Encoding.UTF8.GetBytes(s);
            return File(buf, ctype, $"data_{id}.csv");

        }
    }
}