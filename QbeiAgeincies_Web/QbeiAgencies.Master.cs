using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace QbeiAgeincies_Web
{
    public partial class QbeiAgencies : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
                
            if (Session["UserID"] != null)
            {
                //btnLoginUser.Text = Session["UserID"].ToString();

                //HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                //HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
                //HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //HttpContext.Current.Response.Cache.SetNoStore();
            }
            else
            {
                Response.Redirect("~/Login/Login.aspx");
            }
        }

        protected void UserEntry_Click(object sender,EventArgs e)
        {
            Response.Redirect("~/Users/UserEntry.aspx");
        }

        protected void UserList_Click(object sender,EventArgs e)
        {
            Response.Redirect("~/Users/UserList.aspx");
        }


        protected void lbErrorLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ErrorCheck/ErrorList.aspx");
        }

        protected void lbErrorBackup_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/ErrorCheck/ErrorList_Backup.aspx");
        }

        protected void lbLogOut_Click(object sender, EventArgs e)
        {

            Session.Abandon();
            //FormsAuthentication.SignOut();
            //Session["User_ID"] = null;
            //Session["Password"] = null;
            Response.Redirect("~/Login/Login.aspx");


        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DashBoard.aspx");
        }

    }
}