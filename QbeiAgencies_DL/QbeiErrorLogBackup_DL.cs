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
    public class QbeiErrorLogBackup_DL
    {

        //errorlog-start
        public DataTable Qbei_ErrorListSelect()
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //SqlConnection sqlcon = new SqlConnection("Data Source= DEVSERVER\\SQLEXPRESS;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=12345");
            SqlDataAdapter sdas = new SqlDataAdapter("[QbeiErrorLogBack_Select]", sqlcon);
            sdas.SelectCommand.CommandType = CommandType.StoredProcedure;
            sdas.SelectCommand.Connection.Open();
            sdas.Fill(dt);

            sdas.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable GetDataFromErrorLogBackup(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlDataAdapter sda = new SqlDataAdapter("QbeiErrorLogBackup_EqualSearch", sqlcon);

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
        public DataTable FindError(int sitename)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("Qbei_FindSiteName", sqlcon);

            if (sitename == null)
                sda.SelectCommand.Parameters.AddWithValue("@SiteName", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@SiteName", sitename);

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

        public DataTable GetDataFromError_Log(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //SqlDataAdapter sda = new SqlDataAdapter("[QbeiError_Search]", sqlcon);
            SqlDataAdapter sda = new SqlDataAdapter("[QbeiErrorLogBackup_SiteIDSearch]", sqlcon);
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
        public DataTable GetDataFromError(ErrorType_Entity qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("QbeiError_SiteSearch", sqlcon);

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
        public DataTable GetDate(Qbei_ErrorList qer)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("get_Date", sqlcon);



            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public DataTable ErrorlogBackup_SelectAll(ErrorlogBackup_Entity ebe)
        {
            try
            {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("ErrorlogBackup_SelectAll", sqlcon);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);
            sda.SelectCommand.Connection.Close();
            return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable BindError()
        {
            try
            {
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
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
        public DataTable BindSiteName()
        {
            try
            {
                Connection con = new Connection();
                SqlConnection sqlcon = con.GetConnection();
                SqlCommand cmd = new SqlCommand("Select ID,SiteName from dbo.Site_Setting order by ID", sqlcon);

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
        public DataTable GetDataFromError_LogBackupDateSearch(string date)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("QbeiErrorLogBackup_DateSearch", sqlcon);

            if (string.IsNullOrWhiteSpace(date))
                sda.SelectCommand.Parameters.AddWithValue("@Date", DBNull.Value);
            else sda.SelectCommand.Parameters.AddWithValue("@Date", date);

            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }

    }
}

