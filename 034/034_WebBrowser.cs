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

namespace _34シマノ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm034 : Form
    {
        
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt034 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm034()
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
                qe.site = 34;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(34);
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
                    fun.deleteData(34);
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
                fun.WriteLog(ex, "034-");
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
                fun.setURL("034");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(34);
                fun.Qbei_ErrorDelete(34);
                dt034 = fun.GetDatatable("034");
                dt034 = fun.GetOrderData(dt034, "https://sips.shimano.co.jp/front/g/g", "034", string.Empty);
                fun.GetTotalCount("034");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "034-");
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
            qe.SiteID = 34;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.ScriptErrorsSuppressed = true;
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

                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "034-");
                qe.SiteID = 34;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                string password = dt.Rows[0]["Password"].ToString();
                HtmlElementCollection tags = webBrowser1.Document.Body.GetElementsByTagName("input");
                foreach (HtmlElement t in tags)
                {
                    if (t.GetAttribute("name").Equals("uid"))
                    {
                        t.SetAttribute("value", username);
                    }
                    if (t.GetAttribute("name").Equals("pwd"))
                    {
                        t.SetAttribute("value", password);
                    }
                }
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt034.Rows[0]["JANコード"].ToString();
                string orderCode = dt034.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                fun.WriteLog(ex, "034-", janCode, orderCode);

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
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください。"))
                {
                    fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Login Failed", dt034.Rows[0]["JANコード"].ToString(), dt034.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                    fun.WriteLog("Login Failed", "034-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "034-");
                    orderCode = dt034.Rows[i]["発注コード"].ToString();
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.Navigate(fun.url + "/front/g/g" + orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt034.Rows[0]["JANコード"].ToString();
                orderCode = dt034.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                fun.WriteLog(ex, "034-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
          
            string orderCode = dt034.Rows[i]["発注コード"].ToString();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(fun.url + "/front/g/g" + orderCode);
            if (webBrowser1.Url.ToString().Contains("/front/g/g"))
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);                
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
            entity.siteID = 34;
            entity.sitecode = "034";
            entity.janCode = dt034.Rows[i]["JANコード"].ToString();
            entity.partNo = dt034.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt034.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt034.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/front/g/g" + entity.orderCode;
            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("申し訳ございません。"))
                {
                    //2018/01/17 Start
                    if (dt034.Rows[i]["在庫情報"].ToString().Contains("empty") && dt034.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        entity.qtyStatus = dt034.Rows[i]["在庫情報"].ToString();
                        entity.stockDate = dt034.Rows[i]["入荷予定"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                    }
                    entity.price = dt034.Rows[i]["下代"].ToString();
                    fun.Qbei_Inserts(entity);
                    
                    //2018/01/17 End
                }
                else
                {
                    try
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        //Check Element Exist or not
                        if (webBrowser1.Document.GetElementById("spec_stock_msg") == null && (hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[4]/div[1]/div[3]") == null))
                        {
                            fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "034-");                           
                            
                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            //<remark 5/12/2019(更新)>
                            // DIV 在庫状態
                            string alt = webBrowser1.Document.GetElementById("spec_stock_msg").InnerText;
                            // DIV 在庫限り
                            string only = hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[4]/div[1]/div[2]").InnerText;
                            // 在庫状態は、在庫限りの表示がある時=small、それ以外は　〇を含む時＝good、△を含む時＝small、✖を含む時＝empty
                            //entity.qtyStatus = only.Contains("在庫限り") ? "small" : alt.Contains("○") ? "good" : alt.Contains("△") ? "small" : alt.Contains("×") ? "empty" : "NO STATUS CODE";
                            //<remark qtyStatus(編集) 2020/1/7 Start>
                            //entity.qtyStatus = alt.Contains("×") ? "empty" :  only.Contains("在庫限り") ? "small" : alt.Contains("○") ? "good" : alt.Contains("△") ? "small" : "NO STATUS CODE";
                            //</remark 2020/1/7 End>
                            //<remark qtyStatus(編集) 2020/5/19 Start>
                            entity.qtyStatus = alt.Contains("×") ? "empty" : only.Contains("在庫限り") ? "small" : alt.Contains("○") ? "good" : alt.Contains("△") ? "empty" : "NO STATUS CODE";
                            //</remark 2020/5/19 End>
                            // 価格は太文字の販売価格(税抜き)
                            entity.price = hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[3]/p[2]/strong").InnerText;
                            entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                            // DIV 入荷予定日
                            string date = hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[4]/div[1]/div[3]").InnerText;

                            // 入荷予定日がNULLの時
                            if (string.IsNullOrWhiteSpace(date))
                            {
                                // 在庫限りの表示がある時は「2100-02-01：完売」
                                //if (only.Contains("在庫限り"))
                                //{
                                //    entity.stockDate = "2100-02-01";
                                //}
                                // 在庫限りの表示がない時は全て「2100-01-01：未定」
                                //else
                                //{
                                //    entity.stockDate = "2100-01-01";
                                //}
                                //<remark stockdate(編集) 2020/5/19 Start>
                                if (alt.Contains("○"))
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                                else
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                //</remark 2020/5/19 End>
                            }
                            // 入荷予定日がNULLでない時
                            else
                            {
                                // 在庫限りの表示がある時は「2100-02-01：完売」
                                if (only.Contains("在庫限り"))
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                // それ以外の場合は、入荷予定日をそのまま採用（但し、空白・改行等の文字は除去し、yyyy-mm-dd形式にすること）
                                else
                                {
                                    entity.stockDate = date.Replace("入荷予定日", "");
                                    entity.stockDate = entity.stockDate.Replace("\r\n", "");
                                    entity.stockDate = entity.stockDate.Trim();
                                    entity.stockDate = entity.stockDate.Replace("/", "-");
                                }
                            }
                            //</remark end>

                            if (entity.stockDate.Contains("2月"))
                            {   
                                entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                            }
                            if (alt.Contains("完売御礼"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            //2018/01/17 Start
                            //<remark Close Logic 2020/25/22 Start>
                            //if ((dt034.Rows[i]["在庫情報"].ToString().Contains("empty") || dt034.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt034.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            //{
                            //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                            //    {
                            //        entity.qtyStatus = dt034.Rows[i]["在庫情報"].ToString();
                            //        entity.stockDate = dt034.Rows[i]["入荷予定"].ToString();
                            //        entity.price = dt034.Rows[i]["下代"].ToString();
                            //    }
                            //}
                            //2018/01/17 End
                            //</reamark 2020/25/22 End>
                            fun.Qbei_Inserts(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");                        
                        fun.WriteLog(ex, "034-", entity.janCode, entity.orderCode);
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
            }
            if (i < dt034.Rows.Count - 1)
            {   
                string ordercode = dt034.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/front/g/g" + ordercode);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 34;
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
            string janCode = dt034.Rows[i]["JANコード"].ToString();
            string orderCode = dt034.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");            
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "034-");            

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
