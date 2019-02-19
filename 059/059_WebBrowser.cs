using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Threading;

namespace _059
{
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt059 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        public Form1()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            qe.starttime = DateTime.Now.ToString();
            qe.site = 59;
            st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(59);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(59);
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
                fun.setURL("059");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(59);
                fun.Qbei_ErrorDelete(59);
                dt059 = fun.GetDatatable("059");
                dt059 = fun.GetOrderData(dt059, "https://jpsg.bcart.jp/product.php?id=7", "059", "");
                fun.GetTotalCount("059");
                ReadData();

            }
            catch (Exception) { }
        }

        private void ReadData()
        {

            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 59;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url + "/login.php");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        //private void webBrowser1_Logout(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    try
        //    {
        //        string url = webBrowser1.Url.ToString();
        //        string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
        //        fun.GetElement("a", "https://jpsg.bcart.jp/logout.php", "href", webBrowser1).InvokeMember("click");

        //        webBrowser1.DocumentCompleted -= webBrowser1_Logout;
        //        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        //    }
        //    catch (Exception ex)
        //    {
        //        fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), ex.Message, dt059.Rows[0]["JANコード"].ToString(), dt059.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
        //        fun.WriteLog(ex.Message + dt059.Rows[0]["発注コード"].ToString(), "059-");
        //        Application.Exit();
        //        Environment.Exit(0);
        //    }
        //}

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string url = webBrowser1.Url.ToString();
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (url.Equals("https://jpsg.bcart.jp/login.php"))
                {

                    SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                    instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                    url = webBrowser1.Url.ToString();
                    fun.WriteLog("Navigation to Site Url success------", "059-");
                    webBrowser1.ScriptErrorsSuppressed = true;
                    qe.SiteID = 59;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string username = dt.Rows[0]["UserName"].ToString();
                    fun.GetElement("input", "loginEmail", "name", webBrowser1).InnerText = username;
                    string password = dt.Rows[0]["Password"].ToString();
                    fun.GetElement("input", "loginPassword", "name", webBrowser1).InnerText = password;
                    fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");

                    webBrowser1.DocumentCompleted -= webBrowser1_Start;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                }
                else if (url.Equals("https://jpsg.bcart.jp/"))
                {
                    //  string ordercode = "1104-10500-03";
                    string ordercode = dt059.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/list.php?keyword=" + ordercode);
                    webBrowser1.DocumentCompleted -= webBrowser1_Start;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), ex.Message, dt059.Rows[0]["JANコード"].ToString(), dt059.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                fun.WriteLog(ex.Message + dt059.Rows[0]["発注コード"].ToString(), "059-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("入力情報に誤りがあります。入力された内容を再度ご確認ください ") || body.Contains("入力内容が間違っているか、許可されていません。"))
                {
                    fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "059-");
                    string ordercode = dt059.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/list.php?keyword=" + ordercode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), ex.Message, dt059.Rows[0]["JANコード"].ToString(), dt059.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                fun.WriteLog(ex.Message + dt059.Rows[i]["発注コード"].ToString(), "059-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_ItemClick(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                entity.siteID = 59;
                entity.sitecode = "059";
                entity.janCode = dt059.Rows[i]["JANコード"].ToString();
                entity.partNo = dt059.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt059.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt059.Rows[i]["発注コード"].ToString();
                //entity.orderCode = "1104-10500-03";
                string html = webBrowser1.Document.Body.InnerHtml;
                HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                hdoc.LoadHtml(html);
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("該当する商品が登録されていません。"))
                {
                    if (dt059.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt059.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt059.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt059.Rows[i]["下代"].ToString();
                    }
                    entity.purchaseURL = webBrowser1.Url.ToString();
                    fun.Qbei_Inserts(entity);
                    if (i < dt059.Rows.Count - 1)
                    {
                        webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                        string ordercode = dt059.Rows[++i]["発注コード"].ToString();
                        webBrowser1.Navigate(fun.url + "/list.php?keyword=" + ordercode);
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                    }
                    else
                    {
                        qe.site = 59;
                        qe.flag = 2;
                        qe.endtime = DateTime.Now.ToString();
                        fun.ChangeFlag(qe);
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    string url = webBrowser1.Url.ToString();
                    body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                    webBrowser1.Document.GetElementsByTagName("p")[0].InvokeMember("click");
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), ex.Message, dt059.Rows[i]["JANコード"].ToString(), dt059.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                fun.WriteLog(ex.Message + dt059.Rows[i]["発注コード"].ToString(), "065-");
                Application.Exit();
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            string url = webBrowser1.Url.ToString();
            try
            {
                if ((webBrowser1.Document.GetElementsByTagName("td")[3].InnerText == null) && (webBrowser1.Document.GetElementsByTagName("td")[4].InnerText == null) && (webBrowser1.Document.GetElementsByTagName("td")[5].InnerText == null))
                {
                    fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                    fun.WriteLog("Access Denied! " + entity.orderCode, "059-");
                    Application.Exit();
                }
                else
                {
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    //entity.price = webBrowser1.Document.GetElementsByTagName("td")[4].InnerText;
                    //string qtypath = webBrowser1.Document.GetElementsByTagName("td")[5].InnerText;
                    //if (!entity.price.Contains("円"))
                    //{
                    //    entity.price = webBrowser1.Document.GetElementsByTagName("td")[3].InnerText;
                    //    qtypath = webBrowser1.Document.GetElementsByTagName("td")[4].InnerText;
                    //}
                    entity.price = hdoc.DocumentNode.SelectSingleNode("//table[@id='product-set-table']/tbody/tr[1]/td[2]").InnerText;
                    string qtypath = hdoc.DocumentNode.SelectSingleNode("//table[@id='product-set-table']/tbody/tr[1]/td[3]").InnerText;

                    string[] price = entity.price.Split('円');
                    entity.price = price[0].Replace(",", string.Empty);

                    qtypath = qtypath.Replace("\"", string.Empty).Replace(" ", string.Empty);
                    entity.qtyStatus = qtypath.Contains("○") ? "good" : qtypath.Contains("△") ? "small" : qtypath.Contains("×") || qtypath.Contains("現在在庫切れ") || qtypath.Contains("現在 在庫切れ") ? "empty" : qtypath.Contains("取寄せ商品") ? "inquiry" : "invalid status code";
                    entity.stockDate = qtypath.Contains("○") || qtypath.Contains("△") || qtypath.Contains("×") ? "2100-01-01" : qtypath.Contains("現在在庫切れ") || qtypath.Contains("現在 在庫切れ") ? "2100-02-01" : "unknown date";
                    entity.purchaseURL = webBrowser1.Url.ToString();

                    if ((dt059.Rows[i]["在庫情報"].ToString().Contains("empty") || dt059.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt059.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                        {
                            entity.qtyStatus = dt059.Rows[i]["在庫情報"].ToString();
                            entity.price = dt059.Rows[i]["下代"].ToString();
                            entity.stockDate = dt059.Rows[i]["入荷予定"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }

                    else
                        fun.Qbei_Inserts(entity);
                }

            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
                fun.WriteLog(ex.Message, "059-");
                Application.Exit();
            }


            if (i < dt059.Rows.Count - 1)
            {
                string ordercode = dt059.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/list.php?keyword=" + ordercode);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemClick);
            }
            else
            {
                qe.site = 59;
                qe.flag = 2;
                qe.endtime = DateTime.Now.ToString();
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(59, fun.GetSiteName("059"), "Access Denied!", dt059.Rows[i]["JANコード"].ToString(), dt059.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "059");
            fun.WriteLog(StatusCode.ToString() + " " + dt059.Rows[i]["発注コード"].ToString(), "059-");
            Application.Exit();
        }
    }
}

