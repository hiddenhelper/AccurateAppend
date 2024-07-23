/// <reference path="../../../../scripts/typings/jquery/jquery.d.ts" />
var readUrl: any;
var apiTrialDetailViewModel: AccurateAppend.Websites.Admin.Clients.ApiTrialDetail.ApiTrialDetailViewModel;

$(() => {

  apiTrialDetailViewModel = new AccurateAppend.Websites.Admin.Clients.ApiTrialDetail.ApiTrialDetailViewModel();
  readUrl = $("#readUrl").val();


  $("#extendTrialButton").click(() => {
    apiTrialDetailViewModel.extend($("#maxCalls").val());
  });

  $("#copyAccessIdToClipboard").click(() => {
    $("#AccessId").select();
    document.execCommand("copy");
  });

  apiTrialDetailViewModel.init();


});

module AccurateAppend.Websites.Admin.Clients.ApiTrialDetail {

  export class ApiTrialDetailViewModel {

    Links: Links;

    init() {
      this.load();
      this.displayMethodCallCounts();
      this.displayOperationMatchCounts();
      this.setIsEnabledClick();
    }

    load() {
      const self = this;
      $.ajax(
        {
          type: "GET",
          url: `${readUrl}`,
          async: false,
          success(data) {
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
          error(xhr, status, error) {
            self.displayMessage(`Error: ${xhr.responseText}`, "danger", "#globalMessage");
          }
        });
    }

    extend(maxCalls) {
      const self = this;
      $.ajax(
        {
          type: "GET",
          url: `${self.Links.Extend}?maximumCalls=${maxCalls}`,
          success(status) {
            $("#extendTrialModal").modal("hide");
            //$("#IsEnabled").unbind();
            self.load();
            //self.setIsEnabledClick();
            self.displayMessage(`${status.Message}`, "info", "#globalMessage");
          },
          error(xhr, status, error) {
            $("#extendTrialModal").modal("hide");
            self.displayMessage(`Error extending trail. Message: ${xhr.responseText}`, "danger", "#globalMessage");
          }
        });
    }

    disable() {
      const self = this;
      $.ajax(
        {
          type: "GET",
          url: `${self.Links.Disable}`,
          success(status) {
            $("#extendTrialModal").modal("hide");
            //$("#IsEnabled").unbind();
            self.load();
            //self.setIsEnabledClick();
            self.displayMessage(`${status.Message}`, "info", "#globalMessage");
          },
          error(xhr, status, error) {
            self.displayMessage(`Error extending trail. Message: ${xhr.responseText}`, "danger", "#globalMessage");
          }
        });
    }

    displayMethodCallCounts() {
      const self = this;
      const grid = $("#MethodCallCountsGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#MethodCallCountsGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: `${self.Links.MethodCallCounts}`,
                  dataType: "json",
                  type: "GET",
                  success(result) {
                    options.success(result);
                  }
                });
              },
              cache: false
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
                return response.Data.length;
              }
            },
            change() {
              if (this.data().length <= 0) {
                $("#MethodCallCountsGridMessage").show();
                $("#MethodCallCountsGrid").hide();
              } else {
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
    }

    displayOperationMatchCounts() {
      const self = this;
      const grid = $("#OperationMatchCountsGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#OperationMatchCountsGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: `${self.Links.OperationMatchCounts}`,
                  dataType: "json",
                  type: "GET",
                  success(result) {
                    options.success(result);
                  }
                });
              },
              cache: false
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
                return response.Data.length;
              }
            },
            change() {
              if (this.data().length <= 0) {
                $("#OperationMatchCountsGridMessage").show();
                $("#OperationMatchCountsGrid").hide();
              } else {
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
    }

    displayMessage(message: any, type: any, div: any) {
      $(div).removeClass().addClass(`alert alert-${type}`).html(message).show()
        .fadeTo(5000, 500).slideUp(500, () => { $(div).slideUp(500) });
    }

    setIsEnabledClick() {
      $("#IsEnabled").change(function() {
        if (this.checked) {
          $("#extendTrialModal").modal("show");
        } else {
          apiTrialDetailViewModel.disable();
        }
      });
    }
  }

  export class Links {
    Disable: string;
    Extend: string;
    MethodCallCounts: string;
    OperationMatchCounts: string;
    SourceLead: string;

    constructor(data: any) {
      this.Disable = data.Links.Disable;
      this.Extend = data.Links.Extend;
      this.MethodCallCounts = data.Links.MethodCallCounts;
      this.OperationMatchCounts = data.Links.OperationMatchCounts;
      this.SourceLead = data.Links.SourceLead;
    }
  }

}