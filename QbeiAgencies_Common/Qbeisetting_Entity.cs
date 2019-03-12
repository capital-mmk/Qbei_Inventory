using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QbeiAgencies_Common
{
   public class Qbeisetting_Entity
    {
        public string ID { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Url { get; set; }
        public string sitecode { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public int site { get; set; }
        public int flag { get; set; }
     
    }
}
