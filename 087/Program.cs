//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;

//namespace _87ダートフリーク
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
//            Application.Run(new frm087());
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

namespace _87ダートフリーク
{
    class Program
    {

        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt087 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        public static string ordercode;


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
            entitySetting.site = 87;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(087);
            intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());

            
            if (intFlag == 0)
            {
                fun.ChangeFlag(entitySetting);
                StartRun();
            }
            
            else if (intFlag == 1)
            {
                fun.deleteData(87);
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
            DataTable dt087 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("087");
                fun.Qbei_Delete(87);
                fun.Qbei_ErrorDelete(87);
                dt087 = fun.GetDatatable("087");
                fun.GetTotalCount("087");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "087-");
                Environment.Exit(0);
            }
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);//<remark Add Logic for ChormeDriver 2025/04/08 />
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
                    qe.SiteID = 87;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    Thread.Sleep(4000);

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.XPath("/html/body/div[2]/div[2]/form/table/tbody/tr/td/table/tbody/tr[2]/td[2]/input")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.XPath("/html/body/div[2]/div[2]/form/table/tbody/tr/td/table/tbody/tr[3]/td[2]/input")).SendKeys(password);

                    fun.WriteLog("Navigation to Site Url success------", "087-");
                    chrome.FindElement(By.XPath("/html/body/div[2]/div[2]/form/table/tbody/tr/td/table/tbody/tr[4]/td/input")).Click();
                    Thread.Sleep(2000);

                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ログインされていません。"))
                    {
                        fun.WriteLog("Login Failed", "087-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "087-");
                        chrome.FindElement(By.XPath("/html/body/table/tbody/tr/td[1]/div/a[1]")).Click();
                    }


                    try
                    {

                        int Lastrow = dt087.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {
                                ordercode = dt087.Rows[i]["発注コード"].ToString();
                                try
                                {
                                    chrome.FindElement(By.Name("edit")).Click();
                                    chrome.FindElement(By.Name("cataloghinban")).SendKeys(ordercode);
                                    chrome.FindElement(By.Name("submitall")).Click();
                                }
                                catch
                                {
                                    Thread.Sleep(1000);
                                    chrome.FindElement(By.Name("edit")).Click();
                                    chrome.FindElement(By.Name("cataloghinban")).SendKeys(ordercode);
                                    Thread.Sleep(1000);
                                    chrome.FindElement(By.Name("submitall")).Click();
                                }


                                entity = new Qbei_Entity();
                                entity.siteID = 87;
                                entity.sitecode = "087";
                                entity.janCode = dt087.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt087.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt087.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt087.Rows[i]["発注コード"].ToString();


                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    Thread.Sleep(2000);
                                    string Check_Message = chrome.FindElement(By.TagName("body")).Text;

                                    if (Check_Message.Contains("検索商品がありませんでした。"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.purchaseURL = chrome.Url;
                                        entity.price = dt087.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);

                                    }

                                    else
                                    {
                                        entity.price = chrome.FindElement(By.XPath("/html/body/table/tbody/tr/td[2]/div/form/table/tbody/tr[2]/td/div/table/tbody/tr/td[7]/font")).Text;
                                        entity.price = entity.price.Replace("円", string.Empty).Replace(",", string.Empty);

                                        try
                                        {
                                            qty = chrome.FindElement(By.XPath("/html/body/table/tbody/tr/td[2]/div/form/table/tbody/tr[2]/td/div/table/tbody/tr/td[10]/font/b")).Text;
                                            entity.qtyStatus = qty.Equals("◎") ? "good" : qty.Equals("○") || qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("※") ? "empty" : "invalid status code";
                                            entity.stockDate = qty.Equals("◎") || qty.Equals("○") || qty.Equals("▲") ? "2100-01-01" : qty.Equals("×") || qty.Equals("※") ? "2100-02-01" : "unknown date";
                                            entity.True_Quantity = qty;
                                        }
                                        catch
                                        {
                                            qty = chrome.FindElement(By.XPath("/html/body/table/tbody/tr/td[2]/div/form/table/tbody/tr[2]/td/div/table/tbody/tr/td[10]/font")).Text;
                                            entity.qtyStatus = qty.Equals("◎") ? "good" : qty.Equals("○") || qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("※") ? "empty" : "invalid status code";
                                            entity.stockDate = qty.Equals("◎") || qty.Equals("○") || qty.Equals("▲") ? "2100-01-01" : qty.Equals("×") || qty.Equals("※") ? "2100-02-01" : "unknown date";
                                            entity.True_Quantity = qty;
                                        }
                                        entity.purchaseURL = chrome.Url;
                                        entity.True_StockDate = "項目無し";
                                        fun.Qbei_Inserts(entity);

                                    }


                                    if (entity.price == null || entity.qtyStatus == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt087.Rows[i]["下代"].ToString();
                                        entity.purchaseURL = "";
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                    }

                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                                }
                            }
                        }
                        qe.site = 87;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt087.Rows[i]["JANコード"].ToString();
                        ordercode = dt087.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(87, fun.GetSiteName("087"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "087");
                        fun.WriteLog(ex, "087-", janCode, ordercode);
                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "087-");
                Environment.Exit(0);
            }
        }
    }

}