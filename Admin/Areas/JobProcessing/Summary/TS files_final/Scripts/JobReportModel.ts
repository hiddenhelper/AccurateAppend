/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

/// <reference path="OperationReportModel.ts" />

module AccurateAppend.JobProcessing.Summary {

    export class JobReportModel
        extends AccurateAppend.JobProcessing.Summary.ParentModel {
      constructor(Email, JobId, links) {
          super(Email, JobId, links)
      }


        display(jobid: any, userid: any) {
        debugger
    $.ajax({
        url: "/JobProcessing/Reports/GetAvailableOperationsForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
        // 2020-06-10, chris => Added this link to the Links collection in Job Detail. The link should be passed in through the constructor. You can probably replace the jobid parameter with a parameter for the url
        data: { jobid: jobid },
        dataType: 'json',
        type: 'GET',
        success(result) {
            debugger
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
                        read: function (options) {
                            $.ajax({
                                url: "/JobProcessing/Reports/GetCassReportForJob",
                                dataType: 'json',
                                data: { jobid: jobid },
                                type: 'GET',
                                success(result) {
                                    debugger
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
                dataBound: function () { }
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
                function (i, operationName) {
                    jobProcessingSummaryOperationReportModel = new AccurateAppend.JobProcessing.Summary.OperationReportModel(jobid, operationName); jobProcessingSummaryOperationReportModel.display();
                });
        }
    });

    $('#job-report-modal').modal('show');
    }
  }

}