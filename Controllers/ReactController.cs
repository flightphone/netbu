using Microsoft.AspNetCore.Mvc;
using WpfBu.Models;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace netbu.Controllers
{
    public class ReactController : Controller
    {
        public JsonResult FinderStart(string id, string mode, string page, string Fc)
        {
            var F = new Finder();
            F.nrows = 30;
            if (!string.IsNullOrEmpty(mode))
                F.Mode = mode;
            else
                F.Mode = "new";

            if (!string.IsNullOrEmpty(page))    
                F.page = int.Parse(page);

            if (!string.IsNullOrEmpty(Fc))
            {
                List<FinderField> Fcols =JsonConvert.DeserializeObject<List<FinderField>>(Fc);
                F.Fcols = Fcols;
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
    }
}