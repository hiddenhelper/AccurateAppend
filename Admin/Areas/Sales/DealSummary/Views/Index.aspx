<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<DealSummaryViewModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealSummary" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DownloadDeals" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealSummary.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Deals
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div>

    <div class="row" style="padding: 0 20px 15px 0;">
      <div class="pull-right">
        <%: this.Html.SiteDropDown(null) %>
      </div>
    </div>

    <div class="row" style="padding: 0 20px 15px 0;">
      <div class="pull-right">
       
        <span style="margin-right: 7px;">Show Only Incomplete Deals</span><input type="checkbox" checked data-toggle="toggle" data-size="small" id="hideCompleteDeals"/>
        <button type="button" class="btn btn-default" onclick="javascript:reset();"><span class="fa fa-refresh"></span>Reset</button>
        <%--<button type="button" class="btn btn-default" onclick="javascript:openinvoices();"><span class="fa fa-report"></span>Open Invoices</button>--%>
        <a class="btn btn-default" onclick="javascript:downloadDeals();"><span class="fa fa-download"></span>Download</a>
        <input id="siteUsers" style="width: 250px;"/>
        <span id="dealsSummaryDateRange"></span>
        <select id="statuses" style="display: inline; width: 250px;" class="form-control"></select>
      </div>
    </div>

    <div class="row" style="padding: 0 20px 0 20px;">
      <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="dealSummaryGridInfo">No deals found</div>
      <div id="dealSummaryGrid" style="margin-bottom: 20px;"></div>
    </div>

  </div>

  <%--<div class="modal fade" tabindex="-1" role="dialog" id="open-invoices">
    <div class="modal-dialog" role="document" style="width: 80%;">
      <div class="modal-content">
        <div class="modal-body">
          <div style="padding: 63% 0 0 0; position: relative;">
            <iframe src="https://app.databox.com/datawall/f62522644fd4ac3a44b9ebae70bac39905bc9ff88?i" style="height: 100%; left: 0; position: absolute; top: 0; width: 100%;" frameborder="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>--%>
  
  <%= @Html.HiddenFor(a => a.ApplicationId) %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <!-- http://www.bootstraptoggle.com/ -->
  <link href="//gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
  <script src="//gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>

  <script type="text/javascript">

    var pUserId = <%= this.Model.UserId == null ? "null" : "'" + this.Model.UserId + "'" %>;
    var pOrderId = null;
    var pDealId = null;
    var pDealStatus = <%= this.Model.Status == null ? "null" : "'" + this.Model.Status + "'" %>;
    var pEmail = <%= this.Model.Email == null ? "null" : "'" + this.Model.Email + "'" %>;

    var DealsDateRangeWidget;

    $(function() {

      DealsDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("dealsSummaryDateRange",
        new AccurateAppend.Ui.DateRangeWidgetSettings(
          [
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
                AccurateAppend.Ui.DateRangeValue.Last60Days,
            AccurateAppend.Ui.DateRangeValue.Last90Days,
            AccurateAppend.Ui.DateRangeValue.Custom
          ],
          AccurateAppend.Ui.DateRangeValue.Last90Days,
          [
            updateStatusSelect,
            viewModel.renderDealSummaryGrid
          ]));
      <% if (this.Model.DateRange.HasValue)
         { %>
      DealsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.<%: this.Model.DateRange %>);
      <% } %>
      var autocomplete = $("#siteUsers").kendoAutoComplete({
        minLength: 3,
        dataTextField: "Email",
        placeholder: "Search by email address...",
        dataSource: {
          transport: {
            read: {
              dataType: "json",
              url: "/Clients/SearchClients/List?activeWithin=<%: DateRange.LastYear %>&applicationid=" +
                $("#ApplicationId").val()
            }
          }
        },
        height: 370,
        change: function() {
          console.log('autocomplete change firing');
          if (this.value() === '') {
            pEmail = null;
            DealsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
          } else {
            pEmail = this.value();
            DealsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last90Days);
              autocomplete.value(pEmail);
            $('#hideCompleteDeals').bootstrapToggle('off');
          }
        }
      }).data("kendoAutoComplete");

      if (pEmail != null) {
        autocomplete.value(pEmail);
        DealsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last90Days);
        $('#hideCompleteDeals').bootstrapToggle('off');
      } else {
        pEmail = '';
        autocomplete.value(pEmail);
      }

      loadApplicationId();

      $("#ApplicationId").bind('change',
        function() {
          setApplicationId();
          if ($('ul#tabs li.active [href$="reports"]').length !== 0) {
            viewModel.renderDealMetricOverviewReportGrid();
            viewModel.renderRevenueMetricChartChart();
            viewModel.renderProcessingMetricOverviewReportGrid();
            viewModel.renderLeadMetricOverviewReportGrid();
            viewModel.renderLeadMetricChart();
          }
          pEmail = '';
          autocomplete.value(pEmail);
          DealsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last7Days);
        });

      $("#statuses").bind('change',
        function() {
          $('#hideCompleteDeals').bootstrapToggle('off');
          viewModel.renderDealSummaryGrid();
        });

      $("#hideCompleteDeals").change(function() {
        viewModel.renderDealSummaryGrid();
      });

      DealsDateRangeWidget.refresh();
      setInterval(viewModel.renderDealSummaryGrid, 60000);

    });

    var viewModel = {
      renderDealSummaryGrid: function() {
        console.log('renderDealSummaryGrid');
        var grid = $("#dealSummaryGrid").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#dealSummaryGrid").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  var data = {
                    applicationid: $("#ApplicationId").val(),
                    startdate: moment(DealsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                    enddate: moment(DealsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'),
                    status: $("#statuses").val(),
                    email: pEmail,
                    nonCompleteOnly: $("#hideCompleteDeals").prop("checked")
                  };
                  if (pUserId != null) data.userid = pUserId;
                  if (pEmail != null) data.email = pEmail;
                  if (pDealStatus != null) data.status = pDealStatus;

                  $.ajax({
                    url: '<%= this.Url.Action("Query", "DealSummary", new {area = "Sales"}) %>',
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
                  $("#dealSummaryGridInfo").show();
                  $("#dealSummaryGrid").hide();
                } else {
                  $("#dealSummaryGridInfo").hide();
                  $("#dealSummaryGrid").show();
                }
              }
            },
            scrollable: false,
            sortable: true,
            //groupable: true,
            pageable: {
              input: true,
              numeric: false
            },
            dataBound: function() {
              $.cookie("DealSummaryGridState",
                kendo.stringify({
                  //page: this.dataSource.page(),
                  //pageSize: this.dataSource.pageSize(),
                  sort: this.dataSource.sort(),
                  group: this.dataSource.group(),
                  //filter: this.dataSource.filter()
                }));
            },
            columns: [
              {
                field: "Id",
                title: "Id",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" },
                template: kendo.template($("#IdTemplate").html()),
                filterable: false,
                groupable: false
              },
              {
                field: "Email",
                title: "User",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" },
                template: kendo.template("<a href=\"#=Links.ClientView#\">#=Email#</a>")
              },
              {
                field: "DateCreated",
                title: "Date Added",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" }
              },
              {
                field: "Status",
                title: "Status",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" }
              },
              {
                field: "AdjustedTotal",
                title: "Amount",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" },
                format: "{0:c2}"
              },
              {
                field: "Title",
                title: "Title",
                filterable: false,
                headerAttributes: { style: "text-align: center;" }
              },
              {
                field: "Email",
                title: "Sales Rep",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" },
                template: kendo.template("#=Owner.UserName#")
              },
              {
                field: "",
                title: "",
                filterable: false,
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" },
                template: kendo.template("<a class=\"btn btn-default\" href=\"#=Links.DetailView#\">View Details</a>")
              }
            ]
          });
        }

        var state = JSON.parse($.cookie("DealSummaryGridState"));
        if (state) {
          $('#dealSummaryGrid').data('kendoGrid').dataSource.query(state);
        } else {
          $('#dealSummaryGrid').data('kendoGrid').dataSource.read();
        }
      }
    };

    function downloadDeals() {
      window.location.replace("<%= this.Url.BuildFor<DownloadDealsController>().IndexRoot() %>?applicationid=" +
        $("#ApplicationId").val() +
        "&startdate=" +
        moment(DealsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm') +
        "&enddate=" +
        moment(DealsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'));
    }

    // update status select using values from DateRange 
    function updateStatusSelect(callback) {
      console.log('updateStatusSelect');
      $("#statuses option").remove();
      $.getJSON('<%= this.Url.Action("GetDealStatuses", "DealSummary", new {Area = "Sales"}) %>',
        {
          applicationid: $("#ApplicationId").val(),
          startdate: moment(DealsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
          enddate: moment(DealsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'),
          status: $("#statuses").val(),
          email: pEmail
        },
        function(data) {
          $("#statuses").append("<option value='All' selected=\"selected\">- All Status ------</option>");
          $.each(data,
            function(index, status) {
              $("#statuses").append("<option value=" +
                status.Description +
                ">" +
                status.Description +
                " (" +
                status.Cnt +
                ")" +
                "</option>");
            });
          if (pDealStatus != null) {  
            $("#statuses").val(pDealStatus);
          }
          if (callback) callback.call();
        }
      );
    }

    function reset() {
      var uri = '<%= this.Url.BuildFor<DealSummaryController>().ToIndex() %>';
      history.pushState(null, "Deals", uri);
      window.location.replace(uri);
    }

    function viewDetails(e) {
      var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
      history.pushState(null, "Deals", "<%= this.Url.BuildFor<DealSummaryController>().ToIndex() %>");
      window.location.replace(dataItem.Links.DetailView);
    }

    function loadApplicationId() {
      var v = $("#ApplicationId").val() !== '' ? $("#ApplicationId").val() : $.cookie('ApplicationId');
      if (v !== '') {
        $('#ApplicationId option[value=' + $.cookie('ApplicationId') + ']').attr('selected', 'selected');
      }
    }

    function setApplicationId() {
      $.cookie('ApplicationId', $('#ApplicationId option:selected').val());
    }

    function openinvoices() {
      $('#open-invoices').modal('show');
    }

  </script>

  <script type="text/x-kendo-template" id="IdTemplate">
        <p style="margin-bottom: 3px;">DealId: #= Id #</p>
    </script>

</asp:Content>