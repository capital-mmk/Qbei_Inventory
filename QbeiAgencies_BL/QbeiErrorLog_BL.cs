using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Data;

namespace QbeiAgencies_BL
{
    public class QbeiErrorLog_BL
    {
        QbeiErrorLog_DL qudl = new QbeiErrorLog_DL();
        public DataTable Qbei_ErrorListSelect(Qbei_ErrorList qer)
        {
            
            return qudl.GetDataFromError_Log(qer);
        }
        public DataTable Qbei_ErrorListDateSearch(string date)
        {

            return qudl.GetDataFromError_LogDateSearch(date);
        }
        public DataTable Qbei_ErrorListEqualSelect(ErrorType_Entity qer)
        {

            return qudl.GetDataFromErrorLog(qer);
        }
        public DataTable Qbei_ErrorListEqualSearch(ErrorType_Entity qer)
        {

            return qudl.GetDataFromErrorLogSearch(qer);
        }
        public DataTable Qbei_ErrorListSelect(ErrorType_Entity ee)
        {
           
            return qudl.GetDataFromError_Log(ee);
        }
        public DataTable Qbei_ErrorListSelectAll(ErrorType_Entity qer)
        {
          
            return qudl.GetAllDataFromError_Log(qer);
        }

        public DataTable Qbei_ErrorListBackup(ErrorType_Entity qer)
        {
         
            return qudl.GetDataFromError_LogBackup(qer);
        }
        public DataTable BindSiteName()
        {
            return qudl.BindSiteName();
        }

        public DataTable GetErrorSiteList(Qbei_ErrorList qer)
        {
         
            return qudl.GetErrorSiteList(qer);
        }

        public DataTable FindError(int errortype)
        {
      
            return qudl.FindError(errortype);
        }

        public DataTable BindError()
        {
         
            return qudl.BindError();
        }
    }
}
