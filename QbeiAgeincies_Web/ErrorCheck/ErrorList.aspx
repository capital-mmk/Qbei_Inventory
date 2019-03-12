<%@  Page Title="" Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="ErrorList.aspx.cs" Inherits="QbeiAgeincies_Web.ErrorCheck.ErrorList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="http://localhost:2492/css/iconMoon.css" rel="stylesheet" />
  
    <style type="text/css">
        .panel-custom1 {
    border-color: #395587;
    margin-bottom: 0px;
}  .panel-custom1 > .panel-heading {
    color: white;
    background-color: #395587;
    border-color: #395587;
    text-align: center;
    font-size: 16px;
}
        .panel-Search > .panel-heading {
    color: white;
    background-color: #395587;
    border-color: #395587;
    text-align: center;
    font-size: 16px;
}
        tr.GridPager {
            background-color:white !important;
        }
    .ddl11
        {
            /*box-shadow: 10px 10px 5px #888888;
            border-radius:5px;
            border:1px solid gray;*/
        }
    </style>
      <script type="text/javascript">
          $(document).on('click', '.panel-heading span.clickable', function (e) {
              var $this = $(this);
              if (!$this.hasClass('panel-collapsed')) {
                  $this.parents('.panel').find('.SearchPanel').slideUp();
                  $this.addClass('panel-collapsed');
                  $this.find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
              } else {
                  $this.parents('.panel').find('.SearchPanel').slideDown();
                  $this.removeClass('panel-collapsed');
                  $this.find('i').removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
              }
          })
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server"> 
 <div align="center" style="padding-bottom:18%;">
    <div style="margin-top: -12px !important;">
    <div class="panel panel-custom1">
        <div class="panel-heading"><span class="fas fa-walking"></span> エラーログ</div>
            </div>
    <%--start panel--%>
     <div>
		        <div>
		        	<div class="panel panel-Search" style="margin:28px;border:1px solid grey">
		        	    <div class="panel-heading">
		        		    <h3 class="panel-title"><span class="glyphicon glyphicon-search"></span>  検索条件</h3>
		        		    <span class="pull-right clickable"><i class="glyphicon glyphicon-chevron-up" style="top:-15px;cursor:pointer;"></i></span>
		        	    </div>
		        	    <div class="SearchPanel">
                            <div class="panel-body" style="padding: 10px 0px 0px 0px; margin-left:10px;">
                                <table style="height: 60px; width: 1200px;">
                                   <tr>
                                        <td>
                                             代理店
                                        </td>
                                        <td>
                                             <asp:DropDownListChosen ID="ddlSiteName" runat="server"  AllowSingleDeselect="true" Width="170px"   CssClass="form-control" NoResultsText ="No results match.">
   </asp:DropDownListChosen>
                                        </td>
                                        <td>
                                             エラータイプ
                                        </td>
                                        <td>
                                            <asp:DropDownListChosen ID="ddlErrorTypeSearch" runat="server"  Width="170px"   CssClass="form-control" NoResultsText ="No results match."
                                              AllowSingleDeselect="true" >                
                                            </asp:DropDownListChosen>
                                              <%-- <asp:DropDownListChosen ID="ddlErrorTypeSearch" runat="server"   CssClass="form-control ddl11">
                                                    </asp:DropDownListChosen>--%>
                                        </td>
                                        <td style ="padding-left:8px;text-align:right;padding-right:10px" class="auto-style1" >日付 </td>
                                 <td class="auto-style1">
                                
                                   <div runat="server" id="div1" class="input-group date form_date col-md-1" data-date="" data-date-format="yyyy mm dd" data-link-field="dtp_input2" data-link-format="yyyy-mm-dd">
                                  <input id="txtDateSearch" runat="server"  class="form-control calendartxt"  style="height:30px; width: 120px;cursor:pointer" type="text" placeholder="yyyy/mm/dd" value=""   readonly="readonly" />
					             <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                </div>                        
                                    </td>
                                          <td colspan="2"> <button id="btnDate" type="button" class="btn btn-primary" runat="server" onserverclick="btnDate_Click" style="background:#58ACFA">
                                        <span class="glyphicon glyphicon-search" aria-hidden="true" style="background:#58ACFA"></span>&nbsp;&nbsp; 
                                    検索
                            </button></td> 
                                    </tr>
                                </table>
                            </div>
		        	    </div>
		        	</div>
		        </div>
            </div>
        </div>
<div style ="padding-top:36px;width: 96%; margin-bottom:5%;" align="center">
<asp:Gridview ID="gvTestErrorList" runat="server" CssClass="table table-striped table-bordered table-hover"  ShowFooter="true"
    style="margin-top: -50px;" ShowHeaderWhenEmpty="true"
        PagerStyle-CssClass="pagination" HeaderStyle-CssClass="header" 
        RowStyle-CssClass="rows" AutoGenerateColumns="False" AllowPaging ="True"
        onpageindexchanging="gvTestErrorList_PageIndexChanging" 
        PagerSettings-Mode="NumericFirstLast"   FirstPageText="First" 
        LastPageText="Last" Width="100%" RowStyle-BackColor="white" >

<HeaderStyle CssClass="header"></HeaderStyle>

        <AlternatingRowStyle    />
        
        <Columns>

         <asp:TemplateField>
                <HeaderTemplate>ID</HeaderTemplate >
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" width="100px" ID="lblID"   Text='<%# Bind("ID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

         <asp:TemplateField>
            <HeaderTemplate>代理店ID</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblSiteID" Text='<%# Bind("SiteID") %>'></asp:Label>
                </ItemTemplate>

            </asp:TemplateField>
        
         <asp:TemplateField>
          <HeaderTemplate>JANコード</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
          <ItemTemplate>
            <asp:Label runat="server" ID="JanCode" Text='<%# Bind("JanCode") %>'></asp:Label>
          </ItemTemplate>
          </asp:TemplateField>

         <asp:TemplateField>
                <HeaderTemplate>発注コード</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="OrderCode" Text='<%# Bind("OrderCode") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField> 

      <%--   <asp:TemplateField>
                <HeaderTemplate>エラータイプ</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="ErrorType" Text='<%# Bind("Type") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>--%>

         <asp:TemplateField>
                <HeaderTemplate>エラータイプ</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="Description" Text='<%# Bind("Description") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

         <asp:TemplateField>
                <HeaderTemplate>日付</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="Date" Text='<%# Bind("Date","{0:yyyy/MM/dd}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
        <HeaderStyle  Font-Bold="True"  />
        
<PagerSettings Mode="NumericFirstLast"></PagerSettings>

<PagerStyle CssClass="GridPager" BackColor="White"></PagerStyle>

<RowStyle CssClass="rows"></RowStyle>
  
   
    </asp:Gridview>
        <div align="center" style="margin-top:-20px" >
       <div  >
<div class="panel-footer"  style="border:1px solid #ddd">
                    <div class="row" style="border:none;" >
                          
                              <div class="pull-left">
<asp:Label ID="lblGoToPage" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Go To Page : " ForeColor="black"></asp:Label>

<asp:TextBox ID="txtGoToPage" width="40px" Height="25px" runat="server"  onkeypress="return allowOnlyNumber(event);"></asp:TextBox>

         <button id="btnGo" type="button" class="btn btn-primary" runat="server" onserverclick="btnGo_Click" >
                                        <span class="glyphicon glyphicon-hand-right" aria-hidden="true"></span></button></div>
                            <div class="pull-right">
                                 <asp:Label ID="Label2" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Tota_Count : " ForeColor="black"></asp:Label>
        <asp:Label runat="server" id="lblrowCount" ForeColor="black"/>  
   <asp:Label ID="Label1" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Page Size : " ForeColor="black"></asp:Label>
<asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" Height="25px"    OnSelectedIndexChanged="PageSize_Changed">
    <asp:ListItem Text="10" Value="10" />
    <asp:ListItem Text="30" Value="30" />
    <asp:ListItem Text="50" Value="50" />
    <asp:ListItem Text="100" Value="100"/>
</asp:DropDownList>
       
                                </div>
   <asp:Button Visible="false" ID="cld" runat="server" Text="Calendar" OnClick="cld_Click" BackColor="#FFA07A"/>
  <asp:Calendar ID="Calendar1" runat="server"  onselectionchanged="Calendar1_SelectionChanged" Visible="False" > </asp:Calendar>
                            </div>
        </div>
           
                        </div>
      </div>
    </div>
    </div>
  <%--<script type="text/javascript" src="https://www.malot.fr/bootstrap-datetimepicker/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js?t=20130302"></script>
    <link href="https://www.malot.fr/bootstrap-datetimepicker/bootstrap-datetimepicker/css/bootstrap-datetimepicker.css" rel="stylesheet" type="text/css" />--%>
 <%--   <link href="http://localhost:2492/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <script src="http://localhost:2492/Scripts/bootstrap-datetimepicker.js" type="text/javascript"></script>--%>
<%--  <link href="http://localhost:12616/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
      <script src="http://localhost:12616/Scripts/bootstrap-datetimepicker.js"></script>--%>
      <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/bootstrap-datetimepicker.js") %>"></script>
        <link rel="Stylesheet" href="<%= Page.ResolveClientUrl("~/css/bootstrap-datetimepicker.min.css") %>"/>


    <script type="text/javascript">
        
         $('.form_date').datetimepicker(
            //alert("Hi Script"),
             {
           
             language: 'es',
             format: 'yyyy-mm-dd',
             clearBtn: 1,
             autoclose: 1,
             weekStart: 1,
             startView: 2,
             todayBtn: 1,
             forceParse: 0,
             minView: 2,
             pickerPosition: "bottom-right"

         });
         </script>
         <script type="text/javascript" >
         
    function allowOnlyNumber(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
</script>
</asp:Content>