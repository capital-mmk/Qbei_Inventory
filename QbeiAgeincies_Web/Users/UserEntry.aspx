<%@ Page Title="" Language="C#" MasterPageFile="~/inventory.Master" AutoEventWireup="true" CodeBehind="UserEntry.aspx.cs" Inherits="QbeiAgeincies_Web.Users.UserEntry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    

    <script src="../Scripts/userentry.js"></script>
    <link href="../Styles/userentrycommon.css" rel="stylesheet" />
     <script src="../Scripts/jquery-3.2.0.min.js" type="text/javascript"></script>
    <script type="../text/javascript" src="Scripts/bootstrap.min.js"></script>
    <link href="Styles/Common.css" rel="stylesheet" type="text/css" />
    <link href="Styles/bootstrap.min.css" rel="stylesheet" type="text/css"  />
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
            box-shadow: 10px 10px 5px #888888;
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
        <div class="panel-heading"><span class="fas fa-walking"></span>ユーザー作成</div>
                    </div>
            </div>
          </div>
    <div style="padding-top:4%;padding-bottom:10%;" >
        <div style="text-align:center;">
			<div  align="center" >
				<div class="panel panel-login ddl11" style="width:500px;">
				<%--	<div >
						<div class="row">
							
							<div class="col-xs-6">
								<span style="font-size:30px; color:#567A92; font-weight:bold; text-shadow: 1px 1px #A5FFE3;">登録</span>
							</div>
						</div>
						<hr>
					</div>--%>
					<div class="panel-body">
						<div class="row">
							<div class="col-lg-12">
								<form id="login-form"  method="post" role="form" style="display: block;" >
								
                                    <div class="form-group">
                                          <asp:label ID= "lbluserid" runat="server" text="ユーザーID" Width ="135px" style="font-size:20px;  "></asp:label>
                                        <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator1" controltovalidate="txtuserid" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />
										  <asp:Textbox ID = "txtuserid" runat="server"  MaxLength="8" CssClass="form-control" placeholder="ユーザーID"  TabIndex="1"></asp:Textbox>
									</div>
                                    <div class="form-group">
                                        <asp:label  ID= "lblinfo" runat="server" text="ユーザーIDは既に存在します" Visible = "false" Forecolor = "Red" Font-size ="9pt" Width ="135px"></asp:label>
                                    </div>
									<div class="form-group">
                                        <asp:label ID = "lblpassword" Visible = "true" runat="server" text="パスワード" Width ="150px" style="font-size:20px;"></asp:label>
                                         <asp:RequiredFieldValidator runat="server" id="RequiredFieldValidator3" controltovalidate="txtpassword" errormessage="*" ForeColor="Red" Font-Size="40px" style="height:30px; vertical-align: bottom;" ValidationGroup = "LoginFrame" Height="37px" />
                                         <asp:textbox ID="txtpassword" placeholder="パスワード" Visible = "true" runat="server" TextMode ="Password" Maxlength = "8" TabIndex="1" CssClass="form-control" ></asp:textbox>
                                       <br> <asp:label  ID= "lblpw" runat="server" text="Incorrect Password" Visible = "false" Forecolor = "Red"  Width ="300px" style="font-weight:bold;"></asp:label>
                                   
                                   </div>
                                    <div class="form-group">
                                        <asp:label ID = "lblnewpassword" Visible = "true" runat="server" text="新しいパスワード" style="font-size:20px; "></asp:label>
                                        <asp:textbox ID="txtnewpassword" style=" box-shadow: 10px 10px 5px;" Visible = "true" runat="server" TextMode ="Password" Maxlength = "8" CssClass="form-control" TabIndex="1"></asp:textbox>
                                    </div>
                                    <div class="form-group">
                                        <asp:label ID = "lblconfirm" Visible = "true" runat="server" text="パスワードの確認"  style="font-size:20px;"></asp:label>
                                         <asp:textbox ID="txtconfirm" style=" box-shadow: 10px 10px 5px;" Visible = "true" runat="server" TextMode ="Password" Maxlength = "8" CssClass="form-control" TabIndex="1"></asp:textbox>
                                        <asp:label  ID= "lblpass" runat="server" Text ="パスワードが一致しません" Visible = "false" Forecolor = "Red" style="font-weight:bold;"></asp:label>

                                    </div>
									<div class="form-group" style="text-align:left;" runat="server" id="adminrdb">
										 <asp:label ID="lbluserlevel" runat="server" text="ユーザーレベル:"  style="font-size:18px" Font-Bold="true"></asp:label>
                                         &nbsp;&nbsp;&nbsp;<asp:radiobutton ID="rdbUser" runat="server" text ="ユーザー"  Groupname = "UserLevel" Checked = "true"  style="text-align:center;">
                      </asp:radiobutton> &nbsp;&nbsp;&nbsp;&nbsp;

                      <asp:radiobutton ID="rdbAdmin" runat="server" text ="管理者" Groupname = "UserLevel" style="font-size:20px;">
                      </asp:radiobutton>

									</div>
									<div class="form-group">
										<div class="row">
											<div class="col-sm-6 col-sm-offset-3">
												<asp:button ID = "btnsave" runat="server" text="登録" onclick ="btnSave_Click" BackColor="skyblue" style="height:40px; width:80px;" ValidationGroup ="LoginFrame" />
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
