<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User_Entry.aspx.cs" Inherits="QbeiAgeincies_Web.Users.User_Entry" MasterPageFile="~/inventory.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    

    <script src="../Scripts/userentry.js"></script>
    <link href="../Styles/userentrycommon.css" rel="stylesheet" />
     <script src="../Scripts/jquery-3.2.0.min.js" type="text/javascript"></script>
    <script type="../text/javascript" src="Scripts/bootstrap.min.js"></script>
    <link href="Styles/Common.css" rel="stylesheet" type="text/css" />
    <link href="Styles/bootstrap.min.css" rel="stylesheet" type="text/css"  />
<%--    <style type="text/css">
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
</style> --%> 
    <style>
     



.testbox {
  margin: 20px auto;
  width: 343px; 
  height: 464px; 
  -webkit-border-radius: 8px/7px; 
  -moz-border-radius: 8px/7px; 
  border-radius: 8px/7px; 
  background-color: #ebebeb; 
  -webkit-box-shadow: 1px 2px 5px rgba(0,0,0,.31); 
  -moz-box-shadow: 1px 2px 5px rgba(0,0,0,.31); 
  box-shadow: 1px 2px 5px rgba(0,0,0,.31); 
  border: solid 1px #cbc9c9;
}

input[type=radio] {
  visibility: hidden;
}


label.radio {
	cursor: pointer;
  text-indent: 35px;
  overflow: visible;
  display: inline-block;
  position: relative;
  margin-bottom: 15px;
}

label.radio:before {
  background: #3a57af;
  content:'';
  position: absolute;
  top:2px;
  left: 0;
  width: 20px;
  height: 20px;
  border-radius: 100%;
}

label.radio:after {
    opacity: 0;
    content: '';
    position: absolute;
    width: 0.5em;
    height: 0.25em;
    background: transparent;
    top: 7.5px;
    left: 4.5px;
    border: 3px solid #ffffff;
     border-top: none; 
     border-right: none; 
     -webkit-transform: rotate(-45deg); 
    -moz-transform: rotate(-45deg);
    -o-transform: rotate(-45deg);
    -ms-transform: rotate(-45deg);
    transform: rotate(-45deg);
}

input[type=radio]:checked + label:after {
	opacity: 1;
}

hr{
  color: #a9a9a9;
  opacity: 0.3;
}

input[type=text],input[type=password]{
  width: 300px; 
  height: 39px; 
  -webkit-border-radius: 0px 4px 4px 0px/5px 5px 4px 4px; 
  -moz-border-radius: 0px 4px 4px 0px/0px 0px 4px 4px; 
  border-radius: 0px 4px 4px 0px/5px 5px 4px 4px; 
  background-color: #fff; 
  -webkit-box-shadow: 1px 2px 5px rgba(0,0,0,.09); 
  -moz-box-shadow: 1px 2px 5px rgba(0,0,0,.09); 
  box-shadow: 1px 2px 5px rgba(0,0,0,.09); 
  border: solid 1px #cbc9c9;
  margin-left: -5px;
  margin-top: 13px; 
  padding-left: 10px;
}


#icon {
  display: inline-block;
  width: 30px;
  background-color: #3a57af;
  padding: 8px 0px 8px 15px;
  margin-left: 15px;
  -webkit-border-radius: 4px 0px 0px 4px; 
  -moz-border-radius: 4px 0px 0px 4px; 
  border-radius: 4px 0px 0px 4px;
  color: white;
  -webkit-box-shadow: 1px 2px 5px rgba(0,0,0,.09);
  -moz-box-shadow: 1px 2px 5px rgba(0,0,0,.09); 
  box-shadow: 1px 2px 5px rgba(0,0,0,.09); 
  border: solid 0px #cbc9c9;
}

.gender {
  margin-left: 30px;
  margin-bottom: 30px;
}

.accounttype{
  margin-left: 8px;
  margin-top: 20px;
}

a.button {
  font-size: 14px;
  font-weight: 600;
  color: white;
  padding: 6px 25px 0px 20px;
  /*margin: 10px 8px 20px 0px;*/
  display: inline-block;

  text-decoration: none;
     width: 100px; height: 27px; 
  -webkit-border-radius: 5px; 
  -moz-border-radius: 5px; 
  border-radius: 5px; 
  background-color: #3a57af; 
  -webkit-box-shadow: 0 3px rgba(58,87,175,.75); 
  -moz-box-shadow: 0 3px rgba(58,87,175,.75); 
  box-shadow: 0 3px rgba(58,87,175,.75);
  transition: all 0.1s linear 0s; 
  top: 0px;
}

a.button:hover {
  top: 3px;
  background-color:#2e458b;
  -webkit-box-shadow: none; 
  -moz-box-shadow: none; 
  box-shadow: none;
  
}
.ico_{
    width: 30px;
    vertical-align: middle !important;
    height: 36px;
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
    <link href='https://fonts.googleapis.com/css?family=Open+Sans:400,300,300italic,400italic,600' rel='stylesheet' type='text/css'>
<link href="//netdna.bootstrapcdn.com/font-awesome/3.1.1/css/font-awesome.css" rel="stylesheet">

<div class="testbox" style="height:600px; width:450px; display:none " align="center" >
  <h1>Registration</h1>


      <hr>
  <hr>
    <div>
  <label id="icon" for="name"><i class="icon-user ico_"></i></label>
  <input type="text" name="name" id="txtuserid1" runat="server" placeholder="Name" required/>
     </div> <br />
     <div> <label id="icon" for="name"><i class="icon-shield ico_"></i></label>
  <input type="password" name="name" id="txtpassword1" runat="server" placeholder="Oldpassword" required/></div><br />
  <div><label id="icon" for="name"><i class="icon-shield ico_"></i></label>
  <input type="password" name="name" id="name" placeholder="Password" required/></div>
      <br /><div><label id="icon" for="name"><i class="icon-shield ico_"></i></label>
     <input type="password" name="name" id="name" placeholder="Password" required/></div>
  <div class="gender">
    <input type="radio" value="None" id="male" name="gender" checked/>
    <label for="male" class="radio" chec>User</label>
    <input type="radio" value="None" id="female" name="gender" />
    <label for="female" class="radio">Admin</label>
   </div> 
 <br />
    <div>   <input  type="reset" " class="button"  style="float:left; left:20px; float: left; left: 20px; background-color: #b42795;color: white;
    border-radius: 5px;" value="Clear" />  <asp:Button  class="button" runat="server"  style="background-color: #246cab;color: white;
    border-radius: 5px;float:right; right:20px;width:100px" Text="登録"  ></asp:Button></div>

</div>
    <div  >
        <div style="text-align:center;">
			<div  align="center" >
				<div class="panel panel-login ddl11" style="width:500px;">
					<div >
						<div class="row">
							
							<div class="col-xs-6">
								<span style="font-size:30px; color:#567A92; font-weight:bold; text-shadow: 1px 1px #A5FFE3;">登録</span>
							</div>
						</div>
						<hr>
					</div>
					<div class="panel-body">
						<div class="row">
							<div class="col-lg-12">
								<form id="login-form"  " style="display: block;" class="ddl11">
								
                                    <div class="form-group">
                                          <asp:label ID= "lbluserid" runat="server" text="ユーザーID" Width ="135px" style="font-size:20px;  "></asp:label>
										  <asp:Textbox style=" box-shadow: 10px 10px 5px;"  ID = "txtuserid" runat="server"  MaxLength="8" CssClass="form-control" placeholder="ユーザーID"  TabIndex="1"></asp:Textbox>
                                          <asp:RequiredFieldValidator runat="server" id="requserid" controltovalidate="txtuserid" errormessage="*" ForeColor="Red" ValidationGroup = "LoginFrame" />
									</div>
                                    <div class="form-group">
                                        <asp:label  ID= "lblinfo" runat="server" text="ユーザーIDは既に存在します" Visible = "false" Forecolor = "Red" Font-size ="9pt" Width ="135px"></asp:label>
                                    </div>
									<div class="form-group">
                                        <asp:label ID = "lblpassword" Visible = "true" runat="server" text="パスワード" Width ="135px" style="font-size:20px;"></asp:label>
                                         <asp:Textbox style=" box-shadow: 10px 10px 5px;" ID="txtpassword" placeholder="パスワード" Visible = "true" runat="server"  Maxlength = "8" TabIndex="1" CssClass="form-control" ></asp:Textbox>
                                       <br> <asp:label  ID= "lblpw" runat="server" text="Incorrect Password" Visible = "false" Forecolor = "Red"  Width ="300px" style="font-weight:bold;"></asp:label>
                                         <asp:RequiredFieldValidator runat="server" id="reqpassword" controltovalidate="txtpassword" errormessage="*" ForeColor="Red" ValidationGroup = "LoginFrame" />
                                   
                                   </div>
                                <%--    <div class="form-group" runat="server" visible="false">
                                        <asp:label ID = "lblnewpassword" Visible = "true" runat="server" text="新しいパスワード" style="font-size:20px; "></asp:label>
                                        <asp:textbox ID="txtnewpassword" style=" box-shadow: 10px 10px 5px;" Visible = "true" runat="server" TextMode ="Password" Maxlength = "8" CssClass="form-control" TabIndex="1"></asp:textbox>
                                    </div>
                                    <div class="form-group" runat="server" visible="false">
                                        <asp:label ID = "lblconfirm" Visible = "true" runat="server" text="パスワードの確認"  style="font-size:20px;"></asp:label>
                                         <asp:textbox ID="txtconfirm" style=" box-shadow: 10px 10px 5px;" Visible = "true" runat="server" TextMode ="Password" Maxlength = "8" CssClass="form-control" TabIndex="1"></asp:textbox>
                                        <asp:label  ID= "lblpass" runat="server" Text ="パスワードが一致しません" Visible = "false" Forecolor = "Red" style="font-weight:bold;"></asp:label>

                                    </div>
									<div class="form-group" style="text-align:center;" runat="server" id="adminrdb" visible="false">
										 <asp:label ID="lbluserlevel" runat="server" text="ユーザーレベル"  style="font-size:18px;"></asp:label>
                                         &nbsp;&nbsp;&nbsp;<asp:radiobutton ID="rdbUser" runat="server" text ="ユーザー"  Groupname = "UserLevel" Checked = "true" style="text-align:left;">
                      </asp:radiobutton> &nbsp;&nbsp;&nbsp;&nbsp;

                      <asp:radiobutton ID="rdbAdmin" runat="server" text ="管理者" Groupname = "UserLevel" style="font-size:15px;">
                      </asp:radiobutton>

									</div>--%>
									<div class="form-group">
										<div class="row">
											<div class="col-sm-6 col-sm-offset-3">
												<button id = "btnsave" runat="server"  onserverclick="btnSave_Click" type="button"  class="ddl11 btn btn-default"  > <span> 登録</span></button>
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