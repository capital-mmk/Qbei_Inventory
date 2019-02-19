using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _24東_アズマ_
{
    //public partial class _24_Tab_Browser : Form
    //{
    //    public _24_Tab_Browser()
    //    {
    //        InitializeComponent();
    //    }

    //    private void button3_Click(object sender, EventArgs e)
    //    {

    //    }
    //}

    public partial class _24_Tab_Browser : Form
    {
        //RoutedEventArgs f = new RoutedEventArgs();
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt024 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public WebBrowser wb_pri = new WebBrowser();
        public bool IsreadOnly = false;
        public static string st = string.Empty;
        int epoch;
        public _24_Tab_Browser()
        {
            InitializeComponent();
            //testflag();
            tabControl1.TabPages.Remove(tabPage2);
            webBrowser1.ScriptErrorsSuppressed = true;
            testflag();
            //button3_Click(new object(), new EventArgs());
            //this.button3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }
        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.google.com");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://www.facebook.com");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.c-sharpcorner.com/UploadFile/6e17c7/how-to-create-a-simple-multi-tabbed-webbrowser-in-C-Sharp/");

        }
        private void button3_Click(Object sender, EventArgs e)
        {
            TabPage tabpage = new TabPage();
            tabpage.Text = "Check Readonly";
            tabControl1.Controls.Add(tabpage);
            //int idx = tabControl1.TabPages.IndexOf(tabPage4); 
            //tabControl2.TabPages.Remove(tabPage4);
            //wb_pri.Parent
            //WebBrowser webbrowser = new WebBrowser();
            wb_pri.Parent = tabpage;
            wb_pri.Dock = DockStyle.Fill;
            //wb_pri.Navigate(this.webBrowser1.Url);
            //Thread.Sleep(3000);
            wb_pri.ScriptErrorsSuppressed = true;
            wb_pri.Navigate("https://ew.azuma-weborder.jp/product_detail/index/" + entity.orderCode);
            wb_pri.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(readonly_check);
            tabControl1.SelectedIndex = 1;
        }
        //protected void readonly_check(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    wb_pri.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(readonly_check);
        //    wb_pri.Navigate("https://ew.azuma-weborder.jp/product_detail/index/" + entity.orderCode);
        //    wb_pri.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(check_status);
        //}

        protected void readonly_check(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wb_pri.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(readonly_check);
            string html = wb_pri.Document.Body.InnerHtml;
            string test = string.Empty;
            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
            hdoc.LoadHtml(html);
            test = hdoc.GetElementbyId("formInputAmount0").OuterHtml;
            if (test.Contains("readonly"))
            {
                entity.stockDate = "2100-02-01";
            }
            else
            {
                entity.stockDate = "2100-01-01";
            }



            tabControl1.SelectedIndex = 0;
            tabControl1.TabPages.RemoveAt(1);

        }
        private void testflag()
        {
            //try
            //{
            //}
            //catch
            //{
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 24;
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(24);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {
                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(24);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
            //}
        }
        public void StartRun()
        {
            try
            {
                fun.setURL("024");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(24);
                fun.Qbei_ErrorDelete(24);
                dt024 = fun.GetDatatable("024");
                dt024 = fun.GetOrderData(dt024, "https://ew.azuma-weborder.jp/azuma/product_detail/multi_request?id=", "024", "");
                fun.GetTotalCount("024");
                ReadData();
            }
            catch (Exception ex) {
                fun.WriteLog(ex.Message,"024"); }
        }
        private void ReadData()
        {
            qe.SiteID = 24;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This page can't reach", entity.janCode, entity.orderCode, 6, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 24;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.WriteLog("Navigation to Site Url success------", "024-");
                webBrowser1.Document.GetElementById("MemberLoginId").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("MemberLoginPasswd").InnerText = password;
                fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex.Message + entity.orderCode, "024-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This site can't reach", entity.janCode, entity.orderCode, 6, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                webBrowser1.ScriptErrorsSuppressed = true;
                body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains(" 会員コード・パスワードは旧システムの得意先コード、パスワードをご入力ください"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                    fun.WriteLog("Login Failed", "024");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "024-");
                    epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                    string ordercode = dt024.Rows[i]["発注コード"].ToString().Trim();
                    webBrowser1.Navigate(fun.url + "/azuma/product_detail/multi_request?id=" + ordercode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex.Message + entity.orderCode, "024-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This site can't reach", entity.janCode, entity.orderCode, 6, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.ScriptErrorsSuppressed = true;
                string ordercode = dt024.Rows[i]["発注コード"].ToString().Trim();
                webBrowser1.Navigate(fun.url + "/azuma/product_detail/multi_request?id=" + ordercode);
                if (webBrowser1.Url.ToString().Contains("/product_detail/"))
                {
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                    string url = webBrowser1.Url.ToString();
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex.Message + entity.orderCode, "024-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (IsreadOnly)
            {

                fun.Qbei_Inserts(entity);
            }
            try
            {

                entity.orderCode = dt024.Rows[i]["発注コード"].ToString();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This site can't reach", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                webBrowser1.ScriptErrorsSuppressed = true;
                entity = new Qbei_Entity();
                entity.siteID = 24;
                entity.sitecode = "024";
                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                entity.partNo = dt024.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt024.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString().Trim();
                entity.purchaseURL = fun.url + "/azuma/product_detail/index/" + entity.orderCode;

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    //Check Element Exist or not
                    if (webBrowser1.Document.GetElementsByTagName("html")[0] == null)
                    {
                        fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                        fun.WriteLog("Access Denied! " + entity.orderCode, "024-");
                        Application.Exit();
                    }
                    body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    string Date = DateTime.Now.ToString("yyyy-MM-dd");

                    string qtyStatus = string.Empty;
                    string[] strarr = body.Split('|');
                    if (strarr[1] == "[0]")
                    {
                        entity.qtyStatus = "empty";
                        entity.price = dt024.Rows[i]["下代"].ToString();
                        if ((dt024.Rows[i]["在庫情報"].ToString().Contains("empty") || dt024.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && (dt024.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                        {
                            entity.stockDate = "2100-01-10";
                        }
                        else
                        { entity.stockDate = "2100-02-01"; }
                        fun.Qbei_Inserts(entity);
                    }

                    else
                    {
                        string url = webBrowser1.Document.Url.ToString();
                        string html = webBrowser1.Document.Body.InnerText;
                        string price = strarr[1];
                        price = price.Replace("[", string.Empty).Replace("]", string.Empty);
                        entity.price = price;
                        //string stockdateExist = strarr[5];
                        string dateexists = string.Empty;
                        qtyStatus = strarr[4];
                        qtyStatus = qtyStatus.Replace("[", string.Empty).Replace("]", string.Empty).Replace("=", string.Empty);
                        //entity.qtyStatus = qtyStatus.Equals("◎") || qtyStatus.Equals("○") ? "good" : qtyStatus.Equals("△") || qtyStatus.Equals("台|個|ロット") ? "smalll" : qtyStatus.Equals("×") || qtyStatus.Equals("予約受付中") || qtyStatus.Equals("入荷予定") ? "empty" : "unknown status";
                        if (qtyStatus.Contains("○") || qtyStatus.Contains("◎"))
                        {
                            entity.qtyStatus = "good";
                            dateexists = qtyStatus.Replace("○", string.Empty).Replace("◎", string.Empty);
                        }
                        else if (qtyStatus.Contains("△") || qtyStatus.Contains("台|個|ロット"))
                        {
                            entity.qtyStatus = "small";
                            dateexists = qtyStatus.Replace("△", string.Empty).Replace("台|個|ロット", string.Empty);
                        }
                        else if (qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中"))
                        {
                            entity.qtyStatus = "empty";
                            dateexists = qtyStatus.Replace("×", string.Empty).Replace("入荷予定", string.Empty).Replace("予約受付中", string.Empty);
                        }
                        else
                        {
                            entity.qtyStatus = "Unknown status";
                        }
                        if (dateexists != "")
                        {
                            entity.stockDate = dateexists;
                        }
                        else
                        {
                            entity.stockDate = qtyStatus.Equals("◎") || qtyStatus.Equals("○") || qtyStatus.Equals("△") || qtyStatus.Equals("台|個|ロット") || qtyStatus.Equals("×") || qtyStatus.Equals("予約受付中") || qtyStatus.Equals("入荷予定") ? "2100-01-01" : "unknown date";
                        }

                        if (entity.stockDate.Contains("2月"))
                        {
                            //entity.stockDate = "2018-02-28";
                            entity.stockDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("yyyy-MM-dd");
                        }

                        if ((dt024.Rows[i]["在庫情報"].ToString().Contains("empty") || dt024.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt024.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {

                            if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                            {
                                entity.qtyStatus = dt024.Rows[i]["在庫情報"].ToString();
                                entity.stockDate = dt024.Rows[i]["入荷予定"].ToString();
                                entity.price = dt024.Rows[i]["下代"].ToString();
                            }

                        }
                        if (entity.stockDate == "2100-01-01" && entity.qtyStatus == "empty")   // Condition for true
                        {
                            IsreadOnly = true;   //Changed Status for New tab Window
                            try
                            {
                                //button3.Click -= new EventHandler(button3_Click);
                                //button3.PerformClick();

                                //button3.Click += new EventHandler(void (sender,e) button3_Click);

                                //this.button3.Click += new EventHandler(this.button3_Click);
                                //button3.PerformClick();
                                button3_Click(new object(), new EventArgs());

                            }
                            catch
                            {

                                // button3.Click(new object(), new EventArgs());
                                //button3.Click(new object(), new EventArgs());
                            }

                        }
                        else
                        {
                            IsreadOnly = false;
                        }

                        //2017/12/22 End
                        if (!IsreadOnly)
                        {
                            fun.Qbei_Inserts(entity);
                        }



                    }
                }
                else
                {
                    IsreadOnly = false;
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }

                if (i < dt024.Rows.Count - 1)
                {
                    string ordercode = fun.ReplaceOrderCode(dt024.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "在庫処分/empty/", "/", "20161027ワイヤービードのため-/", "在庫処分/empry/-", "在庫処分/good/", "在庫処分empry-", "在庫処分empry", "在庫処分/empry/", "在庫処分good", "在庫処分/small/", "在庫処分small", "東特価のため完売/", "東特価のため完売", "未契約", "バラ注文できない為発注禁止/" });
                    ordercode = ordercode.Replace("東特価のため完売", string.Empty);
                    ordercode = ordercode.Replace("在庫処分small", string.Empty);
                    ordercode = ordercode.Replace("在庫処分good", string.Empty);
                    ordercode = ordercode.Replace("在庫処分empry", string.Empty);
                    ordercode = ordercode.Replace("在庫処分empry-", string.Empty);
                    webBrowser1.Navigate(fun.url + "/azuma/product_detail/multi_request?id=" + ordercode);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 24;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex.Message + entity.orderCode, "024-");
            }
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Access Denied!", dt024.Rows[i]["JANコード"].ToString(), dt024.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
            fun.WriteLog(StatusCode.ToString() + " " + dt024.Rows[i]["発注コード"].ToString(), "024-");
            Application.Exit();
        }
    }
}
