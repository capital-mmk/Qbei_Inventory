using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using HtmlAgilityPack;
using System.Security.Permissions;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Threading;
using System.Text.RegularExpressions;

namespace _12カワシマ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm012 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt012 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm012()
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
                qe.site = 12;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(12);
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
                    fun.deleteData(12);
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
                fun.WriteLog(ex, "012-");
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
                webBrowser1.AllowWebBrowserDrop = false;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.WebBrowserShortcutsEnabled = false;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
                fun.setURL("012");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(12);
                fun.Qbei_ErrorDelete(12);
                dt012 = fun.GetDatatable("012");
                //dt012 = fun.GetOrderData(dt012, "https://www.riobike.com/shop/g/g", "012", string.Empty);
                fun.GetTotalCount("012");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "012-");
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
            qe.SiteID = 12;
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
                fun.WriteLog("Navigation to Site Url success------", "012-");
                qe.SiteID = 12;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("login_uid").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("login_pwd").InnerText = password;
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt012.Rows[0]["JANコード"].ToString();
                string orderCode = dt012.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex, "012-", janCode, orderCode);

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

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください"))
                {
                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Login Failed", dt012.Rows[0]["JANコード"].ToString(), dt012.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                    fun.WriteLog("Login Failed", "012-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "012-");
                    entity.orderCode = fun.ReplaceOrderCode(dt012.Rows[0]["発注コード"].ToString(), new string[] { "--" });
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + entity.orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt012.Rows[0]["JANコード"].ToString();
                string orderCode = dt012.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex, "012-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strStockDate = string.Empty;
            webBrowser1.ScriptErrorsSuppressed = true;
            if (webBrowser1.Url.ToString().Trim() != fun.url + "/shop/g/g" + entity.orderCode) return;

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);

            try
            {
                fun.ClearMemory();

                entity = new Qbei_Entity();
                entity.siteID = 12;
                entity.sitecode = "012";
                entity.janCode = dt012.Rows[i]["JANコード"].ToString();
                entity.partNo = dt012.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt012.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt012.Rows[i]["発注コード"].ToString();
                entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode;

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;

                    if (body.Contains("申し訳ございません"))
                    {
                        if (dt012.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt012.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt012.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt012.Rows[i]["下代"].ToString();
                        }
                        //<remark 2021/01/06>
                        entity.True_StockDate = "Not Found";
                        entity.True_Quantity = "Not Found";
                        //</remark 2021/01/06>
                        fun.Qbei_Inserts(entity);
                    }
                    else
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);

                        /// <remark>
                        /// To Check of condition at stockDate and quantity.
                        /// </remark>
                        if (hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span") == null && webBrowser1.Document.GetElementById("spec_stock_msg") == null)
                        {
                            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "012-");

                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            //<remark Add Logic for check element path to price 2022/01/17 Start>
                            if (hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span") == null)
                            {
                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");
                            }
                            else
                            {
                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");
                            }
                            //entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");
                            //entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");//<remark Edit Logic for Price 2022/01/10 />
                            //</remark 2022/01/17 End>
                            string alt = webBrowser1.Document.GetElementById("spec_stock_msg").InnerText.Trim();

                            if (alt.Contains("×") && ((webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText.Contains("在庫限り")) || (webBrowser1.Document.GetElementById("spec_goods_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_name").InnerText.Contains("在庫限り"))))
                            {
                                entity.stockDate = "2100-02-01";
                                entity.qtyStatus = "empty";

                                //<remark 2020/01/06>
                                entity.True_StockDate = "項目無し";
                                entity.True_Quantity = alt;
                                //</remark 2021/01/06>
                            }
                            else
                            {
                                //<remark Edit Logic of stockdate 2020/07/21 Start>
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Contains("×") || alt.Contains("終了") || alt.Equals("×(終了)") ? "empty" : "unknown status";//<remark ロジックの変更　2022/01/19 />                         
                                //entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("終了") || alt.Equals("×(終了)") ? "2100-02-01" : "unknown date";
                                //entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") || alt.Contains("×") || alt.Contains("終了") || alt.Equals("×(終了)") ? "empty" : "unknown status";
                                entity.stockDate = alt.Equals("○") ||  alt.Equals("△") ||alt.Equals("×") ? "2100-01-01" : alt.Equals("終了") || alt.Equals("×(終了)") ? "2100-02-01" : "unknown date";//<remark ロジックの変更　2022/01/19 />
                                //entity.stockDate = alt.Equals("○") ? "2100-01-01" : alt.Equals("△") || alt.Equals("×") || alt.Equals("終了") || alt.Equals("×(終了)") ? "2100-02-01" : "unknown date";//<remark Change Logic of stockdate 2020/07/27 />
                                //</remark 2020/07/21 End>                                                                                                                                                                  //</remark 2020/07/21 End>

                                //<remark 2021/01/06>
                                entity.True_StockDate = "項目無し";
                                entity.True_Quantity = alt;
                                //</remark 2021/01/06>
                            }
                            //<remark Close Logic 2020/25/22 Start>
                            //if ((dt012.Rows[i]["在庫情報"].ToString().Contains("empty") || dt012.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt012.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            //{                               
                            //    if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
                            //    {                                  
                            //        entity.qtyStatus = dt012.Rows[i]["在庫情報"].ToString();                                 
                            //        entity.price = dt012.Rows[i]["下代"].ToString();                                
                            //        entity.stockDate = dt012.Rows[i]["入荷予定"].ToString();
                            //    }                            
                            //    fun.Qbei_Inserts(entity);
                            //}
                            //else     
                            //</reamark 2020/25/22 End>
                            fun.Qbei_Inserts(entity);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex, "012-", entity.janCode, entity.orderCode);
            }
            finally
            {
                if (i < dt012.Rows.Count - 1)
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    entity.orderCode = dt012.Rows[++i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + entity.orderCode);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 12;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt012.Rows[i]["JANコード"].ToString();
            string orderCode = dt012.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "012-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
