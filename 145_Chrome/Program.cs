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

namespace _145_Chrome
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    class Program
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>               
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt145 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        private static int i;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        static void Main(string[] args)
        {
            testFlag();
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
        public static void testFlag()
        {
            try
            {
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                DataTable dtSetting = new DataTable();

                int intFlag;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 145;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(145);
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
                    fun.deleteData(145);
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
                fun.WriteLog(ex, "145-");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        public static void startRun()
        {
            DataTable dt145 = new DataTable();
            try
            {
                fun.setURL("145");
                fun.Qbei_Delete(145);
                fun.Qbei_ErrorDelete(145);
                dt145 = fun.GetDatatable("145");
                fun.GetTotalCount("145");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "145-");
                Environment.Exit(0);
            }

            /// <summary>
            /// Use to ChormeDriver and Data Table and Common Function and Field
            /// </summary>
            var chromeOptions = new ChromeOptions();
            //chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";//<Add Logic for Chrome Path 2021/05/24 />
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");//<remark Add Logic for ChormeDriver 2021/09/02 />
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");//<remark Add Logic for ChormeDriver 2021/09/02 />
            chromeOptions.AddArguments("-no-sandbox");//<remark Add Logic for ChormeDriver 2021/09/02 />
            var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);//<remark Add Logic for ChormeDriver 2021/09/02 />                                                                                                                       
            //using (IWebDriver chrome = new ChromeDriver(chromeOptions))
            using (IWebDriver chrome = new ChromeDriver(service, chromeOptions, TimeSpan.FromMinutes(3)))//<remark Edit Logic for ChormeDriver 2021/09/02 />
            {
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                Qbei_Entity entity = new Qbei_Entity();

                /// <summary>
                /// Login of Mall.
                /// </summary>
                qe.SiteID = 145;
                dt = qubl.Qbei_Setting_Select(qe);
                string url = dt.Rows[0]["Url"].ToString();
                chrome.Url = url;
                //<remark Add Logic for Site Page at Maintain 2021/11/30 Start> >
                string check_page = chrome.FindElement(By.TagName("body")).Text;
                if (check_page.Contains("棚卸中") ||chrome.Url.Contains("stop.php"))
                {
                    int i = 0;
                    while (i <= dt145.Rows.Count - 1)
                    {
                        try
                        {
                            entity = new Qbei_Entity();
                            entity.siteID = 145;
                            entity.sitecode = "145";
                            entity.janCode = dt145.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt145.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt145.Rows[i]["最終反映日"].ToString();
                            entity.stockDate = dt145.Rows[i]["入荷予定"].ToString();
                            entity.orderCode = dt145.Rows[i]["発注コード"].ToString().Trim();// "8022530007719";                
                            entity.purchaseURL = fun.url + "/shop/g/g" + entity.orderCode + "/";
                            if (dt145.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt145.Rows[i]["在庫情報"].ToString().Contains("empty"))
                                {
                                    entity.qtyStatus = "empty";
                                    entity.stockDate = "2100-01-10";
                                    entity.price = dt145.Rows[i]["下代"].ToString();
                                    //<remark 2021/01/06>
                                    entity.True_StockDate = "Not Found";
                                    entity.True_Quantity = "Not Found";
                                    //</remark 2021/01/06>
                                    fun.Qbei_Inserts(entity);
                                }
                             else
                                {
                                entity.qtyStatus = "empty";
                                entity.stockDate = "2100-02-01";
                                entity.price = dt145.Rows[i]["下代"].ToString();
                                entity.True_StockDate = "出荷停止中";
                                entity.True_Quantity = "出荷停止中";
                                fun.Qbei_Inserts(entity);
                                }
                            ++i;
                        }
                        catch (Exception ex)
                        {
                            fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                            fun.WriteLog(ex, "145-", entity.janCode, entity.orderCode);
                        }
                    }
                    qe.site = 145;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    chrome.Quit();
                    Environment.Exit(0);
                }
                //</remark 2021/11/30 End>
                string title = chrome.Title;

                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Name("loginEmail")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Name("loginPassword")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "037-");
                chrome.FindElement(By.Name("login")).Click();
                Thread.Sleep(8000);

                /// <summary>
                /// Check Login
                /// </summary>
                string orderCode = string.Empty;
                string body = chrome.FindElement(By.TagName("body")).Text;
                if (body.Contains("入力情報に誤りがあります。入力された内容を再度ご確認ください"))
                {
                    fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), "Login Failed", dt145.Rows[0]["JANコード"].ToString(), dt145.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                    fun.WriteLog("Login Failed", "145-");

                    chrome.Quit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "145-");
                    chrome.Navigate().GoToUrl("https://manys.i10.bcart.jp/list.php?keyword=");
                    chrome.FindElement(By.CssSelector("body > div.wrapper.wrapper--column-2.wrapper--product-list.wrapper--bg > div > div > section.__control > div.__view-control > div.__select > form > button")).Click();  
                }

                /// <summary>
                /// Inspection of item information at Mall.
                /// </summary>
                try
                {
                    int Lastrow = dt145.Rows.Count;
                    for (i = 0; i < Lastrow; i++)
                    {
                        if (i < Lastrow)
                        {
                            string od;
                            //od = dt145.Rows[i]["JANコード"].ToString();//<Edit Logic for Search 2021/03/24 />
                            od = dt145.Rows[i]["発注コード"].ToString();
                            //chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);//<Edit Logic for Search 2021/03/24 />
                            chrome.Navigate().GoToUrl("https://manys.i10.bcart.jp/list.php?keyword=" + od);
                            Thread.Sleep(2000);//<reamark 追加　18/05/2021 />

                            entity = new Qbei_Entity();
                            entity.siteID = 145;
                            entity.sitecode = "145";
                            entity.janCode = dt145.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt145.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt145.Rows[i]["最終反映日"].ToString();
                            entity.orderCode = dt145.Rows[i]["発注コード"].ToString().Trim();
                            //entity.purchaseURL = "https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od;//<Edit Logic for Search 2021/03/24 />
                            entity.purchaseURL = "https://manys.i10.bcart.jp/list.php?keyword=" + od;

                            //<remark>
                            //Check to  Url is Correct 
                            //</remark>
                            //if (!chrome.Url.ToString().Contains("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1="))//<Edit Logic for Search 2021/03/24 />
                            if (!chrome.Url.ToString().Contains("https://manys.i10.bcart.jp/list.php?keyword="))

                            {
                                Thread.Sleep(50000);
                                //od = dt145.Rows[i]["JANコード"].ToString();
                                od = dt145.Rows[i]["発注コード"].ToString();
                                //chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);//<Edit Logic for Search 2021/03/24 />
                                chrome.Navigate().GoToUrl("https://manys.i10.bcart.jp/list.php?keyword=" + od);
                            }

                            //<remark>
                            //Check to  Item is Correct Data 
                            //</remark>
                            string ItemCheck;
                            ItemCheck= chrome.FindElement(By.TagName("body")).Text;
                            //try
                            //{
                            //    ItemCheck = chrome.FindElement(By.CssSelector("body > div.wrapper.wrapper--column-2.wrapper--product-list.wrapper--bg > div > div > section.__pagination.p-pagination > div > span")).Text;
                            //}
                            //catch
                            //{
                            //    //od = dt145.Rows[i]["JANコード"].ToString();
                            //    od = dt145.Rows[i]["発注コード"].ToString();
                            //    //chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);//<Edit Logic for Search 2021/03/24 />
                            //    chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&originalcode1=" + od);
                            //    Thread.Sleep(20000);
                            //    ItemCheck = chrome.FindElement(By.Id("M_total")).Text;
                            //}

                            //<remark>
                            //Check to Ordercode
                            //</remark>
                            if (!string.IsNullOrWhiteSpace(entity.orderCode))
                            {
                                if (ItemCheck.Contains("お探しの検索条件に合致する商品は見つかりませんでした。"))
                                {                                   
                                    entity.qtyStatus = "empty";
                                    entity.stockDate = "2100-02-01";
                                    entity.price = dt145.Rows[i]["下代"].ToString();
                                    //<remark 2021/01/06>
                                    entity.True_StockDate = "Not Found";
                                    entity.True_Quantity = "Not Found";
                                    //</remark 2021/01/06>
                                    fun.Qbei_Inserts(entity);//<remark Add Logic 2020/05/30 />
                                }
                                else
                                {
                                    //<remark Add Logic for Item Search 2021/05/11 Start>      
                                    int n = Convert.ToInt32(chrome.FindElements(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr")).Count());
                                    if (n == 0)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                        entity.price = dt145.Rows[i]["下代"].ToString();
                                        entity.True_StockDate = "Not Found";
                                        entity.True_Quantity = "Not Found";
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        for (int i = 1; i <= n; i++)
                                        {
                                            //<remark Add Logic for Check to Order Code 2021/12/02 Start>
                                            string check_ordercode = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[1]/span[2]")).Text.Replace("[", " ").Replace("]", " ").Trim();
                                            if (check_ordercode.Equals(od))
                                            //if (chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr["+i+"]/td[1]/span[2]")).Text.Equals("[" + od + "]"))                                            
                                            //if (chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[" + (i + 1) + "]/td[2]/p[2]")).Text.Equals(entity.orderCode))//<remark Edit Logic for check to jancode 2021/05/27 />
                                            //</remark 2021/12/02 End>
                                            {
                                                //<remark>
                                                //Check to Quantity
                                                //</remark>      
                                                string qty;
                                                try
                                                {
                                                    //</remark Add Logic for Take to Quantity 2021/12/02 Start>
                                                        qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[2]/dl/dd")).Text;
                                                    //qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;//<remark Edit Logic for Quantity 2021/05/12 />
                                                    //qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[3]/dl[1]/dd")).Text;
                                                    //<remark 2021/12/02 End>
                                                }
                                                catch
                                                {
                                                    //<remark Edit Logic for Quantity 2020/1/29 Start>
                                                    try
                                                    {
                                                        Thread.Sleep(2000);
                                                        //qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;//<remark Edit Logic for Quantity 2021/05/12 />
                                                        qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[3]/dl[1]/dd")).Text;
                                                    }
                                                    catch
                                                    {
                                                        try
                                                        {
                                                            chrome.Navigate().GoToUrl("https://manys.i10.bcart.jp/list.php?keyword=" + od);//<remark Add Logic for Stockdate 2021/04/06 />                                                                              
                                                            //qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;//<remark Edit Logic for Quantity 2021/05/12 />
                                                            qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[3]/dl[1]/dd")).Text;
                                                        }
                                                        catch
                                                        {
                                                            //<remark Add Logic for Stockdate 2021/04/08 Start>
                                                            try
                                                            {
                                                                Thread.Sleep(20000);
                                                                //qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;//<remark Edit Logic for Quantity 2021/05/12 />
                                                                qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[3]/dl[1]/dd")).Text;
                                                            }
                                                            catch
                                                            {                            
                                                                chrome.Navigate().GoToUrl("https://manys.i10.bcart.jp/list.php?keyword=" + od);//<remark Add Logic for Stockdate 2021/11/01 /> 
                                                                Thread.Sleep(20000);
                                                                //qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span")).Text;//<remark Edit Logic for Quantity 2021/05/12 />
                                                                qty = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[2]/div[3]/dl[1]/dd")).Text;
                                                            }
                                                            //<remark Add Logic for Stockdate 2021/04/08 End>
                                                        }
                                                    }
                                                    //</remark 2020/1/29 End>
                                                }

                                                //<remark>
                                                //Check to have Digits at Quantity and take Quantity of Status
                                                //</remark>
                                                bool isDigitPresent = qty.Any(c => char.IsDigit(c));
                                                if (isDigitPresent == true)
                                                {
                                                    //entity.qtyStatus = "small";//<remark Edit Logic of quantity 2020/07/21 />
                                                    entity.qtyStatus = "empty";
                                                }
                                                else
                                                {
                                                    //entity.qtyStatus = qty.Equals("在庫○") ? "good" : qty.Equals("取寄×") ? "empty" : qty.Equals("在庫△") ? "small" : qty.Equals("欠品×") ? "empty" : qty.Equals("廃番×") ? "empty" : qty.Equals("廃番(n)") ? "small" : qty.Equals("廃番○") ? "good" : "unknown status";//<remark Edit Logic of quantity 2020/07/21 />
                                                    entity.qtyStatus = qty.Equals("○") ? "good" : qty.Equals("△") ? "small" : qty.Equals("欠品中") ? "empty" : "empty";
                                                }

                                                //<remark>
                                                //Check to Price
                                                //</remark>
                                                //entity.price = chrome.FindElement(By.ClassName("price_em")).Text;//<remark Edit Logic for price 2021/05/12 />
                                                entity.price = chrome.FindElement(By.XPath("/html/body/div[1]/div/div/section[3]/ul/li/div/form/table/tbody/tr[" + i + "]/td[1]/div/span[1]/span")).Text;
                                                entity.price = entity.price.Replace("円", " ").Replace(",", string.Empty);

                                                //<remark>
                                                //Check to Stockdate
                                                //</remark>
                                                //string stockdate = chrome.FindElement(By.ClassName("r_cate_title")).Text;//<remark Edit Logic for stockdate 2021/05/12 />
                                                //string stockdate = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[" + (i + 1) + "]/td[2]")).Text;
                                                //if (stockdate.Contains("入荷予定日"))
                                                //{
                                                //    //entity.stockDate = chrome.FindElement(By.ClassName("availabilityDate")).Text;//<remark Edit Logic for stockdate 2021/05/12 />
                                                //    entity.stockDate = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[" + (i + 1) + "]/td[2]/p[5]")).Text;
                                                //    entity.stockDate = entity.stockDate.Replace("入荷予定日:", " ").Replace("/", "-");
                                                //    //entity.stockDate = entity.stockDate.Replace("/", "-");
                                                //    DateTime da = Convert.ToDateTime(entity.stockDate);
                                                //    entity.stockDate = da.ToString("yyyy-MM-dd");
                                                //    //<remark 2021/01/06>
                                                //    entity.True_StockDate = chrome.FindElement(By.ClassName("availabilityDate")).Text.Replace("入荷予定日:", " ");
                                                //    entity.True_Quantity = qty;
                                                //    //</remark 2021/01/06>
                                                //}
                                                //else
                                                //{
                                                //    //<remark>
                                                //    //Check to have Digits at Quantity and take Stockdate of Status
                                                //    //</remark>
                                                //    if (isDigitPresent == true)
                                                //    {
                                                //        entity.stockDate = "2100-02-01";
                                                //    }
                                                //    else
                                                //    {
                                                //        //entity.stockDate = qty.Equals("在庫○") ? "2100-01-01" : qty.Equals("取寄×") ? "2100-02-01" : qty.Equals("在庫△") ? "2100-01-01" : qty.Equals("欠品×") ? "2100-02-01" : qty.Equals("廃番×") ? "2100-02-01" : qty.Equals("廃番(n)") ? "2100-02-01" : qty.Equals("廃番○") ? "2100-02-01" : "unknown status";//<remark Edit Logic of stockdate 2020/07/21 />
                                                //        entity.stockDate = qty.Equals("在庫○") ? "2100-01-01" : qty.Equals("取寄×") ? "2100-02-01" : qty.Equals("在庫△") ? "2100-02-01" : qty.Equals("欠品×") ? "2100-02-01" : qty.Equals("廃番×") ? "2100-02-01" : qty.Equals("廃番(n)") ? "2100-02-01" : qty.Equals("廃番○") ? "2100-02-01" : "unknown status";
                                                //    }
                                                //    //<remark 2021/01/06>
                                                //    entity.True_StockDate = "項目無し";
                                                //    entity.True_Quantity = qty;
                                                //    //</remark 2021/01/06>
                                                //}
                                                entity.stockDate = qty.Equals("◯") ? "2100-01-01" : qty.Equals("△") ? "2100-01-01" : qty.Equals("欠品中") ? "2100-02-01" : "2100-01-01";
                                                entity.True_Quantity = qty;
                                                entity.True_StockDate = "項目無し";
                                                break;
                                            }
                                        }
                                        //</remark 2021/05/11 End>

                                        //<remark Add Logic 2021/05/27 />
                                        if ((entity.qtyStatus != "unknown status"))
                                        {
                                            if (entity.qtyStatus != null)
                                            {
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else
                                            {
                                                fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), "Jancode doesn't exit!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), "Item doesn't Check!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                                        }
                                        //<remark Add Logic 2021/05/27 />
                                    }
                                    //<remark>
                                    //Insert to QbeiTable of Statuss
                                    //</remark>
                                    //fun.Qbei_Inserts(entity);//<remark Close Logic 2020/05/30 />
                                }
                            }
                            else
                            {
                                fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                            }
                        }
                    }
                    qe.site = 145;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    chrome.Quit();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    string janCode = dt145.Rows[i]["JANコード"].ToString();
                    orderCode = dt145.Rows[i]["発注コード"].ToString();
                    fun.Qbei_ErrorInsert(145, fun.GetSiteName("145"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "145");
                    fun.WriteLog(ex, "145-", janCode, orderCode);
                }
            }
        }
    }
}
