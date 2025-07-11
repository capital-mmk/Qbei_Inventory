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
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _110_Chrome
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt110 = new DataTable();
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
            entitySetting.site = 110;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(110);
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
                fun.deleteData(110);
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
            DataTable dt110 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("110");
                fun.Qbei_Delete(110);
                fun.Qbei_ErrorDelete(110);
                dt110 = fun.GetDatatable("110");
                fun.GetTotalCount("110");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "110-");
            }
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";//<Add Logic for Chrome Path 2021/05/24 />
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");//<remark Add Logic for ChormeDriver 2021/09/02 />
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");//<remark Add Logic for ChormeDriver 2021/09/02 />
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false); //<remark Add Logic for ChormeDriver 2025/04/08 />
                chromeOptions.AddArguments("-no-sandbox");//<remark Add Logic for ChormeDriver 2021/09/02 />
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
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 110;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    chrome.FindElement(By.XPath("/html/body/center/div/div[1]/div/div/div[2]/ul/li/a")).Click();
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("passwd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "110-");
                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/form/div/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("IDかパスワードが正しくありません"))
                    {
                        fun.WriteLog("Login Failed", "110-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "110-");
                    }


                    // start  change logic  10/01/2025 
                    try
                    {
                        int Lastrow = dt110.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt110.Rows[i]["発注コード"].ToString();

                                try
                                {
                                    chrome.FindElement(By.ClassName("search_input")).SendKeys(ordercode);
                                    Thread.Sleep(1000);
                                }
                                catch
                                {
                                    Thread.Sleep(4000);
                                    chrome.FindElement(By.ClassName("search_input")).SendKeys(ordercode);
                                }

                                try
                                {
                                    chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[1]/div/div[1]/div[1]/div/a/img")).Click();
                                }
                                catch
                                {
                                    chrome.FindElement(By.XPath("/html/body/center/div/div[2]/div[2]/table/tbody/tr[1]/td[1]/div/div[1]/div[1]/div/a/img")).Click();
                                }

                                entity = new Qbei_Entity();
                                entity.siteID = 110;
                                entity.sitecode = "110";
                                entity.janCode = dt110.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt110.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt110.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt110.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    if (chrome.FindElement(By.Id("r_resultInfo")).GetAttribute("innerHTML").Contains("全1件"))
                                    {
                                        chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[4]/ul/li/div/div[1]/a/img")).Click();
                                        Thread.Sleep(1000);

                                        try
                                        {
                                            Thread.Sleep(2000);
                                            if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody")).Text.Contains(entity.orderCode))
                                            {
                                                int c = chrome.FindElements(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr")).Count();
                                                for (int i = 1; i <= c; i++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/th")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/td[1]/span")).Text;
                                                        entity.price = entity.price.Replace(",", string.Empty);

                                                        string stock = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/td[2]")).Text;
                                                        entity.True_Quantity = stock;
                                                        entity.qtyStatus = stock.Contains("○") || stock.Contains("〇") ? "good" : stock.Contains("個") && stock.Contains("残り僅か") ? "small" : stock.Contains("個") && stock.Contains("入荷予定") ? "small" : stock.Contains("在庫在") || stock.Contains("在庫有") ? "small" : stock.Contains("在庫なし") ? "empty" : "unknown status";

                                                        if (stock.Contains("入荷予定"))
                                                        {
                                                            entity.stockDate = stock.Replace("上旬", "10").Replace("入荷予定", String.Empty).Replace("/", "-").Replace("年", "-").Replace("月", "-").Replace("日", String.Empty).Replace("在庫なし ", String.Empty); // .Replace("個", String.Empty) .Replace(" ", String.Empty)

                                                            if (entity.stockDate.Contains("個"))
                                                            {
                                                                string Item = entity.stockDate;
                                                                int item = entity.stockDate.IndexOf('個');
                                                                string CutItem = Item.Substring(item, Item.Length - item).Replace("個", String.Empty);

                                                                DateTime da = Convert.ToDateTime(CutItem);
                                                                entity.stockDate = da.ToString("yyyy-MM-dd");
                                                            }

                                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                                            entity.stockDate = d.ToString("yyyy-MM-dd");
                                                        }

                                                        else
                                                        {
                                                            entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";

                                                        }

                                                        entity.True_StockDate = "Not Found";
                                                        string current_url = chrome.Url;
                                                        entity.purchaseURL = current_url;
                                                        fun.Qbei_Inserts(entity);

                                                        break;
                                                    }
                                                }
                                            }


                                            else
                                            {
                                                fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "Order Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                            }
                                        }


                                        catch
                                        {
                                            int c = chrome.FindElements(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[5]/td/div/table[2]/tbody/tr")).Count(); ///tr
                                            if (c != 0)
                                            {
                                                for (int i = 1; i <= c; i++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[5]/td/div/table[2]/tbody/tr[" + (i) + "]/th")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[5]/td/div/table[2]/tbody/tr[" + (i) + "]/td[1]/span")).Text;
                                                        entity.price = entity.price.Replace(",", string.Empty);

                                                        string stock = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[5]/td/div/table[2]/tbody/tr[" + (i) + "]/td[2]")).Text;
                                                        entity.True_Quantity = stock;
                                                        entity.qtyStatus = stock.Contains("○") || stock.Contains("〇") ? "good" : stock.Contains("個") && stock.Contains("残り僅か") ? "small" : stock.Contains("個") && stock.Contains("入荷予定") ? "small" : stock.Contains("在庫在") || stock.Contains("在庫有") ? "small" : stock.Contains("在庫なし") ? "empty" : "unknown status";

                                                        if (stock.Contains("入荷予定"))
                                                        {
                                                            entity.stockDate = stock.Replace("上旬", "10").Replace("入荷予定", String.Empty).Replace("/", "-").Replace("年", "-").Replace("月", "-").Replace("日", String.Empty).Replace("在庫なし ", String.Empty); // .Replace("個", String.Empty) .Replace(" ", String.Empty)

                                                            if (entity.stockDate.Contains("個"))
                                                            {
                                                                string Item = entity.stockDate;
                                                                int item = entity.stockDate.IndexOf('個');
                                                                string CutItem = Item.Substring(item, Item.Length - item).Replace("個", String.Empty);

                                                                DateTime da = Convert.ToDateTime(CutItem);
                                                                entity.stockDate = da.ToString("yyyy-MM-dd");
                                                            }
                                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                                            entity.stockDate = d.ToString("yyyy-MM-dd");

                                                        }

                                                        else
                                                        {
                                                            entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";

                                                        }

                                                        entity.True_StockDate = "Not Found";
                                                        string current_url = chrome.Url;
                                                        entity.purchaseURL = current_url;
                                                        fun.Qbei_Inserts(entity);

                                                        break;
                                                    }
                                                }
                                            }

                                            else
                                            {
                                                int a = chrome.FindElements(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[7]/td/div/table[2]/tbody/tr")).Count();
                                                for (int i = 1; i <= a; i++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[7]/td/div/table[2]/tbody/tr[" + (i) + "]/th")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[7]/td/div/table[2]/tbody/tr[" + (i) + "]/td[1]/span")).Text;
                                                        entity.price = entity.price.Replace(",", string.Empty);

                                                        string stock = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[7]/td/div/table[2]/tbody/tr[" + (i) + "]/td[2]")).Text;
                                                        entity.True_Quantity = stock;
                                                        entity.qtyStatus = stock.Contains("○") || stock.Contains("〇") ? "good" : stock.Contains("個") && stock.Contains("残り僅か") ? "small" : stock.Contains("個") && stock.Contains("入荷予定") ? "small" : stock.Contains("在庫在") || stock.Contains("在庫有") ? "small" : stock.Contains("在庫なし") ? "empty" : "unknown status";

                                                        if (stock.Contains("入荷予定"))
                                                        {
                                                            entity.stockDate = stock.Replace("上旬", "10").Replace("入荷予定", String.Empty).Replace("/", "-").Replace("年", "-").Replace("月", "-").Replace("日", String.Empty).Replace("在庫なし ", String.Empty); // .Replace("個", String.Empty) .Replace(" ", String.Empty)

                                                            if (entity.stockDate.Contains("個"))
                                                            {
                                                                string Item = entity.stockDate;
                                                                int item = entity.stockDate.IndexOf('個');
                                                                string CutItem = Item.Substring(item, Item.Length - item).Replace("個", String.Empty);

                                                                DateTime da = Convert.ToDateTime(CutItem);
                                                                entity.stockDate = da.ToString("yyyy-MM-dd");
                                                            }
                                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                                            entity.stockDate = d.ToString("yyyy-MM-dd");

                                                        }

                                                        else
                                                        {
                                                            entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";

                                                        }

                                                        entity.True_StockDate = "Not Found";
                                                        string current_url = chrome.Url;
                                                        entity.purchaseURL = current_url;
                                                        fun.Qbei_Inserts(entity);

                                                        break;
                                                    }
                                                }
                                            }

                                        }

                                    }

                                    else if (chrome.FindElement(By.CssSelector("#r_searchList")).Text.Contains(entity.orderCode))
                                    {
                                        string n = chrome.FindElement(By.CssSelector(".total")).Text.Replace("全", string.Empty).Replace("件", string.Empty);
                                        int m = Convert.ToInt32(n);

                                        for (int j = 1; j <= m; j++)
                                        {

                                            if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[4]/ul/li[" + (j) + "]/div/div[2]/div[1]/ul/li[3]")).Text.Contains(entity.orderCode))
                                            {
                                                chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[4]/ul/li[" + (j) + "]/div/div[2]/p[1]/a")).Click();
                                                goto label;
                                            }

                                        }
                                    label:

                                        Thread.Sleep(2000);
                                        if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody")).Text.Contains(entity.orderCode))
                                        {
                                            int c = chrome.FindElements(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr")).Count();
                                            for (int i = 1; i <= c; i++)
                                            {
                                                if (chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/th")).Text.Contains(entity.orderCode))
                                                {
                                                    entity.price = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/td[1]/span")).Text;
                                                    entity.price = entity.price.Replace(",", string.Empty);

                                                    string stock = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[1]/div[2]/div[1]/table/tbody/tr[6]/td/div/table[2]/tbody/tr[" + (i) + "]/td[2]")).Text;
                                                    entity.True_Quantity = stock;
                                                    entity.qtyStatus = stock.Contains("○") || stock.Contains("〇") ? "good" : stock.Contains("個") && stock.Contains("残り僅か") ? "small" : stock.Contains("個") && stock.Contains("入荷予定") ? "small" : stock.Contains("在庫在") || stock.Contains("在庫有") ? "small" : stock.Contains("在庫なし") ? "empty" : "unknown status";

                                                    if (stock.Contains("入荷予定"))
                                                    {
                                                        entity.stockDate = stock.Replace("上旬", "10").Replace("入荷予定", String.Empty).Replace("/", "-").Replace("年", "-").Replace("月", "-").Replace("日", String.Empty).Replace("在庫なし ", String.Empty); // .Replace("個", String.Empty) .Replace(" ", String.Empty)

                                                        if (entity.stockDate.Contains("個"))
                                                        {
                                                            string Item = entity.stockDate;
                                                            int item = entity.stockDate.IndexOf('個');
                                                            string CutItem = Item.Substring(item, Item.Length - item).Replace("個", String.Empty);

                                                            DateTime da = Convert.ToDateTime(CutItem);
                                                            entity.stockDate = da.ToString("yyyy-MM-dd");
                                                        }

                                                        DateTime d = Convert.ToDateTime(entity.stockDate);
                                                        entity.stockDate = d.ToString("yyyy-MM-dd");
                                                    }

                                                    else
                                                    {
                                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";

                                                    }

                                                    entity.True_StockDate = "Not Found";
                                                    string current_url = chrome.Url;
                                                    entity.purchaseURL = current_url;
                                                    fun.Qbei_Inserts(entity);

                                                    break;
                                                }
                                            }

                                        }


                                        else
                                        {
                                            fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "Order Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                        }



                                    }

                                    else
                                    {
                                        fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "Order Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                    }

                                }


                                else
                                {
                                    fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "Order Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                }

                            }

                        }
                        qe.site = 110;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt110.Rows[i]["JANコード"].ToString();
                        ordercode = dt110.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                        fun.WriteLog(ex, "110-", janCode, ordercode);
                    }
                    // end  change logic  10/01/2025 
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "110-");
                Environment.Exit(0);
            }
        }
    }
}
