var queryUrl;
var selectUrl;
var nextUrl;
var viewModel;
$(function () {
    viewModel = new SelectRule.ViewModel();
    viewModel.renderGrid(queryUrl);
    $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
    $(".k-grid .k-grid-header").hide();
});
var SelectRule;
(function (SelectRule) {
    var ViewModel = (function () {
        function ViewModel() {
        }
        ViewModel.prototype.renderGrid = function () {
            var grid = $("#grid").data("kendoGrid");
            if (grid !== undefined && grid !== null) {
                grid.dataSource.read();
            }
            else {
                $("#grid").kendoGrid({
                    dataSource: {
                        type: "json",
                        transport: {
                            read: function (options) {
                                $.ajax({
                                    url: queryUrl,
                                    dataType: "json",
                                    type: "GET",
                                    data: {},
                                    success: function (result) {
                                        options.success(result);
                                    }
                                });
                            },
                            cache: false
                        },
                        pageSize: 5,
                        schema: {
                            type: "json",
                            data: "Data",
                            total: function (response) {
                                return response.Data.length;
                            }
                        },
                        change: function () {
                            if (this.data().length <= 0) {
                                $("#message").show();
                                $("#grid").hide();
                                $("#pager").hide();
                            }
                            else {
                                $("#message").hide();
                                $("#grid").show();
                                $("#pager").show();
                                $("#warning").hide();
                            }
                        }
                    },
                    scrollable: false,
                    filterable: false,
                    pageable: true,
                    columns: [
                        {
                            title: "Description",
                            template: kendo.template($("#grid-description-column-template").html()),
                            media: "(min-width: 450px)"
                        },
                        {
                            width: 200,
                            template: kendo.template($("#grid-commandButton-column-template").html()),
                            media: "(min-width: 450px)",
                            attributes: { style: "text-align: center;" },
                            headerAttributes: { style: "text-align: center;" }
                        }
                    ]
                });
            }
        };
        ViewModel.prototype.selectManifest = function (url) {
            console.log("accessing manifest detail at " + url);
            $.ajax({
                type: "GET",
                url: url,
                success: function (data) {
                    $.ajax({
                        type: "POST",
                        url: selectUrl,
                        data: { ManifestContent: data.Manifest },
                        success: function (data) {
                            console.log("Manifest posted to controller");
                        },
                        error: function (xhr, status, error) {
                            alert(xhr.statusText);
                        }
                    });
                    location.href = nextUrl;
                },
                error: function (xhr, status, error) {
                    alert(xhr.statusText);
                }
            });
        };
        return ViewModel;
    }());
    SelectRule.ViewModel = ViewModel;
})(SelectRule || (SelectRule = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiU2VsZWN0UnVsZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIlNlbGVjdFJ1bGUudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBRUEsSUFBSSxRQUFhLENBQUM7QUFDbEIsSUFBSSxTQUFjLENBQUM7QUFDbkIsSUFBSSxPQUFZLENBQUM7QUFDakIsSUFBSSxTQUFjLENBQUM7QUFFbkIsQ0FBQyxDQUFDO0lBRUEsU0FBUyxHQUFHLElBQUksVUFBVSxDQUFDLFNBQVMsRUFBRSxDQUFDO0lBQ3ZDLFNBQVMsQ0FBQyxVQUFVLENBQUMsUUFBUSxDQUFDLENBQUM7SUFHL0IsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLElBQUksQ0FBQyxtQkFBbUIsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxFQUFFLGVBQWUsRUFBRSxDQUFDLEVBQUUsQ0FBQyxDQUFDO0lBQ2pFLENBQUMsQ0FBQyx3QkFBd0IsQ0FBQyxDQUFDLElBQUksRUFBRSxDQUFDO0FBQ3JDLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBVSxVQUFVLENBd0duQjtBQXhHRCxXQUFVLFVBQVU7SUFDbEI7UUFBQTtRQXNHQSxDQUFDO1FBbkdDLDhCQUFVLEdBQVY7WUFFRSxJQUFNLElBQUksR0FBRyxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxDQUFDO1lBQzFDLElBQUksSUFBSSxLQUFLLFNBQVMsSUFBSSxJQUFJLEtBQUssSUFBSSxFQUFFO2dCQUN2QyxJQUFJLENBQUMsVUFBVSxDQUFDLElBQUksRUFBRSxDQUFDO2FBQ3hCO2lCQUFNO2dCQUNMLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxTQUFTLENBQUM7b0JBQ25CLFVBQVUsRUFBRTt3QkFDVixJQUFJLEVBQUUsTUFBTTt3QkFDWixTQUFTLEVBQUU7NEJBQ1QsSUFBSSxZQUFDLE9BQU87Z0NBQ1YsQ0FBQyxDQUFDLElBQUksQ0FBQztvQ0FDTCxHQUFHLEVBQUUsUUFBUTtvQ0FDYixRQUFRLEVBQUUsTUFBTTtvQ0FD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QUs7b0JBQ3RCLEtBQUssQ0FBQyxHQUFHLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQ3hCLENBQUM7YUFDRixDQUFDLENBQUM7UUFDUCxDQUFDO1FBQ0gsZ0JBQUM7SUFBRCxDQUFDLEFBdEdELElBc0dDO0lBdEdZLG9CQUFTLFlBc0dyQixDQUFBO0FBQ0gsQ0FBQyxFQXhHUyxVQUFVLEtBQVYsVUFBVSxRQXdHbkIiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vU2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxudmFyIHF1ZXJ5VXJsOiBhbnk7XHJcbnZhciBzZWxlY3RVcmw6IGFueTtcclxudmFyIG5leHRVcmw6IGFueTtcclxudmFyIHZpZXdNb2RlbDogYW55O1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIHZpZXdNb2RlbCA9IG5ldyBTZWxlY3RSdWxlLlZpZXdNb2RlbCgpO1xyXG4gIHZpZXdNb2RlbC5yZW5kZXJHcmlkKHF1ZXJ5VXJsKTtcclxuXHJcbiAgLy8gcmVtb3ZlcyBleHRyYSBtYXJnaW4gYXBwZWFyaW5nIGF0IGJvdHRvbSBvZiBncmlkXHJcbiAgJChcIiNncmlkXCIpLmZpbmQoXCJbZGF0YS1yb2xlPXBhZ2VyXVwiKS5jc3MoeyBcIm1hcmdpbi1ib3R0b21cIjogMCB9KTtcclxuICAkKFwiLmstZ3JpZCAuay1ncmlkLWhlYWRlclwiKS5oaWRlKCk7XHJcbn0pO1xyXG5cclxubmFtZXNwYWNlIFNlbGVjdFJ1bGUge1xyXG4gIGV4cG9ydCBjbGFzcyBWaWV3TW9kZWwge1xyXG4gICAgdXJsOiBzdHJpbmc7XHJcblxyXG4gICAgcmVuZGVyR3JpZCgpIHtcclxuXHJcbiAgICAgIGNvbnN0IGdyaWQgPSAkKFwiI2dyaWRcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgICAgZ3JpZC5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICAkKFwiI2dyaWRcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgdXJsOiBxdWVyeVVybCxcclxuICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICBkYXRhOiB7fSxcclxuICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICBjYWNoZTogZmFsc2VcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgcGFnZVNpemU6IDUsXHJcbiAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBjaGFuZ2U6IGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAgIGlmICh0aGlzLmRhdGEoKS5sZW5ndGggPD0gMCkge1xyXG4gICAgICAgICAgICAgICAgJChcIiNtZXNzYWdlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI3BhZ2VyXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNtZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICQoXCIjZ3JpZFwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI3BhZ2VyXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjd2FybmluZ1wiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICBmaWx0ZXJhYmxlOiBmYWxzZSxcclxuICAgICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNncmlkLWRlc2NyaXB0aW9uLWNvbHVtbi10ZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIC8ve1xyXG4gICAgICAgICAgICAvLyAgZmllbGQ6IFwiTGFzdFVzZWRcIixcclxuICAgICAgICAgICAgLy8gIHRpdGxlOiBcIkxhc3QgVXNlZFwiLFxyXG4gICAgICAgICAgICAvLyAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCIsXHJcbiAgICAgICAgICAgIC8vICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAvLyAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfVxyXG4gICAgICAgICAgICAvL30sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICB3aWR0aDogMjAwLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI2dyaWQtY29tbWFuZEJ1dHRvbi1jb2x1bW4tdGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgXVxyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgc2VsZWN0TWFuaWZlc3QodXJsKSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKGBhY2Nlc3NpbmcgbWFuaWZlc3QgZGV0YWlsIGF0ICR7dXJsfWApO1xyXG4gICAgICAkLmFqYXgoXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgIHVybDogdXJsLFxyXG4gICAgICAgICAgc3VjY2VzcyhkYXRhKSB7XHJcbiAgICAgICAgICAgICQuYWpheChcclxuICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIlBPU1RcIixcclxuICAgICAgICAgICAgICAgIHVybDogc2VsZWN0VXJsLFxyXG4gICAgICAgICAgICAgICAgZGF0YTogeyBNYW5pZmVzdENvbnRlbnQ6IGRhdGEuTWFuaWZlc3QgfSxcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZyhcIk1hbmlmZXN0IHBvc3RlZCB0byBjb250cm9sbGVyXCIpO1xyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGVycm9yKHhociwgc3RhdHVzLCBlcnJvcikge1xyXG4gICAgICAgICAgICAgICAgICBhbGVydCh4aHIuc3RhdHVzVGV4dCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICBsb2NhdGlvbi5ocmVmID0gbmV4dFVybDtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgICAgYWxlcnQoeGhyLnN0YXR1c1RleHQpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG4gIH1cclxufSJdfQ==