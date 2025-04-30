using System;
using OfficeOpenXml;
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

namespace _019_Chrome
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt019 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;


        static void Main(string[] args)
        {
            testFlag();
        }

        public static void testFlag()
        {
            try
            {
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                DataTable dtSetting = new DataTable();

                int intFlag;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 019;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(019);
                intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());

                if (intFlag == 0)
                {
                    fun.ChangeFlag(qe);
                    startRun();
                }

                else if (intFlag == 1)
                {
                    fun.deleteData(019);
                    fun.ChangeFlag(qe);
                    startRun();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Environment.Exit(0);
            }
        }

        private static void startRun()
        {
            try
            {
                fun.setURL("019");
                fun.MoveToTrash("019");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(019);
                fun.Qbei_ErrorDelete(019);
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Environment.Exit(0);
            }
        }

        public static void ReadData()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\019_Download\");
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            chromeOptions.AddArguments("-no-sandbox");

            var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);

            using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromSeconds(30)))
            {
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                Qbei_Entity entity = new Qbei_Entity();


                qe.SiteID = 19;
                dt = qubl.Qbei_Setting_Select(qe);
                string url = dt.Rows[0]["Url"].ToString();
                chrome.Url = url;
                Thread.Sleep(2000);
                string title = chrome.Title;

                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Name("loginId")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Name("password")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "019-");
                chrome.FindElement(By.Id("login")).Click();
                Thread.Sleep(8000);


                string orderCode = string.Empty;
                string body = chrome.FindElement(By.TagName("body")).Text;

                if (body.Contains("入力された会員ID、パスワードは不正です。"))
                {
                    fun.WriteLog("Login Failed", "019-");

                    chrome.Quit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "019-");
                    chrome.Navigate().GoToUrl("https://webcart.fukaya-nagoya.co.jp/consumer/productDownload/index");
                }

                chrome.FindElement(By.Id("fnishedType1")).Click();
                chrome.FindElement(By.CssSelector("button.btn:nth-child(1)")).Click();
                Thread.Sleep(5000);

                int counter = 0;
            label:
                if (counter < 10)
                {
                    string[] filelist = Directory.GetFiles(@"C:\Qbei_Log\019_Download\");
                    foreach (string file in filelist)
                    {
                        string ext = Path.GetFileName(file);
                        goto label1;
                    }
                    Thread.Sleep(8000);
                    counter++;
                    goto label;
                }

            label1:
                fun.WriteLog("Navigation to Download success------", "019-");

                string[] flist = Directory.GetFiles(@"C:\Qbei_Log\019_Download\");
                String filename = flist[0].ToString();
                string csvPath = filename;

                DataTable dt019 = fun.GetDatatable("019");
                fun.GetTotalCount("019");
                int Count = dt019.Rows.Count;
                if (dt019 == null)
                {
                    chrome.Quit();
                    Environment.Exit(0);
                }

                DataTable dtItem = XlsxToDataTable(csvPath);
                int count = dtItem.Rows.Count;

                fun.WriteLog("Download success match with datatable------", "019-");
                fun.Qbei_Insert_XML(dt019, dtItem, "Qbei_Insert_Xml_019");
                fun.WriteLog("Insert data to db success------", "019-");
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 2;
                qe.site = 19;
                fun.ChangeFlag(qe);
                chrome.Quit();
                Environment.Exit(0);
            }
        }
        static DataTable XlsxToDataTable(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Important for EPPlus 5+     
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet        
                var dt = new DataTable();
                // Assume the first row has column names        
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                { dt.Columns.Add(firstRowCell.Text); }

                // Start loading data from row 2        
                for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                    DataRow row = dt.NewRow();
                    int i = 0;
                    foreach (var cell in wsRow)
                    {
                        row[i++] = cell.Text;
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
        }

    }
}
