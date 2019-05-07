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
    public partial class frm034 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt034 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public frm034()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 34;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(34);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
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
                            string alt = webBrowser1.Document.GetElementById("spec_stock_msg").InnerText;
                            entity.qtyStatus = alt.Contains("○") ? "good" : alt.Contains("△") || alt.Contains("在庫限り") ? "small" : alt.Contains("×") ? "empty" : "NO STATUS CODE";

                            entity.price = hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[3]/p[2]/strong").InnerText;
                            entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);

                            string date = hdoc.DocumentNode.SelectSingleNode("div[2]/div/div[2]/div/div[1]/div[2]/div/div[2]/div[4]/div[1]/div[3]").InnerText;
                            
                            if (string.IsNullOrWhiteSpace(date))
                                entity.stockDate = "2100-01-01";
                            else if (date.Contains("在庫限り") || alt.Contains("×在庫なし"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            else
                                entity.stockDate = date.Replace("入荷予定日", "");
                            entity.stockDate = entity.stockDate.Replace("\r\n", "");
                            
                            if (entity.stockDate.Contains("2月"))
                            {   
                                entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                            }
                            if (alt.Contains("完売御礼"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            //2018/01/17 Start
                            if ((dt034.Rows[i]["在庫情報"].ToString().Contains("empty") || dt034.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt034.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                {
                                    entity.qtyStatus = dt034.Rows[i]["在庫情報"].ToString();
                                    entity.stockDate = dt034.Rows[i]["入荷予定"].ToString();
                                    entity.price = dt034.Rows[i]["下代"].ToString();
                                }
                            }
                            //2018/01/17 End
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
