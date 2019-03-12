using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Data;

namespace QbeiAgencies_BL
{
    public class Qbeisetting_BL
    {
        Qbeisetting_DL qudl = new Qbeisetting_DL();
        public DataTable Qbei_Setting_Select(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.Qbei_Setting_Select(qe);
        }

        public DataTable QubeiSettting_SelectAll(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.QubeiSettting_SelectAll(qe);
        }
        public DataTable Qbei_Setting_GetData()
        {
            return qudl.Qbei_Setting_GetData();
        }
        public bool Qbeisetting_Delete(string id)
        {
            return qudl.Qbeisetting_Delete(id);
        }

        public DataTable Qbei_SettingUpdate(Qbeisetting_Entity qe)
        {
            return qudl.Qbei_SettingUpdate(qe);
        }
        public bool StartTime_Save(Qbeisetting_Entity qe)
        {

            return qudl.StartTime_Save(qe);
        }
        public void EndTime_Save(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            qudl.EndTime_Save(qe);
        }
        public bool checkSiteID(Qbeisetting_Entity qe)
        {
            DataTable dt = qudl.Qbei_Setting_Select(qe);
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        public DataTable Qbei_SettingSelect(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.Qbei_Setting_Select(qe);
        }
        public void ResetFlag()
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            qudl.ResetFlag();

        }

        public DataTable Qbei_UserEdit(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.Qbei_UserSelect(qe);
        }
        public bool Qbeisetting_Update(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.Qbeisetting_Update(qe);
        }
        public bool Qbeisetting_Save(Qbeisetting_Entity qe)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            return qudl.Qbeisetting_Save(qe);
        }
        public bool CheckUpdate(Qbeisetting_Entity qe, string ID)
        {
            Qbeisetting_DL qudl = new Qbeisetting_DL();
            DataTable dt = qudl.Qbei_Setting_Select(qe);
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
    }
}