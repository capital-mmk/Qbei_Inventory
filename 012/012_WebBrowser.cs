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
        ///</remark>
        private void testflag()
        {           
            try
            {                
                qe.site = 12;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");                
                qe.flag = 1;

                //Input DataTable of Common Function Flag Table. 
                /// <remark>
                ///Select Flag from Site_setting Table. 
                /// </remark>
                DataTable dtflag = fun.SelectFlag(12);                
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());

                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                ///  "0" or "1" of Check.
                /// </remark>
                if (flag == 0)
                {
                    ///<remark>
                    ///when flag is 0,Change to flag is 1 .To Continue Next Process.
                    ///</remark>

                    //Common Function of ChangFlage Process.
                    fun.ChangeFlag(qe);
                    //StartRun Process.
                    StartRun();
                }

                ///<remark>
                ///when flag is 1,To Continue Next Process.
                ///</remark>
                else if (flag == 1)
                {
                    //Common Function of deleteData Process.
                    ///<remark>
                    ///Delete to AllData at Qbei Table.
                    ///</remark>
                    fun.deleteData(12);

                    //Common Function of ChangFlage Process.
                    ///<remark>
                    ///Change to flag is 1 at site_setting Table.
                    ///</remark>
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
                // Common Function of WriteLog Process.   
                fun.WriteLog(ex, "012-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing.
        /// </remark>
        public void StartRun()
        {
            try
            {
                webBrowser1.AllowWebBrowserDrop = false;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.WebBrowserShortcutsEnabled = false;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;

                //Common Function of setURL Process.
                ///<remark>
                ///Database connection string at Qbei_Log of App.Config. 
                ///</remark>
                fun.setURL("012");

                //Common Function of CreateFileAndFolder Process.
                fun.CreateFileAndFolder();

                //Common Function of Qbei_Delete Process.
                ///<remark>
                ///Insert Qbei_Backup Table where select from Qbei Table.
                ///After Delete to Qbei Table of AllData  and Qbei_Backup Table of Updated Date is Greater than 14days.
                ///</remark>
                fun.Qbei_Delete(12);

                //Common Function of Qbei_ErrorDelete Process.
                ///<remark>
                ///Insert Qbei_ErrorLog Backup Table where select from Qbei_ErrorLog Table.
                ///After Delete to Qbei_ErrorLog Table of AllData  and Qbei_ErrorLog_Backup Table of Date is less than 14days.
                ///</remark>
                fun.Qbei_ErrorDelete(12);

                // To Input dt012(Data Table) at Common Function of GetDatatable Process.       
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt012 = fun.GetDatatable("012");

                // To Input dt012(データテーブル) at Common Function of GetDatatable Process. 
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt012 = fun.GetOrderData(dt012, "https://www.riobike.com/shop/g/g", "012", string.Empty);

                // Common Function of GetTotalCount Process.
                ///<remark>
                ///Update to Site_setting Table of TotalCount.
                ///</remark>   
                fun.GetTotalCount("012");     
                ReadData();
            }
            catch (Exception ex)
            {
                //Common Function of WriteLog Process. 
                fun.WriteLog(ex, "012-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Reading.
        /// </remark>
        private void ReadData()
        {           
            qe.SiteID = 12;

            // To Input Data Table at Connection Database of Qbei_Setting_Select Process.    
            /// <remark>
            /// Select Data from Site_Setting Table for Input to Dt Table .
            /// </remark>
            dt = qubl.Qbei_Setting_Select(qe);

            // To Input Common Funtion of url at Data Table of Url.           
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
       
            //Check WebBrwser Navigate at Common Function of url process.
            webBrowser1.Navigate(fun.url);
            //Continue to webBrowser1_Start process.
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);

                // Common Function of WriteLog Process.    
                fun.WriteLog("Navigation to Site Url success------", "012-");
                qe.SiteID = 12;

                // To Input Data Table at Connection Database of Qbei_Setting_Select Process. 
                /// <remark>
                /// Select Data from Site_Setting Table for Input to Dt Table .
                /// </remark>
                dt = qubl.Qbei_Setting_Select(qe);
                //To Input string of username at Data Table of "UserName".
                string username = dt.Rows[0]["UserName"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("login_uid").InnerText = username;      
                //To Input string of username at Data Table of "Password".
                string password = dt.Rows[0]["Password"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("login_pwd").InnerText = password;
                //Click at webpage of Login.
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                //Continue to webBrowser1_Login process.
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt012.Rows[0]["JANコード"].ToString();
                string orderCode = dt012.Rows[0]["発注コード"].ToString();

                // Common Function of Qbei_ErrorInsert Process.     
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");

                // Common Function of WriteLog Process.  
                fun.WriteLog(ex, "012-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                //To Input string of body at WebBrowser Document of GetElementsByTagName("body")[0].
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください"))
                {
                    // Common Function of Qbei_ErrorInsert Process. 
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Login Failed", dt012.Rows[0]["JANコード"].ToString(), dt012.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");

                    // Common Function of WriteLog Process.
                    fun.WriteLog("Login Failed", "012-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    // Common Function of WriteLog Process. 
                    fun.WriteLog("Login success             ------", "012-");
                    //To Input string of username at Data Table of ("発注コード").
                    entity.orderCode = fun.ReplaceOrderCode(dt012.Rows[0]["発注コード"].ToString(), new string[] { "--" });
                    //WebBrowser Navigate of url.
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + entity.orderCode);
                    //Continue to webBrowser1_ItemSearch process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                //To Input String of jancode at dt012 of "JANコード".
                string janCode = dt012.Rows[0]["JANコード"].ToString();
                //To Input String of orderCode at dt012 of "発注コード".
                string orderCode = dt012.Rows[0]["発注コード"].ToString();

                // Common Function of Qbei_ErrorInsert Process.            
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");

                // Common Function of WriteLog Process.
                fun.WriteLog(ex, "012-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strStockDate = string.Empty;
            webBrowser1.ScriptErrorsSuppressed = true;

            ///<remark>
            ///When webBroswer of Url is not the same of Common Function url process.
            ///</remark>
            if (webBrowser1.Url.ToString().Trim() != fun.url + "/shop/g/g" + entity.orderCode) return;

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);

            try
            {
                fun.ClearMemory();

                entity = new Qbei_Entity();  
                entity.siteID = 12;
                entity.sitecode = "012";
                //To Input janCode at dt012 of "JANコード".
                entity.janCode = dt012.Rows[i]["JANコード"].ToString();
                //To Input partNo at dt012 of "自社品番".
                entity.partNo = dt012.Rows[i]["自社品番"].ToString();
                //To Input partNo at Common Function of getCurrentDate process.
                entity.makerDate = fun.getCurrentDate();
                //To Input reflectDate at dt012 of "最終反映日".
                entity.reflectDate = dt012.Rows[i]["最終反映日"].ToString();
                //To Input orderCode at dt012 of "発注コード".
                entity.orderCode = dt012.Rows[i]["発注コード"].ToString();
                //To Input purchaseURL.
                entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode;

                /// <remark>
                /// To Check of Condition at Input Data.
                /// </remark>
                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    //To Input body at Webpage Inspect of html.
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;

                    /// <remark>
                    /// To Check of body Condition at Webpage Inspect and Input Data.
                    /// </remark>
                    if (body.Contains("申し訳ございません"))
                    {
                        /// <remark>
                        /// To Check of stockDate and quantity.
                        /// </remark>
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
                        //Common Function of Qbei_Inserts Process.
                        ///<remark>
                        ///Delete from Qbei_OrderData Table of Data.
                        ///Insert to Qebei Table.
                        ///</remark>   
                        fun.Qbei_Inserts(entity);
                    }
                    else
                    {
                        //To Input html at Webpage Inspect.
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        //hdoc Documennt of html.
                        hdoc.LoadHtml(html);

                        /// <remark>
                        /// To Check of condition at stockDate and quantity.
                        /// </remark>
                        if (hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span") == null && webBrowser1.Document.GetElementById("spec_stock_msg") == null)
                        {
                            // Common Function of Qbei_ErrorInsert Process.  
                            ///<remark>
                            ///Insert to Data of Qbei_ErrorLog Table.
                            ///</remark>
                            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");

                            // Common Function of WriteLog Process.
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "012-");
                            
                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            //To Input price at hdoc Documennt of html.
                            entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");
                            //To Input alt at hdoc Documennt of html.
                            string alt = webBrowser1.Document.GetElementById("spec_stock_msg").InnerText.Trim();

                            /// <remark>
                            /// To Check of condition at alt and hdoc Documennt of html.
                            /// </remark>
                            if (alt.Contains("×") && ((webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText.Contains("在庫限り")) || (webBrowser1.Document.GetElementById("spec_goods_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_name").InnerText.Contains("在庫限り"))))
                            {
                                entity.stockDate = "2100-02-01";
                                entity.qtyStatus = "empty";
                            }
                            else
                            {
                                //To Input quantity at Check of alt Condition ["○" , "△" , "×" , "終了" ,  "×(終了)" ,"unknown status"] .
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Contains("×") || alt.Contains("終了") || alt.Equals("×(終了)") ? "empty" : "unknown status";
                                //To Input stockDate at Check of alt Condition ["○" , "△" , "×" , "終了" ,  "×(終了)" ,"unknown date"] .
                                entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("終了") || alt.Equals("×(終了)") ? "2100-02-01" : "unknown date";
                            }


                            /// <remark>
                            /// To Check of condition at dt012 of stockDate and quantity.
                            /// </remark>
                            if ((dt012.Rows[i]["在庫情報"].ToString().Contains("empty") || dt012.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt012.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                /// <remark>
                                /// To Check of condition at stockDate and quantity.
                                /// </remark>
                                if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
                                {
                                    //To Input quantity at dt012 of "在庫情報".
                                    entity.qtyStatus = dt012.Rows[i]["在庫情報"].ToString();
                                    //To Input price at dt012 of "下代".
                                    entity.price = dt012.Rows[i]["下代"].ToString();
                                    //To Input stockDate at dt012 of "入荷予定".
                                    entity.stockDate = dt012.Rows[i]["入荷予定"].ToString();
                                }
                                //Common Function of Qbei_Inserts Process.
                                ///<remark>
                                ///Delete from Qbei_OrderData Table of Data.
                                ///Insert to Qebei Table.
                                ///</remark> 
                                fun.Qbei_Inserts(entity);
                            }
                            else
                                //Common Function of Qbei_Inserts Process.
                                ///<remark>
                                ///Delete from Qbei_OrderData Table of Data.
                                ///Insert to Qebei Table.
                                ///</remark> 
                                fun.Qbei_Inserts(entity);
                        }
                    }
                }
                else
                {
                    // Common Function of Qbei_ErrorInsert Process.  
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                }
            }
            catch (Exception ex)
            {
                // Common Function of Qbei_ErrorInsert Process.  
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");


                // Common Function of WriteLog Process. 
                fun.WriteLog(ex, "012-", entity.janCode, entity.orderCode);
            }
            finally
            {
                /// <remark>
                /// To Check of condition at dt012 rows .
                /// </remark>
                if (i < dt012.Rows.Count - 1)
                {
                    //To Input ordercode at Common Function of ReplaceOrderCode process.
                    ///<remark>
                    ///Replace to odercode at string of empty.
                    ///</remark>
                    webBrowser1.ScriptErrorsSuppressed = true;
                    //To Input ordercode at dt012 of "発注コード".
                    entity.orderCode = dt012.Rows[++i]["発注コード"].ToString();
                    //WebBrowser Navigate of url.
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + entity.orderCode);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    //To Continue webBrowser1_ItemSearch process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    //Input SiteID Number.
                    qe.site = 12;
                    //Input flag Number.
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //Common Function of ChangeFlag Process.
                    ///<remark>
                    ///Change to flag is 2 at site_setting Table.
                    ///</remark>
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }

        //NavigateErrorの　表示。
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            //To Input janCode at dt012 of "JANコード".
            string janCode = dt012.Rows[i]["JANコード"].ToString();
            //To Input orderCode at dt012 of "発注コード".
            string orderCode = dt012.Rows[i]["発注コード"].ToString();

            // Common Function of Qbei_ErrorInsert Process. 
            ///<remark>
            ///Insert to Data of Qbei_ErrorLog Table.
            ///</remark>
            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");

            // Common Function of WriteLog Process.
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "012-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
