using System;
using System.Collections.Generic;
using System.Linq;
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

namespace _014_chrome
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    static class Program
    {
        public static string janCode = string.Empty;
        public static CommonFunction fun = new CommonFunction();
        public static ChromeOptions option = new ChromeOptions();
        public static string st = string.Empty;
        static string strParam = string.Empty;

        //// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        [STAThread]
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

            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            qe.starttime = DateTime.Now.ToString();
            st = qe.starttime;
            qe.site = 14;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(14);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());

            /// <summary>
            /// Flag Number of Check.
            /// </summary>
            /// <remark>
            /// Check to flag is "0" or "1" or "2".
            /// when flag is 0,Change to flag is 1 and Continue to StartRun Process.
            /// </remark>
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            ///<remark>
            ///when flag is 1,To Continue to StartRun Process.
            ///</remark>
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

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
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

        /// <summary>
        /// Use to ChormeDriver and Read to Data at Download CSV.
        /// </summary>
        public static void ReadData()
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";//<Add Logic for Chrome Path 2021/05/24 />
                chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\014_Download\");
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);

                using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))
                {
                    chrome.Manage().Window.Maximize();
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    qe.SiteID = 14;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string url = dt.Rows[0]["Url"].ToString();
                    chrome.Url = url;
                    string title = chrome.Title;

                    ///<remark>
                    ///Login to mall.
                    ///</remark>
                    //2019-08-09 Start
                    string username = dt.Rows[0]["UserName"].ToString();
                    //chrome.FindElement(By.Name("c_LOGONID")).SendKeys(username);
                    chrome.FindElement(By.Name("username")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    //chrome.FindElement(By.Name("c_PASSWD")).SendKeys(password);
                    chrome.FindElement(By.Name("password")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "014-");
                    //chrome.FindElement(By.Name("c_PASSWD")).Submit();
                    chrome.FindElement(By.Id("btnLogin")).Click();
                    //2019-08-09 End

                    // Thread.Sleep(2000);
                    //url = chrome.Url.ToString();
                    //fun.WriteLog("Login success             ------", "014-");

                    //<remark Add Logic for check of Login 2020/06/02 start>
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("ユーザが存在しないか、パスワードが間違っています。"))
                    {
                        fun.Qbei_ErrorInsert(14, fun.GetSiteName("014"), "Login Failed", "0", "0", 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "014");
                        fun.WriteLog("Login Failed", "014-");

                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "014-");
                    }
                    //</remark 2020/06/02 End>

                    ///<remark>
                    ///Download to CSV file.
                    ///</remark>

                    // chrome.Navigate().GoToUrl("https://edi.iwaishokai.co.jp/weborder/i2_0003/i2_0003.php?b_DOWNLOAD=1&w_KEYWORD=0");
                    //chrome.Navigate().GoToUrl("https://iwaishokai.net/search");
                    //chrome.FindElement(By.XPath("//*[@id='app']/div[1]/nav/div/div/div[1]/a/img")).Click();
                    //chrome.FindElement(By.XPath("//*[@id='navbar-collapse2']/ul/li[1]/a")).Click();
                    //2019-08-09 Start
                    Thread.Sleep(20000);//<remark Add Plus Wait time 2020/06/01 />
                    chrome.FindElement(By.XPath("//*[@id='listForm']/div[5]/div/label/input")).Click();
                    chrome.FindElement(By.XPath("//*[@id='listForm']/div[7]/div[2]/button[1]")).Click();
                    //2019-08-09 End
                    fun.WriteLog("Navigation to Download Url success------", "014-");
                    Thread.Sleep(5000);

                    //DataTable dtCancelUpdate = new DataTable();
                    DataTable dt014 = fun.GetDatatable("014");
                    // dt014 = fun.GetOrderData(dt014, "https://edi.iwaishokai.co.jp", "014", "");
                    //2019-08-09 Start
                    dt014 = fun.GetOrderData(dt014, "https://edi.iwaishokai.net", "014", "");
                    //2019-08-09 End
                    fun.GetTotalCount("014");

                    //if (!File.Exists(@"C:\Qbei_Log\014_Download\items.csv"))
                    //{
                    //    fun.Qbei_ErrorInsert(14, fun.GetSiteName("014"), "Access Denied!", "0", "0", 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "014");
                    //    fun.WriteLog("Access Denied! ", "014--");
                    //    Environment.Exit(0);
                    //}
                    //2019-08-09 Start

                    string path = "C:\\Qbei_Log\\014_Download\\";

                    string[] filelist = Directory.GetFiles(path);

                    if (!filelist[0].Contains(".csv"))
                    //if (!fname.Contains(".csv"))
                    {
                        fun.Qbei_ErrorInsert(14, fun.GetSiteName("014"), "Access Denied!", "0", "0", 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "014");
                        fun.WriteLog("Access Denied! ", "014--");
                        Environment.Exit(0);
                    }
                    //2019-08-09 End
                    else
                    {
                        ///<remark>
                        ///Inspection of item information at Mall.
                        ///</remark>

                        //2018-05-04 Start
                        //string[] str = { "商品コード", "現在庫数", "卸価格" };                           
                        //2019-08-09 Start
                        string[] str = { "商品コード", "在庫状況", "卸価格" };
                        //2019-08-09 End
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
