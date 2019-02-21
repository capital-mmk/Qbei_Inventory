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
using SHDocVw;
using System.Runtime.InteropServices;

namespace _0035
{
    public partial class frm035 : Form
    {

        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt035 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        string strParam = string.Empty;

        public frm035()
        {
            InitializeComponent();
            testflag();
        }

        public frm035(string strObj)
        {
            InitializeComponent();
            strParam = strObj;
            StartRun();
        }

        private void testflag()
        {
            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            qe.starttime = DateTime.Now.ToString();
            qe.site = 35;
            st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(35);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {
                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(35);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void StartRun()
        {
            try
            {
                fun.setURL("035");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(35);
                fun.Qbei_ErrorDelete(35);

                if (String.IsNullOrEmpty(strParam))
                {
                    dt035 = fun.GetDatatable("035");
                    dt035 = fun.GetOrderData(dt035, "https://intertecinc.jp/ecuser/item/itemDetail?itemCd=", "035", "");
                }
                else
                {
                    dt035 = fun.GetRerunData("035");
                }

                fun.GetTotalCount("035");
                if (dt035 != null)
                    ReadData();
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch
            {
            }
        }

        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 35;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                String url = webBrowser1.Url.ToString();
                fun.WriteLog("Navigation to Site Url success------", "035-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 35;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("id").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("pw").InnerText = password;
                var links = webBrowser1.Document.GetElementsByTagName("a");
                foreach (HtmlElement current in webBrowser1.Document.GetElementsByTagName("a"))
                {
                    if (current.GetAttribute("InnerText").Equals("ログイン"))
                    {
                        current.InvokeMember("onclick");
                    }

                }
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);

            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, dt035.Rows[0]["JANコード"].ToString(), dt035.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex.Message + dt035.Rows[0]["発注コード"].ToString(), "035-");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string url = webBrowser1.Url.ToString();
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                url = webBrowser1.Url.ToString();
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains(" IDを入力してください") || body.Contains("パスワードを入力してください") || body.Contains("IDを正しく入力してください"))
                {
                    fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "035-");
                    string ordercode = dt035.Rows[i]["発注コード"].ToString();
                    //string ordercode = "53145";
                    webBrowser1.Navigate("https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + ordercode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, dt035.Rows[0]["JANコード"].ToString(), dt035.Rows[0]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex.Message + dt035.Rows[0]["発注コード"].ToString(), "035-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                entity = new Qbei_Entity();
                entity.siteID = 35;
                entity.sitecode = "035";
                entity.janCode = dt035.Rows[i]["JANコード"].ToString();
                entity.partNo = dt035.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt035.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt035.Rows[i]["発注コード"].ToString();
                //entity.orderCode = "53145";
                entity.purchaseURL = "https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + entity.orderCode;

                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    if (body.Contains("有効な商品ではありません"))
                    {
                        if (dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt035.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            //fun.Qbei_ErrorInsert(35, "マルイ", "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt035.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt035.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }
                    else
                    {
                        try
                        {
                            string html = webBrowser1.Document.Body.InnerHtml;
                            string year = string.Empty;
                            string month = string.Empty;
                            string day = string.Empty;
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(html);
                            if ((hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[5]") == null) && (hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/form/div/div/table/tbody/tr/td[7]/div/span[1]") == null))
                            {
                                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                                fun.WriteLog("Access Denied! " + entity.orderCode, "035--");
                                Application.Exit();
                            }
                            else
                            {
                                string qty = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[5]").InnerText;

                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[7]/div/span[2]").InnerText;
                                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);
                                //entity.price = ((int)(Convert.ToDouble(entity.price) * 0.98)).ToString();

                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div/div[1]/section[2]/div/div/table/tbody/tr/td[6]/div/span[1]").InnerText;




                                if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("下旬") || entity.stockDate.Contains("中旬") || entity.stockDate.Contains("初旬"))
                                {
                                    month = Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty);
                                    month = int.Parse(month).ToString();

                                    if (entity.stockDate.Contains("上旬"))
                                    {
                                        day = "10";
                                    }
                                    else if (entity.stockDate.Contains("下旬"))
                                    {
                                        if (month == "2")
                                            day = "28";
                                        else
                                        day = "30";
                                    }
                                    else if (entity.stockDate.Contains("中旬"))
                                        day = "20";
                                    else if (entity.stockDate.Contains("初旬"))
                                        day = "10";

                                   
                                    year = DateTime.Now.ToString("yyyy");
                                    DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                    string d = fun.getCurrentDate();
                                    if (dt < Convert.ToDateTime(d))
                                        dt = dt.AddYears(1);

                                    entity.stockDate = dt.ToString("yyyy-MM-dd");


                                }
                                if (entity.stockDate.Equals("-") || entity.stockDate.Equals(""))
                                {
                                    entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsSmall1(qty) ? "small" : qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";
                                    entity.stockDate = qty.Equals("◯") || qty.Equals("◎") || qty.Equals("△") || fun.IsSmall1(qty) || fun.IsLessthanzero(qty) || qty.Equals("×") ? "2100-01-01" : qty.Equals("完売") || qty.Equals("終了") ? "2100-02-01" : "unknown date";
                                }
                                else
                                {
                                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                    entity.qtyStatus = qty.Equals("◯") || qty.Equals("◎") ? "good" : qty.Equals("△") || fun.IsSmall1(qty) ? "small" : qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? "empty" : "unknown status";
                                    entity.stockDate = qty.Equals("◯") || qty.Equals("◎") || qty.Equals("△") || fun.IsSmall1(qty) || qty.Equals("×") || qty.Equals("完売") || qty.Equals("終了") || fun.IsLessthanzero(qty) ? entity.stockDate : "unknown date";
                                }


                                if (dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                {
                                    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                    {
                                        entity.qtyStatus = dt035.Rows[i]["在庫情報"].ToString();
                                        entity.stockDate = dt035.Rows[i]["入荷予定"].ToString();
                                        entity.price = dt035.Rows[i]["下代"].ToString();
                                    }
                                    fun.Qbei_Inserts(entity);
                                }
                                else if ((!String.IsNullOrEmpty(strParam)) && ((dt035.Rows[i]["在庫情報"].ToString().Contains("empty") && (String.IsNullOrEmpty(dt035.Rows[i]["入荷予定"].ToString()) || dt035.Rows[i]["入荷予定"].ToString().Contains("2100-01-01") || dt035.Rows[i]["入荷予定"].ToString().Contains("2100-02-01"))) || dt035.Rows[i]["在庫情報"].ToString().Contains("inquiry")))
                                {
                                    fun.RerunOrder(entity);
                                }
                                else
                                    //2017/12/22 End
                                    fun.Qbei_Inserts(entity);
                            }
                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(35, "マルイ", ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                            fun.WriteLog(ex.Message + entity.orderCode, "035-");
                        }
                    }
                }

                else
                {
                    fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                }

                if (i < dt035.Rows.Count - 1)
                {

                    string ordercode = dt035.Rows[++i]["発注コード"].ToString();
                    webBrowser1.AllowNavigation = true;
                    webBrowser1.Navigate("https://intertecinc.jp/ecuser/item/itemDetail?itemCd=" + ordercode);

                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 35;
                    qe.flag = 2;
                    qe.starttime = st;
                    qe.endtime = DateTime.Now.ToString();
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                fun.WriteLog(ex.Message + entity.orderCode, "035-");
            }
        }

        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Access Denied!", dt035.Rows[i]["JANコード"].ToString(), dt035.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
            fun.WriteLog(StatusCode.ToString() + " " + dt035.Rows[i]["発注コード"].ToString(), "035--");
            Application.Exit();
        }

    }
}
