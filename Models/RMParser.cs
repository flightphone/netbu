using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using netbu;

namespace netbu.Models
{
    public enum FileTipe { Agents, Process, De_icing };

    class allRowParam
    {
        public string FileName = "";
        public DateTime FileDate;     //Дата создания файла
        public DateTime WriteReport;  //Время записи
        public String Msg = "OK";
        public int retCode = 0;    //Код возврата
        //public FileInfo fi;        //Системная информация о файле
        public Guid RF_PK;
    }
    class RMParser
    {
        private SqlConnection connection;
        public string Parse(FileTipe FType, string dataLine, string cnStr)
        {
            connection = new SqlConnection(cnStr);
            allRowParam addParam = new allRowParam();
            addParam.Msg = "OK";
            if (FType == FileTipe.Agents)
                addParam.FileName = "agents.xml"; //Path.GetFileName(fileName);
            else
                if (FType == FileTipe.Process)
                addParam.FileName = "process.xml";
            else
                addParam.FileName = "de-icing.xml";

            addParam.WriteReport = DateTime.Now;
            addParam.FileDate = DateTime.Now;
            addParam.RF_PK = Guid.NewGuid();

            string logstr1 = addParam.FileName + ":" + addParam.FileDate.ToString() + "-";
            try
            {
                DataTable AODBTable = getTable(dataLine, addParam, FType);
                int n = AODBTable.Rows.Count;
                connection.Open();
                //Записываем в базу строки таблицы
                for (int i = 0; i < n; i++)
                {
                    WriteBase(AODBTable.Rows[i], FType);
                }
                //Обновляем основную таблицу
                CloseBase(addParam);
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                addParam.Msg = "Error:" + ex.Message;
                addParam.retCode = -1;
            }
            string logstr2 = addParam.WriteReport.ToString() + "-" + addParam.Msg;
            return logstr1 + logstr2 + "\n";
        }

        private string GetField(String ResHTML, String FieldName)
        {
            String res = "";
            String Find = "<" + FieldName + ">(.*?)</" + FieldName + ">";
            Regex rFind = new Regex(Find, RegexOptions.IgnoreCase);
            if (rFind.IsMatch(ResHTML))
            {
                res = rFind.Match(ResHTML).Groups[1].Value;
            }
            return res;
        }

        private DataTable getTable(String ResHTML, allRowParam addParam, FileTipe FType)
        {
            string sql = "";
            if (FType == FileTipe.Process)
                sql = "select * from RMS_Tasks_Hist (nolock) where 1=2";
            else
                if (FType == FileTipe.Agents)
                sql = "select * from RMS_Users_Hist (nolock) where 1=2";
            else
                sql = "select * from RMS_Deicing_Hist (nolock) where 1=2";

            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            DataTable rTab = new DataTable();
            da.Fill(rTab);
            String FindTr = @"<ROW num=.*?>((.|\n)*?)</ROW>";
            Regex rr = new Regex(FindTr, RegexOptions.IgnoreCase);

            MatchCollection ListRow = rr.Matches(ResHTML);

            for (int i = 0; i < ListRow.Count; i++)
            {

                Match mRow = ListRow[i];
                String RowTxt = mRow.Groups[1].Value;
                DataRow rw = rTab.NewRow();
                rTab.Rows.Add(rw);
                if (FType == FileTipe.Agents)
                {
                    rw["RU_PK"] = Guid.NewGuid();
                    rw["RU_HIST"] = addParam.RF_PK;
                    rw["RU_Flt_ID"] = int.Parse(GetField(RowTxt, "FLIGHT_ID"));
                    rw["RU_AgentID"] = GetField(RowTxt, "AGENT_CODE");
                    rw["RU_AgentName"] = GetField(RowTxt, "AGENT_NAME");

                }
                else
                    if (FType == FileTipe.Process) //25.09.2019
                {
                    rw["RM_PK"] = Guid.NewGuid();
                    rw["RM_HIST"] = addParam.RF_PK;
                    rw["RM_Flt_ID"] = int.Parse(GetField(RowTxt, "FLT_ID"));
                    rw["RM_ProcessID"] = int.Parse(GetField(RowTxt, "PROCESSID"));
                    rw["RM_ProcessName"] = GetField(RowTxt, "PROCESSNAME");
                    rw["RM_NotSendInfo"] = GetField(RowTxt, "NOT_SEND_INFO");

                    if (!String.IsNullOrEmpty(GetField(RowTxt, "ACTUALTIMEBEGIN")))
                        rw["RM_ActualTimeBegin"] = DateTime.Parse(GetField(RowTxt, "ACTUALTIMEBEGIN"));

                    if (!String.IsNullOrEmpty(GetField(RowTxt, "ACTUALTIMEEND")))
                        rw["RM_ActualTimeEnd"] = DateTime.Parse(GetField(RowTxt, "ACTUALTIMEEND"));

                    if (!String.IsNullOrEmpty(GetField(RowTxt, "PROCESS_MOD_FLAG")))
                        rw["RM_ProcessModFlag"] = int.Parse(GetField(RowTxt, "PROCESS_MOD_FLAG"));
                    if (!String.IsNullOrEmpty(GetField(RowTxt, "SOURCEREF")))
                        rw["RM_SourceRef"] = int.Parse(GetField(RowTxt, "SOURCEREF"));
                }
                else
                {
                    rw["DE_PK"] = Guid.NewGuid();
                    rw["DE_HIST"] = addParam.RF_PK;
                    rw["DE_FLIGHT_ID"] = int.Parse(GetField(RowTxt, "FLIGHT_ID"));
                    rw["DE_OPERATOR_NAME"] = GetField(RowTxt, "OPERATOR_NAME");
                    rw["DE_AUTO_NUM_POM"] = GetField(RowTxt, "AUTO_NUM_POM");
                    rw["DE_TYPE_I"] = int.Parse(GetField(RowTxt, "TYPE_I"));
                    rw["DE_TYPE_IV"] = int.Parse(GetField(RowTxt, "TYPE_IV"));
                    rw["DE_WATER"] = int.Parse(GetField(RowTxt, "WATER"));
                    rw["DE_TAB_NUM"] = GetField(RowTxt, "TAB_NUM");
                    rw["DE_IO"] = GetField(RowTxt, "IO");
                }


            }


            return rTab;
        }


        private void WriteBase(DataRow rw, FileTipe FType)
        {
            string sql = "";
            if (FType == FileTipe.Process)
                sql = "p_RMS_Tasks_Hist_INSERT";
            else
                if (FType == FileTipe.Agents)
                sql = "p_RMS_Users_Hist_INSERT";
            else
                sql = "p_RMS_Deicing_Hist_INSERT";

            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < rw.Table.Columns.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + rw.Table.Columns[i].ColumnName, rw[i]);
            }
            cmd.ExecuteNonQuery();
        }


        private void CloseBase(allRowParam par)
        {
            string sql = "p_RMS_Files_Insert";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RF_PK", par.RF_PK);
            cmd.Parameters.AddWithValue("@RF_FileName", par.FileName);
            cmd.Parameters.AddWithValue("@RF_Date", par.FileDate);
            cmd.Parameters.AddWithValue("@RF_AUDTDATE", par.WriteReport);


            cmd.ExecuteNonQuery();

        }


    }
}