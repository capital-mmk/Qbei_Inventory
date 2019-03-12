using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using QbeiAgencies_BL;
using System.Configuration;
using QbeiAgencies_DL;
using QbeiAgencies_Common;
using AjaxControlToolkit;
using DropDownListChosen;


namespace QbeiAgeincies_Web.ErrorCheck
{
    public partial class ErrorList : System.Web.UI.Page
    {
        QbeiErrorLog_BL qud = new QbeiErrorLog_BL();
        DataTable _objdt = new DataTable();
        Qbei_ErrorList qer = new Qbei_ErrorList();
        ErrorType_Entity ee = new ErrorType_Entity();
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                QbeiErrorLog_BL qud = new QbeiErrorLog_BL();
                Qbei_ErrorList qer = new Qbei_ErrorList();
                DataTable dterrorbind = new DataTable();
                DataTable dtsitename = new DataTable();
                dtsitename = qud.BindSiteName();
                ddlSiteName.DataSource = dtsitename;
                ddlSiteName.DataTextField = "SiteName";
                ddlSiteName.DataValueField = "ID";
                ddlSiteName.DataBind();
                ddlSiteName.Items.Insert(0, new ListItem("-----選択-----"));
                if (Request.QueryString.AllKeys.Contains("SiteName"))
                {
                    if (Request.QueryString["SiteName"] != null)
                    {
                        string dd = Convert.ToString(Request.QueryString["SiteName"]);
                        ddlSiteName.SelectedIndex = -1;
                        ddlSiteName.Items.FindByText(Request.QueryString["SiteName"].ToString()).Selected = true;
                    }
                }
                else
                    ddlSiteName.SelectedIndex = 0;

                //txtDateSearch.Value=Request.QueryString["Date"].ToString();
                if (Request.QueryString.AllKeys.Contains("Date"))
                {
                    if (Request.QueryString["Date"] != null)
                    {
                        txtDateSearch.Value = Request.QueryString["Date"].ToString();
                    }
                }
                else
                    txtDateSearch.Value = "";
                dterrorbind = qud.BindError();
                ddlErrorTypeSearch.DataSource = dterrorbind;
                ddlErrorTypeSearch.DataTextField = "Description";
                ddlErrorTypeSearch.DataValueField = "Type";
                ddlErrorTypeSearch.DataBind();
                ddlErrorTypeSearch.Items.Insert(0, "-----選択-----");
                if (Request.QueryString.AllKeys.Contains("Description"))
                {
                    if (Request.QueryString["Description"] != null)
                    {
                        string dd = Convert.ToString(Request.QueryString["Description"]);
                        ddlErrorTypeSearch.SelectedIndex = -1;
                        ddlErrorTypeSearch.Items.FindByText(Request.QueryString["Description"].ToString()).Selected = true;
                    }
                }
                else

                    ddlErrorTypeSearch.SelectedIndex = 0;
                if (Request.QueryString["SiteID"] != null)
                {
                    qer.SiteID = Request.QueryString["SiteID"].ToString();
                    //qer.SiteName = ddlSiteName.Items.FindByText(Request.QueryString["SiteName"].ToString()).Text;
                    ErrorListFind();
                }
                else
                {
                    Search();
                }
            }

        }

        public void Clear()
        {
            //txtSiteIDSearch.Text = "";
            //ddlErrorTypeSearch.Text = "";
            //txtDateSearch.Value = "";

        }

        protected void gvTestErrorList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTestErrorList.PageIndex = e.NewPageIndex;
            if (ddlSiteName.SelectedValue.Equals("-----選択-----") && ddlErrorTypeSearch.SelectedValue.Equals("-----選択-----") && txtDateSearch.Value.Equals(""))
            {
                _objdt = qud.Qbei_ErrorListSelectAll(ee);
            }
            else
            {

               
                if (ddlSiteName.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.SiteName = "";
                }
                else
                {
                    ee.SiteName = ddlSiteName.SelectedItem.ToString();
                }

                if (ddlErrorTypeSearch.SelectedValue.Equals("-----選択-----"))
                {
                    ee.ErrorType = 0;
                }
                else
                {
                    ee.ErrorType = Convert.ToInt32(ddlErrorTypeSearch.SelectedValue);
                }
                ee.Date = txtDateSearch.Value;
                if (string.IsNullOrWhiteSpace(txtDateSearch.Value))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch.Value;
                }

                if (ee.SiteName != "" && ee.ErrorType != 0 && ee.Date != "")
                {
                    _objdt = qud.Qbei_ErrorListEqualSearch(ee);
                }
                else
                {
                    _objdt = qud.Qbei_ErrorListSelect(ee);
                }
            }

            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);


            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }

            else
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }
        }

        protected void PageSize_Changed(object sender, EventArgs e)
        {
            gvTestErrorList.PageSize = Convert.ToInt32(ddlPageSize.Text);
            //if (Request.QueryString["SiteID"] != null)
            //{
            //    if (ddlSiteName.SelectedValue.Equals("") || ddlErrorTypeSearch.SelectedValue.Equals("-----選択-----") || txtDateSearch.Value.Equals(""))
            //    {

            //        ErrorFind();
            //    }
            //}
            //Search();

            if (ddlSiteName.SelectedItem.ToString().Equals("-----選択-----") && ddlErrorTypeSearch.SelectedItem.ToString().Equals("-----選択-----") && txtDateSearch.Value.Equals(""))
            {
                _objdt = qud.Qbei_ErrorListSelectAll(ee);
            }
            else
            {

                //ee.SiteName = ddlSiteName.SelectedItem.ToString();
                if (ddlSiteName.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.SiteName = "";
                }
                else
                {
                    ee.SiteName = ddlSiteName.SelectedItem.ToString();
                }
                if (ddlErrorTypeSearch.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.ErrorType = 0;
                }
                else
                {
                    ee.ErrorType = Convert.ToInt32(ddlErrorTypeSearch.SelectedValue);
                }
                ee.Date = txtDateSearch.Value;
                if (string.IsNullOrWhiteSpace(txtDateSearch.Value))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch.Value;
                }

                if (ee.SiteName != "" && ee.ErrorType != 0 && ee.Date != "")
                {
                    _objdt = qud.Qbei_ErrorListEqualSearch(ee);
                }
                else
                {
                    if (ee.Date != "" && ee.SiteName.Equals("") && ee.ErrorType.Equals(0))
                    {
                        _objdt = qud.Qbei_ErrorListDateSearch(ee.Date);
                    }
                    else
                    {

                        _objdt = qud.Qbei_ErrorListSelect(ee);
                    }
                }
            }


            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);


            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }

            else
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
                //Clear();
            }



        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGoToPage.Text))
            {
                gvTestErrorList.PageIndex = Convert.ToInt32(txtGoToPage.Text) - 1; //since PageIndex starts from 0 by default.
                txtGoToPage.Text = "";
                Search();
            }
            else
            {
                Clear();
                //gvTestErrorList.DataSource = _objdt;
                //gvTestErrorList.DataBind();
            }
        }

        protected void btnSiteID_Click(object sender, EventArgs e)
        {
            DataTable _objdt = new DataTable();

            QbeiErrorLog_BL qud = new QbeiErrorLog_BL();

            Qbei_ErrorList qer = new Qbei_ErrorList();
            ErrorType_Entity ee = new ErrorType_Entity();
            _objdt = qud.Qbei_ErrorListSelect(ee);
            //if (!string.IsNullOrWhiteSpace(txtSiteIDSearch.Text))
            //{

            //    qer.SiteID = txtSiteIDSearch.Text;


            //    if (_objdt.Rows.Count > 0)
            //    {
            //        gvTestErrorList.DataSource = _objdt;
            //        gvTestErrorList.DataBind();
            //    }
            //}
            //else
            //{
            gvTestErrorList.DataSource = _objdt;
            gvTestErrorList.DataBind();
            //Clear();
            //}
        }
        //protected void btnErrorType_Click(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(txtErrorTypeSearch.Text))
        //    {

        //        QbeiErrorLog_BL qud = new QbeiErrorLog_BL();
        //        DataTable _objdt = new DataTable();
        //        Qbei_ErrorList qer = new Qbei_ErrorList();
        //        qer.ErrorType = txtErrorTypeSearch.Text;
        //        _objdt = qud.Qbei_ErrorListSelect(qer);

        //        if (_objdt.Rows.Count > 0)
        //        {
        //            gvTestErrorList.DataSource = _objdt;
        //            gvTestErrorList.DataBind();
        //        }
        //    }
        //    else
        //    {
        //        Clear();
        //    }

        //}

        protected void btnDate_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {

            if (ddlSiteName.SelectedItem.ToString().Equals("-----選択-----") && ddlErrorTypeSearch.SelectedItem.ToString().Equals("-----選択-----") && txtDateSearch.Value.Equals(""))
            {
                _objdt = qud.Qbei_ErrorListSelectAll(ee);
            }
            else
            {

                //ee.SiteName = ddlSiteName.SelectedItem.ToString();
                if (ddlSiteName.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.SiteName = "";
                }
                else
                {
                    ee.SiteName = ddlSiteName.SelectedItem.ToString();
                }
                if (ddlErrorTypeSearch.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.ErrorType =0;
                }
                else
                {
                    ee.ErrorType = Convert.ToInt32(ddlErrorTypeSearch.SelectedValue);
                }
                ee.Date = txtDateSearch.Value;
                if (string.IsNullOrWhiteSpace(txtDateSearch.Value))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch.Value;
                }
             
                if (ee.SiteName != "" && ee.ErrorType != 0 && ee.Date != "")
                {
                    _objdt = qud.Qbei_ErrorListEqualSearch(ee);
                }
                else
                {
                    if (ee.Date != "" && ee.SiteName.Equals("") && ee.ErrorType.Equals(0))
                    {
                        _objdt = qud.Qbei_ErrorListDateSearch(ee.Date);
                    }
                    else
                    {

                        _objdt = qud.Qbei_ErrorListSelect(ee);
                    }
                }
            }


            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);


            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }

            else
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
                //Clear();
            }
        }

        protected void cld_Click(object sender, EventArgs e)
        {
            Calendar1.Visible = true;
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            Calendar1.Visible = false;
        }
        protected void ErrorListFind()
        {
            if (ddlSiteName.SelectedValue.Equals("-----選択-----") && ddlErrorTypeSearch.SelectedValue.Equals("-----選択-----") && txtDateSearch.Value.Equals(""))
            {
                _objdt = qud.Qbei_ErrorListSelectAll(ee);
            }
            else
            {

                ee.SiteName = Request.QueryString["SiteName"];
                string description = Request.QueryString["Description"].ToString();
                int errortype = Convert.ToInt32(Request.QueryString["ErrorType"]);
                Qbei_ErrorList qer = new Qbei_ErrorList();
                ee.Description = description;
                ee.ErrorType = Convert.ToInt32(errortype.ToString());
                ee.Date = txtDateSearch.Value;
                string date = Request.QueryString["Date"];
                if (string.IsNullOrWhiteSpace(ee.Date) || string.IsNullOrWhiteSpace(date))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch.Value;
                }

                DataTable dtddl = qud.FindError(errortype);
                ddlErrorTypeSearch.DataSource = dtddl;
                _objdt = qud.Qbei_ErrorListEqualSelect(ee);
            }
            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);
            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }
            else
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }
        }
        protected void ErrorFind()
        {

            string description = Request.QueryString["Description"].ToString();
            int errortype = Convert.ToInt32(Request.QueryString["ErrorType"]);
            QbeiErrorLog_BL qud = new QbeiErrorLog_BL();
            Qbei_ErrorList qer = new Qbei_ErrorList();
            qer.SiteName = ddlSiteName.SelectedValue;
            qer.Description = description;

            DataTable dtddl = qud.FindError(errortype);
            ddlErrorTypeSearch.DataSource = dtddl;
            DataTable _objdt = new DataTable();
            ddlErrorTypeSearch.SelectedValue = dtddl.Rows[0]["Type"].ToString(); ;
            if (ddlSiteName.SelectedValue.Equals("") || ddlErrorTypeSearch.SelectedValue.Equals("-----選択-----") || txtDateSearch.Value.Equals(""))
            {
                _objdt = qud.Qbei_ErrorListSelectAll(ee);
            }
            else
            { 
                ee.SiteName = ddlSiteName.SelectedValue;
                qer.ErrorType = ddlErrorTypeSearch.SelectedValue;
                qer.Date = txtDateSearch.Value;
                _objdt = qud.Qbei_ErrorListSelect(ee);
            }
            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);
            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }
            else
            {
                gvTestErrorList.DataSource = _objdt;
                gvTestErrorList.DataBind();
            }
        }

    }
}