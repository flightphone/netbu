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
using Microsoft.Extensions.Primitives;
using System.Drawing;
using System.Drawing.Imaging;



namespace netbu.Controllers
{
    public class PrintController : Controller
    {
        public ActionResult tgo(string id)
        {

            string FC_PK = id;
            //"9f076395-ebb8-4ade-b65a-3c6571fb0805"
            DataTable _MapH = new DataTable();
            string sql = "select * from v_MapH_strong where FC_PK = @FC_PK and RH_Category = @RH_Category";
            var cnstr = Program.AppConfig["mscns"];
            string RH_Category = "Обслуживание ВС";
            SqlDataAdapter _da = new SqlDataAdapter(sql, cnstr);
            _da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            _da.SelectCommand.Parameters.AddWithValue("@RH_Category", RH_Category);
            _da.Fill(_MapH);
            if (_MapH.Rows.Count == 0)
            {
                return File(Encoding.UTF8.GetBytes("Не найден регламент."), "application/octet-stream", "Регламент не найден.txt");
            }

            DataTable MapH = new DataTable();
            DataTable MapD = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("p_prnMap", cnstr);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@FC_PK", FC_PK);
            da.SelectCommand.Parameters.AddWithValue("@RH_Category", RH_Category);
            da.SelectCommand.Parameters.AddWithValue("@MH_RH", _MapH.Rows[0]["MH_RH"]);
            DataTable[] par = { MapH, MapD };
            da.Fill(0, 0, par);


            if (MapH.Rows.Count == 0)
            {
                return File(Encoding.UTF8.GetBytes("Не найден регламент."), "application/octet-stream", "Регламент не найден.txt");
            }

            string RH_Template = MapH.Rows[0]["MH_Template"].ToString().ToLower().Replace(".docx", "");


            string codeurl = "https://www.barcodesinc.com/generator/image.php?type=C128B&width=120&height=100&xres=1&font=3&code=" + MapH.Rows[0]["AD_NN"].ToString();
            var codeRequest = (HttpWebRequest)WebRequest.Create(codeurl);
            codeRequest.Method = "GET";
            HttpWebResponse codeResponse = (HttpWebResponse)codeRequest.GetResponse();
            long nc = codeResponse.ContentLength;
            Stream st = codeResponse.GetResponseStream();
            //byte[] buf = new byte[(int)nc];
            //st.Read(buf, 0, (int)nc);
            System.Drawing.Image img = Image.FromStream(st);
            Bitmap bimg = new Bitmap(img);
            for (int i = 0; i < 100; i++)
            {
                bimg.SetPixel(0, i, Color.White);
                bimg.SetPixel(img.Width - 1, i, Color.White);

            }
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
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] buf = ms.ToArray();

            Dictionary<string, byte[]> Images = new Dictionary<string, byte[]>() { { "image2.png", buf } };


            /*
            //Штрих код 17052012
            */


            string FileName = MapH.Rows[0]["MH_ArrivalFlNumber"].ToString() + "_" + MapH.Rows[0]["MH_DepartFlNumber"].ToString();
            List<DataTable> atab = new List<DataTable>() { MapD };
            PrintDoc pd = new PrintDoc();
            byte[] res = pd.PrintDocx(RH_Template, MapH.Rows[0], atab, Images);
            
            

            //return File(res, "application/octet-stream", FileName + ".docx");
            
            Dictionary<string, object> param = new Dictionary<string, object>();
            Dictionary<string, object> FileValue = new Dictionary<string, object>();
            FileValue.Add("Name", FileName + ".docx");
            FileValue.Add("Data", Convert.ToBase64String(res, Base64FormattingOptions.None));
            param.Add("Name", "File");
            param.Add("FileValue", FileValue);

            string convurl = "https://v2.convertapi.com/convert/docx/to/pdf?Secret=asey91obtGbzCCSV";
            var httpRequest = (HttpWebRequest)WebRequest.Create(convurl);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            var serializer = new JsonSerializer();
            using (var w = new StreamWriter(httpRequest.GetRequestStream()))
            {
                using (JsonWriter writer = new JsonTextWriter(w))
                {
                    serializer.Serialize(writer, new { Parameters = new object[]{param} });
                }
            }
            string responseText = "";
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var r = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = r.ReadToEnd();
            }
            ConvertApiResponse resp = JsonConvert.DeserializeObject<ConvertApiResponse>(responseText);
            return File(Convert.FromBase64String(resp.Files[0].FileData), "application/pdf", resp.Files[0].FileName);
            
            
        }
    }
}