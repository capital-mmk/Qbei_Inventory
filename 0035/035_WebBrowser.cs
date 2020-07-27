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
using SHDocVw;
using System.Runtime.InteropServices;

namespace _0035
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm035 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt035 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        string strParam = string.Empty;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm035()
        {
            InitializeComponent();
            testflag();
        }

        /// <summary>
        /// SiteID for String Parameter.
        /// </summary>
        public frm035(string strObj)
        {
            InitializeComponent();
            strParam = strObj;
            StartRun();
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
                qe.starttime = DateTime.Now.ToString();
                qe.site = 35;
                st = qe.starttime;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(35);
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
                    fun.deleteData(35);
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
                fun.WriteLog(ex, "035-");
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
        private void StartRun()
        {
            try
            {
                fun.setURL("035");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(35);
                fun.Qbei_ErrorDelete(35);

                if (String.IsNullOrEmpty(strParam))
                {
                    dt035 = fun.GetDatatable("035");
                    dt035 = fun.GetOrderData(dt035, "https://intertecinc.jp/ecuser/item/itemDetail?itemCd=", "035", "");
                }
                else
                {
                    dt035 = fun.GetRerunData("035");
                }

                fun.GetTotalCount("035");
                if (dt035 != null)
                    ReadData();
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "035-");
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
            qe.SiteID = 35;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
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
                fun.WriteLog("Navigation to Site Url success------", "035-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 35;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("id").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("pw").InnerText = password;
                fun.GetElement("a", "ログイン", "InnerText", webBrowser1).InvokeMember("onclick");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt035.Rows[0]["JANコード"].ToString();
                string orderCode = dt035.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex, "035-", janCode, orderCode);

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
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;                
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                
                if (body.Contains(" IDを入力してください") || body.Contains("パスワードを入力してください") || body.Contains("IDを正しく入力してください"))
                {
                    fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Login Failed", dt035.Rows[0]["JANコード"].ToString(), dt035.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                    fun.WriteLog("Login Failed", "035-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "035-");
                    orderCode = dt035.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate("https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt035.Rows[0]["JANコード"].ToString();
                orderCode = dt035.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex, "035-", janCode, orderCode);

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
                entity.siteID = 35;
                entity.sitecode = "035";
                entity.janCode = dt035.Rows[i]["JANコード"].ToString();
                entity.partNo = dt035.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt035.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt035.Rows[i]["発注コード"].ToString();                
                entity.purchaseURL = "https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + entity.orderCode;                

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    if (body.Contains("有効な商品ではありません"))
                    {
                        if (dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt035.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt035.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt035.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }
                    else
                    {
                        try
                        {
                            string html = webBrowser1.Document.Body.InnerHtml;
                            string year = string.Empty;
                            string month = string.Empty;
                            string day = string.Empty;
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(html);
                            if ((hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[5]") == null) && (hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/form/div/div/table/tbody/tr/td[7]/div/span[1]") == null))
                            {
                                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");                                
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "035-");
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                string qty = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[5]").InnerText;

                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[7]/div/span[2]").InnerText;
                                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);
                                //entity.price = ((int)(Convert.ToDouble(entity.price) * 0.98)).ToString();

                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[6]/div/span[1]").InnerText;
                                                                
                                if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("下旬") || entity.stockDate.Contains("中旬") || entity.stockDate.Contains("初旬"))
                                {
                                    month = Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty);
                                    month = int.Parse(month).ToString();

                                    if (entity.stockDate.Contains("上旬"))
                                    {
                                        day = "10";
                                    }
                                    else if (entity.stockDate.Contains("下旬"))
                                    {
                                        if (month == "2")
                                            day = "28";
                                        else
                                        day = "30";
                                    }
                                    else if (entity.stockDate.Contains("中旬"))
                                        day = "20";
                                    else if (entity.stockDate.Contains("初旬"))
                                        day = "10";

                                   
                                    year = DateTime.Now.ToString("yyyy");
                                    DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                    string d = fun.getCurrentDate();
                                    if (dt < Convert.ToDateTime(d))
                                        dt = dt.AddYears(1);

                                    entity.stockDate = dt.ToString("yyyy-MM-dd");
                                }
                                if (entity.stockDate.Equals("-") || entity.stockDate.Equals(""))
                                {
                                    //<remak Change Logic of stockdate 2020/07/23 Start>
                                    //entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsSmall1(qty) ? "small" : qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";
                                    //entity.stockDate = qty.Equals("◯") || qty.Equals("◎") || qty.Equals("△") || fun.IsSmall1(qty) || fun.IsLessthanzero(qty) || qty.Equals("×") ? "2100-01-01" : qty.Equals("完売") || qty.Equals("終了") ? "2100-02-01" : "unknown date";
                                    entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsLessthanzero(qty) || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";
                                    //entity.stockDate = qty.Equals("◯") || qty.Equals("◎") ||  fun.IsLessthanzero(qty)  ? "2100-01-01" : qty.Equals("△") || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") ? "2100-02-01" : "unknown date";
                                    entity.stockDate = qty.Equals("◯") || qty.Equals("◎") ? "2100-01-01" : qty.Equals("△") || fun.IsLessthanzero(qty) || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") ? "2100-02-01" : "unknown date";
                                    //</remark 2020/07/23 End>
                                }
                                else
                                {
                                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                    //entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsSmall1(qty) ? "small" : qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";
                                    entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsLessthanzero(qty) || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";//<remark Change Logic of Stockdate 2020/07/23 />
                                    entity.stockDate = qty.Equals("◯") || qty.Equals("◎") || qty.Equals("△") || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? entity.stockDate : "unknown date";
                                }
                                //<remark Close Logic 2020/25/22 Start>
                                //if (dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                //{
                                //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                //    {
                                //        entity.qtyStatus = dt035.Rows[i]["在庫情報"].ToString();
                                //        entity.stockDate = dt035.Rows[i]["入荷予定"].ToString();
                                //        entity.price = dt035.Rows[i]["下代"].ToString();
                                //    }
                                //    fun.Qbei_Inserts(entity);
                                //}
                                //</reamark 2020/25/22 End>
                                if ((!String.IsNullOrEmpty(strParam)) && ((dt035.Rows[i]["在庫情報"].ToString().Contains("empty") && (String.IsNullOrEmpty(dt035.Rows[i]["入荷予定"].ToString()) || dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-01") || dt035.Rows[i]["入荷予定"].ToString().Contains("2100-02-01"))) || dt035.Rows[i]["在庫情報"].ToString().Contains("inquiry")))
                                {
                                    fun.RerunOrder(entity);
                                }
                                else
                                    //2017/12/22 End
                                    fun.Qbei_Inserts(entity);
                            }
                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                            fun.WriteLog(ex, "035-", entity.janCode, entity.orderCode);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                }

                if (i < dt035.Rows.Count - 1)
                {
                    string ordercode = dt035.Rows[++i]["発注コード"].ToString();
                    webBrowser1.AllowNavigation = true;
                    webBrowser1.Navigate("https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + ordercode);

                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 35;
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
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex, "035-", entity.janCode, entity.orderCode);

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
            string janCode = dt035.Rows[i]["JANコード"].ToString();
            string orderCode = dt035.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "035-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
