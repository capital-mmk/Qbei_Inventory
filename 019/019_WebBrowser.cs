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

namespace _019深谷_フカヤ_
{
    public partial class frm019 : Form
    {
        //データーを　呼び出し。
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt019 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        //システム(Start)。
        public frm019()
        {
            InitializeComponent();
            testflag();
        }

        //Flagの　チャック。
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 19;
                //st = qe.starttime;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(19);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(19);
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        //サイト　や　データーtableの　検査と処理。
        public void StartRun()
        {
            try
            {
                fun.setURL("019");
                fun.Qbei_Delete(19);
                fun.CreateFileAndFolder();
                fun.Qbei_ErrorDelete(19);
                dt019 = fun.GetDatatable("019");
                dt019 = fun.GetOrderData(dt019, "http://weborder.fukaya-nagoya.jp/Fukaya_Web2/FKWO0080.aspx?mode=1&code=", "019", "");
                fun.GetTotalCount("019");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        //サイトのデーターを　読み出し。
        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 19;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();

            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url + "/Fukaya_Web2/FKWO0010.aspx");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        //Mallの　ログイン。
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "019-");
                qe.sitecode = "019";
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_txtLOGIN").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_txtPassword").InnerText = password;
                fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
               // Thread.Sleep(5000);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt019.Rows[0]["JANコード"].ToString();
                string orderCode = dt019.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                fun.WriteLog(ex, "019-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        //ログインの　チャック。
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string url = webBrowser1.Url.ToString();
                webBrowser1.ScriptErrorsSuppressed = true;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("IDまたはパスワードが違います") || body.Contains("画面を終了し、再度ログインを実行してください。"))
                {
                    fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Login Failed", dt019.Rows[0]["JANコード"].ToString(), dt019.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                    fun.WriteLog("Login Failed", "019-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "019-");
                    //string orderCode = "IFDM9025D6";
                    orderCode = dt019.Rows[i]["発注コード"].ToString().Trim();
                    webBrowser1.Navigate(fun.url + "/Fukaya_Web2/FKWO0080.aspx?mode=1&code=" + orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt019.Rows[0]["JANコード"].ToString();
                orderCode = dt019.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                fun.WriteLog(ex, "019-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        //Mallに　項目を検査。
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
           // string orderCode = "IFDM9025D6";
            string orderCode = dt019.Rows[i]["発注コード"].ToString().Trim();
            webBrowser1.Navigate(fun.url + "/Fukaya_Web2/FKWO0080.aspx?mode=1&code=" + orderCode);
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            if (webBrowser1.Url.ToString().Contains("FKWO0080.aspx"))
            {
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
        }

        //Mallに　項目情報の検査。
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 19;
            entity.sitecode = "019";
            entity.janCode = dt019.Rows[i]["JANコード"].ToString();
            entity.partNo = dt019.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt019.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt019.Rows[i]["発注コード"].ToString().Trim();
            //entity.orderCode = "IFDM9025D6";
            entity.purchaseURL = fun.url + "/Fukaya_Web2/FKWO0080.aspx?mode=1&code=" + entity.orderCode;

            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;

            if (body.Contains("対象のデータは存在しませんでした。") || body.Contains("画面を終了し、再度ログインを実行してください"))
            {
                entity.qtyStatus = "empty";
                entity.price = dt019.Rows[i]["下代"].ToString();
                if ((dt019.Rows[i]["在庫情報"].ToString().Contains("empty") && dt019.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                {
                    entity.stockDate = "2100-01-10";
                }
                else
                { entity.stockDate = "2100-02-01"; }
                fun.Qbei_Inserts(entity);
            }
            else
            {
                try
                {
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    entity.qtyStatus = string.Empty;
                    var qty = "";
                    //Check Element Exist or not
                    if (webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_ImageZako") == null && (webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_txtHanbaiPrice") == null))
                    {
                        fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "019-");
                        
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    try
                    {
                        var div1 = webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_ImageZako");
                        qty = div1.GetAttribute("src");
                        if (qty.Contains("image/ZAIKO01.png"))
                            entity.qtyStatus = "good";
                        else if (qty.Contains("image/ZAIKO02.png"))
                            entity.qtyStatus = "small";
                        else if (qty.Contains("HAIBAN.png"))
                        {
                            string rem = webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_lblZaiko").InnerText;
                            rem = rem.Replace("(残：", string.Empty);
                            rem = rem.Replace(")", string.Empty);
                            if (Convert.ToInt16(rem) > 0)
                                entity.qtyStatus = "small";
                            else if (Convert.ToInt16(rem) == 0)
                                entity.qtyStatus = "empty";
                        }
                        else if (qty.Contains("image/ZAIKO03.png") || qty.Contains("image/HAIBAN.png"))
                            entity.qtyStatus = "empty";
                        else
                        {
                            entity.qtyStatus = "unknown status";
                        }
                        entity.stockDate = (qty.Contains("image/ZAIKO01.png") || qty.Contains("image/ZAIKO02.png") || qty.Contains("image/ZAIKO03.png")) ? "2100-01-01" : qty.Contains("HAIBAN.png") ? "2100-02-01" : "unknown date";
                        entity.price = webBrowser1.Document.GetElementById("ctl00_ContentPlaceHolder2_txtHanbaiPrice").InnerText.Replace(",", string.Empty);

                    }
                    catch
                    {
                        //fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                        entity.qtyStatus = "empty";
                        entity.price = dt019.Rows[i]["下代"].ToString();
                        entity.stockDate = "2100-02-01";
                        fun.Qbei_Inserts(entity);
                    }

                    if ((dt019.Rows[i]["在庫情報"].ToString().Contains("empty")||(dt019.Rows[i]["在庫情報"].ToString().Contains("inquiry"))) && dt019.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                        {
                            entity.qtyStatus = dt019.Rows[i]["在庫情報"].ToString();
                            entity.stockDate = dt019.Rows[i]["入荷予定"].ToString();
                            entity.price = dt019.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }
                    else
                        fun.Qbei_Inserts(entity);
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                    fun.WriteLog(ex, "019-", entity.janCode, entity.orderCode);
                }
            }
            if (i < dt019.Rows.Count - 1)
            {
                string orderCode = dt019.Rows[++i]["発注コード"].ToString().Trim();
                webBrowser1.Navigate(fun.url + "/Fukaya_Web2/FKWO0080.aspx?mode=1&code=" + orderCode);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 19;
                qe.flag = 2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        //NavigateErrorの　表示。
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt019.Rows[i]["JANコード"].ToString();
            string orderCode = dt019.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "019-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
