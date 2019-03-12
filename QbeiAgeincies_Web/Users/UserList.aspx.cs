using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Web.UI.HtmlControls;

namespace QbeiAgeincies_Web.Users
{
    public partial class UserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            QbeiUser_Entity qe = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            inventory i1 = this.Master as inventory;
            string userid = i1.getUserID();
            ddlPgsize.Visible = true;

            if (!string.IsNullOrWhiteSpace(userid))
            {
                qe.UserID = userid;
                DataTable dt = qubl.Qbei_UserSelectAll(qe);
                string UserLevel = dt.Rows[0]["UserLevel"].ToString();
                if (!IsPostBack)
                {
                    Getdata();
                    if (UserLevel == "1")
                    {
                        gvTest.Columns[4].Visible = false;
                        gvTest.Columns[3].Visible = false;
                    }
                }
            }

        }

        private void Getdata()
        {
            DataTable dt = new DataTable();
            QbeiUser_BL qubl = new QbeiUser_BL();
            QbeiUser_Entity qe = new QbeiUser_Entity();
            dt = qubl.Qbei_UserSelectAll(qe);

            if (dt.Rows.Count > 0)
            {
                gvTest.DataSource = dt;
                gvTest.DataBind();
            }
        }

        protected void gv_userlevel_rowdatabound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lbluserlevel = (Label)e.Row.FindControl("lbluserlevel");
                if (lbluserlevel.Text == "1")
                {
                    lbluserlevel.Text = "ユーザー";
                }
                else
                {
                    lbluserlevel.Text = "管理者";
                }
            }
        }

        protected void gvTest_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Getdata();
            gvTest.PageIndex = e.NewPageIndex;
            //gvTest.DataBind();
            Search();

        }

        protected void btgo_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtPage.Text))
            {
                Getdata();
                gvTest.PageIndex = Convert.ToInt32(txtPage.Text) - 1;
                txtPage.Text = "";
                gvTest.DataBind();
                Search();
            }

        }

        protected void PageSize_IndexChanged(object sender, EventArgs e)
        {
            Getdata();
            gvTest.PageSize = Convert.ToInt32(ddlPgsize.Text);
            gvTest.DataBind();
            Search();
        }

        protected void btnEdit1_Click(object sender, EventArgs e)
        {
            HtmlAnchor anc = sender as HtmlAnchor;
            GridViewRow Grow = (GridViewRow)anc.NamingContainer;
            string ID = ((Label)Grow.FindControl("lblid")).Text;
            Response.Redirect("UserEntry.aspx?ID=" + ID);
        }

        protected void btnDelete1_Click(object sender, EventArgs e)
        {
            QbeiUser_Entity que = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            HtmlAnchor anc = sender as HtmlAnchor;
            GridViewRow Grow = (GridViewRow)anc.NamingContainer;
            que.ID = ((Label)Grow.FindControl("lblid")).Text;

            
                if (qubl.Qbei_UserDelete(que))
                {

                //Response.Write("<script>alert('削除が完了しました');</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('削除が完了しました')", true);
                DataTable dt = new DataTable();
                    QbeiUser_Entity qe = new QbeiUser_Entity();
                    dt = qubl.Qbei_UserSelectAll(qe);

                    if (dt.Rows.Count > 0)
                    {
                        gvTest.DataSource = dt;
                        gvTest.DataBind();
                        Search();
                    }
                }            

        }

        protected void Search()
        {
            DataTable dt = new DataTable();
            QbeiUser_BL qubl = new QbeiUser_BL();
            QbeiUser_Entity qe = new QbeiUser_Entity();
            dt = qubl.Qbei_UserSelectAll(qe);

            if (dt.Rows.Count > 0)
            {
                string search = string.Empty;
                if (!string.IsNullOrWhiteSpace(txtSearchUserID.Text))
                    search = "UserID LIKE '%" + txtSearchUserID.Text + "%'";
                if (ddluserlevel.SelectedItem.Value != "-1")
                {
                    if (!string.IsNullOrWhiteSpace(search))
                        search += " AND ";

                    search += "UserLevel =" + ddluserlevel.SelectedItem.Value;
                }

                gvTest.DataSource = dt;
                dt.DefaultView.RowFilter = search;
                gvTest.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }
    }
}






