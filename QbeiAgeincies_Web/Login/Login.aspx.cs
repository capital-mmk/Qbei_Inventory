using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Collections;
using System.Web.Security;
using System.Data;

namespace QbeiAgeincies_Web.Login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            txtUserID.Focus();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            Crypto crypto = new Crypto();
            QbeiUser_Entity que = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            que.UserID = txtUserID.Text;
            que.Password = txtPassword.Text;
            if (!String.IsNullOrWhiteSpace(que.UserID) && !String.IsNullOrWhiteSpace(que.Password))
            {
                DataTable dt = qubl.Qbei_UserSelectAll(que);
                if (dt.Rows.Count > 0)
                {
                    if ((dt.Rows[0]["User_Role"]).ToString() == "0")
                    {
                        Session["admin"] = "1";
                    }
                    else
                    {
                        Session["admin"] = "0";
                    }
                    Session["ID"] = dt.Rows[0]["ID"].ToString() ;
                    Session["UserID"] = dt.Rows[0]["UID"].ToString();
                    Session["Password"]= dt.Rows[0]["Password"].ToString();
                    lblErrorMsg.Visible = false;
                    Response.Redirect("~/DashBoard.aspx");
                }
                else
                {

                    lblErrorMsg.Visible = true;
                }
                        
            }
            else
            {
               
                lblErrorMsg.Visible = true;
               
            }
           
            // que.Password = crypto.Encrypt(txtPassword.Text, "Qbei12345");
            //if (qubl.checkLogin(que))
            //{
            //    DataTable dtUserLevel = qubl.UserLevel_Select(que);
            //    if ((dtUserLevel.Rows[0]["UserLevel"]).ToString() == "0")
            //    {
            //        Session["admin"] = "0";
            //    }
            //    else
            //    {
            //        Session["admin"] = "1";
            //    }
            //    Session["UserID"] = que.UserID;
            //    session();
            //    lblErrorMsg.Visible = false;
            //    Response.Redirect("~/DashBoard.aspx");
            //}
            //else
            //{
            //    lblErrorMsg.Visible = true;
            //}



        }
        protected void session()
        {

            Crypto crypto = new Crypto();
            QbeiUser_Entity que = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            que.UserID = Session["UserID"].ToString();
            DataTable dt = qubl.checkLoginSession(que);

            Session["ID"] = dt.Rows[0]["ID"].ToString();

        }
    }
}