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
using Microsoft.AspNetCore.Authorization;


namespace netbu.Controllers
{
    [Authorize]
    public class DocfilesController : Controller
    {

        public ActionResult file (string id) {
            string idf = id.Replace("/", @"\");
            string path = Program.AppConfig["docfiles"] + @"\" + idf;
            return PhysicalFile(path, "application/octet-stream", Path.GetFileName(path));

        }
        public ActionResult dir(string id)
		{
            string idf = id.Replace("/", @"\");
            string[] paths = id.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string parent = "";
            if (paths.Length > 1)
                parent = string.Join("/", paths, 0, paths.Length-1) + "/";
            string path = Program.AppConfig["docfiles"] + @"\" + idf;
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DirectoryInfo di = new DirectoryInfo(path);
            //DirectoryInfo[] dirs = di.GetDirectories();
            //FileInfo[] files = di.GetFiles();

            ViewBag.di = di;
            //ViewBag.dirs = dirs;
            //ViewBag.files = files;
            ViewBag.id = id;
            ViewBag.parent = parent;

            return View();
        }
    }
}