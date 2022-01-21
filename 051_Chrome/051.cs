using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _051スタイルバイク
{
    class _051
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>               
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt051 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        private static string h;
        public static string ordercode;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        static void Main(string[] args)
        {
            testFlag();
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
        public static void testFlag()
        {

            try
            {
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                DataTable dtSetting = new DataTable();

                int intFlag;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 51;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(51);
                intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());

                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                /// Check to flag is "0" or "1" or "2".
                /// when flag is 0,Change to flag is 1 and Continue to StartRun Process.
                /// </remark>
                if (intFlag == 0)
                {
                    fun.ChangeFlag(qe);
                    startRun();
                }

                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (intFlag == 1)
                {
                    fun.deleteData(51);
                    fun.ChangeFlag(qe);
                    startRun();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "051-");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        public static void startRun()
        {
            DataTable dt051 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("051");
                fun.Qbei_Delete(51);
                fun.Qbei_ErrorDelete(51);
                dt051 = fun.GetDatatable("051");
                fun.GetTotalCount("051");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "051-");
                Environment.Exit(0);
            }

            /// <summary>
            /// Use to ChormeDriver and Data Table and Common Function and Field
            /// </summary>
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";//<Add Logic for Chrome Path 2021/05/24 />
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");//<remark Add Logic for ChormeDriver 2021/09/02 />
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");//<remark Add Logic for ChormeDriver 2021/09/02 />
                chromeOptions.AddArguments("-no-sandbox");//<remark Add Logic for ChormeDriver 2021/09/02 />
                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);//<remark Add Logic for ChormeDriver 2021/09/02 />                                                                                                                       
                //using (IWebDriver chrome = new ChromeDriver(chromeOptions))
                using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))//<remark Edit Logic for ChormeDriver 2021/09/02 />
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    Qbei_Entity entity = new Qbei_Entity();
                    int Month;
                    string Day;
                    string strStockDate = string.Empty;
                    string qty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));

                    /// <summary>
                    /// Login of Mall.
                    /// </summary>
                    qe.SiteID = 51;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string url = dt.Rows[0]["Url"].ToString();
                    chrome.Url = url;
                    string title = chrome.Title;

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("loginEmail")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("loginPassword")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "051-");
                    chrome.FindElement(By.Name("login")).Click();
                    Thread.Sleep(8000);

                    /// <summary>
                    /// Check Login
                    /// </summary>
                    string orderCode = string.Empty;
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains(" 入力情報に誤りがあります。入力された内容を再度ご確認ください") || body.Contains("当サイトは完全会員制です。") || body.Contains("会員登録がお済みでないお客様は、会員登録フォームよりお申し込みください。"))
                    {
                        fun.Qbei_ErrorInsert(51, fun.GetSiteName("051"), "Login Failed", dt051.Rows[0]["JANコード"].ToString(), dt051.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "051");
                        fun.WriteLog("Login Failed", "051-");

                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "051-");
                        try
                        {
                            if (chrome.FindElement(By.ClassName("modaal-close")) != null)
                            {
                                //string check_page = chrome.FindElement(By.XPath("/html/body/div[13]/div/div/div/div/div/section/div/div[1]/span[1]")).Text;
                                //string check_page = chrome.FindElement(By.ClassName("__count")).Text;
                                //var page_no = check_page.Split(' ');
                                //for (int i = 1; i <= Convert.ToInt32(page_no[1]); ++i)
                                //{
                                //    Thread.Sleep(2000);
                                //    chrome.FindElement(By.Id("modaal-close")).Click();
                                //}
                                //<remark Edit&Add Logic for Close Advertisments 2021/08/26 Start>
                                //var page_no = chrome.FindElement(By.ClassName("__count")).GetAttribute("innerHTML");
                                string page = chrome.FindElement(By.ClassName("__page-info")).GetAttribute("innerHTML").ToString();
                                string[] page_no = page.Split('>');
                                page = page_no[3];
                                string[] page_last = page.Split('<');
                                for (int i = 1; i <= Convert.ToInt32(page_last[0]); ++i)
                                // for (int i = 1; i <= Convert.ToInt32(page_no); ++i)
                                //</remark 2021/08/26 End>
                                {
                                    Thread.Sleep(2000);
                                    chrome.FindElement(By.Id("modaal-close")).Click();
                                }
                                fun.WriteLog("Finished Advertisemnts             ------", "051-");
                            }
                            else
                            {
                                fun.WriteLog("No Advertisemnts             ------", "051-");
                            }
                        }
                        catch
                        {
                            fun.WriteLog("Error Case of Advertisemnts             ------", "051-");
                        }

                        /// <summary>
                        /// Inspection of item information at Mall.
                        /// </summary>
                        try
                        {
                            int Lastrow = dt051.Rows.Count;
                            for (i = 0; i < Lastrow; i++)
                            {
                                if (i < Lastrow)
                                {
                                    Thread.Sleep(2000);
                                    ordercode = dt051.Rows[i]["発注コード"].ToString();
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/section[3]/form/div/input")).Clear();
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/section[3]/form/div/input")).SendKeys(ordercode);
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/section[3]/form/div/button")).Click();

                                    entity = new Qbei_Entity();
                                    entity.siteID = 51;
                                    entity.sitecode = "051";
                                    entity.janCode = dt051.Rows[i]["JANコード"].ToString();
                                    entity.partNo = dt051.Rows[i]["自社品番"].ToString();
                                    entity.makerDate = fun.getCurrentDate();
                                    entity.reflectDate = dt051.Rows[i]["最終反映日"].ToString();
                                    entity.orderCode = dt051.Rows[i]["発注コード"].ToString().Trim();
                                    entity.purchaseURL = "https://sb2b.jp/list.php?keyword=" + entity.orderCode;

                                    if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                    {
                                        body = chrome.FindElement(By.TagName("body")).Text;
                                        if (body.Contains("お探しの検索条件に合致する商品は見つかりませんでした。"))
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt051.Rows[i]["下代"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = "Not Found";
                                            fun.Qbei_Inserts(entity);
                                        }
                                        else
                                        {
                                            if (chrome.FindElement(By.ClassName("__photo")) == null)
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.price = dt051.Rows[i]["下代"].ToString();
                                                entity.True_StockDate = "Not Found";
                                                entity.True_Quantity = "Not Found";
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else
                                            {
                                                int n = chrome.FindElements(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr")).Count();

                                                if (n == 0)
                                                {
                                                    entity.qtyStatus = "empty";
                                                    entity.stockDate = "2100-02-01";
                                                    entity.price = dt051.Rows[i]["下代"].ToString();
                                                    entity.True_StockDate = "Not Found";
                                                    entity.True_Quantity = "Not Found";
                                                    fun.Qbei_Inserts(entity);
                                                }
                                                else
                                                {
                                                    for (int i = 1; i <= n; i++)
                                                    {
                                                        if (chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + (i) + "]/td[1]/span[2]")).Text.Equals("[" + entity.orderCode + "]"))
                                                        {
                                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + (i) + "]/td[1]/div/span[1]/span")).Text.Replace("円", "").Replace(",", "").Trim();
                                                            string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + (i) + "]")).GetAttribute("innerHTML").ToString().Trim();
                                                            if (stock.Contains("__input"))
                                                            {
                                                                //if (chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + (i) + "]/td[2]/div[3]/dl/dd")) == null)//<remark Edit Logic for Check to Quantity 2021/11/05 />
                                                                if (!stock.Contains("在庫"))
                                                                {
                                                                    entity.qtyStatus = "empty";
                                                                    entity.stockDate = "2100-02-01";
                                                                    entity.True_StockDate = "Not Found";
                                                                    entity.True_Quantity = "Not Found";
                                                                }
                                                                else
                                                                {
                                                                    qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + (i) + "]/td[2]/div[3]/dl/dd")).Text;
                                                                    //entity.qtyStatus = qty.Equals("○") || qty.Contains("最少") ? "good" : qty.Equals("△") ? "empty" : qty.Equals("×") ? "empty" : "unknown status";
                                                                    entity.qtyStatus = qty.Equals("○") || qty.Contains("最少") ? "good" : qty.Equals("△") ? "small" : qty.Equals("×") ? "empty" : "unknown status";//<remark ロジックの変更　2022/01/21 />
                                                                    //entity.stockDate = qty.Equals("○") || qty.Contains("最少") ? "2100-01-01" : qty.Equals("△") ? "2100-02-01" : "unknown status";
                                                                    entity.stockDate = qty.Equals("○") || qty.Equals("△") || qty.Contains("最少") ? "2100-01-01" : "unknown status";//<remark ロジックの変更　2022/01/21 />
                                                                    entity.True_StockDate = "Not Found";
                                                                    entity.True_Quantity = qty;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                entity.qtyStatus = "empty";
                                                                entity.stockDate = "2100-02-01";
                                                                entity.True_StockDate = "Not Found";
                                                                entity.True_Quantity = "Not Found";
                                                            }
                                                            break;
                                                        }
                                                    }
                                                    if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                                    {
                                                        entity.qtyStatus = "empty";
                                                        entity.stockDate = "2100-02-01";
                                                        entity.price = dt051.Rows[i]["下代"].ToString();
                                                        entity.True_StockDate = "Not Found";
                                                        entity.True_Quantity = "Not Found";
                                                    }
                                                    DateTime d = Convert.ToDateTime(entity.stockDate);
                                                    if (d <= (DateTime.Now))
                                                    {
                                                        d = d.AddYears(1);
                                                    }
                                                    entity.stockDate = d.ToString("yyyy-MM-dd");

                                                    if ((entity.qtyStatus != "unknown status"))
                                                    {
                                                        fun.Qbei_Inserts(entity);
                                                    }
                                                    else
                                                    {
                                                        fun.Qbei_ErrorInsert(51, fun.GetSiteName("051"), "Item doesn't Check!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "051");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fun.Qbei_ErrorInsert(51, fun.GetSiteName("051"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "051");
                                    }
                                }
                            }
                            qe.site = 51;
                            qe.flag = 2;
                            qe.starttime = string.Empty;
                            qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            fun.ChangeFlag(qe);
                            chrome.Quit();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            string janCode = dt051.Rows[i]["JANコード"].ToString();
                            orderCode = dt051.Rows[i]["発注コード"].ToString();
                            fun.Qbei_ErrorInsert(51, fun.GetSiteName("051"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "051");
                            fun.WriteLog(ex, "051-", janCode, orderCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "051-");
                Environment.Exit(0);
            }
        }
    }
}
