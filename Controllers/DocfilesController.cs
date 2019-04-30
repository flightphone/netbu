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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;


namespace netbu.Controllers
{
    [Authorize]
    public class DocfilesController : Controller
    {



        public ActionResult file(string id, string id64)
        {

            if (!string.IsNullOrEmpty(id64))
            {
                id64 = id64.Replace(" ", "+");
                id = Encoding.UTF8.GetString(Convert.FromBase64String(id64));
            }
            else
                id = WebUtility.HtmlDecode(id);


            string idf = id.Replace("/", @"\");
            string path = Program.AppConfig["docfiles"] + @"\" + idf;
            string ext = Path.GetExtension(path).ToLower().Replace(".", "");
            string ctype = "application/octet-stream";
            /*
            if (ext == "pdf")
                ctype = "application/pdf";
            */
            if (ext == "gif" || ext == "bmp" || ext == "jpg" || ext == "jpeg" || ext == "png")
                ctype = "image/jpeg";
            if (ext == "tiff")
                ctype = "image/tiff";

            if (ctype == "application/octet-stream")
                return PhysicalFile(path, ctype, Path.GetFileName(path));
            else
            {
                byte[] buf = System.IO.File.ReadAllBytes(path);
                return File(buf, ctype);
            }


        }
        public JsonResult delete_file(string id, string mode)
        {
            id = WebUtility.HtmlDecode(id);
            string res = "";
            try
            {
                string idf = id.Replace("/", @"\");
                string path = Program.AppConfig["docfiles"] + @"\" + idf;
                if (mode == "file")
                    System.IO.File.Delete(path);
                else
                    Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                res = e.Message;
            }
            return Json(new { error = res });
        }

        public JsonResult newdir(string id, string dir)
        {
            id = WebUtility.HtmlDecode(id);
            string res = "";
            try
            {
                string idf = id.Replace("/", @"\");
                string path = Program.AppConfig["docfiles"] + @"\" + idf + @"\" + dir;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                res = e.Message;
            }
            return Json(new { error = res });
        }
        public JsonResult upload(string id, List<IFormFile> files)
        {

            string res = "";
            id = WebUtility.HtmlDecode(id);
            string idf = id.Replace("/", @"\");
            string path = Program.AppConfig["docfiles"] + @"/" + idf;
            try
            {
                if (files != null)
                    foreach (IFormFile img in files)
                    {

                        string FileName = img.FileName;
                        int n = (int)img.Length;
                        byte[] buf = new byte[n];
                        Stream ms = img.OpenReadStream();
                        ms.Read(buf, 0, n);
                        System.IO.File.WriteAllBytes(path + img.FileName, buf);
                    }
            }
            catch (Exception e)
            {
                res = e.Message;
            }

            return Json(new { error = res });

        }

        public ActionResult dir(string id, string id64)
        {
            if (!string.IsNullOrEmpty(id64))
            {
                id64 = id64.Replace(" ", "+");
                id = Encoding.UTF8.GetString(Convert.FromBase64String(id64));
            }
            else
                id = WebUtility.HtmlDecode(id);

            string idf = id.Replace("/", @"\");
            string[] paths = id.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string parent = "";
            if (paths.Length > 1)
                parent = string.Join("/", paths, 0, paths.Length - 1) + "/";
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