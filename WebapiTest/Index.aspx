<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebapiTest.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="上传" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="获取" /><br />
        <asp:TextBox ID="TextBox1" runat="server" Text="a58b1afd-d767-4113-ac53-c849441b65a6.jpg"></asp:TextBox>
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="Button" /><br />
        <asp:TextBox ID="TextBox2" runat="server" Text="a58b1afd-d767-4113-ac53-c849441b65a6.jpg"></asp:TextBox>
        <asp:Button ID="Button4" runat="server" Text="Button" OnClick="Button4_Click" />
    </div>   
    </form>
</body>
</html>
