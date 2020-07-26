using Microsoft.AspNetCore.Mvc;
using WpfBu.Models;
using System;

namespace netbu.Controllers
{
    public class ReactController : Controller
    {
        public JsonResult FinderStart(string id)
        {
            var F = new Finder();
            F.nrows = 100;
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