//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;

//namespace _46トライスポーツ
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
//            Application.Run(new frm046());
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

namespace _46トライスポーツ
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
        DataTable dt046 = new DataTable();
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
            entitySetting.site = 46;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(046);
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
                fun.deleteData(46);
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
            DataTable dt046 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("046");
                fun.Qbei_Delete(46);
                fun.Qbei_ErrorDelete(46);
                dt046 = fun.GetDatatable("046");
                fun.GetTotalCount("046");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "046-");
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
                    qe.SiteID = 46;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);

                    //chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/aside/div[3]/section[1]/div/ul/li[1]/a")).Click();
                    chrome.FindElement(By.XPath("/html/body/div[1]/div/div/div/p/a")).Click();

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("passwd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "046-");
                    chrome.FindElement(By.XPath("/html/body/div/div[2]/div/form/div/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("会員ID/メールアドレスかパスワードが正しくありません"))
                    {
                        fun.WriteLog("Login Failed", "046-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "046-");
                    }


                    try
                    {

                        int Lastrow = dt046.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt046.Rows[i]["発注コード"].ToString();

                                entity = new Qbei_Entity();
                                entity.siteID = 46;
                                entity.sitecode = "046";
                                entity.janCode = dt046.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt046.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt046.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt046.Rows[i]["発注コード"].ToString();
                                entity.purchaseURL = dt046.Rows[i]["purchaserURL"].ToString();

                                if (entity.purchaseURL == "")
                                {
                                    fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), "PurchaseURL Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                                }

                                else
                                {

                                    chrome.Navigate().GoToUrl(entity.purchaseURL);
                                    Thread.Sleep(2000);

                                    string Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Message.Contains("申し訳ございません。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = "";
                                        entity.price = dt046.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }

                                    else if (Message.Contains("在庫無し"))
                                    {
                                        fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), "OrderCode Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                                    }

                                    else if (Message.Contains("独自商品コード"))
                                    {
                                        try
                                        {
                                            entity.price = chrome.FindElement(By.CssSelector("p.price:nth-child(1)")).Text;
                                            entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                entity.price = chrome.FindElement(By.CssSelector(".item-yen")).Text;
                                                entity.price = entity.price.Replace("円（税込）", string.Empty).Replace(",", string.Empty);
                                            }
                                            catch
                                            {
                                                entity.price = chrome.FindElement(By.CssSelector(".item-yen > span:nth-child(1)")).Text;
                                                entity.price = entity.price.Replace("円（税込）", string.Empty).Replace(",", string.Empty);
                                            }
                                        }

                                        if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                        {
                                            qty = chrome.FindElement(By.CssSelector(".item-stock")).Text;
                                            entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                            entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                            entity.True_Quantity = qty;
                                            entity.True_StockDate = "項目無し";
                                        }

                                        else
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.True_Quantity = "項目無し";
                                            entity.True_StockDate = "項目無し";
                                        }

                                        entity.purchaseURL = chrome.Url;
                                        fun.Qbei_Inserts(entity);
                                    }


                                    else
                                    {
                                        try
                                        {
                                            Thread.Sleep(1000);
                                            int n = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr")).Count());

                                            if (n == 0)
                                            {
                                                int c = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div[3]/table/tbody/tr")).Count());

                                                if (c == 0)
                                                {
                                                    int b = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div[4]/main/div/div[2]/div[3]/table/tbody/tr")).Count());

                                                    if (b == 0)
                                                    {
                                                        int a = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div[5]/main/div/div[2]/div[3]/table/tbody/tr")).Count());

                                                        for (int j = 2; j <= a; j++)
                                                        {
                                                            if (chrome.FindElement(By.XPath("/html/body/div[1]/div[5]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/p")).Text.Contains(entity.orderCode))
                                                            {
                                                                entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[5]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/div/p")).Text;
                                                                entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                                if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                                {
                                                                    qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[5]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[2]/p")).Text;
                                                                    entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                                    entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                                    entity.True_Quantity = qty;
                                                                    entity.True_StockDate = "項目無し";
                                                                }

                                                                else
                                                                {
                                                                    entity.qtyStatus = "empty";
                                                                    entity.stockDate = "2100-02-01";
                                                                    entity.True_Quantity = "項目無し";
                                                                    entity.True_StockDate = "項目無し";
                                                                }

                                                                entity.purchaseURL = chrome.Url;
                                                                fun.Qbei_Inserts(entity);
                                                                break;
                                                            }

                                                        }

                                                    }
                                                    else
                                                    {
                                                        for (int j = 2; j <= b; j++)
                                                        {
                                                            if (chrome.FindElement(By.XPath("/html/body/div[1]/div[4]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/p")).Text.Contains(entity.orderCode))
                                                            {
                                                                entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[4]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/div/p")).Text;
                                                                entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                                if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                                {
                                                                    qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[4]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[2]/p")).Text;
                                                                    entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                                    entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                                    entity.True_Quantity = qty;
                                                                    entity.True_StockDate = "項目無し";
                                                                }

                                                                else
                                                                {
                                                                    entity.qtyStatus = "empty";
                                                                    entity.stockDate = "2100-02-01";
                                                                    entity.True_Quantity = "項目無し";
                                                                    entity.True_StockDate = "項目無し";
                                                                }

                                                                entity.purchaseURL = chrome.Url;
                                                                fun.Qbei_Inserts(entity);
                                                                break;
                                                            }

                                                        }
                                                    }

                                                }

                                                else
                                                {
                                                    for (int j = 2; j <= c; j++)
                                                    {

                                                        if (chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/p")).Text.Contains(entity.orderCode))
                                                        {
                                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/div/p")).Text;
                                                            entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                            if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                            {
                                                                qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[2]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[2]/p")).Text;
                                                                entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                                entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                                entity.True_Quantity = qty;
                                                                entity.True_StockDate = "項目無し";
                                                            }

                                                            else
                                                            {
                                                                entity.qtyStatus = "empty";
                                                                entity.stockDate = "2100-02-01";
                                                                entity.True_Quantity = "項目無し";
                                                                entity.True_StockDate = "項目無し";
                                                            }

                                                            entity.purchaseURL = chrome.Url;
                                                            fun.Qbei_Inserts(entity);
                                                            break;
                                                        }

                                                    }
                                                }


                                            }
                                            else
                                            {
                                                for (int j = 2; j <= n; j++)
                                                {

                                                    if (chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[4]/div[1]/div/p")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[4]/div[1]/div/div/p")).Text;
                                                        entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                        if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                        {
                                                            qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/p")).Text;
                                                            entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                            entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                            entity.True_Quantity = qty;
                                                            entity.True_StockDate = "項目無し";
                                                        }

                                                        else
                                                        {
                                                            entity.qtyStatus = "empty";
                                                            entity.stockDate = "2100-02-01";
                                                            entity.True_Quantity = "項目無し";
                                                            entity.True_StockDate = "項目無し";
                                                        }

                                                        entity.purchaseURL = chrome.Url;
                                                        fun.Qbei_Inserts(entity);
                                                        break;
                                                    }


                                                }
                                            }


                                        }

                                        catch
                                        {
                                            if (chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody")).Text.Contains(entity.orderCode))
                                            {
                                                int n = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr")).Count());

                                                for (int j = 2; j <= n; j++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/p")).Text.Contains(entity.orderCode))
                                                    {
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/div/p")).Text;
                                                        entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                        if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                        {
                                                            qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[2]/p")).Text;
                                                            entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                            entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                            entity.True_Quantity = qty;
                                                            entity.True_StockDate = "項目無し";
                                                        }

                                                        else
                                                        {
                                                            entity.qtyStatus = "empty";
                                                            entity.stockDate = "2100-02-01";
                                                            entity.True_Quantity = "項目無し";
                                                            entity.True_StockDate = "項目無し";
                                                        }

                                                        entity.purchaseURL = chrome.Url;
                                                        fun.Qbei_Inserts(entity);
                                                        break;
                                                    }

                                                    else
                                                    {
                                                        if (chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[1]/div")).Text.Contains(entity.orderCode))
                                                        {
                                                            entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[3]/div[1]/div/div/p")).Text;
                                                            entity.price = entity.price.Replace("円(税込)", string.Empty).Replace(",", string.Empty);

                                                            if (Message.Contains("〇在庫あり") || Message.Contains("△残りわずか") || Message.Contains("×在庫なし"))
                                                            {
                                                                qty = chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/main/div/div[2]/div[3]/table/tbody/tr[" + (j) + "]/td[2]/p")).Text;
                                                                entity.qtyStatus = qty.Equals("〇在庫あり") ? "good" : qty.Contains("△残りわずか") ? "small" : qty.Equals("×在庫なし") ? "empty" : "unknown status";
                                                                entity.stockDate = qty.Equals("〇在庫あり") || qty.Contains("△残りわずか") ? "2100-01-01" : qty.Equals("×在庫なし") ? "2100-02-01" : "unknown date";
                                                                entity.True_Quantity = qty;
                                                                entity.True_StockDate = "項目無し";
                                                            }

                                                            else
                                                            {
                                                                entity.qtyStatus = "empty";
                                                                entity.stockDate = "2100-02-01";
                                                                entity.True_Quantity = "項目無し";
                                                                entity.True_StockDate = "項目無し";
                                                            }

                                                            entity.purchaseURL = chrome.Url;
                                                            fun.Qbei_Inserts(entity);
                                                            break;
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                    }


                                }
                            }
                        }
                        qe.site = 46;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt046.Rows[i]["JANコード"].ToString();
                        ordercode = dt046.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(46, fun.GetSiteName("046"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "046");
                        fun.WriteLog(ex, "046-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "046-");
                Environment.Exit(0);
            }
        }
    }

}
