using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QbeiAgencies_Common
{
    public class QbeiUser_Entity
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserLevel { get; set; }


    }
}
