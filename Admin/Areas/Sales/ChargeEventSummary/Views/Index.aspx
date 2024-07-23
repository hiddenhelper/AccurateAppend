<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<ClientModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Charge Events
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div>

        <div class="row" style="padding-right: 20px;">
            <div class="pull-right">
                <%: this.Html.SiteDropDown(null) %>
            </div>
        </div>

        <div class="row" style="padding: 0 20px 0 20px;">

            <ul class="nav nav-tabs" id="tabs">
                <li class="active"><a href="#deals" data-toggle="tab">Charge Events</a></li>
            </ul>

            <div class="tab-content">

                <div class="tab-pane active" id="deals" style="margin-top: 20px;">

                    <div class="row">
                        <div class="pull-right" style="padding: 15px 20px;">
                            <button type="button" class="btn btn-default" onclick="javascript:reset();"><span class="fa fa-refresh"></span>Reset</button>
                            <input id="siteUsers" style="width: 250px;" />
                            <span id="chargeEventSummaryDateRange"></span>
                            <select id="statuses" style="width: 250px; display: inline;" class="form-control"></select>
                        </div>
                    </div>
                    <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="chargeEventSummaryGridInfo">No charge events found</div>
                    <div id="chargeEventSummaryGrid" style="margin-bottom: 20px;"></div>

                </div>

            </div>

        </div>

    </div>
    

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        var pUserId = <%= this.Model.UserId == null ? "null" : "'" + this.Model.UserId + "'" %>;
        var pEmail = <%= this.Model.Email == null ? "null" : "'" + this.Model.Email + "'" %>;
        var pDealId = <%= this.Model.DealId == null ? "null" : this.Model.DealId.ToString() %>;

        var ChargeEventDateRangeWidget;

        $(function() {
            
            ChargeEventDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("chargeEventSummaryDateRange",
               new AccurateAppend.Ui.DateRangeWidgetSettings(
                   [
                       AccurateAppend.Ui.DateRangeValue.Last24Hours,
                       AccurateAppend.Ui.DateRangeValue.Last7Days,
                       AccurateAppend.Ui.DateRangeValue.Last30Days,
                       AccurateAppend.Ui.DateRangeValue.Custom
                   ],
                   AccurateAppend.Ui.DateRangeValue.Last7Days,
                   [
                       viewModel.renderStatusSelect,
                       viewModel.renderChargeEventSummaryGrid
                   ]));

            var autocomplete = $("#siteUsers").kendoAutoComplete({
                minLength: 3,
                dataTextField: "Email",
                placeholder: "Search by email address...",
                dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: "/Clients/SearchClients/List?activeWithin=<%: DateRange.LastYear %>&applicationid=" + $("#ApplicationId").val()
                        }
                    }
                },
                height: 370,
                change: function () {
                    console.log('autocomplete change firing');
                    if (this.value() === '') {
                        pEmail = null;
                        ChargeEventDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
                    } else {
                        pEmail = this.value();
                        ChargeEventDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
                        autocomplete.value(pEmail);
                    }
                }
            }).data("kendoAutoComplete");

            if (pEmail != null) {
                autocomplete.value(pEmail);
                ChargeEventDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
            } else {
                pEmail = '';
                autocomplete.value(pEmail);
            }

            AccurateAppend.Ui.ApplicationId.load();

            $("#ApplicationId").bind('change', function () {
                AccurateAppend.Ui.ApplicationId.set();
                ChargeEventDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
            });

            $("#statuses").bind('change', function () {
                viewModel.renderChargeEventSummaryGrid();
            });

            ChargeEventDateRangeWidget.refresh();
            setInterval(viewModel.renderChargeEventSummaryGrid, 60000);
        });

        var viewModel = {
            renderChargeEventSummaryGrid: function () {
                var grid = $("#chargeEventSummaryGrid").data("kendoGrid");
                if (grid !== undefined && grid !== null) {
                    grid.dataSource.read();
                } else {
                    $("#chargeEventSummaryGrid").kendoGrid({
                        dataSource: {
                            type: "json",
                            transport: {
                                read: function (options) {
                                    var data = { applicationid: $("#ApplicationId").val(), startdate: moment(ChargeEventDateRangeWidget.getStartDate()).format('YYYY-MM-DD'), enddate: moment(ChargeEventDateRangeWidget.getEndDate()).format('YYYY-MM-DD'), status: $("#statuses").val(), email: pEmail }
                                    if (pUserId != null) data.userid = pUserId;
                                    if (pDealId != null) data.dealId = pDealId;
                                    $.ajax({
                                        url: '<%= this.Url.BuildFor<ChargeEventSummaryController>().GetChargeEventsJson() %>',
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
                                }
                            },
                            pageSize: 20,
                            change: function () {
                                if (this.data().length <= 0) {
                                    $("#chargeEventSummaryGridInfo").show();
                                    $("#chargeEventSummaryGrid").hide();
                                } else {
                                    $("#chargeEventSummaryGridInfo").hide();
                                    $("#chargeEventSummaryGrid").show();
                                }
                            }
                        },
                        scrollable: false,
                        sortable: true,
                        pageable: {
                            input: true,
                            numeric: false
                        },
                        columns: [
                            { field: "Id", title: "Id", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, filterable: false, groupable: false },
                            { field: "UserName", title: "User", filterable: false, headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: left;" }, template: kendo.template("<a href=\"#= Links.UserDetail#\">#=UserName#</a>") },
                            { field: "EventDate", title: "Date Added", filterable: false, headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { field: "Status", title: "Status", filterable: false, headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } },
                            { field: "Amount", title: "Amount", filterable: false, headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, format: "{0:c2}" },
                            {
                                command: {
                                    text: "View Details",
                                    click: function (e) {
                                        var g = $("#chargeEventSummaryGrid").data("kendoGrid");
                                        var dataItem = g.dataItem($(e.target).closest("tr"));
                                        viewModel.displayChargeEventDetail(dataItem.Data.TransactionDetail, dataItem.Id);
                                    }
                                },
                                title: " ",
                                width: "140px"
                            }
                        ]
                    });
                }
            },
            renderStatusSelect: function (callback) {
                $("#statuses option").remove();
                $.getJSON('<%= this.Url.BuildFor<ChargeEventSummaryController>().GetChargeEventsStatusesJson() %>',
                    {
                        applicationid: $("#ApplicationId").val(),
                        startdate: moment(ChargeEventDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                        enddate: moment(ChargeEventDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'),
                        status: $("#statuses").val(),
                        email: pEmail
                    },
                    function(data) {
                        $("#statuses").append("<option value='All' selected=\"selected\">- All Status ------</option>");
                        $.each(data, function(index, status) {
                            $("#statuses").append("<option value=" + status.Description + ">" + status.Description + " (" + status.Cnt + ")" + "</option>");
                        });
                        if (callback) callback.call();
                    }
                );
            },
            displayChargeEventDetail: function (chargeEventDetail, chargeEventId) {
                $.get(chargeEventDetail, function (data) {
                    $('<div id="chargeEventDetails" style="padding: 20px;"></div>').kendoWindow({
                        title: "Charge Event Detail: " + chargeEventId,
                        resizable: false,
                        modal: true,
                        viewable: false,
                        content: {
                            template: kendo.template($("#chargeEventDetailTemplate").html())(data)
                        },
                        width: "800px",
                        position: { top: "200px", left: "600px" },
                        scrollable: false
                    }).data("kendoWindow").open();
                });
            }
        }

        function reset() {
            history.pushState(null, 'ChargeEvents', '<%= this.Url.BuildFor<ChargeEventSummaryController>().ToIndex() %>');
            window.location.replace('<%= this.Url.BuildFor<ChargeEventSummaryController>().ToIndex() %>');
        }

    </script>

    <script type="text/x-kendo-template" id="chargeEventDetailTemplate">
    <div class="panel panel-default" style="margin-top: 10px;">
    <table class="table table-bordered" style="margin-top: 10px;">
        <tr>
            <th style="width: 200px;">Email</th>
            <td style="width: 560px;"><a href="#: UserDetail #">#= UserName #</a></td>
        </tr>
        <tr>
            <th>Id</th>
            <td>#= Id #</td>
        </tr>
        <tr>
            <th>Amount</th>
            <td>$#= Amount #</td>
        </tr>
        <tr>
            <th>Status</th>
            <td>#= Status #</td>
        </tr>
        <tr>
            <th>OrderId</th>
            <td><a href="#: OrderDetail #">#= OrderId #</a></td>
        </tr>
        <tr>
            <th>Gateway Response</th>
            <td>
                <table class="table">
                    <tr>
                        <th style="width: 150px;">TransactionId</th>
                        <td>#= TransactionId #</td>
                    </tr>
                    <tr>
                        <th>TransactionType</th>
                        <td>#= TransactionType #</td>
                    </tr>
                    <tr>
                        <th>AuthorizationCode</th>
                        <td>#= AuthorizationCode #</td>
                    </tr>
                    <tr>
                        <th>Message</th>
                        <td>#= Message #</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <th>Card Info</th>
            <td>
                <table class="table">
                    <tr>
                        <th style="width: 150px;">Name</th>
                        <td>#= FullName #</td>
                    </tr>
                    <tr>
                        <th>Address</th>
                        <td>#= Address #</td>
                    </tr>
                    <tr>
                        <th>City, State, Zip</th>
                        <td>#= City #, #= State # #= ZipCode #</td>
                    </tr>
                    <tr>
                        <th>Card Info</th>
                        <td>XXXX-XXXX-XXXX-#= DisplayValue #, #= (ExpirationDate.length === 6 ? ExpirationDate.substring(0, 2) + "/" + ExpirationDate.substring(2, 6) : "") #</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
    </script>

</asp:Content>

