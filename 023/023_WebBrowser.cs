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
using System.Security.Permissions;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Threading;
using System.Text.RegularExpressions;


namespace _023パナソニック
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Data Table and Common Function and Field
    /// </remark>
    public partial class frm023 : Form
    {
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        CommonFunction fun = new CommonFunction();
        DataTable dt023 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm023()
        {
            InitializeComponent();
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
        private void testflag()
        {
            try
            {
                qe.site = 23;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 1;
                DataTable dtflag = fun.SelectFlag(23);
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());


                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                /// Check to flag is "0" or "1" or "2".
                /// when flag is 0,Change to flag is 1 and Continue to StartRun Process.
                /// </remark>
                if (flag == 0)
                {
                    fun.ChangeFlag(qe);
                    StartRun();
                }

                ///<remark>
                ///when flag is 1,To Continue to StartRun Process.
                ///</remark>
                else if (flag == 1)
                {
                    fun.deleteData(23);
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
                fun.WriteLog(ex, "023-");
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
        public void StartRun()
        {
            try
            {
                webBrowser1.AllowWebBrowserDrop = false;
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.WebBrowserShortcutsEnabled = false;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
                fun.setURL("023");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(23);
                fun.Qbei_ErrorDelete(23);
                dt023 = fun.GetDatatable("023");
                //dt023 = fun.GetOrderData(dt023, "https://weborder.panabyc.co.jp", "023", string.Empty);
                fun.GetTotalCount("023");
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "023-");
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
        private void ReadData()
        {
            qe.SiteID = 23;
            dt = qubl.Qbei_Setting_Select(qe);
            fun.url = dt.Rows[0]["Url"].ToString();           
            Thread.Sleep(2000);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(fun.url);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)

        {
            try
            {
                fun.ClearMemory();

                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);
                fun.WriteLog("Navigation to Site Url success------", "023-");
                qe.SiteID = 23;
                dt = qubl.Qbei_Setting_Select(qe);              
                string username = dt.Rows[0]["UserName"].ToString();
                fun.GetElement("input", "txtUserName", "name", webBrowser1).InnerText = username;               
                string password = dt.Rows[0]["Password"].ToString();
                webBrowser1.Document.GetElementById("txtPassword").InnerText = password;
                fun.GetElement("input", "btnLogin", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
            }
            catch (Exception ex)
            {
                string janCode = dt023.Rows[0]["JANコード"].ToString();
                string orderCode = dt023.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                fun.WriteLog(ex, "023-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Check Login
        /// </summary>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string orderCode = string.Empty;
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains("ログインできません。お客様ID・パスワードをご確認ください"))
                {
                    fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), "Login Failed", dt023.Rows[0]["JANコード"].ToString(), dt023.Rows[0]["発注コード"].ToString(), 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                    fun.WriteLog("Login Failed", "023-");

                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    fun.WriteLog("Login success             ------", "023-");

                    webBrowser1.Navigate("https://weborder.panabyc.co.jp/dotnet/forms/Sc/ZaiTouMenu.aspx");                   
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Button);
                }
            }
            catch (Exception ex)
            {
                string janCode = dt023.Rows[0]["JANコード"].ToString();
                orderCode = dt023.Rows[0]["発注コード"].ToString();
                fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                fun.WriteLog(ex, "023-", janCode, orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        private void webBrowser1_Button(object sender, WebBrowserDocumentCompletedEventArgs e)
        {           
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_Button);
            if (i < dt023.Rows.Count)
            {                
                string orderCode = dt023.Rows[+i]["発注コード"].ToString();               
                fun.GetElement("input", "TextBoxHinban1", "name", webBrowser1).InnerText = orderCode;
                fun.GetElement("input", "ImageButton3", "name", webBrowser1).InvokeMember("click");
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            }
            else
            {
                qe.site = 23;
                qe.flag = 2;
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                fun.ChangeFlag(qe);
                Application.Exit();
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            fun.ClearMemory();
            entity = new Qbei_Entity();
            entity.siteID = 23;
            entity.sitecode = "023";
            entity.janCode = dt023.Rows[i]["JANコード"].ToString();
            entity.partNo = dt023.Rows[i]["自社品番"].ToString();
            entity.makerDate = fun.getCurrentDate();
            entity.reflectDate = dt023.Rows[i]["最終反映日"].ToString();
            entity.orderCode = dt023.Rows[i]["発注コード"].ToString();            
            entity.purchaseURL = fun.url;
            entity.price = dt023.Rows[i]["下代"].ToString();
            webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
            string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;
            if (body.Contains("条件に合う商品が存在しません"))
            {               
                    entity.qtyStatus = "empty";
                    entity.stockDate = "2100-02-01";            
                 fun.Qbei_Inserts(entity);
                fun.GetElement("input", "Button1", "name", webBrowser1).InvokeMember("click");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Button);
            }
            else
            {
                try
                {
                    string html = webBrowser1.Document.Body.InnerHtml;
                    HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                    hdoc.LoadHtml(html);
                    if (hdoc.DocumentNode.SelectSingleNode("/html/body/form/table[3]/tbody/tr[3]/td/font/table/tbody/tr[5]/td/span[1]") != null)
                    {

                        fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), "複数表示", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                        fun.WriteLog("複数表示" + entity.janCode + " " + entity.orderCode, "023-");
                    }
                    else
                    {
                        if (fun.GetElement("span", "DataGrid1__ctl3_LabelKanYotei", "id", webBrowser1).InnerText != null)
                        {
                            string stockdate = fun.GetElement("span", "DataGrid1__ctl3_LabelKanYotei", "id", webBrowser1).InnerText;
                            //string stockdate = "2020/07/21頃";
                            if (stockdate == "即" || stockdate.Contains("程度"))
                            {
                                entity.stockDate = "2100-01-01";
                            }
                            else if (stockdate == "生産中止")
                            {
                                entity.stockDate = "2100-02-01";
                            }
                            else if (stockdate.Contains("/"))
                            {
                                if (stockdate.Contains("頃"))
                                {
                                    stockdate = stockdate.Replace("頃", "");
                                }
                                string Day;
                                int Month;
                                int Year;
                                int pcMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                                string year = DateTime.Now.ToString("yyyy");
                                string compare = DateTime.Now.ToString("yyyy");
                                int a = stockdate.ToString().Count();
                                var b = stockdate.Split('/').ToArray();
                                if (a > 7)
                                {
                                    int array = b.Length;
                                    if (array == 3)
                                    {
                                        year = b[0];
                                    }
                                    Month = Convert.ToInt32(b[1]);
                                    if (Month < pcMonth)
                                    { year = Convert.ToString(Convert.ToInt32(b[0]) + 1); }
                                    if (Convert.ToInt32(year) < Convert.ToInt32(compare))
                                    {
                                        year = Convert.ToString(Convert.ToInt32(b[0]) + 1);
                                    }
                                    entity.stockDate = year + "-" + Month + "-" + b[2];
                                    DateTime dt = Convert.ToDateTime(entity.stockDate);
                                    if (dt <= (DateTime.Now))
                                    {
                                        dt = dt.AddYears(1);
                                    }
                                    entity.stockDate = dt.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    int array = b.Length;
                                    Month = Convert.ToInt32(b[0]);
                                    if (Month < pcMonth)
                                    { year = Convert.ToString((Convert.ToInt32(year)) + 1); }
                                    Day = b[1];
                                    entity.stockDate = year + "-" + Month + "-" + Day;
                                    DateTime dt = Convert.ToDateTime(entity.stockDate);
                                    if (dt <= (DateTime.Now))
                                    {
                                        dt = dt.AddYears(1);
                                    }
                                    entity.stockDate = dt.ToString("yyyy-MM-dd");
                                    //entity.stockDate = year + "-" + Month + "-" + Day;
                                }
                            }
                        }
                        else
                        {
                            entity.stockDate = "2100-02-01";
                        }

                        if (fun.GetElement("span", "DataGrid1__ctl3_LabelKansu", "id", webBrowser1).InnerText != null)
                        {
                            string qtyStatus = fun.GetElement("span", "DataGrid1__ctl3_LabelKansu", "id", webBrowser1).InnerText;
                            if (qtyStatus.All(char.IsDigit))
                            {
                                int num = Convert.ToInt32(qtyStatus);
                                if (num >= 10)
                                {
                                    entity.qtyStatus = "good";
                                }
                                else if (num >= 1 && num <= 9)
                                {
                                    entity.qtyStatus = "small";
                                }
                                else if (num == 0)
                                {
                                    entity.qtyStatus = "empty";
                                }
                            }
                            if (qtyStatus.Equals("-"))
                            {
                                entity.qtyStatus = "empty";
                            }
                            else if (qtyStatus.Equals("○"))
                            {
                                entity.qtyStatus = "good";
                            }
                            else if (qtyStatus.Equals("△"))
                            {
                                entity.qtyStatus = "small";
                            }
                            else if (qtyStatus.Equals("×"))
                            {
                                entity.qtyStatus = "empty";
                            }
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                        }
                        fun.Qbei_Inserts(entity);
                    }
                }
                catch (Exception ex)
                {
                    fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
                    fun.WriteLog(ex, "023-", entity.janCode, entity.orderCode);
                }
                webBrowser1.Navigate("https://weborder.panabyc.co.jp/dotnet/forms/Sc/ZaiTouMenu.aspx");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Button);
            }
            ++i;
        }

        /// <summary>
        /// Inspection of Instance_NavigateError 
        /// </summary>
        /// <param name="StatusCode">Insert to Status of Code from Error Data.</param>
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt023.Rows[i]["JANコード"].ToString();
            string orderCode = dt023.Rows[i]["発注コード"].ToString();
            fun.Qbei_ErrorInsert(23, fun.GetSiteName("023"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "023");
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "023-");

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
