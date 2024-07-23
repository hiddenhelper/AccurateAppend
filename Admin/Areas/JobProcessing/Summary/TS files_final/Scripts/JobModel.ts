
/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

/// <reference path="ParentModel.ts" />

module AccurateAppend.JobProcessing.Summary {

    export class JobModel
        extends AccurateAppend.JobProcessing.Summary.ParentModel
    {

        constructor(Email, JobId, links) {
            super(Email, JobId, links);
        }

        reset(url: any) {
            if (url) {
                $.getJSON(url,
                    function (result) {
                        if (!result.success) {
                            alert(result.error);
                        } else
                            jobProcessingSummaryJobModel.renderInProcessGrid();
                        jobProcessingSummaryJobModel.renderCompleteGrid();
                    });
                this.closeJobDetail();
            }
        }

        downloadInputfile(jobid: any) {
        window.location.replace("/JobProcessing/DownloadFiles/Input?jobid=" + jobid);
        }

        downloadOutputfile(jobid: any) {
        window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobid);
        }

        openOutputFileDownloadRenameModal(jobid : any, fileName : any) {
            $('.jobDetails').data("kendoWindow").destroy();
            $("#rename-output-file-modal input[name=jobid]").val(jobid);
            $("#rename-output-file-modal input[name=newFileName]").val(fileName);
            $("#rename-output-file-modal").modal("show");
        }

        downloadOutputFileWithRename() {
            var jobId = $("#rename-output-file-modal input[name=jobid]").val();
            var newFileName = $("#rename-output-file-modal input[name=newFileName]").val();
            $("#rename-output-file-modal").modal("hide");
            window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobId + "&fileName=" + newFileName);
        }

        downloadManifest(url: any) {
            window.location.replace(url);
        }

        remapInputfile(url: any) {
            history.pushState(null, "Jobs", "/JobProcessing/Summary");
            window.location.replace(url);
        }

        setJobComplete(url: any) {
            $.getJSON( url,
                function(result) {
                    if (!result.success) {
                        alert("An error occurred. Job could not be set to complete.\r\n" + result.Message);
                    } else {
                        jobProcessingSummaryJobModel.renderInProcessGrid();
                        jobProcessingSummaryJobModel.renderCompleteGrid();
                    }
                });
            this.closeJobDetail();
        }

        changePriority(url: any, priority: any) {
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
                        jobProcessingSummaryJobModel.renderInProcessGrid();
                        jobProcessingSummaryJobModel.renderCompleteGrid();
                        $("#jobPriority")
                        .append(
                            "<div style=\"display: inline; margin-left: 5px; padding: 8px;\" class=\"alert alert-success\">Priority updated.</div>");
                    }
                });

        }

        deleteJob(jobid: any) {
            $.getJSON($("#DeleteJobController").val(),
                { jobid: jobid },
                function(result) {
                    if (!result.success) {
                        alert(result.error);
                    } else {
                        jobProcessingSummaryJobModel.renderInProcessGrid();
                        jobProcessingSummaryJobModel.renderCompleteGrid();
                    }
                });
            this.closeJobDetail();
        }

        closeJobDetail() {

            var detailsWindow = $('.jobDetails').data("kendoWindow");
            if (detailsWindow) detailsWindow.destroy();

        }

        reassignJob(e) {
            var grid = $("#gridUsers").data("kendoGrid");
            var item : any = grid.dataItem($(e.target).closest("tr"));
            var id = $("#currentJobId").val();
            var userid = item.UserId;

            $.ajax({
                type: "POST",
                url: jobProcessingSummaryJobModel.reassignJob,
                data: { jobid: id, userid: userid },
                success(result) {
                    if (!result.success) {
                        alert(result.message);
                    }

                    $('#users-modal').modal('hide');
                    jobProcessingSummaryJobModel.renderInProcessGrid();
                    jobProcessingSummaryJobModel.renderCompleteGrid();
                    $("#userSearchTerm").val('');
                }
            });
        }

        displayJobDetail(jobid: any) {

            if ($('.jobDetails').data("kendoWindow") != null) $('.jobDetails').data("kendoWindow").destroy();

            $.get("/JobProcessing/Detail?jobid=" + jobid,
                function(data) {
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
                        activate: function() {   $("#priority").kendoDropDownList({
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
                                change: function() {
                                    this.changePriority(jobid, this.value());
                                },
                                value: data.Priority,
                                enable: data.Status !== "<%: (Number) JobStatus.Complete %>"
                            });
                        }
                    }).data("kendoWindow").open();
                });
        }

        associateJobWithExistingDeal(url: any) {
            window.location.replace(url);
        }

        newJob(jobid: any, userid: any) {
            var url = "/Batch/FromExistingJob?userId=" +
                userid +
                "&jobid=" +
                jobid;
            window.location.replace(url);
        }

        openTextReport(url: any) {
            window.open(url);
        }

        downloadTextReport(url: any) {
            window.location.replace(url);
        }

        openJobReassignModal(jobid: any) {
            this.closeJobDetail();
            $("#currentJobId").val(jobid);
            $('#users-modal').modal('show');
}

        newDealFromJob(url: any) {
            window.location.replace(url);
        }

        previewInputFile(jobid: any) {
            window.location.replace("/JobProcessing/PreviewJob/Input?jobid=" +
                jobid);
        }

        previewOuputFile(jobid: any) {
            window.location.replace("/JobProcessing/PreviewJob/Output?jobid=" +
                jobid);
        }

        pause(url: any) {
            $.get(url,
                function(data) {
                    this.closeJobDetail();
                });
        }

        resume(url: any) {
            $.get(url,
                function (data) {
                    this.closeJobDetail();
                });
        }
    }
}