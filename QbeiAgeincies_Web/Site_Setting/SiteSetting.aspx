<%@ Page Language="C#" AutoEventWireup="true"MasterPageFile="~/inventory.Master" CodeBehind="SiteSetting.aspx.cs" Inherits="QbeiAgeincies_Web.Qbei_Setting.qbeisetting" %>

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
        .wrench
        {
            text-align: center !important;
            
        }

        .ddl11
        {
            box-shadow: 10px 10px 5px #888888;
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

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <div align="center">
        <div style="margin-top: -12px !important;">
           <div class="panel panel-custom1" style="width:100%" >
        <div class="panel-heading"><span class="fas fa-walking"></span>   代理店設定</div>
                    </div>
            </div>
<%--<div class="form-group col-xs-12"  style="vertical-align:middle">--%>
<%--<div class = "search" style="width: 100%; padding-top: 25px; height: 0px;margin-top:-5px;">
    
 

</div>--%>
<%--</div>--%>
     <div  style="padding-right:100px;padding-top:10px;margin-bottom:10px;">  <button style="background-color:#395587; color:white" id="btnnew" type="submit" class="btn btn-custom5 pull-right" runat="server" onserverclick="btnnew_ServerClick">
                    <span id="sp1" runat="server" class="icon-user-plus"></span> <asp:Label runat="server" ID="lblAddSetting" Text="代理店加する" ></asp:Label>
                </button></div>
<div style="width:100%;padding-top:37px;margin-bottom:5%; " align="center">
     
    <asp:GridView runat="server" ID="gvTest" AutoGenerateColumns="False" 
         AllowPaging="True"  ShowHeaderWhenEmpty="true"
        CssClass="table table-striped table-bordered table-hover usd" onpageindexchanging="gvTest_PageIndexChanging"  RowStyle-CssClass="rsc1" Width="98%" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" >
        <PagerStyle CssClass="pagination" BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center"/>
        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
        <AlternatingRowStyle   BackColor="#DAEFA1" />
        <HeaderStyle BackColor="#008080" Font-Bold="True" ForeColor="#FFFFCC" />
        <PagerSettings FirstPageText="<<" LastPageText=">>" PageButtonCount = "4"/>
        <Columns>
         <asp:TemplateField>
                <HeaderTemplate>ID</HeaderTemplate>
            <%-- <HeaderStyle CssClass="text-center" />--%>
              <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                  <asp:Label runat="server" ID="lblid" Text='<%# Bind("ID") %>'></asp:Label>

                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField>
                <HeaderTemplate>名称</HeaderTemplate>
              <%--  <HeaderStyle CssClass="text-center"/>--%>
                 <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblsitename" Text='<%# Bind("SiteName") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>

            <asp:TemplateField>
                <HeaderTemplate >代理店ID</HeaderTemplate>
               <%-- <HeaderStyle CssClass="text-center"/>--%>
                 <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblsiteid" Text='<%# Bind("SiteID") %>'></asp:Label>
                    </ItemTemplate>
                      <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>

             <asp:TemplateField>
                <HeaderTemplate >ユーザー名</HeaderTemplate>
               <%-- <HeaderStyle CssClass="text-center"/>--%>
                  <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblusername" Text='<%# Bind("UserName") %>'></asp:Label>
                    </ItemTemplate>
                      <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
              
             <asp:TemplateField>
                <HeaderTemplate >パスワード</HeaderTemplate>
               <%-- <HeaderStyle CssClass="text-center"/>--%>
                  <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblpassword" Text='<%# Bind("Password") %>'></asp:Label>
                    </ItemTemplate>
                      <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>

             <asp:TemplateField>
                <HeaderTemplate >URL</HeaderTemplate>
               <%-- <HeaderStyle CssClass="text-center"/>--%>
                  <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblurl" Text='<%# Bind("Url") %>'></asp:Label>
                    </ItemTemplate>
                      <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
              
            <asp:TemplateField>
                <HeaderTemplate><span class="glyphicon glyphicon-wrench" aria-hidden="true"></span></HeaderTemplate>
                 <%--<HeaderStyle CssClass="col-md-2 text-center" />--%>
                 <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <a id="btnEdit2" class="btn btn-info btn-xs edit" runat="server" onserverclick="btnEdit2_Click"><span class="glyphicon glyphicon-edit" aria-hidden="true" style="width:41px;"> 編集</span></a>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <HeaderTemplate><span class="glyphicon glyphicon-wrench" aria-hidden="true"></span></HeaderTemplate>
                <%-- <HeaderStyle CssClass="col-md-2 text-center" />--%>
                 <HeaderStyle  HorizontalAlign="Center" CssClass="text-center" BackColor="#58ACFA" ForeColor="#ffffff"/>
                <ItemTemplate>
                    <a id="btnDelete" class="btn btn-info btn-xs edit" runat="server" onserverclick="btnDelete_Click" onclick="return confirm('削除してもよろしいでしょうか?')" ><span class="glyphicon glyphicon-delete" aria-hidden="true" style="width:41px;"> 削除</span></a>
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
    </div>
        <div align="center" >
          
                           
                  </div>      
        </asp:Content>