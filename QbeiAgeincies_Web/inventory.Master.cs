using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QbeiAgeincies_Web
{

    public partial class inventory : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                string str = Session["UserID"].ToString();
                lblUserID.Text = str;
                admin_Menu();
            }

            else
            {
                Response.Redirect("~/Login/Login.aspx");
            }
        }
        protected void admin_Menu()
        {
            try
            {
                string status = Session["admin"] as string;
                if (status == "0")
                {
                    admin_role.Visible = true;
                }
                else
                {
                    admin_role.Visible = false;
                }
            }
            catch
            {
                Session.Abandon();
                Response.Redirect("~/Login/Login.aspx");
            }
        }
        protected void Anchor_Click(Object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Login/Login.aspx");
        }
        protected void Setting_Click(Object sender, EventArgs e)
        {
            string id1 = string.Empty;
            try { 
             id1 = Session["ID"].ToString();
        }
            catch {
                Session.Abandon();
                Response.Redirect("~/Login/Login.aspx");
            }
            Response.Redirect("~/Users/User_Entry.aspx?ID=" + id1);
        }

        public string getUserID()
        {
            if (Session["UserID"] != null)
            {
                string str = Session["UserID"].ToString();
                return str;
            }
            else
            {
                Response.Redirect("~/Login/Login.aspx");
                return string.Empty;
            }
        }
    }
}