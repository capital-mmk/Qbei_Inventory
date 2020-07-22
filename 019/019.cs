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

namespace _019深谷_フカヤ_
{
    class _019
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>               
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt019 = new DataTable();
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
                qe.site = 19;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(19);
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
                    fun.deleteData(19);
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

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        public static void startRun()
        {
            DataTable dt019 = new DataTable();
            try
            {
                fun.setURL("019");
                fun.Qbei_Delete(19);
                fun.Qbei_ErrorDelete(19);
                dt019 = fun.GetDatatable("019");
                //dt019 = fun.GetOrderData(dt019, "https://webcart.fukaya-nagoya.co.jp/consumer/", "019", "");//<remark Close Logic 2020/05/26 />
                fun.GetTotalCount("019");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Environment.Exit(0);
            }

            /// <summary>
            /// Use to ChormeDriver and Data Table and Common Function and Field
            /// </summary>
            var chromeOptions = new ChromeOptions();
            using (IWebDriver chrome = new ChromeDriver(chromeOptions))
            {
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                Qbei_Entity entity = new Qbei_Entity();

                /// <summary>
                /// Login of Mall.
                /// </summary>
                qe.SiteID = 19;
                dt = qubl.Qbei_Setting_Select(qe);
                string url = dt.Rows[0]["Url"].ToString();
                chrome.Url = url;
                string title = chrome.Title;

                string username = dt.Rows[0]["UserName"].ToString();
                chrome.FindElement(By.Name("loginId")).SendKeys(username);
                string password = dt.Rows[0]["Password"].ToString();
                chrome.FindElement(By.Name("password")).SendKeys(password);
                fun.WriteLog("Navigation to Site Url success------", "037-");
                chrome.FindElement(By.Id("login")).Click();
                Thread.Sleep(8000);

                /// <summary>
                /// Check Login
                /// </summary>
                string orderCode = string.Empty;
                string body = chrome.FindElement(By.TagName("body")).Text;
                if (body.Contains("入力された会員ID、パスワードは不正です。"))
                {
                    fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Login Failed", dt019.Rows[0]["JANコード"].ToString(), dt019.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                    fun.WriteLog("Login Failed", "019-");

                    chrome.Quit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "019-");
                }

                /// <summary>
                /// Inspection of item information at Mall.
                /// </summary>
                try
                {
                    int Lastrow = dt019.Rows.Count;
                    for (i = 0; i < Lastrow; i++)
                    {
                        if (i < Lastrow)
                        {
                            string od;
                            od = dt019.Rows[i]["JANコード"].ToString();
                            chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);

                            entity = new Qbei_Entity();
                            entity.siteID = 19;
                            entity.sitecode = "019";
                            entity.janCode = dt019.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt019.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt019.Rows[i]["最終反映日"].ToString();
                            entity.orderCode = dt019.Rows[i]["発注コード"].ToString().Trim();
                            entity.purchaseURL = "https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od;

                            //<remark>
                            //Check to  Url is Correct 
                            //</remark>
                            if (!chrome.Url.ToString().Contains("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1="))

                            {
                                Thread.Sleep(50000);
                                od = dt019.Rows[i]["JANコード"].ToString();
                                chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);
                            }

                            //<remark>
                            //Check to  Item is Correct Data 
                            //</remark>
                            string ItemCheck;
                            try
                            {
                                ItemCheck = chrome.FindElement(By.Id("M_total")).Text;
                            }
                            catch
                            {
                                od = dt019.Rows[i]["JANコード"].ToString();
                                chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);

                                Thread.Sleep(20000);
                                ItemCheck = chrome.FindElement(By.Id("M_total")).Text;
                            }

                            //<remark>
                            //Check to Ordercode
                            //</remark>
                            if (!string.IsNullOrWhiteSpace(entity.orderCode))
                            {
                                if (ItemCheck.Contains("総 0件"))
                                {
                                    //if (dt019.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt019.Rows[i]["在庫情報"].ToString().Contains("empty"))
                                    //{
                                    //    //fun.Qbei_ErrorInsert(11, "マルイ", "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                                    //    entity.qtyStatus = "empty";
                                    //    entity.stockDate = "2100-01-10";
                                    //    entity.price = dt019.Rows[i]["下代"].ToString();
                                    //}
                                    //else
                                    //{
                                    //    entity.qtyStatus = "empty";
                                    //    entity.stockDate = "2100-02-01";
                                    //    entity.price = dt019.Rows[i]["下代"].ToString();
                                    //}
                                    entity.qtyStatus = "empty";
                                    entity.stockDate = "2100-02-01";
                                    entity.price = dt019.Rows[i]["下代"].ToString();
                                    fun.Qbei_Inserts(entity);//<remark Add Logic 2020/05/30 />
                                }
                                else
                                {
                                    //<remark>
                                    //Check to Quantity
                                    //</remark>      
                                    string qty;
                                    try
                                    {
                                        qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            //od = dt019.Rows[i]["JANコード"].ToString();
                                            //chrome.Navigate().GoToUrl("https://weborder.fukaya-nagoya.co.jp/shop/shopbrand.html?search=&page=&sort=order&content1=" + od);
                                            Thread.Sleep(10000);
                                            qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span")).Text;
                                        }
                                        catch
                                        {
                                            Thread.Sleep(10000);
                                            qty = chrome.FindElement(By.XPath("/html/body/center/center/div[2]/div[7]/form[3]/div/div[3]/div/table/tbody/tr[2]/td[5]/span[2]")).Text;
                                        }
                                    }

                                    //<remark>
                                    //Check to have Digits at Quantity and take Quantity of Status
                                    //</remark>   
                                    bool isDigitPresent = qty.Any(c => char.IsDigit(c));
                                    if (isDigitPresent == true)
                                    {
                                        entity.qtyStatus = "small";
                                    }
                                    else
                                    {
                                        //entity.qtyStatus = qty.Equals("在庫○") ? "good" : qty.Equals("取寄×") ? "empty" : qty.Equals("在庫△") ? "small" : qty.Equals("欠品×") ? "empty" : qty.Equals("廃番×") ? "empty" : qty.Equals("廃番(n)") ? "small" : qty.Equals("廃番○") ? "good" : "unknown status";//<remark Edit Logic of quantity 2020/07/21 />
                                        entity.qtyStatus = qty.Equals("在庫○") ? "good" : qty.Equals("取寄×") ? "empty" : qty.Equals("在庫△") ? "empty" : qty.Equals("欠品×") ? "empty" : qty.Equals("廃番×") ? "empty" : qty.Equals("廃番(n)") ? "small" : qty.Equals("廃番○") ? "good" : "unknown status";
                                    }

                                    //<remark>
                                    //Check to Price
                                    //</remark>
                                    entity.price = chrome.FindElement(By.ClassName("price_em")).Text;
                                    entity.price = entity.price.Replace("卸価格：￥", " ").Replace(",", string.Empty);

                                    //<remark>
                                    //Check to Stockdate
                                    //</remark>
                                    string stockdate = chrome.FindElement(By.ClassName("r_cate_title")).Text;
                                    if (stockdate.Contains("入荷予定日"))
                                    {
                                        entity.stockDate = chrome.FindElement(By.ClassName("availabilityDate")).Text;
                                        entity.stockDate = entity.stockDate.Replace("入荷予定日:", " ").Replace("/", "-");
                                        //entity.stockDate = entity.stockDate.Replace("/", "-");
                                        DateTime da = Convert.ToDateTime(entity.stockDate);
                                        entity.stockDate = da.ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        //<remark>
                                        //Check to have Digits at Quantity and take Stockdate of Status
                                        //</remark>
                                        if (isDigitPresent == true)
                                        {
                                            entity.stockDate = "2100-02-01";
                                        }
                                        else
                                        {
                                            //entity.stockDate = qty.Equals("在庫○") ? "2100-01-01" : qty.Equals("取寄×") ? "2100-02-01" : qty.Equals("在庫△") ? "2100-01-01" : qty.Equals("欠品×") ? "2100-02-01" : qty.Equals("廃番×") ? "2100-02-01" : qty.Equals("廃番(n)") ? "2100-02-01" : qty.Equals("廃番○") ? "2100-02-01" : "unknown status";//<remark Edit Logic of stockdate 2020/07/21 />
                                            entity.stockDate = qty.Equals("在庫○") ? "2100-01-01" : qty.Equals("取寄×") ? "2100-02-01" : qty.Equals("在庫△") ? "2100-02-01" : qty.Equals("欠品×") ? "2100-02-01" : qty.Equals("廃番×") ? "2100-02-01" : qty.Equals("廃番(n)") ? "2100-02-01" : qty.Equals("廃番○") ? "2100-02-01" : "unknown status";
                                        }
                                    }                                 
                                    //<remark Add Logic 2020/05/30 />
                                    if (entity.qtyStatus != "unknown status")
                                    {
                                        fun.Qbei_Inserts(entity);
                                    }
                                    else
                                    {
                                        fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Item doesn't Check!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                                    }
                                    //<remark Add Logic 2020/05/30 />
                                }
                                //<remark>
                                //Insert to QbeiTable of Statuss
                                //</remark>
                                //fun.Qbei_Inserts(entity);//<remark Close Logic 2020/05/30 />
                            }
                            else
                            {
                                fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                            }
                        }
                    }
                    qe.site = 19;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    chrome.Quit();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    string janCode = dt019.Rows[i]["JANコード"].ToString();
                    orderCode = dt019.Rows[i]["発注コード"].ToString();
                    fun.Qbei_ErrorInsert(19, fun.GetSiteName("019"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "019");
                    fun.WriteLog(ex, "019-", janCode, orderCode);
                }
            }
        }
    }
}
