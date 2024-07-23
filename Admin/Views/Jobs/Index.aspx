<%@ Page Inherits="System.Web.Mvc.ViewPage<JobsRequest>" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Title="" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.ViewModels.Job" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="Integration.NationBuilder.Data" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.JobProcessing.Review" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.NationBuilder.Controllers" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.JobProcessing.ChangeJobPriority" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.JobProcessing.DeleteJob" %>
<%@ Import Namespace="AccurateAppend.JobProcessing" %>
<%@ Import Namespace="AccurateAppend.Core.Definitions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Jobs
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div>

    <div class="row" style="padding-right: 20px;">
      <div class="pull-right">
        <%: Html.SiteDropDown(null) %>
      </div>
    </div>

    <div class="row" style="padding: 0 20px 0 20px;">

      <ul class="nav nav-tabs" id="tabs">
        <li class="active">
          <a href="#jobs" data-toggle="tab">Jobs</a>
        </li>
        <li>
          <a href="#nationbuilderpushes" data-toggle="tab">Nation Builder</a>
        </li>
      </ul>

      <div class="tab-content">

        <%-- Jobs tab --%>
        <div class="tab-pane active" id="jobs" style="margin-top: 20px;">

          <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="inProcessInfo">No pending jobs</div>
          <div id="gridInprocess" style="margin-bottom: 20px;"></div>
          <div class="row">
            <div class="col-md-3">
              <h3>Complete</h3>
            </div>
            <div class="pull-right" style="padding: 15px 20px;">
              <input type="text" class="form-control" id="searchTerm" style="display: inline; width: 250px;" placeholder="Search term..."/>
              <button onclick="viewModel.renderSearchResultsGrid()" class="btn btn-default">Search</button>
              <span id="jobsDateRange"></span>
            </div>
          </div>
          <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="completeInfo">No complete jobs</div>
          <div id="gridComplete" style="margin-bottom: 20px;"></div>
          <div id="gridCompleteSingleUser" style="display: none; margin-bottom: 20px;"></div>

        </div>

        <%-- NationBuilder pushes tab --%>
        <div class="tab-pane" id="nationbuilderpushes" style="margin-top: 20px;">

          <div class="alert alert-info" style="display: none; margin-top: 10px;" id="nationBuilderPushMessageInProcess">No pending jobs.</div>
          <div id="nationBuilderPushGridInProcess" style="margin-bottom: 20px;"></div>
          <div class="row">
            <div class="col-md-3">
              <h3>Complete</h3>
            </div>
            <div class="pull-right" style="padding: 15px 20px;">
              <span id="nbDateRange"></span>
            </div>
          </div>
          <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="nationBuilderPushMessageComplete">No complete jobs.</div>
          <div id="nationBuilderPushGridComplete" style="margin-bottom: 20px;"></div>
        </div>

      </div>

    </div>

  </div>

  <div class="modal fade" id="users-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Reassign Job</h4>
        </div>
        <div class="modal-body">
          <div class="input-group" style="margin-bottom: 10px;">
            <input type="text" id="userSearchTerm" class="form-control" placeholder="Email"/>
            <div class="input-group-btn">
              <button class="btn btn-default" onclick="viewModel.renderUserAssignmentGrid()" type="button"><span class="fa fa-search"></span>Search</button>
            </div>
          </div>
          <div class="alert alert-info" style="display: none; margin-bottom: 10px; padding: 10px;" id="userInfo">No users found</div>
          <div id="gridUsers" style="display: none;"></div>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="search-results-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content" style="width: 900px;">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Search Results</h4>
        </div>
        <div class="modal-body">
          <div class="alert alert-info" style="display: none; margin-bottom: 10px; padding: 10px;" id="gridCompleteSearchResultsMessage"></div>
          <div id="gridCompleteSearchResults" style="display: none;"></div>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="job-report-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog" style="width: 900px;">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Report</h4>
        </div>
        <div class="modal-body">
          <div id="parsingReport"></div>
          <div id="operationReports"></div>
          <!-- report goes here -->
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="file-preview-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog" style="width: 900px;">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title"></h4>
        </div>
        <div class="modal-body">
          <div id="file-preview"></div>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="rename-output-file-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content" style="width: 900px;">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Rename Output File</h4>
        </div>
        <div class="modal-body">
          <div class="alert" style="display: none; margin-bottom: 10px; padding: 10px;" id="message"></div>
          <form>
            <div class="form-group">
              <label>New file name</label>
              <input type="text" class="form-control" name="newFileName"/>
              <input type="hidden" name="newFileName"/>
              <input type="hidden" name="jobid"/>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
          <button type="button" class="btn btn-primary" onclick="jobModel.downloadOutputFileWithRename()">Download File</button>
        </div>
      </div>
    </div>
  </div>

  <input type="hidden" id="currentJobId"/>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <style type="text/css">
        
        /*.k-loading-mask{
          display:none; 
        }*/
    #gridComplete .k-toolbar {
      height: 30px;
      min-height: 27px;
      padding: 1.3em;
    }

    .category-label {
      padding-right: .5em;
      vertical-align: middle;
    }

    #newJob { vertical-align: middle; }

    .toolbar { float: right; }

    .k-tabstrip .k-state-active { border-color: #C5C5C5; }

    .k-window .k-window-content { padding: 10px; }

    .k-grid td { overflow: visible !important; } 

  </style>

  <script src="<%= Url.Content("~/Scripts/jquery-confirm.js") %>" type="text/javascript"> </script>

  <script type="text/javascript">

    var JobsDateRangeWidget;
    var NBDateRangeWidget;

    var pJobId = <%= Model.JobId == null ? "null" : "'" + Model.JobId + "'" %>;
    var pEmail = <%= Model.Email == null ? "null" : "'" + Model.Email + "'" %>;

    var renderCompleteTimer;
    var renderUserCompleteTimer;
    var renderInProcessTimer;
    var renderNBCompleteTimer;
    var renderNBInProcessTimer;
    var siteUsersdataSource;
    var autocomplete;
    var links; 

    $(function() {

      links = {
        ListClients:
          "<%: Url.Action("List", "SearchClients", new {Area = "Clients"}) %>?activeWithin=<%: DateRange.LastYear %>&applicationid=" +
            $("#ApplicationId").val(),
        SearchClients: "/Clients/SearchClients/Query",
        JobProcessing_InProcess: "/JobProcessing/Queue/InProcess",
        JobProcessing_Complete: "/JobProcessing/Queue/Complete",
        JobProcessing_CompleteSummary: "/JobProcessing/Queue/CompleteSummary",
        NationBuilderList: "/NationBuilder/List",
          ReassignJob: "/JobProcessing/Reassign",
          NewJob: "/Batch/UploadFile/DynamicAppend",
        SearchJobs: "/JobProcessing/Queue/Search"
      };

      sessionStorage.removeItem("grid");

      JobsDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("jobsDateRange",
        new AccurateAppend.Ui.DateRangeWidgetSettings(
          [
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
            AccurateAppend.Ui.DateRangeValue.Last60Days,
            AccurateAppend.Ui.DateRangeValue.LastMonth,
            AccurateAppend.Ui.DateRangeValue.Custom
          ],
          AccurateAppend.Ui.DateRangeValue.Last24Hours,
          [
            viewModel.renderCompleteGrid
          ]));

      NBDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("nbDateRange",
        new AccurateAppend.Ui.DateRangeWidgetSettings(
          [
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
            AccurateAppend.Ui.DateRangeValue.Custom
          ],
          AccurateAppend.Ui.DateRangeValue.Last7Days,
          [
            viewModel.renderNationBuilderCompleteGrid
          ]));
      NBDateRangeWidget.refresh();

      siteUsersdataSource = new kendo.data.DataSource({
        transport: {
          read: {
            dataType: "json",
            url: links.ListClients
          }
        }
      });

      if (pJobId != null) {
        jobModel.displayJobDetail(pJobId);
      } else if (pEmail != null) {
        JobsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
      } else {
        viewModel.renderCompleteGrid();
      }

      viewModel.renderInProcessGrid();

      // runs callbacks including grid refresh
      JobsDateRangeWidget.refresh();

      $("#nationBuilderDateRange").change(function() {
        viewModel.renderNationBuilderCompleteGrid();
      });

      $("#ApplicationId").bind('change',
        function() {
          setApplicationId();
          // begin update of autocomplete
          siteUsersdataSource = new kendo.data.DataSource({
            transport: {
              read: {
                dataType: "json",
                url: links.SearchClients
              }
            }
          });
          // end update of autocomplete

          clearInterval(renderNBCompleteTimer);
          clearInterval(renderNBInProcessTimer);
          viewModel.renderCompleteGrid();
          viewModel.renderInProcessGrid();
          renderCompleteTimer = setInterval(viewModel.renderCompleteGrid, 60000);
          renderInProcessTimer = setInterval(viewModel.renderInProcessGrid, 60000);
        });

      $("#Source").bind('change',
        function() {
          viewModel.renderChart_subscriberActivity_history();
        });

      $("#searchTerm").keypress(function(e) {
        if (e.which === 13) {
          viewModel.renderSearchResultsGrid();
          return false;
        }
      });

      $("#searchTerm").bind('change',
        function() {
          viewModel.renderChart_subscriberActivity_history();
        });

      $('a[data-toggle="tab"][href$="nationbuilderpushes"]').on('shown.bs.tab',
        function(e) {
          clearInterval(renderCompleteTimer);
          clearInterval(renderInProcessTimer);
          viewModel.renderNationBuilderInprocessGrid();
          viewModel.renderNationBuilderCompleteGrid();
          renderNBCompleteTimer = window.setInterval(viewModel.renderNationBuilderCompleteGrid, 60000);
          renderNBInProcessTimer = window.setInterval(viewModel.renderNationBuilderInprocessGrid, 60000);
        });

      $('a[data-toggle="tab"][href$="jobs"]').on('shown.bs.tab',
        function(e) {
          clearInterval(renderNBCompleteTimer);
          clearInterval(renderNBInProcessTimer);
          viewModel.renderCompleteGrid();
          viewModel.renderInProcessGrid();
          renderCompleteTimer = setInterval(viewModel.renderCompleteGrid, 60000);
          renderInProcessTimer = setInterval(viewModel.renderInProcessGrid, 60000);
        });

      // timers
      renderCompleteTimer = setInterval(viewModel.renderCompleteGrid, 60000);
      renderInProcessTimer = setInterval(viewModel.renderInProcessGrid, 60000);

    });

    function reset() {
      history.pushState(null, "Jobs", "/Jobs");
      window.location.replace("/Jobs");
    }

    var viewModel = {
      renderCompleteGrid: function() {
        if (pEmail != null) {
          viewModel.renderCompleteGridforSingleUser();
        } else {
          viewModel.renderCompleteGridGlobal();
        }
        },
      // renders grid for jobs that are currenlty processing. This is the section contained in the Jobs tab at the top of the page
      renderInProcessGrid: function() {
        console.log('renderInProcessGrid');
        var grid = $("#gridInprocess").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#gridInprocess").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  var data = { applicationid: $("#ApplicationId").val() };
                  if (pJobId != null)
                    data.jobid = pJobId;
                  else
                    data.applicationid = $('#ApplicationId').val();
                  $.ajax({
                    url: links.JobProcessing_InProcess,
                    dataType: 'json',
                    type: 'GET',
                    data: data,
                    success: function(result) {
                      options.success(result);
                    }
                  });
                },
                cache: false
              },
              pageSize: 20,
              schema: {
                type: 'json',
                data: "Data",
                total: function(response) {
                  return response.Data.length;
                }
              },
              change: function() {
                if (this.data().length <= 0) {
                  $("#inProcessInfo").show();
                  $("#gridInprocess").hide();
                } else {
                  $("#inProcessInfo").hide();
                  $("#gridInprocess").show();
                }
              }
            },
            pageable: true,
            columns: [
              {
                field: "SubmittedDescription",
                title: "Submitted",
                width: 140,
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "UserName",
                title: "Username",
                width: 200,
                headerAttributes: { style: "text-align: center;" },
                template: '<a href="=#: Links.UserDetail #">#= UserName #</a>',
                media: "(min-width: 450px)"
              },
              {
                field: "CustomerFileName",
                title: "File Name",
                //width: 200,
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "RecordCount",
                title: "Records",
                width: 60,
                format: "{0:n0}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "ProcessedCount",
                title: "Processed",
                width: 60,
                format: "{0:n0}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "MatchCount",
                title: "Matched Records",
                width: 60,
                format: "{0:n0}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "MatchRate",
                title: "Rate",
                width: 70,
                format: "{0:p}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "StatusDescription",
                title: "Status",
                width: 175,
                template: kendo.template($("#statusDescriptionTemplate").html()),
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                template: kendo.template($("#cmdViewDetailsInProcess").html()),
                width: 200,
                attributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              }, {
                title: "Summary",
                template: kendo.template($("#responsive-column-template-inprocess").html()),
                media: "(max-width: 450px)"
              }
            ],
            scrollable: false
          });
        }
        },
      // renders the complete jobs grid. This is the grid that contains a summary for each client with a carret that can be clicked to view the customers 
      //  jobs, which are lazy loaded.
      renderCompleteGridGlobal: function() {
        console.log('renderCompleteGrid');
        var expandedRow;
        var grid = $("#gridComplete").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#gridComplete").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  var data = {
                    applicationid: $("#ApplicationId").val(),
                    startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                    enddate: moment(JobsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm')
                  };
                  if (pJobId != null) data.jobid = pJobId;
                  $.ajax({
                    url: links.JobProcessing_CompleteSummary,
                    data: data,
                    dataType: 'json',
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                },
                cache: false
              },
              schema: {
                type: 'json',
                data: "Data",
                total: function(response) {
                  return response.Data.length;
                },
                model: {
                  id: "UserId",
                  UserId: "UserId",
                  UserName: "UserName",
                  FileCount: "FileCount",
                  RecordCount: "RecordCount",
                  MatchCount: "MatchCount",
                  LastActivity: "LastActivity",
                  LastActivityDescription: "LastActivityDescription",
                  Jobs: "Jobs"
                }
              },
              pageSize: 20,
              change: function() {
                if (this.data().length <= 0) {
                  $("#completeInfo").text('No complete jobs found for the date range ' +
                    moment(JobsDateRangeWidget.getStartDate()).format("MM-DD-YYYY") +
                    ' to ' +
                    moment(JobsDateRangeWidget.getEndDate()).format("MM-DD-YYYY"));
                  $("#completeInfo").show();
                  $("#gridComplete").hide();
                } else {
                  $("#completeInfo").hide();
                  $("#gridComplete").show();
                }
              }
            },
            pageable: true,
            columns: [
              {
                field: "UserName",
                title: "Username",
                width: 800,
                headerAttributes: { style: "text-align: center;" },
                template:
                  '<a href="#: Links.UserDetail #">#= UserName #</a><a style="font-size: .8em;margin-left: 5px;" href="#: Links.JobsForClient #">view jobs</a>',
                media: "(min-width: 450px)"
              },
              {
                field: "FileCount",
                title: "File Count",
                format: "{0:n0}",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: right;" },
                media: "(min-width: 450px)"
              },
              {
                field: "RecordCount",
                title: "Record Count",
                format: "{0:n0}",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: right;" },
                media: "(min-width: 450px)"
              },
              {
                field: "MatchCount",
                title: "Matched Records",
                format: "{0:n0}",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: right;" },
                media: "(min-width: 450px)"
              },
              {
                field: "LastActivityDescription",
                title: "Last Activity",
                headerAttributes: { style: "text-align: center;" },
                width: 175,
                attributes: { style: "text-align: right;" },
                media: "(min-width: 450px)"
              }, {
                title: "Summary",
                template: kendo.template($("#responsive-column-template-complete").html()),
                media: "(max-width: 450px)"
              }
            ],
            toolbar:
              "<div class=\"toolbar\"><input class=\"btn btn-default\" style=\"margin-right: 10px;\" type=\"submit\" onclick=\"window.location.replace('/Batch/UploadFile/DynamicAppend')\" value=\"New Job\"/>",
            detailTemplate: '<div class="details" style="margin: 5px 5px 10px 5px;"></div>',
            detailInit: detailInit,
            scrollable: false,
            dataBound: function() {
              var grid = $('#gridComplete').data('kendoGrid');
              if (this.dataSource.total() == 1) {
                grid.expandRow($('#gridComplete tbody>tr:first'));
              }
              // expand detail row that was previously expanded before data source was refreshed
              var state = sessionStorage.getItem("grid");
              if (state) {
                state = JSON.parse(state);
                for (var id in state) {
                  var dataItem =
                    grid.dataSource
                      .get(id); // "get" method requires model to be set in dataSource with field name "id"
                  if (dataItem != null)
                    grid.expandRow("tr[data-uid=" + dataItem.uid + "]");
                }
              }
            },
            detailExpand: function(e) {
              // collapse all expanded rows before expanding this one
              if (expandedRow != null && expandedRow[0] != e.masterRow[0]) {
                var grid = $('#gridComplete').data('kendoGrid');
                grid.collapseRow(expandedRow);
              }
              expandedRow = e.masterRow;
              // save grid state so expanded rows don't collapse when the dataSource is refreshed
              var state = sessionStorage.getItem("grid");
              if (!state) {
                state = {};
              } else {
                state = JSON.parse(state);
              }
              state[this.dataItem(e.masterRow).id] = true;
              sessionStorage.setItem("grid", JSON.stringify(state));

            },
            detailCollapse: function(e) {
              // save grid state so expanded rows don't collapse when the dataSource is refreshed
              var state = sessionStorage.getItem("grid");
              if (state) {
                state = JSON.parse(state);
                delete state[this.dataItem(e.masterRow).id];
                sessionStorage.setItem("grid", JSON.stringify(state));
              }
            }
          });
        }
        },
      // renders grid when email is passed into page. This allows us to view jobs for specific client
      // /Jobs/Index?email=trish@focuscss.com
      renderCompleteGridforSingleUser: function() {
        console.log('renderCompleteGridforSingleUser');
        $("#gridCompleteSingleUser").show();
        var grid = $("#gridCompleteSingleUser").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#gridCompleteSingleUser").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  var data = {
                    startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                    enddate: moment(JobsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'),
                    email: pEmail
                  };
                  $.ajax({
                    url: "/JobProcessing/Queue/Complete",
                    data: data,
                    dataType: 'json',
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              pageSize: 10,
              schema: {
                type: 'json',
                data: "Data",
                total: function(response) {
                  return response.Data.length;
                }
              },
              change: function() {
                console.log('grid single user data count = ' + this.data().length);
                if (this.data().length <= 0) {
                  $("#completeInfo").show();
                  $("#completeInfo").text('No complete jobs found for ' +
                    pEmail +
                    ' for the date range ' +
                    moment(JobsDateRangeWidget.getStartDate()).format("MM-DD-YYYY") +
                    ' to ' +
                    moment(JobsDateRangeWidget.getEndDate()).format("MM-DD-YYYY"));
                  $("#gridCompleteSingleUser").hide();
                } else {
                  $("#completeInfo").hide();
                  $("#gridCompleteSingleUser").show();
                  $('[role="gridcell"] span').tooltip();
                }
              }
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            columns: [
              {
                field: "DateComplete",
                title: "Date Complete",
                width: "160px",
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "UserName",
                title: "Username",
                width: 200,
                headerAttributes: { style: "text-align: center;" },
                template: '<a href="=#: Links.UserDetail #">#= UserName #</a>',
                media: "(min-width: 450px)"
              },
              {
                field: "JobId",
                title: "JobId",
                width: "75px",
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "CustomerFileName",
                title: "File Name",
                width: "200px",
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "Product",
                title: "Product",
                width: "250px",
                headerAttributes: { style: "text-align: center;" },
                template:
                  '<span title="#= Product #" data-toggle="tooltip" data-placement="top">#=truncateProductDescription(Product)#</span>',
                media: "(min-width: 450px)"
              },
              {
                field: "SourceDescription",
                title: "Source",
                width: "100px",
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "TotalRecords",
                title: "Records",
                //width: "60px",
                format: "{0:n0}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "MatchRecords",
                title: "Matched Records",
                //width: 60,
                format: "{0:n0}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                field: "MatchRate",
                title: "Rate",
                //width: 60,
                format: "{0:p}",
                attributes: { style: "text-align: right;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
              },
              {
                width: "200px",
                attributes: { style: "text-align: center;" },
                template: kendo.template($("#cmdViewDetailsComplete").html()),
                media: "(min-width: 450px)"
              },
              {
                title: "Summary",
                template: kendo.template($("#responsive-column-template-user").html()),
                media: "(max-width: 450px)"
              }
            ]
          });
        }
        },
      // Renders search results for search function in a modal window
      renderSearchResultsGrid: function() {
        $("#search-results-modal").modal('show');
        var grid = $("#gridCompleteSearchResults").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#gridCompleteSearchResults").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  var data = {
                    searchTerm: $("#searchTerm").val(),
                    applicationId: $("#ApplicationId").val()
                  };
                  $.ajax({
                    url: links.SearchJobs,
                    data: data,
                    dataType: 'json',
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              pageSize: 10,
              schema: {
                type: 'json',
                data: "Data",
                total: function(response) {
                  return response.Data.length;
                }
              },
              change: function() {
                if (this.data().length <= 0) {
                  $("#gridCompleteSearchResultsMessage").text('No jobs found for ' + $("#searchTerm").val()).show();
                } else {
                  $("#gridCompleteSearchResults").show();
                }
              }
            },
            scrollable: false,
            sortable: true,
            pageable: true,
            columns: [
              {
                field: "DateComplete",
                title: "Date Complete",
                headerAttributes: { style: "text-align: center;" }
              },
              {
                field: "UserName",
                title: "Username",
                headerAttributes: { style: "text-align: center;" }
              },
              {
                field: "JobId",
                title: "JobId",
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" }
              },
              {
                title: "File Name",
                width: "200px",
                headerAttributes: { style: "text-align: center;" },
                template: '<div style="word-wrap: break-word;">#=truncateProductDescription(CustomerFileName)#</div>'
              },
              {
                field: "SourceDescription",
                title: "Source",
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" }
              },
              {
                template: '<a class="btn btn-default" href="#: Links.JobDetail #">View Job</a>'
              }
            ]
          });
        }
        },
      // renders nation builder grid, contained in Nation Builder grid
      renderNationBuilderInprocessGrid: function() {
        console.log('renderNationBuilderInprocessGrid');
        $("#nationBuilderPushGridInProcess").kendoGrid({
          autobind: false,
          dataSource: {
            type: "json",
            transport: {
              read: function(options) {
                $.ajax({
                  traditional: true,
                  url: links.NationBuilderList,
                  dataType: 'json',
                  type: 'GET',
                  data: {
                    pushStatuses: [
                      '<%: PushStatus.Review %>', '<%: PushStatus.Failed %>',
                      '<%: PushStatus.Pending %>', '<%: PushStatus.Acquired %>',
                      '<%: PushStatus.Processing %>', '<%: PushStatus.Pushing %>',
                      '<%: PushStatus.Paused %>'
                    ],
                    startdate: moment().subtract(5, 'months').format('YYYY-MM-DD H:mm'),
                    enddate: moment().format('YYYY-MM-DD H:mm'),
                  },
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
                $("#nationBuilderPushMessageInProcess").show();
                $("#nationBuilderPushGridInProcess").hide();
              } else {
                $("#nationBuilderPushMessageInProcess").hide();
                $("#nationBuilderPushGridInProcess").show();
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
            { field: "Id", title: "Id" },
            { field: "RequestDate", title: "Date", width: "175px" },
            {
              field: "UserName",
              title: "Username",
              template: '<a href="=#: Links.UserDetail #">#= UserName #</a>'
            },
            { field: "Name", title: "List Name", template: '<a href="=#: Links.JobDetail #">#= Name #</a>' },
            { field: "NationName", title: "Nation Name" },
            { field: "Product", title: "Description" },
            {
              field: "StatusDescription",
              title: "Status",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: center;" },
              template: kendo.template($("#nationBuilderStatusDescriptionTemplate").html())
            },
            {
              field: "TotalRecords",
              title: "Records",
              format: "{0:n0}",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: right;" }
            },
            {
              field: "Progress",
              title: "Progress",
              format: "{0:n0}",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: right;" }
            },
            {
              field: "ErrorsEncountered",
              title: "Error Count",
              format: "{0:n0}",
              attributes: { style: "text-align: right;" },
              template:
                '<a href="=#: Links.Events #">#= ErrorsEncountered #</a>'
            },
            {
              title: " ",
              width: "110px",
              attributes: { style: "text-align: center;" },
              template: kendo.template($("#nationBuilderCommandButtonsTemplate").html())
            }
          ]
        });
      },
      renderNationBuilderCompleteGrid: function() {
        console.log('renderNationBuilderCompleteGrid');
        $("#nationBuilderPushGridComplete").kendoGrid({
          autobind: false,
          dataSource: {
            type: "json",
            transport: {
              read: function(options) {
                $.ajax({
                  traditional: true,
                  url: links.NationBuilderList,
                  dataType: 'json',
                  type: 'GET',
                  data: {
                    pushStatuses: ['Canceled', 'Complete'],
                    startdate: moment(NBDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                    enddate: moment(NBDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm')
                  },
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
                $("#nationBuilderPushMessageComplete").show();
                $("#nationBuilderPushGridComplete").hide();
              } else {
                $("#nationBuilderPushMessageComplete").hide();
                $("#nationBuilderPushGridComplete").show();
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
            { field: "Id", title: "Id" },
            { field: "RequestDate", title: "Date", width: "175px" },
            {
              field: "UserName",
              title: "Username",
              template: '<a href="=#: Links.UserDetail #">#= UserName #</a>'
            },
            { field: "Name", title: "List Name", template: '<a href="#: Links.JobDetail #">#= Name #</a>' },
            { field: "NationName", title: "Nation Name" },
            { field: "Product", title: "Description" },
            {
              field: "StatusDescription",
              title: "Status",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: center;" },
              template: kendo.template($("#nationBuilderStatusDescriptionTemplate").html())
            },
            {
              field: "TotalRecords",
              title: "Records",
              format: "{0:n0}",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: right;" }
            },
            {
              field: "Progress",
              title: "Progress",
              format: "{0:n0}",
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: right;" }
            },
            {
              field: "ErrorsEncountered",
              title: "Error Count",
              format: "{0:n0}",
              attributes: { style: "text-align: right;" },
              template:
                '<a href="/Operations/EventLog/Index?correlationId=#: CorrelationId #">#= ErrorsEncountered #</a>'
            }
          ]
        });
      },
      renderUserAssignmentGrid: function() {
        var grid = $("#gridUsers").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#gridUsers").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  $.ajax({
                    url: links.SearchClients + "?searchterm=" + $('#userSearchTerm').val(),
                    dataType: 'json',
                    type: 'GET',
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
              pageSize: 10,
              change: function() {
                if (this.data().length <= 0) {
                  $("#userInfo").show();
                  $("#gridUsers").hide();
                } else {
                  $("#userInfo").hide();
                  $("#gridUsers").show();
                }
              }
            },
            pageable: true,
            columns: [
              {
                field: "Email",
                title: "Username",
                width: "800px",
                headerAttributes: { style: "text-align: center;" },
                template: '<a href="=#: Links.UserDetail #">#= Email #</a>'
              },
              {
                field: "LastActivityDescription",
                title: "Last Activity",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: right;" }
              },
              {
                command: { text: "Select", click: jobModel.reassignJob },
                title: " ",
                width: "110px",
                attributes: { style: "text-align: center;" },
                template:
                  '<button type="button" class="btn btn-default"><span class="fa fa-edit"></span>Select</button>'
              }
            ],
            scrollable: false
          });
        }
      }
    };

    var jobModel = {
      reset: function(url) {
        if (url) {
          $.getJSON(url,
            function(result) {
              if (!result.success) {
                alert(result.error);
              } else
                viewModel.renderInProcessGrid();
              viewModel.renderCompleteGrid();
            });
          jobModel.closeJobDetail();
        }
      },
      downloadInputfile: function(jobid) {
        window.location.replace("/JobProcessing/DownloadFiles/Input?jobid=" + jobid);
      },
      downloadOutputfile: function(jobid) {
        window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobid);
      },
      openOutputFileDownloadRenameModal(jobid, fileName) {
        $('.jobDetails').data("kendoWindow").destroy();
        $("#rename-output-file-modal input[name=jobid]").val(jobid);
        $("#rename-output-file-modal input[name=newFileName]").val(fileName);
        $("#rename-output-file-modal").modal("show");
      },
      downloadOutputFileWithRename: function() {
        var jobId = $("#rename-output-file-modal input[name=jobid]").val();
        var newFileName = $("#rename-output-file-modal input[name=newFileName]").val();
        $("#rename-output-file-modal").modal("hide");
        window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobId + "&fileName=" + newFileName);
      },
      downloadManifest: function(url) {
        window.location.replace(url);
      },
      remapInputfile: function(url) {
        history.pushState(null, "Jobs", "/Jobs");
        window.location.replace(url);
      },
      setJobComplete: function(url) {
        $.getJSON(
          url,
          function(result) {
            if (!result.success) {
              alert("An error occurred. Job could not be set to complete.\r\n" + result.Message);
            } else {
              viewModel.renderInProcessGrid();
              viewModel.renderCompleteGrid();
            }
          });
        jobModel.closeJobDetail();
      },
      changePriority: function(url, priority) {
        $.getJSON(
          url,
          { priority: priority },
          function(result) {
            $("#jobPriority .alert").remove();
            if (!result.success) {
              alert("An error occurred. Priority could not be updated.");
              $("#jobPriority")
                .append(
                  "<div style=\"display: inline; margin-left: 5px; padding: 8px;\" class=\"alert alert-danger\">Error. Priority not updated.</div>");
            } else {
              viewModel.renderInProcessGrid();
              viewModel.renderCompleteGrid();
              $("#jobPriority")
                .append(
                  "<div style=\"display: inline; margin-left: 5px; padding: 8px;\" class=\"alert alert-success\">Priority updated.</div>");
            }
          });

      },
      deleteJob: function(jobid) {
        $.getJSON("<%= Url.BuildFor<DeleteJobController>().Root() %>", // TODO: link needs to come from job detail
          { jobid: jobid },
          function(result) {
            if (!result.success) {
              alert(result.error);
            } else {
              viewModel.renderInProcessGrid();
              viewModel.renderCompleteGrid();
            }
          });
        jobModel.closeJobDetail();

      },
      closeJobDetail: function() {
        // note: destroy removes the window from the DOM which is necessary so multiple windows are not added when another detail is viewed
        var detailsWindow = $('.jobDetails').data("kendoWindow");
        if (detailsWindow) detailsWindow.destroy();
        //$('.jobDetails').data("kendoWindow").destroy();
      },
      reassignJob: function(e) {
        var grid = $("#gridUsers").data("kendoGrid");
        var item = grid.dataItem($(e.target).closest("tr"));
        var id = $("#currentJobId").val();
        var userid = item.UserId;

        $.ajax(
          {
            type: "POST",
            url: links.ReassignJob,
            data: { jobid: id, userid: userid },
            success: function(result) {
              if (!result.success) {
                alert(result.message);
              }

              $('#users-modal').modal('hide');
              viewModel.renderInProcessGrid();
              viewModel.renderCompleteGrid();
              $("#userSearchTerm").val('');
            }
          });
      },
      displayJobDetail: function(jobid) {
        // ensure there are no previous detail windows in the DOM
        if ($('.jobDetails').data("kendoWindow") != null) $('.jobDetails').data("kendoWindow").destroy();

        $.get("/JobProcessing/Detail?jobid=" + jobid, // TODO: link needs to come from job summary
          function(data) {
            $('<div class="jobDetails" style="padding: 10px;"></div>').kendoWindow({
              title: "Detail: " + data.JobId,
              resizable: false,
              modal: true,
              viewable: false,
              content: {
                template: kendo.template($("#jobDetailTemplate").html())(data)
              },
              width: "800px",
              position: { top: "200px", left: "600px" },
              scrollable: false,
              activate: function() {

                // note: control has to be initialized after window is active
                $("#priority").kendoDropDownList({
                  dataTextField: "text",
                  dataValueField: "value",
                  dataSource: [
                    {
                      text: "<%: Priority.High %>",
                      value: "<%: (int) Priority.High %>"
                    }, //TODO: NEED TO EVENTUALLY CREATE TS ENUM
                    { text: "<%: Priority.Medium %>", value: "<%: (int) Priority.Medium %>" },
                    { text: "<%: Priority.Low %>", value: "<%: (int) Priority.Low %>" },
                    { text: "<%: Priority.Minimal %>", value: "<%: (int) Priority.Minimal %>" },
                    { text: "<%: Priority.None %>", value: "<%: (int) Priority.None %>" },
                  ],
                  change: function() {
                    jobModel.changePriority(jobid, this.value());
                  },
                  value: data.Priority,
                  enable: data.Status !== <%: (int) JobStatus.Complete %> //TODO: NEED TO EVENTUALLY CREATE TS ENUM
                });
              }
            }).data("kendoWindow").open();
          });
      },
      associateJobWithExistingDeal: function(url) {
        window.location.replace(url);
      },
      newJob: function(jobid, userid) {
        var url = "/Batch/FromExistingJob?userId=" +
          userid +
          "&jobid=" +
          jobid; // TODO: link needs to come from job summary
        window.location.replace(url);
      },
      openTextReport: function(url) {
        window.open(url);
      },
      downloadTextReport: function(url) {
        window.location.replace(url);
      },
      openJobReassignModal: function(jobid) {
        jobModel.closeJobDetail();
        $("#currentJobId").val(jobid);
        $('#users-modal').modal('show');
      },
      newDealFromJob: function(url) {
        window.location.replace(url);
      },
      previewInputFile: function(jobid) {
        window.location.replace("/JobProcessing/PreviewJob/Input?jobid=" +
          jobid); // TODO: link needs to come from job detail
      },
      previewOuputFile: function(jobid) {
        window.location.replace("/JobProcessing/PreviewJob/Output?jobid=" +
          jobid); // TODO: link needs to come from job detail
      },
      pause: function(url) {
        $.get(url,
          function(data) {
            jobModel.closeJobDetail();
          });
      },
      resume: function(url) {
        $.get(url,
          function(data) {
            jobModel.closeJobDetail();
          });
      }
    };

    var nationBuilderModel = {
      resume: function(pushid) {
        $.confirm({
          text: "Are you sure you want to resume this push?",
          confirm: function(button) {
            console.log('calling resume for ' + pushid);
            $.getJSON(
              "<%= Url.BuildFor<ResumeController>().ToResume() %>?id=" + // TODO: NEEDS TO COME FROM JOB SUMMARY
              pushid,
              function(result) {
                console.log('resume returning status = ' +
                  result.HttpStatusCode +
                  ', message = ' +
                  result.Message);
                if (result.HttpStatusCode !== 200) {
                  $('#nationbuilderpushes')
                    .prepend(
                      '<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                      result.Message +
                      '</div>');
                } else {
                  viewModel.renderNationBuilderInprocessGrid();
                  viewModel.renderNationBuilderCompleteGrid();
                  $('#nationbuilderpushes .alert').remove();
                }
              });
          },
          cancel: function(button) {
            // do something
          },
          confirmButton: "Yes",
          cancelButton: "Close"
        });
      },
      cancel: function(pushid) {
        $.confirm({
          text: "Are you sure you want to cancel this push?",
          confirm: function(button) {
            console.log('calling confirm for ' + pushid);
            $.getJSON(
              "<%= Url.BuildFor<CancelController>().ToCancel() %>?id=" + pushid, // TODO: NEEDS TO COME FROM JOB SUMMARY
              function(result) {
                console.log('confirm returning status = ' +
                  result.HttpStatusCode +
                  ', message = ' +
                  result.Message);
                if (result.HttpStatusCode !== 200) {
                  $('#nationbuilderpushes')
                    .prepend(
                      '<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                      result.Message +
                      '</div>');
                } else {
                  viewModel.renderNationBuilderInprocessGrid();
                  viewModel.renderNationBuilderCompleteGrid();
                  $('#nationbuilderpushes .alert').remove();
                }
              });
          },
          cancel: function(button) {
            // do something
          },
          confirmButton: "Yes",
          cancelButton: "Close"
        });
      }
    };

    var jobReport = {
      display: function(jobid, userid) {
        $.ajax({
          url: "/JobProcessing/Reports/GetAvailableOperationsForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
          data: { jobid: jobid },
          dataType: 'json',
          type: 'GET',
          success: function(result) {
            var div = $('<div/>');
            var panel = $('<div class="panel panel-default">' +
              '<div class="panel-heading">' +
              '<h3 class="panel-title">Address Standarization</h3>' +
              '</div>' +
              '</div>');
            var panelBody = $('<div class="panel-body"></div>');
            var cassChart = $('<div/>');
            cassChart.kendoChart({
              title: {
                text: "Standardization Status"
              },
              dataSource: {
                transport: {
                  read: function(options) {
                    $.ajax({
                      url: "/JobProcessing/Reports/GetCassReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                      dataType: 'json',
                      data: { jobid: jobid },
                      type: 'GET',
                      success: function(result) {
                        options.success(result);
                      }
                    });
                  }
                },
                schema: { data: "Data" }
              },
              seriesDefaults: {
                type: "column",
                stack: {
                  type: "100%"
                }
              },
              series: [
                {
                  name: "Fail",
                  type: "column",
                  field: "F",
                  color: ""
                }, {
                  name: "Parse only",
                  type: "column",
                  field: "P",
                  color: ""
                }, {
                  name: "Sucess",
                  type: "column",
                  field: "S",
                  color: ""
                }, {
                  name: "Unknown",
                  type: "column",
                  field: "U",
                  color: ""
                }, {
                  name: "Canadian",
                  type: "column",
                  field: "C",
                  color: ""
                }
              ],
              valueAxis: {
                labels: {
                  format: "{0:p0}"
                },
                visible: true,
                min: 0,
                max: 1
              },
              categoryAxis: {
                field: "Type",
                visible: true,
              },
              tooltip: {
                visible: true,
                format: "{0:p0}"
              },
              legend: {
                visible: true,
                position: "bottom"
              },
              dataBound: function() {}
            });
            $('#parsingReport div').remove();
            $('#parsingReport').append(
              div.append(
                panel.append(panelBody
                  .append($('<div class="col-md-6"/>').append(cassChart))
                  .append($('<div class="col-md-6"/>').append("</div>"))
                )
              )
            );
            $('#operationReports div').remove();
            $.each(result.Data,
              function(i, operationName) {
                var operationReport = new OperationReport(jobid, operationName);
                operationReport.display();
              });
          }
        });
        $('#job-report-modal').modal('show');
      }
    };

    var OperationReport = function(jobid, operationName) {
      this.jobid = jobid,
        this.productKey = operationName,
        this.display = function() {
          var div = $('<div/>');
          var panel = $('<div class="panel panel-default">' +
            '<div class="panel-heading">' +
            '<h3 class="panel-title">Operation: ' +
            operationName +
            '</h3>' +
            '</div>' +
            '</div>');
          var panelBody = $('<div class="panel-body"></div>');
          var matchLevelchart = $('<div/>');
          matchLevelchart.kendoChart({
            title: {
              text: "Match Level"
            },
            dataSource: {
              transport: {
                read: function(options) {
                  $.ajax({
                    url: "/JobProcessing/Reports/GetMatchLevelReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                    dataType: 'json',
                    data: { jobid: jobid, operationName: operationName },
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              schema: { data: "Data" }
            },
            series: [
              {
                name: "This File",
                type: "column",
                field: "File",
                color: ""
              }, {
                name: "This User",
                type: "column",
                field: "User",
                color: ""
              }, {
                name: "Entire System",
                type: "column",
                field: "System",
                color: ""
              }
            ],
            valueAxis: {
              labels: {
                format: "{0:p0}"
              },
              visible: true,
              min: 0,
              max: 1
            },
            categoryAxis: {
              field: "MatchLevel",
              visible: true
            },
            tooltip: {
              visible: true,
              format: "{0:p0}"
            },
            legend: {
              visible: true,
              position: "bottom"
            },
            dataBound: function() {}
          });

          var qualityLevelChart = $('<div/>');
          qualityLevelChart.kendoChart({
            title: {
              text: "Quality Level"
            },
            dataSource: {
              transport: {
                read: function(options) {
                  $.ajax({
                    url: "/JobProcessing/Reports/GetQualityLevelReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                    dataType: 'json',
                    data: { jobid: jobid, operationName: operationName },
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              schema: { data: "Data" }
            },
            series: [
              {
                name: "This File",
                type: "column",
                field: "File",
                color: ""
              }, {
                name: "This User",
                type: "column",
                field: "User",
                color: ""
              }, {
                name: "Entire System",
                type: "column",
                field: "System",
                color: ""
              }
            ],
            valueAxis: {
              labels: {
                format: "{0:p0}"
              },
              visible: true,
              min: 0,
              max: 1
            },
            categoryAxis: {
              field: "QualityLevel",
              visible: true
            },
            tooltip: {
              visible: true,
              format: "{0:p0}"
            },
            legend: {
              visible: true,
              position: "bottom"
            },
            dataBound: function() {}
          });
          var maxValidationLevelChart = $('<div/>');
          maxValidationLevelChart.kendoChart({
            title: {
              text: "Maximum Validation Level"
            },
            dataSource: {
              transport: {
                read: function(options) {
                  $.ajax({
                    url:
                      "/JobProcessing/Reports/GetMaxValidationLevelReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                    dataType: 'json',
                    data: { jobid: jobid, operationName: operationName },
                    type: 'GET',
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              schema: { data: "Data" }
            },
            series: [
              {
                name: "This File",
                type: "column",
                field: "File",
                color: ""
              }, {
                name: "This User",
                type: "column",
                field: "User",
                color: ""
              }, {
                name: "Entire System",
                type: "column",
                field: "System",
                color: ""
              }
            ],
            valueAxis: {
              labels: {
                format: "{0:p0}"
              },
              visible: true,
              min: 0,
              max: 1
            },
            categoryAxis: {
              field: "MatchLevel",
              visible: true,
              step: 10
            },
            tooltip: {
              visible: true,
              format: "{0:p0}"
            },
            legend: {
              visible: true,
              position: "bottom"
            },
            dataBound: function() {}
          });

          $('#operationReports').append(
            div.append(panel.append(panelBody
                .append($('<div = class="col-md-4"/>').append(maxValidationLevelChart))
                .append($('<div = class="col-md-4"/>').append(matchLevelchart))
                .append($('<div = class="col-md-4"/>').append(qualityLevelChart))
              )
            )
          );
        };
    };

    // collapses all rows except the currently expanded row
    function toggleDetail(e) {
      var grid = $(e).closest('.t-grid').data('tGrid');
      if ($(e).hasClass('t-minus')) {
        grid.$rows().each(function(index) { grid.collapseRow(this); });
        $(e).removeClass('t-minus');
      } else {
        grid.$rows().each(function(index) { grid.expandRow(this); });
        $(e).addClass('t-minus');
      }
      return false;
    }

    // displays grid containing jobs associated with the specific client in the master record
    function detailInit(e) {

      var detailRow = e.detailRow;

      detailRow.find(".details").kendoGrid({
        dataSource: {
          type: "json",
          transport: {
            read: function(options) {
              var data = {
                userid: e.data.UserId,
                startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                enddate: moment(JobsDateRangeWidget.getEndDate()).add(10, 'minutes')
                  .format('YYYY-MM-DD H:mm')
              };
              $.ajax({
                url: links.JobProcessing_Complete,
                data: data,
                dataType: 'json',
                type: 'GET',
                success: function(result) {
                  options.success(result);
                }
              });
            }
          },
          pageSize: 10,
          schema: {
            type: 'json',
            data: "Data",
            total: function(response) {
              return response.Data.length;
            }
          }
        },
        scrollable: false,
        sortable: true,
        pageable: true,
        columns: [
          {
            field: "DateComplete",
            title: "Date Complete",
            width: "160px",
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "ProcessingTime",
            title: "Minutes",
            width: "75px",
            attributes: { style: "text-align: center;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "JobId",
            title: "JobId",
            //width: "75px",
            attributes: { style: "text-align: center;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "CustomerFileName",
            title: "File Name",
            //width: "200px",
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          //{ field: "Product", title: "Product", width: "250px", headerAttributes: { style: "text-align: center;" } },
          {
            template: kendo.template($("#productTemplate").html()),
            width: "250px",
            attributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "SourceDescription",
            title: "Source",
            width: "100px",
            attributes: { style: "text-align: center;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "TotalRecords",
            title: "Records",
            width: "60px",
            format: "{0:n0}",
            attributes: { style: "text-align: right;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "MatchRecords",
            title: "Matched Records",
            width: "60px",
            format: "{0:n0}",
            attributes: { style: "text-align: right;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            field: "MatchRate",
            title: "Rate",
            width: "70px",
            format: "{0:p}",
            attributes: { style: "text-align: right;" },
            headerAttributes: { style: "text-align: center;" },
            media: "(min-width: 450px)"
          },
          {
            command: { text: "View Details", click: clickDetailsCompleteSingleUser },
            title: " ",
            media: "(min-width: 450px)",
            width: "110px",
            attributes: { style: "text-align: center;" },
            //template: '<button type="button" class="btn btn-default"><span class="fa fa-edit" onclick="clickDetailsComplete(#= JobId #)"></span>View Details</button>'
          },
          {
            title: "Summary",
            template: kendo.template($("#responsive-column-template-user").html()),
            media: "(max-width: 450px)"
          }
        ]
      });
    }

    // creates modal window contains details for a specific job that has already been completed
    function clickDetailsComplete(jobid) {

      //var g = $(".details").data("kendoGrid");
      //var dataItem = g.dataItem($(e.target).closest("tr"));
      //var jobid = dataItem.JobId;

      jobModel.displayJobDetail(jobid);
    }

    function clickDetailsCompleteSingleUser(e) {
      var jobid = this.dataItem($(e.currentTarget).closest("tr")).JobId;
      jobModel.displayJobDetail(jobid);
    }

    // creates modal window contains details for a specific job that is in process
    function clickDetailsInProcess(jobid) {

      //var g = $("#gridInprocess").data("kendoGrid");
      //var dataItem = g.dataItem($(e.target).closest("tr"));
      //var jobid = dataItem.JobId;

      jobModel.displayJobDetail(jobid);
    }

    function clickDetailsNationBuilderImport(e) {

      var g = $("#nationBuilderImportGrid").data("kendoGrid");
      var dataItem = g.dataItem($(e.target).closest("tr"));
      var jobid = dataItem.JobId;

      jobModel.displayJobDetail(jobid);
    }

    function viewJobReport(jobid, userid) {
      $('.jobDetails').data("kendoWindow").destroy();
      jobReport.display(jobid, userid);
    }

    function setApplicationId() {
      $.cookie('ApplicationId', $('#ApplicationId option:selected').val());
    }

    function truncateProductDescription(description) {
      var len = description.length;
      if (len > 50) {
        return description.substring(0, 40) + '...';
      }
      return description;
    }

    $('.input[name=searchterm]').keypress(function(e) {
      if (e.which == 13) {
        $('form#searchForm').submit();
        return false; //<---- Add this line
      }
    });

  </script>


  <script id="responsive-column-template-complete" type="text/x-kendo-template">
  <p class="col-template-val"><a href="=#: Links.UserDetail #">#= UserName #</a></p>

  <strong>File Count</strong>
  <p class="col-template-val">#=kendo.toString(data.FileCount, "n0")#</p>

  <strong>Record Count</strong>
  <p class="col-template-val">#=kendo.toString(data.RecordCount, "n0")#</p>

  <strong>Matched Records</strong>
  <p class="col-template-val">#=kendo.toString(data.MatchCount, "n0")#</p>
  
  <strong>Last Activity</strong>
  <p class="col-template-val">#=data.LastActivityDescription#</p>
    
  <a href="#: Links.JobsForClient #">View Jobs</a>
</script>

  <script id="responsive-column-template-user" type="text/x-kendo-template">

  <strong>Date Complete</strong>
  <p class="col-template-val">#=data.DateComplete#</p>

  <strong>JobId</strong>
  <p class="col-template-val">#=data.JobId#</p>

  <strong>File Name</strong>
  <p class="col-template-val">#=data.CustomerFileName#</p>
  
  <strong>Source</strong>
  <p class="col-template-val">#=data.SourceDescription#</p>
    
  <strong>Total Records</strong>
  <p class="col-template-val">#=kendo.toString(data.TotalRecords, "n0")#</p>
    
  <strong>Match Records</strong>
  <p class="col-template-val">#=kendo.toString(data.MatchRecords, "n0")#</p>
    
  <strong>Rate</strong>
  <p class="col-template-val">#=kendo.toString(data.MatchRate, "p0")#</p>
    
  <a onclick="jobModel.displayJobDetail(#= data.JobId #)">View Job Details</a>
</script>

  <script id="responsive-column-template-inprocess" type="text/x-kendo-template">
  <strong>Customer</strong>
  <p class="col-template-val"><a href="=#: data.Links.UserDetail #">#= data.UserName #</a></p>

  <strong>Date Submitted</strong>
  <p class="col-template-val">#=data.DateSubmitted#</p>

  <strong>JobId</strong>
  <p class="col-template-val">#=data.JobId#</p>

  <strong>File Name</strong>
  <p class="col-template-val">#=data.CustomerFileName#</p>
  
  <strong>Source</strong>
  <p class="col-template-val">#=data.SourceDescription#</p>
    
  <strong>Total Records</strong>
  <p class="col-template-val">#=kendo.toString(data.RecordCount, "n0")#</p>
    
  <strong>Processed Records</strong>
  <p class="col-template-val">#=kendo.toString(data.ProcessedCount, "n0")#</p>

  <strong>Match Records</strong>
  <p class="col-template-val">#=kendo.toString(data.MatchCount, "n0")#</p>
    
  <strong>Rate</strong>
  <p class="col-template-val">#=kendo.toString(data.MatchRate, "p0")#</p>
    
  <strong>Status</strong>
  #= kendo.render(kendo.template($("\\#statusDescriptionTemplate").html()), [data]) #
    
  <button onclick="jobModel.displayJobDetail(#= data.JobId #)">View Job Details</button>
</script>


  <script type="text/x-kendo-template" id="nationBuilderCommandButtonsTemplate">
    #if (CanResume)  { #
        <a href="\\#" class="btn btn-success btn-sm" onclick="nationBuilderModel.resume(#= Id #)" alt="Resumes selected job">Resume</a> 
    # } #
    #if (CanCancel)  { #
        <a href="\\#" class="btn btn-danger btn-sm" onclick="nationBuilderModel.cancel(#= Id #)" alt="Cancels selected job">Cancel</a>
    # } #
    </script>

  <script type="text/x-kendo-template" id="matchLevelTemplate">
    length #= data.length #
        <table class="table-condensed">
        <tr>
            <td>Match Level</td>
            <td>This File</td>
            <td>User</td>
            <td>System</td>
        </tr>
        # for (var i = 0; i < data.length; i++) { #
            <tr>
                <td>#= data.MatchLevel #</td>
                <td>#= data.File #</td>
                <td>#= data.User #</td>
                <td>#= data.System #</td>
            </tr>
        # } #    
    </table>
    </script>

  <script type="text/x-kendo-template" id="progressBarTemplate">
    #if (kendo.parseInt(TotalRecords) > 1000)  { #
        <div class="jobProgress"></div> 
    # } #
    </script>

  <script type="text/x-kendo-template" id="estCompletionTemplate">
    #if (Progress.EstimatedCompletionTime != '1/1/1900 12:00:00 AM')  { #
        #= Progress.EstimatedCompletionTime #
    # } #
    </script>

  <script type="text/x-kendo-template" id="statusDescriptionTemplate">
    <p style="margin-bottom: 3px;"># switch(Status) {
        case '<%: JobStatus.NeedsReview %>': 
            # <span style='color: orange; font-weight: bold;'><span class='fa fa-warning'></span> #= StatusDescription #</span> #
            break;
        case '<%: JobStatus.Failed %>': 
            # <span style='color: red; font-weight: bold;'><span class='fa fa-exclamation'></span> #= StatusDescription #</span> #
            break;
        default:
            # <span style='color: black'>#= StatusDescription #</span> #
            break;
    } #</p>
    #if (kendo.parseInt(RecordCount) > 1000)  { #
        <div class="jobProgress" style="margin-bottom: 3px;"></div> 
    # } #
    #if (Progress.EstimatedCompletionTime != null)  { #
        <p>Est: #= Progress.EstimatedCompletionTime #</p>
    # } #
    </script>

  <script type="text/x-kendo-template" id="nationBuilderStatusDescriptionTemplate">
    <p style="margin-bottom: 3px;"># switch(Status) {
        case -1: 
            # <span style='color: red; font-weight: bold;'><span class='fa fa-exclamation'></span> #= StatusDescription #</span> #
            break;
        default:
            # <span style='color: black'>#= StatusDescription #</span> #
            break;
    } #</p>
    </script>

  <script type="text/x-kendo-template" id="jobDetailTemplate">
    <div class="panel panel-default" style="margin-top: 10px;">
      <div class="panel-heading">Job Functions</div>
      <div class="panel-body">
    <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.reset('#: Actions.Reset #')"><span class="fa fa-repeat"></span>Reset Job</button>    
        #if(Actions.Resume !== null)
        {#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.resume('#: Actions.Resume #')"><span class="fa fa-repeat"></span>Resume Job</button>
        # }#
        #if(Actions.Pause !== null)
        {#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.pause('#: Actions.Pause #')"><span class="fa fa-repeat"></span>Pause Job</button>
        # }#
        #if(Actions.Review !== null)
        {#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.setJobComplete(#: Links.SetJobComplete #)"><span class="fa fa-check-circle-o"></span>Set Complete</button>
        # }#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.deleteJob(#: JobId #)"><span class="fa fa-warning"></span>Delete Job</button>
        #if(Actions.Reassign !== null)
        {#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.openJobReassignModal(#: JobId #)"><span class="fa fa-user"></span>Reassign Job</button>
        # }#
        #if(Actions.ExistingDeal != null){#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.associateJobWithExistingDeal('#: Actions.ExistingDeal #')"><span class="fa fa-external-link"></span>Update Existing Deal from Job</button>    
        #}#
        #if(Actions.NewDeal != null){#     
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.newDealFromJob('#: Actions.NewDeal #')"><span class="fa fa-plus"></span>New Deal from Job</button>
        #}#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.newJob(#: JobId #,'#: UserId #')"><span class="fa fa-plus"></span>New Job</button>
      </div>
    </div>
        <div class="panel panel-default">
      <div class="panel-heading">File</div>
      <div class="panel-body">
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.downloadInputfile(#: JobId #)"><span class="fa fa-arrow-down"></span>Download Input</button>
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.previewInputFile(#: JobId #)"><span class="fa fa-arrow-down"></span>Preview Input</button>
        #if(Status == <%: (int) JobStatus.Complete %>){#
        
    <div class="btn-group" styley="z-index">
         <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.downloadOutputfile(#: JobId #)"><span class="fa fa-tasks"></span>Download Output</button>
         <button type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="dropdown">
          <span class="caret"></span>
          <span class="sr-only">Toggle Dropdown</span>
         </button>
         <ul class="dropdown-menu" role="menu">
          <li><button class="btn btn-link" onclick="javascript:jobModel.openOutputFileDownloadRenameModal(#: JobId #,'#: CustomerFileName #')">Download Output with Rename</button></li>
         </ul>
        </div>
    
    <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.previewOuputFile(#: JobId #)"><span class="fa fa-arrow-down"></span>Preview Output</button>
        #}#
        #if(Actions.Remap != null){#
        <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.remapInputfile('#: Actions.Remap #')"><span class="fa fa-sitemap"></span>Re-map</button>
        #}#
        <div class="btn-group" styley="z-index">
         <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.downloadManifest('#: Links.DownloadManifest #')"><span class="fa fa-tasks"></span>Download Manifest</button>
         <button type="button" class="btn btn-default dropdown-toggle btn-xs" data-toggle="dropdown">
          <span class="caret"></span>
          <span class="sr-only">Toggle Dropdown</span>
         </button>
         <ul class="dropdown-menu" role="menu">
          <li><a href="#: Links.ViewManifest #">Read</a></li>
         </ul>
        </div>
      </div>
    </div>
    #if(Status == <%: (int) JobStatus.Complete %>){#
    <div class="panel panel-default">
      <div class="panel-heading">Reports</div>
      <div class="panel-body">
          <button type="button" class="btn btn-default btn-xs" onclick="javascript:viewJobReport(#: JobId #, '#: UserId #')">Processing Report</button>
          <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.openTextReport('#: Links.MatchLevelReport #')"><span class="fa fa-tasks"></span>E1,E2.. - View</button>        
          <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.downloadTextReport('#: Links.DownloadMatchLevelReport #')"><span class="fa fa-download"></span>E1,E2.. - Download</button>        
          <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.openTextReport('#: Links.MatchTypeReport #')"><span class="fa fa-tasks"></span>IND,HH - View</button>        
          <button type="button" class="btn btn-default btn-xs" onclick="javascript:jobModel.downloadTextReport('#: Links.DownloadMatchTypeReport #')"><span class="fa fa-download"></span>IND,HH - Download</button>
      </div>
    </div>
    #}#
    <table class="table table-bordered" style="margin-top: 10px;">
        <tr>
            <th style="width: 200px;">Email</th>
            <td style="width: 560px;"><a href="#: Links.UserDetail #">#= UserName #</a></td>
        </tr>
        <tr>
            <th>JobId</th>
            <td>#= JobId #</td>
        </tr>
        <tr>
            <th>Private Link</th>
            <td><input class=form-control" value="#: Links.Detail #" onclick="this.select();" style="width: 98%; padding: 3px 3px 3px 0; border: none;" />
        </td>
        </tr>
        <tr>
            <th>Source Description</th>
            <td>#= Source  #</td>
        </tr>
        #if(Source == 'FTP'){#
        <tr>
            <th>Folder</th>
            <td>#= PublicFtpFolder  #</td>
        </tr>
        #}#
        <tr>
            <th>Product</th>
            <td style="width: 200px;">#=truncateProductDescription(Product)#</td>
        </tr>
        <tr>
            <th>Date Ordered</th>
            <td>#= DateSubmitted #</td>
        </tr>
        #if(Status == 'Complete'){#
        <tr>
            <th>Date Complete</th>
            <td>#= DateComplete #</td>
        </tr>
        #}#
        <tr>
            <th>Status Description</th>
            <td># switch(Status) {
                case <%: (int) JobStatus.NeedsReview %>: 
                    # <span style='color: orange; font-weight: bold;'><span class='fa fa-warning'></span> #= StatusDescription #</span> #
                    break;
                case <%: (int) JobStatus.Failed %>: 
                    # <span style='color: red; font-weight: bold;'><span class='fa fa-exclamation'></span> #= StatusDescription #</span> #
                    break;
                default:
                    # <span style='color: black'>#= StatusDescription #</span> #
                    break;
            } #
            </td>

        </tr>
        #if(Status == <%: (int) JobStatus.EmailVerifyError %> || Status == <%: (int) JobStatus.Failed %> || Status == <%: (int) JobStatus.NeedsReview %>){#
        <tr>
            <th>Error Message</th>
            <td>#= Message #</td>
        </tr>
        #}#
        <tr>
            <th></th>
            <td>
    <a href="#: Links.SliceDetail#">View Events</a>
    #if(Links.ViewDeal != null){#
       &nbsp;&nbsp; <a href="#=Links.ViewDeal#">View Deal</a>
        #}#
    </td>
        </tr>
        <tr>
            <th>Priority</th>
            <td id="jobPriority">#if(Status == <%: (int) JobStatus.Complete %>){#
                    #= Priority #
                #}else{#
                    <select id="priority"></select>
                #}#
            </td>
        </tr>
        <tr>
            <th>File Size</th>
            <td>#= FileSizeDescription #</td>
        </tr>
        <tr>
            <th>Customer File Name</th>
            <td>#= CustomerFileName #</td>
        </tr>
        <tr>
            <th>Total Records</th>
            <td>#= kendo.toString(TotalRecords, "n0") #</td>
        </tr>
        <tr>
            <th>Processed Records</th>
            <td>#= kendo.toString(ProcessedRecords, "n0") #</td>
        </tr>
        <tr>
            <th>Match Records</th>
            <td>#= kendo.toString(MatchRecords, "n0") #</td>
        </tr>
        <tr>
            <th>System Errors</th>
            <td>#= kendo.toString(SystemErrors, "n0") #</td>
        </tr>
    </table>
    
    </script>

  <script type="text/x-kendo-template" id="cmdViewDetailsComplete">
        
        <div class="btn-group" styley="z-index">
          <button type="button" class="btn btn-default" onclick="jobModel.displayJobDetail(#= JobId #)">View Details</button>
          <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
            <span class="caret"></span>
            <span class="sr-only">Toggle Dropdown</span>
          </button>
          <ul class="dropdown-menu" role="menu">
            <li><a href="\\#" onclick="jobModel.previewOuputFile(#= JobId #)">Preview Output</a></li>
            <li><a href="\\#" onclick="jobModel.downloadOutputfile(#= JobId #)">Download Output</a></li>
          </ul>
        </div>
    
    </script>

  <script type="text/x-kendo-template" id="cmdViewDetailsInProcess">
        
        <div class="btn-group" styley="z-index">
          <button type="button" class="btn btn-default" onclick="jobModel.displayJobDetail(#= JobId #)">View Details</button>
          <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
            <span class="caret"></span>
            <span class="sr-only">Toggle Dropdown</span>
          </button>
          <ul class="dropdown-menu" role="menu">
            <li><a href="\\#" onclick="jobModel.previewInputFile(#= JobId #)">Preview Input</a></li>
            <li><a href="\\#" onclick="jobModel.downloadInputfile(#= JobId #)">Download Input</a></li>
          </ul>
        </div>
    
    </script>

  <script type="text/x-kendo-template" id="productTemplate">
        
        <div style="word-wrap: break-word;">#=truncateProductDescription(Product)#</div>
    
    </script>


</asp:Content>