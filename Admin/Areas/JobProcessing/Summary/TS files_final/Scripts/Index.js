var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var JobsDateRangeWidget;
var NBDateRangeWidget;
var jobProcessingSummaryViewModel;
var jobProcessingSummaryJobModel;
AccurateAppend.JobProcessing.Summary.JobModel;
var jobProcessingSummaryJobReportModel;
var jobProcessingSummaryNationBuilderModel;
var jobProcessingSummaryOperationReportModel;
var renderCompleteTimer;
var renderUserCompleteTimer;
var renderInProcessTimer;
var renderNBCompleteTimer;
var renderNBInProcessTimer;
var siteUsersdataSource;
var autocomplete;
var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var ViewModel = (function (_super) {
                __extends(ViewModel, _super);
                function ViewModel(Email, JobId, links) {
                    return _super.call(this, Email, JobId, links) || this;
                }
                ViewModel.prototype.renderSearchResultsGrid = function () {
                    console.log("renderSearchResultsGrid");
                    $("#search-results-modal").modal('show');
                    var grid = $("#gridCompleteSearchResults").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#gridCompleteSearchResults").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = {
                                            searchTerm: $("#searchTerm").val(),
                                            applicationId: $("#ApplicationId").val()
                                        };
                                        $.ajax({
                                            url: jobProcessingSummaryViewModel._links.SearchJobs,
                                            data: data,
                                            dataType: 'json',
                                            type: 'GET',
                                            success: function (result) {
                                                options.success(result);
                                            }
                                        });
                                    }
                                },
                                pageSize: 10,
                                schema: {
                                    type: 'json',
                                    data: "Data",
                                    total: function (response) {
                                        return response.Data.length;
                                    }
                                },
                                change: function () {
                                    if (this.data().length <= 0) {
                                        $("#gridCompleteSearchResultsMessage").text('No jobs found for ' + $("#searchTerm").val()).show();
                                    }
                                    else {
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
                };
                ViewModel.prototype.renderUserAssignmentGrid = function () {
                    var grid = $("#gridUsers").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#gridUsers").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        $.ajax({
                                            url: this.links.SearchClients + "?searchterm=" + $('#userSearchTerm').val(),
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
                                pageSize: 10,
                                change: function () {
                                    if (this.data().length <= 0) {
                                        $("#userInfo").show();
                                        $("#gridUsers").hide();
                                    }
                                    else {
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
                                    command: { text: "Select", click: jobProcessingSummaryJobModel.reassignJob },
                                    title: " ",
                                    width: "110px",
                                    attributes: { style: "text-align: center;" },
                                    template: '<button type="button" class="btn btn-default"><span class="fa fa-edit"></span>Select</button>'
                                }
                            ],
                            scrollable: false
                        });
                    }
                };
                return ViewModel;
            }(AccurateAppend.JobProcessing.Summary.ParentModel));
            Summary.ViewModel = ViewModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
function toggleDetail(e) {
    var grid = $(e).closest('.t-grid').data('tGrid');
    if ($(e).hasClass('t-minus')) {
        grid.$rows().each(function (index) { grid.collapseRow(this); });
        $(e).removeClass('t-minus');
    }
    else {
        grid.$rows().each(function (index) { grid.expandRow(this); });
        $(e).addClass('t-minus');
    }
    return false;
}
function detailInit(e) {
    var detailRow = e.detailRow;
    detailRow.find(".details").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: function (options) {
                    var data = {
                        userid: e.data.UserId,
                        startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                        enddate: moment(JobsDateRangeWidget.getEndDate()).add(10, 'minutes')
                            .format('YYYY-MM-DD H:mm')
                    };
                    $.ajax({
                        url: jobProcessingSummaryViewModel._links.JobProcessing_Complete,
                        data: data,
                        dataType: 'json',
                        type: 'GET',
                        success: function (result) {
                            options.success(result);
                        }
                    });
                }
            },
            pageSize: 10,
            schema: {
                type: 'json',
                data: "Data",
                total: function (response) {
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
                attributes: { style: "text-align: center;" },
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
            },
            {
                field: "CustomerFileName",
                title: "File Name",
                headerAttributes: { style: "text-align: center;" },
                media: "(min-width: 450px)"
            },
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
            },
            {
                title: "Summary",
                template: kendo.template($("#responsive-column-template-user").html()),
                media: "(max-width: 450px)"
            }
        ]
    });
}
;
function clickDetailsComplete(jobid) {
    jobProcessingSummaryJobModel.displayJobDetail(jobid);
}
function clickDetailsCompleteSingleUser(e) {
    var jobid = this.dataItem($(e.currentTarget).closest("tr")).JobId;
    jobProcessingSummaryJobModel.displayJobDetail(jobid);
}
;
function clickDetailsInProcess(jobid) {
    jobProcessingSummaryJobModel.displayJobDetail(jobid);
}
function clickDetailsNationBuilderImport(e) {
    var g = $("#nationBuilderImportGrid").data("kendoGrid");
    var dataItem = g.dataItem($(e.target).closest("tr"));
    var jobid = dataItem.JobId;
    jobProcessingSummaryJobModel.displayJobDetail(jobid);
}
function viewJobReport(jobid, userid) {
    $('.jobDetails').data("kendoWindow").destroy();
    jobProcessingSummaryJobReportModel.display(jobid, userid);
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
$('.input[name=searchterm]').keypress(function (e) {
    if (e.which == 13) {
        $('form#searchForm').submit();
        return false;
    }
});
function initializeViewModel(Email, JobId, SearchClientURL, DateRangeLastYear, IdVal) {
    var _this = this;
    $(document).ready(function () {
        $("#sidenav li").removeClass("active");
        $("#sidenav li#li-job").addClass("active");
        var links = {
            ListClients: SearchClientURL + "?activeWithin=" + DateRangeLastYear + "&applicationid=" + IdVal,
            SearchClients: "/Clients/SearchClients/Query",
            JobProcessing_InProcess: "/JobProcessing/Queue/InProcess",
            JobProcessing_Complete: "/JobProcessing/Queue/Complete",
            JobProcessing_CompleteSummary: "/JobProcessing/Queue/CompleteSummary",
            NationBuilderList: "/NationBuilder/List",
            ReassignJob: "/JobProcessing/Reassign",
            NewJob: "/Batch/UploadFile/DynamicAppend",
            SearchJobs: "/JobProcessing/Queue/Search"
        };
        _this.jobProcessingSummaryViewModel = new AccurateAppend.JobProcessing.Summary.ViewModel(Email, JobId, links);
        _this.jobProcessingSummaryJobModel = new AccurateAppend.JobProcessing.Summary.JobModel(Email, JobId, links);
        _this.jobProcessingSummaryJobReportModel = new AccurateAppend.JobProcessing.Summary.JobReportModel(Email, JobId, links);
        jobProcessingSummaryNationBuilderModel = new AccurateAppend.JobProcessing.Summary.NationBuilderModel(Email, JobId, links);
        JobsDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("jobsDateRange", new AccurateAppend.Ui.DateRangeWidgetSettings([
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
            AccurateAppend.Ui.DateRangeValue.Last60Days,
            AccurateAppend.Ui.DateRangeValue.LastMonth,
            AccurateAppend.Ui.DateRangeValue.Custom
        ], AccurateAppend.Ui.DateRangeValue.Last30Days, []));
        NBDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("nbDateRange", new AccurateAppend.Ui.DateRangeWidgetSettings([
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
            AccurateAppend.Ui.DateRangeValue.Custom
        ], AccurateAppend.Ui.DateRangeValue.Last7Days, []));
        NBDateRangeWidget.refresh();
        siteUsersdataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: jobProcessingSummaryViewModel._links.ListClients
                }
            }
        });
        if (JobId != null) {
            jobProcessingSummaryJobModel.displayJobDetail(JobId);
        }
        else if (Email != null) {
            JobsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
            jobProcessingSummaryViewModel.renderCompleteGrid();
        }
        else {
            jobProcessingSummaryViewModel.renderCompleteGrid();
        }
        jobProcessingSummaryViewModel.renderInProcessGrid();
        JobsDateRangeWidget.refresh();
        $("#nationBuilderDateRange").change(function () {
            jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid();
        });
        $("#ApplicationId").bind('change', function () {
            setApplicationId();
            siteUsersdataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        dataType: "json",
                        url: links.SearchClients
                    }
                }
            });
            clearInterval(renderNBCompleteTimer);
            clearInterval(renderNBInProcessTimer);
            jobProcessingSummaryViewModel.renderCompleteGrid();
            jobProcessingSummaryViewModel.renderInProcessGrid();
            renderCompleteTimer = setInterval(jobProcessingSummaryViewModel.renderCompleteGrid, 60000);
            renderInProcessTimer = setInterval(jobProcessingSummaryViewModel.renderInProcessGrid, 60000);
        });
        $("#searchTerm").keypress(function (e) {
            if (e.which === 13) {
                jobProcessingSummaryViewModel.renderSearchResultsGrid();
                return false;
            }
        });
        $('a[data-toggle="tab"][href$="nationbuilderpushes"]').on('shown.bs.tab', function (e) {
            clearInterval(renderCompleteTimer);
            clearInterval(renderInProcessTimer);
            jobProcessingSummaryViewModel.renderNationBuilderInprocessGrid();
            jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid();
            renderNBCompleteTimer = window.setInterval(jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid, 60000);
            renderNBInProcessTimer = window.setInterval(jobProcessingSummaryViewModel.renderNationBuilderInprocessGrid, 60000);
        });
        $(document).on('change', '#jobsDateRange_dateRange', function () {
            console.log($(this).val());
            jobProcessingSummaryViewModel.renderCompleteGrid();
        });
        $('a[data-toggle="tab"][href$="jobs"]').on('shown.bs.tab', function (e) {
            clearInterval(renderNBCompleteTimer);
            clearInterval(renderNBInProcessTimer);
            jobProcessingSummaryViewModel.renderCompleteGrid();
            jobProcessingSummaryViewModel.renderInProcessGrid();
            renderCompleteTimer = setInterval(jobProcessingSummaryViewModel.renderCompleteGrid, 60000);
            renderInProcessTimer = setInterval(jobProcessingSummaryViewModel.renderInProcessGrid, 60000);
        });
        renderCompleteTimer = setInterval(jobProcessingSummaryViewModel.renderCompleteGrid, 60000);
        renderInProcessTimer = setInterval(jobProcessingSummaryViewModel.renderInProcessGrid, 60000);
    });
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJJbmRleC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7O0FBUUEsSUFBSSxtQkFBc0QsQ0FBQztBQUMzRCxJQUFJLGlCQUFvRCxDQUFDO0FBQ3pELElBQUksNkJBQTZFLENBQUM7QUFDbEYsSUFBSSw0QkFBNEIsQ0FBQTtBQUM1QixjQUFjLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxRQUFRLENBQUM7QUFDbEQsSUFBSSxrQ0FBdUYsQ0FBQztBQUM1RixJQUFJLHNDQUErRixDQUFDO0FBQ3BHLElBQUksd0NBQ3FELENBQUE7QUFDekQsSUFBSSxtQkFBd0IsQ0FBQztBQUM3QixJQUFJLHVCQUE0QixDQUFDO0FBQ2pDLElBQUksb0JBQXlCLENBQUM7QUFDOUIsSUFBSSxxQkFBMEIsQ0FBQztBQUMvQixJQUFJLHNCQUEyQixDQUFDO0FBQ2hDLElBQUksbUJBQXdCLENBQUM7QUFDN0IsSUFBSSxZQUFpQixDQUFDO0FBSXRCLElBQU8sY0FBYyxDQWlLcEI7QUFqS0QsV0FBTyxjQUFjO0lBQUMsSUFBQSxhQUFhLENBaUtsQztJQWpLcUIsV0FBQSxhQUFhO1FBQUMsSUFBQSxPQUFPLENBaUsxQztRQWpLbUMsV0FBQSxPQUFPO1lBS3ZDO2dCQUErQiw2QkFBZ0Q7Z0JBQzNFLG1CQUFZLEtBQWEsRUFBRSxLQUFhLEVBQUUsS0FBVTsyQkFDaEQsa0JBQU0sS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLENBQUM7Z0JBRTlCLENBQUM7Z0JBRUQsMkNBQXVCLEdBQXZCO29CQUNJLE9BQU8sQ0FBQyxHQUFHLENBQUMseUJBQXlCLENBQUMsQ0FBQztvQkFDdkMsQ0FBQyxDQUFDLHVCQUF1QixDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDO29CQUN6QyxJQUFJLElBQUksR0FBRyxDQUFDLENBQUMsNEJBQTRCLENBQUMsQ0FBQyxJQUFJLENBQUMsV0FBVyxDQUFDLENBQUM7b0JB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LEdBQUcsV0FBVyxDQUFDLDZCQUE2QixDQUFDLG1CQUFtQixFQUFFLEtBQUssQ0FBQyxDQUFDO0lBQ2pHLENBQUMsQ0FBQyxDQUFDO0FBQ1AsQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3MvbW9tZW50L21vbWVudC5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIkpvYk1vZGVsLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIkpvYlJlcG9ydE1vZGVsLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIk5hdGlvbkJ1aWxkZXJNb2RlbC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJPcGVyYXRpb25SZXBvcnRNb2RlbC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCJQYXJlbnRNb2RlbC50c1wiIC8+XHJcblxyXG52YXIgSm9ic0RhdGVSYW5nZVdpZGdldDogQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlV2lkZ2V0O1xyXG52YXIgTkJEYXRlUmFuZ2VXaWRnZXQ6IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldDtcclxudmFyIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsOiBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkuVmlld01vZGVsO1xyXG52YXIgam9iUHJvY2Vzc2luZ1N1bW1hcnlKb2JNb2RlbFxyXG4gICAgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LkpvYk1vZGVsO1xyXG52YXIgam9iUHJvY2Vzc2luZ1N1bW1hcnlKb2JSZXBvcnRNb2RlbDogQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LkpvYlJlcG9ydE1vZGVsO1xyXG52YXIgam9iUHJvY2Vzc2luZ1N1bW1hcnlOYXRpb25CdWlsZGVyTW9kZWw6IEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3NpbmcuU3VtbWFyeS5OYXRpb25CdWlsZGVyTW9kZWw7XHJcbnZhciBqb2JQcm9jZXNzaW5nU3VtbWFyeU9wZXJhdGlvblJlcG9ydE1vZGVsOlxyXG5BY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkuT3BlcmF0aW9uUmVwb3J0TW9kZWxcclxudmFyIHJlbmRlckNvbXBsZXRlVGltZXI6IGFueTtcclxudmFyIHJlbmRlclVzZXJDb21wbGV0ZVRpbWVyOiBhbnk7XHJcbnZhciByZW5kZXJJblByb2Nlc3NUaW1lcjogYW55O1xyXG52YXIgcmVuZGVyTkJDb21wbGV0ZVRpbWVyOiBhbnk7XHJcbnZhciByZW5kZXJOQkluUHJvY2Vzc1RpbWVyOiBhbnk7XHJcbnZhciBzaXRlVXNlcnNkYXRhU291cmNlOiBhbnk7XHJcbnZhciBhdXRvY29tcGxldGU6IGFueTtcclxuXHJcblxyXG5cclxubW9kdWxlIEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3NpbmcuU3VtbWFyeSB7XHJcblxyXG4gIC8vIG5ldyBtZXRob2RzIHdpdGggdGhlIHNhbWUgbmFtZXMgc2hvdWxkIGJlIGNyZWF0ZWQgb24gdGhlIFRTIGNsYXNzXHJcbiAgLy8gY29weSB0aGUgZ3V0cyBvZiB0aGUgSlMgbWV0aG9kIGludG8gdGhlIG5ldyBUUyBjbGFzcyBtZXRob2RcclxuXHJcbiAgICBleHBvcnQgY2xhc3MgVmlld01vZGVsIGV4dGVuZHMgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlBhcmVudE1vZGVsIHtcclxuICAgICAgICBjb25zdHJ1Y3RvcihFbWFpbDogc3RyaW5nLCBKb2JJZDogbnVtYmVyLCBsaW5rczogYW55KSB7XHJcbiAgICAgICAgICAgIHN1cGVyKEVtYWlsLCBKb2JJZCwgbGlua3MpO1xyXG5cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbmRlclNlYXJjaFJlc3VsdHNHcmlkKCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhcInJlbmRlclNlYXJjaFJlc3VsdHNHcmlkXCIpO1xyXG4gICAgICAgICAgICAkKFwiI3NlYXJjaC1yZXN1bHRzLW1vZGFsXCIpLm1vZGFsKCdzaG93Jyk7XHJcbiAgICAgICAgICAgIHZhciBncmlkID0gJChcIiNncmlkQ29tcGxldGVTZWFyY2hSZXN1bHRzXCIpLmRhdGEoXCJrZW5kb0dyaWRcIik7XHJcbiAgICAgICAgICAgIGlmIChncmlkICE9PSB1bmRlZmluZWQgJiYgZ3JpZCAhPT0gbnVsbCkge1xyXG4gICAgICAgICAgICAgICAgZ3JpZC5kYXRhU291cmNlLnJlYWQoKTtcclxuICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICQoXCIjZ3JpZENvbXBsZXRlU2VhcmNoUmVzdWx0c1wiKS5rZW5kb0dyaWQoe1xyXG4gICAgICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmVhZDogZnVuY3Rpb24gKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB2YXIgZGF0YSA9IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc2VhcmNoVGVybTogJChcIiNzZWFyY2hUZXJtXCIpLnZhbCgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBhcHBsaWNhdGlvbklkOiAkKFwiI0FwcGxpY2F0aW9uSWRcIikudmFsKClcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVybDogam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwuX2xpbmtzLlNlYXJjaEpvYnMsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFnZVNpemU6IDEwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdG90YWw6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRDb21wbGV0ZVNlYXJjaFJlc3VsdHNNZXNzYWdlXCIpLnRleHQoJ05vIGpvYnMgZm91bmQgZm9yICcgKyAkKFwiI3NlYXJjaFRlcm1cIikudmFsKCkpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNncmlkQ29tcGxldGVTZWFyY2hSZXN1bHRzXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICAgICAgc29ydGFibGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgcGFnZWFibGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJEYXRlQ29tcGxldGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIkRhdGUgQ29tcGxldGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVXNlck5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlVzZXJuYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkpvYklkXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJKb2JJZFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiRmlsZSBOYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogXCIyMDBweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiAnPGRpdiBzdHlsZT1cIndvcmQtd3JhcDogYnJlYWstd29yZDtcIj4jPXRydW5jYXRlUHJvZHVjdERlc2NyaXB0aW9uKEN1c3RvbWVyRmlsZU5hbWUpIzwvZGl2PidcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiU291cmNlRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlNvdXJjZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGVtcGxhdGU6ICc8YSBjbGFzcz1cImJ0biBidG4tZGVmYXVsdFwiIGhyZWY9XCIjOiBMaW5rcy5Kb2JEZXRhaWwgI1wiPlZpZXcgSm9iPC9hPidcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIF1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW5kZXJVc2VyQXNzaWdubWVudEdyaWQoKSB7XHJcbiAgICAgICAgICAgIHZhciBncmlkID0gJChcIiNncmlkVXNlcnNcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgICAgICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNncmlkVXNlcnNcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiB0aGlzLmxpbmtzLlNlYXJjaENsaWVudHMgKyBcIj9zZWFyY2h0ZXJtPVwiICsgJCgnI3VzZXJTZWFyY2hUZXJtJykudmFsKCksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzIChyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ2pzb24nLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0b3RhbDogZnVuY3Rpb24gKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBwYWdlU2l6ZTogMTAsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNoYW5nZTogZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuZGF0YSgpLmxlbmd0aCA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiN1c2VySW5mb1wiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNncmlkVXNlcnNcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI3VzZXJJbmZvXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRVc2Vyc1wiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRW1haWxcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlVzZXJuYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogXCI4MDBweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiAnPGEgaHJlZj1cIj0jOiBMaW5rcy5Vc2VyRGV0YWlsICNcIj4jPSBFbWFpbCAjPC9hPidcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiTGFzdEFjdGl2aXR5RGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIkxhc3QgQWN0aXZpdHlcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY29tbWFuZDogeyB0ZXh0OiBcIlNlbGVjdFwiLCBjbGljazogam9iUHJvY2Vzc2luZ1N1bW1hcnlKb2JNb2RlbC5yZWFzc2lnbkpvYiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiIFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IFwiMTEwcHhcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZTpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAnPGJ1dHRvbiB0eXBlPVwiYnV0dG9uXCIgY2xhc3M9XCJidG4gYnRuLWRlZmF1bHRcIj48c3BhbiBjbGFzcz1cImZhIGZhLWVkaXRcIj48L3NwYW4+U2VsZWN0PC9idXR0b24+J1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgXSxcclxuICAgICAgICAgICAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHRvZ2dsZURldGFpbChlKSB7XHJcbiAgICB2YXIgZ3JpZCA9ICQoZSkuY2xvc2VzdCgnLnQtZ3JpZCcpLmRhdGEoJ3RHcmlkJyk7XHJcbiAgICBpZiAoJChlKS5oYXNDbGFzcygndC1taW51cycpKSB7XHJcbiAgICAgICAgZ3JpZC4kcm93cygpLmVhY2goZnVuY3Rpb24gKGluZGV4KSB7IGdyaWQuY29sbGFwc2VSb3codGhpcyk7IH0pO1xyXG4gICAgICAgICQoZSkucmVtb3ZlQ2xhc3MoJ3QtbWludXMnKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgZ3JpZC4kcm93cygpLmVhY2goZnVuY3Rpb24gKGluZGV4KSB7IGdyaWQuZXhwYW5kUm93KHRoaXMpOyB9KTtcclxuICAgICAgICAkKGUpLmFkZENsYXNzKCd0LW1pbnVzJyk7XHJcbiAgICB9XHJcbiAgICByZXR1cm4gZmFsc2U7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGRldGFpbEluaXQoZSkge1xyXG5cclxuICAgIHZhciBkZXRhaWxSb3cgPSBlLmRldGFpbFJvdztcclxuXHJcbiAgICBkZXRhaWxSb3cuZmluZChcIi5kZXRhaWxzXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgICByZWFkOiBmdW5jdGlvbiAob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgICAgIHZhciBkYXRhID0ge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB1c2VyaWQ6IGUuZGF0YS5Vc2VySWQsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN0YXJ0ZGF0ZTogbW9tZW50KEpvYnNEYXRlUmFuZ2VXaWRnZXQuZ2V0U3RhcnREYXRlKCkpLmZvcm1hdCgnWVlZWS1NTS1ERCBIOm1tJyksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVuZGRhdGU6IG1vbWVudChKb2JzRGF0ZVJhbmdlV2lkZ2V0LmdldEVuZERhdGUoKSkuYWRkKDEwLCAnbWludXRlcycpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKVxyXG4gICAgICAgICAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5fbGlua3MuSm9iUHJvY2Vzc2luZ19Db21wbGV0ZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBwYWdlU2l6ZTogMTAsXHJcbiAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgICAgdHlwZTogJ2pzb24nLFxyXG4gICAgICAgICAgICAgICAgZGF0YTogXCJEYXRhXCIsXHJcbiAgICAgICAgICAgICAgICB0b3RhbDogZnVuY3Rpb24gKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSxcclxuICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICBzb3J0YWJsZTogdHJ1ZSxcclxuICAgICAgICBwYWdlYWJsZTogdHJ1ZSxcclxuICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIGZpZWxkOiBcIkRhdGVDb21wbGV0ZVwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiRGF0ZSBDb21wbGV0ZVwiLFxyXG4gICAgICAgICAgICAgICAgd2lkdGg6IFwiMTYwcHhcIixcclxuICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZDogXCJQcm9jZXNzaW5nVGltZVwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiTWludXRlc1wiLFxyXG4gICAgICAgICAgICAgICAgd2lkdGg6IFwiNzVweFwiLFxyXG4gICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZDogXCJKb2JJZFwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiSm9iSWRcIixcclxuICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgZmllbGQ6IFwiQ3VzdG9tZXJGaWxlTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiRmlsZSBOYW1lXCIsXHJcbiAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgLy97IGZpZWxkOiBcIlByb2R1Y3RcIiwgdGl0bGU6IFwiUHJvZHVjdFwiLCB3aWR0aDogXCIyNTBweFwiLCBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9IH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI3Byb2R1Y3RUZW1wbGF0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgICAgd2lkdGg6IFwiMjUwcHhcIixcclxuICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZDogXCJTb3VyY2VEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiU291cmNlXCIsXHJcbiAgICAgICAgICAgICAgICB3aWR0aDogXCIxMDBweFwiLFxyXG4gICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZDogXCJUb3RhbFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgIHRpdGxlOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgIHdpZHRoOiBcIjYwcHhcIixcclxuICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgIHRpdGxlOiBcIk1hdGNoZWQgUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICAgICAgd2lkdGg6IFwiNjBweFwiLFxyXG4gICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIGZpZWxkOiBcIk1hdGNoUmF0ZVwiLFxyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiUmF0ZVwiLFxyXG4gICAgICAgICAgICAgICAgd2lkdGg6IFwiNzBweFwiLFxyXG4gICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnB9XCIsXHJcbiAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgY29tbWFuZDogeyB0ZXh0OiBcIlZpZXcgRGV0YWlsc1wiLCBjbGljazogY2xpY2tEZXRhaWxzQ29tcGxldGVTaW5nbGVVc2VyIH0sXHJcbiAgICAgICAgICAgICAgICB0aXRsZTogXCIgXCIsXHJcbiAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIixcclxuICAgICAgICAgICAgICAgIHdpZHRoOiBcIjExMHB4XCIsXHJcbiAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG5cclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgdGl0bGU6IFwiU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICAgICAgdGVtcGxhdGU6IGtlbmRvLnRlbXBsYXRlKCQoXCIjcmVzcG9uc2l2ZS1jb2x1bW4tdGVtcGxhdGUtdXNlclwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1heC13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgIH0pO1xyXG59O1xyXG5cclxuZnVuY3Rpb24gY2xpY2tEZXRhaWxzQ29tcGxldGUoam9iaWQpIHtcclxuXHJcbiAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLmRpc3BsYXlKb2JEZXRhaWwoam9iaWQpO1xyXG59XHJcblxyXG5mdW5jdGlvbiBjbGlja0RldGFpbHNDb21wbGV0ZVNpbmdsZVVzZXIoZSkge1xyXG4gICAgdmFyIGpvYmlkID0gdGhpcy5kYXRhSXRlbSgkKGUuY3VycmVudFRhcmdldCkuY2xvc2VzdChcInRyXCIpKS5Kb2JJZDtcclxuICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwuZGlzcGxheUpvYkRldGFpbChqb2JpZCk7XHJcblxyXG59O1xyXG5cclxuZnVuY3Rpb24gY2xpY2tEZXRhaWxzSW5Qcm9jZXNzKGpvYmlkKSB7XHJcbiAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwuZGlzcGxheUpvYkRldGFpbChqb2JpZCk7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGNsaWNrRGV0YWlsc05hdGlvbkJ1aWxkZXJJbXBvcnQoZSkge1xyXG5cclxuICAgIHZhciBnID0gJChcIiNuYXRpb25CdWlsZGVySW1wb3J0R3JpZFwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgdmFyIGRhdGFJdGVtOiBhbnkgPSBnLmRhdGFJdGVtKCQoZS50YXJnZXQpLmNsb3Nlc3QoXCJ0clwiKSk7XHJcbiAgICB2YXIgam9iaWQgPSBkYXRhSXRlbS5Kb2JJZDtcclxuXHJcbiAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLmRpc3BsYXlKb2JEZXRhaWwoam9iaWQpO1xyXG59XHJcblxyXG5mdW5jdGlvbiB2aWV3Sm9iUmVwb3J0KGpvYmlkLCB1c2VyaWQpIHtcclxuICAgICQoJy5qb2JEZXRhaWxzJykuZGF0YShcImtlbmRvV2luZG93XCIpLmRlc3Ryb3koKTtcclxuICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iUmVwb3J0TW9kZWwuZGlzcGxheShqb2JpZCwgdXNlcmlkKTtcclxufVxyXG5cclxuZnVuY3Rpb24gc2V0QXBwbGljYXRpb25JZCgpIHtcclxuICAgICQuY29va2llKCdBcHBsaWNhdGlvbklkJywgJCgnI0FwcGxpY2F0aW9uSWQgb3B0aW9uOnNlbGVjdGVkJykudmFsKCkpO1xyXG59XHJcblxyXG5mdW5jdGlvbiB0cnVuY2F0ZVByb2R1Y3REZXNjcmlwdGlvbihkZXNjcmlwdGlvbikge1xyXG4gICAgdmFyIGxlbiA9IGRlc2NyaXB0aW9uLmxlbmd0aDtcclxuICAgIGlmIChsZW4gPiA1MCkge1xyXG4gICAgICAgIHJldHVybiBkZXNjcmlwdGlvbi5zdWJzdHJpbmcoMCwgNDApICsgJy4uLic7XHJcbiAgICB9XHJcbiAgICByZXR1cm4gZGVzY3JpcHRpb247XHJcbn1cclxuXHJcbiQoJy5pbnB1dFtuYW1lPXNlYXJjaHRlcm1dJykua2V5cHJlc3MoZnVuY3Rpb24gKGUpIHtcclxuICAgIGlmIChlLndoaWNoID09IDEzKSB7XHJcbiAgICAgICAgJCgnZm9ybSNzZWFyY2hGb3JtJykuc3VibWl0KCk7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlOyAvLzwtLS0tIEFkZCB0aGlzIGxpbmVcclxuICAgIH1cclxufSk7XHJcblxyXG5mdW5jdGlvbiBpbml0aWFsaXplVmlld01vZGVsKEVtYWlsOiBzdHJpbmcsIEpvYklkOiBudW1iZXIsIFNlYXJjaENsaWVudFVSTDogYW55LCBEYXRlUmFuZ2VMYXN0WWVhcjogYW55LCBJZFZhbDogYW55KSB7XHJcbiAgICAkKGRvY3VtZW50KS5yZWFkeSgoKSA9PiB7XHJcbiAgICAgICAgJChcIiNzaWRlbmF2IGxpXCIpLnJlbW92ZUNsYXNzKFwiYWN0aXZlXCIpO1xyXG4gICAgICAgICQoXCIjc2lkZW5hdiBsaSNsaS1qb2JcIikuYWRkQ2xhc3MoXCJhY3RpdmVcIik7XHJcbiAgICAgICAgdmFyIGxpbmtzOiBhbnkgPSB7XHJcbiAgICAgICAgICAgIExpc3RDbGllbnRzOiBTZWFyY2hDbGllbnRVUkwgKyBcIj9hY3RpdmVXaXRoaW49XCIgKyBEYXRlUmFuZ2VMYXN0WWVhciArIFwiJmFwcGxpY2F0aW9uaWQ9XCIgKyBJZFZhbCxcclxuICAgICAgICAgICAgU2VhcmNoQ2xpZW50czogXCIvQ2xpZW50cy9TZWFyY2hDbGllbnRzL1F1ZXJ5XCIsXHJcbiAgICAgICAgICAgIEpvYlByb2Nlc3NpbmdfSW5Qcm9jZXNzOiBcIi9Kb2JQcm9jZXNzaW5nL1F1ZXVlL0luUHJvY2Vzc1wiLFxyXG4gICAgICAgICAgICBKb2JQcm9jZXNzaW5nX0NvbXBsZXRlOiBcIi9Kb2JQcm9jZXNzaW5nL1F1ZXVlL0NvbXBsZXRlXCIsXHJcbiAgICAgICAgICAgIEpvYlByb2Nlc3NpbmdfQ29tcGxldGVTdW1tYXJ5OiBcIi9Kb2JQcm9jZXNzaW5nL1F1ZXVlL0NvbXBsZXRlU3VtbWFyeVwiLFxyXG4gICAgICAgICAgICBOYXRpb25CdWlsZGVyTGlzdDogXCIvTmF0aW9uQnVpbGRlci9MaXN0XCIsXHJcbiAgICAgICAgICAgIFJlYXNzaWduSm9iOiBcIi9Kb2JQcm9jZXNzaW5nL1JlYXNzaWduXCIsXHJcbiAgICAgICAgICAgIE5ld0pvYjogXCIvQmF0Y2gvVXBsb2FkRmlsZS9EeW5hbWljQXBwZW5kXCIsXHJcbiAgICAgICAgICAgIFNlYXJjaEpvYnM6IFwiL0pvYlByb2Nlc3NpbmcvUXVldWUvU2VhcmNoXCJcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlZpZXdNb2RlbChFbWFpbCwgSm9iSWQsIGxpbmtzKTtcclxuICAgICAgICB0aGlzLmpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LkpvYk1vZGVsKEVtYWlsLCBKb2JJZCwgbGlua3MpO1xyXG4gICAgICAgIHRoaXMuam9iUHJvY2Vzc2luZ1N1bW1hcnlKb2JSZXBvcnRNb2RlbCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkuSm9iUmVwb3J0TW9kZWwoRW1haWwsIEpvYklkLCBsaW5rcyk7XHJcbiAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlOYXRpb25CdWlsZGVyTW9kZWwgPSBuZXcgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5Lk5hdGlvbkJ1aWxkZXJNb2RlbChFbWFpbCwgSm9iSWQsIGxpbmtzKTtcclxuICAgICAgICBKb2JzRGF0ZVJhbmdlV2lkZ2V0ID0gbmV3IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldChcImpvYnNEYXRlUmFuZ2VcIixcclxuICAgICAgICAgICAgbmV3IEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVdpZGdldFNldHRpbmdzKFxyXG4gICAgICAgICAgICAgICAgW1xyXG4gICAgICAgICAgICAgICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QyNEhvdXJzLFxyXG4gICAgICAgICAgICAgICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3Q3RGF5cyxcclxuICAgICAgICAgICAgICAgICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0MzBEYXlzLFxyXG4gICAgICAgICAgICAgICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3Q2MERheXMsXHJcbiAgICAgICAgICAgICAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdE1vbnRoLFxyXG4gICAgICAgICAgICAgICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkN1c3RvbVxyXG4gICAgICAgICAgICAgICAgXSxcclxuXHJcbiAgICAgICAgICAgICAgICBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VWYWx1ZS5MYXN0MzBEYXlzLFxyXG4gICAgICAgICAgICAgICAgW1xyXG4gICAgICAgICAgICAgICAgXSkpO1xyXG4gICAgICAgIC8vam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyQ29tcGxldGVHcmlkKClcclxuICAgICAgICBOQkRhdGVSYW5nZVdpZGdldCA9IG5ldyBBY2N1cmF0ZUFwcGVuZC5VaS5EYXRlUmFuZ2VXaWRnZXQoXCJuYkRhdGVSYW5nZVwiLFxyXG4gICAgICAgICAgICBuZXcgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlV2lkZ2V0U2V0dGluZ3MoXHJcbiAgICAgICAgICAgICAgICBbXHJcbiAgICAgICAgICAgICAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDI0SG91cnMsXHJcbiAgICAgICAgICAgICAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDdEYXlzLFxyXG4gICAgICAgICAgICAgICAgICAgIEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QzMERheXMsXHJcbiAgICAgICAgICAgICAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuQ3VzdG9tXHJcbiAgICAgICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICAgICAgQWNjdXJhdGVBcHBlbmQuVWkuRGF0ZVJhbmdlVmFsdWUuTGFzdDdEYXlzLFxyXG4gICAgICAgICAgICAgICAgW1xyXG5cclxuICAgICAgICAgICAgICAgIF0pKTtcclxuICAgICAgICBOQkRhdGVSYW5nZVdpZGdldC5yZWZyZXNoKCk7XHJcblxyXG4gICAgICAgIHNpdGVVc2Vyc2RhdGFTb3VyY2UgPSBuZXcga2VuZG8uZGF0YS5EYXRhU291cmNlKHtcclxuICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgICByZWFkOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHVybDogam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwuX2xpbmtzLkxpc3RDbGllbnRzXHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgICAgICBpZiAoSm9iSWQgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLmRpc3BsYXlKb2JEZXRhaWwoSm9iSWQpO1xyXG4gICAgICAgIH0gZWxzZSBpZiAoRW1haWwgIT0gbnVsbCkge1xyXG4gICAgICAgICAgICBKb2JzRGF0ZVJhbmdlV2lkZ2V0LnNldERhdGVSYW5nZVZhbHVlKEFjY3VyYXRlQXBwZW5kLlVpLkRhdGVSYW5nZVZhbHVlLkxhc3QzMERheXMpO1xyXG4gICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJDb21wbGV0ZUdyaWQoKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJDb21wbGV0ZUdyaWQoKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckluUHJvY2Vzc0dyaWQoKTtcclxuXHJcbiAgICAgICAgLy8gcnVucyBjYWxsYmFja3MgaW5jbHVkaW5nIGdyaWQgcmVmcmVzaFxyXG4gICAgICAgIEpvYnNEYXRlUmFuZ2VXaWRnZXQucmVmcmVzaCgpO1xyXG5cclxuICAgICAgICAkKFwiI25hdGlvbkJ1aWxkZXJEYXRlUmFuZ2VcIikuY2hhbmdlKGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyTmF0aW9uQnVpbGRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICAkKFwiI0FwcGxpY2F0aW9uSWRcIikuYmluZCgnY2hhbmdlJyxcclxuICAgICAgICAgICAgZnVuY3Rpb24gKCkge1xyXG4gICAgICAgICAgICAgICAgc2V0QXBwbGljYXRpb25JZCgpO1xyXG4gICAgICAgICAgICAgICAgLy8gYmVnaW4gdXBkYXRlIG9mIGF1dG9jb21wbGV0ZVxyXG4gICAgICAgICAgICAgICAgc2l0ZVVzZXJzZGF0YVNvdXJjZSA9IG5ldyBrZW5kby5kYXRhLkRhdGFTb3VyY2Uoe1xyXG4gICAgICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZWFkOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhVHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB1cmw6IGxpbmtzLlNlYXJjaENsaWVudHNcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgLy8gZW5kIHVwZGF0ZSBvZiBhdXRvY29tcGxldGVcclxuXHJcbiAgICAgICAgICAgICAgICBjbGVhckludGVydmFsKHJlbmRlck5CQ29tcGxldGVUaW1lcik7XHJcbiAgICAgICAgICAgICAgICBjbGVhckludGVydmFsKHJlbmRlck5CSW5Qcm9jZXNzVGltZXIpO1xyXG4gICAgICAgICAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyQ29tcGxldGVHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJJblByb2Nlc3NHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICByZW5kZXJDb21wbGV0ZVRpbWVyID0gc2V0SW50ZXJ2YWwoam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyQ29tcGxldGVHcmlkLCA2MDAwMCk7XHJcbiAgICAgICAgICAgICAgICByZW5kZXJJblByb2Nlc3NUaW1lciA9IHNldEludGVydmFsKGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckluUHJvY2Vzc0dyaWQsIDYwMDAwKTtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICQoXCIjc2VhcmNoVGVybVwiKS5rZXlwcmVzcyhmdW5jdGlvbiAoZSkge1xyXG4gICAgICAgICAgICBpZiAoZS53aGljaCA9PT0gMTMpIHtcclxuICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlclNlYXJjaFJlc3VsdHNHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgJCgnYVtkYXRhLXRvZ2dsZT1cInRhYlwiXVtocmVmJD1cIm5hdGlvbmJ1aWxkZXJwdXNoZXNcIl0nKS5vbignc2hvd24uYnMudGFiJyxcclxuICAgICAgICAgICAgZnVuY3Rpb24gKGUpIHtcclxuICAgICAgICAgICAgICAgIGNsZWFySW50ZXJ2YWwocmVuZGVyQ29tcGxldGVUaW1lcik7XHJcbiAgICAgICAgICAgICAgICBjbGVhckludGVydmFsKHJlbmRlckluUHJvY2Vzc1RpbWVyKTtcclxuICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlck5hdGlvbkJ1aWxkZXJJbnByb2Nlc3NHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJOYXRpb25CdWlsZGVyQ29tcGxldGVHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICByZW5kZXJOQkNvbXBsZXRlVGltZXIgPSB3aW5kb3cuc2V0SW50ZXJ2YWwoam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyTmF0aW9uQnVpbGRlckNvbXBsZXRlR3JpZCwgNjAwMDApO1xyXG4gICAgICAgICAgICAgICAgcmVuZGVyTkJJblByb2Nlc3NUaW1lciA9IHdpbmRvdy5zZXRJbnRlcnZhbChqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJOYXRpb25CdWlsZGVySW5wcm9jZXNzR3JpZCwgNjAwMDApO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAkKGRvY3VtZW50KS5vbignY2hhbmdlJywgJyNqb2JzRGF0ZVJhbmdlX2RhdGVSYW5nZScsIGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coJCh0aGlzKS52YWwoKSk7XHJcbiAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICAkKCdhW2RhdGEtdG9nZ2xlPVwidGFiXCJdW2hyZWYkPVwiam9ic1wiXScpLm9uKCdzaG93bi5icy50YWInLFxyXG4gICAgICAgICAgICBmdW5jdGlvbiAoZSkge1xyXG4gICAgICAgICAgICAgICAgY2xlYXJJbnRlcnZhbChyZW5kZXJOQkNvbXBsZXRlVGltZXIpO1xyXG4gICAgICAgICAgICAgICAgY2xlYXJJbnRlcnZhbChyZW5kZXJOQkluUHJvY2Vzc1RpbWVyKTtcclxuICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVySW5Qcm9jZXNzR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgcmVuZGVyQ29tcGxldGVUaW1lciA9IHNldEludGVydmFsKGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCwgNjAwMDApO1xyXG4gICAgICAgICAgICAgICAgcmVuZGVySW5Qcm9jZXNzVGltZXIgPSBzZXRJbnRlcnZhbChqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJJblByb2Nlc3NHcmlkLCA2MDAwMCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAvLyB0aW1lcnNcclxuICAgICAgICByZW5kZXJDb21wbGV0ZVRpbWVyID0gc2V0SW50ZXJ2YWwoam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwucmVuZGVyQ29tcGxldGVHcmlkLCA2MDAwMCk7XHJcbiAgICAgICAgcmVuZGVySW5Qcm9jZXNzVGltZXIgPSBzZXRJbnRlcnZhbChqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJJblByb2Nlc3NHcmlkLCA2MDAwMCk7XHJcbiAgICB9KTtcclxufSJdfQ==