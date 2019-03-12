<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QbeiAgeincies_Web.Login.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Qbei Agencies</title>
    <link rel="stylesheet" href="../Styles/Login.css" />
    <script type = "text/javascript" >
        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
</head>

    <body onunLoad="preventBack()" style="background:url('../Images/aa.jpg') no-repeat center fixed; background-size:cover;">
    <div class="lg_container">
		<h1>きゅうべえ<br />ログイン</h1>
		<form action="" id="lg_form"  method="post" runat="server">		
			<div>
                <asp:TextBox runat="server" ID="txtUserID" placeholder="UserID" BackColor="#FFE4E1"></asp:TextBox>
			</div>
            <br />
			<div>
                <asp:TextBox runat="server" ID="txtPassword" placeholder="password" 
                    TextMode="Password" Maxlength = "8" BackColor="#FFE4E1"></asp:TextBox>
			</div>
            <br />
			<div align="center">				
                <asp:Button runat="server" ID="btnLogin" Text="ログイン"
                    onclick="btnLogin_Click" BackColor="#FFA07A"/>
			</div>
		</form>
		<div id="message">
            <asp:Label runat="server" ID="lblErrorMsg" Text="パスワードに誤りがあります。再入力してください。" ForeColor="Red" Visible="false"></asp:Label>
        </div>
	</div>
</body>
</html>
