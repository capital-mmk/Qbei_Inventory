using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QbeiAgencies_DL;

namespace OrderData_DL
{
   public class OrderData_Dl1
    {
       static void Main(string[] args)
       {

       }
        public bool QbeiOrder_Delete(string checkdate)
        {
           // Connection con = new Connection();
            // SqlConnection sqlcon = con.GetConnection();
            Connection con = new Connection();
            SqlConnection sqlcon = con.GetConnection();
         // SqlConnection sqlcon = new SqlConnection("Data Source= DEVSERVER\\SQLEXPRESS;Initial Catalog=Qbei_Inventory;Persist Security Info=True;User ID=sa;Password=12345");
            SqlCommand cmd = new SqlCommand("OrderData_Delete", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection.Open();
            cmd.Parameters.AddWithValue("@checkDate",checkdate);
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return true;
        }
        
    }
}
