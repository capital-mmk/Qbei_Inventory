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

namespace _916_Chrome
{
    class Program
    {
        DataRow dr;
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt916 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;
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
        public static void testflag()
        {
            Qbeisetting_Entity entitySetting = new Qbeisetting_Entity();
            DataTable dtSetting = new DataTable();
            int intFlag;
            entitySetting.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //st = entitySetting.starttime;
            entitySetting.site = 916;
            entitySetting.flag = 1;
            dtSetting = fun.SelectFlag(916);
            intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());
            if (intFlag == 0)
            {
                fun.ChangeFlag(entitySetting);
                StartRun();
            }
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
        public static void ReadData()
        {
            try
            {
                string strCol;
                DataTable dt916 = new DataTable();
                DateTime dtDate = DateTime.Now;
                string[] strDate = new string[12];
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

                    string title = chrome.Title;
                    chrome.Url = fun.url;
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("H_TOKCD")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("H_PWDCD")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "916-");
                    chrome.FindElement(By.Id("H_LOGIN")).Click();

                    Thread.Sleep(2000);
                    fun.WriteLog("Login success             ------", "916-");

                    chrome.FindElement(By.Id("LIST040")).Click();
                    Thread.Sleep(2000);
                    fun.WriteLog("Navigation to Download Url success------", "916-");

                    chrome.FindElement(By.Id("H_BTNDLD")).Click();
                    Thread.Sleep(15000);
                    chrome.Quit();
                    if (String.IsNullOrEmpty(strParam))
                    {
                        dt916 = fun.GetDatatable("916");
                        dt916 = fun.GetOrderData(dt916, "http://222.151.239.218/akiweb/webLOGIN_Asahi.aspx", "916", "");
                    }
                    else
                        dt916 = fun.GetRerunData("916");
                    fun.GetTotalCount("916");
                    if (dt916 == null)
                    {
                        Environment.Exit(0);
                    }

                    string[] str = { "商品コード", "現在庫", "定価" };
                    DataTable dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\916_Download\", str);
                    Thread.Sleep(2000);
                    dtItem.Columns.Add(dc);
                    if (dtItem.Rows.Count > 0)
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            strCol = dtItem.Columns[dtItem.Columns.IndexOf("現在庫") + i].ColumnName;
                            strCol = strCol.Replace("'", "");
                            dtItem.Columns["'" + strCol].ColumnName = strCol;
                            strDate[i - 1] = strCol;
                        }
                    }

                    //empty + 2100-02-01
                    dtItem.AsEnumerable().Where(x => x.Field<string>("現在庫").Equals("×") && x.Field<string>(strDate[0]).Equals("×") && x.Field<string>(strDate[1]).Equals("×") && x.Field<string>(strDate[2]).Equals("×")
                        && x.Field<string>(strDate[3]).Equals("×") && x.Field<string>(strDate[4]).Equals("×") && x.Field<string>(strDate[5]).Equals("×") && x.Field<string>(strDate[6]).Equals("×") && x.Field<string>(strDate[7]).Equals("×")
                        && x.Field<string>(strDate[8]).Equals("×") && x.Field<string>(strDate[9]).Equals("×") && x.Field<string>(strDate[10]).Equals("×") && x.Field<string>(strDate[11]).Equals("×")).ToList<DataRow>()
                        .ForEach(z => z[20] = "2100-02-01");

                    //good || small + 2100-01-01
                    dtItem.AsEnumerable().Where(x => !x.Field<string>("現在庫").Equals("×") && x.Field<string>(strDate[0]).Equals("×") && x.Field<string>(strDate[1]).Equals("×") && x.Field<string>(strDate[2]).Equals("×")
                        && x.Field<string>(strDate[3]).Equals("×") && x.Field<string>(strDate[4]).Equals("×") && x.Field<string>(strDate[5]).Equals("×") && x.Field<string>(strDate[6]).Equals("×") && x.Field<string>(strDate[7]).Equals("×")
                        && x.Field<string>(strDate[8]).Equals("×") && x.Field<string>(strDate[9]).Equals("×") && x.Field<string>(strDate[10]).Equals("×") && x.Field<string>(strDate[11]).Equals("×")).ToList<DataRow>()
                        .ForEach(z => z[20] = "2100-01-01");

                    //stockdate
                    foreach (DataRow row in dtItem.AsEnumerable().Where(x => String.IsNullOrEmpty(x.Field<string>("stockdate"))).ToList<DataRow>())
                    {
                        foreach (string strTemp in strDate)
                        {
                            if (!row.Field<string>(strTemp).Contains("×"))
                            {
                                row["stockdate"] = new DateTime(DateTime.Now.Year, int.Parse(strTemp.Substring(strTemp.IndexOf('/') + 1)), DateTime.DaysInMonth(DateTime.Now.Year, int.Parse(strTemp.Substring(strTemp.IndexOf('/') + 1)))).ToString("yyyy-MM-dd");
                                break;
                            }
                        }
                    }

                    int count = dtItem.Rows.Count;
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

