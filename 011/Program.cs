//<remark Edit Logic for new process 2022/02/28 Start>
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace _011マルイ
//{
//    static class Program
//    {
//        /// <summary>
//        /// The main entry point for the application.
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
//            Application.Run(new frm011());
//        }
//    }
//}
//</remark Edit Logic for new process 2022/02/28 End>

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
using System.Globalization;

namespace _011マルイ
{
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
        DataTable dt011 = new DataTable();
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
                qe.site = 11;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(11);
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
                    fun.deleteData(11);
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
                fun.WriteLog(ex, "011-");
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
            DataTable dt011 = new DataTable();
            try
            {
                fun.setURL("011");
                fun.Qbei_Delete(11);
                fun.Qbei_ErrorDelete(11);
                dt011 = fun.GetDatatable("011");
                fun.GetTotalCount("011");

            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "011-");
                Environment.Exit(0);
            }            
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                Qbei_Entity entity = new Qbei_Entity();          
                string orderCode = string.Empty;

                /// <summary>
                /// Inspection of item information at Mall.
                /// </summary>
                try
                {
                    int Lastrow = dt011.Rows.Count;
                    for (i = 0; i < Lastrow; i++)
                    {
                        if (i < Lastrow)
                        {                           
                            entity = new Qbei_Entity();
                            entity.siteID = 11;
                            entity.sitecode = "011";
                            entity.janCode = dt011.Rows[i]["JANコード"].ToString();
                            entity.partNo = dt011.Rows[i]["自社品番"].ToString();
                            entity.makerDate = fun.getCurrentDate();
                            entity.reflectDate = dt011.Rows[i]["最終反映日"].ToString();
                            entity.orderCode = dt011.Rows[i]["発注コード"].ToString().Trim();
                            entity.purchaseURL = "https://marui-ltd.jp/products?search_product_no="+entity.orderCode;

                        if (dt011.Rows[i]["在庫情報"].ToString().Equals("good"))
                        {
                            entity.qtyStatus = dt011.Rows[i]["在庫情報"].ToString();
                            entity.stockDate = dt011.Rows[i]["入荷予定"].ToString();
                            entity.price= dt011.Rows[i]["下代"].ToString();
                            entity.True_Quantity = entity.qtyStatus;
                            entity.True_StockDate = entity.stockDate;
                        }
                        else
                        {
                            entity.qtyStatus = "empty";
                            entity.stockDate = "2100-02-01";
                            entity.price = dt011.Rows[i]["下代"].ToString();
                            entity.True_Quantity = entity.qtyStatus;
                            entity.True_StockDate = entity.stockDate;
                        }

                           
                                        if ((entity.qtyStatus != "unknown status"))
                                        {
                                            if (entity.qtyStatus != null)
                                            {
                                                fun.Qbei_Inserts(entity);
                                            }
                                            else
                                            {
                                                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Jancode doesn't exit!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                                            }
                                        }
                                        else
                                        {
                                            fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Item doesn't Check!", entity.janCode, entity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                                        }
                                    }
                            else
                            {
                                fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), "Order Code Not Found!", entity.janCode, entity.orderCode, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                            }
                    }
                    qe.site = 11;
                    qe.flag = 2;
                    qe.starttime = string.Empty;
                    qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    fun.ChangeFlag(qe);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    string janCode = dt011.Rows[i]["JANコード"].ToString();
                    orderCode = dt011.Rows[i]["発注コード"].ToString();
                    fun.Qbei_ErrorInsert(11, fun.GetSiteName("011"), ex.Message, janCode, orderCode, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "011");
                    fun.WriteLog(ex, "011-", janCode, orderCode);
                }
            }
        }
    }
//}
