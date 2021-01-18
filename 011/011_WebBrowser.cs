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

namespace _011マルイ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm011 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt011 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm011()
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
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 11;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(11);
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
                    fun.deleteData(11);
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
                fun.WriteLog(ex, "011-");
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
                fun.setURL("011");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(11);
                fun.Qbei_ErrorDelete(11);
                dt011 = fun.GetDatatable("011");
                //dt011 = fun.GetOrderData(dt011, "http://www.maruiltd.jp/index.php?action_goods=true&id=", "011", "");
                fun.GetTotalCount("011");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "011-");
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
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 11;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            //Thread.Sleep(1000);
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
                fun.WriteLog("Navigation to Site Url success------", "011-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 11;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("id").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("psw").InnerText = password;
                fun.GetElement("input", "　ロ グ イ ン　", "value", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt011.Rows[0]["JANコード"].ToString();
                string orderCode = dt011.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                fun.WriteLog(ex, "011-", janCode, orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check Login
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string janCode = string.Empty;
            string orderCode = string.Empty;

            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains(" IDを入力してください") || body.Contains("パスワードを入力してください") || body.Contains("IDを正しく入力してください"))
                {
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                    fun.WriteLog("Login Failed", "011-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "011-");
                    orderCode = fun.ReplaceOrderCode(dt011.Rows[0]["発注コード"].ToString(), new string[] { "-" });
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + orderCode + "00000");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                janCode = dt011.Rows[i]["JANコード"].ToString();
                orderCode = dt011.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                fun.WriteLog(ex, "011-", janCode, orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }
        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                entity = new Qbei_Entity();
                entity.siteID = 11;
                entity.sitecode = "011";
                entity.janCode = dt011.Rows[i]["JANコード"].ToString();
                entity.partNo = dt011.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt011.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt011.Rows[i]["発注コード"].ToString();
                entity.orderCode = fun.ReplaceOrderCode(entity.orderCode, new string[] { "-" });
                entity.purchaseURL = fun.url + "/index.php?action_goods=true&id=" + entity.orderCode + "00000";

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;

                    if (body.Contains("エラー [Error]"))
                    {
                        if (dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt011.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            //fun.Qbei_ErrorInsert(11, "マルイ", "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                            //<remark 2021/01/06>
                            entity.True_StockDate = "Not Found";
                            entity.True_Quantity = "Not Found";
                            //</remark 2021/01/06>
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                            //<remark 2021/01/06>
                            entity.True_StockDate = "Not Found";
                            entity.True_Quantity = "Not Found";
                            //</remark 2021/01/06>
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

                            /// <remark>
                            /// To Check of condition at stockDate and quantity.
                            /// </remark>
                            if ((hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[7]/td[2]/table/tbody/tr[1]/td/img") == null) && (hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]") == null))
                            {
                                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "011-");
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                HtmlNode node1 = hdoc.DocumentNode.SelectSingleNode("div[6]//div[2]//div//table//tbody//tr[4]//td//table//tbody//tr//td[2]//table//tbody//tr//td//table//tbody//tr[7]//td[2]//table//tbody//tr//td//img");
                                string alt = node1.GetAttributeValue("alt", string.Empty);
                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[6]/td[2]").InnerText;
                                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);
                                entity.price = ((int)(Convert.ToDouble(entity.price) * 0.98)).ToString();
                                string stockDate = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]").InnerText;
                                //entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Equals("×") || alt.Equals("完売") ? "empty" : "unknown status";
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") || alt.Equals("×") || alt.Equals("完売") ? "empty" : "unknown status";//<remark ロジックの変更　2020/05/20>

                                //<remark 2021/01/06>
                                entity.True_StockDate = stockDate;
                                entity.True_Quantity = alt;
                                //</remark 2021/01/06>

                                if (stockDate.Equals("-") || stockDate.Equals("未定"))
                                {
                                    //entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("完売") ? "2100-02-01" : "unknown status";
                                    //entity.stockDate = alt.Equals("○") || alt.Equals("△") ? "2100-01-01" : alt.Equals("完売") || alt.Equals("×") ? "2100-02-01" : "unknown status";//<remark ロジックの変更　2020/03/17>
                                    entity.stockDate = alt.Equals("○") ? "2100-01-01" : alt.Equals("完売") || alt.Equals("△") || alt.Equals("×") ? "2100-02-01" : "unknown status";//<remark ロジックの変更　2020/05/20>
                                }
                                else
                                {
                                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                    //entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? stockDate : alt.Equals("完売") ? "2100-02-01" : "unknown date";
                                    //entity.stockDate = alt.Equals("○") || alt.Equals("△") ? stockDate : alt.Equals("×") ||alt.Equals("完売") ? "2100-02-01" : "unknown date";//<remark ロジックの変更　2020/04/22>
                                    entity.stockDate = alt.Equals("○") ? stockDate : alt.Equals("×") || alt.Equals("△") || alt.Equals("完売") ? "2100-02-01" : "unknown date";//<remark ロジックの変更　2020/05/20>
                                }
                                //<remark Close Logic 2020/25/22 Start>
                                //if ((dt011.Rows[i]["在庫情報"].ToString().Contains("empty") || dt011.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                //{
                                //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                //    {                                      
                                //        entity.qtyStatus = dt011.Rows[i]["在庫情報"].ToString();
                                //        entity.stockDate = dt011.Rows[i]["入荷予定"].ToString();
                                //        entity.price = dt011.Rows[i]["下代"].ToString();
                                //    }
                                //    fun.Qbei_Inserts(entity);
                                //}
                                //else
                                //</reamark 2020/25/22 End>
                                //2017/12/22 End
                                fun.Qbei_Inserts(entity);
                            }
                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                            fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                }

                if (i < dt011.Rows.Count - 1)
                {
                    string ordercode = fun.ReplaceOrderCode(dt011.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "在庫処分/empry/", "-", "在庫処分/good/", "在庫処分/small/", "在庫処分/empty/", "バラ注文できない為発注禁止/" });
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + ordercode + "00000");
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 11;
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
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);
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
            string janCode = dt011.Rows[i]["JANコード"].ToString();
            string orderCode = dt011.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "011-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
