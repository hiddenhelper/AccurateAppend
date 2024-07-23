/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />

var id: any;
var start: any;
var end: any;
var dataUrl: any;
var usageDateRangeWidget: any;

$(() => {

  var today = moment().add(1, "days");
  var monthAgo = moment(today).add(-14, "days");

  start = $("#start").kendoDatePicker({
    change: startChange,
    value: monthAgo.toDate()
  }).data("kendoDatePicker");

  end = $("#end").kendoDatePicker({
    change: endChange,
    value: today.toDate(),
  }).data("kendoDatePicker");

  renderOrderGrid(dataUrl);

  window.setInterval(() => {
      renderOrderGrid(dataUrl);
    },
    60000);
  // removes extra margin appearing at bottom of grid
  $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });

  $("#DownloadUsage").click(() => { $("#downloadUsageModal").modal("show"); });

  usageDateRangeWidget = new AccurateAppend.DatePicker.DateRangeWidget("usageDateRangeWidget",
    new AccurateAppend.DatePicker.DateRangeWidgetSettings(
      [
        AccurateAppend.DatePicker.DateRangeValue.Last24Hours,
        AccurateAppend.DatePicker.DateRangeValue.Last7Days,
        AccurateAppend.DatePicker.DateRangeValue.Last30Days,
        AccurateAppend.DatePicker.DateRangeValue.Last60Days,
        AccurateAppend.DatePicker.DateRangeValue.LastMonth,
        AccurateAppend.DatePicker.DateRangeValue.Custom
      ],
      AccurateAppend.DatePicker.DateRangeValue.LastMonth,
            []));
});

class View {
  queryUrl: string;
}

const OrderTypeBatch = "Batch";
const OrderTypePush = "Push";
const OrderTypeClient = "Client";

function startChange() {
  let startDate = start.value();
  let endDate = end.value();
  if (startDate) {
    startDate = new Date(startDate);
    startDate.setDate(startDate.getDate());
    end.min(startDate);
  } else if (endDate) {
    start.max(new Date(endDate));
  } else {
    endDate = new Date();
    start.max(endDate);
    end.min(endDate);
  }
  renderOrderGrid(dataUrl);
}

function endChange() {
  let endDate = end.value();
  const startDate = start.value();
  if (endDate) {
    endDate = new Date(endDate);
    endDate.setDate(endDate.getDate());
    start.max(endDate);
  } else if (startDate) {
    end.min(new Date(startDate));
  } else {
    endDate = new Date();
    start.max(endDate);
    end.min(endDate);
  }
  renderOrderGrid(dataUrl);
}

function renderOrderGrid(url) {

  const grid = $("#grid").data("kendoGrid");
  if (grid !== undefined && grid !== null) {
    grid.dataSource.read();
  } else {
    $("#grid").kendoGrid({
      dataSource: {
        type: "json",
        transport: {
          read(options) {
            $.ajax({
              url: url,
              dataType: "json",
              type: "GET",
              data: {
                startDate: moment($("#start").val()).utc().format("YYYY-MM-DD"),
                endDate: moment($("#end").val()).add(1, "days").utc().format("YYYY-MM-DD")
              },
              success(result) {
                options.success(result);
              }
            });
          },
          cache: false
        },
        pageSize: 20,
        schema: {
          type: "json",
          data: "Data",
          total(response) {
            return response.Data.length;
          }
        },
        change: function() {
          if (this.data().length <= 0) {
            $("#no-orders-message").show();
            $("#grid").hide();
            $("#dataSecurityDisclaimer").hide();
            $("#pager").hide();
          } else {
            $("#no-orders-message").hide();
            $("#grid").show();
            $("#dataSecurityDisclaimer").show();
            $("#pager").show();
            $("#warning").hide();
          }
        }
      },
      scrollable: false,
      filterable: false,
      pageable: {
          pageSize: 10,
          pageSizes: false,
      },
      columns: [
        {
          field: "Name",
          title: "List Name/Source",
          attributes: { style: "text-align: center;" },
              template: kendo.template($("#grid-description-column-template").html()),
          media: "(min-width: 450px)"
        },
        {
          field: "DateSubmitted",
          title: "Order Date",
          attributes: { style: "text-align: center;" },
          width: 200,
          template: "#= kendo.toString(kendo.parseDate(DateSubmitted, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
          media: "(min-width: 450px)"
        },
        {
          field: "SourceDescription",
          title: "Submitted Through",
          attributes: { style: "text-align: center;" },
          media: "(min-width: 450px)"
        },
        //{
        //  field: "TotalRecords",
        //  title: "Record Count",
        //  attributes: { style: "text-align: center;" },
        //  format: "{0:n0}",
        //  media: "(min-width: 450px)"
        //},
        {
          field: "StatusDescription",
          title: "Status",
          width: 200,
          attributes: { style: "text-align: center; white-space: nowrap" },
          template: kendo.template($("#grid-status-column-template").html()),
          media: "(min-width: 450px)"
        },
        {
          title: "Summary",
          template: kendo.template($("#responsive-column-template").html()),
          media: "(max-width: 450px)"
        }
      ]
    });
  }
}

function displayDownloadConfirmation(downloadUrl, customerFileName, slug, status) {
  switch (status) {
  case "Uploading":
    if (slug)
      $("#downloadConfirmation .modal-body p").text(
        `The results for ${customerFileName} are currently being uploaded to ${slug
        }. This file is for reference only and should not be imported into your nation.`);
    break;
  case "Complete":
    $("#downloadConfirmation .modal-body p").text(`The results for ${customerFileName} have been uploaded to ${slug
      }. This file is for reference only and should not be imported into your nation.`);
    break;
  }
  this.id = downloadUrl;
  $("#downloadConfirmation").modal("show");
}

function downloadPushOutputFile() {
  $("#downloadConfirmation").modal("hide");
  history.pushState(null, "Orders", "/Order/Current");
  window.location.replace(`${id}`);
}

function downloadFile(url) {
  history.pushState(null, "Orders", "/Order/Current");
  window.location.replace(url);
}

function downloadUsage() {
  $("#downloadUsageModal").modal("hide");
  window.location.replace(
    `/Order/DownloadUsage?start=${moment($("#usageDateRangeWidget_startDate").val()).format("YYYY-MM-DD")
    }&end=${moment($("#usageDateRangeWidget_endDate").val()).format("YYYY-MM-DD")}`);
}

function displayMessage(type, message) {
  const alert = $("#alert");
  switch (type) {
  case "success":
    alert.addClass("alert").addClass("alert-success");
    alert.text(message);
    alert.show();
    break;
  case "info":
    alert.addClass("alert").addClass("alert-info");
    alert.text(message);
    alert.show();
    break;
  case "warning":
    alert.addClass("alert").addClass("alert-warning");
    alert.text(message);
    alert.show();
    break;
  case "danger":
    alert.addClass("alert").addClass("alert-danger");
    alert.text(message);
    alert.show();
    break;
  default:
    alert.removeClass();
    alert.hide();
    break;
  }
}