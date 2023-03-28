//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace _124_Mizutani
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
//            Application.Run(new frm124_ミズタニ());
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

namespace _124_Mizutani
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
        DataTable dt124 = new DataTable();
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
            entitySetting.site = 124;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(124);
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
                fun.deleteData(124);
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
            DataTable dt124 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("124");
                fun.Qbei_Delete(124);
                fun.Qbei_ErrorDelete(124);
                dt124 = fun.GetDatatable("124");
                fun.GetTotalCount("124");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "124-");
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
                    string qty = string.Empty;
                    string color = string.Empty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 124;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Id("tokuisakicode")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Id("loginpasswd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "124-");
                    chrome.FindElement(By.Id("btnLogin")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("得意先コード、パスワードが正しくありません"))
                    {
                        fun.WriteLog("Login Failed", "124-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "124-");
                        chrome.Navigate().GoToUrl(fun.url + "/SyohinSearch.aspx");
                    }


                    try
                    {
                        int Lastrow = dt124.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                //<remark Add Logic for check to search process 2023/03/28 Start>
                                //chrome.FindElement(By.Id("btnClear")).Click();
                                //ordercode = dt0124.Rows[i]["発注コード"].ToString();
                                //chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                //chrome.FindElement(By.Id("btnSearch")).Click();
                                try
                                {
                                    chrome.FindElement(By.Id("btnClear")).Click();
                                    ordercode = dt124.Rows[i]["発注コード"].ToString();
                                    chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                    chrome.FindElement(By.Id("btnSearch")).Click();
                                }
                                catch
                                {
                                    Thread.Sleep(2000);
                                    chrome.FindElement(By.Id("btnClear")).Click();
                                    ordercode = dt124.Rows[i]["発注コード"].ToString();
                                    chrome.FindElement(By.Id("keyword")).SendKeys(ordercode);
                                    chrome.FindElement(By.Id("btnSearch")).Click();
                                }
                                //</remark 2023/03/28 End>

                                entity = new Qbei_Entity();
                                entity.siteID = 124;
                                entity.sitecode = "124";
                                entity.janCode = dt124.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt124.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt124.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt124.Rows[i]["発注コード"].ToString();


                                //<remark>
                                //Check to Ordercode
                                //</remark>
                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;
                                    if (Check_Message.Contains("検索条件に該当する商品は、見つかりませんでした"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt124.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string[] Check_Page = chrome.FindElement(By.Id("GridView1_ctl53_lblPages")).Text.Split('/');
                                            int Total_Page = Convert.ToInt32(string.Concat(Check_Page[1].Where(char.IsNumber)));
                                            for (int j = 1; j <= Total_Page; j++)
                                            {
                                                int n = chrome.FindElements(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr")).Count();
                                                for (int i = 2; i <= n - 1; i++)
                                                {
                                                    if (chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[3]/table/tbody/tr[1]/td/span")).Text.Contains(entity.orderCode))
                                                    {
                                                        qty = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[5]/span")).Text;
                                                        entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") || qty.Equals("★") || qty.Equals("？") ? "empty" : "unknown status";                                                                                                                                                                                                                    //</remark>
                                                        entity.price = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[4]/div/span")).Text;
                                                        entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                                                        entity.purchaseURL = chrome.Url;
                                                        strStockDate = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[6]")).Text;
                                                        if (strStockDate != " ")
                                                        {
                                                            entity.True_StockDate = strStockDate;
                                                        }
                                                        else
                                                        {
                                                            entity.True_StockDate = string.Empty;
                                                        }
                                                        color = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[5]")).GetAttribute("style");
                                                        entity.True_Quantity = qty;
                                                        if (color.Contains("red"))
                                                        {
                                                            if (qty.Equals("×") || qty.Equals("★") || qty.Equals("？"))
                                                            {
                                                                entity.stockDate = "2100-02-01";
                                                            }
                                                            else if (qty.Equals("▲"))
                                                            {
                                                                entity.stockDate = "2100-01-01";
                                                            }
                                                            entity.True_Quantity = qty + "(red)";
                                                        }
                                                        else
                                                        {
                                                            entity.stockDate = qty.Equals("○") || qty.Equals("▲") ? "2100-01-01" : qty.Equals("×") || qty.Equals("☆") ? "2100-02-01" : entity.stockDate.Replace("/", "-");
                                                        }
                                                        break;
                                                    }
                                                    if (i == 51)
                                                    {
                                                        chrome.FindElement(By.Id("btnNextPage")).Click();
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            int n = chrome.FindElements(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr")).Count();
                                            for (int i = 2; i <= n; i++)
                                            {
                                                if (chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[3]/table/tbody/tr[1]/td/span")).Text.Contains(entity.orderCode))
                                                {
                                                    qty = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[5]/span")).Text;
                                                    entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") || qty.Equals("★") || qty.Equals("？") ? "empty" : "unknown status";                                                                                                                                                                                                                    //</remark>
                                                    entity.price = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[4]/div/span")).Text;
                                                    entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);
                                                    entity.purchaseURL = chrome.Url;
                                                    strStockDate = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[6]")).Text;
                                                    if (strStockDate != " ")
                                                    {
                                                        entity.True_StockDate = strStockDate;
                                                    }
                                                    else
                                                    {
                                                        entity.True_StockDate = string.Empty;
                                                    }
                                                    color = chrome.FindElement(By.XPath("/html/body/form/div[3]/div[4]/table/tbody/tr[" + (i) + "]/td[5]")).GetAttribute("style");
                                                    entity.True_Quantity = qty;
                                                    if (color.Contains("red"))
                                                    {
                                                        if (qty.Equals("×") || qty.Equals("★") || qty.Equals("？"))
                                                        {
                                                            entity.stockDate = "2100-02-01";
                                                        }
                                                        else if (qty.Equals("▲"))
                                                        {
                                                            entity.stockDate = "2100-01-01";
                                                        }
                                                        entity.True_Quantity = qty + "(red)";
                                                    }
                                                    else
                                                    {
                                                        entity.stockDate = qty.Equals("○") || qty.Equals("▲") ? "2100-01-01" : qty.Equals("×") || qty.Equals("☆") ? "2100-02-01" : entity.stockDate.Replace("/", "-");
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt124.Rows[i]["下代"].ToString();
                                            entity.purchaseURL = chrome.Url;
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
                                                fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), "entity.qtyStatus is null!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), "entity.qtyStatus is unknown status!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                                        }
                                    }
                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                                }
                            }
                        }
                        qe.site = 124;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt124.Rows[i]["JANコード"].ToString();
                        ordercode = dt124.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(124, fun.GetSiteName("124"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "124");
                        fun.WriteLog(ex, "124-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "124-");
                Environment.Exit(0);
            }
        }
    }
}
