/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

var userDetailViewModel: AccurateAppend.Websites.Admin.Clients.UserDetail.UserDetailViewModel;
var notesViewModel: AccurateAppend.Websites.Admin.Clients.UserDetail.NotesViewModel;
var documentsViewModel: AccurateAppend.Websites.Admin.Clients.UserDetail.DocumentsViewModel;
var ticketsViewModel: AccurateAppend.Websites.Admin.Clients.UserDetail.TicketsApiViewModel;
var adminUsers: string;

$(() => {

  userDetailViewModel = new AccurateAppend.Websites.Admin.Clients.UserDetail.UserDetailViewModel();
  userDetailViewModel.init(adminUsers);
  userDetailViewModel.loadUserOperatingMetricReport();
  userDetailViewModel.loadUserProductUsageOverviewReport();
  notesViewModel = new AccurateAppend.Websites.Admin.Clients.UserDetail.NotesViewModel(userDetailViewModel.Links);
  notesViewModel.read();
  documentsViewModel =
    new AccurateAppend.Websites.Admin.Clients.UserDetail.DocumentsViewModel(userDetailViewModel.Links);
  documentsViewModel.read();
  ticketsViewModel =
    new AccurateAppend.Websites.Admin.Clients.UserDetail.TicketsApiViewModel(userDetailViewModel.Links);
  ticketsViewModel.read();

  // click handlers
  $("#addNoteButton").click(() => { $("#accountNoteModal").modal("show"); });
  $("#addDocumentButton").click(() => { $("#documentUploadModal").modal("show"); });
  $("#StoreData").change(() => { userDetailViewModel.save(); });
  $("#batchUser").change(() => { userDetailViewModel.save(); });
  $("#xmlUser").change(() => { userDetailViewModel.save(); });
  $("#IsLockedOut").change(() => { userDetailViewModel.save(); });
  $("#copyUserIdToClipboard").click(() => {
    $("#userid").select();
    document.execCommand("copy");
  });

  // button bar handlers
  $("#buttonBar #Cards").attr("href", userDetailViewModel.Links.Cards);
  $("#buttonBar #Charges").attr("href", userDetailViewModel.Links.Charges);
  $("#buttonBar #NewDeal").attr("href", userDetailViewModel.Links.NewDeal);
  $("#buttonBar #Deals").attr("href", userDetailViewModel.Links.Deals);
  $("#buttonBar #JobsNew").attr("href", userDetailViewModel.Links.JobsNew);
  $("#buttonBar #Jobs").attr("href", userDetailViewModel.Links.Jobs);
  $("#buttonBar #LogInAsUser").attr("href", userDetailViewModel.Links.LogInAsUser);
  $("#buttonBar #UserMustChangePassword").click(() => { $("#confirmationForcePasswordReset").modal("show");});
  $("#buttonBar #Contacts").attr("href", userDetailViewModel.Links.Contacts);
  $("#buttonBar #Files").attr("href", userDetailViewModel.Links.Files);
  $("#buttonBar #Messages").attr("href", userDetailViewModel.Links.Messages);
  $("#buttonBar #AutoProcessorRules").attr("href", userDetailViewModel.Links.AutoProcessorRules);
  $("#buttonBar #RateCards").attr("href", userDetailViewModel.Links.RateCards);
  $("#buttonBar #ServiceAccounts").attr("href", userDetailViewModel.Links.ServiceAccounts);
  $("#buttonBar #APIReporting").attr("href", userDetailViewModel.Links.APIReporting);
  $("#buttonBar #DownloadUsage").click(() => { $("#downloadUsageModal").modal("show"); });
  $("#buttonBar #CopyPaymentLinkToClipboard").click(() => {
    $("#clipboard").val(userDetailViewModel.Links.PaymentUpdateLink).select();
    document.execCommand("copy");
  });
  $("#buttonBar #ViewNations").click(() => { userDetailViewModel.displayNations(); });
  $("#buttonBar #SourceLead").attr("href", userDetailViewModel.Links.SourceLead);
  $("#createTicket").attr("href", userDetailViewModel.Links.CreateTicket);
  $("#xmlUser").change(() => {
    if (this.checked) alert("Ensure the user has a valid subscription before enabling API access.");
  });
  $("#accountOwner").change(() => {
    const publicKey = $("#publicKey").val();
    const ownerId = $("#accountOwner").val();
    userDetailViewModel.updateOwner(publicKey, ownerId);
  });

  // other
  $("#btnForcePasswordReset").attr("href", userDetailViewModel.Links.UserMustChangePassword);

  // initialize admin document upload control
  $("#adminFilesToUpload").kendoUpload({
    async: {
      saveUrl: userDetailViewModel.Links.AdminFileUpload,
      autoUpload: true.valueOf(),
      withCredentials: false
    },
    complete() {
      $("#documentUploadModal").modal("hide");
      $(".k-upload-files li").remove();
      $(".k-upload-status").remove();
      documentsViewModel.read();
    }
  });

});

module AccurateAppend.Websites.Admin.Clients.UserDetail {

  export class UserDetailViewModel {

    Links: Links;

    init(adminUsers: any) {

      $("#error").hide();
      var self = this;
      $.ajax(
        {
          type: "GET",
          context: this,
          url: $("input[type=hidden][id=userDetailUri]").val(),
          async: false,
          success(data) {
            $("#DateAdded").text(data.DateAdded);
            $("#LastActivityDate").text(`Last: ${data.LastActivityDate}`);
            $("#Email").val(data.Email);
            $("#BusinessName").val(data.BusinessName);
            $("#FirstName").val(data.FirstName);
            $("#LastName").val(data.LastName);
            $("#Address").val(data.Address);
            $("#City").val(data.City);
            $("#State").val(data.State);
            $("#Zip").val(data.Zip);
            $("#Email").val(data.Email);
            $("#Phone").val(data.Phone);
            $("#userid").val(data.UserId);
            // initialize sales rep drop down
            this.initAdminUsersSelect(adminUsers, data.PublicKey);
            $("#accountOwner").val(data.OwnerId);
            // set values for bool properties
            $("#IsLockedOut").prop("checked", data.IsLockedOut).change();
            $("#batchUser").prop("checked", data.batchUser).change();
            $("#xmlUser").prop("checked", data.xmlUser).change();
            $("#StoreData").prop("checked", data.StoreData).change();

            // load links
            self.Links = new Links(data);
          },
          error(xhr, status, error) {
            displayMessage(`<strong>Error retrieving user. </strong>${xhr.responseText}`, "danger");
          }
        });
    }

      initAdminUsersSelect(adminUsers: any, publicKey: string) {
      $("#publicKey").val(publicKey);
      $.each(adminUsers,
        (key, value) => {
          $("#accountOwner")
            .append($("<option></option>")
              .attr("value", value.UserId)
              .text(value.UserName));
              });
    }

    save() {
      $("#error").hide();
      $.ajax({
        type: "POST",
        url: this.Links.Edit,
        data: {
          email: $("#Email").val(),
          firstname: $("#FirstName").val(),
          lastname: $("#LastName").val(),
          businessname: $("#BusinessName").val(),
          address: $("#Address").val(),
          city: $("#City").val(),
          state: $("#State").val(),
          zip: $("#Zip").val(),
          phone: $("#Phone").val(),
          defaultproduct: "",
          defaultcolumnmap: "",
          userid: $("#userid").val(),
          islockedout: $("#IsLockedOut").prop("checked"),
          batchUser: $("#batchUser").prop("checked"),
          xmlUser: $("#xmlUser").prop("checked"),
          storeData: $("#StoreData").prop("checked")
        },
        success() {
          displayMessage(`User successfully updated`, "success");
        },
        error(xhr, status, error) {
          displayMessage(`<strong>Error updating user. </strong>${xhr.responseText}`, "danger");
        }
      });
    }

    updateOwner(publicKey: string, ownerId: string) {
      $.ajax({
        type: "POST",
        url: this.Links.ChangeOwner,
        data: {
          Id: publicKey,
          OwnerId: ownerId
        },
        success(data) {
          displayMessage(data.Message, "success");
        },
        error(xhr, status, error) {
            displayMessage(xhr.responseText, "danger");
        }
      });
    }

    downloadUsage() {
      $("#downloadUsageModal").modal("hide");
      window.location.replace(
        `${this.Links.DownloadUsage}&start=${moment($("#usageDateRangeWidget_startDate").val()).format("YYYY-MM-DD")
        }&end=${
        moment($("#usageDateRangeWidget_endDate").val()).format("YYYY-MM-DD")}`);
    }

    saveUsage() {
      $("#downloadUsageModal").modal("hide");
      $.ajax(
        {
          type: "GET",
          url: `${this.Links.SaveUsage}&start=${moment($("#usageDateRangeWidget_startDate").val()).format("YYYY-MM-DD")
            }&end=${
            moment($("#usageDateRangeWidget_endDate").val()).format("YYYY-MM-DD")}`,
          async: true,
          success(data) {
            if (data.success)
              displayMessage(data.message, "success");
            else
              displayMessage(`<strong>Error saving usage report to user files. </strong>`, "danger");
          },
          error(xhr, status, error) {
            displayMessage(`<strong>Error save usage report to user files. </strong>${xhr.responseText}`, "danger");
          }
        });
    }

    displayNations() {
      const self = this;
      $("#nationBuilderRegistrations").modal("show");
      $.ajax(
        {
          type: "GET",
          url: self.Links.ViewNations,
          success(registrations) {
            if (registrations.length === 0) {
              $("#nationsMessage").text("No Nations found.").show();
            } else {
              $("#nations tbody tr").remove();
              $(registrations).each((i, r: any) => {
                $(`<tr><td>${r.Slug}</td><td><input type="text" size=45 style="border: 0" value="${r.AccessToken
                  }" onclick="$(this).select()"/></td></tr>`).appendTo($("#nations tbody"));
              });
            }
          },
          error(xhr, status, error) {
            displayMessage(`Error updating user. Message: ${xhr.responseText}`, "danger");
          }
        });
    }

    loadUserOperatingMetricReport() {
      var self = this;
      const grid = $("#UserOperatingMetricsOverviewGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#UserOperatingMetricsOverviewGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: self.Links.UserOperatingMetric,
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
            }
          },
          columns: [
            {
              field: "MetricNameDescription",
              title: "Description",
              headerAttributes: { style: "text-align: right;" },
              media: "(min-width: 450px)"
            },
            {
              field: "Today",
              title: "Today",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#todayTemplate").html()),
              media: "(min-width: 450px)"
            },
            {
              field: "Last7",
              title: "Last 7",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#last7Template").html()),
              media: "(min-width: 450px)"
            },
            {
              field: "CurrentMonth",
              title: "Current Month",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#currentMonthTemplate").html()),
              media: "(min-width: 450px)"
            },
            {
              field: "SamePeriodLastMonth",
              title: "Same Period Last Month",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#samePeriodLastMonthTemplate").html()),
              media: "(min-width: 450px)"
            },
            {
              field: "LastMonth",
              title: "Last Month",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#LastMonthTemplate").html()),
              media: "(min-width: 450px)"
            },
            {
              field: "PreviousToLastMonth",
              title: "Previous To Last Month",
              headerAttributes: { style: "text-align: right;" },
              template: kendo.template($("#previousToLastMonthTemplate").html()),
              media: "(min-width: 450px)"
            },
            {
              title: "Summary",
              template: kendo.template($("#operating-metrics-responsive-template").html()),
              media: "(max-width: 450px)"
            }
          ],
          scrollable: false
        });
      }
    }

    loadUserProductUsageOverviewReport() {
      var self = this;
      const grid = $("#UserProductUsageOverviewGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#UserProductUsageOverviewGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: self.Links.UserProductUsageMetric,
                  dataType: "json",
                  type: "GET",
                  success(result) {
                    options.success(result);
                  }
                });
              }
            },
            change: function() {
              if (this.data().length <= 0) {
                $("#UserProductUsageOverviewGridMessage").show();
                $("#UserProductUsageOverviewGrid").hide();
              } else {
                $("#UserProductUsageOverviewGridMessage").hide();
                $("#UserProductUsageOverviewGrid").show();
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
              template: kendo.template("#= processingMetricFormatter(TodayRecords,TodayMatches) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "Last7",
              title: "Last 7",
              template: kendo.template("#= processingMetricFormatter(Last7Records,Last7Matches) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "CurrentMonth",
              title: "Current Month",
              template: kendo.template("#= processingMetricFormatter(CurrentMonthRecords,CurrentMonthMatches) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "SamePeriodLastMonth",
              title: "Same Period Last Month",
              template: kendo.template(
                "#= processingMetricFormatter(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "LastMonth",
              title: "Last Month",
              template: kendo.template("#= processingMetricFormatter(LastMonthRecords,LastMonthMatches) #"),
              attributes: { style: "text-align:right;" }
            },
            {
              field: "PreviousToLastMonth",
              title: "Previous Month",
              template: kendo.template(
                "#= processingMetricFormatter(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
              attributes: { style: "text-align:right;" }
            }
          ]
        });
      }
    }

    forcePasswordChange() {
      $.ajax({
        type: "GET",
        url: this.Links.UserMustChangePassword,
        success() {
          $("#confirmationForcePasswordReset").modal("hide");
          displayMessage(`User will be forced to change their password the next time they login.`, "success");
        },
        error(xhr, status, error) {
          displayMessage(`<strong>Error updating user. </strong>${xhr.responseText}`, "danger");
        }
      });
    }
  }

  export class TicketsApiViewModel {

    constructor(private readonly links: Links) {
    }

    read() {
      const self = this;
      const grid = $("#ticketsGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#ticketsGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: self.links.TicketSummary,
                  dataType: "json",
                  type: "GET",
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
                $("#ticketsGridMessage").text("No tickets found").show();
                $("#ticketsGrid").hide();
                $("#pager").hide();
              } else {
                $("#ticketsGridMessage").text("").hide();
                $("#ticketsGrid").show();
                $("#pager").show();
                $("#warning").hide();
              }
            }
          },
          scrollable: false,
          filterable: false,
          pageable: true,
          dataBound(e) {
            const grid = $("#ticketsGrid").data("kendoGrid");
            const dataView = grid.dataSource.view();

            for (let i = 0; i < dataView.length; i++) {
              for (let j = 0; j < dataView[i].items.length; j++) {
                if (dataView[i].items[j].status === "Closed") {
                  const uid = dataView[i].items[j].uid;
                  grid.collapseGroup($("#ticketsGrid").find(`tr[data-uid=${uid}]`).prev("tr.k-grouping-row"));
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
              attributes: { style: "text-align: center;" },
              media: "(min-width: 450px)"
            },
            //{
            //  field: "Type",
            //  title: "Type",
            //  attributes: { style: "text-align: center;" },
            //  width: 200,
            //  //template: "#= kendo.toString(kendo.parseDate(DateSubmitted, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
            //  media: "(min-width: 450px)"
            //},
            //{
            //  field: "Status",
            //  title: "Status",
            //  attributes: { style: "text-align: center;" },
            //  width: 200,
            //  //template: "#= kendo.toString(kendo.parseDate(DateSubmitted, 'MM/dd/yyyy'), 'MM/dd/yyyy') #",
            //  media: "(min-width: 450px)"
            //},
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
                "<a href='\\#' class=\"btn btn-default\" style=\"margin-right: 5px;\" onclick=\"ticketsViewModel.viewDetail('#= uid #')\">View Detail</a><a href=\"#= ZendeskDetail #\" class=\"btn btn-default\">View In Zendesk</a>",
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
      const grid = $("#ticketsGrid").data("kendoGrid");
      const row = grid.tbody.find(`tr[data-uid='${uid}']`);
      const item = grid.dataItem(row);
      $("#detailsModal .modal-header").html(`Details for Ticket ${item["Id"]}`);
      $("#detailsModal .modal-body pre").html(item["Description"]);
      $("#detailsModal").appendTo("body").modal("show");
    }
  }

  export class NotesViewModel {

    constructor(private readonly links: Links) {
    }

    read() {
      $.ajax(
        {
          type: "GET",
          url: this.links.UserNotes,
          success(notes) {
            $("#notes table tbody tr").remove();
            if (notes.length === 0) {
              $("#notes #message").show();
              $("#notes_table").hide();
            } else {
              $("#notes_no_notes_message").hide();
              $("#notes_table").show();
              $(notes).each((i, note: any) => {
                $(`<tr><td>${note.DateAdded}</td><td>${note.AddedBy
                    }</td><td>${note.Body}</td></tr>`)
                  .appendTo("#notes table tbody");
              });
            }
          },
          error(xhr, status, error) {
            $("#globalMessage").removeClass().addClass("alert alert-danger").html(`Error: ${xhr.responseText}`).show();
          }
        });
    }

    save() {
      $("#error").hide();
      var self = this;
      $("#accountNoteModal").modal("hide");
      $.ajax(
        {
          type: "POST",
          url: self.links.AddUserNote,
          data: { addedby: $("#adminusername").val(), body: $("#notebody").val(), userid: $("#userid").val() },
          success() {
            self.read();
            $("#notebody").val("");
          },
          error(xhr, status, error) {
            displayMessage(`Error saving note. Message: ${xhr.statusText}`, "danger");
          }
        });
    }
  }

  export class DocumentsViewModel {

    constructor(private readonly links: Links) {
    }

    save() {
    }

    read() {
      var self = this;
      $.ajax(
        {
          type: "GET",
          url: this.links.AdminFiles,
          success(files) {
            $("#documents table tbody tr").remove();
            if (files.length === 0) {
              $("#documents #message").show();
              $("#documents_table").hide();
            } else {
              $("#documents_table").show();
              $("#documents #message").hide();
              $(files).each((i, document: any) => {
                $(`<tr><td>${document.DateAdded}</td><td>${document.CustomerFileName}</td><td id="${document.FileId}">${
                  document.Notes
                  }</td><td style="text-align: right;"><a href="#" class="btn btn-danger btn-sm" style="margin-right: 5px;" onclick=\"documentsViewModel.delete(${
                  document.FileId
                  });">Delete</a><a href="#" class="btn btn-default btn-sm" style="margin-right: 5px;" onclick="documentsViewModel.openAddNoteModal(${
                  document.FileId
                  });">Add&nbsp;Note</a><a class="btn btn-default btn-sm" href="${self.links.AdminFileDownload
                  }?fileid=${
                  document.FileId}">Download</a></td></tr>`).appendTo("#documents table tbody");
              });
            }
          },
          error(xhr, status, error) {
            displayMessage(`Error: ${xhr.responseText}`, "danger");
          }
        });

    }

    delete(fileId) {
      var self = this;
      $("#error").hide();
      if (confirm("Delete this file?")) {
        // delete file
        $.ajax(
          {
            type: "POST",
            url: self.links.AdminFileDelete,
            data: { fileid: fileId },
            success() {
              self.read();
            }
          });
      }
    }

    openAddNoteModal(fileid: number) {
      $("#error").hide();
      $("#documentId").val(fileid);
      $("#adminFileNoteModal").modal("show");
    }

    addNote() {
      $("#error").hide();
      var self = this;
      $("#adminFileNoteModal").modal("hide");
      $.ajax(
        {
          type: "POST",
          url: this.links.AdminFileAddNote,
          data: { notes: $("#documentNoteBody").val(), fileid: $("#documentId").val() },
          success() {
            self.read();
          },
          error(xhr, status, error) {
            displayMessage(`Error: ${xhr.responseText}`, "danger");
          }
        });
    }

  }

  export class Links {
    Cards: string;
    Detail: string;
    Edit: string;
    Files: string;
    AdminFiles: string;
    AdminFileDelete: string;
    AdminFileAddNote: string;
    AdminFileDownload: string;
    AdminFileUpload: string;
    SaveUserFile: string;
    UserNotes: string;
    AddUserNote: string;
    Jobs: string;
    JobsNew: string;
    LogInAsUser: string;
    AutoProcessorRules: string;
    Contacts: string;
    UserMustChangePassword: string;
    ServiceAccounts: string;
    Deals: string;
    NewDeal: string;
    Charges: string;
    Messages: string;
    RateCards: string;
    APIReporting: string;
    DownloadUsage: string;
    SaveUsage: string;
    LeadDetail: string;
    ViewNations: string;
    PaymentUpdateLink: string;
    SourceLead: string;
    UserOperatingMetric: string;
    UserProductUsageMetric: string;
    TicketSummary: string;
    CreateTicket: string;
    ChangeOwner: string;

    constructor(data: any) {
      this.Cards = data.Links.Cards;
      this.Detail = data.Links.Detail;
      this.Edit = data.Links.Edit;
      this.Files = data.Links.Files;
      this.AdminFiles = data.Links.AdminFiles;
      this.AdminFileDelete = data.Links.AdminFileDelete;
      this.AdminFileAddNote = data.Links.AdminFileAddNote;
      this.AdminFileDownload = data.Links.AdminFileDownload;
      this.AdminFileUpload = data.Links.AdminFileUpload;
      this.SaveUserFile = data.Links.SaveUserFile;
      this.UserNotes = data.Links.UserNotes;
      this.AddUserNote = data.Links.AddUserNote;
      this.Jobs = data.Links.Jobs;
      this.JobsNew = data.Links.JobsNew;
      this.LogInAsUser = data.Links.LogInAsUser;
      this.AutoProcessorRules = data.Links.AutoProcessorRules;
      this.Contacts = data.Links.Contacts;
      this.UserMustChangePassword = data.Links.UserMustChangePassword;
      this.ServiceAccounts = data.Links.ServiceAccounts;
      this.Deals = data.Links.Deals;
      this.NewDeal = data.Links.NewDeal;
      this.Charges = data.Links.Charges;
      this.Messages = data.Links.Messages;
      this.RateCards = data.Links.RateCards;
      this.APIReporting = data.Links.APIReporting;
      this.DownloadUsage = data.Links.DownloadUsage;
      this.SaveUsage = data.Links.SaveUsage;
      this.LeadDetail = data.Links.LeadDetail;
      this.ViewNations = data.Links.ViewNations;
      this.PaymentUpdateLink = data.Links.PaymentUpdateLink;
      this.SourceLead = data.Links.SourceLead;
      this.UserOperatingMetric = data.Links.UserOperatingMetric;
      this.UserProductUsageMetric = data.Links.UserProductUsageMetric;
      this.TicketSummary = data.Links.TicketSummary;
      this.CreateTicket = data.Links.CreateTicket;
      this.ChangeOwner = data.Links.ChangeOwner;
    }
  }

  function displayMessage(message, type) {
    $("#globalMessage").removeClass().addClass(`alert alert-${type}`).html(message).show()
      .fadeTo(7000, 500).slideUp(500, () => { $("#globalMessage").slideUp(500) });
  }

}

function processingMetricFormatter(records: number, matches: number) {
  if (records === 0) return "-";
  return kendo.toString(records, "n0") +
    " / " +
    kendo.toString(matches, "n0") +
    " (" +
    Math.floor((matches / records) * 100) +
    "%)";
}