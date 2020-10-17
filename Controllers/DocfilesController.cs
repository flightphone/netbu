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
using System.Threading.Tasks;



namespace netbu.Controllers
{
    [Authorize]
    public class DocfilesController : Controller
    {
        [AllowAnonymous]
        public IActionResult fcheck()
        {
            filecheckAsync();
            return Content("Process started");
        }
        private async void filecheckAsync()
        {
            await Task.Run(() => filecheck());
        }
        private void filecheck()
        {
            try
            {
                string sqlu = "update cntfilehistory set fh_vol = @fh_vol where fh_pk = @fh_pk";
                SqlConnection cn = new SqlConnection(Program.AppConfig["mscns"]);
                SqlCommand cmd = new SqlCommand(sqlu, cn);
                string sql = "select fh_pk,  fh_filename from cntfilehistory(nolock) where fh_vol is null";
                SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                DataTable res = new DataTable();
                da.Fill(res);

                cn.Open();
                foreach (DataRow rw in res.Rows)
                {
                    try
                    {
                        var filename = rw["fh_filename"].ToString();
                        var fi = new FileInfo(filename);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@fh_vol", fi.Length);
                        cmd.Parameters.AddWithValue("@fh_pk", rw["fh_pk"]);
                        cmd.ExecuteNonQuery();
                    }
                    catch {; }
                }
                cn.Close();
            }
            catch {; }
        }

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

        public IActionResult file(string id, string id64)
        {
            try
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
                //async log 17/10/2020
                filelogAsync(path, "get");

                if (ext == "gif" || ext == "bmp" || ext == "jpg" || ext == "jpeg" || ext == "png")
                    ctype = "image/jpeg";
                if (ext == "tiff")
                    ctype = "image/tiff";

                //17.10.2020
                return PhysicalFile(path, ctype, Path.GetFileName(path));
                /*
                if (ctype == "application/octet-stream")
                    return PhysicalFile(path, ctype, Path.GetFileName(path));
                else
                {
                    byte[] buf = System.IO.File.ReadAllBytes(path);
                    return File(buf, ctype);
                }
                */

            }
            catch (Exception ex)
            {
                string mes = "Ошибка приложения. " + ex.Message;
                return Content(mes);
            }

        }

        //async log 17/10/2020
        private async void filelogAsync(string path, string action)
        {
            await Task.Run(() => filelog(path, action));
        }

        private void filelog(string path, string action)
        {
            //лог 18.12.2019
            try
            {
                String sql = "p_cntfilehistory_add";
                SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@fh_filename", path);
                da.SelectCommand.Parameters.AddWithValue("@fh_account", User.Identity.Name);
                da.SelectCommand.Parameters.AddWithValue("@fh_action", action);  //19/02/2020
                DataTable head = new DataTable();
                da.Fill(head);
            }
            catch
            {; }
            //лог 18.12.2019

        }
        public JsonResult delete_file(string id, string mode)
        {
            id = WebUtility.HtmlDecode(id);
            string res = "";
            try
            {
                string idf = id.Replace("/", @"\");
                string path = Program.AppConfig["docfiles"] + @"\" + idf;

                //лог 19.02.2020
                try
                {
                    String sql = "p_cntfilehistory_add";
                    SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@fh_filename", path);
                    da.SelectCommand.Parameters.AddWithValue("@fh_account", User.Identity.Name);
                    da.SelectCommand.Parameters.AddWithValue("@fh_action", "delete");
                    DataTable head = new DataTable();
                    da.Fill(head);
                }
                catch
                {; }
                //лог 19.02.2020


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
            string path = Program.AppConfig["docfiles"] + @"\" + idf;  //  / заменили на \
            try
            {


                if (files != null)
                    foreach (IFormFile img in files)
                    {

                        //лог 19.02.2020
                        try
                        {
                            String sql = "p_cntfilehistory_add";
                            SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.SelectCommand.Parameters.AddWithValue("@fh_filename", path + img.FileName);
                            da.SelectCommand.Parameters.AddWithValue("@fh_account", User.Identity.Name);
                            da.SelectCommand.Parameters.AddWithValue("@fh_action", "add");
                            DataTable head = new DataTable();
                            da.Fill(head);
                        }
                        catch
                        {; }
                        //лог 19.02.2020

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

        public IActionResult getphoto(string audtuser)
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
        public IActionResult comments(int ag_id, string ag_type, string cm_status, string cm_message)
        {
           try
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
           catch (Exception ex)
           {
               return Content(ex.Message);
           }
        }

        public IActionResult dir(string id, string id64)
        {
            try
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
            catch (Exception ex)
            {
                string mes = "Ошибка приложения. " + ex.Message;
                return Content(mes);
            }
        }
    }
}