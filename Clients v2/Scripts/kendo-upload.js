$(document).ready(function () {
    $("#files").kendoUpload({
        multiple: false,
        async: {
            saveUrl: saveUri,
            autoUpload: true,
            withCredentials: false
        },
        select: function () {
            $("#errorMessage").hide();
        },
        success: function (e) {
            if (e.response.status === 200) {
                window.location.replace(e.response.data);
            }
            else {
                $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
                console.log("Failed to upload ");
            }
        },
        error: function (e) {
            $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
            console.log("Failed to upload " + e.files.length + " files " + e.XMLHttpRequest.status + " " + e.XMLHttpRequest.responseText);
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
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoia2VuZG8tdXBsb2FkLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsia2VuZG8tdXBsb2FkLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUdBLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQyxLQUFLLENBQUM7SUFFaEIsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDLFdBQVcsQ0FBQztRQUN0QixRQUFRLEVBQUUsS0FBSztRQUNmLEtBQUssRUFBRTtZQUNMLE9BQU8sRUFBRSxPQUFPO1lBQ2hCLFVBQVUsRUFBRSxJQUFJO1lBQ2hCLGVBQWUsRUFBRSxLQUFLO1NBQ3ZCO1FBSUQsTUFBTSxFQUFFO1lBQ04sQ0FBQyxDQUFDLGVBQWUsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO1FBQzVCLENBQUM7UUFDRCxPQUFPLEVBQUUsVUFBQSxDQUFDO1lBQ1IsSUFBSSxDQUFDLENBQUMsUUFBUSxDQUFDLE1BQU0sS0FBSyxHQUFHLEVBQUU7Z0JBQzdCLE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxRQUFRLENBQUMsSUFBSSxDQUFDLENBQUM7YUFDMUM7aUJBQU07Z0JBQ0wsQ0FBQyxDQUFDLGVBQWUsQ0FBQyxDQUFDLElBQUksQ0FBQyx3RUFBd0UsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO2dCQUN6RyxPQUFPLENBQUMsR0FBRyxDQUFDLG1CQUFtQixDQUFDLENBQUM7YUFDbEM7UUFDSCxDQUFDO1FBQ0QsS0FBSyxFQUFFLFVBQUEsQ0FBQztZQUNOLENBQUMsQ0FBQyxlQUFlLENBQUMsQ0FBQyxJQUFJLENBQUMsd0VBQXdFLENBQUMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztZQUN6RyxPQUFPLENBQUMsR0FBRyxDQUNULHNCQUFvQixDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sZUFBVSxDQUFDLENBQUMsY0FBYyxDQUFDLE1BQU0sU0FBSSxDQUFDLENBQUMsY0FBYyxDQUFDLFlBQWMsQ0FBQyxDQUFDO1FBQzVHLENBQUM7UUFDRCxRQUFRLEVBQUUsS0FBSyxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7UUFDbkQsWUFBWSxFQUFFO1lBQ1osTUFBTSxFQUFFLGFBQWE7U0FDdEI7S0FDRixDQUFDLENBQUM7QUFFTCxDQUFDLENBQUMsQ0FBQztBQUVILFNBQVMsaUJBQWlCLENBQUMsU0FBUztJQUNsQyxRQUFRLFNBQVMsRUFBRTtRQUNuQixLQUFLLE1BQU0sQ0FBQztRQUNaLEtBQUssTUFBTTtZQUNULE9BQU8sVUFBVSxDQUFDO1FBQ3BCLEtBQUssTUFBTSxDQUFDO1FBQ1osS0FBSyxNQUFNLENBQUM7UUFDWixLQUFLLE1BQU0sQ0FBQztRQUNaLEtBQUssTUFBTTtZQUNULE9BQU8sVUFBVSxDQUFDO1FBQ3BCLEtBQUssTUFBTSxDQUFDO1FBQ1osS0FBSyxPQUFPO1lBQ1YsT0FBTyxVQUFVLENBQUM7UUFDcEIsS0FBSyxNQUFNLENBQUM7UUFDWixLQUFLLE9BQU87WUFDVixPQUFPLFVBQVUsQ0FBQztRQUNwQixLQUFLLE1BQU07WUFDVCxPQUFPLFVBQVUsQ0FBQztRQUNwQixLQUFLLE1BQU0sQ0FBQztRQUNaLEtBQUssTUFBTTtZQUNULE9BQU8sVUFBVSxDQUFDO1FBQ3BCO1lBQ0UsT0FBTyxjQUFjLENBQUM7S0FDdkI7QUFDSCxDQUFDIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cInR5cGluZ3Mva2VuZG8tdWkva2VuZG8tdWkuZC50c1wiIC8+XHJcbmRlY2xhcmUgbGV0IHNhdmVVcmk6IHN0cmluZztcclxuXHJcbiQoZG9jdW1lbnQpLnJlYWR5KCgpID0+IHtcclxuXHJcbiAgJChcIiNmaWxlc1wiKS5rZW5kb1VwbG9hZCh7XHJcbiAgICBtdWx0aXBsZTogZmFsc2UsXHJcbiAgICBhc3luYzoge1xyXG4gICAgICBzYXZlVXJsOiBzYXZlVXJpLFxyXG4gICAgICBhdXRvVXBsb2FkOiB0cnVlLFxyXG4gICAgICB3aXRoQ3JlZGVudGlhbHM6IGZhbHNlXHJcbiAgICB9LFxyXG4gICAgLy92YWxpZGF0aW9uOiB7XHJcbiAgICAvLyAgYWxsb3dlZEV4dGVuc2lvbnM6IFtcIi50eHRcIiwgXCIuY3N2XCIsIFwiLnhsc1wiLCBcIi54bHN4XCJdXHJcbiAgICAvL30sXHJcbiAgICBzZWxlY3Q6ICgpID0+IHtcclxuICAgICAgJChcIiNlcnJvck1lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgfSxcclxuICAgIHN1Y2Nlc3M6IGUgPT4ge1xyXG4gICAgICBpZiAoZS5yZXNwb25zZS5zdGF0dXMgPT09IDIwMCkge1xyXG4gICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKGUucmVzcG9uc2UuZGF0YSk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiNlcnJvck1lc3NhZ2VcIikudGV4dChcIlRoZSB3YXMgYW4gZXJyb3IgdXBsb2FkaW5nIHlvdXIgZmlsZS4gUGxlYXNlIGNvbnRhY3QgY3VzdG9tZXIgc3VwcG9ydC5cIikuc2hvdygpO1xyXG4gICAgICAgIGNvbnNvbGUubG9nKFwiRmFpbGVkIHRvIHVwbG9hZCBcIik7IC8vIHdlIHNob3VsZCBiZSBhYmxlIHRvIGRpc3BsYXkgdGhpcyBtZXNzYWdlIGZyb20gdGhlIHJlbW90ZSBzeXN0ZW1cclxuICAgICAgfVxyXG4gICAgfSxcclxuICAgIGVycm9yOiBlID0+IHtcclxuICAgICAgJChcIiNlcnJvck1lc3NhZ2VcIikudGV4dChcIlRoZSB3YXMgYW4gZXJyb3IgdXBsb2FkaW5nIHlvdXIgZmlsZS4gUGxlYXNlIGNvbnRhY3QgY3VzdG9tZXIgc3VwcG9ydC5cIikuc2hvdygpO1xyXG4gICAgICBjb25zb2xlLmxvZyhcclxuICAgICAgICBgRmFpbGVkIHRvIHVwbG9hZCAke2UuZmlsZXMubGVuZ3RofSBmaWxlcyAke2UuWE1MSHR0cFJlcXVlc3Quc3RhdHVzfSAke2UuWE1MSHR0cFJlcXVlc3QucmVzcG9uc2VUZXh0fWApO1xyXG4gICAgfSxcclxuICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI2ZpbGVUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgbG9jYWxpemF0aW9uOiB7XHJcbiAgICAgIHNlbGVjdDogXCJTZWxlY3QgZmlsZVwiXHJcbiAgICB9XHJcbiAgfSk7XHJcblxyXG59KTtcclxuXHJcbmZ1bmN0aW9uIGFkZEV4dGVuc2lvbkNsYXNzKGV4dGVuc2lvbikge1xyXG4gIHN3aXRjaCAoZXh0ZW5zaW9uKSB7XHJcbiAgY2FzZSBcIi5jc3ZcIjpcclxuICBjYXNlIFwiLnR4dFwiOlxyXG4gICAgcmV0dXJuIFwidHh0LWZpbGVcIjtcclxuICBjYXNlIFwiLmpwZ1wiOlxyXG4gIGNhc2UgXCIuaW1nXCI6XHJcbiAgY2FzZSBcIi5wbmdcIjpcclxuICBjYXNlIFwiLmdpZlwiOlxyXG4gICAgcmV0dXJuIFwiaW1nLWZpbGVcIjtcclxuICBjYXNlIFwiLmRvY1wiOlxyXG4gIGNhc2UgXCIuZG9jeFwiOlxyXG4gICAgcmV0dXJuIFwiZG9jLWZpbGVcIjtcclxuICBjYXNlIFwiLnhsc1wiOlxyXG4gIGNhc2UgXCIueGxzeFwiOlxyXG4gICAgcmV0dXJuIFwieGxzLWZpbGVcIjtcclxuICBjYXNlIFwiLnBkZlwiOlxyXG4gICAgcmV0dXJuIFwicGRmLWZpbGVcIjtcclxuICBjYXNlIFwiLnppcFwiOlxyXG4gIGNhc2UgXCIucmFyXCI6XHJcbiAgICByZXR1cm4gXCJ6aXAtZmlsZVwiO1xyXG4gIGRlZmF1bHQ6XHJcbiAgICByZXR1cm4gXCJkZWZhdWx0LWZpbGVcIjtcclxuICB9XHJcbn0iXX0=