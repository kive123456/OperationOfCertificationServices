<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileList.aspx.cs" Inherits="WebapiTest.FileList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="js/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div runat="server" id="div">        
    </div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="上传" OnClick="Button1_Click" OnClientClick="location.reload();" />
        <div id="divimg" style="display:none; z-index:10000;"></div>
        <script>
            $(function () {
                $("#div table tr td img").each(function () {
                    $(this).click(function () {
                        var filename = $(this).parent().parent().children().html();
                        $("#divimg").html("<img src='handler/FileDownload.ashx?filename=" + filename + "' />");
                        $("#divimg").css("display", "block");
                        //location.href = "handler/FileDownload.ashx?filename=" + filename;
                    });
                });
                $("#div table tr td a[title='download']").each(function () {
                    $(this).click(function () {
                        var filename = $(this).parent().parent().children().html();
                        location.href = "handler/FileDownload.ashx?filename=" + filename;
                        
                    });
                });
                $("#div table tr td a[title='delete']").each(function () {
                    $(this).click(function () {
                        var filename = $(this).parent().parent().children().html();
                        $.ajax({
                            type: "Post",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            url: "FileList.aspx/Delete",
                            data: "{'filename':'" + filename + "'}",
                            async: false,
                            success: function (msg) {
                                alert(msg.d);
                                location.reload();
                            },
                            error: function (err) {
                                var obj = JSON.parse(err.responseText);
                                alert(obj.Message);
                            }
                        });
                    });
                });
            })
        </script>
    </form>
</body>
</html>
