using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _143
{
    public partial class frm143 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt143 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        public frm143()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 143;
            //st = qe.starttime;
            qe.flag = 0;
            DataTable dtflag = fun.SelectFlag(143);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {
                fun.ChangeFlag(qe);
                fun.WriteLog("Flag0 ------", "143--");
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(143);
                fun.ChangeFlag(qe);
                fun.WriteLog("Flag1 ------", "143--");
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public void StartRun()
        {
            try
            {
                fun.setURL("143");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(143);
                fun.Qbei_ErrorDelete(143);
                dt143 = fun.GetDatatable("143");
                dt143 = fun.GetOrderData(dt143, "http://www.podium-edi.com//goods/goods_list.html", "143", "");
                fun.GetTotalCount("143");
                ReadData();
            }
            catch (Exception) { }
        }

        private void ReadData()
        {
            qe.SiteID = 143;
            dt = qubl.Qbei_SettingSelect(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.Navigate(fun.url + "/login");
            fun.WriteLog("Go to Url------", "143--");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "143--");
                qe.SiteID = 143;
                dt = qubl.Qbei_SettingSelect(qe);
                string code = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "customer_code", "name", webBrowser1).InnerText = code;
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "login_id", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "password", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "login", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, dt143.Rows[0]["JANコード"].ToString(), dt143.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex.Message, "143-");
                Application.Exit();
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string url = webBrowser1.Url.ToString();
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("IDまたはパスワードが誤っています"))
                {
                    fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                    fun.WriteLog("Login Failed", "143-");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "143--");
                    // webBrowser1.Navigate("https://www.podium-edi.com/index.html");
                    webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, dt143.Rows[0]["JANコード"].ToString(), dt143.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex.Message, "143-");
                Application.Exit();
            }
        }

        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = webBrowser1.Url.ToString();
            webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
            if (webBrowser1.Url.ToString().Contains("goods_list.html"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (i <= dt143.Rows.Count - 1)
                {
                    string Url = webBrowser1.Url.ToString();
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    string orderCode = dt143.Rows[i]["発注コード"].ToString();

                    fun.GetElement("input", "15", "maxlength", webBrowser1).InnerText = orderCode;

                    fun.GetElement("input", "検索", "alt", webBrowser1).InvokeMember("Click");

                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 143;
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
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, dt143.Rows[0]["JANコード"].ToString(), dt143.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex.Message + dt143.Rows[0]["発注コード"].ToString(), "143-");
                Application.Exit();
            }

        }
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection hc;
            string strUrl = string.Empty;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            entity.janCode = dt143.Rows[i]["JANコード"].ToString();
            entity.partNo = dt143.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt143.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt143.Rows[i]["発注コード"].ToString();

            entity.purchaseURL = fun.url + "/goods/goods_list.html";
            entity.siteID = 143;
            entity.sitecode = "143";

            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("該当データはありません。"))
                {
                    if (dt143.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt143.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt143.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt143.Rows[i]["下代"].ToString();
                    }
                    fun.Qbei_Inserts(entity);
                }
                else
                {
                    try
                    {
                        string stockdate = string.Empty;
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        string url = webBrowser1.Url.ToString();
                        body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerHtml;
                        hc = webBrowser1.Document.GetElementsByTagName("table");

                        //Check Element Exist or not
                        if (hc.Count == 0)
                        {
                            fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "143-");
                            Application.Exit();
                        }

                        foreach (HtmlElement a in hc)
                        {
                            if (a.GetAttribute("className").Contains("item-table"))
                            {
                                string aa = a.InnerHtml;
                                hdoc.LoadHtml(aa);
                                break;
                            }
                        }
                        //Check Element Exist or not
                        if (hdoc.DocumentNode.SelectSingleNode("/tbody/tr[3]/td[6]/table/tbody/tr[2]/td") == null && (hdoc.DocumentNode.SelectNodes("table/tbody/tr[2]/td/div[2]/div[2]/table/tbody/tr[3]/td[4]/img") == null))
                        {
                            fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "143-");
                            Application.Exit();
                        }

                        string alt = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[3]/td[6]/table/tbody/tr[2]/td").InnerText;
                        string price = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[3]/td[5]/table/tbody/tr/td[1]").InnerText;
                        string[] ae = price.Split('!');
                        strUrl = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[4]/td").InnerHtml;
                        strUrl = strUrl.Split('>')[0];
                        var syntax = new string[] { "amp;", "<a", "href=", "\"", "'", "onclick=", "winOpenResizable(", "../", ",580,400);", "return false;", ">", " " };
                        syntax.ToList().ForEach(o => strUrl = strUrl.Replace(o, string.Empty));
                        entity.purchaseURL = fun.url + strUrl;
                        entity.price = ae[0];
                        entity.price = entity.price.Replace(",", string.Empty).Replace("<", string.Empty);
                        try
                        {
                            //string stockpath = "table/tbody/tr[2]/td/div[2]/div[2]/table/tbody/tr[3]/td[4]/img";
                            string stockpath = "/tbody/tr[3]/td[4]/img";
                            HtmlNodeCollection nc = hdoc.DocumentNode.SelectNodes(stockpath);
                            if (nc == null)
                            {
                                //2018-05-08 Start
                                //stockpath = fun.GetElement("select", "order_status[0]", "name", webBrowser1).InnerText;
                                //if (stockpath.Contains("注文"))
                                //{
                                entity.stockDate = "2100-01-01";
                                //}
                                //2018-05-08 End
                            }
                            else
                            {
                                string sdimg = hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("src", "");
                                //if (sdimg.Contains("stock.gif"))
                                if (hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("alt", "").Contains("在庫限り") || sdimg.Contains("stock.gif") || alt.Contains("完売") || (sdimg.Contains("stock.gif") && (alt.Equals("○") || alt.Contains("▲"))))
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                else if (sdimg.Contains("new.gif") && alt.Contains("×"))
                                {
                                    entity.stockDate = "2100-01-10";
                                }
                                else
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                            }

                        }
                        catch
                        {
                            entity.stockDate = "2100-01-10";
                        }
                        entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("▲") ? "small" : alt.Equals("×") || alt.Contains("完売") ? "empty" : "unknown status";
                        if ((dt143.Rows[i]["在庫情報"].ToString().Contains("empty") || dt143.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt143.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            if (((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-01-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-02-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2100-01-10")) && (entity.qtyStatus.Equals("inquiry"))))
                            {
                                entity.qtyStatus = dt143.Rows[i]["在庫情報"].ToString();
                                entity.price = dt143.Rows[i]["下代"].ToString();
                                entity.stockDate = dt143.Rows[i]["入荷予定"].ToString();
                            }
                            fun.Qbei_Inserts(entity);
                        }
                        else
                            fun.Qbei_Inserts(entity);
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                        fun.WriteLog(ex.Message, "143-");
                    }

                }
            }
            else
            {
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");

            }

            if (i <= dt143.Rows.Count - 1)
            {
                webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
                i++;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
            else
            {
                qe.site = 143;
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
            fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Access Denied!", dt143.Rows[i]["JANコード"].ToString(), dt143.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
            fun.WriteLog(StatusCode.ToString() + " " + dt143.Rows[i]["発注コード"].ToString(), "143-");
            Application.Exit();
        }
    }
}