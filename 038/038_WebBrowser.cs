using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using HtmlAgilityPack;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Text.RegularExpressions;

namespace _38フタバ
{
    public partial class frm038 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt038 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = -1;
        public frm038()
        {
            InitializeComponent();
            testflag();
        }

        private void testflag()
        {
            try
            {
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 38;
                qe.flag = 0;
                DataTable dtflag = fun.SelectFlag(38);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else if (flag == 1)
                {
                    fun.deleteData(38);
                    fun.ChangeFlag(qe);
                    StartRun();
                }
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "038-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        public void StartRun()
        {
            try
            {
                fun.setURL("038");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(38);
                fun.Qbei_ErrorDelete(38);
                dt038 = fun.GetDatatable("038");
                dt038 = fun.GetOrderData(dt038, "https://www.ftb-weborder.com/Account/SyohinSearch.aspx", "038", "");
                fun.GetTotalCount("038");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "038-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void ReadData()
        {
            qe.SiteID = 38;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();
            Thread.Sleep(1000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url + "/Default.aspx");

            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.ScriptErrorsSuppressed = true;
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "038-");
                qe.SiteID = 38;
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();
                webBrowser1.Document.GetElementById("tbx_tokuisakicode").InnerText = username;

                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("tbx_loginpasswd").InnerText = password;
                fun.GetElement("input", "btn_loginbutton", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt038.Rows[0]["JANコード"].ToString();
                string orderCode = dt038.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");                
                fun.WriteLog(ex, "038-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;
                if (body.Contains("パスワードが必要です。") || body.Contains("得意先コードが必要です。"))
                {
                    fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");                    
                    fun.WriteLog("Login Failed", "038-");
                    
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    webBrowser1.Navigate(fun.url + "/Account/SyohinSearch.aspx");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt038.Rows[0]["JANコード"].ToString();
                string orderCode = dt038.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");                
                fun.WriteLog(ex, "038-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }
        private void webBrowser1_WaitForSearchPage(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;

            webBrowser1.Navigate(fun.url + "/Account/SyohinSearch.aspx");
            if (webBrowser1.Url.ToString().Contains("SyohinSearch.aspx"))
            {
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_WaitForSearchPage);
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }
        private void webBrowser1_Search(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
                System.Windows.Forms.HtmlDocument doc = this.webBrowser1.Document;
                if (i < dt038.Rows.Count - 1)
                {
                    doc.GetElementById("MainContent_tbx_freeword").SetAttribute("Value", dt038.Rows[++i]["発注コード"].ToString());
                    webBrowser1.Document.GetElementById("MainContent_btn_search").InvokeMember("Click");
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
                }
                else
                {
                    qe.site = 38;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt038.Rows[i]["JANコード"].ToString();
                string orderCode = dt038.Rows[i]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                fun.WriteLog(ex, "038-", janCode, orderCode);

                webBrowser1.Document.GetElementById("MainContent_btn_clear").InvokeMember("Click");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
            }
        }

        private void webBrowser1_ItemProcessing(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();

            webBrowser1.ScriptErrorsSuppressed = true;
            entity = new Qbei_Entity();
            entity.siteID = 38;
            entity.sitecode = "038";
            entity.janCode = dt038.Rows[i]["JANコード"].ToString();
            entity.partNo = dt038.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt038.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt038.Rows[i]["発注コード"].ToString();
            entity.purchaseURL = fun.url + "/Account/SyohinSearch.aspx";
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemProcessing);
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            if (body.Contains("条件に該当する商品が見つかりませんでした"))
            {
                if (dt038.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt038.Rows[i]["在庫情報"].ToString().Contains("empty"))
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-01-10";
                    entity.price = dt038.Rows[i]["下代"].ToString();
                }
                else
                {
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";
                    entity.price = dt038.Rows[i]["下代"].ToString();
                }
                fun.Qbei_Inserts(entity);
            }
            else
            {
                string html = webBrowser1.Document.Body.InnerHtml;
                try
                {
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    if ((webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_zaikojokyo_0") == null) && (webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0") == null))
                    {
                        fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
                        fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "038-");
                        
                        Application.Exit();
                        Environment.Exit(0);
                    }
                    else
                    {
                        string qty = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_zaikojokyo_0").InnerText;
                        entity.qtyStatus = qty.Contains("○") || fun.IsGood(qty) ? "good" : qty.Equals("△") || fun.IsSmall(qty) ? "small" : qty.Equals("×") ? "empty" : "unknown status";
                        entity.price = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_tankazeinuki_0").InnerText;
                        entity.price = entity.price.Replace(",", string.Empty);
                        entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_nyukayotei_0").InnerText;

                        if (string.IsNullOrWhiteSpace(entity.stockDate) || entity.stockDate.Contains("お取り寄せ品"))
                        {
                            try
                            {
                                entity.stockDate = string.IsNullOrEmpty(entity.stockDate) ? "" : entity.stockDate;
                               // entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                                if (entity.stockDate.Contains("在庫限り"))
                                {
                                    entity.stockDate = "2100-02-01";
                                }
                                else
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                            }
                            catch
                            {
                                entity.stockDate = "2100-01-01";
                            }
                        }
                        else if (entity.stockDate.Contains("在庫限り") || entity.stockDate.Contains("廃盤") || entity.stockDate.Contains("限定") || entity.stockDate.Contains("完全廃盤"))
                        {
                            entity.stockDate = "2100-02-01";
                        }
                        else if (entity.stockDate.Contains("2月") || entity.stockDate.Contains("2"))
                        {
                           // entity.stockDate = "2018-02-28";
                            entity.stockDate = new DateTime(DateTime.Now.Year, 2, DateTime.DaysInMonth(DateTime.Now.Year, 2)).ToString("yyyy-MM-dd");
                        }
                            //2018-05-15 Start
                        else if (entity.stockDate.Contains("月"))
                        {
                            //6-7月予定
                            if (entity.stockDate.Contains("-"))
                            {
                                entity.stockDate = entity.stockDate.Remove(0, entity.stockDate.IndexOf("-"));
                                entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty)), 30).ToString("yyyy-MM-dd");
                            }                                                       
                            else if (entity.stockDate.Contains("～"))
                                {
                                    entity.stockDate = "2100-01-01";
                                }
                            
                        }
                        //2018-05-15 End
                        else if (entity.stockDate.Contains("/中旬頃予定"))
                        {
                            entity.stockDate = new DateTime(DateTime.Now.Year, int.Parse(Regex.Replace(entity.stockDate, "[^0-9]+", string.Empty)), 20).ToString("yyyy-MM-dd");
                        }
                        else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定") || entity.stockDate.Contains("今夏頃発売予定") || entity.stockDate.Contains(":"))
                        //2018-05-15 End
                        {
                            entity.stockDate = "2100-01-01";
                        }
                        else if ((entity.stockDate.Contains("/")) || entity.stockDate.Contains("/予定"))
                        {
                            string day = string.Empty;
                            string month = string.Empty;
                            string year = string.Empty;
                            //2018-05-15 Start
                            
                            if (entity.stockDate.Contains("中～下") || entity.stockDate.Contains("下") || entity.stockDate.Contains("末頃") || entity.stockDate.Contains("末"))
                            //2018-05-15 End
                            {
                                day = "30";
                                month = entity.stockDate.Split('/')[0];
                                year = DateTime.Now.ToString("yyyy");
                            }
                            else if (entity.stockDate.Contains("初旬頃") || entity.stockDate.Contains("上旬頃") || entity.stockDate.Contains("上旬"))
                            {
                                day = "10";
                            }
                            else
                            { 
                                day = entity.stockDate.Split('/')[1];
                                //2018-05-04 Start
                                //8頃予定
                                day = Regex.Replace(day, "[^0-9]+", string.Empty);
                                //2018-05-04 End
                            }
                            month = entity.stockDate.Split('/')[0];
                            year = DateTime.Now.ToString("yyyy");
                            DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                            if (dt < DateTime.Now)
                            {
                                dt = dt.AddYears(1);
                            }
                            entity.stockDate = dt.ToString("yyyy-MM-dd");
                        }
                        else if (entity.stockDate.Contains("ロット価格有り") && !string.IsNullOrWhiteSpace(entity.stockDate) || string.IsNullOrWhiteSpace(entity.stockDate))
                        {
                            try
                            {
                                entity.stockDate = webBrowser1.Document.GetElementById("MainContent_gv_syohin_lbl_kubun_0").InnerText;
                                
                                string day = entity.stockDate.Split('/')[1];
                                string month = entity.stockDate.Split('/')[0];
                                string year = DateTime.Now.ToString("yyyy");
                                DateTime dt = Convert.ToDateTime(year + "-" + month + "-" + day);
                                if (dt < DateTime.Now)
                                {
                                    dt = dt.AddYears(1);
                                }
                                entity.stockDate = dt.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                                entity.stockDate = "2100-01-01";
                            }
                        }
                        //2015-05-10 Start
                        //else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("納期未定"))
                        //2018-05-15 Start
                        //else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定"))
                        //else if (entity.stockDate.Contains("NEW!!") || entity.stockDate.Contains("未定") || entity.stockDate.Contains("今夏頃発売予定"))
                        ////2018-05-15 End
                        //{
                        //    entity.stockDate = "2100-01-01";
                        //}
                                          
                        if ((dt038.Rows[i]["在庫情報"].ToString().Contains("empty") || dt038.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt038.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                        {
                            if (((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry")) || ((entity.qtyStatus.Equals("empty")) && (entity.stockDate.Equals(" "))) || ((entity.stockDate.Equals(" ")) && (entity.qtyStatus.Equals("inquiry"))) || ((entity.stockDate.Equals("2018-02-28")) && (entity.qtyStatus.Equals("inquiry"))))
                            {
                                entity.qtyStatus = dt038.Rows[i]["在庫情報"].ToString();
                                entity.stockDate = dt038.Rows[i]["入荷予定"].ToString();
                                entity.price = dt038.Rows[i]["下代"].ToString();
                            }
                        }
                        fun.Qbei_Inserts(entity);
                    }
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");                    
                    fun.WriteLog(ex, "038-", entity.janCode, entity.orderCode);
                }
            }

            webBrowser1.Document.GetElementById("MainContent_btn_clear").InvokeMember("Click");
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Search);
        }
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt038.Rows[i]["JANコード"].ToString();
           string orderCode = dt038.Rows[i]["発注コード"].ToString(); 
            fun.Qbei_ErrorInsert(38, fun.GetSiteName("038"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "038");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "038-");
            
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
