/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../Scripts/typings/HttpStatusCode.ts" />

declare let saveUrl: string;

$(document).ready(() => {

    $("#files").kendoUpload({
        multiple: false,
        async: {
            saveUrl: saveUrl,
            autoUpload: true,
            withCredentials: false
        },
        validation: {
            allowedExtensions: ["TXT", "CSV", "XLS", "XLSX", "TEXT", "ZIP"],
            maxFileSize: 33554432
        },
        select: () => {
            $("#errorMessage").hide();
        },
        success: e => {
          console.log("SUCCESS::");
            if (e.response.status === AccurateAppend.Web.HttpStatusCode.OK) {
                window.location.replace(e.response.data);
            } else {
                $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
                console.log("Failed to upload "); // we should be able to display this message from the remote system
            }
        },
        error: e => {
          console.log("ERROR::");
            if (e.files[0].extension === ".xls" || e.files[0].extension === ".xlsx")
                $("#errorMessage")
                    .text(
                        "The was an error uploading your file. If you are uploading a .xls or .xlsx, use Excel to save your file as a .csv and retry the upload. If the problem persists, please contact customer support.")
                    .show();
            else
                $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
            console.log(
                `Failed to upload ${e.files.length} files ${e.XMLHttpRequest.status} ${e.XMLHttpRequest.responseText}`);
        },
        template: kendo.template($("#fileTemplate").html()),
        localization: {
            select: "Select file. (.txt, .text, .csv, .xls, .xlsx, .zip)"
        }
    });

});

function addExtensionClass1(extension) {
  switch (extension) {
  case ".csv":
  case ".txt":
    return "txt-file";
  case ".jpg":
  case ".img":
  case ".png":
  case ".gif":
    return "img-file";
  case ".doc":
  case ".docx":
    return "doc-file";
  case ".xls":
  case ".xlsx":
    return "xls-file";
  case ".pdf":
    return "pdf-file";
  case ".zip":
  case ".rar":
    return "zip-file";
  default:
    return "default-file";
  }
}

