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

namespace QbeiAgeincies_Web.ErrorCheck
{
    public partial class ErrorList_Backup : System.Web.UI.Page
    {
        Qbei_ErrorList qer = new Qbei_ErrorList();
        ErrorType_Entity ee = new ErrorType_Entity();
        DataTable _objdt = new DataTable();
        ErrorlogBackup_Entity ebe = new ErrorlogBackup_Entity();
        QbeiErrorLogBackup_BL qud = new QbeiErrorLogBackup_BL();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                {

                    QbeiErrorLogBackup_BL qud = new QbeiErrorLogBackup_BL();
                    Qbei_ErrorList qer = new Qbei_ErrorList();
                    DataTable dterrorbind = new DataTable();
                    txtDateSearch2.Value = DateTime.Today.ToString("yyyy-MM-dd");
                    dterrorbind = qud.BindSiteName();
                    ddlSiteName.DataSource = dterrorbind;
                    ddlSiteName.DataTextField = "SiteName";
                    ddlSiteName.DataValueField = "ID";
                    ddlSiteName.DataBind();

                    ddlSiteName.Items.Insert(0, new ListItem("-----選択-----", ""));
                    ddlSiteName.SelectedIndex = 0;
                    dterrorbind = qud.BindError();
                    ddlErrorType.DataSource = dterrorbind;
                    ddlErrorType.DataTextField = "Description";
                    ddlErrorType.DataValueField = "Type";
                    ddlErrorType.DataBind();
                    //ddlErrorType.Items.Insert(0, new ListItem("", " "));
                    ddlErrorType.Items.Insert(0, new ListItem("-----選択-----", ""));
                    ddlErrorType.SelectedIndex = 0;
                    if (Request.QueryString["SiteID"] != null)
                    {
                        //qer.SiteID = Request.QueryString["SiteID"].ToString();
                        ErrorFind();
                    }
                    else
                    {
                        //_objdt = qud.ErrorLogBackup_SelectAll(ebe);
                        _objdt = qud.QbeiErrorLogBackup_DateSearch(txtDateSearch2.Value);
                        lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);
                        if (_objdt.Rows.Count > 0)
                        {
                            gvTestErrorList2.DataSource = _objdt;
                            gvTestErrorList2.DataBind();

                        }
                        else
                        {
                            gvTestErrorList2.DataSource = _objdt;
                            gvTestErrorList2.DataBind();
                        }
                    }
                    Search();

                }
            }
        }

        protected void ErrorFind()
        {
            int siteName = Convert.ToInt32(Request.QueryString["SiteName"]);
            string description = Request.QueryString["Description"].ToString();
            int errortype = Convert.ToInt32(Request.QueryString["ErrorType"]);

            Qbei_ErrorList qer = new Qbei_ErrorList();
            qer.SiteID = siteName.ToString();
            qer.Description = description;

            DataTable dtddl = qud.FindError(errortype);
            ddlSiteName.DataSource = dtddl;
            ddlSiteName.SelectedValue = dtddl.Rows[0]["SiteID"].ToString();

            DataTable dt = qud.GetErrorSiteList(qer);
            lblrowCount.Text = Convert.ToString(dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                gvTestErrorList2.DataSource = dt;
                gvTestErrorList2.DataBind();
            }
            else
            {
                gvTestErrorList2.DataSource = dt;
                gvTestErrorList2.DataBind();
            }
        }


        public void Clear()
        {
            //// txtSiteIDSearch2.Text = "";
            //// txtErrorTypeSearch2.Text = "";
            //gvTestErrorList2.EmptyDataText = "";
            //txtDateSearch2.Text = "";
        }

        protected void gvTestErrorList2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTestErrorList2.PageIndex = e.NewPageIndex;
            Search();

            //Bind();
            //_objdt = qud.ErrorLogBackup_SelectAll(ebe);
            //_objdt = qud.QbeiErrorLogBackup_DateSearch(txtDateSearch2.Value);
            //if (_objdt.Rows.Count > 0)
            //{
            //    gvTestErrorList2.DataSource = _objdt;
            //    gvTestErrorList2.DataBind();

            //}
            //else
            //{
            //    gvTestErrorList2.DataSource = _objdt;
            //    gvTestErrorList2.DataBind();
            //}
        }

        protected void PageSize2_Changed(object sender, EventArgs e)
        {
            gvTestErrorList2.PageSize = Convert.ToInt32(ddlPageSize2.Text);
            //if (Request.QueryString["SiteID"] != null)
            //{
            //    if (ddlSiteName.SelectedValue.Equals("") || ddlErrorType.SelectedValue.Equals("-----選択-----") || txtDateSearch2.Value.Equals(""))
            //    {

            //        ErrorFind();
            //    }
            //}
            //Search();

            QbeiErrorLogBackup_BL qud = new QbeiErrorLogBackup_BL();



            if (ddlSiteName.SelectedItem.Text.Equals("-----選択-----") && ddlErrorType.SelectedValue.Equals("-----選択-----") && txtDateSearch2.Value.Equals(""))
            {
                _objdt = qud.ErrorLogBackup_SelectAll(ebe);
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
                if (ddlErrorType.SelectedItem.ToString().Equals("-----選択-----"))
                {
                    ee.ErrorType = 0;
                }
                else
                {
                    ee.ErrorType = Convert.ToInt32(ddlErrorType.SelectedValue);
                }
                ee.Date = txtDateSearch2.Value;
                if (string.IsNullOrWhiteSpace(txtDateSearch2.Value))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch2.Value;
                }
                //_objdt = qud.Qbei_ErrorListSelect(ee);
                //if (Request.QueryString["SiteName"] != null && Request.QueryString["Description"] != "-----選択-----" && Request.QueryString["Date"] != null)
                if (ee.SiteName != "" && ee.ErrorType != 0 && ee.Date != "")
                {
                    _objdt = qud.Qbei_ErrorBackUpEqualSelect(ee);
                }
                else
                {
                    if (ee.Date != "" && ee.SiteName.Equals("") && ee.ErrorType.Equals(0))
                    {
                        _objdt = qud.QbeiErrorLogBackup_DateSearch(ee.Date);
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
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();
            }
            //}
            else
            {
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();
            }
            //Response.Redirect("~/ErrorCheck/ErrorList.aspx?");

        }

        protected void btnGo2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtGoToPage2.Text))
            {
                gvTestErrorList2.PageIndex = Convert.ToInt32(txtGoToPage2.Text) - 1; //since PageIndex starts from 0 by default.
                txtGoToPage2.Text = "";
                Search();
                //Bind();
            }
            else
            {
                Clear();
            }
        }
        protected void TextBoxStartDate_TextChanged(object sender, EventArgs e)
        {
            //txtDateSearch2.Text = imgPopup.ToString();
        }


        protected void btnDate2_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {


            QbeiErrorLogBackup_BL qud = new QbeiErrorLogBackup_BL();


            
            if (ddlSiteName.SelectedItem.Text.Equals("-----選択-----") && ddlErrorType.SelectedValue.Equals("-----選択-----") && txtDateSearch2.Value.Equals(""))
            {
                _objdt = qud.ErrorLogBackup_SelectAll(ebe);
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
                if (ddlErrorType.SelectedItem.ToString().Equals("-----選択-----")) 
                {
                    ee.ErrorType = 0;
                }
                else
                {
                    ee.ErrorType = Convert.ToInt32(ddlErrorType.SelectedValue);
                }
                ee.Date = txtDateSearch2.Value;
                if (string.IsNullOrWhiteSpace(txtDateSearch2.Value))
                {
                    ee.Date = "";
                }
                else
                {
                    ee.Date = txtDateSearch2.Value;
                }
                //_objdt = qud.Qbei_ErrorListSelect(ee);
                //if (Request.QueryString["SiteName"] != null && Request.QueryString["Description"] != "-----選択-----" && Request.QueryString["Date"] != null)
                if (ee.SiteName != "" && ee.ErrorType != 0 && ee.Date != "")
                {
                    _objdt = qud.Qbei_ErrorBackUpEqualSelect(ee);
                }
                else
                {
                    if (ee.Date != "" && ee.SiteName.Equals("") && ee.ErrorType.Equals(0))
                    {
                        _objdt = qud.QbeiErrorLogBackup_DateSearch(ee.Date);
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
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();
            }
            //}
            else
            {
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();
            }
            //Response.Redirect("~/ErrorCheck/ErrorList.aspx?");
        }
        private void Bind()
        {


            QbeiErrorLogBackup_BL qud = new QbeiErrorLogBackup_BL();
            DataTable _objdt = new DataTable();
            Qbei_ErrorList qer = new Qbei_ErrorList();
            ErrorType_Entity ee = new ErrorType_Entity();
            qer.SiteName = ddlSiteName.SelectedItem.Text;
            //qer.SiteID = ddlSiteName.SelectedItem.Value;
            qer.ErrorType = ddlErrorType.SelectedValue;
            DataTable dt = new DataTable();
            dt = qud.GetDate(qer);
            txtDateSearch2.Value = dt.Rows[0]["Column1"].ToString();
            txtDateSearch2.Value = txtDateSearch2.Value.Replace("12:00:00 AM", string.Empty);
            txtDateSearch2.Value = txtDateSearch2.Value.Replace("0:00:00", string.Empty);
            qer.Date = txtDateSearch2.Value;
            //qer.Date = qer.Date.Replace("12:00:00 AM", string.Empty);
            _objdt = qud.Qbei_ErrorList(ee);
            lblrowCount.Text = Convert.ToString(_objdt.Rows.Count);

            if (_objdt.Rows.Count > 0)
            {
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();

            }
            //}
            else
            {
                gvTestErrorList2.DataSource = _objdt;
                gvTestErrorList2.DataBind();
                // Clear();
            }
        }

        protected void btnDdl_Click(object sender, EventArgs e)
        {
            ddlTest.Visible = true;
        }


        protected void ddlTest_Changed(object sender, EventArgs e)
        {
            ddlTest.Visible = false;
        }
    }
}