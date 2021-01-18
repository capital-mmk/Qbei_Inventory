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
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm143 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt143 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;


        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm143()
        {
            InitializeComponent();
            testflag();
        }

        /// <summary>
        /// testflag processing.
        /// </summary>
        ///<remark>
        ///"0,1,2"Flage Number of Check. 
        ///"0" is Start Process.
        ///"1" is Processing.
        ///"2" is End Process.
        ///</remark>
        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 143;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(143);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());

                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                /// Check to flag is "0" or "1" or "2".
                /// when flag is 0,Change to flag is 1 and Continue to StartRun Process.
                /// </remark>
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    fun.WriteLog("Flag0 ------", "143-");
                    StartRun();
                }

                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (flag == 1)
                {
                    fun.deleteData(143);
                    fun.ChangeFlag(qe);
                    fun.WriteLog("Flag1 ------", "143-");
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
                fun.WriteLog(ex, "143-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        public void StartRun()
        {
            try
            {
                fun.setURL("143");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(143);
                fun.Qbei_ErrorDelete(143);
                dt143 = fun.GetDatatable("143");
                //dt143 = fun.GetOrderData(dt143, "http://www.podium-edi.com//goods/goods_list.html", "143", "");//<remark Close Logic Of Onceaweek />
                fun.GetTotalCount("143");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "143-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Read to Data and Url.
        /// </remark>
        private void ReadData()
        {
            qe.SiteID = 143;
            dt = qubl.Qbei_SettingSelect(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(2000);
            webBrowser1.Navigate(fun.url + "/login");
            fun.WriteLog("Go to Url------", "143-");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "143-");
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
                string janCode = dt143.Rows[0]["JANコード"].ToString();
                string orderCode = dt143.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex, "143-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("IDまたはパスワードが誤っています"))
                {
                    fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                    fun.WriteLog("Login Failed", "143-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "143-");
                    webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt143.Rows[0]["JANコード"].ToString();
                string orderCode = dt143.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex, "143-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
            if (webBrowser1.Url.ToString().Contains("goods_list.html"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
        }

        /// <summary>
        /// Inspection of item at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                fun.ClearMemory();

                if (i <= dt143.Rows.Count - 1)
                {
                    string Url = webBrowser1.Url.ToString();
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    orderCode = dt143.Rows[i]["発注コード"].ToString();
                    fun.GetElement("input", "15", "maxlength", webBrowser1).InnerText = orderCode;

                    //fun.GetElement("input", "search_C_sSyohinCd_item", "name", webBrowser1).InnerText = orderCode;

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
                string janCode = dt143.Rows[i]["JANコード"].ToString();
                orderCode = dt143.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                fun.WriteLog(ex, "143-", janCode, orderCode);

                i++;
                webBrowser1.Navigate(fun.url + "/goods/goods_list.html");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection hc;
            string strUrl = string.Empty;
            entity = new Qbei_Entity();
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
                    //<remark 2021/01/06>
                    entity.True_StockDate = "Not Found";
                    entity.True_Quantity = "Not Found";
                    //</remark 2021/01/06>
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
                        body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerHtml;
                        hc = webBrowser1.Document.GetElementsByTagName("table");

                        //Check Element Exist or not
                        if (hc.Count == 0)
                        {
                            fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "143-");
                            Application.Exit();
                            Environment.Exit(0);
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
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "143-");
                            Application.Exit();
                            Environment.Exit(0);
                        }

                        string alt = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[3]/td[6]/table/tbody/tr[2]/td").InnerText;
                        entity.price = dt143.Rows[i]["下代"].ToString();//5月31日 
                        //string price = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[3]/td[5]/table/tbody/tr/td[1]").InnerText;
                        //string[] ae = price.Split('!');
                        //strUrl = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[4]/td").InnerHtml;
                        //strUrl = strUrl.Split('>')[0];
                        //var syntax = new string[] { "amp;", "<a", "href=", "\"", "'", "onclick=", "winOpenResizable(", "../", ",580,400);", "return false;", ">", " " };
                        //syntax.ToList().ForEach(o => strUrl = strUrl.Replace(o, string.Empty));
                        //entity.purchaseURL = fun.url + strUrl;
                        //entity.price = ae[0];
                        //entity.price = entity.price.Replace(",", string.Empty).Replace("<", string.Empty);
                        //<remark 2021/01/06>
                        entity.True_StockDate = "項目無し";
                        entity.True_Quantity = alt;
                        //</remark 2021/01/06>
                        try
                        {
                            //string stockpath = "table/tbody/tr[2]/td/div[2]/div[2]/table/tbody/tr[3]/td[4]/img";
                            string stockpath = "/tbody/tr[3]/td[4]/img";
                            string stockpath2 = "/ tbody / tr[3] / td[4] / img[2]";//<remark Stockdate Logic 追加　2020/02/28>
                            string stockpath3 = "/ tbody / tr[3] / td[4] / img[3]";//<remark Stockdate Logic 追加　2020/04/03>

                            HtmlNodeCollection nc = hdoc.DocumentNode.SelectNodes(stockpath);
                            if (nc == null)
                            {
                                //2018-05-08 Start
                                //stockpath = fun.GetElement("select", "order_status[0]", "name", webBrowser1).InnerText;
                                //if (stockpath.Contains("注文"))
                                //{
                                //entity.stockDate = "2100-01-01";
                                entity.stockDate = "2100-02-01";//<remark Edit Logic of Stockdate />
                                //}
                                //2018-05-08 End
                            }
                            else
                            {
                                //<remark Stockdateについて、ロジックの編集　2020/04/03 Start>
                                //<remark Stockdate Logic 追加　2020/02/28>                              
                                //if (sdimg.Contains("stock.gif"))
                                //if (hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("alt", "").Contains("在庫限り") || sdimg.Contains("stock.gif") || alt.Contains("完売") || (sdimg.Contains("stock.gif") && (alt.Equals("○") || alt.Contains("▲"))))
                                //{
                                //    entity.stockDate = "2100-02-01";
                                //}
                                ////<remark Stockdate Logic 追加　2020/02/28 Start>
                                ////<remark Stockdate Logic 追加　2020/03/26 Start>
                                ////else if (sdimg.Contains("new.gif") && sdimg2.Contains("limited_stock.gif") && alt.Contains("×"))
                                //else if ((sdimg.Contains("new.gif") && sdimg2.Contains("limited_stock.gif") && alt.Contains("×")) || sdimg2.Contains("limited_stock.gif") && alt.Contains("×"))
                                ////</remark 2020/03/26 End>
                                //{
                                //    entity.stockDate = "2100-02-01";
                                //}
                                ////</remark 2020/02/28 End>
                                //else if (sdimg.Contains("new.gif") && alt.Contains("×"))
                                //{
                                //    entity.stockDate = "2100-01-10";
                                //}
                                //else
                                //{
                                //    entity.stockDate = "2100-01-01";
                                //}
                                string sdimg;
                                string sdimg2;
                                string sdimg3;
                                //if (alt.Equals("○") || alt.Equals("▲"))
                                if (alt.Equals("○"))//<remark Change Logic of stockdate 2020/07/24 />
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                                //else if (alt.Contains("完売"))
                                else if (alt.Equals("▲") || alt.Contains("完売"))//<remark Change Logic of stockdate 2020/07/24 />
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                else if (hdoc.DocumentNode.SelectSingleNode(stockpath2) == null)
                                {
                                    sdimg = hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("src", "");

                                    //if (sdimg.Contains("stock.gif"))
                                    //if (alt.Contains("完売") || (sdimg.Contains("limited_stock.gif") && (alt.Equals("○") || alt.Contains("▲") || alt.Contains("×"))))
                                    if ((sdimg.Contains("limited_stock.gif") && alt.Contains("×")) || (sdimg.Contains("senkou") && alt.Contains("完売")) || (sdimg.Contains("finish") && alt.Contains("×")))
                                    {
                                        entity.stockDate = "2100-02-01";
                                    }
                                    else if (hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("alt", "").Contains("在庫限り") || sdimg.Contains("limited_stock.gif"))
                                    {
                                        entity.stockDate = "2100-02-01";
                                        alt = "×";
                                    }
                                    else if (sdimg.Contains("new.gif") && alt.Contains("×"))
                                    {
                                        //<remark Stockdateの変更　2020/07/08 Start>
                                        // entity.stockDate = "2100-01-10";
                                        entity.stockDate = "2100-02-01";
                                        //<remark Stockdateの変更　2020/07/08 End>
                                    }
                                }
                                else if (hdoc.DocumentNode.SelectSingleNode(stockpath3) != null)
                                {
                                    sdimg = hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("src", "");
                                    sdimg2 = hdoc.DocumentNode.SelectSingleNode(stockpath2).GetAttributeValue("src", "");
                                    sdimg3 = hdoc.DocumentNode.SelectSingleNode(stockpath3).GetAttributeValue("src", "");//<remark Stockdate Logic 追加　2020/04/03>
                                    if (sdimg.Contains("limited_stock.gif") || sdimg2.Contains("limited_stock.gif") || sdimg3.Contains("limited_stock.gif"))
                                    {
                                        entity.stockDate = "2100-02-01";
                                    }
                                }
                                else if (hdoc.DocumentNode.SelectSingleNode(stockpath2) != null)
                                {
                                    sdimg = hdoc.DocumentNode.SelectSingleNode(stockpath).GetAttributeValue("src", "");
                                    sdimg2 = hdoc.DocumentNode.SelectSingleNode(stockpath2).GetAttributeValue("src", "");//<remark Stockdate Logic 追加　2020/02/28>                                    

                                    //if ((sdimg.Contains("new.gif") && sdimg2.Contains("limited_stock.gif") && alt.Contains("×")) || (sdimg.Contains("limited_stock.gif") && sdimg2.Contains("new.gif") && alt.Contains("×")) || sdimg.Contains("limited_stock.gif"))
                                    //</remark 2020/03/26 End
                                    if (sdimg.Contains("limited_stock.gif") || sdimg2.Contains("limited_stock.gif"))
                                    {
                                        entity.stockDate = "2100-02-01";
                                    }
                                }

                                else
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                            }
                        }
                        catch
                        {
                            entity.stockDate = "2100-01-10";
                        }
                        //entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("▲") ? "small" : alt.Equals("×") || alt.Contains("完売") ? "empty" : "unknown status";
                        entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("▲") || alt.Equals("×") || alt.Contains("完売") ? "empty" : "unknown status";//<remark Change Logic of quantity 2020/07/24 />

                        //if ((dt143.Rows[i]["在庫情報"].ToString().Contains("empty") || dt143.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt143.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        //{
                        //    if (((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-01-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals("2100-02-01"))) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2100-01-10")) && (entity.qtyStatus.Equals("inquiry"))))
                        //    {
                        //        entity.qtyStatus = dt143.Rows[i]["在庫情報"].ToString();
                        //        entity.price = dt143.Rows[i]["下代"].ToString();
                        //        entity.stockDate = dt143.Rows[i]["入荷予定"].ToString();
                        //    }
                        //    fun.Qbei_Inserts(entity);
                        //}
                        //else
                        //</remark 2020/04/03 End>

                        fun.Qbei_Inserts(entity);
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                        fun.WriteLog(ex, "143-", entity.janCode, entity.orderCode);
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

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt143.Rows[i]["JANコード"].ToString();
            string orderCode = dt143.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "143-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}