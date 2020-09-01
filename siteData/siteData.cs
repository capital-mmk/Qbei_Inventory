using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Management;

namespace siteData
{
    /// <summary>
    /// Export and Generate of CSV.
    /// </summary>
    public static class Rfc4180Writer
    {     
        /// <summary>
        /// Write of Log.
        /// </summary>
        /// <param name="dt">To Input of Datatable . </param>
        /// <param name="writer">Result of Text.</param>
    public static void WriteDataTable(DataTable dt, TextWriter writer, bool includeHeaders) 
    {
        if (includeHeaders) 
        {
            IEnumerable<String> headerValues = dt.Columns .OfType<DataColumn>().Select(column => QuoteValue(column.ColumnName));
            writer.WriteLine(String.Join(",", headerValues));
        }

        IEnumerable<String> items = null;

        foreach (DataRow row in dt.Rows) 
        {
            items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
            writer.WriteLine(String.Join(",", items));
        }

             writer.Flush();
        }

        /// <summary>
        /// Replace of String.
        /// </summary>
        /// <param name="value">To Input of vlue.</param>
        /// <returns>QuoteValue of String.</returns>
      private static string QuoteValue(string value)
      {
                return String.Concat("\"",value.Replace("\"", "\"\""), "\"");
      }

     }

    /// <summary>
    /// SiteData.
    /// </summary>
    /// <remark>
    /// Inspection Site Data and Site Name.
    /// </remark>
    public static class siteData
    {
        /// <remark>
        /// Inspection and Kill processing to SiteData of ID and Name.
        /// </remark>
        static void Main(string[] args)
        {
            List<string> lstProcess = new List<string>();
            lstProcess.Add("Rerun");
            lstProcess.Add("11マルイ");
            lstProcess.Add("12カワシマ");
            lstProcess.Add("013-mizutani");
            lstProcess.Add("014_Firefox");
            lstProcess.Add("016ライトウェイ");
            lstProcess.Add("17インターマックス");
            lstProcess.Add("018日直(ニチナオ)");
            lstProcess.Add("019深谷(フカヤ)");
            lstProcess.Add("20ダイアテック(高難易度)");
            lstProcess.Add("24東(アズマ)");
            lstProcess.Add("030");
            lstProcess.Add("031アキボウ");
            lstProcess.Add("34シマノ");
            lstProcess.Add("035");
            lstProcess.Add("0035");
            lstProcess.Add("36PRインターナショナル");
            lstProcess.Add("037_Chrome");
            lstProcess.Add("38フタバ");
            lstProcess.Add("46トライスポーツ");
            lstProcess.Add("053");
            lstProcess.Add("57モトクロス");
            lstProcess.Add("059");
            lstProcess.Add("65野口");
            lstProcess.Add("87ダートフリーク");
            lstProcess.Add("139");
            lstProcess.Add("143");
            lstProcess.Add("914");
            lstProcess.Add("916_Chrome");
            //(2019/07/24)Add
            lstProcess.Add("124-Mizutani");
            lstProcess.Add("023パナソニック");//Add Logic 2020/09/01

            foreach (string processName in lstProcess)
            {
                Process.GetProcessesByName(processName).ToList().ForEach(p => KillProcessAndChildren(p.Id));
            }

            /// <remark>
            /// Generate of CSV from SiteData of ID and Name.
            /// </remark>
            GenerateCSV(11, "マルイ");
            GenerateCSV(12, "カワシマ");
            GenerateCSV(13, "ミズタニ");
            GenerateCSV(14, "イワイ");
            //GenerateCSV(15, "アキ");
            GenerateCSV(16, "ライトウェイ");
            GenerateCSV(17, "インターマックス");
            GenerateCSV(18, "日直");
            GenerateCSV(19, "深谷_フカヤ_");
            GenerateCSV(20, "ダイアテック(高難易度)");
            //GenerateCSV(22, "ブリヂストンサイクル西日本販売(株)物流部専用");
            GenerateCSV(23, "パナソニック");
            GenerateCSV(24, "東(アズマ)");
            GenerateCSV(30, "城東");
            GenerateCSV(31, " アキボウ");
            GenerateCSV(34, "シマノ");
            GenerateCSV(35, "インターテック");
            GenerateCSV(36, "インターナショナル");
            GenerateCSV(37, "東京サンエス(株)");
            GenerateCSV(38, "フタバ");
            //GenerateCSV(42, "アメア");
            GenerateCSV(46, " トライスポーツ");
            GenerateCSV(53, " 発注モジュールひな形");
            GenerateCSV(57, " モトクロス");
            GenerateCSV(59, "(株)ジェイピースポーツグループ");
            //GenerateCSV(58, " リンエイ");
            GenerateCSV(65, "野口");
            GenerateCSV(84, "(今期取り扱い商品無し保留)");
            GenerateCSV(87, "ダートフリーク");
            //GenerateCSV(104, "宮田(中川商会扱い)");
            //GenerateCSV(124, "ミズタニ自転車（下鴨アカウントのみ発注可能");
            GenerateCSV(139, "ウエイブワン株式会社");
            GenerateCSV(143, "（株）ポディウム");
            GenerateCSV(914, "（株）イノセントデザインワークス");
            GenerateCSV(916, "(株)あさひ");
            //(2019/07/24)Add
            GenerateCSV(124, "ミズタニ自転車");
        }

        /// <summary>
        /// Kill Process of DataBase.
        /// </summary>
        /// <param name="pid">Input to ProcessID.</param>
        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                try
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                catch
                {
                    break;
                }
            }

            try
            {
                Process proc = Process.GetProcessById(pid);
                string SiteName = proc.ProcessName;
                WriteLogSitData("Process have been Killed  at SiteID- ", SiteName);
                proc.Kill();

            }
            catch
            {

            }
        }

        /// <summary>
        /// GenerateCSV of DataTable.
        /// </summary>
        /// <param name="siteID">Input to siteID.</param>
        /// <param name="sitename">Input to sitename.</param>
        /// <remark>
        /// Create of DataTable.
        /// </remark>
        private static void GenerateCSV(int siteID, string sitename)
        {
            QbeiUser_Entity que = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            DataTable dtresult = qubl.GetSiteData(siteID);
            if (dtresult!= null && dtresult.Rows.Count > 0)
            {
                int dtcount = Convert.ToInt32(dtresult.Rows.Count);
                string date = string.Format("{0:yyyyMMddhhmm}", DateTime.Now);
                DataTable dt = new DataTable();

                dt.Columns.Add("代理店ID");
                dt.Columns.Add("JANコード");
                dt.Columns.Add("発注コード");
                dt.Columns.Add("在庫情報");
                dt.Columns.Add("下代");
                dt.Columns.Add("入荷予定");
                dt.Columns.Add("purchaserURL");

                dt.Columns.Add("自社品番");
                dt.Columns.Add("メーカー情報日");
                dt.Columns.Add("最終反映日");
                dt.Columns.Add("Updated_Date");
                dt.Columns.Add("sitecode");

                for (int i = 0; i < dtcount; i++)
                {
                    dt.Rows.Add(dtresult.Rows[i]["siteID"].ToString(),
                                dtresult.Rows[i]["jancode"].ToString(),
                                dtresult.Rows[i]["orderCode"].ToString(),
                                dtresult.Rows[i]["quantity"].ToString(),
                                dtresult.Rows[i]["price"].ToString(),
                                dtresult.Rows[i]["stockDate"].ToString(),
                                dtresult.Rows[i]["purchaserURL"].ToString(),
                                dtresult.Rows[i]["partNo"].ToString(),
                                dtresult.Rows[i]["makerDate"].ToString(),
                                dtresult.Rows[i]["reflectDate"].ToString(),
                                dtresult.Rows[i]["Updated_Date"].ToString(),
                                dtresult.Rows[i]["sitecode"].ToString());
                }


                using (StreamWriter writer = new StreamWriter(new FileStream("C:\\Qbei_Log\\ExportCSV\\" + siteID + "_maker_stock_" + date + ".csv", FileMode.Create, FileAccess.ReadWrite), Encoding.GetEncoding(932)))
                {
                    Rfc4180Writer.WriteDataTable(dt, writer, true);
                }
            }
            else {
                
            }
        }

        /// <summary>
        /// WriteLog for ProcessKill.
        /// </summary>
        /// <param name="strLog">Input to Log.</param>
        /// <param name="siteID">Input to siteID.</param>
        public static void WriteLogSitData(string strLog, string siteID)
        {
            string logFilePath = "C:\\Qbei_Log\\ProcessKill_file\\" + "Log" + siteID + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            FileStream fileStream = null;
            FileInfo logFileInfo = new FileInfo(logFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            StreamWriter log = new StreamWriter(fileStream);
            log.WriteLine(strLog + siteID + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
            log.Close();
        }
    }
}
       
    

