<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Guid>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Suppression: Upload File
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3 style="margin-top: 0">Upload File</h3>
    <div class="alert" style="display: none;">
        <span></span></div>
    <% using (Html.BeginForm("Index", "UploadSuppression", FormMethod.Post, new {enctype = "multipart/form-data"}))
       { %>

        <div style="width: 500px; margin-bottom: 20px; border: solid 1px #e0e0e0;">
            <p style="margin: 10px;"><%= this.Html.Label("id", "ID") %> <%= this.Html.TextBox("id", this.Model, new {maxlength=50,size=40}) %></p>
            
            <script id="fileTemplate" type="text/x-kendo-template">
                <span class='k-progress'></span>
                <div class='file-wrapper'>
                    <span class='file-icon #=addExtensionClass(files[0].extension)#'></span>
                    <div style="di">Name: #=name# <button type='button' class='k-upload-action'></button></div>
                </div>
            </script>

            <input type="file" name="files" id="files"/>
            
            <p style="margin: 10px;">
                File must be a CSV formatted text document without a header row. The first column must contain fully complete email addresses. Each email <b>must be distinct</b>.<br />
                For suppression with more than 2000 records, multiple upload / import actions utilizing the same id value can be made to append to an existing set vs creating a new one. 
            </p>
        </div>

        <p style="margin-top: 10px;">
            <input type="submit" id="next" value="Upload" class="btn btn-primary" style="display: inline;" disabled="disabled"/>
        </p>

    <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {

            $("#files").kendoUpload({
                multiple: false,
                template: kendo.template($('#fileTemplate').html()),
                select: onSelect,
                error: onError
            });
        });

        function onError(e) {
            alert("Error (" + e.operation + ") :: " + getFileInfo(e));
        }

        function getFileInfo(e) {
            return $.map(e.files, function (file) {
                var info = file.name;

                // File size is not available in all browsers
                if (file.size > 0) {
                    info += " (" + Math.ceil(file.size / 1024) + " KB)";
                }
                return info;
            }).join(", ");
        }

        function onSelect(e) {
            $.each(e.files, function(index, value) {
                if (!(value.extension.toLowerCase() === ".txt"
                    || value.extension.toLowerCase() === ".csv")) {
                    e.preventDefault();
                    $(".alert").addClass("alert-danger").show().find("span").text("Only .txt and .csv files can be uploaded.");
                } else {
                    $(".alert").removeClass("alert-danger").hide();
                    $("#next").prop('disabled', false);
                }
            });
        };

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