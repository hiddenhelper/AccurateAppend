<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Storage.Models.UploadModel>" %>
<!DOCTYPE html>
<html>
<head>
    <title>UPLOAD</title>
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.1.118/styles/kendo.common.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.1.118/styles/kendo.default.min.css" />
    <link rel="stylesheet" href="https://kendo.cdn.telerik.com/2017.1.118/styles/kendo.default.mobile.min.css" />

    <script src="https://kendo.cdn.telerik.com/2017.1.118/js/jquery.min.js"></script>
    <script src="https://kendo.cdn.telerik.com/2017.1.118/js/kendo.all.min.js"></script>
</head>
<body>

        <div id="example">
            <div class="box">
                <h4>Information</h4>
                <p>
                    Please upload your file(s).
                </p>
            </div>

            <div>
                <div class="demo-section k-content">
                    <input name="files" id="files" type="file" />
                </div>
            </div>
            
            <script>
                $(document).ready(function() {
                    $("#files").kendoUpload({
                        async: {
                            saveUrl: "<%= this.Url.Action("Save", "Upload") %>",
                            autoUpload: true
                        },
                        cancel: onCancel,
                        success: onSuccess
                    });
                });

                 function onCancel(e) {
                    window.location.replace('<%= this.Model.Cancel %>');
                }

                function onSuccess(e) {
                    window.location.replace('<%= this.Model.Success %>');
                }

                function onFail(e) {
                    window.location.replace('<%= this.Model.Fail %>');
                }
            </script>
        </div>
    
        <div>
            <div class="box">
                <h4>Information</h4>
                <p>
                    Please upload your file(s) using HTML.
                </p>
            </div>

            <div>
                <div class="demo-section k-content">
                    <form action="/Upload/Save?supportRedirect=true" method="post" enctype = "multipart/form-data">
                        <input type="file" id="fileinput" />
                        <input type="submit" value="upload"/>
                    </form>
                </div>
            </div>
        </div>

</body>
</html>