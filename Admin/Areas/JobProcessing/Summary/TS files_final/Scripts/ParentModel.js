var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var ParentModel = (function () {
                function ParentModel(Email, JobId, links) {
                    this.pEmail = Email;
                    this.pJobId = JobId;
                    this._links = links;
                }
                ParentModel.prototype.renderCompleteGrid = function () {
                    if (jobProcessingSummaryViewModel.pEmail != null) {
                        jobProcessingSummaryViewModel.renderCompleteGridForSingleUser();
                    }
                    else {
                        jobProcessingSummaryViewModel.renderCompleteGridGlobal();
                    }
                };
                ParentModel.prototype.renderInProcessGrid = function () {
                    console.log('renderInProcessGrid');
                    var grid = $("#gridInprocess").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#gridInprocess").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = { applicationid: $("#ApplicationId").val() };
                                        if (this.pJobId != null)
                                            data.jobid = this.pJobId;
                                        else
                                            data.applicationid = $('#ApplicationId').val();
                                        $.ajax({
                                            url: jobProcessingSummaryViewModel._links.JobProcessing_InProcess,
                                            dataType: 'json',
                                            type: 'GET',
                                            data: data,
                                            success: function (result) {
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
                                    }
                                    else {
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
                };
                ParentModel.prototype.renderNationBuilderInprocessGrid = function () {
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
                                    $("#nationBuilderPushMessageInProcess").show();
                                    $("#nationBuilderPushGridInProcess").hide();
                                }
                                else {
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
                                template: '<a href="=#: Links.Events #">#= ErrorsEncountered #</a>'
                            },
                            {
                                title: " ",
                                width: "110px",
                                attributes: { style: "text-align: center;" },
                                template: kendo.template($("#nationBuilderCommandButtonsTemplate").html())
                            }
                        ]
                    });
                };
                ParentModel.prototype.renderNationBuilderCompleteGrid = function () {
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
                                    $("#nationBuilderPushMessageComplete").show();
                                    $("#nationBuilderPushGridComplete").hide();
                                }
                                else {
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
                                template: '<a href="/Operations/EventLog/Index?correlationId=#: CorrelationId #">#= ErrorsEncountered #</a>'
                            }
                        ]
                    });
                };
                ParentModel.prototype.renderCompleteGridForSingleUser = function () {
                    console.log('renderCompleteGridforSingleUser');
                    $("#gridCompleteSingleUser").show();
                    var grid = $("#gridCompleteSingleUser").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
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
                                    }
                                    else {
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
                                    template: '<span title="#= Product #" data-toggle="tooltip" data-placement="top">#=truncateProductDescription(Product)#</span>',
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
                };
                ParentModel.prototype.renderCompleteGridGlobal = function () {
                    console.log('renderCompleteGrid');
                    var expandedRow;
                    var grid = $("#gridComplete").data("kendoGrid");
                    if (grid !== undefined && grid !== null) {
                        grid.dataSource.read();
                    }
                    else {
                        $("#gridComplete").kendoGrid({
                            dataSource: {
                                type: "json",
                                transport: {
                                    read: function (options) {
                                        var data = {
                                            applicationid: $("#ApplicationId").val(),
                                            startdate: moment(JobsDateRangeWidget.getStartDate()).format('YYYY-MM-DD H:mm'),
                                            enddate: moment(JobsDateRangeWidget.getEndDate()).format('YYYY-MM-DD H:mm')
                                        };
                                        if (this.pJobId != null)
                                            data.jobid = this.pJobId;
                                        $.ajax({
                                            url: jobProcessingSummaryViewModel._links.JobProcessing_CompleteSummary,
                                            data: data,
                                            dataType: 'json',
                                            type: 'GET',
                                            success: function (result) {
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
                                    }
                                    else {
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
                                    template: '<a href="#: Links.UserDetail #">#= UserName #</a><a style="font-size: .8em;margin-left: 5px;" href="#: Links.JobsForClient #">view jobs</a>',
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
                                var state = sessionStorage.getItem("grid");
                                if (state) {
                                    state = JSON.parse(state);
                                    for (var id in state) {
                                        var dataItem = grid.dataSource
                                            .get(id);
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
                                var state = sessionStorage.getItem("grid");
                                if (!state) {
                                    state = {};
                                }
                                else {
                                    state = JSON.parse(state);
                                }
                                state[this.dataItem(e.masterRow).id] = true;
                                sessionStorage.setItem("grid", JSON.stringify(state));
                            },
                            detailCollapse: function (e) {
                                var state = sessionStorage.getItem("grid");
                                if (state) {
                                    state = JSON.parse(state);
                                    delete state[this.dataItem(e.masterRow).id];
                                    sessionStorage.setItem("grid", JSON.stringify(state));
                                }
                            }
                        });
                    }
                };
                return ParentModel;
            }());
            Summary.ParentModel = ParentModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiUGFyZW50TW9kZWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJQYXJlbnRNb2RlbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFHQSxJQUFPLGNBQWMsQ0FpcEJwQjtBQWpwQkQsV0FBTyxjQUFjO0lBQUMsSUFBQSxhQUFhLENBaXBCbEM7SUFqcEJxQixXQUFBLGFBQWE7UUFBQyxJQUFBLE9BQU8sQ0FpcEIxQztRQWpwQm1DLFdBQUEsT0FBTztZQUV2QztnQkFLSSxxQkFBWSxLQUFhLEVBQUUsS0FBYSxFQUFFLEtBQVU7b0JBQ2hELElBQUksQ0FBQyxNQUFNLEdBQUcsS0FBSyxDQUFDO29CQUNwQixJQUFJLENBQUMsTUFBTSxHQUFHLEtBQUssQ0FBQztvQkFDcEIsSUFBSSxDQUFDLE1BQU0sR0FBRyxLQUFLLENBQUM7Z0JBQ3hCLENBQUM7Z0JBRUQsd0NBQWtCLEdBQWxCO29CQUNJLElBQUksNkJBQTZCLENBQUMsTUFBTSxJQUFJLElBQUksRUFBRTt3QkFDOUMsNkJBQTZCLENBQUMsK0JBQStCLEVBQUUsQ0FBQztxQkFDbkU7eUJBQU07d0JBQ0osNkJBQTZCLENBQUMsd0JBQXdCLEVBQUUsQ0FBQztxQkFDM0Q7Z0JBQ0wsQ0FBQztnQkFFRCx5Q0FBbUIsR0FBbkI7b0JBQ0ksT0FBTyxDQUFDLEdBQUcsQ0FBQyxxQkFBcUIsQ0FBQyxDQUFDO29CQUNuQyxJQUFJLElBQUksR0FBRyxDQUFDLENBQUMsZ0JBQWdCLENBQUMsQ0FBQyxJQUFJLENBQUMsV0FBVyxDQUFDLENBQUM7b0JBQ2pELElBQUksSUFBSSxLQUFLLFNBQVMsSUFBSSxJQUFJLEtBQUssSUFBSSxFQUFFO3dCQUNyQyxJQUFJLENBQUMsVUFBVSxDQUFDLElBQUksRUFBRSxDQUFDO3FCQUMxQjt5QkFBTTt3QkFDSCxDQUFDLENBQUMsZ0JBQWdCLENBQUMsQ0FBQyxTQUFTLENBQUM7NEJBQzFCLFVBQVUsRUFBRTtnQ0FDUixJQUFJLEVBQUUsTUFBTTtnQ0FDWixTQUFTLEVBQUU7b0NB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LENBQUMsTUFBTSxDQUFDLENBQUM7Z0NBQ2hELElBQUksS0FBSyxFQUFFO29DQUNQLEtBQUssR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxDQUFDO29DQUMxQixPQUFPLEtBQUssQ0FBQyxJQUFJLENBQUMsUUFBUSxDQUFDLENBQUMsQ0FBQyxTQUFTLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQztvQ0FDNUMsY0FBYyxDQUFDLE9BQU8sQ0FBQyxNQUFNLEVBQUUsSUFBSSxDQUFDLFNBQVMsQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDO2lDQUN6RDs0QkFDTCxDQUFDO3lCQUNKLENBQUMsQ0FBQztxQkFDTjtnQkFDTCxDQUFDO2dCQUNMLGtCQUFDO1lBQUQsQ0FBQyxBQTlvQkQsSUE4b0JDO1lBOW9CWSxtQkFBVyxjQThvQnZCLENBQUE7UUFDTCxDQUFDLEVBanBCbUMsT0FBTyxHQUFQLHFCQUFPLEtBQVAscUJBQU8sUUFpcEIxQztJQUFELENBQUMsRUFqcEJxQixhQUFhLEdBQWIsNEJBQWEsS0FBYiw0QkFBYSxRQWlwQmxDO0FBQUQsQ0FBQyxFQWpwQk0sY0FBYyxLQUFkLGNBQWMsUUFpcEJwQiIsInNvdXJjZXNDb250ZW50IjpbIi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3MvbW9tZW50L21vbWVudC5kLnRzXCIgLz5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9rZW5kby11aS9rZW5kby11aS5kLnRzXCIgLz5cclxuXHJcbm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkge1xyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBQYXJlbnRNb2RlbCB7XHJcbiAgICAgICAgcEVtYWlsOiBhbnk7XHJcbiAgICAgICAgcEpvYklkOiBhbnk7XHJcbiAgICAgICAgX2xpbmtzOiBhbnk7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKEVtYWlsOiBzdHJpbmcsIEpvYklkOiBudW1iZXIsIGxpbmtzOiBhbnkpIHtcclxuICAgICAgICAgICAgdGhpcy5wRW1haWwgPSBFbWFpbDtcclxuICAgICAgICAgICAgdGhpcy5wSm9iSWQgPSBKb2JJZDtcclxuICAgICAgICAgICAgdGhpcy5fbGlua3MgPSBsaW5rcztcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbmRlckNvbXBsZXRlR3JpZCgpIHtcclxuICAgICAgICAgICAgaWYgKGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnBFbWFpbCAhPSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5yZW5kZXJDb21wbGV0ZUdyaWRGb3JTaW5nbGVVc2VyKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnJlbmRlckNvbXBsZXRlR3JpZEdsb2JhbCgpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW5kZXJJblByb2Nlc3NHcmlkKCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZygncmVuZGVySW5Qcm9jZXNzR3JpZCcpO1xyXG4gICAgICAgICAgICB2YXIgZ3JpZCA9ICQoXCIjZ3JpZElucHJvY2Vzc1wiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICAgICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiI2dyaWRJbnByb2Nlc3NcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGRhdGE6IGFueSA9IHsgYXBwbGljYXRpb25pZDogJChcIiNBcHBsaWNhdGlvbklkXCIpLnZhbCgpIH07XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMucEpvYklkICE9IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEuam9iaWQgPSB0aGlzLnBKb2JJZDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGEuYXBwbGljYXRpb25pZCA9ICQoJyNBcHBsaWNhdGlvbklkJykudmFsKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5fbGlua3MuSm9iUHJvY2Vzc2luZ19JblByb2Nlc3MsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiBkYXRhLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjYWNoZTogZmFsc2VcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFnZVNpemU6IDIwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdG90YWw6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2luUHJvY2Vzc0luZm9cIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZ3JpZElucHJvY2Vzc1wiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjaW5Qcm9jZXNzSW5mb1wiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNncmlkSW5wcm9jZXNzXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgcGFnZWFibGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJTdWJtaXR0ZWREZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiU3VibWl0dGVkXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogMTQwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJVc2VyTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiVXNlcm5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiAyMDAsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGVtcGxhdGU6ICc8YSBocmVmPVwiPSM6IExpbmtzLlVzZXJEZXRhaWwgI1wiPiM9IFVzZXJOYW1lICM8L2E+JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkN1c3RvbWVyRmlsZU5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIkZpbGUgTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJSZWNvcmRDb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IDYwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlByb2Nlc3NlZENvdW50XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJQcm9jZXNzZWRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiA2MCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaENvdW50XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJNYXRjaGVkIFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiA2MCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaFJhdGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlJhdGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiA3MCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpwfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlN0YXR1c0Rlc2NyaXB0aW9uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJTdGF0dXNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiAxNzUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNzdGF0dXNEZXNjcmlwdGlvblRlbXBsYXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI2NtZFZpZXdEZXRhaWxzSW5Qcm9jZXNzXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogMjAwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlN1bW1hcnlcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI3Jlc3BvbnNpdmUtY29sdW1uLXRlbXBsYXRlLWlucHJvY2Vzc1wiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1heC13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2VcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW5kZXJOYXRpb25CdWlsZGVySW5wcm9jZXNzR3JpZCgpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coJ3JlbmRlck5hdGlvbkJ1aWxkZXJJbnByb2Nlc3NHcmlkJyk7XHJcbiAgICAgICAgICAgICQoXCIjbmF0aW9uQnVpbGRlclB1c2hHcmlkSW5Qcm9jZXNzXCIpLmtlbmRvR3JpZCh7XHJcbiAgICAgICAgICAgICAgICBhdXRvQmluZDogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJqc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdHJhbnNwb3J0OiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHJlYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRyYWRpdGlvbmFsOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHVybDogam9iUHJvY2Vzc2luZ1N1bW1hcnlWaWV3TW9kZWwuX2xpbmtzLk5hdGlvbkJ1aWxkZXJMaXN0LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwdXNoU3RhdHVzZXM6IFtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICc8JTogUHVzaFN0YXR1cy5SZXZpZXcgJT4nLCAnPCU6IFB1c2hTdGF0dXMuRmFpbGVkICU+JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICc8JTogUHVzaFN0YXR1cy5QZW5kaW5nICU+JywgJzwlOiBQdXNoU3RhdHVzLkFjcXVpcmVkICU+JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICc8JTogUHVzaFN0YXR1cy5Qcm9jZXNzaW5nICU+JywgJzwlOiBQdXNoU3RhdHVzLlB1c2hpbmcgJT4nLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJzwlOiBQdXNoU3RhdHVzLlBhdXNlZCAlPidcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgXSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3RhcnRkYXRlOiBtb21lbnQoKS5zdWJ0cmFjdCg1LCAnbW9udGhzJykuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZW5kZGF0ZTogbW9tZW50KCkuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ2pzb24nLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiBcIkRhdGFcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdG90YWw6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHJlc3BvbnNlLkRhdGEubGVuZ3RoO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBwYWdlU2l6ZTogMjAsXHJcbiAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLmRhdGEoKS5sZW5ndGggPD0gMCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNuYXRpb25CdWlsZGVyUHVzaE1lc3NhZ2VJblByb2Nlc3NcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNuYXRpb25CdWlsZGVyUHVzaEdyaWRJblByb2Nlc3NcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNuYXRpb25CdWlsZGVyUHVzaE1lc3NhZ2VJblByb2Nlc3NcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNuYXRpb25CdWlsZGVyUHVzaEdyaWRJblByb2Nlc3NcIikuc2hvdygpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHNjcm9sbGFibGU6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgc29ydGFibGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICBwYWdlYWJsZToge1xyXG4gICAgICAgICAgICAgICAgICAgIGlucHV0OiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIG51bWVyaWM6IGZhbHNlXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY29sdW1uczogW1xyXG4gICAgICAgICAgICAgICAgICAgIHsgZmllbGQ6IFwiSWRcIiwgdGl0bGU6IFwiSWRcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHsgZmllbGQ6IFwiUmVxdWVzdERhdGVcIiwgdGl0bGU6IFwiRGF0ZVwiLCB3aWR0aDogXCIxNzVweFwiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJVc2VyTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJVc2VybmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZTogJzxhIGhyZWY9XCI9IzogTGlua3MuVXNlckRldGFpbCAjXCI+Iz0gVXNlck5hbWUgIzwvYT4nXHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICB7IGZpZWxkOiBcIk5hbWVcIiwgdGl0bGU6IFwiTGlzdCBOYW1lXCIsIHRlbXBsYXRlOiAnPGEgaHJlZj1cIj0jOiBMaW5rcy5Kb2JEZXRhaWwgI1wiPiM9IE5hbWUgIzwvYT4nIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgeyBmaWVsZDogXCJOYXRpb25OYW1lXCIsIHRpdGxlOiBcIk5hdGlvbiBOYW1lXCIgfSxcclxuICAgICAgICAgICAgICAgICAgICB7IGZpZWxkOiBcIlByb2R1Y3RcIiwgdGl0bGU6IFwiRGVzY3JpcHRpb25cIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiU3RhdHVzRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI25hdGlvbkJ1aWxkZXJTdGF0dXNEZXNjcmlwdGlvblRlbXBsYXRlXCIpLmh0bWwoKSlcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVG90YWxSZWNvcmRzXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiUHJvZ3Jlc3NcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiUHJvZ3Jlc3NcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRXJyb3JzRW5jb3VudGVyZWRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiRXJyb3IgQ291bnRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJzxhIGhyZWY9XCI9IzogTGlua3MuRXZlbnRzICNcIj4jPSBFcnJvcnNFbmNvdW50ZXJlZCAjPC9hPidcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiIFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogXCIxMTBweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNuYXRpb25CdWlsZGVyQ29tbWFuZEJ1dHRvbnNUZW1wbGF0ZVwiKS5odG1sKCkpXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgXVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbmRlck5hdGlvbkJ1aWxkZXJDb21wbGV0ZUdyaWQoKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUubG9nKCdyZW5kZXJOYXRpb25CdWlsZGVyQ29tcGxldGVHcmlkJyk7XHJcbiAgICAgICAgICAgICQoXCIjbmF0aW9uQnVpbGRlclB1c2hHcmlkQ29tcGxldGVcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgIGF1dG9CaW5kOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImpzb25cIixcclxuICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVhZDogZnVuY3Rpb24gKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHJhZGl0aW9uYWw6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBqb2JQcm9jZXNzaW5nU3VtbWFyeVZpZXdNb2RlbC5fbGlua3MuTmF0aW9uQnVpbGRlckxpc3QsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnR0VUJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHB1c2hTdGF0dXNlczogWydDYW5jZWxlZCcsICdDb21wbGV0ZSddLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdGFydGRhdGU6IG1vbWVudChOQkRhdGVSYW5nZVdpZGdldC5nZXRTdGFydERhdGUoKSkuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZW5kZGF0ZTogbW9tZW50KE5CRGF0ZVJhbmdlV2lkZ2V0LmdldEVuZERhdGUoKSkuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHNjaGVtYToge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0b3RhbDogZnVuY3Rpb24gKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICByZXR1cm4gcmVzcG9uc2UuRGF0YS5sZW5ndGg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHBhZ2VTaXplOiAyMCxcclxuICAgICAgICAgICAgICAgICAgICBjaGFuZ2U6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHRoaXMuZGF0YSgpLmxlbmd0aCA8PSAwKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25hdGlvbkJ1aWxkZXJQdXNoTWVzc2FnZUNvbXBsZXRlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjbmF0aW9uQnVpbGRlclB1c2hHcmlkQ29tcGxldGVcIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNuYXRpb25CdWlsZGVyUHVzaE1lc3NhZ2VDb21wbGV0ZVwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI25hdGlvbkJ1aWxkZXJQdXNoR3JpZENvbXBsZXRlXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgIHNvcnRhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgcGFnZWFibGU6IHtcclxuICAgICAgICAgICAgICAgICAgICBpbnB1dDogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBudW1lcmljOiBmYWxzZVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgICAgICAgICB7IGZpZWxkOiBcIklkXCIsIHRpdGxlOiBcIklkXCIgfSxcclxuICAgICAgICAgICAgICAgICAgICB7IGZpZWxkOiBcIlJlcXVlc3REYXRlXCIsIHRpdGxlOiBcIkRhdGVcIiwgd2lkdGg6IFwiMTc1cHhcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVXNlck5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiVXNlcm5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGVtcGxhdGU6ICc8YSBocmVmPVwiPSM6IExpbmtzLlVzZXJEZXRhaWwgI1wiPiM9IFVzZXJOYW1lICM8L2E+J1xyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgeyBmaWVsZDogXCJOYW1lXCIsIHRpdGxlOiBcIkxpc3QgTmFtZVwiLCB0ZW1wbGF0ZTogJzxhIGhyZWY9XCIjOiBMaW5rcy5Kb2JEZXRhaWwgI1wiPiM9IE5hbWUgIzwvYT4nIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgeyBmaWVsZDogXCJOYXRpb25OYW1lXCIsIHRpdGxlOiBcIk5hdGlvbiBOYW1lXCIgfSxcclxuICAgICAgICAgICAgICAgICAgICB7IGZpZWxkOiBcIlByb2R1Y3RcIiwgdGl0bGU6IFwiRGVzY3JpcHRpb25cIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiU3RhdHVzRGVzY3JpcHRpb25cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiU3RhdHVzXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOiBrZW5kby50ZW1wbGF0ZSgkKFwiI25hdGlvbkJ1aWxkZXJTdGF0dXNEZXNjcmlwdGlvblRlbXBsYXRlXCIpLmh0bWwoKSlcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVG90YWxSZWNvcmRzXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiUHJvZ3Jlc3NcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiUHJvZ3Jlc3NcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRXJyb3JzRW5jb3VudGVyZWRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiRXJyb3IgQ291bnRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJzxhIGhyZWY9XCIvT3BlcmF0aW9ucy9FdmVudExvZy9JbmRleD9jb3JyZWxhdGlvbklkPSM6IENvcnJlbGF0aW9uSWQgI1wiPiM9IEVycm9yc0VuY291bnRlcmVkICM8L2E+J1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIF1cclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW5kZXJDb21wbGV0ZUdyaWRGb3JTaW5nbGVVc2VyKCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZygncmVuZGVyQ29tcGxldGVHcmlkZm9yU2luZ2xlVXNlcicpO1xyXG4gICAgICAgICAgICAkKFwiI2dyaWRDb21wbGV0ZVNpbmdsZVVzZXJcIikuc2hvdygpO1xyXG4gICAgICAgICAgICB2YXIgZ3JpZCA9ICQoXCIjZ3JpZENvbXBsZXRlU2luZ2xlVXNlclwiKS5kYXRhKFwia2VuZG9HcmlkXCIpO1xyXG4gICAgICAgICAgICBpZiAoZ3JpZCAhPT0gdW5kZWZpbmVkICYmIGdyaWQgIT09IG51bGwpIHtcclxuICAgICAgICAgICAgICAgIGdyaWQuZGF0YVNvdXJjZS5yZWFkKCk7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAkKFwiI2dyaWRDb21wbGV0ZVNpbmdsZVVzZXJcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGRhdGEgPSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN0YXJ0ZGF0ZTogbW9tZW50KEpvYnNEYXRlUmFuZ2VXaWRnZXQuZ2V0U3RhcnREYXRlKCkpLmZvcm1hdCgnWVlZWS1NTS1ERCBIOm1tJyksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGVuZGRhdGU6IG1vbWVudChKb2JzRGF0ZVJhbmdlV2lkZ2V0LmdldEVuZERhdGUoKSkuZm9ybWF0KCdZWVlZLU1NLUREIEg6bW0nKSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZW1haWw6IGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnBFbWFpbFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBcIi9Kb2JQcm9jZXNzaW5nL1F1ZXVlL0NvbXBsZXRlXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IGRhdGEsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGFnZVNpemU6IDEwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdG90YWw6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBjb25zb2xlLmxvZygnZ3JpZCBzaW5nbGUgdXNlciBkYXRhIGNvdW50ID0gJyArIHRoaXMuZGF0YSgpLmxlbmd0aCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2NvbXBsZXRlSW5mb1wiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNjb21wbGV0ZUluZm9cIikudGV4dCgnTm8gY29tcGxldGUgam9icyBmb3VuZCBmb3IgJyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLnBFbWFpbCArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICcgZm9yIHRoZSBkYXRlIHJhbmdlICcgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoSm9ic0RhdGVSYW5nZVdpZGdldC5nZXRTdGFydERhdGUoKSkuZm9ybWF0KFwiTU0tREQtWVlZWVwiKSArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICcgdG8gJyArXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1vbWVudChKb2JzRGF0ZVJhbmdlV2lkZ2V0LmdldEVuZERhdGUoKSkuZm9ybWF0KFwiTU0tREQtWVlZWVwiKSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNncmlkQ29tcGxldGVTaW5nbGVVc2VyXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJChcIiNjb21wbGV0ZUluZm9cIikuaGlkZSgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjZ3JpZENvbXBsZXRlU2luZ2xlVXNlclwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJCgnW3JvbGU9XCJncmlkY2VsbFwiXSBzcGFuJykudG9vbHRpcCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICBzb3J0YWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBwYWdlYWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBjb2x1bW5zOiBbXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkRhdGVDb21wbGV0ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiRGF0ZSBDb21wbGV0ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IFwiMTYwcHhcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJVc2VyTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiVXNlcm5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiAyMDAsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGVtcGxhdGU6ICc8YSBocmVmPVwiPSM6IExpbmtzLlVzZXJEZXRhaWwgI1wiPiM9IFVzZXJOYW1lICM8L2E+JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkpvYklkXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJKb2JJZFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IFwiNzVweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJDdXN0b21lckZpbGVOYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJGaWxlIE5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiBcIjIwMHB4XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiUHJvZHVjdFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiUHJvZHVjdFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IFwiMjUwcHhcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZTpcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAnPHNwYW4gdGl0bGU9XCIjPSBQcm9kdWN0ICNcIiBkYXRhLXRvZ2dsZT1cInRvb2x0aXBcIiBkYXRhLXBsYWNlbWVudD1cInRvcFwiPiM9dHJ1bmNhdGVQcm9kdWN0RGVzY3JpcHRpb24oUHJvZHVjdCkjPC9zcGFuPicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJTb3VyY2VEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiU291cmNlXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogXCIxMDBweFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJUb3RhbFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIk1hdGNoZWQgUmVjb3Jkc1wiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiByaWdodDtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIk1hdGNoUmF0ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiUmF0ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnB9XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBoZWFkZXJBdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IGNlbnRlcjtcIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1pbi13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgd2lkdGg6IFwiMjAwcHhcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNjbWRWaWV3RGV0YWlsc0NvbXBsZXRlXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJTdW1tYXJ5XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNyZXNwb25zaXZlLWNvbHVtbi10ZW1wbGF0ZS11c2VyXCIpLmh0bWwoKSksXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWF4LXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgXVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJlbmRlckNvbXBsZXRlR3JpZEdsb2JhbCgpIHtcclxuICAgICAgICAgICAgY29uc29sZS5sb2coJ3JlbmRlckNvbXBsZXRlR3JpZCcpO1xyXG4gICAgICAgICAgICB2YXIgZXhwYW5kZWRSb3c7XHJcbiAgICAgICAgICAgIHZhciBncmlkID0gJChcIiNncmlkQ29tcGxldGVcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgICAgICAgaWYgKGdyaWQgIT09IHVuZGVmaW5lZCAmJiBncmlkICE9PSBudWxsKSB7XHJcbiAgICAgICAgICAgICAgICBncmlkLmRhdGFTb3VyY2UucmVhZCgpO1xyXG4gICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgJChcIiNncmlkQ29tcGxldGVcIikua2VuZG9HcmlkKHtcclxuICAgICAgICAgICAgICAgICAgICBkYXRhU291cmNlOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwianNvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJlYWQ6IGZ1bmN0aW9uIChvcHRpb25zKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGRhdGE6IGFueSA9IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgYXBwbGljYXRpb25pZDogJChcIiNBcHBsaWNhdGlvbklkXCIpLnZhbCgpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdGFydGRhdGU6IG1vbWVudChKb2JzRGF0ZVJhbmdlV2lkZ2V0LmdldFN0YXJ0RGF0ZSgpKS5mb3JtYXQoJ1lZWVktTU0tREQgSDptbScpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBlbmRkYXRlOiBtb21lbnQoSm9ic0RhdGVSYW5nZVdpZGdldC5nZXRFbmREYXRlKCkpLmZvcm1hdCgnWVlZWS1NTS1ERCBIOm1tJylcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLnBKb2JJZCAhPSBudWxsKSBkYXRhLmpvYmlkID0gdGhpcy5wSm9iSWQ7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOmpvYlByb2Nlc3NpbmdTdW1tYXJ5Vmlld01vZGVsLl9saW5rcy5Kb2JQcm9jZXNzaW5nX0NvbXBsZXRlU3VtbWFyeSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogZGF0YSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGNhY2hlOiBmYWxzZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IFwiRGF0YVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdG90YWw6IGZ1bmN0aW9uIChyZXNwb25zZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHJldHVybiByZXNwb25zZS5EYXRhLmxlbmd0aDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtb2RlbDoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlkOiBcIlVzZXJJZFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIFVzZXJJZDogXCJVc2VySWRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBVc2VyTmFtZTogXCJVc2VyTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEZpbGVDb3VudDogXCJGaWxlQ291bnRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBSZWNvcmRDb3VudDogXCJSZWNvcmRDb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIE1hdGNoQ291bnQ6IFwiTWF0Y2hDb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIExhc3RBY3Rpdml0eTogXCJMYXN0QWN0aXZpdHlcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBMYXN0QWN0aXZpdHlEZXNjcmlwdGlvbjogXCJMYXN0QWN0aXZpdHlEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIEpvYnM6IFwiSm9ic1wiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhZ2VTaXplOiAyMCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbiAoKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAodGhpcy5kYXRhKCkubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2NvbXBsZXRlSW5mb1wiKS50ZXh0KCdObyBjb21wbGV0ZSBqb2JzIGZvdW5kIGZvciB0aGUgZGF0ZSByYW5nZSAnICtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgbW9tZW50KEpvYnNEYXRlUmFuZ2VXaWRnZXQuZ2V0U3RhcnREYXRlKCkpLmZvcm1hdChcIk1NLURELVlZWVlcIikgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAnIHRvICcgK1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBtb21lbnQoSm9ic0RhdGVSYW5nZVdpZGdldC5nZXRFbmREYXRlKCkpLmZvcm1hdChcIk1NLURELVlZWVlcIikpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjY29tcGxldGVJbmZvXCIpLnNob3coKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRDb21wbGV0ZVwiKS5oaWRlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjY29tcGxldGVJbmZvXCIpLmhpZGUoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2dyaWRDb21wbGV0ZVwiKS5zaG93KCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHBhZ2VhYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIGNvbHVtbnM6IFtcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVXNlck5hbWVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRpdGxlOiBcIlVzZXJuYW1lXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB3aWR0aDogODAwLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRlbXBsYXRlOlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICc8YSBocmVmPVwiIzogTGlua3MuVXNlckRldGFpbCAjXCI+Iz0gVXNlck5hbWUgIzwvYT48YSBzdHlsZT1cImZvbnQtc2l6ZTogLjhlbTttYXJnaW4tbGVmdDogNXB4O1wiIGhyZWY9XCIjOiBMaW5rcy5Kb2JzRm9yQ2xpZW50ICNcIj52aWV3IGpvYnM8L2E+JyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkZpbGVDb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiRmlsZSBDb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOm4wfVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogcmlnaHQ7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG1lZGlhOiBcIihtaW4td2lkdGg6IDQ1MHB4KVwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlJlY29yZENvdW50XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJSZWNvcmQgQ291bnRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaENvdW50XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJNYXRjaGVkIFJlY29yZHNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpuMH1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGhlYWRlckF0dHJpYnV0ZXM6IHsgc3R5bGU6IFwidGV4dC1hbGlnbjogY2VudGVyO1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJMYXN0QWN0aXZpdHlEZXNjcmlwdGlvblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU6IFwiTGFzdCBBY3Rpdml0eVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaGVhZGVyQXR0cmlidXRlczogeyBzdHlsZTogXCJ0ZXh0LWFsaWduOiBjZW50ZXI7XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiAxNzUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBhdHRyaWJ1dGVzOiB7IHN0eWxlOiBcInRleHQtYWxpZ246IHJpZ2h0O1wiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBtZWRpYTogXCIobWluLXdpZHRoOiA0NTBweClcIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJTdW1tYXJ5XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNyZXNwb25zaXZlLWNvbHVtbi10ZW1wbGF0ZS1jb21wbGV0ZVwiKS5odG1sKCkpLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbWVkaWE6IFwiKG1heC13aWR0aDogNDUwcHgpXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgICAgICAgICAgdG9vbGJhcjogW3sgdGVtcGxhdGU6IFwiPGRpdiBjbGFzcz1cXFwidG9vbGJhclxcXCI+PGlucHV0IGNsYXNzPVxcXCJidG4gYnRuLWRlZmF1bHRcXFwiIHN0eWxlPVxcXCJtYXJnaW4tcmlnaHQ6IDEwcHg7XFxcIiB0eXBlPVxcXCJzdWJtaXRcXFwiIG9uY2xpY2s9XFxcIndpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKCcvQmF0Y2gvVXBsb2FkRmlsZS9EeW5hbWljQXBwZW5kJylcXFwiIHZhbHVlPVxcXCJOZXcgSm9iXFxcIi8+XCIgfV0sXHJcbiAgICAgICAgICAgICAgICAgICAgZGV0YWlsVGVtcGxhdGU6ICc8ZGl2IGNsYXNzPVwiZGV0YWlsc1wiIHN0eWxlPVwibWFyZ2luOiA1cHggNXB4IDEwcHggNXB4O1wiPjwvZGl2PicsXHJcbiAgICAgICAgICAgICAgICAgICAgZGV0YWlsSW5pdDogZGV0YWlsSW5pdCxcclxuICAgICAgICAgICAgICAgICAgICBzY3JvbGxhYmxlOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICBkYXRhQm91bmQ6IGZ1bmN0aW9uICgpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdmFyIGdyaWQgPSAkKCcjZ3JpZENvbXBsZXRlJykuZGF0YSgna2VuZG9HcmlkJyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICh0aGlzLmRhdGFTb3VyY2UudG90YWwoKSA9PSAxKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBncmlkLmV4cGFuZFJvdygkKCcjZ3JpZENvbXBsZXRlIHRib2R5PnRyOmZpcnN0JykpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICB2YXIgc3RhdGU6IGFueSA9IHNlc3Npb25TdG9yYWdlLmdldEl0ZW0oXCJncmlkXCIpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoc3RhdGUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN0YXRlID0gSlNPTi5wYXJzZShzdGF0ZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBmb3IgKHZhciBpZCBpbiBzdGF0ZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBkYXRhSXRlbSA9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGdyaWQuZGF0YVNvdXJjZVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLmdldChpZCk7IC8vIFwiZ2V0XCIgbWV0aG9kIHJlcXVpcmVzIG1vZGVsIHRvIGJlIHNldCBpbiBkYXRhU291cmNlIHdpdGggZmllbGQgbmFtZSBcImlkXCJcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoZGF0YUl0ZW0gIT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZ3JpZC5leHBhbmRSb3coXCJ0cltkYXRhLXVpZD1cIiArIGRhdGFJdGVtLnVpZCArIFwiXVwiKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgZGV0YWlsRXhwYW5kOiBmdW5jdGlvbiAoZSkge1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKGV4cGFuZGVkUm93ICE9IG51bGwgJiYgZXhwYW5kZWRSb3dbMF0gIT0gZS5tYXN0ZXJSb3dbMF0pIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhciBncmlkID0gJCgnI2dyaWRDb21wbGV0ZScpLmRhdGEoJ2tlbmRvR3JpZCcpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgZ3JpZC5jb2xsYXBzZVJvdyhleHBhbmRlZFJvdyk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZXhwYW5kZWRSb3cgPSBlLm1hc3RlclJvdztcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHZhciBzdGF0ZTogYW55ID0gc2Vzc2lvblN0b3JhZ2UuZ2V0SXRlbShcImdyaWRcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmICghc3RhdGUpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN0YXRlID0ge307XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdGF0ZSA9IEpTT04ucGFyc2Uoc3RhdGUpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHN0YXRlW3RoaXMuZGF0YUl0ZW0oZS5tYXN0ZXJSb3cpLmlkXSA9IHRydWU7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHNlc3Npb25TdG9yYWdlLnNldEl0ZW0oXCJncmlkXCIsIEpTT04uc3RyaW5naWZ5KHN0YXRlKSk7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgZGV0YWlsQ29sbGFwc2U6IGZ1bmN0aW9uIChlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC8vIHNhdmUgZ3JpZCBzdGF0ZSBzbyBleHBhbmRlZCByb3dzIGRvbid0IGNvbGxhcHNlIHdoZW4gdGhlIGRhdGFTb3VyY2UgaXMgcmVmcmVzaGVkXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHZhciBzdGF0ZTogYW55ID0gc2Vzc2lvblN0b3JhZ2UuZ2V0SXRlbShcImdyaWRcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChzdGF0ZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgc3RhdGUgPSBKU09OLnBhcnNlKHN0YXRlKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRlbGV0ZSBzdGF0ZVt0aGlzLmRhdGFJdGVtKGUubWFzdGVyUm93KS5pZF07XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBzZXNzaW9uU3RvcmFnZS5zZXRJdGVtKFwiZ3JpZFwiLCBKU09OLnN0cmluZ2lmeShzdGF0ZSkpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iXX0=