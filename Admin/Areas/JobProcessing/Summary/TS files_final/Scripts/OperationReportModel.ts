/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

module AccurateAppend.JobProcessing.Summary {
    export class OperationReportModel {
        jobid: any;
        productKey: any;

        constructor(jobid: any, operationName: any) {
            debugger
            this.jobid = jobid;
            this.productKey = operationName;
        }

        display () {
            var div = $('<div/>');
            var panel = $('<div class="panel panel-default">' +
                '<div class="panel-heading">' +
                '<h3 class="panel-title">Operation: ' +
                this.productKey +
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
                        read: function (options) {
                            $.ajax({
                                url: "/JobProcessing/Reports/GetMatchLevelReportForJob", // TODO: NEEDS TO COME FROM JOB DETAIL
                                // 2020-06-10, chris => Added this link to the Links collection in Job Detail. The link should be passed in through the constructor. You can probably replace the jobid parameter with a parameter for the url
                                dataType: 'json',
                                data: { jobid: this.jobid, operationName: this.productKey },
                                type: 'GET',
                                success (result) {
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
                dataBound: function () { }
            });

            var qualityLevelChart = $('<div/>');
            qualityLevelChart.kendoChart({
                title: {
                    text: "Quality Level"
                },
                dataSource: {
                    transport: {
                        read: function (options) {
                            $.ajax({
                                url: "/JobProcessing/Reports/GetQualityLevelReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                                  // 2020-06-10, chris => Added this link to the Links collection in Job Detail. The link should be passed in through the constructor. You can probably replace the jobid parameter with a parameter for the url
                                dataType: 'json',
                                data: { jobid: this.jobid, operationName: this.productKey },
                                type: 'GET',
                                success (result) {
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
                dataBound: function () { }
            });
            var maxValidationLevelChart = $('<div/>');
            maxValidationLevelChart.kendoChart({
                title: {
                    text: "Maximum Validation Level"
                },
                dataSource: {
                    transport: {
                        read: function (options) {
                            $.ajax({
                                url:
                                    "/JobProcessing/Reports/GetMaxValidationLevelReportForJob", // TODO: NEEDS TO COME FROM JOB SUMMARY
                                  // 2020-06-10, chris => Added this link to the Links collection in Job Detail. The link should be passed in through the constructor. You can probably replace the jobid parameter with a parameter for the url
                                dataType: 'json',
                                data: { jobid: this.jobid, operationName: this.productKey },
                                type: 'GET',
                                success (result) {
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
                    labels: {
                        step: 10
                    }
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

            $('#operationReports').append(
                div.append(panel.append(panelBody
                    .append($('<div = class="col-md-4"/>').append(maxValidationLevelChart))
                    .append($('<div = class="col-md-4"/>').append(matchLevelchart))
                    .append($('<div = class="col-md-4"/>').append(qualityLevelChart))
                )
                )
            );
        }
    }
}