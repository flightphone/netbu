using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using netbu.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading;

namespace netbu.Controllers
{

    public class TeleController : Controller
    {
        public async Task<string> test(string channelID, string UserToken, string content)
        {
            //Tele/test?channelID=-1001299813271&UserToken=123&content=aaa
            dadataINN di = new dadataINN();
            string res = await di.sendtele(channelID, UserToken, content);
            return res;
        }

        public string send(string fc_pk)
        {
            string res = "OK";
            try
            {
                Guid pk = Guid.Parse(fc_pk);
                sendtele(pk);
                //sendteleAsync(pk);
            }
            catch (Exception e)
            {
                res = e.Message;
            }
            return res;
        }
        /*
        private async void sendteleAsync(Guid fc_pk)
        {

            await Task.Run(() => sendall(fc_pk));

        }
        private void sendall(Guid fc_pk)
        {
            sendtele(fc_pk);
        }
        */
        private void sendsms(Guid fc_pk)
        {

            //Отправляем СМС по новому АПИ 26.08.2022
            string sms_address = Program.AppConfig["sms_address"];
            string sms_login = Program.AppConfig["sms_login"];
            string sms_password = Program.AppConfig["sms_password"];
            string sms_url = "https://omnichannel.mts.ru/http-api/v1/messages";

            string sql = "select MG_PK, MG_Connect, MG_text from uKnow..[AM_messages] (nolock) where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
            string cnstr = Program.AppConfig["mscns"];
            string sqlerr = "";
            SqlCommand cmd = null;
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();

            SqlDataAdapter da = new SqlDataAdapter(sql, cnstr);
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", fc_pk);
            da.SelectCommand.CommandTimeout = 0;
            DataTable res = new DataTable();
            da.Fill(res);
            if (res.Rows.Count == 0)
                return;

            List<object> messages = new List<object>();
            List<string> msg_ids = new List<string>();
            for (int i = 0; i < res.Rows.Count; i++)
            {
                messages.Add(new
                {
                    content = new { short_text = res.Rows[i]["MG_text"].ToString() },
                    to = new object[] { new { msisdn = res.Rows[i]["MG_Connect"].ToString(), message_id = res.Rows[i]["MG_PK"].ToString().ToLowerInvariant()} }

                });
                msg_ids.Add(res.Rows[i]["MG_PK"].ToString().ToLowerInvariant());
            }

            Dictionary<string, object> send0 = new Dictionary<string, object>(){
                    {"messages", messages},
                    {"options", new {from = new {sms_address = sms_address}}}
                };


            var httpRequest = (HttpWebRequest)WebRequest.Create(sms_url);
            httpRequest.Credentials = new NetworkCredential(sms_login, sms_password);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            var serializer = new JsonSerializer();
            using (var w = new StreamWriter(httpRequest.GetRequestStream()))
            {
                using (JsonWriter writer = new JsonTextWriter(w))
                {
                    serializer.Serialize(writer, send0);
                }
            }
            string responseText = "";
            bool err = false;
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var r = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText = r.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                responseText = ex.Message;
                err = true;
            }
            finally
            {
                sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 0 where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
                cmd = new SqlCommand(sqlerr, cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@FC_PK", fc_pk);
                cmd.Parameters.AddWithValue("@MG_error", responseText.Substring(0, Math.Min(200, responseText.Length)));
                cmd.ExecuteNonQuery();
                //cn.Close();
            }
            if (err)
            {
                cn.Close();
                return;
            }
            //Ждем 3 секундны
            Thread.Sleep(3000);
            //Статусы
            httpRequest = (HttpWebRequest)WebRequest.Create($"{sms_url}/info");
            httpRequest.Credentials = new NetworkCredential(sms_login, sms_password);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            serializer = new JsonSerializer();
            using (var w = new StreamWriter(httpRequest.GetRequestStream()))
            {
                using (JsonWriter writer = new JsonTextWriter(w))
                {
                    serializer.Serialize(writer, new { msg_ids = msg_ids });
                }
            }
            responseText = "";
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var r = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText = r.ReadToEnd();
                }

                //responseText = "{\"events_info\": [{\"error\": true,\"error_description\": \"key not exists\",\"events_info\": [],\"key\": \"06C763bd-a94b-4b70-bc36-8742360ad306\"}]}";
                sms_info m_i = JsonConvert.DeserializeObject<sms_info>(responseText);
                var res2 = m_i.events_info.Select(ei0 => (new { message_id = ei0.events_info.Last<sms_event>().message_id, errors = ei0.events_info.Last<sms_event>().internal_errors }));
                sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 1 where MG_PK = @MG_PK";
                cmd = new SqlCommand(sqlerr, cn);
                foreach (var r in res2)
                {
                    string MG_error = "OK";
                    if (r.errors != null)
                    {
                        MG_error = r.errors[0].ToString();
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@MG_PK", r.message_id);
                    cmd.Parameters.AddWithValue("@MG_error", MG_error);
                    cmd.ExecuteNonQuery();
                }
                cn.Close();
            }
            catch 
            {
                sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 3 where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
                cmd = new SqlCommand(sqlerr, cn);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@FC_PK", fc_pk);
                cmd.Parameters.AddWithValue("@MG_error", responseText.Substring(0, Math.Min(400, responseText.Length)));
                cmd.ExecuteNonQuery();
                cn.Close();
                return;
            }


        }

        private async void sendtele(Guid fc_pk)
        {

            string cnstr = Program.AppConfig["mscns"];
            string sql = "uKnow..p_createTelegram";
            string sqlerr = "insert into uKnow..AM_telegramLog(fc_pk, servmessage) values (@fc_pk, @servmessage)";
            SqlDataAdapter da = new SqlDataAdapter(sql, cnstr);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", fc_pk);
            da.SelectCommand.CommandTimeout = 0;
            DataTable res = new DataTable();
            da.Fill(res);


            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            SqlCommand cmd = new SqlCommand(sqlerr, cn);

            dadataINN di = new dadataINN();
            foreach (DataRow rw in res.Rows)
            {
                string channelID = rw["channelID"].ToString();
                string UserToken = rw["UserToken"].ToString();
                string content = rw["MG_text"].ToString();
                Guid PK = (Guid)rw["FC_PK"];
                string err = await di.sendtele(channelID, UserToken, content);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@fc_pk", PK);
                cmd.Parameters.AddWithValue("@servmessage", err);
                cmd.ExecuteNonQuery();
            }
            cn.Close();

        }
    }
}