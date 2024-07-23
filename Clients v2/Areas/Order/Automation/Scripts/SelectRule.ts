/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />

var queryUrl: any;
var selectUrl: any;
var nextUrl: any;
var viewModel: any;

$(() => {

  viewModel = new SelectRule.ViewModel();
  viewModel.renderGrid(queryUrl);

  // removes extra margin appearing at bottom of grid
  $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
  $(".k-grid .k-grid-header").hide();
});

namespace SelectRule {
  export class ViewModel {
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
                  url: queryUrl,
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
          scrollable: false,
          filterable: false,
          pageable: true,
          columns: [
            {
              title: "Description",
              template: kendo.template($("#grid-description-column-template").html()),
              media: "(min-width: 450px)"
            },
            //{
            //  field: "LastUsed",
            //  title: "Last Used",
            //  media: "(min-width: 450px)",
            //  attributes: { style: "text-align: center;" },
            //  headerAttributes: { style: "text-align: center;" }
            //},
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
    }

    selectManifest(url) {
      console.log(`accessing manifest detail at ${url}`);
      $.ajax(
        {
          type: "GET",
          url: url,
          success(data) {
            $.ajax(
              {
                type: "POST",
                url: selectUrl,
                data: { ManifestContent: data.Manifest },
                success(data) {
                  console.log("Manifest posted to controller");
                },
                error(xhr, status, error) {
                  alert(xhr.statusText);
                }
              });

            location.href = nextUrl;
          },
          error(xhr, status, error) {
            alert(xhr.statusText);
          }
        });
    }
  }
}