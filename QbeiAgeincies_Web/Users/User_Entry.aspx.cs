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

namespace QbeiAgeincies_Web.Users
{
    public partial class User_Entry : System.Web.UI.Page
    {
        QbeiUser_Entity que = new QbeiUser_Entity();
        QbeiUser_BL qubl = new QbeiUser_BL();

        protected void Page_Load(object sender, EventArgs e)
        {
            //     lblinfo
            //lblpw
            //lblpass
           // txtuserid.Focus();
            //lblnewpassword.Visible = false;
          
            //txtnewpassword.Visible = false;
            //lblconfirm.Visible = false;
            //txtconfirm.Visible = false;
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {

                    Edit();
                }
                string status = Session["admin"] as string;
                if (status == "1")
                {
                    
                    //rdbAdmin.Enabled = false;
                }
            }
            txtpassword.Attributes.Add("type", "password");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //if (btnsave.Text == "登録")
            //{
            //    Save();
            //}
            //else
            //{
                Update();
            //}
        }
        // Qbei_UserEdit
        protected void Edit()
        {
            txtuserid.Enabled = false;
            //btnsave.Text = "更新";
            //lblnewpassword.Visible = true;
            //txtnewpassword.Visible = true;
            //lblconfirm.Visible = true;
            //txtconfirm.Visible = true;
            // lblpassword.Text = "古いパスワード";
            string st = Session["UserID"] as String;

            int ID = Convert.ToInt32(Request.QueryString["ID"]);
            Crypto crypto = new Crypto();
            que.ID = Request.QueryString["ID"];
            que.UserID = st;
            que.Password = (Session["Password"] as string);

            txtuserid.Text = que.UserID;
            txtpassword.Text = que.Password;

           // DataTable dt = qubl.Qbei_UserEdit(que);



            //txtuserid.Text = dt.Rows[0]["UserID"].ToString();
            //string userlevel = dt.Rows[0]["UserLevel"].ToString();
            //if (userlevel == "0")
            //{
            //    rdbAdmin.Checked = true;
            //}
            //else
            //{
            //    rdbUser.Checked = true;
            //}

        }

        //protected void Save()
        //{
        //    if (Check())
        //    {
        //        Crypto crypto = new Crypto();
        //        que.UserID = txtuserid.Text;
        //        //que.Password = txtpassword.Text;
        //        que.Password = crypto.Encrypt(txtpassword.Text, "Qbei12345");
        //        que.UserLevel = UserLevel().ToString();
        //        if (qubl.User_Save(que))
        //        {
        //            Response.Write("<script>alert('登録完了');</script>");
        //            clear();
        //            //Response.Redirect("UserList.aspx");
        //        }
        //    }
        //}

        protected void Update()
        {
            if (!string.IsNullOrWhiteSpace(txtuserid.Text) && !string.IsNullOrWhiteSpace(txtpassword.Text))
            {
                Crypto crypto = new Crypto();
                que.ID = Request.QueryString["ID"];
                que.UserID = txtuserid.Text;

                //que.ConfirmPassword = crypto.Encrypt(txtconfirm.Text, "Qbei12345");
                //que.NewPassword = crypto.Encrypt(txtnewpassword.Text, "Qbei12345");
                //que.UserLevel = UserLevel().ToString();
                que.Password = txtpassword.Text;
                if (qubl.User_Update(que))
                {

                    Response.Write("<script>alert('更新が完了しました');</script>");
                    //Response.Redirect("UserList.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Failed to update');</script>");
                }
            }
            else {
                Response.Write("<script>alert('Please Fill ID and Password');</script>");
            }
        }


        //protected bool Check()
        //{

        //    que.UserID = txtuserid.Text;
        //    if (String.IsNullOrWhiteSpace(txtpassword.Text))
        //    {
        //        txtpassword.Focus();
        //        return false;
        //    }
        //    if (qubl.checkUser(que))
        //    {
        //        lblinfo.Visible = true;
        //        txtuserid.Focus();
        //    }
        //    else
        //    {
        //        lblinfo.Visible = false;
        //        return true;
        //    }

        //    return false;
        //}

        //protected bool Checkinfo()
        //{
        //    int ID = Convert.ToInt32(Request.QueryString["ID"]);
        //    Crypto crypto = new Crypto();
        //    que.ID = Request.QueryString["ID"];
        //    DataTable dt = qubl.Qbei_UserEdit(que);
        //    if (!CheckUpdate())
        //    {
        //        lblinfo.Visible = true;
        //        return false;
        //    }


        //    if (!String.IsNullOrWhiteSpace(txtpassword.Text))
        //    {
        //        if (crypto.Decrypt(dt.Rows[0]["Password"].ToString(), "Qbei12345").Equals(txtpassword.Text))
        //        {
        //            lblpw.Visible = false;
        //        }

        //        else
        //        {
        //            lblpw.Visible = true;
        //            txtpassword.Attributes.Add("onfocus", "this.select();");
        //            lblnewpassword.Visible = true;
        //            txtnewpassword.Visible = true;
        //            lblconfirm.Visible = true;
        //            txtconfirm.Visible = true;

        //            return false;
        //        }

        //    }
        //    else
        //    {
        //        lblnewpassword.Visible = true;
        //        txtnewpassword.Visible = true;
        //        lblconfirm.Visible = true;
        //        txtconfirm.Visible = true;
        //        return false;
        //    }
        //    if (!String.IsNullOrWhiteSpace(txtnewpassword.Text))
        //    {
        //        if (!txtconfirm.Text.Equals(txtnewpassword.Text))
        //        {
        //            lblpass.Visible = true;
        //            lblnewpassword.Visible = true;
        //            txtnewpassword.Visible = true;
        //            lblconfirm.Visible = true;
        //            txtconfirm.Visible = true;
        //            return false;
        //        }
        //        else
        //        {
        //            lblpass.Visible = false;
        //            lblpassword.Text = "Password";
        //            lblnewpassword.Visible = true;
        //            txtnewpassword.Visible = true;
        //            lblconfirm.Visible = true;
        //            txtconfirm.Visible = true;

        //        }

        //    }
        //    else
        //    {
        //        lblpass.Visible = true;
        //        lblpass.Text = "*";
        //        lblnewpassword.Visible = true;
        //        txtnewpassword.Visible = true;
        //        lblconfirm.Visible = true;
        //        txtconfirm.Visible = true;
        //        return false;
        //    }
        //    return true;

        //}

        //public void clear()
        //{
        //    txtuserid.Text = "";
        //    txtpassword.Text = "";
        //    rdbUser.Checked = true;
        //}

        //public int UserLevel()
        //{
        //    QbeiUser_Entity que = new QbeiUser_Entity();
        //    if (rdbUser.Checked == true)

        //        return 1;
        //    else { return 0; }

        //}

        //public bool CheckUpdate()
        //{
        //    que.UserID = txtuserid.Text;
        //    return qubl.CheckUpdate(que, Request.QueryString["ID"].ToString());

        //}

    }
}