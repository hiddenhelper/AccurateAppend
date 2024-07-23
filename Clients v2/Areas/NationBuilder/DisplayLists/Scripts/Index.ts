/// <reference path="../../../../Scripts/typings/httpstatuscode.ts" />
/// <reference path="../../../../Scripts/typings/kendo-ui/kendo-ui.d.ts" />

module AccurateAppend.Websites.Clients.NationBuilder {

  // Used to hold the maximum list size
  declare var maxListSize: number;

  export interface IDisplayListsViewModel {
    GetListNameJson_DisplayLists_NationBuilder_Url: string,
    Index_DisplayLists_NationBuilder_Url: string,
    Index_Renew_NationBuilder_Url: string,
    GetListsJson_DisplayLists_NationBuilder_Url: string,
    GetNationsForUserJson_DisplayLists_NationBuilder_Url: string,
    CheckRegistrationValidToken_DisplayLists_NationBuilder_Url: string,
    Profile_Nations_Url: string,
    CartId: string;
  }

  export class DisplayListsView {
    ViewModel: IDisplayListsViewModel;

    initialize(viewModel: IDisplayListsViewModel) {
      console.log("initialize DisplayListsView");
      this.ViewModel = viewModel;
      this.populateNationsDropDown();
      var self = this;
      $("#nations").change(function() {
        console.log(`nations change firing. value = ${this.value}`);
        $("#error").hide();
        if (this.value === "AddNew") {
          history.pushState(null, "DisplayLists", this.ViewModel.Index_DisplayLists_NationBuilder_Url);
          window.location.replace(this.ViewModel.Index_Renew_NationBuilder_Url);
        } else {
          console.log("nations change break 1");
          $("#selectedListName").val("");
          const autocomplete = $("#listNames").data("kendoAutoComplete");
          autocomplete.value("");
          autocomplete.dataSource.read();
          self.renderGrid();
        }
      });

      $("#clearFilter").click(() => {
        history.pushState(null, "DisplayLists", "/NationBuilder/DisplayLists");
        window.location.replace(this.ViewModel.Index_DisplayLists_NationBuilder_Url);
      });

      this.initKendoUi();
    }

    initKendoUi() {
      var self = this;

      $("#listNames").kendoAutoComplete({
        dataSource: {
          type: "json",
          schema: {
            type: "json",
            data: "Data",
            total(response) {
              return response.length;
            }
          },
          transport: {
            read(options) {
              $.ajax({
                url: self.ViewModel.GetListNameJson_DisplayLists_NationBuilder_Url,
                dataType: "json",
                type: "GET",
                data: { id: $("#nations").val() },
                success: result => {
                  options.success(result);
                }
              });
            }
          }
        },
        filter: "startswith",
        placeholder: "Search by list name...",
        dataTextField: "name",
        select(e) {
          const dataItem = this.dataItem(e.item.index());
          $("#selectedListName").val(dataItem.name);
          $("#selectedListId").val(dataItem.id);
          self.renderGrid();
        },
        template: "<span >#: data.name #</span>",
        change() {
          const value = this.value();
          if (value === "") {
            $("#selectedListName").val("");
            $("#selectedListId").val("");
            self.renderGrid();
          }
        }
      });
    }

    populateNationsDropDown() {
      var self = this;
      console.log("populateNationsDropDown firing");
      $.getJSON(this.ViewModel.GetNationsForUserJson_DisplayLists_NationBuilder_Url,
        data => {
          $("#nationSelectHolder")
            .append(`<select id="nations" class="form-control" style="width: 300px;">`)
            .change(e => {
              $("#error").hide();
              if ($(e.target).val() === "AddNew") {
                history.pushState(null, "DisplayLists", this.ViewModel.Index_DisplayLists_NationBuilder_Url);
                window.location.replace(this.ViewModel.Index_Renew_NationBuilder_Url);
              } else {
                console.log("nations change break 1");
                $("#selectedListName").val("");
                const autocomplete = $("#listNames").data("kendoAutoComplete");
                autocomplete.value("");
                autocomplete.dataSource.read();
                self.renderGrid();
              }
            });
          $("#nations").append("<option value='AddNew'>Add new nation</option>");
          $.each(data,
            (index, value) => {
              if (data.length === 1)
                $("#nations").append(`<option selected='selected' value=${value.Id}>${value.NationName}</option>`);
              else if (value.IsActive) // select first active nation
                $("#nations").append(`<option selected='selected' value=${value.Id}>${value.NationName}</option>`);
              else
                $("#nations").append(`<option value=${value.Id}>${value.NationName}</option>`);

            });
          //$("#nations").removeClass();
          self.renderGrid();
        });
    }

    cmdClick(e) {
      e.preventDefault();
      const listId = $(e.target).closest("tr").find("td:eq(0)").text();
      const recordCount = parseFloat($(e.target).closest("tr").find("td:eq(2)").text().replace(/\,/g, ""));
      const listName = encodeURIComponent($(e.target).closest("tr").find("td:eq(1)").text());
      // validate list prior to next screen
      if (recordCount === 0) {
        $("#error span")
          .html("The list you have chosen does not appear to contain any records and can not be processed.");
        $("#error").show();
        return false;
      } else if (Math.round(recordCount) > maxListSize) {
        $("#error span")
          .html(
            `The list you have chosen exceeds the ${maxListSize.toLocaleString()
            } record processing limit for self-service. Please contact customer support and we will submit the list for you.`);
        $("#error").show();
        return false;
      }
      // redirect to order screen
      history.pushState(null, "NationBuilder lists", this.ViewModel.Index_DisplayLists_NationBuilder_Url);
      window.location.replace(
        `/NationBuilder/Order/SelectList?cartId=${this.ViewModel.CartId}&listId=${listId}&listName=${listName
        }&recordCount=${recordCount
        }&regId=${$("#nations").val()}`);
      return false;
    }

    renderGrid() {
      var self = this;
      console.log("calling renderGrid()");

      $("#error").hide();

      var nationName = $("#nations option:selected").text();
      $.ajax({
        url: self.ViewModel.CheckRegistrationValidToken_DisplayLists_NationBuilder_Url,
        dataType: "json",
        type: "GET",
        async: false,
        data: { nationName: $("#nations option:selected").text() },
        success(token) {

          if (token.HttpStatusCodeResult === Web.HttpStatusCode.FOUND) {
            console.log("CheckRegistrationValidToken returned 302");
            $("#error span").html(`We contacted NationBuilder and our access to your nation, '${nationName
              }' appears to have been removed. <a href="/NationBuilder/Renew?slug=${nationName
              }" class="alert-link">Click here to renew your access.</a>`);
            $("#error").show();
            $("#listDisplay").hide();
            $("#grid").hide();
          } else if (token.HttpStatusCodeResult === Web.HttpStatusCode.GONE ||
            token.HttpStatusCodeResult == Web.HttpStatusCode.BAD_REQUEST) {
            console.log("CheckRegistrationValidToken returned 400");
            $("#error span")
              .html(`We contacted NationBuilder and the Nation '${nationName
                }' appears to have been closed. Please contact support.`);
            $("#error").show();
            $("#listDisplay").hide();
            $("#grid").hide();
          } else {
            $("#listDisplay").show();
            console.log(`CheckRegistrationValidToken returned ${token.HttpStatusCodeResult}`);
            const grid = $("#grid").data("kendoGrid");
            if (grid !== undefined && grid !== null) {
              grid.dataSource.read();
            } else {
              const dsOptions: kendo.data.DataSourceOptions = {
                type: "json",
                schema: {
                  type: "json",
                  data: "Data",
                  total(response) {
                    return response.Data.length;
                  }
                },
                pageSize: 10,
                change() {
                  const autocomplete = $("#listNames").data("kendoAutoComplete");
                  if (this.total() <= 0) {
                    $("#warning span").html(`No lists found for ${nationName
                      }. This usually occurs when your nation has not been populated with data. <a href="http://nationbuilder.com/how_to_create_a_list" class="alert-link" target="_blank">Click here for NationBuilder's how-to on creating lists.</a>`);
                    $("#warning").show();
                    $("#listDisplay").hide();
                    autocomplete.enable(false);
                    autocomplete.value("");
                  } else {
                    $("#warning").hide();
                    $("#listDisplay").show();
                    autocomplete.enable(true);
                  }
                }
              };

              $("#grid").kendoGrid({
                autoBind: true,
                dataSource: {
                  type: "json",
                  schema: {
                    type: "json",
                    data: "Data",
                    total(response) {
                      return response.Data.length;
                    }
                  },
                  pageSize: 10,
                  transport: {
                    read(options) {
                      $("#warning").hide();
                      $.ajax({
                        url: self.ViewModel.GetListsJson_DisplayLists_NationBuilder_Url,
                        dataType: "json",
                        type: "GET",
                        data: {
                          listname: encodeURIComponent($("#selectedListName").val()),
                          listid: $("#selectedListId").val(),
                          Id: $("#nations").val()
                        },
                        success(result) {
                          if (result.HttpStatusCodeResult === 500) {
                            console.log("GetListsJson returned 500 status.");
                            $("#grid").hide();
                            $("#error span")
                              .html(`We were unable to retrieve the lists for ${nationName
                                }. Please contact customer support.`);
                            $("#error").show();
                          } else {
                            $("#grid").show();
                            $("#error").hide();
                            options.success(result);
                          }
                          $("#grid").find("[data-role=pager]").css({ "margin-bottom": 0 });
                        }
                      });
                    }
                  }
                },
                scrollable: false,
                sortable: true,
                pageable: {
                  input: true,
                  numeric: false
                },
                filterable: {
                  extra: false,
                  operators: {
                    string: {
                      eq: "Equals",
                      contains: "Contains"
                    }
                  }
                },
                columns: [
                  {
                    field: "id",
                    title: "Id",
                    filterable: true,
                    sortable: true,
                    headerAttributes: { style: "text-align: center;" },
                    attributes: { style: "text-align: center;" },
                    media: "(min-width: 450px)"
                  },
                  {
                    field: "name",
                    title: "Name",
                    filterable: true,
                    sortable: true,
                    media: "(min-width: 450px)"
                  },
                  {
                    field: "count",
                    title: "Count",
                    filterable: false,
                    sortable: true,
                    format: "{0:n0}",
                    headerAttributes: { style: "text-align: center;" },
                    attributes: { style: "text-align: center;" },
                    media: "(min-width: 450px)"
                  },
                  {
                    command: [
                      {
                        template:
                          '<a href="##" onClick="displayListView.cmdClick(event)">Get Estimate<i class="icon-arrow-right"></i></a>'
                      }
                    ],
                    title: " ",
                    width: "180px",
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
        }
      });
    }
  }
}

let displayListView: AccurateAppend.Websites.Clients.NationBuilder.DisplayListsView;

function initializeView(viewModel: AccurateAppend.Websites.Clients.NationBuilder.IDisplayListsViewModel) {
  $(document).ready(() => {
    displayListView = new AccurateAppend.Websites.Clients.NationBuilder.DisplayListsView();
    displayListView.initialize(viewModel);

  });
}