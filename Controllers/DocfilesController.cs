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

        [AllowAnonymous]
        public ActionResult photo(string id)
        {
            string path = Program.AppConfig["photofiles"] + @"\" + id;
            string ctype = "image/jpeg";
            if (System.IO.File.Exists(path))
            {
                byte[] buf = System.IO.File.ReadAllBytes(path);
                return File(buf, ctype);
            }
            else
                return NotFound();
        }

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
            //лог 18.12.2019
            try
            {
                String sql = "p_cntfilehistory_add";
                SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@fh_filename", path);
                da.SelectCommand.Parameters.AddWithValue("@fh_account", User.Identity.Name);
                DataTable head = new DataTable();
                da.Fill(head);
            }
            catch
            {; }
            //лог 18.12.2019


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

        public ActionResult getphoto(string audtuser)
        {
            string sql = "select  top 1 image_bmp from dbo.cntEmployees (nolock) where AD_Name = @audtuser and isnull(image_bmp, '') <> ''";
            SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
            da.SelectCommand.Parameters.AddWithValue("@audtuser", audtuser);
            DataTable foto = new DataTable();
            da.Fill(foto);
            string ctype = "image/jpeg";
            byte[] buf = { };
            if (foto.Rows.Count > 0)
            {
                buf = Convert.FromBase64String(foto.Rows[0][0].ToString());
            }
            else
            {
                buf = System.IO.File.ReadAllBytes(@"wwwroot\Image\avatar_blank.jpg");
            }
            return File(buf, ctype);

        }
        public ActionResult comments(int ag_id, string ag_type, string cm_status, string cm_message)
        {

            if (!string.IsNullOrEmpty(cm_message))
            {
                SqlDataAdapter de = new SqlDataAdapter("p_cntcomments_EDIT", Program.AppConfig["mscns"]);
                de.SelectCommand.CommandType = CommandType.StoredProcedure;
                de.SelectCommand.Parameters.AddWithValue("@cm_id", -1);
                de.SelectCommand.Parameters.AddWithValue("@ag_id", ag_id);
                de.SelectCommand.Parameters.AddWithValue("@ag_type", ag_type);
                de.SelectCommand.Parameters.AddWithValue("@cm_status", cm_status);
                de.SelectCommand.Parameters.AddWithValue("@cm_message", cm_message);
                de.SelectCommand.Parameters.AddWithValue("@audtuser", User.Identity.Name);
                de.SelectCommand.Parameters.AddWithValue("@audtdate", DateTime.Now);
                DataTable res = new DataTable();
                de.Fill(res);
            }


            String sql = "p_cntcomments_view";
            SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@ag_id", ag_id);
            da.SelectCommand.Parameters.AddWithValue("@ag_type", ag_type);
            DataTable head = new DataTable();
            DataTable comments = new DataTable();
            da.Fill(0, 0, new DataTable[] { head, comments });

            ViewBag.info = head.Rows[0][0];
            ViewBag.rows = comments.Rows;
            ViewBag.ag_id = ag_id;
            ViewBag.ag_type = ag_type;
            ViewBag.user = User.Identity.Name;

            return View();
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
            //Доступ
            int fileacc = dadataINN.FileAccess(User.Identity.Name, paths[0]);
            ViewBag.fileacc = fileacc;

            ViewBag.di = di;
            //ViewBag.dirs = dirs;
            //ViewBag.files = files;
            ViewBag.id = id;
            ViewBag.parent = parent;

            return View();
        }
    }
}