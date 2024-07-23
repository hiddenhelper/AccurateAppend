<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Users
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div>

        <div class="row">
            <div class="col-md-7">
                <button class="btn btn-default" id="downloadUsers"><span class="fa fa-download"></span>Download Users</button>
            </div>
            <div class="col-md-5" style="text-align: right;">
                <a href="#" class="k-button k-button-icontext" id="clearFilter"><span class="k-icon k-i-plus"></span>Clear Filter</a>
                <%= Html.Kendo().DropDownList()
                                .Name("dateRange")
                                .DataTextField("Text")
                                .DataValueField("Value")
                                .BindTo(AccurateAppend.Core.EnumExtensions.ToLookup<DateRange>().Select(a => new SelectListItem { Text = a.Key, Value = ((DateRange)a.Value).ToString() }))
                                .Value("Last7Days")
                %>
                <%: Html.SiteDropDown(null) %>
            </div>
        </div>

        <div id="grid" style="margin-bottom: 20px; margin-top: 10px;"></div>

    </div>    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    
    <script src="<%= Url.Content("~/Scripts/moment.min.js") %>" type="text/javascript"> </script>

    <script type="text/javascript">

        $(function () {

            updateDateRangeDropDown();
            renderLeadSummaryGrid();
            loadApplicationId();

            $("#ApplicationId").bind('change', function() {
                setApplicationId();
                renderLeadSummaryGrid();
            });
            $("#dateRange").bind('change', renderLeadSummaryGrid);
            $('#downloadUsers').click(function () { window.location.replace('/Clients/UserSummary/Download?dateRange=' + $("#dateRange").val() + '&applicationid=' + $("#ApplicationId").val()); });
        });

        function updateDateRangeDropDown() {
            var value = $.cookie("clientDateRangeState");
            if (value) $("#dateRange").val(value);
        }

        function renderLeadSummaryGrid() {

            $.cookie("dateRangeState", $("#dateRange").val());

            var dataSource = new kendo.data.DataSource({
                autobind: false,
                type: "json",
                transport: {
                    read: function (options) {
                        $.ajax({
                            url: "/Clients/UserSummary/Read?dateRange=" + $('#dateRange').val() + "&applicationid=" + $('#ApplicationId').val(),
                            dataType: 'json',
                            type: 'GET',
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
                pageSize: 20
            });

            var grid = $('#grid').data('kendoGrid');
            if (grid !== undefined) {
                //grid.dataSource.data(dataSource);
                grid.dataSource.read();
            } else {
                $("#grid").kendoGrid({
                    autobind: false,
                    dataSource: dataSource,
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
                                eq: "Is equal to",
                                neq: "Is not equal to"
                            }
                        }
                    },
                    groupable: true,
                    dataBound: function (e) {
                        var state = kendo.stringify({
                            page: this.dataSource.page(),
                            pageSize: this.dataSource.pageSize(),
                            sort: this.dataSource.sort(),
                            group: this.dataSource.group(),
                            filter: this.dataSource.filter()
                        });
                        $.cookie("clientGridState", state);
                    },
                    columns: [
                        { field: "UserName", title: "Email", filterable: false },
                        { field: "CompositeName", title: "Name", filterable: false },
                        { field: "Location", title: "Location", filterable: false },
                        { field: "Status", title: "Status", filterable: { ui: statusFilter } },
                        { field: "IsSubscriber", title: "Subscriber", filterable: { ui: subscriberFilter }},
                        { field: "LastActivityDescription", title: "Last Activity", filterable: false, width: "120px" },
                        { field: "LifeTimeRevenueDescription", title: "$", filterable: { ui: lifetimeRevenueFilter } },
                        { command: { text: "View Details", click: showDetails }, title: " ", width: "120px" }
                    ]
                });
            }
        }
        
        function subscriberFilter(element) {
            element.kendoDropDownList({
                dataSource: ["True", "False"]
                , optionLabel: "--Select --------"
            });
        }
        
        function statusFilter(element) {
            element.kendoDropDownList({
                dataSource: ["Active", "Inactive"]
                , optionLabel: "--Select --------"
            });
        }
        
        function lifetimeRevenueFilter(element) {
            element.kendoDropDownList({
                dataSource: ["$", "$$", "$$$", "$$$$", "$$$$$"]
                , optionLabel: "--Select --------"
            });
        }
        
        function showDetails(e) {
            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            history.pushState(null, "Clients", "<%= this.Url.Action("Index", "UserSummary", new {Area = "Clients"}) %>");
            window.location.replace(dataItem.DetailUrl);
        }
       
        function loadApplicationId() {
            var v = $.cookie('ApplicationId');
            if (v != '') {
                $('#ApplicationId option[value=' + $.cookie('ApplicationId') + ']').attr('selected', 'selected');
                
            }
        }
        function setApplicationId() {
            $.cookie('ApplicationId', $('#ApplicationId option:selected').val());
        }
    </script>
</asp:Content>
