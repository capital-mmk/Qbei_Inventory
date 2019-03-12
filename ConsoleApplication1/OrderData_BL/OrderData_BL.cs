using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderData_DL;

namespace OrderData_BL
{
  public  class OrderData_BL1
    {
      public bool QbeiOrder_Delete(string checkdate)
      {
          OrderData_Dl1 qudl = new OrderData_Dl1();
          return qudl.QbeiOrder_Delete(checkdate);
      }

    }
}
