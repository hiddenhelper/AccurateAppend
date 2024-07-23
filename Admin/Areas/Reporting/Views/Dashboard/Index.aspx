<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Queries" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Reporting.Controllers" %>
<%@ Import Namespace="AccurateAppend.Security" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Dashboard
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div>

<div class="row" style="padding-right: 20px;">
  <div class="pull-right">
    <%: Html.SiteDropDown(null) %>
  </div>
</div>

<div class="row" style="padding: 0 20px 0 20px;">

<ul class="nav nav-tabs" role="tablist" id="tabs">
  <li role="presentation" class="active">
    <a href="#operating" aria-controls="operating" role="tab" data-toggle="tab">Operating Metrics</a>
  </li>
  <li role="presentation">
    <a href="#leads" aria-controls="leads" role="tab" data-toggle="tab">Leads</a>
  </li>
  <li role="presentation">
    <a href="#clients" aria-controls="clients" role="tab" data-toggle="tab">Clients</a>
  </li>
  <li role="presentation">
    <a href="#processing" aria-controls="processing" role="tab" data-toggle="tab">Job Processing</a>
  </li>
  <li role="presentation">
    <a href="#webservices" aria-controls="webservices" role="tab" data-toggle="tab" style="<%= User.Identity.Is2020ConnectAdmin() && User.Identity.IsAccurateAppendAdmin() ? string.Empty : "display: none" %>">Web Services</a>
  </li>
  <li role="presentation">
    <a href="#website-analytics" aria-controls="website-analytics" role="tab" data-toggle="tab" style="<%= User.Identity.Is2020ConnectAdmin() && User.Identity.IsAccurateAppendAdmin() ? string.Empty : "display: none" %>">Website Analytics</a>
  </li>
  <li role="presentation">
    <a href="#adminUserActivity" aria-controls="adminUserActivity" role="tab" data-toggle="tab" style="<%= User.Identity.Is2020ConnectAdmin() && User.Identity.IsAccurateAppendAdmin() ? string.Empty : "display: none" %>">Admin User Activity</a>
  </li>
  <li role="presentation">
    <a href="#databox" aria-controls="databox" role="tab" data-toggle="tab">Databox</a>
  </li>
</ul>

<div class="tab-content">

<div class="tab-pane active" id="operating" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Operating Metrics</h3>
    </div>
    <div class="panel-body">
      <div id="DealMetricOverviewReportGrid"></div>
      <div id="RevenueMetricChartLoading"></div>
      <div id="RevenueMetricChart" style="margin-top: 20px;"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Revenue By Agent</h3>
    </div>
    <div class="panel-body">
      <div id="AgentMetricOverviewGrid" class="k-grid k-widget"></div>
    </div>
  </div>

  <% if (User.Identity.IsSuperUser())
     { %>
    <div class="panel panel-default">
      <div class="panel-heading">
        <h3 class="panel-title">Revenue & MRR Histogram</h3>
      </div>
      <div class="panel-body">
        <div id="MrrMetricOverviewGrid" class="k-grid k-widget"></div>
      </div>
    </div>
  <% } %>
</div>

<div class="tab-pane" id="leads" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Lead Metrics</h3>
    </div>
    <div class="panel-body">
      <div id="leadSumamryChart"></div>
      <div id="LeadMetricOverviewReportGrid" style="margin-top: 20px;"></div>
      <div id="LeadSummaryChartLoading"></div>
      <h4 style="margin-top: 15px;">Organic</h4>
      <div id="LeadChannelMetricOverviewGrid_Organic" style="margin-top: 20px;" class="k-grid k-widget"></div>
      <h4 style="margin-top: 15px;">Direct</h4>
      <div id="LeadChannelMetricOverviewGrid_Direct" style="margin-top: 20px;" class="k-grid k-widget"></div>
      <h4 style="margin-top: 15px;">Referral</h4>
      <div id="LeadChannelMetricOverviewGrid_Referral" style="margin-top: 20px;" class="k-grid k-widget"></div>
      <h4 style="margin-top: 15px;">Nation Builder</h4>
      <div id="LeadChannelMetricOverviewGrid_NationBuilder" style="margin-top: 20px;" class="k-grid k-widget"></div>
    </div>
  </div>

</div>

<div class="tab-pane" id="clients" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Recent Deals</h3>
    </div>
    <div class="panel-body">
      <div id="recentDealsGrid"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Revenue By Client</h3>
    </div>
    <div class="panel-body">
      <div id="clientDealMetricsGrid"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Processing Activity By Subscriber</h3>
    </div>
    <div class="panel-body">
      <div class="form-group">
        <select id="clientProcessingMetricsGridToolbarSource" class="form-control pull-left" style="margin-bottom: 15px; width: 150px;">
          <option value="<%: JobMetricsController.Source.Batch %>" selected="selected">Batch</option>
          <option value="<%: JobMetricsController.Source.Api %>">Api</option>
        </select>
      </div>
      <div class="clearfix"></div>
      <div id="clientProcessingMetricsGrid"></div>
    </div>
  </div>
</div>

<div class="tab-pane" id="processing" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Job Queue Activity - Last 24 Hours</h3>
    </div>
    <div class="panel-body">
      <div id="jobQueueActivityLast24HoursLoading"></div>
      <div id="jobQueueActivityLast24Hours"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Subscriber Activity - Last 12 Months</h3>

    </div>
    <div class="panel-body">
      <div id="subscriberProcessingHistoryLoading"></div>
      <div id="subscriberProcessingHistory"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Subscriber Activity - Month Over Month</h3>

    </div>
    <div class="panel-body">
      <div style="margin: 20px; width: 150px;">
        <select id="Source" class="form-control" style="height: 20px;">
          <option value="<%: JobMetricsController.Source.All %>" selected="selected">All</option>
          <option value="<%: JobMetricsController.Source.Batch %>">Batch</option>
          <option value="<%: JobMetricsController.Source.Api %>">Api</option>
        </select>
      </div>
      <div id="subscriberActivityMonthComparisonChartLoading"></div>
      <div id="subscriberActivityMonthComparison"></div>
    </div>
  </div>

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Processing Metrics</h3>
    </div>
    <div class="panel-body">
      <div id="ProcessingMetricOverviewReportGrid"></div>
    </div>
  </div>

</div>

<div class="tab-pane" id="webservices" style="margin-top: 20px;">

  <div class="row">

    <div class="col-md-4">

      <div class="panel panel-default">
        <div class="panel-heading">Activity by user - Last 24 Hours</div>
        <div class="panel-body">
          <div id="transactionsByUser"></div>
        </div>
      </div>

    </div>

    <div class="col-md-8">

      <div class="panel panel-default">
        <div class="panel-heading">Execution Time by Date (milliseconds)</div>
        <div class="panel-body">
          <%: Html.Kendo().Grid<ServiceCallsStatistics>()
                .Name("statisticsGrid")
                .Columns(columns =>
                {
                  columns.Bound(c => c.TransactionDate).Title("Date").ClientTemplate("#= kendo.toString(TransactionDate, \"MM/dd/yyyy\") #").Width(140);
                  columns.Bound(c => c.Calls).Width(190);
                  columns.Bound(c => c.Min);
                  columns.Bound(c => c.Max).Width(110);
                  columns.Bound(c => c.Median).Width(110);
                }).DataSource(datasource => datasource.Ajax().Read("WebServiceStatistics", "ApiMetrics", new {userId = Model, area = "Reporting"}))
  %>
        </div>
      </div>

    </div>

  </div>

  <div class="row">

    <div class="col-md-7">

      <div class="panel panel-default">
        <div class="panel-body">
          <%= Html.Kendo().Chart<ServiceOperationByCount>()
                .Name("operationsChart")
                .Title(title => title
                  .Text("Most Called Operations - TODAY")
                  .Position(ChartTitlePosition.Top))
                .Legend(legend => legend
                  .Visible(true)
                )
                .DataSource(datasource => { datasource.Read(read => read.Action("WebServiceByOperation", "ApiMetrics", new {userId = Model, area = "Reporting"})); })
                .Series(series => series.Pie(model => model.Calls, model => model.Operation))
                .Tooltip(tooltip => tooltip
                  .Visible(true)
                  .Format("{0} calls")
                )
  %>
        </div>
      </div>

    </div>

    <div class="col-md-5">

      <div class="panel panel-default">
        <div class="panel-body">
          <%= Html.Kendo().Chart<ServiceCallByExecutionTime>()
                .Name("responseChart")
                .Title("Distribution by Response Time in seconds - TODAY")
                .Legend(legend => legend
                  .Position(ChartLegendPosition.Top)
                )
                .DataSource(ds => ds.Read(read => read.Action("WebServiceByResponseTime", "ApiMetrics", new {userId = Model, area = "Reporting"})))
                .ChartArea(chartArea => chartArea
                  .Background("transparent")
                )
                .Series(series => { series.Bar(model => model.Calls).Name("Calls"); })
                .CategoryAxis(axis => axis
                  .Categories(model => model.Seconds)
                  .MajorGridLines(lines => lines.Visible(false))
                )
                .ValueAxis(axis => axis
                  .Numeric().Labels(labels => labels.Format("{0}"))
                  .Line(line => line.Visible(false))
                  .AxisCrossingValue(-10)
                )
                .Tooltip(tooltip => tooltip
                  .Visible(true)
                  .Format("{0} Calls")
                )
  %>
        </div>
      </div>

    </div>

  </div>

</div>

<div class="tab-pane" id="website-analytics" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">www.accurateappend.com</h3>
    </div>
    <div class="panel-body">
      <div style="padding: 56% 0 0 0; position: relative;">
        <iframe src="https://app.databox.com/datawall/06631efc225d32c06b5dbbec5d5d116a05bcd36ae" style="height: 100%; left: 0; position: absolute; top: 0; width: 100%;" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
      </div>
    </div>
  </div>

</div>

<div class="tab-pane" id="adminUserActivity" style="margin-top: 20px;">

  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">Admin User Activity - Last 7 Days</h3>
    </div>
    <div class="panel-body">
      <div class="form-group">
        <select id="adminUserActivityUserSummaryGridToolbarSource" class="form-control pull-left" style="margin-bottom: 15px; width: 150px;">
          <option value="74A0CC9B-DE78-40E3-A556-0732AADF4C46" selected="selected">Steve</option>
        </select>
      </div>
      <div class="clearfix"></div>
      <div id="adminUserActivityUserSummary"></div>
    </div>
  </div>

</div>

<div class="tab-pane" id="databox" style="margin-top: 20px;">
  
  <!-- to remove black border: use iframe "embed" code but replace src with link form "shareable link"-->
  <div style="padding:63% 0 0 0; position:relative;"><iframe id="databox_operations_scorecard" src="https://app.databox.com/datawall/db1a884c9f98922659259e25006840d905e8a0629" style="position:absolute; top:0; left:0; width:100%; height:100%;" frameborder="0"></iframe></div>

</div>

</div>

</div>

</div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    $(function() {

    })

  </script>

  <style>
        
    #LeadMetricOverviewReportGrid .k-grid-header, #DealMetricOverviewReportGrid .k-grid-header, #ProcessingMetricOverviewReportGrid .k-grid-header, #clientMetricsGrid .k-grid-header { padding: 0 !important; }

    #LeadMetricOverviewReportGrid .k-grid-content, #DealMetricOverviewReportGrid .k-grid-content, #ProcessingMetricOverviewReportGrid .k-grid-content, #clientMetricsGrid .k-grid-content { overflow-y: visible; }

  </style>

  <script type="text/x-kendo-template" id="toolbarTemplate">
      <div class="toolbar">
      <select id="Source" class="form-control" style="height: 20px;">
      <option value="<%: JobMetricsController.Source.All %>" selected="selected">All</option>
                <option value="<%: JobMetricsController.Source.Batch %>">Batch</option>
                <option value="<%: JobMetricsController.Source.Api %>">Api</option>
            </select>
        </div>
    </script>

  <%-- <script type="text/x-kendo-template" id="clientProcessingMetricsGridToolbarTemplate">
        <div class="toolbar">
            <select id="clientProcessingMetricsGridToolbarSource" class="form-control pull-left" style="width: 150px; height: 20px;">
                <option value="<%: JobMetricsController.Source.Batch %>" selected="selected">Batch</option>
                <option value="<%: JobMetricsController.Source.Api %>">Api</option>
            </select>
        </div>
    </script>--%>

  <%-- <script type="text/x-kendo-template" id="adminUserActivityUserSummaryGridToolbarTemplate">
        <div class="toolbar">
            <select id="adminUserActivityUserSummaryGridToolbarSource" class="form-control pull-left" style="width: 150px; height: 20px;">
                <option value="74A0CC9B-DE78-40E3-A556-0732AADF4C46" selected="selected">Steve</option>
                <option value="E7467CD2-8B4E-4BBA-B4FB-AAA11A8D7C23">Alec</option>
            </select>
        </div>
    </script>--%>

  <script src="<%= Url.Content("~/Areas/Reporting/Scripts/dashboard.js") %>" type="text/javascript"></script>

  <script id="todayTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(Today, "c0")#
            # break; #
        # default:#
            #= kendo.toString(Today, "n0")#
        # }#
    </script>
  <script id="yesterdayTemplate" type="text/x-kendo-tmpl">
       # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(Yesterday, "c0")#
            # break; #
        # default:#
            #= kendo.toString(Yesterday, "n0")#
        # }#
    </script>
  <script id="last7Template" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(Last7, "c0")#
            # break; #
        # default:#
            #= kendo.toString(Last7, "n0")#
        # }#
    </script>
  <script id="currentMonthTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(CurrentMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(CurrentMonth, "n0")#
        # }#
    </script>
  <script id="samePeriodLastMonthTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(SamePeriodLastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(SamePeriodLastMonth, "n0")#
        # }#
    </script>
  <script id="LastMonthTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(LastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(LastMonth, "n0")#
        # }#
    </script>
  <script id="previousToLastMonthTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'AverageDealAmount': #
        # case 'TotalRevenue': #
        # case 'SubscriberRevenue': #
        # case 'NonSubscriberRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'NationBuilderRevenue': #
        # case 'SelfServiceRevenue': #
            #= kendo.toString(PreviousToLastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(PreviousToLastMonth, "n0")#
        # }#
    </script>

</asp:Content>