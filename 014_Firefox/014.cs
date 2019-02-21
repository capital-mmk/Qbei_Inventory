using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
//using Selenium.WebDriver.GeckoDriver.Win64;
using OpenQA.Selenium.Chrome;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
//using OpenQA.Selenium.Interactions;



namespace _014_Firefox
{
    class Program : CommonFunction
    {

        public static string janCode = string.Empty;
        public static CommonFunction fun = new CommonFunction();
        public static string st = string.Empty;
        static string strParam = string.Empty;
        DataTable dt014;

        public static FirefoxOptions FirefoxDriverDirectory { get; private set; }

        static void Main(string[] args)
        {
            testflag();
        }
        public static void testflag()
        {

            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            qe.starttime = DateTime.Now.ToString();
            st = qe.starttime;
            qe.site = 14;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(14);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(14);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }
        public static void StartRun()
        {
            try
            {
                fun.setURL("014");
                fun.MoveToTrash("014");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(14);
                fun.Qbei_ErrorDelete(14);

                ReadData();

            }
            catch (Exception)
            { }
        }

        public static void ReadData()
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\014_Download\");
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                using (IWebDriver chrome = new ChromeDriver(chromeOptions))
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    qe.SiteID = 14;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string url = dt.Rows[0]["Url"].ToString();
                    chrome.Url = url;
                    string title = chrome.Title;

                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("c_LOGONID")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("c_PASSWD")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "014-");
                    chrome.FindElement(By.Name("c_PASSWD")).Submit();

                    Thread.Sleep(2000);
                    url = chrome.Url.ToString();
                    fun.WriteLog("Login success             ------", "014-");
                    chrome.Navigate().GoToUrl("https://edi.iwaishokai.co.jp/weborder/i2_0003/i2_0003.php?b_DOWNLOAD=1&w_KEYWORD=0");
                    fun.WriteLog("Navigation to Download Url success------", "014-");
                    Thread.Sleep(5000);

                    DataTable dt014 = fun.GetDatatable("014");
                    dt014 = fun.GetOrderData(dt014, "https://edi.iwaishokai.co.jp", "014", "");
                    fun.GetTotalCount("014");

                    if (!File.Exists(@"C:\Qbei_Log\014_Download\items.csv"))
                    {
                        fun.Qbei_ErrorInsert(14, fun.GetSiteName("014"), "Access Denied!", "0", "0", 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "014");
                        fun.WriteLog("Access Denied! ", "014--");
                        Environment.Exit(0);
                    }
                    else
                    {
                        //2018-05-04 Start
                        string[] str = { "商品コード", "現在庫数", "卸価格" };
                        DataTable dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\014_Download\", str);
                        fun.WriteLog("Download success match with datatable------", "014-");
                        //2018-05-04 End
                        fun.Qbei_Insert_XML(dt014, dtItem, "Qbei_Insert_Xml", strParam);
                        fun.WriteLog("Insert data to db success------", "014-");
                        qe.endtime = DateTime.Now.ToString();
                        qe.flag = 2;
                        qe.starttime = st;
                        qe.site = 14;
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                }

            }

            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "014-");
                Environment.Exit(0);
            }

        }

    }
}
