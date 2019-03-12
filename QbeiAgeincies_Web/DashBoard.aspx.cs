using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Net.Mail;
using System.Net;


namespace QbeiAgeincies_Web
{
    public partial class DashBoard : System.Web.UI.Page
    {
        System.Data.DataTable dt;
        DataTable dterrordetail;
        QbeiAgencies_BL.QbeiUser_BL errorcount = new QbeiAgencies_BL.QbeiUser_BL();
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListViewControlBind();
            }

        }

        public void ListViewControlBind()
        {
            dt = errorcount.GetErrorList();//get total count of Qbei
            siteList.DataSource = dt;
            siteList.DataBind();
         
        }

        protected void siteList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Label lblid = e.Item.FindControl("lblSiteID") as Label;
                GridView gv = e.Item.FindControl("gdTest") as GridView;
                dterrordetail = errorcount.GetErrordetail(Convert.ToInt32(lblid.Text));
                gv.DataSource = dterrordetail;
                gv.DataBind();
            }

        }

        protected void lberror_Click(object sender, EventArgs e)
        {
            LinkButton btntrans = (LinkButton)sender;
            GridViewRow Grow = (GridViewRow)btntrans.NamingContainer;
            string desc = ((Label)Grow.FindControl("lblDescription")).Text;
            string errortype = ((HiddenField)Grow.FindControl("HFErrorType")).Value;
            GridView gv = btntrans.NamingContainer.NamingContainer as GridView;
            ListViewItem lvItem = (ListViewItem)gv.NamingContainer;
            int index = lvItem.DisplayIndex;
            Label lbl = siteList.Items[index].FindControl("lblSiteID") as Label;
            Label lblsitename = siteList.Items[index].FindControl("lblSiteName") as Label;
            Label lbldate = siteList.Items[index].FindControl("lblDate") as Label;
            // string ID = ((Label)gv.FindControl("lblDescription")).Text;
            //KTP do this 5:22PM 2/13/2017
            // Determine the RowIndex of the Row whose Button was clicked.
            //int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            // Get the value of column from the DataKeys using the RowIndex.
            //int id = Convert.ToInt32(siteList.DataKeys[rowIndex].Value.ToString());
            //Response.Redirect("~/ErrorCheck/ErrorList.aspx?SiteID=" + id);
            Response.Redirect("~/ErrorCheck/ErrorList.aspx?SiteID=" + lbl.Text + "&Description=" + desc + "&ErrorType=" + errortype + "&SiteName=" + lblsitename.Text + "&Date=" + lbldate.Text);
        }
        protected void ListView_ItemCommand(object sender,  ListViewCommandEventArgs e)
        {
            if (e.CommandName == "edit_id")
            {
                //Label1.Text = "Edit From Button";
            }
        }


        protected void OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            if (status)
            {

                siteList.SelectedIndex = 0;

                DataPager1.SetPageProperties(0, e.MaximumRows, false);
            }
            else
            {
                siteList.SelectedIndex = 0;

                DataPager1.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            }
            this.ListViewControlBind();
        }

        protected void ResultsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            status = true;
            siteList.SelectedIndex = 0;
            DataPager1.PageSize = Convert.ToInt32(ResultsList.SelectedValue);
        }
        protected void Rendering(object sender, EventArgs e)
        {
            DataPager1.PageSize = Convert.ToInt32(ResultsList.SelectedValue);
        }

    }
}
