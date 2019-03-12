using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Data;

namespace QbeiAgencies_BL
{
    public class QbeiUser_BL
    {
        QbeiUser_DL qudl = new QbeiUser_DL();
        public bool checkLogin(QbeiUser_Entity qe)
        {
           
            DataTable dt = qudl.Qbei_User_Select(qe);
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
        public bool User_Save(QbeiUser_Entity qe)
        {
           
            return qudl.User_Save(qe);
        }
        public DataTable UserLevel_Select(QbeiUser_Entity que)
        {
            return qudl.UserLevel_Select(que);
        }
        public DataTable checkLoginSession(QbeiUser_Entity qe)
        {
          
            DataTable dt = qudl.Qbei_User_Select1(qe);
            return dt;
        }

        public bool User_Update(QbeiUser_Entity que)
        {
           
            return qudl.User_Update(que);
        }
        public bool checkUser(QbeiUser_Entity que)
        {
          
            DataTable dt = qudl.Qbei_User_Select(que);
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
        public DataTable Qbei_UserSelectAll(QbeiUser_Entity qe)
        {
           
            return qudl.Qbei_User_Select(qe);
        }
        public DataTable GetErrorList()
        {
            QbeiUser_DL errorcount = new QbeiUser_DL();
            return errorcount.GetErrorList();
        }
        public DataTable Qbei_UserEdit(QbeiUser_Entity qe)
        {
           
            return qudl.Qbei_User_Select(qe);
        }

        public bool Qbei_UserDelete(QbeiUser_Entity que)
        {
           
            return qudl.Qbei_UserDelete(que);
        }
        public DataTable GetErrordetail(int SiteID)
        {
            QbeiUser_DL errorcount = new QbeiUser_DL();
            return errorcount.GetErrordetail(SiteID);
        }
        ////qbeierrorlog
        //public DataTable Qbei_ErrorListSelect(Qbei_ErrorList qer)
        //{
        //    QbeiUser_DL qudl = new QbeiUser_DL();
        //    return qudl.GetDataFromSiteID(qer);
        //}
        ////qbeierrorlog
        public bool CheckUpdate(QbeiUser_Entity qe, string ID)
        {
            DataTable dt = qudl.Qbei_User_Select(qe);
            if (dt.Rows.Count <= 0)
            {
                return true;
            }
            else
            {
                if (ID == dt.Rows[0]["ID"].ToString())
                {
                    return true;
                }
                else { return false; }
            }
        }
        public DataTable GetSiteData(int SiteID)
        {
            QbeiUser_DL sitedata = new QbeiUser_DL();
            return sitedata.GetSiteData(SiteID);
        }
        public DataTable GetSiteTest()
        {
            QbeiUser_DL sitetest = new QbeiUser_DL();
            return sitetest.GetSiteTest();
        }
    }
}
 