using System;
using System.Data;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Common;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _037
{
    class Program
    {

        DataRow dr;
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt037 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static void Main(string[] args)
        {
            testFlag();
        }
        public static void testFlag()
        {
            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            DataTable dtSetting = new DataTable();

            int intFlag;
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 037;
            qe.flag = 1;
            dtSetting = fun.SelectFlag(037);
            intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());
            if (intFlag == 0)
            {
                fun.ChangeFlag(qe);
                startRun();
            }
            else if (intFlag == 1)
            {
                fun.deleteData(037);
                fun.ChangeFlag(qe);
                startRun();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private static void startRun()
        {
            try
            {
                fun.setURL("037");
                fun.MoveToTrash("037");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(037);
                fun.Qbei_ErrorDelete(037);
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "037-");
            }
        }


        public static void ReadData()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\037_Download\");
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            using (IWebDriver chrome = new ChromeDriver(chromeOptions))
            {
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                qe.SiteID = 37;
                dt = qubl.Qbei_Setting_Select(qe);
                string url = dt.Rows[0]["Url"].ToString();
                chrome.Url = url;
                string title = chrome.Title;

                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Name("id")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Name("pw")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "037-");
                chrome.FindElement(By.XPath("/html/body/div/div[1]/div/form/div/div[7]/a")).Submit();
                url = chrome.Url.ToString();
                Thread.Sleep(2000);
                fun.WriteLog("Login success             ------", "037-");
                chrome.Navigate().GoToUrl("https://ec.tsss.co.jp/aec/user/itemstock_download_call");
                // chrome.Navigate().GoToUrl("https://ec.tsss.co.jp/aec/user/csv_export_download_info?schedule_id=csv_export_49c35718d482aecec14e92168a955b66");
                url = chrome.Url.ToString();
                Thread.Sleep(8000);
                var progress_value = chrome.FindElement(By.Id("export-progress")).GetAttribute("value");
                //string txt = chrome.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/article/section/section/table/tbody/tr[2]/td/div[2]/a")).Text;
                if (int.Parse(progress_value) < 100)
                {
                    Thread.Sleep(15000);
                }

                chrome.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/article/section/section/table/tbody/tr[2]/td/div[2]/a")).Click();

                DataTable dt037 = fun.GetDatatable("037");
                dt037 = fun.GetOrderData(dt037, "https://ec.tsss.co.jp/aec/user/csv_export_download_info?schedule_id=csv_export_49c35718d482aecec14e92168a955b66", "037", "");
                fun.GetTotalCount("037");
                string[] str = { "在庫数量", "商品コード", "下代単価" };
                DataTable dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\037_Download\", str);

                Thread.Sleep(2000);

                int count = dtItem.Rows.Count;
                fun.WriteLog("Download success match with datatable------", "037-");
                fun.Qbei_Insert_XML(dt037, dtItem, "Qbei_Insert_Xml_37");
                fun.WriteLog("Insert data to db success------", "037-");
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 2;
                qe.site = 37;
                fun.ChangeFlag(qe);
                chrome.Quit();
                Environment.Exit(0);

            }

        }

    }


}
