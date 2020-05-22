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

namespace _46トライスポーツ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm046 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt046 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm046()
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
                qe.site = 46;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(46);
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
                    fun.deleteData(46);
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
                fun.WriteLog(ex, "046-");
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
                fun.setURL("046");
                fun.Qbei_Delete(46);
                fun.CreateFileAndFolder();
                fun.Qbei_ErrorDelete(46);
                dt046 = fun.GetDatatable("046");
                dt046 = fun.GetOrderData(dt046, "http://www.trisportsdesu.com/products/list.php?mode=search&category_id=&name=", "046", "");
                fun.GetTotalCount("046");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "046-");
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
            qe.SiteID = 46;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
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

                fun.WriteLog("Navigation to Site Url success------", "046-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 46;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "login_email", "name", webBrowser1).InnerText = username;

                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "login_pass", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "ログイン", "alt", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt046.Rows[0]["JANコード"].ToString();
                string orderCode = dt046.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                fun.WriteLog(ex, "046-", janCode, orderCode);

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
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                webBrowser1.ScriptErrorsSuppressed = true;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                //<remark Jancode,ordercode 編集。2020/03/06 Start>
                entity.janCode = dt046.Rows[i]["JANコード"].ToString();
                entity.orderCode = dt046.Rows[i]["発注コード"].ToString();
                if (body.Contains("お客様IDもしくはパスワードが正しくありません。"))
                {
                    //entity.janCode = dt046.Rows[i]["JANコード"].ToString();
                    //entity.orderCode = dt046.Rows[i]["発注コード"].ToString();
                    //</remark Jancode,ordercode 編集。2020/03/06 End>
                    fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                    fun.WriteLog("Login Failed", "046-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "046-");
                    webBrowser1.Navigate(fun.url + "/products/list.php?mode=search&category_id=&name=" + entity.orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                fun.WriteLog(ex, "046-", entity.janCode, entity.orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of item at Mall.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                fun.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.ScriptErrorsSuppressed = true;
                entity = new Qbei_Entity();
                entity.siteID = 46;
                entity.sitecode = "046";
                entity.janCode = dt046.Rows[i]["JANコード"].ToString();
                entity.partNo = dt046.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt046.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt046.Rows[i]["発注コード"].ToString();
                entity.purchaseURL = fun.url + "/products/list.php?mode=search&category_id=&name=" + entity.orderCode;

                if ((!string.IsNullOrWhiteSpace(entity.purchaseURL)) && (entity.orderCode != ""))
                {
                    string url = webBrowser1.Url.ToString();
                    string colName = string.Empty;
                    orderCode = dt046.Rows[i]["発注コード"].ToString().Trim();
                    HtmlElement tBody = webBrowser1.Document.GetElementById("undercolumn_list");
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);

                    int iTableCount = tBody.GetElementsByTagName("tr").Count;
                    for (int j = 1; j <= iTableCount; j++)
                    {
                        colName = (hdoc.DocumentNode.SelectSingleNode("div/div[3]/div[2]/div/div/div/table/tbody/tr[" + j + "]/td[3]").InnerText).Trim();
                        if (colName == orderCode)
                        {
                            string linktext = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div[2]/div/div/div/table/tbody/tr[" + j + "]/td[6]/a").GetAttributeValue("href", "");
                            webBrowser1.Navigate(fun.url + linktext); break;
                        }
                    }
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                    if (colName != orderCode)
                    {
                        webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                        entity.qtyStatus = "empty";
                        entity.price = dt046.Rows[i]["下代"].ToString();
                        if ((dt046.Rows[i]["在庫情報"].ToString().Contains("empty") && dt046.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                        {
                            entity.stockDate = "2100-01-10";
                        }
                        else
                        {
                            entity.stockDate = "2100-02-01";
                        }
                        entity.purchaseURL = fun.url;
                        fun.Qbei_Inserts(entity);
                        if (i < dt046.Rows.Count - 1)
                        {
                            orderCode = fun.ReplaceOrderCode(dt046.Rows[++i]["発注コード"].ToString(), new string[] { "shiyouhenkou/", "-", "在庫処分/inquiry/", "在庫処分/empty/", "在庫処分/empry/", "在庫処分/good/", "在庫処分/small/" });
                            webBrowser1.Navigate(fun.url + "/products/list.php?mode=search&category_id=&name=" + orderCode);
                            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                        }
                        else
                        {
                            qe.SiteID = 42;
                            qe.flag = 2;
                            qe.starttime = string.Empty;
                            qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            fun.ChangeFlag(qe);
                            Application.Exit();
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                    if (i < dt046.Rows.Count - 1)
                    {
                        orderCode = fun.ReplaceOrderCode(dt046.Rows[++i]["発注コード"].ToString(), new string[] { "shiyouhenkou/", "-", "在庫処分/inquiry/", "在庫処分/empty/", "在庫処分/empry/", "在庫処分/good/", "在庫処分/small/" });
                        webBrowser1.Navigate(fun.url + "/products/list.php?mode=search&category_id=&name=" + orderCode);
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                    }
                    else
                    {
                        qe.SiteID = 46;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                fun.WriteLog(ex, "046-", entity.janCode, entity.orderCode);

                orderCode = fun.ReplaceOrderCode(dt046.Rows[++i]["発注コード"].ToString(), new string[] { "shiyouhenkou/", "-", "在庫処分/inquiry/", "在庫処分/empty/", "在庫処分/empry/", "在庫処分/good/", "在庫処分/small/" });
                webBrowser1.Navigate(fun.url + "/products/list.php?mode=search&category_id=&name=" + orderCode);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;

            try
            {
                string html = webBrowser1.Document.Body.InnerHtml;
                HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                hdoc.LoadHtml(html);

                string qty = "div/div[3]/div[2]/div[3]/div[2]/table/tbody/tr[2]/td[3]/img";
                string alt;
                HtmlNodeCollection nc;
                nc = hdoc.DocumentNode.SelectNodes(qty);
                if (nc == null)
                {
                    qty = "div[1]/div[3]/div[2]/div[3]/div/div/table/tbody/tr[2]/td[3]/img";
                }

                //<remar Add Logic 2020/05/04 Start>
                nc = hdoc.DocumentNode.SelectNodes(qty);
                if (nc == null)
                {
                    alt = "null";
                }
                else
                {
                    alt = hdoc.DocumentNode.SelectSingleNode(qty).GetAttributeValue("alt", "");
                }
                //</remark 2020/05/04 End>

                //string alt = hdoc.DocumentNode.SelectSingleNode(qty).GetAttributeValue("alt", "");
                //entity.qtyStatus = alt.Contains("○") ? "good" : alt.Contains("△") || alt.Contains("▲") ? "small" : alt.Contains("予約受付中") || alt.Contains("在庫なし") || alt.Contains("受付終了") || alt.Contains("×") ? "empty" : "unknown status";
                entity.qtyStatus = alt.Contains("○") ? "good" : alt.Contains("△") || alt.Contains("▲") ? "small" : alt.Contains("予約受付中") || alt.Contains("在庫なし") || alt.Contains("受付終了") || alt.Contains("null") || alt.Contains("×") ? "empty" : "unknown status";//<remar Edit Logic 2020/05/04 />
                //entity.stockDate = alt.Contains("○") || alt.Contains("△") || alt.Contains("▲") || alt.Contains("予約受付中") || alt.Contains("在庫なし") || alt.Contains("×") ? "2100-01-01" : alt.Contains("受付終了") ? "2100-02-01" : "unknown date";
                //<remark 06/12/2019変更>
                entity.stockDate = alt.Contains("○") || alt.Contains("△") || alt.Contains("▲") ? "2100-01-01" : alt.Contains("予約受付中") || alt.Contains("在庫なし") || alt.Contains("受付終了") || alt.Contains("null") || alt.Contains("×") ? "2100-02-01" : "unknown date";//<remar Edit Logic 2020/05/04 />
                //entity.stockDate = alt.Contains("○") || alt.Contains("△") || alt.Contains("▲") ? "2100-01-01" : alt.Contains("予約受付中") || alt.Contains("在庫なし") || alt.Contains("×") || alt.Contains("受付終了") ? "2100-02-01" : "unknown date";
                //</remark>
                entity.price = webBrowser1.Document.GetElementById("price02_default").InnerText; ;
                entity.price = entity.price.Replace(",", string.Empty);
                entity.purchaseURL = webBrowser1.Url.ToString();
                //fun.Qbei_Inserts(entity);
                //<remark Close Logic 2020/25/22 Start>
                //if ((dt046.Rows[i]["在庫情報"].ToString().Contains("empty") || dt046.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt046.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                //{
                //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                //    {
                //        entity.qtyStatus = dt046.Rows[i]["在庫情報"].ToString();
                //        entity.price = dt046.Rows[i]["下代"].ToString();
                //        entity.stockDate = dt046.Rows[i]["入荷予定"].ToString();
                //    }
                //    fun.Qbei_Inserts(entity);
                //}
                //else
                //</reamark 2020/25/22 End>
                //2018/1/12
                fun.Qbei_Inserts(entity);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                fun.WriteLog(ex, "046-", entity.janCode, entity.orderCode);
            }

            if (i < dt046.Rows.Count - 1)
            {
                string ordercode = fun.ReplaceOrderCode(dt046.Rows[++i]["発注コード"].ToString(), new string[] { "shiyouhenkou/", "-", "在庫処分/inquiry/", "在庫処分/empty/", "在庫処分/empry/", "在庫処分/good/", "大きすぎる為取扱停止", "在庫処分/small/" });
                webBrowser1.Navigate(fun.url + "/products/list.php?mode=search&category_id=&name=" + ordercode);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
            else
            {
                qe.site = 46;
                qe.flag = 2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt046.Rows[i]["JANコード"].ToString();
            string orderCode = dt046.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "046-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}