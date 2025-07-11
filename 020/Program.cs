//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;

//namespace _20ダイアテック_高難易度_
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
//            Application.Run(new frm020());
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
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _20ダイアテック_高難易度_
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
        DataTable dt020 = new DataTable();
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
            entitySetting.site = 20;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(20);
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
                fun.deleteData(20);
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
            DataTable dt020 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("020");
                fun.Qbei_Delete(20);
                fun.Qbei_ErrorDelete(20);
                dt020 = fun.GetDatatable("020");
                fun.GetTotalCount("020");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "020-");
                Environment.Exit(0);
            }

            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddArguments("-no-sandbox");
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);
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
                    qe.SiteID = 20;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Id("login_uid")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Id("login_pwd")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "020-");
                    chrome.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div/div/form/div/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("会員IDとパスワードを入力してログインしてください。"))
                    {
                        fun.WriteLog("Login Failed", "020-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "020-");
                    }


                    try
                    {
                        int Lastrow = dt020.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                string od;
                                od = dt020.Rows[i]["発注コード"].ToString();
                                chrome.Navigate().GoToUrl("https://www.b2bdiatec.jp/shop/g/g" + od);


                                entity = new Qbei_Entity();
                                entity.siteID = 20;
                                entity.sitecode = "020";
                                entity.janCode = dt020.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt020.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt020.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt020.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    string Message = chrome.FindElement(By.TagName("body")).Text;
                                    if (Message.Contains("大変申し訳ありませんが、該当ページがございません。") || Message.Contains("申し訳ございません。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt020.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }


                                    else
                                    {
                                        int n = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[2]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/form/div/div[1]/div/div/dl")).Count());

                                        for (int j = 1; j <= n; j++)
                                        {

                                            if (chrome.FindElement(By.XPath("/html/body/div[2]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/form/div/div[1]/div/div/dl[" + (j) + "]/dd[1]")).Text.Contains(entity.orderCode))
                                            {

                                                entity.price = chrome.FindElement(By.ClassName("goods_detail_price_")).Text;
                                                entity.price = entity.price.Replace("￥", string.Empty).Replace("（税抜）", string.Empty);
                                                entity.price = ((int)(Convert.ToDouble(entity.price) * 0.75)).ToString();

                                                string strQtyStatus = chrome.FindElement(By.XPath("/html/body/div[2]/div[4]/div[1]/div[1]/div[1]/div[1]/div[2]/form/div/div[1]/div/div/dl[" + (j) + "]/dd[2]/p")).Text.Replace("在庫：", string.Empty);
                                                entity.qtyStatus = strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || strQtyStatus.Contains('〇') || fun.IsGood(strQtyStatus) ? "good" : strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus) ? "small" : strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬") || strQtyStatus.Contains("未定") || strQtyStatus.Contains("次回入荷限り") || strQtyStatus.Contains("新型切替") || strQtyStatus.Contains("受注停止中") ? "empty" : "unknown status";


                                                if (strQtyStatus.Contains('◎') || strQtyStatus.Contains('○') || strQtyStatus.Contains('〇') || strQtyStatus.Contains("未定") || fun.IsGood(strQtyStatus) || strQtyStatus.Contains('△') || fun.IsSmall(strQtyStatus))
                                                    entity.stockDate = "2100-01-01";

                                                else if (strQtyStatus.Contains("完売") || strQtyStatus.Contains("×") || strQtyStatus.Contains("入荷待ち") || strQtyStatus.Contains("次回入荷限り") || strQtyStatus.Contains("新型切替") || strQtyStatus.Contains("受注停止中"))
                                                    entity.stockDate = "2100-02-01";

                                                else if (strQtyStatus.Contains("上旬") || strQtyStatus.Contains("中旬") || strQtyStatus.Contains("下旬"))
                                                {
                                                    string strTemp = Regex.Replace(strQtyStatus, "[^0-9]+", string.Empty);
                                                    int intMonth = int.Parse(strTemp);
                                                    if (strQtyStatus.Contains("下旬"))
                                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, DateTime.DaysInMonth(DateTime.Now.Year, intMonth)).ToString("yyyy-MM-dd");
                                                    else if (strQtyStatus.Contains("中旬"))
                                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 20).ToString("yyyy-MM-dd");
                                                    else
                                                        entity.stockDate = new DateTime(DateTime.Now.Year, intMonth, 10).ToString("yyyy-MM-dd");
                                                }

                                                else entity.stockDate = "unknown date";

                                                entity.True_StockDate = "項目無し";
                                                entity.True_Quantity = strQtyStatus;
                                                entity.purchaseURL = chrome.Url;
                                                fun.Qbei_Inserts(entity);

                                                break;
                                            }
                                        }
                                    }


                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                                }
                            }
                        }
                        qe.site = 20;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt020.Rows[i]["JANコード"].ToString();
                        ordercode = dt020.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(20, fun.GetSiteName("020"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "020");
                        fun.WriteLog(ex, "020-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "020-");
                Environment.Exit(0);
            }
        }
    }
}

