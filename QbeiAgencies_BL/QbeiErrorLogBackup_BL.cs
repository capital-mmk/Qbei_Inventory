using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Configuration;

namespace QbeiAgencies_BL
{
    public class QbeiErrorLogBackup_BL
    {

        QbeiErrorLogBackup_DL dl = new QbeiErrorLogBackup_DL();
        QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
        //qbeierrorlog
        public DataTable Qbei_ErrorListSelect(ErrorType_Entity qer)
        {
           
            return qudl.GetDataFromError_Log(qer);
        }
        public DataTable Qbei_ErrorBackUpEqualSelect(ErrorType_Entity qer)
        {

            return qudl.GetDataFromErrorLogBackup(qer);
        }
        public DataTable Qbei_ErrorList(ErrorType_Entity qer)
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.GetDataFromError(qer);
        }
        public DataTable ErrorLogBackup_SelectAll(ErrorlogBackup_Entity ebe)
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.ErrorlogBackup_SelectAll(ebe);
        }
        public DataTable GetDate(Qbei_ErrorList qer)
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.GetDate(qer);
        }

        public DataTable BindError()
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.BindError();
        }
        public DataTable QbeiErrorLogBackup_DateSearch(string date)
        {

            return qudl.GetDataFromError_LogBackupDateSearch(date);
        }
        public DataTable BindSiteName()
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.BindSiteName();
        }
        public DataTable GetErrorSiteList(Qbei_ErrorList qer)
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.GetErrorSiteList(qer);
        }
        public DataTable FindError(int sitename)
        {
            QbeiErrorLogBackup_DL qudl = new QbeiErrorLogBackup_DL();
            return qudl.FindError(sitename);
        }


    }
}