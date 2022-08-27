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

namespace netbu.Controllers
{

    public class TeleController : Controller
    {
        public string test(string channelID, string UserToken, string content)
        {
            dadataINN di = new dadataINN();
            return di.sendtele(channelID, UserToken, content);
        }

        public string send(string fc_pk)
        {
            string res = "OK";
            try
            {
                Guid pk = Guid.Parse(fc_pk);
                sendteleAsync(pk);
            }
            catch (Exception e)
            {
                res = e.Message;
            }
            return res;
        }
        private async void sendteleAsync(Guid fc_pk)
        {

            await Task.Run(() => sendtele(fc_pk));

        }

        private void sendtele(Guid fc_pk)
        {
            try
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
                    string err = di.sendtele(channelID, UserToken, content);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@fc_pk", PK);
                    cmd.Parameters.AddWithValue("@servmessage", err);
                    cmd.ExecuteNonQuery();
                }
                cn.Close();


                //Отправляем СМС по новому АПИ 26.08.2022
                string sms_address = Program.AppConfig["sms_address"];
                string sms_login = Program.AppConfig["sms_login"];
                string sms_password = Program.AppConfig["sms_password"];
                string sms_url = "https://omnichannel.mts.ru/http-api/v1/messages";

                sql = "select MG_PK, MG_Connect, MG_text from uKnow..[AM_messages] where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
                da = new SqlDataAdapter(sql, cnstr);
                da.SelectCommand.Parameters.AddWithValue("@FC_PK", fc_pk);
                da.SelectCommand.CommandTimeout = 0;
                res = new DataTable();
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
                        to = new object[] { new { msisdn = res.Rows[i]["MG_Connect"].ToString(), message_id = res.Rows[i]["MG_PK"].ToString() } }

                    });
                    msg_ids.Add(res.Rows[i]["MG_PK"].ToString());
                }

                Dictionary<string, object> send = new Dictionary<string, object>(){
                    {"messages", messages},
                    {"options", new {from = new {sms_address = sms_address}}}
                };
                //SMSmodel sms = new SMSmodel();    

                var httpRequest = (HttpWebRequest)WebRequest.Create(sms_url);
                httpRequest.Credentials = new NetworkCredential(sms_login, sms_password);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                var serializer = new JsonSerializer();
                using (var w = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    using (JsonWriter writer = new JsonTextWriter(w))
                    {
                        serializer.Serialize(writer, send);
                    }
                }
                string responseText = "";
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
                    sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 3 where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
                    cn = new SqlConnection(cnstr);
                    cmd = new SqlCommand(sqlerr, cn);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@FC_PK", fc_pk);
                    cmd.Parameters.AddWithValue("@MG_error", ex.Message);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }

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
                }
                catch (Exception ex)
                {
                    sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 3 where [MG_FC] = @FC_PK and isnull(MG_connect, '') <> '' and MG_Type = 'SMS'";
                    cn = new SqlConnection(cnstr);
                    cmd = new SqlCommand(sqlerr, cn);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@FC_PK", fc_pk);
                    cmd.Parameters.AddWithValue("@MG_error", ex.Message);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }

                sms_info m_i = JsonConvert.DeserializeObject<sms_info>(responseText);
                if (m_i.events_info == null)
                    return;
                var res2 = m_i.events_info.Select(ei0 => (new { message_id = ei0.events_info.Last<sms_event>().message_id, errors = ei0.events_info.Last<sms_event>().internal_errors }));
                sqlerr = "update uKnow..AM_messages set MG_error = @MG_error, MG_status = 1 where MG_PK = @MG_PK";
                cn = new SqlConnection(cnstr);
                cn.Open();
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

            }

        }
    }
}