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

namespace _011マルイ
{
    /// <summary>
    /// frm011マルイ Start.
    /// </summary>
    public partial class frm011 : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remark>
        /// Data Table and Common Function and Field
        /// </remark>
        
        DataTable dt = new DataTable();
        Qbeisetting_BL qubl = new Qbeisetting_BL();//Connection Database of Object.
        Qbeisetting_Entity qe = new Qbeisetting_Entity();//Field of Object.
        CommonFunction fun = new CommonFunction();//Common Function of Object.
        DataTable dt011 = new DataTable();
        Qbei_Entity entity = new Qbei_Entity();//Field of Object.
        int i = 0;

        /// <summary>
        /// System(Start).
        /// </summary>
        ///  /// <remark>
        /// flag Change.
        /// </remark>
        public frm011()
        {

            InitializeComponent();
            testflag();//testflag processing.
        }

        /// <summary>
        /// testflag processing.
        /// </summary>
        ///<remark>
        ///Site of Processing Progress.
        ///</remark>
        private void testflag()
        {
            ///<summary>
            ///Flag Number.
            ///</summary>
            ///<remark>
            ///"0,1,2"Flage Number of Check. 
            ///</remark>
            try
            {          
                Qbeisetting_Entity qe = new Qbeisetting_Entity();//Field of Object.

                /// <summary>
                /// Starttime,ID Number,Flag Number of Site.
                /// </summary>          
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Input Starttime.
                qe.site = 11;//Input SiteID Number.
                qe.flag = 1;//Input Flag Number.
                DataTable dtflag = fun.SelectFlag(11);//Input DataTable of Common Function Flag Table.             
                int flag = Convert.ToInt32(dtflag.Rows[0]["FlagIsFinished"].ToString());//To Check "FlagIsFinished" of Flag Number at Flag Table.

                /// <summary>
                /// Flag Number of Check.
                /// </summary>
                /// <remark>
                ///  "0" or "1" of Check.
                /// </remark>
                if (flag == 0)
                {      
                    fun.ChangeFlag(qe);//Common Function of ChangFlage Process.
                    StartRun();//StartRun Process.
                }
                else if (flag == 1)
                {      
                    fun.deleteData(11);//Common Function of deleteData Process.
                    fun.ChangeFlag(qe);//Common Function of ChangFlage Process.
                    StartRun();//StartRun Process.
                }
                else
                {
                    Application.Exit();
                    Environment.Exit(0);
                }
            } 
            catch (Exception ex)
            {
                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog(ex, "011-")>
                /// Common Function.
                /// </param>
                fun.WriteLog(ex, "011-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site and Data Table.
        /// </summary>
        /// <remark>
        /// Inspection and processing.
        /// </remark>
        public void StartRun()
        {
            try
            {
                /// <remark>
                /// Common Function of setURL Process.
                /// </remark>
                /// <param　setURL("011")>
                /// Common Function.
                /// </param>
                fun.setURL("011");
                fun.CreateFileAndFolder();//Common Function of CreateFileAndFolder Process.
                fun.Qbei_Delete(11);//Common Function of Qbei_Delete Process.
                fun.Qbei_ErrorDelete(11);//Common Function of Qbei_ErrorDelete Process.

                /// <remark>
                /// To Input dt011(Data Table) at Common Function of GetDatatable Process.
                /// </remark>           
                /// <param　dt011=fun.GetDatatable("011")>
                /// Datatable.
                /// </param>
                dt011 = fun.GetDatatable("011");

                /// <remark>
                /// To Input dt011(データテーブル) at Common Function of GetDatatable Process.
                /// </remark>           
                /// <param　dt011=fun.GetOrderData(dt011, "http://www.maruiltd.jp/index.php?action_goods=true&id=", "011","")>
                /// Datatable、PurchaseUrl、SiteCode、Post.
                /// </param>
                dt011 = fun.GetOrderData(dt011, "http://www.maruiltd.jp/index.php?action_goods=true&id=", "011","");

                /// <remark>
                /// Common Function of GetTotalCount Process.
                /// </remark>
                /// <param　setURL("011")>
                /// Common Function.
                /// </param>             
                fun.GetTotalCount("011");
                ReadData();//ReadData Process.v
            }
            catch (Exception ex)
            {
                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog(ex, "011-")>
                /// Common Function.
                /// </param>
                fun.WriteLog(ex, "011-");
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Site of Data.
        /// </summary>
        /// <remark>
        /// Reading.
        /// </remark>
        private void ReadData()
        {
           webBrowser1.ScriptErrorsSuppressed = true;
            qe.SiteID = 11;//Input SiteID Number.

            /// <remark>
            /// To Input Data Table at Connection Database of Qbei_Setting_Select Process.
            /// </remark>           
            /// <param　dt = qubl.Qbei_Setting_Select(qe)>
            /// Data Table.
            /// </param>
            dt = qubl.Qbei_Setting_Select(qe);

            /// <remark>
            /// To Input Common Funtion of url at Data Table of Url. 
            /// </remark>
            /// /// <param　fun.url = dt.Rows[0]["Url"].ToString()>
            /// Common Funtion.
            /// </param>
            fun.url = dt.Rows[0]["Url"].ToString();
            //Thread.Sleep(1000);
            webBrowser1.Navigate(fun.url);//Check WebBrwser Navigate at Common Function of url process.
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Start);//Next Process.
        }

        /// <summary>
        /// Login of Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_Start(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();
                SHDocVw.WebBrowser instance = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                instance.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(instance_NavigateError);

                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog("Navigation to Site Url success------", "011-")>
                /// Common Function.
                /// </param>
                fun.WriteLog("Navigation to Site Url success------", "011-");
                webBrowser1.ScriptErrorsSuppressed = true;
                qe.SiteID = 11;//Input SiteID Number.

                /// <remark>
                /// To Input Data Table at Connection Database of Qbei_Setting_Select Process.
                /// </remark>           
                /// <param　dt = qubl.Qbei_Setting_Select(qe)>
                /// Data Table.
                /// </param>
                dt = qubl.Qbei_Setting_Select(qe);
                string username = dt.Rows[0]["UserName"].ToString();//To Input string of username at Data Table of "UserName".
                webBrowser1.Document.GetElementById("id").InnerText = username;//Webpage Inspect of  CodeID.
                string password = dt.Rows[0]["Password"].ToString();//To Input string of username at Data Table of "Password".
                webBrowser1.Document.GetElementById("psw").InnerText = password;//Webpage Inspect of  CodeID.
                fun.GetElement("input", "　ロ グ イ ン　", "value", webBrowser1).InvokeMember("click");//Click at webpage of Login. 

                webBrowser1.DocumentCompleted -= webBrowser1_Start;
                webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_Login);//Next Process.
            }
            catch (Exception ex)
            {
                string janCode = dt011.Rows[0]["JANコード"].ToString();//To Input string of janCode at Data Table of "JANコード".
                string orderCode = dt011.Rows[0]["発注コード"].ToString();//To Input string of orderCode at Data Table of "orderCode".
               
                /// <remark>
                /// Common Function of Qbei_ErrorInsert Process.
                /// </remark>
                /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")>
                /// Common Function.
                /// </param>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog(ex, "011-", janCode, orderCode)>
                /// Common Function.
                /// </param>
                fun.WriteLog(ex, "011-", janCode, orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_Login(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string janCode = string.Empty;
            string orderCode = string.Empty;
            
            try
            {
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= webBrowser1_Login;
                string body = webBrowser1.Document.GetElementsByTagName("body")[0].InnerText;//To Input string of username at Data Table of ("body")[0].

                /// <remark>
                /// To Check of Condition at WebPage.
                /// </remark>
                if (body.Contains(" IDを入力してください") || body.Contains("パスワードを入力してください") || body.Contains("IDを正しく入力してください"))
                {
                    /// <remark>
                    /// Common Function of Qbei_ErrorInsert Process.
                    /// </remark>
                    /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                    /// Common Function.
                    /// </param>
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Login Failed", entity.janCode, entity.orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                    /// <remark>
                    /// Common Function of WriteLog Process.
                    /// </remark>
                    /// <param　WriteLog("Login Failed", "011-")>
                    /// Common Function.
                    /// </param>
                    fun.WriteLog("Login Failed", "011-");
                    Application.Exit();
                    Environment.Exit(0);
                }
                else
                {
                    /// <remark>
                    /// Common Function of WriteLog Process.
                    /// </remark>
                    /// <param　WriteLog("Login success             ------", "011-")>
                    /// Common Function.
                    /// </param>
                    fun.WriteLog("Login success             ------", "011-");
                    orderCode = fun.ReplaceOrderCode(dt011.Rows[0]["発注コード"].ToString(), new string[] { "-" });//To Input string of username at Data Table of ("発注コード").
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + orderCode + "00000");//WebBrowser Navigate of url.
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);//Next Process.
                }
            }
            catch (Exception ex)
            {
                janCode = dt011.Rows[i]["JANコード"].ToString();//To Input String of jancode at dt011 of "JANコード".
                orderCode = dt011.Rows[i]["発注コード"].ToString();//To Input String of orderCode at dt011 of "発注コード".

                /// <remark>
                /// Common Function of Qbei_ErrorInsert Process.
                /// </remark>
                /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                /// Common Function.
                /// </param>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 1, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog(ex, "011-", janCode, orderCode)>
                /// Common Function.
                /// </param>
                fun.WriteLog(ex, "011-", janCode, orderCode);
                Application.Exit();
                Environment.Exit(0);
            }
        }
        /// <summary>
        /// Inspection of item information at Mall.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_ItemSearch(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                fun.ClearMemory();

                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);
                entity = new Qbei_Entity();
                entity.siteID = 11;//Input SiteID Number.
                entity.sitecode = "011";//Input sitecode.
                entity.janCode = dt011.Rows[i]["JANコード"].ToString();//To Input janCode at dt011 of "JANコード".
                entity.partNo = dt011.Rows[i]["自社品番"].ToString();//To Input partNo at dt011 of "自社品番".
                entity.makerDate = fun.getCurrentDate();//To Input partNo at Common Function of getCurrentDate process.
                entity.reflectDate = dt011.Rows[i]["最終反映日"].ToString();//To Input reflectDate at dt011 of "最終反映日".
                // entity.orderCode = "ACZ10200";
                entity.orderCode = dt011.Rows[i]["発注コード"].ToString();//To Input orderCode at dt011 of "発注コード".
                entity.orderCode = fun.ReplaceOrderCode(entity.orderCode, new string[] { "-" });//To Input orderCode at Common Function of ReplaceOrderCode process.
                entity.purchaseURL = fun.url + "/index.php?action_goods=true&id=" + entity.orderCode + "00000";//To Input purchaseURL.

                /// <remark>
                /// To Check of Condition at Input Data.
                /// </remark>
                if (!string.IsNullOrWhiteSpace(entity.orderCode))
                {
                    string body = webBrowser1.Document.GetElementsByTagName("html")[0].InnerText;//To Input body at Webpage Inspect of html.

                    /// <remark>
                    /// To Check of body Condition at Webpage Inspect and Input Data.
                    /// </remark>
                    if (body.Contains("エラー [Error]"))
                    {
                        /// <remark>
                        /// To Check of stockDate and quantity.
                        /// </remark>
                        if (dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10") && dt011.Rows[i]["在庫情報"].ToString().Contains("empty"))
                        {
                            //fun.Qbei_ErrorInsert(11, "マルイ", "Item doesn't Exists!", entity.janCode, entity.orderCode, 2, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-01-10";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                        }
                        fun.Qbei_Inserts(entity);//Common Function of Qbei_Inserts Process.
                    }
                    else
                    {
                        try
                        {
                            string html = webBrowser1.Document.Body.InnerHtml;//To Input html at Webpage Inspect.
                            HtmlAgilityPack.HtmlDocument hdoc = new HtmlAgilityPack.HtmlDocument();
                            hdoc.LoadHtml(html);//hdoc Documennt of html.

                            /// <remark>
                            /// To Check of condition at stockDate and quantity.
                            /// </remark>
                            if ((hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[7]/td[2]/table/tbody/tr[1]/td/img") == null) && (hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]") == null))
                            {
                                /// <remark>
                                /// Common Function of Qbei_ErrorInsert Process.
                                /// </remark>
                                /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                                /// Common Function.
                                /// </param>
                                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                                /// <remark>
                                /// Common Function of WriteLog Process.
                                /// </remark>
                                /// <param　WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "011-")>
                                /// Common Function.
                                /// </param>
                                fun.WriteLog("Access Denied! " + entity.janCode + " " + entity.orderCode, "011-");
                                Application.Exit();
                                Environment.Exit(0);
                            }
                            else
                            {
                                HtmlNode node1 = hdoc.DocumentNode.SelectSingleNode("div[6]//div[2]//div//table//tbody//tr[4]//td//table//tbody//tr//td[2]//table//tbody//tr//td//table//tbody//tr[7]//td[2]//table//tbody//tr//td//img");//To Input node1 at hdoc Documennt of html.
                                string alt = node1.GetAttributeValue("alt", string.Empty);//To Input alt at node1.

                                entity.price = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[6]/td[2]").InnerText;//To Input price at hdoc Documennt of html.
                                entity.price = entity.price.Replace("￥", string.Empty).Replace(",", string.Empty);//Replace "￥" , ","  at entity.price.
                                entity.price = ((int)(Convert.ToDouble(entity.price) * 0.98)).ToString();//Convert to integer at entity.price.

                                string stockDate = hdoc.DocumentNode.SelectSingleNode("div[6]/div[2]/div/table/tbody/tr[4]/td/table/tbody/tr/td[2]/table/tbody/tr[1]/td/table/tbody/tr[8]/td[2]").InnerText;//To Input stockDate at hdoc Documennt of html.
                                entity.qtyStatus = alt.Equals("○") ? "good" : alt.Equals("△") ? "small" : alt.Equals("×") || alt.Equals("完売") ? "empty" : "unknown status";//To Input quantity at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown status"] .

                                /// <remark>
                                /// To Check of condition at stockDate .
                                /// </remark>
                                if (stockDate.Equals("-") || stockDate.Equals("未定"))
                                {
                                    entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? "2100-01-01" : alt.Equals("完売") ? "2100-02-01" : "unknown status";//To Input stockDate at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown status"] .
                                }
                                else
                                {
                                    string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//To Input date at Now.
                                    entity.stockDate = alt.Equals("○") || alt.Equals("△") || alt.Equals("×") ? stockDate : alt.Equals("完売") ? "2100-02-01" : "unknown date";//To Input stockDate at Check of alt Condition ["○" , "△" , "×" , "完売" , "unknown date"] .
                                }

                                /// <remark>
                                /// To Check of condition at stockDate and quantity.
                                /// </remark>
                                if ((dt011.Rows[i]["在庫情報"].ToString().Contains("empty") || dt011.Rows[i]["在庫情報"].ToString().Contains("inquiry")) && dt011.Rows[i]["入荷予定"].ToString().Contains("2100-01-10"))
                                {
                                    /// <remark>
                                    /// To Check of condition at stockDate and quantity.
                                    /// </remark>
                                    if ((entity.qtyStatus.Contains("empty") && (entity.stockDate.Contains("2100-01-01") || entity.stockDate.Contains("2100-02-01"))) || entity.qtyStatus.Contains("inquiry"))
                                    {
                                        entity.qtyStatus = dt011.Rows[i]["在庫情報"].ToString();//To Input quantity at dt011 of "在庫情報".
                                        entity.stockDate = dt011.Rows[i]["入荷予定"].ToString();//To Input stockDate at dt011 of "入荷予定".
                                        entity.price = dt011.Rows[i]["下代"].ToString();//To Input price at dt011 of "下代".
                                    }
                                    fun.Qbei_Inserts(entity);//Common Function of Qbei_Inserts Process.
                                }
                                else
                                    //2017/12/22 End
                                    fun.Qbei_Inserts(entity);//Common Function of Qbei_Inserts Process.
                            }
                        }
                        catch (Exception ex)
                        {
                            /// <remark>
                            /// Common Function of Qbei_ErrorInsert Process.
                            /// </remark>
                            /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                            /// Common Function.
                            /// </param>
                            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");


                            /// <remark>
                            /// Common Function of WriteLog Process.
                            /// </remark>
                            /// <param　WriteLog(ex, "011-", entity.janCode, entity.orderCode)>
                            /// Common Function.
                            /// </param>
                            fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);
                        }
                    }
                }
                else
                {
                    /// <remark>
                    /// Common Function of Qbei_ErrorInsert Process.
                    /// </remark>
                    /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                    /// Common Function.
                    /// </param>
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                }

                /// <remark>
                /// To Check of condition at dt011 rows .
                /// </remark>
                if (i < dt011.Rows.Count - 1)
                {
                    // string ordercode = "TIR16800";
                    string ordercode = fun.ReplaceOrderCode(dt011.Rows[++i]["発注コード"].ToString(), new string[] { "在庫処分/inquiry/", "在庫処分/empry/", "-", "在庫処分/good/", "在庫処分/small/", "在庫処分/empty/", "バラ注文できない為発注禁止/" });//To Input ordercode at Common Function of ReplaceOrderCode process.
                    webBrowser1.Navigate(fun.url + "/index.php?action_goods=true&id=" + ordercode + "00000");//WebBrowser Navigate of url.
                    webBrowser1.ScriptErrorsSuppressed = true;
                    webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_ItemSearch);//Next process.
                }
                else
                {
                    qe.site = 11;//Input SiteID Number.
                    qe.flag = 2;//Input flag Number.
                    qe.starttime = string.Empty; //Input starttime.
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//Input endtime.
                    fun.ChangeFlag(qe);//Common Function of ChangeFlag Process.
                    Application.Exit();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                /// <remark>
                /// Common Function of Qbei_ErrorInsert Process.
                /// </remark>
                /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
                /// Common Function.
                /// </param>
                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, entity.janCode, entity.orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

                /// <remark>
                /// Common Function of WriteLog Process.
                /// </remark>
                /// <param　WriteLog(ex, "011-", entity.janCode, entity.orderCode)>
                /// Common Function.
                /// </param>
                fun.WriteLog(ex, "011-", entity.janCode, entity.orderCode);

                Application.Exit();
                Environment.Exit(0);
            }
        }

        //NavigateErrorの　表示。
        private void instance_NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            string janCode = dt011.Rows[i]["JANコード"].ToString();//To Input janCode at dt011 of "JANコード".
            string orderCode = dt011.Rows[i]["発注コード"].ToString();//To Input orderCode at dt011 of "発注コード".

            /// <remark>
            /// Common Function of Qbei_ErrorInsert Process.
            /// </remark>
            /// <param　Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011")>
            /// Common Function.
            /// </param>
            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Access Denied!", janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");

            /// <remark>
            /// Common Function of WriteLog Process.
            /// </remark>
            /// <param　WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "011-")>
            /// Common Function.
            /// </param>
            fun.WriteLog(StatusCode.ToString() + " " + janCode + " " + orderCode, "011-");
            Application.Exit();
            Environment.Exit(0);
        }        
    }
}
