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
        ///</remark>
        private void testflag()
        {
         try
            {                
                Qbeisetting_Entity qe = new Qbeisetting_Entity();     
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");                
                qe.site = 11;                
                qe.flag = 1;

                //Input DataTable of Common Function Flag Table. 
                /// <remark>
                ///Select Flag from Site_setting Table. 
                /// </remark>
                DataTable dtflag = fun.SelectFlag(11);               
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
         
                /// <remark>
                ///when flag is 0,Change to flag is 1 .To Continue Next Process.
                /// </remark>
                if (flag == 0)
                {   
                    //Common Function of ChangFlage Process.
                    ///<remark>
                    ///Change to flag is 1 at site_setting Table.
                    ///</remark>
                    fun.ChangeFlag(qe);                 
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
                    fun.deleteData(11);

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
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog(ex, "011-");
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
                //Common Function of setURL Process.
                ///<remark>
                ///Database connection string at Qbei_Log of App.Config. 
                ///</remark>
                fun.setURL("011");

                //Common Function of CreateFileAndFolder Process.
                ///<remark>
                ///Create FilePath and Directory at Qbei_Log.
                ///</remark>
                fun.CreateFileAndFolder();

                //Common Function of Qbei_Delete Process.
                ///<remark>
                ///Insert Qbei_Backup Table where select from Qbei Table.
                ///After Delete to Qbei Table of AllData  and Qbei_Backup Table of Updated Date is Greater than 14days.
                ///</remark>
                fun.Qbei_Delete(11);

                //Common Function of Qbei_ErrorDelete Process.
                ///<remark>
                ///Insert Qbei_ErrorLog Backup Table where select from Qbei_ErrorLog Table.
                ///After Delete to Qbei_ErrorLog Table of AllData  and Qbei_ErrorLog_Backup Table of Date is less than 14days.
                ///</remark>
                fun.Qbei_ErrorDelete(11);

                // To Input dt011(Data Table) at Common Function of GetDatatable Process.       
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt011 = fun.GetDatatable("011");

                // To Input dt011(データテーブル) at Common Function of GetDatatable Process. 
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt011 = fun.GetOrderData(dt011, "http://www.maruiltd.jp/index.php?action_goods=true&id=", "011","");

                // Common Function of GetTotalCount Process.
                ///<remark>
                ///Update to Site_setting Table of TotalCount.
                ///</remark>
                fun.GetTotalCount("011");        
                ReadData();
            }
            catch (Exception ex)
            {
                //Common Function of WriteLog Process.
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog(ex, "011-");
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
           webBrowser1.ScriptErrorsSuppressed = true;   
            qe.SiteID = 11;
            // To Input Data Table at Connection Database of Qbei_Setting_Select Process.    
            /// <remark>
            /// Select Data from Site_Setting Table for Input to Dt Table .
            /// </remark>
            dt = qubl.Qbei_Setting_Select(qe);

            // To Input Common Funtion of url at Data Table of Url. 
            fun.url = dt.Rows[0]["Url"].ToString();
            //Thread.Sleep(1000);
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
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog("Navigation to Site Url success------", "011-");
                webBrowser1.ScriptErrorsSuppressed = true;                
                qe.SiteID = 11;

                // To Input Data Table at Connection Database of Qbei_Setting_Select Process. 
                /// <remark>
                /// Select Data from Site_Setting Table for Input to Dt Table .
                /// </remark>
                dt = qubl.Qbei_Setting_Select(qe);
                
                string username = dt.Rows[0]["UserName"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("id").InnerText = username;                
                string password = dt.Rows[0]["Password"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("psw").InnerText = password;
                //Click at webpage of Login.
                fun.GetElement("input", "　ロ グ イ ン　", "value", webBrowser1).InvokeMember("click"); 
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                //Continue to webBrowser1_Login process.
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {            
                string janCode = dt011.Rows[0]["JANコード"].ToString();    
                string orderCode = dt011.Rows[0]["発注コード"].ToString();

                // Common Function of Qbei_ErrorInsert Process.     
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                
                // Common Function of WriteLog Process.
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog(ex, "011-", janCode, orderCode);
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
            string janCode = string.Empty;
            string orderCode = string.Empty;
            
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                //To Input string of body at WebBrowser Document of GetElementsByTagName("body")[0].
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains(" IDを入力してください") || body.Contains("パスワードを入力してください") || body.Contains("IDを正しく入力してください"))
                {
                    // Common Function of Qbei_ErrorInsert Process. 
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                    // Common Function of WriteLog Process.
                    ///<remark>
                    ///Write to process at Logfile.
                    ///</remark>
                    fun.WriteLog("Login Failed", "011-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    // Common Function of WriteLog Process.
                    ///<remark>
                    ///Write to process at Logfile.
                    ///</remark>
                    fun.WriteLog("Login success             ------", "011-");
   
                    orderCode = fun.ReplaceOrderCode(dt011.Rows[0]["発注コード"].ToString(), new string[] { "-" });
                    //WebBrowser Navigate of url.
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + orderCode + "00000");
                    //Continue to webBrowser1_ItemSearch process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                janCode = dt011.Rows[i]["JANコード"].ToString();             
                orderCode = dt011.Rows[i]["発注コード"].ToString();

                // Common Function of Qbei_ErrorInsert Process.            
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
               
                // Common Function of WriteLog Process.
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog(ex, "011-", janCode, orderCode);
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
                //To Input orderCode at Common Function of ReplaceOrderCode process.
                entity.orderCode = fun.ReplaceOrderCode(entity.orderCode, new string[] { "-" });
                //To Input purchaseURL.
                entity.purchaseURL = fun.url + "/index.php?action_goods=true&id=" + entity.orderCode + "00000";

                /// <remark>
                /// To Check of Condition at Input Data.
                /// </remark>
                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    //To Input body at Webpage Inspect of html.
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;

                    /// <remark>
                    /// To Check of body Condition at Webpage Inspect and Input Data.
                    /// </remark>
                    if (body.Contains("エラー [Error]"))
                    {
                        /// <remark>
                        /// To Check of stockDate and quantity.
                        /// </remark>
                        if (dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt011.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            //fun.Qbei_ErrorInsert(11, "マルイ", "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt011.Rows[i]["下代"].ToString();
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
                        try
                        {
                            //To Input html at Webpage Inspect.
                            string html = webBrowser1.Document.Body.InnerHtml;
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            //hdoc Documennt of html.
                            hdoc.LoadHtml(html);

                            /// <remark>
                            /// To Check of condition at stockDate and quantity.
                            /// </remark>
                            if ((hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[7]/td[2]/table/tbody/tr[1]/td/img") == null) && (hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]") == null))
                            {
                                // Common Function of Qbei_ErrorInsert Process.  
                                ///<remark>
                                ///Insert to Data of Qbei_ErrorLog Table.
                                ///</remark>
                                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                                // Common Function of WriteLog Process.   
                                ///<remark>
                                ///Write to process at Logfile.
                                ///</remark>
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "011-");
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                //To Input node1 at hdoc Documennt of html.
                                HtmlNode node1 = hdoc.DocumentNode.SelectSingleNode("div[6]//div[2]//div//table//tbody//tr[4]//td//table//tbody//tr//td[2]//table//tbody//tr//td//table//tbody//tr[7]//td[2]//table//tbody//tr//td//img");
                                //To Input alt at node1.
                                string alt = node1.GetAttributeValue("alt", string.Empty);
                                //To Input price at hdoc Documennt of html.
                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[6]/td[2]").InnerText;
                                //Replace "￥" , ","  at entity.price.
                                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);
                                //Convert to integer at entity.price.
                                entity.price = ((int)(Convert.ToDouble(entity.price) * 0.98)).ToString();
                                //To Input stockDate at hdoc Documennt of html.
                                string stockDate = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]").InnerText;
                                //To Input quantity at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown status"] .
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Equals("×") || alt.Equals("完売") ? "empty" : "unknown status";

                                /// <remark>
                                /// To Check of condition at stockDate .
                                /// </remark>
                                if (stockDate.Equals("-") || stockDate.Equals("未定"))
                                {
                                    //To Input stockDate at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown status"] .
                                    entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("完売") ? "2100-02-01" : "unknown status";
                                }
                                else
                                {
                                    //To Input date at Now.
                                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                    //To Input stockDate at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown date"] .
                                    entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? stockDate : alt.Equals("完売") ? "2100-02-01" : "unknown date";
                                }

                                /// <remark>
                                /// To Check of condition at dt011 of stockDate and quantity.
                                /// </remark>
                                if ((dt011.Rows[i]["在庫情報"].ToString().Contains("empty") || dt011.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                {
                                    /// <remark>
                                    /// To Check of condition at stockDate and quantity.
                                    /// </remark>
                                    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                    {                                      
                                        entity.qtyStatus = dt011.Rows[i]["在庫情報"].ToString();
                                        entity.stockDate = dt011.Rows[i]["入荷予定"].ToString();
                                        entity.price = dt011.Rows[i]["下代"].ToString();
                                    }
                                    //Common Function of Qbei_Inserts Process.
                                    ///<remark>
                                    ///Delete from Qbei_OrderData Table of Data.
                                    ///Insert to Qebei Table.
                                    ///</remark>   
                                    fun.Qbei_Inserts(entity);
                                }
                                else
                                    //2017/12/22 End
                                    //Common Function of Qbei_Inserts Process.
                                    ///<remark>
                                    ///Delete from Qbei_OrderData Table of Data.
                                    ///Insert to Qebei Table.
                                    ///</remark>   
                                    fun.Qbei_Inserts(entity);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Common Function of Qbei_ErrorInsert Process.  
                            ///<remark>
                            ///Insert to Data of Qbei_ErrorLog Table.
                            ///</remark>
                            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                            // Common Function of WriteLog Process.
                            ///<remark>
                            ///Write to process at Logfile.
                            ///</remark>
                            fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);
                        }
                    }
                }
                else
                { 
                    // Common Function of Qbei_ErrorInsert Process.  
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                }

                /// <remark>
                /// To Check of condition at dt011 rows .
                /// </remark>
                if (i < dt011.Rows.Count - 1)
                {
                    //To Input ordercode at Common Function of ReplaceOrderCode process.
                    string ordercode = fun.ReplaceOrderCode(dt011.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "在庫処分/empry/", "-", "在庫処分/good/", "在庫処分/small/", "在庫処分/empty/", "バラ注文できない為発注禁止/" });
                    //WebBrowser Navigate of url.
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + ordercode + "00000");
                    webBrowser1.ScriptErrorsSuppressed = true;
                    //To Continue webBrowser1_ItemSearch process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {         
                    qe.site = 11;
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
            catch (Exception ex)
            {
                // Common Function of Qbei_ErrorInsert Process.   
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                
                // Common Function of WriteLog Process.
                ///<remark>
                ///Write to process at Logfile.
                ///</remark>
                fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="pDisp"></param>
        /// <param name="URL"></param>
        /// <param name="Frame"></param>
        /// <param name="StatusCode"></param>
        /// <param name="Cancel"></param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt011.Rows[i]["JANコード"].ToString();           
            string orderCode = dt011.Rows[i]["発注コード"].ToString();

            // Common Function of Qbei_ErrorInsert Process. 
            ///<remark>
            ///Insert to Data of Qbei_ErrorLog Table.
            ///</remark>
            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

            // Common Function of WriteLog Process.
            ///<remark>
            ///Write to process at Logfile.
            ///</remark>
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "011-");
            Application.Exit();
            Environment.Exit(0);
        }        
    }
}
