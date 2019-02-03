using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using netbu;

namespace netbu.Models
{

    public class treeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<object> children { get; set; }

        public string iconCls { get; set; }

        public object attributes { get; set; }

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
                        ilist.attributes = new { link1 = mi["link1"].ToString(), params1 = mi["params"].ToString() };
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