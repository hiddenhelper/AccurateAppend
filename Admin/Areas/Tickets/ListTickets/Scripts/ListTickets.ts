/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="../../../../scripts/accurateappend.ui.ts" />

var ticketsApiViewModel: AccurateAppend.Websites.Admin.Operations.TicketsApi.TicketsApiViewModel;
var TicketsApiDateRangeWidget: any;
declare let queryUrl: string;

$(() => {

  ticketsApiViewModel = new AccurateAppend.Websites.Admin.Operations.TicketsApi.TicketsApiViewModel();

  TicketsApiDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("ticketsDateRange",
    new AccurateAppend.Ui.DateRangeWidgetSettings(
      [
        AccurateAppend.Ui.DateRangeValue.Last24Hours,
        AccurateAppend.Ui.DateRangeValue.Last7Days,
        AccurateAppend.Ui.DateRangeValue.Last30Days,
        AccurateAppend.Ui.DateRangeValue.Last60Days,
        AccurateAppend.Ui.DateRangeValue.LastMonth,
        AccurateAppend.Ui.DateRangeValue.Custom
      ],
      AccurateAppend.Ui.DateRangeValue.Last7Days,
      [
        ticketsApiViewModel.renderGrid
      ]));

  window.setInterval("ticketsApiViewModel.renderGrid()", 30000);

  ticketsApiViewModel.renderGrid();

});

module AccurateAppend.Websites.Admin.Operations.TicketsApi {

  export class TicketsApiViewModel {

    renderGrid() {

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
                  url: queryUrl,
                  dataType: "json",
                  type: "GET",
                  data: {
                    start: moment(TicketsApiDateRangeWidget.getStartDate()).utc().format("YYYY-MM-DD"),
                    end: moment(TicketsApiDateRangeWidget.getEndDate()).add(1, "days").utc().format("YYYY-MM-DD")
                  },
                  success(result) {
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
              total(response) {
                return response.Data.length;
              }
            },
            change: function() {
              if (this.data().length <= 0) {
                $("#gridMessage").show();
                $("#grid").hide();
                $("#pager").hide();
              } else {
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
          dataBound(e) {
            const grid = $("#grid").data("kendoGrid");
            const dataView = grid.dataSource.view();

            for (let i = 0; i < dataView.length; i++) {
              for (let j = 0; j < dataView[i].items.length; j++) {
                if (dataView[i].items[j].status === "Closed") {
                  const uid = dataView[i].items[j].uid;
                  grid.collapseGroup($("#grid").find(`tr[data-uid=${uid}]`).prev("tr.k-grouping-row"));
                }
              }
            }
          },
          columns: [
            {
              field: "CreatedAt",
              title: "Date Created",
              attributes: { style: "text-align: center;" },
              //template: "#= Name # " + '# if(Type === "' + OrderTypePush + '") { #' + " - #:Slug# " + "# } # ",
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
              //template: "#= kendo.toString(kendo.parseDate(DateSubmitted, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
              media: "(min-width: 450px)"
            },
            {
              field: "Status",
              title: "Status",
              attributes: { style: "text-align: center;" },
              width: 200,
              //template: "#= kendo.toString(kendo.parseDate(DateSubmitted, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
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
              template:
                "<a href='\\#' class=\"btn btn-default\" style=\"margin-right: 5px;\" onclick=\"ticketsApiViewModel.viewDetail('#= uid #')\">View Detail</a><a href=\"#= ZendeskDetail #\" class=\"btn btn-default\">View In Zendesk</a>",
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
    }

    viewDetail(uid) {
      const grid = $("#grid").data("kendoGrid");
      const row = grid.tbody.find(`tr[data-uid='${uid}']`);
      const item = grid.dataItem(row);
      $("#detailsModal .modal-header").html(`Details for Ticket ${item["Id"]}`);
      $("#detailsModal .modal-body pre").html(item["Description"]);
      $("#detailsModal").appendTo("body").modal("show");
    }

  }

}