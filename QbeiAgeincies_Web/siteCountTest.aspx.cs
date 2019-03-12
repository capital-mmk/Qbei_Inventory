using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QbeiAgeincies_Web
{
    public partial class siteCountTest : System.Web.UI.Page
    {
        DataTable dt = new DataTable();
        QbeiAgencies_BL.QbeiUser_BL sitetest = new QbeiAgencies_BL.QbeiUser_BL();
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }
        protected void DataBind()
        {
            dt = sitetest.GetSiteTest();
            gvsiteCountTest.DataSource = dt;
            gvsiteCountTest.DataBind();
        }
    }
}