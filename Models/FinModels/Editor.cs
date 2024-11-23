using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
//using System.Windows.Data;
using System.Data;
//using System.Windows.Controls;
using System.Globalization;
using System.Drawing;
using System.Linq;

namespace WpfBu.Models
{
    public class Editor : RootForm
    {
        private Finder ReferFinder { get; set; }

        public List<EditField> Editors { get; set; }

        public string[] SaveFieldList { get; set; }

        public override void start(object o)
        {
            ReferFinder = (Finder)o;
            string slist = ReferFinder.SaveFieldList;
            SaveFieldList = slist.Split(',');

            Editors = new List<EditField>();

            string sql = $"select * from t_sysFieldMap where decname = '{ReferFinder.DecName}'";
            DataTable sysFieldMap = MainObj.Dbutil.Runsql(sql);

            for (int i = 0; i < ReferFinder.Fcols.Count; i++)
            {
                JoinRow jr = null;
                Finder fc = null;
                string GroupDec = "";
                DataRow[] a = sysFieldMap.Select($"dstfield = '{ReferFinder.Fcols[i].FieldName}' and isnull(classname, '') <> ''");

                if (a.Length > 0)
                {
                    jr = new JoinRow();
                    string ClassName = a[0]["classname"].ToString();
                    if (ClassName == "Bureau.Finder" || ClassName == "Bureau.GridCombo")
                    {
                        jr.classname = ClassName;
                        jr.IdDeclare = a[0]["iddeclare"].ToString();
                        GroupDec = a[0]["groupdec"].ToString();
                        DataRow[] b = sysFieldMap.Select($"groupdec = '{GroupDec}'");
                        jr.fields = new Dictionary<string, string>();
                        foreach (var rw in b)
                        {
                            jr.fields.Add(rw["srcfield"].ToString(), rw["dstfield"].ToString());
                        }
						/*
                        fc = new Finder();
                        fc.OKFun = true;
						//fc.Mode = "empty";
                        if (ClassName == "Bureau.Finder")
                            fc.nrows = 30;
                        else
                            fc.nrows = 7;
                        fc.Account = ReferFinder.Account;
                        //Передаем текстовые параметры основного Finder    
                        fc.TextParams = ReferFinder.TextParams;
                        fc.start(jr.IdDeclare);
                        jr.FindConrol = fc;
						*/

                    }

                    if (ClassName == "Bureau.Finder")
                    {

                    }

                    if (ClassName == "Bureau.GridCombo")
                    {
                        DataRow[] c = sysFieldMap.Select($"groupdec = '{GroupDec}' and keyfield = 1");
                        jr.keyField = c[0]["srcfield"].ToString();
                        jr.valField = c[0]["dstfield"].ToString();

                    }
                }
                Editors.Add(
                    new EditField()
                    {
                        FieldCaption = ReferFinder.Fcols[i].FieldCaption,
                        FieldName = ReferFinder.Fcols[i].FieldName,
                        DisplayFormat = ReferFinder.Fcols[i].DisplayFormat,
                            //FieldEditor = fe,
                            joinRow = jr
                    }
                    );

            }

        }
    }
}
