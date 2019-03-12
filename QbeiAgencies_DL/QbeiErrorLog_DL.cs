using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using System.Configuration;

namespace QbeiAgencies_DL
{
   public class QbeiErrorLog_DL
    {

        public DataTable Qbei_ErrorListSelect()
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sdas = new SqlDataAdapter("Qbei_ErrorListSelect", sqlcon);
            sdas.SelectCommand.CommandType = CommandType.StoredProcedure;
            sdas.SelectCommand.Connection.Open();
            sdas.Fill(dt);

            sdas.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable GetDataFromError_LogDateSearch(string date)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_LogDateSearch", sqlcon);

            //if (string.IsNullOrWhiteSpace(qer.SiteName))
            //    sda.SelectCommand.Parameters.AddWithValue("@SiteName", DBNull.Value);
            //else sda.SelectCommand.Parameters.AddWithValue("@SiteName", qer.SiteName);

            //if (qer.ErrorType == null)
            //    sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            //else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", qer.ErrorType);

            if (string.IsNullOrWhiteSpace(date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }

        public DataTable GetDataFromError_Log(Qbei_ErrorList qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_SiteIDSearch", sqlcon);

            if (string.IsNullOrWhiteSpace(qer.SiteID))
                sda.SelectCommand.Parameters.AddWithValue("@SiteID", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteID", qer.SiteID);

            if (string.IsNullOrWhiteSpace(qer.ErrorType))
                sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", qer.ErrorType);

            if (string.IsNullOrWhiteSpace(qer.Date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", qer.Date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable GetDataFromError_Log(ErrorType_Entity ee)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_SiteIDSearch", sqlcon);

            if (string.IsNullOrWhiteSpace(ee.SiteName))
                sda.SelectCommand.Parameters.AddWithValue("@SiteName", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteName", ee.SiteName);

            if ((ee.ErrorType)==null)
                sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", ee.ErrorType);

            if (string.IsNullOrWhiteSpace(ee.Date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", ee.Date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }

        public DataTable GetAllDataFromError_Log(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_AllDisplay", sqlcon);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable GetDataFromErrorLog(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_EqualSelect", sqlcon);

            if (string.IsNullOrWhiteSpace(qer.SiteName))
                sda.SelectCommand.Parameters.AddWithValue("@SiteName", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteName", qer.SiteName);

            if (qer.ErrorType == null)
                sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", qer.ErrorType);

            if (string.IsNullOrWhiteSpace(qer.Date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", qer.Date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable GetDataFromErrorLogSearch(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_EqualSearch", sqlcon);

            if (string.IsNullOrWhiteSpace(qer.SiteName))
                sda.SelectCommand.Parameters.AddWithValue("@SiteName", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteName", qer.SiteName);

            if (qer.ErrorType == null)
                sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", qer.ErrorType);

            if (string.IsNullOrWhiteSpace(qer.Date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", qer.Date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable BindSiteName()
        {
            try
            {
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
                SqlCommand cmd = new SqlCommand("Select  SiteName,ID from dbo.Site_Setting where SiteName is not NULL order by ID", sqlcon);

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                cmd.Connection.Open();
                sda.Fill(dt);
                cmd.Connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetDataFromError_LogBackup(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_LogBackupSearch", sqlcon);

            if (qer.SiteID==null)
                sda.SelectCommand.Parameters.AddWithValue("@SiteID", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteID", qer.SiteID);

            if (qer.ErrorType==null)
                sda.SelectCommand.Parameters.AddWithValue("@ErrorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@ErrorType", qer.ErrorType);

            if (string.IsNullOrWhiteSpace(qer.Date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", qer.Date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }

        public DataTable GetErrorSiteList(Qbei_ErrorList qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("ErrorSiteList", sqlcon);

            if (string.IsNullOrWhiteSpace(qer.SiteID))
                sda.SelectCommand.Parameters.AddWithValue("@siteID", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@siteID", qer.SiteID);

            if (string.IsNullOrWhiteSpace(qer.Description))
                sda.SelectCommand.Parameters.AddWithValue("@description", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@description", qer.Description);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable FindError(int errortype)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("Qbei_FindError", sqlcon);

            if (errortype == null)
                sda.SelectCommand.Parameters.AddWithValue("@errorType", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@errorType", errortype);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }

        public DataTable BindError()
        {
            try
            {
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
                //SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
                SqlCommand cmd = new SqlCommand("Select Type,Description from dbo.ErrorType order by Type", sqlcon);

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                cmd.Connection.Open();
                sda.Fill(dt);
                cmd.Connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
