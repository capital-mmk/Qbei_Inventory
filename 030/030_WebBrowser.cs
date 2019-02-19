using System;
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


namespace _030
{
    public partial class frm030 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt030 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;
        public static string st = string.Empty;
        public frm030()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 30;
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(30);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(30);
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
                fun.setURL("030");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(30);
                fun.Qbei_ErrorDelete(30);
                dt030 = fun.GetDatatable("030");
                dt030 = fun.GetOrderData(dt030, "https://joto-order.jp/jotwebb2b/itemList/searchItemList?searchKeiWord=", "030", string.Empty);
                fun.GetTotalCount("030");
                ReadData();
            }
            catch (Exception) { }
        }

        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 30;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "030-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 30;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("userName").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("password").InnerText = password;
                webBrowser1.Document.GetElementsByTagName("input").GetElementsByName("tradePriceViewFlg")[0].InvokeMember("Click");
                fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), ex.Message, dt030.Rows[i]["JANコード"].ToString(), dt030.Rows[i]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
                fun.WriteLog(ex.Message, "030-");
                Application.Exit();
            }
        }

        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = webBrowser1.Url.ToString();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= webBrowser1_Login;
            string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
            if (body.Contains(" ユーザ名またはパスワードが間違っています"))
            {
                fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
                fun.WriteLog("Login Failed", "030-");
                Application.Exit();
            }
            webBrowser1.DocumentCompleted += webBrowser1_WaitForSearchPage;
        }

        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            fun.WriteLog("Login success             ------", "030-");
            string ordercode = dt030.Rows[i]["発注コード"].ToString();
            // string ordercode = "1010001201";
            webBrowser1.Navigate("https://joto-order.jp/jotwebb2b/itemList/searchItemList?searchKeiWord=" + ordercode);
            if (webBrowser1.Url.ToString().Contains("/itemList/searchItemList"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }

        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = webBrowser1.Url.ToString();
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.sitecode = "030";
            entity.siteID = 30;
            entity.janCode = dt030.Rows[i]["JANコード"].ToString();
            entity.partNo = dt030.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt030.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt030.Rows[i]["発注コード"].ToString();
            //entity.orderCode = "1010001201";
            entity.purchaseURL = "https://joto-order.jp/jotwebb2b/itemList/searchItemList?searchKeiWord=" + entity.orderCode;

            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                if (!body.Contains("メーカー"))
                {
                    //2018/01/17 Start
                    if (dt030.Rows[i]["在庫情報"].ToString().Contains("empty") && dt030.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                    {
                        entity.qtyStatus = dt030.Rows[i]["在庫情報"].ToString();
                        entity.stockDate = dt030.Rows[i]["入荷予定"].ToString();
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                    }
                    entity.price = dt030.Rows[i]["下代"].ToString();
                    fun.Qbei_Inserts(entity);
                    //fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
                    //2018/01/17 End
                }
                else
                {
                    try
                    {
                        string html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        //Check Element Exist or not
                        if (hdoc.DocumentNode.SelectSingleNode("div/div[4]/div[2]/table/tbody/tr[2]/td[2]/span") == null)
                        {
                            fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
                            fun.WriteLog("Access Denied! " + entity.orderCode, "030-");
                            Application.Exit();
                        }
                        string alt = hdoc.DocumentNode.SelectSingleNode("div/div[4]/div[2]/table/tbody/tr[2]/td[2]/span").InnerText;

                        entity.price = hdoc.DocumentNode.SelectSingleNode("div/div[4]/div[2]/table/tbody/tr[2]/td[4]").InnerText;
                        entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);

                        entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Equals("×") || alt.Equals("完売") ? "empty" : "unknown status";

                        string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        entity.stockDate = alt.Equals("○") || alt.Equals("△") ? "2100-01-01" : alt.Equals("×") || alt.Equals("完売") ? "2100-02-01" : "unknown date";
                        //2018/01/17 Start
                        if ((dt030.Rows[i]["在庫情報"].ToString().Contains("empty") || dt030.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt030.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                            {
                                entity.qtyStatus = dt030.Rows[i]["在庫情報"].ToString();
                                entity.stockDate = dt030.Rows[i]["入荷予定"].ToString();
                                entity.price = dt030.Rows[i]["下代"].ToString();
                            }
                        }
                        //2018/01/17 End
                        fun.Qbei_Inserts(entity);
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
                        fun.WriteLog(ex.Message + entity.orderCode, "030-");
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
            }
            if (i < dt030.Rows.Count - 1)
            {
                string ordercode = fun.ReplaceOrderCode(dt030.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "-", "在庫処分/empty/", "バラ注文できない為発注禁止/" });
                webBrowser1.Navigate("https://joto-order.jp/jotwebb2b/itemList/searchItemList?searchKeiWord=" + ordercode);
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 30;
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
            fun.Qbei_ErrorInsert(30, fun.GetSiteName("030"), "Access Denied!", dt030.Rows[i]["JANコード"].ToString(), dt030.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "030");
            fun.WriteLog(StatusCode.ToString() + " " + dt030.Rows[i]["発注コード"].ToString(), "030-");
            Application.Exit();
        }
    }
}
