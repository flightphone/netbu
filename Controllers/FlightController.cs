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
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace netbu.Controllers
{
    public class FlightController : Controller
    {
        //
        // GET: /Flight/
        [Authorize]
        public ActionResult Index(Guid FC_PK)
        {
            string sql = "select * from dbo.v_FlightNumber where FC_PK = @FC_PK";
            SqlDataAdapter da = new SqlDataAdapter(sql, DBClient.CnStr);
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            DataTable rtab = new DataTable();
            da.Fill(rtab);
            ViewBag.Title = rtab.Rows[0]["FC_NumberDate"].ToString();
            ViewBag.FC_PK = FC_PK;
            ViewBag.O_FC_flState = rtab.Rows[0]["FC_flState"].ToString();
            ViewBag.FC_flDirection = rtab.Rows[0]["FC_flDirection"].ToString();
            ViewBag.FC_Date = ((DateTime)rtab.Rows[0]["FC_Date"]).ToString("dd.MM.yyyy HH:mm");

            DBClient dc = new DBClient();
            ViewBag.FieldTask = dc.CreateFields(1224, User.Identity.Name);
            ViewBag.FieldUser = dc.CreateFields(1225, User.Identity.Name);

            string selstr = "<select id = \"{0}\" class=\"textbox\" style=\"margin-left: 0px; margin-right: 0px; padding-top: 0px; padding-bottom: 0px; height:19.7778px; line-height: 19.7778px; width: 100%;\" onchange = \"FlightRecord.{0}=this.value; FlagEdit();\">";
            string[] FC_flState = new string[] { "План", "Закрыт", "Проверен", "Проведен", "Архив" };
            string[] FC_Cancel = new string[] { "Нет", "Да" };
            string[] FC_needHandle = new string[] { "Да", "Нет" };
            string[] FC_flCategory = new string[] { "ЦР", "Чартер", "Груз", "Перегон", "VIP", "Технич" };
            string[] FC_flType = new string[] { "ВВЛ", "МВЛ", "СНГ" };

            ViewBag.FC_flState = rtab.Rows[0]["FC_flState"].ToString();
            ViewBag.FC_Cancel = rtab.Rows[0]["FC_Cancel"].ToString();
            ViewBag.FC_needHandle = rtab.Rows[0]["FC_needHandle"].ToString();
            ViewBag.FC_flCategory = rtab.Rows[0]["FC_flCategory"].ToString();
            ViewBag.FC_flType = rtab.Rows[0]["FC_flType"].ToString();
            ViewBag.FC_RM = string.Format(selstr, "FC_RM") + dc.CreateComboOption("select RM_PK, RM_Name from Ramp (nolock) order by RM_Name", rtab.Rows[0]["FC_RM"].ToString()) + "</select>";

            sql = "select Statusname v, Statusname n FROM t_sysStatus (NOLOCK) WHERE StatusType  = 'FH_Type' AND dbo.fn_CheckAccessStrict(Statusname, '" + User.Identity.Name + "') > 0 ORDER BY SortOrder";
            ViewBag.FH_Type = dc.CreateComboOption(sql, "");

            return View();
        }
        public string getFlight(Guid FC_PK)
        {
            string res = "";

            SqlDataAdapter da = new SqlDataAdapter("select * from v_FlightCards_w where FC_PK = @FC_PK", DBClient.CnStr);
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            DataTable FTab = new DataTable();
            da.Fill(FTab);

            if (FTab.Rows[0]["FC_flDirection"].ToString() == "Вылет")
                res = System.IO.File.ReadAllText(@"wwwroot\Content\FlightD.json", Encoding.GetEncoding(1251));
            else
                res = System.IO.File.ReadAllText(@"wwwroot\Content\FlightA.json", Encoding.GetEncoding(1251));

            DataRow rw = FTab.Rows[0];
            for (int i = 0; i < FTab.Columns.Count; i++)
            {
                res = res.Replace("[" + FTab.Columns[i].ColumnName + "]", rw[i].ToString().Replace(@"""", @"\"""));
            }

            return res;
        }

        public JsonResult getOrderD(Guid FC_PK)
        {

            string Account = User.Identity.Name;
            //string res = System.IO.File.ReadAllText(@"wwwroot\Content\OrderD.json", Encoding.GetEncoding(1251));


            SqlDataAdapter da = new SqlDataAdapter("LoadOrder", DBClient.CnStr);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            da.SelectCommand.Parameters.AddWithValue("@Account", Account);
            DataTable FTab = new DataTable();
            DataTable HTab = new DataTable();
            da.Fill(0, 0, new DataTable[] { HTab, FTab });

            string[] parentid = new string[] { "00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003" };
            string[] Captions = new string[] {"Список работ", "Необязательные услуги", "Справочная информация", "Дополнительная информация"};
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            for (int i = 0; i < parentid.Length; i++)
            {
                var user = new Dictionary<string, object>();
                user.Add("QD_PK", parentid[i]);
                user.Add("NN", "");
                user.Add("Caption", Captions[i]);
                user.Add("QD_Comment", "");
                user.Add("QD_isPosted", "");
                user.Add("iconCls", "icon-blank");
                if (i > 0)
                {
                    user.Add("state", "closed");
                }    
                rows.Add(user);    
            }

   

            for (int i = 0; i < FTab.Rows.Count; i++)
                if ((int)FTab.Rows[i]["SV_CATEGORY"] < 4)
                {
                    var user = new Dictionary<string, object>();
                    user.Add("QD_PK", FTab.Rows[i]["QD_PK"].ToString());
                    user.Add("NN", FTab.Rows[i]["NN"].ToString());
                    user.Add("Caption", FTab.Rows[i]["Caption"].ToString());
                    user.Add("QD_QTY", FTab.Rows[i]["QD_QTY"].ToString());
                    user.Add("QD_Comment", FTab.Rows[i]["QD_Comment"].ToString());
                    user.Add("QD_isPosted", FTab.Rows[i]["QD_isPosted"].ToString());
                    user.Add("iconCls", "icon-blank");
                    user.Add("_parentId", parentid[(int)FTab.Rows[i]["SV_CATEGORY"]]);
                    user.Add("ClassName", FTab.Rows[i]["ClassName"].ToString());
                    user.Add("QD_SV", FTab.Rows[i]["QD_SV"].ToString());
                    user.Add("QD_LineNo", FTab.Rows[i]["QD_LineNo"].ToString());
                    user.Add("SV_IsRequired", FTab.Rows[i]["SV_IsRequired"].ToString());
                    rows.Add(user);

                }

             return Json(new { rows = rows });
        }

        public string getTasks(Guid FC_PK)
        {
            string sql = "select T.* from RMS_Tasks T(nolock) inner join v_VKO_ADD_FLD X on X.FLT_ID = T.RM_Flt_ID AND X.FC_PK = '" + FC_PK + "' order by T.RM_ProcessName";
            DBClient db = new DBClient();
            return db.getJSONData(sql, db.getFormats(1224));

        }

        public string getUsers(Guid FC_PK)
        {
            string sql = "select U.* from RMS_Users U(nolock) inner join v_VKO_ADD_FLD X on X.FLT_ID = U.RU_Flt_ID AND X.FC_PK = '" + FC_PK + "' order by U.RU_AgentName";
            DBClient db = new DBClient();
            return db.getJSONData(sql, null);

        }

        public string getFiles(Guid FC_PK)
        {
            string Account = User.Identity.Name;
            string sql = "exec LoadFile '" + FC_PK + "', '" + Account + "'";
            DBClient db = new DBClient();
            return db.getJSONData(sql, null);
        }

        public ActionResult getFile(Guid LD_PK, string QD_SQTY)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select LD_File from FileLoadSheetD (nolock) where LD_PK = @LD_PK", DBClient.CnStr);
                da.SelectCommand.Parameters.AddWithValue("@LD_PK", LD_PK);
                DataTable td = new DataTable();
                da.Fill(td);
                byte[] buf = (byte[])td.Rows[0][0];

                return File(buf, "application/octet-stream", QD_SQTY);
            }
            catch (Exception e)
            {
                byte[] buf = System.Text.Encoding.Unicode.GetBytes(e.Message);
                return File(buf, "application/octet-stream", "ОшибкаЗагрузкиФайла.txt");
            }

        }

        [HttpPost]
        public string Save(string Obj)
        {
            return "OK";
        }

        [HttpPost]
        public JsonResult prePrintMap(Guid FC_PK, string RH_Category)
        {

            DataTable _MapH = new DataTable();
            string sql = "select * from v_MapH where FC_PK = @FC_PK and RH_Category = @RH_Category";
            SqlDataAdapter _da = new SqlDataAdapter(sql, DBClient.CnStr);
            _da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            _da.SelectCommand.Parameters.AddWithValue("@RH_Category", RH_Category);
            _da.Fill(_MapH);
            string MH_RH = "0";
            if (_MapH.Rows.Count > 0)
                MH_RH = _MapH.Rows[0]["MH_RH"].ToString();
            for (int i = 1; i < _MapH.Rows.Count; i++)
            {
                MH_RH = MH_RH + "," + _MapH.Rows[i]["MH_RH"].ToString();
            }

            //return "{ \"n\" : \"" + _MapH.Rows.Count.ToString() + "\", \"RH_Category\" : \"" + RH_Category + "\", \"RH_PK\":\"" + MH_RH + "\"}";
            return Json(new { n = _MapH.Rows.Count.ToString(), RH_Category = RH_Category, RH_PK = MH_RH });
        }
        [HttpPost]
        public string Reload(Guid FC_PK)
        {
            SqlConnection cn = new SqlConnection(DBClient.CnStr);
            SqlCommand cmd = new SqlCommand("delete from OrderD where QD_QH = @FC_PK", cn);
            cmd.Parameters.AddWithValue("@FC_PK", FC_PK);
            string res = "OK";
            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return res;


        }

        public ActionResult PrintMap(Guid FC_PK, string RH_Category, int MH_RH)
        {
            try
            {
                DataTable MapH = new DataTable();
                DataTable MapD = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("p_prnMap", DBClient.CnStr);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
                da.SelectCommand.Parameters.AddWithValue("@RH_Category", RH_Category);
                da.SelectCommand.Parameters.AddWithValue("@MH_RH", MH_RH);
                DataTable[] par = { MapH, MapD };
                da.Fill(0, 0, par);

                /*
                if (MapH.Rows.Count == 0)
                {
                    MessageBox.Show("Не найден регламент!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                */
                String RH_Template = MapH.Rows[0]["MH_Template"].ToString();
                /*
                if (String.IsNullOrEmpty(RH_Template))
                {
                    MessageBox.Show("Не указан файл шаблона!", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                */
                /*
                String code = MapH.Rows[0]["AD_NN"].ToString();
                //Штрих код 17052012
                
                Barcode128 bc = new Barcode128();
                bc.BarHeight = 100f;
                bc.Code = code;
                System.Drawing.Image img = bc.CreateDrawingImage(Color.Black, Color.White);


                Bitmap bimg = new Bitmap(img);
                //Расширяем картинку
                int bmpw = 790;
                int imgw = img.Width;
                Bitmap bmp = new Bitmap(bmpw, 100);
                for (int i = 0; i < bmpw; i++)
                {
                    int ii = i * imgw / bmpw;
                    Color c = bimg.GetPixel(ii, 50);
                    for (int j = 0; j < 100; j++)
                        bmp.SetPixel(i, j, c);
                }
                
                SortedList images = new SortedList();
                images.Add("image2", bmp);
                */




                List<DataTable> atab = new List<DataTable>();
                Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();
                atab.Add(MapD);
                Guid gd = Guid.NewGuid();
                String FileName = "MapRep" + gd.ToString() + ".docx";

                PrintDoc pd = new PrintDoc();
                byte[] buf = pd.PrintDocx(RH_Template, MapH.Rows[0], atab, images);
                return File(buf, "application/octet-stream", RH_Template);

            }
            catch (Exception e)
            {
                byte[] buf = System.Text.Encoding.Unicode.GetBytes(e.Message);
                return File(buf, "application/octet-stream", "ОшибкаПечати.txt");
            }

        }

        [HttpPost]
        public string fileUpdate(Guid FC_PK, string LD_Comment, string LD_type, List<IFormFile> files)
        {
            string res = "OK";

            if (files != null)
                foreach (IFormFile img in files)
                {

                    string FileName = img.FileName;
                    int n = (int)img.Length;
                    byte[] buf = new byte[n];
                    Stream ms = img.OpenReadStream();
                    ms.Read(buf, 0, n);

                    SqlConnection cn = new SqlConnection(DBClient.CnStr);
                    SqlCommand cmd = new SqlCommand("p_FileLoadSheetD_ADD", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LD_DisplayName", Path.GetFileName(FileName));
                    cmd.Parameters.AddWithValue("@LD_Comment", LD_Comment);
                    cmd.Parameters.AddWithValue("@LD_FC", FC_PK);
                    cmd.Parameters.AddWithValue("@LD_File", buf);
                    cmd.Parameters.AddWithValue("@Account", User.Identity.Name);
                    cmd.Parameters.AddWithValue("@LD_Date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LD_type", LD_type);
                    try
                    {
                        cn.Open();
                        Guid LD_PK = (Guid)cmd.ExecuteScalar();
                        cn.Close();

                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                    }

                }


            return res;

        }

        [HttpPost]
        public string deleteFile(Guid LD_PK)
        {
            string res = "OK";
            SqlConnection cn = new SqlConnection(DBClient.CnStr);
            SqlCommand cmd = new SqlCommand("p_v_FlightsFile_DEL", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LD_PK", LD_PK);
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            { res = ex.Message; }
            return res;
        }


        public string execFindFlight(string FC_flNumber, string FC_Date, string FC_AT)
        {
            int Flag = 0;
            if (!string.IsNullOrEmpty(FC_Date))
                Flag = Flag + 1;
            if (!string.IsNullOrEmpty(FC_AT))
                Flag = Flag + 2;

            DateTime FC_DateStart = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(FC_Date))
                FC_DateStart = DateTime.Parse(FC_Date);
            DateTime FC_DateFinish = FC_DateStart.AddDays(1).AddMinutes(-1);

            DateTime FC_ATStart = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(FC_AT))
                FC_ATStart = DateTime.Parse(FC_AT);
            DateTime FC_ATFinish = FC_ATStart.AddDays(1).AddMinutes(-1);

            string Account = User.Identity.Name;

            SqlDataAdapter da = new SqlDataAdapter("p_FindFlight", DBClient.CnStr);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@FC_flNumber", FC_flNumber);
            da.SelectCommand.Parameters.AddWithValue("@FC_DateStart", FC_DateStart);
            da.SelectCommand.Parameters.AddWithValue("@FC_DateFinish", FC_DateFinish);
            da.SelectCommand.Parameters.AddWithValue("@FC_ATStart", FC_ATStart);
            da.SelectCommand.Parameters.AddWithValue("@FC_ATFinish", FC_ATFinish);
            da.SelectCommand.Parameters.AddWithValue("@Flag", Flag);
            da.SelectCommand.Parameters.AddWithValue("@Account", Account);
            DataTable res = new DataTable();
            da.Fill(res);

            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> formats = new Dictionary<string, string>();
            formats.Add("FC_STD", "t");
            formats.Add("FC_ETD", "t");
            formats.Add("FC_ATD", "t");
            formats.Add("FC_STA", "t");
            formats.Add("FC_ETA", "t");
            formats.Add("FC_ATA", "t");
            formats.Add("FC_Date", "dd.MM.yyyy HH:mm");

            DBClient dc = new DBClient();

            for (int i = 0; i < Math.Min(res.Rows.Count, 100); i++)
            {
                string s = dc.getJSONRow(res.Rows[i], formats);

                if (i == Math.Min(res.Rows.Count, 100) - 1)
                    sb.AppendLine(s);
                else
                    sb.AppendLine(s + ",");
            }

            string result = "[" + sb.ToString() + "]";
            return result;

        }

    }
}
