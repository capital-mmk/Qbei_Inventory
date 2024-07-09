using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Data.SqlClient;
using LumenWorks.Framework.IO.Csv;
using HtmlAgilityPack;
using OpenQA.Selenium;
using System.Windows.Forms;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Text.RegularExpressions;

namespace Common
{
    public class CommonFunction
    {
        public string url = string.Empty;
        public string user = string.Empty;
        public string password = string.Empty;
        public string downloadPath014 = string.Empty;
        public string downloadPath015 = string.Empty;
        public string downloadPath035 = string.Empty;
        public string logPath = string.Empty;
        public string csvPath = string.Empty;
        public string trashPath = string.Empty;
        public string logFilepath = string.Empty;
        public string excelPath016 = string.Empty;
        int ddr;
        DataTable dtOrder = new DataTable();

        /// <summary>
        /// Convert DataTable from Xml format string
        /// </summary>
        /// <param name="dt">DataTable for Xml.</param>
        /// <returns>xml format string form DataTable dt.</returns>
        public String DataTableToXml(DataTable dt)
        {
            dt.TableName = "test";
            System.IO.StringWriter writer = new System.IO.StringWriter();
            dt.WriteXml(writer, XmlWriteMode.WriteSchema, false);
            string result = writer.ToString();
            result = Regex.Replace(result, @"&#(x?)([A-Fa-f0-9]+);", "");
            return result;
        }

        /// <summary>
        /// Remove Invalid Character. 
        /// </summary>
        /// <param name="content">Remove to invalid chars of string.</param>
        /// <returns> string already remove special characters</returns>
        /// <remark>
        /// Xml doesn't allow special characters.
        /// </remark>
        public static string RemoveInvalidXmlChars(string content)
        {
            return content = Regex.Replace(content, @"&#(x?)([A-Fa-f0-9]+);", "");
        }

        /// <summary>
        /// ShopID setURL.
        /// </summary>
        /// <param name="shopID">Insert to setURl of Shop ID.</param>    
        public void setURL(string shopID)
        {
            createConfig(shopID);
        }


        /// <summary>
        /// Check  createconfig.
        /// </summary>
        /// <param name="shopID">Insert to Config of ShopID.</param>
        ///<remark>
        ///Check to Database connection string at App.config.
        ///</remark>
        private static void createConfig(string shopID)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(@"C:\Qbei_Log\Config1\App.config");
            if (config.ConnectionStrings.ConnectionStrings["Qbei_DB"] == null)
            {
                ConnectionStringSettings setting = new ConnectionStringSettings("Qbei_DB", "Data Source= WIN-OIL4TFU9NBH\\LOCAL2014;Initial Catalog=Qbei_Inventory;Persist Security Info=True;User ID=sa;Password=admin123456!", "System.Data.SqlClient");
                config.ConnectionStrings.ConnectionStrings.Add(setting);
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// updateSetting.
        /// </summary>
        /// <param name="key">Insert to AppSettings Setting of key.</param>
        /// <param name="value">Insert to AppSettings Setting of value.</param>
        /// <remark>
        /// Do not Use of Qbei_Inventory at now.
        /// </remark>
        public static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Add Config.
        /// </summary>
        /// <param name="config">Insert to Appsetting of config.</param>
        /// <param name="key">Insert to AppSettings Setting of key.</param>
        /// <param name="value">Insert to AppSettings Setting of value.</param>
        /// <remark>
        /// Check at config appsetting of key.
        /// Do not Use of Qbei_Inventory at now.
        /// </remark>
        private static void AddtoConfig(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        /// <summary>
        /// Add config(user).
        /// </summary>
        /// <param name="config">Insert to Appsetting of config.</param>
        /// <param name="shopID">Insert to shopID.</param>
        /// <param name="url">Insert to shop for url.</param>
        /// <param name="user">Insert to user.</param>
        /// <param name="password">Insert to password.</param>
        /// <remark>
        /// Check at config appsetting of User.
        /// Do not Use of Qbei_Inventory at now.
        /// </remark>
        private static void AddtoConfig(Configuration config, string shopID, string url, string user, string password)
        {
            if (config.AppSettings.Settings["url" + shopID] == null)
                config.AppSettings.Settings.Add("url" + shopID, url);
            else
                config.AppSettings.Settings["url" + shopID].Value = url;

            if (config.AppSettings.Settings["user" + shopID] == null)
                config.AppSettings.Settings.Add("user" + shopID, user);
            else
                config.AppSettings.Settings["user" + shopID].Value = user;

            if (config.AppSettings.Settings["password" + shopID] == null)
                config.AppSettings.Settings.Add("password" + shopID, password);
            config.AppSettings.Settings["password" + shopID].Value = password;
        }

        /// <summary>
        /// Create of Directory.
        /// </summary>
        public void CreateFileAndFolder()
        {
            CreateDirectory(@"C:\Qbei_Log");
            CreateDirectory(@"C:\Qbei_Log\Csv");
            CreateFilePath(@"C:\Qbei_Log\logfile\log.txt");
            CreateDirectory(@"C:\Qbei_Log\Trash\");
            CreateDirectory(@"C:\Qbei_Log\014_Download\");
            CreateDirectory(@"C:\Qbei_Log\015_Download\");
            CreateDirectory(@"C:\Qbei_Log\035_Download\");
            CreateDirectory(@"C:\Qbei_Log\016_Excel\");
        }

        /// <summary>
        /// Writetolog of message.
        /// </summary>
        /// <param name="message"> Message for Write Log.</param>
        public void WritetoLog(string message)
        {
            File.AppendAllText(logFilepath, Environment.NewLine + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + message);
        }

        /// <summary>
        /// GetDatatable of shopID.
        /// </summary>
        /// <param name="shopID">To insert of shopID. </param>
        /// <remark>
        ///Check to CSV File of Data and Insert to Data at Table.
        /// </remark>
        public DataTable GetDatatable(string shopID)
        {
            DataTable dtResult = new DataTable();
            DataTable dtNotNull = new DataTable();
            DataTable dtNotInteger = new DataTable();
            DataTable dtNotRun = new DataTable();
            DataColumn dc = new DataColumn("SiteName");
            DataColumn da = new DataColumn("SiteName");
            string xml;
            Connection con;
            SqlConnection sqlcon;
            SqlCommand cmd;
            dc.DefaultValue = GetSiteName(shopID);
            string[] columns = { "代理店ID", "JANコード", "在庫情報", "入荷予定", "自社品番", "メーカー情報日", "最終反映日" };
            string[] filelist = Directory.GetFiles(@"C:\Qbei_Log\Csv");

            foreach (string file in filelist)
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".csv"))
                {
                    using (var csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding(932)), true))
                    {
                        DataTable dtCsv = new DataTable();
                        dtCsv.Load(csv);
                        if (dtResult.Columns.Count <= 0)
                            dtResult = dtCsv.Clone();

                        if (checkCsvFormat(dtCsv, columns))
                        {
                            dtResult.Merge(dtCsv);
                        }
                        else
                        {
                            WritetoLog("CsvFile Format Wrong!");
                            return null;
                        }

                    }

                }
                else
                    File.Move(file, @"C:\Qbei_Log\Trash\" + @"\" + Path.GetFileName(file));
            }
            if (!dtResult.Equals(null))
            {
                DataRow[] dr = dtResult.Select("代理店ID='" + shopID + "'");

                ddr = dr.Count();
                if (dr.Count() > 0)
                {
                    DataTable dtTemp = dtResult.Select("代理店ID='" + shopID + "'").CopyToDataTable();
                    //if (shopID == "036")
                    //{ return dtTemp; }
                    if (shopID.Equals("053"))
                    {
                        dtTemp = GetBrandCode(dtTemp);
                        ddr = dtTemp.Rows.Count;
                    }
                    //
                    //<remark Close Logic 2021/10/29 Start>
                    //if (shopID.Equals("036"))
                    //{
                    //    DataTable dtpblank = dtTemp.Select("purchaserURL =' ' OR purchaserURL = '' OR purchaserURL is NULL").CopyToDataTable();
                    //    int dtcount = dtpblank.Rows.Count;
                    //    if (dtpblank != null)
                    //    {
                    //        /// <remark>
                    //        /// Save Data into Qbei_ErrorLog
                    //        /// </remark>

                    //        xml = DataTableToXml(dtpblank);

                    //        con = new Connection();
                    //        sqlcon = con.GetConnection();
                    //        cmd = new SqlCommand("Qbei_ErrorLog_3_InsertXml", sqlcon);
                    //        cmd.CommandType = CommandType.StoredProcedure;
                    //        cmd.Parameters.AddWithValue("@xml", xml);
                    //        cmd.Parameters.AddWithValue("@sitename", GetSiteName(shopID));
                    //        cmd.Parameters.AddWithValue("@Description", "Item doesn't Run!");
                    //        cmd.Parameters.AddWithValue("@ErrorType", 6);
                    //        cmd.Parameters.AddWithValue("@SiteCode", shopID);
                    //        cmd.CommandTimeout = 600;
                    //        cmd.Connection.Open();
                    //        cmd.ExecuteNonQuery();
                    //        cmd.Connection.Close();
                    //        var runData = dtTemp.AsEnumerable().Where(x => !dtpblank.AsEnumerable().Any(y => y.Field<string>("JANコード") == x.Field<string>("JANコード") && y.Field<string>("発注コード") == x.Field<string>("発注コード")));
                    //        dtTemp = runData.Any() ? runData.CopyToDataTable() : null;
                    //    }
                    //}
                    //</remark 2021/10/29 End>
                    //
                    //dr = dtTemp.Select("発注コード=' ' OR 発注コード = '' OR 発注コード is NULL OR 発注コード= '-' OR 発注コード= '--'");<remark Edit Logic for ordercode 2021/04/05 />
                    dr = dtTemp.Select("発注コード=' ' OR 発注コード = '‐' OR 発注コード like '|%' OR 発注コード = '' OR 発注コード is NULL OR 発注コード = 'NULL' OR 発注コード= '-' OR 発注コード= '--' OR 発注コード like '/%' OR 発注コード like '-%'");
                    if (dr.Count() > 0)
                    {
                        //if (!shopID.Equals("036"))
                        //{
                        /// <remark>
                        /// Save Data into Qbei_ErrorLog
                        /// </remark>
                        //DataTable dtBlankOrder = dtTemp.Select("発注コード=' ' OR 発注コード = '' OR 発注コード is NULL OR 発注コード='-' OR 発注コード= '--'").CopyToDataTable();<remark Edit Logic for ordercode 2021/04/05 />
                        DataTable dtBlankOrder = dtTemp.Select("発注コード=' 'OR 発注コード = '‐' OR 発注コード like '|%' OR 発注コード = '' OR 発注コード is NULL OR 発注コード = 'NULL' OR 発注コード='-' OR 発注コード= '--'OR 発注コード like '/%' OR 発注コード like '-%'").CopyToDataTable();
                        dtBlankOrder.Columns.Add(dc);
                        int col = dtBlankOrder.Rows.Count;
                        xml = DataTableToXml(dtBlankOrder);
                        con = new Connection();
                        sqlcon = con.GetConnection();
                        cmd = new SqlCommand("Qbei_ErrorLog_InsertXml", sqlcon);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@xml", xml);
                        cmd.Parameters.AddWithValue("@SiteCode", shopID);
                        cmd.CommandTimeout = 600;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();

                        //}
                    }


                    dr = dtTemp.Select("発注コード<>' ' AND 発注コード <> '‐' AND 発注コード <> '' AND 発注コード is not NULL AND  発注コード <> 'NULL' AND 発注コード<> '-' AND 発注コード<> '--' ");
                    if (dr.Count() > 0)
                    {
                        //if (!shopID.Equals("036"))
                        //{
                        dtNotNull = dtTemp.Select("発注コード<>' 'AND 発注コード <> '‐' AND 発注コード <> '' AND 発注コード is not NULL AND 発注コード <> 'NULL' AND 発注コード<> '-' AND 発注コード<> '--'").CopyToDataTable();
                        //Trim 
                        dtNotNull.AsEnumerable().ToList().ForEach(r => r["発注コード"] = r.Field<string>("発注コード").Trim());
                        //<remark Add Logic 2021/05/04 Start>
                        //<remark Add Logic 2021/09/10 Start>
                        DataRow[] dr_percent = dtTemp.Select("発注コード like '/%' OR 発注コード like '|%' OR 発注コード like '-%'");
                        if (dr_percent.Count() > 0)
                        {
                            DataTable dtselect = dtTemp.Select("発注コード like '/%' OR 発注コード like '|%' OR 発注コード like '-%'").CopyToDataTable();
                            var select = dtNotNull.AsEnumerable().Where(r => !dtselect.AsEnumerable().Any(y => y.Field<string>("JANコード") == r.Field<string>("JANコード")));
                            dtNotNull = select.OrderBy(x => x.Field<string>("メーカー情報日")).CopyToDataTable();
                        }
                        //</remark Add Logic 2021/09/10 End>
                        //</remark Add Logic 2021/05/04 End>
                        //2018-05-07 Start
                        //var notInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Contains("在庫") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("/") || r.Field<string>("発注コード").Contains("データ登録")));
                        // var notInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Contains("在庫") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録")));

                        //var notInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録")));
                        //var notInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録") || r.Field<string>("発注コード").Contains("#N/A")));//<remark Add Logic for Ordercode 2020/07/30 />
                        var notInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫処分/empty/-") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録") || r.Field<string>("発注コード").Contains("#N/A") || r.Field<string>("発注コード").Contains("|") || r.Field<string>("発注コード").Contains(".")));//<remark Add Logic for Ordercode 2022/05/24 />
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     //2018-05-07 End
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     //2018-08-29 Start
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     //var notInteger2 = notInteger.AsEnumerable().Where(r => (!r.Field<string>("発注コード").Contains("在庫更新中止")));
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     //2018-08-29 End
                        if (notInteger.Any())
                        {
                            /// <remark>
                            /// Save Data into Qbei_ErrorLog
                            /// </remark>

                            //2018-05-07 Start
                            //dtNotInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Contains("在庫") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("/") || r.Field<string>("発注コード").Contains("データ登録"))).CopyToDataTable();
                            //dtNotInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録"))).CopyToDataTable();
                            //dtNotInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録") || r.Field<string>("発注コード").Contains("#N/A"))).CopyToDataTable();//<remark Add Logic for Ordercode 2020/07/30 />
                            dtNotInteger = dtNotNull.AsEnumerable().Where(r => (r.Field<string>("発注コード").Equals("在庫処分/empty/") || r.Field<string>("発注コード").Equals("在庫処分/empty/-") || r.Field<string>("発注コード").Equals("在庫更新中止/-") || r.Field<string>("発注コード").Equals("在庫更新中止") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("データ登録") || r.Field<string>("発注コード").Contains("#N/A") || r.Field<string>("発注コード").Contains("|") || r.Field<string>("発注コード").Contains("."))).CopyToDataTable();//<remark Add Logic for Ordercode 2022/05/24 />
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         //2018-05-07 End
                            dc = new DataColumn("SiteName");
                            dc.DefaultValue = GetSiteName(shopID);
                            dtNotInteger.Columns.Add(dc);
                            xml = DataTableToXml(dtNotInteger);
                            con = new Connection();
                            sqlcon = con.GetConnection();
                            cmd = new SqlCommand("Qbei_ErrorLog_2_InsertXml", sqlcon);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@xml", xml);
                            cmd.Parameters.AddWithValue("@SiteCode", shopID);
                            cmd.CommandTimeout = 600;
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();

                            var data = dtNotNull.AsEnumerable().Where(r => !dtNotInteger.AsEnumerable().Any(y => y.Field<string>("JANコード") == r.Field<string>("JANコード") && y.Field<string>("発注コード") == r.Field<string>("発注コード")));
                            if (data.Any())
                                dtOrder = data.OrderBy(x => x.Field<string>("メーカー情報日")).CopyToDataTable();
                            else
                                dtOrder = null;
                        }
                        else
                            dtOrder = dtNotNull.AsEnumerable().OrderBy(x => x.Field<string>("メーカー情報日")).CopyToDataTable();
                        //}


                        //if (shopID.Equals("036"))
                        //{
                        //    dtOrder = dtTemp.AsEnumerable().OrderBy(x => x.Field<string>("メーカー情報日")).CopyToDataTable();
                        //}
                        //Check (ステータス変更日+6) <= today   

                        //<remark ９ヶ月前の　データーについて、更新ロジック　2020-01-30 Start>
                        ////2018-07-04 Start
                        ////var notRun = dtOrder.AsEnumerable().Where(x => x.Field<string>("在庫情報").Contains("empty") && x.Field<string>("ステータス変更日") != null && DateTime.Parse(x.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-6).Date);
                        //var notRun = dtOrder.AsEnumerable().Where(x => x.Field<string>("在庫情報").Contains("empty") && x.Field<string>("ステータス変更日") != null && DateTime.Parse(x.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-9).Date);
                        ////2018-07-04 End
                        ////2018-08-29 Start
                        //var empty = notRun.AsEnumerable().Where(r => (r.Field<string>("発注コード").Contains("在庫更新中止")));
                        //var select = empty.AsEnumerable().Select(x =>( x.Field<string>("発注コード").Contains("在庫更新中止")));
                        //dtNotRun = empty.Any() ? empty.CopyToDataTable() : null;
                        ////2018-08-29 End
                        //// dtNotRun = notRun.Any() ? notRun.CopyToDataTable() : null;
                        //var notRun = dtOrder.AsEnumerable().Where(x => x.Field<string>("在庫情報").Contains("empty") && x.Field<string>("ステータス変更日") != null && x.Field<string>("入荷予定") == "2100-02-01" && DateTime.Parse(x.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-9).Date);//<remark Edit Logic for 18ヶ月前の　データー 2021/05/26 />
                        var notRun = dtOrder.AsEnumerable().Where(x => x.Field<string>("在庫情報").Contains("empty") && x.Field<string>("ステータス変更日") != null && x.Field<string>("入荷予定") == "2100-02-01" && DateTime.Parse(x.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-18).Date);
                        dtNotRun = notRun.Any() ? notRun.CopyToDataTable() : null;
                        //</remark 2020-01-30 End>
                        if (dtNotRun != null)
                        {

                            /// <remark>
                            /// Save Data into Qbei_ErrorLog
                            /// </remark>

                            xml = DataTableToXml(dtNotRun);
                            //xml = RemoveInvalidXmlChars(xml);
                            con = new Connection();
                            sqlcon = con.GetConnection();
                            cmd = new SqlCommand("Qbei_ErrorLog_3_InsertXml", sqlcon);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@xml", xml);
                            cmd.Parameters.AddWithValue("@sitename", GetSiteName(shopID));
                            cmd.Parameters.AddWithValue("@Description", "Item doesn't Run!(18M)");
                            cmd.Parameters.AddWithValue("@ErrorType", 6);
                            cmd.Parameters.AddWithValue("@SiteCode", shopID);
                            cmd.CommandTimeout = 600;
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                            //<remark Edit Logic for dtNotRun 2021/04/23 Start>
                            var runData = dtOrder.AsEnumerable().Where(x => !dtNotRun.AsEnumerable().Any(y => y.Field<string>("JANコード") == x.Field<string>("JANコード") && y.Field<string>("発注コード") == x.Field<string>("発注コード")));
                            dtOrder = runData.Any() ? runData.CopyToDataTable() : null;
                        }

                        //var runData = dtOrder.AsEnumerable().Where(x => !dtNotRun.AsEnumerable().Any(y => y.Field<string>("JANコード") == x.Field<string>("JANコード") && y.Field<string>("発注コード") == x.Field<string>("発注コード")));
                        //dtOrder = runData.Any() ? runData.CopyToDataTable() : null;
                        //</remark 2021/04/23 End>

                        //<remark 18ヶ月前の　データーについて、更新ロジック　2021-06-03 Start> 
                        //<remark ９ヶ月前の　データーについて、更新ロジック　2020-01-30 Start>                         
                        // dtOrder.AsEnumerable().Where(r => (DateTime.Parse(r.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-9).Date))
                        //.Select(r => r["入荷予定"] = "2100-02-01").ToList();
                        // dtOrder.AsEnumerable().Where(r => (DateTime.Parse(r.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-9).Date))
                        //.Select(r => r["在庫情報"] = "empty").ToList();

                        if (dtOrder != null) //</remark 2023-04-26> "IF CONDITION" added by ct
                        {
                            dtOrder.AsEnumerable().Where(r => (DateTime.Parse(r.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-18).Date))
                           .Select(r => r["入荷予定"] = "2100-02-01").ToList();
                            dtOrder.AsEnumerable().Where(r => (DateTime.Parse(r.Field<string>("ステータス変更日").ToString()) <= DateTime.Now.AddMonths(-18).Date))
                           .Select(r => r["在庫情報"] = "empty").ToList();
                        }
                        //</remark 2020-01-30 End>   
                        //</remark 2021-06-03 End>

                        //<remark Add Logic for Siteid-110 2021/04/09 Start>
                        if (shopID.Equals("110"))
                        {
                            int check_ci_count = dtOrder.Select("自社品番 like 'ci%'").Count();//<remark Add Logic for Siteid-110 2022/03/07 />
                            if (check_ci_count > 0)//<remark Add Logic for Siteid-110 2022/03/07 />
                            {
                                DataTable dt_ci = dtOrder.Select("自社品番 like 'ci-%'").CopyToDataTable();
                                xml = DataTableToXml(dt_ci);
                                //xml = RemoveInvalidXmlChars(xml);
                                con = new Connection();
                                sqlcon = con.GetConnection();
                                cmd = new SqlCommand("Qbei_ErrorLog_3_InsertXml", sqlcon);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@xml", xml);
                                cmd.Parameters.AddWithValue("@sitename", GetSiteName(shopID));
                                cmd.Parameters.AddWithValue("@Description", "ci-Item");
                                cmd.Parameters.AddWithValue("@ErrorType", 6);
                                cmd.Parameters.AddWithValue("@SiteCode", shopID);
                                cmd.CommandTimeout = 600;
                                cmd.Connection.Open();
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                            int check_count = dtOrder.Select("自社品番 not like 'ci-%'").Count();//<remark Add Logic for Siteid-110 2022/03/07 />
                            if (check_count > 0)//<remark Add Logic for Siteid-110 2022/03/07 />
                            {
                                dtOrder = dtOrder.Select("自社品番 not like 'ci-%'").CopyToDataTable();
                            }
                        }
                        //</remark 2021/04/09 End>
                    }
                    return dtOrder;
                }
                return null;
            }
            else return null;
        }

        /// <summary>
        /// StopApplication of site.
        /// </summary>
        /// <param name="siteID">Insert to siteid for Site.</param>
        public void StopApplication(int siteID)
        {
            /// <remark>
            /// Change to flag of siteid.
            /// </remark>
            Qbeisetting_Entity qe = new Qbeisetting_Entity();
            qe.site = siteID;
            qe.flag = 2;
            qe.starttime = string.Empty;
            qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ChangeFlag(qe);
            Application.Exit();
            Environment.Exit(0);
        }

        /// <summary>
        /// GetTotalCount of shopID.
        /// </summary>
        /// <param name="shopID">Insert to shopID for Totalcount.</param>
        public void GetTotalCount(string shopID)
        {
            /// <remark>
            /// Update to TotalCount at site_setting Table.
            /// </remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("TotalCount_Update", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TotalCount", ddr);
            cmd.Parameters.AddWithValue("@SiteID", shopID);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }

        /// <summary>
        /// Old Code.
        /// </summary>
        /// <param name="dtResult">Check to Data at Data Table.</param>
        /// <param name="shopID">Insert to shopID for Sitecode.</param>
        /// <returns>Insert to Error data at Datable.</returns>
        public DataTable DeleteOldCode(DataTable dtResult, int shopID)
        {
            DataTable dtOrder;
            //<remark Add Logic for check to OldCode 2023/03/28 Start>
            int check = dtResult.AsEnumerable().Where(x => (x.Field<string>("発注コード").Contains("|"))).Count();
            if (check == 0)
            {
                return dtResult;
            }
            //if (!dtResult.Equals(null))
            //</remark 2023/03/28 End>
            else if (!dtResult.Equals(null))
            {
                /// <remark>
                /// Insert Data into Qbei_ErrorLog
                /// </remark>
                DataColumn dc = new DataColumn("SiteName");
                DataTable dtoldcode = dtResult.AsEnumerable().Where(x => (x.Field<string>("発注コード").Contains("|"))).CopyToDataTable();
                int col = dtoldcode.Rows.Count;
                dtoldcode.Columns.Add("dc");
                string xml = DataTableToXml(dtoldcode);
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
                Configuration config = ConfigurationManager.OpenExeConfiguration(@"C:\Qbei_Log\Config1\App.config");
                string constr = config.ConnectionStrings.ConnectionStrings["Qbei_DB"].ConnectionString;
                SqlCommand cmd = new SqlCommand("Qbei_ErrorLog_OldCode_InsertXml", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@xml", xml);
                cmd.Parameters.AddWithValue("@sitename", "ダイアテック");
                cmd.Parameters.AddWithValue("@SiteCode", shopID);
                cmd.CommandTimeout = 600;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                dtOrder = dtResult.AsEnumerable().Where(x => !(x.Field<string>("発注コード").Contains("|"))).CopyToDataTable();


                return dtOrder;
            }
            else return null;

        }

        /// <summary>
        /// Selete to Order Data.
        /// </summary>
        /// <param name="strSiteCode">Insert to sitecode.</param>
        /// <returns>Select to order Data from Sitecode.</returns>
        public DataTable Qbei_OrderSelect(string strSiteCode)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_OrderSelect", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@siteCode", strSiteCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtQbei_Order = new DataTable();
            try
            {
                cmd.Connection.Open();
                da.Fill(dtQbei_Order);
                return dtQbei_Order;
            }
            catch (Exception)
            { return new DataTable(); }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Check to GetBrandCode.
        /// </summary>
        /// <param name="dtCsv">Check to CSV DataTable.</param>
        /// <returns>Check to Datatable of Brandcode("00349") for shopID("053").</returns>
        public DataTable GetBrandCode(DataTable dtCsv)
        {
            try
            {
                DataTable dtcode = dtCsv.AsEnumerable().Where(x => (x.Field<string>("ブランドコード").Contains("00349"))).CopyToDataTable();
                return dtcode;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// GetOrderData of Once a Week.
        /// </summary>
        /// <param name="dtCsv">Check to CSV DataTable.</param>
        /// <param name="strPurchaseUrl">Insert to PruchaseUrl of site.</param>
        /// <param name="strSiteCd">Insert to SiteCode of site.</param>
        /// <param name="strPost">Insert to Post of site.</param>
        /// <returns>Check to Onceaweek csae at DataTable.</returns>
        public DataTable GetOrderData(DataTable dtCsv, string strPurchaseUrl, string strSiteCd, string strPost)
        {
            Qbei_Entity objEntity;
            DataTable dtData = null;
            DataTable dtUncheck;
            DataTable dtOnceaWeek;
            DataTable dtID = new DataTable();
            DataView dvID;
            string strCondition = string.Empty;
            try
            {
                //Retrieve Order Data
                DataTable dtOrder = Qbei_OrderSelect(strSiteCd);
                dtData = dtCsv.Copy();
                //Retrieve Once a Week Data from CSV
                var data = dtData.AsEnumerable().Where(r => (r.Field<string>("在庫情報").Equals("empty") &&
                                                                (r.Field<string>("入荷予定") == null ||
                                                                 r.Field<string>("入荷予定").Equals("2100-01-01") ||
                                                                 r.Field<string>("入荷予定").Equals("2100-02-01"))
                                                            ) ||
                                                            (r.Field<string>("在庫情報").Equals("inquiry") &&
                                                                (r.Field<string>("入荷予定") == null ||
                                                                 !(r.Field<string>("入荷予定").Equals("2100-01-01") && string.IsNullOrEmpty(r.Field<string>("purchaserURL"))) ||
                                                                 !r.Field<string>("入荷予定").Equals("2100-01-10"))
                                                            )
                                                      );
                dtOnceaWeek = data.Any() ? data.CopyToDataTable() : null;

                //2018-08-29 Start
                //var notintegerdata2 = data.AsEnumerable().Where(r => (!r.Field<string>("発注コード").Contains("在庫更新中止")));
                //dtOnceaWeek = notintegerdata2.Any() ? notintegerdata2.CopyToDataTable() : null;
                //2018-08-29 End
                if (dtOnceaWeek != null)
                {
                    var notexistdata = dtData.AsEnumerable().Where(r => !dtOnceaWeek.AsEnumerable().Any(y => y.Field<string>("JANコード") == r.Field<string>("JANコード")));
                    if (notexistdata.Any()) dtData = notexistdata.CopyToDataTable();
                }
                if (dtOrder.Rows.Count > 0)
                {
                    var temp = dtOrder.AsEnumerable().Where(x => dtOnceaWeek.AsEnumerable().Any(y => y.Field<string>("JANコード") == x.Field<string>("jancode") && DateTime.Parse(x.Field<string>("checkDate")) <= DateTime.Now));
                    if (temp.Any())
                    {
                        dtUncheck = temp.CopyToDataTable();
                        dvID = new DataView(dtUncheck);
                        dtID = dvID.ToTable(false, "jancode");
                        var onceaweek = dtOnceaWeek.AsEnumerable().Where(r => !dtUncheck.AsEnumerable().Any(y => y.Field<string>("jancode") == r.Field<string>("JANコード")));
                        if (onceaweek.Any())
                        {
                            dtOnceaWeek = onceaweek.CopyToDataTable();
                            var tempdata = dtCsv.AsEnumerable().Where(x => !dtOnceaWeek.AsEnumerable().Any(y => y.Field<string>("JANコード") == x.Field<string>("JANコード")));
                            dtData = tempdata.Any() ? tempdata.CopyToDataTable() : null;
                        }
                        else
                        {
                            dtOnceaWeek = null;
                            dtData = dtCsv.Copy();
                        }
                        //Retrieve Old data 
                        Qbei_OrderDataDelete(int.Parse(strSiteCd), dtID);
                    }
                }
                if (dtOnceaWeek != null)
                {
                    var exist = dtOnceaWeek.AsEnumerable().Where(x => dtOrder.AsEnumerable().Any(y => x.Field<string>("JANコード") == y.Field<string>("jancode")));
                    if (exist.Any())
                    {
                        foreach (DataRow dr in exist.CopyToDataTable().Rows)
                        {
                            /// <remark>
                            /// Insert Data into Qbei_ErrorLog.
                            /// </remark>
                            objEntity = new Qbei_Entity();
                            objEntity.siteID = int.Parse(strSiteCd);
                            objEntity.sitecode = strSiteCd;
                            objEntity.janCode = dr.Field<string>("JANコード");
                            objEntity.orderCode = dr.Field<string>("発注コード");
                            Qbei_ErrorInsert(objEntity.siteID, GetSiteName(strSiteCd), "Item doesn't Check!", objEntity.janCode, objEntity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strSiteCd);
                        }
                    }
                    var notexist = dtOnceaWeek.AsEnumerable().Where(x => !dtOrder.AsEnumerable().Any(y => x.Field<string>("JANコード") == y.Field<string>("jancode")));
                    if (notexist.Any())
                    {
                        foreach (DataRow dr in notexist.CopyToDataTable().Rows)
                        {
                            /// <remark>
                            /// Insert Data into Qbei_ErrorLog.
                            /// </remark>
                            objEntity = new Qbei_Entity();
                            objEntity.siteID = int.Parse(strSiteCd);
                            objEntity.sitecode = strSiteCd;
                            objEntity.janCode = dr.Field<string>("JANコード");
                            objEntity.orderCode = dr.Field<string>("発注コード");
                            Qbei_OrderDataInsert(objEntity);
                            Qbei_ErrorInsert(objEntity.siteID, GetSiteName(strSiteCd), "Item doesn't Check!", objEntity.janCode, objEntity.orderCode, 5, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), strSiteCd);
                        }
                    }
                }
                if (dtData != null)
                {
                    //<remark 27/12/2019 追加　Start>
                    //var empty = dtData.AsEnumerable().Where(r => (r.Field<string>("メーカー情報日") != null));
                    //DataTable a = empty.CopyToDataTable();
                    //dtData = empty.Any() ? empty.CopyToDataTable() : null;
                    //</remark End>

                    //<remark 1/23/2020 追加　Start>
                    string Nowdate = DateTime.Now.ToString("yyyy-MM-dd");
                    dtData.AsEnumerable().Where(r => (r.Field<string>("メーカー情報日") == null))
                       .Select(r => r["メーカー情報日"] = Nowdate).ToList();
                    dtData = dtData.AsEnumerable().OrderBy(x => x["メーカー情報日"]).ThenBy(x => x["JANコード"]).CopyToDataTable();
                    dtData.AsEnumerable().ToList().ForEach(r => r["発注コード"] = r.Field<string>("発注コード").Trim());
                    //</remark End>
                }
            }
            catch (Exception ex)
            {

                dtData = null;
            }
            finally
            {
                if (dtData == null)
                {
                    GetTotalCount(strSiteCd);
                    StopApplication(int.Parse(strSiteCd));
                }
            }

            return dtData;
        }

        /// <summary>
        ///  Insert to Order Date.
        /// </summary>
        /// <param name="entity">Insert to Common Variable.</param>
        public void Qbei_OrderDataInsert(Qbei_Entity entity)
        {
            ///<remark>
            ///Insert to Order Data at Qbei_OrderData Table.
            ///</remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_OrderDataInsert", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", entity.siteID);
            cmd.Parameters.AddWithValue("@jancode", entity.janCode);
            cmd.Parameters.AddWithValue("@ordercode", entity.orderCode);
            cmd.Parameters.AddWithValue("@sitecode", entity.sitecode);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Delete to Order Data.
        /// </summary>
        /// <param name="intSiteparam">Insert to site for Order Delete of Data.</param>
        /// <param name="dtCond">Insert to data at Table.</param>
        public void Qbei_OrderDataDelete(int intSiteparam, DataTable dtCond)
        {
            /// <remark>
            /// Delete to Order Data at Qbei_OrderData Table.
            /// </remark>
            string xml = string.Empty;

            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_OrderDelete", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", intSiteparam);
            if (dtCond.Rows.Count > 0)
            {
                xml = DataTableToXml(dtCond);
                cmd.Parameters.AddWithValue("@condition", xml);
            }
            else cmd.Parameters.AddWithValue("@condition", DBNull.Value);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// All GetSiteName.
        /// </summary>
        public string GetSiteName(string shopID)
        {
            switch (shopID)
            {
                case "011": return "マルイ";
                case "012": return "カワシマ";
                case "013": return "ミズタニ";
                case "014": return "イワイ";
                case "015": return "アキ";
                case "016": return "ライトウェイ";
                case "017": return "インターマックス";
                case "018": return "日直";
                case "019": return "深谷";
                case "020": return "ダイアテック";
                case "024": return "東";
                case "030": return "城東";
                case "031": return "アキボウ";
                case "034": return "シマノ";
                case "035": return "インターテック";
                case "036": return "PRインターナショナル";
                case "037": return "東京サンエス(株)";
                case "038": return "フタバ";
                case "046": return "トライスポーツ";
                case "053": return "中川商会";
                case "057": return "モトクロス";
                case "065": return "野口";
                case "084": return "NBS";
                case "087": return "ダートフリーク";
                //2017/12/06 Add
                case "139": return "ウエイブワン";
                //2017/12/06 Add
                case "143": return "ポディウム";
                //2018/06/12 Add
                case "059": return "(株)ジェイピースポーツグループ";
                case "916": return "(株)あさひ";
                case "914": return "（株）イノセントデザインワークス";
                //2019/07/24 Add
                case "124": return "ミズタニ";
                //2021/04/21 Add
                case "051": return "スタイルバイク";
                case "110": return "アサヒサイクル";
                //2021/06/15 Add
                case "58": return "リンエイ";
                //2021/09/14 Add
                case "145": return "Many'S";
                //2022/02/04 Add
                case "028": return "サイクルヨーロッパ";
                default: return "unknwon site";



            }

        }
        /// <summary>
        /// Download CSV of Site.
        /// </summary>
        /// <param name="path">Insert to File of path.</param>
        /// <param name="columns">Insert to colum name.</param>
        /// <returns>Check of Download CSV.</returns>
        public DataTable GetDatatableFromDownloadPath(string path, string[] columns)
        {
            ///<remark>
            ///Check to Data of Downbload CSV from Site.
            ///</remark>
            CachedCsvReader csv;
            DataTable dtResult = new DataTable(); ;
            string[] filelist = Directory.GetFiles(path);
            foreach (string file in filelist)
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".csv"))
                {
                    if (path.Contains("916_Download"))
                    {
                        csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding(932)), true, ',', '\0', '\0', '#', LumenWorks.Framework.IO.Csv.ValueTrimmingOptions.All);
                    }
                    else
                    {
                        csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding(932)), true);
                    }
                    //using (var csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding(932)), true, ',', '\0', '\0', '#', LumenWorks.Framework.IO.Csv.ValueTrimmingOptions.All))
                    //{
                    DataTable dtCsv = new DataTable();
                    dtCsv.Load(csv);
                    if (dtResult.Columns.Count <= 0)
                        dtResult = dtCsv.Clone();
                    if (checkCsvFormat(dtCsv, columns))
                    {
                        dtResult.Merge(dtCsv);
                    }
                    else
                    {
                        WritetoLog("CsvFile Format Wrong!");
                        return null;
                    }

                    //}
                }
                else
                    File.Move(file, @"C:\Qbei_Log\Trash\" + @"\" + Path.GetFileName(file));
                //File.Move(file, trashPath + @"\" + Path.GetFileName(file));
            }

            if (!dtResult.Equals(null))
                return dtResult;
            else return null;
        }

        /// <summary>
        /// Insert to Qbei Table.
        /// </summary>
        /// <param name="stockDate">Insert to stockdate of Qbei data.</param>
        /// <param name="qtyStatus">Insert to quantity of Qbei data.</param>
        /// <param name="site">Insert to siteID of  Qbei data.</param>
        /// <param name="janCode">Insert to janCode of  Qbei data.</param>
        /// <param name="partNo">Insert to partNo of  Qbei data.</param>
        /// <param name="makerDate">Insert to makerDate at  Qbei data.</param>
        /// <param name="reflectDate">Insert to reflectDate at  Qbei data.</param>
        public void Qbei_Insert(string stockDate, string qtyStatus, string site, string janCode, string partNo, string makerDate, string reflectDate)
        {
            ///<remark>
            ///Insert to Mall(Night) of Data at Qbei Table. 
            ///</remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_Insert", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@stockDate", stockDate);
            cmd.Parameters.AddWithValue("@quantity", qtyStatus);
            cmd.Parameters.AddWithValue("@site", site);
            cmd.Parameters.AddWithValue("@jancode", janCode);
            cmd.Parameters.AddWithValue("@partNo", partNo);
            cmd.Parameters.AddWithValue("@makerDate", makerDate);
            cmd.Parameters.AddWithValue("@reflectDate", reflectDate);
            cmd.Parameters.AddWithValue("@Updated_Date", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Insert to XML data at Qbei Table.
        /// </summary>
        /// <param name="dtCsv">Check to CSV DataTable.</param>
        /// <param name="dtItem">Insert to CSV of Data for DataTable </param>
        /// <param name="name">Store Prodducer Name</param>
        /// <param name="strRerun">Insert to "".</param>
        public void Qbei_Insert_XML(DataTable dtCsv, DataTable dtItem, string name, string strRerun = "")
        {
            ///<remark>
            ///Insert to Mall of XML Data at Qbei Table. 
            ///</remark>
            string xmlCsv = DataTableToXml(dtCsv);

            string xmlItem = DataTableToXml(dtItem);

            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand(name, sqlcon);
            cmd.CommandTimeout = 600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@xmlCsv", xmlCsv);
            cmd.Parameters.AddWithValue("@xmlItem", xmlItem);
            cmd.Parameters.AddWithValue("@param", strRerun);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        ///Insert to Qbei Error Table.
        /// </summary>
        /// <param name="site">Insert to site of Error Data.</param>
        /// <param name="sitename">Insert to sitename of Error Data.</param>
        /// <param name="description">Insert to description of Error Data.</param>
        /// <param name="janCode">Insert to janCode of Error Data.</param>
        /// <param name="orderCode">Insert to orderCode of Error Data.</param>
        /// <param name="errortype">Insert to errortype of Error Data.</param>
        /// <param name="Date">Insert to Date of Error Data.</param>
        /// <param name="sitecode">Insert to sitecode of Error Data.</param>
        public void Qbei_ErrorInsert(int site, string sitename, string description, string janCode, string orderCode, int errortype, string Date, string sitecode)
        {
            /// <remark>
            /// Insert to  Error of Data into Qbei_ErrorLog Table.
            /// </remark>
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
                SqlCommand cmd = new SqlCommand("Qbei_ErrorInsert", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@site", site);
                cmd.Parameters.AddWithValue("@sitename", sitename);
                cmd.Parameters.AddWithValue("@jancode", janCode);
                cmd.Parameters.AddWithValue("@OrderCode", orderCode);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@errortype", errortype);
                cmd.Parameters.AddWithValue("@Date", Date);
                cmd.Parameters.AddWithValue("@sitecode", sitecode);

                //  cmd.Parameters.AddWithValue("@Updated_Date", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
        }

        /// <summary>
        /// Delete to Qbei Error.
        /// </summary>
        /// <param name="site">Insert to site of Error Data.</param>
        public void Qbei_ErrorDelete(int site)
        {
            /// <remark>
            /// Delete to  Error of Data into Qbei_ErrorLog Table.
            /// </remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_DeleteErrorLog", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", site);
            cmd.Connection.Open();
            cmd.CommandTimeout = 0;
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Qbei Backup.
        /// </summary>
        /// <param name="site">Insert to site for Backup Data.</param>
        public void Qbei_Delete(int site)
        {
            /// <remark>
            /// Insert to Data into Qbei Backup Table. 
            /// Delete to Data into Qbei Backup Table.
            /// </remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_DeleteSiteData", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", site);
            cmd.CommandTimeout = 600;
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }

        /// <summary>
        /// Selenium Driver.
        /// </summary>
        /// <param name="driver">Insert to Use Driver.</param>
        /// <param name="value">Insert to Selenium of optionName.</param>
        /// <param name="sleeptime">Insert to wait of time.</param>
        /// <returns>Use to Chrome and Mozilla.</returns>
        protected static IWebElement FindElement(IWebDriver driver, By value, int sleeptime)
        {
            ///<remark>
            ///Check to Selenium Driver for Site.
            ///Wait Time of Site.
            ///</remark>
            bool found = false;
            int count = 0;
            do
            {
                try
                {
                    driver.FindElement(value);
                    found = true;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Unable to locate element"))
                    {
                        count++;
                        Thread.Sleep(sleeptime);
                        found = false;
                    }
                    else throw e;
                }
            } while (!found && count < 5);
            return driver.FindElement(value);
        }

        /// <summary>
        /// CheckCsvFormat 
        /// </summary>
        /// <param name="dtCsv">Check to CSV DataTable.</param>
        /// <param name="columns"></param>
        /// <returns>Check of CSV Format.</returns>
        /// <remark>
        /// Check to dtCsv of Columns Name. 
        /// </remark>
        private static bool checkCsvFormat(DataTable dtCsv, string[] columns)
        {

            foreach (string colName in columns)
            {
                DataColumnCollection cols = dtCsv.Columns;
                if (!cols.Contains(colName))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Create of File Path.
        /// </summary>
        /// <param name="path">Insert to CreatFIle of path.</param>
        private static void CreateFilePath(string path)
        {
            if (!File.Exists(path))
                File.Create(path);
        }

        /// <summary>
        /// Create of Directory Path.
        /// </summary>
        /// <param name="path">Insert to CreatDirectory of path.</param>
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }


        /// <summary>
        /// DateTime at Now.
        /// </summary>
        /// <returns>Get to Currentdate.</returns>
        public string getCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Chabge of Flag.
        /// </summary>
        /// <param name="qe">Insert to Common Variable.</param>
        public void ChangeFlag(Qbeisetting_Entity qe)
        {
            ///<remark>
            ///Change to Flage of Site.
            ///Update to Start Time and End Time of Site.
            ///</remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Console_FlagChange", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", qe.site);
            cmd.Parameters.AddWithValue("@flag", qe.flag);

            if (qe.starttime == null)
                cmd.Parameters.AddWithValue("@Start_time", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Start_time", qe.starttime);
            if (qe.endtime == null)
                cmd.Parameters.AddWithValue("@End_time", DBNull.Value);
            else cmd.Parameters.AddWithValue("@End_time", qe.endtime);
            // cmd.Parameters.AddWithValue("@End_time", qe.endtime);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }

        /// <summary>
        /// Select Flag.
        /// </summary>
        /// <param name="site">Insert to site of slectflag.</param>
        /// <returns>Select to flage of string.</returns>
        public DataTable SelectFlag(int site)
        {
            ///<remark>
            ///Select to Flag at Site Setting Table.
            ///</remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("SelectFlag", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", site);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                cmd.Connection.Open();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            { return new DataTable(); }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Delete Data.
        /// </summary>
        /// <param name="site">Insert to site of delete data.</param>
        /// <returns>Delete data from site.</returns>
        public DataTable deleteData(int site)
        {
            ///<remark>
            ///Delete to Data at Qbel Table.
            ///</remark>
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("DeleteData", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", site);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                cmd.Connection.Open();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            { return new DataTable(); }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Kill Process
        /// </summary>
        public void KillProcess()
        {
            ///<remark>
            ///Exit of site at Time Schedule from Export CVS. 
            ///</remark>
            foreach (var process in Process.GetProcessesByName("Qbei_Agencies"))
            {
                process.Kill();
            }
        }

        /// <summary>
        /// ClearMemory.
        /// </summary>
        public void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Qbei Table.
        /// </summary>
        /// <param name="entity">Insert to Common Variable.</param>
        public void Qbei_Inserts(Qbei_Entity entity)
        {
            ///<remark>
            ///Insert to Mall(AM.PM) of Data at Qbei Table. 
            ///</remark>
            ClearMemory();
            SqlCommand cmd;
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //Delete Order Data before inserting Qbei Table
            cmd = new SqlCommand("Qbei_Rerun_ODelete", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@site", entity.siteID);
            cmd.Parameters.AddWithValue("@jancode", entity.janCode);
            cmd.Parameters.AddWithValue("@ordercode", entity.orderCode);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

            cmd = new SqlCommand("Qbei_Insert", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@stockDate", entity.stockDate);
            cmd.Parameters.AddWithValue("@price", entity.price);
            cmd.Parameters.AddWithValue("@orderCode", entity.orderCode);
            cmd.Parameters.AddWithValue("@purchaseURL", entity.purchaseURL);
            cmd.Parameters.AddWithValue("@quantity", entity.qtyStatus);
            cmd.Parameters.AddWithValue("@site", entity.sitecode);
            cmd.Parameters.AddWithValue("@jancode", entity.janCode);
            cmd.Parameters.AddWithValue("@partNo", entity.partNo);
            cmd.Parameters.AddWithValue("@makerDate", entity.makerDate);
            cmd.Parameters.AddWithValue("@reflectDate", entity.reflectDate);
            cmd.Parameters.AddWithValue("@siteID", entity.siteID);

            //<remark 2020/11/05>
            cmd.Parameters.AddWithValue("@True_StockDate", entity.True_StockDate);
            cmd.Parameters.AddWithValue("@True_Quantity", entity.True_Quantity);
            //</remark 202/11/05>

            cmd.Parameters.AddWithValue("@Updated_Date", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        public bool IsNumber(string num)
        {
            try
            {
                Convert.ToInt32(num);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsGood(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) > 10)
                    return true;
                return false;
            }
            return false;
        }

        //<remark Quantityの　追加ロジック　2020/04/07 Start>
        public bool IsGood_38(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) >= 10)
                    return true;
                return false;
            }
            return false;
        }
        public bool IsSmall_38(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) >= 5 && Convert.ToInt32(value) < 10)
                    return true;
                return false;
            }
            return false;
        }
        public bool IsEmpty_38(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) < 5)
                    return true;
                return false;
            }
            return false;
        }
        //</remark 2020/04/07 End>
        public bool IsGood1(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) > 9)
                    return true;
                return false;
            }
            return false;
        }
        public bool IsEmpty(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) == 0)
                    return true;
                return false;
            }
            return false;
        }

        public bool IsLessthanzero(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) <= 0)
                    return true;
                return false;
            }
            return false;
        }
        public bool IsSmall(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) <= 10)
                    return true;
                return false;
            }
            return false;
        }
        public bool IsSmall1(string value)
        {
            if (IsNumber(value))
            {
                if (Convert.ToInt32(value) > 0)
                    return true;
                return false;
            }
            return false;
        }

        /// <summary>
        /// Move to Trash.
        /// </summary>
        /// <param name="shopID">Insert to shopID for Move CSV.</param>
        public void MoveToTrash(string shopID)
        {
            ///<remark>
            ///Move to Download CSV.
            ///</remark>
            string path = string.Empty;
            switch (shopID)
            {
                case "014": path = @"C:\Qbei_Log\014_Download\"; break;
                case "015": path = @"C:\Qbei_Log\015_Download\"; break;
                case "035": path = @"C:\Qbei_Log\035_Download\"; break;
                case "916": path = @"C:\Qbei_Log\916_Download"; break;
                case "037": path = @"C:\Qbei_Log\037_Download"; break;
            }

            string[] filelist = Directory.GetFiles(path);
            foreach (string file in filelist)
            {
                string ext = Path.GetExtension(file);
                File.Move(file, @"C:\Qbei_Log\Trash\" + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(file));
                //break;
            }
        }

        /// <summary>
        /// Replace of Order Code.
        /// </summary>
        /// <param name="orderCode">Insert to ordercode of Data.</param>
        /// <param name="str">Remove to string arrays.</param>
        /// <returns>Remove to string at ordercode.</returns>
        public string ReplaceOrderCode(string orderCode, string[] str)
        {
            foreach (string str1 in str)
            {
                orderCode = orderCode.Replace(str1, string.Empty);
            }
            return orderCode;
        }

        /// <summary>
        /// GetElement from Html of Element.
        /// </summary>
        /// <param name="tagName">Insert to Hlml Element of tagName.</param>
        /// <param name="value">Insert to Hlml Element of value.</param>
        /// <param name="attrName">Insert to Hlml Element of attrName.</param>
        /// <param name="webBrowser1">Insert to Hlml Element of webBrowser.</param>
        /// <returns>Use to Html Element for webBrowser at windows form.</returns>
        public HtmlElement GetElement(string tagName, string value, string attrName, WebBrowser webBrowser1)
        {
            HtmlElementCollection col = webBrowser1.Document.GetElementsByTagName(tagName);
            HtmlElement element;
            foreach (HtmlElement item in col)
            {
                if (item.GetAttribute(attrName).Equals(value))
                {
                    element = item;
                    return element;
                }
            }
            return null;
        }

        public void WriteLog(string strLog, string siteID)
        {
            string logFilePath = "C:\\Qbei_Log\\Logfile\\" + "Log" + siteID + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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

        public void WriteLog(Exception ex, string siteID, string strLog1 = "", string strlog2 = "")
        {
            string logFormat = "{0} {1} {2} {3} {4}" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string logFilePath = "C:\\Qbei_Log\\Logfile\\" + "Log" + siteID + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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
            log.WriteLine(string.Format(logFormat, ex.Message, ex.StackTrace, strLog1, strlog2, siteID));
            log.Close();
        }

        /// <summary>
        /// Delete Qbei,Qbei_ErrorLog,Qbei_OrderData when data exist in Qbei_OrderData.
        /// Insert Qbei_OrderData when data does not exist in Qbei_OrderData
        /// </summary>
        /// <param name="entity">Insert to Common Variable.</param>
        public void RerunOrder(Qbei_Entity entity)
        {
            try
            {
                Connection conn = new Connection();
                SqlConnection sqlcon = conn.GetConnection();
                SqlCommand cmd = new SqlCommand("Qbei_Rerun_Insert", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@siteId", entity.siteID);
                cmd.Parameters.AddWithValue("@siteNm", GetSiteName(entity.sitecode));
                cmd.Parameters.AddWithValue("@JanCd", entity.janCode);
                cmd.Parameters.AddWithValue("@OrderCd", entity.orderCode);
                cmd.Parameters.AddWithValue("@quantity", entity.qtyStatus);
                cmd.Parameters.AddWithValue("@stockDate", entity.stockDate);
                cmd.Parameters.AddWithValue("@price", entity.price);
                cmd.Parameters.AddWithValue("@purchaserURL", entity.purchaseURL);
                cmd.Parameters.AddWithValue("@partNo", entity.partNo);
                cmd.Parameters.AddWithValue("@makerDate", entity.makerDate);
                cmd.Parameters.AddWithValue("@reflectDate", entity.reflectDate);
                cmd.Parameters.AddWithValue("@siteCd", entity.sitecode);
                //<remark 2020/11/05>
                cmd.Parameters.AddWithValue("@True_StockDate", entity.True_StockDate);
                cmd.Parameters.AddWithValue("@True_Quantity", entity.True_Quantity);
                //</remark 202/11/05>
                cmd.CommandTimeout = 600;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, entity.sitecode);
            }
        }
        /// <summary>
        /// Delete Qbei and Qbei_ErrorLog when rerun.
        /// </summary>
        /// <param name="intSite">Insert to site for Rerun Delete.</param>
        /// <param name="entity">Insert to Common Variable.</param>
        public void DeleteRerunData(int intSite, Qbei_Entity entity)
        {
            try
            {
                Connection conn = new Connection();
                SqlConnection sqlconn = conn.GetConnection();
                SqlCommand cmd = new SqlCommand("Qbei_Rerun_ODelete", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@site", intSite);
                cmd.Parameters.AddWithValue("@jancode", entity.janCode);
                cmd.Parameters.AddWithValue("@ordercode", entity.orderCode);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        ///Read to CSV File. 
        /// </summary>
        /// <param name="strShopID">Insert to shopID from CSV.</param>
        /// <returns>Read from CSV for data.</returns>
        public DataTable ReadCsv(string strShopID)
        {
            try
            {
                DataTable dtCsv = new DataTable();
                DataTable dtTemp = new DataTable();
                string strext = ".csv";
                string[] columns = { "代理店ID", "JANコード", "在庫情報", "入荷予定", "自社品番", "メーカー情報日", "最終反映日", "ブランドコード", "ステータス変更日" };
                string[] filelist = Directory.GetFiles(@"C:\Qbei_Log\Csv");
                foreach (string file in filelist)
                {
                    string ext = Path.GetExtension(file);
                    if (ext.Equals(strext))
                    {
                        using (var csv = new CachedCsvReader(new StreamReader(file, Encoding.GetEncoding(932)), true))
                        {
                            dtTemp = new DataTable();
                            dtTemp.Load(csv);
                            if (!checkCsvFormat(dtTemp, columns))
                            {
                                WritetoLog("Wrong File");
                                dtCsv = new DataTable();
                            }
                            else
                            {
                                dtCsv.Merge(dtTemp);
                            }
                        }
                    }
                }
                if (dtCsv == null)
                    return null;
                else
                {
                    if (string.IsNullOrEmpty(strShopID))
                        return dtCsv;
                    else
                    {
                        var a = dtCsv.Select("代理店ID='" + strShopID + "'");
                        dtCsv = a.Any() ? a.CopyToDataTable() : null;
                        return dtCsv;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Read Data from CSV.
        /// remove  once a week,order code contains null or - or japan text data from CSV 
        /// </summary>
        /// <returns>except remove data</returns>
        public DataTable GetRerunData(string strShopId)
        {
            DataColumn dc = new DataColumn("SiteName");
            string xml;
            Connection con;
            SqlConnection sqlcon;
            SqlCommand cmd;
            DataTable dtCsv = new DataTable();
            DataTable dtNull = new DataTable();
            DataTable dtNotInteger = new DataTable();
            DataTable dtOrder = new DataTable();
            DataTable dtBlankUrl = new DataTable();
            string strDescription = string.Empty;
            int siteID = int.Parse(strShopId);

            try
            {
                dtCsv = ReadCsv(strShopId);

                if (dtCsv == null)
                    return null;
                else
                {
                    ddr = dtCsv.Rows.Count;

                    if (strShopId.Equals("053"))
                    {
                        dtCsv = GetBrandCode(dtCsv);
                    }
                    if (dtCsv != null)
                    {
                        //Set Site Name
                        dc.DefaultValue = GetSiteName(strShopId);

                        //sort
                        dtCsv = dtCsv.AsEnumerable().OrderBy(x => x.Field<string>("メーカー情報日")).ThenBy(x => x.Field<string>("JANコード")).CopyToDataTable();

                        //Remove Blank Url from Site 36
                        //<remark Close Logic 2021/10/29 Start>
                        //if (strShopId.Equals("036"))
                        //{
                        //    var blankUrl = dtCsv.Select("purchaserURL =' ' OR purchaserURL = '' OR purchaserURL is NULL");
                        //    if (blankUrl.Any())
                        //    {
                        //        dtBlankUrl = blankUrl.CopyToDataTable();
                        //        var NonBlankUrl = dtCsv.AsEnumerable().Where(x => !dtBlankUrl.AsEnumerable().Any(y => x.Field<string>("JANコード") == y.Field<string>("JANコード") && x.Field<string>("発注コード") == y.Field<string>("発注コード")));
                        //        dtCsv = NonBlankUrl.Any() ? NonBlankUrl.CopyToDataTable() : null;
                        //        strDescription = "Item doesn't Run!";
                        //        dtBlankUrl.Columns.Add(dc);
                        //        xml = DataTableToXml(dtBlankUrl);
                        //        con = new Connection();
                        //        sqlcon = con.GetConnection();
                        //        cmd = new SqlCommand("Qbei_Rerun_ErrorInsert", sqlcon);
                        //        cmd.CommandType = CommandType.StoredProcedure;
                        //        cmd.Parameters.AddWithValue("@condition", xml);
                        //        cmd.Parameters.AddWithValue("@siteCd", strShopId);
                        //        cmd.Parameters.AddWithValue("@description", strDescription);
                        //        cmd.Parameters.AddWithValue("@ErrorType", 6);
                        //        cmd.CommandTimeout = 600;
                        //        cmd.Connection.Open();
                        //        cmd.ExecuteNonQuery();
                        //        cmd.Connection.Close();
                        //    }
                        //    //return dtCsv;
                        //}
                        //</remark 2021/10/29 End>
                        var temp = dtCsv.Select("発注コード=' ' OR 発注コード = '' OR 発注コード is NULL OR 発注コード= '-' OR 発注コード= '--'");
                        //Insert Null Order Code
                        if (temp.Any())
                        {
                            dtNull = temp.CopyToDataTable();
                            var data = dtCsv.AsEnumerable().Where(r => !dtNull.AsEnumerable().Any(y => (r.Field<string>("JANコード") == y.Field<string>("JANコード") && r.Field<string>("発注コード") == y.Field<string>("発注コード"))));
                            if (data.Any())
                            {
                                dtCsv = data.CopyToDataTable();
                            }
                            else
                                dtCsv = null;

                            strDescription = "Order Code Not Found";
                            dc = new DataColumn("SiteName");
                            dc.DefaultValue = GetSiteName(strShopId);
                            dtNull.Columns.Add(dc);
                            xml = DataTableToXml(dtNull);
                            con = new Connection();
                            sqlcon = con.GetConnection();
                            cmd = new SqlCommand("Qbei_Rerun_ErrorInsert", sqlcon);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@siteCd", strShopId);
                            cmd.Parameters.AddWithValue("@condition", xml);
                            cmd.Parameters.AddWithValue("@description", strDescription);
                            cmd.Parameters.AddWithValue("@ErrorType", 3);
                            cmd.CommandTimeout = 600;
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }

                        //<remark Add Logic for Siteid-110 2021/04/09 Start>
                        if (strShopId.Equals("110"))
                        {
                            int check_ci_count = dtCsv.Select("自社品番 like 'ci%'").Count();//<remark Add Logic for Siteid-110 2022/03/07 />
                            if (check_ci_count > 0)//<remark Add Logic for Siteid-110 2022/03/07 />
                            {
                                DataTable dt_ci = dtCsv.Select("自社品番 like 'ci%'").CopyToDataTable();
                                xml = DataTableToXml(dt_ci);
                                //xml = RemoveInvalidXmlChars(xml);
                                con = new Connection();
                                sqlcon = con.GetConnection();
                                cmd = new SqlCommand("Qbei_ErrorLog_3_InsertXml", sqlcon);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@xml", xml);
                                cmd.Parameters.AddWithValue("@sitename", GetSiteName(strShopId));
                                cmd.Parameters.AddWithValue("@Description", "ci-Item");
                                cmd.Parameters.AddWithValue("@ErrorType", 6);
                                cmd.Parameters.AddWithValue("@SiteCode", strShopId);
                                cmd.CommandTimeout = 600;
                                cmd.Connection.Open();
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }

                            int check_count = dtCsv.Select("自社品番 not like 'ci-%'").Count();//<remark Add Logic for Siteid-110 2022/03/07 />
                            if (check_count > 0)//<remark Add Logic for Siteid-110 2022/03/07 />
                            {
                                dtCsv = dtCsv.Select("自社品番 not like 'ci-%'").CopyToDataTable();
                            }
                        }
                        //</remark 2021/04/09 End>

                        if (dtCsv != null)
                        {
                            //Order Code Trim
                            dtCsv.AsEnumerable().ToList().ForEach(y => y["発注コード"] = y.Field<string>("発注コード").Trim());
                            //Insert Japananese Text Order Code
                            var notintegerdata = dtCsv.AsEnumerable().Where(r => (r.Field<string>("発注コード").Contains("在庫") || r.Field<string>("発注コード").Contains("発注禁止") || r.Field<string>("発注コード").Contains("東特価") || r.Field<string>("発注コード").Contains("バラ注文") || r.Field<string>("発注コード").Contains("（カワシマ）") || r.Field<string>("発注コード").Contains("/") || r.Field<string>("発注コード").Contains("データ登録")));
                            if (notintegerdata.Any())
                            {
                                dc = new DataColumn("SiteName");
                                dc.DefaultValue = GetSiteName(strShopId);
                                dtNotInteger = notintegerdata.CopyToDataTable();
                                var integerdata = dtCsv.AsEnumerable().Where(r => !dtNotInteger.AsEnumerable().Any(z => (r.Field<string>("JANコード").Equals(z.Field<string>("JANコード")) && r.Field<string>("発注コード").Equals(z.Field<string>("発注コード")))));
                                dtCsv = integerdata.Any() ? integerdata.CopyToDataTable() : null;
                                strDescription = "Item doesn't Exists!";
                                dtNotInteger.Columns.Add(dc);
                                xml = DataTableToXml(dtNotInteger);
                                con = new Connection();
                                sqlcon = con.GetConnection();
                                cmd = new SqlCommand("Qbei_Rerun_ErrorInsert", sqlcon);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@siteCd", strShopId);
                                cmd.Parameters.AddWithValue("@condition", xml);
                                cmd.Parameters.AddWithValue("@description", strDescription);
                                cmd.Parameters.AddWithValue("@ErrorType", 2);
                                cmd.CommandTimeout = 600;
                                cmd.Connection.Open();
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                        }
                    }
                    //return dtCsv;
                }
            }
            catch (Exception ex)
            {
                WritetoLog(ex.Message);
                dtCsv = null;
            }
            finally
            {
                if (dtCsv == null)
                {
                    GetTotalCount(strShopId);
                    StopApplication(siteID);
                }
            }

            return dtCsv;
        }
    }
}