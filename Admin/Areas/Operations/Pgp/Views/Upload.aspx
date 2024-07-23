<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Uri>" %>
<%@ Import Namespace="System.Net" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
        PGP: Upload File
    </asp:Content>

    <asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <h3 style="margin-top: 0">Upload File</h3>
        <p>requires .pgp file extension!</p>
        <div class="alert" style="display: none;">
            <span></span>
        </div>

        <div style="width: 500px; margin-bottom: 20px; border: solid 1px #e0e0e0;">
            <div class="alert alert-danger" style="display: none;" id="errorMessage">The was an error uploading your file.</div>
            <script id="fileTemplate" type="text/x-kendo-template">
                <span class='k-progress'></span>
                <div class='file-wrapper'>
                    <span class='file-icon #=addExtensionClass(files[0].extension)#'></span>
                    <div style="di">Name: #=name# <button type='button' class='k-upload-action'></button></div>
                </div>
            </script>

            <input type="file" name="files" id="files" />

        </div>

    </asp:Content>
    <asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

        <script type="text/javascript">

        $(document).ready(function() {

            var httpStatusCodeOk = <%: (Int32)HttpStatusCode.OK %>;

            $("#files").kendoUpload({
                multiple: false,
                async: {
                        saveUrl: "<%= this.Model.ToString() %>",
                        autoUpload: true,
                        withCredentials: false
                },
                validation: {
                    allowedExtensions: [".pgp"]
                },
                select: function() {
                    $("#errorMessage").hide();
                },
                success: function (e) {
                    if (e.response.status === httpStatusCodeOk) {
                        window.location.replace(e.response.data);
                    } else {
                        $("#errorMessage").show();
                        console.log("Failed to upload " + e.message); // we should be able to display this message from the remote system
                    }
                },
                error: function (e) {
                    $("#errorMessage").show();
                    console.log("Failed to upload " + e.files.length + " files " + e.XMLHttpRequest.status + " " + e.XMLHttpRequest.responseText);
                },
                template: kendo.template($('#fileTemplate').html())
            });
        });

        function addExtensionClass(extension) {
            switch (extension) {
            case '.jpg':
            case '.img':
            case '.png':
            case '.gif':
                return "img-file";
            case '.doc':
            case '.docx':
                return "doc-file";
            case '.xls':
            case '.xlsx':
                return "xls-file";
            case '.pdf':
                return "pdf-file";
            case '.zip':
            case '.rar':
                return "zip-file";
            default:
                return "default-file";
            }
        }

        </script>

    </asp:Content>
