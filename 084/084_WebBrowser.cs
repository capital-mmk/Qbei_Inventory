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
//using mshtml;
using System.Text.RegularExpressions;
using System.Threading;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _84NBS_今期取り扱い商品無し保留_
{
    public partial class frm084 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt084 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        public static string st = string.Empty;
        int i = -1;
        public frm084()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 84;
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(84);
            fun.WriteLog("Flag Got ------", "084-");
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                fun.WriteLog("Flag0 ------", "084-");
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(84);
                fun.ChangeFlag(qe);
                fun.WriteLog("Flag1 ------", "084-");
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
                fun.setURL("084");
                fun.WriteLog("Set URL success ------", "084-");
                fun.CreateFileAndFolder();
                fun.WriteLog("File Create success ------", "084-");
                fun.Qbei_Delete(84);
                fun.WriteLog("Qbei_Delet OK ------", "084-");
                fun.Qbei_ErrorDelete(84);
                fun.WriteLog("Qbei_ErrorDelet OK ------", "084-");
                dt084 = fun.GetDatatable("084");
                fun.WriteLog("GetDataTable OK ------", "084-");
                dt084 = fun.GetOrderData(dt084, "https://weborder.colnago.jp/goods/goods_list.html?page_from=goods_detail&goods_detail=1&C_sSyohinCd=", "084", "");
                int dtcount = Convert.ToInt32(dt084.Rows.Count);
                fun.WriteLog("Start Running ------", "084-");
                fun.GetTotalCount("084");
                ReadData();
            }
            catch (Exception e)
            {
                fun.WriteLog(e.Message.ToString(), "084-");
            }
        }

        private void ReadData()
        {
            qe.SiteID = 84;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            fun.WriteLog("Go to Url ------", "084-");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.WriteLog("Navigation to Site Url success------", "084-");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                qe.SiteID = 84;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "customer_code", "name", webBrowser1).InnerText = username;
                fun.GetElement("input", "login_id", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "password", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "login", "name", webBrowser1).InvokeMember("click");
                Thread.Sleep(2000);
                webBrowser1.Navigate(fun.url + "/index.html");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), ex.Message, dt084.Rows[0]["JANコード"].ToString(), dt084.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
                fun.WriteLog(ex.Message + dt084.Rows[0]["発注コード"].ToString(), "084-");
                Application.Exit();
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= webBrowser1_Login;
            string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
            if (body.Contains("入力されていません") || body.Contains("入力されていません。"))
            {
                fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), "Login Failed", dt084.Rows[0]["JANコード"].ToString(), dt084.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
                fun.WriteLog("Login Failed", "084-");
                Application.Exit();
            }
            else
            {
                fun.WriteLog("Login success             ------", "084-");
                // string ordercode = "18ADCOMPP40WB";
                string ordercode = dt084.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/goods/goods_list.html?page_from=goods_detail&goods_detail=1&C_sSyohinCd=" + ordercode);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
        }
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // string ordercode = "18ADCOMPP40WB";
            string ordercode = dt084.Rows[i]["発注コード"].ToString();
            webBrowser1.Navigate(fun.url + "/goods/goods_list.html?page_from=goods_detail&goods_detail=1&C_sSyohinCd=" + ordercode);
            Thread.Sleep(7000);
            if (webBrowser1.Url.ToString().Contains("/goods/goods_list.html?"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }

        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            entity = new Qbei_Entity();
            entity.siteID = 84;
            entity.sitecode = "084";
            entity.janCode = dt084.Rows[i]["JANコード"].ToString();
            entity.partNo = dt084.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt084.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt084.Rows[i]["発注コード"].ToString();

            entity.purchaseURL = fun.url + "/goods/goods_list.html?page_from=goods_detail&goods_detail=1&C_sSyohinCd=" + entity.orderCode;

            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string url = webBrowser1.Document.Url.ToString();
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("該当データはありません。"))
                {
                    if (dt084.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt084.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt084.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt084.Rows[i]["下代"].ToString();
                    }
                    fun.Qbei_Inserts(entity);
                    //fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
                }
                else
                {
                    try
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        entity.stockDate = string.Empty;
                        //Check Element Exist or not
                        if (hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr[2]/td/div[1]/div[2]/div/div[1]/table[1]/tbody/tr/td[2]/table[2]/tbody/tr[1]/td[1]") == null && (hdoc.DocumentNode.SelectSingleNode("table/tbody/tr[2]/td/div[1]/div[2]/div/div[1]/table[1]/tbody/tr/td[2]/table[2]/tbody/tr[2]/td[1]") == null))
                        {
                            fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "084-");
                            Application.Exit();
                        }
                        else
                        {
                            string alt = hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr[2]/td/div[1]/div[2]/div/div[1]/table[1]/tbody/tr/td[2]/table[2]/tbody/tr[1]/td[1]").InnerText;

                            entity.qtyStatus = alt.Contains("○") ? "good" : alt.Contains("△") || alt.Contains("▲") || Regex.IsMatch(alt, @"^\d$") ? "small" : alt.Contains("無し") || alt.Contains("なし") || alt.Contains("準備中") || alt.Contains("終了") ? "empty" : "invalid status code";
                            entity.price = hdoc.DocumentNode.SelectSingleNode("/table/tbody/tr[2]/td/div[1]/div[2]/div/div[1]/table[2]/tbody/tr[2]/td[1]").InnerText;
                            entity.price = entity.price.Replace("　　　　　　　　　　　　　　　　　　　　　￥", string.Empty);
                            entity.price = entity.price.Replace(",", string.Empty).Replace("円", string.Empty).Replace("（税抜）", string.Empty).Replace("\n                \t\t", string.Empty);
                            entity.stockDate = hdoc.DocumentNode.SelectSingleNode("table/tbody/tr[2]/td/div[1]/div[2]/div/div[1]/table[1]/tbody/tr/td[2]/table[2]/tbody/tr[2]/td[1]").InnerText;
                            if (entity.stockDate.Contains("-") || string.IsNullOrWhiteSpace(entity.stockDate))
                            {
                                entity.stockDate = "2100-01-01";
                                //entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Contains("×") ? "2100-01-01" : alt.Contains("終了") ? "2100-02-01" : "unknown date";
                            }
                            else if (entity.stockDate.Contains("2月"))
                            {
                                //entity.stockDate = "2018-02-28";
                                entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                            }
                            else if (entity.stockDate.Contains("月"))
                            {
                                string[] arr = entity.stockDate.Split('月');
                                string month = arr[0];
                                string day = string.Empty;
                                int mon = Convert.ToInt32(month);
                                if (mon <= 12)
                                {
                                    //2018-05-07 Start
                                    //int day1 = DateTime.DaysInMonth(DateTime.Now.Year, mon);
                                    //day = Convert.ToString(day1);
                                    //2018-05-07 End
                                    day = "30";
                                }
                                string year = DateTime.Now.ToString("yyyy");

                                DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                string d = fun.getCurrentDate();
                                if (dt < Convert.ToDateTime(d))
                                    dt = dt.AddYears(1);

                                entity.stockDate = dt.ToString("yyyy-MM-dd");

                            }
                            //未定受付数　○
                            else if (entity.stockDate.Contains("未定"))
                            {
                                entity.stockDate = "2100-01-01";
                            }
                            //2018-04-26 Start
                            //終了
                            else if (entity.stockDate.Contains("終了"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            //2018-04-26 End
                            if (dt084.Rows[i]["在庫情報"].ToString().Contains("empty") && dt084.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || (((entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("unknown date")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                                {
                                    entity.qtyStatus = dt084.Rows[i]["在庫情報"].ToString();
                                    entity.price = dt084.Rows[i]["下代"].ToString();
                                    entity.stockDate = dt084.Rows[i]["入荷予定"].ToString();
                                }
                                fun.Qbei_Inserts(entity);
                            }

                            else
                                fun.Qbei_Inserts(entity);
                        }
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
                        fun.WriteLog(ex.Message + entity.orderCode, "084-");
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
            }

            if (i < dt084.Rows.Count - 1)
            {
                string ordercode = dt084.Rows[i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/goods/goods_list.html?page_from=goods_detail&goods_detail=1&C_sSyohinCd=" + ordercode);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);

            }
            else
            {
                qe.site = 84;
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
            fun.Qbei_ErrorInsert(84, fun.GetSiteName("084"), "Access Denied!", dt084.Rows[i]["JANコード"].ToString(), dt084.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "084");
            fun.WriteLog(StatusCode.ToString() + " " + dt084.Rows[i]["発注コード"].ToString(), "084-");
            Application.Exit();
        }
    }
}
