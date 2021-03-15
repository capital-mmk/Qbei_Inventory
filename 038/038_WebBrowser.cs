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
using Microsoft.VisualBasic;

namespace _38フタバ
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm038 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt038 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = -1;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm038()
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
                qe.site = 38;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(38);
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
                    fun.deleteData(38);
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
                fun.WriteLog(ex, "038-");
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
                fun.setURL("038");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(38);
                fun.Qbei_ErrorDelete(38);
                dt038 = fun.GetDatatable("038");
                //dt038 = fun.GetOrderData(dt038, "https://www.ftb-weborder.com/Account/SyohinSearch.aspx", "038", "");//<remark Close Logic of Onceaweek 2020/10/15 />
                fun.GetTotalCount("038");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "038-");
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
            qe.SiteID = 38;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url + "/Default.aspx");

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

                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "038-");
                qe.SiteID = 38;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("tbx_tokuisakicode").InnerText = username;

                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("tbx_loginpasswd").InnerText = password;
                fun.GetElement("input", "btn_loginbutton", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt038.Rows[0]["JANコード"].ToString();
                string orderCode = dt038.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                fun.WriteLog(ex, "038-", janCode, orderCode);

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
                if (body.Contains("パスワードが必要です。") || body.Contains("得意先コードが必要です。"))
                {
                    fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                    fun.WriteLog("Login Failed", "038-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "038-");
                    webBrowser1.Navigate(fun.url + "/Account/SyohinSearch.aspx");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt038.Rows[0]["JANコード"].ToString();
                string orderCode = dt038.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                fun.WriteLog(ex, "038-", janCode, orderCode);

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

            webBrowser1.Navigate(fun.url + "/Account/SyohinSearch.aspx");
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }


        /// <summary>
        /// Inspection of item at Mall.
        /// </summary>
        private void webBrowser1_Search(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;
                if (i < dt038.Rows.Count - 1)
                {
                    doc.GetElementById("MainContent_tbx_freeword").SetAttribute("Value", dt038.Rows[++i]["発注コード"].ToString());
                    webBrowser1.Document.GetElementById("MainContent_btn_search").InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 38;
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
                string janCode = dt038.Rows[i]["JANコード"].ToString();
                string orderCode = dt038.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                fun.WriteLog(ex, "038-", janCode, orderCode);

                webBrowser1.Document.GetElementById("MainContent_btn_clear").InvokeMember("Click");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }


        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            //<remark ロジックの変更　2020/04/08 Start>
            string strStockDate = string.Empty;
            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 38;
            entity.sitecode = "038";
            entity.janCode = dt038.Rows[i]["JANコード"].ToString();
            entity.partNo = dt038.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt038.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt038.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/Account/SyohinSearch.aspx";
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            if (body.Contains("条件に該当する商品が見つかりませんでした"))
            {
                if (dt038.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt038.Rows[i]["在庫情報"].ToString().Contains("empty"))
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-01-10";
                    entity.price = dt038.Rows[i]["下代"].ToString();
                }
                else
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";
                    entity.price = dt038.Rows[i]["下代"].ToString();
                }
                //<remark 2021/01/06>
                entity.True_StockDate = "Not Found";
                entity.True_Quantity = "Not Found";
                //</remark 2021/01/06>
                fun.Qbei_Inserts(entity);
            }
            else
            {
                string html = webBrowser1.Document.Body.InnerHtml;

                try
                {
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    if ((webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_zaikojokyo_0") == null) && (webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0") == null))
                    {
                        fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "038-");

                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        strStockDate = string.Empty;
                        int Month;
                        string sMonth;
                        string Day;
                        DateTime dt;
                        int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                        string qty = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_zaikojokyo_0").InnerText;
                        //entity.qtyStatus = qty.Contains("○") || fun.IsGood(qty) ? "good" : qty.Equals("△") || fun.IsSmall(qty) ? "small" : qty.Equals("×") ? "empty" : "unknown status";
                        //entity.qtyStatus = qty.Equals("×") ||fun.IsEmpty_38(qty)? "empty" : qty.Equals("△") || fun.IsSmall_38(qty) ? "small" : qty.Contains("○") || fun.IsGood_38(qty) ? "good" :  "unknown status";//<remark Quantityの編集ロジック　2020/04/07 />
                        entity.qtyStatus = qty.Equals("△") || fun.IsSmall_38(qty) || qty.Equals("×") || fun.IsEmpty_38(qty) ? "empty" : qty.Contains("○") || fun.IsGood_38(qty) ? "good" : "unknown status";//<remark Quantityの編集ロジック　2020/07/24 />
                        entity.price = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_tankazeinuki_0").InnerText;
                        entity.price = entity.price.Replace(",", string.Empty);
                        //<remark Edit Logic for stockdate 2021/1/12 Start>
                        //entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                        //strStockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;

                        if (webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0").InnerText != null)
                        {
                            entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0").InnerText;
                            strStockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0").InnerText;
                        }
                        else
                        {
                            entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                            strStockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                        }
                        //</remark 2021/1/12 End>

                        //<remark 2021/01/06>
                        entity.True_StockDate = entity.stockDate;
                        entity.True_Quantity = qty;
                        //</remark 2021/01/06>

                        //if (string.IsNullOrWhiteSpace(entity.stockDate) || entity.stockDate.Contains("お取り寄せ品"))
                        //{
                        //    try
                        //    {
                        //        entity.stockDate = string.IsNullOrEmpty(entity.stockDate) ? "" : entity.stockDate;
                        //        // entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                        //        if (entity.stockDate.Contains("在庫限り"))
                        //        {
                        //            entity.stockDate = "2100-02-01";
                        //        }
                        //        else
                        //        {
                        //            entity.stockDate = "2100-01-01";
                        //        }
                        //    }
                        //    catch
                        //    {
                        //        entity.stockDate = "2100-01-01";
                        //    }
                        //}
                        //else if (entity.stockDate.Contains("在庫限り") || entity.stockDate.Contains("廃盤") || entity.stockDate.Contains("限定") || entity.stockDate.Contains("完全廃盤"))
                        //{
                        //    entity.stockDate = "2100-02-01";
                        //}
                        //else if (entity.stockDate.Contains("2月") || entity.stockDate.Contains("2"))
                        //{
                        //    // entity.stockDate = "2018-02-28";
                        //    entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                        //}
                        ////2018-05-15 Start
                        //else if (entity.stockDate.Contains("月"))
                        //{
                        //    //6-7月予定
                        //    if (entity.stockDate.Contains("-"))
                        //    {
                        //        entity.stockDate = entity.stockDate.Remove(0, entity.stockDate.IndexOf("-"));
                        //        entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty)), 30).ToString("yyyy-MM-dd");
                        //    }
                        //    else if (entity.stockDate.Contains("～"))
                        //    {
                        //        entity.stockDate = "2100-01-01";
                        //    }

                        //}
                        ////2018-05-15 End
                        //else if (entity.stockDate.Contains("/中旬頃予定"))
                        //{
                        //    entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty)), 20).ToString("yyyy-MM-dd");
                        //}
                        //else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定") || entity.stockDate.Contains("今夏頃発売予定") || entity.stockDate.Contains(":"))
                        ////2018-05-15 End
                        //{
                        //    entity.stockDate = "2100-01-01";
                        //}
                        //else if ((entity.stockDate.Contains("/")) || entity.stockDate.Contains("/予定"))
                        //{
                        //    string day = string.Empty;
                        //    string month = string.Empty;
                        //    string year = string.Empty;
                        //    //2018-05-15 Start

                        //    if (entity.stockDate.Contains("中～下") || entity.stockDate.Contains("下") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末"))
                        //    //2018-05-15 End
                        //    {
                        //        day = "30";
                        //        month = entity.stockDate.Split('/')[0];
                        //        year = DateTime.Now.ToString("yyyy");
                        //    }
                        //    else if (entity.stockDate.Contains("初旬頃") || entity.stockDate.Contains("上旬頃") || entity.stockDate.Contains("上旬") || entity.stockDate.Contains("初旬"))
                        //    {
                        //        day = "10";
                        //    }
                        //    else
                        //    {
                        //        day = entity.stockDate.Split('/')[1];
                        //        //2018-05-04 Start
                        //        //8頃予定
                        //        day = Regex.Replace(day, "[^0-9]+", string.Empty);
                        //        //2018-05-04 End
                        //    }
                        //    month = entity.stockDate.Split('/')[0];
                        //    year = DateTime.Now.ToString("yyyy");
                        //    DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                        //    if (dt < DateTime.Now)
                        //    {
                        //        dt = dt.AddYears(1);
                        //    }
                        //    entity.stockDate = dt.ToString("yyyy-MM-dd");
                        //}
                        //else if (entity.stockDate.Contains("ロット価格有り") && !string.IsNullOrWhiteSpace(entity.stockDate) || string.IsNullOrWhiteSpace(entity.stockDate))
                        //{
                        //    try
                        //    {
                        //        entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;

                        //        string day = entity.stockDate.Split('/')[1];
                        //        string month = entity.stockDate.Split('/')[0];
                        //        string year = DateTime.Now.ToString("yyyy");
                        //        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                        //        if (dt < DateTime.Now)
                        //        {
                        //            dt = dt.AddYears(1);
                        //        }
                        //        entity.stockDate = dt.ToString("yyyy-MM-dd");
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        entity.stockDate = "2100-01-01";
                        //    }
                        //}
                        ////2015-05-10 Start
                        ////else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("納期未定"))
                        ////2018-05-15 Start
                        ////else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定"))
                        ////else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定") || entity.stockDate.Contains("今夏頃発売予定"))
                        //////2018-05-15 End
                        ////{
                        ////    entity.stockDate = "2100-01-01";
                        ////}                       

                        if (string.IsNullOrWhiteSpace(entity.stockDate) || entity.stockDate.Contains("お取り寄せ品"))
                        {

                            try
                            {
                                entity.stockDate = string.IsNullOrEmpty(entity.stockDate) ? "" : entity.stockDate;
                                if (entity.stockDate.Contains("在庫限り"))
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                else
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                //<remark 2021/01/06>
                                entity.True_StockDate = "項目無し";
                                entity.True_Quantity = qty;
                                //</remark 2021/01/06>
                            }
                            catch
                            {
                                entity.stockDate = "2100-02-01";
                            }
                        }
                        else if (entity.stockDate.Contains("在庫限り") || entity.stockDate.Contains("廃盤") || entity.stockDate.Contains("限定") || entity.stockDate.Contains(("受付終了")))
                        {
                            entity.stockDate = "2100-02-01";
                        }
                        else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定") || entity.stockDate.Contains("今夏頃発売予定") || entity.stockDate.Contains(":") || entity.stockDate.Contains("欠品"))
                        {
                            //entity.stockDate = "2100-01-01";
                            entity.stockDate = "2100-02-01";//<remark Edit Logic of Stockdate 2020/08/04 />
                        }
                        else if (entity.stockDate.Contains("BO"))
                        {
                            string year = DateTime.Now.ToString("yyyy");
                            int pcMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                            int MIndex = entity.stockDate.IndexOf('月');
                            if (MIndex == 6)
                            {
                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex - 5));
                                sMonth = "0" + Month;
                            }
                            else if (MIndex == 5)
                            {
                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex - 3));
                                sMonth = entity.stockDate.Substring(MIndex - 2, MIndex - 3);
                            }
                            else if (MIndex == 4)
                            {
                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex - 3));
                                sMonth = "0" + Month;
                            }
                            else
                            {
                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex - 5));
                                sMonth = entity.stockDate.Substring(MIndex - 2, MIndex - 5);
                            }
                            if (Month < pcMonth)
                            { year = Convert.ToString(Convert.ToInt32(year) + 1); }
                            int Y = Convert.ToInt32(year);
                            Day = DateTime.DaysInMonth(Y, Month).ToString();
                            entity.stockDate = year + "-" + sMonth + "-" + Day;
                        }

                        else if (entity.stockDate.Contains("/") && (entity.stockDate.Contains("旬") || entity.stockDate.Contains("予定")))
                        {
                            int Year;
                            if (entity.stockDate.Contains("年"))
                            {
                                int YIndex = entity.stockDate.IndexOf('年');
                                Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                            }
                            else
                            {
                                string year = DateTime.Now.ToString("yyyy");
                                Year = Convert.ToInt32(year);
                            }
                            if (entity.stockDate.Contains("/"))
                            {
                                string[] m = entity.stockDate.Split('/');

                                //<remark Edit Logic for Stockdate 2020/11/04 Start>
                                if (entity.stockDate.Contains("月"))
                                {
                                    Year = Convert.ToInt32(m[0]);
                                    int J_Month = m[1].IndexOf("月");
                                    if (J_Month == 1 || J_Month == 2)
                                    {
                                        if (J_Month == 1)
                                        {
                                            Month = Convert.ToInt32(m[1].Substring(J_Month - 1, J_Month + 0));
                                            Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                        }
                                        else
                                        {
                                            Month = Convert.ToInt32(m[1].Substring(J_Month - 2, J_Month + 0));
                                            Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                        }

                                        if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                        {
                                            entity.stockDate = Year + "-" + Month + "-" + "10";
                                        }
                                        else if (strStockDate.Contains("中旬") || strStockDate.Contains("中"))
                                        {
                                            entity.stockDate = Year + "-" + Month + "-" + "20";
                                        }
                                        else if (strStockDate.Contains("下旬") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末") || strStockDate.Contains("下"))
                                        {
                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                        }

                                    }
                                }
                                //</remark 2020/11/04 End>

                                //<remark Edit Logic of Stockdate 2020/10/15 Start>         
                                else if (m.Count() == 3)
                                {
                                    Month = Convert.ToInt32(m[1]);
                                    Year = Convert.ToInt32(m[0]);
                                    Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                    if (m[2].Contains("初旬") || m[2].Contains("上旬") || m[2].Contains("上"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "10";
                                    }
                                    else if (m[2].Contains("中旬") || m[2].Contains("中"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "20";
                                    }
                                    else if (m[2].Contains("下旬") || m[2].Contains("末") || m[2].Contains("下"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + Day;
                                    }
                                }
                                else
                                {
                                    Month = Convert.ToInt32(m[0]);
                                    if (Month < pcmonth)
                                    { Year = Year + 1; }
                                    //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                    Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                    //<remak　追加ロジック 2020/04/20 Start>
                                    if (m[1].Contains("初旬") || m[1].Contains("上旬") || m[1].Contains("上"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "10";
                                    }
                                    else if (m[1].Contains("中旬") || m[1].Contains("中"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "20";
                                    }
                                    else if (m[1].Contains("下旬") || m[1].Contains("末") || m[1].Contains("下"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + Day;
                                    }
                                }
                                //Month = Convert.ToInt32(m[0]);
                                //if (Month < pcmonth)
                                //{ Year = Year + 1; }
                                ////Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                //Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                //entity.stockDate = Year + "-" + Month + "-" + Day;
                                ////<remak　追加ロジック 2020/04/20 Start>
                                //if (m[1].Contains("初旬") || m[1].Contains("上旬") || m[1].Contains("上"))
                                //{
                                //    entity.stockDate = Year + "-" + Month + "-" + "10";
                                //}
                                //else if (m[1].Contains("中旬")||m[1].Contains("中"))
                                //{
                                //    entity.stockDate = Year + "-" + Month + "-" + "20";
                                //}
                                //else if (m[1].Contains("下旬") || m[1].Contains("末")||m[1].Contains("下"))
                                //{
                                //    entity.stockDate = Year + "-" + Month + "-" + Day;
                                //}
                                //</remak 2020/04/20 End>
                                //</remark 2020/10/15 End>
                            }
                            else
                            {
                                int MIndex = entity.stockDate.IndexOf('月');
                                if (MIndex == 1 || MIndex == 2)
                                {
                                    if (MIndex == 1)
                                    {
                                        Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex + 0));
                                        if (Month < pcmonth)
                                        {
                                            Year = (Year + 1);
                                        }
                                        //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                        Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                        entity.stockDate = Year + "-" + Month + "-" + Day;
                                    }
                                    else
                                    {
                                        Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex + 0));
                                        if (Month < pcmonth)
                                        {
                                            Year = (Year + 1);
                                        }
                                        //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                        Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remark Edit Logic for Date of Year 2020/09/25 />
                                        entity.stockDate = Year + "-" + Month + "-" + Day;
                                    }

                                    if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "10";
                                    }
                                    else if (strStockDate.Contains("中旬") || strStockDate.Contains("中"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + "20";
                                    }
                                    else if (strStockDate.Contains("下旬") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末") || strStockDate.Contains("下"))
                                    {
                                        entity.stockDate = Year + "-" + Month + "-" + Day;
                                    }

                                }
                            }
                        }
                        else if (entity.stockDate.Contains("/"))
                        {
                            string year = DateTime.Now.ToString("yyyy");
                            string compare = DateTime.Now.ToString("yyyy");
                            int pcMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                            //<remark 追加ロジック　2020/04/29 Start>
                            if (entity.stockDate.Contains("/") && entity.stockDate.Contains("~"))
                            {
                                var m = entity.stockDate.Split('~').ToArray();
                                m = m[1].Split('/').ToArray();
                                Month = Convert.ToInt32(m[0]);
                                if (Month < pcMonth)
                                { year = Convert.ToString((Convert.ToInt32(year)) + 1); }
                                Day = DateTime.DaysInMonth(Convert.ToInt32(year), Month).ToString();
                                if (m[1].Contains("初旬") || m[1].Contains("上旬") || m[1].Contains("上"))
                                {
                                    entity.stockDate = year + "-" + Month + "-" + "10";
                                }
                                else if (m[1].Contains("中旬") || m[1].Contains("中"))
                                {
                                    entity.stockDate = year + "-" + Month + "-" + "20";
                                }
                                else if (m[1].Contains("下旬") || m[1].Contains("末頃") || m[1].Contains("末") || m[1].Contains("下"))
                                {
                                    entity.stockDate = year + "-" + Month + "-" + Day;
                                }
                            }
                            else
                            //</remark 2020/04/29 End>
                            {
                                int a = entity.stockDate.ToString().Count();
                                var b = entity.stockDate.Split('/').ToArray();
                                if (a > 7)
                                {
                                    int array = b.Length;
                                    if (array == 3)
                                    {
                                        year = b[0];
                                    }
                                    Month = Convert.ToInt32(b[1]);
                                    if (Month < pcMonth)
                                    { year = Convert.ToString(Convert.ToInt32(b[0]) + 1); }
                                    if (Convert.ToInt32(year) < Convert.ToInt32(compare))
                                    {
                                        year = Convert.ToString(Convert.ToInt32(b[0]) + 1);
                                    }
                                    entity.stockDate = year + "-" + Month + "-" + b[2];
                                }
                                else
                                {
                                    int array = b.Length;
                                    Month = Convert.ToInt32(b[0]);
                                    if (Month < pcMonth)
                                    { year = Convert.ToString((Convert.ToInt32(year)) + 1); }
                                    Day = b[1];
                                    //<remark Add Logic for Stockdate 2021/03/15 Start>
                                    if (Day.Contains("の週"))
                                    {
                                        int DayIndex = Day.IndexOf('の');
                                        if (DayIndex == 2)
                                        {
                                            Day = Day.Substring(DayIndex - 2, DayIndex + 0);
                                            Day= Convert.ToString((Convert.ToInt32(Day)) + 4);
                                        }
                                        else if(DayIndex == 1)
                                        {
                                            Day = Day.Substring(DayIndex - 1, DayIndex + 0);
                                            Day = Convert.ToString((Convert.ToInt32(Day)) + 4);
                                        }
                                    }
                                    //</remark 2021/03/15 End>
                                    entity.stockDate = year + "-" + Month + "-" + Day;
                                }
                            }
                        }

                        else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                        {
                            int YIndex = entity.stockDate.IndexOf('年');
                            int MIndex = entity.stockDate.IndexOf('月');
                            int Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                            Month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                            if ((Month < pcmonth) && (Year <= DateTime.Now.Year))
                            { Year = Year + 1; }
                            int Y = Convert.ToInt32(Year);
                            Day = DateTime.DaysInMonth(Y, Month).ToString();
                            if (entity.stockDate.Contains("日"))
                            {
                                entity.stockDate = entity.stockDate.Replace("年", "-").Replace("月", "-").Replace("日", "-");
                            }
                            else if (entity.stockDate.Contains("初旬") || entity.stockDate.Contains("上旬") || entity.stockDate.Contains("上"))
                            {
                                entity.stockDate = Year + "-" + Month + "-" + "10";
                            }
                            else if (entity.stockDate.Contains("中旬") || entity.stockDate.Contains("中"))
                            {
                                entity.stockDate = Year + "-" + Month + "-" + "20";
                            }
                            else if (entity.stockDate.Contains("下旬") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末") || entity.stockDate.Contains("下"))
                            {
                                entity.stockDate = Year + "-" + Month + "-" + Day;
                            }
                            else if (entity.stockDate.Contains("月") && (entity.stockDate.Contains("ごろ") || entity.stockDate.Contains("予定")))
                            {
                                entity.stockDate = Year + "-" + Month + "-" + Day;
                            }
                            else
                            {
                                entity.stockDate = Year + "-" + Month + "-" + Day;
                            }
                        }

                        else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("頃"))
                        {
                            int YIndex = entity.stockDate.IndexOf('年');
                            int Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                            int pcMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                            if (entity.stockDate.Contains("月"))
                            {
                                int MIndex = entity.stockDate.IndexOf('月');
                                Month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                                if (Month < pcmonth)
                                { Year = Year + 1; }
                                int Y = Convert.ToInt32(Year);
                                Day = DateTime.DaysInMonth(Y, Month).ToString();
                                entity.stockDate = Year + "-" + Month + "-" + Day;
                            }
                            else
                            {
                                if (entity.stockDate.Contains("春"))
                                {
                                    Month = 4;
                                    if (Month < pcMonth)
                                    { Year = Year + 1; }
                                    int Y = Convert.ToInt32(Year);
                                    Day = DateTime.DaysInMonth(Y, Month).ToString();
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                                else if (entity.stockDate.Contains("夏"))
                                {
                                    Month = 7;
                                    if (Month < pcMonth)
                                    { Year = Year + 1; }
                                    int Y = Convert.ToInt32(Year);
                                    Day = DateTime.DaysInMonth(Y, Month).ToString();
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                                else if (entity.stockDate.Contains("秋"))
                                {
                                    Month = 10;
                                    if (Month < pcMonth)
                                    { Year = Year + 1; }
                                    int Y = Convert.ToInt32(Year);
                                    Day = DateTime.DaysInMonth(Y, Month).ToString();
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                                else if (entity.stockDate.Contains("冬"))
                                {
                                    Month = 1;
                                    if (Month < pcMonth)
                                    { Year = Year + 1; }
                                    int Y = Convert.ToInt32(Year);
                                    Day = DateTime.DaysInMonth(Y, Month).ToString();
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                            }
                        }
                        //else if (entity.stockDate.Contains("~") && entity.stockDate.Contains("月"))//<remark Add Logic for stockdate 2021/02/24 />
                        else if ((entity.stockDate.Contains("~") && entity.stockDate.Contains("月")) || (entity.stockDate.Contains("～") && entity.stockDate.Contains("月")))
                        {
                            int mIndex = entity.stockDate.IndexOf("月");
                            string year = DateTime.Now.ToString("yyyy");
                            int month2;


                            if (entity.stockDate.Contains("月") || entity.stockDate.Contains("予定"))
                            {
                                //if (entity.stockDate.Contains("~"))//<remark Add Logic for Stockdate 2021/02/24 />
                                if (entity.stockDate.Contains("~") || entity.stockDate.Contains("～"))
                                {
                                    //int bIndex = entity.stockDate.IndexOf('~');
                                    //string charater = entity.stockDate.Substring(bIndex + 1, mIndex - 2);
                                    //if (charater.Contains("月"))
                                    //{
                                    //    month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 3));
                                    //}
                                    //else if (mIndex < 5)
                                    //{
                                    //    month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 2));
                                    //}
                                    //else
                                    //{
                                    //    month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 3));
                                    //}

                                    //<remak　追加ロジック と　変更ロジック 2020/04/20 Start>
                                    //var D = entity.stockDate.Split('~').ToArray();
                                    //<remark Add Logic for Stockdate 2021/02/24 Start>                                
                                    var D = entity.stockDate.Contains("~") ? entity.stockDate.Split('~').ToArray() : entity.stockDate.Split('～').ToArray();
                                    //</remark 2021/02/24 End>
                                    if (D[1].Contains("月"))
                                    {
                                        var M = D[1].ToString().Split('月').ToArray();
                                        month2 = Convert.ToInt32(M[0]);
                                        if (month2 < pcmonth)
                                        {
                                            year = Convert.ToString(Convert.ToInt32(year) + 1);
                                        }
                                    }
                                    else
                                    {
                                        var M = D[0].ToString().Split('月').ToArray();
                                        month2 = Convert.ToInt32(M[0]);
                                        if (month2 < pcmonth)
                                        {
                                            year = Convert.ToString(Convert.ToInt32(year) + 1);
                                        }
                                    }
                                    //Day = DateTime.DaysInMonth(DateTime.Now.Year, month2).ToString();
                                    Day = DateTime.DaysInMonth(Convert.ToInt32(year), month2).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />                                    
                                    entity.stockDate = year + "-" + month2 + "-" + Day;

                                    if (D[1].Contains("初旬") || D[1].Contains("上旬") || D[1].Contains("上"))
                                    {
                                        entity.stockDate = year + "-" + month2 + "-" + "10";
                                    }
                                    else if (D[1].Contains("中旬") || D[1].Contains("中"))
                                    {
                                        entity.stockDate = year + "-" + month2 + "-" + "20";
                                    }
                                    else if (D[1].Contains("下旬") || D[1].Contains("末頃") || D[1].Contains("末") || D[1].Contains("下"))
                                    {
                                        entity.stockDate = year + "-" + month2 + "-" + Day;
                                    }
                                    //</remak 2020/04/20 End>
                                }
                            }
                        }
                        else if (entity.stockDate.Contains('月'))
                        {
                            int MIndex = entity.stockDate.IndexOf("月");
                            string Year = DateTime.Now.ToString("yyyy");
                            if (MIndex == 1 || MIndex == 2)
                            {
                                if (MIndex == 1)
                                {
                                    Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex + 0));
                                    if (Month < pcmonth)
                                    {
                                        Year = Convert.ToString(Convert.ToInt32(Year) + 1);
                                    }
                                    //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                    Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />                                   
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                                else
                                {
                                    Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex + 0));
                                    if (Month < pcmonth)
                                    {
                                        Year = Convert.ToString(Convert.ToInt32(Year) + 1);
                                    }
                                    //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                    Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />        
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                                if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + "10";
                                }
                                else if (strStockDate.Contains("中旬") || strStockDate.Contains("中"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + "20";
                                }
                                else if (strStockDate.Contains("下旬") || strStockDate.Contains("末頃") || strStockDate.Contains("末") || strStockDate.Contains("下"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                            }
                            //<remark Edit Logic of Stockdate 2020/10/15 Start>
                            else if (MIndex == 5)
                            {
                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex - 3));
                                if (Month < pcmonth)
                                {
                                    Year = Convert.ToString(Convert.ToInt32(Year) + 1);
                                }
                                //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />        
                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + "10";
                                }
                                else if (strStockDate.Contains("中旬") || strStockDate.Contains("中"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + "20";
                                }
                                else if (strStockDate.Contains("下旬") || strStockDate.Contains("末頃") || strStockDate.Contains("末") || strStockDate.Contains("下"))
                                {
                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                }
                            }
                            //<remark 2020/10/15 End>                                  
                        }
                        else if (entity.stockDate.Contains("ロット価格有り") && !string.IsNullOrWhiteSpace(entity.stockDate) || string.IsNullOrWhiteSpace(entity.stockDate))
                        {
                            try
                            {
                                entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;

                                string day = entity.stockDate.Split('/')[1];
                                string month = entity.stockDate.Split('/')[0];
                                string year = DateTime.Now.ToString("yyyy");
                                dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                if (dt < DateTime.Now)
                                {
                                    dt = dt.AddYears(1);
                                }
                                entity.stockDate = dt.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                                entity.stockDate = "2100-02-01";
                            }
                        }
                        else
                        {
                            entity.stockDate = "2100-02-01";
                        }
                        //if ((dt038.Rows[i]["在庫情報"].ToString().Contains("empty") || dt038.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt038.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        //{
                        //    if (((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry")) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                        //    {
                        //        entity.qtyStatus = dt038.Rows[i]["在庫情報"].ToString();
                        //        entity.stockDate = dt038.Rows[i]["入荷予定"].ToString();
                        //        entity.price = dt038.Rows[i]["下代"].ToString();
                        //    }
                        //}    
                        dt = Convert.ToDateTime(entity.stockDate);
                        if (dt <= (DateTime.Now))
                        {
                            dt = dt.AddYears(1);
                        }
                        entity.stockDate = dt.ToString("yyyy-MM-dd");
                        fun.Qbei_Inserts(entity);
                    }
                    //</remark 2020/04/08 End>
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                    fun.WriteLog(ex, "038-", entity.janCode, entity.orderCode);
                }
            }

            webBrowser1.Document.GetElementById("MainContent_btn_clear").InvokeMember("Click");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt038.Rows[i]["JANコード"].ToString();
            string orderCode = dt038.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "038-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
