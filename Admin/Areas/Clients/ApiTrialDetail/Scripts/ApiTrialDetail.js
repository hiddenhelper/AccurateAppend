var readUrl;
var apiTrialDetailViewModel;
$(function () {
    apiTrialDetailViewModel = new AccurateAppend.Websites.Admin.Clients.ApiTrialDetail.ApiTrialDetailViewModel();
    readUrl = $("#readUrl").val();
    $("#extendTrialButton").click(function () {
        apiTrialDetailViewModel.extend($("#maxCalls").val());
    });
    $("#copyAccessIdToClipboard").click(function () {
        $("#AccessId").select();
        document.execCommand("copy");
    });
    apiTrialDetailViewModel.init();
});
var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Admin;
        (function (Admin) {
            var Clients;
            (function (Clients) {
                var ApiTrialDetail;
                (function (ApiTrialDetail) {
                    var ApiTrialDetailViewModel = (function () {
                        function ApiTrialDetailViewModel() {
                        }
                        ApiTrialDetailViewModel.prototype.init = function () {
                            this.load();
                            this.displayMethodCallCounts();
                            this.displayOperationMatchCounts();
                            this.setIsEnabledClick();
                        };
                        ApiTrialDetailViewModel.prototype.load = function () {
                            var self = this;
                            $.ajax({
                                type: "GET",
                                url: "" + readUrl,
                                async: false,
                                success: function (data) {
                                    self.Links = new Links(data);
                                    $("#DateCreated").val(data.DateCreated);
                                    $("#ExpirationDate").val(data.ExpirationDate);
                                    $("#MaximumCalls").val(data.MaximumCalls);
                                    $("#AccessId").val(data.AccessId);
                                    $("#IsEnabled").unbind();
                                    $("#viewSourceLead").attr("href", self.Links.SourceLead);
                                    $("#DefaultEmail").val(data.DefaultEmail);
                                    self.setIsEnabledClick();
                                },
                                error: function (xhr, status, error) {
                                    self.displayMessage("Error: " + xhr.responseText, "danger", "#globalMessage");
                                }
                            });
                        };
                        ApiTrialDetailViewModel.prototype.extend = function (maxCalls) {
                            var self = this;
                            $.ajax({
                                type: "GET",
                                url: self.Links.Extend + "?maximumCalls=" + maxCalls,
                                success: function (status) {
                                    $("#extendTrialModal").modal("hide");
                                    self.load();
                                    self.displayMessage("" + status.Message, "info", "#globalMessage");
                                },
                                error: function (xhr, status, error) {
                                    $("#extendTrialModal").modal("hide");
                                    self.displayMessage("Error extending trail. Message: " + xhr.responseText, "danger", "#globalMessage");
                                }
                            });
                        };
                        ApiTrialDetailViewModel.prototype.disable = function () {
                            var self = this;
                            $.ajax({
                                type: "GET",
                                url: "" + self.Links.Disable,
                                success: function (status) {
                                    $("#extendTrialModal").modal("hide");
                                    self.load();
                                    self.displayMessage("" + status.Message, "info", "#globalMessage");
                                },
                                error: function (xhr, status, error) {
                                    self.displayMessage("Error extending trail. Message: " + xhr.responseText, "danger", "#globalMessage");
                                }
                            });
                        };
                        ApiTrialDetailViewModel.prototype.displayMethodCallCounts = function () {
                            var self = this;
                            var grid = $("#MethodCallCountsGrid").data("kendoGrid");
                            if (grid !== undefined && grid !== null) {
                                grid.dataSource.read();
                            }
                            else {
                                $("#MethodCallCountsGrid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: function (options) {
                                                $.ajax({
                                                    url: "" + self.Links.MethodCallCounts,
                                                    dataType: "json",
                                                    type: "GET",
                                                    success: function (result) {
                                                        options.success(result);
                                                    }
                                                });
                                            },
                                            cache: false
                                        },
                                        schema: {
                                            type: "json",
                                            data: "Data",
                                            total: function (response) {
                                                return response.Data.length;
                                            }
                                        },
                                        change: function () {
                                            if (this.data().length <= 0) {
                                                $("#MethodCallCountsGridMessage").show();
                                                $("#MethodCallCountsGrid").hide();
                                            }
                                            else {
                                                $("#MethodCallCountsGridMessage").hide();
                                                $("#MethodCallCountsGrid").show();
                                            }
                                        }
                                    },
                                    columns: [
                                        {
                                            field: "Description",
                                            title: "Description",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Today",
                                            title: "Today",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Yesterday",
                                            title: "Yesterday",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Last7",
                                            title: "Last 7",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Last30",
                                            title: "Last 30",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        }
                                    ],
                                    scrollable: false
                                });
                            }
                        };
                        ApiTrialDetailViewModel.prototype.displayOperationMatchCounts = function () {
                            var self = this;
                            var grid = $("#OperationMatchCountsGrid").data("kendoGrid");
                            if (grid !== undefined && grid !== null) {
                                grid.dataSource.read();
                            }
                            else {
                                $("#OperationMatchCountsGrid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: function (options) {
                                                $.ajax({
                                                    url: "" + self.Links.OperationMatchCounts,
                                                    dataType: "json",
                                                    type: "GET",
                                                    success: function (result) {
                                                        options.success(result);
                                                    }
                                                });
                                            },
                                            cache: false
                                        },
                                        schema: {
                                            type: "json",
                                            data: "Data",
                                            total: function (response) {
                                                return response.Data.length;
                                            }
                                        },
                                        change: function () {
                                            if (this.data().length <= 0) {
                                                $("#OperationMatchCountsGridMessage").show();
                                                $("#OperationMatchCountsGrid").hide();
                                            }
                                            else {
                                                $("#OperationMatchCountsMessage").hide();
                                                $("#OperationMatchCountsGrid").show();
                                            }
                                        }
                                    },
                                    columns: [
                                        {
                                            field: "Description",
                                            title: "Description",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Today",
                                            title: "Today",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Yesterday",
                                            title: "Yesterday",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Last7",
                                            title: "Last 7",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Last30",
                                            title: "Last 30",
                                            format: "{0:N0}",
                                            headerAttributes: { style: "text-align: right;" },
                                            media: "(min-width: 450px)"
                                        }
                                    ],
                                    scrollable: false
                                });
                            }
                        };
                        ApiTrialDetailViewModel.prototype.displayMessage = function (message, type, div) {
                            $(div).removeClass().addClass("alert alert-" + type).html(message).show()
                                .fadeTo(5000, 500).slideUp(500, function () { $(div).slideUp(500); });
                        };
                        ApiTrialDetailViewModel.prototype.setIsEnabledClick = function () {
                            $("#IsEnabled").change(function () {
                                if (this.checked) {
                                    $("#extendTrialModal").modal("show");
                                }
                                else {
                                    apiTrialDetailViewModel.disable();
                                }
                            });
                        };
                        return ApiTrialDetailViewModel;
                    }());
                    ApiTrialDetail.ApiTrialDetailViewModel = ApiTrialDetailViewModel;
                    var Links = (function () {
                        function Links(data) {
                            this.Disable = data.Links.Disable;
                            this.Extend = data.Links.Extend;
                            this.MethodCallCounts = data.Links.MethodCallCounts;
                            this.OperationMatchCounts = data.Links.OperationMatchCounts;
                            this.SourceLead = data.Links.SourceLead;
                        }
                        return Links;
                    }());
                    ApiTrialDetail.Links = Links;
                })(ApiTrialDetail = Clients.ApiTrialDetail || (Clients.ApiTrialDetail = {}));
            })(Clients = Admin.Clients || (Admin.Clients = {}));
        })(Admin = Websites.Admin || (Websites.Admin = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQXBpVHJpYWxEZXRhaWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJBcGlUcmlhbERldGFpbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFDQSxJQUFJLE9BQVksQ0FBQztBQUNqQixJQUFJLHVCQUFxRyxDQUFDO0FBRTFHLENBQUMsQ0FBQztJQUVBLHVCQUF1QixHQUFHLElBQUksY0FBYyxDQUFDLFFBQVEsQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLGNBQWMsQ0FBQyx1QkFBdUIsRUFBRSxDQUFDO0lBQzdHLE9BQU8sR0FBRyxDQUFDLENBQUMsVUFBVSxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUM7SUFHOUIsQ0FBQyxDQUFDLG9CQUFvQixDQUFDLENBQUMsS0FBSyxDQUFDO1FBQzVCLHVCQUF1QixDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQztJQUN2RCxDQUFDLENBQUMsQ0FBQztJQUVILENBQUMsQ0FBQywwQkFBMEIsQ0FBQyxDQUFDLEtBQUssQ0FBQztRQUNsQyxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsTUFBTSxFQUFFLENBQUM7UUFDeEIsUUFBUSxDQUFDLFdBQVcsQ0FBQyxNQUFNLENBQUMsQ0FBQztJQUMvQixDQUFDLENBQUMsQ0FBQztJQUVILHVCQUF1QixDQUFDLElBQUksRUFBRSxDQUFDO0FBR2pDLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBTyxjQUFjLENBNFFwQjtBQTVRRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFFBQVEsQ0E0UTdCO0lBNVFxQixXQUFBLFFBQVE7UUFBQyxJQUFBLEtBQUssQ0E0UW5DO1FBNVE4QixXQUFBLEtBQUs7WUFBQyxJQUFBLE9BQU8sQ0E0UTNDO1lBNVFvQyxXQUFBLE9BQU87Z0JBQUMsSUFBQSxjQUFjLENBNFExRDtnQkE1UTRDLFdBQUEsY0FBYztvQkFFekQ7d0JBQUE7d0JBd1BBLENBQUM7d0JBcFBDLHNDQUFJLEdBQUo7NEJBQ0Us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O3dCQUNMLENBQUM7d0JBQ0gsOEJBQUM7b0JBQUQsQ0FBQyxBQXhQRCxJQXdQQztvQkF4UFksc0NBQXVCLDBCQXdQbkMsQ0FBQTtvQkFFRDt3QkFPRSxlQUFZLElBQVM7NEJBQ25CLElBQUksQ0FBQyxPQUFPLEdBQUcsSUFBSSxDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUM7NEJBQ2xDLElBQUksQ0FBQyxNQUFNLEdBQUcsSUFBSSxDQUFDLEtBQUssQ0FBQyxNQUFNLENBQUM7NEJBQ2hDLElBQUksQ0FBQyxnQkFBZ0IsR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLGdCQUFnQixDQUFDOzRCQUNwRCxJQUFJLENBQUMsb0JBQW9CLEdBQUcsSUFBSSxDQUFDLEtBQUssQ0FBQyxvQkFBb0IsQ0FBQzs0QkFDNUQsSUFBSSxDQUFDLFVBQVUsR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLFVBQVUsQ0FBQzt3QkFDMUMsQ0FBQzt3QkFDSCxZQUFDO29CQUFELENBQUMsQUFkRCxJQWNDO29CQWRZLG9CQUFLLFFBY2pCLENBQUE7Z0JBRUgsQ0FBQyxFQTVRNEMsY0FBYyxHQUFkLHNCQUFjLEtBQWQsc0JBQWMsUUE0UTFEO1lBQUQsQ0FBQyxFQTVRb0MsT0FBTyxHQUFQLGFBQU8sS0FBUCxhQUFPLFFBNFEzQztRQUFELENBQUMsRUE1UThCLEtBQUssR0FBTCxjQUFLLEtBQUwsY0FBSyxRQTRRbkM7SUFBRCxDQUFDLEVBNVFxQixRQUFRLEdBQVIsdUJBQVEsS0FBUix1QkFBUSxRQTRRN0I7QUFBRCxDQUFDLEVBNVFNLGNBQWMsS0FBZCxjQUFjLFFBNFFwQiIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3MvanF1ZXJ5L2pxdWVyeS5kLnRzXCIgLz5cclxudmFyIHJlYWRVcmw6IGFueTtcclxudmFyIGFwaVRyaWFsRGV0YWlsVmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5DbGllbnRzLkFwaVRyaWFsRGV0YWlsLkFwaVRyaWFsRGV0YWlsVmlld01vZGVsO1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIGFwaVRyaWFsRGV0YWlsVmlld01vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkFkbWluLkNsaWVudHMuQXBpVHJpYWxEZXRhaWwuQXBpVHJpYWxEZXRhaWxWaWV3TW9kZWwoKTtcclxuICByZWFkVXJsID0gJChcIiNyZWFkVXJsXCIpLnZhbCgpO1xyXG5cclxuXHJcbiAgJChcIiNleHRlbmRUcmlhbEJ1dHRvblwiKS5jbGljaygoKSA9PiB7XHJcbiAgICBhcGlUcmlhbERldGFpbFZpZXdNb2RlbC5leHRlbmQoJChcIiNtYXhDYWxsc1wiKS52YWwoKSk7XHJcbiAgfSk7XHJcblxyXG4gICQoXCIjY29weUFjY2Vzc0lkVG9DbGlwYm9hcmRcIikuY2xpY2soKCkgPT4ge1xyXG4gICAgJChcIiNBY2Nlc3NJZFwiKS5zZWxlY3QoKTtcclxuICAgIGRvY3VtZW50LmV4ZWNDb21tYW5kKFwiY29weVwiKTtcclxuICB9KTtcclxuXHJcbiAgYXBpVHJpYWxEZXRhaWxWaWV3TW9kZWwuaW5pdCgpO1xyXG5cclxuXHJcbn0pO1xyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkFkbWluLkNsaWVudHMuQXBpVHJpYWxEZXRhaWwge1xyXG5cclxuICBleHBvcnQgY2xhc3MgQXBpVHJpYWxEZXRhaWxWaWV3TW9kZWwge1xyXG5cclxuICAgIExpbmtzOiBMaW5rcztcclxuXHJcbiAgICBpbml0KCkge1xyXG4gICAgICB0aGlzLmxvYWQoKTtcclxuICAgICAgdGhpcy5kaXNwbGF5TWV0aG9kQ2FsbENvdW50cygpO1xyXG4gICAgICB0aGlzLmRpc3BsYXlPcGVyYXRpb25NYXRjaENvdW50cygpO1xyXG4gICAgICB0aGlzLnNldElzRW5hYmxlZENsaWNrKCk7XHJcbiAgICB9XHJcblxyXG4gICAgbG9hZCgpIHtcclxuICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQuYWpheChcclxuICAgICAgICB7XHJcbiAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgdXJsOiBgJHtyZWFkVXJsfWAsXHJcbiAgICAgICAgICBhc3luYzogZmFsc2UsXHJcbiAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgc2VsZi5MaW5rcyA9IG5ldyBMaW5rcyhkYXRhKTtcclxuICAgICAgICAgICAgJChcIiNEYXRlQ3JlYXRlZFwiKS52YWwoZGF0YS5EYXRlQ3JlYXRlZCk7XHJcbiAgICAgICAgICAgICQoXCIjRXhwaXJhdGlvbkRhdGVcIikudmFsKGRhdGEuRXhwaXJhdGlvbkRhdGUpO1xyXG4gICAgICAgICAgICAkKFwiI01heGltdW1DYWxsc1wiKS52YWwoZGF0YS5NYXhpbXVtQ2FsbHMpO1xyXG4gICAgICAgICAgICAkKFwiI0FjY2Vzc0lkXCIpLnZhbChkYXRhLkFjY2Vzc0lkKTtcclxuICAgICAgICAgICAgJChcIiNJc0VuYWJsZWRcIikudW5iaW5kKCk7XHJcbiAgICAgICAgICAgICQoXCIjdmlld1NvdXJjZUxlYWRcIikuYXR0cihcImhyZWZcIiwgc2VsZi5MaW5rcy5Tb3VyY2VMZWFkKTtcclxuICAgICAgICAgICAgJChcIiNEZWZhdWx0RW1haWxcIikudmFsKGRhdGEuRGVmYXVsdEVtYWlsKTtcclxuICAgICAgICAgICAgc2VsZi5zZXRJc0VuYWJsZWRDbGljaygpO1xyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGVycm9yKHhociwgc3RhdHVzLCBlcnJvcikge1xyXG4gICAgICAgICAgICBzZWxmLmRpc3BsYXlNZXNzYWdlKGBFcnJvcjogJHt4aHIucmVzcG9uc2VUZXh0fWAsIFwiZGFuZ2VyXCIsIFwiI2dsb2JhbE1lc3NhZ2VcIik7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgZXh0ZW5kKG1heENhbGxzKSB7XHJcbiAgICAgIGNvbnN0IHNlbGYgPSB0aGlzO1xyXG4gICAgICAkLmFqYXgoXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgIHVybDogYCR7c2VsZi5MaW5rcy5FeHRlbmR9P21heGltdW1DYWxscz0ke21heENhbGxzfWAsXHJcbiAgICAgICAgICBzdWNjZXNzKHN0YXR1cykge1xyXG4gICAgICAgICAgICAkKFwiI2V4dGVuZFRyaWFsTW9kYWxcIikubW9kYWwoXCJoaWRlXCIpO1xyXG4gICAgICAgICAgICAvLyQoXCIjSXNFbmFibGVkXCIpLnVuYmluZCgpO1xyXG4gICAgICAgICAgICBzZWxmLmxvYWQoKTtcclxuICAgICAgICAgICAgLy9zZWxmLnNldElzRW5hYmxlZENsaWNrKCk7XHJcbiAgICAgICAgICAgIHNlbGYuZGlzcGxheU1lc3NhZ2UoYCR7c3RhdHVzLk1lc3NhZ2V9YCwgXCJpbmZvXCIsIFwiI2dsb2JhbE1lc3NhZ2VcIik7XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyLCBzdGF0dXMsIGVycm9yKSB7XHJcbiAgICAgICAgICAgICQoXCIjZXh0ZW5kVHJpYWxNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICAgICAgIHNlbGYuZGlzcGxheU1lc3NhZ2UoYEVycm9yIGV4dGVuZGluZyB0cmFpbC4gTWVzc2FnZTogJHt4aHIucmVzcG9uc2VUZXh0fWAsIFwiZGFuZ2VyXCIsIFwiI2dsb2JhbE1lc3NhZ2VcIik7XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgZGlzYWJsZSgpIHtcclxuICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQuYWpheChcclxuICAgICAgICB7XHJcbiAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgdXJsOiBgJHtzZWxmLkxpbmtzLkRpc2FibGV9YCxcclxuICAgICAgICAgIHN1Y2Nlc3Moc3RhdHVzKSB7XHJcbiAgICAgICAgICAgICQoXCIjZXh0ZW5kVHJpYWxNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICAgICAgIC8vJChcIiNJc0VuYWJsZWRcIikudW5iaW5kKCk7XHJcbiAgICAgICAgICAgIHNlbGYubG9hZCgpO1xyXG4gICAgICAgICAgICAvL3NlbGYuc2V0SXNFbmFibGVkQ2xpY2soKTtcclxuICAgICAgICAgICAgc2VsZi5kaXNwbGF5TWVzc2FnZShgJHtzdGF0dXMuTWVzc2FnZX1gLCBcImluZm9cIiwgXCIjZ2xvYmFsTWVzc2FnZVwiKTtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgICAgc2VsZi5kaXNwbGF5TWVzc2FnZShgRXJyb3IgZXh0ZW5kaW5nIHRyYWlsLiBNZXNzYWdlOiAke3hoci5yZXNwb25zZVRleHR9YCwgXCJkYW5nZXJcIiwgXCIjZ2xvYmFsTWVzc2FnZVwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBkaXNwbGF5TWV0aG9kQ2FsbENvdW50cygpIHtcclxuICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XHJcbiAgICAgIGNvbnN0IGdyaWQgPSAkKFwiI01ldGhvZENhbGxDb3VudHNHcmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgIGlmIChncmlkICE9PSB1bmRlZmluZWQgJiYgZ3JpZCAhPT0gbnVsbCkge1xyXG4gICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiNNZXRob2RDYWxsQ291bnRzR3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICB1cmw6IGAke3NlbGYuTGlua3MuTWV0aG9kQ2FsbENvdW50c31gLFxyXG4gICAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgY2FjaGU6IGZhbHNlXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBjaGFuZ2UoKSB7XHJcbiAgICAgICAgICAgICAgaWYgKHRoaXMuZGF0YSgpLmxlbmd0aCA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiI01ldGhvZENhbGxDb3VudHNHcmlkTWVzc2FnZVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI01ldGhvZENhbGxDb3VudHNHcmlkXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNNZXRob2RDYWxsQ291bnRzR3JpZE1lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNNZXRob2RDYWxsQ291bnRzR3JpZFwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJUb2RheVwiLFxyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlllc3RlcmRheVwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlllc3RlcmRheVwiLFxyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkxhc3Q3XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCA3XCIsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdDMwXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCAzMFwiLFxyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgXSxcclxuICAgICAgICAgIHNjcm9sbGFibGU6IGZhbHNlXHJcbiAgICAgICAgfSk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBkaXNwbGF5T3BlcmF0aW9uTWF0Y2hDb3VudHMoKSB7XHJcbiAgICAgIGNvbnN0IHNlbGYgPSB0aGlzO1xyXG4gICAgICBjb25zdCBncmlkID0gJChcIiNPcGVyYXRpb25NYXRjaENvdW50c0dyaWRcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgICAgZ3JpZC5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICAkKFwiI09wZXJhdGlvbk1hdGNoQ291bnRzR3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICB1cmw6IGAke3NlbGYuTGlua3MuT3BlcmF0aW9uTWF0Y2hDb3VudHN9YCxcclxuICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgIGNhY2hlOiBmYWxzZVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgY2hhbmdlKCkge1xyXG4gICAgICAgICAgICAgIGlmICh0aGlzLmRhdGEoKS5sZW5ndGggPD0gMCkge1xyXG4gICAgICAgICAgICAgICAgJChcIiNPcGVyYXRpb25NYXRjaENvdW50c0dyaWRNZXNzYWdlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjT3BlcmF0aW9uTWF0Y2hDb3VudHNHcmlkXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNPcGVyYXRpb25NYXRjaENvdW50c01lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNPcGVyYXRpb25NYXRjaENvdW50c0dyaWRcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkRlc2NyaXB0aW9uXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlRvZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJZZXN0ZXJkYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJZZXN0ZXJkYXlcIixcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJMYXN0N1wiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgN1wiLFxyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkxhc3QzMFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgMzBcIixcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIF0sXHJcbiAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZVxyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgZGlzcGxheU1lc3NhZ2UobWVzc2FnZTogYW55LCB0eXBlOiBhbnksIGRpdjogYW55KSB7XHJcbiAgICAgICQoZGl2KS5yZW1vdmVDbGFzcygpLmFkZENsYXNzKGBhbGVydCBhbGVydC0ke3R5cGV9YCkuaHRtbChtZXNzYWdlKS5zaG93KClcclxuICAgICAgICAuZmFkZVRvKDUwMDAsIDUwMCkuc2xpZGVVcCg1MDAsICgpID0+IHsgJChkaXYpLnNsaWRlVXAoNTAwKSB9KTtcclxuICAgIH1cclxuXHJcbiAgICBzZXRJc0VuYWJsZWRDbGljaygpIHtcclxuICAgICAgJChcIiNJc0VuYWJsZWRcIikuY2hhbmdlKGZ1bmN0aW9uKCkge1xyXG4gICAgICAgIGlmICh0aGlzLmNoZWNrZWQpIHtcclxuICAgICAgICAgICQoXCIjZXh0ZW5kVHJpYWxNb2RhbFwiKS5tb2RhbChcInNob3dcIik7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgIGFwaVRyaWFsRGV0YWlsVmlld01vZGVsLmRpc2FibGUoKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZXhwb3J0IGNsYXNzIExpbmtzIHtcclxuICAgIERpc2FibGU6IHN0cmluZztcclxuICAgIEV4dGVuZDogc3RyaW5nO1xyXG4gICAgTWV0aG9kQ2FsbENvdW50czogc3RyaW5nO1xyXG4gICAgT3BlcmF0aW9uTWF0Y2hDb3VudHM6IHN0cmluZztcclxuICAgIFNvdXJjZUxlYWQ6IHN0cmluZztcclxuXHJcbiAgICBjb25zdHJ1Y3RvcihkYXRhOiBhbnkpIHtcclxuICAgICAgdGhpcy5EaXNhYmxlID0gZGF0YS5MaW5rcy5EaXNhYmxlO1xyXG4gICAgICB0aGlzLkV4dGVuZCA9IGRhdGEuTGlua3MuRXh0ZW5kO1xyXG4gICAgICB0aGlzLk1ldGhvZENhbGxDb3VudHMgPSBkYXRhLkxpbmtzLk1ldGhvZENhbGxDb3VudHM7XHJcbiAgICAgIHRoaXMuT3BlcmF0aW9uTWF0Y2hDb3VudHMgPSBkYXRhLkxpbmtzLk9wZXJhdGlvbk1hdGNoQ291bnRzO1xyXG4gICAgICB0aGlzLlNvdXJjZUxlYWQgPSBkYXRhLkxpbmtzLlNvdXJjZUxlYWQ7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxufSJdfQ==