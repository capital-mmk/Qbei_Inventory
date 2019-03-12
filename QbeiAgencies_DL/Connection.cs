using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace QbeiAgencies_DL
{
    public class Connection
    {
        public SqlConnection GetConnection()
        {
            if (!Directory.Exists(@"C:\Qbei_Log\Config"))
                Directory.CreateDirectory(@"C:\Qbei_Log\Config");
            if (!File.Exists(@"C:\Qbei_Log\Config\App.config"))
                File.Create(@"C:\Qbei_Log\Config\App.config");

            Configuration config = ConfigurationManager.OpenExeConfiguration(@"C:\Qbei_Log\Config\App.config");

            if (config.ConnectionStrings.ConnectionStrings["Qbei_DB"] == null)
            {
                ConnectionStringSettings setting = new ConnectionStringSettings("Qbei_DB", "Data Source= WIN-OIL4TFU9NBH\\LOCAL2014;Initial Catalog=Qbei_Inventory;Persist Security Info=True;User ID=sa;Password=admin123456!", "System.Data.SqlClient");
                config.ConnectionStrings.ConnectionStrings.Add(setting);
                config.Save(ConfigurationSaveMode.Modified);
            }

            string constr = config.ConnectionStrings.ConnectionStrings["Qbei_DB"].ToString();     
            return new SqlConnection(constr);
        }
    }
}
