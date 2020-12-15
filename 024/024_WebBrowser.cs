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
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm024 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt024 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm024()
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
                qe.starttime = DateTime.Now.ToString();
                qe.site = 24;
                st = qe.starttime;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(24);
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
                    fun.deleteData(24);
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
                fun.WriteLog(ex, "024-");
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
                fun.setURL("024");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(24);
                fun.Qbei_ErrorDelete(24);
                dt024 = fun.GetDatatable("024");
                //dt024 = fun.GetOrderData(dt024, "https://ew.azuma-weborder.jp/azuma/product_detail/multi_request?id=", "024", "");//<remark Close Logic 2020/06/16 />
                fun.GetTotalCount("024");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "024-");
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
            qe.SiteID = 24;
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
                fun.WriteLog(ex, "024-", entity.janCode, entity.orderCode);

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
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString().Trim();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString().Trim();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This site can't reach", entity.janCode, entity.orderCode, 6, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                webBrowser1.ScriptErrorsSuppressed = true;
                body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("会員コード・パスワードは旧システムの得意先コード、パスワードをご入力ください"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                    fun.WriteLog("Login Failed", "024");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "024-");
                    webBrowser1.Navigate(fun.url + "/azuma/product_detail/multi_request?id=" + entity.orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex, "024-", entity.janCode, entity.orderCode);
                
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                entity.orderCode = dt024.Rows[i]["発注コード"].ToString().Trim();
                entity.janCode = dt024.Rows[i]["JANコード"].ToString().Trim();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("This page can't be displayed"))
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "This site can't reach", entity.janCode, entity.orderCode, 6, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.Navigate(fun.url + "/azuma/product_detail/multi_request?id=" + entity.orderCode);
                if (webBrowser1.Url.ToString().Contains("/product_detail/"))
                {
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");                
                fun.WriteLog(ex, "024-", entity.janCode, entity.orderCode);
                
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
                entity.purchaseURL = fun.url + "/azuma/product_detail/multi_request?id=" + entity.orderCode;

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    //Check Element Exist or not
                    if (webBrowser1.Document.GetElementsByTagName("html")[0] == null)
                    {
                        fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "024-");

                        Application.Exit();
                        Environment.Exit(0);
                    }
                    body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    //fun.WriteLog(body, "024_Record");//<remark Close Logic for Data of Record 2020/06/19 />
                    string Date = DateTime.Now.ToString("yyyy-MM-dd");

                    string qtyStatus = string.Empty;
                    string[] strarr = body.Split('|');
                    //if (strarr[1] == "[0]")//<remark Close Logic  2020/06/18 />
                    if (strarr[4] == "[=]")//<remark Edit Logic 2020/06/19 />
                    {
                        entity.qtyStatus = "empty";
                        entity.price = dt024.Rows[i]["下代"].ToString();
                        if ((dt024.Rows[i]["在庫情報"].ToString().Contains("empty") || dt024.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && (dt024.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                        {
                            // entity.stockDate = "2100-01-10";
                            //<remark StockDateのロジックを更新 24/1/2020 Start>
                            entity.stockDate = "2100-02-01";
                            //</remark 24/1/2020 End>
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
                        int year = DateTime.Now.Year;
                        //string stockdateExist = strarr[5];
                        string dateexists = string.Empty;
                        qtyStatus = strarr[4];
                        qtyStatus = qtyStatus.Replace("[", string.Empty).Replace("]", string.Empty).Replace("=", string.Empty);

                        if (qtyStatus.Contains("○") || qtyStatus.Contains("◎"))
                        {
                            entity.qtyStatus = "good";
                            dateexists = qtyStatus.Replace("○", string.Empty).Replace("◎", string.Empty);
                        }
                        //<remark quantity & stockdateの　編集ロジック　2020/06/16 Start>
                        //<remark quantity & stockdateの　編集ロジック　2020/04/07 Start>
                        //else if (qtyStatus.Contains("△") || qtyStatus.Contains("台|個|ロット") || qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中"))
                        //else if ( qtyStatus.Contains("台|個|ロット") || qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中"))
                        //<remark Change Logic of quantity 2020/07/23 Start>
                        //else if (qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中"))
                        //{
                        //    entity.qtyStatus = "empty";
                            //dateexists = qtyStatus.Replace("△", string.Empty).Replace("台|個|ロット", string.Empty);
                            //dateexists = qtyStatus.Replace("×", string.Empty).Replace("入荷予定", string.Empty).Replace("予約受付中", string.Empty);
                        //}
                        //else if (qtyStatus.Contains("△"))
                        //else if (qtyStatus.Contains("台|個|ロット") || qtyStatus.Contains("△"))
                        //{
                            //entity.qtyStatus = "good";
                            //entity.qtyStatus = "small";                       
                        //}
                        else if (qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中") || qtyStatus.Contains("台|個|ロット") || qtyStatus.Contains("△"))
                        {
                            entity.qtyStatus = "empty";
                            dateexists = qtyStatus.Replace("×", string.Empty).Replace("入荷予定", string.Empty).Replace("予約受付中", string.Empty).Replace("台|個|ロット", string.Empty).Replace("△", string.Empty);
                        }
                        //</remak 2020/07/23 End>
                        //</remark 2020/04/07 End>
                        else
                        {
                            entity.qtyStatus = "unknown status";
                        }

                        if (dateexists != "")
                        {
                            entity.stockDate = dateexists;
                        }
                        else
                        {
                            //entity.stockDate = qtyStatus.Equals("○") || qtyStatus.Equals("◎") ? "2100-01-01" : qtyStatus.Contains("△") || qtyStatus.Contains("×") || qtyStatus.Contains("台|個|ロット") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中") ? "2100-02-01" : "unknown date";
                            //entity.stockDate = qtyStatus.Equals("○") || qtyStatus.Equals("◎") || qtyStatus.Contains("△") || qtyStatus.Contains("台|個|ロット") ? "2100-01-01" :  qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中") ? "2100-02-01" : "unknown date";//<remark Change Logic of Stockdate />
                            entity.stockDate = qtyStatus.Equals("○") || qtyStatus.Equals("◎")? "2100-01-01" : qtyStatus.Contains("×") || qtyStatus.Contains("入荷予定") || qtyStatus.Contains("予約受付中") || qtyStatus.Contains("△") || qtyStatus.Contains("台|個|ロット") ? "2100-02-01" : "unknown date";
                        }
                        //</remark 2020/06/16 End>

                        //if ((dt024.Rows[i]["在庫情報"].ToString().Contains("empty") || dt024.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt024.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        //{
                        //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                        //    {
                        //        entity.qtyStatus = dt024.Rows[i]["在庫情報"].ToString();
                        //        entity.stockDate = dt024.Rows[i]["入荷予定"].ToString();
                        //        entity.price = dt024.Rows[i]["下代"].ToString();
                        //    }
                        //    fun.Qbei_Inserts(entity);
                        //}
                        //else

                        fun.Qbei_Inserts(entity);
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                fun.WriteLog(ex, "024-", entity.janCode, entity.orderCode);
            }
            finally
            {
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
                    qe.starttime = st;
                    qe.endtime = DateTime.Now.ToString();
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
            string janCode = dt024.Rows[i]["JANコード"].ToString();
            string orderCode = dt024.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "024-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
