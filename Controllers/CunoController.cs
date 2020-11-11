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
using WpfBu.Models;


namespace netbu.Controllers
{

    public class CunoController : Controller
    {
        private IHostingEnvironment _env;
        public CunoController(IHostingEnvironment env)
        {
            _env = env;
        }
        private string savefile(string id, string format, string path, string dateformat, string pref)
        {
            try
            {
                if (string.IsNullOrEmpty(format))
                    format = "csv";
                var F = new Finder();
                F.Account = "malkin";
                F.Mode = "csv";
                F.start(id);
                char r = ';';
                if (format != "csv")
                    r = '\t';
                string s = F.ExportCSV(r);
                string filename = pref + DateTime.Now.ToString(dateformat) + "." + format;
                string filepath = Program.AppConfig[path];
                string ff = filepath + @"\" +  filename;
                System.IO.File.WriteAllText(ff, s, Encoding.GetEncoding(1251));
                return ff;
            }
            catch (Exception err)
            {
                var webRoot = _env.WebRootPath;
                var errorpath = webRoot +  @"\..\netbu_error.log";
                string mes = $"{err.Message}\r\n{err.StackTrace} - {DateTime.Now}\r\n\r\n";
                System.IO.File.AppendAllText(errorpath, mes);
                return "";
            }
        }
        private async void savefileAsync(string id, string format, string path, string dateformat, string pref)
        {
            await Task.Run(() => savefile(id, format, path, dateformat, pref));
        }

        public string save(string id, string format, string path, string dateformat, string pref, string copy)
        {
            if (string.IsNullOrEmpty(copy))
            {
                savefileAsync(id, format, path, dateformat, pref);
                return "OK";
            }
            else
            {
                string ff = savefile(id, format, path, dateformat, pref);
                string res = $"copy {ff} {copy}\r\n del {ff}";
                return res;
            }
        }

        //Private Project

        public string update(string cur, string mode)
        {
            string res = "OK";

            try
            {
                String sql = "p_cuno_update_usd";
                if (cur == "rur")
                    sql = "p_cuno_update_rur";
                SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable head = new DataTable();
                da.Fill(head);

                sql = "select * from v_cuno_file_usd";
                if (cur == "rur")
                    sql = "select * from v_cuno_file_rur";
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
                if (mode == "show")
                {
                    res = resFile;
                }
                else
                {
                    string path = Program.AppConfig["cuno_usd_files"] + @"\";
                    if (cur == "rur")
                        path = Program.AppConfig["cuno_rur_files"] + @"\";
                    path = path + ((DateTime)head.Rows[0][0]).ToString("UTG_yyyyMMddHHmmss") + ".txt";
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