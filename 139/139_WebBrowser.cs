using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Threading;

namespace _139
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm139 : Form
    {
        CommonFunction objCom = new CommonFunction();
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbeisetting_Entity entitySetting = new Qbeisetting_Entity();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt139 = new DataTable();
        DataTable dtSetting = new DataTable();
        DataTable dtGroupData = new DataTable();
        int intCnt = 0;
        string strUrl;
        string strSize;
        string Really_size;
        string Really_jancode;
        string Check_jancode;
        string[] Really_sizedata;
        HtmlAgilityPack.HtmlDocument hdoc;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm139()
        {
            InitializeComponent();
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
        private void testFlag()
        {
            try
            {
                int intFlag;
                entitySetting.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                entitySetting.site = 139;
                entitySetting.flag = 1;
                dtSetting = objCom.SelectFlag(139);
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
                    objCom.ChangeFlag(entitySetting);
                    startRun();
                }

                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (intFlag == 1)
                {
                    objCom.deleteData(139);
                    objCom.ChangeFlag(entitySetting);
                    startRun();
                }
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                objCom.WriteLog(ex, "139-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing to Data and Data Table.
        /// </remark>
        private void startRun()
        {
            try
            {
                objCom.setURL("139");
                objCom.CreateFileAndFolder();
                objCom.Qbei_Delete(139);
                objCom.Qbei_ErrorDelete(139);
                dt139 = objCom.GetDatatable("139");
                //dt139 = objCom.GetOrderData(dt139, "https://www.wave-one.com/oroshi/item/", "139", "/");//<remark Close Logic of Onceaweek 2020/10/1 />
                dt139 = dt139.AsEnumerable().OrderBy(x => x.Field<string>("発注コード")).CopyToDataTable();
                objCom.GetTotalCount("139");
                readData();
            }
            catch (Exception ex)
            {
                objCom.WriteLog(ex, "139-");
                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Read to Data and Url.
        /// </remark>
        private void readData()
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            entitySetting.SiteID = 139;
            dtSetting = blQbei.Qbei_Setting_Select(entitySetting);
            objCom.url = dtSetting.Rows[0]["Url"].ToString();
            strUrl = "https://www.wave-one.com/oroshi/";
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(objCom.url + "login");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                objCom.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                webBrowser1.ScriptErrorsSuppressed = true;
                objCom.WriteLog("Navigation to Site Url success------", "139-");
                entity.janCode = string.Empty;
                entity.orderCode = string.Empty;
                if (dtSetting.Rows.Count <= 0)
                {
                    entitySetting.SiteID = 139;
                    dtSetting = blQbei.Qbei_Setting_Select(entitySetting);
                }
                objCom.GetElement("input", "text", "type", webBrowser1).SetAttribute("value", dtSetting.Rows[0]["UserName"].ToString());

                objCom.GetElement("input", "password", "type", webBrowser1).SetAttribute("value", dtSetting.Rows[0]["Password"].ToString());

                objCom.GetElement("input", "submit", "type", webBrowser1).InvokeMember("click");
                //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_LoginConfirm);//<remark Edit>
            }
            catch (Exception ex)
            {
                string janCode = dt139.Rows[0]["JANコード"].ToString();
                string orderCode = dt139.Rows[0]["発注コード"].ToString();
                objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                objCom.WriteLog(ex, "139-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Login of Confirm.
        /// </summary>
        private void webBrowser1_LoginConfirm(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                objCom.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_LoginConfirm);
                if (webBrowser1.Document.Body.InnerText.Contains("納品先コード、またはパスワードが違います。"))
                {
                    objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                    objCom.WriteLog("Login Failed", "139-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    objCom.WriteLog("Login success             ------", "139-");
                    webBrowser1.Navigate(objCom.url);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt139.Rows[intCnt]["JANコード"].ToString();
                objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), ex.Message, janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                objCom.WriteLog(ex, "139-", janCode, entity.orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Search(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                objCom.ClearMemory();

                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);

                //<remark Edit Logic for ItermSearch 2020/10/21 Start>
                //if (intCnt < dt139.Rows.Count)
                //{
                //    objCom.WriteLog("Search Start", "139-");
                //    if (webBrowser1.Document.Body.InnerText.Contains("納品先コード、またはパスワードが違います。"))
                //    {
                //        objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                //        objCom.WriteLog("Login Failed", "139-");

                //        Application.Exit();
                //        Environment.Exit(0);
                //    }
                //    else
                //    {
                //        objCom.WriteLog("Login success             ------", "139-");
                //        entity.orderCode = dt139.Rows[intCnt]["発注コード"].ToString();
                //        dtGroupData = dt139.Select("発注コード ='" + entity.orderCode + "'").CopyToDataTable();                        
                //        webBrowser1.Navigate(objCom.url + "item/" + entity.orderCode);
                //        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
                //    }
                //}
                //else
                //{
                //    entitySetting.site = 139;
                //    entitySetting.flag = 2;
                //    entitySetting.starttime = string.Empty;
                //    entitySetting.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //    objCom.ChangeFlag(entitySetting);
                //    Application.Exit();
                //    Environment.Exit(0);
                //}
                //</remark 2020/10/21 End>

                if (intCnt < dt139.Rows.Count)
                {
                    entity.orderCode = dt139.Rows[intCnt]["発注コード"].ToString();                                         
                    webBrowser1.Navigate(objCom.url + "item/" + entity.orderCode);
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
                }
                else
                {
                    entitySetting.site = 139;
                    entitySetting.flag = 2;
                    entitySetting.starttime = string.Empty;
                    entitySetting.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    objCom.ChangeFlag(entitySetting);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt139.Rows[intCnt]["JANコード"].ToString();
                objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), ex.Message, janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                objCom.WriteLog(ex, "139-", janCode, entity.orderCode);

                //++intCnt;
                //webBrowser1.Navigate(strUrl);
                //webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }


        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_Item(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                //int intLCnt = 0;
                DataTable Check_Ordercode = dt139.AsEnumerable().Where(y => (y["発注コード"].Equals(entity.orderCode))).CopyToDataTable();
                int Amount_Order = Check_Ordercode.Rows.Count;
                foreach (DataRow Check_Rows in Check_Ordercode.Rows)
                {


                    entity = new Qbei_Entity();
                    string[] strData;
                    webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
 
                    Really_jancode = Check_Rows["JANコード"].ToString();
                    Really_size = Check_Rows["サイズ"].ToString();
                    DataTable Really_Table = dt139.AsEnumerable().Where(y => (y["JANコード"].Equals(Really_jancode) && y["サイズ"].Equals(Really_size))).CopyToDataTable();
                    strSize = Really_Table.Rows[0]["サイズ"].ToString();
                    strData = strSize.Split(new char[] { '（', '(' }, StringSplitOptions.RemoveEmptyEntries);
                    strSize = strData[0].Trim();
                    entity.partNo = Check_Rows["自社品番"].ToString();
                    entity.reflectDate = Check_Rows["最終反映日"].ToString();
                    entity.janCode = Check_Rows["JANコード"].ToString();
                    entity.orderCode = Check_Rows["発注コード"].ToString();
                    entity.price = Check_Rows["下代"].ToString();

                    entity.siteID = 139;
                    entity.sitecode = "139";                   
                    entity.makerDate = objCom.getCurrentDate();                  
                    entity.purchaseURL = webBrowser1.Url.ToString();
                    entity.qtyStatus = string.Empty;
                    entity.stockDate = string.Empty;                   
                    if (webBrowser1.Url.Equals(strUrl) || webBrowser1.Url.Equals(objCom.url))
                    {
                        entity.purchaseURL = "https://www.wave-one.com/oroshi/item/" + entity.orderCode + "/"; 
                        entity.qtyStatus = "empty";
                        entity.stockDate = "2100-02-01";
                    }
                    else
                    {
                        //Check Element Exist or not
                        if (webBrowser1.Document.GetElementById("c_sale_price") == null)
                        {
                            objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                            objCom.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "139-");

                            Application.Exit();
                            Environment.Exit(0);
                        }
                        else
                        {
                            entity.price = webBrowser1.Document.GetElementById("c_sale_price").InnerText;
                            entity.price = String.IsNullOrEmpty(entity.price) ? "0" : entity.price.Replace(",", "");
                            hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(webBrowser1.Document.Body.InnerHtml);

                            if (webBrowser1.Document.Body.InnerHtml.Contains("売り切れました"))
                            {
                                entity.qtyStatus = "empty";
                                entity.stockDate = "2100-02-01";
                            }
                            else
                            {
                                var exist = hdoc.DocumentNode.SelectSingleNode("//table[@summary='" + entity.orderCode + " 在庫表']");
                                if (exist != null)
                                {
                                    string strdata = hdoc.DocumentNode.SelectSingleNode("//table[@summary='" + entity.orderCode + " 在庫表']").InnerHtml;
                                    hdoc.LoadHtml(strdata);
                                    var b = hdoc.DocumentNode.SelectNodes("/tbody/tr");
                                    //Check Size is exist or not
                                    if (b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).SingleOrDefault() == null)
                                    {
                                        entity.qtyStatus = "empty";
                                        entity.stockDate = "2100-02-01";
                                    }
                                    else
                                    {
                                        entity.qtyStatus = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[2]").InnerText).SingleOrDefault();
                                        if (!string.IsNullOrEmpty(entity.qtyStatus))
                                        {
                                            //<remark Change of Quantity State 2020/03/25 Start>
                                            //if (int.Parse(entity.qtyStatus) > 10)
                                            if (int.Parse(entity.qtyStatus) >= 10)
                                                entity.qtyStatus = "good";
                                            //else if (int.Parse(entity.qtyStatus) > 0 && int.Parse(entity.qtyStatus) <= 10)
                                            //<remark Change of Quantity State 2020/07/14 Start>
                                            //else if (int.Parse(entity.qtyStatus) >= 5 && int.Parse(entity.qtyStatus) < 10)
                                            else if (int.Parse(entity.qtyStatus) >= 7 && int.Parse(entity.qtyStatus) < 10)
                                                //</remark  2020/07/14 End>
                                                entity.qtyStatus = "small";
                                            else
                                                entity.qtyStatus = "empty";
                                            //</remark  2020/03/25 End>
                                        }
                                        //entity.stockDate = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[4]").InnerText).SingleOrDefault();
                                        entity.stockDate = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[4]").InnerHtml).SingleOrDefault();
                                        if (!String.IsNullOrEmpty(entity.stockDate))
                                        {
                                            //strData = entity.stockDate.Split(new char[] { '/', '(' }, StringSplitOptions.RemoveEmptyEntries);
                                            //entity.stockDate = (int.Parse(strData[0]) < DateTime.Now.Month) ? (int.Parse(strData[0]) == 0) ? "2100-01-01" : (DateTime.Now.Year + 1).ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0') : DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
                                            strData = entity.stockDate.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                                            var temp = strData.Where(x => !x.Contains("0/0")).FirstOrDefault();
                                            if (temp == null)
                                                entity.stockDate = "2100-01-01";
                                            else
                                            {
                                                strData = temp.ToString().Split(new char[] { '/', '(' }, StringSplitOptions.RemoveEmptyEntries);
                                                //entity.stockDate = (int.Parse(strData[0]) < DateTime.Now.Month) ? (int.Parse(strData[0]) == 0) ? "2100-01-01" : (DateTime.Now.Year + 1).ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0') : DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
                                                entity.stockDate = DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
                                            }
                                        }
                                        //2018-06-21 Start 
                                        else if (entity.qtyStatus.Equals("empty") && String.IsNullOrEmpty(entity.stockDate))
                                            entity.stockDate = "2100-02-01";
                                        //2018-06-21 End
                                        else entity.stockDate = "2100-01-01";
                                    }
                                }
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(entity.qtyStatus))
                        objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                    else
                    {
                        //<remark Close Logic 2020/25/22 Start>
                        //if ((dr["在庫情報"].ToString().Contains("empty") || dr["在庫情報"].ToString().Contains("inquiry")) && dr["入荷予定"].ToString().Contains("2100-01-10"))
                        //{
                        //    if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
                        //    {
                        //        entity.qtyStatus = dr["在庫情報"].ToString();
                        //        entity.price = dr["下代"].ToString();
                        //        entity.stockDate = dr["入荷予定"].ToString();
                        //    }
                        //}
                        //</reamark 2020/25/22 End>
                        objCom.Qbei_Inserts(entity);
                    }
                    intCnt++;
                }
            }
            catch (Exception ex)
            {
                objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
                objCom.WriteLog(ex, "139-", entity.janCode, entity.orderCode);
            }

            //<remark Edit Logic for Jancode 2020/10/21 Start>
            //intCnt++;
            //foreach (DataRow dr in dtGroupData.Rows)
            //{
            //    objCom.ClearMemory();
            //    try
            //    {
            //        intLCnt++;
            //        entity = new Qbei_Entity();
            //        entity.siteID = 139;
            //        entity.sitecode = "139";
            //        entity.partNo = dr["自社品番"].ToString();
            //        entity.makerDate = objCom.getCurrentDate();
            //        entity.reflectDate = dr["最終反映日"].ToString();
            //        entity.janCode = dr["JANコード"].ToString();
            //        entity.orderCode = dr["発注コード"].ToString();
            //        entity.purchaseURL = webBrowser1.Url.ToString();
            //        entity.qtyStatus = string.Empty;
            //        entity.stockDate = string.Empty;
            //        strSize = dr["サイズ"].ToString();
            //        strData = strSize.Split(new char[] { '（', '(' }, StringSplitOptions.RemoveEmptyEntries);
            //        strSize = strData[0].Trim();
            //        if (webBrowser1.Url.Equals(strUrl) || webBrowser1.Url.Equals(objCom.url))
            //        {
            //            entity.purchaseURL = "https://www.wave-one.com/oroshi/item/" + entity.orderCode + "/";
            //            entity.price = dr["下代"].ToString();
            //            entity.qtyStatus = "empty";
            //            entity.stockDate = "2100-02-01";
            //        }
            //        else
            //        {
            //            //Check Element Exist or not
            //            if (webBrowser1.Document.GetElementById("c_sale_price") == null)
            //            {
            //                objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
            //                objCom.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "139-");

            //                Application.Exit();
            //                Environment.Exit(0);
            //            }
            //            else
            //            {
            //                entity.price = webBrowser1.Document.GetElementById("c_sale_price").InnerText;
            //                entity.price = String.IsNullOrEmpty(entity.price) ? "0" : entity.price.Replace(",", "");              
            //                hdoc = new HtmlAgilityPack.HtmlDocument();                            
            //                hdoc.LoadHtml(webBrowser1.Document.Body.InnerHtml);

            //                if (webBrowser1.Document.Body.InnerHtml.Contains("売り切れました"))
            //                {
            //                    entity.qtyStatus = "empty";
            //                    entity.stockDate = "2100-02-01";
            //                }
            //                else
            //                {
            //                    var exist = hdoc.DocumentNode.SelectSingleNode("//table[@summary='" + entity.orderCode + " 在庫表']");
            //                    if (exist != null)
            //                    {
            //                        string strdata = hdoc.DocumentNode.SelectSingleNode("//table[@summary='" + entity.orderCode + " 在庫表']").InnerHtml;
            //                        hdoc.LoadHtml(strdata);
            //                        var b = hdoc.DocumentNode.SelectNodes("/tbody/tr");
            //                        //Check Size is exist or not
            //                        if (b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).SingleOrDefault() == null)
            //                        {
            //                            entity.qtyStatus = "empty";
            //                            entity.stockDate = "2100-02-01";
            //                        }
            //                        else
            //                        {
            //                            entity.qtyStatus = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[2]").InnerText).SingleOrDefault();                                      
            //                            if (!string.IsNullOrEmpty(entity.qtyStatus))
            //                            {
            //                                //<remark Change of Quantity State 2020/03/25 Start>
            //                                //if (int.Parse(entity.qtyStatus) > 10)
            //                                    if (int.Parse(entity.qtyStatus) >= 10)
            //                                        entity.qtyStatus = "good";
            //                                //else if (int.Parse(entity.qtyStatus) > 0 && int.Parse(entity.qtyStatus) <= 10)
            //                                //<remark Change of Quantity State 2020/07/14 Start>
            //                                //else if (int.Parse(entity.qtyStatus) >= 5 && int.Parse(entity.qtyStatus) < 10)
            //                                else if (int.Parse(entity.qtyStatus) >= 7 && int.Parse(entity.qtyStatus) < 10)
            //                                    //</remark  2020/07/14 End>
            //                                    entity.qtyStatus = "small";
            //                                else
            //                                    entity.qtyStatus = "empty";
            //                                //</remark  2020/03/25 End>
            //                            }
            //                            //entity.stockDate = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[4]").InnerText).SingleOrDefault();
            //                            entity.stockDate = b.Where(r => r.SelectNodes("td") != null && r.SelectSingleNode("td[1]").InnerText.Equals(strSize)).Select(z => z.SelectSingleNode("td[4]").InnerHtml).SingleOrDefault();
            //                            if (!String.IsNullOrEmpty(entity.stockDate))
            //                            {
            //                                //strData = entity.stockDate.Split(new char[] { '/', '(' }, StringSplitOptions.RemoveEmptyEntries);
            //                                //entity.stockDate = (int.Parse(strData[0]) < DateTime.Now.Month) ? (int.Parse(strData[0]) == 0) ? "2100-01-01" : (DateTime.Now.Year + 1).ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0') : DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
            //                                strData = entity.stockDate.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            //                                var temp = strData.Where(x => !x.Contains("0/0")).FirstOrDefault();
            //                                if (temp == null)
            //                                    entity.stockDate = "2100-01-01";
            //                                else
            //                                {
            //                                    strData = temp.ToString().Split(new char[] { '/', '(' }, StringSplitOptions.RemoveEmptyEntries);
            //                                    //entity.stockDate = (int.Parse(strData[0]) < DateTime.Now.Month) ? (int.Parse(strData[0]) == 0) ? "2100-01-01" : (DateTime.Now.Year + 1).ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0') : DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
            //                                    entity.stockDate = DateTime.Now.Year.ToString() + "-" + strData[0].PadLeft(2, '0') + "-" + strData[1].PadLeft(2, '0');
            //                                }
            //                            }
            //                                //2018-06-21 Start 
            //                            else if (entity.qtyStatus.Equals("empty") && String.IsNullOrEmpty(entity.stockDate))                                          
            //                                entity.stockDate = "2100-02-01";
            //                                //2018-06-21 End
            //                            else entity.stockDate = "2100-01-01";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        if (String.IsNullOrEmpty(entity.qtyStatus))
            //            objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
            //        else
            //        {
            //            //<remark Close Logic 2020/25/22 Start>
            //            //if ((dr["在庫情報"].ToString().Contains("empty") || dr["在庫情報"].ToString().Contains("inquiry")) && dr["入荷予定"].ToString().Contains("2100-01-10"))
            //            //{
            //            //    if ((entity.qtyStatus.Equals("empty") && (entity.stockDate.Equals("2100-01-01") || entity.stockDate.Equals("2100-02-01"))) || entity.qtyStatus.Equals("inquiry"))
            //            //    {
            //            //        entity.qtyStatus = dr["在庫情報"].ToString();
            //            //        entity.price = dr["下代"].ToString();
            //            //        entity.stockDate = dr["入荷予定"].ToString();
            //            //    }
            //            //}
            //            //</reamark 2020/25/22 End>
            //            objCom.Qbei_Inserts(entity);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");                    
            //        objCom.WriteLog(ex, "139-", entity.janCode, entity.orderCode);
            //    }
            //    intCnt++;
            //}           
            //webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
            //</remark 2020/10/21 End>
  
            webBrowser1.Navigate(strUrl);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt139.Rows[intCnt]["JANコード"].ToString();
            string orderCode = dt139.Rows[intCnt]["発注コード"].ToString();
            objCom.Qbei_ErrorInsert(139, objCom.GetSiteName("139"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "139");
            objCom.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "139-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
