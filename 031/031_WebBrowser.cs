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

namespace _031アキボウ
{
    public partial class frm031 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt031 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        bool exit = false;
        public static string st = string.Empty;
        public frm031()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //st = qe.starttime;
                qe.site = 31;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(31);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(31);
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
                fun.WriteLog(ex, "031-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        public void StartRun()
        {
            try
            {
                //  webBrowser1.ScriptErrorsSuppressed = true;
                fun.setURL("031");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(31);
                fun.Qbei_ErrorDelete(31);
                dt031 = fun.GetDatatable("031");
                dt031 = fun.GetOrderData(dt031, "https://joto-order.jp/jotwebb2b/itemList/searchItemList?searchKeiWord=", "031", "");
                fun.GetTotalCount("031");
                ReadData();
            }            
            catch (Exception ex)
            {
                fun.WriteLog(ex, "031-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void ReadData()
        {
            qe.SiteID = 31;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();

            webBrowser1.AllowNavigation = true;
            Thread.Sleep(5000);
            webBrowser1.Navigate(fun.url);
            Thread.Sleep(5000);
            webBrowser1.Refresh();
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);

        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                //  webBrowser1.ScriptErrorsSuppressed = true;
                fun.WriteLog("Navigation to Site Url success------", "031-");
                qe.SiteID = 31;

                dt = qubl.Qbei_Setting_Select(qe);
                fun.user = dt.Rows[0]["UserName"].ToString();
                Thread.Sleep(5000);
                //  webBrowser1.Document.GetElementsByTagName("input")[0].InnerHtml= fun.user;
                fun.GetElement("input", "customer_code", "name", webBrowser1).InnerText = fun.user;
                // webBrowser1.Document.GetElementByName("customer_code").InnerText = username;
                //webBrowser1.Document.GetElementsByTagName("input")[1].InnerText = fun.user;
                fun.GetElement("input", "login_id", "name", webBrowser1).InnerText = fun.user;

                string password = dt.Rows[0]["Password"].ToString();
                //  webBrowser1.Document.GetElementsByTagName("input")[2].InnerText = password;
                fun.GetElement("input", "password", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "login", "name", webBrowser1).InvokeMember("click");
                Thread.Sleep(10000);
                // fun.GetElement("input", "ログイン", "alt", webBrowser1).InvokeMember("click");
                webBrowser1.Refresh();
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt031.Rows[0]["JANコード"].ToString();
                string orderCode = dt031.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");                
                fun.WriteLog(ex, "031-", janCode, orderCode);
                fun.Qbei_Maker_Insert("031", dt031);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("入力されていません。") || body.Contains("得意先コードまたはIDまたはパスワードが誤っています"))
                {
                    fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
                    fun.WriteLog("Login Faliled", "031-");
                    fun.Qbei_Maker_Insert("031", dt031);
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    escapeBlankOrderCode();
                    if (exit)
                    {
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "031-");
                        orderCode = dt031.Rows[i]["発注コード"].ToString();

                        webBrowser1.Navigate("http://weborder.akibo.jp/item_detail/item_detail.html?goods_code=" + orderCode);
                        Thread.Sleep(2000);
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                    }
                }
            }
            catch (Exception ex)
            {
                string janCode = dt031.Rows[i]["JANコード"].ToString();
                fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
                fun.WriteLog(ex, "031-", janCode, orderCode);
                fun.Qbei_Maker_Insert("031", dt031);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void escapeBlankOrderCode()
        {
            while (string.IsNullOrWhiteSpace((dt031.Rows[i]["発注コード"].ToString())))
            {
                entity = new Qbei_Entity();
                entity.siteID = 31;
                entity.janCode = dt031.Rows[i]["JANコード"].ToString();
                entity.orderCode = dt031.Rows[i]["発注コード"].ToString();
                i++;
                fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
                if (i >= dt031.Rows.Count)
                {
                    fun.Qbei_Maker_Insert("031", dt031, i);

                    qe.site = 31;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    exit = true;
                    break;
                }
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 31;
            entity.sitecode = "031";
            entity.janCode = dt031.Rows[i]["JANコード"].ToString();
            entity.partNo = dt031.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt031.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt031.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = "https://weborder.akibo.jp/item_detail/item_detail.html?goods_code=" + entity.orderCode;
            
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            if (!(body.Contains("上代")))
            {
                if (dt031.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt031.Rows[i]["在庫情報"].ToString().Contains("empty"))
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-01-10";
                    entity.price = dt031.Rows[i]["下代"].ToString();
                }
                else
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";
                    entity.price = dt031.Rows[i]["下代"].ToString();
                }
                fun.Qbei_Inserts(entity);
                // fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
            }
            else
            {
                try
                {
                    string html = webBrowser1.Document.GetElementsByTagName("html")[0].OuterHtml;
                    html = html.Replace("</DIV></FORM>", "<DIV></DIV></FORM>");

                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    //Check Element Exist or not
                    if (hdoc.DocumentNode.SelectSingleNode("/html/body/div/div/table/tbody/tr/td[2]/table[2]/tbody/tr[2]/td[1]") == null)
                    {
                        fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "031-");
                        fun.Qbei_Maker_Insert("031", dt031, i);
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        var qty = hdoc.DocumentNode.SelectSingleNode("/html/body/div/div/table/tbody/tr/td[2]/table[2]/tbody/tr[2]/td[1]").InnerText;

                        entity.qtyStatus = qty.Contains("○") ? "good" : qty.Contains("△") || qty.Contains("▲") ? "small" : qty.Contains("予約受付中") || qty.Contains("在庫なし") || qty.Contains("受付終了") ? "empty" : "unknown status";
                        entity.stockDate = qty.Contains("○") || qty.Contains("△") || qty.Contains("▲") || qty.Contains("予約受付中") || qty.Contains("在庫なし") ? "2100-01-01" : qty.Contains("受付終了") ? "2100-02-01" : "unknown date";

                        //entity.qtyStatus = qty.Contains("○") ? "good" : qty.Contains("△") || qty.Contains("▲") ? "small" : qty.Contains("予約受付中") || qty.Contains("在庫なし") || qty.Contains("受付終了") ? "empty" : "unknown status";
                        //entity.stockDate = qty.Contains("○") || qty.Contains("△") || qty.Contains("▲") || qty.Contains("予約受付中") || qty.Contains("在庫なし") ? "2100-01-01" : qty.Contains("受付終了") ? "2100-02-01" : "unknown date";
                        if (entity.stockDate.Contains("2月"))
                        {
                            //entity.stockDate = "2018-02-28";
                            entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                        }
                        //entity.price = hdoc.DocumentNode.SelectSingleNode("/html/body/div/div/table/tbody/tr/td[2]/table[2]/tbody/tr[1]/td[1]").InnerText;
                        entity.price = dt031.Rows[i]["下代"].ToString();

                        if ((dt031.Rows[i]["在庫情報"].ToString().Contains("empty") || dt031.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt031.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                            {
                                entity.qtyStatus = dt031.Rows[i]["在庫情報"].ToString();
                                entity.price = dt031.Rows[i]["下代"].ToString();
                                entity.stockDate = dt031.Rows[i]["入荷予定"].ToString();
                            }
                            fun.Qbei_Inserts(entity);
                        }
                        else
                            fun.Qbei_Inserts(entity);
                    }
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");                    
                    fun.WriteLog(ex, "031-", entity.janCode, entity.orderCode);
                }
            }

            if (++i < dt031.Rows.Count)
            {
                escapeBlankOrderCode();
                if (exit)
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    string ordercode = dt031.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate("http://weborder.akibo.jp/item_detail/item_detail.html?goods_code=" + ordercode);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            else
            {
                fun.Qbei_Maker_Insert("031", dt031, i);

                qe.site = 31;
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
            string janCode = dt031.Rows[i]["JANコード"].ToString();
            string orderCode = dt031.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(31, fun.GetSiteName("031"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "031");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "031-");

            fun.Qbei_Maker_Insert("031", dt031, i);
            Application.Exit();
            Environment.Exit(0);
        }
    }
}