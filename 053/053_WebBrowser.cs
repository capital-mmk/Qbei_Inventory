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

namespace _053
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm053 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt053 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm053()
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
                qe.site = 53;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(53);
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
                    fun.deleteData(53);
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
                fun.WriteLog(ex, "053-");
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
                fun.setURL("053");
                fun.WriteLog("Set URL success ------", "053-");
                fun.CreateFileAndFolder();
                fun.WriteLog("File Create success ------", "053-");
                fun.Qbei_Delete(53);
                fun.WriteLog("Qbei_Delet OK ------", "053-");
                fun.Qbei_ErrorDelete(53);
                fun.WriteLog("Qbei_ErrorDelet OK ------", "053-");
                dt053 = fun.GetDatatable("053");
                dt053 = fun.GetOrderData(dt053, "http://www2.gear-m.co.jp/moss/item_code/", "053", "");
                fun.GetTotalCount("053");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "053-");
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
            qe.SiteID = 53;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
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

                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "053-");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                qe.SiteID = 53;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "user", "name", webBrowser1).InnerText = username;
                string password = dt.Rows[0]["Password"].ToString();
                fun.GetElement("input", "pass", "name", webBrowser1).InnerText = password;
                fun.GetElement("input", "login", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt053.Rows[0]["JANコード"].ToString();
                string orderCode = dt053.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");                
                fun.WriteLog(ex, "053-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                webBrowser1.ScriptErrorsSuppressed = true;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("お客様番号を入力してください") || body.Contains("パスワードを入力してください") || body.Contains("もう一度、ログインしてください"))
                {
                    fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
                    fun.WriteLog("Login Failed", "053-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "053-");
                    orderCode = dt053.Rows[i]["発注コード"].ToString();
                    
                    if (orderCode.Contains('│') || orderCode.Contains('|'))
                    {
                        string[] arr = orderCode.Split(new char[] { '│', '|' }, StringSplitOptions.RemoveEmptyEntries);
                        entity.orderCode = string.IsNullOrEmpty(arr[0]) ? Regex.Replace(arr[1], "[/]+", string.Empty) : Regex.Replace(arr[0], "[/]+", string.Empty);
                        orderCode = entity.orderCode;
                    }
                    if (dt053.Rows[i]["ブランドコード"].ToString().Contains("00349"))
                        webBrowser1.Navigate(fun.url + "/SearchSv?tono=" + orderCode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                    //else if (dt053.Rows[i]["ブランドコード"].ToString().Contains("00013"))
                    //    webBrowser1.Navigate(fun.url + "/SearchSv?tono=&seihin=" + ordercode + "&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt053.Rows[i]["JANコード"].ToString();
                fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");                
                fun.WriteLog(ex, "053-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {            
            try
            {
                if (webBrowser1.Url.ToString().Contains("/SearchSv?tono="))
                {
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                    if (dt053.Rows[i]["ブランドコード"].ToString().Contains("00013")||dt053.Rows[i]["ブランドコード"].ToString().Contains("00349"))
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                }
            }

            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), ex.Message, dt053.Rows[i]["JANコード"].ToString(), dt053.Rows[i]["発注コード"].ToString(), 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
                fun.WriteLog(ex.Message + dt053.Rows[i]["発注コード"].ToString(), "053-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Search for BrandCode 00349.
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!webBrowser1.Url.ToString().Contains("/SearchSv?tono=")) return;

            fun.ClearMemory();

            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            webBrowser1.ScriptErrorsSuppressed = true;
            string body;
            string html;
            entity = new Qbei_Entity();
            entity.siteID = 53;
            entity.sitecode = "053";
            entity.janCode = dt053.Rows[i]["JANコード"].ToString();
            entity.partNo = dt053.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt053.Rows[i]["最終反映日"].ToString();
            entity.stockDate = dt053.Rows[i]["入荷予定"].ToString();
            entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
            entity.price = dt053.Rows[i]["下代"].ToString();
           
            if (entity.orderCode.Contains('│') || entity.orderCode.Contains("|"))
            {
                //string[] arr = entity.orderCode.Split('│');
                string[] arr = entity.orderCode.Split(new char[] { '│', '|' }, StringSplitOptions.RemoveEmptyEntries);
                entity.orderCode = string.IsNullOrEmpty(arr[0]) ? Regex.Replace(arr[1], "[/]+", string.Empty) : Regex.Replace(arr[0], "[/]+", string.Empty); ;
            }
            entity.purchaseURL = webBrowser1.Url.ToString();
            //entity.purchaseURL = fun.url + "/SearchSv?tono=" + entity.orderCode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25";

            if (!string.IsNullOrWhiteSpace(entity.orderCode))
            {
                body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("該当する製品はございません。もう一度、検索しなおして下さい。") || body.Contains("検索中にエラーが発生しました"))
                {
                    entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
                    if (dt053.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && (dt053.Rows[i]["在庫情報"].ToString().Contains("empty") || dt053.Rows[i]["在庫情報"].ToString().Contains("inquiry")))
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-01-10";
                    }
                    else
                    {
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                    }
                    fun.Qbei_Inserts(entity);
                }
                else
                {
                    try
                    {
                        entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
                        html = webBrowser1.Document.Body.InnerHtml;
                        HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(html);
                        //Check Element Exist or not
                        if ((hdoc.DocumentNode.SelectSingleNode("/table[1]/tbody/tr/td[9]") == null) && (hdoc.DocumentNode.SelectSingleNode("/table[1]/tbody/tr/td[11]") == null))
                        {
                            fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
                            fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "053-");
                            
                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            string alt = hdoc.DocumentNode.SelectSingleNode("/table[1]/tbody/tr/td[9]").InnerText;
                            alt = alt.Replace("台", string.Empty);
                            alt = alt.Replace(" 以上", string.Empty);

                            entity.qtyStatus = alt.Contains("○") || fun.IsGood(alt) ? "good" : alt.Contains("△") || alt.Contains("▲") || fun.IsSmall(alt) ? "small" : alt.Contains("×") || alt.Contains("完売") ? "empty" : alt.Contains("問合せ下さい") ? "inquiry" : "unknown status";
                            entity.stockDate = hdoc.DocumentNode.SelectSingleNode("/table[1]/tbody/tr/td[11]").InnerText;
                            
                            entity.stockDate = string.IsNullOrEmpty(entity.stockDate) || entity.stockDate.Equals("&nbsp;") ? alt.Contains("完売") ? "2100-02-01" : "2100-01-01" : entity.stockDate.Replace ("/","-");
                           
                            if (dt053.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                            {
                                if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")))
                                {
                                    entity.qtyStatus = dt053.Rows[i]["在庫情報"].ToString();
                                    
                                    entity.stockDate = dt053.Rows[i]["入荷予定"].ToString();
                                }
                                fun.Qbei_Inserts(entity);
                            }
                            else
                                fun.Qbei_Inserts(entity);
                            //i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");                        
                        fun.WriteLog(ex, "053-", entity.janCode, entity.orderCode);
                    }
                }
            }
            else
            {
                fun.Qbei_ErrorInsert(053, fun.GetSiteName("053"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
            }
            if (i < dt053.Rows.Count - 1)
            {
                string ordercode = dt053.Rows[++i]["発注コード"].ToString();
                //if (entity.orderCode.Contains('│'))
                if (entity.orderCode.Contains('│') || entity.orderCode.Contains("|"))
                {
                    //string[] arr = entity.orderCode.Split('│');
                    string[] arr = entity.orderCode.Split(new char[] { '│', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    entity.orderCode = string.IsNullOrEmpty(arr[0]) ? Regex.Replace(arr[1], "[/]+", string.Empty) : Regex.Replace(arr[0], "[/]+", string.Empty); ;
                }
                webBrowser1.Navigate(fun.url + "/SearchSv?tono=" + ordercode + "&seihin=&kang=0&b_kensaku.x=47&b_kensaku.y=25");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 53;
                qe.flag = 2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Search for BrandCode 00013
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void webBrowser2_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    string[] arr;
        //    try
        //    {
        //        webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser2_ItemSearch);
        //        webBrowser1.ScriptErrorsSuppressed = true;
        //        HtmlAgilityPack.HtmlDocument hdoc;
        //        string strBody;
        //        string strHtml;
        //        string strSeihinCd = string.Empty;
        //        string strQtyStatus;
        //        entity = new Qbei_Entity();
        //        entity.siteID = 53;
        //        entity.sitecode = "053";
        //        entity.janCode = dt053.Rows[i]["JANコード"].ToString();
        //        entity.partNo = dt053.Rows[i]["自社品番"].ToString();
        //        entity.makerDate = fun.getCurrentDate();
        //        entity.reflectDate = dt053.Rows[i]["最終反映日"].ToString();
        //        entity.stockDate = dt053.Rows[i]["入荷予定"].ToString();
        //        entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
        //        entity.price = dt053.Rows[i]["下代"].ToString();
        //        //if (entity.orderCode.Contains('│'))
        //        if (entity.orderCode.Contains('│') || entity.orderCode.Contains('|'))
        //        {
        //            //arr = entity.orderCode.Split('│');
        //            arr = entity.orderCode.Split(new char[] { '│', '|' }, StringSplitOptions.RemoveEmptyEntries);
        //            entity.orderCode = string.IsNullOrEmpty(arr[0]) ? Regex.Replace(arr[1], "[/]+", string.Empty) : Regex.Replace(arr[0], "[/]+", string.Empty); ;
        //            strSeihinCd = string.IsNullOrEmpty(arr[0]) ? arr[2] : arr[1];
        //        }
        //        entity.purchaseURL = fun.url + "/SearchSv?tono=&seihin=" + entity.orderCode + "&kang=0&b_kensaku.x=47&b_kensaku.y=25";
        //        strBody = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
        //        strHtml = webBrowser1.Document.GetElementsByTagName("table")[0].InnerHtml;
        //        hdoc = new HtmlAgilityPack.HtmlDocument();
        //        hdoc.LoadHtml(strHtml);
        //        var row = hdoc.DocumentNode.SelectNodes("/tbody/tr");
        //        //if (strBody.Contains("該当する製品はございません。もう一度、検索しなおして下さい。") || strBody.Contains("検索中にエラーが発生しました") || (row.Where(x => x.SelectSingleNode("td[4]") != null && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).SingleOrDefault() == null))
        //        if (strBody.Contains("該当する製品はございません。もう一度、検索しなおして下さい。") || strBody.Contains("検索中にエラーが発生しました") || (row.Where(x => x.SelectSingleNode("td[4]") != null && x.SelectSingleNode("td[3]").InnerText.Equals(entity.orderCode) && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).SingleOrDefault() == null))
        //        {
        //            entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
        //            if (dt053.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && (dt053.Rows[i]["在庫情報"].ToString().Contains("empty") || dt053.Rows[i]["在庫情報"].ToString().Contains("inquiry")))
        //            {
        //                entity.qtyStatus = "empty";
        //                entity.stockDate = "2100-01-10";
        //            }
        //            else
        //            {
        //                entity.qtyStatus = "empty";
        //                entity.stockDate = "2100-02-01";
        //            }
        //            fun.Qbei_Inserts(entity);
        //        }
        //        else
        //        {                   
        //            //if ((row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[9]")).SingleOrDefault() == null) && (row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[11]")).SingleOrDefault() == null))
        //            if ((row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[3]").InnerText.Equals(entity.orderCode) && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[9]")).SingleOrDefault() == null) && (row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[3]").InnerText.Equals(entity.orderCode) && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[11]")).SingleOrDefault() == null))
        //            {
        //                fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
        //                fun.WriteLog("Access Denied! " + entity.orderCode, "053--");
        //                Application.Exit();
        //            }
        //            else
        //            {
        //                //strQtyStatus = row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[9]").InnerText).SingleOrDefault();
        //                strQtyStatus = row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[3]").InnerText.Equals(entity.orderCode) && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[9]").InnerText).SingleOrDefault();
        //                strQtyStatus = strQtyStatus.Replace("台", string.Empty);
        //                strQtyStatus = strQtyStatus.Replace(" 以上", string.Empty);
        //                entity.qtyStatus = strQtyStatus.Contains("○") || fun.IsGood(strQtyStatus) ? "good" : strQtyStatus.Contains("△") || strQtyStatus.Contains("▲") || fun.IsSmall(strQtyStatus) ? "small" : strQtyStatus.Contains("×") || strQtyStatus.Contains("完売") ? "empty" : strQtyStatus.Contains("問合せ下さい") ? "inquiry" : "unknown status";
        //                //entity.stockDate = row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[11]").InnerText).SingleOrDefault();
        //                entity.stockDate = row.Where(x => x.SelectNodes("td") != null && x.SelectSingleNode("td[3]").InnerText.Equals(entity.orderCode) && x.SelectSingleNode("td[4]").InnerText.Equals(strSeihinCd)).Select(y => y.SelectSingleNode("td[11]").InnerText).SingleOrDefault();
        //                entity.stockDate = String.IsNullOrEmpty(entity.stockDate) || entity.stockDate.Equals("&nbsp;") ? strQtyStatus.Contains("完売") ? "2100-02-01" : "2100-01-01" : entity.stockDate.Replace("/", "-");
        //                entity.orderCode = dt053.Rows[i]["発注コード"].ToString();
        //                if ((dt053.Rows[i]["在庫情報"].ToString().Contains("empty") || dt053.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt053.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
        //                {
        //                    if (((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry")))
        //                    {
        //                        entity.qtyStatus = dt053.Rows[i]["在庫情報"].ToString();
        //                        entity.stockDate = dt053.Rows[i]["入荷予定"].ToString();
        //                    }
        //                }
        //                fun.Qbei_Inserts(entity);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");
        //        fun.WriteLog(ex.Message + entity.orderCode, "053--");
        //    }
        //    if (i < dt053.Rows.Count - 1)
        //    {
        //        string ordercode = dt053.Rows[++i]["発注コード"].ToString();
        //        if (entity.orderCode.Contains('│'))
        //        {
        //            arr = entity.orderCode.Split('│');
        //            entity.orderCode = string.IsNullOrEmpty(arr[0]) ? Regex.Replace(arr[1], "[/]+", string.Empty) : Regex.Replace(arr[0], "[/]+", string.Empty);
        //        }
        //        webBrowser1.Navigate(fun.url + "/SearchSv?tono=&seihin=" + ordercode + "&kang=0&b_kensaku.x=47&b_kensaku.y=25");
        //        webBrowser1.ScriptErrorsSuppressed = true;
        //        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
        //    }
        //    else
        //    {
        //        qe.site = 53;
        //        qe.flag = 2;
        //        qe.starttime = string.Empty;
        //        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        fun.ChangeFlag(qe);
        //        Application.Exit();
        //        Environment.Exit(0);
        //    }
        //}

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt053.Rows[i]["JANコード"].ToString();
            string orderCode = dt053.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(53, fun.GetSiteName("053"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "053");            
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "053-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
