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
    public class QbeiUser_DL
    {
        public DataTable Qbei_User_Select(QbeiUser_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_User_Select_by", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            //if (string.IsNullOrWhiteSpace(qe.ID))
            //    cmd.Parameters.AddWithValue("@ID", DBNull.Value);
            //else cmd.Parameters.AddWithValue("@ID", qe.ID);   

            if (string.IsNullOrWhiteSpace(qe.UserID))
                cmd.Parameters.AddWithValue("@UserID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@UserID", qe.UserID);

            if (string.IsNullOrWhiteSpace(qe.Password))
                cmd.Parameters.AddWithValue("@Password", DBNull.Value);
            else cmd.Parameters.AddWithValue("@Password", qe.Password);

            //if (string.IsNullOrWhiteSpace(qe.UserLevel))
            //    cmd.Parameters.AddWithValue("@UserLevel", DBNull.Value);
            //else cmd.Parameters.AddWithValue("@UserLevel", qe.UserLevel);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            da.Fill(dt);
            return dt;

        }
        public DataTable UserLevel_Select(QbeiUser_Entity que)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("UserLevel_Select", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@userID", que.UserID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public bool User_Save(QbeiUser_Entity que)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_UserSave", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();

            cmd.Parameters.AddWithValue("@UserID", que.UserID);
            cmd.Parameters.AddWithValue("@Password", que.Password);

            cmd.Parameters.AddWithValue("@UserLevel", que.UserLevel);

            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;


        }

        public bool User_Update(QbeiUser_Entity que)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_UserUpdate_by", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@id", que.ID);
            cmd.Parameters.AddWithValue("@UserID", que.UserID);
            cmd.Parameters.AddWithValue("@Password", que.Password);

           // cmd.Parameters.AddWithValue("@UserLevel", que.UserLevel);

            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;


        }

        public bool Qbei_UserDelete(QbeiUser_Entity que)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
            // SqlConnection sqlcon = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlCommand cmd = new SqlCommand("Qbei_UserDelete", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@id", que.ID);
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;

        }
        public DataTable Qbei_User_Select1(QbeiUser_Entity qe)
        {
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
        
            SqlCommand cmd = new SqlCommand("Qbei_User_Select1", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            if (string.IsNullOrWhiteSpace(qe.ID))
                cmd.Parameters.AddWithValue("@ID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@ID", qe.ID);

            if (string.IsNullOrWhiteSpace(qe.UserID))
                cmd.Parameters.AddWithValue("@UserID", DBNull.Value);
            else cmd.Parameters.AddWithValue("@UserID", qe.UserID);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
          
            da.Fill(dt);
            return dt;
      
        }

        public DataTable GetErrorList()
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            //SqlConnection conn = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("Qbei_ErrorCount", sqlcon);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }


        public DataTable GetErrordetail(int SiteID)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            //SqlConnection conn = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_ErrorDetailCount", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@site", SiteID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                cmd.Connection.Open();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                cmd.Connection.Close();
            }



        }

        public DataTable GetSiteData(int SiteID)
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            // SqlConnection conn = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlConnection sqlcon = con.GetConnection();
            SqlCommand cmd = new SqlCommand("Qbei_SiteData", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SiteID", SiteID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                cmd.Connection.Open();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
            finally
            {
                cmd.Connection.Close();
            }



        }

        public DataTable GetSiteTest()
        {
            DataTable dt = new DataTable();
            Connection con = new Connection();
            // SqlConnection conn = new SqlConnection("Data Source= WIN-QUBJD0QF04H\\SQL2014;Initial Catalog=Qbei_Agencies;Persist Security Info=True;User ID=sa;Password=capital12345k!");
            SqlConnection sqlcon = con.GetConnection();
            SqlDataAdapter sda = new SqlDataAdapter("Qbei_SiteCountTest", sqlcon);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.CommandTimeout = 500;
            sda.SelectCommand.Connection.Open();
            sda.Fill(dt);

            sda.SelectCommand.Connection.Close();
            return dt;
        }
    }
}
