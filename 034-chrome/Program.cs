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

namespace _034_chrome
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    static class Program
    {
        public static DataTable dt = new DataTable();
        public static Qbeisetting_BL qubl = new Qbeisetting_BL();
        public static Qbeisetting_Entity qe = new Qbeisetting_Entity();
        public static CommonFunction fun = new CommonFunction();
        public static DataTable dt034 = new DataTable();
        public static Qbei_Entity entity = new Qbei_Entity();
        public static int i = 0;
        public static ChromeOptions option = new ChromeOptions();

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        [STAThread]
        static void Main(string[] args)
        {
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
        public static void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 34;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(34);
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
                    fun.deleteData(34);
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "034-");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>

        public static void StartRun()
        {
            try
            {
                fun.setURL("034");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(34);
                fun.Qbei_ErrorDelete(34);
                dt034 = fun.GetDatatable("034");
                //dt034 = fun.GetOrderData(dt034, "https://sips.shimano.co.jp/front/g/g", "034", string.Empty);
                fun.GetTotalCount("034");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "034-");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Read to Data and Url.
        /// </remark>


        public static void ReadData()
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddArguments("-no-sandbox");

                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);
                using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))
                {
                    chrome.Manage().Window.Maximize();
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    qe.SiteID = 34;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();
                    chrome.Url = fun.url;
                    string title = chrome.Title;
                    Thread.Sleep(8000);
                    ///<remark>
                    ///Login to mall.
                    ///</remark>
                    //2023-08-23 Start
                    try
                    {
                        string username = dt.Rows[0]["UserName"].ToString();
                        chrome.FindElement(By.Id("login-form-email")).SendKeys(username);
                        Thread.Sleep(2000);
                        string password = dt.Rows[0]["Password"].ToString();
                        chrome.FindElement(By.Id("login-form-password")).SendKeys(password);
                        Thread.Sleep(2000);
                        fun.WriteLog("Navigation to Site Url success------", "034-");
                        chrome.FindElement(By.XPath("/html/body/app-root/cx-storefront/main/cx-page-layout/cx-page-slot[2]/app-login/div/div/form/div[7]/button")).Click();
                        Thread.Sleep(2000);
                        string body = chrome.FindElement(By.TagName("body")).Text;
                        if (body.Contains("メールアドレスまたはパスワードが正しくありません"))
                        {
                            fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Login Failed", dt034.Rows[0]["JANコード"].ToString(), dt034.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                            fun.WriteLog("Login Failed", "034-");
                            chrome.Quit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            fun.WriteLog("Login success             ------", "034-");
                            try
                            {
                                int Lastrow = dt034.Rows.Count;
                                for (i = 0; i < Lastrow; i++)
                                {
                                    if (i < Lastrow)
                                    {
                                        Thread.Sleep(2000);
                                        entity = new Qbei_Entity();
                                        entity.siteID = 34;
                                        entity.sitecode = "034";
                                        entity.janCode = dt034.Rows[i]["JANコード"].ToString();
                                        entity.partNo = dt034.Rows[i]["自社品番"].ToString();
                                        entity.makerDate = fun.getCurrentDate();
                                        entity.reflectDate = dt034.Rows[i]["最終反映日"].ToString();
                                        entity.orderCode = dt034.Rows[i]["発注コード"].ToString();

                                        entity.purchaseURL = fun.url + "/product/" + entity.orderCode;

                                        if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                        {
                                            chrome.Url = entity.purchaseURL;
                                            Thread.Sleep(8000);
                                            string NGbody = chrome.FindElement(By.TagName("body")).Text;
                                            if (NGbody.Contains("販売終了") || chrome.Url.Contains("product-not-found"))
                                            {

                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.price = dt034.Rows[i]["下代"].ToString();
                                                entity.True_StockDate = "項目なし";
                                                entity.True_Quantity = "項目なし";
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    Thread.Sleep(2000);
                                                    int counter = 0;
                                                label1:
                                                    if (counter < 10)
                                                    {
                                                        chrome.Url = entity.purchaseURL;
                                                        Thread.Sleep(8000);

                                                        string date = string.Empty;
                                                        string alt = string.Empty;
                                                        string Stockbody = chrome.FindElement(By.TagName("body")).Text;
                                                        if (Stockbody.Contains("在庫なし") || Stockbody.Contains("在庫あり"))
                                                        {
                                                            alt = chrome.FindElement(By.XPath("/html/body/app-root/cx-storefront/main/cx-page-layout/cx-page-slot[1]/app-product-stock-info/div/span")).Text;
                                                            Thread.Sleep(2000);
                                                            entity.qtyStatus = alt.Contains("在庫なし") ? "empty" : alt.Contains("在庫あり") ? "good" : "empty";
                                                        }
                                                        else
                                                        {
                                                            Thread.Sleep(3000);
                                                            counter++;
                                                            goto label1;
                                                        }

                                                        entity.price = chrome.FindElement(By.XPath("/html/body/app-root/cx-storefront/main/cx-page-layout/cx-page-slot[1]/app-product-pricing/div/div/span")).Text;
                                                        Thread.Sleep(2000);
                                                        entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);


                                                        string StockTxtbody = chrome.FindElement(By.TagName("body")).Text;
                                                        if (StockTxtbody.Contains("入荷予定: "))
                                                        {
                                                            date = chrome.FindElement(By.XPath("/html/body/app-root/cx-storefront/main/cx-page-layout/cx-page-slot[1]/app-product-stock-info/div/span[2]")).Text;
                                                        }
                                                        // 入荷予定日がNULLの時
                                                        if (string.IsNullOrWhiteSpace(date))
                                                        {

                                                            entity.stockDate = "2100-02-01";
                                                            entity.True_StockDate = "項目なし";
                                                            entity.True_Quantity = alt;
                                                        }
                                                        else
                                                        {
                                                            if ((!string.IsNullOrWhiteSpace(date)) && alt.Contains("在庫なし"))
                                                            {
                                                                string Day = date.Replace("入荷予定:", "").Replace("日後", "").Replace("\r\n", "").Trim();//"入荷予定: 34 日後";
                                                                string CalDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(int.Parse(Day)).ToString("yyyy-MM-dd");
                                                                entity.stockDate = CalDate;
                                                                entity.True_StockDate = date;
                                                                entity.True_Quantity = alt;
                                                            }
                                                            else
                                                            {
                                                                entity.stockDate = "2100-02-01";
                                                                entity.True_StockDate = "項目なし";
                                                                entity.True_Quantity = alt;

                                                            }

                                                        }
                                                    }
                                                    fun.Qbei_Inserts(entity);
                                                }
                                                catch (Exception ex)
                                                {
                                                    fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                                                    fun.WriteLog(ex, "034-", entity.janCode, entity.orderCode);

                                                    chrome.Quit();
                                                    Environment.Exit(0);
                                                }
                                            }
                                        }

                                        else
                                        {
                                            fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                                        }
                                    }
                                }
                                qe.site = 34;
                                qe.flag = 2;
                                qe.starttime = string.Empty;
                                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                fun.ChangeFlag(qe);
                                chrome.Quit();
                                Environment.Exit(0);
                            }
                                    
                            catch (Exception ex)
                            {
                                fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                                fun.WriteLog(ex, "034-", entity.janCode, entity.orderCode);

                                chrome.Quit();
                                Environment.Exit(0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt034.Rows[0]["JANコード"].ToString();
                        string orderCode = dt034.Rows[0]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(34, fun.GetSiteName("034"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "034");
                        fun.WriteLog(ex, "034-", janCode, orderCode);

                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    //2023-08-23 End
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "034-");
                Environment.Exit(0);
            }
        }
        /// <summary>
        /// Login of Mall.
        /// </summary>
    }
}
