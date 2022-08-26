using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using netbu.Models;
using Newtonsoft.Json;
using Npgsql;
using Microsoft.AspNetCore.Authorization;
using Novell.Directory.Ldap;

namespace netbu.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index(string id)
        {
            //ViewBag.id = id;
            //ViewBag.account = User.Identity.Name;
            //return View();
            return Redirect("~/index.html#" + id);
        }

        #region secret
        [Route("ustore/gettree")]
        //[Authorize]
        public JsonResult gettree()
        {
            try
            {
                
                string account = User.Identity.Name;
                if (string.IsNullOrEmpty(account))
                    account = "malkin";
                var tu = new treeutil();

                var data = new DataTable();
                var cnstr = Program.isPostgres ? Program.AppConfig["cns"] : Program.AppConfig["mscns"]; 
                var sql = "select a.* , fn_getmenuimageid(a.caption) idimage from fn_mainmenu('ALL', @Account) a order by a.ordmenu, idmenu";
                if (!Program.isPostgres)
                    //sql = "select a.* , dbo.fn_getmenuimageid(a.caption) idimage from fn_mainmenu('ALL', @Account) a order by a.ordmenu, idmenu";
                    sql = "exec p_fn_getmenuimageid @Account";
                if (Program.isPostgres)
                {
                    var da = new NpgsqlDataAdapter(sql, cnstr);
                    da.SelectCommand.Parameters.AddWithValue("@Account", account);
                    da.Fill(data);
                }
                else
                {
                    var da = new SqlDataAdapter(sql, cnstr);
                    da.SelectCommand.Parameters.AddWithValue("@Account", account);
                    da.Fill(data);
                }

                var rootItem = new treeItem("root");
                rootItem.children = new List<object>();

                tu.CreateItems("Root/", rootItem, data);
                return Json(rootItem.children);
            }
            catch (Exception e)
            {
                return Json(new object[] { new { text = e.Message } });
            }
        }
        
        [Route("/ustore/tree.css")]
        public string treecss()
        {
            try
            {


                var sql = "select idimage, image_bmp from t_sysmenuimage";
                var cnstr = Program.isPostgres ? Program.AppConfig["cns"] : Program.AppConfig["mscns"];

                var data = new DataTable();
                if (Program.isPostgres)
                {
                    var da = new NpgsqlDataAdapter(sql, cnstr);
                    da.Fill(data);
                }
                else
                {
                    var da = new SqlDataAdapter(sql, cnstr);
                    da.Fill(data);
                }
                var sb = new StringBuilder();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    sb.AppendLine(".tree-" + data.Rows[i]["idimage"].ToString() + " { background:url(data:image/gif;base64," + data.Rows[i]["image_bmp"].ToString() + ") no-repeat center center; }");
                }
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }


        [Authorize]
        [Route("/pg/runsql")]
        public JsonResult runsql()
        {
            try
            {
                string sql = Request.Form["sql"];
                if (!Program.isPostgres && !string.IsNullOrEmpty(sql))
                    sql = sql.Replace("||", "+");
                var cnstr = Program.isPostgres ? Program.AppConfig["cns"] : Program.AppConfig["mscns"];
                var IdDeclare = Request.Form["IdDeclare"];
                var account = User.Identity.Name;

                //Не передаем account
                //var account = Request.Form["account"];
                //var password = Request.Form["password"];
                //NpgsqlDataAdapter da;
                /*
                var tu = new treeutil();
                if (!tu.checkAccess(account, password))
                {
                    return Json(new { message = "Access denied." });
                }
                */

                if (string.IsNullOrEmpty(sql) && string.IsNullOrEmpty(IdDeclare))
                {
                    return Json(new { message = "Пустая строка sql" });
                };
                if (!string.IsNullOrEmpty(IdDeclare))
                {
                    var sqldec = "select decsql from t_rpdeclare where iddeclare = @IdDeclare";
                    var redec = new DataTable();
                    if (Program.isPostgres)
                    {
                        var da = new NpgsqlDataAdapter(sqldec, cnstr);
                        da.SelectCommand.Parameters.AddWithValue("@IdDeclare", int.Parse(IdDeclare));
                        da.Fill(redec);
                    }
                    else
                    {
                        var da = new SqlDataAdapter(sqldec, cnstr);
                        da.SelectCommand.Parameters.AddWithValue("@IdDeclare", int.Parse(IdDeclare));
                        da.Fill(redec);
                    }

                    if (redec.Rows.Count == 0)
                    {
                        return Json(new { message = "Не найден IdDeclare: " + IdDeclare });
                    }
                    sql = redec.Rows[0]["decsql"].ToString();

                }

                if (sql.Trim().ToLower().Substring(0, 6) == "select")
                    sql = sql.Replace("[Account]", account);

                Int64 total = 0;
                var sqltotal = sql;
                Dictionary<string, object> footRow = new Dictionary<string, object>();

                if (Request.Form["pagination"] == "1" || !string.IsNullOrEmpty(Request.Form["LabelField"]))
                {
                    var nrows = 50;
                    var page = 1;

                    if (!string.IsNullOrEmpty(Request.Form["rows"]))
                        nrows = int.Parse(Request.Form["rows"]);

                    if (!string.IsNullOrEmpty(Request.Form["page"]))
                        page = int.Parse(Request.Form["page"]);

                    /*
                    TODO
                    Фильтровка сортировка
                     */

                    filterRule[] filterRules = new filterRule[0];
                    if (!string.IsNullOrEmpty(Request.Form["filterRules"]))
                        filterRules = JsonConvert.DeserializeObject<filterRule[]>(Request.Form["filterRules"]);
                    string order = Request.Form["order"];
                    string sort = Request.Form["sort"];

                    var addFilter = "";
                    if (filterRules.Length > 0)
                    {
                        var filters = new string[filterRules.Length];
                        for (var i = 0; i < filterRules.Length; i++)
                        {
                            var not = "";
                            string val = filterRules[i].value.Replace("'", "''");
                            if (val.Substring(0, 1) == "!")
                            {
                                if (val.Length > 1)
                                {
                                    not = " not ";
                                    val = val.Substring(1);
                                    filters[i] = (not + filterRules[i].field + " like '%" + val + "%'");
                                }
                            }
                            else
                                filters[i] = (not + filterRules[i].field + " like '%" + val + "%'");
                        }

                        addFilter = string.Join(" and ", filters);
                    }

                    var newsort = "";
                    if (!string.IsNullOrEmpty(sort))
                    {

                        string[] s = sort.Split(",");
                        string[] o = order.Split(",");
                        string[] sorts = new string[s.Length];

                        for (var i = 0; i < s.Length; i++)
                            sorts[i] = s[i] + " " + o[i];

                        newsort = string.Join(",", sorts);
                    }

                    string decSQL = sql;
                    string OrdField = "";
                    var n = sql.ToLowerInvariant().IndexOf("order by");
                    if (n != -1)
                    {
                        decSQL = sql.Substring(0, n);
                        OrdField = sql.Substring(n + 8);
                    }

                    if (!string.IsNullOrEmpty(newsort))
                        OrdField = newsort;

                    if (!string.IsNullOrEmpty(addFilter))
                    {
                        if (decSQL.ToLowerInvariant().IndexOf(" where ") == -1 && decSQL.ToLowerInvariant().IndexOf(" where\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere\n") == -1 && decSQL.ToLowerInvariant().IndexOf("\nwhere ") == -1)
                            decSQL = decSQL + " where ";
                        else
                            decSQL = decSQL + " and ";

                        decSQL = decSQL + addFilter;
                    }

                    sqltotal = decSQL;
                    var sqlpag = decSQL;
                    if (!string.IsNullOrEmpty(OrdField))
                        decSQL = decSQL + " order by " + OrdField;
                    sql = decSQL;

                    if (Request.Form["pagination"] == "1")
                    {
                        if (Program.isPostgres)
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

                    }

                    /*
                    Итоги
                     */

                    string[] sums = new string[0];
                    if (!string.IsNullOrEmpty(Request.Form["LabelField"]))
                    {
                        var sql1 = sqltotal;
                        string SumFields = Request.Form["SumFields"];
                        sums = SumFields.ToLowerInvariant().Split(",");

                        //footRow = new Dictionary<string, object>();
                        footRow.Add(Request.Form["LabelField"], Request.Form["LabelText"]);
                        sqltotal = "select count(*) n_total";
                        for (var i = 0; i < sums.Length; i++)
                            sqltotal = sqltotal + ", sum(" + sums[i] + ") " + sums[i];
                        sqltotal = sqltotal + "  from (" + sql1 + ") a";
                    }
                    else
                    {
                        sqltotal = "select count(*) n_total from (" + sqltotal + ") a";
                    }

                    try
                    {
                        var rec = new DataTable();
                        if (Program.isPostgres)
                        {
                            var da = new NpgsqlDataAdapter(sqltotal, cnstr);
                            da.Fill(rec);
                        }
                        else
                        {
                            var da = new SqlDataAdapter(sqltotal, cnstr);
                            da.Fill(rec);
                        }
                        if (Program.isPostgres)
                        {
                            total = (Int64)rec.Rows[0]["n_total"];
                        }
                        else
                        {
                            total = (Int32)rec.Rows[0]["n_total"];
                        }
                        if (!string.IsNullOrEmpty(Request.Form["LabelField"]))
                            for (var i = 0; i < sums.Length; i++)
                                footRow.Add(sums[i], rec.Rows[0][sums[i]]);

                    }
                    catch (Exception err)
                    {
                        return Json(new { message = err.Message });
                    }

                }

                var res = new DataTable();
                if (Program.isPostgres)
                {
                    var da = new NpgsqlDataAdapter(sql, cnstr);
                    da.Fill(res);
                }
                else
                {
                    var da = new SqlDataAdapter(sql, cnstr);
                    da.Fill(res);
                }
                var fields = new List<object>();
                foreach (DataColumn col in res.Columns)
                {
                    fields.Add(new { name = col.ColumnName });
                }

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                foreach (DataRow rw in res.Rows)
                {
                    var user = new Dictionary<string, object>();
                    foreach (DataColumn col in res.Columns)
                    {
                        if (col.DataType == typeof(System.DateTime) & rw[col] != DBNull.Value)
                            user.Add(col.ColumnName, ((DateTime)rw[col]).ToString("yyyy-MM-ddTHH:mm:ss.000Z"));
                        else
                            user.Add(col.ColumnName, (rw[col] == DBNull.Value) ? null : rw[col]);
                    }
                    rows.Add(user);
                }
                if (string.IsNullOrEmpty(Request.Form["array"]))

                    if (!string.IsNullOrEmpty(Request.Form["LabelField"]))
                        return Json(new { fields = fields, rows = rows, total = total, footer = new object[] { footRow } });
                    else
                        return Json(new { fields = fields, rows = rows, total = total });
                else
                    return Json(rows);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Invalid storage type: DBNull.")
                    return Json(new { });
                else
                    return Json(new { message = ex.Message });
            }

        }


        [Route("/pg/getid/{table_name}")]
        public JsonResult getid(string table_name)
        {
            if (Program.isPostgres)
            {
                var sql = "select column_default, udt_name  from information_schema.columns  where table_name = @table_name and ordinal_position = 1";
                var cnstr = Program.AppConfig["cns"];
                var da = new NpgsqlDataAdapter(sql, cnstr);
                da.SelectCommand.Parameters.AddWithValue("@table_name", table_name);
                var rec = new DataTable();
                da.Fill(rec);

                if (rec.Rows.Count == 0)
                {
                    return Json(new { id = "" });
                };
                if (rec.Rows[0]["column_default"].ToString() == "" && rec.Rows[0]["udt_name"].ToString() != "uuid")
                {
                    return Json(new { id = "" });
                };
                var c_default = rec.Rows[0]["column_default"].ToString();
                if (rec.Rows[0]["udt_name"].ToString() == "uuid")
                    c_default = "uuid_generate_v1()";
                sql = "select " + c_default + " id";
                da = new NpgsqlDataAdapter(sql, cnstr);
                var result = new DataTable();
                da.Fill(result);
                return Json(new { id = result.Rows[0]["id"] });
            }
            else
            {
                var sql = "select c.user_type_id from sys.tables t(nolock) inner join sys.columns c(nolock) on t.object_id = c.object_id where t.name = @tablename and column_id = 1";
                var cnstr = Program.AppConfig["mscns"];
                var da = new SqlDataAdapter(sql, cnstr);
                da.SelectCommand.Parameters.AddWithValue("@tablename", table_name);
                var rec = new DataTable();
                da.Fill(rec);
                if (rec.Rows.Count == 0)
                {
                    return Json(new { id = "" });
                };
                if ((int)rec.Rows[0][0] == 36)
                {
                    return Json(new { id = Guid.NewGuid().ToString() });
                }
                else
                {
                    return Json(new { id = "" });
                }
            }

        }

        #endregion


        private async Task Authenticate(string userName)
        {

            // создаем один claim
            var claims = new List<Claim> {
                new Claim (ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(id),
            new AuthenticationProperties  //запоминает пользователя
            {
                IsPersistent = true
            });
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/Home/Login");
        }
        public ActionResult Login()
        {
            DBClient dc = new DBClient();
            return View(dc);
        }

        [HttpPost]
        public async Task<ActionResult> Login(DBClient model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                DBClient dc = new DBClient();
                bool auth = dc.CheckLogon(model.UserName, model.Password);
                if (!auth)
                {
                    //Пробуем через LDAP
                    string ldap_server = Program.AppConfig["ldap_server"];
                    string ldap_root = Program.AppConfig["ldap_root"];
                    int ldap_port = int.Parse(Program.AppConfig["ldap_port"]);
                    string ldap_user = "cn=" + model.UserName + "," + ldap_root;
                    if (!string.IsNullOrEmpty(Program.AppConfig["domain"]))
                        ldap_user = Program.AppConfig["domain"] + @"\" + model.UserName;
                    try
                    {
                        LdapConnection ldapConn = new LdapConnection();
                        ldapConn.Connect(ldap_server, ldap_port);
                        ldapConn.Bind(ldap_user, model.Password);
                        auth = true;
                    }
                    catch
                    {; }
                }
                if (auth)
                {
                    await Authenticate(model.UserName); // аутентификация
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин или пароль");
                    return View();
                }
            }
            else
            {
                return View();
            }

        }

        public async Task<ActionResult> opendir(string cnt_sid, string id, string id64)
        {
            SqlDataAdapter da = new SqlDataAdapter("p_cntsession", Program.AppConfig["mscns"]);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@cnt_sid", cnt_sid);
            DataTable res = new DataTable();
            da.Fill(res);
            if (res.Rows.Count == 0)
            {
                return Redirect("~/Docfiles/dir?id=" + id + "&id64=" + id64);
            }
            else
            {
                string account = res.Rows[0]["username"].ToString();
                await Authenticate(account); // аутентификация
                return Redirect("~/Docfiles/dir?id=" + id + "&id64=" + id64);
            }
        }



        public async Task<ActionResult> openсomment(string cnt_sid, int ag_id, string ag_type)
        {
            SqlDataAdapter da = new SqlDataAdapter("p_cntsession", Program.AppConfig["mscns"]);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@cnt_sid", cnt_sid);
            DataTable res = new DataTable();
            da.Fill(res);
            if (res.Rows.Count == 0)
            {
                return Redirect("~/Access.html?ReturnUrl=/Docfiles/comments?ag_id=" + ag_id.ToString() + "&ag_type=" + ag_type);
            }
            else
            {
                string account = res.Rows[0]["username"].ToString();
                await Authenticate(account); // аутентификация
                return Redirect("~/Docfiles/comments?ag_id=" + ag_id.ToString() + "&ag_type=" + ag_type);
            }
        }


        [Route("/usmart/getcntinfo/{inn}")]
        public ActionResult getcntinfo(string inn)
        {

            dadataINN di = new dadataINN();
            string responseText = di.getjson(inn);
            return File(Encoding.UTF8.GetBytes(responseText), "application/json");

        }

    }

}