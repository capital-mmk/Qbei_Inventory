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
using System.Text.RegularExpressions;

namespace _57モトクロス
{
    public partial class frm057 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt057 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = -1;
        public static string st = string.Empty;
        public frm057()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 57;
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(57);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(57);
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
                fun.setURL("057");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(57);
                fun.Qbei_ErrorDelete(57);
                dt057 = fun.GetDatatable("057");
                dt057 = fun.GetOrderData(dt057, "http://www.mxweborder.com/Member/SyohinSearch.aspx", "057", "");
                fun.GetTotalCount("057");
                ReadData();
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 57;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(3000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);

            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "057-");
                qe.SiteID = 57;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "txttokuisakicode", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "txtloginpasswd", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "btnLogin", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), ex.Message, dt057.Rows[0]["JANコード"].ToString(), dt057.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
                fun.WriteLog(ex.Message, "057-");
                Application.Exit();
            }
        }
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            webBrowser1.DocumentCompleted -= webBrowser1_Login;
            webBrowser1.ScriptErrorsSuppressed = true;
            string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
            if (body.Contains("パスワードが必要です。") || body.Contains("得意先コードに誤りがあります。再入力してください。") || body.Contains("得意先コードが必要です"))
            {
                fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
                fun.WriteLog("Login Failed", "057-");
                Application.Exit();
            }
            else
            {
                fun.WriteLog("Login success             ------", "057-");
                webBrowser1.Navigate(fun.url + "/Member/SyohinSearch.aspx");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
        }
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(fun.url + "/Member/SyohinSearch.aspx");
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
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
                if (i < dt057.Rows.Count - 1)
                {
                    string orderCode = dt057.Rows[++i]["発注コード"].ToString();
                    //string orderCode = "2000020831";
                    doc.GetElementById("ctl00$MainContent$txtfreeword").SetAttribute("Value", orderCode);
                    webBrowser1.Document.GetElementById("ctl00$MainContent$btnSearch").InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 57;
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
                fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), ex.Message, dt057.Rows[i]["JANコード"].ToString(), dt057.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
                fun.WriteLog(ex.Message + dt057.Rows[i]["発注コード"].ToString(), "057-");
                Application.Exit();
            }

        }
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 57;
            entity.sitecode = "057";
            entity.janCode = dt057.Rows[i]["JANコード"].ToString();
            entity.partNo = dt057.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt057.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt057.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/Member/SyohinSearch.aspx";
            string strKubun = string.Empty;
            string html;
            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                string count = webBrowser1.Document.GetElementById("MainContent_lblkensu").InnerText;
                if (count.Equals("0"))
                {
                    if (dt057.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt057.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt057.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt057.Rows[i]["下代"].ToString();
                    }
                    fun.Qbei_Inserts(entity);
                }
                else
                {
                    try
                    {

                        if ((fun.GetElement("span", "MainContent_gvSyohin_lblzaikojokyo_0", "ID", webBrowser1) == null) && (fun.GetElement("span", "MainContent_gvSyohin_lblnyukayotei_0", "ID", webBrowser1) == null))
                        {
                            fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "057--");
                            Application.Exit();
                        }
                        else
                        {
                            //2018-05-11 Start
                            html = webBrowser1.Document.GetElementById("MainContent_gvSyohin").InnerHtml;
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(html);
                            strKubun = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[2]/td[7]") == null ? string.Empty : hdoc.DocumentNode.SelectSingleNode("/tbody/tr[2]/td[7]").InnerText;
                            //2018-05-11 End
                            entity.stockDate = fun.GetElement("span", "MainContent_gvSyohin_lblnyukayotei_0", "ID", webBrowser1).InnerText;
                            entity.stockDate = entity.stockDate == null ? string.Empty : entity.stockDate;
                            string qty = fun.GetElement("span", "MainContent_gvSyohin_lblzaikojokyo_0", "ID", webBrowser1).InnerText;

                            entity.qtyStatus = qty.Contains("○") ? "good" : qty.Contains("△") || fun.IsNumber(qty) ? "small" : qty.Contains("×") ? "empty" : qty.Contains("☆") ? "inquiry" : "unknown status";
                            entity.price = fun.GetElement("span", "MainContent_gvSyohin_lbltanka_0", "ID", webBrowser1).InnerText.Replace("円", string.Empty).Replace(",", string.Empty);
                            string day = string.Empty;
                            string month = string.Empty;
                            string year = string.Empty;

                            if (qty.Contains("×"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            else if (entity.stockDate.Equals("-") || string.IsNullOrWhiteSpace(entity.stockDate))
                            {
                                if (!String.IsNullOrEmpty(strKubun) && strKubun.Contains("売り切り"))
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                else
                                    entity.stockDate = qty.Contains("○") || qty.Contains("△") || fun.IsNumber(qty) || qty.Contains("☆") ? "2100-01-01" : "unknown date";
                            }

                            else if (qty.Contains("○") || qty.Contains("△") || fun.IsNumber(qty) || qty.Contains("☆"))
                            {
                                if (entity.stockDate.Contains("ごろ"))
                                {
                                    day = "30";
                                    string[] ae = entity.stockDate.Split('～');
                                    month = Regex.Replace(ae[1], "[^0-9]+", string.Empty);
                                    string[] aee = ae[0].Split('年');
                                    year = Regex.Replace(aee[0], "[^0-9]]+", string.Empty);
                                    DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                    string d = fun.getCurrentDate();
                                    if (dt < Convert.ToDateTime(d))
                                        dt = dt.AddYears(1);

                                    entity.stockDate = dt.ToString("yyyy-MM-dd");
                                }
                                else if (entity.stockDate.Contains("翌"))
                                {
                                    day = "30";
                                    string[] ae = entity.stockDate.Split('～');
                                    month = Regex.Replace(ae[1], "[^0-9]+", string.Empty);
                                    year = DateTime.Now.ToString("yyyy");
                                    entity.stockDate = year + "-" + month + "-" + day;
                                }
                                else
                                {
                                    if (entity.stockDate.Contains("完売"))
                                        entity.stockDate = "2100-02-01";
                                    else if (entity.stockDate.Contains("旬") || entity.stockDate.Contains("月"))
                                    {

                                        if (entity.stockDate.Contains("年"))
                                        {
                                            string[] arr = entity.stockDate.Split('年');
                                            month = Regex.Replace(arr[1], "[^0-9]+", string.Empty);
                                            year = Regex.Replace(arr[0], "[^0-9]+", string.Empty);
                                            if (month == "2")
                                                day = "28";
                                            else
                                                day = "30";

                                        }
                                        else
                                        {

                                            month = Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty);

                                            if (entity.stockDate.Contains("上旬"))
                                                day = "10";
                                            else if (entity.stockDate.Contains("中旬"))
                                                day = "20";
                                            year = DateTime.Now.ToString("yyyy");
                                        }


                                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                        string d = fun.getCurrentDate();
                                        if (dt < Convert.ToDateTime(d))
                                            dt = dt.AddYears(1);

                                        entity.stockDate = dt.ToString("yyyy-MM-dd");
                                    }
                                }
                            }
                            else
                            {
                                entity.stockDate = "unknown date";
                            }

                            if ((dt057.Rows[i]["在庫情報"].ToString().Contains("empty") || dt057.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt057.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-01-30")) && (entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry")))))
                                {
                                    entity.qtyStatus = dt057.Rows[i]["在庫情報"].ToString();
                                    entity.price = dt057.Rows[i]["下代"].ToString();
                                    entity.stockDate = dt057.Rows[i]["入荷予定"].ToString();
                                }
                                fun.Qbei_Inserts(entity);
                            }
                            else
                                fun.Qbei_Inserts(entity);

                        }
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
                        fun.WriteLog(ex.Message + entity.orderCode, "057-");
                        Application.Exit();
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
            }
            //i++;
            webBrowser1.Navigate(fun.url + "/Member/SyohinSearch.aspx");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
        }
       
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(57, fun.GetSiteName("057"), "Access Denied!", dt057.Rows[i]["JANコード"].ToString(), dt057.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "057");
            fun.WriteLog(StatusCode.ToString() + " " + dt057.Rows[i]["発注コード"].ToString(), "057--");
            Application.Exit();
        }
    }
}