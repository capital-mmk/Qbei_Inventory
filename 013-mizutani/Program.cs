using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;
using Wait = OpenQA.Selenium.Support.UI;
using System.Runtime.Serialization;

namespace _013_mizutani
{
    static class Program
    {
        static Qbei_Entity entity = new Qbei_Entity();
        static DataTable dt013 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        static ChromeOptions option = new ChromeOptions();
        static ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        static IWebDriver chrome;//= new ChromeDriver(service, option,TimeSpan.FromMinutes(3));
        static string strParam = string.Empty;
        public static string st = string.Empty;
        private static readonly TimeSpan TIMEOUT = new TimeSpan(0, 3, 0);
        static DataTable dt = new DataTable();
        static Qbeisetting_BL qubl = new Qbeisetting_BL();
        static Qbeisetting_Entity qe = new Qbeisetting_Entity();
        static Wait.WebDriverWait wait;
        static int i = -1;
        static void Main(string[] args)
        {            
                testflag();
        }
        private static void testflag()
        {
            qe.site = 13;

            qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            qe.flag = 1;
            DataTable dtflag = fun.SelectFlag(13);
            int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
            if (flag == 0)
            {

                fun.ChangeFlag(qe);
                StartRun();
            }
            else if (flag == 1)
            {
                fun.deleteData(13);
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
          
            fun.setURL("013");
            fun.CreateFileAndFolder();
            fun.Qbei_Delete(13);
            fun.Qbei_ErrorDelete(13);
            dt013 = fun.GetDatatable("013");
            dt013 = fun.GetOrderData(dt013, "https://www.ordermz.jp/weborder/SyohinSearch.aspx", "013", string.Empty);
            fun.GetTotalCount("013");
            ReadData();
        }
        private static void ReadData()
        {
            qe.SiteID = 13;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            chrome = new ChromeDriver();
            chrome.Navigate().GoToUrl("https://www.ordermz.jp/weborder");


            wait = new Wait.WebDriverWait(chrome, TIMEOUT);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnLogin")));
            fun.WriteLog("Navigation to Site Url success------", "013-");
            qe.SiteID = 13;
            dt = qubl.Qbei_Setting_Select(qe);
            string username = dt.Rows[0]["UserName"].ToString();
            chrome.FindElement(By.Id("tokuisakicode")).SendKeys(username);
            string password = dt.Rows[0]["Password"].ToString();
            chrome.FindElement(By.Id("loginpasswd")).SendKeys(password);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnLogin")));
            chrome.FindElement(By.Id("btnLogin")).Click();
            //WAIT
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnSyohincodeOrder")));
            chrome.FindElement(By.Id("btnSyohincodeOrder")).Click();

            string body = chrome.FindElement(By.TagName("body")).Text;
            if (body.Contains(" 得意先コード、パスワードが正しくありません"))
            {
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                Environment.Exit(0);
            }
            else
            {
                fun.WriteLog("Login success             ------", "013-");
                chrome.Navigate().GoToUrl(fun.url + "/SyohincodeOrder.aspx");
                if (chrome.Url.Contains("SyohincodeOrder.aspx"))
                {
                    ItemSearch();
                }
            }
        }
        private static bool IsDate(string date)
        {
            try
            {
                Convert.ToDateTime(date.Trim());
                return true;
            }
            catch (Exception)
            { return false; }
        }
        private static void ItemSearch()
        {
            string orderCode = string.Empty;
            try
            {
                int count = dt013.Rows.Count - 1;
                if (count > 0)
                {
                    for (int j = i; j < count; j++)
                    {
                        try
                        {
                            orderCode = dt013.Rows[++i]["発注コード"].ToString().Trim();
                            IJavaScriptExecutor js = (IJavaScriptExecutor)chrome;
                            js.ExecuteScript("document.getElementById('gvSyohin_ctl02_syohincode').value='" + orderCode + "';");
                            js.ExecuteScript("javascript:setTimeout(__doPostBack('gvSyohin$ctl02$syohincode',''), 0);");
                            System.Threading.Thread.Sleep(1600);

                            entity = new Qbei_Entity();
                            string color = string.Empty;
                            string node = string.Empty;
                            string qty = string.Empty;
                            entity.janCode = dt013.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt013.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt013.Rows[i]["最終反映日"].ToString();
                            entity.orderCode = dt013.Rows[i]["発注コード"].ToString().Trim();
                            entity.purchaseURL = chrome.Url;
                            entity.stockDate = string.Empty;

                            string body = chrome.FindElement(By.TagName("body")).Text;
                            entity.siteID = 13;
                            entity.sitecode = "013";
                            if (body.Contains(("検索条件に該当する商品は、見つかりませんでした")) || body.Contains(("WEB販売対象外の商品です")) || body.Contains(("該当商品は存在しません")))
                            {
                                entity.qtyStatus = "empty";
                                entity.price = dt013.Rows[i]["下代"].ToString();
                                if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10")))
                                {
                                    entity.stockDate = "2100-01-10";
                                }
                                else
                                { entity.stockDate = "2100-02-01"; }
                                fun.Qbei_Inserts(entity);
                            }
                            else
                            {
                                
                                if (chrome.FindElement(By.Id("gvSyohin_ctl02_zaikojokyo")) == null && (chrome.FindElement(By.XPath("div[3]/div[6]/div/table/tbody/tr[2]")) == null))
                                {
                                    fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                                    fun.WriteLog("Access Denied! " + entity.orderCode, "013-");
                                    Environment.Exit(0);
                                }
                                else
                                {

                                    if (chrome.FindElement(By.Id("gvSyohin_ctl02_syohincode")).GetAttribute("value").Equals(entity.orderCode))
                                    {
                                        qty = chrome.FindElement(By.Id("gvSyohin_ctl02_zaikojokyo")).Text;

                                        entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("▲") ? "small" : qty.Equals("×") || qty.Equals("☆") ? "empty" : qty.Equals("★") || qty.Equals("？") ? "inquiry" : "unknown status";

                                        entity.price = chrome.FindElement(By.Id("gvSyohin_ctl02_hanbaikakakuzeibetu")).Text;
                                        entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty).Replace("円", string.Empty);

                                        node = chrome.FindElement(By.Id("gvSyohin_ctl02_nyukayotei")).Text;

                                        if (node != "")
                                            entity.stockDate = chrome.FindElement(By.Id("gvSyohin_ctl02_nyukayotei")).Text;

                                        color = chrome.FindElement(By.Id("gvSyohin_ctl02_zaikojokyo")).GetAttribute("style");
                                    }

                                    if (IsDate(entity.stockDate))
                                        entity.stockDate = entity.stockDate.Replace("/", "-");
                                    else if (color.Contains("red"))
                                    {
                                        if (qty.Equals("▲") || qty.Equals("×"))
                                            entity.stockDate = "2100-02-01";
                                        else if (qty.Equals("★") || qty.Equals("？"))
                                            entity.stockDate = "2100-01-01";
                                    }
                                    else
                                    {
                                        entity.stockDate = qty.Equals("○") || qty.Equals("▲") || qty.Equals("×") ? "2100-01-01" : entity.stockDate;
                                    }

                                    if (entity.stockDate.Contains("月中旬") || entity.stockDate.Contains("月上旬"))
                                    {
                                        entity.stockDate = entity.stockDate.Replace("次回", "").Replace("入荷", "");
                                        string day = string.Empty;
                                        if (entity.stockDate.Contains("中旬"))
                                            day = "20";
                                        else if (entity.stockDate.Contains("上旬") || entity.stockDate.Contains("月予定"))
                                            day = "10";
                                        else if (entity.stockDate.Contains("下旬"))
                                        {
                                            if (entity.stockDate.Contains("2月"))
                                                day = "28";
                                            day = "30";
                                        }


                                        else day = "25";

                                        string month = entity.stockDate.Split('月')[0];

                                        string year = DateTime.Now.ToString("yyyy");

                                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                                        if (dt < DateTime.Now)
                                            dt = dt.AddYears(1);

                                        entity.stockDate = dt.ToString("yyyy-MM-dd");

                                    }

                                    else if (entity.stockDate.Contains("月末～"))
                                    {
                                        entity.stockDate = "未定(=2100-01-01)";
                                    }

                                    else if (entity.stockDate.Contains("月末"))
                                    {
                                        string day = "25";
                                        string month = entity.stockDate.Replace("月末", string.Empty).Replace("予定", string.Empty);

                                        string year = DateTime.Now.ToString("yyyy");

                                        DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);

                                        if (dt < DateTime.Now)
                                            dt = dt.AddYears(1);

                                        entity.stockDate = dt.ToString("yyyy-MM-dd");
                                    }

                                    else if (entity.stockDate.Contains("未定"))
                                    {
                                        entity.stockDate = "2100-01-01";
                                    }

                                    else if ((qty.Equals("☆")) && string.IsNullOrWhiteSpace(entity.stockDate))
                                    { 
                                        entity.stockDate = "2100-01-10";
                                    }                   
                     
                                    else if (entity.stockDate.Contains("在庫限り"))
                                        entity.stockDate = "2100-02-01";
                                    //2018-08-14 Start
                                    else if (entity.stockDate.Contains("月以降"))
                                    {
                                        entity.stockDate = Regex.Replace(entity.stockDate, "[^0-9]", string.Empty);
                                        entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(entity.stockDate), 30).ToString("yyyyMMdd");
                                    }
                                    //2018-08-14 End
                                    entity.stockDate = entity.stockDate.Replace("/", "-");

                                    if ((dt013.Rows[i]["在庫情報"].ToString().Contains("empty") || dt013.Rows[i]["在庫情報"].ToString().Contains("inquriry")) && dt013.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                    {
                                        if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                        {
                                            entity.qtyStatus = dt013.Rows[i]["在庫情報"].ToString();
                                            entity.price = dt013.Rows[i]["下代"].ToString();
                                            entity.stockDate = dt013.Rows[i]["入荷予定"].ToString();
                                        }
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                        //2018/1/12
                                        fun.Qbei_Inserts(entity);
                                }
                            }

                            //クリアボタン
                            js.ExecuteScript("javascript:__doPostBack('gvSyohin','clearRow$0');");
                            System.Threading.Thread.Sleep(500);

                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                            fun.WriteLog(ex.Message + ex.StackTrace + entity.orderCode, "013-");
                        }
                    }
                }
                qe.site = 13;
                qe.flag = 2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                chrome.Dispose();
                chrome.Quit();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                fun.Qbei_ErrorInsert(13, fun.GetSiteName("013"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "013");
                fun.WriteLog(ex.Message + entity.orderCode, "013-");
                Environment.Exit(0);
                chrome.Dispose();
                chrome.Quit();
            }
        }
    }
}
