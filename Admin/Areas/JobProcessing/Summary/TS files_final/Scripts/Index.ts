/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />
/// <reference path="JobModel.ts" />
/// <reference path="JobReportModel.ts" />
/// <reference path="NationBuilderModel.ts" />
/// <reference path="OperationReportModel.ts" />
/// <reference path="ParentModel.ts" />

var JobsDateRangeWidget: AccurateAppend.Ui.DateRangeWidget;
var NBDateRangeWidget: AccurateAppend.Ui.DateRangeWidget;
var jobProcessingSummaryViewModel: AccurateAppend.JobProcessing.Summary.ViewModel;
var jobProcessingSummaryJobModel
    AccurateAppend.JobProcessing.Summary.JobModel;
var jobProcessingSummaryJobReportModel: AccurateAppend.JobProcessing.Summary.JobReportModel;
var jobProcessingSummaryNationBuilderModel: AccurateAppend.JobProcessing.Summary.NationBuilderModel;
var jobProcessingSummaryOperationReportModel:
AccurateAppend.JobProcessing.Summary.OperationReportModel
var renderCompleteTimer: any;
var renderUserCompleteTimer: any;
var renderInProcessTimer: any;
var renderNBCompleteTimer: any;
var renderNBInProcessTimer: any;
var siteUsersdataSource: any;
var autocomplete: any;



module AccurateAppend.JobProcessing.Summary {

  // new methods with the same names should be created on the TS class
  // copy the guts of the JS method into the new TS class method

    export class ViewModel extends AccurateAppend.JobProcessing.Summary.ParentModel {
        constructor(Email: string, JobId: number, links: any) {
            super(Email, JobId, links);

        }

        renderSearchResultsGrid() {
            console.log("renderSearchResultsGrid");
            $("#search-results-modal").modal('show');
            var grid = $("#gridCompleteSearchResults").data("kendoGrid");
            if (grid !== undefined && grid !== null) {
                grid.dataSource.read();
            } else {
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
                                    success(result) {
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
        }

        renderUserAssignmentGrid() {
            var grid = $("#gridUsers").data("kendoGrid");
            if (grid !== undefined && grid !== null) {
                grid.dataSource.read();
            } else {
                $("#gridUsers").kendoGrid({
                    dataSource: {
                        type: "json",
                        transport: {
                            read: function (options) {
                                $.ajax({
                                    url: this.links.SearchClients + "?searchterm=" + $('#userSearchTerm').val(),
                                    dataType: 'json',
                                    type: 'GET',
                                    success (result) {
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
                            command: { text: "Select", click: jobProcessingSummaryJobModel.reassignJob },
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
    }
}

function toggleDetail(e) {
    var grid = $(e).closest('.t-grid').data('tGrid');
    if ($(e).hasClass('t-minus')) {
        grid.$rows().each(function (index) { grid.collapseRow(this); });
        $(e).removeClass('t-minus');
    } else {
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
                        success(result) {
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

            },
            {
                title: "Summary",
                template: kendo.template($("#responsive-column-template-user").html()),
                media: "(max-width: 450px)"
            }
        ]
    });
};

function clickDetailsComplete(jobid) {

    jobProcessingSummaryJobModel.displayJobDetail(jobid);
}

function clickDetailsCompleteSingleUser(e) {
    var jobid = this.dataItem($(e.currentTarget).closest("tr")).JobId;
    jobProcessingSummaryJobModel.displayJobDetail(jobid);

};

function clickDetailsInProcess(jobid) {
   jobProcessingSummaryJobModel.displayJobDetail(jobid);
}

function clickDetailsNationBuilderImport(e) {

    var g = $("#nationBuilderImportGrid").data("kendoGrid");
    var dataItem: any = g.dataItem($(e.target).closest("tr"));
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
        return false; //<---- Add this line
    }
});

function initializeViewModel(Email: string, JobId: number, SearchClientURL: any, DateRangeLastYear: any, IdVal: any) {
    $(document).ready(() => {
        $("#sidenav li").removeClass("active");
        $("#sidenav li#li-job").addClass("active");
        var links: any = {
            ListClients: SearchClientURL + "?activeWithin=" + DateRangeLastYear + "&applicationid=" + IdVal,
            SearchClients: "/Clients/SearchClients/Query",
            JobProcessing_InProcess: "/JobProcessing/Queue/InProcess",
            JobProcessing_Complete: "/JobProcessing/Queue/Complete",
            JobProcessing_CompleteSummary: "/JobProcessing/Queue/CompleteSummary",
            NationBuilderList: "/NationBuilder/List",
            ReassignJob: "/JobProcessing/Reassign",
            NewJob: "/Batch/UploadFile/DynamicAppend",
            SearchJobs: "/JobProcessing/Queue/Search"
        }

        this.jobProcessingSummaryViewModel = new AccurateAppend.JobProcessing.Summary.ViewModel(Email, JobId, links);
        this.jobProcessingSummaryJobModel = new AccurateAppend.JobProcessing.Summary.JobModel(Email, JobId, links);
        this.jobProcessingSummaryJobReportModel = new AccurateAppend.JobProcessing.Summary.JobReportModel(Email, JobId, links);
        jobProcessingSummaryNationBuilderModel = new AccurateAppend.JobProcessing.Summary.NationBuilderModel(Email, JobId, links);
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

                AccurateAppend.Ui.DateRangeValue.Last30Days,
                [
                ]));
        //jobProcessingSummaryViewModel.renderCompleteGrid()
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

                ]));
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
        } else if (Email != null) {
            JobsDateRangeWidget.setDateRangeValue(AccurateAppend.Ui.DateRangeValue.Last30Days);
            jobProcessingSummaryViewModel.renderCompleteGrid();
        } else {
            jobProcessingSummaryViewModel.renderCompleteGrid();
        }

        jobProcessingSummaryViewModel.renderInProcessGrid();

        // runs callbacks including grid refresh
        JobsDateRangeWidget.refresh();

        $("#nationBuilderDateRange").change(function () {
            jobProcessingSummaryViewModel.renderNationBuilderCompleteGrid();
        });

        $("#ApplicationId").bind('change',
            function () {
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

        $('a[data-toggle="tab"][href$="nationbuilderpushes"]').on('shown.bs.tab',
            function (e) {
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

        $('a[data-toggle="tab"][href$="jobs"]').on('shown.bs.tab',
            function (e) {
                clearInterval(renderNBCompleteTimer);
                clearInterval(renderNBInProcessTimer);
                jobProcessingSummaryViewModel.renderCompleteGrid();
                jobProcessingSummaryViewModel.renderInProcessGrid();
                renderCompleteTimer = setInterval(jobProcessingSummaryViewModel.renderCompleteGrid, 60000);
                renderInProcessTimer = setInterval(jobProcessingSummaryViewModel.renderInProcessGrid, 60000);
            });

        // timers
        renderCompleteTimer = setInterval(jobProcessingSummaryViewModel.renderCompleteGrid, 60000);
        renderInProcessTimer = setInterval(jobProcessingSummaryViewModel.renderInProcessGrid, 60000);
    });
}