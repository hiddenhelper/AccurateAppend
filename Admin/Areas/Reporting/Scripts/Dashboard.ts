/// <reference path="../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../scripts/typings/jquery.cookie/jquery.cookie.d.ts" />
/// <reference path="../../../scripts/typings/moment/moment.d.ts" />

var viewModel: AccurateAppend.Reporting.Dashboard.ViewModel;
var operatingMetricsViewModel: AccurateAppend.Reporting.Dashboard.OperatingMetricsViewModel;
var processingMetricsViewModel: AccurateAppend.Reporting.Dashboard.ProcessingMetricsViewModel;
var webserviceMetricsViewModel: AccurateAppend.Reporting.Dashboard.WebServicesMetricsModel;
var webserviceMetricsViewModel: AccurateAppend.Reporting.Dashboard.WebServicesMetricsModel;
var clientMetricsViewModel: AccurateAppend.Reporting.Dashboard.ClientMetricsModel;
var adminUserMetricsModel: AccurateAppend.Reporting.Dashboard.AdminUserMetricsModel;

$(() => {

  viewModel = new AccurateAppend.Reporting.Dashboard.ViewModel();
  operatingMetricsViewModel = new AccurateAppend.Reporting.Dashboard.OperatingMetricsViewModel();
  processingMetricsViewModel = new AccurateAppend.Reporting.Dashboard.ProcessingMetricsViewModel();
  webserviceMetricsViewModel = new AccurateAppend.Reporting.Dashboard.WebServicesMetricsModel();
  clientMetricsViewModel = new AccurateAppend.Reporting.Dashboard.ClientMetricsModel;
  adminUserMetricsModel = new AccurateAppend.Reporting.Dashboard.AdminUserMetricsModel;

  $("#ApplicationId").bind("change",
    () => {
      viewModel.setApplicationId();
      renderTabs();
    });

  // resize charts window is resized
  $(window).resize(() => {
    kendo.resize($("div.k-chart[data-role='chart']"), true);
  });

  // resize charts when tab is switched
  $('a[data-toggle="tab"]').on("shown.bs.tab",
    e => {
      kendo.resize($("div.k-chart[data-role='chart']"), true);
    });

  renderTabs();

  $("#Source").bind("change",
    () => {
      processingMetricsViewModel.renderSubscriberProcessingHistory();
    });

  $("#clientProcessingMetricsGridToolbarSource").bind("change",
    () => {
      console.log("firing change");
      clientMetricsViewModel.renderProcessingMetrics();
    });

  $("#adminUserActivityUserSummaryGridToolbarSource").bind("change",
    () => {
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

module AccurateAppend.Reporting.Dashboard {

  export class ViewModel {

    loadApplicationId() {
      const v = $.cookie("ApplicationId");
      if (v !== "") {
        $(`#ApplicationId option[value=${$.cookie("ApplicationId")}]`).attr("selected", "selected");
      }
    }

    setApplicationId() {
      $.cookie("ApplicationId", $("#ApplicationId option:selected").val());
    }
  }

  export class OperatingMetricsViewModel {

    renderTab() {
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
    }

    renderDealMetricOverviewReportGrid() {
      const grid = $("#DealMetricOverviewReportGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#DealMetricOverviewReportGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                const data = { applicationid: $("#ApplicationId").val() };
                $.ajax({
                  url: "/Reporting/OperatingMetrics/OverviewReport",
                  dataType: "json",
                  type: "GET",
                  data: data,
                  success(result) {
                    options.success(result);
                  }
                });
              }
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
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
    }

    renderMrrMetricOverviewGrid() {
        if ($("#MrrMetricOverviewGrid").length === 0) return;

      $.ajax({
        url: "/Reporting/MrrMetrics/Query",
        dataType: "json",
        type: "GET",
        data: { applicationId: $("#ApplicationId").val() },
        success(data) {
          // write table
          const container = $("#MrrMetricOverviewGrid").empty();;
          var table = $(`<table>`);
          $.each(data,
            (key, value) => {
              if (key === 0) {
                var th = $(`<thead class="k-grid-header"><tr>`);
                $.each(Object.keys(value),
                  (i, propertyName) => {
                    th.append(`<th class="k-header">${propertyName}</th>`);
                  });
                table.append(th).append(`</thead>`);
              }
              var tr = $(`<tr ${key & 1 ? 'class="k-alt"' : ""} >`);
              $.each(value,
                (i, propertyValue) => {
                  tr.append(`<td style="text-align: right;">${propertyValue}</td>`);
                });
              table.append(tr);
            });
          container.append(table);
        }
      });
      }

    renderAgentMetricOverviewGrid() {

      $.ajax({
        url: "/Reporting/AgentMetrics/Query",
        dataType: "json",
        type: "GET",
        data: { applicationId: $("#ApplicationId").val() },
        success(data) {
          // write table
          const container = $("#AgentMetricOverviewGrid").empty();;
          var table = $(`<table>`);
          $.each(data,
            (key, value) => {
              if (key === 0) {
                var th = $(`<thead class="k-grid-header"><tr>`);
                $.each(Object.keys(value),
                  (i, propertyName) => {
                    th.append(`<th class="k-header">${propertyName}</th>`);
                  });
                table.append(th).append(`</thead>`);
              }
              var tr = $(`<tr ${key & 1 ? 'class="k-alt"' : ""} >`);
              $.each(value,
                (i, propertyValue) => {
                  tr.append(`<td style="text-align: right;">${propertyValue}</td>`);
                });
              table.append(tr);
            });
          container.append(table);
        }
      });
    }

    renderLeadChannelMetricOverviewGrid(leadSource, selector) {

      $.ajax({
        url: "/Reporting/LeadChannelMetrics/Query",
        dataType: "json",
        type: "GET",
        data: { applicationId: $("#ApplicationId").val(), leadSource: leadSource },
        success(data) {
          // write table
          const container = $(selector).empty();;
          var table = $(`<table>`);
          $.each(data,
            (key, value) => {
              // determine format for the row, 3 is revenue and needs to be formatted as currency, otherwise number
              var format: string;
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
              // if first row then create header using key name
              if (key === 0) {
                var th = $(`<thead class="k-grid-header"><tr>`);
                $.each(Object.keys(value),
                  (i, propertyName) => {
                    if (propertyName !== "MetricName") {
                        th.append(`<th class="k-header" style="text-align: right;">${propertyName}</th>`);
                    }
                  });
                table.append(th).append(`</thead>`);
              }
              var tr = $(`<tr ${key & 1 ? 'class="k-alt"' : ""} >`);
              $.each(value,
                (name, value) => {
                  if (name !== "MetricName") {
                    tr.append(`<td style="text-align: right;">${kendo.toString(value, format)}</td>`);
                  }
                });
              table.append(tr);
            });
          container.append(table);
        }
      });

    }

      renderRevenueMetricChart() {
          if ($("#RevenueMetricChart").length === 0) return;

      $("#RevenueMetricChart").kendoChart({
        title: {
          text: "Revenue By Month"
        },
        dataSource: {
          transport: {
            read(options) {
              $.ajax({
                url: "/Reporting/OperatingMetrics/Revenue",
                dataType: "json",
                type: "GET",
                data: { applicationid: $("#ApplicationId").val() },
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: { data: "Data" },
          requestStart() {
            kendo.ui.progress($("#RevenueMetricChartLoading"), true);
          },
          requestEnd() {
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
    }

    renderLeadMetricOverviewReportGrid() {
      const grid = $("#LeadMetricOverviewReportGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#LeadMetricOverviewReportGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                const data = { applicationid: $("#ApplicationId").val() };
                $.ajax({
                  url: "/Reporting/LeadMetrics/OverviewReport",
                  dataType: "json",
                  type: "GET",
                  data: data,
                  success(result) {
                    options.success(result);
                  }
                });
              }
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
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
              template: kendo.template(
                "#= displayLeadMetricShort(Last7RecordsLeadCount, Last7RecordsNewClientCount) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "CurrentMonth",
              title: "Current Month",
              template: kendo.template(
                "#= displayLeadMetric(CurrentMonthLeadCount, CurrentMonthNewClientCount, CurrentMonthRevenueAmount) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "SamePeriodLastMonth",
              title: "Same Period Last Month",
              template: kendo.template(
                "#= displayLeadMetric(SamePeriodLastMonthLeadCount, SamePeriodLastMonthNewClientCount, SamePeriodLastMonthRevenueAmount) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "LastMonth",
              title: "Last Month",
              template: kendo.template(
                "#= displayLeadMetric(LastMonthLeadCount, LastMonthNewClientCount, LastMonthRevenueAmount) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "PreviousToLastMonth",
              title: "Previous Month",
              template: kendo.template(
                "#= displayLeadMetric(PreviousToLastLeadCount, PreviousToLastNewClientCount, PreviousToLastRevenueAmount) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "Rolling12Months",
              title: "Rolling 12 Months",
              template: kendo.template(
                "#= displayLeadMetric(Rolling12MonthsLeadCount, Rolling12MonthsNewClientCount, Rolling12MonthsRevenueAmount) #"),
              attributes: { style: "text-align:right;" },
              width: 200
            },
            { field: "TimeToFirstPurchase", title: "Days To $$$", attributes: { style: "text-align:right;" } }
          ]
        });
      }
    }

    renderLeadMetricChart() {
      $("#leadSumamryChart").kendoChart({
        title: {
          text: "Lead Activity By Week"
        },
        dataSource: {
          transport: {
            read(options) {
              $.ajax({
                url: "/Reporting/LeadReports/LeadMetricSummary",
                dataType: "json",
                type: "GET",
                data: { applicationid: $("#ApplicationId").val() },
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: { data: "Data" },
          requestStart() {
            kendo.ui.progress($("#LeadSummaryChartLoading"), true);
          },
          requestEnd() {
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
    }
  }

  export class ProcessingMetricsViewModel {

    renderTab() {
      console.log("ProcessingMetricsViewModel.renderTab");
      this.renderProcessingMetricOverviewReportGrid();
      this.renderSubscriberProcessingHistory();
      this.renderSubscriberActivityMonthComparison();
      this.renderJobQueueActivityLast24Hours();
    }

    renderProcessingMetricOverviewReportGrid() {
      const grid = $("#ProcessingMetricOverviewReportGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#ProcessingMetricOverviewReportGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                const data = { applicationid: $("#ApplicationId").val() };
                $.ajax({
                  url: "/Reporting/OperationMetrics/OverviewReport",
                  dataType: "json",
                  type: "GET",
                  data: data,
                  success(result) {
                    options.success(result);
                  }
                });
              }
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
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
              template: kendo.template(
                "#= displayProcessingMetric(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
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
              template: kendo.template(
                "#= displayProcessingMetric(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
              attributes: { style: "text-align:right;" }
            }
          ]
        });
      }
    }

    renderSubscriberProcessingHistory() {
      $("#subscriberProcessingHistory").kendoChart({
        dataSource: {
          transport: {
            read(options) {
              $.ajax({
                url:
                  `/Reporting/JobMetrics/SubscriberJobMatchCounts?aggregate=0&source=${$("#Source").val()}&startDate=${
                    moment().subtract("month", 8).format("L")}&endDate=${moment().format("L")}&applicationid=${$(
                      "#ApplicationId").val()}`,
                dataType: "json",
                type: "GET",
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: { data: "Data" },
          requestStart() {
            kendo.ui.progress($("#subscriberProcessingHistoryLoading"), true);
          },
          requestEnd() {
            kendo.ui.progress($("#subscriberProcessingHistoryLoading"), false);
          }
        },
        series:
        [
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
    }

    renderSubscriberActivityMonthComparison() {
      $("#subscriberActivityMonthComparison").kendoChart({
        dataSource: {
          transport: {
            read(options) {
              $.ajax({
                url:
                  `/Reporting/JobMetrics/SubscriberJobMatchCountsComparison?compare=0&applicationid=${$(
                    "#ApplicationId").val()}`,
                dataType: "json",
                type: "GET",
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: { data: "Data" },
          requestStart() {
            kendo.ui.progress($("#subscriberActivityMonthComparisonChartLoading"), true);
          },
          requestEnd() {
            kendo.ui.progress($("#subscriberActivityMonthComparisonChartLoading"), false);
          }
        },
        series:
        [
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
    }

    renderJobQueueActivityLast24Hours() {
      $("#jobQueueActivityLast24Hours").kendoChart({
        dataSource: {
          transport: {
            read(options) {
              $.ajax({
                url: `/Reporting/JobMetrics/GetGraphForLast24Hours?applicationid=${$("#ApplicationId").val()}`,
                dataType: "json",
                type: "GET",
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: { data: "Data" },
          requestStart() {
            kendo.ui.progress($("#jobQueueActivityLast24HoursLoading"), true);
          },
          requestEnd() {
            kendo.ui.progress($("#jobQueueActivityLast24HoursLoading"), false);
          }
        },
        series:
        [
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
            //stack: true,
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
            //stack: true,
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
            //stack: true,
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
            //stack: true,
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
            //stack: true,
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
            //stack: true,
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
          //title: "Hour of Day"
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
    }
  }

  export class WebServicesMetricsModel {

    renderTab() {
      console.log("WebServicesMetricsModel.renderTab");
      this.renderTransactionsByUserGrid();
    }

    renderTransactionsByUserGrid() {
      $("#transactionsByUser").kendoGrid({
        dataSource: {
          autobind: false,
          type: "json",
          transport: {
            read(options) {
              $.ajax({
                url: "/Reporting/ApiMetrics/GetCallsByUserJson",
                dataType: "json",
                type: "GET",
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: {
            type: "json",
            data: "Data",
            total(response) {
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
          //{ command: { text: "View Details", click: viewDetails }, title: " ", width: "140px" }
        ]
      });
    }
  }

  export class ClientMetricsModel {

    renderTab() {
      console.log("ClientMetricsModel.renderTab");
      this.renderRecentDeals();
      this.renderDealMetrics();
      this.renderProcessingMetrics();
    }

    renderRecentDeals() {
      $("#recentDealsGrid").kendoGrid({
        dataSource: {
          autobind: false,
          type: "json",
          transport: {
            read(options) {
              const data = { applicationid: $("#ApplicationId").val() };
              $.ajax({
                url: "/Reporting/DealMetrics/RecentDeals",
                dataType: "json",
                type: "GET",
                data: data,
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: {
            type: "json",
            data: "Data",
            total(response) {
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
    }

    renderDealMetrics() {
      $("#clientDealMetricsGrid").kendoGrid({
        dataSource: {
          autobind: false,
          type: "json",
          transport: {
            read(options) {
              const data = { applicationid: $("#ApplicationId").val() };
              $.ajax({
                url: "/Reporting/DealMetrics/ClientDeals",
                dataType: "json",
                type: "GET",
                data: data,
                success(result) {
                  options.success(result);
                }
              });
            }
          },
          schema: {
            type: "json",
            data: "Data",
            total(response) {
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
          //{ field: "YesterdayRevenue", title: "Yesterday", format: "{0:c}", attributes: { style: "text-align:right;" }, headerAttributes: { style: "text-align: center;" } },
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
    }

    renderProcessingMetrics() {
        $("#clientProcessingMetricsGrid").kendoGrid({
            dataSource: {
                autobind: false,
                type: "json",
                transport: {
                    read(options) {
                        const data = {
                            applicationid: $("#ApplicationId").val(),
                            source: $("#clientProcessingMetricsGridToolbarSource").val()
                        };
                        $.ajax({
                            url: "/Reporting/UserProcessingMetrics/OverviewReport",
                            dataType: "json",
                            type: "GET",
                            data: data,
                            success(result) {
                                options.success(result);
                            }
                        });
                    }
                },
                schema: {
                    type: "json",
                    data: "Data",
                    total(response) {
                        return response.Data.length;
                    }
                },
                pageSize: 20,
            },
            //toolbar: { template: kendo.template($("#clientProcessingMetricsGridToolbarTemplate").html()) },
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
          //{ field: "Yesterday", title: "Yesterday", template: kendo.template("#= displayProcessingMetric(YesterdayRecords,YesterdayMatches) #"), attributes: { style: "text-align:right;" } },
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
            template: kendo.template(
              "#= displayProcessingMetric(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
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
            template: kendo.template(
              "#= displayProcessingMetric(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
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


    }

    showJobs(email) {
      history.pushState(null, "Reporting", "/Reporting");
        window.location.replace(`/JobProcessing/Summary?email=${email}`);
    }
  }

  export class AdminUserMetricsModel {

    renderTab() {
      console.log("AdminUserMetricsModel.renderTab");
      this.renderAdminUserSummary();
    }

    renderAdminUserSummary() {
        $("#adminUserActivityUserSummary").kendoGrid({
            dataSource: {
                autobind: false,
                type: "json",
                transport: {
                    read(options) {
                        const data = { userid: "74A0CC9B-DE78-40E3-A556-0732AADF4C46" };
                        $.ajax({
                            url: "/Reporting/AdminUserActivityMetrics/UserSummary",
                            dataType: "json",
                            type: "GET",
                            data: data,
                            success(result) {
                                options.success(result);
                            }
                        });
                    }
                },
                schema: {
                    type: "json",
                    data: "Data",
                    total(response) {
                        return response.Data.length;
                    }
                },
                //pageSize: 20,
                group: {
                    field: "Date",
                    dir: "desc"
                },
                sort: { field: "Hour", dir: "asc" }
            },
            //dataBound() {
            //  var grid = $("#adminUserActivityUserSummary").data('kendoGrid');
            //  grid.collapseRow($("#adminUserActivityUserSummary tbody > tr"));
            //  grid.collapseGroup($("#adminUserActivityUserSummary tbody > tr.k-grouping-row"));
            //  grid.expandRow($("#adminUserActivityUserSummary tbody > tr:first"));
            //  //$('tr[role*="row"]').hide();
            //},
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
          //{ field: "Callers", title: "Callers", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("#= kendo.toString(Callers, 'n0') == 0 ? '-' : kendo.toString(Callers, 'n0') #") },
          //{ field: "Inbound", title: "Inbound", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("#= kendo.toString(Inbound, 'n0') == 0 ? '-' : kendo.toString(Inbound, 'n0') #") },
          //{ field: "Outbound", title: "Outbound", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("#= kendo.toString(Outbound, 'n0') == 0 ? '-' : kendo.toString(Outbound, 'n0') #") },
          //{ field: "Voicemail", title: "Voicemail", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("#= kendo.toString(Voicemail, 'n0') == 0 ? '-' : kendo.toString(Voicemail, 'n0') #") },
          {
            field: "LoginEvent",
            title: "Logon Event",
            headerAttributes: { style: "text-align: center;" },
            attributes: { style: "text-align: center;" },
            template: kendo.template(
              "#= kendo.toString(LoginEvent, 'n0') == 0 ? '-' : kendo.toString(LoginEvent, 'n0') #")
          },
          {
            field: "LeadsTouched",
            title: "Leads Touched",
            headerAttributes: { style: "text-align: center;" },
            attributes: { style: "text-align: center;" },
            template: kendo.template(
              "#= kendo.toString(LeadsTouched, 'n0') == 0 ? '-' : kendo.toString(LeadsTouched, 'n0') #")
          },
          {
            field: "CustomersTouched",
            title: "Customers Touched",
            headerAttributes: { style: "text-align: center;" },
            attributes: { style: "text-align: center;" },
            template: kendo.template(
              "#= kendo.toString(CustomersTouched, 'n0') == 0 ? '-' : kendo.toString(CustomersTouched, 'n0') #")
          },
          {
            field: "DealsTouched",
            title: "Deals Touched",
            headerAttributes: { style: "text-align: center;" },
            attributes: { style: "text-align: center;" },
            template: kendo.template(
              "#= kendo.toString(DealsTouched, 'n0') == 0 ? '-' : kendo.toString(DealsTouched, 'n0') #")
          }
        ],
        pageable: false,
        scrollable: false
      });


    }

  }

}

function displayProcessingMetric(records: number, matches: number) {
  if (records === 0) return "-";
  return kendo.toString(records, "n0") +
    " / " +
    kendo.toString(matches, "n0") +
    " (" +
    Math.floor((matches / records) * 100) +
    "%)";
}

function displayLeadMetric(qualifiedLeads: number, newClients: number, revenue: number) {
  if (qualifiedLeads === 0) return "-";
  return kendo.toString(qualifiedLeads, "n0") +
    " / " +
    kendo.toString(newClients, "n0") +
    " (" +
    Math.floor((newClients / qualifiedLeads) * 100) +
    "%) - " +
    kendo.toString(revenue, "c0");
}

function displayLeadMetricShort(qualifiedLeads: number, newClients: number) {
  if (qualifiedLeads === 0) return "-";
  return kendo.toString(qualifiedLeads, "n0") + "&nbsp;/&nbsp;" + kendo.toString(newClients, "n0");
}

var formatter = new Intl.NumberFormat("en-US",
  {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 2
  });