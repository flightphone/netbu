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
using suggestionscsharp;


namespace netbu.Controllers
{
    [Authorize]
    public class DaDataController : Controller
    {

        public ActionResult csv(string id)
        {
            string sql = "select [ls_inn], [ls_status] ,[ls_actuality_date] ,[ls_type] ,[ls_full_with_opf] ,[ls_short_with_opf], ls_load_status " +
            "from cntload_detail (nolock) where ls_ld = @ls_ld order by ls_pk";
            SqlDataAdapter da = new SqlDataAdapter(sql, Program.AppConfig["mscns"]);
            DataTable restab = new DataTable();
            da.SelectCommand.Parameters.AddWithValue("@ls_ld", id);
            da.Fill(restab);
            PrintDoc pd = new PrintDoc();
            string restxt = pd.ExportCSV(restab);
            string filen = "Task_" + id + ".csv";
            string ctype = "application/csv";
            byte[] buf = Encoding.GetEncoding(1251).GetBytes(restxt);
            return File(buf, ctype, filen);
        }

        private void Dadataupdate()
        {
            string cnstr = Program.AppConfig["mscns"];
            string sql0 = "select  * from v_not_cntdadata";
            SqlDataAdapter da = new SqlDataAdapter(sql0, cnstr);
            DataTable resTab = new DataTable();
            da.Fill(resTab);
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            try
            {

                string sql = "insert into cntdadata (contractor_id,  ls_inn,  ls_status,  ls_actuality_date, ls_type, ls_full_with_opf,  ls_short_with_opf, ls_load_status) " +
                " values (@ls_ld,  @ls_inn,  @ls_status,  @ls_actuality_date, @ls_type, @ls_full_with_opf,  @ls_short_with_opf, @ls_load_status) ";
                SqlCommand cmd = new SqlCommand(sql, cn);
                dadataINN di = new dadataINN();

                for (int i = 0; i < resTab.Rows.Count; i++)
                {
                    if (!Program.FlagDadataUpdate)
                        break;
                    string inn = resTab.Rows[i]["contractor_inn"].ToString().Trim();
                    string ls_load_status = "";
                    PartyData pdata = null;
                    try
                    {
                        pdata = di.exec(inn);
                        if (pdata == null)
                            ls_load_status = "Не найдено";
                    }
                    catch (Exception ee)
                    {
                        ls_load_status = ee.Message;
                    }
                    string ls_status = "";
                    string ls_actuality_date = "";
                    string ls_type = "";
                    string ls_full_with_opf = "";
                    string ls_short_with_opf = "";
                    if (pdata != null)
                    {

                        ls_status = pdata.state.status.ToString();

                        if (pdata.state.actuality_date_fmt != null)
                            ls_actuality_date = pdata.state.actuality_date_fmt;

                        ls_type = pdata.type.ToString();

                        if (pdata.name.full_with_opf != null)
                            ls_full_with_opf = pdata.name.full_with_opf;

                        if (pdata.name.short_with_opf != null)
                            ls_short_with_opf = pdata.name.short_with_opf;
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ls_ld", resTab.Rows[i]["contractor_id"]);
                    cmd.Parameters.AddWithValue("@ls_inn", inn);
                    cmd.Parameters.AddWithValue("@ls_status", ls_status);
                    cmd.Parameters.AddWithValue("@ls_actuality_date", ls_actuality_date);
                    cmd.Parameters.AddWithValue("@ls_type", ls_type);
                    cmd.Parameters.AddWithValue("@ls_full_with_opf", ls_full_with_opf);
                    cmd.Parameters.AddWithValue("@ls_short_with_opf", ls_short_with_opf);
                    cmd.Parameters.AddWithValue("@ls_load_status", ls_load_status);
                    cmd.ExecuteNonQuery();
                }
                Program.FlagDadataUpdate = false;
                cn.Close();
            }
            catch 
            {
                Program.FlagDadataUpdate = false;
                cn.Close();
            }

        }

        private async void DadataupdateAsync()
        {

            await Task.Run(() => Dadataupdate());

        }


        private void Dadataload(int ld_pk, string restxt)
        {
            string cnstr = Program.AppConfig["mscns"];
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            string sqlerr = "update cntload set ld_status = @ld_status, stopdate = getdate()  where ld_pk = @ld_pk";
            SqlCommand cmderr = new SqlCommand(sqlerr, cn);
            try
            {

                string sql = "insert into cntload_detail (ls_ld,  ls_inn,  ls_status,  ls_actuality_date, ls_type, ls_full_with_opf,  ls_short_with_opf, ls_load_status) " +
                " values (@ls_ld,  @ls_inn,  @ls_status,  @ls_actuality_date, @ls_type, @ls_full_with_opf,  @ls_short_with_opf, @ls_load_status) ";
                SqlCommand cmd = new SqlCommand(sql, cn);
                dadataINN di = new dadataINN();
                string[] inns = restxt.Split("\n", StringSplitOptions.None);
                for (int i = 1; i < inns.Length; i++)
                {
                    string inn = inns[i].Trim();
                    string ls_load_status = "";
                    PartyData pdata = null;
                    try
                    {
                        pdata = di.exec(inn);
                        if (pdata == null)
                            ls_load_status = "Не найдено";
                    }
                    catch (Exception ee)
                    {
                        ls_load_status = ee.Message;
                    }
                    string ls_status = "";
                    string ls_actuality_date = "";
                    string ls_type = "";
                    string ls_full_with_opf = "";
                    string ls_short_with_opf = "";
                    if (pdata != null)
                    {

                        ls_status = pdata.state.status.ToString();

                        if (pdata.state.actuality_date_fmt != null)
                            ls_actuality_date = pdata.state.actuality_date_fmt;

                        ls_type = pdata.type.ToString();

                        if (pdata.name.full_with_opf != null)
                            ls_full_with_opf = pdata.name.full_with_opf;

                        if (pdata.name.short_with_opf != null)
                            ls_short_with_opf = pdata.name.short_with_opf;
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ls_ld", ld_pk);
                    cmd.Parameters.AddWithValue("@ls_inn", inn + "/");
                    cmd.Parameters.AddWithValue("@ls_status", ls_status);
                    cmd.Parameters.AddWithValue("@ls_actuality_date", ls_actuality_date);
                    cmd.Parameters.AddWithValue("@ls_type", ls_type);
                    cmd.Parameters.AddWithValue("@ls_full_with_opf", ls_full_with_opf);
                    cmd.Parameters.AddWithValue("@ls_short_with_opf", ls_short_with_opf);
                    cmd.Parameters.AddWithValue("@ls_load_status", ls_load_status);
                    cmd.ExecuteNonQuery();
                }
                cmderr.Parameters.AddWithValue("ld_pk", ld_pk);
                cmderr.Parameters.AddWithValue("ld_status", "OK");
                cmderr.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {
                cmderr.Parameters.AddWithValue("ld_pk", ld_pk);
                cmderr.Parameters.AddWithValue("ld_status", ex.Message);
                cmderr.ExecuteNonQuery();
                cn.Close();
            }

        }
        private async void DadataloadAsync(int ld_pk, string restxt)
        {

            await Task.Run(() => Dadataload(ld_pk, restxt));

        }


        [AllowAnonymous]
        public string due()
        {
                DateTime nd = DateTime.Now;
                DateTime start =  nd.Date.AddDays(1).AddHours(9);
                long du = (long)(start.Subtract(nd).TotalMilliseconds);
                return du.ToString();

        }

        [AllowAnonymous]
        public string update()
        {
            string res = "На сервере запущен процесс загрузки с dadata";
            if (!Program.FlagDadataUpdate)
            {
                Program.FlagDadataUpdate = true;
                DadataupdateAsync();
            }
            else
                res = "На сервере уже был запущен процесс загрузки с dadata";

            return res;
        }

        [AllowAnonymous]
        public string stopupdate()
        {
            string res = "Процесс загрузки с dadata остановлен";
            Program.FlagDadataUpdate = false;
            return res;
        }
        public JsonResult upload(IFormFile img)
        {

            string res = "";
            try
            {
                if (img != null)
                {

                    string FileName = img.FileName;
                    int n = (int)img.Length;
                    byte[] buf = new byte[n];
                    Stream ms = img.OpenReadStream();
                    ms.Read(buf, 0, n);
                    string restxt = Encoding.Unicode.GetString(buf);
                    var cnstr = Program.AppConfig["mscns"];
                    SqlDataAdapter da = new SqlDataAdapter("p_cntload_insert", cnstr);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@ld_filename", FileName);
                    da.SelectCommand.Parameters.AddWithValue("@audtuser", User.Identity.Name);
                    DataTable r = new DataTable();
                    da.Fill(r);
                    int ld_pk = (int)r.Rows[0][0];
                    //Запускаем асинхронно
                    DadataloadAsync(ld_pk, restxt);

                }
            }
            catch (Exception e)
            {
                res = e.Message;
            }

            return Json(new { error = res });

        }


    }
}