using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using QbeiAgencies_BL;
using QbeiAgencies_Common;

namespace QbeiAgeincies_Web.Qbei_Setting
{
    public partial class settingedit : System.Web.UI.Page
    {
        Qbeisetting_Entity qe = new Qbeisetting_Entity();
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        protected void Page_Load(object sender, EventArgs e)
        {

            txtsitename.Focus();
            lblpassword.Visible = true;
            txtsitename.Visible = true;
            txtpassword.Visible = true;
          

            if (!IsPostBack)
            {
                // int ID = Convert.ToInt32(Request.QueryString["SiteID"]);

                if (Request.QueryString["ID"] != null)
                {
                    btnSave.Text = "更新";
                    Edit();
                }
                else
                {
                    Edit();
                }
            }
        }

        protected void Edit()
        {
            if (Request.QueryString["ID"] != null)
            {
                txtsiteid.ReadOnly = true;
            }
            else
            {
                txtsiteid.ReadOnly = false;
            }
            txtsiteid.Visible = true;
            txtpassword.Visible = true;
            txtsitename.Enabled = true;
            txtusername.Enabled = true;
            txtpassword.Enabled = true;
            int ID = Convert.ToInt32(Request.QueryString["SiteID"]);
            Crypto crypto = new Crypto();
            qe.SiteID = Convert.ToInt32(Request.QueryString["ID"]);


            DataTable dt = qubl.Qbei_SettingSelect(qe);
            if (dt.Rows.Count > 0)
            {
                txtsiteid.Text = dt.Rows[0]["SiteID"].ToString();

                txtsitename.Text = dt.Rows[0]["SiteName"].ToString();

                txtusername.Text = dt.Rows[0]["UserName"].ToString();
               
                txtpassword.Text = dt.Rows[0]["Password"].ToString();
                txturl.Text = dt.Rows[0]["Url"].ToString();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "更新")
            {
                Update();
            }
            else
            {
                Save();

            }
        }
        protected void Save()
        {
            if (Check())
            {
                txtsiteid.Visible = true;
                txtsitename.Visible = true;
                txtusername.Visible = true;
                txtpassword.Visible = true;
                txtsiteid.Enabled = true;
                txtsitename.Enabled = true;
                txtusername.Enabled = true;
                txtpassword.Enabled = true;
                qe.SiteID = Convert.ToInt32(txtsiteid.Text);
                qe.SiteName = txtsitename.Text;
                qe.UserName = txtusername.Text;
                qe.Password = txtpassword.Text;
                qe.Url = txturl.Text;
                if (qubl.Qbeisetting_Save(qe))
                {
                    //Response.Write("<script>alert('登録完了');</script>");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('登録完了')", true);
                    Clear();
                  
                }
            }

        }
        protected bool Check()
        {

            qe.SiteID = Convert.ToInt32(txtsiteid.Text);
            if (String.IsNullOrWhiteSpace(txtpassword.Text))
            {
                txtpassword.Focus();
                return false;
            }
            if (qubl.checkSiteID(qe))
            {
                lblinfo1.Visible = true;
                txtsiteid.Focus();
            }
            else
            {
                lblinfo1.Visible = false;
                return true;
            }

            return false;
        }
        protected void Update()
        {

            Crypto crypto = new Crypto();
            // qe.ID = Request.QueryString["ID"];
            qe.ID = txtsiteid.Text;
            qe.SiteID = Convert.ToInt32(txtsiteid.Text);
            qe.SiteName = txtsitename.Text;
            qe.UserName = txtusername.Text;
            qe.Password = txtpassword.Text;
            qe.Url = txturl.Text;
            if (qubl.Qbeisetting_Update(qe))
            {
                //Response.Redirect("SiteSetting.aspx");
                //Response.Write("<script>alert('更新が完了しました');</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('更新が完了しました')", true);
            }
        }


        private void Clear()
        {
            txtsiteid.Text = string.Empty;
            txtsitename.Text = string.Empty;
            txtusername.Text = string.Empty;
            txtpassword.Text = string.Empty;
            txturl.Text = string.Empty;
        }
    }


}
