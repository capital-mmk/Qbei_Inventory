using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using OrderData_BL;

namespace ConsoleApplication1
{
    public class Program
    {
        static void Main(string[] args)
        {
        
            //string checkdate=string.Empty;
            string checkdate = DateTime.Now.ToString("yyyy/MM/dd");
            OrderData_BL1 obl = new OrderData_BL1();
            if (obl.QbeiOrder_Delete(checkdate))
            {
            
            }
        }
        }
    }
