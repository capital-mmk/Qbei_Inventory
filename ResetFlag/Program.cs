using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;
using QbeiAgencies_BL;

namespace ResetFlag
{
   public class Program
    {
        static void Main(string[] args)
        {

            Qbeisetting_BL qubl = new Qbeisetting_BL();
            qubl.ResetFlag();
        }
    }
}
