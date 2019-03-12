<%@ Page Title="" Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="QbeiAgeincies_Web.DashBoard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
        <script src="../Scripts/jquery-3.2.0.min.js" type="text/javascript"></script>
    <script type="../text/javascript" src="~/Scripts/bootstrap.min.js"></script>
    <link href="~/Styles/Common.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/bootstrap.min.css" rel="stylesheet" type="text/css"/>
<style type = "text/css">
    td>a {

color:blue !important; 
padding-right:5px !important;

    }

        body {
            /*padding: 10px;*/
            font-family: 'Open Sans', sans-serif;
            font-size: 14px;
            
        }
    }

    tr.my_item td{
        background-color:white;
        border-radius:10px;
        text-align: center;
        }
    tr.my_item > td {
    background-color: white;
    border-radius: 10px;
    text-align: center;
    height: 160px;
    width: 300px;
}
    .GridPager a, .GridPager span
    {
        height: 15px;
        width: 15px;
        font-weight: bold;
        text-align: center;
        text-decoration: none;
        
    }
    .GridPager a
    {
       border-radius:2px;
       background:white ;
       /*border-radius:10px;*/
       padding:8px 8px;
       /*border:solid 1px gray;*/
       color: #2b2b2b;
    }
    .GridPager span
    {
       color: white !important;
       
      padding : 8px 5px;
       border-radius:1px;
    }
    div.gridPager > span {
        width:50px !important;
    }
    .gridPager > span {
        width:5px !important;
    }
    .GridPager
    {
      margin:0px;
      padding:0px;  
     }
    .gdstyle
    {
        border-radius:5px;
        border: 1px solid gray;
    }
    .footer {
    background-color:white;
    padding:inherit;
    }
    footer {
        background-color: white !important;
        padding: 0px 0px !important;
    }
    /*tr.my_item td {
        border:10px groove green;
    }*/
    #main > td, #siteList> td, td{
        /*width: 300px;*/
    }
</style>
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
 </asp:Content>
<%--margin:0%  0%  5%  5% !important;--%>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server" align="center" >
        <div align="center" style="width:100%;height:100%; margin-bottom:4%;margin-top:-4%;border-radius:10px;" align="center">
            	<%--<div class="panel panel-Search" style="width:100%; position:fixed;">
		        	    <div class="panel-heading" style="width:100%; height:48px;background-color:#05046b; text-align:center;padding-top:1.3%;">
		        		    <h3 class="panel-title"><span class="glyphicon glyphicon-log-in"></span> 【ダッシュボード】－トップ画面   </h3>
		        		    <span class="pull-right clickable"><i  style="top:-15px;cursor:pointer;"></i></span>
		        	    </div>
                    </div>--%>
            <div  style="width:100%; position:fixed;">
           <div class="panel panel-custom1" style="width:100%; text-align:center;" >              
       <div class="panel-heading" style="background-color:#05046b;padding-top:1.3%;"><span class="glyphicon glyphicon-log-in"></span> 【ダッシュボード】－トップ画面 </div>
           </div>
            </div>

            <div style=" margin-top:3%;border-radius:10px;" align="center">
               <div style=" height:505px !important;padding-top: 5%;overflow-x: auto; ">

                   <%--Dont put ViewStage bcoz it would facing Proble,--%>
                   <asp:ListView ID="siteList" runat="server"     OnRowCommand="ListView_ItemCommand"
                DataKeyNames="SiteID" OnPagePropertiesChanging="OnPagePropertiesChanging"
               
                   onitemdatabound="siteList_ItemDataBound" GroupItemCount="3" >
                <EmptyDataTemplate>
                   <table >
                        <tr>
                            <td>No data was returned.</td>
                        </tr>
                    </table>
                </EmptyDataTemplate>

               <EmptyItemTemplate>
                  
                </EmptyItemTemplate>

                <GroupTemplate>
                    <tr id="itemPlaceholderContainer" runat="server">
                        <td id="itemPlaceholder" runat="server"></td>
                    </tr>
                    
                </GroupTemplate>
                    
                <ItemTemplate>
                    <td id="Td1" runat="server" style="width:300px !important" align="center">
                      
                  <table  >
                            <tr class = "my_item">
                                <td>
                                  <br />
                                    <span>
                                       <b>代理店ID: </b><asp:Label runat="server" ID="lblSiteID" Text ='<%# Eval("SiteID") %>'></asp:Label><br /></b>
                                       <b>名称:</b><asp:Label runat="server" ID="lblSiteName" Text ='<%# Eval("SiteName") %>'></asp:Label><br /></b>
                                       <b>日付:</b><asp:Label runat="server" ID="lblDate" Text ='<%# Eval("Date") %>'></asp:Label><br /></b>
                                      <b>商品数:</b><asp:Label runat="server" ID="lbltotalcount" Text ='<%# Eval("TotalCount") %>'></asp:Label><br /></b>
                                     </span>                    
                                       
                                      <asp:GridView runat="server" ID="gdTest"    CellPadding="2" ForeColor="#333333" AutoGenerateColumns="False"  width="300"  GridLines="None" ShowHeader = "false" RowStyle-ForeColor="Black">
                                      <Columns>
                                      <asp:TemplateField>
                                       <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDescription" Text ='<%# Eval("Description") %>'></asp:Label>
                                         </ItemTemplate>
                                          <ItemStyle ForeColor="Black" Font-Bold="true"  />
                                         </asp:Templatefield>
                                         </Columns>
                                          
                                         <Columns>
                                          <asp:TemplateField>
                                          <ItemTemplate>
                                            <asp:LinkButton ID="lberror" runat="server" CommandName="edit_id"  OnClick="lberror_Click"     Text='<%# Eval("Error") %>'></asp:LinkButton>
                                              <%--<asp:Button runat="server" onserverclick="lberror_Click"><asp:Label runat="server" ID="lber" Text="Text='<%# Eval("Error") %>'"></asp:Label></asp:Button>--%>
                                        </ItemTemplate>
                                              <ItemStyle ForeColor="Black" Font-Bold="true"  />
                                        </asp:Templatefield>
                                        </Columns>
                                          <Columns>
                                          <asp:TemplateField>
                                            <ItemTemplate>        
                                             <asp:HiddenField ID="HFErrorType" runat="server" Value='<%# Eval("ErrorType") %>' />
                                           </ItemTemplate>
                                          </asp:TemplateField>
                                              </Columns>                                       
                                        </asp:GridView>                                    
                                    <br /> 
                                </td>
                            </tr>
                                <td>&nbsp;</td>
                            </tr>
                       </table>
                        </p>
                    </td>
                </ItemTemplate>

                <LayoutTemplate>
                    <table style="width:100%;">
                      
                            <tr>
                                <td>
                                    <table id="groupPlaceholderContainer" runat="server" style="width:100%">
                                        <tr id="groupPlaceholder"></tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr></tr>
                       
                    </table>
                   
                </LayoutTemplate>
            </asp:ListView>
            </div>
    <div class="GridPager" style="text-align:center;width:60%;height:55px; right:0px; padding-top:10px; background-color:#05046b;">
       <%-- <asp:DataPager ID="DataPager1" runat="server" PagedControlID="siteList" PageSize="9" >
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                            ShowNextPageButton="false"  PreviousPageText="<" />
                        <asp:NumericPagerField ButtonType="Link"/>
                        <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false" ShowPreviousPageButton = "false"  NextPageText=">" />
                    </Fields>
                   
       </asp:DataPager>--%>
        <asp:DataPager ID="DataPager1" runat="server"  PagedControlID="siteList" OnPreRender="Rendering"  >
    <Fields>
        <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" />
        <asp:NumericPagerField />
        <asp:NextPreviousPagerField ShowLastPageButton="True" ShowPreviousPageButton="False" />
        
     </Fields>
</asp:DataPager>
        <asp:Label id="ResultsLabel" runat="server" 
              AssociatedControlID="ResultsList" Text="Results per page:" ForeColor="White" />            
            <asp:DropDownList runat="server" id="ResultsList"  style="width: 50px;height: 30px;border-radius: 5px;"  
              OnSelectedIndexChanged="ResultsList_SelectedIndexChanged" AutoPostBack="true" >              
              <%--<asp:ListItem Value="3"  />
                  <asp:ListItem Value="4"  />
                <asp:ListItem Value="5"  />--%>
              
              <asp:ListItem Value="6" />
                  <asp:ListItem Value="7"  />
                  <asp:ListItem Value="8"  />
              <asp:ListItem Value="9" />
                <asp:ListItem Value="10" />
                  <asp:ListItem Value="12" />
            </asp:DropDownList>
    </div>
                <div>
                    
                </div>
 </div>
        </div>

        

</asp:Content>
