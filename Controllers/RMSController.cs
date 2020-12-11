
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using netbu.Models;
using System.IO;


namespace netbu.Controllers
{

    public class RMSController : Controller
    {
        public IActionResult Deicing(List<IFormFile> files)
        {
            if (files.Count == 0)
            {
                return Content("Не загружен файл de-icing.xml");
            }
            else
            {
                IFormFile img = files[0];
                int n = (int)img.Length;
                byte[] buf = new byte[n];
                Stream ms = img.OpenReadStream();
                ms.Read(buf, 0, n);
                string dataLine = Encoding.UTF8.GetString(buf);
                RMParser rm = new RMParser();
                string res = rm.Parse(FileTipe.De_icing, dataLine, Program.AppConfig["mscns"]);
                return Content(res);
            }
        }

        public IActionResult Deicing2(string dataLine)
        {
            
                RMParser rm = new RMParser();
                string res = rm.Parse(FileTipe.De_icing, dataLine, Program.AppConfig["mscns"]);
                return Content(res);
            
        }

    }
}