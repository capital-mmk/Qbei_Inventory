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
using System.Runtime.InteropServices;
//using mshtml;
using System.Text.RegularExpressions;

namespace _20ダイアテック_高難易度_
{
    public partial class frm020 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt020 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        public static string st = string.Empty;
        int i = 0;

        System.Windows.Forms.Timer timer1;
        string[] strarr = { };
        string strsize = string.Empty;
        string strcolor = string.Empty;
        string orderCode = string.Empty;

        public frm020()
        {
            InitializeComponent();
            testflag();

        }

        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 20;
            //st = qe.starttime;

            DataTable dtflag = fun.SelectFlag(20);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {
                qe.flag = 1;
                fun.deleteData(20);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(20);
                fun.ChangeFlag(qe);
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
                fun.setURL("020");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(20);
                fun.Qbei_ErrorDelete(20);
                dt020 = fun.GetDatatable("020");
                dt020 = fun.DeleteOldCode(dt020, 20);
                dt020 = fun.GetOrderData(dt020, "https://www.b2bdiatec.jp/shop/g/g", "020", string.Empty);
                fun.GetTotalCount("020");
                ReadData();
            }
            catch (Exception) { }
        }

        private void ReadData()
        {
            qe.SiteID = 20;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);

                string body = webBrowser1.Document.Body.InnerText;
                string url = webBrowser1.Url.ToString();

                fun.WriteLog("Navigation to Site Url success------", "020-");
                qe.SiteID = 20;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();

                webBrowser1.Document.GetElementById("login_uid").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("login_pwd").InnerText = password;
                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), ex.Message, dt020.Rows[0]["JANコード"].ToString(), dt020.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                fun.WriteLog(ex.Message, "020-");
                Application.Exit();
            }
        }
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= webBrowser1_Login;
            string body = webBrowser1.Document.Body.InnerText;
            if (body == "会員IDとパスワードを入力してログインしてください。")
            {
                fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                fun.WriteLog("Login Failed", "020-");
                Application.Exit();
            }
            else
            {
                fun.WriteLog("Login success             ------", "020-");
                orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
                //orderCode = fun.ReplaceOrderCode(dt020.Rows[0]["発注コード"].ToString(), new string[] { "--" });
                // orderCode = "54-3556360502";
                webBrowser1.AllowNavigation = true;
                webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + orderCode);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);
            }
        }

        private void webBrowser_ItemSearch2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= webBrowser_ItemSearch2;
            string body = webBrowser1.Document.Body.InnerText;

            orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
            //  webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + orderCode);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);

        }

        //private void webBrowser_ItemSearch1(object sender, EventArgs e)
        //{

        //    string url = webBrowser1.Url.ToString();
        //    entity.orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
        //    string urlCode = url.Replace("https://www.b2bdiatec.jp/shop/g/g", String.Empty);
        //    if (!urlCode.Equals(entity.orderCode))
        //    {
        //        orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
        //        //  webBrowser1.AllowNavigation = true;
        //        webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + orderCode);
        //        webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);
        //        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch2);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            webBrowser1.ScriptErrorsSuppressed = true;
        //            SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
        //            instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);

        //            Thread.Sleep(2000);

        //            HtmlElementCollection hcElements;
        //            HtmlElementCollection hcInner;
        //            string strQtyStatus;
        //            string strTemp;
        //            int intMonth;
        //            entity = new Qbei_Entity();
        //            url = webBrowser1.Url.ToString();
        //            entity.siteID = 20;
        //            entity.sitecode = "020";
        //            entity.janCode = dt020.Rows[i]["JANコード"].ToString();
        //            entity.partNo = dt020.Rows[i]["自社品番"].ToString();
        //            entity.makerDate = fun.getCurrentDate();
        //            entity.reflectDate = dt020.Rows[i]["最終反映日"].ToString();
        //            // entity.orderCode="54-3556360502";
        //            entity.orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
        //            entity.purchaseURL = fun.url + "g/g" + entity.orderCode;

        //            string body = webBrowser1.Document.Body.InnerText;
        //            if (webBrowser1.Url.ToString().Split('g').Last() == entity.orderCode)
        //            {
        //            }




        //            if (webBrowser1.Document.Body.InnerHtml.Contains("申し訳ございません。"))
        //            {
        //                entity.qtyStatus = "empty";
        //                entity.stockDate = "2100-02-01";
        //                entity.price = dt020.Rows[i]["下代"].ToString();
        //            }
        //            else
        //            {
        //                hcElements = webBrowser1.Document.Body.GetElementsByTagName("span");
        //                //Check Element Exist or not
        //                if (webBrowser1.Document.All.GetElementsByName("frm").Count == 0)
        //                {
        //                    fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
        //                    fun.WriteLog("Access Denied! " + entity.orderCode, "020-");
        //                    Application.Exit();
        //                }
        //                foreach (HtmlElement he in hcElements)
        //                {
        //                    body = webBrowser1.Document.Body.InnerText;
        //                    if (he.GetAttribute("className").Equals("goods_detail_price_"))
        //                    { 
        //                        entity.price = Regex.Replace(he.InnerText, "[^0-9]+", string.Empty);
        //                    entity.price = ((int)(Convert.ToDouble(entity.price) * 0.75)).ToString();
        //                    }
        //                    if (he.GetAttribute("className").Equals("variationlist_"))
        //                    {
        //                        url = webBrowser1.Url.ToString();
        //                        // Thread.Sleep(2000);
        //                        hcInner = he.GetElementsByTagName("DL");

        //                        foreach (HtmlElement element in hcInner)
        //                        {
        //                            body = webBrowser1.Document.Body.InnerText;
        //                            //Thread.Sleep(8000);
        //                            if (element.GetElementsByTagName("input")[0].GetAttribute("value").Equals(entity.orderCode))
        //                            {
        //                                strQtyStatus = element.GetElementsByTagName("P")[0].InnerText.Replace("在庫：", string.Empty);
        //                                //2018/01/19 Start
        //                                //entity.qtyStatus = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') ? "good" : strQtyStatus.Contains('△') ? "small" : strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") ? "empty" : "unknown status";
        //                                //entity.stockDate = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || strQtyStatus.Contains('△') || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") ? "2100-01-01" : strQtyStatus.Contains("完売") ? "2100-02-01" : "unknown date";
        //                                //2018-05-15 Start
        //                                //entity.qtyStatus = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') ? "good" : strQtyStatus.Contains('△') ? "small" : strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬") || strQtyStatus.Contains("未定") ? "empty" : "unknown status";
        //                                entity.qtyStatus = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || fun.IsGood(strQtyStatus) ? "good" : strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus) ? "small" : strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬") || strQtyStatus.Contains("未定") ? "empty" : "unknown status";
        //                                //if (strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || strQtyStatus.Contains('△') || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("未定"))
        //                                if (strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || fun.IsGood(strQtyStatus) || strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus) || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("未定"))
        //                                    //2018-05-15 End
        //                                    entity.stockDate = "2100-01-01";
        //                                else if (strQtyStatus.Contains("完売"))
        //                                    entity.stockDate = "2100-02-01";
        //                                else if (strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬"))
        //                                {
        //                                    strTemp = Regex.Replace(strQtyStatus, "[^0-9]+", string.Empty);
        //                                    intMonth = int.Parse(strTemp);
        //                                    if (strQtyStatus.Contains("下旬"))
        //                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, DateTime.DaysInMonth(DateTime.Now.Year, intMonth)).ToString("yyyy-MM-dd");
        //                                    else if (strQtyStatus.Contains("中旬"))
        //                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 20).ToString("yyyy-MM-dd");
        //                                    else
        //                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 10).ToString("yyyy-MM-dd");
        //                                }
        //                                else entity.stockDate = "unknown date";
        //                                //2018/01/19 End
        //                                break;
        //                            }


        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            if ((dt020.Rows[i]["在庫情報"].ToString().Contains("empty") || (dt020.Rows[i]["在庫情報"].ToString().Contains("inquiry"))) && dt020.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
        //            {
        //                if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
        //                {
        //                    entity.qtyStatus = dt020.Rows[i]["在庫情報"].ToString();
        //                    entity.stockDate = dt020.Rows[i]["入荷予定"].ToString();
        //                    entity.price = dt020.Rows[i]["下代"].ToString();
        //                }
        //                fun.Qbei_Inserts(entity);
        //            }
        //            else
        //                fun.Qbei_Inserts(entity);
        //        }

        //        catch (Exception ex)
        //        {
        //            fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
        //            fun.WriteLog(ex.Message + entity.orderCode, "020-");
        //        }
        //        finally
        //        {
        //            ++i;
        //            if (i < dt020.Rows.Count)
        //            {
        //                orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
        //                webBrowser1.AllowNavigation = true;
        //                //webBrowser1.GoForward();
        //                webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + orderCode);
        //                Thread.Sleep(5000);
        //                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);

        //            }
        //            else
        //            {
        //                qe.site = 20;
        //                qe.flag = 2;
        //                qe.starttime = string.Empty;
        //                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                fun.ChangeFlag(qe);
        //                Application.Exit();
        //                Environment.Exit(0);
        //            }
        //        }
        //    }
        //}
        private void webBrowser_ItemSearch1(object sender, EventArgs e)
        {
            string url = webBrowser1.Url.ToString();
            entity.orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
            string urlCode = url.Replace("https://www.b2bdiatec.jp/shop/g/g", String.Empty);
            if (!urlCode.Equals(entity.orderCode))
            {
                orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
                //  webBrowser1.AllowNavigation = true;
                webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + entity.orderCode);
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch2);
            }
            else
            {
                try
                {
                    SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                    instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);

                    Thread.Sleep(2000);

                    //HtmlElementCollection hcElements;
                    //HtmlElementCollection hcInner;
                    string strQtyStatus;
                    string strTemp;
                    int intMonth;
                    entity = new Qbei_Entity();
                    url = webBrowser1.Url.ToString();
                    entity.siteID = 20;
                    entity.sitecode = "020";
                    entity.janCode = dt020.Rows[i]["JANコード"].ToString();
                    entity.partNo = dt020.Rows[i]["自社品番"].ToString();
                    entity.makerDate = fun.getCurrentDate();
                    entity.reflectDate = dt020.Rows[i]["最終反映日"].ToString();
                    entity.orderCode = dt020.Rows[i]["発注コード"].ToString().Trim();
                    entity.purchaseURL = fun.url + "g/g" + entity.orderCode;

                    string body = webBrowser1.Document.Body.InnerText;
                    if (webBrowser1.Url.ToString().Split('g').Last() == entity.orderCode)
                    {
                    }




                    if (webBrowser1.Document.Body.InnerHtml.Contains("申し訳ございません。"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt020.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        //hcElements = webBrowser1.Document.Body.GetElementsByTagName("span");
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        HtmlNode priceNode = hdoc.DocumentNode.SelectSingleNode("/div[2]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/h2[2]/span");

                        //Check Element Exist or not
                        if (webBrowser1.Document.All.GetElementsByName("frm").Count == 0)
                        {
                            fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "020-");
                            Application.Exit();
                        }

                        if (priceNode != null)
                        {
                            entity.price = Regex.Replace(priceNode.InnerText, "[^0-9]+", string.Empty);
                            entity.price = ((int)(Convert.ToDouble(entity.price) * 0.75)).ToString();
                        }

                        HtmlNode variationlistNode = hdoc.DocumentNode.SelectSingleNode("div[2]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/div/div[1]");

                        foreach (var he in variationlistNode.ChildNodes)
                        {

                            var dlNodes = he.Descendants("DL");

                            if (dlNodes != null)
                            {
                                foreach (HtmlNode element in dlNodes)
                                {

                                    if (element.Descendants("input").SingleOrDefault().Attributes["value"].Value.Equals(entity.orderCode))
                                    {
                                        strQtyStatus = element.Descendants("P").FirstOrDefault().InnerText.Replace("在庫：", string.Empty);

                                        entity.qtyStatus = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || fun.IsGood(strQtyStatus) ? "good" : strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus) ? "small" : strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬") || strQtyStatus.Contains("未定") ? "empty" : "unknown status";
                                        if (strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || fun.IsGood(strQtyStatus) || strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus) || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("未定"))
                                            entity.stockDate = "2100-01-01";
                                        else if (strQtyStatus.Contains("完売"))
                                            entity.stockDate = "2100-02-01";
                                        else if (strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬"))
                                        {
                                            strTemp = Regex.Replace(strQtyStatus, "[^0-9]+", string.Empty);
                                            intMonth = int.Parse(strTemp);
                                            if (strQtyStatus.Contains("下旬"))
                                                entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, DateTime.DaysInMonth(DateTime.Now.Year, intMonth)).ToString("yyyy-MM-dd");
                                            else if (strQtyStatus.Contains("中旬"))
                                                entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 20).ToString("yyyy-MM-dd");
                                            else
                                                entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 10).ToString("yyyy-MM-dd");
                                        }
                                        else entity.stockDate = "unknown date";
                                        //2018/01/19 End
                                        // break;

                                        if ((dt020.Rows[i]["在庫情報"].ToString().Contains("empty") || (dt020.Rows[i]["在庫情報"].ToString().Contains("inquiry"))) && dt020.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                        {
                                            if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                            {
                                                entity.qtyStatus = dt020.Rows[i]["在庫情報"].ToString();
                                                entity.stockDate = dt020.Rows[i]["入荷予定"].ToString();
                                                entity.price = dt020.Rows[i]["下代"].ToString();
                                            }

                                        }
                                        fun.Qbei_Inserts(entity);
                                    }


                                    //    }
                                    //    break;
                                }
                            }
                        }
                    }



                    //else
                    //    fun.Qbei_Inserts(entity);
                }

                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                    fun.WriteLog(ex.Message + entity.orderCode, "020-");
                }
                finally
                {

                    if (i < dt020.Rows.Count - 1)
                    {
                        orderCode = dt020.Rows[++i]["発注コード"].ToString().Trim();
                        webBrowser1.AllowNavigation = true;
                        //webBrowser1.GoForward();
                        webBrowser1.Navigate("https://www.b2bdiatec.jp/shop/g/g" + orderCode);
                        Thread.Sleep(5000);
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch1);

                    }
                    else
                    {
                        qe.site = 20;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), "Access Denied!", dt020.Rows[i]["JANコード"].ToString(), dt020.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
            fun.WriteLog(StatusCode.ToString() + " " + dt020.Rows[i]["発注コード"].ToString(), "020-");
            Application.Exit();
        }
    }
}

