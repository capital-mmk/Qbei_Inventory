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

namespace ExportCSVforSite146
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
                IEnumerable<String> headerValues = dt.Columns.OfType<DataColumn>().Select(column => QuoteValue(column.ColumnName));
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
            return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
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
            lstProcess.Add("146");
            //(2025/05/08)Add

            foreach (string processName in lstProcess)
            {
                Process.GetProcessesByName(processName).ToList().ForEach(p => KillProcessAndChildren(p.Id));
            }

            /// <remark>
            /// Generate of CSV from SiteData of ID and Name.
            /// </remark>
            
            GenerateCSV(146, "ウインクレル");
            //(2025/05/08)Add
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
            try
            {
                QbeiUser_Entity que = new QbeiUser_Entity();
                QbeiUser_BL qubl = new QbeiUser_BL();
                DataTable dtresult = qubl.GetSiteData(siteID);
                //<remark Add Logic for change to unknowndate 2022/01/20 Start>
                dtresult.AsEnumerable().Where(r => r.Field<string>("quantity") == "unknown status").Select(r => r["quantity"] = "empty").ToList();
                dtresult.AsEnumerable().Where(r => r.Field<string>("stockDate") == "unknown date").Select(r => r["stockDate"] = "2100-01-01").ToList();
                //</remark 2022/01/20 End>
                //<remark Add Logic for stockdate at CSV 2021/09/24 Start>
                dtresult.AsEnumerable().Where(r => ((Convert.ToDateTime(r.Field<string>("stockDate")) >= DateTime.Now.AddMonths(9).Date)) && (r.Field<string>("stockDate") != "2100-02-01") && (r.Field<string>("stockDate") != "2100-01-10") && (r.Field<string>("stockDate") != "2100-01-01"))
                .Select(r => r["stockDate"] = "2100-01-01").ToList();
                //</remark 2021/09/24 End>
                if (dtresult != null && dtresult.Rows.Count > 0)
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
                else
                {

                }
            }
            catch (Exception ex)
            {
                string log = ex.ToString();
                ErrorLogSitData(log, siteID, sitename);
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

        //<remark Add Logic for GenerateCSV of Errorlog 2021/10/12 />
        public static void ErrorLogSitData(string strLog, int siteID, string siteName)
        {
            string logFilePath = "C:\\Qbei_Log\\ProcessKill_file\\" + "ErrorLog" + siteID + siteName + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
            log.WriteLine(strLog + "/" + siteID + "/" + siteName + "/" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
            log.Close();
        }
    }
}
