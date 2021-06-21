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
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                using (IWebDriver chrome = new ChromeDriver(chromeOptions))
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
                    chrome.FindElement(By.Name("Login[loginid]")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("Login[loginpass]")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "058-");
                    chrome.FindElement(By.Name("login")).Click();
                    Thread.Sleep(8000);

                    /// <summary>
                    /// Check Login
                    /// </summary>
                    string orderCode = string.Empty;
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("お客さまコードを入力してください。") || body.Contains("パスワードを入力してください。") || body.Contains("お客さまコード、あるいは、パスワードが間違っています。入力内容をご確認ください。"))
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
                            chrome.Navigate().GoToUrl("https://www.rinei-web.jp/shop/item_code/");
                            int Lastrow = dt058.Rows.Count;
                            for (i = 0; i < Lastrow; i++)
                            {
                                if (i < Lastrow)
                                {
                                    ordercode = dt058.Rows[i]["発注コード"].ToString();
                                    chrome.FindElement(By.Id("ItemCode_cdgds")).Clear();
                                    chrome.FindElement(By.Id("ItemCode_cdgds")).SendKeys(ordercode);
                                    try
                                    {
                                        chrome.FindElement(By.Id("button_kensaku")).Click();
                                    }
                                    catch
                                    {
                                        Thread.Sleep(4000);
                                        chrome.FindElement(By.Id("ItemCode_cdgds")).Clear();
                                        chrome.FindElement(By.Id("ItemCode_cdgds")).SendKeys(ordercode);
                                        chrome.FindElement(By.Id("button_kensaku")).Click();
                                    }
                                    Thread.Sleep(4000);

                                    entity = new Qbei_Entity();
                                    entity.siteID = 58;
                                    entity.sitecode = "058";
                                    entity.janCode = dt058.Rows[i]["JANコード"].ToString();
                                    entity.partNo = dt058.Rows[i]["自社品番"].ToString();
                                    entity.makerDate = fun.getCurrentDate();
                                    entity.reflectDate = dt058.Rows[i]["最終反映日"].ToString();
                                    entity.orderCode = dt058.Rows[i]["発注コード"].ToString().Trim();
                                    entity.purchaseURL = "https://www.rinei-web.jp/shop/item_code/";

                                    if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                    {
                                        body = chrome.FindElement(By.TagName("body")).Text;
                                        if (body.Contains("該当する商品はありません。"))
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
                                            entity.price = chrome.FindElement(By.Id("ICInputSuryo_tanka1")).GetAttribute("value").Replace(",", "").Trim();
                                            if (entity.price == "")
                                            {
                                                entity.price = dt058.Rows[i]["下代"].ToString();
                                            }
                                            qty = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[1]/div[2]/div[3]/form/table[3]/tbody/tr[2]/td[3]")).Text;
                                            strStockDate = qty;
                                            if (qty.Contains("月"))
                                            {
                                                string[] stringSeparators = new string[] { "\r\n" };
                                                string[] lines = qty.Split(stringSeparators, StringSplitOptions.None);
                                                var check_month = lines[1].Split('月');
                                                Month = Convert.ToInt32(check_month[0]);
                                                Year = DateTime.Now.ToString("yyyy");
                                                Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();
                                                entity.True_Quantity = lines[0];
                                                entity.True_StockDate = lines[1];
                                                if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                                {
                                                    entity.stockDate = Year + "-" + Month + "-" + "10";
                                                }
                                                else if (strStockDate.Contains("中旬") || strStockDate.Contains("中") || strStockDate.Contains("中頃"))
                                                {
                                                    entity.stockDate = Year + "-" + Month + "-" + "20";
                                                }
                                                else if (strStockDate.Contains("下旬") || strStockDate.Contains("末頃") || strStockDate.Contains("末") || strStockDate.Contains("下"))
                                                {
                                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                                }
                                                else
                                                {
                                                    entity.stockDate = Year + "-" + Month + "-" + Day;
                                                }

                                            }
                                            entity.qtyStatus = qty.Contains("○") ? "good" : qty.Contains("△") ? "small" : qty.Contains("×") || qty.Contains("入荷待ち") || qty.Contains("廃番") ? "empty" : qty.Contains("取り寄せ") ? "inquily" : "unknown status";
                                            if (entity.stockDate == null || entity.stockDate == " ")
                                            {
                                                entity.stockDate = qty.Contains("○") || qty.Contains("最少") ? "2100-01-01" : qty.Contains("△") || qty.Contains("入荷待ち") || qty.Contains("×") || qty.Contains("取り寄せ") ? "2100-01-01" : qty.Contains("廃番") ? "2100-02-01" : "unknown status";
                                                entity.True_Quantity = qty;
                                                entity.True_StockDate = "項目無し";
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
