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

namespace _058リンエイ
{
    class _058
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>               
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt058 = new DataTable();
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
                qe.site = 58;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(58);
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
                    fun.deleteData(58);
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
                fun.WriteLog(ex, "058-");
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
            DataTable dt058 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("058");
                fun.Qbei_Delete(58);
                fun.Qbei_ErrorDelete(58);
                dt058 = fun.GetDatatable("058");
                fun.GetTotalCount("058");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "058-");
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
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);//<remark Add Logic for ChormeDriver 2025/04/08 />
                chromeOptions.AddArguments("-no-sandbox");//<remark Add Logic for ChormeDriver 2021/09/02 />
                chromeOptions.AddArgument("--start-maximized");
                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);//<remark Add Logic for ChormeDriver 2021/09/02 />                                                                                                                       
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
                    string Year;

                    /// <summary>
                    /// Login of Mall.
                    /// </summary>
                    qe.SiteID = 58;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string url = dt.Rows[0]["Url"].ToString();
                    chrome.Url = url;
                    string title = chrome.Title;

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("member[email]")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("member[password]")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "058-");
                    chrome.FindElement(By.XPath("/html/body/div/main/div[3]/div[1]/div[2]/div[2]/div[1]/div/form/div/button")).Click();
                    Thread.Sleep(8000);

                    /// <summary>
                    /// Check Login
                    /// </summary>
                    string orderCode = string.Empty;
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("メールアドレスまたはパスワードが違います"))
                    {
                        fun.Qbei_ErrorInsert(58, fun.GetSiteName("058"), "Login Failed", dt058.Rows[0]["JANコード"].ToString(), dt058.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "058");
                        fun.WriteLog("Login Failed", "058-");

                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "058-");

                        /// <summary>
                        /// Inspection of item information at Mall.
                        /// </summary>
                        try
                        {
                            int Lastrow = dt058.Rows.Count;
                            for (i = 0; i < Lastrow; i++)
                            {
                                if (i < Lastrow)
                                {
                                    ordercode = dt058.Rows[i]["JANコード"].ToString();
                                    chrome.FindElement(By.Id("keyword")).Clear();
                                    chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                    try
                                    {
                                        chrome.FindElement(By.Id("search-button")).Click();
                                    }
                                    catch
                                    {
                                        Thread.Sleep(20000);
                                        chrome.FindElement(By.Id("keyword")).Clear();
                                        chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                        chrome.FindElement(By.Id("search-button")).Click();
                                    }

                                    entity = new Qbei_Entity();
                                    entity.siteID = 58;
                                    entity.sitecode = "058";
                                    entity.janCode = dt058.Rows[i]["JANコード"].ToString();
                                    entity.partNo = dt058.Rows[i]["自社品番"].ToString();
                                    entity.makerDate = fun.getCurrentDate();
                                    entity.reflectDate = dt058.Rows[i]["最終反映日"].ToString();
                                    entity.orderCode = dt058.Rows[i]["発注コード"].ToString().Trim();
                                    entity.purchaseURL = "https://www.rinei-web.jp/products?keyword=" + entity.orderCode;

                                    if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                    {
                                        Thread.Sleep(500);
                                        body = chrome.FindElement(By.TagName("body")).Text;
                                        if (body.Contains("検索結果はありません。"))
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt058.Rows[i]["下代"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = " ";
                                            fun.Qbei_Inserts(entity);
                                        }
                                        else if (body.Contains("コードNoが不正です。"))
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt058.Rows[i]["下代"].ToString();
                                            entity.True_StockDate = "コードNoが不正です。";
                                            entity.True_Quantity = " ";
                                            fun.Qbei_Inserts(entity);
                                        }
                                        else
                                        {
                                            int n = chrome.FindElements(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr")).Count();

                                            if (n == 0)
                                            {

                                                Thread.Sleep(18000);
                                                chrome.FindElement(By.Id("keyword")).Clear();
                                                chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                                chrome.FindElement(By.Id("search-button")).Click();
                                            }
                                            int count = 0;
                                        Searchagain:

                                            Thread.Sleep(1000);
                                            try
                                            {
                                                body = chrome.FindElement(By.TagName("body")).Text;
                                            }
                                            catch (Exception ex)
                                            {
                                                Thread.Sleep(3000);
                                                fun.WriteLog(ex.ToString(), "058-");
                                                count++;
                                                if (count < 2)
                                                {
                                                    goto Searchagain;
                                                }
                                                break;
                                            }
                                            Thread.Sleep(500);
                                            if (body.Contains("検索結果はありません。"))
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.price = dt058.Rows[i]["下代"].ToString();
                                                entity.True_StockDate = "Not Found";
                                                entity.True_Quantity = " ";
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else if (n == 1)
                                            {
                                                if (chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/th[1]")).Text.Contains(entity.orderCode))
                                                {
                                                    entity.price = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/th[3]")).Text.Replace("円", "").Replace(",", "").Replace("@", "").Trim();
                                                    string stock = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/th[6]")).GetAttribute("innerHTML").ToString().Trim();
                                                    entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("▲") ? "small" : stock.Equals("×") ? "empty" : stock.Contains("入荷予定") ? "empty" : stock.Contains("入荷待ち") ? "empty" : stock.Equals("取り寄せ") ? "empty" : stock.Equals("廃番") ? "empty" : "unknown status";
                                                    entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                    entity.True_StockDate = "項目無し";
                                                    entity.True_Quantity = stock;
                                                    string a = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/td[1]")).Text;
                                                    if (chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/td[1]")).Text.Contains("未定") || chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/td[1]")).Text.Contains("月"))
                                                    {
                                                        entity.stockDate = "2100-02-01";
                                                        entity.True_StockDate = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr/td[1]")).Text;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 1; i <= n; i++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/th[1]")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/th[3]")).Text.Replace("円", "").Replace(",", "").Replace("@", "").Trim();
                                                        string stock = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/th[6]")).GetAttribute("innerHTML").ToString().Trim();
                                                        entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("▲") ? "small" : stock.Equals("×") ? "empty" : stock.Contains("入荷予定") ? "empty" : stock.Contains("入荷待ち") ? "empty" : stock.Equals("取り寄せ") ? "empty" : stock.Equals("廃番") ? "empty" : "unknown status";
                                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                        entity.True_StockDate = "項目無し";
                                                        entity.True_Quantity = stock;
                                                        string a = chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/td[1]")).GetAttribute("innerHTML").ToString();
                                                        if (chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/td[1]")).Text.Contains("未定") || chrome.FindElement(By.XPath("/html/body/div/main/div[4]/section/div[2]/div[2]/div[1]/div/form/div[2]/table/tbody/tr[" + (i) + "]/td[1]")).Text.Contains("月"))
                                                        {
                                                            entity.stockDate = "2100-02-01";
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                            if (d <= (DateTime.Now))
                                            {
                                                d = d.AddYears(1);
                                            }
                                            entity.stockDate = d.ToString("yyyy-MM-dd");

                                            if (entity.qtyStatus != "unknown status")
                                            {
                                                if (entity.qtyStatus != null)
                                                {
                                                    fun.Qbei_Inserts(entity);
                                                }
                                                else
                                                {
                                                    fun.Qbei_ErrorInsert(58, fun.GetSiteName("058"), "entity.qtyStatus is null!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "058");
                                                }
                                            }
                                            else
                                            {
                                                fun.Qbei_ErrorInsert(58, fun.GetSiteName("058"), "entity.qtyStatus is unknown status!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "058");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fun.Qbei_ErrorInsert(58, fun.GetSiteName("058"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "058");
                                    }
                                }
                            }
                            qe.site = 58;
                            qe.flag = 2;
                            qe.starttime = string.Empty;
                            qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            fun.ChangeFlag(qe);
                            chrome.Quit();
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            string janCode = dt058.Rows[i]["JANコード"].ToString();
                            orderCode = dt058.Rows[i]["発注コード"].ToString();
                            fun.Qbei_ErrorInsert(58, fun.GetSiteName("058"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "058");
                            fun.WriteLog(ex, "058-", janCode, orderCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "058-");
                Environment.Exit(0);
            }
        }
    }
}
