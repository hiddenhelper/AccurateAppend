var ticketsApiViewModel;
var TicketsApiDateRangeWidget;
$(function () {
    ticketsApiViewModel = new AccurateAppend.Websites.Admin.Operations.TicketsApi.TicketsApiViewModel();
    TicketsApiDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("ticketsDateRange", new AccurateAppend.Ui.DateRangeWidgetSettings([
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Last60Days,
        AccurateAppend.Ui.DateRangeValue.LastMonth,
        AccurateAppend.Ui.DateRangeValue.Custom
    ], AccurateAppend.Ui.DateRangeValue.Last7Days, [
        ticketsApiViewModel.renderGrid
    ]));
    window.setInterval("ticketsApiViewModel.renderGrid()", 30000);
    ticketsApiViewModel.renderGrid();
});
var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Admin;
        (function (Admin) {
            var Operations;
            (function (Operations) {
                var TicketsApi;
                (function (TicketsApi) {
                    var TicketsApiViewModel = (function () {
                        function TicketsApiViewModel() {
                        }
                        TicketsApiViewModel.prototype.renderGrid = function () {
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
                                                    data: {
                                                        start: moment(TicketsApiDateRangeWidget.getStartDate()).utc().format("YYYY-MM-DD"),
                                                        end: moment(TicketsApiDateRangeWidget.getEndDate()).add(1, "days").utc().format("YYYY-MM-DD")
                                                    },
                                                    success: function (result) {
                                                        options.success(result);
                                                    }
                                                });
                                            },
                                            cache: false
                                        },
                                        group: {
                                            field: "Status",
                                            dir: "desc"
                                        },
                                        pageSize: 20,
                                        schema: {
                                            type: "json",
                                            data: "Data",
                                            total: function (response) {
                                                return response.Data.length;
                                            }
                                        },
                                        change: function () {
                                            if (this.data().length <= 0) {
                                                $("#gridMessage").show();
                                                $("#grid").hide();
                                                $("#pager").hide();
                                            }
                                            else {
                                                $("#gridMessage").hide();
                                                $("#grid").show();
                                                $("#pager").show();
                                                $("#warning").hide();
                                            }
                                        }
                                    },
                                    scrollable: false,
                                    filterable: false,
                                    pageable: true,
                                    dataBound: function (e) {
                                        var grid = $("#grid").data("kendoGrid");
                                        var dataView = grid.dataSource.view();
                                        for (var i = 0; i < dataView.length; i++) {
                                            for (var j = 0; j < dataView[i].items.length; j++) {
                                                if (dataView[i].items[j].status === "Closed") {
                                                    var uid = dataView[i].items[j].uid;
                                                    grid.collapseGroup($("#grid").find("tr[data-uid=" + uid + "]").prev("tr.k-grouping-row"));
                                                }
                                            }
                                        }
                                    },
                                    columns: [
                                        {
                                            field: "CreatedAt",
                                            title: "Date Created",
                                            attributes: { style: "text-align: center;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Recipient",
                                            title: "Recipient",
                                            template: "<a href='#= SearchUrl #'>#= Recipient #</a>",
                                            attributes: { style: "text-align: center;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Type",
                                            title: "Type",
                                            attributes: { style: "text-align: center;" },
                                            width: 200,
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Status",
                                            title: "Status",
                                            attributes: { style: "text-align: center;" },
                                            width: 200,
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "Subject",
                                            title: "Subject",
                                            attributes: { style: "text-align: left;" },
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            field: "",
                                            title: "",
                                            attributes: { style: "text-align: center;" },
                                            width: 250,
                                            template: "<a href='\\#' class=\"btn btn-default\" style=\"margin-right: 5px;\" onclick=\"ticketsApiViewModel.viewDetail('#= uid #')\">View Detail</a><a href=\"#= ZendeskDetail #\" class=\"btn btn-default\">View In Zendesk</a>",
                                            media: "(min-width: 450px)"
                                        },
                                        {
                                            title: "Summary",
                                            template: kendo.template($("#responsive-column-template-complete").html()),
                                            media: "(max-width: 450px)"
                                        }
                                    ]
                                });
                            }
                        };
                        TicketsApiViewModel.prototype.viewDetail = function (uid) {
                            var grid = $("#grid").data("kendoGrid");
                            var row = grid.tbody.find("tr[data-uid='" + uid + "']");
                            var item = grid.dataItem(row);
                            $("#detailsModal .modal-header").html("Details for Ticket " + item["Id"]);
                            $("#detailsModal .modal-body pre").html(item["Description"]);
                            $("#detailsModal").appendTo("body").modal("show");
                        };
                        return TicketsApiViewModel;
                    }());
                    TicketsApi.TicketsApiViewModel = TicketsApiViewModel;
                })(TicketsApi = Operations.TicketsApi || (Operations.TicketsApi = {}));
            })(Operations = Admin.Operations || (Admin.Operations = {}));
        })(Admin = Websites.Admin || (Websites.Admin = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGlzdFRpY2tldHMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJMaXN0VGlja2V0cy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFJQSxJQUFJLG1CQUE0RixDQUFDO0FBQ2pHLElBQUkseUJBQThCLENBQUM7QUFHbkMsQ0FBQyxDQUFDO0lBRUEsbUJBQW1CLEdBQUcsSUFBSSxjQUFjLENBQUMsUUFBUSxDQUFDLEtBQUssQ0FBQyxVQUFVLENBQUMsVUFBVSxDQUFDLG1CQUFtQixFQUFFLENBQUM7SUFFcEcseUJBQXlCLEdBQUcsSUFBSSxjQUFjLENBQUMsRUFBRSxDQUFDLGVBQWUsQ0FBQyxrQkFBa0IsRUFDbEYsSUFBSSxjQUFjLENBQUMsRUFBRSxDQUFDLHVCQUF1QixDQUMzQztRQUNFLGNBQWMsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLFdBQVc7UUFDNUMsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsU0FBUztRQUMxQyxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxVQUFVO1FBQzNDLGNBQWMsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLFVBQVU7UUFDM0MsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsU0FBUztRQUMxQyxjQUFjLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxNQUFNO0tBQ3hDLEVBQ0QsY0FBYyxDQUFDLEVBQUUsQ0FBQyxjQUFjLENBQUMsU0FBUyxFQUMxQztRQUNFLG1CQUFtQixDQUFDLFVBQVU7S0FDL0IsQ0FBQyxDQUFDLENBQUM7SUFFUixNQUFNLENBQUMsV0FBVyxDQUFDLGtDQUFrQyxFQUFFLEtBQUssQ0FBQyxDQUFDO0lBRTlELG1CQUFtQixDQUFDLFVBQVUsRUFBRSxDQUFDO0FBRW5DLENBQUMsQ0FBQyxDQUFDO0FBRUgsSUFBTyxjQUFjLENBMElwQjtBQTFJRCxXQUFPLGNBQWM7SUFBQyxJQUFBLFFBQVEsQ0EwSTdCO0lBMUlxQixXQUFBLFFBQVE7UUFBQyxJQUFBLEtBQUssQ0Ew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QXNCLElBQUksQ0FBQyxJQUFJLENBQUcsQ0FBQyxDQUFDOzRCQUMxRSxDQUFDLENBQUMsK0JBQStCLENBQUMsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUM7NEJBQzdELENBQUMsQ0FBQyxlQUFlLENBQUMsQ0FBQyxRQUFRLENBQUMsTUFBTSxDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDO3dCQUNwRCxDQUFDO3dCQUVILDBCQUFDO29CQUFELENBQUMsQUF0SUQsSUFzSUM7b0JBdElZLDhCQUFtQixzQkFzSS9CLENBQUE7Z0JBRUgsQ0FBQyxFQTFJK0MsVUFBVSxHQUFWLHFCQUFVLEtBQVYscUJBQVUsUUEwSXpEO1lBQUQsQ0FBQyxFQTFJb0MsVUFBVSxHQUFWLGdCQUFVLEtBQVYsZ0JBQVUsUUEwSTlDO1FBQUQsQ0FBQyxFQTFJOEIsS0FBSyxHQUFMLGNBQUssS0FBTCxjQUFLLFFBMEluQztJQUFELENBQUMsRUExSXFCLFFBQVEsR0FBUix1QkFBUSxLQUFSLHVCQUFRLFFBMEk3QjtBQUFELENBQUMsRUExSU0sY0FBYyxLQUFkLGNBQWMsUUEwSXBCIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9tb21lbnQvbW9tZW50LmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy9hY2N1cmF0ZWFwcGVuZC51aS50c1wiIC8+XHJcblxyXG52YXIgdGlja2V0c0FwaVZpZXdNb2RlbDogQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQWRtaW4uT3BlcmF0aW9ucy5UaWNrZXRzQXBpLlRpY2tldHNBcGlWaWV3TW9kZWw7XHJcbnZhciBUaWNrZXRzQXBpRGF0ZVJhbmdlV2lkZ2V0OiBhbnk7XHJcbmRlY2xhcmUgbGV0IHF1ZXJ5VXJsOiBzdHJpbmc7XHJcblxyXG4kKCgpID0+IHtcclxuXHJcbiAgdGlja2V0c0FwaVZpZXdNb2RlbCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5PcGVyYXRpb25zLlRpY2tldHNBcGkuVGlja2V0c0FwaVZpZXdNb2RlbCgpO1xyXG5cclxuICBUaWNrZXRzQXBpRGF0ZVJhbmdlV2lkZ2V0ID0gbmV3IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldChcInRpY2tldHNEYXRlUmFuZ2VcIixcclxuICAgIG5ldyBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VXaWRnZXRTZXR0aW5ncyhcclxuICAgICAgW1xyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QyNEhvdXJzLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3Q3RGF5cyxcclxuICAgICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0MzBEYXlzLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3Q2MERheXMsXHJcbiAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdE1vbnRoLFxyXG4gICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkN1c3RvbVxyXG4gICAgICBdLFxyXG4gICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0N0RheXMsXHJcbiAgICAgIFtcclxuICAgICAgICB0aWNrZXRzQXBpVmlld01vZGVsLnJlbmRlckdyaWRcclxuICAgICAgXSkpO1xyXG5cclxuICB3aW5kb3cuc2V0SW50ZXJ2YWwoXCJ0aWNrZXRzQXBpVmlld01vZGVsLnJlbmRlckdyaWQoKVwiLCAzMDAwMCk7XHJcblxyXG4gIHRpY2tldHNBcGlWaWV3TW9kZWwucmVuZGVyR3JpZCgpO1xyXG5cclxufSk7XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQWRtaW4uT3BlcmF0aW9ucy5UaWNrZXRzQXBpIHtcclxuXHJcbiAgZXhwb3J0IGNsYXNzIFRpY2tldHNBcGlWaWV3TW9kZWwge1xyXG5cclxuICAgIHJlbmRlckdyaWQoKSB7XHJcblxyXG4gICAgICBjb25zdCBncmlkID0gJChcIiNncmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgIGlmIChncmlkICE9PSB1bmRlZmluZWQgJiYgZ3JpZCAhPT0gbnVsbCkge1xyXG4gICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiNncmlkXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICByZWFkKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgIHVybDogcXVlcnlVcmwsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICAgICAgICAgIHN0YXJ0OiBtb21lbnQoVGlja2V0c0FwaURhdGVSYW5nZVdpZGdldC5nZXRTdGFydERhdGUoKSkudXRjKCkuZm9ybWF0KFwiWVlZWS1NTS1ERFwiKSxcclxuICAgICAgICAgICAgICAgICAgICBlbmQ6IG1vbWVudChUaWNrZXRzQXBpRGF0ZVJhbmdlV2lkZ2V0LmdldEVuZERhdGUoKSkuYWRkKDEsIFwiZGF5c1wiKS51dGMoKS5mb3JtYXQoXCJZWVlZLU1NLUREXCIpXHJcbiAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgY2FjaGU6IGZhbHNlXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGdyb3VwOiB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgICAgZGlyOiBcImRlc2NcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBwYWdlU2l6ZTogMjAsXHJcbiAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBjaGFuZ2U6IGZ1bmN0aW9uKCkge1xyXG4gICAgICAgICAgICAgIGlmICh0aGlzLmRhdGEoKS5sZW5ndGggPD0gMCkge1xyXG4gICAgICAgICAgICAgICAgJChcIiNncmlkTWVzc2FnZVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI2dyaWRcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNwYWdlclwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoXCIjZ3JpZE1lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNncmlkXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjcGFnZXJcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgJChcIiN3YXJuaW5nXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICAgIGZpbHRlcmFibGU6IGZhbHNlLFxyXG4gICAgICAgICAgcGFnZWFibGU6IHRydWUsXHJcbiAgICAgICAgICBkYXRhQm91bmQoZSkge1xyXG4gICAgICAgICAgICBjb25zdCBncmlkID0gJChcIiNncmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgICAgICAgIGNvbnN0IGRhdGFWaWV3ID0gZ3JpZC5kYXRhU291cmNlLnZpZXcoKTtcclxuXHJcbiAgICAgICAgICAgIGZvciAobGV0IGkgPSAwOyBpIDwgZGF0YVZpZXcubGVuZ3RoOyBpKyspIHtcclxuICAgICAgICAgICAgICBmb3IgKGxldCBqID0gMDsgaiA8IGRhdGFWaWV3W2ldLml0ZW1zLmxlbmd0aDsgaisrKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZGF0YVZpZXdbaV0uaXRlbXNbal0uc3RhdHVzID09PSBcIkNsb3NlZFwiKSB7XHJcbiAgICAgICAgICAgICAgICAgIGNvbnN0IHVpZCA9IGRhdGFWaWV3W2ldLml0ZW1zW2pdLnVpZDtcclxuICAgICAgICAgICAgICAgICAgZ3JpZC5jb2xsYXBzZUdyb3VwKCQoXCIjZ3JpZFwiKS5maW5kKGB0cltkYXRhLXVpZD0ke3VpZH1dYCkucHJldihcInRyLmstZ3JvdXBpbmctcm93XCIpKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJDcmVhdGVkQXRcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJEYXRlIENyZWF0ZWRcIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgIC8vdGVtcGxhdGU6IFwiIz0gTmFtZSAjIFwiICsgJyMgaWYoVHlwZSA9PT0gXCInICsgT3JkZXJUeXBlUHVzaCArICdcIikgeyAjJyArIFwiIC0gIzpTbHVnIyBcIiArIFwiIyB9ICMgXCIsXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlJlY2lwaWVudFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlJlY2lwaWVudFwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxhIGhyZWY9JyM9IFNlYXJjaFVybCAjJz4jPSBSZWNpcGllbnQgIzwvYT5cIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJUeXBlXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiVHlwZVwiLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgd2lkdGg6IDIwMCxcclxuICAgICAgICAgICAgICAvL3RlbXBsYXRlOiBcIiM9IGtlbmRvLnRvU3RyaW5nKGtlbmRvLnBhcnNlRGF0ZShEYXRlU3VibWl0dGVkLCAnTU0vZGQveXl5eScpLCAnTU0vZGQveXl5eScpICNcIixcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICB3aWR0aDogMjAwLFxyXG4gICAgICAgICAgICAgIC8vdGVtcGxhdGU6IFwiIz0ga2VuZG8udG9TdHJpbmcoa2VuZG8ucGFyc2VEYXRlKERhdGVTdWJtaXR0ZWQsICdNTS9kZC95eXl5JyksICdNTS9kZC95eXl5JykgI1wiLFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJTdWJqZWN0XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU3ViamVjdFwiLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogbGVmdDtcIiB9LFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJcIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgIHdpZHRoOiAyNTAsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6XHJcbiAgICAgICAgICAgICAgICBcIjxhIGhyZWY9J1xcXFwjJyBjbGFzcz1cXFwiYnRuIGJ0bi1kZWZhdWx0XFxcIiBzdHlsZT1cXFwibWFyZ2luLXJpZ2h0OiA1cHg7XFxcIiBvbmNsaWNrPVxcXCJ0aWNrZXRzQXBpVmlld01vZGVsLnZpZXdEZXRhaWwoJyM9IHVpZCAjJylcXFwiPlZpZXcgRGV0YWlsPC9hPjxhIGhyZWY9XFxcIiM9IFplbmRlc2tEZXRhaWwgI1xcXCIgY2xhc3M9XFxcImJ0biBidG4tZGVmYXVsdFxcXCI+VmlldyBJbiBaZW5kZXNrPC9hPlwiLFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICB0aXRsZTogXCJTdW1tYXJ5XCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjcmVzcG9uc2l2ZS1jb2x1bW4tdGVtcGxhdGUtY29tcGxldGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWF4LXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICBdXHJcbiAgICAgICAgfSk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICB2aWV3RGV0YWlsKHVpZCkge1xyXG4gICAgICBjb25zdCBncmlkID0gJChcIiNncmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgIGNvbnN0IHJvdyA9IGdyaWQudGJvZHkuZmluZChgdHJbZGF0YS11aWQ9JyR7dWlkfSddYCk7XHJcbiAgICAgIGNvbnN0IGl0ZW0gPSBncmlkLmRhdGFJdGVtKHJvdyk7XHJcbiAgICAgICQoXCIjZGV0YWlsc01vZGFsIC5tb2RhbC1oZWFkZXJcIikuaHRtbChgRGV0YWlscyBmb3IgVGlja2V0ICR7aXRlbVtcIklkXCJdfWApO1xyXG4gICAgICAkKFwiI2RldGFpbHNNb2RhbCAubW9kYWwtYm9keSBwcmVcIikuaHRtbChpdGVtW1wiRGVzY3JpcHRpb25cIl0pO1xyXG4gICAgICAkKFwiI2RldGFpbHNNb2RhbFwiKS5hcHBlbmRUbyhcImJvZHlcIikubW9kYWwoXCJzaG93XCIpO1xyXG4gICAgfVxyXG5cclxuICB9XHJcblxyXG59Il19