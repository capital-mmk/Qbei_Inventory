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
using Microsoft.VisualBasic;

namespace _914
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm914 : Form
    {
        CommonFunction objCom = new CommonFunction();
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbeisetting_Entity entitySetting = new Qbeisetting_Entity();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt914 = new DataTable();
        DataTable dtSetting = new DataTable();
        DataTable dtGroupData = new DataTable();
        int intCnt = 0;
        string strUrl;
        string strSize;
        string strColor;
        HtmlAgilityPack.HtmlDocument hdoc;

        /// <summary>
        /// System(Start).
        /// </summary>
        /// <remark>
        /// Continue to testflag process.
        /// </remark>
        public frm914()
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
                entitySetting.site = 914;
                entitySetting.flag = 1;
                dtSetting = objCom.SelectFlag(914);
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
                    objCom.deleteData(914);
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
                objCom.WriteLog(ex, "914-");
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
                objCom.setURL("914");
                objCom.CreateFileAndFolder();
                objCom.Qbei_Delete(914);
                objCom.Qbei_ErrorDelete(914);
                dt914 = objCom.GetDatatable("914");
                dt914 = objCom.GetOrderData(dt914, "https://reric-order.com/ecuser/item/itemList", "914", "");
                objCom.GetTotalCount("914");
                readData();
            }
            catch (Exception ex)
            {
                objCom.WriteLog(ex, "914-");
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
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                entitySetting.SiteID = 914;
                dtSetting = blQbei.Qbei_Setting_Select(entitySetting);
                objCom.url = dtSetting.Rows[0]["Url"].ToString();
                strUrl = "https://reric-order.com/ecuser/";
                webBrowser1.AllowNavigation = true;
                webBrowser1.Navigate(objCom.url);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                objCom.WriteLog(ex, "914-");
                Application.Exit();
                Environment.Exit(0);
            }
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
                objCom.WriteLog("Navigation to Site Url success------", "914-");
                entity.janCode = string.Empty;
                entity.orderCode = string.Empty;
                if (dtSetting.Rows.Count <= 0)
                {
                    entitySetting.SiteID = 914;
                    dtSetting = blQbei.Qbei_Setting_Select(entitySetting);
                }
                objCom.GetElement("input", "login_id", "id", webBrowser1).SetAttribute("value", dtSetting.Rows[0]["UserName"].ToString());
                objCom.GetElement("input", "login_pw", "id", webBrowser1).SetAttribute("value", dtSetting.Rows[0]["Password"].ToString());
                webBrowser1.Document.GetElementsByTagName("a")[0].InvokeMember("onclick");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WateForStartPageLogin);
            }
            catch (Exception ex)
            {  
                objCom.Qbei_ErrorInsert(914, objCom.GetSiteName("914"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "914");
                objCom.WriteLog(ex, "914-", entity.janCode, entity.orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Wait For Search Page Process.
        /// </summary>
        private void WateForStartPageLogin(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(WateForStartPageLogin);
            webBrowser1.Document.GetElementsByTagName("a")[0].InvokeMember("onclick");
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
        }

        /// <summary>
        /// Check to Login.
        /// </summary>
        private void webBrowser1_Search(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                if (intCnt < dt914.Rows.Count)
                {
                    objCom.WriteLog("Search Start", "914-");
                    if (webBrowser1.Document.Body.InnerText.Contains("ログインできません。ログインID、パスワードを確認してください"))
                    {   
                        objCom.Qbei_ErrorInsert(914, objCom.GetSiteName("914"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "914");
                        objCom.WriteLog("Login Failed", "914-");
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        objCom.WriteLog("Login success             ------", "914-");
                        entity.janCode = dt914.Rows[intCnt]["JANコード"].ToString();
                        entity.orderCode = dt914.Rows[intCnt]["発注コード"].ToString();
                        //entity.orderCode = "1181803";
                        dtGroupData = dt914.Select("発注コード ='" + entity.orderCode + "'").CopyToDataTable();
                        webBrowser1.Navigate(objCom.url + "item/itemDetail?itemCd=" + entity.orderCode + "&dispMode=MATRIX");
                        webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
                    }
                }
                else
                {
                    entitySetting.site = 914;
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
                objCom.Qbei_ErrorInsert(914, objCom.GetSiteName("914"), ex.Message, entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "914");
                objCom.WriteLog(ex, "914-", entity.janCode, entity.orderCode);

                intCnt++;
                webBrowser1_Search(null, null);
            }
        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_Item(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int intHCnt = 0;
            bool isFound = false;
            objCom.WriteLog("Item Start", "914-");
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Item);
            foreach (DataRow dr in dtGroupData.Rows)
            {
                try
                {
                    objCom.ClearMemory();
                    entity = new Qbei_Entity();
                    entity.siteID = 914;
                    entity.sitecode = "914";
                    entity.partNo = dr["自社品番"].ToString();
                    entity.makerDate = objCom.getCurrentDate();
                    entity.reflectDate = dr["最終反映日"].ToString();
                    entity.janCode = dr["JANコード"].ToString();
                    entity.orderCode = dr["発注コード"].ToString();
                    //entity.orderCode = "1181803";
                    entity.price = dr["下代"].ToString();
                    entity.purchaseURL = webBrowser1.Url.ToString();
                    entity.qtyStatus = string.Empty;
                    entity.stockDate = string.Empty;
                    strSize = dr["サイズ"].ToString();
                    strColor = dr["カラー"].ToString();
                    strSize = Strings.StrConv(strSize, VbStrConv.Wide, 1041);
                    string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                    if (body.Contains("有効な商品ではありません。"))
                    {
                        if (dt914.Rows[intCnt]["入荷予定"].ToString().Contains("2100-01-10") && (dt914.Rows[intCnt]["在庫情報"].ToString().Contains("empty") || dt914.Rows[intCnt]["在庫情報"].ToString().Contains("inquiry")))
                        {
                            entity.qtyStatus = dt914.Rows[intCnt]["在庫情報"].ToString();
                            entity.stockDate = dt914.Rows[intCnt]["入荷予定"].ToString();
                            entity.price = dt914.Rows[intCnt]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt914.Rows[intCnt]["下代"].ToString();
                        }
                        objCom.Qbei_Inserts(entity);
                    }
                    else
                    {
                        hdoc = new HtmlAgilityPack.HtmlDocument();
                        hdoc.LoadHtml(webBrowser1.Document.Body.InnerHtml);

                        var exist = hdoc.DocumentNode.SelectSingleNode("//table[@class='matrix']");
                        if (exist != null)
                        {
                            string strdata = hdoc.DocumentNode.SelectSingleNode("//table[@class='matrix']").InnerHtml;
                            hdoc.LoadHtml(strdata);
                            var b = hdoc.DocumentNode.SelectNodes("/tbody/tr");
                            foreach (var tmp in b.ToList())
                            {
                                //if (tmp.SelectSingleNode("th").InnerText.Trim().Equals(strColor))                                
                                //{
                                    intHCnt = tmp.SelectNodes("td").ToList().Count;
                                    for (int i = 1; i <= intHCnt; i++)
                                    {
                                        HtmlNode colorNode = tmp.SelectSingleNode("td[" + i + "]/input[2]");
                                        if (colorNode != null && colorNode.GetAttributeValue("value", "").Trim().Equals(strColor))
                                        {
                                            string webSize = tmp.SelectSingleNode("td[" + i + "]/input[4]").GetAttributeValue("value", "");
                                            webSize = Strings.StrConv(webSize, VbStrConv.Wide, 1041);
                                            if (webSize.Equals(strSize))
                                            {
                                                entity.qtyStatus = tmp.SelectSingleNode("td[" + i + "]/div[3]/div[2]").InnerText;
                                                entity.price = tmp.SelectSingleNode("td[" + i + "]/div[2]/div[2]/span").InnerText.Replace(",", "");
                                                isFound = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (isFound) break;
                            }
                            //<remark 13/07/2020(変更)>
                            //entity.stockDate = entity.qtyStatus.Contains("○") || entity.qtyStatus.Contains("△") ? "2100-01-01" : entity.qtyStatus.Contains("×") ? "2100-02-01" : "unknown date";
                            //entity.qtyStatus = entity.qtyStatus.Contains("○") ? "good" : entity.qtyStatus.Contains("△") ? "small" : entity.qtyStatus.Contains("×") ? "empty" : "empty";
                            entity.stockDate = entity.qtyStatus.Contains("○") ? "2100-01-01" : entity.qtyStatus.Contains("△") || entity.qtyStatus.Contains("×") ? "2100-02-01" : "unknown date";
                            entity.qtyStatus = entity.qtyStatus.Contains("○") ? "good" :  entity.qtyStatus.Contains("△") || entity.qtyStatus.Contains("×") ? "empty" : "empty";
                            //</remark>
                            //<remark Close Logic 2020/25/22 Start>
                            //if (dr["入荷予定"].ToString().Contains("2100-01-10"))
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
                    }
                }
                catch (Exception ex)
                {
                    objCom.Qbei_ErrorInsert(914, objCom.GetSiteName("914"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "914");
                    objCom.WriteLog(ex, "914-", entity.janCode, entity.orderCode);
                }
                isFound = false;
                intCnt++;
            }

            webBrowser1_Search(null, null);
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt914.Rows[intCnt]["JANコード"].ToString();
            string orderCode = dt914.Rows[intCnt]["発注コード"].ToString();
            objCom.Qbei_ErrorInsert(914, objCom.GetSiteName("914"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "914");
            objCom.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "914-");
            Application.Exit();
            Environment.Exit(0);
        }
    }
}