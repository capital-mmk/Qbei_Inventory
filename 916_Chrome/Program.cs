using System;
using System.Collections.Generic;
using System.Linq;
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

namespace _916_Chrome
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt916 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;

        /// <summary>
        /// System(Start).
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        /// </summary>
        /// <param name="args">constant string</param>
        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                strParam = args[0].ToString();
                StartRun();
            }
            else
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
            entitySetting.site = 916;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(916);
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
                fun.deleteData(916);
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
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("916");
                fun.MoveToTrash("916");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(916);
                fun.Qbei_ErrorDelete(916);

                ReadData();

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "916-");
            }
        }

        /// <summary>
        /// Use to ChormeDriver and Read to Data at Download CSV.
        /// </summary>
        public static void ReadData()
        {
            try
            {
                DataTable dt916 = new DataTable();
                DataColumn dc = new DataColumn("stockdate");
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddUserProfilePreference("download.default_directory", @"C:\Qbei_Log\916_Download\");
                chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
                chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                using (IWebDriver chrome = new ChromeDriver(chromeOptions))
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    qe.SiteID = 916;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("H_TOKCD")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("H_PWDCD")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "916-");
                    chrome.FindElement(By.Id("H_LOGIN")).Click();

                    Thread.Sleep(2000);
                    string body = chrome.FindElement(By.Id("H_MSGCM")).GetAttribute("value").ToString();
                    if (body.Contains("ID または パスワード が違います。"))
                    {
                        fun.WriteLog("Login Failed", "916-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "916-");
                    }                   
                    chrome.FindElement(By.Id("ORDER010_1")).Click();
                    Thread.Sleep(2000);
                    fun.WriteLog("Navigation to Download Url success------", "916-");

                    chrome.FindElement(By.Id("H_BTNDLD")).Click();
                    Thread.Sleep(15000);
                    chrome.Quit();
                    if (String.IsNullOrEmpty(strParam))
                    {
                        dt916 = fun.GetDatatable("916");                      
                        //dt916 = fun.GetOrderData(dt916, "http://222.151.239.218/akiweb/webLOGIN_Asahi.aspx", "916", "");                        
                    }
                    else
                        dt916 = fun.GetRerunData("916");
                    fun.GetTotalCount("916");
                    if (dt916 == null)
                    {
                        Environment.Exit(0);
                    }

                    string[] str = { "商品コード", "在庫数", "単価" };
                    DataTable dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\916_Download\", str);
                    Thread.Sleep(2000);
                    dtItem.Columns.Add(dc);

                    //<remark Change Stockdate 2020/07/13 Start>
                    //empty + 2100-02-01
                    //dtItem.AsEnumerable().Where(x => string.IsNullOrEmpty(x.Field<string>("次回入荷")) && x.Field<string>("在庫数").Equals("×")).ToList<DataRow>()
                    //      .ForEach(z => z[dc] = "2100-02-01");
                    dtItem.AsEnumerable().Where(x => string.IsNullOrEmpty(x.Field<string>("次回入荷")) && (x.Field<string>("在庫数").Equals("×") ||
                                                                           x.Field<string>("在庫数").Equals("△"))).ToList<DataRow>()
                         .ForEach(z => z[dc] = "2100-02-01");

                    //good || small + 2100-01-01
                    //dtItem.AsEnumerable().Where(x => string.IsNullOrEmpty(x.Field<string>("次回入荷")) &&
                    //                                                      (x.Field<string>("在庫数").Equals("◎") ||
                    //                                                       x.Field<string>("在庫数").Equals("○") ||
                    //                                                       x.Field<string>("在庫数").Equals("△"))).ToList<DataRow>()
                    dtItem.AsEnumerable().Where(x => string.IsNullOrEmpty(x.Field<string>("次回入荷")) &&
                                                                          (x.Field<string>("在庫数").Equals("◎") ||
                                                                           x.Field<string>("在庫数").Equals("○") )).ToList<DataRow>()
                          .ForEach(z => z[dc] = "2100-01-01");
                    //</remark 2020/07/13 End>

                    //stockdate
                    int currentYear = DateTime.Now.Year;
                    int currentMonth = 0;
                    int currentDay = 0;
                    string stockDate = string.Empty;
                    foreach (DataRow row in dtItem.AsEnumerable().Where(x => !String.IsNullOrEmpty(x.Field<string>("次回入荷"))).ToList<DataRow>())
                    {
                        stockDate = row["次回入荷"].ToString();
                        currentMonth = int.Parse(Regex.Replace(stockDate.Substring(stockDate.IndexOf('/')), "[^0-9]+", string.Empty));

                        if (stockDate.Contains("下旬"))
                            currentDay = 10;
                        else if (stockDate.Contains("中旬"))
                            currentDay = 20;
                        else if (stockDate.Contains("上旬"))
                            currentDay = currentMonth == 2 ? 28 : 30;

                        row["stockdate"] = new DateTime(currentYear, currentMonth, currentDay).ToString("yyyy-MM-dd");
                    }


                    ///<remark>
                    ///Inspection of item information at Mall.
                    ///</remark>
                    fun.WriteLog("Download success match with datatable------", "916-");
                    fun.Qbei_Insert_XML(dt916, dtItem, "Qbei_Insert_Xml_916", strParam);
                    fun.WriteLog("Insert data to db success------", "916-");
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    qe.flag = 2;
                    qe.site = 916;
                    fun.ChangeFlag(qe);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "916-");
                Environment.Exit(0);
            }
        }
    }
}
