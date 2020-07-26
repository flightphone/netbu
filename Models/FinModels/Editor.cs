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

        //public DataRow WorkRow { get; set; }

        //public DataRow ParentRow { get; set; }

        //public DataTable ParamTab { get; set; }

        //public EditorList El { get; set; }

        public string[] SaveFieldList { get; set; }
        //public ParamMenu ParamText { get; set; }

        public void SetDefault()
        {
            /*
            foreach (string s in ReferFinder.DefaultValues.Keys)
            {
                if (ParamTab.Columns.Contains(s))
                    WorkRow[s] = ReferFinder.DefaultValues[s];
            }
            */
        }

        public bool FlagAdd { get; set; }
        /*
        public virtual void Add()
        {
            FlagAdd = true;
            ParamText.Descr.Text = "Новая запись";
            for (int i = 0; i < ParamTab.Columns.Count; i++)
                WorkRow[i] = DBNull.Value;

            WorkRow[ReferFinder.KeyF] = MainObj.Dbutil.NewID(ReferFinder.TableName);
            SetDefault();
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;

        }

        public virtual void Edit()
        {

            FlagAdd = false;
            if (ReferFinder.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись для редактирования", "Редактирование записи");
                return;
            }
            DataRow rw = ((DataRowView)ReferFinder.MainGrid.SelectedItem).Row;
            for (int i = 0; i < ParamTab.Columns.Count; i++)
                WorkRow[i] = rw[i];

            ParentRow = rw;

            ParamText.Descr.Text = rw[ReferFinder.DispField].ToString();
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;

        }

        public virtual void Delete()
        {
            
            if (ReferFinder.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись для удаления", "Удаление записи");
                return;
            }
            DataRow rw = ((DataRowView)ReferFinder.MainGrid.SelectedItem).Row;
            string s = $"Удалить запись '{rw[ReferFinder.DispField]}'?";
            if (MessageBox.Show(s, "Подтвердите удаление записи", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;
            var Param = new Dictionary<string, object>();
            string pname = "@" + ReferFinder.KeyF;
            Param.Add(pname, rw[ReferFinder.KeyF]);
            string sql;
            if (MainObj.IsPostgres)
                sql = $"select * from {ReferFinder.DelProc} ({pname});";
            else
                sql = $"exec {ReferFinder.DelProc} {pname}";

            try
            {
                MainObj.Dbutil.ExecSQL(sql, Param);
                ReferFinder.data.Rows.Remove(rw);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        

        public virtual bool Save()
        {
            SetDefault();
            var vals = new List<string>();
            var Param = new Dictionary<string, object>();
            foreach (string fname in SaveFieldList)
            {
                string pname = "@_" + fname;
                Param.Add(pname, WorkRow[fname]);
                string val = "";
                if (MainObj.IsPostgres)
                    val = $"_{fname} => {pname}";
                else
                    val = $"@{fname} = {pname}";
                vals.Add(val);
            }
            string sqlpar = string.Join(",", vals);
            string sql = "";
            if (MainObj.IsPostgres)
                sql = $"select * from {ReferFinder.EditProc}({sqlpar})";
            else
                sql = $"exec {ReferFinder.EditProc} {sqlpar}";

            DataTable data;
            try
            {
                data = MainObj.Dbutil.Runsql(sql, Param);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (data.Rows.Count == 0)
                return true;

            DataRow rw = data.Rows[0];
            if (data.Columns.Count == 1)
                WorkRow[ReferFinder.KeyF] = rw[0];
            else
            {
                for (int i = 0; i < data.Columns.Count; i++)
                    if (ParamTab.Columns.Contains(data.Columns[i].ColumnName))
                        WorkRow[data.Columns[i].ColumnName] = rw[i];
            }

            if (FlagAdd)
                ParentRow = ReferFinder.data.NewRow();

            for (int i = 0; i < ParamTab.Columns.Count; i++)
                ParentRow[i] = WorkRow[i];

            if (FlagAdd)
                ReferFinder.data.Rows.Add(ParentRow);

            return true;

        }


        public void ButOK_Click(object sender, RoutedEventArgs e)
        {
            int i = (int)((Button)sender).Tag;
            JoinRow al = Editors[i].joinRow;
            Finder fc = (Finder)al.FindConrol;
            if (fc.MainGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберете запись", Editors[i].FieldCaption);
                return;
            }
            DataRow rw = ((DataRowView)fc.MainGrid.SelectedItem).Row;
            foreach (string s in al.fields.Keys)
            {
                WorkRow[al.fields[s]] = rw[s];
            }
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;
        }

        public void ButCancel_Click(object sender, RoutedEventArgs e)
        {
            ReferFinder.userContent.Content = El;
            ReferFinder.userMenu.Content = ParamText;
        }

        public void ButFind_Cklick(object sender, RoutedEventArgs e)
        {
            int i = (int)((Button)sender).Tag;
            ReferFinder.userContent.Content = Editors[i].joinRow.FindConrol.userContent;
            ReferFinder.userMenu.Content = Editors[i].joinRow.FindConrol.userMenu;
        }
    */
    public override void start(object o)
        {
            ReferFinder = (Finder)o;
            //ParamTab = ReferFinder.data.Clone();
            //WorkRow = ParamTab.NewRow();
            //ParamTab.Rows.Add(WorkRow);
            
            string slist = ReferFinder.SaveFieldList;
            /*
            if (string.IsNullOrEmpty(slist))
            {
                DataColumn[] a = new DataColumn[ParamTab.Columns.Count];
                ParamTab.Columns.CopyTo(a, 0);
                slist = string.Join(",", a.Select(f => f.ColumnName));
            }
            */
            SaveFieldList = slist.Split(',');

            Editors = new List<EditField>();
            //int minwidth = 500;

            string sql = $"select * from t_sysFieldMap where decname = '{ReferFinder.DecName}'";
            DataTable sysFieldMap = MainObj.Dbutil.Runsql(sql);

            for (int i = 0; i < ReferFinder.Fcols.Count; i++)
            {
                //Control fe = null;
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

                        fc = new Finder();
                        fc.OKFun = true;
                        if (ClassName == "Bureau.Finder")
                            fc.nrows = 100;
                        else
                            fc.nrows = 7;

                        fc.start(jr.IdDeclare);
                        /*
                        fc.MenuControl.ButOK.Tag = i;
                        fc.MenuControl.ButOK.Click += ButOK_Click;
                        fc.MainGrid.MouseDoubleClick += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                        {
                            ButOK_Click(fc.MenuControl.ButOK,  null);
                        };


                        fc.MenuControl.ButCancel.Click += ButCancel_Click;
                        */
                        jr.FindConrol = fc;
                        
                    }

                    if (ClassName == "Bureau.Finder")
                    {
                        /*
                        fe = new FinderEditor()
                        {
                            MinWidth = minwidth
                        };

                        ((FinderEditor)fe).ButFind.Tag = i;
                        ((FinderEditor)fe).ButFind.Click += ButFind_Cklick;
                        */
                        
                    }

                    if (ClassName == "Bureau.GridCombo")
                    {
                        /*
                        fe = new ComboBox()
                        {
                            MinWidth = minwidth,
                            Foreground = System.Windows.Media.Brushes.White
                        };

                        ComboBox cb = (ComboBox)fe;
                        cb.ItemsSource = fc.MainView;
                        cb.DisplayMemberPath = a[0]["srcfield"].ToString();
                        DataRow[] c = sysFieldMap.Select($"groupdec = '{GroupDec}' and keyfield = 1");
                        cb.SelectedValuePath = c[0]["srcfield"].ToString();

                        Binding bd = new Binding(c[0]["dstfield"].ToString());
                        bd.Source = ParamTab;
                        cb.SetBinding(ComboBox.SelectedValueProperty, bd);
                        */
                    }
                }
                /*
                if (fe == null)
                    fe = new TextBox()
                        {
                            MinWidth = minwidth,
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = System.Windows.Media.Brushes.White
                        };
                */        



                
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
                
                /*
                foreach (var e in Editors)
                {
                    Binding bd = new Binding(e.FieldName);
                    bd.Source = ParamTab;
                    bd.StringFormat = e.DisplayFormat;
                    bd.ConverterCulture = CultureInfo.CurrentCulture;


                    if (e.FieldEditor.GetType() == typeof(TextBox))
                        ((TextBox)e.FieldEditor).SetBinding(TextBox.TextProperty, bd);
                    if (e.FieldEditor.GetType() == typeof(FinderEditor))
                    {
                        ((FinderEditor)e.FieldEditor).EditFind.SetBinding(TextBox.TextProperty, bd);
                    }

                    

                }


                El = new EditorList()
                {
                    DataContext = this
                };
                ParamText = new ParamMenu();
                ParamText.Descr.Text = "Редактор";
                ParamText.ButOK.Click += (object sender, RoutedEventArgs e) =>
                {
                    if (Save())
                    {
                        ReferFinder.userContent.Content = ReferFinder.MainGrid;
                        ReferFinder.userMenu.Content = ReferFinder.MenuControl;
                    }

                };

                ParamText.ButCancel.Click += (object sender, RoutedEventArgs e) =>
                {

                    ReferFinder.userContent.Content = ReferFinder.MainGrid;
                    ReferFinder.userMenu.Content = ReferFinder.MenuControl;
                };
                */

                //base.start(o);
            }

        }
    }
}
