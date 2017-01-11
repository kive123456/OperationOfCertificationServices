<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="WebapiTest.Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="plupload/js/jquery.ui.plupload/css/jquery.ui.plupload.css" rel="stylesheet"
        type="text/css" />
    <link href="plupload/js/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript" src="js/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="plupload/js/jquery-ui.min.js"></script>
    <script type="text/javascript" src="plupload/js/browserplus.min.js"></script>
    <script src="plupload/js/plupload.full.min.js" type="text/javascript"></script>
    <script src="plupload/js/i18n/zh_CN.js" type="text/javascript"></script>
    <script src="plupload/js/jquery.ui.plupload/jquery.ui.plupload.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="divUpload">
            </div>
        </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            startUpload();
        });

        var startUpload = function () {
            $("#divUpload").empty();
            var div = $("<div>");
            $("#divUpload").append(div);
            $(div).plupload({
                runtimes: 'html5,flash,silverlight,html4',
                url: 'handler/FileUpload.ashx', // 服务端上传路径
                max_file_size: '20mb', // 文件上传最大限制。
                chunk_size: '20mb', // 上传分块每块的大小，这个值小于服务器最大上传限制的值即可。

                // 文件类型限制
                filters: [
                        { title: "图片", extensions: "jpg,jpeg,gif,png,bmp" },
                        { title: "文档", extensions: "doc,docx,xls,xlsx,ppt,pptx,txt,pdf" }
                ],
                rename: true,
                sortable: true,
                dragdrop: true,
                views: {
                    list: true,
                    thumbs: true,
                    active: 'thumbs'
                },
                buttons: {
                    stop: false
                },

                // Flash文件 的所在路径
                flash_swf_url: 'plupload/js/Moxie.swf',

                // silverlight文件所在路径
                silverlight_xap_url: 'plupload/js/Moxie.xap',
                multipart: true,
                init: {
                    BeforeUpload: function (up, file) {
                        up.settings.multipart_params = {
                            filename: file.name
                        };
                    }
                }
            });
            // 这一块主要是防止在上传未结束前表单
            $('form').submit(function (e) {
                var uploader = $('#uploader').pluploadQueue();
                if (uploader.files.length > 0) {
                    uploader.bind('StateChanged', function () {
                        if (uploader.files.length === (uploader.total.uploaded + uploader.total.failed)) {
                            $('form')[0].submit();
                        }
                    });
                    uploader.start();
                }
                return false;
            });
        };

    </script>
</body>
</html>
