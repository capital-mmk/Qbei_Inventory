using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QbeiAgencies_Common
{
   public class ErrorlogBackup_Entity
    {
        public int ID { get; set; }
        public string SiteName { get; set; }
        public string JanCode { get; set; }
        public string OrderCode { get; set; }
        public int ErrorType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
