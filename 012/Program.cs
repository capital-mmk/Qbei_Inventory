//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;

//namespace _12カワシマ
//{
//    static class Program
//    {
//        /// <summary>
//        /// The main entry point for the application.
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
//            Application.Run(new frm012());
//        }
//    }
//}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using Common;
using System.IO;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _12カワシマ
{
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
        DataTable dt012 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        public static string ordercode;

        /// <summary>
        /// System(Start).
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        /// </summary>
        /// <param name="args">constant string</param>
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
            Qbeisetting_Entity entitySetting = new Qbeisetting_Entity();
            DataTable dtSetting = new DataTable();
            int intFlag;
            entitySetting.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entitySetting.site = 12;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(012);
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
                fun.ChangeFlag(entitySetting);
                StartRun();
            }

            ///<remark>
            ///when flag is 1,To Continue to StartRun Process.
            ///</remark>
            else if (intFlag == 1)
            {
                fun.deleteData(12);
                fun.ChangeFlag(entitySetting);
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
        public static void StartRun()
        {
            DataTable dt012 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("012");
                fun.Qbei_Delete(12);
                fun.Qbei_ErrorDelete(12);
                dt012 = fun.GetDatatable("012");
                fun.GetTotalCount("012");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "012-");
                Environment.Exit(0);
            }
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false); //<remark Add Logic for ChormeDriver 2025/04/08 />
                chromeOptions.AddArguments("-no-sandbox");
                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);
                using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    Qbei_Entity entity = new Qbei_Entity();
                    string strStockDate = string.Empty;
                    string qty = string.Empty;
                    string color = string.Empty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 12;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Id("login_uid")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Id("login_pwd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "012-");
                    chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/form/div/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください。"))
                    {
                        fun.WriteLog("Login Failed", "012-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "012-");
                    }


                    try
                    {

                        int Lastrow = dt012.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt012.Rows[i]["発注コード"].ToString();

                                entity = new Qbei_Entity();
                                entity.siteID = 12;
                                entity.sitecode = "012";
                                entity.janCode = dt012.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt012.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt012.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt012.Rows[i]["発注コード"].ToString();

                                chrome.Navigate().GoToUrl(fun.url + "/shop/g/g" + entity.orderCode);
                                Thread.Sleep(1000);

                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {

                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Check_Message.Contains("申し訳ございません。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt012.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }

                                    else
                                    {
                                        if (chrome.FindElement(By.CssSelector(".selectText_")).Text == entity.janCode)
                                        {
                                            entity.price = chrome.FindElement(By.CssSelector(".price_")).Text;
                                            entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);

                                            qty = chrome.FindElement(By.CssSelector("#spec_stock_msg")).Text;
                                            entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("△") ? "small" : qty.Equals("×") || qty.Equals("終了") || qty.Equals("×(終了)") ? "empty" : "unknown code";
                                            entity.stockDate = qty.Equals("○") || qty.Equals("△") ? "2100-01-01" : qty.Equals("終了") || qty.Equals("×") || qty.Equals("×(終了)") || qty.Equals("× 残 6") ? "2100-02-01" : "unknown date";
                                            entity.True_Quantity = qty;

                                            if (qty == "×")   // <remark check stock quantity × and 予 約 10-07-2025>
                                            {
                                                if (chrome.FindElement(By.CssSelector(".btn_cart_")).GetAttribute("order_type") == "reserve")
                                                {
                                                    entity.qtyStatus = "inquiry";
                                                    entity.stockDate = "2100-01-01";
                                                }
                                                else
                                                {
                                                    entity.qtyStatus = "empty";
                                                    entity.stockDate = "2100-02-01";
                                                }
                                            }

                                            entity.purchaseURL = dt012.Rows[i]["purchaserURL"].ToString();
                                            if (entity.purchaseURL == "")
                                            {
                                                entity.purchaseURL = chrome.Url;
                                            }
                                            entity.True_StockDate = "項目無し";
                                            fun.Qbei_Inserts(entity);
                                        }

                                        else
                                        {
                                            entity.price = chrome.FindElement(By.CssSelector(".price_")).Text;
                                            entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);
                                            qty = chrome.FindElement(By.CssSelector("#spec_stock_msg")).Text;

                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = chrome.Url;
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = qty;
                                            fun.Qbei_Inserts(entity);
                                        }

                                    }


                                    if (entity.price == null || entity.qtyStatus == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt012.Rows[i]["下代"].ToString();
                                        entity.purchaseURL = dt012.Rows[i]["purchaserURL"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                    }

                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                                }
                            }
                        }
                        qe.site = 12;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt012.Rows[i]["JANコード"].ToString();
                        ordercode = dt012.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(12, fun.GetSiteName("012"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "012");
                        fun.WriteLog(ex, "012-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "012-");
                Environment.Exit(0);
            }
        }
    }

}
