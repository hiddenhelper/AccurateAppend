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
var AccurateAppend;
(function (AccurateAppend) {
    var JobProcessing;
    (function (JobProcessing) {
        var Summary;
        (function (Summary) {
            var JobReportModel = (function (_super) {
                __extends(JobReportModel, _super);
                function JobReportModel(Email, JobId, links) {
                    return _super.call(this, Email, JobId, links) || this;
                }
                JobReportModel.prototype.display = function (jobid, userid) {
                    debugger;
                    $.ajax({
                        url: "/JobProcessing/Reports/GetAvailableOperationsForJob",
                        data: { jobid: jobid },
                        dataType: 'json',
                        type: 'GET',
                        success: function (result) {
                            debugger;
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
                                                success: function (result) {
                                                    debugger;
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
                            $('#parsingReport').append(div.append(panel.append(panelBody
                                .append($('<div class="col-md-6"/>').append(cassChart))
                                .append($('<div class="col-md-6"/>').append("</div>")))));
                            $('#operationReports div').remove();
                            $.each(result.Data, function (i, operationName) {
                                jobProcessingSummaryOperationReportModel = new AccurateAppend.JobProcessing.Summary.OperationReportModel(jobid, operationName);
                                jobProcessingSummaryOperationReportModel.display();
                            });
                        }
                    });
                    $('#job-report-modal').modal('show');
                };
                return JobReportModel;
            }(AccurateAppend.JobProcessing.Summary.ParentModel));
            Summary.JobReportModel = JobReportModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSm9iUmVwb3J0TW9kZWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJKb2JSZXBvcnRNb2RlbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7O0FBS0EsSUFBTyxjQUFjLENBOEhwQjtBQTlIRCxXQUFPLGNBQWM7SUFBQyxJQUFBLGFBQWEsQ0E4SGxDO0lBOUhxQixXQUFBLGFBQWE7UUFBQyxJQUFBLE9BQU8sQ0E4SDFDO1FBOUhtQyxXQUFBLE9BQU87WUFFdkM7Z0JBQ1ksa0NBQWdEO2dCQUMxRCx3QkFBWSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUs7MkJBQzNCLGtCQUFNLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxDQUFDO2dCQUM5QixDQUFDO2dCQUdDLGdDQUFPLEdBQVAsVUFBUSxLQUFVLEVBQUUsTUFBVztvQkFDL0IsUUFBUSxDQUFBO29CQUNaLENBQUMsQ0FBQyxJQUFJLENBQUM7d0JBQ0gsR0FBRyxFQUFFLHFEQUFxRDt3QkFFMUQsSUFBSSxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRTt3QkFDdEIsUUFBUSxFQUFFLE1BQU07d0JBQ2hCLElBQUksRUFBRSxLQUFLO3dCQUNYLE9BQU8sWUFBQyxNQUFNOzRCQUNWLFFBQVEsQ0FBQTs0QkFDUixJQUFJLEdBQUcsR0FBRyxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUM7NEJBQ3RCLElBQUksS0FBSyxHQUFHLENBQUMsQ0FBQyxtQ0FBbUM7Z0NBQzdDLDZCQUE2QjtnQ0FDN0IscURBQXFEO2dDQUNyRCxRQUFRO2dDQUNSLFFBQVEsQ0FBQyxDQUFDOzRCQUNkLElBQUksU0FBUyxHQUFHLENBQUMsQ0FBQyxnQ0FBZ0MsQ0FBQyxDQUFDOzRCQUNwRCxJQUFJLFNBQVMsR0FBRyxDQUFDLENBQUMsUUFBUSxDQUFDLENBQUM7NEJBQzVCLFNBQVMsQ0FBQyxVQUFVLENBQUM7Z0NBQ2pCLEtBQUssRUFBRTtvQ0FDSCxJQUFJLEVBQUUsd0JBQXdCO2lDQUNqQztnQ0FDRCxV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QUFhLENBQUMsQ0FBQztnQ0FBQyx3Q0FBd0MsQ0FBQyxPQUFPLEVBQUUsQ0FBQzs0QkFDdkwsQ0FBQyxDQUFDLENBQUM7d0JBQ1gsQ0FBQztxQkFDSixDQUFDLENBQUM7b0JBRUgsQ0FBQyxDQUFDLG1CQUFtQixDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDO2dCQUNyQyxDQUFDO2dCQUNILHFCQUFDO1lBQUQsQ0FBQyxBQTFIQyxDQUNZLGNBQWMsQ0FBQyxhQUFhLENBQUMsT0FBTyxDQUFDLFdBQVcsR0F5SDdEO1lBMUhjLHNCQUFjLGlCQTBINUIsQ0FBQTtRQUVILENBQUMsRUE5SG1DLE9BQU8sR0FBUCxxQkFBTyxLQUFQLHFCQUFPLFFBOEgxQztJQUFELENBQUMsRUE5SHFCLGFBQWEsR0FBYiw0QkFBYSxLQUFiLDRCQUFhLFFBOEhsQztBQUFELENBQUMsRUE5SE0sY0FBYyxLQUFkLGNBQWMsUUE4SHBCIiwic291cmNlc0NvbnRlbnQiOlsiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9tb21lbnQvbW9tZW50LmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIk9wZXJhdGlvblJlcG9ydE1vZGVsLnRzXCIgLz5cclxuXHJcbm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkge1xyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBKb2JSZXBvcnRNb2RlbFxyXG4gICAgICAgIGV4dGVuZHMgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlBhcmVudE1vZGVsIHtcclxuICAgICAgY29uc3RydWN0b3IoRW1haWwsIEpvYklkLCBsaW5rcykge1xyXG4gICAgICAgICAgc3VwZXIoRW1haWwsIEpvYklkLCBsaW5rcylcclxuICAgICAgfVxyXG5cclxuXHJcbiAgICAgICAgZGlzcGxheShqb2JpZDogYW55LCB1c2VyaWQ6IGFueSkge1xyXG4gICAgICAgIGRlYnVnZ2VyXHJcbiAgICAkLmFqYXgoe1xyXG4gICAgICAgIHVybDogXCIvSm9iUHJvY2Vzc2luZy9SZXBvcnRzL0dldEF2YWlsYWJsZU9wZXJhdGlvbnNGb3JKb2JcIiwgLy8gVE9ETzogTkVFRFMgVE8gQ09NRSBGUk9NIEpPQiBTVU1NQVJZXHJcbiAgICAgICAgLy8gMjAyMC0wNi0xMCwgY2hyaXMgPT4gQWRkZWQgdGhpcyBsaW5rIHRvIHRoZSBMaW5rcyBjb2xsZWN0aW9uIGluIEpvYiBEZXRhaWwuIFRoZSBsaW5rIHNob3VsZCBiZSBwYXNzZWQgaW4gdGhyb3VnaCB0aGUgY29uc3RydWN0b3IuIFlvdSBjYW4gcHJvYmFibHkgcmVwbGFjZSB0aGUgam9iaWQgcGFyYW1ldGVyIHdpdGggYSBwYXJhbWV0ZXIgZm9yIHRoZSB1cmxcclxuICAgICAgICBkYXRhOiB7IGpvYmlkOiBqb2JpZCB9LFxyXG4gICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgdHlwZTogJ0dFVCcsXHJcbiAgICAgICAgc3VjY2VzcyhyZXN1bHQpIHtcclxuICAgICAgICAgICAgZGVidWdnZXJcclxuICAgICAgICAgICAgdmFyIGRpdiA9ICQoJzxkaXYvPicpO1xyXG4gICAgICAgICAgICB2YXIgcGFuZWwgPSAkKCc8ZGl2IGNsYXNzPVwicGFuZWwgcGFuZWwtZGVmYXVsdFwiPicgK1xyXG4gICAgICAgICAgICAgICAgJzxkaXYgY2xhc3M9XCJwYW5lbC1oZWFkaW5nXCI+JyArXHJcbiAgICAgICAgICAgICAgICAnPGgzIGNsYXNzPVwicGFuZWwtdGl0bGVcIj5BZGRyZXNzIFN0YW5kYXJpemF0aW9uPC9oMz4nICtcclxuICAgICAgICAgICAgICAgICc8L2Rpdj4nICtcclxuICAgICAgICAgICAgICAgICc8L2Rpdj4nKTtcclxuICAgICAgICAgICAgdmFyIHBhbmVsQm9keSA9ICQoJzxkaXYgY2xhc3M9XCJwYW5lbC1ib2R5XCI+PC9kaXY+Jyk7XHJcbiAgICAgICAgICAgIHZhciBjYXNzQ2hhcnQgPSAkKCc8ZGl2Lz4nKTtcclxuICAgICAgICAgICAgY2Fzc0NoYXJ0LmtlbmRvQ2hhcnQoe1xyXG4gICAgICAgICAgICAgICAgdGl0bGU6IHtcclxuICAgICAgICAgICAgICAgICAgICB0ZXh0OiBcIlN0YW5kYXJkaXphdGlvbiBTdGF0dXNcIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IHtcclxuICAgICAgICAgICAgICAgICAgICB0cmFuc3BvcnQ6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVhZDogZnVuY3Rpb24gKG9wdGlvbnMpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICQuYWpheCh7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdXJsOiBcIi9Kb2JQcm9jZXNzaW5nL1JlcG9ydHMvR2V0Q2Fzc1JlcG9ydEZvckpvYlwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFUeXBlOiAnanNvbicsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgZGF0YTogeyBqb2JpZDogam9iaWQgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiAnR0VUJyxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBzdWNjZXNzKHJlc3VsdCkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkZWJ1Z2dlclxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBvcHRpb25zLnN1Y2Nlc3MocmVzdWx0KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgc2NoZW1hOiB7IGRhdGE6IFwiRGF0YVwiIH1cclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBzZXJpZXNEZWZhdWx0czoge1xyXG4gICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgc3RhY2s6IHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCIxMDAlXCJcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgc2VyaWVzOiBbXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiBcIkZhaWxcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgdHlwZTogXCJjb2x1bW5cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiRlwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb2xvcjogXCJcIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgbmFtZTogXCJQYXJzZSBvbmx5XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlBcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IFwiU3VjZXNzXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIlNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9LCB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG5hbWU6IFwiVW5rbm93blwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICB0eXBlOiBcImNvbHVtblwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBmaWVsZDogXCJVXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNvbG9yOiBcIlwiXHJcbiAgICAgICAgICAgICAgICAgICAgfSwge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBuYW1lOiBcIkNhbmFkaWFuXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHR5cGU6IFwiY29sdW1uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZpZWxkOiBcIkNcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgY29sb3I6IFwiXCJcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBdLFxyXG4gICAgICAgICAgICAgICAgdmFsdWVBeGlzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgbGFiZWxzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGZvcm1hdDogXCJ7MDpwMH1cIlxyXG4gICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBtaW46IDAsXHJcbiAgICAgICAgICAgICAgICAgICAgbWF4OiAxXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgY2F0ZWdvcnlBeGlzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgZmllbGQ6IFwiVHlwZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgdG9vbHRpcDoge1xyXG4gICAgICAgICAgICAgICAgICAgIHZpc2libGU6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgZm9ybWF0OiBcInswOnAwfVwiXHJcbiAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgbGVnZW5kOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgdmlzaWJsZTogdHJ1ZSxcclxuICAgICAgICAgICAgICAgICAgICBwb3NpdGlvbjogXCJib3R0b21cIlxyXG4gICAgICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgICAgIGRhdGFCb3VuZDogZnVuY3Rpb24gKCkgeyB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAkKCcjcGFyc2luZ1JlcG9ydCBkaXYnKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgJCgnI3BhcnNpbmdSZXBvcnQnKS5hcHBlbmQoXHJcbiAgICAgICAgICAgICAgICBkaXYuYXBwZW5kKFxyXG4gICAgICAgICAgICAgICAgICAgIHBhbmVsLmFwcGVuZChwYW5lbEJvZHlcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmFwcGVuZCgkKCc8ZGl2IGNsYXNzPVwiY29sLW1kLTZcIi8+JykuYXBwZW5kKGNhc3NDaGFydCkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5hcHBlbmQoJCgnPGRpdiBjbGFzcz1cImNvbC1tZC02XCIvPicpLmFwcGVuZChcIjwvZGl2PlwiKSlcclxuICAgICAgICAgICAgICAgICAgICApXHJcbiAgICAgICAgICAgICAgICApXHJcbiAgICAgICAgICAgICk7XHJcblxyXG4gICAgICAgICAgICAkKCcjb3BlcmF0aW9uUmVwb3J0cyBkaXYnKS5yZW1vdmUoKTtcclxuICAgICAgICAgICAgJC5lYWNoKHJlc3VsdC5EYXRhLFxyXG4gICAgICAgICAgICAgICAgZnVuY3Rpb24gKGksIG9wZXJhdGlvbk5hbWUpIHtcclxuICAgICAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeU9wZXJhdGlvblJlcG9ydE1vZGVsID0gbmV3IEFjY3VyYXRlQXBwZW5kLkpvYlByb2Nlc3NpbmcuU3VtbWFyeS5PcGVyYXRpb25SZXBvcnRNb2RlbChqb2JpZCwgb3BlcmF0aW9uTmFtZSk7IGpvYlByb2Nlc3NpbmdTdW1tYXJ5T3BlcmF0aW9uUmVwb3J0TW9kZWwuZGlzcGxheSgpO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgfSk7XHJcblxyXG4gICAgJCgnI2pvYi1yZXBvcnQtbW9kYWwnKS5tb2RhbCgnc2hvdycpO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbn0iXX0=