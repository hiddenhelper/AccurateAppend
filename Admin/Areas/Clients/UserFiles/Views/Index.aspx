<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<FilesRequest>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.UserFiles" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.UserFiles.Models" %>
  <asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Files
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>

        <div class="row" style="padding-right: 20px;">
            <div class="pull-right">
                <%: this.Html.SiteDropDown(null) %>
            </div>
        </div>
        
        <div class="row">
            <div class="pull-right" style="padding: 15px 20px;">
                <span id="linkPublicUploadUrl"><label style="margin-right: 5px;">Public Upload Link:</label><input onclick="this.select();" style="padding: 3px 3px 3px 0; border: none; width: 575px;" /></span>
                <button type="button" class="btn btn-info" onclick="viewModel.uploadFile()" id="uploadButton" style="display: none;">Upload File</button>
                <button type="button" class="btn btn-default" onclick="javascript:reset();"><span class="fa fa-refresh"></span>Reset</button>
                <input id="siteUsers" style="width: 250px;" />
                <span id="dateRange"></span>
            </div>
        </div>
        <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="info">No files found</div>
        <div id="grid" style="margin-bottom: 20px;"></div>

    </div>

    <div class="modal fade" id="uploadModal">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <input name="files" id="files" type="file" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    
    <script src="<%= Url.Content("~/Scripts/jquery-confirm.js") %>" type="text/javascript"></script>

    <style type="text/css">

        .k-grid td { overflow: visible !important; } 

    </style>

    <script type="text/javascript">

        var pUserId = <% = this.Model.UserId == null ? "null" : "'" + this.Model.UserId + "'" %>;
        var pEmail =  <% = this.Model.UserName == null ? "null" : "'" + this.Model.UserName + "'" %>;

        var dateRangeWidget;

        $(function () {

            dateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("dateRange",
               new AccurateAppend.Ui.DateRangeWidgetSettings(
                   [
                       AccurateAppend.Ui.DateRangeValue.Last24Hours,
                       AccurateAppend.Ui.DateRangeValue.Last7Days,
                       AccurateAppend.Ui.DateRangeValue.Last30Days,
                       AccurateAppend.Ui.DateRangeValue.Custom
                   ],
                   AccurateAppend.Ui.DateRangeValue.Last7Days,
                   [
                       viewModel.renderGrid
                   ]));

            var autocomplete = $("#siteUsers").kendoAutoComplete({
                minLength: 3,
                dataTextField: "Email",
                placeholder: "Search by email address...",
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: "<%: this.Url.Action("List", "SearchClients", new {area="Clients"}) %>?activeWithin=<%: DateRange.LastYear %>&applicationid=" + $("#ApplicationId").val()
                        }
                    }
                },
                height: 370,
                change: function () {
                    if (this.value() === '') {
                        pEmail = null;
                        dateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
                        $("#uploadButton").hide();
                        $("#linkPublicUploadUrl input").val('');
                        $("#linkPublicUploadUrl").hide();
                        $("#linkRawInputFiles").show();
                    } else {
                        pEmail = this.value();
                        dateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
                        autocomplete.value(pEmail);
                        $("#uploadButton").show();
                        $("#linkRawInputFiles").hide();

                        var url = '<%: this.Model.PublicUploadLink%>';
                        $("#linkPublicUploadUrl input").val(url);
                        $("#linkPublicUploadUrl").show();
                    }
                }
            }).data("kendoAutoComplete");

            if (pEmail != null) {
                autocomplete.value(pEmail);
                dateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
                $("#uploadButton").show();
                $("#linkRawInputFiles").hide();
                var url = '<%: this.Model.PublicUploadLink%>';
                $("#linkPublicUploadUrl input").val(url);
                $("#linkPublicUploadUrl").show();
            } else {
                autocomplete.value('');
                $("#linkRawInputFiles").show();
                $("#linkPublicUploadUrl input").val('');
                $("#linkPublicUploadUrl").hide();
            }

            AccurateAppend.Ui.ApplicationId.load();

            $("#ApplicationId").bind('change', function () {
                AccurateAppend.Ui.ApplicationId.set();
                dateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
            });

            $("#statuses").bind('change', function () {
                viewModel.renderGrid();
            });

            dateRangeWidget.refresh();
            setInterval(viewModel.renderGrid, 60000);
        });

        var viewModel = {
            renderGrid: function () {
                console.log('Render grid');
                var grid = $("#grid").data("kendoGrid");
                if (grid !== undefined && grid !== null) {
                    grid.dataSource.read();
                } else {
                    $("#grid").kendoGrid({
                        dataSource: {
                            type: "json",
                            transport: {
                                read: function (options) {
                                  var data = { applicationid: $("#ApplicationId").val(), startdate: moment(dateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'), enddate: moment(dateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'), email: pEmail }
                                    if (pUserId != null) data.userid = pUserId;
                                    $.ajax({
                                        url: '<%= this.Url.Action("List", "FilesApi", new { Area = "Operations" }) %>',
                                        dataType: 'json',
                                        type: 'GET',
                                        data: data,
                                        success: function(result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: 'json',
                                data: "Data",
                                total: function(response) {
                                    return response.Data.length;
                                }
                            },
                            pageSize: 20,
                            change: function() {
                                if (this.data().length <= 0) {
                                    $("#info").show();
                                    $("#grid").hide();
                                } else {
                                    $("#info").hide();
                                    $("#grid").show();
                                }
                            }
                        },
                        scrollable: false,
                        sortable: true,
                        pageable: {
                            input: true,
                            numeric: false
                        },
                        filterable: false,
                        columns: [
                            { field: "Id", title: "Id", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { field: "UserName", title: "User", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("<a href=\"#=Links.UserDetail#\">#=UserName#</a>") },
                            { field: "CreatedDate", title: "Date Added", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { field: "CustomerFileName", title: "File Name", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { field: "FileSize", title: "Size", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { template: kendo.template($("#cmdTemplate").html()), width: "150px", attributes: { style: "text-align: center;" } }
                            ]
                    });
                }
            },
            viewDetail: function (url) {
                $.get(url, function (data) {
                    $('<div id="detail" style="padding: 20px;"></div>').kendoWindow({
                        title: "File Detail: " + data.Id,
                        resizable: false,
                        modal: true,
                        viewable: false,
                        content: { template: kendo.template($("#DetailTemplate").html())(data) },
                        width: "800px",
                        position: { top: "200px", left: "600px" },
                        scrollable: false
                    }).data("kendoWindow").open();
                });
            },
            deleteFile: function (url) {
                if ($("#detail").data('kendoWindow') != null) $("#detail").data('kendoWindow').destroy();
                $.get(url, function() {
                            viewModel.renderGrid();
                });
                viewModel.renderGrid();
            },
            newJob: function (uri) {
                window.location.replace(uri);
            },
            downloadFile: function (uri) {
                window.location.replace(uri);
            },
            uploadFile: function () {
                // clear files from Upload control
                if ($("#files").data("kendoUpload") != null) {
                    $("#files").data("kendoUpload").destroy();
                    $("#files").closest(".k-upload").remove();
                    $(".modal-body").append('<input name="files" id="files" type="file" />');
                }
                $("#files").kendoUpload({
                    async: {
                        saveUrl: '<%= this.Model.UploadFileLink %>',
                        autoUpload: true,
                        withCredentials: false
                    },
                    success: function (e) {
                        if (e.response.status === 200) {
                        window.location.replace(e.response.data);
                      }
                        else {
                        $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
                        console.log("Failed to upload ");
                      }
                    },
                    error: function (e) {
                        var err = $.parseJSON(e.XMLHttpRequest.responseText);
 
                        alert(err.Message);
 
                        $.map(e.files, function (file) {
                            alert("Could not upload " + file.name);
                        });
                    }
                });
                $("#uploadModal .modal-title").text('Upload File: ' + $("#siteUsers").val());
                $("#uploadModal").modal('show');
            }
        }

        function reset() {
            history.pushState(null, 'Files', '<%= this.Url.BuildFor<UserFilesController>().ToIndex() %>');
            window.location.replace('<%= this.Url.BuildFor<UserFilesController>().ToIndex() %>');
        }

    </script>
    
    <script type="text/x-kendo-template" id="cmdTemplate">
        
        <div class="btn-group" styley="z-index">
          <button type="button" class="btn btn-default" onclick="viewModel.viewDetail('#= Actions.ItemDetail #')">View Details</button>
          <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
            <span class="caret"></span>
            <span class="sr-only">Toggle Dropdown</span>
          </button>
          <ul class="dropdown-menu" role="menu">
            #if (Actions.NewJob != null) {#
            <li><a href="\\#" onclick="viewModel.newJob('#= Actions.NewJob #')">New Job</a></li>
            #}#

            #if (Links.DownloadUri != null) {#
            <li><a href="\\#" onclick="viewModel.downloadFile('#= Links.DownloadUri #')">Download</a></li>
            #}#

            <li><a href="\\#" onclick="viewModel.deleteFile('#= Actions.Delete #')">Delete</a></li>
          </ul>
        </div>
    
    </script>

    <script type="text/x-kendo-template" id="DetailTemplate">
#if (Links.DownloadUri != null) {#
        <button type="button" class="btn btn-default btn-sm" onclick="viewModel.downloadFile('#= Links.DownloadUri #')">Download</button>
#}#
      
#if (Actions.NewJob != null) {#
        <button type="button" class="btn btn-default btn-sm" onclick="viewModel.newJob('#= Actions.NewJob #')">New Job</button>
#}#
        <button type="button" class="btn btn-danger btn-sm" onclick="viewModel.deleteFile('#= Actions.Delete #')">Delete</button>
        
        <table class="table table-bordered" style="margin-top: 10px;">
            <tr>
                <th style="width: 100px;">Id</th>
                <td style="width: 600px;">#= FileId #</td>
            </tr>
            <tr>
                <th style="width: 100px;">User</th>
                <td style="width: 600px;"><a href="#: Links.UserDetail #">#= UserName #</a></td>
            </tr>
            <tr>
                <th style="width: 100px;">Created</th>
                <td style="width: 600px;">#= kendo.toString(kendo.parseDate(DateAdded), "g") #</td>
            </tr>
            <tr>
                <th style="width: 100px;">File Name</th>
                <td style="width: 600px;">#= CustomerFileName #</td>
            </tr>
            <tr>
                <th>Public Download Link</th>
                <td>#if (Links.PublicDownloadUri != null) {# <input class=form-control" value="#= Links.PublicDownloadUri #" onclick="this.select();" style="width: 98%; padding: 3px 3px 3px 0; border: none;"/> #}#</td>
            </tr>
        </table>
    
    </script>

</asp:Content>


