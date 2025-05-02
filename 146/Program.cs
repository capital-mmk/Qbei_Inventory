using System;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.Chrome;
using Common;
using System.IO;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace _146
{
    class Program
    {
        Qbeisetting_BL blQbei = new Qbeisetting_BL();
        Qbei_Entity entity = new Qbei_Entity();
        DataTable dt146 = new DataTable();
        public static CommonFunction fun = new CommonFunction();
        DataTable dtGroupData = new DataTable();
        static string strParam = string.Empty;
        public static string st = string.Empty;

        static void Main(string[] args)
        {
            testFlag();
        }

        public static void testFlag()
        {
            try
            {
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                DataTable dtSetting = new DataTable();

                int intFlag;
                qe.starttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.site = 146;
                qe.flag = 1;
                dtSetting = fun.SelectFlag(146);
                intFlag = int.Parse(dtSetting.Rows[0]["FlagIsFinished"].ToString());

                if (intFlag == 0)
                {
                    fun.ChangeFlag(qe);
                    startRun();
                }

                else if (intFlag == 1)
                {
                    fun.deleteData(146);
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
                fun.WriteLog(ex, "146-");
                Environment.Exit(0);
            }
        }

        private static void startRun()
        {
            try
            {
                fun.setURL("146");
                fun.MoveToTrash("146");
                fun.CreateFileAndFolder();
                fun.Qbei_Delete(146);
                fun.Qbei_ErrorDelete(146);
                ReadData();
            }
            catch (Exception ex)
            {
                fun.WriteLog(ex, "019-");
                Environment.Exit(0);
            }
        }

        public static void ReadData()
        {
            
                DataTable dt = new DataTable();
                Qbeisetting_BL qubl = new Qbeisetting_BL();
                Qbeisetting_Entity qe = new Qbeisetting_Entity();
                Qbei_Entity entity = new Qbei_Entity();


                qe.SiteID = 19;
                dt = qubl.Qbei_Setting_Select(qe);
                
                
                int counter = 0;
            label:
                if (counter < 10)
                {
                    string[] filelist = Directory.GetFiles(@"C:\Qbei_Log\146_Download\");
                    foreach (string file in filelist)
                    {
                        string ext = Path.GetFileName(file);
                        goto label1;
                    }
                    Thread.Sleep(8000);
                    counter++;
                    goto label;
                }

            label1:
                fun.WriteLog("File Already Have------", "019-");

                string[] flist = Directory.GetFiles(@"C:\Qbei_Log\146_Download\");
                String filename = flist[0].ToString();
                string csvPath = filename;

                DataTable dt146 = fun.GetDatatable("146");
                fun.GetTotalCount("146");
                int Count = dt146.Rows.Count;

                if (dt146 == null)
                {
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 2;
                qe.site = 146;
                fun.ChangeFlag(qe);
                }
             
           
                DataTable dtItem = XlsxToDataTable(csvPath);
                int count = dtItem.Rows.Count;

                fun.WriteLog("Download success match with datatable------", "146-");
                fun.Qbei_Insert_XML(dt146, dtItem, "Qbei_Insert_Xml_146");
                fun.WriteLog("Insert data to db success------", "146-");
                qe.starttime = string.Empty;
                qe.endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                qe.flag = 2;
                qe.site = 146;
                fun.ChangeFlag(qe);
            
        }
        static DataTable XlsxToDataTable(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Important for EPPlus 5+     
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet        
                var dt = new DataTable();
                // Assume the first row has column names        
                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                { dt.Columns.Add(firstRowCell.Text); }

                // Start loading data from row 2        
                for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                    DataRow row = dt.NewRow();
                    int i = 0;
                    foreach (var cell in wsRow)
                    {
                        row[i++] = cell.Text;
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }
        }


    }
}
