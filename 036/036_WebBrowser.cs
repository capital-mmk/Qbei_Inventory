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
using QbeiAgencies_Common;
using QbeiAgencies_BL;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic;

namespace _36PRインターナショナル
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm036 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt036 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm036()
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
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 36;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(36);
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
                    fun.deleteData(36);
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
                fun.WriteLog(ex, "036-");
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
                fun.setURL("036");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(36);
                fun.Qbei_ErrorDelete(36);
                dt036 = fun.GetDatatable("036");
                dt036 = fun.GetOrderData(dt036, "http://www.g-style.ne.jp/shop", "036", "");
                fun.GetTotalCount("036");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "036-");
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
            webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 36;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            webBrowser1.Navigate(fun.url);
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
                
                fun.WriteLog("Navigation to Site Url success------", "036-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 36;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "mypage_login_email", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "mypage_login_pass", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "ログイン", "value", webBrowser1).InvokeMember("click");

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt036.Rows[0]["JANコード"].ToString();
                string orderCode = dt036.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");                
                fun.WriteLog(ex, "036-", janCode, orderCode);

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
                if (body.Contains("メールアドレスもしくはパスワードが正しくありません。"))
                {
                    fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), "Login Failed", entity.janCode, entity.purchaseURL, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");                    
                    fun.WriteLog("Login Failed", "036-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "036-");
                    string purchaserURL = dt036.Rows[0]["purchaserURL"].ToString().Trim(); 
                    webBrowser1.Navigate(purchaserURL);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt036.Rows[0]["JANコード"].ToString();
                string orderCode = dt036.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");                
                fun.WriteLog(ex, "036-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                string strStockDate = string.Empty;

                fun.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                webBrowser1.ScriptErrorsSuppressed = true;
                entity = new Qbei_Entity();
                entity.siteID = 36;
                entity.sitecode = "036";

                entity.janCode = dt036.Rows[i]["JANコード"].ToString();
                entity.partNo = dt036.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt036.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt036.Rows[i]["発注コード"].ToString();  
                entity.purchaseURL = dt036.Rows[i]["purchaserURL"].ToString().Trim();     

                if (!string.IsNullOrWhiteSpace(entity.purchaseURL))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
                    if (body.Contains("ご指定のページはございません。"))
                    {
                        if (dt036.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt036.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt036.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt036.Rows[i]["下代"].ToString();
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
                            if ((hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[1]/td[1]") == null))
                            {
                                fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), "Access Denied!", entity.janCode, entity.purchaseURL, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");                                
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "036-");
                                
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                //2018-04-23 Start
                                if (!webBrowser1.Url.ToString().Equals(entity.purchaseURL))
                                {
                                    webBrowser1.Navigate(entity.purchaseURL);
                                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                                    return;
                                }
                                //2018-04-23 End

                                //<remark 2020/1/21 　StockDateのRowチャック　Start>
                                //strStockDate = string.Empty;
                                //entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[1]/td[1]").InnerText;
                                //entity.price = entity.price.Replace("円", string.Empty).Replace(",", string.Empty);
                                //string qtypath = hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[2]/td[1]").InnerText;
                                //entity.qtyStatus = qtypath.Equals("○") ? "good" : qtypath.Equals("△") ? "small" : qtypath.Equals("×") || qtypath.Equals("完売") ? "empty" : "unknown status";
                                //entity.price = string.Empty ;
                                //string stockpath = "div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[2]/td[2]";

                                strStockDate = string.Empty;
                                string qtypath;
                                string stockpath;
                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[1]/td[1]").InnerText;
                                if (entity.price.Contains("円"))
                                {
                                    entity.price = entity.price.Replace("円", string.Empty).Replace(",", string.Empty);
                                    qtypath = hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[2]/td[1]").InnerText;
                                    stockpath = "div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[2]/td[2]";
                                }
                                else
                                {                                 
                                    entity.price = dt036.Rows[i]["下代"].ToString();
                                    qtypath = hdoc.DocumentNode.SelectSingleNode("div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr[1]/td[1]").InnerText;
                                    stockpath = "div[3]/div/div/div[2]/div/div[2]/div/div/div[2]/table/tbody/tr/td[2]";
                                }
                                entity.qtyStatus = qtypath.Equals("○") ? "good" : qtypath.Equals("△") ? "small" : qtypath.Equals("×") || qtypath.Equals("完売") ? "empty" : "unknown status";
                                //</remark 2020/1/21 　End>

                                HtmlNodeCollection nc = hdoc.DocumentNode.SelectNodes(stockpath);

                                if (nc == null)
                                {
                                    entity.stockDate = qtypath.Equals("○") || qtypath.Equals("△") || qtypath.Equals("×") ? "2100-01-01" : qtypath.Equals("限定") ? "2100/02/01" : "unknown date";
                                }                                
                                else
                                {
                                    entity.stockDate = hdoc.DocumentNode.SelectSingleNode(stockpath).InnerText;
                                    strStockDate = entity.stockDate;

                                    //<remark 2020/1/17 年月日のチャック　Start>
                                    //if (entity.stockDate.Contains("未定") || entity.stockDate.Contains("時期未定") || entity.stockDate.Contains("18年春予定") || entity.stockDate.Contains("今季終了品") || entity.stockDate.Contains("2018年以降予定") || entity.stockDate.Contains("今季販売終了品"))
                                    //{
                                    //    entity.stockDate = "2100-01-01";
                                    //}
                                    //else if (entity.stockDate.Contains("取り寄せ商品") || entity.stockDate.Contains("取り寄せ品"))
                                    //{
                                    //    entity.stockDate = "2100-01-01";
                                    //    entity.qtyStatus = "inquiry";
                                    //}
                                    //else if (entity.stockDate.Contains("販売終了品"))
                                    //{
                                    //    entity.stockDate = "2100-02-01";
                                    //}
                                    //else if (entity.stockDate.Contains("月頃") || entity.stockDate.Contains("頃"))
                                    //{
                                    //    string year = DateTime.Now.ToString("yyyy");
                                    //    if (entity.stockDate.Contains("/"))
                                    //    {
                                    //        string[] m = entity.stockDate.Split('/');
                                    //        string month = m[0].ToString();
                                    //        string day = "30";
                                    //        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                    //        entity.stockDate = dt.ToString("yyyy-MM-dd");
                                    //    }
                                    //    else
                                    //    {
                                    //        if (entity.stockDate.Contains("初旬") || entity.stockDate.Contains("上旬"))
                                    //        {
                                    //            entity.stockDate = year + "-" + entity.stockDate.Replace("月", "-").Replace("初旬頃入荷予定", "10").Replace("初旬頃予定", "10").Replace("上旬頃入荷予定", "10").Replace("上旬頃予定", "10");
                                    //        }
                                    //        else if (entity.stockDate.Contains("中旬"))
                                    //        {
                                    //            entity.stockDate = year + "-" + entity.stockDate.Replace("月", "-").Replace("中旬頃入荷予定", "20").Replace("中旬頃予定", "20");
                                    //        }
                                    //        else
                                    //        {
                                    //            entity.stockDate = year + "-" + entity.stockDate.Replace("月", "-").Replace("頃入荷予定", "30").Replace("頃入金予定", "30").Replace("末", "");
                                    //            if (entity.stockDate.Contains('～') || entity.stockDate.Contains('~'))
                                    //            {
                                    //                if (entity.stockDate.Contains('～'))
                                    //                {
                                    //                    string[] arr = entity.stockDate.Split('～');
                                    //                    entity.stockDate = year + "-" + arr[1].Replace("月", "-").Replace("頃入荷予定", "30");
                                    //                }
                                    //                else if (entity.stockDate.Contains('~'))
                                    //                {
                                    //                    string[] arr = entity.stockDate.Split('~');
                                    //                    entity.stockDate = year + "-" + arr[1].Replace("月", "-").Replace("頃入金予定", "30");
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                                    //{
                                    //    entity.stockDate = entity.stockDate.Replace("年", "-") + entity.stockDate.Replace("月", "-").Replace("頃入荷予定", "30");
                                    //}
                                    
                                    
                                    if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                                    {
                                        int YIndex = entity.stockDate.IndexOf('年');
                                        int MIndex = entity.stockDate.IndexOf('月');
                                        int Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                                        int Month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                                        string Day= DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                        if (entity.stockDate.Contains("日"))
                                        {
                                            entity.stockDate = entity.stockDate.Replace("年", "-").Replace("月", "-").Replace("日", "-");
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
                                    else if (entity.stockDate.Contains("月") || entity.stockDate.Contains("頃"))
                                    {
                                        string year = DateTime.Now.ToString("yyyy");
                                        int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                                        int month;
                                        string day;
                                        //<remark "ごろ"の追加　2020/02/25 Start>
                                        //if (entity.stockDate.Contains("月頃") || entity.stockDate.Contains("頃"))
                                        //<remark "末" と　"旬"の追加　2020/03/20 Start>
                                        //if (entity.stockDate.Contains("月頃") || entity.stockDate.Contains("頃") || entity.stockDate.Contains("ごろ"))
                                        //</remark 2020/02/25 End>
                                        if (entity.stockDate.Contains("月頃") || entity.stockDate.Contains("頃") || entity.stockDate.Contains("ごろ") || entity.stockDate.Contains("末") || entity.stockDate.Contains("旬"))
                                        //</remark 2020/03/20  End>
                                        {
                                            if (entity.stockDate.Contains("/"))
                                            {
                                                string[] m = entity.stockDate.Split('/');
                                                int Month =Convert.ToInt32 (m[0]);
                                                if (Month < pcmonth)
                                                { year = Convert.ToString(Convert.ToInt32(year) + 1); }                                               
                                                 day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString(); 
                                                DateTime dt = Convert.ToDateTime(year + "-" + Month + "-" + day);
                                                entity.stockDate = dt.ToString("yyyy-MM-dd");
                                            }
                                            else
                                            {
                                                int MIndex = entity.stockDate.IndexOf('月');
                                                
                                                if (MIndex == 1 || MIndex == 2)
                                                {
                                                    if (MIndex == 1)
                                                    {
                                                         month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex + 0));
                                                        if (month < pcmonth)
                                                        { year = Convert.ToString(Convert.ToInt32(year) + 1); }
                                                        day = DateTime.DaysInMonth(DateTime.Now.Year, month).ToString();
                                                        entity.stockDate = year + "-" + month + "-" + day;
                                                    }
                                                    else
                                                    {
                                                        month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex + 0));
                                                        if (month < pcmonth)
                                                        { year = Convert.ToString(Convert.ToInt32(year) + 1); }
                                                        day = DateTime.DaysInMonth(DateTime.Now.Year, month).ToString();
                                                        entity.stockDate = year + "-" + month + "-" + day;                                                     
                                                    }
  
                                                    if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬"))
                                                    {   
                                                        entity.stockDate = year + "-" + month + "-" + "10";
                                                    }
                                                    else if (strStockDate.Contains("中旬"))
                                                    {
                                                        entity.stockDate = year + "-" + month + "-" + "20";
                                                    }
                                                    else if(strStockDate.Contains("下旬"))
                                                    {
                                                        entity.stockDate = year + "-" + month + "-" + day;                                                
                                                    }

                                                }
                                                else if (entity.stockDate.Contains('～') || entity.stockDate.Contains('~'))
                                                {
                                                    int mIndex = entity.stockDate.IndexOf('月');
                                                    int month2;
                                                    string day2;
                                                    if (entity.stockDate.Contains("月") || entity.stockDate.Contains("予定"))
                                                    {
                                                        if (entity.stockDate.Contains('～'))
                                                        {
                                                            int bIndex = entity.stockDate.IndexOf('～');
                                                            string charater = entity.stockDate.Substring(bIndex + 1, mIndex - 2);
                                                            if (charater.Contains("月"))
                                                            { month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 3)); }
                                                            else if (mIndex < 5)
                                                            { month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 2)); }
                                                            else
                                                            { month2 = Convert.ToInt32(entity.stockDate.Substring(bIndex + 1, mIndex - 3)); }
                                                        }
                                                        else
                                                        {
                                                            int sIndex = entity.stockDate.IndexOf('~');
                                                            string charater = entity.stockDate.Substring(sIndex + 1, mIndex - 2);
                                                            if (charater.Contains("月"))
                                                            { month2 = Convert.ToInt32(entity.stockDate.Substring(sIndex + 1, mIndex - 3)); }
                                                            else if (mIndex < 5)
                                                             {
                                                                
                                                                 month2 = Convert.ToInt32(entity.stockDate.Substring(sIndex + 1, mIndex - 2)); 
                                                            }
                                                            else
                                                            { month2 = Convert.ToInt32(entity.stockDate.Substring(sIndex + 1, mIndex - 3)); }
                                                        }
                                                        if (month2 < pcmonth)
                                                        { year = Convert.ToString(Convert.ToInt32(year) + 1); }
                                                        day2 = DateTime.DaysInMonth(DateTime.Now.Year, month2).ToString();
                                                        entity.stockDate = year + "-" + month2 + "-" + day2;

                                                    }
                                                    
                                                }
                                            }
                                        }
                                    }
                                    else if (entity.stockDate.Contains("未定") || entity.stockDate.Contains("予定") || entity.stockDate.Contains("今季") && entity.stockDate.Contains("終了品"))
                                    {
                                        entity.stockDate = "2100-01-01";
                                    }
                                    else if (entity.stockDate.Contains("取り寄せ"))
                                    {
                                        //<remark 変更　2020/02/04　Start>
                                        //entity.stockDate = "2100-01-01";
                                        //entity.qtyStatus = "inquiry";
                                        entity.stockDate = "2100-02-01";
                                        entity.qtyStatus = "empty";
                                        //</remark 2020/02/04　End>
                                    }
                                    //<remark stockDateのロジックを編集　2020/03/23　Start>
                                    // else if (entity.stockDate.Contains("販売終了品"))
                                    else if (entity.stockDate.Contains("販売終了"))
                                    //</remark 2020/03/23　End>
                                    {
                                        entity.stockDate = "2100-02-01";
                                    }
                                    //</remark 2020/1/17 End>

                                    else if (entity.stockDate.Contains("入荷予定"))
                                    {
                                        entity.stockDate = "2100-01-01";
                                    }               
                                    //2018-04-20 Start
                                    if (strStockDate.Equals("2月"))
                                    {
                                        entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                                    }
                                    else
                                        entity.stockDate = Strings.StrConv(entity.stockDate, VbStrConv.Narrow, 1041);
                                        entity.stockDate = DateTime.Parse(entity.stockDate).ToString("yyyy-MM-dd");
                                    //2018-04-20 End
                                }
                                //2018-04-20 Start
                                //fun.Qbei_Inserts(entity);
                                if (dt036.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                {
                                    if (((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry")) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                                    {
                                        entity.qtyStatus = dt036.Rows[i]["在庫情報"].ToString();
                                        entity.stockDate = dt036.Rows[i]["入荷予定"].ToString();
                                        entity.price = dt036.Rows[i]["下代"].ToString();
                                    }
                                    fun.Qbei_Inserts(entity);
                                }
                                else
                                    fun.Qbei_Inserts(entity);
                                //2018-04-20 End
                            }
                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), ex.Message, entity.janCode, entity.purchaseURL, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");                            
                            fun.WriteLog(ex, "036-", entity.janCode, entity.orderCode);
                        }
                    }
                }
                else
                {
                    fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), "Jan Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");
                }
                if (i < dt036.Rows.Count - 1)
                {
                    string purchaseURL = dt036.Rows[++i]["purchaserURL"].ToString().Trim();
                    webBrowser1.Navigate(purchaseURL);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 36;
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
                fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), ex.Message, entity.janCode, entity.purchaseURL, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");
                fun.WriteLog(ex, "036-", entity.janCode, entity.orderCode);
                
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
            if (StatusCode.ToString().Contains("500"))
            {
                entity = new Qbei_Entity();
                entity.siteID = 36;
                entity.sitecode = "036";
                entity.janCode = dt036.Rows[i]["JANコード"].ToString();
                entity.partNo = dt036.Rows[i]["自社品番"].ToString();
                entity.makerDate = fun.getCurrentDate();
                entity.reflectDate = dt036.Rows[i]["最終反映日"].ToString();
                entity.orderCode = dt036.Rows[i]["発注コード"].ToString();          
                entity.purchaseURL = dt036.Rows[i]["purchaserURL"].ToString().Trim();
                if (dt036.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt036.Rows[i]["在庫情報"].ToString().Contains("empty"))
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-01-10";
                    entity.price = dt036.Rows[i]["下代"].ToString();
                }
                else
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";
                    entity.price = dt036.Rows[i]["下代"].ToString();
                }
                fun.Qbei_Inserts(entity);
                if (i < dt036.Rows.Count - 1)
                {
                    string purchaseURL = dt036.Rows[++i]["purchaserURL"].ToString().Trim();
                    webBrowser1.Navigate(purchaseURL);
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
                else
                {
                    qe.site = 36;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            else
            {
                string janCode = dt036.Rows[i]["JANコード"].ToString();
               string orderCode = dt036.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(36, fun.GetSiteName("036"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "036");
                fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "036-");
                
                Application.Exit();
                Environment.Exit(0);                
            }
        }
    }
}

