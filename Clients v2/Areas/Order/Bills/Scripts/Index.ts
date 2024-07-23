/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />

var dataUrl: any;
var viewModel: any;

$(() => {

  viewModel = new ViewModel();
  console.log(dataUrl);
  viewModel.renderGrid(dataUrl);

  //window.setInterval(() => {
  //    renderOrderGrid(dataUrl);
  //  },
  //  60000);
  // removes extra margin appearing at bottom of grid
    $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
 
});

class ViewModel {
  url: string;

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
                url: dataUrl,
                dataType: "json",
                type: "GET",
                data: {},
                success(result) {
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
            total(response) {
              return response.Data.length;
            }
          },
          change: function() {
            if (this.data().length <= 0) {
              $("#message").show();
              $("#grid").hide();
              $("#pager").hide();
            } else {
              $("#message").hide();
              $("#grid").show();
              $("#pager").show();
              $("#warning").hide();
            }
          }
          },
        dataBound: () => {
          // need to prevent jcf select plugin from applying itself to the Kendo grid's pager element
          jcf.destroyAll();
        },
        sortable: {
          mode: "multiple",
          allowUnsort: true,
          showIndexes: true
        },
        scrollable: false,
        filterable: false,
        pageable: true,
        columns: [
          {
            field: "CompletedDate",
            title: "Date",
            attributes: { style: "text-align: center;" },
            width: 200,
            template: "#= kendo.toString(kendo.parseDate(CompletedDate, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
            media: "(min-width: 450px)"
            },
          {
            field: "Id",
            title: "Id",
            attributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "Title",
            title: "Description",
            attributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "Amount",
            title: "Amount",
            attributes: { style: "text-align: center;" },
            format: "{0:c0}",
            media: "(min-width: 450px)"
          },
          {
            field: "",
            title: "",
            width: 200,
            attributes: { style: "text-align: center; white-space: nowrap" },
            template: kendo.template($("#grid-download-column-template").html()),
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

  downloadFile(url) {
    history.pushState(null, "Orders", "/Order/Bills");
    window.location.replace(url);
  }
}