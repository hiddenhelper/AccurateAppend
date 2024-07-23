var viewModel;
var operatingMetricsViewModel;
var processingMetricsViewModel;
var webserviceMetricsViewModel;
var webserviceMetricsViewModel;
var clientMetricsViewModel;
var adminUserMetricsModel;
$(function () {
    viewModel = new AccurateAppend.Reporting.Dashboard.ViewModel();
    operatingMetricsViewModel = new AccurateAppend.Reporting.Dashboard.OperatingMetricsViewModel();
    processingMetricsViewModel = new AccurateAppend.Reporting.Dashboard.ProcessingMetricsViewModel();
    webserviceMetricsViewModel = new AccurateAppend.Reporting.Dashboard.WebServicesMetricsModel();
    clientMetricsViewModel = new AccurateAppend.Reporting.Dashboard.ClientMetricsModel;
    adminUserMetricsModel = new AccurateAppend.Reporting.Dashboard.AdminUserMetricsModel;
    $("#ApplicationId").bind("change", function () {
        viewModel.setApplicationId();
        renderTabs();
    });
    $(window).resize(function () {
        kendo.resize($("div.k-chart[data-role='chart']"), true);
    });
    $('a[data-toggle="tab"]').on("shown.bs.tab", function (e) {
        kendo.resize($("div.k-chart[data-role='chart']"), true);
    });
    renderTabs();
    $("#Source").bind("change", function () {
        processingMetricsViewModel.renderSubscriberProcessingHistory();
    });
    $("#clientProcessingMetricsGridToolbarSource").bind("change", function () {
        console.log("firing change");
        clientMetricsViewModel.renderProcessingMetrics();
    });
    $("#adminUserActivityUserSummaryGridToolbarSource").bind("change", function () {
        console.log("firing change");
        adminUserMetricsModel.renderAdminUserSummary();
    });
});
function renderTabs() {
    operatingMetricsViewModel.renderTab();
    processingMetricsViewModel.renderTab();
    webserviceMetricsViewModel.renderTab();
    clientMetricsViewModel.renderTab();
    adminUserMetricsModel.renderTab();
}
var AccurateAppend;
(function (AccurateAppend) {
    var Reporting;
    (function (Reporting) {
        var Dashboard;
        (function (Dashboard) {
            var ViewModel = (function () {
                function ViewModel() {
                }
                ViewModel.prototype.loadApplicationId = function () {
                    var v = $.cookie("ApplicationId");
                    if (v !== "") {
                        $("#ApplicationId option[value=" + $.cookie("ApplicationId") + "]").attr("selected", "selected");
                    }
                };
                ViewModel.prototype.setApplicationId = function () {
                    $.cookie("ApplicationId", $("#ApplicationId option:selected").val());
                };
                return ViewModel;
            }());
            Dashboard.ViewModel = ViewModel;
            var OperatingMetricsViewModel = (function () {
                function OperatingMetricsViewModel() {
                }
                OperatingMetricsViewModel.prototype.renderTab = function () {
                    console.log("OperatingMetricsViewModel.renderTab");
                    this.renderDealMetricOverviewReportGrid();
                    this.renderMrrMetricOverviewGrid();
                    this.renderAgentMetricOverviewGrid();
                    this.renderRevenueMetricChart();
                    this.renderLeadMetricOverviewReportGrid();
                    this.renderLeadMetricChart();
                    this.renderLeadChannelMetricOverviewGrid(1, "#LeadChannelMetricOverviewGrid_Direct");
                    this.renderLeadChannelMetricOverviewGrid(2, "#LeadChannelMetricOverviewGrid_Organic");
                    this.renderLeadChannelMetricOverviewGrid(3, "#LeadChannelMetricOverviewGrid_Referral");
                    this.renderLeadChannelMetricOverviewGrid(8, "#LeadChannelMetricOverviewGrid_NationBuilder");
                };
                OperatingMetricsViewModel.prototype.renderDealMetricOverviewReportGrid = function () {
                    var grid = $("#DealMetricOverviewReportGrid").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#DealMetricOverviewReportGrid").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = { applicationid: $("#ApplicationId").val() };
                                        $.ajax({
                                            url: "/Reporting/OperatingMetrics/OverviewReport",
                                            dataType: "json",
                                            type: "GET",
                                            data: data,
                                            success: function (result) {
                                                options.success(result);
                                            }
                                        });
                                    }
                                },
                                schema: {
                                    type: "json",
                                    data: "Data",
                                    total: function (response) {
                                        return response.Data.length;
                                    }
                                }
                            },
                            columns: [
                                { field: "MetricNameDescription", title: "Description" },
                                {
                                    field: "Today",
                                    title: "Today",
                                    template: kendo.template($("#todayTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Yesterday",
                                    title: "Yesterday",
                                    template: kendo.template($("#yesterdayTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Last7",
                                    title: "Last 7",
                                    template: kendo.template($("#last7Template").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "CurrentMonth",
                                    title: "Current Month",
                                    template: kendo.template($("#currentMonthTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "SamePeriodLastMonth",
                                    title: "Same Period Last Month",
                                    template: kendo.template($("#samePeriodLastMonthTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "LastMonth",
                                    title: "Last Month",
                                    template: kendo.template($("#LastMonthTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "PreviousToLastMonth",
                                    title: "Previous Month",
                                    template: kendo.template($("#previousToLastMonthTemplate").html()),
                                    attributes: { style: "text-align:right;" }
                                }
                            ]
                        });
                    }
                };
                OperatingMetricsViewModel.prototype.renderMrrMetricOverviewGrid = function () {
                    if ($("#MrrMetricOverviewGrid").length === 0)
                        return;
                    $.ajax({
                        url: "/Reporting/MrrMetrics/Query",
                        dataType: "json",
                        type: "GET",
                        data: { applicationId: $("#ApplicationId").val() },
                        success: function (data) {
                            var container = $("#MrrMetricOverviewGrid").empty();
                            ;
                            var table = $("<table>");
                            $.each(data, function (key, value) {
                                if (key === 0) {
                                    var th = $("<thead class=\"k-grid-header\"><tr>");
                                    $.each(Object.keys(value), function (i, propertyName) {
                                        th.append("<th class=\"k-header\">" + propertyName + "</th>");
                                    });
                                    table.append(th).append("</thead>");
                                }
                                var tr = $("<tr " + (key & 1 ? 'class="k-alt"' : "") + " >");
                                $.each(value, function (i, propertyValue) {
                                    tr.append("<td style=\"text-align: right;\">" + propertyValue + "</td>");
                                });
                                table.append(tr);
                            });
                            container.append(table);
                        }
                    });
                };
                OperatingMetricsViewModel.prototype.renderAgentMetricOverviewGrid = function () {
                    $.ajax({
                        url: "/Reporting/AgentMetrics/Query",
                        dataType: "json",
                        type: "GET",
                        data: { applicationId: $("#ApplicationId").val() },
                        success: function (data) {
                            var container = $("#AgentMetricOverviewGrid").empty();
                            ;
                            var table = $("<table>");
                            $.each(data, function (key, value) {
                                if (key === 0) {
                                    var th = $("<thead class=\"k-grid-header\"><tr>");
                                    $.each(Object.keys(value), function (i, propertyName) {
                                        th.append("<th class=\"k-header\">" + propertyName + "</th>");
                                    });
                                    table.append(th).append("</thead>");
                                }
                                var tr = $("<tr " + (key & 1 ? 'class="k-alt"' : "") + " >");
                                $.each(value, function (i, propertyValue) {
                                    tr.append("<td style=\"text-align: right;\">" + propertyValue + "</td>");
                                });
                                table.append(tr);
                            });
                            container.append(table);
                        }
                    });
                };
                OperatingMetricsViewModel.prototype.renderLeadChannelMetricOverviewGrid = function (leadSource, selector) {
                    $.ajax({
                        url: "/Reporting/LeadChannelMetrics/Query",
                        dataType: "json",
                        type: "GET",
                        data: { applicationId: $("#ApplicationId").val(), leadSource: leadSource },
                        success: function (data) {
                            var container = $(selector).empty();
                            ;
                            var table = $("<table>");
                            $.each(data, function (key, value) {
                                var format;
                                switch (value.MetricName) {
                                    case 3:
                                        format = "p1";
                                        break;
                                    case 4:
                                    case 6:
                                        format = "c0";
                                        break;
                                    default:
                                        format = "n0";
                                        break;
                                }
                                if (key === 0) {
                                    var th = $("<thead class=\"k-grid-header\"><tr>");
                                    $.each(Object.keys(value), function (i, propertyName) {
                                        if (propertyName !== "MetricName") {
                                            th.append("<th class=\"k-header\" style=\"text-align: right;\">" + propertyName + "</th>");
                                        }
                                    });
                                    table.append(th).append("</thead>");
                                }
                                var tr = $("<tr " + (key & 1 ? 'class="k-alt"' : "") + " >");
                                $.each(value, function (name, value) {
                                    if (name !== "MetricName") {
                                        tr.append("<td style=\"text-align: right;\">" + kendo.toString(value, format) + "</td>");
                                    }
                                });
                                table.append(tr);
                            });
                            container.append(table);
                        }
                    });
                };
                OperatingMetricsViewModel.prototype.renderRevenueMetricChart = function () {
                    if ($("#RevenueMetricChart").length === 0)
                        return;
                    $("#RevenueMetricChart").kendoChart({
                        title: {
                            text: "Revenue By Month"
                        },
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/OperatingMetrics/Revenue",
                                        dataType: "json",
                                        type: "GET",
                                        data: { applicationid: $("#ApplicationId").val() },
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: { data: "Data" },
                            requestStart: function () {
                                kendo.ui.progress($("#RevenueMetricChartLoading"), true);
                            },
                            requestEnd: function () {
                                kendo.ui.progress($("#RevenueMetricChartLoading"), false);
                            }
                        },
                        legend: {
                            position: "bottom"
                        },
                        series: [
                            {
                                field: "TotalRevenue",
                                name: "Total Revenue",
                                color: "#73c100",
                                aggregate: "sum",
                                type: "area",
                                categoryField: "Date"
                            }, {
                                field: "RevenueSubscriber",
                                name: "Subscriber Revenue",
                                color: "#ffae00",
                                aggregate: "sum",
                                type: "line",
                                categoryField: "Date"
                            }, {
                                field: "RevenueNonSubscriber",
                                name: "Non-subscriber Revenue",
                                color: "#007eff",
                                aggregate: "sum",
                                type: "line",
                                categoryField: "Date"
                            }, {
                                field: "RevenueNationBuilder",
                                name: "NationBuilder Revenue",
                                color: "#cc7eff",
                                aggregate: "sum",
                                type: "line",
                                categoryField: "Date"
                            }
                        ],
                        valueAxis: [
                            {
                                labels: {
                                    format: "{0:C}"
                                },
                                line: {
                                    visible: false
                                },
                                axisCrossingValue: -10
                            }
                        ],
                        categoryAxis: [
                            {
                                field: "Date",
                                baseUnit: "months",
                                axisCrossingValue: [0, 100],
                                type: "date"
                            }
                        ],
                        tooltip: {
                            visible: true,
                            format: "{0:C}",
                            template: "#= series.name #: #= kendo.toString(value, 'c') #",
                            color: "#ffffff"
                        }
                    });
                    $("#RevenueMetricChart").data("kendoChart").redraw();
                };
                OperatingMetricsViewModel.prototype.renderLeadMetricOverviewReportGrid = function () {
                    var grid = $("#LeadMetricOverviewReportGrid").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#LeadMetricOverviewReportGrid").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = { applicationid: $("#ApplicationId").val() };
                                        $.ajax({
                                            url: "/Reporting/LeadMetrics/OverviewReport",
                                            dataType: "json",
                                            type: "GET",
                                            data: data,
                                            success: function (result) {
                                                options.success(result);
                                            }
                                        });
                                    }
                                },
                                schema: {
                                    type: "json",
                                    data: "Data",
                                    total: function (response) {
                                        return response.Data.length;
                                    },
                                    model: {
                                        fields: {
                                            MetricNameDescription: { type: "string" },
                                            MetricName: { type: "string" },
                                            Today: { type: "number" },
                                            Yesterday: { type: "number" },
                                            Last7: { type: "number" },
                                            Last30: { type: "number" },
                                            Last60: { type: "number" },
                                            Last90: { type: "number" }
                                        }
                                    }
                                }
                            },
                            columns: [
                                { field: "LeadSourceDescription", title: "Description" },
                                {
                                    field: "Today",
                                    title: "Today",
                                    template: kendo.template("#= displayLeadMetricShort(TodayLeadCount, TodayNewClientCount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Yesterday",
                                    title: "Yesterday",
                                    template: kendo.template("#= displayLeadMetricShort(YesterdayLeadCount, YesterdayNewClientCount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Last7",
                                    title: "Last 7",
                                    template: kendo.template("#= displayLeadMetricShort(Last7RecordsLeadCount, Last7RecordsNewClientCount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "CurrentMonth",
                                    title: "Current Month",
                                    template: kendo.template("#= displayLeadMetric(CurrentMonthLeadCount, CurrentMonthNewClientCount, CurrentMonthRevenueAmount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "SamePeriodLastMonth",
                                    title: "Same Period Last Month",
                                    template: kendo.template("#= displayLeadMetric(SamePeriodLastMonthLeadCount, SamePeriodLastMonthNewClientCount, SamePeriodLastMonthRevenueAmount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "LastMonth",
                                    title: "Last Month",
                                    template: kendo.template("#= displayLeadMetric(LastMonthLeadCount, LastMonthNewClientCount, LastMonthRevenueAmount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "PreviousToLastMonth",
                                    title: "Previous Month",
                                    template: kendo.template("#= displayLeadMetric(PreviousToLastLeadCount, PreviousToLastNewClientCount, PreviousToLastRevenueAmount) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Rolling12Months",
                                    title: "Rolling 12 Months",
                                    template: kendo.template("#= displayLeadMetric(Rolling12MonthsLeadCount, Rolling12MonthsNewClientCount, Rolling12MonthsRevenueAmount) #"),
                                    attributes: { style: "text-align:right;" },
                                    width: 200
                                },
                                { field: "TimeToFirstPurchase", title: "Days To $$$", attributes: { style: "text-align:right;" } }
                            ]
                        });
                    }
                };
                OperatingMetricsViewModel.prototype.renderLeadMetricChart = function () {
                    $("#leadSumamryChart").kendoChart({
                        title: {
                            text: "Lead Activity By Week"
                        },
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/LeadReports/LeadMetricSummary",
                                        dataType: "json",
                                        type: "GET",
                                        data: { applicationid: $("#ApplicationId").val() },
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: { data: "Data" },
                            requestStart: function () {
                                kendo.ui.progress($("#LeadSummaryChartLoading"), true);
                            },
                            requestEnd: function () {
                                kendo.ui.progress($("#LeadSummaryChartLoading"), false);
                            }
                        },
                        legend: {
                            position: "bottom"
                        },
                        series: [
                            {
                                field: "Total",
                                name: "Total",
                                color: "#73c100",
                                aggregate: "sum",
                                type: "area",
                                categoryField: "Date",
                            }, {
                                field: "Qualified",
                                name: "Qualified",
                                color: "#ffae00",
                                aggregate: "sum",
                                type: "line",
                                categoryField: "Date",
                            }, {
                                field: "Converted",
                                name: "To Client",
                                color: "#007eff",
                                aggregate: "sum",
                                type: "line",
                                categoryField: "Date",
                            }
                        ],
                        valueAxis: [
                            {
                                labels: {
                                    format: "{0}"
                                },
                                line: {
                                    visible: false
                                },
                                axisCrossingValue: -10
                            }
                        ],
                        categoryAxis: [
                            {
                                field: "Date",
                                baseUnit: "weeks",
                                axisCrossingValue: [0, 100],
                                type: "date"
                            }
                        ],
                        tooltip: {
                            visible: true,
                            format: "{0}",
                            template: "#= series.name #: #= value #",
                            color: "#ffffff"
                        }
                    });
                    $("#leadSumamryChart").data("kendoChart").redraw();
                };
                return OperatingMetricsViewModel;
            }());
            Dashboard.OperatingMetricsViewModel = OperatingMetricsViewModel;
            var ProcessingMetricsViewModel = (function () {
                function ProcessingMetricsViewModel() {
                }
                ProcessingMetricsViewModel.prototype.renderTab = function () {
                    console.log("ProcessingMetricsViewModel.renderTab");
                    this.renderProcessingMetricOverviewReportGrid();
                    this.renderSubscriberProcessingHistory();
                    this.renderSubscriberActivityMonthComparison();
                    this.renderJobQueueActivityLast24Hours();
                };
                ProcessingMetricsViewModel.prototype.renderProcessingMetricOverviewReportGrid = function () {
                    var grid = $("#ProcessingMetricOverviewReportGrid").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#ProcessingMetricOverviewReportGrid").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = { applicationid: $("#ApplicationId").val() };
                                        $.ajax({
                                            url: "/Reporting/OperationMetrics/OverviewReport",
                                            dataType: "json",
                                            type: "GET",
                                            data: data,
                                            success: function (result) {
                                                options.success(result);
                                            }
                                        });
                                    }
                                },
                                schema: {
                                    type: "json",
                                    data: "Data",
                                    total: function (response) {
                                        return response.Data.length;
                                    },
                                    model: {
                                        fields: {
                                            MetricNameDescription: { type: "string" },
                                            MetricName: { type: "string" },
                                            Today: { type: "number" },
                                            Yesterday: { type: "number" },
                                            Last7: { type: "number" },
                                            Last30: { type: "number" },
                                            Last60: { type: "number" },
                                            Last90: { type: "number" }
                                        }
                                    }
                                }
                            },
                            columns: [
                                { field: "Operation", title: "Description" },
                                {
                                    field: "Today",
                                    title: "Today",
                                    template: kendo.template("#= displayProcessingMetric(TodayRecords,TodayMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Yesterday",
                                    title: "Yesterday",
                                    template: kendo.template("#= displayProcessingMetric(YesterdayRecords,YesterdayMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "Last7",
                                    title: "Last 7",
                                    template: kendo.template("#= displayProcessingMetric(Last7Records,Last7Matches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "CurrentMonth",
                                    title: "Current Month",
                                    template: kendo.template("#= displayProcessingMetric(CurrentMonthRecords,CurrentMonthMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "SamePeriodLastMonth",
                                    title: "Same Period Last Month",
                                    template: kendo.template("#= displayProcessingMetric(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "LastMonth",
                                    title: "Last Month",
                                    template: kendo.template("#= displayProcessingMetric(LastMonthRecords,LastMonthMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                },
                                {
                                    field: "PreviousToLastMonth",
                                    title: "Previous Month",
                                    template: kendo.template("#= displayProcessingMetric(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
                                    attributes: { style: "text-align:right;" }
                                }
                            ]
                        });
                    }
                };
                ProcessingMetricsViewModel.prototype.renderSubscriberProcessingHistory = function () {
                    $("#subscriberProcessingHistory").kendoChart({
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/JobMetrics/SubscriberJobMatchCounts?aggregate=0&source=" + $("#Source").val() + "&startDate=" + moment().subtract("month", 8).format("L") + "&endDate=" + moment().format("L") + "&applicationid=" + $("#ApplicationId").val(),
                                        dataType: "json",
                                        type: "GET",
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: { data: "Data" },
                            requestStart: function () {
                                kendo.ui.progress($("#subscriberProcessingHistoryLoading"), true);
                            },
                            requestEnd: function () {
                                kendo.ui.progress($("#subscriberProcessingHistoryLoading"), false);
                            }
                        },
                        series: [
                            {
                                field: "Count",
                                name: "Total",
                                type: "area",
                                categoryField: "Date",
                                color: "#73c100"
                            },
                            {
                                field: "StandardPhoneAppend",
                                name: "Standard Phone",
                                type: "line",
                                categoryField: "Date",
                                color: "#CC9900"
                            }, {
                                field: "PremiumPhoneAppend",
                                name: "Premium Phone",
                                type: "line",
                                categoryField: "Date",
                                color: "#FF6600"
                            }, {
                                field: "EmailAppend",
                                name: "Email Append",
                                type: "line",
                                categoryField: "Date",
                                color: "#007eff"
                            }, {
                                field: "OtherAppends",
                                name: "Other",
                                type: "line",
                                categoryField: "Date",
                            }
                        ],
                        legend: {
                            position: "bottom"
                        },
                        valueAxis: [
                            {
                                labels: {
                                    format: "{0:N0}"
                                },
                                axisCrossingValue: -10,
                                line: {
                                    visible: false
                                }
                            }
                        ],
                        categoryAxis: [
                            {
                                field: "Date",
                                baseUnit: "months",
                                type: "date"
                            }
                        ],
                        tooltip: {
                            visible: true,
                            format: "{0:N0}",
                            color: "#fff"
                        }
                    });
                    $("#subscriberProcessingHistory").data("kendoChart").redraw();
                };
                ProcessingMetricsViewModel.prototype.renderSubscriberActivityMonthComparison = function () {
                    $("#subscriberActivityMonthComparison").kendoChart({
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/JobMetrics/SubscriberJobMatchCountsComparison?compare=0&applicationid=" + $("#ApplicationId").val(),
                                        dataType: "json",
                                        type: "GET",
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: { data: "Data" },
                            requestStart: function () {
                                kendo.ui.progress($("#subscriberActivityMonthComparisonChartLoading"), true);
                            },
                            requestEnd: function () {
                                kendo.ui.progress($("#subscriberActivityMonthComparisonChartLoading"), false);
                            }
                        },
                        series: [
                            {
                                field: "Current",
                                name: "Current Month",
                                type: "line",
                                color: "#73c100"
                            },
                            {
                                field: "Previous",
                                name: "Previous Month",
                                type: "line",
                                color: "#CC9900"
                            }
                        ],
                        legend: {
                            position: "bottom"
                        },
                        valueAxis: [
                            {
                                labels: {
                                    format: "{0:N0}"
                                },
                                line: {
                                    visible: false
                                }
                            }
                        ],
                        tooltip: {
                            visible: true,
                            format: "{0}",
                            template: "<span style='color: white;'>Matches: #= value #</span>"
                        },
                        categoryAxis: [
                            {
                                field: "Day",
                                type: "number"
                            }
                        ]
                    });
                };
                ProcessingMetricsViewModel.prototype.renderJobQueueActivityLast24Hours = function () {
                    $("#jobQueueActivityLast24Hours").kendoChart({
                        dataSource: {
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/JobMetrics/GetGraphForLast24Hours?applicationid=" + $("#ApplicationId").val(),
                                        dataType: "json",
                                        type: "GET",
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: { data: "Data" },
                            requestStart: function () {
                                kendo.ui.progress($("#jobQueueActivityLast24HoursLoading"), true);
                            },
                            requestEnd: function () {
                                kendo.ui.progress($("#jobQueueActivityLast24HoursLoading"), false);
                            }
                        },
                        series: [
                            {
                                field: "ListbuilderCount",
                                name: "Listbuilder",
                                type: "line",
                                categoryField: "Hour",
                                color: "#00FFCC",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Listbuilder Files: #= value #</span>"
                                }
                            },
                            {
                                field: "ClientCount",
                                name: "Clients",
                                type: "line",
                                categoryField: "Hour",
                                color: "#9944CC",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Client Files: #= value #</span>"
                                }
                            },
                            {
                                field: "NationBuilderCount",
                                name: "NB",
                                type: "line",
                                categoryField: "Hour",
                                color: "#44CC99",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>NB Files: #= value #</span>"
                                }
                            },
                            {
                                field: "FtpCount",
                                name: "Ftp",
                                type: "line",
                                categoryField: "Hour",
                                color: "#CC9900",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>FTP Files: #= value #</span>"
                                }
                            },
                            {
                                field: "EmailCount",
                                name: "Email",
                                type: "line",
                                categoryField: "Hour",
                                color: "#FF6600",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Email Files: #= value #</span>"
                                }
                            },
                            {
                                field: "AdminCount",
                                name: "Admin",
                                type: "line",
                                categoryField: "Hour",
                                color: "#007eff",
                                axis: "Count",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Admin Files: #= value #</span>"
                                }
                            },
                            {
                                field: "FtpRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#CC9900",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>FTP Records: #= value #</span>"
                                }
                            },
                            {
                                field: "EmailRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#FF6600",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Email Records: #= value #</span>"
                                }
                            },
                            {
                                field: "AdminRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#007eff",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Admin Records: #= value #</span>"
                                }
                            },
                            {
                                field: "NationBuilderRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#44CC99",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>NB Records: #= value #</span>"
                                }
                            },
                            {
                                field: "ClientRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#9944CC",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>NB Records: #= value #</span>"
                                }
                            },
                            {
                                field: "ListbuilderRecords",
                                type: "bar",
                                categoryField: "Hour",
                                color: "#00FFCC",
                                axis: "Records",
                                tooltip: {
                                    visible: true,
                                    format: "{0:N0}",
                                    color: "#fff",
                                    template: "<span style='color: white;'>Listbuilder Records: #= value #</span>"
                                }
                            }
                        ],
                        legend: {
                            position: "bottom",
                        },
                        valueAxis: [
                            {
                                name: "Count",
                                labels: {
                                    format: "{0:N0}"
                                },
                                line: {
                                    visible: false
                                },
                                title: {
                                    text: "Files Submitted"
                                }
                            }, {
                                name: "Records",
                                type: "log",
                                labels: {
                                    format: "{0:N0}"
                                },
                                line: {
                                    visible: false
                                },
                                title: {
                                    text: "Records Submitted"
                                }
                            }
                        ],
                        categoryAxis: [
                            {
                                field: "Hour",
                                type: "number",
                                axisCrossingValue: [0, 26]
                            }
                        ]
                    });
                    $("#jobQueueActivityLast24Hours").data("kendoChart").redraw();
                };
                return ProcessingMetricsViewModel;
            }());
            Dashboard.ProcessingMetricsViewModel = ProcessingMetricsViewModel;
            var WebServicesMetricsModel = (function () {
                function WebServicesMetricsModel() {
                }
                WebServicesMetricsModel.prototype.renderTab = function () {
                    console.log("WebServicesMetricsModel.renderTab");
                    this.renderTransactionsByUserGrid();
                };
                WebServicesMetricsModel.prototype.renderTransactionsByUserGrid = function () {
                    $("#transactionsByUser").kendoGrid({
                        dataSource: {
                            autobind: false,
                            type: "json",
                            transport: {
                                read: function (options) {
                                    $.ajax({
                                        url: "/Reporting/ApiMetrics/GetCallsByUserJson",
                                        dataType: "json",
                                        type: "GET",
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: "json",
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                }
                            },
                            pageSize: 10
                        },
                        columns: [
                            {
                                field: "Email",
                                title: "Username",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("<a href=\"/Users/Detail?userid=#=UserId#\">#=Email#</a>")
                            },
                            {
                                field: "Count",
                                title: "Count",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            }
                        ]
                    });
                };
                return WebServicesMetricsModel;
            }());
            Dashboard.WebServicesMetricsModel = WebServicesMetricsModel;
            var ClientMetricsModel = (function () {
                function ClientMetricsModel() {
                }
                ClientMetricsModel.prototype.renderTab = function () {
                    console.log("ClientMetricsModel.renderTab");
                    this.renderRecentDeals();
                    this.renderDealMetrics();
                    this.renderProcessingMetrics();
                };
                ClientMetricsModel.prototype.renderRecentDeals = function () {
                    $("#recentDealsGrid").kendoGrid({
                        dataSource: {
                            autobind: false,
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = { applicationid: $("#ApplicationId").val() };
                                    $.ajax({
                                        url: "/Reporting/DealMetrics/RecentDeals",
                                        dataType: "json",
                                        type: "GET",
                                        data: data,
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: "json",
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                }
                            },
                            pageSize: 20,
                        },
                        columns: [
                            {
                                field: "Email",
                                title: "Email",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("<a href=\"/Users/Detail?userid=#=UserId#\">#=Email#</a>")
                            },
                            {
                                field: "DateDealCreated",
                                title: "Deal Date",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            },
                            {
                                field: "Amount",
                                title: "Amount",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("#= kendo.toString(Amount, 'c0') #")
                            },
                            {
                                field: "DateAccountCreated",
                                title: "Account Create Date",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: "#= kendo.toString(kendo.parseDate(DateAccountCreated), 'MM/dd/yyyy') #"
                            },
                            {
                                field: "Category",
                                title: "Customer Type",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            },
                            {
                                field: "LeadSourceDescription",
                                title: "Channel",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            }
                        ],
                        pageable: true,
                        scrollable: false
                    });
                };
                ClientMetricsModel.prototype.renderDealMetrics = function () {
                    $("#clientDealMetricsGrid").kendoGrid({
                        dataSource: {
                            autobind: false,
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = { applicationid: $("#ApplicationId").val() };
                                    $.ajax({
                                        url: "/Reporting/DealMetrics/ClientDeals",
                                        dataType: "json",
                                        type: "GET",
                                        data: data,
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: "json",
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                }
                            },
                            pageSize: 20,
                        },
                        columns: [
                            {
                                field: "Email",
                                title: "Email",
                                template: kendo.template("<a href=\"/Users/Detail?userid=#=UserId#\">#=Email#</a>")
                            },
                            {
                                field: "TodayRevenue",
                                title: "Today",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "Last7Revenue",
                                title: "Last 7",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "CurrentMonthRevenue",
                                title: "Current Month",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "SamePeriodLastMonthRevenue",
                                title: "Same Period Last Month",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "LastMonthRevenue",
                                title: "Last Month",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "PreviousToLastMonthRevenue",
                                title: "Previous Month",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                headerAttributes: { style: "text-align: center;" }
                            },
                            {
                                field: "Rolling12MonthRevenue",
                                title: "Rolling 12 Months",
                                format: "{0:c}",
                                attributes: { style: "text-align:right;" },
                                sortable: {},
                                headerAttributes: { style: "text-align: center;" }
                            }
                        ],
                        pageable: true,
                        sortable: {
                            mode: "single",
                            allowUnsort: false
                        }
                    });
                };
                ClientMetricsModel.prototype.renderProcessingMetrics = function () {
                    $("#clientProcessingMetricsGrid").kendoGrid({
                        dataSource: {
                            autobind: false,
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = {
                                        applicationid: $("#ApplicationId").val(),
                                        source: $("#clientProcessingMetricsGridToolbarSource").val()
                                    };
                                    $.ajax({
                                        url: "/Reporting/UserProcessingMetrics/OverviewReport",
                                        dataType: "json",
                                        type: "GET",
                                        data: data,
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: "json",
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                }
                            },
                            pageSize: 20,
                        },
                        columns: [
                            {
                                field: "Email",
                                title: "Email",
                                template: kendo.template("<a href=\"/Users/Detail?userid=#=UserId#\">#=Email#</a>")
                            },
                            {
                                field: "Today",
                                title: "Today",
                                template: kendo.template("#= displayProcessingMetric(TodayRecords,TodayMatches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "Last7",
                                title: "Last 7",
                                template: kendo.template("#= displayProcessingMetric(Last7Records,Last7Matches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "CurrentMonth",
                                title: "Current Month",
                                template: kendo.template("#= displayProcessingMetric(CurrentMonthRecords,CurrentMonthMatches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "SamePeriodLastMonth",
                                title: "Same Period Last Month",
                                template: kendo.template("#= displayProcessingMetric(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "LastMonth",
                                title: "Last Month",
                                template: kendo.template("#= displayProcessingMetric(LastMonthRecords,LastMonthMatches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "PreviousToLastMonth",
                                title: "Previous Month",
                                template: kendo.template("#= displayProcessingMetric(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
                                attributes: { style: "text-align:right;" }
                            },
                            {
                                field: "Rolling12Month",
                                title: "Rolling 12 Months",
                                template: kendo.template("#= displayProcessingMetric(Rolling12MonthRecords,Rolling12MonthMatches) #"),
                                attributes: { style: "text-align:right;" }
                            }
                        ],
                        pageable: true,
                        scrollable: false,
                        sortable: {
                            mode: "single",
                            allowUnsort: false
                        }
                    });
                };
                ClientMetricsModel.prototype.showJobs = function (email) {
                    history.pushState(null, "Reporting", "/Reporting");
                    window.location.replace("/JobProcessing/Summary?email=" + email);
                };
                return ClientMetricsModel;
            }());
            Dashboard.ClientMetricsModel = ClientMetricsModel;
            var AdminUserMetricsModel = (function () {
                function AdminUserMetricsModel() {
                }
                AdminUserMetricsModel.prototype.renderTab = function () {
                    console.log("AdminUserMetricsModel.renderTab");
                    this.renderAdminUserSummary();
                };
                AdminUserMetricsModel.prototype.renderAdminUserSummary = function () {
                    $("#adminUserActivityUserSummary").kendoGrid({
                        dataSource: {
                            autobind: false,
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = { userid: "74A0CC9B-DE78-40E3-A556-0732AADF4C46" };
                                    $.ajax({
                                        url: "/Reporting/AdminUserActivityMetrics/UserSummary",
                                        dataType: "json",
                                        type: "GET",
                                        data: data,
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: "json",
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                }
                            },
                            group: {
                                field: "Date",
                                dir: "desc"
                            },
                            sort: { field: "Hour", dir: "asc" }
                        },
                        columns: [
                            {
                                field: "Date",
                                title: "Date",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            },
                            {
                                field: "Hour",
                                title: "Hour",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" }
                            },
                            {
                                field: "LoginEvent",
                                title: "Logon Event",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("#= kendo.toString(LoginEvent, 'n0') == 0 ? '-' : kendo.toString(LoginEvent, 'n0') #")
                            },
                            {
                                field: "LeadsTouched",
                                title: "Leads Touched",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("#= kendo.toString(LeadsTouched, 'n0') == 0 ? '-' : kendo.toString(LeadsTouched, 'n0') #")
                            },
                            {
                                field: "CustomersTouched",
                                title: "Customers Touched",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("#= kendo.toString(CustomersTouched, 'n0') == 0 ? '-' : kendo.toString(CustomersTouched, 'n0') #")
                            },
                            {
                                field: "DealsTouched",
                                title: "Deals Touched",
                                headerAttributes: { style: "text-align: center;" },
                                attributes: { style: "text-align: center;" },
                                template: kendo.template("#= kendo.toString(DealsTouched, 'n0') == 0 ? '-' : kendo.toString(DealsTouched, 'n0') #")
                            }
                        ],
                        pageable: false,
                        scrollable: false
                    });
                };
                return AdminUserMetricsModel;
            }());
            Dashboard.AdminUserMetricsModel = AdminUserMetricsModel;
        })(Dashboard = Reporting.Dashboard || (Reporting.Dashboard = {}));
    })(Reporting = AccurateAppend.Reporting || (AccurateAppend.Reporting = {}));
})(AccurateAppend || (AccurateAppend = {}));
function displayProcessingMetric(records, matches) {
    if (records === 0)
        return "-";
    return kendo.toString(records, "n0") +
        " / " +
        kendo.toString(matches, "n0") +
        " (" +
        Math.floor((matches / records) * 100) +
        "%)";
}
function displayLeadMetric(qualifiedLeads, newClients, revenue) {
    if (qualifiedLeads === 0)
        return "-";
    return kendo.toString(qualifiedLeads, "n0") +
        " / " +
        kendo.toString(newClients, "n0") +
        " (" +
        Math.floor((newClients / qualifiedLeads) * 100) +
        "%) - " +
        kendo.toString(revenue, "c0");
}
function displayLeadMetricShort(qualifiedLeads, newClients) {
    if (qualifiedLeads === 0)
        return "-";
    return kendo.toString(qualifiedLeads, "n0") + "&nbsp;/&nbsp;" + kendo.toString(newClients, "n0");
}
var formatter = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiRGFzaGJvYXJkLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiRGFzaGJvYXJkLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUlBLElBQUksU0FBdUQsQ0FBQztBQUM1RCxJQUFJLHlCQUF1RixDQUFDO0FBQzVGLElBQUksMEJBQXlGLENBQUM7QUFDOUYsSUFBSSwwQkFBc0YsQ0FBQztBQUMzRixJQUFJLDBCQUFzRixDQUFDO0FBQzNGLElBQUksc0JBQTZFLENBQUM7QUFDbEYsSUFBSSxxQkFBK0UsQ0FBQztBQUVwRixDQUFDLENBQUM7SUFFQSxTQUFTLEdBQUcsSUFBSSxjQUFjLENBQUMsU0FBUyxDQUFDLFNBQVMsQ0FBQyxTQUFTLEVBQUUsQ0FBQztJQUMvRCx5QkFBeUIsR0FBRyxJQUFJLGNBQWMsQ0FBQyxTQUFTLENBQUMsU0FBUyxDQUFDLHlCQUF5QixFQUFFLENBQUM7SUFDL0YsMEJBQTBCLEdBQUcsSUFBSSxjQUFjLENBQUMsU0FBUyxDQUFDLFNBQVMsQ0FBQywwQkFBMEIsRUFBRSxDQUFDO0lBQ2pHLDBCQUEwQixHQUFHLElBQUksY0FBYyxDQUFDLFNBQVMsQ0FBQyxTQUFTLENBQUMsdUJBQXVCLEVBQUUsQ0FBQztJQUM5RixzQkFBc0IsR0FBRyxJQUFJLGNBQWMsQ0FBQyxTQUFTLENBQUMsU0FBUyxDQUFDLGtCQUFrQixDQUFDO0lBQ25GLHFCQUFxQixHQUFHLElBQUksY0FBYyxDQUFDLFNBQVMsQ0FBQyxTQUFTLENBQUMscUJBQXFCLENBQUM7SUFFckYsQ0FBQyxDQUFDLGdCQUFnQixDQUFDLENBQUMsSUFBSSxDQUFDLFFBQVEsRUFDL0I7UUFDRSxTQUFTLENBQUMsZ0JBQWdCLEVBQUUsQ0FBQztRQUM3QixVQUFVLEVBQUUsQ0FBQztJQUNmLENBQUMsQ0FBQyxDQUFDO0lBR0wsQ0FBQyxDQUFDLE1BQU0sQ0FBQyxDQUFDLE1BQU0sQ0FBQztRQUNmLEtBQUssQ0FBQyxNQUFNLENBQUMsQ0FBQyxDQUFDLGdDQUFnQyxDQUFDLEVBQUUs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cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9qcXVlcnkuY29va2llL2pxdWVyeS5jb29raWUuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3MvbW9tZW50L21vbWVudC5kLnRzXCIgLz5cclxuXHJcbnZhciB2aWV3TW9kZWw6IEFjY3VyYXRlQXBwZW5kLlJlcG9ydGluZy5EYXNoYm9hcmQuVmlld01vZGVsO1xyXG52YXIgb3BlcmF0aW5nTWV0cmljc1ZpZXdNb2RlbDogQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZC5PcGVyYXRpbmdNZXRyaWNzVmlld01vZGVsO1xyXG52YXIgcHJvY2Vzc2luZ01ldHJpY3NWaWV3TW9kZWw6IEFjY3VyYXRlQXBwZW5kLlJlcG9ydGluZy5EYXNoYm9hcmQuUHJvY2Vzc2luZ01ldHJpY3NWaWV3TW9kZWw7XHJcbnZhciB3ZWJzZXJ2aWNlTWV0cmljc1ZpZXdNb2RlbDogQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZC5XZWJTZXJ2aWNlc01ldHJpY3NNb2RlbDtcclxudmFyIHdlYnNlcnZpY2VNZXRyaWNzVmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5SZXBvcnRpbmcuRGFzaGJvYXJkLldlYlNlcnZpY2VzTWV0cmljc01vZGVsO1xyXG52YXIgY2xpZW50TWV0cmljc1ZpZXdNb2RlbDogQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZC5DbGllbnRNZXRyaWNzTW9kZWw7XHJcbnZhciBhZG1pblVzZXJNZXRyaWNzTW9kZWw6IEFjY3VyYXRlQXBwZW5kLlJlcG9ydGluZy5EYXNoYm9hcmQuQWRtaW5Vc2VyTWV0cmljc01vZGVsO1xyXG5cclxuJCgoKSA9PiB7XHJcblxyXG4gIHZpZXdNb2RlbCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5SZXBvcnRpbmcuRGFzaGJvYXJkLlZpZXdNb2RlbCgpO1xyXG4gIG9wZXJhdGluZ01ldHJpY3NWaWV3TW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZC5PcGVyYXRpbmdNZXRyaWNzVmlld01vZGVsKCk7XHJcbiAgcHJvY2Vzc2luZ01ldHJpY3NWaWV3TW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZC5Qcm9jZXNzaW5nTWV0cmljc1ZpZXdNb2RlbCgpO1xyXG4gIHdlYnNlcnZpY2VNZXRyaWNzVmlld01vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLlJlcG9ydGluZy5EYXNoYm9hcmQuV2ViU2VydmljZXNNZXRyaWNzTW9kZWwoKTtcclxuICBjbGllbnRNZXRyaWNzVmlld01vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLlJlcG9ydGluZy5EYXNoYm9hcmQuQ2xpZW50TWV0cmljc01vZGVsO1xyXG4gIGFkbWluVXNlck1ldHJpY3NNb2RlbCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5SZXBvcnRpbmcuRGFzaGJvYXJkLkFkbWluVXNlck1ldHJpY3NNb2RlbDtcclxuXHJcbiAgJChcIiNBcHBsaWNhdGlvbklkXCIpLmJpbmQoXCJjaGFuZ2VcIixcclxuICAgICgpID0+IHtcclxuICAgICAgdmlld01vZGVsLnNldEFwcGxpY2F0aW9uSWQoKTtcclxuICAgICAgcmVuZGVyVGFicygpO1xyXG4gICAgfSk7XHJcblxyXG4gIC8vIHJlc2l6ZSBjaGFydHMgd2luZG93IGlzIHJlc2l6ZWRcclxuICAkKHdpbmRvdykucmVzaXplKCgpID0+IHtcclxuICAgIGtlbmRvLnJlc2l6ZSgkKFwiZGl2LmstY2hhcnRbZGF0YS1yb2xlPSdjaGFydCddXCIpLCB0cnVlKTtcclxuICB9KTtcclxuXHJcbiAgLy8gcmVzaXplIGNoYXJ0cyB3aGVuIHRhYiBpcyBzd2l0Y2hlZFxyXG4gICQoJ2FbZGF0YS10b2dnbGU9XCJ0YWJcIl0nKS5vbihcInNob3duLmJzLnRhYlwiLFxyXG4gICAgZSA9PiB7XHJcbiAgICAgIGtlbmRvLnJlc2l6ZSgkKFwiZGl2LmstY2hhcnRbZGF0YS1yb2xlPSdjaGFydCddXCIpLCB0cnVlKTtcclxuICAgIH0pO1xyXG5cclxuICByZW5kZXJUYWJzKCk7XHJcblxyXG4gICQoXCIjU291cmNlXCIpLmJpbmQoXCJjaGFuZ2VcIixcclxuICAgICgpID0+IHtcclxuICAgICAgcHJvY2Vzc2luZ01ldHJpY3NWaWV3TW9kZWwucmVuZGVyU3Vic2NyaWJlclByb2Nlc3NpbmdIaXN0b3J5KCk7XHJcbiAgICB9KTtcclxuXHJcbiAgJChcIiNjbGllbnRQcm9jZXNzaW5nTWV0cmljc0dyaWRUb29sYmFyU291cmNlXCIpLmJpbmQoXCJjaGFuZ2VcIixcclxuICAgICgpID0+IHtcclxuICAgICAgY29uc29sZS5sb2coXCJmaXJpbmcgY2hhbmdlXCIpO1xyXG4gICAgICBjbGllbnRNZXRyaWNzVmlld01vZGVsLnJlbmRlclByb2Nlc3NpbmdNZXRyaWNzKCk7XHJcbiAgICB9KTtcclxuXHJcbiAgJChcIiNhZG1pblVzZXJBY3Rpdml0eVVzZXJTdW1tYXJ5R3JpZFRvb2xiYXJTb3VyY2VcIikuYmluZChcImNoYW5nZVwiLFxyXG4gICAgKCkgPT4ge1xyXG4gICAgICBjb25zb2xlLmxvZyhcImZpcmluZyBjaGFuZ2VcIik7XHJcbiAgICAgIGFkbWluVXNlck1ldHJpY3NNb2RlbC5yZW5kZXJBZG1pblVzZXJTdW1tYXJ5KCk7XHJcblxyXG4gICAgfSk7XHJcbn0pO1xyXG5cclxuZnVuY3Rpb24gcmVuZGVyVGFicygpIHtcclxuICBvcGVyYXRpbmdNZXRyaWNzVmlld01vZGVsLnJlbmRlclRhYigpO1xyXG4gIHByb2Nlc3NpbmdNZXRyaWNzVmlld01vZGVsLnJlbmRlclRhYigpO1xyXG4gIHdlYnNlcnZpY2VNZXRyaWNzVmlld01vZGVsLnJlbmRlclRhYigpO1xyXG4gIGNsaWVudE1ldHJpY3NWaWV3TW9kZWwucmVuZGVyVGFiKCk7XHJcbiAgYWRtaW5Vc2VyTWV0cmljc01vZGVsLnJlbmRlclRhYigpO1xyXG59XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuUmVwb3J0aW5nLkRhc2hib2FyZCB7XHJcblxyXG4gIGV4cG9ydCBjbGFzcyBWaWV3TW9kZWwge1xyXG5cclxuICAgIGxvYWRBcHBsaWNhdGlvbklkKCkge1xyXG4gICAgICBjb25zdCB2ID0gJC5jb29raWUoXCJBcHBsaWNhdGlvbklkXCIpO1xyXG4gICAgICBpZiAodiAhPT0gXCJcIikge1xyXG4gICAgICAgICQoYCNBcHBsaWNhdGlvbklkIG9wdGlvblt2YWx1ZT0keyQuY29va2llKFwiQXBwbGljYXRpb25JZFwiKX1dYCkuYXR0cihcInNlbGVjdGVkXCIsIFwic2VsZWN0ZWRcIik7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBzZXRBcHBsaWNhdGlvbklkKCkge1xyXG4gICAgICAkLmNvb2tpZShcIkFwcGxpY2F0aW9uSWRcIiwgJChcIiNBcHBsaWNhdGlvbklkIG9wdGlvbjpzZWxlY3RlZFwiKS52YWwoKSk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgT3BlcmF0aW5nTWV0cmljc1ZpZXdNb2RlbCB7XHJcblxyXG4gICAgcmVuZGVyVGFiKCkge1xyXG4gICAgICBjb25zb2xlLmxvZyhcIk9wZXJhdGluZ01ldHJpY3NWaWV3TW9kZWwucmVuZGVyVGFiXCIpO1xyXG4gICAgICB0aGlzLnJlbmRlckRlYWxNZXRyaWNPdmVydmlld1JlcG9ydEdyaWQoKTtcclxuICAgICAgdGhpcy5yZW5kZXJNcnJNZXRyaWNPdmVydmlld0dyaWQoKTtcclxuICAgICAgdGhpcy5yZW5kZXJBZ2VudE1ldHJpY092ZXJ2aWV3R3JpZCgpO1xyXG4gICAgICB0aGlzLnJlbmRlclJldmVudWVNZXRyaWNDaGFydCgpO1xyXG4gICAgICB0aGlzLnJlbmRlckxlYWRNZXRyaWNPdmVydmlld1JlcG9ydEdyaWQoKTtcclxuICAgICAgdGhpcy5yZW5kZXJMZWFkTWV0cmljQ2hhcnQoKTtcclxuICAgICAgdGhpcy5yZW5kZXJMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZCgxLCBcIiNMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZF9EaXJlY3RcIik7XHJcbiAgICAgIHRoaXMucmVuZGVyTGVhZENoYW5uZWxNZXRyaWNPdmVydmlld0dyaWQoMiwgXCIjTGVhZENoYW5uZWxNZXRyaWNPdmVydmlld0dyaWRfT3JnYW5pY1wiKTtcclxuICAgICAgdGhpcy5yZW5kZXJMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZCgzLCBcIiNMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZF9SZWZlcnJhbFwiKTtcclxuICAgICAgdGhpcy5yZW5kZXJMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZCg4LCBcIiNMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZF9OYXRpb25CdWlsZGVyXCIpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlckRlYWxNZXRyaWNPdmVydmlld1JlcG9ydEdyaWQoKSB7XHJcbiAgICAgIGNvbnN0IGdyaWQgPSAkKFwiI0RlYWxNZXRyaWNPdmVydmlld1JlcG9ydEdyaWRcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgICAgZ3JpZC5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICAkKFwiI0RlYWxNZXRyaWNPdmVydmlld1JlcG9ydEdyaWRcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgY29uc3QgZGF0YSA9IHsgYXBwbGljYXRpb25pZDogJChcIiNBcHBsaWNhdGlvbklkXCIpLnZhbCgpIH07XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9PcGVyYXRpbmdNZXRyaWNzL092ZXJ2aWV3UmVwb3J0XCIsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgeyBmaWVsZDogXCJNZXRyaWNOYW1lRGVzY3JpcHRpb25cIiwgdGl0bGU6IFwiRGVzY3JpcHRpb25cIiB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJUb2RheVwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI3RvZGF5VGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiWWVzdGVyZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiWWVzdGVyZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjeWVzdGVyZGF5VGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdDdcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJMYXN0IDdcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNsYXN0N1RlbXBsYXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkN1cnJlbnRNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNjdXJyZW50TW9udGhUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJTYW1lUGVyaW9kTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU2FtZSBQZXJpb2QgTGFzdCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI3NhbWVQZXJpb2RMYXN0TW9udGhUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJMYXN0TW9udGhcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJMYXN0IE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjTGFzdE1vbnRoVGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiUHJldmlvdXNUb0xhc3RNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlByZXZpb3VzIE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjcHJldmlvdXNUb0xhc3RNb250aFRlbXBsYXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgXVxyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcmVuZGVyTXJyTWV0cmljT3ZlcnZpZXdHcmlkKCkge1xyXG4gICAgICAgIGlmICgkKFwiI01yck1ldHJpY092ZXJ2aWV3R3JpZFwiKS5sZW5ndGggPT09IDApIHJldHVybjtcclxuXHJcbiAgICAgICQuYWpheCh7XHJcbiAgICAgICAgdXJsOiBcIi9SZXBvcnRpbmcvTXJyTWV0cmljcy9RdWVyeVwiLFxyXG4gICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgIGRhdGE6IHsgYXBwbGljYXRpb25JZDogJChcIiNBcHBsaWNhdGlvbklkXCIpLnZhbCgpIH0sXHJcbiAgICAgICAgc3VjY2VzcyhkYXRhKSB7XHJcbiAgICAgICAgICAvLyB3cml0ZSB0YWJsZVxyXG4gICAgICAgICAgY29uc3QgY29udGFpbmVyID0gJChcIiNNcnJNZXRyaWNPdmVydmlld0dyaWRcIikuZW1wdHkoKTs7XHJcbiAgICAgICAgICB2YXIgdGFibGUgPSAkKGA8dGFibGU+YCk7XHJcbiAgICAgICAgICAkLmVhY2goZGF0YSxcclxuICAgICAgICAgICAgKGtleSwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICBpZiAoa2V5ID09PSAwKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgdGggPSAkKGA8dGhlYWQgY2xhc3M9XCJrLWdyaWQtaGVhZGVyXCI+PHRyPmApO1xyXG4gICAgICAgICAgICAgICAgJC5lYWNoKE9iamVjdC5rZXlzKHZhbHVlKSxcclxuICAgICAgICAgICAgICAgICAgKGksIHByb3BlcnR5TmFtZSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoLmFwcGVuZChgPHRoIGNsYXNzPVwiay1oZWFkZXJcIj4ke3Byb3BlcnR5TmFtZX08L3RoPmApO1xyXG4gICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIHRhYmxlLmFwcGVuZCh0aCkuYXBwZW5kKGA8L3RoZWFkPmApO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB2YXIgdHIgPSAkKGA8dHIgJHtrZXkgJiAxID8gJ2NsYXNzPVwiay1hbHRcIicgOiBcIlwifSA+YCk7XHJcbiAgICAgICAgICAgICAgJC5lYWNoKHZhbHVlLFxyXG4gICAgICAgICAgICAgICAgKGksIHByb3BlcnR5VmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgdHIuYXBwZW5kKGA8dGQgc3R5bGU9XCJ0ZXh0LWFsaWduOiByaWdodDtcIj4ke3Byb3BlcnR5VmFsdWV9PC90ZD5gKTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgIHRhYmxlLmFwcGVuZCh0cik7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgY29udGFpbmVyLmFwcGVuZCh0YWJsZSk7XHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgICAgfVxyXG5cclxuICAgIHJlbmRlckFnZW50TWV0cmljT3ZlcnZpZXdHcmlkKCkge1xyXG5cclxuICAgICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9BZ2VudE1ldHJpY3MvUXVlcnlcIixcclxuICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICBkYXRhOiB7IGFwcGxpY2F0aW9uSWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSB9LFxyXG4gICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgLy8gd3JpdGUgdGFibGVcclxuICAgICAgICAgIGNvbnN0IGNvbnRhaW5lciA9ICQoXCIjQWdlbnRNZXRyaWNPdmVydmlld0dyaWRcIikuZW1wdHkoKTs7XHJcbiAgICAgICAgICB2YXIgdGFibGUgPSAkKGA8dGFibGU+YCk7XHJcbiAgICAgICAgICAkLmVhY2goZGF0YSxcclxuICAgICAgICAgICAgKGtleSwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICBpZiAoa2V5ID09PSAwKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgdGggPSAkKGA8dGhlYWQgY2xhc3M9XCJrLWdyaWQtaGVhZGVyXCI+PHRyPmApO1xyXG4gICAgICAgICAgICAgICAgJC5lYWNoKE9iamVjdC5rZXlzKHZhbHVlKSxcclxuICAgICAgICAgICAgICAgICAgKGksIHByb3BlcnR5TmFtZSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoLmFwcGVuZChgPHRoIGNsYXNzPVwiay1oZWFkZXJcIj4ke3Byb3BlcnR5TmFtZX08L3RoPmApO1xyXG4gICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIHRhYmxlLmFwcGVuZCh0aCkuYXBwZW5kKGA8L3RoZWFkPmApO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB2YXIgdHIgPSAkKGA8dHIgJHtrZXkgJiAxID8gJ2NsYXNzPVwiay1hbHRcIicgOiBcIlwifSA+YCk7XHJcbiAgICAgICAgICAgICAgJC5lYWNoKHZhbHVlLFxyXG4gICAgICAgICAgICAgICAgKGksIHByb3BlcnR5VmFsdWUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgdHIuYXBwZW5kKGA8dGQgc3R5bGU9XCJ0ZXh0LWFsaWduOiByaWdodDtcIj4ke3Byb3BlcnR5VmFsdWV9PC90ZD5gKTtcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgIHRhYmxlLmFwcGVuZCh0cik7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgY29udGFpbmVyLmFwcGVuZCh0YWJsZSk7XHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJMZWFkQ2hhbm5lbE1ldHJpY092ZXJ2aWV3R3JpZChsZWFkU291cmNlLCBzZWxlY3Rvcikge1xyXG5cclxuICAgICAgJC5hamF4KHtcclxuICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9MZWFkQ2hhbm5lbE1ldHJpY3MvUXVlcnlcIixcclxuICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICBkYXRhOiB7IGFwcGxpY2F0aW9uSWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSwgbGVhZFNvdXJjZTogbGVhZFNvdXJjZSB9LFxyXG4gICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgLy8gd3JpdGUgdGFibGVcclxuICAgICAgICAgIGNvbnN0IGNvbnRhaW5lciA9ICQoc2VsZWN0b3IpLmVtcHR5KCk7O1xyXG4gICAgICAgICAgdmFyIHRhYmxlID0gJChgPHRhYmxlPmApO1xyXG4gICAgICAgICAgJC5lYWNoKGRhdGEsXHJcbiAgICAgICAgICAgIChrZXksIHZhbHVlKSA9PiB7XHJcbiAgICAgICAgICAgICAgLy8gZGV0ZXJtaW5lIGZvcm1hdCBmb3IgdGhlIHJvdywgMyBpcyByZXZlbnVlIGFuZCBuZWVkcyB0byBiZSBmb3JtYXR0ZWQgYXMgY3VycmVuY3ksIG90aGVyd2lzZSBudW1iZXJcclxuICAgICAgICAgICAgICB2YXIgZm9ybWF0OiBzdHJpbmc7XHJcbiAgICAgICAgICAgICAgc3dpdGNoICh2YWx1ZS5NZXRyaWNOYW1lKSB7XHJcbiAgICAgICAgICAgICAgICBjYXNlIDM6XHJcbiAgICAgICAgICAgICAgICAgIGZvcm1hdCA9IFwicDFcIjtcclxuICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICBjYXNlIDQ6XHJcbiAgICAgICAgICAgICAgICBjYXNlIDY6XHJcbiAgICAgICAgICAgICAgICAgIGZvcm1hdCA9IFwiYzBcIjtcclxuICAgICAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgICAgICBkZWZhdWx0OlxyXG4gICAgICAgICAgICAgICAgICBmb3JtYXQgPSBcIm4wXCI7XHJcbiAgICAgICAgICAgICAgICAgIGJyZWFrO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAvLyBpZiBmaXJzdCByb3cgdGhlbiBjcmVhdGUgaGVhZGVyIHVzaW5nIGtleSBuYW1lXHJcbiAgICAgICAgICAgICAgaWYgKGtleSA9PT0gMCkge1xyXG4gICAgICAgICAgICAgICAgdmFyIHRoID0gJChgPHRoZWFkIGNsYXNzPVwiay1ncmlkLWhlYWRlclwiPjx0cj5gKTtcclxuICAgICAgICAgICAgICAgICQuZWFjaChPYmplY3Qua2V5cyh2YWx1ZSksXHJcbiAgICAgICAgICAgICAgICAgIChpLCBwcm9wZXJ0eU5hbWUpID0+IHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocHJvcGVydHlOYW1lICE9PSBcIk1ldHJpY05hbWVcIikge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aC5hcHBlbmQoYDx0aCBjbGFzcz1cImstaGVhZGVyXCIgc3R5bGU9XCJ0ZXh0LWFsaWduOiByaWdodDtcIj4ke3Byb3BlcnR5TmFtZX08L3RoPmApO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICB0YWJsZS5hcHBlbmQodGgpLmFwcGVuZChgPC90aGVhZD5gKTtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgdmFyIHRyID0gJChgPHRyICR7a2V5ICYgMSA/ICdjbGFzcz1cImstYWx0XCInIDogXCJcIn0gPmApO1xyXG4gICAgICAgICAgICAgICQuZWFjaCh2YWx1ZSxcclxuICAgICAgICAgICAgICAgIChuYW1lLCB2YWx1ZSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgICBpZiAobmFtZSAhPT0gXCJNZXRyaWNOYW1lXCIpIHtcclxuICAgICAgICAgICAgICAgICAgICB0ci5hcHBlbmQoYDx0ZCBzdHlsZT1cInRleHQtYWxpZ246IHJpZ2h0O1wiPiR7a2VuZG8udG9TdHJpbmcodmFsdWUsIGZvcm1hdCl9PC90ZD5gKTtcclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgdGFibGUuYXBwZW5kKHRyKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICBjb250YWluZXIuYXBwZW5kKHRhYmxlKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG5cclxuICAgIH1cclxuXHJcbiAgICAgIHJlbmRlclJldmVudWVNZXRyaWNDaGFydCgpIHtcclxuICAgICAgICAgIGlmICgkKFwiI1JldmVudWVNZXRyaWNDaGFydFwiKS5sZW5ndGggPT09IDApIHJldHVybjtcclxuXHJcbiAgICAgICQoXCIjUmV2ZW51ZU1ldHJpY0NoYXJ0XCIpLmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgIHRpdGxlOiB7XHJcbiAgICAgICAgICB0ZXh0OiBcIlJldmVudWUgQnkgTW9udGhcIlxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9PcGVyYXRpbmdNZXRyaWNzL1JldmVudWVcIixcclxuICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICBkYXRhOiB7IGFwcGxpY2F0aW9uaWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSB9LFxyXG4gICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBzY2hlbWE6IHsgZGF0YTogXCJEYXRhXCIgfSxcclxuICAgICAgICAgIHJlcXVlc3RTdGFydCgpIHtcclxuICAgICAgICAgICAga2VuZG8udWkucHJvZ3Jlc3MoJChcIiNSZXZlbnVlTWV0cmljQ2hhcnRMb2FkaW5nXCIpLCB0cnVlKTtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICByZXF1ZXN0RW5kKCkge1xyXG4gICAgICAgICAgICBrZW5kby51aS5wcm9ncmVzcygkKFwiI1JldmVudWVNZXRyaWNDaGFydExvYWRpbmdcIiksIGZhbHNlKTtcclxuXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBsZWdlbmQ6IHtcclxuICAgICAgICAgIHBvc2l0aW9uOiBcImJvdHRvbVwiXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzZXJpZXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiVG90YWxSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiVG90YWwgUmV2ZW51ZVwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjNzNjMTAwXCIsXHJcbiAgICAgICAgICAgIGFnZ3JlZ2F0ZTogXCJzdW1cIixcclxuICAgICAgICAgICAgdHlwZTogXCJhcmVhXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiRGF0ZVwiXHJcbiAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlJldmVudWVTdWJzY3JpYmVyXCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiU3Vic2NyaWJlciBSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiNmZmFlMDBcIixcclxuICAgICAgICAgICAgYWdncmVnYXRlOiBcInN1bVwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJEYXRlXCJcclxuICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiUmV2ZW51ZU5vblN1YnNjcmliZXJcIixcclxuICAgICAgICAgICAgbmFtZTogXCJOb24tc3Vic2NyaWJlciBSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiMwMDdlZmZcIixcclxuICAgICAgICAgICAgYWdncmVnYXRlOiBcInN1bVwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJEYXRlXCJcclxuICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiUmV2ZW51ZU5hdGlvbkJ1aWxkZXJcIixcclxuICAgICAgICAgICAgbmFtZTogXCJOYXRpb25CdWlsZGVyIFJldmVudWVcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiI2NjN2VmZlwiLFxyXG4gICAgICAgICAgICBhZ2dyZWdhdGU6IFwic3VtXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkRhdGVcIlxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF0sXHJcbiAgICAgICAgdmFsdWVBeGlzOiBbXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGxhYmVsczoge1xyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpDfVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGxpbmU6IHtcclxuICAgICAgICAgICAgICB2aXNpYmxlOiBmYWxzZVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBheGlzQ3Jvc3NpbmdWYWx1ZTogLTEwXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICBjYXRlZ29yeUF4aXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgICBiYXNlVW5pdDogXCJtb250aHNcIixcclxuICAgICAgICAgICAgYXhpc0Nyb3NzaW5nVmFsdWU6IFswLCAxMDBdLFxyXG4gICAgICAgICAgICB0eXBlOiBcImRhdGVcIlxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF0sXHJcbiAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgIGZvcm1hdDogXCJ7MDpDfVwiLFxyXG4gICAgICAgICAgdGVtcGxhdGU6IFwiIz0gc2VyaWVzLm5hbWUgIzogIz0ga2VuZG8udG9TdHJpbmcodmFsdWUsICdjJykgI1wiLFxyXG4gICAgICAgICAgY29sb3I6IFwiI2ZmZmZmZlwiXHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgICAgJChcIiNSZXZlbnVlTWV0cmljQ2hhcnRcIikuZGF0YShcImtlbmRvQ2hhcnRcIikucmVkcmF3KCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVuZGVyTGVhZE1ldHJpY092ZXJ2aWV3UmVwb3J0R3JpZCgpIHtcclxuICAgICAgY29uc3QgZ3JpZCA9ICQoXCIjTGVhZE1ldHJpY092ZXJ2aWV3UmVwb3J0R3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgICQoXCIjTGVhZE1ldHJpY092ZXJ2aWV3UmVwb3J0R3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICBjb25zdCBkYXRhID0geyBhcHBsaWNhdGlvbmlkOiAkKFwiI0FwcGxpY2F0aW9uSWRcIikudmFsKCkgfTtcclxuICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgIHVybDogXCIvUmVwb3J0aW5nL0xlYWRNZXRyaWNzL092ZXJ2aWV3UmVwb3J0XCIsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgbW9kZWw6IHtcclxuICAgICAgICAgICAgICAgIGZpZWxkczoge1xyXG4gICAgICAgICAgICAgICAgICBNZXRyaWNOYW1lRGVzY3JpcHRpb246IHsgdHlwZTogXCJzdHJpbmdcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBNZXRyaWNOYW1lOiB7IHR5cGU6IFwic3RyaW5nXCIgfSxcclxuICAgICAgICAgICAgICAgICAgVG9kYXk6IHsgdHlwZTogXCJudW1iZXJcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBZZXN0ZXJkYXk6IHsgdHlwZTogXCJudW1iZXJcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBMYXN0NzogeyB0eXBlOiBcIm51bWJlclwiIH0sXHJcbiAgICAgICAgICAgICAgICAgIExhc3QzMDogeyB0eXBlOiBcIm51bWJlclwiIH0sXHJcbiAgICAgICAgICAgICAgICAgIExhc3Q2MDogeyB0eXBlOiBcIm51bWJlclwiIH0sXHJcbiAgICAgICAgICAgICAgICAgIExhc3Q5MDogeyB0eXBlOiBcIm51bWJlclwiIH1cclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgIHsgZmllbGQ6IFwiTGVhZFNvdXJjZURlc2NyaXB0aW9uXCIsIHRpdGxlOiBcIkRlc2NyaXB0aW9uXCIgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlRvZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBkaXNwbGF5TGVhZE1ldHJpY1Nob3J0KFRvZGF5TGVhZENvdW50LCBUb2RheU5ld0NsaWVudENvdW50KSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJZZXN0ZXJkYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJZZXN0ZXJkYXlcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBkaXNwbGF5TGVhZE1ldHJpY1Nob3J0KFllc3RlcmRheUxlYWRDb3VudCwgWWVzdGVyZGF5TmV3Q2xpZW50Q291bnQpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkxhc3Q3XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCA3XCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5TGVhZE1ldHJpY1Nob3J0KExhc3Q3UmVjb3Jkc0xlYWRDb3VudCwgTGFzdDdSZWNvcmRzTmV3Q2xpZW50Q291bnQpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkN1cnJlbnRNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgICBcIiM9IGRpc3BsYXlMZWFkTWV0cmljKEN1cnJlbnRNb250aExlYWRDb3VudCwgQ3VycmVudE1vbnRoTmV3Q2xpZW50Q291bnQsIEN1cnJlbnRNb250aFJldmVudWVBbW91bnQpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlNhbWVQZXJpb2RMYXN0TW9udGhcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJTYW1lIFBlcmlvZCBMYXN0IE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5TGVhZE1ldHJpYyhTYW1lUGVyaW9kTGFzdE1vbnRoTGVhZENvdW50LCBTYW1lUGVyaW9kTGFzdE1vbnRoTmV3Q2xpZW50Q291bnQsIFNhbWVQZXJpb2RMYXN0TW9udGhSZXZlbnVlQW1vdW50KSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJMYXN0TW9udGhcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJMYXN0IE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5TGVhZE1ldHJpYyhMYXN0TW9udGhMZWFkQ291bnQsIExhc3RNb250aE5ld0NsaWVudENvdW50LCBMYXN0TW9udGhSZXZlbnVlQW1vdW50KSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJQcmV2aW91c1RvTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiUHJldmlvdXMgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgICBcIiM9IGRpc3BsYXlMZWFkTWV0cmljKFByZXZpb3VzVG9MYXN0TGVhZENvdW50LCBQcmV2aW91c1RvTGFzdE5ld0NsaWVudENvdW50LCBQcmV2aW91c1RvTGFzdFJldmVudWVBbW91bnQpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlJvbGxpbmcxMk1vbnRoc1wiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlJvbGxpbmcgMTIgTW9udGhzXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5TGVhZE1ldHJpYyhSb2xsaW5nMTJNb250aHNMZWFkQ291bnQsIFJvbGxpbmcxMk1vbnRoc05ld0NsaWVudENvdW50LCBSb2xsaW5nMTJNb250aHNSZXZlbnVlQW1vdW50KSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgIHdpZHRoOiAyMDBcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgeyBmaWVsZDogXCJUaW1lVG9GaXJzdFB1cmNoYXNlXCIsIHRpdGxlOiBcIkRheXMgVG8gJCQkXCIsIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9IH1cclxuICAgICAgICAgIF1cclxuICAgICAgICB9KTtcclxuICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlckxlYWRNZXRyaWNDaGFydCgpIHtcclxuICAgICAgJChcIiNsZWFkU3VtYW1yeUNoYXJ0XCIpLmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgIHRpdGxlOiB7XHJcbiAgICAgICAgICB0ZXh0OiBcIkxlYWQgQWN0aXZpdHkgQnkgV2Vla1wiXHJcbiAgICAgICAgfSxcclxuICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgIHVybDogXCIvUmVwb3J0aW5nL0xlYWRSZXBvcnRzL0xlYWRNZXRyaWNTdW1tYXJ5XCIsXHJcbiAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgZGF0YTogeyBhcHBsaWNhdGlvbmlkOiAkKFwiI0FwcGxpY2F0aW9uSWRcIikudmFsKCkgfSxcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgc2NoZW1hOiB7IGRhdGE6IFwiRGF0YVwiIH0sXHJcbiAgICAgICAgICByZXF1ZXN0U3RhcnQoKSB7XHJcbiAgICAgICAgICAgIGtlbmRvLnVpLnByb2dyZXNzKCQoXCIjTGVhZFN1bW1hcnlDaGFydExvYWRpbmdcIiksIHRydWUpO1xyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHJlcXVlc3RFbmQoKSB7XHJcbiAgICAgICAgICAgIGtlbmRvLnVpLnByb2dyZXNzKCQoXCIjTGVhZFN1bW1hcnlDaGFydExvYWRpbmdcIiksIGZhbHNlKTtcclxuXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBsZWdlbmQ6IHtcclxuICAgICAgICAgIHBvc2l0aW9uOiBcImJvdHRvbVwiXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzZXJpZXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiVG90YWxcIixcclxuICAgICAgICAgICAgbmFtZTogXCJUb3RhbFwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjNzNjMTAwXCIsXHJcbiAgICAgICAgICAgIGFnZ3JlZ2F0ZTogXCJzdW1cIixcclxuICAgICAgICAgICAgdHlwZTogXCJhcmVhXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgfSwge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJRdWFsaWZpZWRcIixcclxuICAgICAgICAgICAgbmFtZTogXCJRdWFsaWZpZWRcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiI2ZmYWUwMFwiLFxyXG4gICAgICAgICAgICBhZ2dyZWdhdGU6IFwic3VtXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkRhdGVcIixcclxuICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiQ29udmVydGVkXCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiVG8gQ2xpZW50XCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiMwMDdlZmZcIixcclxuICAgICAgICAgICAgYWdncmVnYXRlOiBcInN1bVwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJEYXRlXCIsXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICB2YWx1ZUF4aXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgbGFiZWxzOiB7XHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswfVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGxpbmU6IHtcclxuICAgICAgICAgICAgICB2aXNpYmxlOiBmYWxzZVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBheGlzQ3Jvc3NpbmdWYWx1ZTogLTEwXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICBjYXRlZ29yeUF4aXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgICBiYXNlVW5pdDogXCJ3ZWVrc1wiLFxyXG4gICAgICAgICAgICBheGlzQ3Jvc3NpbmdWYWx1ZTogWzAsIDEwMF0sXHJcbiAgICAgICAgICAgIHR5cGU6IFwiZGF0ZVwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICB2aXNpYmxlOiB0cnVlLFxyXG4gICAgICAgICAgZm9ybWF0OiBcInswfVwiLFxyXG4gICAgICAgICAgdGVtcGxhdGU6IFwiIz0gc2VyaWVzLm5hbWUgIzogIz0gdmFsdWUgI1wiLFxyXG4gICAgICAgICAgY29sb3I6IFwiI2ZmZmZmZlwiXHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgICAgJChcIiNsZWFkU3VtYW1yeUNoYXJ0XCIpLmRhdGEoXCJrZW5kb0NoYXJ0XCIpLnJlZHJhdygpO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZXhwb3J0IGNsYXNzIFByb2Nlc3NpbmdNZXRyaWNzVmlld01vZGVsIHtcclxuXHJcbiAgICByZW5kZXJUYWIoKSB7XHJcbiAgICAgIGNvbnNvbGUubG9nKFwiUHJvY2Vzc2luZ01ldHJpY3NWaWV3TW9kZWwucmVuZGVyVGFiXCIpO1xyXG4gICAgICB0aGlzLnJlbmRlclByb2Nlc3NpbmdNZXRyaWNPdmVydmlld1JlcG9ydEdyaWQoKTtcclxuICAgICAgdGhpcy5yZW5kZXJTdWJzY3JpYmVyUHJvY2Vzc2luZ0hpc3RvcnkoKTtcclxuICAgICAgdGhpcy5yZW5kZXJTdWJzY3JpYmVyQWN0aXZpdHlNb250aENvbXBhcmlzb24oKTtcclxuICAgICAgdGhpcy5yZW5kZXJKb2JRdWV1ZUFjdGl2aXR5TGFzdDI0SG91cnMoKTtcclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJQcm9jZXNzaW5nTWV0cmljT3ZlcnZpZXdSZXBvcnRHcmlkKCkge1xyXG4gICAgICBjb25zdCBncmlkID0gJChcIiNQcm9jZXNzaW5nTWV0cmljT3ZlcnZpZXdSZXBvcnRHcmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgIGlmIChncmlkICE9PSB1bmRlZmluZWQgJiYgZ3JpZCAhPT0gbnVsbCkge1xyXG4gICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiNQcm9jZXNzaW5nTWV0cmljT3ZlcnZpZXdSZXBvcnRHcmlkXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICByZWFkKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSB7IGFwcGxpY2F0aW9uaWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSB9O1xyXG4gICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgdXJsOiBcIi9SZXBvcnRpbmcvT3BlcmF0aW9uTWV0cmljcy9PdmVydmlld1JlcG9ydFwiLFxyXG4gICAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgIG1vZGVsOiB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZHM6IHtcclxuICAgICAgICAgICAgICAgICAgTWV0cmljTmFtZURlc2NyaXB0aW9uOiB7IHR5cGU6IFwic3RyaW5nXCIgfSxcclxuICAgICAgICAgICAgICAgICAgTWV0cmljTmFtZTogeyB0eXBlOiBcInN0cmluZ1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgIFRvZGF5OiB7IHR5cGU6IFwibnVtYmVyXCIgfSxcclxuICAgICAgICAgICAgICAgICAgWWVzdGVyZGF5OiB7IHR5cGU6IFwibnVtYmVyXCIgfSxcclxuICAgICAgICAgICAgICAgICAgTGFzdDc6IHsgdHlwZTogXCJudW1iZXJcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBMYXN0MzA6IHsgdHlwZTogXCJudW1iZXJcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBMYXN0NjA6IHsgdHlwZTogXCJudW1iZXJcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBMYXN0OTA6IHsgdHlwZTogXCJudW1iZXJcIiB9XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICB7IGZpZWxkOiBcIk9wZXJhdGlvblwiLCB0aXRsZTogXCJEZXNjcmlwdGlvblwiIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJUb2RheVwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlRvZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0gZGlzcGxheVByb2Nlc3NpbmdNZXRyaWMoVG9kYXlSZWNvcmRzLFRvZGF5TWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiWWVzdGVyZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiWWVzdGVyZGF5XCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0gZGlzcGxheVByb2Nlc3NpbmdNZXRyaWMoWWVzdGVyZGF5UmVjb3JkcyxZZXN0ZXJkYXlNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJMYXN0N1wiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgN1wiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcIiM9IGRpc3BsYXlQcm9jZXNzaW5nTWV0cmljKExhc3Q3UmVjb3JkcyxMYXN0N01hdGNoZXMpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkN1cnJlbnRNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhDdXJyZW50TW9udGhSZWNvcmRzLEN1cnJlbnRNb250aE1hdGNoZXMpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlNhbWVQZXJpb2RMYXN0TW9udGhcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJTYW1lIFBlcmlvZCBMYXN0IE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhTYW1lUGVyaW9kTGFzdE1vbnRoUmVjb3JkcyxTYW1lUGVyaW9kTGFzdE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcIiM9IGRpc3BsYXlQcm9jZXNzaW5nTWV0cmljKExhc3RNb250aFJlY29yZHMsTGFzdE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiUHJldmlvdXNUb0xhc3RNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlByZXZpb3VzIE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhQcmV2aW91c1RvTGFzdE1vbnRoUmVjb3JkcyxQcmV2aW91c1RvTGFzdE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICBdXHJcbiAgICAgICAgfSk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJTdWJzY3JpYmVyUHJvY2Vzc2luZ0hpc3RvcnkoKSB7XHJcbiAgICAgICQoXCIjc3Vic2NyaWJlclByb2Nlc3NpbmdIaXN0b3J5XCIpLmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICByZWFkKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgdXJsOlxyXG4gICAgICAgICAgICAgICAgICBgL1JlcG9ydGluZy9Kb2JNZXRyaWNzL1N1YnNjcmliZXJKb2JNYXRjaENvdW50cz9hZ2dyZWdhdGU9MCZzb3VyY2U9JHskKFwiI1NvdXJjZVwiKS52YWwoKX0mc3RhcnREYXRlPSR7XHJcbiAgICAgICAgICAgICAgICAgICAgbW9tZW50KCkuc3VidHJhY3QoXCJtb250aFwiLCA4KS5mb3JtYXQoXCJMXCIpfSZlbmREYXRlPSR7bW9tZW50KCkuZm9ybWF0KFwiTFwiKX0mYXBwbGljYXRpb25pZD0keyQoXHJcbiAgICAgICAgICAgICAgICAgICAgICBcIiNBcHBsaWNhdGlvbklkXCIpLnZhbCgpfWAsXHJcbiAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBzY2hlbWE6IHsgZGF0YTogXCJEYXRhXCIgfSxcclxuICAgICAgICAgIHJlcXVlc3RTdGFydCgpIHtcclxuICAgICAgICAgICAga2VuZG8udWkucHJvZ3Jlc3MoJChcIiNzdWJzY3JpYmVyUHJvY2Vzc2luZ0hpc3RvcnlMb2FkaW5nXCIpLCB0cnVlKTtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICByZXF1ZXN0RW5kKCkge1xyXG4gICAgICAgICAgICBrZW5kby51aS5wcm9ncmVzcygkKFwiI3N1YnNjcmliZXJQcm9jZXNzaW5nSGlzdG9yeUxvYWRpbmdcIiksIGZhbHNlKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9LFxyXG4gICAgICAgIHNlcmllczpcclxuICAgICAgICBbXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkNvdW50XCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiVG90YWxcIixcclxuICAgICAgICAgICAgdHlwZTogXCJhcmVhXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjNzNjMTAwXCJcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlN0YW5kYXJkUGhvbmVBcHBlbmRcIixcclxuICAgICAgICAgICAgbmFtZTogXCJTdGFuZGFyZCBQaG9uZVwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJEYXRlXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiNDQzk5MDBcIlxyXG4gICAgICAgICAgfSwge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJQcmVtaXVtUGhvbmVBcHBlbmRcIixcclxuICAgICAgICAgICAgbmFtZTogXCJQcmVtaXVtIFBob25lXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkRhdGVcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiI0ZGNjYwMFwiXHJcbiAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkVtYWlsQXBwZW5kXCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiRW1haWwgQXBwZW5kXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkRhdGVcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiIzAwN2VmZlwiXHJcbiAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIk90aGVyQXBwZW5kc1wiLFxyXG4gICAgICAgICAgICBuYW1lOiBcIk90aGVyXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkRhdGVcIixcclxuICAgICAgICAgIH1cclxuICAgICAgICBdLFxyXG4gICAgICAgIGxlZ2VuZDoge1xyXG4gICAgICAgICAgcG9zaXRpb246IFwiYm90dG9tXCJcclxuICAgICAgICB9LFxyXG4gICAgICAgIHZhbHVlQXhpczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBsYWJlbHM6IHtcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgYXhpc0Nyb3NzaW5nVmFsdWU6IC0xMCxcclxuICAgICAgICAgICAgbGluZToge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IGZhbHNlXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH1cclxuICAgICAgICBdLFxyXG4gICAgICAgIGNhdGVnb3J5QXhpczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJEYXRlXCIsXHJcbiAgICAgICAgICAgIGJhc2VVbml0OiBcIm1vbnRoc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImRhdGVcIlxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF0sXHJcbiAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIlxyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcbiAgICAgICQoXCIjc3Vic2NyaWJlclByb2Nlc3NpbmdIaXN0b3J5XCIpLmRhdGEoXCJrZW5kb0NoYXJ0XCIpLnJlZHJhdygpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlclN1YnNjcmliZXJBY3Rpdml0eU1vbnRoQ29tcGFyaXNvbigpIHtcclxuICAgICAgJChcIiNzdWJzY3JpYmVyQWN0aXZpdHlNb250aENvbXBhcmlzb25cIikua2VuZG9DaGFydCh7XHJcbiAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6XHJcbiAgICAgICAgICAgICAgICAgIGAvUmVwb3J0aW5nL0pvYk1ldHJpY3MvU3Vic2NyaWJlckpvYk1hdGNoQ291bnRzQ29tcGFyaXNvbj9jb21wYXJlPTAmYXBwbGljYXRpb25pZD0keyQoXHJcbiAgICAgICAgICAgICAgICAgICAgXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKX1gLFxyXG4gICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgc2NoZW1hOiB7IGRhdGE6IFwiRGF0YVwiIH0sXHJcbiAgICAgICAgICByZXF1ZXN0U3RhcnQoKSB7XHJcbiAgICAgICAgICAgIGtlbmRvLnVpLnByb2dyZXNzKCQoXCIjc3Vic2NyaWJlckFjdGl2aXR5TW9udGhDb21wYXJpc29uQ2hhcnRMb2FkaW5nXCIpLCB0cnVlKTtcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICByZXF1ZXN0RW5kKCkge1xyXG4gICAgICAgICAgICBrZW5kby51aS5wcm9ncmVzcygkKFwiI3N1YnNjcmliZXJBY3Rpdml0eU1vbnRoQ29tcGFyaXNvbkNoYXJ0TG9hZGluZ1wiKSwgZmFsc2UpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc2VyaWVzOlxyXG4gICAgICAgIFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiQ3VycmVudFwiLFxyXG4gICAgICAgICAgICBuYW1lOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgdHlwZTogXCJsaW5lXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiM3M2MxMDBcIlxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiUHJldmlvdXNcIixcclxuICAgICAgICAgICAgbmFtZTogXCJQcmV2aW91cyBNb250aFwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiI0NDOTkwMFwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICBsZWdlbmQ6IHtcclxuICAgICAgICAgIHBvc2l0aW9uOiBcImJvdHRvbVwiXHJcbiAgICAgICAgfSxcclxuICAgICAgICB2YWx1ZUF4aXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgbGFiZWxzOiB7XHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGxpbmU6IHtcclxuICAgICAgICAgICAgICB2aXNpYmxlOiBmYWxzZVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICB2aXNpYmxlOiB0cnVlLFxyXG4gICAgICAgICAgZm9ybWF0OiBcInswfVwiLFxyXG4gICAgICAgICAgdGVtcGxhdGU6IFwiPHNwYW4gc3R5bGU9J2NvbG9yOiB3aGl0ZTsnPk1hdGNoZXM6ICM9IHZhbHVlICM8L3NwYW4+XCJcclxuICAgICAgICB9LFxyXG4gICAgICAgIGNhdGVnb3J5QXhpczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJEYXlcIixcclxuICAgICAgICAgICAgdHlwZTogXCJudW1iZXJcIlxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVuZGVySm9iUXVldWVBY3Rpdml0eUxhc3QyNEhvdXJzKCkge1xyXG4gICAgICAkKFwiI2pvYlF1ZXVlQWN0aXZpdHlMYXN0MjRIb3Vyc1wiKS5rZW5kb0NoYXJ0KHtcclxuICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgIHVybDogYC9SZXBvcnRpbmcvSm9iTWV0cmljcy9HZXRHcmFwaEZvckxhc3QyNEhvdXJzP2FwcGxpY2F0aW9uaWQ9JHskKFwiI0FwcGxpY2F0aW9uSWRcIikudmFsKCl9YCxcclxuICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHNjaGVtYTogeyBkYXRhOiBcIkRhdGFcIiB9LFxyXG4gICAgICAgICAgcmVxdWVzdFN0YXJ0KCkge1xyXG4gICAgICAgICAgICBrZW5kby51aS5wcm9ncmVzcygkKFwiI2pvYlF1ZXVlQWN0aXZpdHlMYXN0MjRIb3Vyc0xvYWRpbmdcIiksIHRydWUpO1xyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHJlcXVlc3RFbmQoKSB7XHJcbiAgICAgICAgICAgIGtlbmRvLnVpLnByb2dyZXNzKCQoXCIjam9iUXVldWVBY3Rpdml0eUxhc3QyNEhvdXJzTG9hZGluZ1wiKSwgZmFsc2UpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgc2VyaWVzOlxyXG4gICAgICAgIFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTGlzdGJ1aWxkZXJDb3VudFwiLFxyXG4gICAgICAgICAgICBuYW1lOiBcIkxpc3RidWlsZGVyXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibGluZVwiLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiIzAwRkZDQ1wiLFxyXG4gICAgICAgICAgICBheGlzOiBcIkNvdW50XCIsXHJcbiAgICAgICAgICAgIHRvb2x0aXA6IHtcclxuICAgICAgICAgICAgICB2aXNpYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpOMH1cIixcclxuICAgICAgICAgICAgICBjb2xvcjogXCIjZmZmXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IFwiPHNwYW4gc3R5bGU9J2NvbG9yOiB3aGl0ZTsnPkxpc3RidWlsZGVyIEZpbGVzOiAjPSB2YWx1ZSAjPC9zcGFuPlwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkNsaWVudENvdW50XCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiQ2xpZW50c1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJIb3VyXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiM5OTQ0Q0NcIixcclxuICAgICAgICAgICAgYXhpczogXCJDb3VudFwiLFxyXG4gICAgICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgY29sb3I6IFwiI2ZmZlwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxzcGFuIHN0eWxlPSdjb2xvcjogd2hpdGU7Jz5DbGllbnQgRmlsZXM6ICM9IHZhbHVlICM8L3NwYW4+XCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTmF0aW9uQnVpbGRlckNvdW50XCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiTkJcIixcclxuICAgICAgICAgICAgdHlwZTogXCJsaW5lXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiSG91clwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjNDRDQzk5XCIsXHJcbiAgICAgICAgICAgIGF4aXM6IFwiQ291bnRcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+TkIgRmlsZXM6ICM9IHZhbHVlICM8L3NwYW4+XCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRnRwQ291bnRcIixcclxuICAgICAgICAgICAgbmFtZTogXCJGdHBcIixcclxuICAgICAgICAgICAgdHlwZTogXCJsaW5lXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiSG91clwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjQ0M5OTAwXCIsXHJcbiAgICAgICAgICAgIGF4aXM6IFwiQ291bnRcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+RlRQIEZpbGVzOiAjPSB2YWx1ZSAjPC9zcGFuPlwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkVtYWlsQ291bnRcIixcclxuICAgICAgICAgICAgbmFtZTogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxpbmVcIixcclxuICAgICAgICAgICAgY2F0ZWdvcnlGaWVsZDogXCJIb3VyXCIsXHJcbiAgICAgICAgICAgIGNvbG9yOiBcIiNGRjY2MDBcIixcclxuICAgICAgICAgICAgYXhpczogXCJDb3VudFwiLFxyXG4gICAgICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgY29sb3I6IFwiI2ZmZlwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxzcGFuIHN0eWxlPSdjb2xvcjogd2hpdGU7Jz5FbWFpbCBGaWxlczogIz0gdmFsdWUgIzwvc3Bhbj5cIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJBZG1pbkNvdW50XCIsXHJcbiAgICAgICAgICAgIG5hbWU6IFwiQWRtaW5cIixcclxuICAgICAgICAgICAgdHlwZTogXCJsaW5lXCIsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiSG91clwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjMDA3ZWZmXCIsXHJcbiAgICAgICAgICAgIGF4aXM6IFwiQ291bnRcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+QWRtaW4gRmlsZXM6ICM9IHZhbHVlICM8L3NwYW4+XCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRnRwUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImJhclwiLFxyXG4gICAgICAgICAgICAvL3N0YWNrOiB0cnVlLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiI0NDOTkwMFwiLFxyXG4gICAgICAgICAgICBheGlzOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+RlRQIFJlY29yZHM6ICM9IHZhbHVlICM8L3NwYW4+XCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRW1haWxSZWNvcmRzXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwiYmFyXCIsXHJcbiAgICAgICAgICAgIC8vc3RhY2s6IHRydWUsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiSG91clwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjRkY2NjAwXCIsXHJcbiAgICAgICAgICAgIGF4aXM6IFwiUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgY29sb3I6IFwiI2ZmZlwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxzcGFuIHN0eWxlPSdjb2xvcjogd2hpdGU7Jz5FbWFpbCBSZWNvcmRzOiAjPSB2YWx1ZSAjPC9zcGFuPlwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkFkbWluUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImJhclwiLFxyXG4gICAgICAgICAgICAvL3N0YWNrOiB0cnVlLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiIzAwN2VmZlwiLFxyXG4gICAgICAgICAgICBheGlzOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+QWRtaW4gUmVjb3JkczogIz0gdmFsdWUgIzwvc3Bhbj5cIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJOYXRpb25CdWlsZGVyUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImJhclwiLFxyXG4gICAgICAgICAgICAvL3N0YWNrOiB0cnVlLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiIzQ0Q0M5OVwiLFxyXG4gICAgICAgICAgICBheGlzOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+TkIgUmVjb3JkczogIz0gdmFsdWUgIzwvc3Bhbj5cIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJDbGllbnRSZWNvcmRzXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwiYmFyXCIsXHJcbiAgICAgICAgICAgIC8vc3RhY2s6IHRydWUsXHJcbiAgICAgICAgICAgIGNhdGVnb3J5RmllbGQ6IFwiSG91clwiLFxyXG4gICAgICAgICAgICBjb2xvcjogXCIjOTk0NENDXCIsXHJcbiAgICAgICAgICAgIGF4aXM6IFwiUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0b29sdGlwOiB7XHJcbiAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCIsXHJcbiAgICAgICAgICAgICAgY29sb3I6IFwiI2ZmZlwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBcIjxzcGFuIHN0eWxlPSdjb2xvcjogd2hpdGU7Jz5OQiBSZWNvcmRzOiAjPSB2YWx1ZSAjPC9zcGFuPlwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkxpc3RidWlsZGVyUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImJhclwiLFxyXG4gICAgICAgICAgICAvL3N0YWNrOiB0cnVlLFxyXG4gICAgICAgICAgICBjYXRlZ29yeUZpZWxkOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgY29sb3I6IFwiIzAwRkZDQ1wiLFxyXG4gICAgICAgICAgICBheGlzOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgZm9ybWF0OiBcInswOk4wfVwiLFxyXG4gICAgICAgICAgICAgIGNvbG9yOiBcIiNmZmZcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZTogXCI8c3BhbiBzdHlsZT0nY29sb3I6IHdoaXRlOyc+TGlzdGJ1aWxkZXIgUmVjb3JkczogIz0gdmFsdWUgIzwvc3Bhbj5cIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICBsZWdlbmQ6IHtcclxuICAgICAgICAgIHBvc2l0aW9uOiBcImJvdHRvbVwiLFxyXG4gICAgICAgICAgLy90aXRsZTogXCJIb3VyIG9mIERheVwiXHJcbiAgICAgICAgfSxcclxuICAgICAgICB2YWx1ZUF4aXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgbmFtZTogXCJDb3VudFwiLFxyXG4gICAgICAgICAgICBsYWJlbHM6IHtcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgbGluZToge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IGZhbHNlXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHRpdGxlOiB7XHJcbiAgICAgICAgICAgICAgdGV4dDogXCJGaWxlcyBTdWJtaXR0ZWRcIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgIG5hbWU6IFwiUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICB0eXBlOiBcImxvZ1wiLFxyXG4gICAgICAgICAgICBsYWJlbHM6IHtcclxuICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6TjB9XCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgbGluZToge1xyXG4gICAgICAgICAgICAgIHZpc2libGU6IGZhbHNlXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHRpdGxlOiB7XHJcbiAgICAgICAgICAgICAgdGV4dDogXCJSZWNvcmRzIFN1Ym1pdHRlZFwiXHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH1cclxuICAgICAgICBdLFxyXG4gICAgICAgIGNhdGVnb3J5QXhpczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJIb3VyXCIsXHJcbiAgICAgICAgICAgIHR5cGU6IFwibnVtYmVyXCIsXHJcbiAgICAgICAgICAgIGF4aXNDcm9zc2luZ1ZhbHVlOiBbMCwgMjZdXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXVxyXG4gICAgICB9KTtcclxuICAgICAgJChcIiNqb2JRdWV1ZUFjdGl2aXR5TGFzdDI0SG91cnNcIikuZGF0YShcImtlbmRvQ2hhcnRcIikucmVkcmF3KCk7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgV2ViU2VydmljZXNNZXRyaWNzTW9kZWwge1xyXG5cclxuICAgIHJlbmRlclRhYigpIHtcclxuICAgICAgY29uc29sZS5sb2coXCJXZWJTZXJ2aWNlc01ldHJpY3NNb2RlbC5yZW5kZXJUYWJcIik7XHJcbiAgICAgIHRoaXMucmVuZGVyVHJhbnNhY3Rpb25zQnlVc2VyR3JpZCgpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlclRyYW5zYWN0aW9uc0J5VXNlckdyaWQoKSB7XHJcbiAgICAgICQoXCIjdHJhbnNhY3Rpb25zQnlVc2VyXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgYXV0b2JpbmQ6IGZhbHNlLFxyXG4gICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgIHVybDogXCIvUmVwb3J0aW5nL0FwaU1ldHJpY3MvR2V0Q2FsbHNCeVVzZXJKc29uXCIsXHJcbiAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHBhZ2VTaXplOiAxMFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJVc2VybmFtZVwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCI8YSBocmVmPVxcXCIvVXNlcnMvRGV0YWlsP3VzZXJpZD0jPVVzZXJJZCNcXFwiPiM9RW1haWwjPC9hPlwiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiQ291bnRcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiQ291bnRcIixcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfVxyXG4gICAgICAgICAgfVxyXG4gICAgICAgICAgLy97IGNvbW1hbmQ6IHsgdGV4dDogXCJWaWV3IERldGFpbHNcIiwgY2xpY2s6IHZpZXdEZXRhaWxzIH0sIHRpdGxlOiBcIiBcIiwgd2lkdGg6IFwiMTQwcHhcIiB9XHJcbiAgICAgICAgXVxyXG4gICAgICB9KTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGV4cG9ydCBjbGFzcyBDbGllbnRNZXRyaWNzTW9kZWwge1xyXG5cclxuICAgIHJlbmRlclRhYigpIHtcclxuICAgICAgY29uc29sZS5sb2coXCJDbGllbnRNZXRyaWNzTW9kZWwucmVuZGVyVGFiXCIpO1xyXG4gICAgICB0aGlzLnJlbmRlclJlY2VudERlYWxzKCk7XHJcbiAgICAgIHRoaXMucmVuZGVyRGVhbE1ldHJpY3MoKTtcclxuICAgICAgdGhpcy5yZW5kZXJQcm9jZXNzaW5nTWV0cmljcygpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlclJlY2VudERlYWxzKCkge1xyXG4gICAgICAkKFwiI3JlY2VudERlYWxzR3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgIGF1dG9iaW5kOiBmYWxzZSxcclxuICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSB7IGFwcGxpY2F0aW9uaWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSB9O1xyXG4gICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9EZWFsTWV0cmljcy9SZWNlbnREZWFsc1wiLFxyXG4gICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgcGFnZVNpemU6IDIwLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCI8YSBocmVmPVxcXCIvVXNlcnMvRGV0YWlsP3VzZXJpZD0jPVVzZXJJZCNcXFwiPiM9RW1haWwjPC9hPlwiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRGF0ZURlYWxDcmVhdGVkXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkRlYWwgRGF0ZVwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJBbW91bnRcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiQW1vdW50XCIsXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcIiM9IGtlbmRvLnRvU3RyaW5nKEFtb3VudCwgJ2MwJykgI1wiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiRGF0ZUFjY291bnRDcmVhdGVkXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkFjY291bnQgQ3JlYXRlIERhdGVcIixcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgdGVtcGxhdGU6IFwiIz0ga2VuZG8udG9TdHJpbmcoa2VuZG8ucGFyc2VEYXRlKERhdGVBY2NvdW50Q3JlYXRlZCksICdNTS9kZC95eXl5JykgI1wiXHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJDYXRlZ29yeVwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJDdXN0b21lciBUeXBlXCIsXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkxlYWRTb3VyY2VEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJDaGFubmVsXCIsXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgIH1cclxuICAgICAgICBdLFxyXG4gICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgIHNjcm9sbGFibGU6IGZhbHNlXHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlckRlYWxNZXRyaWNzKCkge1xyXG4gICAgICAkKFwiI2NsaWVudERlYWxNZXRyaWNzR3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgIGF1dG9iaW5kOiBmYWxzZSxcclxuICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSB7IGFwcGxpY2F0aW9uaWQ6ICQoXCIjQXBwbGljYXRpb25JZFwiKS52YWwoKSB9O1xyXG4gICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6IFwiL1JlcG9ydGluZy9EZWFsTWV0cmljcy9DbGllbnREZWFsc1wiLFxyXG4gICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgcGFnZVNpemU6IDIwLFxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCI8YSBocmVmPVxcXCIvVXNlcnMvRGV0YWlsP3VzZXJpZD0jPVVzZXJJZCNcXFwiPiM9RW1haWwjPC9hPlwiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIlRvZGF5XCIsXHJcbiAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpjfVwiLFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIC8veyBmaWVsZDogXCJZZXN0ZXJkYXlSZXZlbnVlXCIsIHRpdGxlOiBcIlllc3RlcmRheVwiLCBmb3JtYXQ6IFwiezA6Y31cIiwgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH0sIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0gfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTGFzdDdSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgN1wiLFxyXG4gICAgICAgICAgICBmb3JtYXQ6IFwiezA6Y31cIixcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkN1cnJlbnRNb250aFJldmVudWVcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiQ3VycmVudCBNb250aFwiLFxyXG4gICAgICAgICAgICBmb3JtYXQ6IFwiezA6Y31cIixcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlNhbWVQZXJpb2RMYXN0TW9udGhSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIlNhbWUgUGVyaW9kIExhc3QgTW9udGhcIixcclxuICAgICAgICAgICAgZm9ybWF0OiBcInswOmN9XCIsXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9LFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJMYXN0TW9udGhSZXZlbnVlXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgTW9udGhcIixcclxuICAgICAgICAgICAgZm9ybWF0OiBcInswOmN9XCIsXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9LFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJQcmV2aW91c1RvTGFzdE1vbnRoUmV2ZW51ZVwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJQcmV2aW91cyBNb250aFwiLFxyXG4gICAgICAgICAgICBmb3JtYXQ6IFwiezA6Y31cIixcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlJvbGxpbmcxMk1vbnRoUmV2ZW51ZVwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJSb2xsaW5nIDEyIE1vbnRoc1wiLFxyXG4gICAgICAgICAgICBmb3JtYXQ6IFwiezA6Y31cIixcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgIHNvcnRhYmxlOiB7fSxcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfVxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF0sXHJcbiAgICAgICAgcGFnZWFibGU6IHRydWUsXHJcbiAgICAgICAgc29ydGFibGU6IHtcclxuICAgICAgICAgIG1vZGU6IFwic2luZ2xlXCIsXHJcbiAgICAgICAgICBhbGxvd1Vuc29ydDogZmFsc2VcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbmRlclByb2Nlc3NpbmdNZXRyaWNzKCkge1xyXG4gICAgICAgICQoXCIjY2xpZW50UHJvY2Vzc2luZ01ldHJpY3NHcmlkXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgIGF1dG9iaW5kOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbnN0IGRhdGEgPSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhcHBsaWNhdGlvbmlkOiAkKFwiI0FwcGxpY2F0aW9uSWRcIikudmFsKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzb3VyY2U6ICQoXCIjY2xpZW50UHJvY2Vzc2luZ01ldHJpY3NHcmlkVG9vbGJhclNvdXJjZVwiKS52YWwoKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBcIi9SZXBvcnRpbmcvVXNlclByb2Nlc3NpbmdNZXRyaWNzL092ZXJ2aWV3UmVwb3J0XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHBhZ2VTaXplOiAyMCxcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgLy90b29sYmFyOiB7IHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI2NsaWVudFByb2Nlc3NpbmdNZXRyaWNzR3JpZFRvb2xiYXJUZW1wbGF0ZVwiKS5odG1sKCkpIH0sXHJcbiAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJFbWFpbFwiLFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCI8YSBocmVmPVxcXCIvVXNlcnMvRGV0YWlsP3VzZXJpZD0jPVVzZXJJZCNcXFwiPiM9RW1haWwjPC9hPlwiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0gZGlzcGxheVByb2Nlc3NpbmdNZXRyaWMoVG9kYXlSZWNvcmRzLFRvZGF5TWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICAvL3sgZmllbGQ6IFwiWWVzdGVyZGF5XCIsIHRpdGxlOiBcIlllc3RlcmRheVwiLCB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhZZXN0ZXJkYXlSZWNvcmRzLFllc3RlcmRheU1hdGNoZXMpICNcIiksIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9IH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkxhc3Q3XCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgN1wiLFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhMYXN0N1JlY29yZHMsTGFzdDdNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiQ3VycmVudE1vbnRoXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0gZGlzcGxheVByb2Nlc3NpbmdNZXRyaWMoQ3VycmVudE1vbnRoUmVjb3JkcyxDdXJyZW50TW9udGhNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiU2FtZVBlcmlvZExhc3RNb250aFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJTYW1lIFBlcmlvZCBMYXN0IE1vbnRoXCIsXHJcbiAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcclxuICAgICAgICAgICAgICBcIiM9IGRpc3BsYXlQcm9jZXNzaW5nTWV0cmljKFNhbWVQZXJpb2RMYXN0TW9udGhSZWNvcmRzLFNhbWVQZXJpb2RMYXN0TW9udGhNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgTW9udGhcIixcclxuICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0gZGlzcGxheVByb2Nlc3NpbmdNZXRyaWMoTGFzdE1vbnRoUmVjb3JkcyxMYXN0TW9udGhNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiUHJldmlvdXNUb0xhc3RNb250aFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJQcmV2aW91cyBNb250aFwiLFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgXCIjPSBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhQcmV2aW91c1RvTGFzdE1vbnRoUmVjb3JkcyxQcmV2aW91c1RvTGFzdE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIlJvbGxpbmcxMk1vbnRoXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIlJvbGxpbmcgMTIgTW9udGhzXCIsXHJcbiAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcIiM9IGRpc3BsYXlQcm9jZXNzaW5nTWV0cmljKFJvbGxpbmcxMk1vbnRoUmVjb3JkcyxSb2xsaW5nMTJNb250aE1hdGNoZXMpICNcIiksXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXSxcclxuICAgICAgICBwYWdlYWJsZTogdHJ1ZSxcclxuICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICBzb3J0YWJsZToge1xyXG4gICAgICAgICAgbW9kZTogXCJzaW5nbGVcIixcclxuICAgICAgICAgIGFsbG93VW5zb3J0OiBmYWxzZVxyXG4gICAgICAgIH1cclxuICAgICAgfSk7XHJcblxyXG5cclxuICAgIH1cclxuXHJcbiAgICBzaG93Sm9icyhlbWFpbCkge1xyXG4gICAgICBoaXN0b3J5LnB1c2hTdGF0ZShudWxsLCBcIlJlcG9ydGluZ1wiLCBcIi9SZXBvcnRpbmdcIik7XHJcbiAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoYC9Kb2JQcm9jZXNzaW5nL1N1bW1hcnk/ZW1haWw9JHtlbWFpbH1gKTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGV4cG9ydCBjbGFzcyBBZG1pblVzZXJNZXRyaWNzTW9kZWwge1xyXG5cclxuICAgIHJlbmRlclRhYigpIHtcclxuICAgICAgY29uc29sZS5sb2coXCJBZG1pblVzZXJNZXRyaWNzTW9kZWwucmVuZGVyVGFiXCIpO1xyXG4gICAgICB0aGlzLnJlbmRlckFkbWluVXNlclN1bW1hcnkoKTtcclxuICAgIH1cclxuXHJcbiAgICByZW5kZXJBZG1pblVzZXJTdW1tYXJ5KCkge1xyXG4gICAgICAgICQoXCIjYWRtaW5Vc2VyQWN0aXZpdHlVc2VyU3VtbWFyeVwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICBhdXRvYmluZDogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb25zdCBkYXRhID0geyB1c2VyaWQ6IFwiNzRBMENDOUItREU3OC00MEUzLUE1NTYtMDczMkFBREY0QzQ2XCIgfTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVybDogXCIvUmVwb3J0aW5nL0FkbWluVXNlckFjdGl2aXR5TWV0cmljcy9Vc2VyU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAvL3BhZ2VTaXplOiAyMCxcclxuICAgICAgICAgICAgICAgIGdyb3VwOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIGRpcjogXCJkZXNjXCJcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzb3J0OiB7IGZpZWxkOiBcIkhvdXJcIiwgZGlyOiBcImFzY1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgLy9kYXRhQm91bmQoKSB7XHJcbiAgICAgICAgICAgIC8vICB2YXIgZ3JpZCA9ICQoXCIjYWRtaW5Vc2VyQWN0aXZpdHlVc2VyU3VtbWFyeVwiKS5kYXRhKCdrZW5kb0dyaWQnKTtcclxuICAgICAgICAgICAgLy8gIGdyaWQuY29sbGFwc2VSb3coJChcIiNhZG1pblVzZXJBY3Rpdml0eVVzZXJTdW1tYXJ5IHRib2R5ID4gdHJcIikpO1xyXG4gICAgICAgICAgICAvLyAgZ3JpZC5jb2xsYXBzZUdyb3VwKCQoXCIjYWRtaW5Vc2VyQWN0aXZpdHlVc2VyU3VtbWFyeSB0Ym9keSA+IHRyLmstZ3JvdXBpbmctcm93XCIpKTtcclxuICAgICAgICAgICAgLy8gIGdyaWQuZXhwYW5kUm93KCQoXCIjYWRtaW5Vc2VyQWN0aXZpdHlVc2VyU3VtbWFyeSB0Ym9keSA+IHRyOmZpcnN0XCIpKTtcclxuICAgICAgICAgICAgLy8gIC8vJCgndHJbcm9sZSo9XCJyb3dcIl0nKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIC8vfSxcclxuICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGZpZWxkOiBcIkRhdGVcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiRGF0ZVwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJIb3VyXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkhvdXJcIixcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIC8veyBmaWVsZDogXCJDYWxsZXJzXCIsIHRpdGxlOiBcIkNhbGxlcnNcIiwgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSwgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSwgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0ga2VuZG8udG9TdHJpbmcoQ2FsbGVycywgJ24wJykgPT0gMCA/ICctJyA6IGtlbmRvLnRvU3RyaW5nKENhbGxlcnMsICduMCcpICNcIikgfSxcclxuICAgICAgICAgIC8veyBmaWVsZDogXCJJbmJvdW5kXCIsIHRpdGxlOiBcIkluYm91bmRcIiwgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSwgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSwgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFwiIz0ga2VuZG8udG9TdHJpbmcoSW5ib3VuZCwgJ24wJykgPT0gMCA/ICctJyA6IGtlbmRvLnRvU3RyaW5nKEluYm91bmQsICduMCcpICNcIikgfSxcclxuICAgICAgICAgIC8veyBmaWVsZDogXCJPdXRib3VuZFwiLCB0aXRsZTogXCJPdXRib3VuZFwiLCBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LCBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LCB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBrZW5kby50b1N0cmluZyhPdXRib3VuZCwgJ24wJykgPT0gMCA/ICctJyA6IGtlbmRvLnRvU3RyaW5nKE91dGJvdW5kLCAnbjAnKSAjXCIpIH0sXHJcbiAgICAgICAgICAvL3sgZmllbGQ6IFwiVm9pY2VtYWlsXCIsIHRpdGxlOiBcIlZvaWNlbWFpbFwiLCBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LCBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LCB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBrZW5kby50b1N0cmluZyhWb2ljZW1haWwsICduMCcpID09IDAgPyAnLScgOiBrZW5kby50b1N0cmluZyhWb2ljZW1haWwsICduMCcpICNcIikgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTG9naW5FdmVudFwiLFxyXG4gICAgICAgICAgICB0aXRsZTogXCJMb2dvbiBFdmVudFwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgXCIjPSBrZW5kby50b1N0cmluZyhMb2dpbkV2ZW50LCAnbjAnKSA9PSAwID8gJy0nIDoga2VuZG8udG9TdHJpbmcoTG9naW5FdmVudCwgJ24wJykgI1wiKVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgZmllbGQ6IFwiTGVhZHNUb3VjaGVkXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkxlYWRzIFRvdWNoZWRcIixcclxuICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgIFwiIz0ga2VuZG8udG9TdHJpbmcoTGVhZHNUb3VjaGVkLCAnbjAnKSA9PSAwID8gJy0nIDoga2VuZG8udG9TdHJpbmcoTGVhZHNUb3VjaGVkLCAnbjAnKSAjXCIpXHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJDdXN0b21lcnNUb3VjaGVkXCIsXHJcbiAgICAgICAgICAgIHRpdGxlOiBcIkN1c3RvbWVycyBUb3VjaGVkXCIsXHJcbiAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcclxuICAgICAgICAgICAgICBcIiM9IGtlbmRvLnRvU3RyaW5nKEN1c3RvbWVyc1RvdWNoZWQsICduMCcpID09IDAgPyAnLScgOiBrZW5kby50b1N0cmluZyhDdXN0b21lcnNUb3VjaGVkLCAnbjAnKSAjXCIpXHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBmaWVsZDogXCJEZWFsc1RvdWNoZWRcIixcclxuICAgICAgICAgICAgdGl0bGU6IFwiRGVhbHMgVG91Y2hlZFwiLFxyXG4gICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgXCIjPSBrZW5kby50b1N0cmluZyhEZWFsc1RvdWNoZWQsICduMCcpID09IDAgPyAnLScgOiBrZW5kby50b1N0cmluZyhEZWFsc1RvdWNoZWQsICduMCcpICNcIilcclxuICAgICAgICAgIH1cclxuICAgICAgICBdLFxyXG4gICAgICAgIHBhZ2VhYmxlOiBmYWxzZSxcclxuICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZVxyXG4gICAgICB9KTtcclxuXHJcblxyXG4gICAgfVxyXG5cclxuICB9XHJcblxyXG59XHJcblxyXG5mdW5jdGlvbiBkaXNwbGF5UHJvY2Vzc2luZ01ldHJpYyhyZWNvcmRzOiBudW1iZXIsIG1hdGNoZXM6IG51bWJlcikge1xyXG4gIGlmIChyZWNvcmRzID09PSAwKSByZXR1cm4gXCItXCI7XHJcbiAgcmV0dXJuIGtlbmRvLnRvU3RyaW5nKHJlY29yZHMsIFwibjBcIikgK1xyXG4gICAgXCIgLyBcIiArXHJcbiAgICBrZW5kby50b1N0cmluZyhtYXRjaGVzLCBcIm4wXCIpICtcclxuICAgIFwiIChcIiArXHJcbiAgICBNYXRoLmZsb29yKChtYXRjaGVzIC8gcmVjb3JkcykgKiAxMDApICtcclxuICAgIFwiJSlcIjtcclxufVxyXG5cclxuZnVuY3Rpb24gZGlzcGxheUxlYWRNZXRyaWMocXVhbGlmaWVkTGVhZHM6IG51bWJlciwgbmV3Q2xpZW50czogbnVtYmVyLCByZXZlbnVlOiBudW1iZXIpIHtcclxuICBpZiAocXVhbGlmaWVkTGVhZHMgPT09IDApIHJldHVybiBcIi1cIjtcclxuICByZXR1cm4ga2VuZG8udG9TdHJpbmcocXVhbGlmaWVkTGVhZHMsIFwibjBcIikgK1xyXG4gICAgXCIgLyBcIiArXHJcbiAgICBrZW5kby50b1N0cmluZyhuZXdDbGllbnRzLCBcIm4wXCIpICtcclxuICAgIFwiIChcIiArXHJcbiAgICBNYXRoLmZsb29yKChuZXdDbGllbnRzIC8gcXVhbGlmaWVkTGVhZHMpICogMTAwKSArXHJcbiAgICBcIiUpIC0gXCIgK1xyXG4gICAga2VuZG8udG9TdHJpbmcocmV2ZW51ZSwgXCJjMFwiKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZGlzcGxheUxlYWRNZXRyaWNTaG9ydChxdWFsaWZpZWRMZWFkczogbnVtYmVyLCBuZXdDbGllbnRzOiBudW1iZXIpIHtcclxuICBpZiAocXVhbGlmaWVkTGVhZHMgPT09IDApIHJldHVybiBcIi1cIjtcclxuICByZXR1cm4ga2VuZG8udG9TdHJpbmcocXVhbGlmaWVkTGVhZHMsIFwibjBcIikgKyBcIiZuYnNwOy8mbmJzcDtcIiArIGtlbmRvLnRvU3RyaW5nKG5ld0NsaWVudHMsIFwibjBcIik7XHJcbn1cclxuXHJcbnZhciBmb3JtYXR0ZXIgPSBuZXcgSW50bC5OdW1iZXJGb3JtYXQoXCJlbi1VU1wiLFxyXG4gIHtcclxuICAgIHN0eWxlOiBcImN1cnJlbmN5XCIsXHJcbiAgICBjdXJyZW5jeTogXCJVU0RcIixcclxuICAgIG1pbmltdW1GcmFjdGlvbkRpZ2l0czogMlxyXG4gIH0pOyJdfQ==