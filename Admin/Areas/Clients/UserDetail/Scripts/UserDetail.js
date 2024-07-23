var _this = this;
var userDetailViewModel;
var notesViewModel;
var documentsViewModel;
var ticketsViewModel;
var adminUsers;
$(function () {
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
    $("#addNoteButton").click(function () { $("#accountNoteModal").modal("show"); });
    $("#addDocumentButton").click(function () { $("#documentUploadModal").modal("show"); });
    $("#StoreData").change(function () { userDetailViewModel.save(); });
    $("#batchUser").change(function () { userDetailViewModel.save(); });
    $("#xmlUser").change(function () { userDetailViewModel.save(); });
    $("#IsLockedOut").change(function () { userDetailViewModel.save(); });
    $("#copyUserIdToClipboard").click(function () {
        $("#userid").select();
        document.execCommand("copy");
    });
    $("#buttonBar #Cards").attr("href", userDetailViewModel.Links.Cards);
    $("#buttonBar #Charges").attr("href", userDetailViewModel.Links.Charges);
    $("#buttonBar #NewDeal").attr("href", userDetailViewModel.Links.NewDeal);
    $("#buttonBar #Deals").attr("href", userDetailViewModel.Links.Deals);
    $("#buttonBar #JobsNew").attr("href", userDetailViewModel.Links.JobsNew);
    $("#buttonBar #Jobs").attr("href", userDetailViewModel.Links.Jobs);
    $("#buttonBar #LogInAsUser").attr("href", userDetailViewModel.Links.LogInAsUser);
    $("#buttonBar #UserMustChangePassword").click(function () { $("#confirmationForcePasswordReset").modal("show"); });
    $("#buttonBar #Contacts").attr("href", userDetailViewModel.Links.Contacts);
    $("#buttonBar #Files").attr("href", userDetailViewModel.Links.Files);
    $("#buttonBar #Messages").attr("href", userDetailViewModel.Links.Messages);
    $("#buttonBar #AutoProcessorRules").attr("href", userDetailViewModel.Links.AutoProcessorRules);
    $("#buttonBar #RateCards").attr("href", userDetailViewModel.Links.RateCards);
    $("#buttonBar #ServiceAccounts").attr("href", userDetailViewModel.Links.ServiceAccounts);
    $("#buttonBar #APIReporting").attr("href", userDetailViewModel.Links.APIReporting);
    $("#buttonBar #DownloadUsage").click(function () { $("#downloadUsageModal").modal("show"); });
    $("#buttonBar #CopyPaymentLinkToClipboard").click(function () {
        $("#clipboard").val(userDetailViewModel.Links.PaymentUpdateLink).select();
        document.execCommand("copy");
    });
    $("#buttonBar #ViewNations").click(function () { userDetailViewModel.displayNations(); });
    $("#buttonBar #SourceLead").attr("href", userDetailViewModel.Links.SourceLead);
    $("#createTicket").attr("href", userDetailViewModel.Links.CreateTicket);
    $("#xmlUser").change(function () {
        if (_this.checked)
            alert("Ensure the user has a valid subscription before enabling API access.");
    });
    $("#accountOwner").change(function () {
        var publicKey = $("#publicKey").val();
        var ownerId = $("#accountOwner").val();
        userDetailViewModel.updateOwner(publicKey, ownerId);
    });
    $("#btnForcePasswordReset").attr("href", userDetailViewModel.Links.UserMustChangePassword);
    $("#adminFilesToUpload").kendoUpload({
        async: {
            saveUrl: userDetailViewModel.Links.AdminFileUpload,
            autoUpload: true.valueOf(),
            withCredentials: false
        },
        complete: function () {
            $("#documentUploadModal").modal("hide");
            $(".k-upload-files li").remove();
            $(".k-upload-status").remove();
            documentsViewModel.read();
        }
    });
});
var AccurateAppend;
(function (AccurateAppend) {
    var Websites;
    (function (Websites) {
        var Admin;
        (function (Admin) {
            var Clients;
            (function (Clients) {
                var UserDetail;
                (function (UserDetail) {
                    var UserDetailViewModel = (function () {
                        function UserDetailViewModel() {
                        }
                        UserDetailViewModel.prototype.init = function (adminUsers) {
                            $("#error").hide();
                            var self = this;
                            $.ajax({
                                type: "GET",
                                context: this,
                                url: $("input[type=hidden][id=userDetailUri]").val(),
                                async: false,
                                success: function (data) {
                                    $("#DateAdded").text(data.DateAdded);
                                    $("#LastActivityDate").text("Last: " + data.LastActivityDate);
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
                                    this.initAdminUsersSelect(adminUsers, data.PublicKey);
                                    $("#accountOwner").val(data.OwnerId);
                                    $("#IsLockedOut").prop("checked", data.IsLockedOut).change();
                                    $("#batchUser").prop("checked", data.batchUser).change();
                                    $("#xmlUser").prop("checked", data.xmlUser).change();
                                    $("#StoreData").prop("checked", data.StoreData).change();
                                    self.Links = new Links(data);
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("<strong>Error retrieving user. </strong>" + xhr.responseText, "danger");
                                }
                            });
                        };
                        UserDetailViewModel.prototype.initAdminUsersSelect = function (adminUsers, publicKey) {
                            $("#publicKey").val(publicKey);
                            $.each(adminUsers, function (key, value) {
                                $("#accountOwner")
                                    .append($("<option></option>")
                                    .attr("value", value.UserId)
                                    .text(value.UserName));
                            });
                        };
                        UserDetailViewModel.prototype.save = function () {
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
                                success: function () {
                                    displayMessage("User successfully updated", "success");
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("<strong>Error updating user. </strong>" + xhr.responseText, "danger");
                                }
                            });
                        };
                        UserDetailViewModel.prototype.updateOwner = function (publicKey, ownerId) {
                            $.ajax({
                                type: "POST",
                                url: this.Links.ChangeOwner,
                                data: {
                                    Id: publicKey,
                                    OwnerId: ownerId
                                },
                                success: function (data) {
                                    displayMessage(data.Message, "success");
                                },
                                error: function (xhr, status, error) {
                                    displayMessage(xhr.responseText, "danger");
                                }
                            });
                        };
                        UserDetailViewModel.prototype.downloadUsage = function () {
                            $("#downloadUsageModal").modal("hide");
                            window.location.replace(this.Links.DownloadUsage + "&start=" + moment($("#usageDateRangeWidget_startDate").val()).format("YYYY-MM-DD") + "&end=" + moment($("#usageDateRangeWidget_endDate").val()).format("YYYY-MM-DD"));
                        };
                        UserDetailViewModel.prototype.saveUsage = function () {
                            $("#downloadUsageModal").modal("hide");
                            $.ajax({
                                type: "GET",
                                url: this.Links.SaveUsage + "&start=" + moment($("#usageDateRangeWidget_startDate").val()).format("YYYY-MM-DD") + "&end=" + moment($("#usageDateRangeWidget_endDate").val()).format("YYYY-MM-DD"),
                                async: true,
                                success: function (data) {
                                    if (data.success)
                                        displayMessage(data.message, "success");
                                    else
                                        displayMessage("<strong>Error saving usage report to user files. </strong>", "danger");
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("<strong>Error save usage report to user files. </strong>" + xhr.responseText, "danger");
                                }
                            });
                        };
                        UserDetailViewModel.prototype.displayNations = function () {
                            var self = this;
                            $("#nationBuilderRegistrations").modal("show");
                            $.ajax({
                                type: "GET",
                                url: self.Links.ViewNations,
                                success: function (registrations) {
                                    if (registrations.length === 0) {
                                        $("#nationsMessage").text("No Nations found.").show();
                                    }
                                    else {
                                        $("#nations tbody tr").remove();
                                        $(registrations).each(function (i, r) {
                                            $("<tr><td>" + r.Slug + "</td><td><input type=\"text\" size=45 style=\"border: 0\" value=\"" + r.AccessToken + "\" onclick=\"$(this).select()\"/></td></tr>").appendTo($("#nations tbody"));
                                        });
                                    }
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("Error updating user. Message: " + xhr.responseText, "danger");
                                }
                            });
                        };
                        UserDetailViewModel.prototype.loadUserOperatingMetricReport = function () {
                            var self = this;
                            var grid = $("#UserOperatingMetricsOverviewGrid").data("kendoGrid");
                            if (grid !== undefined && grid !== null) {
                                grid.dataSource.read();
                            }
                            else {
                                $("#UserOperatingMetricsOverviewGrid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: function (options) {
                                                $.ajax({
                                                    url: self.Links.UserOperatingMetric,
                                                    dataType: "json",
                                                    type: "GET",
                                                    success: function (result) {
                                                        options.success(result);
                                                    }
                                                });
                                            },
                                            cache: false
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
                        };
                        UserDetailViewModel.prototype.loadUserProductUsageOverviewReport = function () {
                            var self = this;
                            var grid = $("#UserProductUsageOverviewGrid").data("kendoGrid");
                            if (grid !== undefined && grid !== null) {
                                grid.dataSource.read();
                            }
                            else {
                                $("#UserProductUsageOverviewGrid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: function (options) {
                                                $.ajax({
                                                    url: self.Links.UserProductUsageMetric,
                                                    dataType: "json",
                                                    type: "GET",
                                                    success: function (result) {
                                                        options.success(result);
                                                    }
                                                });
                                            }
                                        },
                                        change: function () {
                                            if (this.data().length <= 0) {
                                                $("#UserProductUsageOverviewGridMessage").show();
                                                $("#UserProductUsageOverviewGrid").hide();
                                            }
                                            else {
                                                $("#UserProductUsageOverviewGridMessage").hide();
                                                $("#UserProductUsageOverviewGrid").show();
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
                                            template: kendo.template("#= processingMetricFormatter(SamePeriodLastMonthRecords,SamePeriodLastMonthMatches) #"),
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
                                            template: kendo.template("#= processingMetricFormatter(PreviousToLastMonthRecords,PreviousToLastMonthMatches) #"),
                                            attributes: { style: "text-align:right;" }
                                        }
                                    ]
                                });
                            }
                        };
                        UserDetailViewModel.prototype.forcePasswordChange = function () {
                            $.ajax({
                                type: "GET",
                                url: this.Links.UserMustChangePassword,
                                success: function () {
                                    $("#confirmationForcePasswordReset").modal("hide");
                                    displayMessage("User will be forced to change their password the next time they login.", "success");
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("<strong>Error updating user. </strong>" + xhr.responseText, "danger");
                                }
                            });
                        };
                        return UserDetailViewModel;
                    }());
                    UserDetail.UserDetailViewModel = UserDetailViewModel;
                    var TicketsApiViewModel = (function () {
                        function TicketsApiViewModel(links) {
                            this.links = links;
                        }
                        TicketsApiViewModel.prototype.read = function () {
                            var self = this;
                            var grid = $("#ticketsGrid").data("kendoGrid");
                            if (grid !== undefined && grid !== null) {
                                grid.dataSource.read();
                            }
                            else {
                                $("#ticketsGrid").kendoGrid({
                                    dataSource: {
                                        type: "json",
                                        transport: {
                                            read: function (options) {
                                                $.ajax({
                                                    url: self.links.TicketSummary,
                                                    dataType: "json",
                                                    type: "GET",
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
                                                $("#ticketsGridMessage").text("No tickets found").show();
                                                $("#ticketsGrid").hide();
                                                $("#pager").hide();
                                            }
                                            else {
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
                                    dataBound: function (e) {
                                        var grid = $("#ticketsGrid").data("kendoGrid");
                                        var dataView = grid.dataSource.view();
                                        for (var i = 0; i < dataView.length; i++) {
                                            for (var j = 0; j < dataView[i].items.length; j++) {
                                                if (dataView[i].items[j].status === "Closed") {
                                                    var uid = dataView[i].items[j].uid;
                                                    grid.collapseGroup($("#ticketsGrid").find("tr[data-uid=" + uid + "]").prev("tr.k-grouping-row"));
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
                                            attributes: { style: "text-align: center;" },
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
                                            template: "<a href='\\#' class=\"btn btn-default\" style=\"margin-right: 5px;\" onclick=\"ticketsViewModel.viewDetail('#= uid #')\">View Detail</a><a href=\"#= ZendeskDetail #\" class=\"btn btn-default\">View In Zendesk</a>",
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
                            var grid = $("#ticketsGrid").data("kendoGrid");
                            var row = grid.tbody.find("tr[data-uid='" + uid + "']");
                            var item = grid.dataItem(row);
                            $("#detailsModal .modal-header").html("Details for Ticket " + item["Id"]);
                            $("#detailsModal .modal-body pre").html(item["Description"]);
                            $("#detailsModal").appendTo("body").modal("show");
                        };
                        return TicketsApiViewModel;
                    }());
                    UserDetail.TicketsApiViewModel = TicketsApiViewModel;
                    var NotesViewModel = (function () {
                        function NotesViewModel(links) {
                            this.links = links;
                        }
                        NotesViewModel.prototype.read = function () {
                            $.ajax({
                                type: "GET",
                                url: this.links.UserNotes,
                                success: function (notes) {
                                    $("#notes table tbody tr").remove();
                                    if (notes.length === 0) {
                                        $("#notes #message").show();
                                        $("#notes_table").hide();
                                    }
                                    else {
                                        $("#notes_no_notes_message").hide();
                                        $("#notes_table").show();
                                        $(notes).each(function (i, note) {
                                            $("<tr><td>" + note.DateAdded + "</td><td>" + note.AddedBy + "</td><td>" + note.Body + "</td></tr>")
                                                .appendTo("#notes table tbody");
                                        });
                                    }
                                },
                                error: function (xhr, status, error) {
                                    $("#globalMessage").removeClass().addClass("alert alert-danger").html("Error: " + xhr.responseText).show();
                                }
                            });
                        };
                        NotesViewModel.prototype.save = function () {
                            $("#error").hide();
                            var self = this;
                            $("#accountNoteModal").modal("hide");
                            $.ajax({
                                type: "POST",
                                url: self.links.AddUserNote,
                                data: { addedby: $("#adminusername").val(), body: $("#notebody").val(), userid: $("#userid").val() },
                                success: function () {
                                    self.read();
                                    $("#notebody").val("");
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("Error saving note. Message: " + xhr.statusText, "danger");
                                }
                            });
                        };
                        return NotesViewModel;
                    }());
                    UserDetail.NotesViewModel = NotesViewModel;
                    var DocumentsViewModel = (function () {
                        function DocumentsViewModel(links) {
                            this.links = links;
                        }
                        DocumentsViewModel.prototype.save = function () {
                        };
                        DocumentsViewModel.prototype.read = function () {
                            var self = this;
                            $.ajax({
                                type: "GET",
                                url: this.links.AdminFiles,
                                success: function (files) {
                                    $("#documents table tbody tr").remove();
                                    if (files.length === 0) {
                                        $("#documents #message").show();
                                        $("#documents_table").hide();
                                    }
                                    else {
                                        $("#documents_table").show();
                                        $("#documents #message").hide();
                                        $(files).each(function (i, document) {
                                            $("<tr><td>" + document.DateAdded + "</td><td>" + document.CustomerFileName + "</td><td id=\"" + document.FileId + "\">" + document.Notes + "</td><td style=\"text-align: right;\"><a href=\"#\" class=\"btn btn-danger btn-sm\" style=\"margin-right: 5px;\" onclick=\"documentsViewModel.delete(" + document.FileId + ");\">Delete</a><a href=\"#\" class=\"btn btn-default btn-sm\" style=\"margin-right: 5px;\" onclick=\"documentsViewModel.openAddNoteModal(" + document.FileId + ");\">Add&nbsp;Note</a><a class=\"btn btn-default btn-sm\" href=\"" + self.links.AdminFileDownload + "?fileid=" + document.FileId + "\">Download</a></td></tr>").appendTo("#documents table tbody");
                                        });
                                    }
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("Error: " + xhr.responseText, "danger");
                                }
                            });
                        };
                        DocumentsViewModel.prototype.delete = function (fileId) {
                            var self = this;
                            $("#error").hide();
                            if (confirm("Delete this file?")) {
                                $.ajax({
                                    type: "POST",
                                    url: self.links.AdminFileDelete,
                                    data: { fileid: fileId },
                                    success: function () {
                                        self.read();
                                    }
                                });
                            }
                        };
                        DocumentsViewModel.prototype.openAddNoteModal = function (fileid) {
                            $("#error").hide();
                            $("#documentId").val(fileid);
                            $("#adminFileNoteModal").modal("show");
                        };
                        DocumentsViewModel.prototype.addNote = function () {
                            $("#error").hide();
                            var self = this;
                            $("#adminFileNoteModal").modal("hide");
                            $.ajax({
                                type: "POST",
                                url: this.links.AdminFileAddNote,
                                data: { notes: $("#documentNoteBody").val(), fileid: $("#documentId").val() },
                                success: function () {
                                    self.read();
                                },
                                error: function (xhr, status, error) {
                                    displayMessage("Error: " + xhr.responseText, "danger");
                                }
                            });
                        };
                        return DocumentsViewModel;
                    }());
                    UserDetail.DocumentsViewModel = DocumentsViewModel;
                    var Links = (function () {
                        function Links(data) {
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
                        return Links;
                    }());
                    UserDetail.Links = Links;
                    function displayMessage(message, type) {
                        $("#globalMessage").removeClass().addClass("alert alert-" + type).html(message).show()
                            .fadeTo(7000, 500).slideUp(500, function () { $("#globalMessage").slideUp(500); });
                    }
                })(UserDetail = Clients.UserDetail || (Clients.UserDetail = {}));
            })(Clients = Admin.Clients || (Admin.Clients = {}));
        })(Admin = Websites.Admin || (Websites.Admin = {}));
    })(Websites = AccurateAppend.Websites || (AccurateAppend.Websites = {}));
})(AccurateAppend || (AccurateAppend = {}));
function processingMetricFormatter(records, matches) {
    if (records === 0)
        return "-";
    return kendo.toString(records, "n0") +
        " / " +
        kendo.toString(matches, "n0") +
        " (" +
        Math.floor((matches / records) * 100) +
        "%)";
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiVXNlckRldGFpbC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIlVzZXJEZXRhaWwudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IkFBR0EsaUJBeXlCQztBQXp5QkQsSUFBSSxtQkFBeUYsQ0FBQztBQUM5RixJQUFJLGNBQStFLENBQUM7QUFDcEYsSUFBSSxrQkFBdUYsQ0FBQztBQUM1RixJQUFJLGdCQUFzRixDQUFDO0FBQzNGLElBQUksVUFBa0IsQ0FBQztBQUV2QixDQUFDLENBQUM7SUFFQSxtQkFBbUIsR0FBRyxJQUFJLGNBQWMsQ0FBQyxRQUFRLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxVQUFVLENBQUMsbUJBQW1CLEVBQUUsQ0FBQztJQUNqRyxtQkFBbUIsQ0FBQyxJQUFJLENBQUMsVUFBVSxDQUFDLENBQUM7SUFDckMsbUJBQW1CLENBQUMsNkJBQTZCLEVBQUUsQ0FBQztJQUNwRCxtQkFBbUIsQ0FBQyxrQ0FBa0MsRUFBRSxDQUFDO0lBQ3pELGNBQWMsR0FBRyxJQUFJLGNBQWMsQ0FBQyxRQUFRLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxVQUFVLENBQUMsY0FBYyxDQUFDLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxDQUFDO0lBQ2hILGNBQWMsQ0FBQyxJQUFJLEVBQUUsQ0FBQztJQUN0QixrQkFBa0I7UUFDaEIsSUFBSSxjQUFjLENBQUMsUUFBUSxDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsVUFBVSxDQUFDLGtCQUFrQixDQUFDLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxDQUFDO0lBQ3JHLGtCQUFrQixDQUFDLElBQUksRUFBRSxDQUFDO0lBQzFCLGdCQUFnQjtRQUNkLElBQUksY0FBYyxDQUFDLFFBQVEsQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLFVBQVUsQ0FBQyxtQkFBbUIsQ0FBQyxtQkFBbUIsQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUN0RyxnQkFBZ0IsQ0FBQyxJQUFJLEVBQUUsQ0FBQztJQUd4QixDQUFDLENBQUMsZ0JBQWdCLENBQUMsQ0FBQyxLQUFLLENBQUMsY0FBUSxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUMzRSxDQUFDLENBQUMsb0JBQW9CLENBQUMsQ0FBQyxLQUFLLENBQUMsY0FBUSxDQUFDLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUNsRixDQUFDLENBQUMsWUFBWSxDQUFDLENBQUMsTUFBTSxDQUFDLGNBQVEsbUJBQW1CLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUM5RCxDQUFDLENBQUMsWUFBWSxDQUFDLENBQUMsTUFBTSxDQUFDLGNBQVEsbUJBQW1CLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUM5RCxDQUFDLENBQUMsVUFBVSxDQUFDLENBQUMsTUFBTSxDQUFDLGNBQVEsbUJBQW1CLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUM1RCxDQUFDLENBQUMsY0FBYyxDQUFDLENBQUMsTUFBTSxDQUFDLGNBQVEsbUJBQW1CLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUNoRSxDQUFDLENBQUMsd0JBQXdCLENBQUMsQ0FBQyxLQUFLLENBQUM7UUFDaEMsQ0FBQyxDQUFDLFNBQVMsQ0FBQyxDQUFDLE1BQU0sRUFBRSxDQUFDO1FBQ3RCLFFBQVEsQ0FBQyxXQUFXLENBQUMsTUFBTSxDQUFDLENBQUM7SUFDL0IsQ0FBQyxDQUFDLENBQUM7SUFHSCxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUNyRSxDQUFDLENBQUMscUJBQXFCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztJQUN6RSxDQUFDLENBQUMscUJBQXFCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztJQUN6RSxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUNyRSxDQUFDLENBQUMscUJBQXFCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQztJQUN6RSxDQUFDLENBQUMsa0JBQWtCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQztJQUNuRSxDQUFDLENBQUMseUJBQXlCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxXQUFXLENBQUMsQ0FBQztJQUNqRixDQUFDLENBQUMsb0NBQW9DLENBQUMsQ0FBQyxLQUFLLENBQUMsY0FBUSxDQUFDLENBQUMsaUNBQWlDLENBQUMsQ0FBQyxLQUFLLENBQUMsTUFBTSxDQUFDLENBQUMsQ0FBQSxDQUFDLENBQUMsQ0FBQztJQUM1RyxDQUFDLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxRQUFRLENBQUMsQ0FBQztJQUMzRSxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxLQUFLLENBQUMsQ0FBQztJQUNyRSxDQUFDLENBQUMsc0JBQXNCLENBQUMsQ0FBQyxJQUFJLENBQUMsTUFBTSxFQUFFLG1CQUFtQixDQUFDLEtBQUssQ0FBQyxRQUFRLENBQUMsQ0FBQztJQUMzRSxDQUFDLENBQUMsZ0NBQWdDLENBQUMsQ0FBQyxJQUFJLENB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R0FBTCxjQUFLLEtBQUwsY0FBSyxRQXlzQm5DO0lBQUQsQ0FBQyxFQXpzQnFCLFFBQVEsR0FBUix1QkFBUSxLQUFSLHVCQUFRLFFBeXNCN0I7QUFBRCxDQUFDLEVBenNCTSxjQUFjLEtBQWQsY0FBYyxRQXlzQnBCO0FBRUQsU0FBUyx5QkFBeUIsQ0FBQyxPQUFlLEVBQUUsT0FBZTtJQUNqRSxJQUFJLE9BQU8sS0FBSyxDQUFDO1FBQUUsT0FBTyxHQUFHLENBQUM7SUFDOUIsT0FBTyxLQUFLLENBQUMsUUFBUSxDQUFDLE9BQU8sRUFBRSxJQUFJLENBQUM7UUFDbEMsS0FBSztRQUNMLEtBQUssQ0FBQyxRQUFRLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQztRQUM3QixJQUFJO1FBQ0osSUFBSSxDQUFDLEtBQUssQ0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsR0FBRyxHQUFHLENBQUM7UUFDckMsSUFBSSxDQUFDO0FBQ1QsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3MvbW9tZW50L21vbWVudC5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuXHJcbnZhciB1c2VyRGV0YWlsVmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5DbGllbnRzLlVzZXJEZXRhaWwuVXNlckRldGFpbFZpZXdNb2RlbDtcclxudmFyIG5vdGVzVmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5DbGllbnRzLlVzZXJEZXRhaWwuTm90ZXNWaWV3TW9kZWw7XHJcbnZhciBkb2N1bWVudHNWaWV3TW9kZWw6IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkFkbWluLkNsaWVudHMuVXNlckRldGFpbC5Eb2N1bWVudHNWaWV3TW9kZWw7XHJcbnZhciB0aWNrZXRzVmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5DbGllbnRzLlVzZXJEZXRhaWwuVGlja2V0c0FwaVZpZXdNb2RlbDtcclxudmFyIGFkbWluVXNlcnM6IHN0cmluZztcclxuXHJcbiQoKCkgPT4ge1xyXG5cclxuICB1c2VyRGV0YWlsVmlld01vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkFkbWluLkNsaWVudHMuVXNlckRldGFpbC5Vc2VyRGV0YWlsVmlld01vZGVsKCk7XHJcbiAgdXNlckRldGFpbFZpZXdNb2RlbC5pbml0KGFkbWluVXNlcnMpO1xyXG4gIHVzZXJEZXRhaWxWaWV3TW9kZWwubG9hZFVzZXJPcGVyYXRpbmdNZXRyaWNSZXBvcnQoKTtcclxuICB1c2VyRGV0YWlsVmlld01vZGVsLmxvYWRVc2VyUHJvZHVjdFVzYWdlT3ZlcnZpZXdSZXBvcnQoKTtcclxuICBub3Rlc1ZpZXdNb2RlbCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5XZWJzaXRlcy5BZG1pbi5DbGllbnRzLlVzZXJEZXRhaWwuTm90ZXNWaWV3TW9kZWwodXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcyk7XHJcbiAgbm90ZXNWaWV3TW9kZWwucmVhZCgpO1xyXG4gIGRvY3VtZW50c1ZpZXdNb2RlbCA9XHJcbiAgICBuZXcgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQWRtaW4uQ2xpZW50cy5Vc2VyRGV0YWlsLkRvY3VtZW50c1ZpZXdNb2RlbCh1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzKTtcclxuICBkb2N1bWVudHNWaWV3TW9kZWwucmVhZCgpO1xyXG4gIHRpY2tldHNWaWV3TW9kZWwgPVxyXG4gICAgbmV3IEFjY3VyYXRlQXBwZW5kLldlYnNpdGVzLkFkbWluLkNsaWVudHMuVXNlckRldGFpbC5UaWNrZXRzQXBpVmlld01vZGVsKHVzZXJEZXRhaWxWaWV3TW9kZWwuTGlua3MpO1xyXG4gIHRpY2tldHNWaWV3TW9kZWwucmVhZCgpO1xyXG5cclxuICAvLyBjbGljayBoYW5kbGVyc1xyXG4gICQoXCIjYWRkTm90ZUJ1dHRvblwiKS5jbGljaygoKSA9PiB7ICQoXCIjYWNjb3VudE5vdGVNb2RhbFwiKS5tb2RhbChcInNob3dcIik7IH0pO1xyXG4gICQoXCIjYWRkRG9jdW1lbnRCdXR0b25cIikuY2xpY2soKCkgPT4geyAkKFwiI2RvY3VtZW50VXBsb2FkTW9kYWxcIikubW9kYWwoXCJzaG93XCIpOyB9KTtcclxuICAkKFwiI1N0b3JlRGF0YVwiKS5jaGFuZ2UoKCkgPT4geyB1c2VyRGV0YWlsVmlld01vZGVsLnNhdmUoKTsgfSk7XHJcbiAgJChcIiNiYXRjaFVzZXJcIikuY2hhbmdlKCgpID0+IHsgdXNlckRldGFpbFZpZXdNb2RlbC5zYXZlKCk7IH0pO1xyXG4gICQoXCIjeG1sVXNlclwiKS5jaGFuZ2UoKCkgPT4geyB1c2VyRGV0YWlsVmlld01vZGVsLnNhdmUoKTsgfSk7XHJcbiAgJChcIiNJc0xvY2tlZE91dFwiKS5jaGFuZ2UoKCkgPT4geyB1c2VyRGV0YWlsVmlld01vZGVsLnNhdmUoKTsgfSk7XHJcbiAgJChcIiNjb3B5VXNlcklkVG9DbGlwYm9hcmRcIikuY2xpY2soKCkgPT4ge1xyXG4gICAgJChcIiN1c2VyaWRcIikuc2VsZWN0KCk7XHJcbiAgICBkb2N1bWVudC5leGVjQ29tbWFuZChcImNvcHlcIik7XHJcbiAgfSk7XHJcblxyXG4gIC8vIGJ1dHRvbiBiYXIgaGFuZGxlcnNcclxuICAkKFwiI2J1dHRvbkJhciAjQ2FyZHNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5DYXJkcyk7XHJcbiAgJChcIiNidXR0b25CYXIgI0NoYXJnZXNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5DaGFyZ2VzKTtcclxuICAkKFwiI2J1dHRvbkJhciAjTmV3RGVhbFwiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLk5ld0RlYWwpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNEZWFsc1wiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLkRlYWxzKTtcclxuICAkKFwiI2J1dHRvbkJhciAjSm9ic05ld1wiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLkpvYnNOZXcpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNKb2JzXCIpLmF0dHIoXCJocmVmXCIsIHVzZXJEZXRhaWxWaWV3TW9kZWwuTGlua3MuSm9icyk7XHJcbiAgJChcIiNidXR0b25CYXIgI0xvZ0luQXNVc2VyXCIpLmF0dHIoXCJocmVmXCIsIHVzZXJEZXRhaWxWaWV3TW9kZWwuTGlua3MuTG9nSW5Bc1VzZXIpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNVc2VyTXVzdENoYW5nZVBhc3N3b3JkXCIpLmNsaWNrKCgpID0+IHsgJChcIiNjb25maXJtYXRpb25Gb3JjZVBhc3N3b3JkUmVzZXRcIikubW9kYWwoXCJzaG93XCIpO30pO1xyXG4gICQoXCIjYnV0dG9uQmFyICNDb250YWN0c1wiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLkNvbnRhY3RzKTtcclxuICAkKFwiI2J1dHRvbkJhciAjRmlsZXNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5GaWxlcyk7XHJcbiAgJChcIiNidXR0b25CYXIgI01lc3NhZ2VzXCIpLmF0dHIoXCJocmVmXCIsIHVzZXJEZXRhaWxWaWV3TW9kZWwuTGlua3MuTWVzc2FnZXMpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNBdXRvUHJvY2Vzc29yUnVsZXNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5BdXRvUHJvY2Vzc29yUnVsZXMpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNSYXRlQ2FyZHNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5SYXRlQ2FyZHMpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNTZXJ2aWNlQWNjb3VudHNcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5TZXJ2aWNlQWNjb3VudHMpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNBUElSZXBvcnRpbmdcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5BUElSZXBvcnRpbmcpO1xyXG4gICQoXCIjYnV0dG9uQmFyICNEb3dubG9hZFVzYWdlXCIpLmNsaWNrKCgpID0+IHsgJChcIiNkb3dubG9hZFVzYWdlTW9kYWxcIikubW9kYWwoXCJzaG93XCIpOyB9KTtcclxuICAkKFwiI2J1dHRvbkJhciAjQ29weVBheW1lbnRMaW5rVG9DbGlwYm9hcmRcIikuY2xpY2soKCkgPT4ge1xyXG4gICAgJChcIiNjbGlwYm9hcmRcIikudmFsKHVzZXJEZXRhaWxWaWV3TW9kZWwuTGlua3MuUGF5bWVudFVwZGF0ZUxpbmspLnNlbGVjdCgpO1xyXG4gICAgZG9jdW1lbnQuZXhlY0NvbW1hbmQoXCJjb3B5XCIpO1xyXG4gIH0pO1xyXG4gICQoXCIjYnV0dG9uQmFyICNWaWV3TmF0aW9uc1wiKS5jbGljaygoKSA9PiB7IHVzZXJEZXRhaWxWaWV3TW9kZWwuZGlzcGxheU5hdGlvbnMoKTsgfSk7XHJcbiAgJChcIiNidXR0b25CYXIgI1NvdXJjZUxlYWRcIikuYXR0cihcImhyZWZcIiwgdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5Tb3VyY2VMZWFkKTtcclxuICAkKFwiI2NyZWF0ZVRpY2tldFwiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLkNyZWF0ZVRpY2tldCk7XHJcbiAgJChcIiN4bWxVc2VyXCIpLmNoYW5nZSgoKSA9PiB7XHJcbiAgICBpZiAodGhpcy5jaGVja2VkKSBhbGVydChcIkVuc3VyZSB0aGUgdXNlciBoYXMgYSB2YWxpZCBzdWJzY3JpcHRpb24gYmVmb3JlIGVuYWJsaW5nIEFQSSBhY2Nlc3MuXCIpO1xyXG4gIH0pO1xyXG4gICQoXCIjYWNjb3VudE93bmVyXCIpLmNoYW5nZSgoKSA9PiB7XHJcbiAgICBjb25zdCBwdWJsaWNLZXkgPSAkKFwiI3B1YmxpY0tleVwiKS52YWwoKTtcclxuICAgIGNvbnN0IG93bmVySWQgPSAkKFwiI2FjY291bnRPd25lclwiKS52YWwoKTtcclxuICAgIHVzZXJEZXRhaWxWaWV3TW9kZWwudXBkYXRlT3duZXIocHVibGljS2V5LCBvd25lcklkKTtcclxuICB9KTtcclxuXHJcbiAgLy8gb3RoZXJcclxuICAkKFwiI2J0bkZvcmNlUGFzc3dvcmRSZXNldFwiKS5hdHRyKFwiaHJlZlwiLCB1c2VyRGV0YWlsVmlld01vZGVsLkxpbmtzLlVzZXJNdXN0Q2hhbmdlUGFzc3dvcmQpO1xyXG5cclxuICAvLyBpbml0aWFsaXplIGFkbWluIGRvY3VtZW50IHVwbG9hZCBjb250cm9sXHJcbiAgJChcIiNhZG1pbkZpbGVzVG9VcGxvYWRcIikua2VuZG9VcGxvYWQoe1xyXG4gICAgYXN5bmM6IHtcclxuICAgICAgc2F2ZVVybDogdXNlckRldGFpbFZpZXdNb2RlbC5MaW5rcy5BZG1pbkZpbGVVcGxvYWQsXHJcbiAgICAgIGF1dG9VcGxvYWQ6IHRydWUudmFsdWVPZigpLFxyXG4gICAgICB3aXRoQ3JlZGVudGlhbHM6IGZhbHNlXHJcbiAgICB9LFxyXG4gICAgY29tcGxldGUoKSB7XHJcbiAgICAgICQoXCIjZG9jdW1lbnRVcGxvYWRNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICQoXCIuay11cGxvYWQtZmlsZXMgbGlcIikucmVtb3ZlKCk7XHJcbiAgICAgICQoXCIuay11cGxvYWQtc3RhdHVzXCIpLnJlbW92ZSgpO1xyXG4gICAgICBkb2N1bWVudHNWaWV3TW9kZWwucmVhZCgpO1xyXG4gICAgfVxyXG4gIH0pO1xyXG5cclxufSk7XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuV2Vic2l0ZXMuQWRtaW4uQ2xpZW50cy5Vc2VyRGV0YWlsIHtcclxuXHJcbiAgZXhwb3J0IGNsYXNzIFVzZXJEZXRhaWxWaWV3TW9kZWwge1xyXG5cclxuICAgIExpbmtzOiBMaW5rcztcclxuXHJcbiAgICBpbml0KGFkbWluVXNlcnM6IGFueSkge1xyXG5cclxuICAgICAgJChcIiNlcnJvclwiKS5oaWRlKCk7XHJcbiAgICAgIHZhciBzZWxmID0gdGhpcztcclxuICAgICAgJC5hamF4KFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICBjb250ZXh0OiB0aGlzLFxyXG4gICAgICAgICAgdXJsOiAkKFwiaW5wdXRbdHlwZT1oaWRkZW5dW2lkPXVzZXJEZXRhaWxVcmldXCIpLnZhbCgpLFxyXG4gICAgICAgICAgYXN5bmM6IGZhbHNlLFxyXG4gICAgICAgICAgc3VjY2VzcyhkYXRhKSB7XHJcbiAgICAgICAgICAgICQoXCIjRGF0ZUFkZGVkXCIpLnRleHQoZGF0YS5EYXRlQWRkZWQpO1xyXG4gICAgICAgICAgICAkKFwiI0xhc3RBY3Rpdml0eURhdGVcIikudGV4dChgTGFzdDogJHtkYXRhLkxhc3RBY3Rpdml0eURhdGV9YCk7XHJcbiAgICAgICAgICAgICQoXCIjRW1haWxcIikudmFsKGRhdGEuRW1haWwpO1xyXG4gICAgICAgICAgICAkKFwiI0J1c2luZXNzTmFtZVwiKS52YWwoZGF0YS5CdXNpbmVzc05hbWUpO1xyXG4gICAgICAgICAgICAkKFwiI0ZpcnN0TmFtZVwiKS52YWwoZGF0YS5GaXJzdE5hbWUpO1xyXG4gICAgICAgICAgICAkKFwiI0xhc3ROYW1lXCIpLnZhbChkYXRhLkxhc3ROYW1lKTtcclxuICAgICAgICAgICAgJChcIiNBZGRyZXNzXCIpLnZhbChkYXRhLkFkZHJlc3MpO1xyXG4gICAgICAgICAgICAkKFwiI0NpdHlcIikudmFsKGRhdGEuQ2l0eSk7XHJcbiAgICAgICAgICAgICQoXCIjU3RhdGVcIikudmFsKGRhdGEuU3RhdGUpO1xyXG4gICAgICAgICAgICAkKFwiI1ppcFwiKS52YWwoZGF0YS5aaXApO1xyXG4gICAgICAgICAgICAkKFwiI0VtYWlsXCIpLnZhbChkYXRhLkVtYWlsKTtcclxuICAgICAgICAgICAgJChcIiNQaG9uZVwiKS52YWwoZGF0YS5QaG9uZSk7XHJcbiAgICAgICAgICAgICQoXCIjdXNlcmlkXCIpLnZhbChkYXRhLlVzZXJJZCk7XHJcbiAgICAgICAgICAgIC8vIGluaXRpYWxpemUgc2FsZXMgcmVwIGRyb3AgZG93blxyXG4gICAgICAgICAgICB0aGlzLmluaXRBZG1pblVzZXJzU2VsZWN0KGFkbWluVXNlcnMsIGRhdGEuUHVibGljS2V5KTtcclxuICAgICAgICAgICAgJChcIiNhY2NvdW50T3duZXJcIikudmFsKGRhdGEuT3duZXJJZCk7XHJcbiAgICAgICAgICAgIC8vIHNldCB2YWx1ZXMgZm9yIGJvb2wgcHJvcGVydGllc1xyXG4gICAgICAgICAgICAkKFwiI0lzTG9ja2VkT3V0XCIpLnByb3AoXCJjaGVja2VkXCIsIGRhdGEuSXNMb2NrZWRPdXQpLmNoYW5nZSgpO1xyXG4gICAgICAgICAgICAkKFwiI2JhdGNoVXNlclwiKS5wcm9wKFwiY2hlY2tlZFwiLCBkYXRhLmJhdGNoVXNlcikuY2hhbmdlKCk7XHJcbiAgICAgICAgICAgICQoXCIjeG1sVXNlclwiKS5wcm9wKFwiY2hlY2tlZFwiLCBkYXRhLnhtbFVzZXIpLmNoYW5nZSgpO1xyXG4gICAgICAgICAgICAkKFwiI1N0b3JlRGF0YVwiKS5wcm9wKFwiY2hlY2tlZFwiLCBkYXRhLlN0b3JlRGF0YSkuY2hhbmdlKCk7XHJcblxyXG4gICAgICAgICAgICAvLyBsb2FkIGxpbmtzXHJcbiAgICAgICAgICAgIHNlbGYuTGlua3MgPSBuZXcgTGlua3MoZGF0YSk7XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyLCBzdGF0dXMsIGVycm9yKSB7XHJcbiAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGA8c3Ryb25nPkVycm9yIHJldHJpZXZpbmcgdXNlci4gPC9zdHJvbmc+JHt4aHIucmVzcG9uc2VUZXh0fWAsIFwiZGFuZ2VyXCIpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgICAgaW5pdEFkbWluVXNlcnNTZWxlY3QoYWRtaW5Vc2VyczogYW55LCBwdWJsaWNLZXk6IHN0cmluZykge1xyXG4gICAgICAkKFwiI3B1YmxpY0tleVwiKS52YWwocHVibGljS2V5KTtcclxuICAgICAgJC5lYWNoKGFkbWluVXNlcnMsXHJcbiAgICAgICAgKGtleSwgdmFsdWUpID0+IHtcclxuICAgICAgICAgICQoXCIjYWNjb3VudE93bmVyXCIpXHJcbiAgICAgICAgICAgIC5hcHBlbmQoJChcIjxvcHRpb24+PC9vcHRpb24+XCIpXHJcbiAgICAgICAgICAgICAgLmF0dHIoXCJ2YWx1ZVwiLCB2YWx1ZS5Vc2VySWQpXHJcbiAgICAgICAgICAgICAgLnRleHQodmFsdWUuVXNlck5hbWUpKTtcclxuICAgICAgICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBzYXZlKCkge1xyXG4gICAgICAkKFwiI2Vycm9yXCIpLmhpZGUoKTtcclxuICAgICAgJC5hamF4KHtcclxuICAgICAgICB0eXBlOiBcIlBPU1RcIixcclxuICAgICAgICB1cmw6IHRoaXMuTGlua3MuRWRpdCxcclxuICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICBlbWFpbDogJChcIiNFbWFpbFwiKS52YWwoKSxcclxuICAgICAgICAgIGZpcnN0bmFtZTogJChcIiNGaXJzdE5hbWVcIikudmFsKCksXHJcbiAgICAgICAgICBsYXN0bmFtZTogJChcIiNMYXN0TmFtZVwiKS52YWwoKSxcclxuICAgICAgICAgIGJ1c2luZXNzbmFtZTogJChcIiNCdXNpbmVzc05hbWVcIikudmFsKCksXHJcbiAgICAgICAgICBhZGRyZXNzOiAkKFwiI0FkZHJlc3NcIikudmFsKCksXHJcbiAgICAgICAgICBjaXR5OiAkKFwiI0NpdHlcIikudmFsKCksXHJcbiAgICAgICAgICBzdGF0ZTogJChcIiNTdGF0ZVwiKS52YWwoKSxcclxuICAgICAgICAgIHppcDogJChcIiNaaXBcIikudmFsKCksXHJcbiAgICAgICAgICBwaG9uZTogJChcIiNQaG9uZVwiKS52YWwoKSxcclxuICAgICAgICAgIGRlZmF1bHRwcm9kdWN0OiBcIlwiLFxyXG4gICAgICAgICAgZGVmYXVsdGNvbHVtbm1hcDogXCJcIixcclxuICAgICAgICAgIHVzZXJpZDogJChcIiN1c2VyaWRcIikudmFsKCksXHJcbiAgICAgICAgICBpc2xvY2tlZG91dDogJChcIiNJc0xvY2tlZE91dFwiKS5wcm9wKFwiY2hlY2tlZFwiKSxcclxuICAgICAgICAgIGJhdGNoVXNlcjogJChcIiNiYXRjaFVzZXJcIikucHJvcChcImNoZWNrZWRcIiksXHJcbiAgICAgICAgICB4bWxVc2VyOiAkKFwiI3htbFVzZXJcIikucHJvcChcImNoZWNrZWRcIiksXHJcbiAgICAgICAgICBzdG9yZURhdGE6ICQoXCIjU3RvcmVEYXRhXCIpLnByb3AoXCJjaGVja2VkXCIpXHJcbiAgICAgICAgfSxcclxuICAgICAgICBzdWNjZXNzKCkge1xyXG4gICAgICAgICAgZGlzcGxheU1lc3NhZ2UoYFVzZXIgc3VjY2Vzc2Z1bGx5IHVwZGF0ZWRgLCBcInN1Y2Nlc3NcIik7XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGA8c3Ryb25nPkVycm9yIHVwZGF0aW5nIHVzZXIuIDwvc3Ryb25nPiR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHVwZGF0ZU93bmVyKHB1YmxpY0tleTogc3RyaW5nLCBvd25lcklkOiBzdHJpbmcpIHtcclxuICAgICAgJC5hamF4KHtcclxuICAgICAgICB0eXBlOiBcIlBPU1RcIixcclxuICAgICAgICB1cmw6IHRoaXMuTGlua3MuQ2hhbmdlT3duZXIsXHJcbiAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgSWQ6IHB1YmxpY0tleSxcclxuICAgICAgICAgIE93bmVySWQ6IG93bmVySWRcclxuICAgICAgICB9LFxyXG4gICAgICAgIHN1Y2Nlc3MoZGF0YSkge1xyXG4gICAgICAgICAgZGlzcGxheU1lc3NhZ2UoZGF0YS5NZXNzYWdlLCBcInN1Y2Nlc3NcIik7XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgICAgZGlzcGxheU1lc3NhZ2UoeGhyLnJlc3BvbnNlVGV4dCwgXCJkYW5nZXJcIik7XHJcbiAgICAgICAgfVxyXG4gICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBkb3dubG9hZFVzYWdlKCkge1xyXG4gICAgICAkKFwiI2Rvd25sb2FkVXNhZ2VNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKFxyXG4gICAgICAgIGAke3RoaXMuTGlua3MuRG93bmxvYWRVc2FnZX0mc3RhcnQ9JHttb21lbnQoJChcIiN1c2FnZURhdGVSYW5nZVdpZGdldF9zdGFydERhdGVcIikudmFsKCkpLmZvcm1hdChcIllZWVktTU0tRERcIilcclxuICAgICAgICB9JmVuZD0ke1xyXG4gICAgICAgIG1vbWVudCgkKFwiI3VzYWdlRGF0ZVJhbmdlV2lkZ2V0X2VuZERhdGVcIikudmFsKCkpLmZvcm1hdChcIllZWVktTU0tRERcIil9YCk7XHJcbiAgICB9XHJcblxyXG4gICAgc2F2ZVVzYWdlKCkge1xyXG4gICAgICAkKFwiI2Rvd25sb2FkVXNhZ2VNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICQuYWpheChcclxuICAgICAgICB7XHJcbiAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgdXJsOiBgJHt0aGlzLkxpbmtzLlNhdmVVc2FnZX0mc3RhcnQ9JHttb21lbnQoJChcIiN1c2FnZURhdGVSYW5nZVdpZGdldF9zdGFydERhdGVcIikudmFsKCkpLmZvcm1hdChcIllZWVktTU0tRERcIilcclxuICAgICAgICAgICAgfSZlbmQ9JHtcclxuICAgICAgICAgICAgbW9tZW50KCQoXCIjdXNhZ2VEYXRlUmFuZ2VXaWRnZXRfZW5kRGF0ZVwiKS52YWwoKSkuZm9ybWF0KFwiWVlZWS1NTS1ERFwiKX1gLFxyXG4gICAgICAgICAgYXN5bmM6IHRydWUsXHJcbiAgICAgICAgICBzdWNjZXNzKGRhdGEpIHtcclxuICAgICAgICAgICAgaWYgKGRhdGEuc3VjY2VzcylcclxuICAgICAgICAgICAgICBkaXNwbGF5TWVzc2FnZShkYXRhLm1lc3NhZ2UsIFwic3VjY2Vzc1wiKTtcclxuICAgICAgICAgICAgZWxzZVxyXG4gICAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGA8c3Ryb25nPkVycm9yIHNhdmluZyB1c2FnZSByZXBvcnQgdG8gdXNlciBmaWxlcy4gPC9zdHJvbmc+YCwgXCJkYW5nZXJcIik7XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyLCBzdGF0dXMsIGVycm9yKSB7XHJcbiAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGA8c3Ryb25nPkVycm9yIHNhdmUgdXNhZ2UgcmVwb3J0IHRvIHVzZXIgZmlsZXMuIDwvc3Ryb25nPiR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBkaXNwbGF5TmF0aW9ucygpIHtcclxuICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQoXCIjbmF0aW9uQnVpbGRlclJlZ2lzdHJhdGlvbnNcIikubW9kYWwoXCJzaG93XCIpO1xyXG4gICAgICAkLmFqYXgoXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgIHVybDogc2VsZi5MaW5rcy5WaWV3TmF0aW9ucyxcclxuICAgICAgICAgIHN1Y2Nlc3MocmVnaXN0cmF0aW9ucykge1xyXG4gICAgICAgICAgICBpZiAocmVnaXN0cmF0aW9ucy5sZW5ndGggPT09IDApIHtcclxuICAgICAgICAgICAgICAkKFwiI25hdGlvbnNNZXNzYWdlXCIpLnRleHQoXCJObyBOYXRpb25zIGZvdW5kLlwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgJChcIiNuYXRpb25zIHRib2R5IHRyXCIpLnJlbW92ZSgpO1xyXG4gICAgICAgICAgICAgICQocmVnaXN0cmF0aW9ucykuZWFjaCgoaSwgcjogYW55KSA9PiB7XHJcbiAgICAgICAgICAgICAgICAkKGA8dHI+PHRkPiR7ci5TbHVnfTwvdGQ+PHRkPjxpbnB1dCB0eXBlPVwidGV4dFwiIHNpemU9NDUgc3R5bGU9XCJib3JkZXI6IDBcIiB2YWx1ZT1cIiR7ci5BY2Nlc3NUb2tlblxyXG4gICAgICAgICAgICAgICAgICB9XCIgb25jbGljaz1cIiQodGhpcykuc2VsZWN0KClcIi8+PC90ZD48L3RyPmApLmFwcGVuZFRvKCQoXCIjbmF0aW9ucyB0Ym9keVwiKSk7XHJcbiAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgICAgZGlzcGxheU1lc3NhZ2UoYEVycm9yIHVwZGF0aW5nIHVzZXIuIE1lc3NhZ2U6ICR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBsb2FkVXNlck9wZXJhdGluZ01ldHJpY1JlcG9ydCgpIHtcclxuICAgICAgdmFyIHNlbGYgPSB0aGlzO1xyXG4gICAgICBjb25zdCBncmlkID0gJChcIiNVc2VyT3BlcmF0aW5nTWV0cmljc092ZXJ2aWV3R3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgICQoXCIjVXNlck9wZXJhdGluZ01ldHJpY3NPdmVydmlld0dyaWRcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgIHJlYWQob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgdXJsOiBzZWxmLkxpbmtzLlVzZXJPcGVyYXRpbmdNZXRyaWMsXHJcbiAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICBjYWNoZTogZmFsc2VcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgc2NoZW1hOiB7XHJcbiAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgdG90YWwocmVzcG9uc2UpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJNZXRyaWNOYW1lRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJUb2RheVwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiN0b2RheVRlbXBsYXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkxhc3Q3XCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCA3XCIsXHJcbiAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI2xhc3Q3VGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiQ3VycmVudE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiQ3VycmVudCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNjdXJyZW50TW9udGhUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJTYW1lUGVyaW9kTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU2FtZSBQZXJpb2QgTGFzdCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNzYW1lUGVyaW9kTGFzdE1vbnRoVGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNMYXN0TW9udGhUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJQcmV2aW91c1RvTGFzdE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiUHJldmlvdXMgVG8gTGFzdCBNb250aFwiLFxyXG4gICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNwcmV2aW91c1RvTGFzdE1vbnRoVGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI29wZXJhdGluZy1tZXRyaWNzLXJlc3BvbnNpdmUtdGVtcGxhdGVcIikuaHRtbCgpKSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWF4LXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICBdLFxyXG4gICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2VcclxuICAgICAgICB9KTtcclxuICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGxvYWRVc2VyUHJvZHVjdFVzYWdlT3ZlcnZpZXdSZXBvcnQoKSB7XHJcbiAgICAgIHZhciBzZWxmID0gdGhpcztcclxuICAgICAgY29uc3QgZ3JpZCA9ICQoXCIjVXNlclByb2R1Y3RVc2FnZU92ZXJ2aWV3R3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICB9IGVsc2Uge1xyXG4gICAgICAgICQoXCIjVXNlclByb2R1Y3RVc2FnZU92ZXJ2aWV3R3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICB1cmw6IHNlbGYuTGlua3MuVXNlclByb2R1Y3RVc2FnZU1ldHJpYyxcclxuICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIGNoYW5nZTogZnVuY3Rpb24oKSB7XHJcbiAgICAgICAgICAgICAgaWYgKHRoaXMuZGF0YSgpLmxlbmd0aCA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiI1VzZXJQcm9kdWN0VXNhZ2VPdmVydmlld0dyaWRNZXNzYWdlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjVXNlclByb2R1Y3RVc2FnZU92ZXJ2aWV3R3JpZFwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoXCIjVXNlclByb2R1Y3RVc2FnZU92ZXJ2aWV3R3JpZE1lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNVc2VyUHJvZHVjdFVzYWdlT3ZlcnZpZXdHcmlkXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgIHRvdGFsKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICBtb2RlbDoge1xyXG4gICAgICAgICAgICAgICAgZmllbGRzOiB7XHJcbiAgICAgICAgICAgICAgICAgIE1ldHJpY05hbWVEZXNjcmlwdGlvbjogeyB0eXBlOiBcInN0cmluZ1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgIE1ldHJpY05hbWU6IHsgdHlwZTogXCJzdHJpbmdcIiB9LFxyXG4gICAgICAgICAgICAgICAgICBUb2RheTogeyB0eXBlOiBcIm51bWJlclwiIH0sXHJcbiAgICAgICAgICAgICAgICAgIFllc3RlcmRheTogeyB0eXBlOiBcIm51bWJlclwiIH0sXHJcbiAgICAgICAgICAgICAgICAgIExhc3Q3OiB7IHR5cGU6IFwibnVtYmVyXCIgfSxcclxuICAgICAgICAgICAgICAgICAgTGFzdDMwOiB7IHR5cGU6IFwibnVtYmVyXCIgfSxcclxuICAgICAgICAgICAgICAgICAgTGFzdDYwOiB7IHR5cGU6IFwibnVtYmVyXCIgfSxcclxuICAgICAgICAgICAgICAgICAgTGFzdDkwOiB7IHR5cGU6IFwibnVtYmVyXCIgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgeyBmaWVsZDogXCJPcGVyYXRpb25cIiwgdGl0bGU6IFwiRGVzY3JpcHRpb25cIiB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiVG9kYXlcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJUb2RheVwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZShcIiM9IHByb2Nlc3NpbmdNZXRyaWNGb3JtYXR0ZXIoVG9kYXlSZWNvcmRzLFRvZGF5TWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdDdcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJMYXN0IDdcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBwcm9jZXNzaW5nTWV0cmljRm9ybWF0dGVyKExhc3Q3UmVjb3JkcyxMYXN0N01hdGNoZXMpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkN1cnJlbnRNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkN1cnJlbnQgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBwcm9jZXNzaW5nTWV0cmljRm9ybWF0dGVyKEN1cnJlbnRNb250aFJlY29yZHMsQ3VycmVudE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiU2FtZVBlcmlvZExhc3RNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlNhbWUgUGVyaW9kIExhc3QgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXHJcbiAgICAgICAgICAgICAgICBcIiM9IHByb2Nlc3NpbmdNZXRyaWNGb3JtYXR0ZXIoU2FtZVBlcmlvZExhc3RNb250aFJlY29yZHMsU2FtZVBlcmlvZExhc3RNb250aE1hdGNoZXMpICNcIiksXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOnJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIkxhc3RNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgTW9udGhcIixcclxuICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoXCIjPSBwcm9jZXNzaW5nTWV0cmljRm9ybWF0dGVyKExhc3RNb250aFJlY29yZHMsTGFzdE1vbnRoTWF0Y2hlcykgI1wiKSxcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246cmlnaHQ7XCIgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiUHJldmlvdXNUb0xhc3RNb250aFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlByZXZpb3VzIE1vbnRoXCIsXHJcbiAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKFxyXG4gICAgICAgICAgICAgICAgXCIjPSBwcm9jZXNzaW5nTWV0cmljRm9ybWF0dGVyKFByZXZpb3VzVG9MYXN0TW9udGhSZWNvcmRzLFByZXZpb3VzVG9MYXN0TW9udGhNYXRjaGVzKSAjXCIpLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjpyaWdodDtcIiB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIF1cclxuICAgICAgICB9KTtcclxuICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIGZvcmNlUGFzc3dvcmRDaGFuZ2UoKSB7XHJcbiAgICAgICQuYWpheCh7XHJcbiAgICAgICAgdHlwZTogXCJHRVRcIixcclxuICAgICAgICB1cmw6IHRoaXMuTGlua3MuVXNlck11c3RDaGFuZ2VQYXNzd29yZCxcclxuICAgICAgICBzdWNjZXNzKCkge1xyXG4gICAgICAgICAgJChcIiNjb25maXJtYXRpb25Gb3JjZVBhc3N3b3JkUmVzZXRcIikubW9kYWwoXCJoaWRlXCIpO1xyXG4gICAgICAgICAgZGlzcGxheU1lc3NhZ2UoYFVzZXIgd2lsbCBiZSBmb3JjZWQgdG8gY2hhbmdlIHRoZWlyIHBhc3N3b3JkIHRoZSBuZXh0IHRpbWUgdGhleSBsb2dpbi5gLCBcInN1Y2Nlc3NcIik7XHJcbiAgICAgICAgfSxcclxuICAgICAgICBlcnJvcih4aHIsIHN0YXR1cywgZXJyb3IpIHtcclxuICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGA8c3Ryb25nPkVycm9yIHVwZGF0aW5nIHVzZXIuIDwvc3Ryb25nPiR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICB9XHJcbiAgICAgIH0pO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZXhwb3J0IGNsYXNzIFRpY2tldHNBcGlWaWV3TW9kZWwge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHByaXZhdGUgcmVhZG9ubHkgbGlua3M6IExpbmtzKSB7XHJcbiAgICB9XHJcblxyXG4gICAgcmVhZCgpIHtcclxuICAgICAgY29uc3Qgc2VsZiA9IHRoaXM7XHJcbiAgICAgIGNvbnN0IGdyaWQgPSAkKFwiI3RpY2tldHNHcmlkXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgIGlmIChncmlkICE9PSB1bmRlZmluZWQgJiYgZ3JpZCAhPT0gbnVsbCkge1xyXG4gICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgJChcIiN0aWNrZXRzR3JpZFwiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgcmVhZChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICB1cmw6IHNlbGYubGlua3MuVGlja2V0U3VtbWFyeSxcclxuICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgIGNhY2hlOiBmYWxzZVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBncm91cDoge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlN0YXR1c1wiLFxyXG4gICAgICAgICAgICAgIGRpcjogXCJkZXNjXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgcGFnZVNpemU6IDIwLFxyXG4gICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICB0b3RhbChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbigpIHtcclxuICAgICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICQoXCIjdGlja2V0c0dyaWRNZXNzYWdlXCIpLnRleHQoXCJObyB0aWNrZXRzIGZvdW5kXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjdGlja2V0c0dyaWRcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiNwYWdlclwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoXCIjdGlja2V0c0dyaWRNZXNzYWdlXCIpLnRleHQoXCJcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgJChcIiN0aWNrZXRzR3JpZFwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAkKFwiI3BhZ2VyXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICQoXCIjd2FybmluZ1wiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICBmaWx0ZXJhYmxlOiBmYWxzZSxcclxuICAgICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgZGF0YUJvdW5kKGUpIHtcclxuICAgICAgICAgICAgY29uc3QgZ3JpZCA9ICQoXCIjdGlja2V0c0dyaWRcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgICAgICAgY29uc3QgZGF0YVZpZXcgPSBncmlkLmRhdGFTb3VyY2UudmlldygpO1xyXG5cclxuICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBkYXRhVmlldy5sZW5ndGg7IGkrKykge1xyXG4gICAgICAgICAgICAgIGZvciAobGV0IGogPSAwOyBqIDwgZGF0YVZpZXdbaV0uaXRlbXMubGVuZ3RoOyBqKyspIHtcclxuICAgICAgICAgICAgICAgIGlmIChkYXRhVmlld1tpXS5pdGVtc1tqXS5zdGF0dXMgPT09IFwiQ2xvc2VkXCIpIHtcclxuICAgICAgICAgICAgICAgICAgY29uc3QgdWlkID0gZGF0YVZpZXdbaV0uaXRlbXNbal0udWlkO1xyXG4gICAgICAgICAgICAgICAgICBncmlkLmNvbGxhcHNlR3JvdXAoJChcIiN0aWNrZXRzR3JpZFwiKS5maW5kKGB0cltkYXRhLXVpZD0ke3VpZH1dYCkucHJldihcInRyLmstZ3JvdXBpbmctcm93XCIpKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBmaWVsZDogXCJDcmVhdGVkQXRcIixcclxuICAgICAgICAgICAgICB0aXRsZTogXCJEYXRlIENyZWF0ZWRcIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgIC8vdGVtcGxhdGU6IFwiIz0gTmFtZSAjIFwiICsgJyMgaWYoVHlwZSA9PT0gXCInICsgT3JkZXJUeXBlUHVzaCArICdcIikgeyAjJyArIFwiIC0gIzpTbHVnIyBcIiArIFwiIyB9ICMgXCIsXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgIGZpZWxkOiBcIlJlY2lwaWVudFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlJlY2lwaWVudFwiLFxyXG4gICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgLy97XHJcbiAgICAgICAgICAgIC8vICBmaWVsZDogXCJUeXBlXCIsXHJcbiAgICAgICAgICAgIC8vICB0aXRsZTogXCJUeXBlXCIsXHJcbiAgICAgICAgICAgIC8vICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAvLyAgd2lkdGg6IDIwMCxcclxuICAgICAgICAgICAgLy8gIC8vdGVtcGxhdGU6IFwiIz0ga2VuZG8udG9TdHJpbmcoa2VuZG8ucGFyc2VEYXRlKERhdGVTdWJtaXR0ZWQsICdNTS9kZC95eXl5JyksICdNTS9kZC95eXl5JykgI1wiLFxyXG4gICAgICAgICAgICAvLyAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgLy99LFxyXG4gICAgICAgICAgICAvL3tcclxuICAgICAgICAgICAgLy8gIGZpZWxkOiBcIlN0YXR1c1wiLFxyXG4gICAgICAgICAgICAvLyAgdGl0bGU6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgIC8vICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAvLyAgd2lkdGg6IDIwMCxcclxuICAgICAgICAgICAgLy8gIC8vdGVtcGxhdGU6IFwiIz0ga2VuZG8udG9TdHJpbmcoa2VuZG8ucGFyc2VEYXRlKERhdGVTdWJtaXR0ZWQsICdNTS9kZC95eXl5JyksICdNTS9kZC95eXl5JykgI1wiLFxyXG4gICAgICAgICAgICAvLyAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgLy99LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiU3ViamVjdFwiLFxyXG4gICAgICAgICAgICAgIHRpdGxlOiBcIlN1YmplY3RcIixcclxuICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGxlZnQ7XCIgfSxcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgZmllbGQ6IFwiXCIsXHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiXCIsXHJcbiAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICB3aWR0aDogMjUwLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOlxyXG4gICAgICAgICAgICAgICAgXCI8YSBocmVmPSdcXFxcIycgY2xhc3M9XFxcImJ0biBidG4tZGVmYXVsdFxcXCIgc3R5bGU9XFxcIm1hcmdpbi1yaWdodDogNXB4O1xcXCIgb25jbGljaz1cXFwidGlja2V0c1ZpZXdNb2RlbC52aWV3RGV0YWlsKCcjPSB1aWQgIycpXFxcIj5WaWV3IERldGFpbDwvYT48YSBocmVmPVxcXCIjPSBaZW5kZXNrRGV0YWlsICNcXFwiIGNsYXNzPVxcXCJidG4gYnRuLWRlZmF1bHRcXFwiPlZpZXcgSW4gWmVuZGVzazwvYT5cIixcclxuICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgdGl0bGU6IFwiU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI3Jlc3BvbnNpdmUtY29sdW1uLXRlbXBsYXRlLWNvbXBsZXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgbWVkaWE6IFwiKG1heC13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgXVxyXG4gICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgdmlld0RldGFpbCh1aWQpIHtcclxuICAgICAgY29uc3QgZ3JpZCA9ICQoXCIjdGlja2V0c0dyaWRcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgY29uc3Qgcm93ID0gZ3JpZC50Ym9keS5maW5kKGB0cltkYXRhLXVpZD0nJHt1aWR9J11gKTtcclxuICAgICAgY29uc3QgaXRlbSA9IGdyaWQuZGF0YUl0ZW0ocm93KTtcclxuICAgICAgJChcIiNkZXRhaWxzTW9kYWwgLm1vZGFsLWhlYWRlclwiKS5odG1sKGBEZXRhaWxzIGZvciBUaWNrZXQgJHtpdGVtW1wiSWRcIl19YCk7XHJcbiAgICAgICQoXCIjZGV0YWlsc01vZGFsIC5tb2RhbC1ib2R5IHByZVwiKS5odG1sKGl0ZW1bXCJEZXNjcmlwdGlvblwiXSk7XHJcbiAgICAgICQoXCIjZGV0YWlsc01vZGFsXCIpLmFwcGVuZFRvKFwiYm9keVwiKS5tb2RhbChcInNob3dcIik7XHJcbiAgICB9XHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgTm90ZXNWaWV3TW9kZWwge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHByaXZhdGUgcmVhZG9ubHkgbGlua3M6IExpbmtzKSB7XHJcbiAgICB9XHJcblxyXG4gICAgcmVhZCgpIHtcclxuICAgICAgJC5hamF4KFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIHR5cGU6IFwiR0VUXCIsXHJcbiAgICAgICAgICB1cmw6IHRoaXMubGlua3MuVXNlck5vdGVzLFxyXG4gICAgICAgICAgc3VjY2Vzcyhub3Rlcykge1xyXG4gICAgICAgICAgICAkKFwiI25vdGVzIHRhYmxlIHRib2R5IHRyXCIpLnJlbW92ZSgpO1xyXG4gICAgICAgICAgICBpZiAobm90ZXMubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgICAgJChcIiNub3RlcyAjbWVzc2FnZVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgJChcIiNub3Rlc190YWJsZVwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgJChcIiNub3Rlc19ub19ub3Rlc19tZXNzYWdlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAkKFwiI25vdGVzX3RhYmxlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAkKG5vdGVzKS5lYWNoKChpLCBub3RlOiBhbnkpID0+IHtcclxuICAgICAgICAgICAgICAgICQoYDx0cj48dGQ+JHtub3RlLkRhdGVBZGRlZH08L3RkPjx0ZD4ke25vdGUuQWRkZWRCeVxyXG4gICAgICAgICAgICAgICAgICAgIH08L3RkPjx0ZD4ke25vdGUuQm9keX08L3RkPjwvdHI+YClcclxuICAgICAgICAgICAgICAgICAgLmFwcGVuZFRvKFwiI25vdGVzIHRhYmxlIHRib2R5XCIpO1xyXG4gICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyLCBzdGF0dXMsIGVycm9yKSB7XHJcbiAgICAgICAgICAgICQoXCIjZ2xvYmFsTWVzc2FnZVwiKS5yZW1vdmVDbGFzcygpLmFkZENsYXNzKFwiYWxlcnQgYWxlcnQtZGFuZ2VyXCIpLmh0bWwoYEVycm9yOiAke3hoci5yZXNwb25zZVRleHR9YCkuc2hvdygpO1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHNhdmUoKSB7XHJcbiAgICAgICQoXCIjZXJyb3JcIikuaGlkZSgpO1xyXG4gICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQoXCIjYWNjb3VudE5vdGVNb2RhbFwiKS5tb2RhbChcImhpZGVcIik7XHJcbiAgICAgICQuYWpheChcclxuICAgICAgICB7XHJcbiAgICAgICAgICB0eXBlOiBcIlBPU1RcIixcclxuICAgICAgICAgIHVybDogc2VsZi5saW5rcy5BZGRVc2VyTm90ZSxcclxuICAgICAgICAgIGRhdGE6IHsgYWRkZWRieTogJChcIiNhZG1pbnVzZXJuYW1lXCIpLnZhbCgpLCBib2R5OiAkKFwiI25vdGVib2R5XCIpLnZhbCgpLCB1c2VyaWQ6ICQoXCIjdXNlcmlkXCIpLnZhbCgpIH0sXHJcbiAgICAgICAgICBzdWNjZXNzKCkge1xyXG4gICAgICAgICAgICBzZWxmLnJlYWQoKTtcclxuICAgICAgICAgICAgJChcIiNub3RlYm9keVwiKS52YWwoXCJcIik7XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgZXJyb3IoeGhyLCBzdGF0dXMsIGVycm9yKSB7XHJcbiAgICAgICAgICAgIGRpc3BsYXlNZXNzYWdlKGBFcnJvciBzYXZpbmcgbm90ZS4gTWVzc2FnZTogJHt4aHIuc3RhdHVzVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuICB9XHJcblxyXG4gIGV4cG9ydCBjbGFzcyBEb2N1bWVudHNWaWV3TW9kZWwge1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKHByaXZhdGUgcmVhZG9ubHkgbGlua3M6IExpbmtzKSB7XHJcbiAgICB9XHJcblxyXG4gICAgc2F2ZSgpIHtcclxuICAgIH1cclxuXHJcbiAgICByZWFkKCkge1xyXG4gICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQuYWpheChcclxuICAgICAgICB7XHJcbiAgICAgICAgICB0eXBlOiBcIkdFVFwiLFxyXG4gICAgICAgICAgdXJsOiB0aGlzLmxpbmtzLkFkbWluRmlsZXMsXHJcbiAgICAgICAgICBzdWNjZXNzKGZpbGVzKSB7XHJcbiAgICAgICAgICAgICQoXCIjZG9jdW1lbnRzIHRhYmxlIHRib2R5IHRyXCIpLnJlbW92ZSgpO1xyXG4gICAgICAgICAgICBpZiAoZmlsZXMubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgICAgJChcIiNkb2N1bWVudHMgI21lc3NhZ2VcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICQoXCIjZG9jdW1lbnRzX3RhYmxlXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAkKFwiI2RvY3VtZW50c190YWJsZVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgJChcIiNkb2N1bWVudHMgI21lc3NhZ2VcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICQoZmlsZXMpLmVhY2goKGksIGRvY3VtZW50OiBhbnkpID0+IHtcclxuICAgICAgICAgICAgICAgICQoYDx0cj48dGQ+JHtkb2N1bWVudC5EYXRlQWRkZWR9PC90ZD48dGQ+JHtkb2N1bWVudC5DdXN0b21lckZpbGVOYW1lfTwvdGQ+PHRkIGlkPVwiJHtkb2N1bWVudC5GaWxlSWR9XCI+JHtcclxuICAgICAgICAgICAgICAgICAgZG9jdW1lbnQuTm90ZXNcclxuICAgICAgICAgICAgICAgICAgfTwvdGQ+PHRkIHN0eWxlPVwidGV4dC1hbGlnbjogcmlnaHQ7XCI+PGEgaHJlZj1cIiNcIiBjbGFzcz1cImJ0biBidG4tZGFuZ2VyIGJ0bi1zbVwiIHN0eWxlPVwibWFyZ2luLXJpZ2h0OiA1cHg7XCIgb25jbGljaz1cXFwiZG9jdW1lbnRzVmlld01vZGVsLmRlbGV0ZSgke1xyXG4gICAgICAgICAgICAgICAgICBkb2N1bWVudC5GaWxlSWRcclxuICAgICAgICAgICAgICAgICAgfSk7XCI+RGVsZXRlPC9hPjxhIGhyZWY9XCIjXCIgY2xhc3M9XCJidG4gYnRuLWRlZmF1bHQgYnRuLXNtXCIgc3R5bGU9XCJtYXJnaW4tcmlnaHQ6IDVweDtcIiBvbmNsaWNrPVwiZG9jdW1lbnRzVmlld01vZGVsLm9wZW5BZGROb3RlTW9kYWwoJHtcclxuICAgICAgICAgICAgICAgICAgZG9jdW1lbnQuRmlsZUlkXHJcbiAgICAgICAgICAgICAgICAgIH0pO1wiPkFkZCZuYnNwO05vdGU8L2E+PGEgY2xhc3M9XCJidG4gYnRuLWRlZmF1bHQgYnRuLXNtXCIgaHJlZj1cIiR7c2VsZi5saW5rcy5BZG1pbkZpbGVEb3dubG9hZFxyXG4gICAgICAgICAgICAgICAgICB9P2ZpbGVpZD0ke1xyXG4gICAgICAgICAgICAgICAgICBkb2N1bWVudC5GaWxlSWR9XCI+RG93bmxvYWQ8L2E+PC90ZD48L3RyPmApLmFwcGVuZFRvKFwiI2RvY3VtZW50cyB0YWJsZSB0Ym9keVwiKTtcclxuICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGVycm9yKHhociwgc3RhdHVzLCBlcnJvcikge1xyXG4gICAgICAgICAgICBkaXNwbGF5TWVzc2FnZShgRXJyb3I6ICR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgZGVsZXRlKGZpbGVJZCkge1xyXG4gICAgICB2YXIgc2VsZiA9IHRoaXM7XHJcbiAgICAgICQoXCIjZXJyb3JcIikuaGlkZSgpO1xyXG4gICAgICBpZiAoY29uZmlybShcIkRlbGV0ZSB0aGlzIGZpbGU/XCIpKSB7XHJcbiAgICAgICAgLy8gZGVsZXRlIGZpbGVcclxuICAgICAgICAkLmFqYXgoXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIHR5cGU6IFwiUE9TVFwiLFxyXG4gICAgICAgICAgICB1cmw6IHNlbGYubGlua3MuQWRtaW5GaWxlRGVsZXRlLFxyXG4gICAgICAgICAgICBkYXRhOiB7IGZpbGVpZDogZmlsZUlkIH0sXHJcbiAgICAgICAgICAgIHN1Y2Nlc3MoKSB7XHJcbiAgICAgICAgICAgICAgc2VsZi5yZWFkKCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0pO1xyXG4gICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgb3BlbkFkZE5vdGVNb2RhbChmaWxlaWQ6IG51bWJlcikge1xyXG4gICAgICAkKFwiI2Vycm9yXCIpLmhpZGUoKTtcclxuICAgICAgJChcIiNkb2N1bWVudElkXCIpLnZhbChmaWxlaWQpO1xyXG4gICAgICAkKFwiI2FkbWluRmlsZU5vdGVNb2RhbFwiKS5tb2RhbChcInNob3dcIik7XHJcbiAgICB9XHJcblxyXG4gICAgYWRkTm90ZSgpIHtcclxuICAgICAgJChcIiNlcnJvclwiKS5oaWRlKCk7XHJcbiAgICAgIHZhciBzZWxmID0gdGhpcztcclxuICAgICAgJChcIiNhZG1pbkZpbGVOb3RlTW9kYWxcIikubW9kYWwoXCJoaWRlXCIpO1xyXG4gICAgICAkLmFqYXgoXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgdHlwZTogXCJQT1NUXCIsXHJcbiAgICAgICAgICB1cmw6IHRoaXMubGlua3MuQWRtaW5GaWxlQWRkTm90ZSxcclxuICAgICAgICAgIGRhdGE6IHsgbm90ZXM6ICQoXCIjZG9jdW1lbnROb3RlQm9keVwiKS52YWwoKSwgZmlsZWlkOiAkKFwiI2RvY3VtZW50SWRcIikudmFsKCkgfSxcclxuICAgICAgICAgIHN1Y2Nlc3MoKSB7XHJcbiAgICAgICAgICAgIHNlbGYucmVhZCgpO1xyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIGVycm9yKHhociwgc3RhdHVzLCBlcnJvcikge1xyXG4gICAgICAgICAgICBkaXNwbGF5TWVzc2FnZShgRXJyb3I6ICR7eGhyLnJlc3BvbnNlVGV4dH1gLCBcImRhbmdlclwiKTtcclxuICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgfVxyXG5cclxuICBleHBvcnQgY2xhc3MgTGlua3Mge1xyXG4gICAgQ2FyZHM6IHN0cmluZztcclxuICAgIERldGFpbDogc3RyaW5nO1xyXG4gICAgRWRpdDogc3RyaW5nO1xyXG4gICAgRmlsZXM6IHN0cmluZztcclxuICAgIEFkbWluRmlsZXM6IHN0cmluZztcclxuICAgIEFkbWluRmlsZURlbGV0ZTogc3RyaW5nO1xyXG4gICAgQWRtaW5GaWxlQWRkTm90ZTogc3RyaW5nO1xyXG4gICAgQWRtaW5GaWxlRG93bmxvYWQ6IHN0cmluZztcclxuICAgIEFkbWluRmlsZVVwbG9hZDogc3RyaW5nO1xyXG4gICAgU2F2ZVVzZXJGaWxlOiBzdHJpbmc7XHJcbiAgICBVc2VyTm90ZXM6IHN0cmluZztcclxuICAgIEFkZFVzZXJOb3RlOiBzdHJpbmc7XHJcbiAgICBKb2JzOiBzdHJpbmc7XHJcbiAgICBKb2JzTmV3OiBzdHJpbmc7XHJcbiAgICBMb2dJbkFzVXNlcjogc3RyaW5nO1xyXG4gICAgQXV0b1Byb2Nlc3NvclJ1bGVzOiBzdHJpbmc7XHJcbiAgICBDb250YWN0czogc3RyaW5nO1xyXG4gICAgVXNlck11c3RDaGFuZ2VQYXNzd29yZDogc3RyaW5nO1xyXG4gICAgU2VydmljZUFjY291bnRzOiBzdHJpbmc7XHJcbiAgICBEZWFsczogc3RyaW5nO1xyXG4gICAgTmV3RGVhbDogc3RyaW5nO1xyXG4gICAgQ2hhcmdlczogc3RyaW5nO1xyXG4gICAgTWVzc2FnZXM6IHN0cmluZztcclxuICAgIFJhdGVDYXJkczogc3RyaW5nO1xyXG4gICAgQVBJUmVwb3J0aW5nOiBzdHJpbmc7XHJcbiAgICBEb3dubG9hZFVzYWdlOiBzdHJpbmc7XHJcbiAgICBTYXZlVXNhZ2U6IHN0cmluZztcclxuICAgIExlYWREZXRhaWw6IHN0cmluZztcclxuICAgIFZpZXdOYXRpb25zOiBzdHJpbmc7XHJcbiAgICBQYXltZW50VXBkYXRlTGluazogc3RyaW5nO1xyXG4gICAgU291cmNlTGVhZDogc3RyaW5nO1xyXG4gICAgVXNlck9wZXJhdGluZ01ldHJpYzogc3RyaW5nO1xyXG4gICAgVXNlclByb2R1Y3RVc2FnZU1ldHJpYzogc3RyaW5nO1xyXG4gICAgVGlja2V0U3VtbWFyeTogc3RyaW5nO1xyXG4gICAgQ3JlYXRlVGlja2V0OiBzdHJpbmc7XHJcbiAgICBDaGFuZ2VPd25lcjogc3RyaW5nO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKGRhdGE6IGFueSkge1xyXG4gICAgICB0aGlzLkNhcmRzID0gZGF0YS5MaW5rcy5DYXJkcztcclxuICAgICAgdGhpcy5EZXRhaWwgPSBkYXRhLkxpbmtzLkRldGFpbDtcclxuICAgICAgdGhpcy5FZGl0ID0gZGF0YS5MaW5rcy5FZGl0O1xyXG4gICAgICB0aGlzLkZpbGVzID0gZGF0YS5MaW5rcy5GaWxlcztcclxuICAgICAgdGhpcy5BZG1pbkZpbGVzID0gZGF0YS5MaW5rcy5BZG1pbkZpbGVzO1xyXG4gICAgICB0aGlzLkFkbWluRmlsZURlbGV0ZSA9IGRhdGEuTGlua3MuQWRtaW5GaWxlRGVsZXRlO1xyXG4gICAgICB0aGlzLkFkbWluRmlsZUFkZE5vdGUgPSBkYXRhLkxpbmtzLkFkbWluRmlsZUFkZE5vdGU7XHJcbiAgICAgIHRoaXMuQWRtaW5GaWxlRG93bmxvYWQgPSBkYXRhLkxpbmtzLkFkbWluRmlsZURvd25sb2FkO1xyXG4gICAgICB0aGlzLkFkbWluRmlsZVVwbG9hZCA9IGRhdGEuTGlua3MuQWRtaW5GaWxlVXBsb2FkO1xyXG4gICAgICB0aGlzLlNhdmVVc2VyRmlsZSA9IGRhdGEuTGlua3MuU2F2ZVVzZXJGaWxlO1xyXG4gICAgICB0aGlzLlVzZXJOb3RlcyA9IGRhdGEuTGlua3MuVXNlck5vdGVzO1xyXG4gICAgICB0aGlzLkFkZFVzZXJOb3RlID0gZGF0YS5MaW5rcy5BZGRVc2VyTm90ZTtcclxuICAgICAgdGhpcy5Kb2JzID0gZGF0YS5MaW5rcy5Kb2JzO1xyXG4gICAgICB0aGlzLkpvYnNOZXcgPSBkYXRhLkxpbmtzLkpvYnNOZXc7XHJcbiAgICAgIHRoaXMuTG9nSW5Bc1VzZXIgPSBkYXRhLkxpbmtzLkxvZ0luQXNVc2VyO1xyXG4gICAgICB0aGlzLkF1dG9Qcm9jZXNzb3JSdWxlcyA9IGRhdGEuTGlua3MuQXV0b1Byb2Nlc3NvclJ1bGVzO1xyXG4gICAgICB0aGlzLkNvbnRhY3RzID0gZGF0YS5MaW5rcy5Db250YWN0cztcclxuICAgICAgdGhpcy5Vc2VyTXVzdENoYW5nZVBhc3N3b3JkID0gZGF0YS5MaW5rcy5Vc2VyTXVzdENoYW5nZVBhc3N3b3JkO1xyXG4gICAgICB0aGlzLlNlcnZpY2VBY2NvdW50cyA9IGRhdGEuTGlua3MuU2VydmljZUFjY291bnRzO1xyXG4gICAgICB0aGlzLkRlYWxzID0gZGF0YS5MaW5rcy5EZWFscztcclxuICAgICAgdGhpcy5OZXdEZWFsID0gZGF0YS5MaW5rcy5OZXdEZWFsO1xyXG4gICAgICB0aGlzLkNoYXJnZXMgPSBkYXRhLkxpbmtzLkNoYXJnZXM7XHJcbiAgICAgIHRoaXMuTWVzc2FnZXMgPSBkYXRhLkxpbmtzLk1lc3NhZ2VzO1xyXG4gICAgICB0aGlzLlJhdGVDYXJkcyA9IGRhdGEuTGlua3MuUmF0ZUNhcmRzO1xyXG4gICAgICB0aGlzLkFQSVJlcG9ydGluZyA9IGRhdGEuTGlua3MuQVBJUmVwb3J0aW5nO1xyXG4gICAgICB0aGlzLkRvd25sb2FkVXNhZ2UgPSBkYXRhLkxpbmtzLkRvd25sb2FkVXNhZ2U7XHJcbiAgICAgIHRoaXMuU2F2ZVVzYWdlID0gZGF0YS5MaW5rcy5TYXZlVXNhZ2U7XHJcbiAgICAgIHRoaXMuTGVhZERldGFpbCA9IGRhdGEuTGlua3MuTGVhZERldGFpbDtcclxuICAgICAgdGhpcy5WaWV3TmF0aW9ucyA9IGRhdGEuTGlua3MuVmlld05hdGlvbnM7XHJcbiAgICAgIHRoaXMuUGF5bWVudFVwZGF0ZUxpbmsgPSBkYXRhLkxpbmtzLlBheW1lbnRVcGRhdGVMaW5rO1xyXG4gICAgICB0aGlzLlNvdXJjZUxlYWQgPSBkYXRhLkxpbmtzLlNvdXJjZUxlYWQ7XHJcbiAgICAgIHRoaXMuVXNlck9wZXJhdGluZ01ldHJpYyA9IGRhdGEuTGlua3MuVXNlck9wZXJhdGluZ01ldHJpYztcclxuICAgICAgdGhpcy5Vc2VyUHJvZHVjdFVzYWdlTWV0cmljID0gZGF0YS5MaW5rcy5Vc2VyUHJvZHVjdFVzYWdlTWV0cmljO1xyXG4gICAgICB0aGlzLlRpY2tldFN1bW1hcnkgPSBkYXRhLkxpbmtzLlRpY2tldFN1bW1hcnk7XHJcbiAgICAgIHRoaXMuQ3JlYXRlVGlja2V0ID0gZGF0YS5MaW5rcy5DcmVhdGVUaWNrZXQ7XHJcbiAgICAgIHRoaXMuQ2hhbmdlT3duZXIgPSBkYXRhLkxpbmtzLkNoYW5nZU93bmVyO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgZnVuY3Rpb24gZGlzcGxheU1lc3NhZ2UobWVzc2FnZSwgdHlwZSkge1xyXG4gICAgJChcIiNnbG9iYWxNZXNzYWdlXCIpLnJlbW92ZUNsYXNzKCkuYWRkQ2xhc3MoYGFsZXJ0IGFsZXJ0LSR7dHlwZX1gKS5odG1sKG1lc3NhZ2UpLnNob3coKVxyXG4gICAgICAuZmFkZVRvKDcwMDAsIDUwMCkuc2xpZGVVcCg1MDAsICgpID0+IHsgJChcIiNnbG9iYWxNZXNzYWdlXCIpLnNsaWRlVXAoNTAwKSB9KTtcclxuICB9XHJcblxyXG59XHJcblxyXG5mdW5jdGlvbiBwcm9jZXNzaW5nTWV0cmljRm9ybWF0dGVyKHJlY29yZHM6IG51bWJlciwgbWF0Y2hlczogbnVtYmVyKSB7XHJcbiAgaWYgKHJlY29yZHMgPT09IDApIHJldHVybiBcIi1cIjtcclxuICByZXR1cm4ga2VuZG8udG9TdHJpbmcocmVjb3JkcywgXCJuMFwiKSArXHJcbiAgICBcIiAvIFwiICtcclxuICAgIGtlbmRvLnRvU3RyaW5nKG1hdGNoZXMsIFwibjBcIikgK1xyXG4gICAgXCIgKFwiICtcclxuICAgIE1hdGguZmxvb3IoKG1hdGNoZXMgLyByZWNvcmRzKSAqIDEwMCkgK1xyXG4gICAgXCIlKVwiO1xyXG59Il19