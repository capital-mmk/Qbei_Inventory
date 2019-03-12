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
    public class Qbeisetting_DL
    {
        public DataTable Qbei_Setting_Select(Qbeisetting_Entity qe)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            //SqlConnection sqlcon = con.GetConnection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_Setting_Select", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            //  if (string.IsNullOrWhiteSpace(qe.SiteID))
            if (qe.SiteID == null)
                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@ID", qe.SiteID);

            cmd.CommandTimeout = 500;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            da.Fill(dt);

            cmd.Connection.Close();
            return dt;
        }
        public bool StartTime_Save(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("StartTime_Save", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            // cmd.Parameters.AddWithValue("@Start_time", qe.starttime);
            //// cmd.Parameters.AddWithValue("@Duration", qe.duration);
            //cmd.Parameters.AddWithValue("@SiteID", qe.SiteID);
            if (qe.starttime == null)
                cmd.Parameters.AddWithValue("@Start_time", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Start_time", qe.starttime);
            if (qe.SiteID == null)
                cmd.Parameters.AddWithValue("@SiteID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@SiteID", qe.SiteID);
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;


        }
        public void EndTime_Save(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("EndTime_Save", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", qe.site);
            cmd.Parameters.AddWithValue("@flag", qe.flag);
            if (qe.endtime == null)
                cmd.Parameters.AddWithValue("@End_time", DBNull.Value);
            else cmd.Parameters.AddWithValue("@End_time", qe.endtime);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }
        public DataTable Qbei_Setting_GetData()
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //SqlConnection sqlcon = new SqlConnection("Data Source= DEVSERVER\\SQLEXPRESS;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=12345");
            SqlDataAdapter sda = new SqlDataAdapter("Qbei_Setting_GetData", sqlcon);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.CommandTimeout = 500;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
        public bool Qbeisetting_Delete(string id)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbeisetting_Delete", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@id", id);


            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;
        }
        public DataTable Qbei_SettingUpdate(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            //SqlConnection sqlcon = new SqlConnection("Data Source= DEVSERVER\\SQLEXPRESS;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=12345");
            SqlCommand cmd = new SqlCommand("Qbei_Update", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            if (qe.SiteID == null)
                cmd.Parameters.AddWithValue("@SiteID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@SiteID", qe.SiteID);

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
        public bool Qbeisetting_Update(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_SettingUpdate", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            //cmd.Parameters.AddWithValue("@id", qe.ID);
            cmd.Parameters.AddWithValue("@SiteName", qe.SiteName);
            cmd.Parameters.AddWithValue("@SiteID", qe.SiteID);
            cmd.Parameters.AddWithValue("@UserName", qe.UserName);
            cmd.Parameters.AddWithValue("@Password", qe.Password);
            // cmd.Parameters.AddWithValue("@Url", qe.Url);
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;
        }
        public bool Qbeisetting_Save(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_SettingSave", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            //cmd.Parameters.AddWithValue("@id", qe.ID);
            cmd.Parameters.AddWithValue("@SiteName", qe.SiteName);
            cmd.Parameters.AddWithValue("@SiteID", qe.SiteID);
            cmd.Parameters.AddWithValue("@UserName", qe.UserName);
            cmd.Parameters.AddWithValue("@Password", qe.Password);
            cmd.Parameters.AddWithValue("@Url", qe.Url);
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;
        }
        public DataTable Qbei_Setting_Update(Qbeisetting_Entity que)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_SettingUpdate", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@id", que.ID);
            cmd.Parameters.AddWithValue("@SiteName", que.SiteName);
            cmd.Parameters.AddWithValue("@SiteID", que.SiteID);
            cmd.Parameters.AddWithValue("@UserName", que.UserName);
            cmd.Parameters.AddWithValue("@Password", que.Password);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandTimeout = 0;
            DataTable dt = new DataTable();
            cmd.CommandType = CommandType.StoredProcedure;

            da.Fill(dt);

            cmd.Connection.Close();
            return dt;


        }
        public void ResetFlag()
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("ResetFlag", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandTimeout = 0;
            DataTable dt = new DataTable();
            cmd.CommandType = CommandType.StoredProcedure;

            da.Fill(dt);
            cmd.Connection.Close();


        }

        public DataTable Qbei_UserSelect(Qbeisetting_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_SettingUpdate", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;


            if (string.IsNullOrWhiteSpace(qe.ID))
                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@ID", qe.ID);



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

        public DataTable QubeiSettting_SelectAll(Qbeisetting_Entity qe)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            //SqlConnection sqlcon = con.GetConnection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("QubeiSettting_SelectAll", sqlcon);
            cmd.CommandTimeout = 500;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            da.Fill(dt);

            cmd.Connection.Close();
            return dt;
        }



    }
}
