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

namespace _145
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt145 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        public static string ordercode;
        public static string jancode;

        static void Main(string[] args)
        {
            testflag();
        }
        public static void testflag()
        {
            Qbeisetting_Entity entitySetting = new Qbeisetting_Entity();
            DataTable dtSetting = new DataTable();
            int intFlag;
            entitySetting.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entitySetting.site = 145;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(145);
            intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());


            if (intFlag == 0)
            {
                fun.ChangeFlag(entitySetting);
                StartRun();
            }

            else if (intFlag == 1)
            {
                fun.deleteData(145);
                fun.ChangeFlag(entitySetting);
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public static void StartRun()
        {
            DataTable dt145 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("145");
                fun.Qbei_Delete(145);
                fun.Qbei_ErrorDelete(145);
                dt145 = fun.GetDatatable("145");
                fun.GetTotalCount("145");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "145-");
                Environment.Exit(0);
            }
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);
                chromeOptions.AddArguments("-no-sandbox");
                chromeOptions.AddArgument("--start-maximized");
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
                    qe.SiteID = 145;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Id("ctl00_ContentPlaceHolder1_tbLoginIdInMailAddr")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Id("ctl00_ContentPlaceHolder1_tbPassword")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "145-");
                    chrome.FindElement(By.Id("ctl00_ContentPlaceHolder1_lbLogin")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ユーザログインに失敗しました。"))
                    {
                        fun.WriteLog("Login Failed", "145-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "145-");
                    }


                    try
                    {
                        int Lastrow = dt145.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                jancode = dt145.Rows[i]["JANコード"].ToString();   //ordercode = dt145.Rows[i]["発注コード"].ToString();
                                string code = jancode.TrimStart('0');

                                try
                                {
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_tbSearchWord")).Clear();
                                    Thread.Sleep(500);
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_tbSearchWord")).SendKeys(code);
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_lbSearch")).Click();
                                    Thread.Sleep(1000);
                                }
                                catch
                                {
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_tbSearchWord")).Clear();
                                    Thread.Sleep(1000);
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_tbSearchWord")).SendKeys(code);
                                    Thread.Sleep(1500);
                                    chrome.FindElement(By.Id("ctl00_BodyHeaderMain_lbSearch")).Click();
                                    Thread.Sleep(4000);
                                }

                                entity = new Qbei_Entity();
                                entity.siteID = 145;
                                entity.sitecode = "145";
                                entity.janCode = dt145.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt145.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt145.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt145.Rows[i]["発注コード"].ToString();



                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    Thread.Sleep(1000);
                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Check_Message.Contains("該当する商品がありません。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt145.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }

                                    else if (Check_Message.Contains("全1件"))
                                    {
                                        chrome.FindElement(By.CssSelector(".common__item-list__name")).Click();

                                        try
                                        {
                                            if (chrome.FindElement(By.CssSelector(".add-title__page")).Text == "エラー情報")
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.purchaseURL = chrome.Url;
                                                entity.price = dt145.Rows[i]["下代"].ToString();
                                                entity.True_StockDate = "Not Found";
                                                entity.True_Quantity = "Not Found";
                                                fun.Qbei_Inserts(entity);

                                            }
                                        }

                                        catch
                                        {
                                            if (chrome.FindElement(By.CssSelector(".shosai1_area > p:nth-child(1) > span:nth-child(3)")).Text.Replace("JANコード：", string.Empty) == entity.janCode)
                                            {
                                                string Check_Message1 = chrome.FindElement(By.TagName("table")).Text;

                                                if (!Check_Message1.Contains("カートに入れる"))
                                                {
                                                    entity.qtyStatus = "empty";
                                                    entity.stockDate = "2100-02-01";
                                                    entity.purchaseURL = chrome.Url;
                                                    entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                    entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                    entity.True_StockDate = "Not Found";
                                                    entity.True_Quantity = "Not Found";
                                                    fun.Qbei_Inserts(entity);

                                                }

                                                else
                                                {
                                                    if (Check_Message1.Contains("会員ランク"))
                                                    {
                                                        entity.price = chrome.FindElement(By.CssSelector("p.productPrice:nth-child(2)")).Text;
                                                        entity.price = entity.price.Replace("会員ランク 下代(税抜):¥", string.Empty).Replace(",", string.Empty);
                                                    }
                                                    else
                                                    {
                                                        entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                        entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                    }

                                                    qty = chrome.FindElement(By.CssSelector(".productStockTextWrap")).Text;
                                                    entity.qtyStatus = qty.Contains("在庫あり") ? "good" : qty.Contains("在庫わずか") ? "small" : qty.Contains("完売") || qty.Contains("未定") ? "empty" : "invalid status code";
                                                    entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                    entity.True_Quantity = qty;

                                                    if (qty.Contains("年") && qty.Contains("月"))
                                                    {
                                                        entity.qtyStatus = "empty";
                                                        entity.stockDate = qty.Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "30").Replace("入荷予定時期：", String.Empty).Replace("年", "-").Replace("月", "-").Replace("日", String.Empty);
                                                        DateTime d = Convert.ToDateTime(entity.stockDate);
                                                        entity.stockDate = d.ToString("yyyy-MM-dd");
                                                    }
                                                    entity.True_StockDate = "項目無し";
                                                    entity.purchaseURL = chrome.Url;
                                                    fun.Qbei_Inserts(entity);
                                                }
                                            }

                                            else
                                            {
                                                try
                                                {
                                                    int c = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/form/div[4]/div/div/div/table/tbody/tr/td[2]/div[2]/div/div[2]/div[1]/div[1]/div[2]/div[3]/div/div[1]/div/div/div")).Count());

                                                    for (int i = 1; i <= c; i++)
                                                    {
                                                        Thread.Sleep(500);
                                                        chrome.FindElement(By.CssSelector("div.selectVariation__item:nth-child(" + (i) + ")")).Click();
                                                        Thread.Sleep(500);
                                                        string janCode = chrome.FindElement(By.CssSelector(".shosai1_area > p:nth-child(1) > span:nth-child(3)")).Text.Replace("JANコード：", string.Empty);
                                                        Thread.Sleep(500);

                                                        if (janCode.Contains(code))
                                                        {
                                                            string Check_Message1 = chrome.FindElement(By.TagName("table")).Text;

                                                            if (!Check_Message1.Contains("カートに入れる"))
                                                            {
                                                                entity.qtyStatus = "empty";
                                                                entity.stockDate = "2100-02-01";
                                                                entity.purchaseURL = chrome.Url;
                                                                entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                                entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                                entity.True_StockDate = "Not Found";
                                                                entity.True_Quantity = "Not Found";
                                                                fun.Qbei_Inserts(entity);
                                                                break;
                                                            }

                                                            else
                                                            {
                                                                Thread.Sleep(500);
                                                                if (Check_Message1.Contains("会員ランク"))
                                                                {
                                                                    entity.price = chrome.FindElement(By.CssSelector("p.productPrice:nth-child(2)")).Text;
                                                                    entity.price = entity.price.Replace("会員ランク 下代(税抜):¥", string.Empty).Replace(",", string.Empty);
                                                                }
                                                                else
                                                                {
                                                                    entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                                    entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                                }

                                                                qty = chrome.FindElement(By.CssSelector(".productStockTextWrap")).Text;
                                                                entity.qtyStatus = qty.Contains("在庫あり") ? "good" : qty.Contains("在庫わずか") ? "small" : qty.Contains("完売") || qty.Contains("未定") ? "empty" : "invalid status code";
                                                                entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                                entity.True_Quantity = qty;

                                                                if (qty.Contains("年") && qty.Contains("月"))
                                                                {
                                                                    entity.qtyStatus = "empty";
                                                                    entity.stockDate = qty.Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "30").Replace("入荷予定時期：", String.Empty).Replace("年", "-").Replace("月", "-").Replace("日", String.Empty);
                                                                    DateTime d = Convert.ToDateTime(entity.stockDate);
                                                                    entity.stockDate = d.ToString("yyyy-MM-dd");
                                                                }
                                                                entity.True_StockDate = "項目無し";
                                                                entity.purchaseURL = chrome.Url;
                                                                fun.Qbei_Inserts(entity);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    entity.qtyStatus = "empty";
                                                    entity.stockDate = "2100-02-01";
                                                    entity.price = dt145.Rows[i]["下代"].ToString();
                                                    entity.purchaseURL = chrome.Url;
                                                    entity.True_StockDate = "Not Found";
                                                    entity.True_Quantity = "Not Found";
                                                    fun.Qbei_Inserts(entity);
                                                }
                                            }
                                        }
                                    }

                                    else
                                    {
                                        chrome.FindElement(By.XPath("/html/body/form/div[4]/div/div/div/div[2]/div/div/div/div[2]/div[2]/div[2]/div[1]/div[2]/div/div/div[1]/ul/li[2]/a/p")).Click();

                                        if (chrome.FindElement(By.CssSelector(".shosai1_area > p:nth-child(1) > span:nth-child(3)")).Text.Replace("JANコード：", string.Empty) == entity.janCode)
                                        {
                                            string Check_Message1 = chrome.FindElement(By.TagName("table")).Text;

                                            if (!Check_Message1.Contains("カートに入れる"))
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.purchaseURL = chrome.Url;
                                                entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                entity.True_StockDate = "Not Found";
                                                entity.True_Quantity = "Not Found";
                                                fun.Qbei_Inserts(entity);
                                            }

                                            else
                                            {
                                                if (Check_Message1.Contains("会員ランク"))
                                                {
                                                    entity.price = chrome.FindElement(By.CssSelector("p.productPrice:nth-child(2)")).Text;
                                                    entity.price = entity.price.Replace("会員ランク 下代(税抜):¥", string.Empty).Replace(",", string.Empty);
                                                }
                                                else
                                                {
                                                    entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                    entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                }

                                                qty = chrome.FindElement(By.CssSelector(".productStockTextWrap")).Text;
                                                entity.qtyStatus = qty.Contains("在庫あり") ? "good" : qty.Contains("在庫わずか") ? "small" : qty.Contains("完売") || qty.Contains("未定") ? "empty" : "invalid status code";
                                                entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                entity.True_Quantity = qty;

                                                if (qty.Contains("年") && qty.Contains("月"))
                                                {
                                                    entity.qtyStatus = "empty";
                                                    entity.stockDate = qty.Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "30").Replace("入荷予定時期：", String.Empty).Replace("年", "-").Replace("月", "-").Replace("日", String.Empty);
                                                    DateTime d = Convert.ToDateTime(entity.stockDate);
                                                    entity.stockDate = d.ToString("yyyy-MM-dd");
                                                }
                                                entity.True_StockDate = "項目無し";
                                                entity.purchaseURL = chrome.Url;
                                                fun.Qbei_Inserts(entity);
                                            }
                                        }

                                        else
                                        {
                                            int c = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/form/div[4]/div/div/div/table/tbody/tr/td[2]/div[2]/div/div[2]/div[1]/div[1]/div[2]/div[3]/div/div[1]/div/div/div")).Count());

                                            for (int i = 1; i <= c; i++)
                                            {
                                                chrome.FindElement(By.CssSelector("div.selectVariation__item:nth-child(" + (i) + ")")).Click();

                                                Thread.Sleep(500);
                                                string janCode = chrome.FindElement(By.CssSelector(".shosai1_area > p:nth-child(1) > span:nth-child(3)")).Text.Replace("JANコード：", string.Empty);

                                                if (janCode.Contains(code)) //janCode == entity.janCode
                                                {
                                                    string Check_Message1 = chrome.FindElement(By.TagName("table")).Text;

                                                    if (!Check_Message1.Contains("カートに入れる"))
                                                    {
                                                        entity.qtyStatus = "empty";
                                                        entity.stockDate = "2100-02-01";
                                                        entity.purchaseURL = chrome.Url;
                                                        entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                        entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                        entity.True_StockDate = "Not Found";
                                                        entity.True_Quantity = "Not Found";
                                                        fun.Qbei_Inserts(entity);
                                                        break;
                                                    }

                                                    else
                                                    {
                                                        if (Check_Message1.Contains("会員ランク"))
                                                        {
                                                            entity.price = chrome.FindElement(By.CssSelector("p.productPrice:nth-child(2)")).Text;
                                                            entity.price = entity.price.Replace("会員ランク 下代(税抜):¥", string.Empty).Replace(",", string.Empty);
                                                        }
                                                        else
                                                        {
                                                            entity.price = chrome.FindElement(By.CssSelector("p.productPrice > span:nth-child(1)")).Text;
                                                            entity.price = entity.price.Replace("¥", string.Empty).Replace(",", string.Empty);
                                                        }

                                                        qty = chrome.FindElement(By.CssSelector(".productStockTextWrap")).Text;
                                                        entity.qtyStatus = qty.Contains("在庫あり") ? "good" : qty.Contains("在庫わずか") ? "small" : qty.Contains("完売") || qty.Contains("未定") ? "empty" : "invalid status code";
                                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                        entity.True_Quantity = qty;

                                                        if (qty.Contains("年") && qty.Contains("月"))
                                                        {
                                                            entity.qtyStatus = "empty";
                                                            entity.stockDate = qty.Replace("上旬", "10").Replace("中旬", "20").Replace("下旬", "30").Replace("入荷予定時期：", String.Empty).Replace("年", "-").Replace("月", "-").Replace("日", String.Empty);
                                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                                            entity.stockDate = d.ToString("yyyy-MM-dd");
                                                        }
                                                        entity.True_StockDate = "項目無し";
                                                        entity.purchaseURL = chrome.Url;
                                                        fun.Qbei_Inserts(entity);
                                                        break;

                                                    }
                                                }
                                            }
                                        }
                                    }


                                    if (entity.price == null || entity.qtyStatus == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt145.Rows[i]["下代"].ToString();
                                        entity.purchaseURL = chrome.Url;
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }

                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                                }
                            }
                        }
                        qe.site = 145;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt145.Rows[i]["JANコード"].ToString();
                        ordercode = dt145.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), ex.Message, janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                        fun.WriteLog(ex, "145-", janCode, entity.orderCode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "145-");
                Environment.Exit(0);
            }
        }

    }
}
