using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class Qbei_Entity
    {
        public string stockDate { get; set; }
        public string True_Quantity { get; set; }//<remark 2020/11/05>
        public string price { get; set; }
        public string orderCode { get; set; }
        public string qtyStatus { get; set; }
        public string True_StockDate { get; set; }//<remark 2020/11/05>
        public string purchaseURL { get; set; }
        public string sitecode { get; set; }
        public string janCode { get; set; }
        public string partNo { get; set; }
        public string makerDate { get; set; }
        public string reflectDate { get; set; }
        public int siteID { get; set; }
    }
}
