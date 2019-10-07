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

namespace _124
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm124 : Form
    {
        DataTable dt = new DataTable();

        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt124 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i =0;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm124()
        {
            InitializeComponent();
            testflag();
        }

        /// <summary>
        /// testflag processing.
        /// </summary>
        ///<remark>
        ///"0,1,2"Flage Number of Check. 
        ///"0" is Start Process.
        ///"1" is Processing.
        ///"2" is End Process.
        ///</remark>
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 124;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(124);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());

                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                /// Check to flag is "0" or "1" or "2".
                /// when flag is 0,Change to flag is 1 and Continue to StartRun Process.
                /// </remark>
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (flag == 1)
                {
                    fun.deleteData(124);
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
                fun.WriteLog(ex, "124-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        public void StartRun()
        {
            try
            {
                fun.setURL("124");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(124);
                fun.Qbei_ErrorDelete(124);
                dt124 = fun.GetDatatable("124");
                dt124 = fun.GetOrderData(dt124, "https://www.ordermz.jp/weborder/SyohinSearch.aspx", "124", " ");
                fun.GetTotalCount("124");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "124-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Read to Data and Url.
        /// </remark>
        private void ReadData()
        {
            qe.SiteID = 124;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();
                qe.sitecode = "124";
                webBrowser1.ScriptErrorsSuppressed = true;
                fun.WriteLog("Navigation to Site Url success------", "124-" );
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("tokuisakicode").InnerText = username;
         
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("loginpasswd").InnerText = password;
                webBrowser1.Document.GetElementById("btnLogin").InvokeMember("Click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt124.Rows[0]["JANコード"].ToString();
                string orderCode = dt124.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                fun.WriteLog(ex, "124-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("このサイトに関するお問い合わせは"))
                {
                    fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");                    
                    fun.WriteLog("Login Failed", "124-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "124-");
                    webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt124.Rows[0]["JANコード"].ToString();
                string orderCode = dt124.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                fun.WriteLog(ex, "124-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }

        /// <summary>
        /// Inspection of item at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;
                if (i < dt124.Rows.Count)
                {
                    string orderCode = dt124.Rows[i]["発注コード"].ToString();
                    doc.GetElementById("keyword").SetAttribute("Value", orderCode);
                    webBrowser1.Document.GetElementById("btnSearch").InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 124;
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
                string janCode = dt124.Rows[i]["JANコード"].ToString();
                string orderCode = dt124.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                fun.WriteLog(ex, "124-", janCode, orderCode);

                ++i;
                webBrowser1.Document.GetElementById("btnClear").InvokeMember("Click");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            entity = new Qbei_Entity();
            entity.janCode = dt124.Rows[i]["JANコード"].ToString();
            entity.partNo = dt124.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt124.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt124.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/SyohinSearch.aspx" + entity.orderCode;
            string url = webBrowser1.Url.ToString();
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            entity.siteID = 124;
            entity.sitecode = "124";
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);

            if (body.Contains("｢キーワード｣に入力して探してみてください。"))
            {
                if (dt124.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt124.Rows[i]["在庫情報"].ToString().Contains("empty"))
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-01-10";
                    entity.price = dt124.Rows[i]["下代"].ToString();
                }
                else
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";
                    entity.price = dt124.Rows[i]["下代"].ToString();
                }
                fun.Qbei_Inserts(entity);
            }
            else
            {
                try
                {
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);

                    string qty = webBrowser1.Document.GetElementById("GridView1_ctl02_Label1").InnerText;
                    entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";

                    entity.price = webBrowser1.Document.GetElementById("GridView1_ctl02_hanbaikakaku").InnerText;
                    entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);

                    HtmlNode node = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]");
                    string stockDate = string.Empty;
                    if (node != null)
                    stockDate = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]").InnerText;
                    HtmlNode colorpath = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[5]");
                    string color = colorpath.GetAttributeValue("style", string.Empty);
                    entity.stockDate = color.Contains("red") ? "2100-02-01" : (stockDate == "-" || string.IsNullOrWhiteSpace(stockDate) || stockDate.Equals("&nbsp;")) ? "2100-01-01" : stockDate;
                    if (entity.stockDate.Contains("予定"))
                    {
                        entity.stockDate = entity.stockDate.Replace("次回", "").Replace("入荷", "");
                        string day = string.Empty;
                        if (entity.stockDate.Contains("中旬"))
                            day = "15";
                        else if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("月予定"))
                            day = "01";
                        else if (entity.stockDate.Contains("下旬"))
                        {
                            if (entity.stockDate.Contains("2月"))

                                day = "28";
                            day = "20";
                        }
                        else day = "25";

                        string month = entity.stockDate.Split('月')[0];

                        string year = DateTime.Now.ToString("yyyy");

                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                        if (dt < DateTime.Now)
                            dt = dt.AddYears(1);

                        entity.stockDate = dt.ToString("yyyy-MM-dd");
                    }
                    else if (entity.stockDate.Contains("月末～"))
                    {
                        entity.stockDate = "未定(=2100-01-01)";
                    }
                    else if (entity.stockDate.Contains("月末"))
                    {
                        string day = "25";
                        string month = entity.stockDate.Replace("月末", string.Empty);
                        string year = DateTime.Now.ToString("yyyy");

                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                        if (dt < DateTime.Now)
                            dt = dt.AddYears(1);

                        entity.stockDate = dt.ToString("yyyy-MM-dd");
                    }
                    else if (entity.stockDate.Contains("未定"))
                    {
                        entity.stockDate = "2100-01-01";
                    }
                    entity.stockDate = entity.stockDate.Replace("/", "-");
                    if (entity.stockDate.Contains("2月"))
                    {
                        entity.stockDate = "2018-02-28";
                    }

                    if ( dt124.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                        {
                            entity.qtyStatus = dt124.Rows[i]["在庫情報"].ToString();
                            entity.price = dt124.Rows[i]["下代"].ToString();
                            entity.stockDate = dt124.Rows[i]["入荷予定"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }
                    else
                        fun.Qbei_Inserts(entity);                    
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");                    
                    fun.WriteLog(ex, "124-", entity.janCode, entity.orderCode);
                }
            }

            i++;

            webBrowser1.Document.GetElementById("btnClear").InvokeMember("Click");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
        }

        private void frm124_Load(object sender, EventArgs e)
        {

        }
    }
}