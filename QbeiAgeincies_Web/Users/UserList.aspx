<%@ Page Title="" Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="QbeiAgeincies_Web.Users.UserList" EnableEventValidation ="false"  %>


<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
    <script src="../Scripts/jquery-3.2.0.min.js" type="text/javascript"></script>
    <script type="../text/javascript" src="~/Scripts/bootstrap.min.js"></script>
    <link href="~/Styles/Common.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/bootstrap.min.css" rel="stylesheet" type="text/css"/>

    <style type="text/css">
        .edit {
        width: 60px;

height: 30px;


line-height: 30px;

vertical-align: middle;

padding-top: 5px;

background-color: #e40765;
        }
    body
    {
        height:9.0%
    }
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

        .wrench
        {
            text-align: center !important;
            
        }

        .ddl11
        {
            /*box-shadow: 10px 10px 5px #888888;*/
            border-radius:5px;
            border:1px solid gray;
        }
        tr.ul11
        {
            background-color:#99BEC7;
            color:red;
        }
        tr.usd:hover
        {
            background-color:#72A6B3;
            color:white;
        }
        .table tr.normal
        {
            color: black;
   background-color: #FDC64E;
   height: 25px;
   vertical-align: middle;
   text-align: center;
        }
        .ula
        {
            background-color:#6FA3B1;
            color:white;
        }
        .del11
        {
            text-align:center;
            flex-item-align:center;
            
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
    <div   align="center">
 <%--   <div class="row">
                        <div style="text-align:center">
                            <h1 ><b style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:25px;">ユーザー一覧</b></h1>
                        </div>
                    </div>--%>
         <div style="margin-top: -12px !important;">
    <div class="panel panel-custom1" style="width:100%;">
        <div class="panel-heading" style="width:100%;"><span class="fas fa-walking"></span>   ユーザー一覧</div>
                    </div>


    <%--start panel--%>
     <div >
		        <div style="margin:28px;margin-bottom:10px">
		        	<div class="panel panel-Search" style="margin-bottom:-30px;border:1px solid grey; width:75%;">
		        	    <div class="panel-heading">
		        		    <h3 class="panel-title"><span class="glyphicon glyphicon-search"></span> 検索条件</h3>
		        		    <span class="pull-right clickable"><i class="glyphicon glyphicon-chevron-up" style="top:-15px;cursor:pointer;"></i></span>
		        	    </div>
		        	    <div class="SearchPanel" >
                            <div class="panel-body" style="padding: 10px 0px 0px 0px; margin-left:10px; ">
                                <table style="height: 60px; width: 800px;">
                                    <tr>
                                        <td>
                                                    ユーザーID
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSearchUserID" Width="130px" Height="25px" runat="server" CssClass="form-control" ></asp:TextBox>
                                        </td>
                                        <td>
                                             ユーザーレベル

                                        </td>
                                        <td>
                                            <asp:DropDownListChosen ID="ddluserlevel" runat="server"  role="menu" width="150px" Height="25px"  CssClass="form-control"  >
   <asp:ListItem Value="-1">-----選択-----</asp:ListItem>
   <asp:ListItem Value="0">管理者</asp:ListItem>
   <asp:ListItem Value="1">ユーザー</asp:ListItem>
    </asp:DropDownListChosen>
                                              <%-- <asp:DropDownListChosen ID="ddlErrorTypeSearch" runat="server"   CssClass="form-control ddl11">
                                                    </asp:DropDownListChosen>--%>
                                        </td>
                                          <td colspan="2"><button ID="lblSearch" type="button" class="btn btn-primary" runat="server" onserverclick="btnSearch_Click"  >
                               <span class="glyphicon glyphicon-search"  aria-hidden="true"  style="vertical-align:middle" BackColor="#008080"></span>&nbsp;&nbsp; 
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
<div class="form-group col-xs-12"  style="height:50%; padding-top:20px;"style="vertical-align:middle">
<%--<div class = "search" style="width: 96%; padding-top: 25px; height: 67px;margin-top:-5px;">
</div>--%>
</div>
<div  style="width:80%;padding-top:0px;padding-bottom:100px;"  align="center" magin-top:10px;>
    <asp:GridView runat="server" ID="gvTest" AutoGenerateColumns="False" 
        OnRowDataBound="gv_userlevel_rowdatabound" AllowPaging="True" 
      PagerSettings-Mode="NumericFirstLast"   FirstPageText="First" 
        style="padding-top:-30px"
        LastPageText="Last"    CssClass="table table-striped table-bordered table-hover usd"  onpageindexchanging="gvTest_PageIndexChanging"  RowStyle-CssClass="rsc1" Width="90%" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" >
        <PagerStyle CssClass="" BackColor="#FFFFCC" ForeColor="#330099" />
        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
        <AlternatingRowStyle   BackColor="#DAEFA1" />
        <HeaderStyle BackColor="#008000" Font-Bold="True" />
       
        <Columns>
         <asp:TemplateField>
                <HeaderTemplate >ID</HeaderTemplate>
             <HeaderStyle CssClass="text-center"  BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                  <asp:Label runat="server" ID="lblid" Text='<%# Bind("ID") %>'></asp:Label>

                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate >ユーザーID</HeaderTemplate>
                <HeaderStyle CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lbluserid" Text='<%# Bind("UserID") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate >ユーザーレベル</HeaderTemplate>
                <HeaderStyle CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbluserlevel" Text='<%# Bind("UserLevel") %>'></asp:Label>
                    </ItemTemplate>
                      <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
              
            <asp:TemplateField >
                <HeaderTemplate><span class="glyphicon glyphicon-wrench" aria-hidden="true"></span></HeaderTemplate>
                 <HeaderStyle CssClass="col-md-2 text-center"  HorizontalAlign="Center"  BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemStyle CssClass="col-md-2 text-center" HorizontalAlign="Center" width="100"/>
                <ItemTemplate>
                    <a id="btnEdit1" class="btn btn-info btn-xs edit" runat="server" onserverclick="btnEdit1_Click"><span class="glyphicon glyphicon-edit" aria-hidden="true" style="width:41px;"> 編集</span></a>
                </ItemTemplate>
            </asp:TemplateField>

           
             <asp:TemplateField>
                <HeaderTemplate><span class="glyphicon glyphicon-wrench" aria-hidden="true"></span></HeaderTemplate>
                 <HeaderStyle CssClass="text-center" HorizontalAlign="Center"  BackColor="#58ACFA" ForeColor="#ffffff"/>
                 
                 <ItemStyle CssClass="col-md-2 text-center" HorizontalAlign="Center" />
                <ItemTemplate  >
                    <a id ="btnDelete1" class="btn btn-info btn-xs edit" runat="server" onserverclick="btnDelete1_Click" onclick="return confirm('削除してもよろしいでしょうか?')"><span class="glyphicon glyphicon-trash" aria-hidden="true" style="width:41px; text-align:center;"> 削除</span></a>
                
                </ItemTemplate>
                
            </asp:TemplateField>
        </Columns>
        <RowStyle BackColor="White" ForeColor="#330099" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
        <SortedAscendingCellStyle BackColor="#FEFCEB" />
        <SortedAscendingHeaderStyle BackColor="#AF0101" />
        <SortedDescendingCellStyle BackColor="#F6F0C0" />
        <SortedDescendingHeaderStyle BackColor="#7E0000" />
    </asp:GridView>


   
  
    <div align="center" id="show" style="margin-top:-20px;" >
            <%--<footer class="navbar-fixed-bottom" >--%>
        <%--<div style="position:relative; BackColor="#58ACFA"; ForeColor="#ffffff"; border-radius:1px; height:40px;" >--%>
    <%--<div style=" margin: 0px 100px; vertical-align:middle; line-height:40px">--%>
           
<div class="row" id="pageshowdiv" style="border:none; height:40px; line-height:40px;margin-top:-50px; margin-left:0px; margin-right:0px;background-color:white;width: 90%; ">
                        
                            <div class="pull-left" style="padding-left:60px" >
    <asp:label ID = "lblPage" runat="server" ForeColor="black" >Page No</asp:label>
   <%-- <asp:textbox ID= "txtPage" runat="server" Width = "75px"  onkeypress="return allowOnlyNumber(event);" BackColor="#FFE4E1">--%>
    <asp:TextBox ID="txtPage" width="40px" Height="25px" runat="server"  onkeypress="return allowOnlyNumber(event);"></asp:TextBox>
    

    <%--<asp:button ID ="btgo" runat="server" text="Go" onclick="btgo_Click" BackColor="#FFA07A"  CssClass="myButton"/>--%>
     <button id="btgo" type="button" class="btn btn-primary" runat="server" onserverclick="btgo_Click"  style=" " >
                                        <span class="glyphicon glyphicon-hand-right" aria-hidden="true"></span></button>
                                
                                </div>
                            <div class="pull-right" style="padding-right:60px">
   <asp:label ID = "Label1" runat="server" ForeColor="black">Page Size</asp:label>
    <asp:DropDownList ID="ddlPgsize" runat="server" AutoPostBack="true" 
        onselectedindexchanged="PageSize_IndexChanged"   width="60px" Height="25px">
    <asp:ListItem Text="10" Value="10" />
    <asp:ListItem Text="20" Value="20" />
    <asp:ListItem Text="30" Value="30" />
     <asp:ListItem Text="40" Value="40" />
    <asp:ListItem Text="50" Value="50" />

</asp:DropDownList>
                                </div>
                           
                  <%--</div>--%>      
        <%--</div>--%>
            </div>
                <%--</footer>--%>
                </div>
           

    </div>
        
        </div>
<script>
    function allowOnlyNumber(evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
    function lblSearch_onclick() {

    }

</script>
</asp:Content>