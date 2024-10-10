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
using System;

namespace _024_Chrome
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt024 = new DataTable();
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
                qe.site = 24;
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(24);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());


                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }


                else if (flag == 1)
                {
                    fun.deleteData(24);
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
                fun.WriteLog(ex, "024-");
                Environment.Exit(0);
            }
        }

        public static void StartRun()
        {
            DataTable dt024 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("024");
                fun.Qbei_Delete(24);
                fun.Qbei_ErrorDelete(24);
                dt024 = fun.GetDatatable("024");
                fun.GetTotalCount("024");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "024-");
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
                    qe.SiteID = 24;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(2000);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("data[MemberLogin][id]")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("data[MemberLogin][passwd]")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "024-");
                    chrome.FindElement(By.CssSelector("#btn")).Click();

                    string alert = chrome.FindElement(By.TagName("body")).Text;
                    if (alert.Contains("出荷指示可能商品があります"))
                    {
                        chrome.FindElement(By.XPath("/html/body/div[1]/main/div[3]/div[3]/div[2]")).Click();
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ログインできません。ログインID、パスワードを確認してください。"))
                    {
                        fun.WriteLog("Login Failed", "024-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success ------", "024-");
                    }

                    try
                    {
                        int Lastrow = dt024.Rows.Count;

                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt024.Rows[i]["発注コード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.Id("pc-p_c1")).Clear();
                                    chrome.FindElement(By.Id("pc-p_c1")).SendKeys(ordercode);
                                    chrome.FindElement(By.Id("pc-check_data")).Click();
                                }
                                catch
                                {
                                    chrome.FindElement(By.Id("pc-p_c1")).Clear();
                                    Thread.Sleep(3000);
                                    chrome.FindElement(By.Id("pc-p_c1")).SendKeys(ordercode);
                                    chrome.FindElement(By.Id("pc-check_data")).Click();
                                }

                                entity = new Qbei_Entity();
                                entity.siteID = 24;
                                entity.sitecode = "024";
                                entity.janCode = dt024.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt024.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt024.Rows[i]["最終反映日"].ToString();
                                entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                entity.orderCode = dt024.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Message.Contains("検索条件に一致する商品はありません。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = "";
                                        entity.price = dt024.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }

                                    // Start Change Code 10-10-2024
                                    else if (Message.Contains("検索結果：1件"))
                                    {
                                        entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div/div[4]/div[2]/span[1]")).Text;
                                        entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);

                                        string stock = chrome.FindElement(By.CssSelector(".stock_value")).Text;
                                        entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                        entity.True_Quantity = stock;
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }

                                    else if (Message.Contains(entity.janCode))
                                    {

                                        if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[1]/div[3]/div[4]")).Text.Contains(entity.janCode)
                                            && chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[1]/div[3]/div[2]")).Text.Contains(entity.orderCode))
                                        {

                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[1]/div[4]/div[2]/span[1]")).Text;
                                            entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);

                                            string stock = chrome.FindElement(By.CssSelector(".stock_value")).Text;
                                            entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                            entity.True_Quantity = stock;
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);

                                        }

                                        else if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[2]/div[3]/div[4]")).Text.Contains(entity.janCode))
                                        {
                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[2]/div[4]/div[2]/span[1]")).Text;
                                            entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);

                                            try
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[2]/div[4]/div[4]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            catch
                                            { 
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[2]/div[4]/div[5]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);

                                        }

                                        else if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[3]/div[3]/div[4]")).Text.Contains(entity.janCode))
                                        {
                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[3]/div[4]/div[2]/span[1]")).Text;
                                            entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);
                                            try
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[3]/div[4]/div[4]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            catch
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[3]/div[4]/div[5]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);

                                        }

                                        else if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[4]/div[3]/div[4]")).Text.Contains(entity.janCode))
                                        {
                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[4]/div[4]/div[2]/span[1]")).Text;
                                            entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);
                                            try
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[4]/div[4]/div[4]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            catch
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[4]/div[4]/div[5]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);

                                        }

                                        else
                                        {
                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[5]/div[4]/div[2]/span[1]")).Text;
                                            entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);
                                            try
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[5]/div[4]/div[4]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            catch
                                            {
                                                string stock = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div[5]/div[4]/div[5]/div[1]/span[2]")).Text;
                                                entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                                entity.True_Quantity = stock;
                                            }
                                            entity.stockDate = "2100-02-01";
                                            entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "Not Found";
                                            fun.Qbei_Inserts(entity);

                                        }


                                    }


                                    else
                                    {
                                        entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[5]/div/div[4]/div[2]/span[1]")).Text;
                                        entity.price = entity.price.Replace("円", string.Empty).Replace("お渡し価格：", string.Empty).Replace("(税抜)", string.Empty).Replace(",", string.Empty);

                                        string stock = chrome.FindElement(By.CssSelector(".stock_value")).Text;
                                        entity.qtyStatus = stock.Equals("◎") ? "good" : stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : stock.Equals("お取寄品") ? "inquiry" : "unknown status";
                                        entity.True_Quantity = stock;
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = dt024.Rows[i]["purchaserURL"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }
                                    // End Change Code 10-10-2024


                                    if (entity.price == null || entity.qtyStatus == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt024.Rows[i]["下代"].ToString();
                                        entity.purchaseURL = "";
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                    }

                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                                }
                            }
                        }

                        qe.site = 24;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt024.Rows[i]["JANコード"].ToString();
                        ordercode = dt024.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(24, fun.GetSiteName("024"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "024");
                        fun.WriteLog(ex, "024-", janCode, ordercode);
                    }


                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "024-");
                Environment.Exit(0);
            }

        }

    }
}
