<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<MessagesView>" %>

<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Operations.Message.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Operations.Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Messages
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
              <% if (Model.UserDetail != null)
                 { %>
              <a class="btn btn-default" href="<%= this.Model.UserDetail %>" ><span class="fa fa-user"></span>View Contact</a>
                <% } %>
                <button type="button" class="btn btn-default" onclick="javascript:reset();"><span class="fa fa-refresh"></span>Reset</button>
                <input id="siteUsers" style="width: 250px;" />
                <span id="chargeEventSummaryDateRange"></span>
            </div>
        </div>
        <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="alertInfo">No messages found</div>
        <div id="grid" style="margin-bottom: 20px;"></div>

    </div>

    <div class="modal fade" id="detail-modal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" style="width: 1050px;">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #F5F5F5;">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Message Detail</h4>
                </div>
                <div class="modal-body" id="messageDetailContainer"></div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        var pEmail = <%= this.Model.Email == null ? "null" : "'" + this.Model.Email + "'" %>;
        var pMessageId = <%= this.Model.MessageDetail == null ? "null" : "'" + this.Model.MessageDetail + "'" %>;
        var messagesDateRangeWidget;

        $(function () {

            messagesDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("chargeEventSummaryDateRange",
               new AccurateAppend.Ui.DateRangeWidgetSettings(
                   [
                       AccurateAppend.Ui.DateRangeValue.Last24Hours,
                       AccurateAppend.Ui.DateRangeValue.Last7Days,
                       AccurateAppend.Ui.DateRangeValue.Last30Days,
                       AccurateAppend.Ui.DateRangeValue.Custom
                   ],
                   AccurateAppend.Ui.DateRangeValue.Last24Hours,
                   [
                       viewModel.renderMessageSummaryGrid
                   ]));

            var autocomplete = $("#siteUsers").kendoAutoComplete({
                minLength: 3,
                placeholder: "Search by email address...",
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: "<%= this.Url.BuildFor<MessageController>().GetRecentMessageRecipientsJson() %>?applicationid=" + $("#ApplicationId").val()
                        }
                    }
                },
                height: 370,
                change: function () {
                    console.log('autocomplete change firing');
                    if (this.value() === '') {
                        pEmail = null;
                        messagesDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last24Hours);
                    } else {
                        pEmail = this.value();
                        messagesDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
                        autocomplete.value(pEmail);
                    }
                }
            }).data("kendoAutoComplete");

            if (pEmail != null) {
                autocomplete.value(pEmail);
                messagesDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
            }

            $("#ApplicationId").bind('change', function () {
                AccurateAppend.Ui.ApplicationId.set();
                messagesDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last24Hours);
            });

            AccurateAppend.Ui.ApplicationId.load();
            messagesDateRangeWidget.refresh();
            setInterval(viewModel.renderMessageSummaryGrid, 120000);

            if (pMessageId != null) {
                messageView.displayDetail(pMessageId);
            }

        });

        var viewModel = {
            renderMessageSummaryGrid: function () {
                var grid = $('#grid').data('kendoGrid');
                if (grid !== undefined && grid !== null) {
                    grid.dataSource.read();
                } else {
                    $("#grid").kendoGrid({
                        dataSource: {
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = { applicationid: $("#ApplicationId").val(), startdate: moment(messagesDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'), enddate: moment(messagesDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'), status: $("#statuses").val(), email: pEmail }
                                    $.ajax({
                                        url: "<%= this.Url.BuildFor<MessageController>().GetMessagesJson() %>",
                                        dataType: 'json',
                                        type: 'GET',
                                        data: data,
                                        success: function (result) {
                                            options.success(result);
                                        }
                                    });
                                }
                            },
                            schema: {
                                type: 'json',
                                data: "Data",
                                total: function (response) {
                                    return response.Data.length;
                                },
                                model: {
                                    fields: {
                                        Id: { type: "number" },
                                        SendFrom: { type: "string" },
                                        SendTo: {},
                                        Subject: { type: "string" },
                                        Status: { type: "string" },
                                        CreatedDate: { type: "string" },
                                        ModifiedDate: { type: "string" }
                                    }
                                }
                            },
                            pageSize: 20,
                            change: function () {
                                if (this.data().length <= 0) {
                                    $("#alertInfo").show();
                                    $("#grid").hide();
                                } else {
                                    $("#alertInfo").hide();
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
                        groupable: true,
                        columns: [
                            { field: "Id", title: "Id", groupable: false },
                            { field: "SendTo", title: "To", template: kendo.template($("#SendToTemplate").html()), groupable: false },
                            { field: "Subject", title: "Subject", template: kendo.template($("#SubjectTemplate").html()) },
                            { field: "Status", title: "Status" },
                            { field: "CreatedDate", title: "Date Created", width: "160px", groupable: false },
                            { field: "ModifiedDate", title: "Date Modified", width: "160px", groupable: false },
                            { command: { text: "View Details", click: viewDetails }, title: " ", width: "130px" }
                        ],
                    });
                }
            }
        }

        function reset() {
            history.pushState(null, "Message", "<%= this.Url.BuildFor<MessageController>().ToIndex() %>");
            window.location.replace("<%= this.Url.BuildFor<MessageController>().ToIndex() %>");
        }

        function viewDetails(e) {
            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            messageView.displayDetail(dataItem.Detail);
        }

        var messageView = {
            resend: function(url) {
                history.pushState(null, "Resend Message", url);
                window.location.replace(url);
            },
            clearPoison: function (url) {
                if (window.confirm('Are you sure you want to mark this as sent?')) {
                    $.getJSON(url, function (data) {
                        $('#detail-modal').modal('hide');
                        viewModel.renderMessageSummaryGrid();
                    });
                }
            },
            displayDetail: function (url) {
                $.getJSON(url, function (data) {
                    var template = kendo.template($("#DetailTemplate").html());
                    $('#messageDetailContainer').html(template(data));
                });
                $('#detail-modal').modal('show');
            }
        };


    </script>

    <script type="text/x-kendo-template" id="SubjectTemplate">
        #if(Subject.search("Job Complete - JobId: ") >= 0){
            var jobid = parseInt(Subject.replace("Job Complete - JobId: ",""));
            if(jobid != NaN)#
                Job Complete - JobId: <a href="/JobProcessing/Summary/?jobid=#= jobid #">#= jobid #</a>
        #} else if(Subject.search("Order Complete - OrderID: ") >= 0){
            var orderid = parseInt(Subject.replace("Order Complete - OrderID: ",""));
            if(orderid != NaN)#
                Order Complete - OrderID: <a href="/Deals/?orderid=#= orderid #">#= orderid #</a>
        #} else if(Subject.search("FTP Job Submitted - JobId: ") >= 0){
            var orderid = parseInt(Subject.replace("FTP Job Submitted - JobId: ",""));
            if(orderid != NaN)#
                Order Complete - OrderID: <a href="/Deals/?orderid=#= orderid #">#= orderid #</a>
        #} else {#
            #= Subject #
        #}#
    </script>

    <script type="text/x-kendo-template" id="SendToTemplate">
        #$.each(SendTo, function( index, value ) {#
            <p style="margin-bottom:3px;">#= value.Address #</p>
        #});#
    </script>

    <script type="text/x-kendo-template" id="DetailTemplate">
        #if(CanResend == true) {#
        <button type="button" class="btn btn-default btn-sm" onclick="javascript:messageView.resend('#: Resend #')"><span class="fa fa-repeat"></span>Resend</button>
        #}#
        # if(CanClear == true) {#
        <button type="button" class="btn btn-default btn-sm" onclick="javascript:messageView.clearPoison('#: ClearPoison #')"><span class="fa fa-check"></span>Mark Sent</button>
        #}#
        <table class="table table-bordered" style="margin-top: 10px;">
            <tr>
                <th style="width: 100px;">Id</th>
                <td style="width: 600px;">#= Id #</td>
            </tr>
            <tr>
                <th>Private Link</th>
                <td><input class=form-control" value="#=Detail #" onclick="this.select();" style="width: 98%; padding: 3px 3px 3px 0; border: none;" /></td>
            </tr>
            <tr>
                <th style="width: 100px;">Created</th>
                <td style="width: 600px;">#= kendo.toString(kendo.parseDate(CreatedDate), "g") #</td>
            </tr>
            <tr>
                <th style="width: 100px;">Last Attempt</th>
                <td style="width: 600px;">#= kendo.toString(kendo.parseDate(ModifiedDate), "g") #</td>
            </tr>
            <tr>
                <th style="width: 100px;">From</th>
                <td style="width: 600px;">#= SendFrom #</td>
            </tr>
            <tr>
                <th>To</th>
                <td>#$.each(SendTo, function( index, value ) {#
                    <p style="margin-bottom: 3px;">#= value #</p>
                    #});#
                </td>
            </tr>
            #if(BccTo.length > 0) {#
            <tr>
                <th>Bcc</th>
                <td>#$.each(BccTo, function( index, value ) {#
                    <p style="margin-bottom: 3px;">#= value #</p>
                    #});#
                </td>
            </tr>
            #}#
            #if(Attachments.length > 0) {#
            <tr>
                <th>Attachments</th>
                <td>
                    #$.each(Attachments, function( index, attachment ) {#
                        <div class="panel panel-default">
                            <div class="panel-heading"><span class="fa fa-file" style="margin-right: 5px;"></span>#= attachment.SendFileName #</div>
                            <div class="panel-body">
                                <p style="margin-bottom: 3px;"><span style="font-weight: bold; margin-right: 5px;">System Path:</span>#= attachment.FileName #</p>
                                #if(attachment.HasContentType == true) {#
                                <p style="margin-bottom: 7px;"><span style="font-weight: bold; margin-right: 5px;">Type:</span>#= attachment.ContentType #</p>
                                #}#
                                #if(attachment.Exists == false) {#
                                <div class="alert alert-danger" style="margin-bottom: 5px; padding: 5px;">File missing!</div>
                                #}#
                            </div>
                        </div>
                    #});#
                </td>
            </tr>
            #}#
            <tr>
                <th>Status</th>
                <td>#= Status #</td>
            </tr>
            <tr>
                <th>Subject</th>
                <td>#= Subject #</td>
            </tr>
            <tr>
                <th>Body</th>
                <td>
                    <iframe src='#= View #' width=800 height=500></iframe>
                </td>
            </tr>
        </table>

    </script>

</asp:Content>
