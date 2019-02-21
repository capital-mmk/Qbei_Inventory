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
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _65野口
{
    public partial class frm065 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt065 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        public static string st = string.Empty;
        string Date = string.Empty;
        int i = -1;
        public frm065()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 65;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(65);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(65);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public void StartRun()
        {
            try
            {
                fun.setURL("065");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(65);
                fun.Qbei_ErrorDelete(65);
                Date = fun.getCurrentDate();
                dt065 = fun.GetDatatable("065");
                dt065 = fun.GetOrderData(dt065, "http://www.noguchi-shokai.co.jp/nachos/site/item_list", "065", "");
                fun.GetTotalCount("065");
                if (dt065 == null)
                {
                    qe.site = 65;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Environment.Exit(0);
                    Application.Exit();
                }
                fun.GetTotalCount("065");
                ReadData();
            }
            catch (Exception) { }
        }

        private void ReadData()
        {
            qe.SiteID = 65;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
            webBrowser1.Navigate(fun.url + "/nachos/account/login");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.WriteLog("Navigation to Site Url success------", "065-");
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                qe.SiteID = 65;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("login").InnerText = username;

                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("password").InnerText = password;
                fun.GetElement("input", "commit", "name", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), ex.Message, dt065.Rows[0]["JANコード"].ToString(), dt065.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                fun.WriteLog(ex.Message, "065-");
                Application.Exit();
            }
        }
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= webBrowser1_Login;
            webBrowser1.ScriptErrorsSuppressed = true;
            string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
            if (body.Contains("野口商会Web発注システム ログイン画面"))
            {
                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "Login Failed", dt065.Rows[0]["JANコード"].ToString(), dt065.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                fun.WriteLog("Login Failed", "065-");
                Application.Exit();
            }
            else
            {
                fun.WriteLog("Login success             ------", "065-");
                webBrowser1.Navigate(fun.url + "/nachos/site/item_list");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
        }
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(fun.url + "/nachos/site/item_list");
            if (webBrowser1.Url.ToString().Contains("item_list"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }
        private void webBrowser1_Search(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                webBrowser1.ScriptErrorsSuppressed = true;

                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;
                if (i < dt065.Rows.Count - 1)
                {
                    string orderCode = dt065.Rows[++i]["発注コード"].ToString();
                    doc.GetElementById("item_code").SetAttribute("Value", orderCode);
                    fun.GetElement("input", "search_button", "value", webBrowser1).InvokeMember("click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                }
                else
                {
                    qe.site = 65;
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
                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), ex.Message, dt065.Rows[i]["JANコード"].ToString(), dt065.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                fun.WriteLog(ex.Message + dt065.Rows[i]["発注コード"].ToString(), "065-");
                Application.Exit();
            }
        }
        private void webBrowser1_ItemClick(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                webBrowser1.ScriptErrorsSuppressed = true;
                entity = new Qbei_Entity();
                entity.siteID = 65;
                entity.sitecode = "065";
                entity.janCode = dt065.Rows[i]["JANコード"].ToString();
                entity.partNo = dt065.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt065.Rows[i]["最終反映日"].ToString();
                entity.purchaseURL = dt065.Rows[i]["purchaserURL"].ToString();
                entity.orderCode = dt065.Rows[i]["発注コード"].ToString();

                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerHtml;
                if (body.Contains("total: 1"))
                {
                    fun.GetElement("img", "おすすめ", "alt", webBrowser1).InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    if (dt065.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt065.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt065.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt065.Rows[i]["下代"].ToString();
                    }
                    fun.Qbei_Inserts(entity);
                    webBrowser1.Navigate(fun.url + "/nachos/site/item_list");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), ex.Message, dt065.Rows[i]["JANコード"].ToString(), dt065.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                fun.WriteLog(ex.Message + dt065.Rows[i]["発注コード"].ToString(), "065-");
                Application.Exit();
            }

        }
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //entity.purchaseURL = webBrowser1.Url.ToString();
            //if(!entity.purchaseURL.Contains("back_to=%2Fnachos%2Fsite%2Fitem_list")) {return;}

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            webBrowser1.ScriptErrorsSuppressed = true;
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerHtml;
            string url = (webBrowser1.Document.Url).ToString();
            try
            {
                string html = webBrowser1.Document.Body.InnerHtml;
                HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                hdoc.LoadHtml(html);
                if ((hdoc.DocumentNode.SelectSingleNode("div[1]/div[3]/table/tbody/tr[1]/td[1]/table/tbody/tr[7]/td") == null))
                {
                    fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                    fun.WriteLog("Access Denied! " + entity.orderCode, "065--");
                    Application.Exit();
                }
                else
                {
                    entity.purchaseURL = webBrowser1.Url.ToString();
                    string qtypath = hdoc.DocumentNode.SelectSingleNode("div[1]/div[3]/table/tbody/tr[1]/td[1]/table/tbody/tr[7]/td").InnerText;
                    entity.qtyStatus = qtypath.Equals("○") ? "good" : qtypath.Equals("△") ? "small" : qtypath.Equals("×") ? "empty" : qtypath.Equals("取寄せ商品") ? "inquiry" : "invalid status code";
                    entity.stockDate = qtypath.Equals("○") || qtypath.Equals("△") || qtypath.Equals("×") || qtypath.Equals("取寄せ商品") ? "2100-01-01" : "unknown date";

                    entity.price = hdoc.DocumentNode.SelectSingleNode("div[1]/div[3]/table/tbody/tr[1]/td[1]/table/tbody/tr[6]/td").InnerText;
                    entity.price = entity.price.Replace("オープン価格", string.Empty);
                    entity.price = entity.price.Replace("円", string.Empty);
                    entity.price = entity.price.Replace(",", string.Empty);
                    if (entity.stockDate.Contains("2月"))
                    {
                        //entity.stockDate = "2018-02-28";
                        entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                    }
                    if ((dt065.Rows[i]["在庫情報"].ToString().Contains("empty") || dt065.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt065.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                        {
                            entity.qtyStatus = dt065.Rows[i]["在庫情報"].ToString();
                            entity.price = dt065.Rows[i]["下代"].ToString();
                            entity.stockDate = dt065.Rows[i]["入荷予定"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }

                    else
                        fun.Qbei_Inserts(entity);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                fun.WriteLog(ex.Message + entity.orderCode, "065-");
                Application.Exit();
            }
            webBrowser1.Navigate(fun.url + "/nachos/site/item_list");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "Access Denied!", dt065.Rows[i]["JANコード"].ToString(), dt065.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
            fun.WriteLog(StatusCode.ToString() + " " + dt065.Rows[i]["発注コード"].ToString(), "065--");
            Application.Exit();
        }
    }
}

