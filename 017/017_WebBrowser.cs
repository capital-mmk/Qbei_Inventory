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

namespace _17インターマックス
{
    public partial class frm017 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt017 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public frm017()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 17;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(17);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(17);
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
                fun.WriteLog(ex, "017-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        public void StartRun()
        {
            try
            {
                fun.setURL("017");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(17);
                fun.Qbei_ErrorDelete(17);
                dt017 = fun.GetDatatable("017");
                dt017 = fun.GetOrderData(dt017, "https://www.b2bshop.intermax.co.jp/shop/g/g", "017", "/");
                fun.GetTotalCount("017");                
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "017-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void ReadData()
        {
            qe.SiteID = 17;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                webBrowser1.ScriptErrorsSuppressed = true;
                fun.WriteLog("Navigation to Site Url success------", "017-");
                qe.SiteID = 17;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("login_uid").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "pwd", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt017.Rows[0]["JANコード"].ToString();
                string orderCode = dt017.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");                
                fun.WriteLog(ex, "017-", janCode, orderCode);
                fun.Qbei_Maker_Insert("017", dt017);

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
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください"))
                {
                    fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");                    
                    fun.WriteLog("Login Failed", "017-");
                    fun.Qbei_Maker_Insert("017", dt017);
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "017-");
                    orderCode = dt017.Rows[i]["発注コード"].ToString().Trim();
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + orderCode.Trim() + "/");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt017.Rows[0]["JANコード"].ToString();
                orderCode = dt017.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");                
                fun.WriteLog(ex, "017-", janCode, orderCode);
                fun.Qbei_Maker_Insert("017", dt017);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                webBrowser1.ScriptErrorsSuppressed = true;
                entity = new Qbei_Entity();
                entity.siteID = 17;
                entity.sitecode = "017";
                entity.janCode = dt017.Rows[i]["JANコード"].ToString();
                entity.partNo = dt017.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt017.Rows[i]["最終反映日"].ToString();
                entity.stockDate = dt017.Rows[i]["入荷予定"].ToString();
                entity.orderCode = dt017.Rows[i]["発注コード"].ToString().Trim();
                entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode + "/";

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    if (body.Contains("申し訳ございません。"))
                    {
                        if (dt017.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt017.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt017.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt017.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }
                    else
                    {
                        string pricePath = string.Empty;
                        string qtyPath = string.Empty;
                        string stockDatePath = string.Empty;
                        int year = DateTime.Now.Year;

                        string html = webBrowser1.Document.Body.InnerHtml;
                        html = html.Replace("</TR></TR>", "</TR>");
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);

                        if ((hdoc.DocumentNode.SelectSingleNode("div/div[2]/div/table[2]/tbody/tr/td[3]/div/div/table/tbody") == null))
                        {
                            fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "017-");
                            fun.Qbei_Maker_Insert("017", dt017, i);
                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            string tablepath = "div/div[2]/div/table[2]/tbody/tr/td[3]/div/div/table/tbody/tr[2]/td[2]/table/tbody/tr[1]/td/table/tbody/tr";

                            HtmlNodeCollection nc = hdoc.DocumentNode.SelectNodes(tablepath);
                            if (nc == null)
                            {
                                tablepath = "div/div[2]/div/table[2]/tbody/tr/td[3]/div/div/table/tbody/tr[3]/td[2]/table/tbody/tr[1]/td/table/tbody/tr";
                            }

                            foreach (HtmlNode row in hdoc.DocumentNode.SelectNodes(tablepath))
                            {
                                string path = row.SelectSingleNode("td[1]").InnerText;

                                if (path.Contains("下代(税抜)"))
                                {
                                    pricePath = row.SelectSingleNode("td[2]").InnerText;
                                }
                                if (path.Contains("在庫"))
                                {
                                    qtyPath = row.SelectSingleNode("td[2]").InnerText;
                                }
                                if (path.Contains("入荷予定日"))
                                {
                                    stockDatePath = row.SelectSingleNode("td[2]").InnerText;
                                }
                            }

                            if (string.IsNullOrWhiteSpace(pricePath))
                            {
                                fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Cannot find path", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");
                            }
                            else
                            {
                                string prc = pricePath.Replace("￥", string.Empty).Replace(",", string.Empty);
                                string[] str = prc.Split('(');
                                if (str.Length > 0)
                                    entity.price = str[0].Trim();
                                else entity.price = prc;
                                string qty = qtyPath;
                                entity.qtyStatus = qty.Contains("有り") || qty.Contains("あり") ? "good" : qty.Contains("わずか") || qty.Contains("僅か") ? "small" : qty.Contains("欠品中") || qty.Contains("完売") || qty.Contains("終了") ? "empty" : qty.Contains("予約受付中") || qty.Contains("予約") || qty.Contains("取寄") ? "inquiry" : "invalid status code";
                                if (stockDatePath == "")
                                {
                                    if (qty.Contains("月") && qty.Any(c => char.IsDigit(c)))
                                    {
                                        string[] ao = qty.Split('月');
                                        string month = ao[0].ToString();
                                        string day = "10";
                                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                        string currentdate = fun.getCurrentDate();
                                        if (dt <= Convert.ToDateTime(currentdate))
                                        {
                                            dt = dt.AddYears(1);
                                        }
                                    }
                                    else
                                    {
                                        entity.stockDate = qty.Contains("わずか") || qty.Contains("有り") || qty.Contains("あり") || qty.Contains("僅か") || qty.Contains("欠品中") || qty.Contains("予約受付中") || qty.Contains("予約") || qty.Contains("取寄") ? "2100-01-01" : qty.Contains("完売") || qty.Contains("終了") ? "2100-02-01" : "Unknown date";
                                    }
                                }
                                else
                                {
                                    entity.stockDate = qty.Contains("わずか") || qty.Contains("有り") || qty.Contains("あり") || qty.Contains("僅か") || qty.Contains("欠品中") || qty.Contains("予約受付中") || qty.Contains("予約") || qty.Contains("取寄") ? DateTime.Now.ToString("yyyy/MM/dd") : qty.Contains("完売") || qty.Contains("終了") ? "2100-02-01" : "unknown date";
                                    entity.stockDate = entity.stockDate.Replace("/", "-");
                                }

                                if (entity.stockDate.Contains("2月"))
                                {
                                    entity.stockDate = year + "-02-" + DateTime.DaysInMonth(year, 2);
                                }//ssa
                            }

                            string todaydate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            if ((dt017.Rows[i]["在庫情報"].ToString().Contains("empty") || dt017.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt017.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if (((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-01-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-02-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals(year + "-02-" + DateTime.DaysInMonth(year, 2))) && (entity.qtyStatus.Equals("inquiry"))))
                                {
                                    entity.qtyStatus = dt017.Rows[i]["在庫情報"].ToString();
                                    entity.price = dt017.Rows[i]["下代"].ToString();
                                    entity.stockDate = dt017.Rows[i]["入荷予定"].ToString();
                                }
                                fun.Qbei_Inserts(entity);
                            }
                            else
                                fun.Qbei_Inserts(entity);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");
                }                
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");                
                fun.WriteLog(ex, "017-", entity.janCode, entity.orderCode);
            }
            finally
            {
                if (i < dt017.Rows.Count - 1)
                {
                    string ordercode = dt017.Rows[++i]["発注コード"].ToString().Trim();
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + ordercode.Trim() + "/");
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    fun.Qbei_Maker_Insert("017", dt017, i);

                    qe.site = 17;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt017.Rows[i]["JANコード"].ToString();
            string orderCode = dt017.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");            
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "017-");

            fun.Qbei_Maker_Insert("017", dt017, i);
            Application.Exit();
            Environment.Exit(0);
        }
    }
}

