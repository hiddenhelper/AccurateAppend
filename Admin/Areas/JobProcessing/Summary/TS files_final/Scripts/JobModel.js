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
            var JobModel = (function (_super) {
                __extends(JobModel, _super);
                function JobModel(Email, JobId, links) {
                    return _super.call(this, Email, JobId, links) || this;
                }
                JobModel.prototype.reset = function (url) {
                    if (url) {
                        $.getJSON(url, function (result) {
                            if (!result.success) {
                                alert(result.error);
                            }
                            else
                                jobProcessingSummaryJobModel.renderInProcessGrid();
                            jobProcessingSummaryJobModel.renderCompleteGrid();
                        });
                        this.closeJobDetail();
                    }
                };
                JobModel.prototype.downloadInputfile = function (jobid) {
                    window.location.replace("/JobProcessing/DownloadFiles/Input?jobid=" + jobid);
                };
                JobModel.prototype.downloadOutputfile = function (jobid) {
                    window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobid);
                };
                JobModel.prototype.openOutputFileDownloadRenameModal = function (jobid, fileName) {
                    $('.jobDetails').data("kendoWindow").destroy();
                    $("#rename-output-file-modal input[name=jobid]").val(jobid);
                    $("#rename-output-file-modal input[name=newFileName]").val(fileName);
                    $("#rename-output-file-modal").modal("show");
                };
                JobModel.prototype.downloadOutputFileWithRename = function () {
                    var jobId = $("#rename-output-file-modal input[name=jobid]").val();
                    var newFileName = $("#rename-output-file-modal input[name=newFileName]").val();
                    $("#rename-output-file-modal").modal("hide");
                    window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobId + "&fileName=" + newFileName);
                };
                JobModel.prototype.downloadManifest = function (url) {
                    window.location.replace(url);
                };
                JobModel.prototype.remapInputfile = function (url) {
                    history.pushState(null, "Jobs", "/JobProcessing/Summary");
                    window.location.replace(url);
                };
                JobModel.prototype.setJobComplete = function (url) {
                    $.getJSON(url, function (result) {
                        if (!result.success) {
                            alert("An error occurred. Job could not be set to complete.\r\n" + result.Message);
                        }
                        else {
                            jobProcessingSummaryJobModel.renderInProcessGrid();
                            jobProcessingSummaryJobModel.renderCompleteGrid();
                        }
                    });
                    this.closeJobDetail();
                };
                JobModel.prototype.changePriority = function (url, priority) {
                    $.getJSON(url, { priority: priority }, function (result) {
                        $("#jobPriority .alert").remove();
                        if (!result.success) {
                            alert("An error occurred. Priority could not be updated.");
                            $("#jobPriority")
                                .append("<div style=\"display: inline; margin-left: 5px; padding: 8px;\" class=\"alert alert-danger\">Error. Priority not updated.</div>");
                        }
                        else {
                            jobProcessingSummaryJobModel.renderInProcessGrid();
                            jobProcessingSummaryJobModel.renderCompleteGrid();
                            $("#jobPriority")
                                .append("<div style=\"display: inline; margin-left: 5px; padding: 8px;\" class=\"alert alert-success\">Priority updated.</div>");
                        }
                    });
                };
                JobModel.prototype.deleteJob = function (jobid) {
                    $.getJSON($("#DeleteJobController").val(), { jobid: jobid }, function (result) {
                        if (!result.success) {
                            alert(result.error);
                        }
                        else {
                            jobProcessingSummaryJobModel.renderInProcessGrid();
                            jobProcessingSummaryJobModel.renderCompleteGrid();
                        }
                    });
                    this.closeJobDetail();
                };
                JobModel.prototype.closeJobDetail = function () {
                    var detailsWindow = $('.jobDetails').data("kendoWindow");
                    if (detailsWindow)
                        detailsWindow.destroy();
                };
                JobModel.prototype.reassignJob = function (e) {
                    var grid = $("#gridUsers").data("kendoGrid");
                    var item = grid.dataItem($(e.target).closest("tr"));
                    var id = $("#currentJobId").val();
                    var userid = item.UserId;
                    $.ajax({
                        type: "POST",
                        url: jobProcessingSummaryJobModel.reassignJob,
                        data: { jobid: id, userid: userid },
                        success: function (result) {
                            if (!result.success) {
                                alert(result.message);
                            }
                            $('#users-modal').modal('hide');
                            jobProcessingSummaryJobModel.renderInProcessGrid();
                            jobProcessingSummaryJobModel.renderCompleteGrid();
                            $("#userSearchTerm").val('');
                        }
                    });
                };
                JobModel.prototype.displayJobDetail = function (jobid) {
                    if ($('.jobDetails').data("kendoWindow") != null)
                        $('.jobDetails').data("kendoWindow").destroy();
                    $.get("/JobProcessing/Detail?jobid=" + jobid, function (data) {
                        $('<div class="jobDetails" style="padding: 10px;"></div>').kendoWindow({
                            title: "Detail: " + data.JobId,
                            resizable: false,
                            modal: true,
                            visible: false,
                            content: {
                                template: kendo.template($("#jobDetailTemplate").html())(data)
                            },
                            width: "800px",
                            position: { top: "200px", left: "600px" },
                            scrollable: false,
                            activate: function () {
                                $("#priority").kendoDropDownList({
                                    dataTextField: "text",
                                    dataValueField: "value",
                                    dataSource: [
                                        {
                                            text: "<%: Priority.High %>",
                                            value: "<%: (int) Priority.High %>"
                                        },
                                        { text: "<%: Priority.Medium %>", value: "<%: (int) Priority.Medium %>" },
                                        { text: "<%: Priority.Low %>", value: "<%: (int) Priority.Low %>" },
                                        { text: "<%: Priority.Minimal %>", value: "<%: (int) Priority.Minimal %>" },
                                        { text: "<%: Priority.None %>", value: "<%: (int) Priority.None %>" },
                                    ],
                                    change: function () {
                                        this.changePriority(jobid, this.value());
                                    },
                                    value: data.Priority,
                                    enable: data.Status !== "<%: (Number) JobStatus.Complete %>"
                                });
                            }
                        }).data("kendoWindow").open();
                    });
                };
                JobModel.prototype.associateJobWithExistingDeal = function (url) {
                    window.location.replace(url);
                };
                JobModel.prototype.newJob = function (jobid, userid) {
                    var url = "/Batch/FromExistingJob?userId=" +
                        userid +
                        "&jobid=" +
                        jobid;
                    window.location.replace(url);
                };
                JobModel.prototype.openTextReport = function (url) {
                    window.open(url);
                };
                JobModel.prototype.downloadTextReport = function (url) {
                    window.location.replace(url);
                };
                JobModel.prototype.openJobReassignModal = function (jobid) {
                    this.closeJobDetail();
                    $("#currentJobId").val(jobid);
                    $('#users-modal').modal('show');
                };
                JobModel.prototype.newDealFromJob = function (url) {
                    window.location.replace(url);
                };
                JobModel.prototype.previewInputFile = function (jobid) {
                    window.location.replace("/JobProcessing/PreviewJob/Input?jobid=" +
                        jobid);
                };
                JobModel.prototype.previewOuputFile = function (jobid) {
                    window.location.replace("/JobProcessing/PreviewJob/Output?jobid=" +
                        jobid);
                };
                JobModel.prototype.pause = function (url) {
                    $.get(url, function (data) {
                        this.closeJobDetail();
                    });
                };
                JobModel.prototype.resume = function (url) {
                    $.get(url, function (data) {
                        this.closeJobDetail();
                    });
                };
                return JobModel;
            }(AccurateAppend.JobProcessing.Summary.ParentModel));
            Summary.JobModel = JobModel;
        })(Summary = JobProcessing.Summary || (JobProcessing.Summary = {}));
    })(JobProcessing = AccurateAppend.JobProcessing || (AccurateAppend.JobProcessing = {}));
})(AccurateAppend || (AccurateAppend = {}));
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSm9iTW9kZWwuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJKb2JNb2RlbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7Ozs7Ozs7Ozs7O0FBTUEsSUFBTyxjQUFjLENBcU9wQjtBQXJPRCxXQUFPLGNBQWM7SUFBQyxJQUFBLGFBQWEsQ0FxT2xDO0lBck9xQixXQUFBLGFBQWE7UUFBQyxJQUFBLE9BQU8sQ0FxTzFDO1FBck9tQyxXQUFBLE9BQU87WUFFdkM7Z0JBQ1ksNEJBQWdEO2dCQUd4RCxrQkFBWSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUs7MkJBQzNCLGtCQUFNLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxDQUFDO2dCQUM5QixDQUFDO2dCQUVELHdCQUFLLEdBQUwsVUFBTSxHQUFRO29CQUNWLElBQUksR0FBRyxFQUFFO3dCQUNMLENBQUMsQ0FBQyxPQUFPLENBQUMsR0FBRyxFQUNULFVBQVUsTUFBTTs0QkFDWixJQUFJLENBQUMsTUFBTSxDQUFDLE9BQU8sRUFBRTtnQ0FDakIsS0FBSyxDQUFDLE1BQU0sQ0FBQyxLQUFLLENBQUMsQ0FBQzs2QkFDdkI7O2dDQUNHLDRCQUE0QixDQUFDLG1CQUFtQixFQUFFLENBQUM7NEJBQ3ZELDRCQUE0QixDQUFDLGtCQUFrQixFQUFFLENBQUM7d0JBQ3RELENBQUMsQ0FBQyxDQUFDO3dCQUNQLElBQUksQ0FBQyxjQUFjLEVBQUUsQ0FBQztxQkFDekI7Z0JBQ0wsQ0FBQztnQkFFRCxvQ0FBaUIsR0FBakIsVUFBa0IsS0FBVTtvQkFDNUIsTUFBTSxDQUFDLFFBQVEsQ0FBQyxPQUFPLENBQUMsMkNBQTJDLEdBQUcsS0FBSyxDQUFDLENBQUM7Z0JBQzdFLENBQUM7Z0JBRUQscUNBQWtCLEdBQWxCLFVBQW1CLEtBQVU7b0JBQzdCLE1BQU0sQ0FBQyxRQUFRLENBQUMsT0FBTyxDQUFDLDRDQUE0QyxHQUFHLEtBQUssQ0FBQyxDQUFDO2dCQUM5RSxDQUFDO2dCQUVELG9EQUFpQyxHQUFqQyxVQUFrQyxL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Q0FBQyxDQUFDLENBQUM7Z0JBQ1gsQ0FBQztnQkFDTCxlQUFDO1lBQUQsQ0FBQyxBQWxPRCxDQUNZLGNBQWMsQ0FBQyxhQUFhLENBQUMsT0FBTyxDQUFDLFdBQVcsR0FpTzNEO1lBbE9ZLGdCQUFRLFdBa09wQixDQUFBO1FBQ0wsQ0FBQyxFQXJPbUMsT0FBTyxHQUFQLHFCQUFPLEtBQVAscUJBQU8sUUFxTzFDO0lBQUQsQ0FBQyxFQXJPcUIsYUFBYSxHQUFiLDRCQUFhLEtBQWIsNEJBQWEsUUFxT2xDO0FBQUQsQ0FBQyxFQXJPTSxjQUFjLEtBQWQsY0FBYyxRQXFPcEIiLCJzb3VyY2VzQ29udGVudCI6WyJcclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uLy4uLy4uL3NjcmlwdHMvdHlwaW5ncy9tb21lbnQvbW9tZW50LmQudHNcIiAvPlxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vLi4vLi4vLi4vc2NyaXB0cy90eXBpbmdzL2tlbmRvLXVpL2tlbmRvLXVpLmQudHNcIiAvPlxyXG5cclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIlBhcmVudE1vZGVsLnRzXCIgLz5cclxuXHJcbm1vZHVsZSBBY2N1cmF0ZUFwcGVuZC5Kb2JQcm9jZXNzaW5nLlN1bW1hcnkge1xyXG5cclxuICAgIGV4cG9ydCBjbGFzcyBKb2JNb2RlbFxyXG4gICAgICAgIGV4dGVuZHMgQWNjdXJhdGVBcHBlbmQuSm9iUHJvY2Vzc2luZy5TdW1tYXJ5LlBhcmVudE1vZGVsXHJcbiAgICB7XHJcblxyXG4gICAgICAgIGNvbnN0cnVjdG9yKEVtYWlsLCBKb2JJZCwgbGlua3MpIHtcclxuICAgICAgICAgICAgc3VwZXIoRW1haWwsIEpvYklkLCBsaW5rcyk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXNldCh1cmw6IGFueSkge1xyXG4gICAgICAgICAgICBpZiAodXJsKSB7XHJcbiAgICAgICAgICAgICAgICAkLmdldEpTT04odXJsLFxyXG4gICAgICAgICAgICAgICAgICAgIGZ1bmN0aW9uIChyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKCFyZXN1bHQuc3VjY2Vzcykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYWxlcnQocmVzdWx0LmVycm9yKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfSBlbHNlXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLnJlbmRlckluUHJvY2Vzc0dyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgam9iUHJvY2Vzc2luZ1N1bW1hcnlKb2JNb2RlbC5yZW5kZXJDb21wbGV0ZUdyaWQoKTtcclxuICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIHRoaXMuY2xvc2VKb2JEZXRhaWwoKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZG93bmxvYWRJbnB1dGZpbGUoam9iaWQ6IGFueSkge1xyXG4gICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKFwiL0pvYlByb2Nlc3NpbmcvRG93bmxvYWRGaWxlcy9JbnB1dD9qb2JpZD1cIiArIGpvYmlkKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGRvd25sb2FkT3V0cHV0ZmlsZShqb2JpZDogYW55KSB7XHJcbiAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoXCIvSm9iUHJvY2Vzc2luZy9Eb3dubG9hZEZpbGVzL091dHB1dD9qb2JpZD1cIiArIGpvYmlkKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIG9wZW5PdXRwdXRGaWxlRG93bmxvYWRSZW5hbWVNb2RhbChqb2JpZCA6IGFueSwgZmlsZU5hbWUgOiBhbnkpIHtcclxuICAgICAgICAgICAgJCgnLmpvYkRldGFpbHMnKS5kYXRhKFwia2VuZG9XaW5kb3dcIikuZGVzdHJveSgpO1xyXG4gICAgICAgICAgICAkKFwiI3JlbmFtZS1vdXRwdXQtZmlsZS1tb2RhbCBpbnB1dFtuYW1lPWpvYmlkXVwiKS52YWwoam9iaWQpO1xyXG4gICAgICAgICAgICAkKFwiI3JlbmFtZS1vdXRwdXQtZmlsZS1tb2RhbCBpbnB1dFtuYW1lPW5ld0ZpbGVOYW1lXVwiKS52YWwoZmlsZU5hbWUpO1xyXG4gICAgICAgICAgICAkKFwiI3JlbmFtZS1vdXRwdXQtZmlsZS1tb2RhbFwiKS5tb2RhbChcInNob3dcIik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBkb3dubG9hZE91dHB1dEZpbGVXaXRoUmVuYW1lKCkge1xyXG4gICAgICAgICAgICB2YXIgam9iSWQgPSAkKFwiI3JlbmFtZS1vdXRwdXQtZmlsZS1tb2RhbCBpbnB1dFtuYW1lPWpvYmlkXVwiKS52YWwoKTtcclxuICAgICAgICAgICAgdmFyIG5ld0ZpbGVOYW1lID0gJChcIiNyZW5hbWUtb3V0cHV0LWZpbGUtbW9kYWwgaW5wdXRbbmFtZT1uZXdGaWxlTmFtZV1cIikudmFsKCk7XHJcbiAgICAgICAgICAgICQoXCIjcmVuYW1lLW91dHB1dC1maWxlLW1vZGFsXCIpLm1vZGFsKFwiaGlkZVwiKTtcclxuICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoXCIvSm9iUHJvY2Vzc2luZy9Eb3dubG9hZEZpbGVzL091dHB1dD9qb2JpZD1cIiArIGpvYklkICsgXCImZmlsZU5hbWU9XCIgKyBuZXdGaWxlTmFtZSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBkb3dubG9hZE1hbmlmZXN0KHVybDogYW55KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHVybCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZW1hcElucHV0ZmlsZSh1cmw6IGFueSkge1xyXG4gICAgICAgICAgICBoaXN0b3J5LnB1c2hTdGF0ZShudWxsLCBcIkpvYnNcIiwgXCIvSm9iUHJvY2Vzc2luZy9TdW1tYXJ5XCIpO1xyXG4gICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVwbGFjZSh1cmwpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgc2V0Sm9iQ29tcGxldGUodXJsOiBhbnkpIHtcclxuICAgICAgICAgICAgJC5nZXRKU09OKCB1cmwsXHJcbiAgICAgICAgICAgICAgICBmdW5jdGlvbihyZXN1bHQpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAoIXJlc3VsdC5zdWNjZXNzKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGFsZXJ0KFwiQW4gZXJyb3Igb2NjdXJyZWQuIEpvYiBjb3VsZCBub3QgYmUgc2V0IHRvIGNvbXBsZXRlLlxcclxcblwiICsgcmVzdWx0Lk1lc3NhZ2UpO1xyXG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVuZGVySW5Qcm9jZXNzR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB0aGlzLmNsb3NlSm9iRGV0YWlsKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjaGFuZ2VQcmlvcml0eSh1cmw6IGFueSwgcHJpb3JpdHk6IGFueSkge1xyXG4gICAgICAgICAgICAkLmdldEpTT04oXHJcbiAgICAgICAgICAgICAgICB1cmwsXHJcbiAgICAgICAgICAgICAgICB7IHByaW9yaXR5OiBwcmlvcml0eSB9LFxyXG4gICAgICAgICAgICAgICAgZnVuY3Rpb24ocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiNqb2JQcmlvcml0eSAuYWxlcnRcIikucmVtb3ZlKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyZXN1bHQuc3VjY2Vzcykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydChcIkFuIGVycm9yIG9jY3VycmVkLiBQcmlvcml0eSBjb3VsZCBub3QgYmUgdXBkYXRlZC5cIik7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICQoXCIjam9iUHJpb3JpdHlcIilcclxuICAgICAgICAgICAgICAgICAgICAgICAgLmFwcGVuZChcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIFwiPGRpdiBzdHlsZT1cXFwiZGlzcGxheTogaW5saW5lOyBtYXJnaW4tbGVmdDogNXB4OyBwYWRkaW5nOiA4cHg7XFxcIiBjbGFzcz1cXFwiYWxlcnQgYWxlcnQtZGFuZ2VyXFxcIj5FcnJvci4gUHJpb3JpdHkgbm90IHVwZGF0ZWQuPC9kaXY+XCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVuZGVySW5Qcm9jZXNzR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAkKFwiI2pvYlByaW9yaXR5XCIpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIC5hcHBlbmQoXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBcIjxkaXYgc3R5bGU9XFxcImRpc3BsYXk6IGlubGluZTsgbWFyZ2luLWxlZnQ6IDVweDsgcGFkZGluZzogOHB4O1xcXCIgY2xhc3M9XFxcImFsZXJ0IGFsZXJ0LXN1Y2Nlc3NcXFwiPlByaW9yaXR5IHVwZGF0ZWQuPC9kaXY+XCIpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGRlbGV0ZUpvYihqb2JpZDogYW55KSB7XHJcbiAgICAgICAgICAgICQuZ2V0SlNPTigkKFwiI0RlbGV0ZUpvYkNvbnRyb2xsZXJcIikudmFsKCksXHJcbiAgICAgICAgICAgICAgICB7IGpvYmlkOiBqb2JpZCB9LFxyXG4gICAgICAgICAgICAgICAgZnVuY3Rpb24ocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyZXN1bHQuc3VjY2Vzcykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydChyZXN1bHQuZXJyb3IpO1xyXG4gICAgICAgICAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVuZGVySW5Qcm9jZXNzR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBqb2JQcm9jZXNzaW5nU3VtbWFyeUpvYk1vZGVsLnJlbmRlckNvbXBsZXRlR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICB0aGlzLmNsb3NlSm9iRGV0YWlsKCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjbG9zZUpvYkRldGFpbCgpIHtcclxuXHJcbiAgICAgICAgICAgIHZhciBkZXRhaWxzV2luZG93ID0gJCgnLmpvYkRldGFpbHMnKS5kYXRhKFwia2VuZG9XaW5kb3dcIik7XHJcbiAgICAgICAgICAgIGlmIChkZXRhaWxzV2luZG93KSBkZXRhaWxzV2luZG93LmRlc3Ryb3koKTtcclxuXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZWFzc2lnbkpvYihlKSB7XHJcbiAgICAgICAgICAgIHZhciBncmlkID0gJChcIiNncmlkVXNlcnNcIikuZGF0YShcImtlbmRvR3JpZFwiKTtcclxuICAgICAgICAgICAgdmFyIGl0ZW0gOiBhbnkgPSBncmlkLmRhdGFJdGVtKCQoZS50YXJnZXQpLmNsb3Nlc3QoXCJ0clwiKSk7XHJcbiAgICAgICAgICAgIHZhciBpZCA9ICQoXCIjY3VycmVudEpvYklkXCIpLnZhbCgpO1xyXG4gICAgICAgICAgICB2YXIgdXNlcmlkID0gaXRlbS5Vc2VySWQ7XHJcblxyXG4gICAgICAgICAgICAkLmFqYXgoe1xyXG4gICAgICAgICAgICAgICAgdHlwZTogXCJQT1NUXCIsXHJcbiAgICAgICAgICAgICAgICB1cmw6IGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVhc3NpZ25Kb2IsXHJcbiAgICAgICAgICAgICAgICBkYXRhOiB7IGpvYmlkOiBpZCwgdXNlcmlkOiB1c2VyaWQgfSxcclxuICAgICAgICAgICAgICAgIHN1Y2Nlc3MocmVzdWx0KSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKCFyZXN1bHQuc3VjY2Vzcykge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBhbGVydChyZXN1bHQubWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgICAgICAkKCcjdXNlcnMtbW9kYWwnKS5tb2RhbCgnaGlkZScpO1xyXG4gICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVuZGVySW5Qcm9jZXNzR3JpZCgpO1xyXG4gICAgICAgICAgICAgICAgICAgIGpvYlByb2Nlc3NpbmdTdW1tYXJ5Sm9iTW9kZWwucmVuZGVyQ29tcGxldGVHcmlkKCk7XHJcbiAgICAgICAgICAgICAgICAgICAgJChcIiN1c2VyU2VhcmNoVGVybVwiKS52YWwoJycpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGRpc3BsYXlKb2JEZXRhaWwoam9iaWQ6IGFueSkge1xyXG5cclxuICAgICAgICAgICAgaWYgKCQoJy5qb2JEZXRhaWxzJykuZGF0YShcImtlbmRvV2luZG93XCIpICE9IG51bGwpICQoJy5qb2JEZXRhaWxzJykuZGF0YShcImtlbmRvV2luZG93XCIpLmRlc3Ryb3koKTtcclxuXHJcbiAgICAgICAgICAgICQuZ2V0KFwiL0pvYlByb2Nlc3NpbmcvRGV0YWlsP2pvYmlkPVwiICsgam9iaWQsXHJcbiAgICAgICAgICAgICAgICBmdW5jdGlvbihkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgJCgnPGRpdiBjbGFzcz1cImpvYkRldGFpbHNcIiBzdHlsZT1cInBhZGRpbmc6IDEwcHg7XCI+PC9kaXY+Jykua2VuZG9XaW5kb3coe1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB0aXRsZTogXCJEZXRhaWw6IFwiICsgZGF0YS5Kb2JJZCxcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmVzaXphYmxlOiBmYWxzZSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgbW9kYWw6IHRydWUsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHZpc2libGU6IGZhbHNlLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICBjb250ZW50OiB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB0ZW1wbGF0ZToga2VuZG8udGVtcGxhdGUoJChcIiNqb2JEZXRhaWxUZW1wbGF0ZVwiKS5odG1sKCkpKGRhdGEpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHdpZHRoOiBcIjgwMHB4XCIsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBvc2l0aW9uOiB7IHRvcDogXCIyMDBweFwiLCBsZWZ0OiBcIjYwMHB4XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgc2Nyb2xsYWJsZTogZmFsc2UsXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGFjdGl2YXRlOiBmdW5jdGlvbigpIHsgICAkKFwiI3ByaW9yaXR5XCIpLmtlbmRvRHJvcERvd25MaXN0KHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhVGV4dEZpZWxkOiBcInRleHRcIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBkYXRhVmFsdWVGaWVsZDogXCJ2YWx1ZVwiLFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGRhdGFTb3VyY2U6IFtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHRleHQ6IFwiPCU6IFByaW9yaXR5LkhpZ2ggJT5cIixcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdmFsdWU6IFwiPCU6IChpbnQpIFByaW9yaXR5LkhpZ2ggJT5cIlxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgeyB0ZXh0OiBcIjwlOiBQcmlvcml0eS5NZWRpdW0gJT5cIiwgdmFsdWU6IFwiPCU6IChpbnQpIFByaW9yaXR5Lk1lZGl1bSAlPlwiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgeyB0ZXh0OiBcIjwlOiBQcmlvcml0eS5Mb3cgJT5cIiwgdmFsdWU6IFwiPCU6IChpbnQpIFByaW9yaXR5LkxvdyAlPlwiIH0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgeyB0ZXh0OiBcIjwlOiBQcmlvcml0eS5NaW5pbWFsICU+XCIsIHZhbHVlOiBcIjwlOiAoaW50KSBQcmlvcml0eS5NaW5pbWFsICU+XCIgfSxcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB7IHRleHQ6IFwiPCU6IFByaW9yaXR5Lk5vbmUgJT5cIiwgdmFsdWU6IFwiPCU6IChpbnQpIFByaW9yaXR5Lk5vbmUgJT5cIiB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgY2hhbmdlOiBmdW5jdGlvbigpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgdGhpcy5jaGFuZ2VQcmlvcml0eShqb2JpZCwgdGhpcy52YWx1ZSgpKTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHZhbHVlOiBkYXRhLlByaW9yaXR5LFxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIGVuYWJsZTogZGF0YS5TdGF0dXMgIT09IFwiPCU6IChOdW1iZXIpIEpvYlN0YXR1cy5Db21wbGV0ZSAlPlwiXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIH0pLmRhdGEoXCJrZW5kb1dpbmRvd1wiKS5vcGVuKCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGFzc29jaWF0ZUpvYldpdGhFeGlzdGluZ0RlYWwodXJsOiBhbnkpIHtcclxuICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UodXJsKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIG5ld0pvYihqb2JpZDogYW55LCB1c2VyaWQ6IGFueSkge1xyXG4gICAgICAgICAgICB2YXIgdXJsID0gXCIvQmF0Y2gvRnJvbUV4aXN0aW5nSm9iP3VzZXJJZD1cIiArXHJcbiAgICAgICAgICAgICAgICB1c2VyaWQgK1xyXG4gICAgICAgICAgICAgICAgXCImam9iaWQ9XCIgK1xyXG4gICAgICAgICAgICAgICAgam9iaWQ7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHVybCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBvcGVuVGV4dFJlcG9ydCh1cmw6IGFueSkge1xyXG4gICAgICAgICAgICB3aW5kb3cub3Blbih1cmwpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZG93bmxvYWRUZXh0UmVwb3J0KHVybDogYW55KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKHVybCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBvcGVuSm9iUmVhc3NpZ25Nb2RhbChqb2JpZDogYW55KSB7XHJcbiAgICAgICAgICAgIHRoaXMuY2xvc2VKb2JEZXRhaWwoKTtcclxuICAgICAgICAgICAgJChcIiNjdXJyZW50Sm9iSWRcIikudmFsKGpvYmlkKTtcclxuICAgICAgICAgICAgJCgnI3VzZXJzLW1vZGFsJykubW9kYWwoJ3Nob3cnKTtcclxufVxyXG5cclxuICAgICAgICBuZXdEZWFsRnJvbUpvYih1cmw6IGFueSkge1xyXG4gICAgICAgICAgICB3aW5kb3cubG9jYXRpb24ucmVwbGFjZSh1cmwpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHJldmlld0lucHV0RmlsZShqb2JpZDogYW55KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5sb2NhdGlvbi5yZXBsYWNlKFwiL0pvYlByb2Nlc3NpbmcvUHJldmlld0pvYi9JbnB1dD9qb2JpZD1cIiArXHJcbiAgICAgICAgICAgICAgICBqb2JpZCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwcmV2aWV3T3VwdXRGaWxlKGpvYmlkOiBhbnkpIHtcclxuICAgICAgICAgICAgd2luZG93LmxvY2F0aW9uLnJlcGxhY2UoXCIvSm9iUHJvY2Vzc2luZy9QcmV2aWV3Sm9iL091dHB1dD9qb2JpZD1cIiArXHJcbiAgICAgICAgICAgICAgICBqb2JpZCk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwYXVzZSh1cmw6IGFueSkge1xyXG4gICAgICAgICAgICAkLmdldCh1cmwsXHJcbiAgICAgICAgICAgICAgICBmdW5jdGlvbihkYXRhKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5jbG9zZUpvYkRldGFpbCgpO1xyXG4gICAgICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICByZXN1bWUodXJsOiBhbnkpIHtcclxuICAgICAgICAgICAgJC5nZXQodXJsLFxyXG4gICAgICAgICAgICAgICAgZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLmNsb3NlSm9iRGV0YWlsKCk7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iXX0=