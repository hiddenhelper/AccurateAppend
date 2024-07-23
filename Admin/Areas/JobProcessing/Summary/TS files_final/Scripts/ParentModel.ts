/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

module AccurateAppend.JobProcessing.Summary {

    export class ParentModel {
        pEmail: any;
        pJobId: any;
        _links: any;

        constructor(Email: string, JobId: number, links: any) {
            this.pEmail = Email;
            this.pJobId = JobId;
            this._links = links;
        }

        renderCompleteGrid() {
            if (jobProcessingSummaryViewModel.pEmail != null) {
                jobProcessingSummaryViewModel.renderCompleteGridForSingleUser();
            } else {
               jobProcessingSummaryViewModel.renderCompleteGridGlobal();
            }
        }

        renderInProcessGrid() {
            console.log('renderInProcessGrid');
            var grid = $("#gridInprocess").data("kendoGrid");
            if (grid !== undefined && grid !== null) {
                grid.dataSource.read();
            } else {
                $("#gridInprocess").kendoGrid({
                    dataSource: {
                        type: "json",
                        transport: {
                            read: function (options) {
                                var data: any = { applicationid: $("#ApplicationId").val() };
                                if (this.pJobId != null)
                                    data.jobid = this.pJobId;
                                else
                                    data.applicationid = $('#ApplicationId').val();
                                $.ajax({
                                    url: jobProcessingSummaryViewModel._links.JobProcessing_InProcess,
                                    dataType: 'json',
                                    type: 'GET',
                                    data: data,
                                    success(result) {
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
                            total: function (response) {
                                return response.Data.length;
                            }
                        },
                        change: function () {
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
        }

        renderNationBuilderInprocessGrid() {
            console.log('renderNationBuilderInprocessGrid');
            $("#nationBuilderPushGridInProcess").kendoGrid({
                autoBind: false,
                dataSource: {
                    type: "json",
                    transport: {
                        read: function (options) {
                            $.ajax({
                                traditional: true,
                                url: jobProcessingSummaryViewModel._links.NationBuilderList,
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
                                success(result) {
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
        }

        renderNationBuilderCompleteGrid() {
            console.log('renderNationBuilderCompleteGrid');
            $("#nationBuilderPushGridComplete").kendoGrid({
                autoBind: false,
                dataSource: {
                    type: "json",
                    transport: {
                        read: function (options) {
                            $.ajax({
                                traditional: true,
                                url: jobProcessingSummaryViewModel._links.NationBuilderList,
                                dataType: 'json',
                                type: 'GET',
                                data: {
                                    pushStatuses: ['Canceled', 'Complete'],
                                    startdate: moment(NBDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                                    enddate: moment(NBDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm')
                                },
                                success(result) {
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
        }

        renderCompleteGridForSingleUser() {
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
                            read: function (options) {
                                var data = {
                                    startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                                    enddate: moment(JobsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm'),
                                    email: jobProcessingSummaryViewModel.pEmail
                                };
                                $.ajax({
                                    url: "/JobProcessing/Queue/Complete",
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
                            console.log('grid single user data count = ' + this.data().length);
                            if (this.data().length <= 0) {
                                $("#completeInfo").show();
                                $("#completeInfo").text('No complete jobs found for ' +
                                    jobProcessingSummaryViewModel.pEmail +
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
                            format: "{0:n0}",
                            attributes: { style: "text-align: right;" },
                            headerAttributes: { style: "text-align: center;" },
                            media: "(min-width: 450px)"
                        },
                        {
                            field: "MatchRecords",
                            title: "Matched Records",
                            format: "{0:n0}",
                            attributes: { style: "text-align: right;" },
                            headerAttributes: { style: "text-align: center;" },
                            media: "(min-width: 450px)"
                        },
                        {
                            field: "MatchRate",
                            title: "Rate",
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
        }

        renderCompleteGridGlobal() {
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
                            read: function (options) {
                                var data: any = {
                                    applicationid: $("#ApplicationId").val(),
                                    startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                                    enddate: moment(JobsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm')
                                };
                                if (this.pJobId != null) data.jobid = this.pJobId;
                                $.ajax({
                                    url:jobProcessingSummaryViewModel._links.JobProcessing_CompleteSummary,
                                    data: data,
                                    dataType: 'json',
                                    type: 'GET',
                                    success(result) {
                                        options.success(result);
                                    }
                                });
                            },
                            cache: false
                        },
                        schema: {
                            type: 'json',
                            data: "Data",
                            total: function (response) {
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
                        change: function () {
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
                    toolbar: [{ template: "<div class=\"toolbar\"><input class=\"btn btn-default\" style=\"margin-right: 10px;\" type=\"submit\" onclick=\"window.location.replace('/Batch/UploadFile/DynamicAppend')\" value=\"New Job\"/>" }],
                    detailTemplate: '<div class="details" style="margin: 5px 5px 10px 5px;"></div>',
                    detailInit: detailInit,
                    scrollable: false,
                    dataBound: function () {
                        var grid = $('#gridComplete').data('kendoGrid');
                        if (this.dataSource.total() == 1) {
                            grid.expandRow($('#gridComplete tbody>tr:first'));
                        }

                        var state: any = sessionStorage.getItem("grid");
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
                    detailExpand: function (e) {

                        if (expandedRow != null && expandedRow[0] != e.masterRow[0]) {
                            var grid = $('#gridComplete').data('kendoGrid');
                            grid.collapseRow(expandedRow);
                        }
                        expandedRow = e.masterRow;

                        var state: any = sessionStorage.getItem("grid");
                        if (!state) {
                            state = {};
                        } else {
                            state = JSON.parse(state);
                        }
                        state[this.dataItem(e.masterRow).id] = true;
                        sessionStorage.setItem("grid", JSON.stringify(state));

                    },
                    detailCollapse: function (e) {
                        // save grid state so expanded rows don't collapse when the dataSource is refreshed
                        var state: any = sessionStorage.getItem("grid");
                        if (state) {
                            state = JSON.parse(state);
                            delete state[this.dataItem(e.masterRow).id];
                            sessionStorage.setItem("grid", JSON.stringify(state));
                        }
                    }
                });
            }
        }
    }
}