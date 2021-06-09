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
                        int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                        qe.SiteID =110;
                        dt = qubl.Qbei_Setting_Select(qe);
                        fun.url = dt.Rows[0]["Url"].ToString();

                        chrome.Url = fun.url;
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


                        try
                        {
                            int Lastrow = dt110.Rows.Count;
                            for (i = 0; i < Lastrow; i++)
                            {
                            if (i < Lastrow)
                            {
                                ordercode = dt110.Rows[i]["JANコード"].ToString();
                                chrome.FindElement(By.ClassName("search_input")).SendKeys(ordercode);
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
                                entity.purchaseURL = "https://btob.asahi-wsd.jp/website/asahi/product/list";

                                //<remark>
                                //Check to Ordercode
                                //</remark>
                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    if (chrome.FindElement(By.Id("r_resultInfo")).GetAttribute("innerHTML").Contains("全0件"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt110.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/div[4]/ul/li/div/div[1]/a/img")).Click();
                                        entity.price = chrome.FindElement(By.Id("M_member_notaxprice")).GetAttribute("value").ToString().Replace("¥", "").Replace(",", "").Trim();
                                        try
                                        {
                                            string stock = chrome.FindElement(By.ClassName("M_item-stock-instock")).Text.Trim();
                                            entity.qtyStatus = stock.Equals("○在庫あり") ? "good" : stock.Equals("△残りわずか") ? "small" : "unknown status";
                                            entity.True_StockDate = "項目無し";
                                            entity.True_Quantity = stock;
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                Thread.Sleep(4000);
                                                string stock = chrome.FindElement(By.ClassName("M_item-stock-smallstock")).Text.Trim();
                                                entity.qtyStatus = stock.Equals("○在庫あり") ? "good" : stock.Equals("△残りわずか") ? "small" : "unknown status";
                                                entity.True_StockDate = "項目無し";
                                                entity.True_Quantity = stock;
                                            }
                                            catch
                                            {
                                                string stock = chrome.FindElement(By.ClassName("M_item-stock-smallstock")).Text.Trim();
                                                entity.qtyStatus = "empty";
                                                entity.True_StockDate = "項目無し";
                                                entity.True_Quantity = stock;
                                            }
                                        }
                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") || entity.qtyStatus.Equals("inquiry") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                        string check = chrome.FindElement(By.XPath("/html/body/center/center/div/div[2]/div[2]/table/tbody/tr[1]/td[3]/form[2]/div/h1")).Text;
                                        if (check.Contains("【") & check.Contains("】"))
                                        {
                                            var sp_first = check.Split('】').ToArray();
                                            string word = sp_first[0];
                                            var sp_second = word.Split('【').ToArray();
                                            string word2 = sp_second[1];
                                            string day;
                                            string month;
                                            string year;
                                            entity.True_StockDate = word2;
                                            if (word2.Contains("-") & word2.Contains("日"))
                                            {
                                                var sp_third = word.Split('-').ToArray();
                                                sp_third = sp_third[1].Split('日').ToArray();
                                                day = sp_third[0];
                                                month = DateTime.Now.ToString("MM");
                                                year = DateTime.Now.ToString("yyyy");
                                                entity.stockDate = year + "-" + month + "-" + day;
                                            }

                                            else
                                            {
                                                entity.stockDate = "2100-01-01";
                                                entity.True_StockDate = "Not Found";
                                            }

                                        }
                                        //else
                                        //{
                                        //    entity.stockDate = "2100-01-01";
                                        //    entity.True_StockDate = "Not Found";
                                        //}                                        
                                        if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt110.Rows[i]["下代"].ToString();
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
                                                fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "entity.qtyStatus is null!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "entity.qtyStatus is unknown status!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
                                        }
                                    }
                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(110, fun.GetSiteName("110"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "110");
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
