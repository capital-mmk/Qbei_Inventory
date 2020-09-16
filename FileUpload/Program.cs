using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace FileUpload
{
    /// <summary>
    /// File Upload at FTP.
    /// </summary>
    class Program
    {
        static CommonFunction fun = new CommonFunction();
        /// <summary>
        /// UploadFtpFile of path and Clear at path of folder.
        /// </summary>
        static void Main(string[] args)
        {
            //CreateDirectoryOnFTP("ftp://27.134.252.242/Local_User/Qbei_Inventory/", /*user*/"Qbei_Agencies", /*pw*/"=3gYe0+Ycu", "NewDirectory");
            UploadFtpFile(System.Configuration.ConfigurationManager.AppSettings["LocalFodlerpath"]);
            clearFolder("LocalFodlerpath");
        }

        //static public string CreateDirectoryOnFTP(String inFTPServerAndPath, String inUsername, String inPassword)
        //{
        //    // Step 1 - Open a request using the full URI, ftp://ftp.server.tld/path/file.ext
        //    //FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(inFTPServerAndPath + "/" + DateTime.Now.ToString("yyyy-MM-dd"));

        //    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(inFTPServerAndPath);
        //    // Step 2 - Configure the connection request
        //    request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        //    request.Credentials = new NetworkCredential(inUsername, inPassword);
        //    request.UsePassive = true;
        //    request.UseBinary = true;
        //    request.KeepAlive = false;

        //    request.Method = WebRequestMethods.Ftp.MakeDirectory;

        //     //Step 3 - Call GetResponse() method to actually attempt to create the directory
        //    FtpWebResponse makeDirectoryResponse = (FtpWebResponse)request.GetResponse();
        //    string folderpath= Convert.ToString(makeDirectoryResponse.ResponseUri);
        //    return folderpath;

        //    //using (var resp = (FtpWebResponse)request.GetResponse())
        //    //{
        //    //    Console.WriteLine(resp.StatusCode);
        //    //}
        //}

            /// <summary>
            /// Clear of Folder at ExportCSV.
            /// </summary>
            /// <param name="FolderName">Input Flodername of ExportCSV Name.</param>
            /// <returns>Delete to Floder at Qbei_Log.</returns>
        static public bool  clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo("C:\\Qbei_Log\\ExportCSV\\");

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
            return true;
        }

        /// <summary>
        /// FIleUpload to FTP .
        /// </summary>
        /// <param name="strfilename">Input of FTP CSV file Name.</param>
       public static void UploadFtpFile(string strfilename)
       {

        //FTP Server URL.

       string ftp = System.Configuration.ConfigurationManager.AppSettings["FTPUrl"];

       //FTP Folder name. Leave blank if you want to upload to root folder.
       //string ftpFolder = CreateDirectoryOnFTP("ftp://133.242.49.19/maker_stock/", /*user*/"ftp-sks", /*pw*/"lMnx5L2S");
       //string ftpFolder = " ";
       
       
       //string ftpFolder = System.Configuration.ConfigurationManager.AppSettings["FTPFolderName"];

       byte[] fileBytes = null;

       string [] fileName; 

        //Read the FileName and convert it to Byte array.

       
       fileName = Directory.GetFiles(strfilename);
       foreach (string filelist in fileName)
       {
           using (StreamReader fileStream = new StreamReader(filelist,Encoding.GetEncoding(932)))
           {
              
               fileBytes = Encoding.GetEncoding(932).GetBytes(fileStream.ReadToEnd());
               fileStream.Close();

           }

       try
       {

           //Create FTP Request.

           //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://133.242.49.19/maker_stock/" + Path.GetFileName(filelist));
           FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://27.133.149.167/maker_stock/" + Path.GetFileName(filelist));     

           request.Method = WebRequestMethods.Ftp.UploadFile;

           request.Timeout = 3000000;

           //Enter FTP Server credentials.

           request.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FTPUserName"], System.Configuration.ConfigurationManager.AppSettings["FTPPassWord"]);

           request.ContentLength = fileBytes.Length;

           request.UsePassive = true;

           request.UseBinary = true;

           request.ServicePoint.ConnectionLimit = fileBytes.Length;

           request.EnableSsl = false;


           using (Stream requestStream = request.GetRequestStream())
           {
              
               requestStream.Write(fileBytes, 0, fileBytes.Length);
              
               requestStream.Close();

           }

           FtpWebResponse response = (FtpWebResponse)request.GetResponse();

           response.Close();
       }
       
       catch (WebException ex)
       {
                    fun.WriteLog(ex.Message, "File Upload Console");
                    throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
       }
       }
       }

 
    }

}
