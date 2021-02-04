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

namespace _018日直_ニチナオ_
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm018 : Form
    {
        DataTable dt = new DataTable();
        CommonFunction fun = new CommonFunction();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        DataTable dt018 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        public static string st = string.Empty;
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm018()
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
        ///"2" is End Proces
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 18;
                //st = qe.starttime;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(18);
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
                    fun.deleteData(18);
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
                fun.WriteLog(ex, "018-");
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
                fun.setURL("018");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(18);
                fun.Qbei_ErrorDelete(18);
                dt018 = fun.GetDatatable("018");
                //dt018 = fun.GetOrderData(dt018, "https://1908.nichinao.com/shop/g/g", "018", string.Empty);
                fun.GetTotalCount("018");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "018-");
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
            qe.SiteID = 18;
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

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.ScriptErrorsSuppressed = true;
                fun.WriteLog("Navigation to Site Url success-----+", "018-");
                //qe.sitecode = "018";
                qe.SiteID = 18;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "uid", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "pwd", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt018.Rows[0]["JANコード"].ToString();
                string orderCode = dt018.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                fun.WriteLog(ex, "018-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check Login
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください。"))
                {
                    fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), "Login Failed", dt018.Rows[0]["JANコード"].ToString(), dt018.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                    fun.WriteLog("Login Failed", "018-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "018-");
                    string ordercode = dt018.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + ordercode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt018.Rows[0]["JANコード"].ToString();
                string orderCode = dt018.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                fun.WriteLog(ex, "018-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
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
            entity = new Qbei_Entity();
            entity.siteID = 18;
            entity.sitecode = "018";
            entity.janCode = dt018.Rows[i]["JANコード"].ToString();
            entity.partNo = dt018.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt018.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt018.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode;
            entity.stockDate = string.Empty;
            try
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("申し訳ございません。"))
                {
                    entity.price = dt018.Rows[i]["下代"].ToString();
                    //2018/04/25 Start
                    if (dt018.Rows[i]["在庫情報"].ToString().Contains("empty") && dt018.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        entity.stockDate = dt018.Rows[i]["入荷予定"].ToString();
                        entity.qtyStatus = dt018.Rows[i]["在庫情報"].ToString();
                        //<remark 2021/01/06>
                        entity.True_StockDate = "Not Found";
                        entity.True_Quantity = "Not Found";
                        //</remark 2021/01/06>
                    }
                    else
                    {
                        //2018/04/25 End
                        //2018/01/10 Start
                        entity.price = dt018.Rows[i]["下代"].ToString();
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        //fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                        //2018/01/10 End
                        //<remark 2021/01/06>
                        entity.True_StockDate = "Not Found";
                        entity.True_Quantity = "Not Found";
                        //</remark 2021/01/06>
                    }
                }
                else
                {
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    //Check Element Exist or not
                    if ((hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[7]/td") == null) && (hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[8]/td") == null))
                    {
                        fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "018-");

                        Application.Exit();
                        Environment.Exit(0);
                    }

                    int rowCount = hdoc.DocumentNode.SelectNodes("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[1]").Count;

                    if (rowCount > 0)
                    {
                        entity.price = hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[5]/td/span").InnerText.Replace("円", string.Empty).Replace(",", string.Empty);

                        string qtypath = hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[7]/td").InnerText;
                        //entity.qtyStatus = qtypath.Equals("○") ? "good" : qtypath.Equals("△") || fun.IsNumber(qtypath) ? "small" : qtypath.Equals("×") || qtypath.Equals("予約") ? "empty" : "No status code";//<remark Edit Logic of quantity 2020/07/21 />
                        entity.qtyStatus = qtypath.Equals("○") ? "good" : qtypath.Equals("△") || fun.IsNumber(qtypath) || qtypath.Equals("×") || qtypath.Equals("予約") ? "empty" : "No status code";
                        //if (qtypath.Contains("予約"))
                        //{
                        HtmlNode node = hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/div[3]/div/table/tbody/tr/td[2]/table[1]/tbody/tr[8]/td");
                        //<remark 2021/01/06>
                        if (node != null)
                        { entity.True_StockDate = node.InnerText; }
                        else { entity.True_StockDate = "項目無し"; }
                        entity.True_Quantity = qtypath;
                        //</remark 2021/01/06>
                        if (node != null)
                        {
                            if (node.InnerText.Contains("次回入荷"))
                            {
                                if (node.InnerText.Contains("次回入荷分はメーカー出荷準備中"))
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                                else
                                {
                                    string day = string.Empty;
                                    if (node.InnerText.Contains("下旬"))
                                    {
                                        day = DateTime.DaysInMonth(DateTime.Now.Year, int.Parse(Regex.Replace(node.InnerText, "[^0-9]+", string.Empty))).ToString();
                                    }
                                    else if (node.InnerText.Contains("上旬"))
                                        day = "10";
                                    else if (node.InnerText.Contains("中旬"))
                                        day = "20";
                                    string month = string.Empty;
                                    month = Regex.Replace(node.InnerText, "[^0-9]+", string.Empty);
                                    string year = DateTime.Now.ToString("yyyy");
                                    DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                    string d = fun.getCurrentDate();
                                    if (dt < Convert.ToDateTime(d))
                                        dt = dt.AddYears(1);
                                    entity.stockDate = dt.ToString("yyyy-MM-dd");
                                }
                            }
                            else
                                entity.stockDate = "2100-01-01";
                        }
                        //else
                        //    entity.stockDate = "2100-01-01";
                        //}
                        //else
                        //{
                        //<remark Edit Logic of stockdate 2020/07/21  Start>
                        if (string.IsNullOrEmpty(entity.stockDate))
                        {
                            //entity.stockDate = qtypath.Equals("×") ? "2100-02-01" : "2100-01-01";
                            entity.stockDate = qtypath.Equals("△") || fun.IsNumber(qtypath) || qtypath.Equals("×") ? "2100-02-01" : "2100-01-01";
                        }
                        //</remark 2020/07/21  End>   
                        //}
                        //<remark Add Logic for Stockdate 2020/02/04 Start>
                        if (qtypath.Equals("予約"))
                        {
                            entity.stockDate = "2100-02-01";
                        }
                        //</remark 2020/02/04 End>
                    }
                    else
                    {
                        //2018/01/10 Start
                        entity.price = dt018.Rows[i]["下代"].ToString();
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        //2018/01/10 End
                        //<remark 2021/01/06>
                        entity.True_StockDate = "Not Found";
                        entity.True_Quantity = "Not Found";
                        //</remark 2021/01/06>
                    }

                }

                //2018/01/10 Start
                //<remark Close Logic 2020/25/22 Start>
                //if ((dt018.Rows[i]["在庫情報"].ToString().Contains("empty") || dt018.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt018.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                //{
                //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                //    {
                //        entity.qtyStatus = dt018.Rows[i]["在庫情報"].ToString();
                //        entity.stockDate = dt018.Rows[i]["入荷予定"].ToString();
                //        entity.price = dt018.Rows[i]["下代"].ToString();
                //    }
                //    fun.Qbei_Inserts(entity);
                //}
                //else
                //</reamark 2020/25/22 End>
                fun.Qbei_Inserts(entity);
                //2018/01/10 End
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
                fun.WriteLog(ex, "018-", entity.janCode, entity.orderCode);
            }
            if (i < dt018.Rows.Count - 1)
            {
                //string ordercode = fun.ReplaceOrderCode(dt018.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "在庫処分/small/", "在庫処分/empry/在庫処分/empry/inquiry/", "在庫処分/empry/", "在庫処分/good/", "-", "在庫処分/empty/", "バラ注文できない為発注禁止/", "発注禁止/" });
                string ordercode = dt018.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/shop/g/g" + ordercode);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 18;
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
            string janCode = dt018.Rows[i]["JANコード"].ToString();
            string orderCode = dt018.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(18, fun.GetSiteName("018"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "018");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "018-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}