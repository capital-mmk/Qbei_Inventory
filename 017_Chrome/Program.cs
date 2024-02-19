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
using OpenQA.Selenium.Remote;


namespace _017_Chrome
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt017 = new DataTable();
        public static Qbeisetting_Entity qe = new Qbeisetting_Entity();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        public static string jancode;
        public static string ordercode;
        static void Main(string[] args)
        {
            testflag();
        }
        public static void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 17;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(17);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());


                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }


                else if (flag == 1)
                {
                    fun.deleteData(17);
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
                fun.WriteLog(ex, "017-");
                Environment.Exit(0);
            }
        }

        public static void StartRun()
        {
            DataTable dt017 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                st = DateTime.Now.ToString();
                fun.setURL("017");
                fun.Qbei_Delete(17);
                fun.Qbei_ErrorDelete(17);
                dt017 = fun.GetDatatable("017");
                fun.GetTotalCount("017");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "017-");
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

                using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(5)))
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    Qbei_Entity entity = new Qbei_Entity();
                    string strStockDate = string.Empty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 17;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    chrome.FindElement(By.XPath("html/body/div[1]/div[2]/div[2]/div/p/a")).Click();
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("passwd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "017-");
                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/form/div/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ログインできません。ログインID、パスワードを確認してください。"))
                    {
                        fun.WriteLog("Login Failed", "017-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success ------", "017-");
                    }

                    try
                    {
                        int Lastrow = dt017.Rows.Count;

                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                jancode = dt017.Rows[i]["JANコード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/div[2]/div/section[2]/ul/li[4]/input")).Clear();
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/div[2]/div/section[2]/ul/li[4]/input")).SendKeys(jancode);
                                    chrome.FindElement(By.XPath(" /html/body/div[1]/div/aside/div[2]/div/section[2]/a")).Click();
                                }
                                catch
                                {
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/div[2]/div/section[2]/ul/li[4]/input")).Clear();
                                    Thread.Sleep(2000);
                                    chrome.FindElement(By.XPath("/html/body/div[1]/div/aside/div[2]/div/section[2]/ul/li[4]/input")).SendKeys(jancode);
                                    chrome.FindElement(By.XPath(" /html/body/div[1]/div/aside/div[2]/div/section[2]/a")).Click();
                                }

                                Thread.Sleep(2000);
                                entity = new Qbei_Entity();
                                entity.siteID = 17;
                                entity.sitecode = "017";
                                entity.janCode = dt017.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt017.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt017.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt017.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.janCode))
                                {
                                    string Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Message.Contains("ご指定の条件に一致する商品が見つかりませんでした"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt017.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/main/section/ul/li/p[2]/a")).Click();
                                        Thread.Sleep(2000);

                                        try
                                        {
                                            entity.price = chrome.FindElement(By.CssSelector(".item-tax > span:nth-child(1)")).Text;
                                            string stock = chrome.FindElement(By.CssSelector(".item-stock")).Text;
                                            entity.qtyStatus = stock.Equals("〇在庫あり") ? "good" : stock.Equals("△残りわずか") ? "small" : "unknown status";
                                            entity.True_Quantity = stock;
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = chrome.Url;
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);
                                            chrome.FindElement(By.XPath("/html/body/div[1]/div[1]/span[1]/a")).Click();
                                        }

                                        catch
                                        {
                                            entity.price = chrome.FindElement(By.CssSelector(".item-tax > span:nth-child(1)")).Text;
                                            string stock = chrome.FindElement(By.CssSelector(".restock-btn")).Text;
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = chrome.Url;
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = "在庫なし";
                                            fun.Qbei_Inserts(entity);
                                            chrome.FindElement(By.XPath("/html/body/div[1]/div[1]/span[1]/a")).Click();
                                        }

                                        if (entity.price == null || entity.qtyStatus == null)
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt017.Rows[i]["下代"].ToString();
                                            entity.purchaseURL = chrome.Url;
                                            entity.True_StockDate = "Not Found";
                                            entity.True_Quantity = "Not Found";
                                        }
                                    }

                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");
                                }
                            }
                        }

                        qe.site = 17;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt017.Rows[i]["JANコード"].ToString();
                        ordercode = dt017.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(17, fun.GetSiteName("017"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "017");
                        fun.WriteLog(ex, "017-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "017-");
                Environment.Exit(0);
            }
        }
    }
}
