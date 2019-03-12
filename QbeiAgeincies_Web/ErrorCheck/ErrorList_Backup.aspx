<%@ Page Title="" Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="ErrorList_Backup.aspx.cs" Inherits="QbeiAgeincies_Web.ErrorCheck.ErrorList_Backup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
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
        </style>
    <style type="text/css">

       .table > tbody > tr > td, .table > tbody > tr > th, .table > tfoot > tr > td, .table > tfoot > tr > th, .table > thead > tr > td, .table > thead > tr > th {
            background-color:white;
        }
    .ddl111
        {
            box-shadow: 10px 10px 5px #888888;
            border-radius:5px;
            border:1px solid gray;
        }
    </style>
</asp:Content>
  
<asp:Content ID="Content4" ContentPlaceHolderID="Main" runat="server">
    <div align="center" >
         <div style="margin-top: -12px !important;">
    <div class="panel panel-custom1">
        <div class="panel-heading"><span class="fas fa-walking"></span>エラーバックアップ</div>
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
                            <div class="panel-body" style="padding: 10px 0px 0px 0px; margin-left:10px">
                                <table style="height: 60px; width: 1200px;">
                                    <tr>
                                        <td>
                                             代理店
                                        </td>
                                        <td>
                                             <asp:DropDownListChosen ID="ddlSiteName" runat="server"  AllowSingleDeselect="false" Width="170px"   CssClass="form-control" NoResultsText ="No results match.">
                                                </asp:DropDownListChosen>
                                        </td>
                                        <td>
                                             エラータイプ
                                        </td>
                                        <td>
                                         
                                            <asp:DropDownListChosen ID="ddlErrorType" runat="server"  Width="170px"  CssClass="form-control" NoResultsText ="No results match."
                                              AllowSingleDeselect="false" >                
                                            </asp:DropDownListChosen>
                                        </td>
                                        <td style ="padding-left:8px;text-align:right;padding-right:10px" class="auto-style1" >日付 </td>
                                 <td class="auto-style1">
                                
                                   <div runat="server" id="div1" class="input-group date form_date col-md-1" data-date="" data-date-format="yyyy mm dd" data-link-field="dtp_input2" data-link-format="yyyy-mm-dd">
                                  <input id="txtDateSearch2" runat="server"  class="form-control calendartxt"  onkeypress="return false"  style="height:30px; width: 120px;cursor:pointer" type="text" placeholder="yyyy/mm/dd" value=""   readonly="readonly" />
            		             <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                  </div>
                                    </td>
                                          <td colspan="2"> <button id="btnDate2" type="button" class="btn btn-primary " runat="server" onserverclick="btnDate2_Click"  >
                                        <span class="glyphicon glyphicon-search" aria-hidden="true" style="vertical-align:middle; "></span>&nbsp;&nbsp; 
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
<div style ="padding-top: 35px;width: 96%;margin-bottom:5%;padding-bottom:18%;">
<asp:Gridview ID="gvTestErrorList2"  runat="server" CssClass="table table-striped table-bordered table-hover" 
       PagerStyle-CssClass="pagination" HeaderStyle-CssClass="header" 
        RowStyle-CssClass="rows" AutoGenerateColumns="False" 
        PageSize="10" AllowPaging ="true"
        onpageindexchanging="gvTestErrorList2_PageIndexChanging" 
        PagerSettings-Mode="NumericFirstLast"   FirstPageText="First" 
        LastPageText="Last" Width="100%" RowStyle-BackColor="white" style="margin-top: -50px;" HeaderStyle-BorderColor="#58ACFA" ShowHeaderWhenEmpty="true" >
        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
        <AlternatingRowStyle   BackColor="#DAEFA1" />
        <HeaderStyle BackColor="#58ACFA" Font-Bold="True" ForeColor="#FFFFCC" />
        <Columns>

         <asp:TemplateField>
                <HeaderTemplate>ID</HeaderTemplate>
             <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" width="100px" ID="lblID"   visible="true" Text='<%# Bind("ID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

         <asp:TemplateField>
            <HeaderTemplate>代理店</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblSiteID" Text='<%# Bind("SiteName") %>'></asp:Label>
                </ItemTemplate>

            </asp:TemplateField>
        
         <asp:TemplateField>
          <HeaderTemplate>JANコード</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
          <ItemTemplate>
            <asp:Label runat="server" ID="JanCode" Text='<%# Bind("JanCode") %>'></asp:Label>
          </ItemTemplate>
          </asp:TemplateField>

         <asp:TemplateField>
                <HeaderTemplate>発注コード</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="OrderCode" Text='<%# Bind("OrderCode") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField> 

 <%--        <asp:TemplateField>
                <HeaderTemplate>エラータイプ</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="ErrorType" Text='<%# Bind("ErrorType") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>--%>

         <asp:TemplateField>
                <HeaderTemplate>エラータイプ</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="Description" Text='<%# Bind("Description") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

         <asp:TemplateField>
                <HeaderTemplate>日付</HeaderTemplate>
              <HeaderStyle  HorizontalAlign="Center" BackColor="#58ACFA" CssClass="text-center"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="Date" Text='<%# Bind("Date","{0:yyyy/MM/dd}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

    <HeaderStyle CssClass="header"></HeaderStyle>

<PagerStyle CssClass="GridPager"></PagerStyle>

<RowStyle CssClass="rows"></RowStyle>
   </asp:Gridview>
         <div style="margin-top:-20px"  align="center" > 


          <%--<footer class="navbar-fixed-bottom">--%>
    <%--<div style=" BackColor:#58ACFA ForeColor:#ffffff border-radius:1px; height:40px">--%>
    <%--<div style=" margin: 0px 100px; vertical-align:middle; line-height:40px">--%>
        <div class="panel-footer"  style="border:1px solid #ddd">
                    <div class="row" style="border:none;" >
                         
                              <div class="pull-left">
<asp:Label ID="lblGoToPage2" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Go To Page : " ForeColor="black"></asp:Label>
<%--<asp:TextBox ID="txtGoToPage2" runat="server" BackColor="#FFE4E1" Width="47px" height="30px" onkeypress="return allowOnlyNumber(event);"></asp:TextBox>--%>
<asp:TextBox ID="txtGoToPage2" runat="server"  width="40px" Height="25px"   onkeypress="return allowOnlyNumber(event);"></asp:TextBox>
<%--<asp:Button ID="btnGo2" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Go" OnClick="btnGo2_Click"  BackColor="#FFA07A"/>--%>
        <button id="btnGo2" type="button" class="btn btn-primary" runat="server" style="color:black" onserverclick="btnGo2_Click"   >
                                        <span class="glyphicon glyphicon-hand-right" aria-hidden="true"></span></button></div>
        <div class="pull-right">
               <asp:Label ID="Label2" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Total_Count : " ForeColor="black"></asp:Label>
        <asp:Label runat="server" id="lblrowCount" ForeColor="black"/> 
    <asp:Label ID="Label1" runat="server" Font-Bold="false"
        CssClass="StrongText" Text="Page Size : " ForeColor="black"></asp:Label>
<asp:DropDownList ID="ddlPageSize2" runat="server" width="60px" Height="25px" AutoPostBack="true" OnSelectedIndexChanged="PageSize2_Changed" >
    <asp:ListItem Text="10" Value="10" />
    <asp:ListItem Text="30" Value="30" />
    <asp:ListItem Text="50" Value="50" />
    <asp:ListItem Text="100" Value="100"/>
</asp:DropDownList>


        </div>
</div>
            </div>
                        <%--</div>--%>
               <%--</div>--%>
              <%--</footer>--%>
          </div>

    </div>
    </div>
    </div>
    
 
    
        <div>
        <asp:DropDownList ID="ddlTest" onclick="this.size=1;" 
        onMouseOver="this.size=3;" onMouseOut="this.size=1;" 
        style="position:absolute; right:684px; top: 669px; width: 156px;" runat="server" Visible="false" 
        AutoPostBack="true" OnSelectedIndexChanged="ddlTest_Changed" >
             <asp:ListItem>TextTextText1</asp:ListItem>
             <asp:ListItem>TextTextText2</asp:ListItem>
             <asp:ListItem>TextTextText3</asp:ListItem>
             <asp:ListItem>TextTextText4</asp:ListItem>
             <asp:ListItem>TextTextText5</asp:ListItem>
             <asp:ListItem>TextTextText6</asp:ListItem>
             <asp:ListItem>TextTextText7</asp:ListItem>   
             <asp:ListItem>TextTextText8</asp:ListItem>
             <asp:ListItem>TextTextText9</asp:ListItem>
             <asp:ListItem>TextTextText10</asp:ListItem>
             <asp:ListItem>TextTextText11</asp:ListItem>    
        </asp:DropDownList>

    

<asp:Label ID="list" runat="server"  CssClass="hidden" BackColor="#FFE4E1">
<nav>
  <ul style="text-decoration:none; list-style-type:none;">
      <li>Link 1</li>
      <li>Link 2</li>
      <li>Link 3</li>
      <li>Link 4</li>
      <li>Link 5</li>
      <li>Link 6</li> 
      <li>Link 7</li> 
      <li>Link 8</li>
      <li>Link 9</li>
      <li>Link 10</li>
      <li>Link 11</li>
      <li>Link 12</li>
      <li>Link 13</li>

  </ul>
  </nav>  
  </asp:Label>

        <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/bootstrap-datetimepicker.js") %>"></script>
        <link rel="Stylesheet" href="<%= Page.ResolveClientUrl("~/css/bootstrap-datetimepicker.min.css") %>"/>

<%--<link href="http://localhost:2492/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <script src="http://localhost:2492/Scripts/bootstrap-datetimepicker.js" type="text/javascript"></script>--%>
    <script type="text/javascript">

        $('.form_date').datetimepicker(
           //alert("Hi Script"),
            {

                language: 'es',
                format: 'yyyy-mm-dd',
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