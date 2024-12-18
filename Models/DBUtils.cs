using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using netbu;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using suggestionscsharp;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace netbu.Models
{

    class dadataINN
    {
        public async Task<string>  sendtele(string channelID, string UserToken, string content)
        {
            string responseText = "";
            try
            {
                string teleurl = Program.AppConfig["teleurl"];
                responseText = await tele_new(teleurl, UserToken, channelID, content);
                //responseText = await Task.Run<string>(()=>tele_curl(teleurl, UserToken, channelID, content));
                //responseText = await Task.Run<string>(()=>tele_old(teleurl, UserToken, channelID, content));
            }
            catch (Exception e)
            {
                responseText = e.Message;
            }
            return responseText;
        }
        public string tele_curl(string teleurl, string UserToken, string channelID, string content)
        {
                
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                string message = JsonConvert.SerializeObject( new { channelID = channelID, content = content });
                string arg = $@"-X POST ""{teleurl}"" -H ""accept: application/json"" -H ""Content-Type: application/json"" -d ""{message}"" -H ""UserToken:{UserToken}""";
                
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = @"C:\curl\bin\curl.exe";
                process.StartInfo.Arguments = arg;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string responseText = process.StandardOutput.ReadToEnd();
                //responseText += process.StandardError.ReadToEnd();
                return responseText;
        }                

        public async Task<string> tele_new(string teleurl, string UserToken, string channelID, string content)
        {
                string responseText = "";
                HttpClientHandler handler = new HttpClientHandler();
                handler.Proxy = null;
                //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls11; //протокол
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { 
                    return true; 
                };
                // Create an HttpClient object
                HttpClient client = new HttpClient(handler);
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(teleurl),
                    Headers = {{ "UserToken", "34534" }, {"User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36"}},
                    Content = new StringContent(JsonConvert.SerializeObject( new { channelID = channelID, content = content }),
                    Encoding.UTF8,
                    "application/json"
                    )
                };
                var response = client.SendAsync(httpRequestMessage).Result;
                responseText = await response.Content.ReadAsStringAsync();
                return responseText;
        }

        public string tele_old(string teleurl, string UserToken, string channelID, string content)
        {
                string responseText = "";
                var httpRequest = (HttpWebRequest)WebRequest.Create(teleurl);
                httpRequest.Proxy = null;
                httpRequest.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => {
                  return  true;
                };
                httpRequest.Method = "POST";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("UserToken", UserToken);
                var serializer = new JsonSerializer();
                using (var w = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    using (JsonWriter writer = new JsonTextWriter(w))
                    {
                        serializer.Serialize(writer, new { channelID = channelID, content = content });
                    }
                }

                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var r = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText = r.ReadToEnd();
                }
                return responseText;
        }


        public string getjson(string inn)
        {
            if (string.IsNullOrEmpty(inn))
                return "";
            inn = inn.Split('/')[0];

            var httpRequest = (HttpWebRequest)WebRequest.Create(Program.AppConfig["dadataurl"]);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            httpRequest.Headers.Add("Authorization", "Token " + Program.AppConfig["dadatakey"]);
            var serializer = new JsonSerializer();
            using (var w = new StreamWriter(httpRequest.GetRequestStream()))
            {
                using (JsonWriter writer = new JsonTextWriter(w))
                {
                    serializer.Serialize(writer, new { query = inn });
                }
            }
            string responseText = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var r = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = r.ReadToEnd();
            }
            return responseText;
        }

        public PartyData exec(string inn)
        {
            string responseText = getjson(inn);
            if (string.IsNullOrEmpty(responseText))
                return null;
            try
            {
                SuggestResponse suggs = JsonConvert.DeserializeObject<SuggestResponse>(responseText);
                if (suggs.suggestions.Count == 0)
                    return null;
                else
                    return suggs.suggestions[0].data;
            }
            catch
            {
                return null;
            }

        }

        public static int FileAccess(string Account, string dogid)
        {
            String sql = "select dbo.fn_cntFileAccess(@Account, @dogid)";
            SqlConnection cn = new SqlConnection(Program.AppConfig["mscns"]);
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();
            cmd.Parameters.AddWithValue("@Account", Account);
            cmd.Parameters.AddWithValue("@dogid", dogid);
            int r = (int)cmd.ExecuteScalar();
            return r;

        }




    }

    public class treeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<object> children { get; set; }

        public string iconCls { get; set; }

        public Dictionary<string, string> attributes { get; set; }

        public string state { get; set; }

        public treeItem(string t)
        {
            text = t;
        }
    }

    class filterRule
    {
        public string field { get; set; }
        public string value { get; set; }

    }
    public class treeutil
    {

        public bool checkAccess(string account, string password)
        {
            var cnstr = Program.isPostgres ? Program.AppConfig["cns"] : Program.AppConfig["mscns"];
            var sqlcheck = "select username from t_ntusers where username = @account and pass = @password";
            var res = new DataTable();
            if (Program.isPostgres)
            {
                var da = new NpgsqlDataAdapter(sqlcheck, cnstr);
                da.SelectCommand.Parameters.AddWithValue("@account", account);
                da.SelectCommand.Parameters.AddWithValue("@password", password);
                da.Fill(res);
            }
            else
            {
                var da = new SqlDataAdapter(sqlcheck, cnstr);
                da.SelectCommand.Parameters.AddWithValue("@account", account);
                da.SelectCommand.Parameters.AddWithValue("@password", password);
                da.Fill(res);
            }
            return (res.Rows.Count > 0);
        }
        public void CreateItems(string Root, treeItem Mn, DataTable Tab)
        {

            var ListCaption = new List<string>();
            var k = Root.Split('/', StringSplitOptions.RemoveEmptyEntries).Length;
            for (var x = 0; x < Tab.Rows.Count; x++)
            {
                var mi = Tab.Rows[x];
                var Caption = mi["caption"].ToString();
                if (Caption.IndexOf(Root) == 0)
                {
                    var bi = Caption.Split('/');
                    var ItemCaption = bi[k];
                    if (ListCaption.IndexOf(ItemCaption) == -1)
                    {
                        ListCaption.Add(ItemCaption);
                        treeItem ilist = new treeItem(ItemCaption);
                        ilist.id = (k == bi.Length - 1) ? mi["idmenu"].ToString() : mi["idmenu"].ToString() + "_node";
                        ilist.attributes = new Dictionary<string, string>() { { "link1", mi["link1"].ToString() }, { "params", mi["params"].ToString() } };
                        if ((int)mi["idimage"] > 0)
                            ilist.iconCls = "tree-" + mi["idimage"].ToString();

                        if (Mn.children == null)
                        { Mn.children = new List<object>(); }
                        Mn.children.Add(ilist);
                        Mn.state = "closed";
                        if (k != bi.Length - 1)
                        {
                            CreateItems(Root + ItemCaption + "/", ilist, Tab);
                        }
                    }
                }
            }
        }
    }


}