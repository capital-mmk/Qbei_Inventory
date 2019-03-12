using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Threading;
using System.Runtime.InteropServices;


namespace _042
{
    public partial class frm042 : Form
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

        DataTable dt = new DataTable();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        CommonFunction fun = new CommonFunction();
        DataTable dt042 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        private System.Windows.Forms.Timer timer1;
        public static string st = string.Empty;
        //2017/11/15
        string username = string.Empty;
        string password = string.Empty;
        //2017/11/15
        public frm042()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {

            DataTable dtflag = fun.SelectFlag(42);

            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 42;
            //st = qe.starttime;
            qe.flag = 1;
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(42);
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
                fun.setURL("042");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(42);
                fun.Qbei_ErrorDelete(42);
                dt042 = fun.GetDatatable("042");
                dt042 = fun.GetOrderData(dt042, "https://sales.amersports.com/index.php/b2bsfa_6100_ja/b2bsfa/b2b/materials/?aIds[]=current_", "042", "");
                fun.GetTotalCount("042");
                ReadData();
            }
            catch (Exception) { }
        }

        private void ReadData()
        {
            qe.SiteID = 42;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            //2017/11/15
            dt = qubl.Qbei_Setting_Select(qe);
            username = dt.Rows[0]["UserName"].ToString();
            password = dt.Rows[0]["Password"].ToString();
            //2017/11/15
            webBrowser1.Navigate(fun.url);
            FlushMemory();
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            GC.Collect();
            webBrowser1.DocumentCompleted -= webBrowser1_Start;
            webBrowser1.ScriptErrorsSuppressed = true;
            fun.WriteLog("Navigation to Site Url success------", "042-");
            //2017/11/15
            //dt = qubl.Qbei_Setting_Select(qe);
            //string username = dt.Rows[0]["UserName"].ToString();
            //string password = dt.Rows[0]["Password"].ToString();
            //2017/11/15
            fun.GetElement("input", "login", "name", webBrowser1).InnerText = username;
            fun.GetElement("input", "password", "name", webBrowser1).InnerText = password;
            fun.GetElement("button", "connect_button", "id", webBrowser1).InvokeMember("click");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                GC.Collect();
                webBrowser1.ScriptErrorsSuppressed = true;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("あなたの識別子（ログインまたはパスワード）が間違っています"))
                {
                    fun.Qbei_ErrorInsert(42, fun.GetSiteName("042"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "042");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "042-");
                    //string url = webBrowser1.Url.ToString();
                    //webBrowser1.Stop();
                    //webBrowser1.Navigate("https://sales.amersports.com/index.php/b2bsfa_6100_ja/");
                    //FlushMemory();
                    //webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                    //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch);
                    if (i < dt042.Rows.Count)
                    {
                        entity.janCode = dt042.Rows[i]["JANコード"].ToString();
                        fun.GetElement("input", "search-text", "name", webBrowser1).InnerText = entity.janCode;
                        string div = webBrowser1.Document.GetElementById("module-66").InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(div);

                        HtmlNode node = hdoc.DocumentNode.SelectSingleNode("//button");
                        webBrowser1.Document.GetElementsByTagName("button")[0].InvokeMember("click");
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch);
                    }
                    //2017/11/07
                    //timer1 = new System.Windows.Forms.Timer();
                    //timer1.Tick += new EventHandler(webBrowser1_ItemSearch);
                    //timer1.Interval = 25000;
                    //timer1.Start();
                }
            }
            catch (Exception ex)
            {
                this.Controls.Remove(webBrowser1);
                this.Controls.Add(webBrowser1);
                //webBrowser1.Navigate(fun.url);
                webBrowser1.Stop();
                webBrowser1.Url = new Uri(fun.url);
                FlushMemory();
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
            }
        }

        private void webBrowser_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                GC.Collect();
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch);
                timer1 = new System.Windows.Forms.Timer();
                timer1.Tick += new EventHandler(webBrowser1_ItemSearch);
                timer1.Interval = 25000;
                timer1.Start();
            }
            catch (Exception ex)
            {
                this.Controls.Remove(webBrowser1);
                this.Controls.Add(webBrowser1);
                webBrowser1.Stop();
                webBrowser1.Url = new Uri(fun.url);
                FlushMemory();
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
            }
        }
        //private void webBrowser_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    try
        //    {
        //        GC.Collect();
        //        webBrowser1.ScriptErrorsSuppressed = true;
        //        //string url = webBrowser1.Url.ToString();
        //        //string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
        //        webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch);
        //        //2017/11/07
        //        if (i < dt042.Rows.Count)
        //        {
        //            entity.janCode = dt042.Rows[i]["JANコード"].ToString();
        //            fun.GetElement("input", "search-text", "name", webBrowser1).InnerText = entity.janCode;
        //            string div = webBrowser1.Document.GetElementById("module-66").InnerHtml;
        //            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
        //            hdoc.LoadHtml(div);

        //            HtmlNode node = hdoc.DocumentNode.SelectSingleNode("//button");
        //            webBrowser1.Document.GetElementsByTagName("button")[0].InvokeMember("click");
        //        }
        //        //2017/11/07
        //        timer1 = new System.Windows.Forms.Timer();
        //        timer1.Tick += new EventHandler(webBrowser1_ItemSearch);
        //        timer1.Interval = 25000;
        //        timer1.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Controls.Remove(webBrowser1);
        //        this.Controls.Add(webBrowser1);
        //        webBrowser1.Stop();
        //        webBrowser1.Url = new Uri(fun.url);
        //        FlushMemory();
        //        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        //    }
        //}

        private void webBrowser1_ItemSearch(object sender, EventArgs e)
        {
            try
            {
                //webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                GC.Collect();
                webBrowser1.ScriptErrorsSuppressed = true;
                timer1.Tick -= new EventHandler(webBrowser1_ItemSearch);
                if (i <= dt042.Rows.Count - 1)
                {
                    string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                    string orderCode = dt042.Rows[i]["発注コード"].ToString();
                    entity.orderCode = fun.ReplaceOrderCode(orderCode, new string[] { "在庫処分/inquiry/", "-", "在庫処分/empty/", "バラ注文できない為発注禁止/", "/", "]" }).Trim();
                    orderCode.Replace("-", string.Empty);

                    if ((body.Contains("0の商品が見つかりました")))
                    {
                        fun.Qbei_ErrorInsert(42, fun.GetSiteName("042"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "042");
                        i++;
                        //2017/11/15
                        //webBrowser1.Navigate("https://sales.amersports.com/index.php/b2bsfa_6100_ja/");
                        webBrowser1.Stop();
                        webBrowser1.Url = new Uri("https://sales.amersports.com/index.php/b2bsfa_6100_ja/");
                        FlushMemory();
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                        //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
                        //2017/11/15
                    }
                    else
                    {
                        string url = webBrowser1.Url.ToString();
                        body = webBrowser1.Document.Body.InnerHtml;

                        url = webBrowser1.Url.ToString();

                        fun.GetElement("div", "materialstatusmessage", "id", webBrowser1).InvokeMember("click");
                        //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser2_ItemSearch);
                        timer1 = new System.Windows.Forms.Timer();
                        timer1.Tick += new EventHandler(webBrowser2_ItemSearch);
                        timer1.Interval = 7000;
                        timer1.Start();
                    }
                }
                else
                {
                    qe.site = 42;
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
                //webBrowser1.Dispose();
                //webBrowser1 = new WebBrowser();
                this.Controls.Remove(webBrowser1);
                this.Controls.Add(webBrowser1);
                webBrowser1.Stop();
                webBrowser1.Url = new Uri(fun.url);
                FlushMemory();
                //webBrowser1.Navigate(fun.url);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
            }
        }
        private void webBrowser2_ItemSearch(object sender, EventArgs e)
        {
            GC.Collect();
            // webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser2_ItemSearch);
            //webBrowser1.Document.GetElementById("b2bproduct-netprice").InvokeMember("click");
            //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser3_ItemSearch);
            timer1.Tick -= new EventHandler(webBrowser2_ItemSearch);
            webBrowser1.Document.GetElementById("b2bproduct-netprice").InvokeMember("click");
            //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser3_ItemSearch);
            timer1.Tick += new EventHandler(webBrowser3_ItemSearch);
        }
        private void webBrowser3_ItemSearch(object sender, EventArgs e)
        {
            GC.Collect();
            // webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser3_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            timer1.Tick -= new EventHandler(webBrowser3_ItemSearch);

            string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerHtml;
            string url = webBrowser1.Url.ToString();
            entity = new Qbei_Entity();
            entity.siteID = 42;
            entity.sitecode = "042";
            // entity.janCode = "0889645105116";
            entity.janCode = dt042.Rows[i]["JANコード"].ToString();
            entity.partNo = dt042.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt042.Rows[i]["最終反映日"].ToString();
            //entity.orderCode = "33472_617660";
            string orderCode = dt042.Rows[i]["発注コード"].ToString();
            entity.orderCode = fun.ReplaceOrderCode(orderCode, new string[] { "在庫処分/inquiry/", "-", "在庫処分/empty/", "バラ注文できない為発注禁止/", "/", "]" }).Trim();
            orderCode.Replace("-", string.Empty);
            entity.purchaseURL = fun.url + "/index.php/b2bsfa_6100_ja/b2bsfa/b2b/materials/?aIds[]=current_" + entity.orderCode;
            try
            {
                entity.stockDate = string.Empty;
                string html = webBrowser1.Document.Body.InnerHtml;
                HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                hdoc.LoadHtml(html);
                string alt = string.Empty;
                body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                entity.price = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/div[2]/div/p[3]/span").InnerText;
                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);


                try
                {
                    int trCount = hdoc.DocumentNode.SelectNodes("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr").Count;
                    for (int j = 1; j <= trCount; j = j + 1)
                    {
                        string qty1 = hdoc.DocumentNode.SelectSingleNode("/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]").GetAttributeValue("id", "");
                        qty1 = qty1.Replace("row-", string.Empty);
                        if (qty1 == entity.orderCode)
                        {
                            if (hdoc.DocumentNode.SelectNodes("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td").Count < 5)
                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[4]").InnerText;
                            else
                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[5]").InnerText;
                            alt = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[3]").InnerText;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    webBrowser1.Document.GetElementsByTagName("a")[0].InvokeMember("click");
                    fun.GetElement("div", "b2bv2-product-element-49295_556969", "id", webBrowser1).InvokeMember("click");
                    int trCount = hdoc.DocumentNode.SelectNodes("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr").Count;
                    for (int j = 1; j <= trCount; j = j + 1)
                    {
                        string qty1 = hdoc.DocumentNode.SelectSingleNode("/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]").GetAttributeValue("id", "");
                        qty1 = qty1.Replace("row-", string.Empty);
                        if (qty1 == entity.orderCode)
                        {
                            if (hdoc.DocumentNode.SelectNodes("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td").Count < 5)
                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[4]").InnerText;
                            else
                                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[5]").InnerText;
                            alt = hdoc.DocumentNode.SelectSingleNode("div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/table/tbody/tr[" + j + "]/td[3]").InnerText;
                            break;
                        }
                    }
                }
                if (entity.stockDate.Equals("Final availability"))
                {
                    entity.stockDate = "2100/02/01";
                }
                else
                {
                    entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? entity.stockDate : alt.Equals("完売") ? "2100-02-01" : "unknown date";
                }
                entity.qtyStatus = alt.Contains("○") || fun.IsNumber(alt) ? "good" : alt.Equals("△") || fun.IsSmall1(alt) ? "small" : alt.Equals("×") || fun.IsEmpty(alt) ? "empty" : "unknown status";
                if (entity.stockDate.Contains("2月"))
                {
                    entity.stockDate = "2018-02-28";
                }

                fun.Qbei_Inserts(entity);
            }
            catch (Exception ex)
            {
                //fun.Qbei_ErrorInsert(42, fun.GetSiteName("042"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "042");
                fun.WriteLog(ex.Message, "042-" + entity.janCode + " " + entity.orderCode);
                //2017/11/22
                //webBrowser1.Dispose();
                //webBrowser1 = new WebBrowser();
                this.Controls.Remove(webBrowser1);
                //FlushMemory();
                this.Controls.Add(webBrowser1);
                webBrowser1.Stop();
                webBrowser1.Url = new Uri(fun.url);
                FlushMemory();
                //webBrowser1.Navigate(fun.url);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
                return;
                //2017/11/22
            }
            i++;
            webBrowser1.Document.GetElementsByTagName("a")[0].InvokeMember("click");
            //webBrowser1.Document.GetElementsByTagName("a")[1].InvokeMember("click");
            //2017/11/15
            webBrowser1.Stop();
            webBrowser1.Navigate("https://sales.amersports.com/index.php/b2bsfa_6100_ja/");
            FlushMemory();
            //webBrowser1.Navigate(fun.url);
            Thread.Sleep(5000);
            //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_ItemSearch);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            //2017/11/15
        }

        private static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

    }
}
