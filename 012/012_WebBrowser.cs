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
using System.Security.Permissions;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Threading;
using System.Text.RegularExpressions;

namespace _12カワシマ
{
    public partial class frm012 : Form
    {

        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt012 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        public frm012()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            qe.site = 12;
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(12);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {
                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(12);
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
                webBrowser1.AllowWebBrowserDrop = false;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.WebBrowserShortcutsEnabled = false;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
                fun.setURL("012");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(12);
                fun.Qbei_ErrorDelete(12);
                dt012 = fun.GetDatatable("012");
                dt012 = fun.GetOrderData(dt012, "https://www.riobike.com/shop/g/g", "012", string.Empty);
                fun.GetTotalCount("012");
                ReadData();
            }
            catch (Exception ex) { fun.WriteLog(ex.Message, "012"); }
        }

        private void ReadData()
        {
            qe.SiteID = 12;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);

        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "012-");
                qe.SiteID = 12;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("login_uid").InnerText = username;
                dt = qubl.Qbei_Setting_Select(qe);
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("login_pwd").InnerText = password;

                fun.GetElement("input", "order", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, dt012.Rows[0]["JANコード"].ToString(), dt012.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex.Message + dt012.Rows[0]["発注コード"].ToString(), "012-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください"))
                {
                    fun.Qbei_ErrorInsert(12, "カワシマ", "Login Failed", dt012.Rows[0]["JANコード"].ToString(), dt012.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                    fun.WriteLog("Login Failed " + dt012.Rows[0]["発注コード"].ToString(), "012-");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "012-");
                    //string ordercode = fun.ReplaceOrderCode(dt012.Rows[0]["発注コード"].ToString(), new string[] { "在庫処分/empty/","在庫処分/empry/", "在庫処分/inquiry/","-","発注禁止","在庫処分/small/","在庫処分/good/", 
                    //                                                                       "東特価のため完売/","在庫処分/empry/在庫処分/empry/small/", 
                    //                                                                        "バラ注文できない為発注禁止/", "特価発注分/inquiry/", "発注禁止/", 
                    //                                                                        "在庫処分empry","（カワシマ）","バラ注文できない為/small/",
                    //                                                                         "バラ注文できない為small","/","発注禁止/在庫処分/empty/"});
                    string ordercode = fun.ReplaceOrderCode(dt012.Rows[0]["発注コード"].ToString(), new string[] { "--" });
                    //string ordercode = "0177740001";
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + ordercode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, dt012.Rows[0]["JANコード"].ToString(), dt012.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex.Message + dt012.Rows[0]["発注コード"].ToString(), "012-");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string strStockDate = string.Empty;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);

            try
            {
                entity = new Qbei_Entity();
                entity.siteID = 12;
                entity.sitecode = "012";
                entity.janCode = dt012.Rows[i]["JANコード"].ToString();
                entity.partNo = dt012.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt012.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt012.Rows[i]["発注コード"].ToString();
                entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode;


                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;


                    if (body.Contains("申し訳ございません"))
                    {
                        if (dt012.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt012.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt012.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt012.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);
                    }

                    else
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        //Check Element Exist or not
                        if (hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span") == null && webBrowser1.Document.GetElementById("spec_stock_msg") == null)
                        {
                            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                            fun.WriteLog("Access Denied! * " + entity.orderCode, "012-");
                            Application.Exit();
                        }
                        else
                        {
                            entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div[3]/div/div[1]/div[2]/div[2]/table/tbody/tr[7]/td/span").InnerText.Replace("￥", "").Replace(",", "");

                            string alt = webBrowser1.Document.GetElementById("spec_stock_msg").InnerText.Trim();
                            // if (alt.Contains("×") && (webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText.Contains("在庫限り") || webBrowser1.Document.GetElementById("spec_goods_name").InnerText.Contains("在庫限り")))
                            if (alt.Contains("×") && ((webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText.Contains("在庫限り")) || (webBrowser1.Document.GetElementById("spec_goods_name").InnerText != null && webBrowser1.Document.GetElementById("spec_goods_name").InnerText.Contains("在庫限り"))))
                            {
                                entity.stockDate = "2100-02-01";
                                entity.qtyStatus = "empty";

                            }


                            else
                            {
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Contains("×") || alt.Contains("終了") || alt.Equals("×(終了)") ? "empty" : "unknown status";
                                //entity.stockDate = webBrowser1.Document.GetElementById("spec_goods_property_name").InnerText;
                                //entity.stockDate = entity.stockDate == null ? string.Empty : entity.stockDate;
                                entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("終了") || alt.Equals("×(終了)") ? "2100-02-01" : "unknown date";
                            }


                            if ((dt012.Rows[i]["在庫情報"].ToString().Contains("empty") || dt012.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt012.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
                                {
                                    entity.qtyStatus = dt012.Rows[i]["在庫情報"].ToString();
                                    entity.price = dt012.Rows[i]["下代"].ToString();
                                    entity.stockDate = dt012.Rows[i]["入荷予定"].ToString();
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
                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                }


            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                fun.WriteLog(ex.Message + entity.orderCode, "012-");
            }
            finally
            {
                if (i < dt012.Rows.Count - 1)
                {
                    webBrowser1.ScriptErrorsSuppressed = true;
                    string ordercode = dt012.Rows[++i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/shop/g/g" + ordercode);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 12;
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
            fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Access Denied!", dt012.Rows[i]["JANコード"].ToString(), dt012.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
            fun.WriteLog(StatusCode.ToString() + " " + dt012.Rows[i]["発注コード"].ToString(), "012-");
            Application.Exit();
        }
    }
}

