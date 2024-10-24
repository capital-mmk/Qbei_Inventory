//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace _143
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
//            Application.Run(new frm143());
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

namespace _143
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
        DataTable dt143 = new DataTable();
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
            entitySetting.site = 143;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(13);
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
                fun.deleteData(143);
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
            DataTable dt143 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("143");
                fun.Qbei_Delete(143);
                fun.Qbei_ErrorDelete(143);
                dt143 = fun.GetDatatable("143");
                fun.GetTotalCount("143");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "143-");
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
                    qe.SiteID = 143;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);

                    string code = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("customer_code")).SendKeys(code);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("login_id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("password")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "143-");
                    chrome.FindElement(By.Name("login")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("得意先コード、パスワードが正しくありません"))
                    {
                        fun.WriteLog("Login Failed", "143-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "143-");
                        chrome.Navigate().GoToUrl(fun.url + "/goods/goods_list.html?search_C_sSyohinCd_item");
                    }


                    try
                    {

                        int Lastrow = dt143.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt143.Rows[i]["発注コード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.Name("clear")).Click();
                                    chrome.FindElement(By.ClassName("goods_code_box")).SendKeys(ordercode);
                                    chrome.FindElement(By.Name("search")).Click();
                                }
                                catch
                                {
                                    Thread.Sleep(1000);
                                    chrome.FindElement(By.Name("clear")).Click();
                                    chrome.FindElement(By.ClassName("goods_code_box")).SendKeys(ordercode);
                                    Thread.Sleep(1000);
                                    chrome.FindElement(By.Name("search")).Click();
                                }


                                entity = new Qbei_Entity();
                                entity.siteID = 143;
                                entity.sitecode = "143";
                                entity.janCode = dt143.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt143.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt143.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt143.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Check_Message.Contains("該当データはありません。"))
                                    {
                                        if (dt143.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt143.Rows[i]["在庫情報"].ToString().Contains("empty"))
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-01-10";
                                            entity.price = dt143.Rows[i]["下代"].ToString();
                                        }
                                        else
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt143.Rows[i]["下代"].ToString();
                                        }
                                        entity.purchaseURL = "";
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";

                                        fun.Qbei_Inserts(entity);
                                    }


                                    else
                                    {

                                        if (chrome.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td/div[2]/div[2]/form/table/tbody/tr[3]/td[2]")).Text == entity.orderCode)
                                        {
                                            entity.price = dt143.Rows[i]["下代"].ToString();
                                            qty = chrome.FindElement(By.XPath("/html/body/table/tbody/tr[2]/td/div[2]/div[2]/form/table/tbody/tr[3]/td[6]/table/tbody/tr[2]/td")).Text;
                                            entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("今期完売") ? "empty" : "unknown status";                                                                                                                                                                                                                    //</remark>
                                            entity.True_Quantity = qty;
                                            if (qty.Equals("○") || qty.Equals("▲"))
                                            {
                                                entity.stockDate = "2100-01-01";
                                            }
                                            else
                                            {
                                                entity.stockDate = "2100-02-01";
                                            }
                                            entity.purchaseURL = dt143.Rows[i]["purchaserURL"].ToString();
                                            entity.True_StockDate = "項目無し";
                                            fun.Qbei_Inserts(entity);

                                        }

                                    }

                                    if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt143.Rows[i]["下代"].ToString();
                                        entity.purchaseURL = chrome.Url;
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                    }


                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                                }
                            }
                        }
                        qe.site = 143;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt143.Rows[i]["JANコード"].ToString();
                        ordercode = dt143.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(143, fun.GetSiteName("143"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "143");
                        fun.WriteLog(ex, "143-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "143-");
                Environment.Exit(0);
            }
        }
    }

}