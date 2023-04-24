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

namespace _016ライトウェイ
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
        DataTable dt016 = new DataTable();
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
            entitySetting.site = 16;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(16);
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
                fun.deleteData(16);
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
            DataTable dt016 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("016");
                fun.Qbei_Delete(16);
                fun.Qbei_ErrorDelete(16);
                dt016 = fun.GetDatatable("016");
                fun.GetTotalCount("016");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "016-");
                Environment.Exit(0);
            }
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
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    Qbei_Entity entity = new Qbei_Entity();
                    string strStockDate = string.Empty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 16;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("pw")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "016-");
                    chrome.FindElement(By.XPath("/html/body/div/div[1]/div/form/div/div[8]/button")).Click();
                    Thread.Sleep(2000);
                    
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ログインできません。ログインID、パスワードを確認してください。"))
                    {
                        fun.WriteLog("Login Failed", "016-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "016-");
                    }


                    try
                    {
                        int Lastrow = dt016.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt016.Rows[i]["発注コード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.XPath("/html/body/div[1]/header[1]/nav/div/form/div/input[2]")).Clear();
                                    chrome.FindElement(By.XPath("/html/body/div[1]/header[1]/nav/div/form/div/input[2]")).SendKeys(ordercode);
                                    chrome.FindElement(By.XPath("/html/body/div[1]/header[1]/nav/div/form/p/button")).Click();
                                }
                                catch
                                {
                                    Thread.Sleep(6000);
                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;
                                    if (Check_Message.Contains("検索条件に該当するデータは存在しません。"))
                                    {
                                        chrome.FindElement(By.CssSelector("body > div.bootbox.modal.fade.bootbox-alert.show > div > div > div.modal-footer > button")).Click();
                                    }
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).Clear();
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).SendKeys(ordercode);
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[5]/div/button[1]")).Click();
                                }
                                Thread.Sleep(2000);
                                entity = new Qbei_Entity();
                                entity.siteID = 16;
                                entity.sitecode = "016";
                                entity.janCode = dt016.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt016.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt016.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt016.Rows[i]["発注コード"].ToString();


                                //<remark>
                                //Check to Ordercode
                                //</remark>
                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Message = chrome.FindElement(By.TagName("body")).Text;
                                    if (Message.Contains("検索結果がありませんでした。検索条件をお確かめください。"))
                                    {                                        
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;  //"https://rpj-ec.com/aec/user/shohin_list";  2023/04/24 updated by ct
                                        entity.price = dt016.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/article/section/div[2]/div[3]/div/div/div/div/p[1]/a")).Click();
                                        }
                                        catch
                                        {
                                            Thread.Sleep(6000);
                                            chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/div[2]/article/section/div[2]/div[3]/div/div/div/div/p[1]/a")).Click();
                                        }
                                        int n = chrome.FindElements(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr")).Count();
                                        for (int i = 1; i <= n; i++)
                                        {
                                            if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr[" + (i) + "]/td[2]/div")).Text.Contains(entity.janCode))
                                            {
                                                entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr[" + (i) + "]/td[7]/div/div/div/p[2]/span[2]")).Text.Replace("円", "").Replace(",", "").Trim();
                                                entity.purchaseURL = chrome.Url;  //"https://rpj-ec.com/aec/user/shohin_list"; 2023/04/24 updated by ct
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr[" + (i) + "]/td[6]/div/div/div/span/span[1]")).Text;
                                                entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : "unknown status";
                                                entity.True_Quantity = stock;
                                                try
                                                {
                                                    if (!chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr[" + (i) + "]/td[6]/div/div/div/span/span[2]")).Text.Equals(""))
                                                    {
                                                        string str_stockdate = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div/form/div/article/section/div/div/div[5]/table/tbody/tr[" + (i) + "]/td[6]/div/div/div/span/span[2]")).Text;
                                                        entity.True_StockDate = str_stockdate;
                                                        if (str_stockdate.Equals("未定"))
                                                        {
                                                            entity.stockDate = "2100-01-01";
                                                        }
                                                        else
                                                        {
                                                            string[] year_month_split = str_stockdate.Split('/');
                                                            int Year = Convert.ToInt32(year_month_split[0]);
                                                            int Month = Convert.ToInt32(string.Concat(year_month_split[1].Where(char.IsNumber)));
                                                            string Day = DateTime.DaysInMonth(Year, Month).ToString();
                                                            if (str_stockdate.Contains("上旬"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + "10";
                                                            }
                                                            else if (str_stockdate.Contains("中旬"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + "20";
                                                            }
                                                            else if (str_stockdate.Contains("下旬"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown date";
                                                    entity.True_StockDate = "項目無し";                                                   
                                                }
                                                break;
                                            }                                            
                                        }
                                        if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt016.Rows[i]["下代"].ToString();
                                            entity.purchaseURL = chrome.Url;  //"https://rpj-ec.com/aec/user/shohin_list";  2023/04/24 updated by ct
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = "Not Found";
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
                                                fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "entity.qtyStatus is null!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "entity.qtyStatus is unknown status!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "0");
                                        }
                                    }
                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                                }
                            }
                        }
                        qe.site = 16;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt016.Rows[i]["JANコード"].ToString();
                        ordercode = dt016.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(16, fun.GetSiteName("016"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "016");
                        fun.WriteLog(ex, "016-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "016-");
                Environment.Exit(0);
            }
        }
    }
}

