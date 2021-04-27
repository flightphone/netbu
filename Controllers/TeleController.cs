using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using netbu.Models;
using System.Threading.Tasks;
using System.Data.SqlClient;

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
            }
            catch
            {; }

        }
    }
}