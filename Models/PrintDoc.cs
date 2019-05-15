using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Newtonsoft.Json;

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
                for (int i = 1; i < Tab.Columns.Count; i++){
                    s = s + ";" + @"""" + rw[i].ToString().TrimEnd().Replace(@"""", @"""""") + @"""";
                }
                Res.AppendLine(s);     
            }
            
            return Res.ToString().Trim();
        }
        public byte[] PrintDocx(String RepName, DataRow printRow, List<DataTable> Tables, Dictionary<string, byte[]> Images)
        {

            string FileName = @"wwwroot\Reports\rep" + Guid.NewGuid().ToString() + ".zip";
            string OutPath = @"wwwroot\Reports\" + RepName;
            string document = OutPath + @"\word\document.xml";
            string ResWord = File.ReadAllText(document, Encoding.UTF8);


            //Меняем поля
            ResWord = ReplaceFieldXML(ResWord, printRow);

            DataTable printTab;
            if (Tables != null)
                for (int i = 0; i < Tables.Count; i++)
                {
                    printTab = Tables[i];
                    ResWord = SetTableDocx(ResWord, printTab);
                }

            ZipFile.CreateFromDirectory(OutPath, FileName);
            using (FileStream zipToOpen = new FileStream(FileName, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry doc = archive.GetEntry(@"word/document.xml");
                    using (StreamWriter writer = new StreamWriter(doc.Open()))
                    {
                        writer.Write(ResWord);
                    }

                    //Картинки
                    foreach (string key in Images.Keys)
                    {
                        ZipArchiveEntry img = archive.GetEntry(@"word/media/" + key);
                        if (img!=null)
                        {
                            using(Stream s = img.Open())
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