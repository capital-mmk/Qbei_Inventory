using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Globalization;

namespace _028
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    class Program
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>               
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt028 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        private static int i;

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
                qe.site = 28;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(28);
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
                    fun.deleteData(28);
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
                fun.WriteLog(ex, "028-");
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
            DataTable dt028 = new DataTable();
            try
            {
                fun.setURL("028");
                fun.Qbei_Delete(28);
                fun.Qbei_ErrorDelete(28);
                dt028 = fun.GetDatatable("028");
                fun.GetTotalCount("028");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "028-");
                Environment.Exit(0);
            }

            /// <summary>
            /// Use to ChormeDriver and Data Table and Common Function and Field
            /// </summary>
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

                /// <summary>
                /// Login of Mall.
                /// </summary>
                qe.SiteID = 28;
                dt = qubl.Qbei_Setting_Select(qe);
                string url = dt.Rows[0]["Url"].ToString();
                chrome.Url = url;
                string title = chrome.Title;
                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Id("UserName")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Id("UserPW")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "037-");
                chrome.FindElement(By.Id("Button1")).Click();
                Thread.Sleep(8000);

                /// <summary>
                /// Check Login
                /// </summary>
                string orderCode = string.Empty;
                string body = chrome.FindElement(By.TagName("body")).Text;
                if (body.Contains("Invalid username and/or password."))
                {
                    fun.Qbei_ErrorInsert(28, fun.GetSiteName("028"), "Login Failed", dt028.Rows[0]["JANコード"].ToString(), dt028.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "028");
                    fun.WriteLog("Login Failed", "028-");

                    chrome.Quit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "028-");
                }

                /// <summary>
                /// Inspection of item information at Mall.
                /// </summary>
                try
                {
                    int Lastrow = dt028.Rows.Count;
                    for (i = 0; i < Lastrow; i++)
                    {
                        if (i < Lastrow)
                        {
                            string ordercode;
                            ordercode = dt028.Rows[i]["発注コード"].ToString();
                            //chrome.FindElement(By.Id("SearchParam")).Clear();
                            //chrome.FindElement(By.Id("SearchParam")).SendKeys(ordercode);
                            try
                            {
                                chrome.FindElement(By.Id("SearchParam")).Clear();
                                chrome.FindElement(By.Id("SearchParam")).SendKeys(ordercode);
                                chrome.FindElement(By.Id("Search")).Click();
                            }
                            catch
                            {
                                chrome.Navigate().GoToUrl("https://cegnet.com/ustock/storage.aspx");
                                Thread.Sleep(20000);
                                chrome.FindElement(By.Id("SearchParam")).Clear();
                                chrome.FindElement(By.Id("SearchParam")).SendKeys(ordercode);
                                chrome.FindElement(By.Id("Search")).Click();
                            }
                            Thread.Sleep(4000);

                            entity = new Qbei_Entity();
                            entity.siteID = 28;
                            entity.sitecode = "028";
                            entity.janCode = dt028.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt028.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt028.Rows[i]["最終反映日"].ToString();
                            entity.orderCode = dt028.Rows[i]["発注コード"].ToString().Trim();
                            entity.purchaseURL = "https://cegnet.com/ustock/storage.aspx";

                            //<remark>
                            //Check to  Item is Correct Data 
                            //</remark>
                            string ItemCheck;
                            ItemCheck = chrome.FindElement(By.TagName("body")).Text;

                            //<remark>
                            //Check to Ordercode
                            //</remark>
                            if (!string.IsNullOrWhiteSpace(entity.orderCode))
                            {
                                if (ItemCheck.Contains("テーブルにデータがありません"))
                                {
                                    entity.qtyStatus = "empty";
                                    entity.stockDate = "2100-02-01";
                                    entity.price = dt028.Rows[i]["下代"].ToString();
                                    entity.True_StockDate = "Not Found";
                                    entity.True_Quantity = "Not Found";
                                    fun.Qbei_Inserts(entity);
                                }
                                else
                                {   
                                    int n = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr")).Count());
                                    if (n == 0)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt028.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        for (int j = 1; j <= n; j++)
                                        {
                                            string check_Ean = chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td[4]")).Text.Replace("[", " ").Replace("]", " ").Trim();
                                            if (check_Ean.Equals(entity.janCode))
                                            {
                                                //<remark>
                                                //Check to Quantity
                                                //</remark>      
                                                string qty;
                                                try
                                                {
                                                    qty = chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td[3]/b")).Text;
                                                }
                                                catch
                                                {
                                                    try
                                                    {
                                                        Thread.Sleep(2000);
                                                        qty = chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td[3]/b")).Text;
                                                    }
                                                    catch
                                                    {
                                                        qty = " ";
                                                    }
                                                }
                                                entity.qtyStatus = qty.Equals(" ") || qty.Equals(null) || qty.Equals("0") || qty.Equals("1") || qty.Equals("2") ? "empty" : qty.Equals("10+") ? "small" : Convert.ToInt32(qty)>=3 ? "small" : "empty";

                                                //<remark>
                                                //Check to Price
                                                //</remark>
                                                entity.price = chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td[5]")).Text;
                                                if (entity.price.Contains("."))
                                                {
                                                    //string[] price_split = entity.price.Split('.');
                                                    //entity.price = price_split[0];
                                                    entity.price = entity.price.Replace("円", " ").Replace(".", string.Empty);
                                                }
                                                entity.price = entity.price.Replace("円", " ").Replace(",", string.Empty);                                                                                               

                                                string start_month= chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/thead/tr/th[6]")).Text;
                                                int now_month = DateTime.ParseExact(start_month, "MMM", CultureInfo.CreateSpecificCulture("en-GB")).Month;
                                                int now_year= Convert.ToInt32(DateTime.Now.Year);
                                                int month_colunms = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td")).Count());
                                                string month;
                                                for (int g = 6; g <=month_colunms ; g++)
                                                {
                                                    string check_month= chrome.FindElement(By.XPath("/html/body/form/div[3]/div/div/div/div[2]/div/table/tbody/tr[" + j + "]/td[" + g + "]")).Text;
                                                    month = check_month;
                                                    if (check_month =="")
                                                    {
                                                        entity.stockDate = "2100-01-01";
                                                        entity.True_Quantity = qty;
                                                        entity.True_StockDate = "項目無し";
                                                    }
                                                    else
                                                    {
                                                        if (now_month >= 4 && now_month > 12)
                                                        {
                                                            now_year = now_year + 1;
                                                            now_month = now_month - 12;
                                                            entity.stockDate = now_year + "-" + now_month + "-" + "15";
                                                            entity.True_Quantity = qty;
                                                            //entity.True_StockDate = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(now_month);
                                                            entity.True_StockDate = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(now_month);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            entity.stockDate = now_year + "-" + now_month + "-" + "15";
                                                            entity.True_Quantity = qty;
                                                            //entity.True_StockDate = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(now_month);
                                                            entity.True_StockDate = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(now_month);
                                                            break;
                                                        }
                                                    }
                                                    now_month++;
                                                }
                                                break;
                                            }
                                        }
                                        DateTime d = Convert.ToDateTime(entity.stockDate);
                                        if (d <= (DateTime.Now))
                                        {
                                            d = d.AddYears(1);
                                        }
                                        entity.stockDate = d.ToString("yyyy-MM-dd");
                                        if ((entity.qtyStatus != "unknown status"))
                                        {
                                            if (entity.qtyStatus != null)
                                            {
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else
                                            {
                                                fun.Qbei_ErrorInsert(28, fun.GetSiteName("028"), "Jancode doesn't exit!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "028");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(28, fun.GetSiteName("028"), "Item doesn't Check!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "028");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                fun.Qbei_ErrorInsert(28, fun.GetSiteName("028"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "028");
                            }
                        }
                    }
                    qe.site = 28;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    chrome.Quit();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    string janCode = dt028.Rows[i]["JANコード"].ToString();
                    orderCode = dt028.Rows[i]["発注コード"].ToString();
                    fun.Qbei_ErrorInsert(28, fun.GetSiteName("028"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "028");
                    fun.WriteLog(ex, "028-", janCode, orderCode);
                }
            }
        }
    }
}
