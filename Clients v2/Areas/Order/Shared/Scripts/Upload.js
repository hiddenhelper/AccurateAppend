$(document).ready(function () {
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
        select: function () {
            $("#errorMessage").hide();
        },
        success: function (e) {
            console.log("SUCCESS::");
            if (e.response.status === AccurateAppend.Web.HttpStatusCode.OK) {
                window.location.replace(e.response.data);
            }
            else {
                $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
                console.log("Failed to upload ");
            }
        },
        error: function (e) {
            console.log("ERROR::");
            if (e.files[0].extension === ".xls" || e.files[0].extension === ".xlsx")
                $("#errorMessage")
                    .text("The was an error uploading your file. If you are uploading a .xls or .xlsx, use Excel to save your file as a .csv and retry the upload. If the problem persists, please contact customer support.")
                    .show();
            else
                $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
            console.log("Failed to upload " + e.files.length + " files " + e.XMLHttpRequest.status + " " + e.XMLHttpRequest.responseText);
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiVXBsb2FkLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiVXBsb2FkLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUtBLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQyxLQUFLLENBQUM7SUFFZCxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUMsV0FBVyxDQUFDO1FBQ3BCLFFBQVEsRUFBRSxLQUFLO1FBQ2YsS0FBSyxFQUFFO1lBQ0gsT0FBTyxFQUFFLE9BQU87WUFDaEIsVUFBVSxFQUFFLElBQUk7WUFDaEIsZUFBZSxFQUFFLEtBQUs7U0FDekI7UUFDRCxVQUFVLEVBQUU7WUFDUixpQkFBaUIsRUFBRSxDQUFDLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLE1BQU0sRUFBRSxNQUFNLEVBQUUsS0FBSyxDQUFDO1lBQy9ELFdBQVcsRUFBRSxRQUFRO1NBQ3hCO1FBQ0QsTUFBTSxFQUFFO1lBQ0osQ0FBQyxDQUFDLGVBQWUsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO1FBQzlCLENBQUM7UUFDRCxPQUFPLEVBQUUsVUFBQSxDQUFDO1lBQ1IsT0FBTyxDQUFDLEdBQUcsQ0FBQyxXQUFXLENBQUMsQ0FBQztZQUN2QixJQUFJLENBQUMsQ0FBQyxRQUFRLENBQUMsTUFBTSxLQUFLLGNBQWMsQ0FBQyxHQUFHLENBQUMsY0FBYyxDQUFDLEVBQUUsRUFBRTtnQkFDNUQsTUFBTSxDQUFDLFFBQVEsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxJQUFJLENBQUMsQ0FBQzthQUM1QztpQkFBTTtnQkFDSCxDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsSUFBSSxDQUFDLHdFQUF3RSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7Z0JBQ3pHLE9BQU8sQ0FBQyxHQUFHLENBQUMsbUJBQW1CLENBQUMsQ0FBQzthQUNwQztRQUNMLENBQUM7UUFDRCxLQUFLLEVBQUUsVUFBQSxDQUFDO1lBQ04sT0FBTyxDQUFDLEdBQUcsQ0FBQyxTQUFTLENBQUMsQ0FBQztZQUNyQixJQUFJLENBQUMsQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDLENBQUMsU0FBUyxLQUFLLE1BQU0sSUFBSSxDQUFDLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLFNBQVMsS0FBSyxPQUFPO2dCQUNuRSxDQUFDLENBQUMsZUFBZSxDQUFDO3FCQUNiLElBQUksQ0FDRCxtTUFBbU0sQ0FBQztxQkFDdk0sSUFBSSxFQUFFLENBQUM7O2dCQUVaLENBQUMsQ0FBQyxlQUFlLENBQUMsQ0FBQyxJQUFJLENBQUMsd0VBQXdFLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztZQUM3RyxPQUFPLENBQUMsR0FBRyxDQUNQLHNCQUFvQixDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sZUFBVSxDQUFDLENBQUMsY0FBYyxDQUFDLE1BQU0sU0FBSSxDQUFDLENBQUMsY0FBYyxDQUFDLFlBQWMsQ0FBQyxDQUFDO1FBQ2hILENBQUM7UUFDRCxRQUFRLEVBQUUsS0FBSyxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7UUFDbkQsWUFBWSxFQUFFO1lBQ1YsTUFBTSxFQUFFLHFEQUFxRDtTQUNoRTtLQUNKLENBQUMsQ0FBQztBQUVQLENBQUMsQ0FBQyxDQUFDO0FBRUgsU0FBUyxrQkFBa0IsQ0FBQyxTQUFTO0lBQ25DLFFBQVEsU0FBUyxFQUFFO1FBQ25CLEtBQUssTUFBTSxDQUFDO1FBQ1osS0FBSyxNQUFNO1lBQ1QsT0FBTyxVQUFVLENBQUM7UUFDcEIsS0FBSyxNQUFNLENBQUM7UUFDWixLQUFLLE1BQU0sQ0FBQztRQUNaLEtBQUssTUFBTSxDQUFDO1FBQ1osS0FBSyxNQUFNO1lBQ1QsT0FBTyxVQUFVLENBQUM7UUFDcEIsS0FBSyxNQUFNLENBQUM7UUFDWixLQUFLLE9BQU87WUFDVixPQUFPLFVBQVUsQ0FBQztRQUNwQixLQUFLLE1BQU0sQ0FBQztRQUNaLEtBQUssT0FBTztZQUNWLE9BQU8sVUFBVSxDQUFDO1FBQ3BCLEtBQUssTUFBTTtZQUNULE9BQU8sVUFBVSxDQUFDO1FBQ3BCLEtBQUssTUFBTSxDQUFDO1FBQ1osS0FBSyxNQUFNO1lBQ1QsT0FBTyxVQUFVLENBQUM7UUFDcEI7WUFDRSxPQUFPLGNBQWMsQ0FBQztLQUN2QjtBQUNILENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL0h0dHBTdGF0dXNDb2RlLnRzXCIgLz5cclxuXHJcbmRlY2xhcmUgbGV0IHNhdmVVcmw6IHN0cmluZztcclxuXHJcbiQoZG9jdW1lbnQpLnJlYWR5KCgpID0+IHtcclxuXHJcbiAgICAkKFwiI2ZpbGVzXCIpLmtlbmRvVXBsb2FkKHtcclxuICAgICAgICBtdWx0aXBsZTogZmFsc2UsXHJcbiAgICAgICAgYXN5bmM6IHtcclxuICAgICAgICAgICAgc2F2ZVVybDogc2F2ZVVybCxcclxuICAgICAgICAgICAgYXV0b1VwbG9hZDogdHJ1ZSxcclxuICAgICAgICAgICAgd2l0aENyZWRlbnRpYWxzOiBmYWxzZVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdmFsaWRhdGlvbjoge1xyXG4gICAgICAgICAgICBhbGxvd2VkRXh0ZW5zaW9uczogW1wiVFhUXCIsIFwiQ1NWXCIsIFwiWExTXCIsIFwiWExTWFwiLCBcIlRFWFRcIiwgXCJaSVBcIl0sXHJcbiAgICAgICAgICAgIG1heEZpbGVTaXplOiAzMzU1NDQzMlxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc2VsZWN0OiAoKSA9PiB7XHJcbiAgICAgICAgICAgICQoXCIjZXJyb3JNZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3M6IGUgPT4ge1xyXG4gICAgICAgICAgY29uc29sZS5sb2coXCJTVUNDRVNTOjpcIik7XHJcbiAgICAgICAgICAgIGlmIChlLnJlc3BvbnNlLnN0YXR1cyA9PT0gQWNjdXJhdGVBcHBlbmQuV2ViLkh0dHBTdGF0dXNDb2RlLk9LKSB7XHJcbiAgICAgICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVwbGFjZShlLnJlc3BvbnNlLmRhdGEpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNlcnJvck1lc3NhZ2VcIikudGV4dChcIlRoZSB3YXMgYW4gZXJyb3IgdXBsb2FkaW5nIHlvdXIgZmlsZS4gUGxlYXNlIGNvbnRhY3QgY3VzdG9tZXIgc3VwcG9ydC5cIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS5sb2coXCJGYWlsZWQgdG8gdXBsb2FkIFwiKTsgLy8gd2Ugc2hvdWxkIGJlIGFibGUgdG8gZGlzcGxheSB0aGlzIG1lc3NhZ2UgZnJvbSB0aGUgcmVtb3RlIHN5c3RlbVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcjogZSA9PiB7XHJcbiAgICAgICAgICBjb25zb2xlLmxvZyhcIkVSUk9SOjpcIik7XHJcbiAgICAgICAgICAgIGlmIChlLmZpbGVzWzBdLmV4dGVuc2lvbiA9PT0gXCIueGxzXCIgfHwgZS5maWxlc1swXS5leHRlbnNpb24gPT09IFwiLnhsc3hcIilcclxuICAgICAgICAgICAgICAgICQoXCIjZXJyb3JNZXNzYWdlXCIpXHJcbiAgICAgICAgICAgICAgICAgICAgLnRleHQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIFwiVGhlIHdhcyBhbiBlcnJvciB1cGxvYWRpbmcgeW91ciBmaWxlLiBJZiB5b3UgYXJlIHVwbG9hZGluZyBhIC54bHMgb3IgLnhsc3gsIHVzZSBFeGNlbCB0byBzYXZlIHlvdXIgZmlsZSBhcyBhIC5jc3YgYW5kIHJldHJ5IHRoZSB1cGxvYWQuIElmIHRoZSBwcm9ibGVtIHBlcnNpc3RzLCBwbGVhc2UgY29udGFjdCBjdXN0b21lciBzdXBwb3J0LlwiKVxyXG4gICAgICAgICAgICAgICAgICAgIC5zaG93KCk7XHJcbiAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgICQoXCIjZXJyb3JNZXNzYWdlXCIpLnRleHQoXCJUaGUgd2FzIGFuIGVycm9yIHVwbG9hZGluZyB5b3VyIGZpbGUuIFBsZWFzZSBjb250YWN0IGN1c3RvbWVyIHN1cHBvcnQuXCIpLnNob3coKTtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coXHJcbiAgICAgICAgICAgICAgICBgRmFpbGVkIHRvIHVwbG9hZCAke2UuZmlsZXMubGVuZ3RofSBmaWxlcyAke2UuWE1MSHR0cFJlcXVlc3Quc3RhdHVzfSAke2UuWE1MSHR0cFJlcXVlc3QucmVzcG9uc2VUZXh0fWApO1xyXG4gICAgICAgIH0sXHJcbiAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjZmlsZVRlbXBsYXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgbG9jYWxpemF0aW9uOiB7XHJcbiAgICAgICAgICAgIHNlbGVjdDogXCJTZWxlY3QgZmlsZS4gKC50eHQsIC50ZXh0LCAuY3N2LCAueGxzLCAueGxzeCwgLnppcClcIlxyXG4gICAgICAgIH1cclxuICAgIH0pO1xyXG5cclxufSk7XHJcblxyXG5mdW5jdGlvbiBhZGRFeHRlbnNpb25DbGFzczEoZXh0ZW5zaW9uKSB7XHJcbiAgc3dpdGNoIChleHRlbnNpb24pIHtcclxuICBjYXNlIFwiLmNzdlwiOlxyXG4gIGNhc2UgXCIudHh0XCI6XHJcbiAgICByZXR1cm4gXCJ0eHQtZmlsZVwiO1xyXG4gIGNhc2UgXCIuanBnXCI6XHJcbiAgY2FzZSBcIi5pbWdcIjpcclxuICBjYXNlIFwiLnBuZ1wiOlxyXG4gIGNhc2UgXCIuZ2lmXCI6XHJcbiAgICByZXR1cm4gXCJpbWctZmlsZVwiO1xyXG4gIGNhc2UgXCIuZG9jXCI6XHJcbiAgY2FzZSBcIi5kb2N4XCI6XHJcbiAgICByZXR1cm4gXCJkb2MtZmlsZVwiO1xyXG4gIGNhc2UgXCIueGxzXCI6XHJcbiAgY2FzZSBcIi54bHN4XCI6XHJcbiAgICByZXR1cm4gXCJ4bHMtZmlsZVwiO1xyXG4gIGNhc2UgXCIucGRmXCI6XHJcbiAgICByZXR1cm4gXCJwZGYtZmlsZVwiO1xyXG4gIGNhc2UgXCIuemlwXCI6XHJcbiAgY2FzZSBcIi5yYXJcIjpcclxuICAgIHJldHVybiBcInppcC1maWxlXCI7XHJcbiAgZGVmYXVsdDpcclxuICAgIHJldHVybiBcImRlZmF1bHQtZmlsZVwiO1xyXG4gIH1cclxufVxyXG5cclxuIl19