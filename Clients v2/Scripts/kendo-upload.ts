/// <reference path="typings/kendo-ui/kendo-ui.d.ts" />
declare let saveUri: string;

$(document).ready(() => {

  $("#files").kendoUpload({
    multiple: false,
    async: {
      saveUrl: saveUri,
      autoUpload: true,
      withCredentials: false
    },
    //validation: {
    //  allowedExtensions: [".txt", ".csv", ".xls", ".xlsx"]
    //},
    select: () => {
      $("#errorMessage").hide();
    },
    success: e => {
      if (e.response.status === 200) {
        window.location.replace(e.response.data);
      } else {
        $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
        console.log("Failed to upload "); // we should be able to display this message from the remote system
      }
    },
    error: e => {
      $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
      console.log(
        `Failed to upload ${e.files.length} files ${e.XMLHttpRequest.status} ${e.XMLHttpRequest.responseText}`);
    },
    template: kendo.template($("#fileTemplate").html()),
    localization: {
      select: "Select file"
    }
  });

});

function addExtensionClass(extension) {
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