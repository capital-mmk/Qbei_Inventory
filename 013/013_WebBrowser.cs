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

namespace _13ミズタニ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm013 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt013 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = -1;
        public static string st = string.Empty;
        string gridViewFormat = "GridView1_ctl{0}";

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm013()
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
            qe.site = 13;
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(13);
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
                StartRun();
            }

            ///<remark>
            ///when flag is 1,To Continue to StartRun Process.
            ///</remark>
            else if (flag == 1)
            {
                fun.deleteData(13);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
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
                fun.setURL("013");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(13);
                fun.Qbei_ErrorDelete(13);
                dt013 = fun.GetDatatable("013");
                //2017/12/14 Start
                //dt013 = fun.GetOrderData(dt013, "https://www.ordermz.jp/weborder/SyohinSearch.aspx", "013", string.Empty);<remark Close Logic 2020/09/01 />
                fun.GetTotalCount("013");
                //2017/12/14 End
                ReadData();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Read to Data and Url.
        /// </remark>
        private void ReadData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 13;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate("https://www.ordermz.jp/weborder");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Date Process
        /// </summary>
        /// <param name="date">Insert to Trim of date.</param>
        private bool IsDate(string date)
        {
            try
            {
                //To Change DateTime of Date Format.
                Convert.ToDateTime(date.Trim());
                return true;
            }
            catch (Exception)
            { return false; }
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.WriteLog("Navigation to Site Url success------", "013-");
                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                qe.SiteID = 13;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("tokuisakicode").InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("loginpasswd").InnerText = password;
                webBrowser1.Document.GetElementById("btnLogin").InvokeMember("Click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[0]["JANコード"].ToString(), dt013.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                fun.WriteLog(ex.Message + dt013.Rows[0]["発注コード"].ToString(), "013-");
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
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// Check to Condition at WebPage.
                /// </remark>
                if (body.Contains(" 得意先コード、パスワードが正しくありません"))
                {
                    fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                    Application.Exit();
                }
                else
                {
                    fun.WriteLog("Login success             ------", "013-");
                    webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[0]["JANコード"].ToString(), dt013.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                fun.WriteLog(ex.Message + dt013.Rows[0]["発注コード"].ToString(), "013-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate(fun.url + "/SyohinSearch.aspx");
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
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
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                //WebBrowser of HtmlDocument.
                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;
                if (i < dt013.Rows.Count - 1)
                {
                    //To Input orderCode at dt013 of "発注コード".
                    orderCode = dt013.Rows[++i]["発注コード"].ToString().Trim();
                    //string orderCode = fun.ReplaceOrderCode(dt013.Rows[++i]["発注コード"].ToString(), new string[] {"在庫限り発注禁止", "在庫処分/inquiry/", "在庫処分/empry/-", "在庫処分good", "在庫処分empryinquiry", "在庫処分/empty/", "在庫処分small", 
                    //                                                                     "在庫限り発注禁止inquiry", "在庫処分/empry/", "在庫処分empry", "東特価のため完売", "在庫処分small", 
                    //                                                                     "在庫処分empry在庫処分empryempty", "在庫処分empry在庫処分empryinquiry", "在庫処分good",
                    //                                                                     "バラ注文できない為発注禁止/", "在庫処分/","inquiry","empty","/////","////","//","/" });

                    doc.GetElementById("keyword").SetAttribute("Value", orderCode);
                    webBrowser1.Document.GetElementById("btnSearch").InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 13;
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
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, dt013.Rows[i]["JANコード"].ToString(), orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                fun.WriteLog(ex.Message + orderCode, "013-");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //while (string.IsNullOrEmpty(tmpOrderCode))
            //{
            //    js.ExecuteScript("document.getElementById('gvSyohin_ctl02_syohincode').value='" + orderCode + "';");
            //    js.ExecuteScript("javascript:setTimeout(__doPostBack('gvSyohin$ctl02$syohincode',''), 0);");
            //    System.Threading.Thread.Sleep(1000);
            //    tmpOrderCode = chrome.FindElement(By.Id("gvSyohin_ctl02_syohincode")).GetAttribute("value");
            //}
            entity = new Qbei_Entity();
            string color = string.Empty;
            HtmlNode colorpath;
            HtmlNode node;
            string qty = string.Empty;
            int intCnt = 0;
            string strHtml;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            entity.janCode = dt013.Rows[i]["JANコード"].ToString();
            entity.partNo = dt013.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt013.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt013.Rows[i]["発注コード"].ToString().Trim();
            //entity.orderCode = fun.ReplaceOrderCode(entity.orderCode, new string[] {"在庫限り発注禁止", "在庫処分/inquiry/", "在庫処分/empry/-", "在庫処分good", "在庫処分empryinquiry", "在庫処分/empty/", "在庫処分small", 
            //                                                                         "在庫限り発注禁止inquiry", "在庫処分/empry/", "在庫処分empry", "東特価のため完売", "在庫処分small", 
            //                                                                         "在庫処分empry在庫処分empryempty", "在庫処分empry在庫処分empryinquiry", "在庫処分good",
            //                                                                         "バラ注文できない為発注禁止/", "在庫処分/","inquiry","empty","/////","////","//","/" });


            entity.purchaseURL = webBrowser1.Url.ToString();
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            entity.siteID = 13;
            entity.sitecode = "013";
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            if (body.Contains("検索条件に該当する商品は、見つかりませんでした"))
            {
                entity.qtyStatus = "empty";
                entity.price = dt013.Rows[i]["下代"].ToString();

                if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                {
                    entity.stockDate = "2100-01-10";
                }
                else
                { entity.stockDate = "2100-02-01"; }
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
                    //To Input html at Webpage Inspect.
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    //hdoc Documennt of html.
                    hdoc.LoadHtml(html);
                    if (webBrowser1.Document.GetElementById("GridView1_ctl02_Label1") == null && (hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]") == null))
                    {
                        fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                        fun.WriteLog("Access Denied! " + entity.orderCode, "013-");
                        Application.Exit();
                    }
                    else
                    {
                        strHtml = webBrowser1.Document.GetElementById("GridView1").InnerHtml;
                        hdoc.LoadHtml(strHtml);

                        intCnt = hdoc.DocumentNode.SelectNodes("/tbody/tr").Count;

                        //if (intCnt > 1)
                        //{
                        //    for (int j = 2; j <= intCnt; j++)
                        //    {
                        //        if (webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_syohincode").InnerText.Equals(entity.orderCode))
                        //        {
                        //            qty = webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_Label1").InnerText;
                        //            entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";
                        //            entity.price = webBrowser1.Document.GetElementById("GridView1_ctl0" + j + "_hanbaikakaku").InnerText;
                        //            entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                        //            node = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]");
                        //            if (node != null)
                        //                entity.stockDate = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]").InnerText;
                        //            colorpath = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[5]");
                        //            color = colorpath.GetAttributeValue("style", string.Empty);
                        //            break;
                        //        }
                        //    }
                        //}

                        if (intCnt > 1)
                        {
                            for (int j = 2; j <= intCnt; j++)
                            {
                                string gridView = string.Empty;
                                if (j > 1 && j < 10)
                                {
                                    gridView = string.Format(gridViewFormat, "0" + j);
                                }
                                else
                                {
                                    gridView = string.Format(gridViewFormat, j);
                                }

                                if (webBrowser1.Document.GetElementById(gridView + "_syohincode").InnerText.Equals(entity.orderCode))
                                {
                                    qty = webBrowser1.Document.GetElementById(gridView + "_Label1").InnerText;
                                    //entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";
                                    //<remark 13/07/2020(変更)>
                                    //<remark 28/11/2019(変更)>
                                    entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") || qty.Equals("★") || qty.Equals("？") ?  "empty"  : "unknown status";//<remark ロジックの変更　2022/01/19 />
                                    //entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") || qty.Equals("×") || qty.Equals("☆") || qty.Equals("★") || qty.Equals("？") ? "empty" : "unknown status";
                                    //</remark>
                                    //</remark>
                                    entity.price = webBrowser1.Document.GetElementById(gridView + "_hanbaikakaku").InnerText;
                                    entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                                    node = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]");

                                    if (node != null)
                                        entity.stockDate = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[6]").InnerText;
                                    colorpath = hdoc.DocumentNode.SelectSingleNode("/tbody/tr[" + j + "]/td[5]");
                                    color = colorpath.GetAttributeValue("style", string.Empty);
                                    //<remark 2021/01/06>
                                    if (entity.stockDate == "&nbsp;")
                                    { entity.True_StockDate = string.Empty; }
                                    else
                                    { entity.True_StockDate = entity.stockDate; }
                                    entity.True_Quantity = qty;
                                    //</remark 2021/01/06>
                                    break;
                                }
                            }

                            if (string.IsNullOrEmpty(entity.qtyStatus) && string.IsNullOrEmpty(entity.stockDate))
                            {
                                entity.price = dt013.Rows[i]["下代"].ToString();
                                entity.qtyStatus = "empty";
                                entity.stockDate = "2100-02-01";
                                //<remark 2021/01/06>
                                entity.True_StockDate = "Not Found";
                                entity.True_Quantity = "Not Found";
                                //</remark 2021/01/06>
                            }
                        }


                        //string qty = webBrowser1.Document.GetElementById("GridView1_ctl02_Label1").InnerText;
                        //entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";

                        //entity.price = webBrowser1.Document.GetElementById("GridView1_ctl02_hanbaikakaku").InnerText;
                        //entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);

                        //HtmlNode node = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]");

                        //if (node != null)
                        //    entity.stockDate = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[6]").InnerText;
                        //HtmlNode colorpath = hdoc.DocumentNode.SelectSingleNode("div[3]/div[4]/table/tbody/tr[2]/td[5]");
                        //string color = colorpath.GetAttributeValue("style", string.Empty);


                        if (IsDate(entity.stockDate))
                            entity.stockDate = entity.stockDate.Replace("/", "-");

                        else if (color.Contains("red"))
                        {

                            //if (qty.Equals("▲") || qty.Equals("×"))
                            //    entity.stockDate = "2100-02-01";

                            //else if (qty.Equals("★") || qty.Equals("？"))
                            //    entity.stockDate = "2100-01-01";

                            //<remark 28/11/2019(変更)>
                            //if (qty.Equals("▲") || qty.Equals("×") || qty.Equals("★") || qty.Equals("？"))
                            //    entity.stockDate = "2100-02-01";
                            //</remark>

                            //<remark 19/01/2022(変更) Start>
                            if (qty.Equals("×") || qty.Equals("★") || qty.Equals("？"))
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            else if(qty.Equals("▲"))
                            {
                                entity.stockDate = "2100-01-01";
                            }
                            //</remark 19/01/2022 End>

                            //<remark 2021/01/06>                         
                            entity.True_Quantity = qty + "(red)";
                            //</remark 2021/01/06>
                        }
                        else
                        {

                            //entity.stockDate = qty.Equals("○") || qty.Equals("▲") || qty.Equals("×") ? "2100-01-01" : entity.stockDate;
                            //<remark 13/07/2020(変更)>
                            //<remark 06/12/2019(変更)>
                            entity.stockDate = qty.Equals("○") || qty.Equals("▲") ? "2100-01-01" : qty.Equals("×") ? "2100-02-01" : entity.stockDate.Replace("/", "-");//<remark ロジックの変更　2022/01/19 />
                            //entity.stockDate = qty.Equals("○") ? "2100-01-01" : qty.Equals("▲") || qty.Equals("×") ? "2100-02-01" : entity.stockDate.Replace("/", "-");
                            //</remark>
                            //</remark>
                        }

                        //entity.stockDate = color.Contains("red") ? "2100-02-01" : (stockDate == "-" || string.IsNullOrWhiteSpace(stockDate) || stockDate.Equals("&nbsp;")) ? "2100-01-01" : stockDate;

                        //<remark 12/13/2019更新　start>
                        //if (entity.stockDate.Contains("月中旬") || entity.stockDate.Contains("月上旬"))
                        //if (entity.stockDate.Contains("月上旬") || entity.stockDate.Contains("月中旬") || entity.stockDate.Contains("月下旬") || entity.stockDate.Contains("月予定"))//<remark Edit Logic for Stockdate 2021/07/22 />
                        if (entity.stockDate.Contains("月上旬") || entity.stockDate.Contains("月中旬") || entity.stockDate.Contains("月下旬") || entity.stockDate.Contains("月予定") || entity.stockDate.Contains("月初旬"))
                        //</remark 12/13/2019　end>
                        {
                            entity.stockDate = entity.stockDate.Replace("次回", "").Replace("入荷", "");
                            string day = string.Empty;

                            if (entity.stockDate.Contains("中旬"))
                                day = "20";


                            //<remark 12/13/2019更新　start>
                            //else if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("月予定"))
                            //else if (entity.stockDate.Contains("上旬"))//<remark Edit Logic for Stockdate 2021/07/22 />
                            else if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("初旬"))
                                //</remark 12/13/2019　end>
                                day = "10";


                            else if (entity.stockDate.Contains("下旬"))
                            {

                                if (entity.stockDate.Contains("2月"))
                                    day = "28";
                                day = "30";
                            }


                            else day = "25";

                            //<remark Add Logic for Stockdate of Month 2022/01/18 Start>
                            //string month = entity.stockDate.Split('月')[0];

                            //string year = DateTime.Now.ToString("yyyy");

                            string month;
                            string year;
                            if (entity.stockDate.Contains("年"))
                            {
                                int YIndex = entity.stockDate.IndexOf('年');
                                int MIndex = entity.stockDate.IndexOf('月');
                                year = entity.stockDate.Substring(YIndex - 4, YIndex + 0);
                                month = entity.stockDate.Substring(YIndex + 1, MIndex - 5);
                            }
                            else
                            {
                                month = entity.stockDate.Split('月')[0];
                                year = DateTime.Now.ToString("yyyy");
                            }
                            //</remark 2022/01/18 End>

                            DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                            if (dt < DateTime.Now)
                                dt = dt.AddYears(1);

                            entity.stockDate = dt.ToString("yyyy-MM-dd");

                        }
                        else if (entity.stockDate.Contains("月末～"))
                        {
                            //<remark 12/13/2019更新　start>
                            //entity.stockDate = "未定(=2100-01-01)";                          
                            entity.stockDate = "2100-01-01";
                            //</remark 12/13/2019　end>
                        }

                        //<remark 09/09/2020 移動　start>
                        else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                        {

                            int YIndex = entity.stockDate.IndexOf('年');
                            int MIndex = entity.stockDate.IndexOf('月');
                            int year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                            int month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                            //entity.stockDate = year + "-" + month + "-" + "15";
                            DateTime dt = new DateTime(year, month, 15);
                            entity.stockDate = String.Format("{0:yyyy-MM-dd}", dt);
                        }
                        //</remark 09/09/2020　end>

                        //else if (entity.stockDate.Contains("月末"))
                        else if (entity.stockDate.Contains("月末") || entity.stockDate.Contains("月"))//<remark Add Logic of Stockdate 2020/09/02 />
                        {
                            string day = "25";
                            //string month = entity.stockDate.Replace("月末", string.Empty).Replace("予定", string.Empty);
                            string month = entity.stockDate.Replace("月末", string.Empty).Replace("予定", string.Empty).Replace("月", string.Empty);//<remark Add Logic of Stockdate 2020/09/02 />

                            string year = DateTime.Now.ToString("yyyy");
                            DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                            if (dt < DateTime.Now)
                                dt = dt.AddYears(1);

                            entity.stockDate = dt.ToString("yyyy-MM-dd");
                        }

                        else if (entity.stockDate.Contains("未定"))
                        {
                            entity.stockDate = "2100-01-01";
                        }

                        //<remark 12/13/2019追加　start>

                        //else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                        //{

                        //    int YIndex = entity.stockDate.IndexOf('年');
                        //    int MIndex = entity.stockDate.IndexOf('月');
                        //    int year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                        //    int month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                        //    //entity.stockDate = year + "-" + month + "-" + "15";
                        //    DateTime dt = new DateTime(year, month, 15);
                        //    entity.stockDate = String.Format("{0:yyyy-MM-dd}", dt);
                        //}
                        //</remark 12/13/2019　end>

                        //<remark 13/07/2020(変更)>
                        //else if ((qty.Equals("☆")) && string.IsNullOrWhiteSpace(entity.stockDate)) { entity.stockDate = "2100-01-10"; }
                        else if ((qty.Equals("☆")) && string.IsNullOrWhiteSpace(entity.stockDate)) { entity.stockDate = "2100-02-01"; }
                        //</remark>

                        else if (entity.stockDate.Contains("在庫限り"))
                            entity.stockDate = "2100-02-01";
                        //2018-08-14 Start

                        else if (entity.stockDate.Contains("月以降"))
                        {
                            entity.stockDate = Regex.Replace(entity.stockDate, "[^0-9]", string.Empty);
                            entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(entity.stockDate), 30).ToString("yyyyMMdd");
                        }
                        //2018-08-14 End
                        entity.stockDate = entity.stockDate.Replace("/", "-");
                        //2018/1/12
                        //<remark Close Logic 2020/25/22 Start>
                        //if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") || dt013.Rows[i]["在庫情報"].ToString().Contains("inquriry")) && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        //{
                        //    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                        //    {
                        //        entity.qtyStatus = dt013.Rows[i]["在庫情報"].ToString();
                        //        entity.price = dt013.Rows[i]["下代"].ToString();       
                        //        entity.stockDate = dt013.Rows[i]["入荷予定"].ToString();
                        //    }
                        //    fun.Qbei_Inserts(entity);
                        //}

                        //else
                        //</reamark 2020/25/22 End>
                        //2018/1/12
                        fun.Qbei_Inserts(entity);
                    }
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                    fun.WriteLog(ex.Message + entity.orderCode, "013-");
                }
            }
            webBrowser1.Document.GetElementById("btnClear").InvokeMember("Click");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            //}
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Access Denied!", dt013.Rows[i]["JANコード"].ToString(), dt013.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
            fun.WriteLog(StatusCode.ToString() + " " + dt013.Rows[i]["発注コード"].ToString(), "013-");
            Application.Exit();
        }
    }
}
