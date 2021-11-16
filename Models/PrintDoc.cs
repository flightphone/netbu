using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Data;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Newtonsoft.Json;
using WpfBu.Models;
using System.Data.SqlClient;

namespace netbu.Models
{

    public class ConvertApiResponse
    {
        public int ConversionCost { get; set; }
        [JsonProperty(PropertyName = "Files")]
        public ProcessedFile[] Files { get; set; }
    }

    public class ProcessedFile
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileData { get; set; }
    }

    public class PrintDoc
    {

        public string ExportCSV(DataTable Tab)
        {
            StringBuilder Res = new StringBuilder();
            string s = "";
            s = Tab.Columns[0].ColumnName;
            for (int i = 1; i < Tab.Columns.Count; i++)
                s = s + ";" + Tab.Columns[i].ColumnName;
            Res.AppendLine(s);
            foreach (DataRow rw in Tab.Rows)
            {
                s = @"""" + rw[0].ToString().TrimEnd().Replace(@"""", @"""""") + @"""";
                for (int i = 1; i < Tab.Columns.Count; i++)
                {
                    s = s + ";" + @"""" + rw[i].ToString().TrimEnd().Replace(@"""", @"""""") + @"""";
                }
                Res.AppendLine(s);
            }

            return Res.ToString().Trim();
        }
        public byte[] PrintPdf(String RepName, DataRow printRow, List<DataTable> Tables)
        {
           
            string sql = $"select FileDat from ReportFile (nolock) where FileName = '{RepName}'";
            SqlDataAdapter da = new SqlDataAdapter(sql, MainObj.ConnectionString);
            DataTable dat = new DataTable();
            da.Fill(dat);
            if (dat.Rows.Count == 0)
                return null;
            byte[] buf = (byte[])dat.Rows[0][0];
            string OutPath = $@"wwwroot\Reports\{Guid.NewGuid().ToString()}";
            string zpFile = $@"{OutPath}\proj.zip";


            Directory.CreateDirectory(OutPath);
            File.WriteAllBytes(zpFile, buf);
            FileInfo fi = new FileInfo(zpFile);
            OutPath = fi.DirectoryName;
            string texFile = $@"{OutPath}\{RepName.Replace(".zip", ".tex")}";
            string pdfFile = $@"{OutPath}\{RepName.Replace(".zip", ".pdf")}";
            ZipFile.ExtractToDirectory(zpFile, OutPath);

            string texstr = File.ReadAllText(texFile);
            texstr = ReplaceFieldTex(texstr, printRow);

            if (Tables != null)
                for (int i = 0; i < Tables.Count; i++)
                {
                    DataTable printTab = Tables[i];
                    texstr = SetTableTex(texstr, printTab, i);
                }

            File.WriteAllText(texFile, texstr);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.WorkingDirectory = OutPath;
            pi.FileName = "xelatex";
            pi.Arguments = $"-interaction nonstopmode {texFile}";
            Process.Start(pi).WaitForExit();
            byte[] res = File.ReadAllBytes(pdfFile);
            Directory.Delete(OutPath, true);
            return res;

        }
        public byte[] PrintDocx(String RepName, DataRow printRow, List<DataTable> Tables, Dictionary<string, byte[]> Images)
        {

            string FileName = @"wwwroot\Reports\rep" + Guid.NewGuid().ToString() + ".zip";
            string OutPath = @"wwwroot\Reports\" + RepName;
            string document = OutPath + @"\word\document.xml";
            string ResWord;
            //string ResWord = File.ReadAllText(document, Encoding.UTF8);




            //ZipFile.CreateFromDirectory(OutPath, FileName);
            //File.Copy(@"D:\uProjects\CheckList\RegulationPrint\bin\Reports\Map.docx", FileName);
            //Подгружаем файл из базы
            string sql = $"select FileDat from ReportFile (nolock) where FileName = '{RepName}.docx'";
            SqlDataAdapter da = new SqlDataAdapter(sql, MainObj.ConnectionString);
            DataTable dat = new DataTable();
            da.Fill(dat);
            if (dat.Rows.Count == 0)
                return null;
            byte[] buf = (byte[])dat.Rows[0][0];
            File.WriteAllBytes(FileName, buf);

            using (FileStream zipToOpen = new FileStream(FileName, FileMode.Open))
            //using (MemoryStream zipToOpen = new MemoryStream(buf))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry doc = archive.GetEntry(@"word/document.xml");
                    using (StreamReader sr = new StreamReader(doc.Open()))
                    {
                        ResWord = sr.ReadToEnd();
                        //Меняем поля
                        ResWord = ReplaceFieldXML(ResWord, printRow);

                        DataTable printTab;
                        if (Tables != null)
                            for (int i = 0; i < Tables.Count; i++)
                            {
                                printTab = Tables[i];
                                ResWord = SetTableDocx(ResWord, printTab);
                            }

                    }

                    doc = archive.GetEntry(@"word/document.xml");
                    using (StreamWriter writer = new StreamWriter(doc.Open()))
                    {
                        writer.Write(ResWord);
                    }

                    //Картинки
                    foreach (string key in Images.Keys)
                    {
                        ZipArchiveEntry img = archive.GetEntry(@"word/media/" + key);
                        if (img != null)
                        {
                            using (Stream s = img.Open())
                            {
                                s.Write(Images[key], 0, Images[key].Length);
                                s.Flush();
                            }
                        }
                    }
                }
            }
            byte[] res = File.ReadAllBytes(FileName);
            File.Delete(FileName);
            return res;

            /* 
            StreamWriter swr = new StreamWriter(document, false, Encoding.UTF8);
            swr.Write(ResWord);
            swr.Close();

            //Картинки
            if (Images!=null)
                for (int i = 0; i < Images.Count; i++)
                {
                    String key = (String)Images.GetKey(i);
                    Image img = (Image)Images[key];
                    String imgFile = OutPath + @"\word\media\" + key + ".png";
                    img.Save(imgFile, System.Drawing.Imaging.ImageFormat.Png);
                }


            //Запаковываем
            pkZip(OutPath, FName);
            return FName;
            */
        }
        public string esctex(string s)
        {
            return s
            .Replace("\\", "")
            .Replace("^", "")
            .Replace("~", "-")
            .Replace("_", "\\_")
            .Replace("%", "\\%")
            .Replace("&", "\\&")
            .Replace("$", "\\$")
            .Replace("{", "\\{")
            .Replace("}", "\\}")
            .Replace("#", "\\#");
        }
        public string ReplaceFieldTex(string ResFile, DataRow printRow)
        {
            DataTable PrintTab = printRow.Table;

            for (int i = 0; i < PrintTab.Columns.Count; i++)
            {

                string cname = PrintTab.Columns[i].ColumnName.Replace("_", "\\_");    
                if (PrintTab.Columns[i].DataType == Type.GetType("System.DateTime"))
                {
                    if (printRow[i] != DBNull.Value)
                    {
                        ResFile = ResFile.Replace("I" + cname + "I", ((DateTime)printRow[i]).TimeOfDay.ToString().Substring(0, 5));
                        ResFile = ResFile.Replace("A" + cname + "A", ((DateTime)printRow[i]).ToShortDateString());
                        ResFile = ResFile.Replace("[" + cname + "]", ((DateTime)printRow[i]).ToString());
                    }
                    else
                    {
                        ResFile = ResFile.Replace("I" + cname + "I", "");
                        ResFile = ResFile.Replace("A" + cname + "A", "");
                        ResFile = ResFile.Replace("[" + cname + "]", "");
                    }
                }
                else
                {

                    ResFile = ResFile.Replace("I" + cname + "I", esctex(printRow[i].ToString()));
                    ResFile = ResFile.Replace("A" + cname + "A", esctex(printRow[i].ToString()));
                    ResFile = ResFile.Replace("[" + cname + "]", esctex(printRow[i].ToString()));

                }
            }

            return ResFile;
        }

        public string ReplaceFieldXML(string ResFile, DataRow printRow)
        {
            DataTable PrintTab = printRow.Table;

            for (int i = 0; i < PrintTab.Columns.Count; i++)
            {


                if (PrintTab.Columns[i].DataType == Type.GetType("System.DateTime"))
                {
                    if (printRow[i] != DBNull.Value)
                    {
                        ResFile = ResFile.Replace("I" + PrintTab.Columns[i].ColumnName + "I", ((DateTime)printRow[i]).TimeOfDay.ToString().Substring(0, 5));
                        ResFile = ResFile.Replace("A" + PrintTab.Columns[i].ColumnName + "A", ((DateTime)printRow[i]).ToShortDateString());
                        ResFile = ResFile.Replace("<w:t>" + PrintTab.Columns[i].ColumnName + "</w:t>", "<w:t>" + ((DateTime)printRow[i]).ToString() + "</w:t>");
                    }
                    else
                    {
                        ResFile = ResFile.Replace("I" + PrintTab.Columns[i].ColumnName + "I", "");
                        ResFile = ResFile.Replace("A" + PrintTab.Columns[i].ColumnName + "A", "");
                        ResFile = ResFile.Replace("<w:t>" + PrintTab.Columns[i].ColumnName + "</w:t>", "<w:t></w:t>");
                    }
                }
                else
                {

                    ResFile = ResFile.Replace("I" + PrintTab.Columns[i].ColumnName + "I", xmlString(printRow[i].ToString()));
                    ResFile = ResFile.Replace("A" + PrintTab.Columns[i].ColumnName + "A", xmlString(printRow[i].ToString()));
                    ResFile = ResFile.Replace("<w:t>" + PrintTab.Columns[i].ColumnName + "</w:t>", "<w:t>" + xmlString(printRow[i].ToString()) + "</w:t>");

                }
            }

            return ResFile;
        }

        private string xmlString(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public string SetTableTex(String ResFile, DataTable printTable, int indx)
        {
            string StartMarker = @"\startrow{" + indx.ToString() + "}";
            string StopMarker = @"\stoprow{" + indx.ToString() + "}";

            int StartInd = ResFile.IndexOf(StartMarker);
            if (StartInd == -1)
                return ResFile;

            StartInd = StartInd + StartMarker.Length;
            int StepInd = ResFile.IndexOf(StopMarker);
            if (StepInd == -1)
                return ResFile;





            String StrartRes = ResFile.Substring(0, StartInd);
            String FootRes = ResFile.Substring(StepInd);
            String MidRes = ResFile.Substring(StartInd, StepInd - StartInd);



            for (int i = 0; i < printTable.Rows.Count; i++)
            {
                StrartRes = StrartRes + ReplaceFieldTex(MidRes, printTable.Rows[i]);
            }

            StrartRes = StrartRes + FootRes;

            return StrartRes;


        }


        public string SetTableDocx(String ResFile, DataTable printTable)
        {

            int ind = GetFieldPosition(ResFile, printTable);
            if (ind == -1)
                return ResFile;

            int StartInd = 1, StepInd = 1;
            while (StepInd < ind)
            {
                StartInd = StepInd;
                StepInd = ResFile.IndexOf(@"<w:tr ", StartInd + 1);
            }


            StepInd = ResFile.IndexOf(@"</w:tr>", StartInd);

            String StrartRes = ResFile.Substring(0, StartInd);
            String FootRes = ResFile.Substring(StepInd + 7);
            String MidRes = ResFile.Substring(StartInd, StepInd - StartInd + 7);

            //MessageBox.Show(MidRes);

            for (int i = 0; i < printTable.Rows.Count; i++)
            {
                StrartRes = StrartRes + ReplaceFieldXML(MidRes, printTable.Rows[i]);
            }

            StrartRes = StrartRes + FootRes;

            return StrartRes;


        }


        public int GetFieldPosition(string ResFile, DataTable printTable)
        {
            int ind = -1;
            int i = 0;
            while (ind == -1 & i < printTable.Columns.Count)
            {
                String KeyField = "A" + printTable.Columns[i].ColumnName + "A";
                ind = ResFile.IndexOf(KeyField);
                if (ind == -1)
                {
                    KeyField = "I" + printTable.Columns[i].ColumnName + "I";
                    ind = ResFile.IndexOf(KeyField);
                }
                i++;
            }
            return ind;
        }


    }
}