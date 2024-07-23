<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Guid>" %>
<%@ Import Namespace="DomainModel.Queries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Web Services Reporting - <%: this.Model %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">

        
            <div class="panel panel-default">
                <div class="panel-body">
                    <%= this.Html.Kendo().Chart<ServiceOperationByCount>()
                        .Name("operationsChart")
                                .Title(title => title
                                            .Text("Most Called Operations - LAST 30 DAYS")
                                            .Position(ChartTitlePosition.Top))
                                .Legend(legend => legend
                                    .Visible(true)
                                )
                                .DataSource(datasource =>
                                {
                                    datasource.Read(read => read.Action("WebServiceByOperation", "ApiMetrics", new { area = "Reporting", userId = this.Model, startDate = DateTime.UtcNow.AddDays(-30) }));
                                })
                                .Series(series => series.Pie(model => model.Calls, model =>model.Operation))
                                .Tooltip(tooltip => tooltip
                                    .Visible(true)
                                    .Format("{0} calls")
                                )
                            %>
                </div>
            </div>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        $(function () {

            $("#transactionsByUser").kendoGrid({
                autobind: true,
                dataSource: {
                    autobind: false,
                    type: "json",
                    transport: {
                        read: function (options) {
                            $.ajax({
                                url: '<%= this.Url.Action("Query", "ApiUsage", new {area = "Clients", id = this.Model, startDate = DateTime.UtcNow.Date, endDate = DateTime.UtcNow.Date}) %>',
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
                    pageSize: 10
                },
                scrollable: false,
                sortable: false,
                pageable: {
                    input: true,
                    numeric: false
                },
                groupable: false,
                columns: [
                    { field: "Email", title: "Username", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" }, template: kendo.template("<a href=\"/Users/Detail?userid=#=UserId#\">#=Email#</a>") },
                    { field: "Count", title: "Count", headerAttributes: { style: "text-align: center;" }, attributes: { style: "text-align: center;" } }
                    //{ command: { text: "View Details", click: viewDetails }, title: " ", width: "140px" }
                ]
            });
        });

    </script>

</asp:Content>
