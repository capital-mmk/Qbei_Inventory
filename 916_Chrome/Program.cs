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
using HtmlAgilityPack;
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
        private static int i;
        public static string ordercode;

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
            DataTable dt916 = new DataTable();
            try
            {
                if (!String.IsNullOrEmpty(strParam))
                    st = DateTime.Now.ToString();
                fun.setURL("916");
                fun.Qbei_Delete(916);
                fun.Qbei_ErrorDelete(916);
                dt916 = fun.GetDatatable("916");
                fun.GetTotalCount("916");
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "916-");
            }
            try
            {
                var chromeOptions = new ChromeOptions();
                using (IWebDriver chrome = new ChromeDriver(chromeOptions))
                {
                    DataTable dt = new DataTable();
                    Qbeisetting_BL qubl = new Qbeisetting_BL();
                    Qbeisetting_Entity qe = new Qbeisetting_Entity();
                    Qbei_Entity entity = new Qbei_Entity();
                    int Month;
                    string Day;
                    string strStockDate = string.Empty;
                    int pcmonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    qe.SiteID = 916;
                    dt = qubl.Qbei_Setting_Select(qe);
                    fun.url = dt.Rows[0]["Url"].ToString();

                    chrome.Url = fun.url;
                    string username = dt.Rows[0]["UserName"].ToString();
                    chrome.FindElement(By.Name("customer_login_id")).SendKeys(username);
                    string password = dt.Rows[0]["Password"].ToString();
                    chrome.FindElement(By.Name("password")).SendKeys(password);
                    fun.WriteLog("Navigation to Site Url success------", "916-");
                    chrome.FindElement(By.XPath("/html/body/div/main/section/div/div[1]/div/form/button")).Click();                    
                    Thread.Sleep(2000); 
                    
                    string body = chrome.FindElement(By.TagName("body")).Text;
                    if (body.Contains("認証に失敗しました。"))
                    {
                        fun.WriteLog("Login Failed", "916-");
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        fun.WriteLog("Login success             ------", "916-");
                    }


                    try
                    {
                        int Lastrow = dt916.Rows.Count;
                        for (i = 0; i < Lastrow; i++)
                        {
                            if (i < Lastrow)
                            {                                               
                                ordercode = dt916.Rows[i]["JANコード"].ToString();
                                chrome.FindElement(By.Name("header_product_code")).Clear();
                                chrome.FindElement(By.Name("header_product_code")).SendKeys(ordercode);
                                chrome.FindElement(By.XPath("/html/body/header/div/nav/ul/li[2]/form/button")).Click();

                                entity = new Qbei_Entity();
                                entity.siteID = 916;
                                entity.sitecode = "916";
                                entity.janCode = dt916.Rows[i]["JANコード"].ToString();
                                entity.partNo = dt916.Rows[i]["自社品番"].ToString();
                                entity.makerDate = fun.getCurrentDate();
                                entity.reflectDate = dt916.Rows[i]["最終反映日"].ToString();
                                entity.orderCode = dt916.Rows[i]["発注コード"].ToString();
                                entity.purchaseURL = "https://btob.asahi-wsd.jp/website/asahi/product/list";

                                //<remark>
                                //Check to Ordercode
                                //</remark>
                                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                                {
                                    if (!chrome.FindElement(By.Id("productsList")).GetAttribute("innerHTML").Contains("products-list-item"))
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt916.Rows[i]["下代"].ToString();
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        chrome.FindElement(By.ClassName("products-img")).Click();
                                        //string m = chrome.FindElement(By.ClassName("product-sku")).GetAttribute("innerHTML");
                                        //var v = chrome.FindElement(By.ClassName("product-sku")).GetAttribute("innerHTML");
                                        int n = chrome.FindElements(By.XPath("/html/body/div[2]/main/ul/li")).Count();
                                      
                                        if (n == 0)
                                        {
                                            entity.qtyStatus = "empty";
                                            entity.stockDate = "2100-02-01";
                                            entity.price = dt916.Rows[i]["下代"].ToString();
                                            fun.Qbei_Inserts(entity);
                                        }
                                        else
                                        {
                                            for (int i = 1; i <= n; i++)
                                            {
                                                //i =i+ 1;
                                                if (chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]")).Text.Contains(ordercode))
                                                {
                                                    entity.price = chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]/section/dl[2]/div[2]/dd/span")).Text.Replace("¥", "").Replace(",", "").Trim();
                                                    string stock = chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]/section/dl[3]/div[1]/dd")).GetAttribute("innerHTML").ToString().Trim();
                                                    entity.qtyStatus = stock.Equals("あり") ? "small" : stock.Equals("なし") ? "empty" : "unknown status";
                                                    if (chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]")).Text.Contains("メーカー入荷予定"))                                                                     
                                                    {
                                                        entity.stockDate = chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]/section/dl[3]/div[3]/dd")).GetAttribute("innerHTML").ToString().Trim();
                                                        strStockDate = chrome.FindElement(By.XPath("/html/body/div[2]/main/ul/li[" + (i) + "]/section/dl[3]/div[3]/dd")).GetAttribute("innerHTML").ToString().Trim();
                                                    }
                                                    else
                                                    { entity.stockDate = "2100-01-01"; }
                                                    if (entity.stockDate.Contains("なし"))
                                                    {
                                                        entity.stockDate = "2100-02-01";
                                                    }
                                                    else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("月"))
                                                    {
                                                        int YIndex = entity.stockDate.IndexOf('年');
                                                        int MIndex = entity.stockDate.IndexOf('月');
                                                        int Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                                                        Month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                                                        if ((Month < pcmonth) && (Year <= DateTime.Now.Year))
                                                        { Year = Year + 1; }
                                                        int Y = Convert.ToInt32(Year);
                                                        Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                        if (entity.stockDate.Contains("日"))
                                                        {
                                                            entity.stockDate = entity.stockDate.Replace("年", "-").Replace("月", "-").Replace("日", "-");
                                                        }
                                                        else if (entity.stockDate.Contains("初旬") || entity.stockDate.Contains("上旬") || entity.stockDate.Contains("上"))
                                                        {
                                                            entity.stockDate = Year + "-" + Month + "-" + "10";
                                                        }
                                                        else if (entity.stockDate.Contains("中旬") || entity.stockDate.Contains("中"))
                                                        {
                                                            entity.stockDate = Year + "-" + Month + "-" + "20";
                                                        }
                                                        else if (entity.stockDate.Contains("下旬") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末") || entity.stockDate.Contains("下"))
                                                        {
                                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                                        }
                                                        else if (entity.stockDate.Contains("月") && (entity.stockDate.Contains("ごろ") || entity.stockDate.Contains("予定")))
                                                        {
                                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                                        }
                                                        else
                                                        {
                                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                                        }
                                                    }
                                                    else if (entity.stockDate.Contains("年") && entity.stockDate.Contains("頃"))
                                                    {
                                                        int YIndex = entity.stockDate.IndexOf('年');
                                                        int Year = Convert.ToInt32(entity.stockDate.Substring(YIndex - 4, YIndex + 0));
                                                        int pcMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                                                        if (entity.stockDate.Contains("月"))
                                                        {
                                                            int MIndex = entity.stockDate.IndexOf('月');
                                                            Month = Convert.ToInt32(entity.stockDate.Substring(YIndex + 1, MIndex - 5));
                                                            if (Month < pcmonth)
                                                            { Year = Year + 1; }
                                                            int Y = Convert.ToInt32(Year);
                                                            Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                            entity.stockDate = Year + "-" + Month + "-" + Day;
                                                        }
                                                        else
                                                        {
                                                            if (entity.stockDate.Contains("春"))
                                                            {
                                                                Month = 4;
                                                                if (Month < pcMonth)
                                                                { Year = Year + 1; }
                                                                int Y = Convert.ToInt32(Year);
                                                                Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                            else if (entity.stockDate.Contains("夏"))
                                                            {
                                                                Month = 7;
                                                                if (Month < pcMonth)
                                                                { Year = Year + 1; }
                                                                int Y = Convert.ToInt32(Year);
                                                                Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                            else if (entity.stockDate.Contains("秋"))
                                                            {
                                                                Month = 10;
                                                                if (Month < pcMonth)
                                                                { Year = Year + 1; }
                                                                int Y = Convert.ToInt32(Year);
                                                                Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                            else if (entity.stockDate.Contains("冬"))
                                                            {
                                                                Month = 1;
                                                                if (Month < pcMonth)
                                                                { Year = Year + 1; }
                                                                int Y = Convert.ToInt32(Year);
                                                                Day = DateTime.DaysInMonth(Y, Month).ToString();
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                        }
                                                    }
                                                    else if (entity.stockDate.Contains("~") && entity.stockDate.Contains("月"))
                                                    {
                                                        int mIndex = entity.stockDate.IndexOf("月");
                                                        string year = DateTime.Now.ToString("yyyy");
                                                        int month2;

                                                        if (entity.stockDate.Contains("月") || entity.stockDate.Contains("予定"))
                                                        {
                                                            if (entity.stockDate.Contains("~"))
                                                            {                                                                
                                                                var D = entity.stockDate.Split('~').ToArray();
                                                                if (D[1].Contains("月"))
                                                                {
                                                                    var M = D[1].ToString().Split('月').ToArray();
                                                                    month2 = Convert.ToInt32(M[0]);
                                                                    if (month2 < pcmonth)
                                                                    {
                                                                        year = Convert.ToString(Convert.ToInt32(year) + 1);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var M = D[0].ToString().Split('月').ToArray();
                                                                    month2 = Convert.ToInt32(M[0]);
                                                                    if (month2 < pcmonth)
                                                                    {
                                                                        year = Convert.ToString(Convert.ToInt32(year) + 1);
                                                                    }
                                                                }                                                                
                                                                Day = DateTime.DaysInMonth(Convert.ToInt32(year), month2).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />                                    
                                                                entity.stockDate = year + "-" + month2 + "-" + Day;

                                                                if (D[1].Contains("初旬") || D[1].Contains("上旬") || D[1].Contains("上"))
                                                                {
                                                                    entity.stockDate = year + "-" + month2 + "-" + "10";
                                                                }
                                                                else if (D[1].Contains("中旬") || D[1].Contains("中"))
                                                                {
                                                                    entity.stockDate = year + "-" + month2 + "-" + "20";
                                                                }
                                                                else if (D[1].Contains("下旬") || D[1].Contains("末頃") || D[1].Contains("末") || D[1].Contains("下"))
                                                                {
                                                                    entity.stockDate = year + "-" + month2 + "-" + Day;
                                                                }
                                                                //</remak 2020/04/20 End>
                                                            }
                                                        }
                                                    }
                                                    else if (entity.stockDate.Contains('月'))
                                                    {
                                                        int MIndex = entity.stockDate.IndexOf("月");
                                                        string Year = DateTime.Now.ToString("yyyy");
                                                        if (MIndex == 1 || MIndex == 2)
                                                        {
                                                            if (MIndex == 1)
                                                            {
                                                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 1, MIndex + 0));
                                                                if (Month < pcmonth)
                                                                {
                                                                    Year = Convert.ToString(Convert.ToInt32(Year) + 1);
                                                                }
                                                                //Day = DateTime.DaysInMonth(DateTime.Now.Year, Month).ToString();
                                                                Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />                                   
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                            else
                                                            {
                                                                Month = Convert.ToInt32(entity.stockDate.Substring(MIndex - 2, MIndex + 0));
                                                                if (Month < pcmonth)
                                                                {
                                                                    Year = Convert.ToString(Convert.ToInt32(Year) + 1);
                                                                }                                                               
                                                                Day = DateTime.DaysInMonth(Convert.ToInt32(Year), Month).ToString();//<remak Edit Logic for Date of Day 2020/09/25 />        
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                            if (strStockDate.Contains("初旬") || strStockDate.Contains("上旬") || strStockDate.Contains("上"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + "10";
                                                            }
                                                            else if (strStockDate.Contains("中旬") || strStockDate.Contains("中"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + "20";
                                                            }
                                                            else if (strStockDate.Contains("下旬") || strStockDate.Contains("末頃") || strStockDate.Contains("末") || strStockDate.Contains("下"))
                                                            {
                                                                entity.stockDate = Year + "-" + Month + "-" + Day;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (entity.price == null || entity.qtyStatus == null || entity.stockDate == null)
                                            {
                                                entity.qtyStatus = "empty";
                                                entity.stockDate = "2100-02-01";
                                                entity.price = dt916.Rows[i]["下代"].ToString();
                                            }
                                            DateTime d = Convert.ToDateTime(entity.stockDate);
                                            if (d <= (DateTime.Now))
                                            {
                                                d = d.AddYears(1);
                                            }
                                            entity.stockDate = d.ToString("yyyy-MM-dd");
                                            fun.Qbei_Inserts(entity);
                                        }
                                    }                                   
                                }
                                else
                                {
                                    fun.Qbei_ErrorInsert(19, fun.GetSiteName("916"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "916");
                                }
                            }
                        }
                        qe.site = 916;
                        qe.flag = 2;
                        qe.starttime = string.Empty;
                        qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        fun.ChangeFlag(qe);
                        chrome.Quit();
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        string janCode = dt916.Rows[i]["JANコード"].ToString();
                        ordercode = dt916.Rows[i]["発注コード"].ToString();
                        fun.Qbei_ErrorInsert(916, fun.GetSiteName("916"), ex.Message, janCode, ordercode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "916");
                        fun.WriteLog(ex, "916-", janCode, ordercode);
                    }
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
