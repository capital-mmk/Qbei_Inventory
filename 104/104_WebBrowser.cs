using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _104
{
    public partial class frm104 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt104 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i =0;
        
        public frm104()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataTable dtflag = fun.SelectFlag(104);
                qe.site = 104;
                qe.flag = 0;
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(104);
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
                fun.WriteLog(ex, "104-");
                Application.Exit();
                Environment.Exit(0);
            }
        }
        public void StartRun()
        {
            try
            {
                qe.SiteID =104;
                fun.setURL("104");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(104);
                fun.Qbei_ErrorDelete(104);
                dt104 = fun.GetDatatable("104");
                dt104 = fun.GetOrderData(dt104, "http://www2.gear-m.co.jp/moss/SearchSv?tono=", "104", "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                if (dt104 == null)
                {
                    qe.site = 104;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
                fun.GetTotalCount("104");
                ReadData();               
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "104-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void ReadData()
        {
            qe.SiteID = 104;
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
                webBrowser1.ScriptErrorsSuppressed = true;

                fun.WriteLog("Navigation to Site Url success------", "104-");
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "user", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "pass", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "login", "name", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt104.Rows[0]["JANコード"].ToString();
                string orderCode = dt104.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(104, fun.GetSiteName("104"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "104");
                fun.WriteLog(ex, "104-", janCode, orderCode);

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
                if (body.Contains("Your identifiers (login or password) aren't correct"))
                {
                    fun.Qbei_ErrorInsert(104, fun.GetSiteName("104"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "104");                    
                    fun.WriteLog("Login Failed", "104-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "104-");
                    orderCode = dt104.Rows[i]["発注コード"].ToString();
                    webBrowser1.Navigate(fun.url + "/SearchSv?tono=" + orderCode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt104.Rows[i]["JANコード"].ToString();
                orderCode = dt104.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(104, fun.GetSiteName("104"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "104");
                fun.WriteLog(ex, "104-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 104;
            entity.sitecode = "104";
            entity.janCode = dt104.Rows[i]["JANコード"].ToString();
            entity.partNo = dt104.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt104.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt104.Rows[i]["発注コード"].ToString();
                       
            entity.purchaseURL = fun.url + "/SearchSv?tono=" + entity.orderCode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25";

            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (body.Contains("該当する製品はございません。もう一度、検索しなおして下さい。"))
                {
                    if (dt104.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt104.Rows[i]["在庫情報"].ToString().Contains("empty"))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                        entity.price = dt104.Rows[i]["下代"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                        entity.price = dt104.Rows[i]["下代"].ToString();
                    }
                    fun.Qbei_Inserts(entity);
                }
                else
                {
                    try
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);

                        string alt = hdoc.DocumentNode.SelectSingleNode("/table[1]/tbody/tr/td[9]").InnerText;
                        alt = alt.Replace("台", string.Empty);
                        alt = alt.Replace(" 以上", string.Empty);
                        entity.qtyStatus = alt.Contains("○") ? "good" : alt.Contains("△") || alt.Contains("▲") ? "small" : alt.Contains("×") ? "empty" : "unknown status";
                        entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("完売") ? "2100-02-01" : "unknown status";
                        
                        entity.price = hdoc.DocumentNode.SelectSingleNode("table[1]/tbody/tr[1]/td[10]").InnerText;
                        entity.price = entity.price.Replace("￥", string.Empty);
                        entity.price = entity.price.Replace(",", string.Empty).Replace("円", string.Empty).Replace("（税抜）", string.Empty).Replace("\\", string.Empty);
                        if (entity.stockDate.Contains("2月"))
                        {
                            entity.stockDate = "2018-02-28";
                        }
                        if ( dt104.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                            {
                                entity.qtyStatus = dt104.Rows[i]["在庫情報"].ToString();
                                entity.price = dt104.Rows[i]["下代"].ToString();
                                entity.stockDate = dt104.Rows[i]["入荷予定"].ToString();
                            }
                            fun.Qbei_Inserts(entity);
                        }
                        else
                            fun.Qbei_Inserts(entity);
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(104, fun.GetSiteName("104"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "104");
                        fun.WriteLog(ex, "104-", entity.janCode, entity.orderCode);                        
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(104, fun.GetSiteName("104"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),"104");
            }
            
            if (i < dt104.Rows.Count-1)
            {
                string ordercode = dt104.Rows[++i]["発注コード"].ToString();
                webBrowser1.Navigate(fun.url + "/SearchSv?tono=" +ordercode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 104;
                qe.flag =2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }
        }
    }
}
