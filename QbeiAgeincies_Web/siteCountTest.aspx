<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="siteCountTest.aspx.cs" Inherits="QbeiAgeincies_Web.siteCountTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
 <form id="form1" runat="server">
    <div>
     
    </div>
  <asp:GridView ID="gvsiteCountTest" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:TemplateField HeaderText="Update_Date">
                    <ItemTemplate>
                  <asp:Label runat="server" ID="lblUpdate_Date" Text='<%# Eval("UpdateDate") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="siteID">
                    <ItemTemplate>
                  <asp:Label runat="server" ID="lblsiteID" Text='<%# Eval("siteID") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Real Data Count">
                    <ItemTemplate>
                  <asp:Label runat="server" ID="lblrdcount" Text='<%# Bind("Totalcount1") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Runtime Data Count">
                     <ItemTemplate>
                  <asp:Label runat="server" ID="lblrtcount" Text='<%# Bind("totalcount") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Check">
                    <ItemTemplate>
                  <asp:Label runat="server" ID="lblcheck" Text='<%# Bind("MyDesiredResult") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--  <asp:TemplateField HeaderText="Date">
                    <ItemTemplate>
                  <asp:Label runat="server" ID="lblDate" Text='<%# Bind("Date") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
              
              
           
        </asp:GridView>
    </form>
</body>
</html>
