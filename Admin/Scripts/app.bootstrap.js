$(function () {
    AccurateAppend.Ui.ApplicationId.load();
    AccurateAppend.Bootstrap.updateUserStatus();
    AccurateAppend.Bootstrap.updateDealSidebar();
    window.setInterval(AccurateAppend.Bootstrap.updateDealSidebar, 60000);
    window.setInterval(AccurateAppend.Bootstrap.updateUserStatus, 60000);
});
var AccurateAppend;
(function (AccurateAppend) {
    var Bootstrap;
    (function (Bootstrap) {
        function updateUserStatus() {
            $.ajax({
                type: "GET",
                async: true,
                url: "/Operations/UserStatus/ActivitySummary",
                success: function (result) {
                    var html = "";
                    $.each(result, function (i, v) {
                        html += "<li><i class=\"fa fa-user\" ";
                        switch (v.Status) {
                            case "Online":
                                html += "style=\"color:green;\"";
                            case "Away":
                                html += "style=\"color:gold;\"";
                            case "Offline":
                                html += "style=\"color:black;\"";
                        }
                        html += "></i> ";
                        html += "<span class=\"fa fa-user;\"></span>" + v.UserName;
                        if (v.LastActivityDescription != '')
                            html += "<span style=\"margin-left: 13px; font-size: .8em; display:block;color:#d0d0d0;\">" + v.LastActivityDescription + "</span>";
                        html += "</li>";
                    });
                    $(".userStatus").html(html);
                }
            });
        }
        Bootstrap.updateUserStatus = updateUserStatus;
        function updateDealSidebar() {
            if ($('#ApplicationId').val() == undefined) {
                $("#dealActivity").hide();
            }
            else {
                $.ajax({
                    type: "GET",
                    async: true,
                    url: "/Sales/DealActivity/Query",
                    success: function (data) {
                        if (data.length === 0) {
                            $("#dealActivity .alert").remove();
                            $("#dealActivity").append('<div class="alert alert-info">No deals found</div>');
                        }
                        else {
                            $("#dealActivity .alert").remove();
                            $("#dealActivity ul").remove();
                            $("#dealActivity").append('<ul style="padding: 0 0 0 10px;" class="activity"></ul>');
                            $.each(data, function (i, e) {
                                $("#dealActivity ul").append("<li><a href=\"" + e.Links.DetailView + "\">" + e.StatusDescription + " (" + e.Count + ")</a></li>");
                            });
                        }
                    }
                });
            }
        }
        Bootstrap.updateDealSidebar = updateDealSidebar;
    })(Bootstrap = AccurateAppend.Bootstrap || (AccurateAppend.Bootstrap = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYXBwLmJvb3RzdHJhcC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbImFwcC5ib290c3RyYXAudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBRUEsQ0FBQyxDQUFDO0lBRUUsY0FBYyxDQUFDLEVBQUUsQ0FBQyxhQUFhLENBQUMsSUFBSSxFQUFFLENBQUM7SUFFdkMsY0FBYyxDQUFDLFNBQVMsQ0FBQyxnQkFBZ0IsRUFBRSxDQUFDO0lBQzVDLGNBQWMsQ0FBQyxTQUFTLENBQUMsaUJBQWlCLEVBQUUsQ0FBQztJQUM3QyxNQUFNLENBQUMsV0FBVyxDQUFDLGNBQWMsQ0FBQyxTQUFTLENBQUMsaUJBQWlCLEVBQUUsS0FBSyxDQUFDLENBQUM7SUFDdEUsTUFBTSxDQUFDLFdBQVcsQ0FBQyxjQUFjLENBQUMsU0FBUyxDQUFDLGdCQUFnQixFQUFFLEtBQUssQ0FBQyxDQUFDO0FBRXpFLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBTyxjQUFjLENBOERwQjtBQTlERCxXQUFPLGNBQWM7SUFBQyxJQUFBLFNBQVMsQ0E4RDlCO0lBOURxQixXQUFBLFNBQVM7UUFFM0IsU0FBZ0IsZ0JBQWdCO1lBQzVCLENBQUMsQ0FBQyxJQUFJLENBQ0Y7Z0JBQ0ksSUFBSSxFQUFFLEtBQUs7Z0JBQ1gsS0FBSyxFQUFFLElBQUk7Z0JBQ1gsR0FBRyxFQUFFLHdDQUF3QztnQkFDN0MsT0FBTyxFQUFFLFVBQUEsTUFBTTtvQkFDWCxJQUFJLElBQUksR0FBRyxFQUFFLENBQUM7b0JBQ2QsQ0FBQyxDQUFDLElBQUksQ0FBQyxNQUFNLEVBQUUsVUFBQyxDQUFDLEVBQUUsQ0FBQzt3QkFFaEIsSUFBSSxJQUFJLDhCQUE4QixDQUFDO3dCQUN2QyxRQUFRLENBQUMsQ0FBQyxNQUFNLEVBQUU7NEJBQ2xCLEtBQUssUUFBUTtnQ0FDVCxJQUFJLElBQUksd0JBQXdCLENBQUM7NEJBQ3JDLEtBQUssTUFBTTtnQ0FDUCxJQUFJLElBQUksdUJBQXVCLENBQUM7NEJBQ3BDLEtBQUssU0FBUztnQ0FDVixJQUFJLElBQUksd0JBQXdCLENBQUM7eUJBQ3BDO3dCQUNELElBQUksSUFBSSxRQUFRLENBQUM7d0JBQ2pCLElBQUksSUFBSSxxQ0FBcUMsR0FBRyxDQUFDLENBQUMsUUFBUSxDQUFDO3dCQUMzRCxJQUFJLENBQUMsQ0FBQyx1QkFBdUIsSUFBSSxFQUFFOzRCQUFFLElBQUksSUFBSSxtRkFBbUYsR0FBRyxDQUFDLENBQUMsdUJBQXVCLEdBQUcsU0FBUyxDQUFDO3dCQUN6SyxJQUFJLElBQUksT0FBTyxDQUFDO29CQUNwQixDQUFDLENBQUMsQ0FBQztvQkFFSCxDQUFDLENBQUMsYUFBYSxDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxDQUFDO2dCQUNoQyxDQUFDO2FBQ0osQ0FBQyxDQUFDO1FBQ1gsQ0FBQztRQTVCZSwwQkFBZ0IsbUJBNEIvQixDQUFBO1FBRUQsU0FBZ0IsaUJBQWlCO1lBRzdCLElBQUksQ0FBQyxDQUFDLGdCQUFnQixDQUFDLENBQUMsR0FBRyxFQUFFLElBQUksU0FBUyxFQUFFO2dCQUN4QyxDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7YUFDN0I7aUJBQU07Z0JBQ0gsQ0FBQyxDQUFDLElBQUksQ0FDRjtvQkFDSSxJQUFJLEVBQUUsS0FBSztvQkFDWCxLQUFLLEVBQUUsSUFBSTtvQkFDWCxHQUFHLEVBQUUsMkJBQTJCO29CQUNoQyxPQUFPLEVBQUUsVUFBQSxJQUFJO3dCQUNULElBQUksSUFBSSxDQUFDLE1BQU0sS0FBSyxDQUFDLEVBQUU7NEJBQ25CLENBQUMsQ0FBQyxzQkFBc0IsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDOzRCQUNuQyxDQUFDLENBQUMsZUFBZSxDQUFDLENBQUMsTUFBTSxDQUFDLG9EQUFvRCxDQUFDLENBQUM7eUJBQ25GOzZCQUFNOzRCQUNILENBQUMsQ0FBQyxzQkFBc0IsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDOzRCQUNuQyxDQUFDLENBQUMsa0JBQWtCLENBQUMsQ0FBQyxNQUFNLEVBQUUsQ0FBQzs0QkFDL0IsQ0FBQyxDQUFDLGVBQWUsQ0FBQyxDQUFDLE1BQU0sQ0FBQyx5REFBeUQsQ0FBQyxDQUFDOzRCQUNyRixDQUFDLENBQUMsSUFBSSxDQUFDLElBQUksRUFBRSxVQUFDLENBQUMsRUFBRSxDQUFDO2dDQUNkLENBQUMsQ0FBQyxrQkFBa0IsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxtQkFBZ0IsQ0FBQyxDQUFDLEtBQUssQ0FBQyxVQUFVLFdBQUssQ0FBQyxDQUFDLGlCQUFpQixVQUFLLENBQUMsQ0FBQyxLQUFLLGVBQVksQ0FBQyxDQUFDOzRCQUNySCxDQUFDLENBQUMsQ0FBQzt5QkFDTjtvQkFDTCxDQUFDO2lCQUNKLENBQUMsQ0FBQzthQUNWO1FBQ0wsQ0FBQztRQTFCZSwyQkFBaUIsb0JBMEJoQyxDQUFBO0lBSUwsQ0FBQyxFQTlEcUIsU0FBUyxHQUFULHdCQUFTLEtBQVQsd0JBQVMsUUE4RDlCO0FBQUQsQ0FBQyxFQTlETSxjQUFjLEtBQWQsY0FBYyxRQThEcEIiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwidHlwaW5ncy9qcXVlcnkvanF1ZXJ5LmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiQWNjdXJhdGVBcHBlbmQuVWkudHNcIiAvPlxyXG4kKCgpID0+IHtcclxuXHJcbiAgICBBY2N1cmF0ZUFwcGVuZC5VaS5BcHBsaWNhdGlvbklkLmxvYWQoKTtcclxuXHJcbiAgICBBY2N1cmF0ZUFwcGVuZC5Cb290c3RyYXAudXBkYXRlVXNlclN0YXR1cygpO1xyXG4gICAgQWNjdXJhdGVBcHBlbmQuQm9vdHN0cmFwLnVwZGF0ZURlYWxTaWRlYmFyKCk7XHJcbiAgICB3aW5kb3cuc2V0SW50ZXJ2YWwoQWNjdXJhdGVBcHBlbmQuQm9vdHN0cmFwLnVwZGF0ZURlYWxTaWRlYmFyLCA2MDAwMCk7XHJcbiAgICB3aW5kb3cuc2V0SW50ZXJ2YWwoQWNjdXJhdGVBcHBlbmQuQm9vdHN0cmFwLnVwZGF0ZVVzZXJTdGF0dXMsIDYwMDAwKTtcclxuXHJcbn0pO1xyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLkJvb3RzdHJhcCB7XHJcblxyXG4gICAgZXhwb3J0IGZ1bmN0aW9uIHVwZGF0ZVVzZXJTdGF0dXMoKSB7XHJcbiAgICAgICAgJC5hamF4KFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgYXN5bmM6IHRydWUsXHJcbiAgICAgICAgICAgICAgICB1cmw6IFwiL09wZXJhdGlvbnMvVXNlclN0YXR1cy9BY3Rpdml0eVN1bW1hcnlcIixcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3M6IHJlc3VsdCA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmFyIGh0bWwgPSBcIlwiO1xyXG4gICAgICAgICAgICAgICAgICAgICQuZWFjaChyZXN1bHQsIChpLCB2KSA9PiB7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBodG1sICs9IFwiPGxpPjxpIGNsYXNzPVxcXCJmYSBmYS11c2VyXFxcIiBcIjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgc3dpdGNoICh2LlN0YXR1cykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjYXNlIFwiT25saW5lXCI6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBodG1sICs9IFwic3R5bGU9XFxcImNvbG9yOmdyZWVuO1xcXCJcIjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2FzZSBcIkF3YXlcIjpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGh0bWwgKz0gXCJzdHlsZT1cXFwiY29sb3I6Z29sZDtcXFwiXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNhc2UgXCJPZmZsaW5lXCI6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBodG1sICs9IFwic3R5bGU9XFxcImNvbG9yOmJsYWNrO1xcXCJcIjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBodG1sICs9IFwiPjwvaT4gXCI7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGh0bWwgKz0gXCI8c3BhbiBjbGFzcz1cXFwiZmEgZmEtdXNlcjtcXFwiPjwvc3Bhbj5cIiArIHYuVXNlck5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh2Lkxhc3RBY3Rpdml0eURlc2NyaXB0aW9uICE9ICcnKSBodG1sICs9IFwiPHNwYW4gc3R5bGU9XFxcIm1hcmdpbi1sZWZ0OiAxM3B4OyBmb250LXNpemU6IC44ZW07IGRpc3BsYXk6YmxvY2s7Y29sb3I6I2QwZDBkMDtcXFwiPlwiICsgdi5MYXN0QWN0aXZpdHlEZXNjcmlwdGlvbiArIFwiPC9zcGFuPlwiO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBodG1sICs9IFwiPC9saT5cIjtcclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgJChcIi51c2VyU3RhdHVzXCIpLmh0bWwoaHRtbCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIGV4cG9ydCBmdW5jdGlvbiB1cGRhdGVEZWFsU2lkZWJhcigpIHtcclxuXHJcbiAgICAgICAgLy8gaGlkZSBpZiBwYWdlIGRvZXMgbm90IGNvbnRhaW4gQXBwbGljYXRpb24gZHJvcCBkb3duXHJcbiAgICAgICAgaWYgKCQoJyNBcHBsaWNhdGlvbklkJykudmFsKCkgPT0gdW5kZWZpbmVkKSB7XHJcbiAgICAgICAgICAgICQoXCIjZGVhbEFjdGl2aXR5XCIpLmhpZGUoKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAkLmFqYXgoXHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgICBhc3luYzogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICB1cmw6IFwiL1NhbGVzL0RlYWxBY3Rpdml0eS9RdWVyeVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3M6IGRhdGEgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZGVhbEFjdGl2aXR5IC5hbGVydFwiKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZGVhbEFjdGl2aXR5XCIpLmFwcGVuZCgnPGRpdiBjbGFzcz1cImFsZXJ0IGFsZXJ0LWluZm9cIj5ObyBkZWFscyBmb3VuZDwvZGl2PicpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNkZWFsQWN0aXZpdHkgLmFsZXJ0XCIpLnJlbW92ZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNkZWFsQWN0aXZpdHkgdWxcIikucmVtb3ZlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2RlYWxBY3Rpdml0eVwiKS5hcHBlbmQoJzx1bCBzdHlsZT1cInBhZGRpbmc6IDAgMCAwIDEwcHg7XCIgY2xhc3M9XCJhY3Rpdml0eVwiPjwvdWw+Jyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkLmVhY2goZGF0YSwgKGksIGUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2RlYWxBY3Rpdml0eSB1bFwiKS5hcHBlbmQoYDxsaT48YSBocmVmPVwiJHtlLkxpbmtzLkRldGFpbFZpZXd9XCI+JHtlLlN0YXR1c0Rlc2NyaXB0aW9ufSAoJHtlLkNvdW50fSk8L2E+PC9saT5gKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuXHJcbiAgICBcclxufSJdfQ==