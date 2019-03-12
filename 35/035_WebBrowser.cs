using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace _35
{
    public partial class Form1 : Form
    {
        IWebDriver firebox;
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        public static CommonFunction fun = new CommonFunction();
        Qbei_Entity entity = new Qbei_Entity();
        public static string st = string.Empty;
        int i = 0;
        DataTable dtItem;
        public System.Windows.Forms.Timer timer1;
        string strParam = String.Empty;
        public Form1()
        {
            InitializeComponent();
            testflag();
        }
        private void testflag()
        {
            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.site = 35;
            //st = qe.starttime;
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(35);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {


                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(35);
                fun.ChangeFlag(qe);
                StartRun();
            }
            else
            {
                Environment.Exit(0);

            }
        }

        public void StartRun()
        {
            try
            {
                fun.setURL("035");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(35);
                fun.Qbei_ErrorDelete(35);
                fun.MoveToTrash("035");
                //  dt035 = fun.GetDatatable("035");
                ReadData();
            }
            catch (Exception) { }
        }


        public void ReadData()
        {
            try
            {
                FirefoxProfile profile = new FirefoxProfile();

                profile.SetPreference("browser.download.folderList", 2);
                profile.SetPreference("browser.download.dir", @"C:\Qbei_Log\035_Download\");
                profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "text/csv");
                profile.SetPreference("pdfjs.disabled", true);  // disable the built-in PDF viewer
                //using (IWebDriver firefox = new FirefoxDriver(profile))
                using (firebox = new FirefoxDriver(profile))
                //firebox = new FirefoxDriver(profile);
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    qe.SiteID = 35;
                    dt = qubl.Qbei_Setting_Select(qe);
                    string url = dt.Rows[0]["Url"].ToString();
                    firebox.Url = url;
                    string title = firebox.Title;

                    string username = dt.Rows[0]["UserName"].ToString();
                    firebox.FindElement(By.Id("login_id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    firebox.FindElement(By.Id("login_pw")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "035-");
                    firebox.FindElement(By.Name("login")).Submit();


                    Thread.Sleep(2000);
                    fun.WriteLog("Login success             ------", "035-");
                    firebox.FindElement(By.ClassName("no_toggle")).Click();
                    firebox.Navigate().GoToUrl(" https://intertecinc.jp/ecuser/");
                    url = firebox.Url.ToString();
                    // https://intertecinc.jp/ecuser/
                    Thread.Sleep(2000);

                    firebox.Navigate().GoToUrl("https://intertecinc.jp/ecuser/item/itemList");
                    Thread.Sleep(2000);
                    // System.Windows.Forms.Timer timer1;
                    url = firebox.Url.ToString();
                    IJavaScriptExecutor ex = (IJavaScriptExecutor)firebox;
                    ex.ExecuteScript("scroll(0, 250)");
                    DataTable dt035 = fun.GetDatatable("035");
                    dt035 = fun.GetOrderData(dt035, "https://store.intertecinc.jp/intertec/mypage/main_page", "035", "");
                    fun.GetTotalCount("035");
                    string[] str = { "在庫記号", "次回入荷予定日付" };
                    SelectElement ss = new SelectElement(firebox.FindElement(By.Id("itemStockDownloadBrandCd")));
                    foreach (IWebElement element in ss.Options)
                    {
                        if (element.Text == "BELL")
                        {
                            ss.SelectByValue("A01");
                            Thread.Sleep(2000);
                            try
                            {
                                firebox.FindElement(By.TagName("button")).Click();
                            }
                            catch
                            {
                            }
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "BLACKBURN")
                        {
                            ss.SelectByValue("A02");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "MAXXIS")
                        {
                            ss.SelectByValue("A03");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "AVEX")
                        {
                            ss.SelectByValue("A04");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "TIFOSI")
                        {
                            ss.SelectByValue("A05");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "BOOKMAN")
                        {

                            ss.SelectByValue("A06");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "SWIFTWICK")
                        {

                            ss.SelectByValue("A07");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();

                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "SP CONNECT")
                        {

                            ss.SelectByValue("A08");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();
                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "WAHOO")
                        {

                            ss.SelectByValue("A09");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();

                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "BELL APPAREL")
                        {

                            ss.SelectByValue("A10");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();

                            Thread.Sleep(7000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                        else if (element.Text == "STAGES")
                        {

                            ss.SelectByValue("A11");
                            Thread.Sleep(2000);
                            firebox.FindElement(By.TagName("button")).Click();

                            Thread.Sleep(5000);
                            dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);
                        }
                    }
                    // dtItem = fun.GetDatatableFromDownloadPath(@"C:\Qbei_Log\035_Download\", str);

                    Thread.Sleep(2000);

                    int count = dtItem.Rows.Count;

                    // dt035 = fun.GetOrderData(dt035, "https://intertecinc.jp/ecuser/item/itemDetail?itemCd=", "035", "");
                    fun.WriteLog("Download success match with datatable------", "035-");
                    if ((!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_AVEX.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_BELL　APPAREL.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_BELL.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_BLACKBURN.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_BOOKMAN.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_MAXXIS.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_SP　CONNECT.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_STAGES.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_SWIFTWICK.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_TIFOSI.csv")) || (!File.Exists(@"C:\Qbei_Log\035_Download\zaiko_WAHOO.csv")))
                    {
                        fun.Qbei_ErrorInsert(35, fun.GetSiteName("035"), "Access Denied!", "0", "0", 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "035");
                        fun.WriteLog("Access Denied! ", "035--");
                        Environment.Exit(0);
                    }
                    else
                    {
                        //fun.Qbei_Insert_XML(dt035, dtItem, "Qbei_Insert_Xml_35");
                        fun.Qbei_Insert_XML(dt035, dtItem, "Qbei_Insert_Xml_35", strParam);
                        fun.WriteLog("Insert data to db success------", "035-");
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        qe.flag = 2;
                       
                        qe.site = 35;
                        fun.ChangeFlag(qe);
                        firebox.Quit();
                        //  firebox.Close();
                        Application.Exit();
                        Environment.Exit(0);

                    }
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "035-");
                Application.Exit();
            }

        }

    }


}


