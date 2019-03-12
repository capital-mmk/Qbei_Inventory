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

namespace QbeiAgeincies_Web.Qbei_Setting
{
    public partial class qbeisetting : System.Web.UI.Page
    {
        Qbeisetting_BL qubl = new Qbeisetting_BL();
        protected void Page_Load(object sender, EventArgs e)
        {

            Qbeisetting_Entity qe = new Qbeisetting_Entity();

            //qe.ID = Session["ID"].ToString();
            DataTable dt = qubl.Qbei_Setting_Select(qe);

            if (!IsPostBack)
            {
                //  lblSave.Text = "Update";
                Getdata();

            }

        }
        private void Getdata()
        {
            DataTable dt = new DataTable();
            Qbeisetting_BL qubl = new Qbeisetting_BL();
            dt = qubl.Qbei_Setting_GetData();

            if (dt.Rows.Count > 0)
            {
                gvTest.DataSource = dt;
                gvTest.DataBind();
            }
        }
        protected void btnEdit2_Click(object sender, EventArgs e)
        {
            HtmlAnchor anc = sender as HtmlAnchor;
            GridViewRow Grow = (GridViewRow)anc.NamingContainer;
            string ID = ((Label)Grow.FindControl("lblsiteid")).Text;
            Response.Redirect("SiteSetting_Edit.aspx?ID=" + ID);
        }
        protected void btnnew_ServerClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Site_Setting/SiteSetting_Edit.aspx");
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            HtmlAnchor anc = sender as HtmlAnchor;
            GridViewRow Grow = (GridViewRow)anc.NamingContainer;

            string ID = ((Label)Grow.FindControl("lblsiteid")).Text;

                if (qubl.Qbeisetting_Delete(ID))
                {
                //Response.Write("削除してもよろしいでしょうか?");
                //Response.Redirect("SiteSetting.aspx");

                //Response.Write("<script>alert('削除が完了しました');</script>");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('削除が完了しました')", true);
                Qbeisetting_Entity queset = new Qbeisetting_Entity();
                    Qbeisetting_BL queset_bl = new Qbeisetting_BL();

                    DataTable dt = queset_bl.QubeiSettting_SelectAll(queset);

                    if (dt.Rows.Count > 0)
                    {
                        gvTest.DataSource = dt;
                        gvTest.DataBind();
                    }
                }            
        }

        protected void gvTest_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Getdata();
            gvTest.PageIndex = e.NewPageIndex;
            gvTest.DataBind();


        }

    }
}
