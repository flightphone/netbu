using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Linq;

namespace WpfBu.Models
{

    public class JoinRow
    {
        public string IdDeclare { get; set; }
        public string classname { get; set; }
        public Dictionary<string, string> fields { get; set; }
        public string textField { get; set; }
        public RootForm FindConrol { get; set; }
    }
    public class FinderField //: INotifyPropertyChanged
    {
        private string _Sort = "";

        private string _FindString = "";
        public Finder Parent { get; set; }
        //public event PropertyChangedEventHandler PropertyChanged;

        public string FieldName { get; set; }
        public string FieldCaption { get; set; }
        public int Width { get; set; }
        public bool Visible { get; set; }
        public string DisplayFormat { get; set; }
        public string FindString
        {
            get => _FindString;
            set
            {
                _FindString = value;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FindString)));
            }
        }
        public string Sort
        {
            get
            {
                return _Sort;
            }
            set
            {
                _Sort = value;
                if (Parent == null)
                    return;
                if (_Sort != "Нет")
                {
                    Parent.MaxSortOrder++;
                    SortOrder = Parent.MaxSortOrder;
                }
                else
                    SortOrder = null;
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sort)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortOrder)));
            }
        }
        public int? SortOrder
        {
            get; set;
        }
    }

    public class EditField
    {
        public string FieldName { get; set; }
        public string FieldCaption { get; set; }
        //public Control FieldEditor { get; set; }

        public string DisplayFormat { get; set; }

        public JoinRow joinRow { get; set; }
    }

    public class Finder : RootForm
    {


        #region prop
        //public DataGrid MainGrid { get; set; }

        //public DataView MainView { get; set; }

        //public DataTable TotalTable { get; set; }

        //public DataTable data { get; set; }
        public string Mode { get; set; }
        public List<Dictionary<string, object>> MainTab { get; set; }
        public List<Dictionary<string, object>> TotalTab { get; set; }
        public List<string> ColumnTab { get; set; }
        public string SQLText { get; set; }
        public string DecName { get; set; }
        public string Descr { get; set; }

        public string EditProc { get; set; }
        public string DelProc { get; set; }

        public string TableName { get; set; }

        public string SaveFieldList { get; set; }

        public string KeyF { get; set; }
        public string DispField { get; set; }
        public string KeyValue { get; set; }
        public string SumFields { get; set; }
        public int FROZENCOLS { get; set; }

        public string OrdField { get; set; }
        public string addFilter { get; set; }

        public int MaxSortOrder { get; set; }

        public List<FinderField> Fcols { get; set; }

        //public FilterList FilterControl { get; set; }
        //public FinderMenu MenuControl { get; set; }

        public Dictionary<string, string> TextParams { get; set; }

        public Dictionary<string, object> SQLParams { get; set; }

        public Dictionary<string, object> DefaultValues { get; set; }

        public bool pagination { get; set; }

        public int nrows { get; set; }

        private int _page = 1;
        public int page
        {
            get => _page;
            set
            {
                _page = value;
            }
        }
        public string TotalString { get; set; }

        public Int64 MaxPage { get; set; }

        public bool OKFun { get; set; }

        public Editor ReferEdit { get; set; }
        #endregion
        public string Account {get; set; }
        public override void start(object o)
        {

            string sql;
            if (MainObj.IsPostgres)
                sql = "select iddeclare, decname, descr, dectype, decsql, keyfield, dispfield, keyvalue, dispvalue, keyparamname, dispparamname, isbasename, descript, addkeys, tablename, editproc, delproc, image_bmp, savefieldlist, p.paramvalue from t_rpdeclare d left join t_sysparams p on 'GridFind' || d.decname = p.paramname where iddeclare = ";
            else
                sql = "select iddeclare, decname, descr, dectype, decsql, keyfield, dispfield, keyvalue, dispvalue, keyparamname, dispparamname, isbasename, descript, addkeys, tablename, editproc, delproc, image_bmp, savefieldlist, p.paramvalue from t_rpdeclare d left join t_sysparams p on 'GridFind' + d.decname = p.paramname where iddeclare = ";
            sql = sql + o.ToString();
            MainObj.Dbutil = new DBUtil();
            DataTable t_rp = MainObj.Dbutil.Runsql(sql);
            DataRow rd = t_rp.Rows[0];
            string paramvalue = rd["paramvalue"].ToString();
            if (string.IsNullOrEmpty(SQLText))
                SQLText = rd["decsql"].ToString();
            DecName = rd["decname"].ToString();
            Descr = rd["descr"].ToString();
            text = Descr;

            EditProc = rd["editproc"].ToString();
            DelProc = rd["delproc"].ToString();
            TableName = rd["tablename"].ToString();

            if (string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(EditProc))
            {
                TableName = EditProc.ToLower().Replace("p_", "").Replace("_edit", "");
            }

            KeyF = rd["keyfield"].ToString();
            DispField = rd["dispfield"].ToString();
            KeyValue = rd["keyvalue"].ToString();
            SaveFieldList = rd["savefieldlist"].ToString();
            /*
            if (rd["dectype"] == DBNull.Value)
                nrows = 7;
            else
                nrows = (int)rd["dectype"];
            */
            if (nrows == 0)
                nrows = 30;
            pagination = (nrows >= 30);

            if (DefaultValues == null)
                DefaultValues = new Dictionary<string, object>();

            DefaultValues.Add("audtuser", Account);
            DefaultValues.Add("last_change_user", Account);



            if (Fcols == null)
                CreateColumns(paramvalue);
            if (Mode!="csv")
                UpdateTab();
            if (Mode == "new")
                CreateEditor();
            //CreateMenu();
            //CreateFilter();
            //CreateContent();



        }

        #region startinit
        /*
        public virtual void AddInit(FinderMenu fm, Finder form)
        { 
        
        }
        */

        public virtual void CreateEditor()
        {
            if (!string.IsNullOrEmpty(EditProc) && !OKFun)
            {
                ReferEdit = new Editor();
                ReferEdit.start(this);
            }

        }

        /*
                public void OpenDetail()
                {
                    if (MainGrid.SelectedItem == null)
                    {
                        MessageBox.Show("Выберете запись", "Детали");
                        return;
                    }
                    DataRow rw = ((DataRowView)MainGrid.SelectedItem).Row;


                    Finder res;
                    string idchiled = this.id + "_" + rw[KeyF].ToString();

                    if (Parent.formList.ContainsKey(idchiled))
                    {
                        res = (Finder)Parent.formList[idchiled];
                    }
                    else
                    {
                        res = new Finder
                        {
                            id = idchiled,
                            Parent = this.Parent
                        };
                        res.TextParams = new Dictionary<string, string>() { { KeyF, rw[KeyF].ToString() } };
                        res.DefaultValues = new Dictionary<string, object>() { { KeyF, rw[KeyF] } };
                        res.start(KeyValue);
                        res.Descr = res.Descr + " (" + rw[DispField].ToString() + ")";
                        res.text = res.Descr;
                        Parent.formList.Add(idchiled, res);
                        Parent.WinListSource.Add(res);
                    }

                    Parent.userMenu.Content = res.userMenu;
                    Parent.userContent.Content = res.userContent;
                    Parent.CurrentId = idchiled;

                }
                public virtual void CreateMenu()
                {


                    userMenu = new ContentControl();

                    FinderMenu fm = new FinderMenu()
                    {
                        DataContext = this
                    };

                    if (!OKFun)
                    {
                        fm.ButOK.Visibility = Visibility.Collapsed;
                        fm.ButCancel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        fm.ButCSV.Visibility = Visibility.Collapsed;
                    }

                    if (string.IsNullOrEmpty(EditProc) || OKFun)
                    {
                        fm.EditPanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        fm.AddBut.Click += (object sender, RoutedEventArgs e) =>
                        {
                            ReferEdit.Add();
                        };
                        fm.EditBut.Click += (object sender, RoutedEventArgs e) =>
                        {
                            ReferEdit.Edit();
                        };
                        fm.DelBut.Click += (object sender, RoutedEventArgs e) =>
                        {
                            ReferEdit.Delete();
                        };
                        MainGrid.MouseDoubleClick += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                            ReferEdit.Edit();
                    }

                    fm.ButUpdate.Click += (object sender, RoutedEventArgs e) =>
                    {
                        UpdateTab();
                    };

                    fm.ButPage.Click += (object sender, RoutedEventArgs e) =>
                    {
                        fm.PopupPage.IsPopupOpen = true;
                    };

                    if (pagination)
                    {

                        fm.ButLeft.Click += (object sender, RoutedEventArgs e) =>
                        {
                            if (_page > 1)
                                _page--;
                            UpdateTab();
                        };

                        fm.ButFirst.Click += (object sender, RoutedEventArgs e) =>
                        {
                            _page = 1;
                            UpdateTab();
                        };


                        fm.ButRight.Click += (object sender, RoutedEventArgs e) =>
                        {
                            _page++;
                            UpdateTab();
                        };

                        fm.ButLast.Click += (object sender, RoutedEventArgs e) =>
                        {
                            _page = (int)MaxPage;
                            UpdateTab();
                        };
                    }
                    else
                    {
                        fm.NavPanel.Visibility = Visibility.Collapsed;
                    }
                    fm.ButCSV.Click += (object sender, RoutedEventArgs e) =>
                    {
                        ExportCSV();
                    };

                    if (!string.IsNullOrEmpty(KeyValue) && !OKFun)
                    {

                        fm.ButDetail.Click += (object sender, RoutedEventArgs e) =>
                            OpenDetail();


                        if (string.IsNullOrEmpty(EditProc))
                        {
                            MainGrid.MouseDoubleClick += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                            OpenDetail();
                        }

                    }
                    else
                        fm.ButDetail.Visibility = Visibility.Collapsed;

                    fm.FilterBut.Click += (object sender, RoutedEventArgs e) => {
                        userContent.Content = FilterControl;
                    };
                    fm.ClearBut.Click += (object sender, RoutedEventArgs e) => {
                        MaxSortOrder = 0;
                        foreach (var f in Fcols)
                        {
                            f.FindString = "";
                            f.Sort = "Нет";
                        }
                        CompilerFilterOrder();
                        SetFilterOrder();
                    };
                    AddInit(fm, this);
                    MenuControl = fm;
                    userMenu.Content = MenuControl;


                }


                public virtual void CreateContent()
                {
                    userContent = new ContentControl
                    {
                        Content = MainGrid
                    };
                }

                public void CreateFilter()
                {
                    FilterControl = new FilterList()
                    {
                        DataContext = this
                    };
                    FilterControl.CancelBut.Click += (object sender, RoutedEventArgs e)=>
                        {
                            userContent.Content = MainGrid;
                        };
                    FilterControl.OkBut.Click += (object sender, RoutedEventArgs e) =>
                    {
                        SetFilterOrder();
                        userContent.Content = MainGrid;
                    };

                }
                */

        public void CreateColumns(string s)
        {
            MaxSortOrder = 0;
            Fcols = new List<FinderField>();
            /*
            MainGrid = new DataGrid()
            {
                IsReadOnly = true,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserSortColumns = false,
                AutoGenerateColumns = false
            };
            */
            if (string.IsNullOrEmpty(s))
                return;
            XmlDocument xm = new XmlDocument();
            XmlElement xRoot, xCol;
            xm.LoadXml(s);
            xRoot = xm.DocumentElement;

            SumFields = xRoot.GetAttribute("SumFields");
            int frcols = 0;
            int.TryParse(xRoot.GetAttribute("FROZENCOLS"), out frcols);
            FROZENCOLS = frcols;
            int n = xRoot.ChildNodes.Count;

            for (int i = 0; i < n; i++)
            {

                xCol = (XmlElement)xRoot.ChildNodes.Item(i);
                if (xCol.Name == "COLUMN")
                {
                    string FName = xCol.Attributes["FieldName"].Value;
                    if (MainObj.IsPostgres)
                        FName = FName.ToLower();
                    string Title = xCol.Attributes["FieldCaption"].Value;
                    int Width = 0;
                    int.TryParse(xCol.Attributes["Width"].Value, out Width);
                    bool Vis = (xCol.Attributes["Visible"].Value == "1");
                    string DispFormat = xCol.Attributes["DisplayFormat"].Value;
                    if (Vis)
                    {
                        Fcols.Add(new FinderField()
                        {
                            FieldName = FName,
                            FieldCaption = Title,
                            Width = Width,
                            DisplayFormat = DispFormat,
                            Visible = Vis,
                            //Parent = this,
                            Sort = "Нет"

                        });
                    }
                }
            }


            /*
            MainGrid.LoadingRow += MainGrid_LoadingRow;

            foreach (FinderField f in Fcols)
            {
                if (f.Visible)
                {
                    Binding bn = new Binding(f.FieldName);
                    bn.StringFormat = f.DisplayFormat;
                    MainGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn()
                    {
                        Header = f.FieldCaption,
                        Binding = bn,
                        MaxWidth = 500
                    });
                }
            }
            */

        }

        /*
        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
            DataRowView item = e.Row.Item as DataRowView;
            if (item != null)
            {
                DataRow row = item.Row;
                int c = 0;
                if (row.Table.Columns.Contains("Color"))
                    if (row["Color"] != DBNull.Value)
                        c = (int)row["Color"];
                
                if (row.Table.Columns.Contains("color"))
                    if (row["color"] != DBNull.Value)
                        c = (int)row["color"];
                if (c != 0)
                {
                    System.Drawing.Color dc = System.Drawing.Color.FromArgb(c);
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(dc.R, dc.G, dc.B));
                    
                }    
            }
        }
        */
        /*
        public virtual void UpdateTotal()
        {
            string res = "";
            if (!pagination)
                res = "число записей: " + MainView.Count.ToString();
            else
            {
                Int64 total = 0;
                if (MainObj.IsPostgres)
                    total = (Int64)TotalTable.Rows[0]["n_total"];
                else
                    total = (Int32)TotalTable.Rows[0]["n_total"];

                if (page > 0)
                    res = string.Format("{0} - {1}/{2}", (page - 1) * nrows + 1, ((page * nrows) < total) ? (page * nrows) : total, total);
                else
                    res = "0 записей";
            }
            TotalString = res;
            if ( MenuControl != null)
            {
                MenuControl.TotalString.Text = TotalString;
                MenuControl.page.Text = page.ToString();
                MenuControl.MaxPage.Text = MaxPage.ToString();
            }

        }
        */
        public DataTable UpdateCSV()
        {
            string PrepareSQL = SQLText;
            PrepareSQL = PrepareSQL.Replace("[Account]", Account);
            if (TextParams != null)
                foreach (string k in TextParams.Keys)
                {
                    PrepareSQL = PrepareSQL.Replace("[" + k + "]", TextParams[k]);
                }

            string sql = PrepareSQL;
            CompilerFilterOrder();
            string decSQL = sql;
            var localOrdField = "";
            var n = sql.ToLowerInvariant().IndexOf("order by");
            if (n != -1)
            {
                decSQL = sql.Substring(0, n);
                localOrdField = sql.Substring(n + 8);
            }
            if (string.IsNullOrEmpty(OrdField))
                OrdField = localOrdField;
            if (!string.IsNullOrEmpty(addFilter))
            {
                if (decSQL.ToLowerInvariant().IndexOf(" where ") == -1 && decSQL.ToLowerInvariant().IndexOf(" where\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere ") == -1)
                    decSQL += " where ";
                else
                    decSQL += " and ";

                decSQL += addFilter;
            }
            if (!string.IsNullOrEmpty(OrdField))
                decSQL = decSQL + " order by " + OrdField;
            sql = decSQL;
            var data = MainObj.Dbutil.Runsql(sql, SQLParams);
            return data;
        }


        public virtual void UpdateTab()
        {
            DataTable TTable = null;
            DataTable data = null;
            /*
            if (userContent!=null)
                userContent.Content = MainGrid;
            */

            string PrepareSQL = SQLText;
            PrepareSQL = PrepareSQL.Replace("[Account]", Account);
            if (TextParams != null)
                foreach (string k in TextParams.Keys)
                {
                    PrepareSQL = PrepareSQL.Replace("[" + k + "]", TextParams[k]);
                }


            var sqltotal = PrepareSQL;
            string sql = PrepareSQL;
            CompilerFilterOrder();
            string decSQL = sql;
            var localOrdField = "";
            var n = sql.ToLowerInvariant().IndexOf("order by");
            if (n != -1)
            {
                decSQL = sql.Substring(0, n);
                localOrdField = sql.Substring(n + 8);
            }
            if (string.IsNullOrEmpty(OrdField))
                OrdField = localOrdField;
            if (!string.IsNullOrEmpty(addFilter))
            {
                if (decSQL.ToLowerInvariant().IndexOf(" where ") == -1 && decSQL.ToLowerInvariant().IndexOf(" where\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere ") == -1)
                    decSQL += " where ";
                else
                    decSQL += " and ";

                decSQL += addFilter;
            }

            sqltotal = decSQL;
            var sqlpag = decSQL;
            if (!string.IsNullOrEmpty(OrdField))
                decSQL = decSQL + " order by " + OrdField;
            sql = decSQL;

            /*
                Итоги
            */

            string[] sums = new string[0];
            if (!string.IsNullOrEmpty(SumFields))
            {
                var sql1 = sqltotal;
                sums = SumFields.ToLowerInvariant().Split(",");
                sqltotal = "select count(*) n_total";
                for (var i = 0; i < sums.Length; i++)
                    sqltotal = sqltotal + ", sum(" + sums[i] + ") " + sums[i];
                sqltotal = sqltotal + "  from (" + sql1 + ") a";
            }
            else
            {
                sqltotal = "select count(*) n_total from (" + sqltotal + ") a";
            }

            if (pagination)
            {
                TTable = MainObj.Dbutil.Runsql(sqltotal, SQLParams);
                Int64 total = 0;
                if (MainObj.IsPostgres)
                    total = (Int64)TTable.Rows[0]["n_total"];
                else
                    total = (Int32)TTable.Rows[0]["n_total"];
                MaxPage = total / nrows;

                if ((total % nrows) != 0)
                    MaxPage += 1;
                if (page > MaxPage)
                    _page = (int)MaxPage;

                if (MaxPage > 0 && _page == 0)
                    _page = 1;

                TotalTab = MainObj.Dbutil.DataToJson(TTable);

            }
            else
            {
                MaxPage = 1;
                _page = 1;
            }

            if (MainObj.IsPostgres)
                sql = sql + " limit " + nrows.ToString() + " offset " + ((page - 1) * nrows).ToString();
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("WITH tmpWebFind AS (");
                sb.AppendLine(" SELECT TMPA.*, ");
                sb.AppendLine(string.Format(" ROW_NUMBER() OVER (ORDER BY {0}) AS IDTMPNUM", OrdField));
                sb.AppendLine(string.Format(" FROM ({0}) TMPA ", sqlpag));
                sb.AppendLine(") ");
                sb.AppendLine(" SELECT * FROM tmpWebFind A ");
                sb.AppendLine(string.Format(" WHERE IDTMPNUM BETWEEN {0} AND {1}", (page - 1) * nrows + 1, page * nrows));
                sb.AppendLine(" ORDER BY IDTMPNUM");
                sql = sb.ToString();
            }




            //DataTable data;
            if (pagination)
                data = MainObj.Dbutil.Runsql(sql, SQLParams);
            else
                data = MainObj.Dbutil.Runsql(PrepareSQL, SQLParams);


            MainTab = MainObj.Dbutil.DataToJson(data);
            ColumnTab = MainObj.Dbutil.DataColumn(data);
            //MainView = data.DefaultView;
            //MainGrid.ItemsSource = MainView;
            //UpdateTotal();
        }
        #endregion
        public void CompilerFilterOrder()
        {

            var fls = Fcols.Where(f => !string.IsNullOrEmpty(f.FindString)).Select(f =>
            {
                string s = "";
                if (f.FindString[0] == '!')
                    s = " (not " + f.FieldName + " like '%" + f.FindString.Substring(1) + "%') ";
                else
                    s = " (" + f.FieldName + " like '%" + f.FindString + "%') ";
                return s;
            });

            var ords = Fcols.Where(f => f.SortOrder > 0 && f.Sort != "Нет").OrderBy(f => f.SortOrder).Select(f =>
            {
                string s = "";
                if (f.Sort == "По возрастанию")
                    s = " " + f.FieldName;
                if (f.Sort == "По убыванию")
                    s = " " + f.FieldName + " desc";
                return s;
            });



            addFilter = string.Join(" and ", fls);
            OrdField = string.Join(",", ords);
        }

        public void SetFilterOrder()
        {
            if (!pagination)
            {
                CompilerFilterOrder();
                //MainView.RowFilter = addFilter;
                //MainView.Sort = OrdField;
                //UpdateTotal();
            }
            else
            {
                UpdateTab();
            }

        }

        //public IEnumerable<string> Foods => new[] { "Нет", "По возрастанию", "По убыванию" };


        private string rwCSV(DataRow rw)
        {
            return string.Join(";", Fcols.Select(f =>
            {
                return @"""" + rw[f.FieldName].ToString().Replace(@"""", @"""""") + @"""";
            }));
        }

        public string ExportCSV()
        {

            StringBuilder Res = new StringBuilder();
            var cols = Fcols.Select(f =>
            {
                return @"""" + f.FieldCaption.ToString().Replace(@"""", @"""""") + @"""";
            });

            string s = string.Join(';', cols);
            Res.AppendLine(s);

            DataTable data = UpdateCSV();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow rw = data.Rows[i];
                s = rwCSV(rw);
                Res.AppendLine(s);
            }
            return Res.ToString();
            
        }

    }
}