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

namespace _65野口
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
        DataTable dt065 = new DataTable();
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
            entitySetting.site = 65;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(65);
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
                fun.deleteData(65);
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
            DataTable dt065 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("065");
                fun.Qbei_Delete(65);
                fun.Qbei_ErrorDelete(65);
                dt065 = fun.GetDatatable("065");
                fun.GetTotalCount("065");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "065-");
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
                    qe.SiteID = 65;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("tokuisakicode")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("webloginpassword")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "065-");
                    chrome.FindElement(By.XPath("/html/body/div/div[1]/div/form/div/div[3]/input")).Click();
                    Thread.Sleep(2000);

                    //<remark Add Logic for Check of Login URL 2023/01/16 Start>
                    string check_URL = chrome.Url;
                    if (!check_URL.Equals("https://noguchi-shokai.com/News"))
                    {
                        Thread.Sleep(4000);
                        string username_return = dt.Rows[0]["UserName"].ToString();
                        chrome.FindElement(By.Name("tokuisakicode")).SendKeys(username_return);
                        string password_return = dt.Rows[0]["Password"].ToString();
                        chrome.FindElement(By.Name("webloginpassword")).SendKeys(password_return);
                        fun.WriteLog("Navigation to Site Url success------", "065-");
                        chrome.FindElement(By.XPath("/html/body/div/div[1]/div/form/div/div[3]/input")).Click();
                        Thread.Sleep(6000);
                    }
                    //</remark 2023/01/16 End>

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("得意先コードまたはパスワードが正しくありません"))
                    {
                        fun.WriteLog("Login Failed", "065-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "065-");
                        chrome.Navigate().GoToUrl("https://noguchi-shokai.com/SyohinSearch");
                    }


                    try
                    {
                        int Lastrow = dt065.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt065.Rows[i]["発注コード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).Clear();
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).SendKeys(ordercode);
                                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[5]/div/button[1]")).Click();
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
                                entity.siteID = 65;
                                entity.sitecode = "065";
                                entity.janCode = dt065.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt065.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt065.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt065.Rows[i]["発注コード"].ToString();


                                //<remark>
                                //Check to Ordercode
                                //</remark>
                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Message = chrome.FindElement(By.TagName("body")).Text;
                                    if (Message.Contains("検索条件に該当するデータは存在しません。"))
                                    {
                                        try
                                        {
                                            chrome.FindElement(By.CssSelector("body > div.bootbox.modal.fade.bootbox-alert.show > div > div > div.modal-footer > button")).Click();
                                        }
                                        catch
                                        {
                                            //<remark Add&Edit Logic for Check of Message 2023/01/16 Start>
                                            try
                                            {
                                                chrome.Navigate().GoToUrl("https://noguchi-shokai.com/SyohinSearch");
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
                                            catch
                                            {
                                                chrome.Navigate().GoToUrl("https://noguchi-shokai.com/SyohinSearch");
                                                Thread.Sleep(8000);
                                                string Check_Message = chrome.FindElement(By.TagName("body")).Text;
                                                if (Check_Message.Contains("検索条件に該当するデータは存在しません。"))
                                                {
                                                    chrome.FindElement(By.CssSelector("body > div.bootbox.modal.fade.bootbox-alert.show > div > div > div.modal-footer > button")).Click();
                                                }
                                                chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).Clear();
                                                chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[1]/input")).SendKeys(ordercode);
                                                chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[3]/div[5]/div/button[1]")).Click();
                                            }
                                            //</remark 2023/01/16 End>
                                        }
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = "https://noguchi-shokai.com/SyohinSearch";
                                        entity.price = dt065.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        int n = chrome.FindElements(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr")).Count();
                                        for (int i = 1; i <= n; i++)
                                        {
                                            if (chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[3]/span[3]")).Text.Contains(entity.janCode))
                                            {
                                                if (chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[9]")).Text.Equals(""))
                                                {
                                                    entity.price = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[5]/span[1]")).Text.Replace("円", "").Replace(",", "").Trim();
                                                    entity.purchaseURL = "https://noguchi-shokai.com/SyohinDetail?id=" + ordercode + "&amount=1";
                                                    string stock = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[8]/span/b")).GetAttribute("innerHTML").ToString().Trim();
                                                    if (stock.All(char.IsDigit))
                                                    {
                                                        entity.qtyStatus = "empty";
                                                    }
                                                    else
                                                    {
                                                        entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : "unknown status";
                                                    }
                                                    entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                    entity.True_StockDate = "項目無し";
                                                    entity.True_Quantity = stock;
                                                }
                                                else
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[9]")).Text.Contains("特価"))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[7]/span[1]")).Text.Replace("円", "").Replace(",", "").Trim();
                                                        entity.purchaseURL = "https://noguchi-shokai.com/SyohinDetail?id=" + ordercode + "&amount=1";
                                                        string stock = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[8]/span/b")).GetAttribute("innerHTML").ToString().Trim();
                                                        if (stock.All(char.IsDigit))
                                                        {
                                                            entity.qtyStatus = "empty";
                                                        }
                                                        else
                                                        {
                                                            entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : "unknown status";
                                                        }
                                                        string maker_stockdate = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[9]")).Text;
                                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : "unknown status";
                                                        entity.True_StockDate = maker_stockdate;
                                                        entity.True_Quantity = stock;
                                                    }
                                                    else
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[5]/span[1]")).Text.Replace("円", "").Replace(",", "").Trim();
                                                        entity.purchaseURL = "https://noguchi-shokai.com/SyohinDetail?id=" + ordercode + "&amount=1";
                                                        string stock = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[8]/span/b")).GetAttribute("innerHTML").ToString().Trim();
                                                        if (stock.All(char.IsDigit))
                                                        {
                                                            entity.qtyStatus = "empty";
                                                        }
                                                        else
                                                        {
                                                            entity.qtyStatus = stock.Equals("○") ? "good" : stock.Equals("△") ? "small" : stock.Equals("×") ? "empty" : "unknown status";
                                                        }
                                                        string maker_stockdate = chrome.FindElement(By.XPath("/html/body/div/div[2]/div/div/div[6]/table/tbody/tr[" + (i) + "]/td[9]")).Text;
                                                        entity.stockDate = entity.qtyStatus.Equals("good") || entity.qtyStatus.Equals("small") ? "2100-01-01" : entity.qtyStatus.Equals("empty") ? "2100-02-01" : maker_stockdate.Equals("取り寄せ品") ? "2100-01-01" : "unknown status";
                                                        entity.True_StockDate = maker_stockdate;
                                                        entity.True_Quantity = stock;
                                                    }
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.purchaseURL = "https://noguchi-shokai.com/SyohinSearch";
                                                entity.price = dt065.Rows[i]["下代"].ToString();
                                                entity.True_StockDate = "Not Found";
                                                entity.True_Quantity = "Not Found";
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
                                                fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "entity.qtyStatus is null!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "entity.qtyStatus is unknown status!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "0");
                                        }
                                    }
                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                                }
                            }
                        }
                        qe.site = 65;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt065.Rows[i]["JANコード"].ToString();
                        ordercode = dt065.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(65, fun.GetSiteName("065"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "065");
                        fun.WriteLog(ex, "065-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "065-");
                Environment.Exit(0);
            }
        }
    }
}
