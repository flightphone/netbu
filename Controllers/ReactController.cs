using Microsoft.AspNetCore.Mvc;
using WpfBu.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace netbu.Controllers
{
   //[Authorize]
    public class ReactController : Controller
    {
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
                List<FinderField> Fcols =JsonConvert.DeserializeObject<List<FinderField>>(Fc);
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