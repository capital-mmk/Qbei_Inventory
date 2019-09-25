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

namespace _13ミズタニ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm013 : Form
    {        
        DataTable dt = new DataTable();        
        Qbeisetting_BL qubl = new Qbeisetting_BL();        
        Qbeisetting_Entity qe = new Qbeisetting_Entity();        
        CommonFunction fun = new CommonFunction();
        DataTable dt013 = new DataTable();        
        Qbei_Entity entity = new Qbei_Entity();
        int i = -1;
        public static string st = string.Empty;
        //To Input gridViewFormat is "GridView1_ctl{0}".
        string gridViewFormat = "GridView1_ctl{0}";

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm013()
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
            qe.site = 13;
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");            
            qe.flag = 1;

            //Input DataTable of Common Function Flag Table. 
            /// <remark>
            ///Select Flag from Site_setting Table. 
            /// </remark>
            DataTable dtflag = fun.SelectFlag(13);
            //To Check "FlagIsFinished" of Flag Number at Flag Table.
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
                fun.deleteData(13);

                //Common Function of ChangFlage Process.
                ///<remark>
                ///Change to flag is 1 at site_setting Table.
                ///</remark>
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
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
                fun.setURL("013");

                //Common Function of CreateFileAndFolder Process.
                fun.CreateFileAndFolder();

                //Common Function of Qbei_Delete Process.
                ///<remark>
                ///Insert Qbei_Backup Table where select from Qbei Table.
                ///After Delete to Qbei Table of AllData  and Qbei_Backup Table of Updated Date is Greater than 14days.
                ///</remark>
                fun.Qbei_Delete(13);

                //Common Function of Qbei_ErrorDelete Process.
                ///<remark>
                ///Insert Qbei_ErrorLog Backup Table where select from Qbei_ErrorLog Table.
                ///After Delete to Qbei_ErrorLog Table of AllData  and Qbei_ErrorLog_Backup Table of Date is less than 14days.
                ///</remark>
                fun.Qbei_ErrorDelete(13);

                // To Input dt013(Data Table) at Common Function of GetDatatable Process.       
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt013 = fun.GetDatatable("013");
                //2017/12/14 Start

                // To Input dt013(データテーブル) at Common Function of GetDatatable Process. 
                ///<remark>
                ///Get Data from Qbei_Log of CVS File .
                ///</remark>
                dt013 = fun.GetOrderData(dt013, "https://www.ordermz.jp/weborder/SyohinSearch.aspx", "013", string.Empty);

                // Common Function of GetTotalCount Process.
                ///<remark>
                ///Update to Site_setting Table of TotalCount.
                ///</remark> 
                fun.GetTotalCount("013");
                //2017/12/14 End
                ReadData();
            }
            catch (Exception) { }
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
            qe.SiteID = 13;

            // To Input Data Table at Connection Database of Qbei_Setting_Select Process.    
            /// <remark>
            /// Select Data from Site_Setting Table for Input to Dt Table .
            /// </remark>
            dt = qubl.Qbei_Setting_Select(qe);

            // To Input Common Funtion of url at Data Table of Url.
            fun.url = dt.Rows[0]["Url"].ToString();

            webBrowser1.AllowNavigation = true;
            //Check WebBrwser Navigate at Common Function of url process.
            webBrowser1.Navigate("https://www.ordermz.jp/weborder");
            //Continue to webBrowser1_Start process.
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Date Process
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool IsDate(string date)
        {
            try
            {
                //To Change DateTime of Date Format.
                Convert.ToDateTime(date.Trim());
                return true;
            }
            catch (Exception)
            { return false; }
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
                // Common Function of WriteLog Process. 
                fun.WriteLog("Navigation to Site Url success------", "013-");
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);                
                qe.SiteID = 13;

                // To Input Data Table at Connection Database of Qbei_Setting_Select Process. 
                /// <remark>
                /// Select Data from Site_Setting Table for Input to Dt Table .
                /// </remark>
                dt = qubl.Qbei_Setting_Select(qe);
                //To Input string of username at Data Table of "UserName".
                string username = dt.Rows[0]["UserName"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("tokuisakicode").InnerText = username;
                //To Input string of username at Data Table of "Password".
                string password = dt.Rows[0]["Password"].ToString();
                //Webpage Inspect of  CodeID.
                webBrowser1.Document.GetElementById("loginpasswd").InnerText = password;
                //Click at webpage of Login.
                webBrowser1.Document.GetElementById("btnLogin").InvokeMember("Click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                //Continue to webBrowser1_Login process.
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                // Common Function of Qbei_ErrorInsert Process.     
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[0]["JANコード"].ToString(), dt013.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

                // Common Function of WriteLog Process.  
                fun.WriteLog(ex.Message + dt013.Rows[0]["発注コード"].ToString(), "013-");
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
                //To Input string of  body at WebBrowser Document of GetElementsByTagName("body")[0].
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains(" 得意先コード、パスワードが正しくありません"))
                {
                    // Common Function of Qbei_ErrorInsert Process. 
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                    Application.Exit();
                }
                else
                {
                    // Common Function of WriteLog Process.
                    fun.WriteLog("Login success             ------", "013-");
                    //WebBrowser Navigate of url.
                    webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");
                    //Continue to webBrowser1_WaitForSearchPage process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                // Common Function of Qbei_ErrorInsert Process.            
                ///<remark>
                ///Insert to Data of Qbei_ErrorLog Table.
                ///</remark>
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[0]["JANコード"].ToString(), dt013.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

                // Common Function of WriteLog Process.
                fun.WriteLog(ex.Message + dt013.Rows[0]["発注コード"].ToString(), "013-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            //WebBrowser Navigate of url.
            webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");

            /// <remark>
            /// To Check of Condition at WebBrowser of Url is contain "SyohinSearch.aspx".
            /// </remark>
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                //Continue to webBrowser1_ItemSearch process.
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }


        /// <summary>
        /// Inspection of item at Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                //WebBrowser of HtmlDocument.
                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;

                /// <remark>
                /// To Check of condition at dt013 rows .
                /// </remark>
                if (i < dt013.Rows.Count - 1)
                {
                    //To Input orderCode at dt013 of "発注コード".
                    orderCode = dt013.Rows[++i]["発注コード"].ToString().Trim();
                    //string orderCode = fun.ReplaceOrderCode(dt013.Rows[++i]["発注コード"].ToString(), new string[] {"在庫限り発注禁止", "在庫処分/inquiry/", "在庫処分/empry/-", "在庫処分good", "在庫処分empryinquiry", "在庫処分/empty/", "在庫処分small", 
                    //                                                                     "在庫限り発注禁止inquiry", "在庫処分/empry/", "在庫処分empry", "東特価のため完売", "在庫処分small", 
                    //                                                                     "在庫処分empry在庫処分empryempty", "在庫処分empry在庫処分empryinquiry", "在庫処分good",
                    //                                                                     "バラ注文できない為発注禁止/", "在庫処分/","inquiry","empty","/////","////","//","/" });
                    
                    //To Input ordercode at HtmlDocument of "keyword" ;
                    doc.GetElementById("keyword").SetAttribute("Value", orderCode);
                    //Click at webpage of "btnSearch".
                    webBrowser1.Document.GetElementById("btnSearch").InvokeMember("Click");
                    //Continue to webBrowser1_ItemProcessing process.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {                   
                    qe.site = 13;                   
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //Common Function of ChangeFlag Process.
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
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[i]["JANコード"].ToString(), orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

                // Common Function of WriteLog Process.
                fun.WriteLog(ex.Message + orderCode, "013-");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //while (string.IsNullOrEmpty(tmpOrderCode))
            //{
            //    js.ExecuteScript("document.getElementById('gvSyohin_ctl02_syohincode').value='" + orderCode + "';");
            //    js.ExecuteScript("javascript:setTimeout(__doPostBack('gvSyohin$ctl02$syohincode',''), 0);");
            //    System.Threading.Thread.Sleep(1000);
            //    tmpOrderCode = chrome.FindElement(By.Id("gvSyohin_ctl02_syohincode")).GetAttribute("value");
            //}
            entity = new Qbei_Entity();
            string color = string.Empty;
            HtmlNode colorpath;
            HtmlNode node;
            string qty = string.Empty;
            int intCnt = 0;
            string strHtml;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);    
            entity.janCode = dt013.Rows[i]["JANコード"].ToString();      
            entity.partNo = dt013.Rows[i]["自社品番"].ToString();
            //To Input partNo at Common Function of getCurrentDate process.
            entity.makerDate = fun.getCurrentDate();           
            entity.reflectDate = dt013.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt013.Rows[i]["発注コード"].ToString().Trim();
            //entity.orderCode = fun.ReplaceOrderCode(entity.orderCode, new string[] {"在庫限り発注禁止", "在庫処分/inquiry/", "在庫処分/empry/-", "在庫処分good", "在庫処分empryinquiry", "在庫処分/empty/", "在庫処分small", 
            //                                                                         "在庫限り発注禁止inquiry", "在庫処分/empry/", "在庫処分empry", "東特価のため完売", "在庫処分small", 
            //                                                                         "在庫処分empry在庫処分empryempty", "在庫処分empry在庫処分empryinquiry", "在庫処分good",
            //                                                                         "バラ注文できない為発注禁止/", "在庫処分/","inquiry","empty","/////","////","//","/" });
            

            //To Input purchase at WebBrowser of Url.
            entity.purchaseURL = webBrowser1.Url.ToString();
            //To Input string body at WebBrowser Document of GetElementsByTagName("html").
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;    
            entity.siteID = 13;
            entity.sitecode = "013";
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);

            /// <remark>
            /// To Check of condition at body of html.
            /// </remark>
            if (body.Contains("検索条件に該当する商品は、見つかりませんでした"))
            {
                entity.qtyStatus = "empty";
                entity.price = dt013.Rows[i]["下代"].ToString();


                /// <remark>
                /// To Check of condition at dt013 of stockDate and quantity.
                /// </remark>
                if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                {
                    entity.stockDate = "2100-01-10";
                }
                else
                { entity.stockDate = "2100-02-01"; }
                //Common Function of Qbei_Inserts Process.
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
                    /// To Check of condition at WebBrowser Document of GetElementById is null and Html DocumentNode is null. 
                    /// </remark>
                    if (webBrowser1.Document.GetElementById("GridView1_ctl02_Label1") == null && (hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]") == null))
                    {
                        // Common Function of Qbei_ErrorInsert Process. 
                        ///<remark>
                        ///Insert to Data of Qbei_ErrorLog Table.
                        ///</remark>
                        fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

                        // Common Function of WriteLog Process.
                        fun.WriteLog("Access Denied! " + entity.orderCode, "013-");
                        Application.Exit();
                    }
                    else
                    {
                        //To Input strHtml at webBrowser Document of GetElementById.
                        strHtml = webBrowser1.Document.GetElementById("GridView1").InnerHtml;
                        //hdoc Documennt of html.
                        hdoc.LoadHtml(strHtml);

                        //To Input intCnt at Html DocumentNode.
                        intCnt = hdoc.DocumentNode.SelectNodes("/tbody/tr").Count;

                        //if (intCnt > 1)
                        //{
                        //    for (int j = 2; j <= intCnt; j++)
                        //    {
                        //        if (webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_syohincode").InnerText.Equals(entity.orderCode))
                        //        {
                        //            qty = webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_Label1").InnerText;
                        //            entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";
                        //            entity.price = webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_hanbaikakaku").InnerText;
                        //            entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                        //            node = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]");
                        //            if (node != null)
                        //                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]").InnerText;
                        //            colorpath = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[5]");
                        //            color = colorpath.GetAttributeValue("style", string.Empty);
                        //            break;
                        //        }
                        //    }
                        //}


                        /// <remark>
                        /// To Check of condition at intCnt .
                        /// </remark>
                        if (intCnt > 1)
                        {
                            /// <remark>
                            /// To Check of condition j at j is greater than equal to intCnt and increase of j is 1.
                            /// </remark>
                            for (int j = 2; j <= intCnt; j++)
                            {
                                string gridView = string.Empty;
                                /// <remark>
                                /// To Check of condition j at j is greater than 1 and increase of j is less than 10.
                                /// </remark>
                                if (j > 1 && j < 10)
                                {
                                    gridView = string.Format(gridViewFormat, "0" + j);
                                }
                                else
                                {
                                    gridView = string.Format(gridViewFormat, j);
                                }

                                /// <remark>
                                /// To Check of condition at webBrowser Document GetElementById of code is equal Field of orderCode.
                                /// </remark>
                                if (webBrowser1.Document.GetElementById(gridView + "_syohincode").InnerText.Equals(entity.orderCode))
                                {
                                    //To Input quantity at webBrowser Document GetElementById.
                                    qty = webBrowser1.Document.GetElementById(gridView + "_Label1").InnerText;
                                    //To Input quantity at Check of qty Condition ["○" , "△" , "×" , "☆" ,  "★" ,"？" , "unknown status"] .
                                    entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";
                                    //To Input price at webBrowser Document GetElementById.
                                    entity.price = webBrowser1.Document.GetElementById(gridView + "_hanbaikakaku").InnerText;
                                    //To Input price at replace ["￥" , "," , "円"] of  entity.price.
                                    entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                                    //To Input node at html.SelectSingleNode.
                                    node = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]");

                                    /// <remark>
                                    /// To Check of condition at node is not null.
                                    /// </remark>
                                    if (node != null)
                                        //To Input stockDate at html.SelectSingleNode.
                                        entity.stockDate = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]").InnerText;
                                    //To Input colorpath at html.SelectSingleNode.
                                    colorpath = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[5]");
                                    //To Input color at colorpath of GetAttributeValue.
                                    color = colorpath.GetAttributeValue("style", string.Empty);
                                    break;
                                }
                            }

                            /// <remark>
                            /// To Check of condition at stockDate and quantity is string of Null or Empty .
                            /// </remark>
                            if (string.IsNullOrEmpty(entity.qtyStatus) && string.IsNullOrEmpty(entity.stockDate))
                                {
                                entity.price = dt013.Rows[i]["下代"].ToString();
                                entity.qtyStatus = "empty";
                                entity.stockDate = "2100-02-01";
                            }
                        }


                        //string qty = webBrowser1.Document.GetElementById("GridView1_ctl02_Label1").InnerText;
                        //entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";

                        //entity.price = webBrowser1.Document.GetElementById("GridView1_ctl02_hanbaikakaku").InnerText;
                        //entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);

                        //HtmlNode node = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]");

                        //if (node != null)
                        //    entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]").InnerText;
                        //HtmlNode colorpath = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[5]");
                        //string color = colorpath.GetAttributeValue("style", string.Empty);


                        /// <remark>
                        /// To Check of condition at stockDate .
                        /// </remark>
                        if (IsDate(entity.stockDate))
                            //To Input stockDate at replace("/", "-") of  entity.stockDate.
                            entity.stockDate = entity.stockDate.Replace("/", "-");

                        /// <remark>
                        /// To Check of condition at color is contains red.
                        /// </remark>
                        else if (color.Contains("red"))
                        {
                            /// <remark>
                            /// To Check of condition at quantity is ("▲") or ("×").
                            /// </remark>
                            if (qty.Equals("▲") || qty.Equals("×"))
                                entity.stockDate = "2100-02-01";

                            /// <remark>
                            /// To Check of condition at quantity is ("★") or ("？").
                            /// </remark>
                            else if (qty.Equals("★") || qty.Equals("？"))
                                entity.stockDate = "2100-01-01";
                        }
                        else
                        {
                            //To Input stockDate at quantity ("▲") or ("×") or ("○") is "2100-01-01" (OR) entity.stockDate.
                            entity.stockDate = qty.Equals("○") || qty.Equals("▲") || qty.Equals("×") ? "2100-01-01" : entity.stockDate;
                        }

                        //entity.stockDate = color.Contains("red") ? "2100-02-01" : (stockDate == "-" || string.IsNullOrWhiteSpace(stockDate) || stockDate.Equals("&nbsp;")) ? "2100-01-01" : stockDate;
                       
                        /// <remark>
                        /// To Check of condition at stockDate is contain ("月中旬")  or ("月上旬").
                        /// </remark>
                        if (entity.stockDate.Contains("月中旬") || entity.stockDate.Contains("月上旬"))
                        {
                            //To Input stockDate at replace ("次回","入荷") of entity.stockDate.
                            entity.stockDate = entity.stockDate.Replace("次回", "").Replace("入荷", "");
                            string day = string.Empty;

                            /// <remark>
                            /// To Check of condition at stockDate is contain ("中旬").
                            /// </remark>
                            if (entity.stockDate.Contains("中旬"))
                                day = "20";

                            /// <remark>
                            /// To Check of condition at stockDate is contain ("上旬") or ("月予定").
                            /// </remark>
                            else if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("月予定"))
                                day = "10";

                            /// <remark>
                            /// To Check of condition at stockDate is contain ("下旬").
                            /// </remark>
                            else if (entity.stockDate.Contains("下旬"))
                            {
                                /// <remark>
                                /// To Check of condition at stockDate is contain ("2月").
                                /// </remark>
                                if (entity.stockDate.Contains("2月"))
                                    day = "28";
                                day = "30";
                            }


                            else day = "25";
                            //To Input month at split('月') of entity.stockDate.
                            string month = entity.stockDate.Split('月')[0];

                            string year = DateTime.Now.ToString("yyyy");
                            //To Input dt at convert of ToDateTime(year + "-" + month + "-" + day).
                            DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                            /// <remark>
                            /// To Check of condition at dt is less than DateTime of now.
                            /// </remark>
                            if (dt < DateTime.Now)
                                //To Input dt at add to 1 year of dt.
                                dt = dt.AddYears(1);

                            entity.stockDate = dt.ToString("yyyy-MM-dd");

                        }
                        /// <remark>
                        /// To Check of condition at stockDate is contain ("月末～").
                        /// </remark>
                        else if (entity.stockDate.Contains("月末～"))
                        {
                            entity.stockDate = "未定(=2100-01-01)";
                        }

                        /// <remark>
                        /// To Check of condition at stockDate is contain("月末").
                        /// </remark>
                        else if (entity.stockDate.Contains("月末"))
                        {
                            string day = "25";
                            //To Input stockDate at replace ("月末","予定") of entity.stockDate.
                            string month = entity.stockDate.Replace("月末", string.Empty).Replace("予定", string.Empty);

                            string year = DateTime.Now.ToString("yyyy");
                            //To Input dt at convert of ToDateTime(year + "-" + month + "-" + day).
                            DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                            /// <remark>
                            /// To Check of condition at dt is less than DateTime of now.
                            /// </remark>
                            if (dt < DateTime.Now)
                                //To Input dt at add to 1 year of dt.
                                dt = dt.AddYears(1);

                            entity.stockDate = dt.ToString("yyyy-MM-dd");
                        }

                        /// <remark>
                        /// To Check of condition at stockDate is contain("未定").
                        /// </remark>
                        else if (entity.stockDate.Contains("未定"))
                        {
                            entity.stockDate = "2100-01-01";
                        }

                        /// <remark>
                        /// To Check of condition at quantity is Equals("☆") (And) stockDate is Null or WhiteSpace .
                        /// </remark>
                        else if ((qty.Equals("☆")) && string.IsNullOrWhiteSpace(entity.stockDate)) { entity.stockDate = "2100-01-10"; }

                        /// <remark>
                        /// To Check of condition at stockDate is contain("在庫限り").
                        /// </remark>
                        else if (entity.stockDate.Contains("在庫限り"))
                            entity.stockDate = "2100-02-01";
                        //2018-08-14 Start

                        /// <remark>
                        /// To Check of condition at stockDate is contain("月以降").
                        /// </remark>
                        else if (entity.stockDate.Contains("月以降"))
                        {
                            //To Input stockDate at replace (Regex,Empty) of entity.stockDate.
                            entity.stockDate = Regex.Replace(entity.stockDate, "[^0-9]", string.Empty);
                            //To Input stockDate at Change to (DateTimeYear of Now) of entity.stockDate.
                            entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(entity.stockDate), 30).ToString("yyyyMMdd");
                        }
                        //2018-08-14 End
                        //To Input stockDate at replace ("/", "-") of entity.stockDate.
                        entity.stockDate = entity.stockDate.Replace("/", "-");
                        //2018/1/12

                        /// <remark>
                        /// To Check of condition at dt013 of stockDate and quantity.
                        /// </remark>
                        if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") || dt013.Rows[i]["在庫情報"].ToString().Contains("inquriry")) && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            /// <remark>
                            /// To Check of condition at stockDate and quantity.
                            /// </remark>
                            if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                            {
                                entity.qtyStatus = dt013.Rows[i]["在庫情報"].ToString();
                                entity.price = dt013.Rows[i]["下代"].ToString();       
                                entity.stockDate = dt013.Rows[i]["入荷予定"].ToString();
                            }
                            //Common Function of Qbei_Inserts Process.
                            fun.Qbei_Inserts(entity);
                        }

                        else
                            //2018/1/12
                            //Common Function of Qbei_Inserts Process.
                            fun.Qbei_Inserts(entity);
                    }
                }
                catch (Exception ex)
                {
                    // Common Function of Qbei_ErrorInsert Process. 
                    ///<remark>
                    ///Insert to Data of Qbei_ErrorLog Table.
                    ///</remark>
                    fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

                    // Common Function of WriteLog Process.
                    fun.WriteLog(ex.Message + entity.orderCode, "013-");
                }
            }
            //Click at webpage of "btnClear".
            webBrowser1.Document.GetElementById("btnClear").InvokeMember("Click");
            webBrowser1.ScriptErrorsSuppressed = true;
            //To Continue webBrowser1_ItemSearch process.
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            //}
        }

        //NavigateErrorの　表示。
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            // Common Function of Qbei_ErrorInsert Process.  
            ///<remark>
            ///Insert to Data of Qbei_ErrorLog Table.
            ///</remark>
            fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Access Denied!", dt013.Rows[i]["JANコード"].ToString(), dt013.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");

            // Common Function of WriteLog Process.
            fun.WriteLog(StatusCode.ToString() + " " + dt013.Rows[i]["発注コード"].ToString(), "013-");
            Application.Exit();
        }
    }
}
