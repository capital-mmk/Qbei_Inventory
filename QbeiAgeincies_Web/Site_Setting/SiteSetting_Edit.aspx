<%@ Page Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="SiteSetting_Edit.aspx.cs" Inherits="QbeiAgeincies_Web.Qbei_Setting.settingedit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script src="../Scripts/userentry.js"></script>
    <link href="../Styles/userentrycommon.css" rel="stylesheet" />
     <script src="../Scripts/jquery-3.2.0.min.js" type="text/javascript"></script>
    <script type="../text/javascript" src="Scripts/bootstrap.min.js"></script>
    <link href="Styles/Common.css" rel="stylesheet" type="text/css" />
    <link href="Styles/bootstrap.min.css" rel="stylesheet" type="text/css"  />
        <script type="text/javascript">
            function allowOnlyNumber(evt) {
                var charCode = (evt.which) ? evt.which : event.keyCode
                if (charCode > 31 && (charCode < 48 || charCode > 57))
                    return false;
                return true;
            }

</script>
    <style type="text/css">
    body
    {
        height:9.0%;

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
     .ddl11
        {
            /*box-shadow: 10px 10px 5px #888888;*/
            border-radius:5px;
            border:1px solid gray;
        }
     .confirm_selection {
    -webkit-transition: text-shadow 0.2s linear;
    -moz-transition: text-shadow 0.2s linear;
    -ms-transition: text-shadow 0.2s linear;
    -o-transition: text-shadow 0.2s linear;
    transition: text-shadow 0.2s linear;
}
.confirm_selection:hover {
    text-shadow: 0 0 10px red; /* replace with whatever color you want */
}
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
      <div align="center">
        <div style="margin-top: -12px !important;">
           <div class="panel panel-custom1" style="width:100%" >
        <div class="panel-heading"><span class="fas fa-walking"></span>    代理店編集</div>
                    </div>
            </div>
          </div>

    <div style="padding-top:3%;" >
        <div style="text-align:center;">
			<div  align="center" >
				<div class="panel panel-login ddl11" style="width:500px;">
				<%--	<div >
						<div class="row" >
							
							<div class="col-xs-6" >
								<span style="font-size:30px; color:#567A92; font-weight:bold; text-shadow: 1px 1px #A5FFE3;">登録</span>
							</div>
						</div>
						<hr>
					</div>--%>
					<div class="panel-body">
						<div class="row">
							<div class="col-lg-12">
								<form id="login-form"  method="post" role="form" style="display: block;" class="ddl11">
								
                                    <div class="form-group">
                                          <asp:label ID= "lblsitename" runat="server" text="代理店名称" Width ="135px" style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:20px; color:#D0FFF3; "></asp:label>
                                          <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator2" controltovalidate="txtsitename" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />
										  <asp:Textbox style=" box-shadow: 10px 10px 5px #888888;"  ID = "txtsitename" runat="server"  MaxLength="30" CssClass="form-control" placeholder="SiteName" TabIndex="1" ></asp:Textbox>
                                    <div class="form-group">
                                        <asp:label  ID= "lblinfo" runat="server" text="代理店名称もう存在している" Visible = "false" Forecolor = "Red" Font-size ="9pt" Width ="135px"></asp:label>
                                    </div>

                                      <div class="form-group">
                                          <asp:label ID= "lblsiteid" runat="server" text="代理店ID" Width ="135px" style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:20px; color:#D0FFF3; "></asp:label>
                                          <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" controltovalidate="txtsiteid" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />
										  <asp:Textbox style=" box-shadow: 10px 10px 5px #888888;"  ID = "txtsiteid" runat="server"  MaxLength="8" CssClass="form-control" placeholder="SiteID" onkeypress="return allowOnlyNumber(event);" TabIndex="1"></asp:Textbox>
                                    <div class="form-group">
                                        <asp:label  ID= "lblinfo1" runat="server" text="代理店IDもう存在している" Visible = "false" Forecolor = "Red" Font-size ="9pt" Width ="135px"></asp:label>
                                    </div>

                                     <div class="form-group">
                                          <asp:label ID= "lblusername" runat="server" text="ユーザー名" Width ="135px" style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:20px; color:#D0FFF3; "></asp:label>
                                         <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator4" controltovalidate="txtusername" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />                                       
										  <asp:Textbox style=" box-shadow: 10px 10px 5px #888888;"  ID = "txtusername" runat="server"  MaxLength="30" CssClass="form-control" placeholder="UserName"  TabIndex="1"></asp:Textbox>
									</div>
                                    <div class="form-group">
                                        <asp:label  ID= "lblinfo2" runat="server" text="ユーザー名もう存在している" Visible = "false" Forecolor = "Red" Font-size ="9pt" Width ="135px"></asp:label>
                                    </div>


									<div class="form-group">
                                        <asp:label ID = "lblpassword" Visible="true"  runat="server" text="パスワード" Width ="135px" style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:20px; color:#D0FFF3"></asp:label>                                        
                                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator3" controltovalidate="txtpassword" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />
                                         <asp:textbox style=" box-shadow: 10px 10px 5px #888888;" ID="txtpassword" MaxLength="30" runat="server"  CssClass="form-control"  TabIndex="1"></asp:textbox>
                                     <br> <asp:label  ID= "lblpw" runat="server" text="Incorrect Password" Visible = "false" Forecolor = "Red"  Width ="300px" style="font-weight:bold;"></asp:label>
                                   
                                   </div>

                                    <div class="form-group">
                                        <asp:label ID = "lblurl" Visible="true"  runat="server" text="URL" Width ="135px" style="font-weight:bold;  text-shadow: 2px 2px grey; font-size:20px; color:#D0FFF3"></asp:label>
                                         <asp:textbox style=" box-shadow: 10px 10px 5px #888888;" ID="txturl"   runat="server"   CssClass="form-control"  TabIndex="1"></asp:textbox>
                               
                                   
                                   </div>
									<div class="form-group">
										<div class="row">
											<div class="col-sm-6 col-sm-offset-3">
												<asp:button ID = "btnSave" runat="server" text="登録" onclick ="btnSave_Click" BackColor="skyblue" CssClass="ddl11" ValidationGroup ="LoginFrame" />
                                                 <%-- <span id="sp1" runat="server" class="fa fa-floppy-o"></span> <asp:Label runat="server" ID="lblSave" Text="Save"></asp:Label>--%>
											</div>
										</div>
									</div>
									
								</form>
								
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>

</asp:Content>
