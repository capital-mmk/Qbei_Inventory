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

namespace _87ダートフリーク
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm087 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt087 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        string Date = string.Empty;
        int i = -1;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm087()
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
                qe.site = 87;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(87);
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
                    fun.deleteData(87);
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
                fun.WriteLog(ex, "087-");
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
                fun.setURL("087");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(87);
                fun.Qbei_ErrorDelete(87);
                dt087 = fun.GetDatatable("087");
                dt087 = fun.GetOrderData(dt087, "http://www.dirtfreak.co.jp/cycledealer/search_form.php?dfhinbanA=&dfhinbanB=&textsearch=&cataloghinban=", "087", "&submitall=submit");
                fun.GetTotalCount("087");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "087-");
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
            qe.SiteID = 87;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.Navigate(fun.url + "/index.php");
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

                fun.WriteLog("Navigation to Site Url success------", "087-");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                qe.SiteID = 87;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();

                fun.GetElement("input", "username", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "passwd", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt087.Rows[0]["JANコード"].ToString();
                string orderCode = dt087.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                fun.WriteLog(ex, "087-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                webBrowser1.ScriptErrorsSuppressed = true;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("問題発生:"))
                {
                    fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), "Login Failed", dt087.Rows[0]["JANコード"].ToString(), dt087.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                    fun.WriteLog("Login Failed", "087-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "087-");
                    orderCode = dt087.Rows[++i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/search_form.php?dfhinbanA=&dfhinbanB=&textsearch=&cataloghinban=" + orderCode + "&submitall=submit");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt087.Rows[0]["JANコード"].ToString();
                orderCode = dt087.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                fun.WriteLog(ex, "087-", janCode, orderCode);                

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
            entity.siteID = 87;
            entity.sitecode = "087";
            entity.janCode = dt087.Rows[i]["JANコード"].ToString();

            entity.partNo = dt087.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt087.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt087.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/search_form.php?dfhinbanA=&dfhinbanB=&textsearch=&cataloghinban=" + entity.orderCode + "&submitall=submit";
            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("検索商品がありませんでした。"))
                {
                    if (dt087.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt087.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt087.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt087.Rows[i]["下代"].ToString();
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
                        if ((hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr/td[2]/div/table/tbody/tr[2]/td/div/table/tbody/tr/td[9]/font") == null))
                        {
                            fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "087-");
                            
                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            string qtypath = hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr/td[2]/div/table/tbody/tr[2]/td/div/table/tbody/tr/td[9]/font").InnerText;
                            entity.qtyStatus = qtypath.Equals("◎") ? "good" : qtypath.Equals("○") || qtypath.Equals("▲") ? "small" : qtypath.Equals("×") ? "empty" : qtypath.Equals("※") ? "inquiry" : "invalid status code";
                            entity.stockDate = qtypath.Equals("◎") || qtypath.Equals("○") || qtypath.Equals("▲") || qtypath.Equals("×") || qtypath.Equals("※") ? "2100-01-01" : "unknown date";

                            entity.price = hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr/td[2]/div/table/tbody/tr[2]/td/div/table/tbody/tr/td[7]/font").InnerText;
                            entity.price = entity.price.Replace(",", string.Empty);
                            if (entity.stockDate.Contains("2月"))
                            {
                                //entity.stockDate = "2018-02-28";
                                entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                            }
                            if ((dt087.Rows[i]["在庫情報"].ToString().Contains("empty") || dt087.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt087.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                                {
                                    entity.qtyStatus = dt087.Rows[i]["在庫情報"].ToString();
                                    entity.price = dt087.Rows[i]["下代"].ToString();
                                    entity.stockDate = dt087.Rows[i]["入荷予定"].ToString();
                                }
                                fun.Qbei_Inserts(entity);
                            }

                            else
                                fun.Qbei_Inserts(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                        fun.WriteLog(ex, "087-", entity.janCode, entity.orderCode);
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),"087");
            }
            if (i < dt087.Rows.Count-1)
            {
                string ordercode = dt087.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/search_form.php?dfhinbanA=&dfhinbanB=&textsearch=&cataloghinban=" + ordercode + "&submitall=submit");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 87;
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
            string janCode = dt087.Rows[i]["JANコード"].ToString();
            string orderCode = dt087.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "087-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}