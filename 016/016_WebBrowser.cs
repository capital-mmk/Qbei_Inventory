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

namespace _016ライトウェイ
{
    public partial class frm016 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt016 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        int counts = 0;

        public static string st = string.Empty;
        public frm016()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 16;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(16);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(16);
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
                fun.WriteLog(ex, "016-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        public void StartRun()
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                fun.setURL("016");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(16);
                fun.Qbei_ErrorDelete(16);
                dt016 = fun.GetDatatable("016");
                //2017/12/15 Start
                dt016 = fun.GetOrderData(dt016, "https://rpj-ec.com/rpj/order/searchItemNo　", "016", string.Empty);
                fun.GetTotalCount("016");
                //2017/12/15 End
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "016-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 16;
            dt = qubl.Qbei_Setting_Select(qe);

            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url + "/rpj/login");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                //webBrowser1.ScriptErrorsSuppressed = true;
                fun.WriteLog("Navigation to Site Url success------", "016-");
                qe.SiteID = 16;
                dt = qubl.Qbei_Setting_Select(qe);
                HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                string username = dt.Rows[0]["UserName"].ToString().Trim();
                webBrowser1.Document.GetElementById("MemberLoginId").SetAttribute("value", username);
                string password = dt.Rows[0]["Password"].ToString().Trim();
                webBrowser1.Document.GetElementById("MemberLoginPw").SetAttribute("value", password);
                fun.GetElement("input", "PCサイトログイン", "alt", webBrowser1).InvokeMember("onclick");                
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Main);
            }
            catch (Exception ex)
            {
                string janCode = dt016.Rows[0]["JANコード"].ToString();
                string orderCode = dt016.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_Main(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Main);
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください。"))
                {
                    fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Login Failed", dt016.Rows[i]["JANコード"].ToString(), dt016.Rows[i]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                    fun.WriteLog("Login Failed", "016-");
                    fun.Qbei_Maker_Insert("016", dt016);
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "016-");
                    webBrowser1.Navigate(fun.url + "/rpj/order/searchItemNo");
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt016.Rows[0]["JANコード"].ToString();
                string orderCode = dt016.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (webBrowser1.Url.ToString().Equals(fun.url + "/rpj/order/searchItemNo"))
                {
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);

                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt016.Rows[i]["JANコード"].ToString();
                string orderCode = dt016.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016, i);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        //private void webBrowser1_SearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    string url = webBrowser1.Url.ToString();
        //    webBrowser1.ScriptErrorsSuppressed = true;
        //    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchPage);
        //    webBrowser1.Navigate(fun.url + "/rpj/order/searchItemNo");
        //    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchPage1);
        //}

        //private void webBrowser1_SearchPage1(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    webBrowser1.ScriptErrorsSuppressed = true;
        //    string url = webBrowser1.Url.ToString();
        //    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchPage1);
        //    webBrowser1.Navigate(fun.url + "/rpj/order/searchItemNo");
        //    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
        //}

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string url = webBrowser1.Url.ToString();
                string strJancode;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                i = counts;
                if (i < dt016.Rows.Count)
                {
                    for (int k = 0; k < 95; k++)
                        AddRow();
                    string ordercode = string.Empty;
                    for (int j = 0; j < 100; i++)
                    {
                        fun.ClearMemory();
                        if (i < dt016.Rows.Count)
                        {
                            strJancode = string.Empty;
                            ordercode = dt016.Rows[i]["発注コード"].ToString();
                            if (string.IsNullOrEmpty(ordercode))
                            {
                                strJancode = dt016.Rows[i]["JANコード"].ToString();
                                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Order Code Not Found!", strJancode, ordercode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                            }
                            else
                            {
                                webBrowser1.Document.GetElementById("orderSearch_" + j).SetAttribute("value", ordercode);
                                j++;
                            }
                        }
                        else break;
                    }

                    fun.GetElement("img", "商品を一括で呼び出す", "value", webBrowser1).InvokeMember("click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage1);
                }
                else
                {
                    fun.Qbei_Maker_Insert("016", dt016, i);
                    qe.site = 16;
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
                string janCode = dt016.Rows[i]["JANコード"].ToString();
                string orderCode = dt016.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016, i);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_SearchClick(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchClick);
                fun.GetElement("img", "商品を一括で呼び出す", "value", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchClick1);
            }
            catch (Exception ex)
            {
                string janCode = dt016.Rows[i]["JANコード"].ToString();
                string orderCode = dt016.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016, i);

                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_WaitForSearchPage1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(webBrowser1.Document.GetElementById("orderSearch_0").GetAttribute("value")))
                {
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage1);

                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchClick1);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt016.Rows[i]["JANコード"].ToString();
                string orderCode = dt016.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                fun.WriteLog(ex, "016-", janCode, orderCode);
                fun.Qbei_Maker_Insert("016", dt016, i);

                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_SearchClick1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strUrl;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_SearchClick1);
            for (int k = 1; k <= 100; counts++)
            {
                fun.ClearMemory();
                if (counts < dt016.Rows.Count)
                {
                    try
                    {
                        entity = new Qbei_Entity();
                        entity.siteID = 16;
                        entity.sitecode = "016";
                        entity.janCode = dt016.Rows[counts]["JANコード"].ToString();
                        entity.partNo = dt016.Rows[counts]["自社品番"].ToString();
                        entity.makerDate = fun.getCurrentDate();
                        entity.reflectDate = dt016.Rows[counts]["最終反映日"].ToString();
                        entity.orderCode = dt016.Rows[counts]["発注コード"].ToString();
                        entity.purchaseURL = fun.url + "/rpj/order/searchItemNo";

                        if (string.IsNullOrWhiteSpace(entity.orderCode))
                        {
                            fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                        }
                        else
                        {

                            string html = webBrowser1.Document.Body.InnerHtml;
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(html);
                            //  if ((hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table").InnerText == null))
                            if ((hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[4]") == null) && (hdoc.DocumentNode.SelectSingleNode("div/div[3]/div[1]/div[1]/div[2]/div/div[1]/table/tbody/tr[3]/td[4]/font") == null))
                            {
                                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "016-");
                                fun.Qbei_Maker_Insert("016", dt016, counts);
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                string code = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[2]/div").InnerText;
                                if (code.Contains("["))
                                {
                                    strUrl = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[3]").InnerHtml;
                                    strUrl = strUrl.Substring(0, strUrl.IndexOf('>'));
                                    var syntax = new string[] { "amp;", "<a", "href=", "\"", "..", " " };
                                    syntax.ToList().ForEach(o => strUrl = strUrl.Replace(o, string.Empty));
                                    entity.purchaseURL = fun.url + "/rpj" + strUrl;

                                    // string qty = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[4]").InnerText.Replace("\r\n", "");

                                    string qty = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[4]").InnerText.Replace("\r\n", "");
                                    entity.qtyStatus = qty.Contains("○") ? "good" : qty.Contains("△") ? "small" : qty.Contains("予約") || qty.Equals("×") || qty.Contains("入荷予定なし") ? "empty" : "empty";

                                    if (qty.Contains("("))
                                    {
                                        string[] str = qty.Split('(');
                                        string date = str[1].Replace(")", string.Empty);
                                        if (date.Contains("2月"))
                                        { entity.stockDate = date.Replace("年", "-").Replace("月", "-").Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "28"); }
                                        //entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 2) + "]/td[4]/font").InnerText;
                                        else
                                            entity.stockDate = date.Replace("年", "-").Replace("月", "-").Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "30");
                                        if (entity.stockDate.Contains("入荷未定") || entity.stockDate.Contains("入荷予定有"))
                                        {
                                            entity.stockDate = "2100-01-01";
                                        }
                                        entity.stockDate = DateTime.Parse(entity.stockDate).ToString("yyyy-MM-dd");
                                    }
                                    else if (qty.Contains("入荷予定なし"))
                                    {
                                        entity.stockDate = "2100-02-01";
                                    }
                                    else if (qty.Contains("入荷未定"))
                                    {
                                        entity.stockDate = "2100-01-01";
                                    }
                                    else
                                    {
                                        entity.stockDate = "2100-01-01";
                                    }

                                    string price = hdoc.DocumentNode.SelectSingleNode("div/div[3]/div/div/div[2]/div/div/table/tbody/tr[" + (k + 1) + "]/td[6]").InnerText;
                                    if (string.IsNullOrWhiteSpace(price))
                                        entity.price = "0";
                                    else
                                    {
                                        string[] pstr = price.Split('円');

                                        entity.price = pstr[pstr.Length - 2].Replace("\r\n", "").Replace(",", "");

                                        if (entity.price.Contains(")"))
                                        {
                                            pstr = entity.price.Split(')');
                                            entity.price = pstr[pstr.Length - 1];
                                        }
                                    }
                                    if ((dt016.Rows[counts]["在庫情報"].ToString().Contains("empty") || dt016.Rows[counts]["在庫情報"].ToString().Contains("inquiry")) && dt016.Rows[counts]["入荷予定"].ToString().Contains("2100-01-10"))
                                    {
                                        if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
                                        // if ((entity.qtyStatus.Equals("empty") && entity.stockDate.Equals("2100-01-01")) || (entity.qtyStatus.Equals("empty") && entity.stockDate.Equals("2100-02-01")) || (entity.qtyStatus.Equals("empty") && entity.stockDate.Equals("")) || (entity.qtyStatus.Equals("inquiry") && entity.stockDate.Equals("2100-01-01")) || (entity.qtyStatus.Equals("inquiry") && entity.stockDate.Equals("2100-02-01")) || (entity.qtyStatus.Equals("inquiry") && entity.stockDate.Contains("2018")) || (entity.qtyStatus.Equals("inquiry") && entity.stockDate.Equals("")))
                                        {
                                            entity.qtyStatus = dt016.Rows[counts]["在庫情報"].ToString();
                                            entity.price = dt016.Rows[counts]["下代"].ToString();
                                            entity.stockDate = dt016.Rows[counts]["入荷予定"].ToString();
                                        }
                                        fun.Qbei_Inserts(entity);
                                    }

                                    else
                                        fun.Qbei_Inserts(entity);
                                }
                                else
                                {
                                    if (dt016.Rows[counts]["入荷予定"].ToString().Contains("2100-01-10") && dt016.Rows[counts]["在庫情報"].ToString().Contains("empty"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-01-10";
                                        entity.price = dt016.Rows[counts]["下代"].ToString();
                                    }
                                    else
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt016.Rows[counts]["下代"].ToString();
                                    }
                                    fun.Qbei_Inserts(entity);
                                }
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                        fun.WriteLog(ex, "016-", entity.janCode, entity.orderCode);
                    }

                    k++;
                }
                else
                {
                    fun.Qbei_Maker_Insert("016", dt016, counts);

                    qe.site = 16;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }

            webBrowser1.Navigate(fun.url + "/rpj/order/searchItemNo");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
        }

        private void AddRow()
        {
            // webBrowser1.Document.GetElementsByTagName("span")[0].InvokeMember("click");
            fun.GetElement("img", "もう一件追加する", "value", webBrowser1).InvokeMember("click");
            webBrowser1.ScriptErrorsSuppressed = true;
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt016.Rows[i]["JANコード"].ToString();
            string orderCode = dt016.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "016-");

            fun.Qbei_Maker_Insert("016", dt016, i);
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
