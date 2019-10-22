using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Ionic.Zip;
using Common;

namespace Import_Data_From_FTP
{

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remark>
    /// Create to string of parameter.
    /// </remark>
    class Program
    {
        static string LocalFilePath = ConfigurationManager.AppSettings["LocalFilePath"].ToString();
        static string FTP_Host = ConfigurationManager.AppSettings["FTP_Host"].ToString();
        static string Username = ConfigurationManager.AppSettings["Username"].ToString();
        static string Password = ConfigurationManager.AppSettings["Password"].ToString();
        static string strFileNm = DateTime.Now.ToString("yyyyMMdd") + "_maker_status.zip";
        //2018-04-18 Start
        static string strFileNm_Am = DateTime.Now.ToString("yyyyMMdd") + "_maker_status_am.zip";
        static string strFileNm_Pm = DateTime.Now.ToString("yyyyMMdd") + "_maker_status_pm.zip";
        static string[] strFile;
        static CommonFunction fun = new CommonFunction();
        //2018-04-18 End
        static FtpWebRequest reqFTP;
        static void Main(string[] args)
        {
            //2018-04-18 Start
            strFile = new string[] { strFileNm_Am, strFileNm_Pm, strFileNm };
            //2018-04-18 End
            Download();
        }

        /// <summary>
        /// FTP
        /// </summary>
        /// <remark>
        /// Download and Import Data From FTP.
        /// </remark>
        static void Download()
        {
            try
            {


                string strName;
                ZipFile objZip;
                FtpWebResponse response;
                foreach (string strNm in strFile)
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(FTP_Host + strNm));
                    reqFTP.Credentials = new NetworkCredential(Username, Password);
                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                    reqFTP.UseBinary = true;

                    try
                    {
                        response = (FtpWebResponse)reqFTP.GetResponse();

                        if ((response.LastModified >= DateTime.Now.AddHours(-1)) && (response.LastModified <= DateTime.Now))
                        {
                            fun.WriteLog("Csv file exist             ------", "Import Data From FTP");
                            Directory.GetFiles(LocalFilePath).ToList().ForEach(File.Delete);
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(FTP_Host + strNm));
                            reqFTP.Credentials = new NetworkCredential(Username, Password);
                            reqFTP.KeepAlive = false;
                            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                            reqFTP.UseBinary = true;
                            response = (FtpWebResponse)reqFTP.GetResponse();
                            Stream responseStream = response.GetResponseStream();
                            FileStream writeStream = new FileStream(LocalFilePath + strNm, FileMode.Create);
                            fun.WriteLog("DownLoad success             ------", "Import Data From FTP");
                            int Length = 2048;
                            Byte[] buffer = new Byte[Length];
                            int bytesRead = responseStream.Read(buffer, 0, Length);
                            while (bytesRead > 0)
                            {
                                writeStream.Write(buffer, 0, bytesRead);
                                bytesRead = responseStream.Read(buffer, 0, Length);
                            }
                            writeStream.Close();
                            response.Close();
                            if (File.Exists(LocalFilePath + strNm))
                            {
                                objZip = new ZipFile(LocalFilePath + strNm, UTF8Encoding.UTF8);
                                objZip.ExtractAll(LocalFilePath, ExtractExistingFileAction.OverwriteSilently);
                                objZip.Dispose();
                                File.Delete(LocalFilePath + strNm);
                                strName = strNm.Substring(strNm.IndexOf('_') + 1).Replace("zip", "csv");
                                File.SetLastWriteTime(LocalFilePath + strName, File.GetLastAccessTime(LocalFilePath + strName));
                            }

                        }
                        else
                        {
                            fun.WriteLog("Csv file does not exist             ------", "Import Data From FTP");
                        }
                    }
                    catch (WebException ex)
                    {
                        fun.WriteLog(ex.Message, "Import Data From FTP");
                        FtpWebResponse ftpresponse = (FtpWebResponse)ex.Response;
                        if (ftpresponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            //Continue Loop
                            continue;
                        }
                    }
                }
                return;
                //2018-04-18 End
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex.Message, "Import Data From FTP");
                Console.WriteLine("Download Fail");
            }
        }
    }
}
