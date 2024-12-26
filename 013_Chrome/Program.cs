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
using LumenWorks.Framework.IO.Csv;


namespace _013_Chrome
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt013 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static int i;
        public static string ordercode;
        DataRow dr;

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
                qe.site = 013;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(013);
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
                    fun.ChangeFlag(qe);
                    startRun();
                }

                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (intFlag == 1)
                {
                    fun.deleteData(013);
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
                fun.WriteLog(ex, "013-");
                Environment.Exit(0);
            }
        }

        private static void startRun()
        {
            try
            {
                fun.setURL("013");
                fun.MoveToTrash("013");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(013);
                fun.Qbei_ErrorDelete(013);
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "013-");
                Environment.Exit(0);
            }
        }

        public static void ReadData()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\013_Download\");
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            chromeOptions.AddArguments("-no-sandbox");
            var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);

            using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))
            {
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                qe.SiteID = 13;
                dt = qubl.Qbei_Setting_Select(qe);
                fun.url = dt.Rows[0]["Url"].ToString();

                chrome.Url = fun.url;
                Thread.Sleep(4000);

                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Id("tokuisakicode")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Id("loginpasswd")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "013-");
                chrome.FindElement(By.Id("btnLogin")).Click();
                Thread.Sleep(2000);

                string body = chrome.FindElement(By.TagName("body")).Text;
                if (body.Contains("得意先コード、パスワードが正しくありません"))
                {
                    fun.WriteLog("Login Failed", "013-");
                    chrome.Quit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "013-");
                    chrome.Navigate().GoToUrl(fun.url + "/SyohinOrderCapture.aspx");
                }

                chrome.FindElement(By.Id("rblImportKey2")).Click();
                chrome.FindElement(By.Id("btnZaikoExport")).Click();
                chrome.SwitchTo().Alert().Accept();
                Thread.Sleep(10000);

                int counter = 0;
            label:
                if (counter < 10)
                {
                    string[] filelist = Directory.GetFiles(@"C:\Qbei_Log\013_Download\");
                    foreach (string file in filelist)
                    {
                        string ext = Path.GetFileName(file);
                        goto label1;
                    }
                    Thread.Sleep(3000);
                    counter++;
                    goto label;
                }

            label1:
                fun.WriteLog("Navigation to Download success------", "013-");
                
                string[] flist = Directory.GetFiles(@"C:\Qbei_Log\013_Download\");
                
                String filename = flist[0].ToString();
                string csvPath = filename; 

                // Step 1: Convert CSV to DataTable without headers
                DataTable dataTable = ConvertCSVToDataTableWithoutHeaders(csvPath);

                // Step 2: Add custom headers
                AddCustomHeaders(dataTable, new string[] { "商品コード", "JANコード", "在庫数量", "StockDate" });
                
                DataTable dtItem = new DataTable();
                dtItem = dataTable;

                DataTable dt013 = fun.GetDatatable("013");
                fun.GetTotalCount("013");

                fun.WriteLog("Download success match with datatable------", "013-");
                fun.Qbei_Insert_XML(dt013, dtItem, "Qbei_Insert_Xml_13");
                fun.WriteLog("Insert data to db success------", "013-");
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 2;
                qe.site = 13;
                fun.ChangeFlag(qe);
                chrome.Quit();
                Environment.Exit(0);

            }

        }
        

        static DataTable ConvertCSVToDataTableWithoutHeaders(string filePath)
        {
            DataTable dataTable = new DataTable();

            using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding(932)))
            {
                // Assume the number of columns is based on the first row
                string firstLine = reader.ReadLine().Replace("'", String.Empty);
                string[] values = firstLine.Split(',');

                // Dynamically create columns without headers
                for (int i = 0; i < values.Length; i++)
                {
                    dataTable.Columns.Add($"Column{i + 1}"); // Default column names: Column1, Column2, etc.
                }
                
                // Add the first line as a row
                dataTable.Rows.Add(values);

                // Add remaining rows
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Replace("'", String.Empty);
                    string[] rowValues = line.Split(',');
                    dataTable.Rows.Add(rowValues);
                }
            }

            return dataTable;
        }

        static void AddCustomHeaders(DataTable table, string[] newHeaders)
        {
            if (table.Columns.Count != newHeaders.Length)
            {
                throw new Exception("The number of new headers must match the number of columns.");
            }

            for (int i = 0; i < newHeaders.Length; i++)
            {
                table.Columns[i].ColumnName = newHeaders[i];
            }
        }
        
    }
}
