var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var OperationReportModel = (function () {
                function OperationReportModel(jobid, operationName) {
                    debugger;
                    this.jobid = jobid;
                    this.productKey = operationName;
                }
                OperationReportModel.prototype.display = function () {
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
                                        url: "/JobProcessing/Reports/GetMatchLevelReportForJob",
                                        dataType: 'json',
                                        data: { jobid: this.jobid, operationName: this.productKey },
                                        type: 'GET',
                                        success: function (result) {
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
                                        url: "/JobProcessing/Reports/GetQualityLevelReportForJob",
                                        dataType: 'json',
                                        data: { jobid: this.jobid, operationName: this.productKey },
                                        type: 'GET',
                                        success: function (result) {
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
                                        url: "/JobProcessing/Reports/GetMaxValidationLevelReportForJob",
                                        dataType: 'json',
                                        data: { jobid: this.jobid, operationName: this.productKey },
                                        type: 'GET',
                                        success: function (result) {
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
                    $('#operationReports').append(div.append(panel.append(panelBody
                        .append($('<div = class="col-md-4"/>').append(maxValidationLevelChart))
                        .append($('<div = class="col-md-4"/>').append(matchLevelchart))
                        .append($('<div = class="col-md-4"/>').append(qualityLevelChart)))));
                };
                return OperationReportModel;
            }());
            Summary.OperationReportModel = OperationReportModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT3BlcmF0aW9uUmVwb3J0TW9kZWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJPcGVyYXRpb25SZXBvcnRNb2RlbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFHQSxJQUFPLGNBQWMsQ0ErTnBCO0FBL05ELFdBQU8sY0FBYztJQUFDLElBQUEsYUFBYSxDQStObEM7SUEvTnFCLFdBQUEsYUFBYTtRQUFDLElBQUEsT0FBTyxDQStOMUM7UUEvTm1DLFdBQUEsT0FBTztZQUN2QztnQkFJSSw4QkFBWSxLQUFVLEVBQUUsYUFBa0I7b0JBQ3RDLFFBQVEsQ0FBQTtvQkFDUixJQUFJLENBQUMsS0FBSyxHQUFHLEtBQUssQ0FBQztvQkFDbkIsSUFBSSxDQUFDLFVBQVUsR0FBRyxhQUFhLENBQUM7Z0JBQ3BDLENBQUM7Z0JBRUQsc0NBQU8sR0FBUDtvQkFDSSxJQUFJLEdBQUcsR0FBRyxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUM7b0JBQ3RCLElBQUksS0FBSyxHQUFHLENBQUMsQ0FBQyxtQ0FBbUM7d0JBQzdDLDZCQUE2Qjt3QkFDN0IscUNBQXFDO3dCQUNyQyxJQUFJLENBQUMsVUFBVTt3QkFDZixPQUFPO3dCQUNQLFFBQVE7d0JBQ1IsUUFBUSxDQUFDLENBQUM7b0JBQ2QsSUFBSSxTQUFTLEdBQUcsQ0FBQyxDQUFDLGdDQUFnQyxDQUFDLENBQUM7b0JBQ3BELElBQUksZUFBZSxHQUFHLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQztvQkFDbEMsZUFBZSxDQUFDLFVBQVUsQ0FBQzt3QkFDdkIsS0FBSyxFQUFFOzRCQUNILElBQUksRUFBRSxhQUFhO3lCQUN0Qjt3QkFDRCxVQUFVLEVBQUU7NEJBQ1IsU0FBUyxFQUFFO2dDQUNQLElBQUksRUFBRSxVQUFVLE9BQU87b0NBQ25CLENBQUMsQ0FBQyxJQUFJLENBQUM7d0NBQ0gsR0FBRyxFQUFFLGtEQUFrRDt3Q0FFdkQsUUFBUSxFQUFFLE1BQU07d0NBQ2hCLElBQUksRUFBRSxFQUFFLEtBQUssRUFBRSxJQUFJLENBQUMsS0FBSyxFQUFFLGFBQWEsRUFBRSxJQUFJLENBQUMsVUFBVSxFQUFFO3dDQUMzRCxJQUFJLEVBQUUsS0FBSzt3Q0FDWCxPQUFPLFlBQUUsTUFBTTs0Q0FDWCxPQUFPLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxDQUFDO3dDQUM1QixDQUFDO3FDQUNKLENBQUMsQ0FBQztnQ0FDUCxDQUFDOzZCQUNKOzRCQUNELE1BQU0sRUFBRSxFQUFFLElBQUksRUFBRSxNQUFNLEVBQUU7eUJBQzNCO3dCQUNELE1BQU0sRUFBRTs0QkFDSjtnQ0FDSSxJQUFJLEVBQUUsV0FBVztnQ0FDakIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLE1BQU07Z0NBQ2IsS0FBSyxFQUFFLEVBQUU7NkJBQ1osRUFBRTtnQ0FDQyxJQUFJLEVBQUUsV0FBVztnQ0FDakIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLE1BQU07Z0NBQ2IsS0FBSyxFQUFFLEVBQUU7NkJBQ1osRUFBRTtnQ0FDQyxJQUFJLEVBQUUsZUFBZTtnQ0FDckIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLFFBQVE7Z0NBQ2YsS0FBSyxFQUFFLEVBQUU7NkJBQ1o7eUJBQ0o7d0JBQ0QsU0FBUyxFQUFFOzRCQUNQLE1BQU0sRUFBRTtnQ0FDSixNQUFNLEVBQUUsUUFBUTs2QkFDbkI7NEJBQ0QsT0FBTyxFQUFFLElBQUk7NEJBQ2IsR0FBRyxFQUFFLENBQUM7NEJBQ04sR0FBRyxFQUFFLENBQUM7eUJBQ1Q7d0JBQ0QsWUFBWSxFQUFFOzRCQUNWLEtBQUssRUFBRSxZQUFZOzRCQUNuQixPQUFPLEVBQUUsSUFBSTt5QkFDaEI7d0JBQ0QsT0FBTyxFQUFFOzRCQUNMLE9BQU8sRUFBRSxJQUFJOzRCQUNiLE1BQU0sRUFBRSxRQUFRO3lCQUNuQjt3QkFDRCxNQUFNLEVBQUU7NEJBQ0osT0FBTyxFQUFFLElBQUk7NEJBQ2IsUUFBUSxFQUFFLFFBQVE7eUJBQ3JCO3dCQUNELFNBQVMsRUFBRSxjQUFjLENBQUM7cUJBQzdCLENBQUMsQ0FBQztvQkFFSCxJQUFJLGlCQUFpQixHQUFHLENBQUMsQ0FBQyxRQUFRLENBQUMsQ0FBQztvQkFDcEMsaUJBQWlCLENBQUMsVUFBVSxDQUFDO3dCQUN6QixLQUFLLEVBQUU7NEJBQ0gsSUFBSSxFQUFFLGVBQWU7eUJBQ3hCO3dCQUNELFVBQVUsRUFBRTs0QkFDUixTQUFTLEVBQUU7Z0NBQ1AsSUFBSSxFQUFFLFVBQVUsT0FBTztvQ0FDbkIsQ0FBQyxDQUFDLElBQUksQ0FBQzt3Q0FDSCxHQUFHLEVBQUUsb0RBQW9EO3dDQUV6RCxRQUFRLEVBQUUsTUFBTTt3Q0FDaEIsSUFBSSxFQUFFLEVBQUUsS0FBSyxFQUFFLElBQUksQ0FBQyxLQUFLLEVBQUUsYUFBYSxFQUFFLElBQUksQ0FBQyxVQUFVLEVBQUU7d0NBQzNELElBQUksRUFBRSxLQUFLO3dDQUNYLE9BQU8sWUFBRSxNQUFNOzRDQUNYLE9BQU8sQ0FBQyxPQUFPLENBQUMsTUFBTSxDQUFDLENBQUM7d0NBQzVCLENBQUM7cUNBQ0osQ0FBQyxDQUFDO2dDQUNQLENBQUM7NkJBQ0o7NEJBQ0QsTUFBTSxFQUFFLEVBQUUsSUFBSSxFQUFFLE1BQU0sRUFBRTt5QkFDM0I7d0JBQ0QsTUFBTSxFQUFFOzRCQUNKO2dDQUNJLElBQUksRUFBRSxXQUFXO2dDQUNqQixJQUFJLEVBQUUsUUFBUTtnQ0FDZCxLQUFLLEVBQUUsTUFBTTtnQ0FDYixLQUFLLEVBQUUsRUFBRTs2QkFDWixFQUFFO2dDQUNDLElBQUksRUFBRSxXQUFXO2dDQUNqQixJQUFJLEVBQUUsUUFBUTtnQ0FDZCxLQUFLLEVBQUUsTUFBTTtnQ0FDYixLQUFLLEVBQUUsRUFBRTs2QkFDWixFQUFFO2dDQUNDLElBQUksRUFBRSxlQUFlO2dDQUNyQixJQUFJLEVBQUUsUUFBUTtnQ0FDZCxLQUFLLEVBQUUsUUFBUTtnQ0FDZixLQUFLLEVBQUUsRUFBRTs2QkFDWjt5QkFDSjt3QkFDRCxTQUFTLEVBQUU7NEJBQ1AsTUFBTSxFQUFFO2dDQUNKLE1BQU0sRUFBRSxRQUFROzZCQUNuQjs0QkFDRCxPQUFPLEVBQUUsSUFBSTs0QkFDYixHQUFHLEVBQUUsQ0FBQzs0QkFDTixHQUFHLEVBQUUsQ0FBQzt5QkFDVDt3QkFDRCxZQUFZLEVBQUU7NEJBQ1YsS0FBSyxFQUFFLGNBQWM7NEJBQ3JCLE9BQU8sRUFBRSxJQUFJO3lCQUNoQjt3QkFDRCxPQUFPLEVBQUU7NEJBQ0wsT0FBTyxFQUFFLElBQUk7NEJBQ2IsTUFBTSxFQUFFLFFBQVE7eUJBQ25CO3dCQUNELE1BQU0sRUFBRTs0QkFDSixPQUFPLEVBQUUsSUFBSTs0QkFDYixRQUFRLEVBQUUsUUFBUTt5QkFDckI7d0JBQ0QsU0FBUyxFQUFFLGNBQWMsQ0FBQztxQkFDN0IsQ0FBQyxDQUFDO29CQUNILElBQUksdUJBQXVCLEdBQUcsQ0FBQyxDQUFDLFFBQVEsQ0FBQyxDQUFDO29CQUMxQyx1QkFBdUIsQ0FBQyxVQUFVLENBQUM7d0JBQy9CLEtBQUssRUFBRTs0QkFDSCxJQUFJLEVBQUUsMEJBQTBCO3lCQUNuQzt3QkFDRCxVQUFVLEVBQUU7NEJBQ1IsU0FBUyxFQUFFO2dDQUNQLElBQUksRUFBRSxVQUFVLE9BQU87b0NBQ25CLENBQUMsQ0FBQyxJQUFJLENBQUM7d0NBQ0gsR0FBRyxFQUNDLDBEQUEwRDt3Q0FFOUQsUUFBUSxFQUFFLE1BQU07d0NBQ2hCLElBQUksRUFBRSxFQUFFLEtBQUssRUFBRSxJQUFJLENBQUMsS0FBSyxFQUFFLGFBQWEsRUFBRSxJQUFJLENBQUMsVUFBVSxFQUFFO3dDQUMzRCxJQUFJLEVBQUUsS0FBSzt3Q0FDWCxPQUFPLFlBQUUsTUFBTTs0Q0FDWCxPQUFPLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxDQUFDO3dDQUM1QixDQUFDO3FDQUNKLENBQUMsQ0FBQztnQ0FDUCxDQUFDOzZCQUNKOzRCQUNELE1BQU0sRUFBRSxFQUFFLElBQUksRUFBRSxNQUFNLEVBQUU7eUJBQzNCO3dCQUNELE1BQU0sRUFBRTs0QkFDSjtnQ0FDSSxJQUFJLEVBQUUsV0FBVztnQ0FDakIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLE1BQU07Z0NBQ2IsS0FBSyxFQUFFLEVBQUU7NkJBQ1osRUFBRTtnQ0FDQyxJQUFJLEVBQUUsV0FBVztnQ0FDakIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLE1BQU07Z0NBQ2IsS0FBSyxFQUFFLEVBQUU7NkJBQ1osRUFBRTtnQ0FDQyxJQUFJLEVBQUUsZUFBZTtnQ0FDckIsSUFBSSxFQUFFLFFBQVE7Z0NBQ2QsS0FBSyxFQUFFLFFBQVE7Z0NBQ2YsS0FBSyxFQUFFLEVBQUU7NkJBQ1o7eUJBQ0o7d0JBQ0QsU0FBUyxFQUFFOzRCQUNQLE1BQU0sRUFBRTtnQ0FDSixNQUFNLEVBQUUsUUFBUTs2QkFDbkI7NEJBQ0QsT0FBTyxFQUFFLElBQUk7NEJBQ2IsR0FBRyxFQUFFLENBQUM7NEJBQ04sR0FBRyxFQUFFLENBQUM7eUJBQ1Q7d0JBQ0QsWUFBWSxFQUFFOzRCQUNWLEtBQUssRUFBRSxZQUFZOzRCQUNuQixPQUFPLEVBQUUsSUFBSTs0QkFDYixNQUFNLEVBQUU7Z0NBQ0osSUFBSSxFQUFFLEVBQUU7NkJBQ1g7eUJBQ0o7d0JBQ0QsT0FBTyxFQUFFOzRCQUNMLE9BQU8sRUFBRSxJQUFJOzRCQUNiLE1BQU0sRUFBRSxRQUFRO3lCQUNuQjt3QkFDRCxNQUFNLEVBQUU7NEJBQ0osT0FBTyxFQUFFLElBQUk7NEJBQ2IsUUFBUSxFQUFFLFFBQVE7eUJBQ3JCO3dCQUNELFNBQVMsRUFBRSxjQUFjLENBQUM7cUJBQzdCLENBQUMsQ0FBQztvQkFFSCxDQUFDLENBQUMsbUJBQW1CLENBQUMsQ0FBQyxNQUFNLENBQ3pCLEdBQUcsQ0FBQyxNQUFNLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxTQUFTO3lCQUM1QixNQUFNLENBQUMsQ0FBQyxDQUFDLDJCQUEyQixDQUFDLENBQUMsTUFBTSxDQUFDLHVCQUF1QixDQUFDLENBQUM7eUJBQ3RFLE1BQU0sQ0FBQyxDQUFDLENBQUMsMkJBQTJCLENBQUMsQ0FBQyxNQUFNLENBQUMsZUFBZSxDQUFDLENBQUM7eUJBQzlELE1BQU0sQ0FBQyxDQUFDLENBQUMsMkJBQTJCLENBQUMsQ0FBQyxNQUFNLENBQUMsaUJBQWlCLENBQUMsQ0FBQyxDQUNwRSxDQUNBLENBQ0osQ0FBQztnQkFDTixDQUFDO2dCQUNMLDJCQUFDO1lBQUQsQ0FBQyxBQTdORCxJQTZOQztZQTdOWSw0QkFBb0IsdUJBNk5oQyxDQUFBO1FBQ0wsQ0FBQyxFQS9ObUMsT0FBTyxHQUFQLHFCQUFPLEtBQVAscUJBQU8sUUErTjFDO0lBQUQsQ0FBQyxFQS9OcUIsYUFBYSxHQUFiLDRCQUFhLEtBQWIsNEJBQWEsUUErTmxDO0FBQUQsQ0FBQyxFQS9OTSxjQUFjLEtBQWQsY0FBYyxRQStOcEIiLCJzb3VyY2VzQ29udGVudCI6WyIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL21vbWVudC9tb21lbnQuZC50c1wiIC8+XHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi8uLi8uLi9zY3JpcHRzL3R5cGluZ3Mva2VuZG8tdWkva2VuZG8tdWkuZC50c1wiIC8+XHJcblxyXG5tb2R1bGUgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5IHtcclxuICAgIGV4cG9ydCBjbGFzcyBPcGVyYXRpb25SZXBvcnRNb2RlbCB7XHJcbiAgICAgICAgam9iaWQ6IGFueTtcclxuICAgICAgICBwcm9kdWN0S2V5OiBhbnk7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKGpvYmlkOiBhbnksIG9wZXJhdGlvbk5hbWU6IGFueSkge1xyXG4gICAgICAgICAgICBkZWJ1Z2dlclxyXG4gICAgICAgICAgICB0aGlzLmpvYmlkID0gam9iaWQ7XHJcbiAgICAgICAgICAgIHRoaXMucHJvZHVjdEtleSA9IG9wZXJhdGlvbk5hbWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBkaXNwbGF5ICgpIHtcclxuICAgICAgICAgICAgdmFyIGRpdiA9ICQoJzxkaXYvPicpO1xyXG4gICAgICAgICAgICB2YXIgcGFuZWwgPSAkKCc8ZGl2IGNsYXNzPVwicGFuZWwgcGFuZWwtZGVmYXVsdFwiPicgK1xyXG4gICAgICAgICAgICAgICAgJzxkaXYgY2xhc3M9XCJwYW5lbC1oZWFkaW5nXCI+JyArXHJcbiAgICAgICAgICAgICAgICAnPGgzIGNsYXNzPVwicGFuZWwtdGl0bGVcIj5PcGVyYXRpb246ICcgK1xyXG4gICAgICAgICAgICAgICAgdGhpcy5wcm9kdWN0S2V5ICtcclxuICAgICAgICAgICAgICAgICc8L2gzPicgK1xyXG4gICAgICAgICAgICAgICAgJzwvZGl2PicgK1xyXG4gICAgICAgICAgICAgICAgJzwvZGl2PicpO1xyXG4gICAgICAgICAgICB2YXIgcGFuZWxCb2R5ID0gJCgnPGRpdiBjbGFzcz1cInBhbmVsLWJvZHlcIj48L2Rpdj4nKTtcclxuICAgICAgICAgICAgdmFyIG1hdGNoTGV2ZWxjaGFydCA9ICQoJzxkaXYvPicpO1xyXG4gICAgICAgICAgICBtYXRjaExldmVsY2hhcnQua2VuZG9DaGFydCh7XHJcbiAgICAgICAgICAgICAgICB0aXRsZToge1xyXG4gICAgICAgICAgICAgICAgICAgIHRleHQ6IFwiTWF0Y2ggTGV2ZWxcIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVhZDogZnVuY3Rpb24gKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBcIi9Kb2JQcm9jZXNzaW5nL1JlcG9ydHMvR2V0TWF0Y2hMZXZlbFJlcG9ydEZvckpvYlwiLCAvLyBUT0RPOiBORUVEUyBUTyBDT01FIEZST00gSk9CIERFVEFJTFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIC8vIDIwMjAtMDYtMTAsIGNocmlzID0+IEFkZGVkIHRoaXMgbGluayB0byB0aGUgTGlua3MgY29sbGVjdGlvbiBpbiBKb2IgRGV0YWlsLiBUaGUgbGluayBzaG91bGQgYmUgcGFzc2VkIGluIHRocm91Z2ggdGhlIGNvbnN0cnVjdG9yLiBZb3UgY2FuIHByb2JhYmx5IHJlcGxhY2UgdGhlIGpvYmlkIHBhcmFtZXRlciB3aXRoIGEgcGFyYW1ldGVyIGZvciB0aGUgdXJsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YVR5cGU6ICdqc29uJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhOiB7IGpvYmlkOiB0aGlzLmpvYmlkLCBvcGVyYXRpb25OYW1lOiB0aGlzLnByb2R1Y3RLZXkgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnR0VUJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzIChyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgb3B0aW9ucy5zdWNjZXNzKHJlc3VsdCk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHNjaGVtYTogeyBkYXRhOiBcIkRhdGFcIiB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2VyaWVzOiBbXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiBcIlRoaXMgRmlsZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImNvbHVtblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJGaWxlXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiBcIlwiXHJcbiAgICAgICAgICAgICAgICAgICAgfSwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiBcIlRoaXMgVXNlclwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImNvbHVtblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJVc2VyXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiBcIlwiXHJcbiAgICAgICAgICAgICAgICAgICAgfSwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiBcIkVudGlyZSBTeXN0ZW1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJjb2x1bW5cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiU3lzdGVtXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiBcIlwiXHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgXSxcclxuICAgICAgICAgICAgICAgIHZhbHVlQXhpczoge1xyXG4gICAgICAgICAgICAgICAgICAgIGxhYmVsczoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBmb3JtYXQ6IFwiezA6cDB9XCJcclxuICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgbWluOiAwLFxyXG4gICAgICAgICAgICAgICAgICAgIG1heDogMVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGNhdGVnb3J5QXhpczoge1xyXG4gICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIk1hdGNoTGV2ZWxcIixcclxuICAgICAgICAgICAgICAgICAgICB2aXNpYmxlOiB0cnVlXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnAwfVwiXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgbGVnZW5kOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBwb3NpdGlvbjogXCJib3R0b21cIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFCb3VuZDogZnVuY3Rpb24gKCkgeyB9XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgdmFyIHF1YWxpdHlMZXZlbENoYXJ0ID0gJCgnPGRpdi8+Jyk7XHJcbiAgICAgICAgICAgIHF1YWxpdHlMZXZlbENoYXJ0LmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgICAgICAgICAgdGl0bGU6IHtcclxuICAgICAgICAgICAgICAgICAgICB0ZXh0OiBcIlF1YWxpdHkgTGV2ZWxcIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVhZDogZnVuY3Rpb24gKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBcIi9Kb2JQcm9jZXNzaW5nL1JlcG9ydHMvR2V0UXVhbGl0eUxldmVsUmVwb3J0Rm9ySm9iXCIsIC8vIFRPRE86IE5FRURTIFRPIENPTUUgRlJPTSBKT0IgU1VNTUFSWVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgLy8gMjAyMC0wNi0xMCwgY2hyaXMgPT4gQWRkZWQgdGhpcyBsaW5rIHRvIHRoZSBMaW5rcyBjb2xsZWN0aW9uIGluIEpvYiBEZXRhaWwuIFRoZSBsaW5rIHNob3VsZCBiZSBwYXNzZWQgaW4gdGhyb3VnaCB0aGUgY29uc3RydWN0b3IuIFlvdSBjYW4gcHJvYmFibHkgcmVwbGFjZSB0aGUgam9iaWQgcGFyYW1ldGVyIHdpdGggYSBwYXJhbWV0ZXIgZm9yIHRoZSB1cmxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhVHlwZTogJ2pzb24nLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGE6IHsgam9iaWQ6IHRoaXMuam9iaWQsIG9wZXJhdGlvbk5hbWU6IHRoaXMucHJvZHVjdEtleSB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHN1Y2Nlc3MgKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgc2NoZW1hOiB7IGRhdGE6IFwiRGF0YVwiIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzZXJpZXM6IFtcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IFwiVGhpcyBGaWxlXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkZpbGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IFwiVGhpcyBVc2VyXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlVzZXJcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IFwiRW50aXJlIFN5c3RlbVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImNvbHVtblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJTeXN0ZW1cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICAgICAgdmFsdWVBeGlzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgbGFiZWxzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpwMH1cIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBtaW46IDAsXHJcbiAgICAgICAgICAgICAgICAgICAgbWF4OiAxXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY2F0ZWdvcnlBeGlzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiUXVhbGl0eUxldmVsXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHRvb2x0aXA6IHtcclxuICAgICAgICAgICAgICAgICAgICB2aXNpYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpwMH1cIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGxlZ2VuZDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgcG9zaXRpb246IFwiYm90dG9tXCJcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBkYXRhQm91bmQ6IGZ1bmN0aW9uICgpIHsgfVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgdmFyIG1heFZhbGlkYXRpb25MZXZlbENoYXJ0ID0gJCgnPGRpdi8+Jyk7XHJcbiAgICAgICAgICAgIG1heFZhbGlkYXRpb25MZXZlbENoYXJ0LmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgICAgICAgICAgdGl0bGU6IHtcclxuICAgICAgICAgICAgICAgICAgICB0ZXh0OiBcIk1heGltdW0gVmFsaWRhdGlvbiBMZXZlbFwiXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgZGF0YVNvdXJjZToge1xyXG4gICAgICAgICAgICAgICAgICAgIHRyYW5zcG9ydDoge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICByZWFkOiBmdW5jdGlvbiAob3B0aW9ucykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgJC5hamF4KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB1cmw6XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIFwiL0pvYlByb2Nlc3NpbmcvUmVwb3J0cy9HZXRNYXhWYWxpZGF0aW9uTGV2ZWxSZXBvcnRGb3JKb2JcIiwgLy8gVE9ETzogTkVFRFMgVE8gQ09NRSBGUk9NIEpPQiBTVU1NQVJZXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAvLyAyMDIwLTA2LTEwLCBjaHJpcyA9PiBBZGRlZCB0aGlzIGxpbmsgdG8gdGhlIExpbmtzIGNvbGxlY3Rpb24gaW4gSm9iIERldGFpbC4gVGhlIGxpbmsgc2hvdWxkIGJlIHBhc3NlZCBpbiB0aHJvdWdoIHRoZSBjb25zdHJ1Y3Rvci4gWW91IGNhbiBwcm9iYWJseSByZXBsYWNlIHRoZSBqb2JpZCBwYXJhbWV0ZXIgd2l0aCBhIHBhcmFtZXRlciBmb3IgdGhlIHVybFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogeyBqb2JpZDogdGhpcy5qb2JpZCwgb3BlcmF0aW9uTmFtZTogdGhpcy5wcm9kdWN0S2V5IH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgc3VjY2VzcyAocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9wdGlvbnMuc3VjY2VzcyhyZXN1bHQpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICBzY2hlbWE6IHsgZGF0YTogXCJEYXRhXCIgfVxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIHNlcmllczogW1xyXG4gICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogXCJUaGlzIEZpbGVcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJjb2x1bW5cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRmlsZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogXCJcIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogXCJUaGlzIFVzZXJcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJjb2x1bW5cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVXNlclwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogXCJcIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogXCJFbnRpcmUgU3lzdGVtXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlN5c3RlbVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogXCJcIlxyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgICAgICB2YWx1ZUF4aXM6IHtcclxuICAgICAgICAgICAgICAgICAgICBsYWJlbHM6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnAwfVwiXHJcbiAgICAgICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgICAgICB2aXNpYmxlOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIG1pbjogMCxcclxuICAgICAgICAgICAgICAgICAgICBtYXg6IDFcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBjYXRlZ29yeUF4aXM6IHtcclxuICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJNYXRjaExldmVsXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBsYWJlbHM6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgc3RlcDogMTBcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnAwfVwiXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgbGVnZW5kOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBwb3NpdGlvbjogXCJib3R0b21cIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFCb3VuZDogZnVuY3Rpb24gKCkgeyB9XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgJCgnI29wZXJhdGlvblJlcG9ydHMnKS5hcHBlbmQoXHJcbiAgICAgICAgICAgICAgICBkaXYuYXBwZW5kKHBhbmVsLmFwcGVuZChwYW5lbEJvZHlcclxuICAgICAgICAgICAgICAgICAgICAuYXBwZW5kKCQoJzxkaXYgPSBjbGFzcz1cImNvbC1tZC00XCIvPicpLmFwcGVuZChtYXhWYWxpZGF0aW9uTGV2ZWxDaGFydCkpXHJcbiAgICAgICAgICAgICAgICAgICAgLmFwcGVuZCgkKCc8ZGl2ID0gY2xhc3M9XCJjb2wtbWQtNFwiLz4nKS5hcHBlbmQobWF0Y2hMZXZlbGNoYXJ0KSlcclxuICAgICAgICAgICAgICAgICAgICAuYXBwZW5kKCQoJzxkaXYgPSBjbGFzcz1cImNvbC1tZC00XCIvPicpLmFwcGVuZChxdWFsaXR5TGV2ZWxDaGFydCkpXHJcbiAgICAgICAgICAgICAgICApXHJcbiAgICAgICAgICAgICAgICApXHJcbiAgICAgICAgICAgICk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG59Il19