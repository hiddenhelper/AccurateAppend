/// <reference path="../../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../../../scripts/typings/kendo-ui/kendo-ui.d.ts" />

/// <reference path="ParentModel.ts" />

module AccurateAppend.JobProcessing.Summary {
    export class NationBuilderModel extends AccurateAppend.JobProcessing.Summary.ParentModel {

        constructor(Email, JobId, links) {
            super(Email, JobId, links);
        }

        static resume(pushid: any) {
            $.confirm({
                text: "Are you sure you want to resume this push?",
                confirm: function (button) {
                    console.log('calling resume for ' + pushid);
                    $.getJSON(
                        "<%= Url.BuildFor<ResumeController>().ToResume() %>?id=" + // TODO: NEEDS TO COME FROM JOB SUMMARY
                        pushid,
                        function (result) {
                            console.log('resume returning status = ' +
                                result.HttpStatusCode +
                                ', message = ' +
                                result.Message);
                            if (result.HttpStatusCode !== 200) {
                                $('#nationbuilderpushes')
                                    .prepend(
                                        '<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                                        result.Message +
                                        '</div>');
                            } else {
                                this.renderNationBuilderInprocessGrid();
                                this.renderNationBuilderCompleteGrid();
                                $('#nationbuilderpushes .alert').remove();
                            }
                        });
                },
                cancel: function (button) {
                    // do something
                },
                confirmButton: "Yes",
                cancelButton: "Close"
            });
        }

        cancel(pushid: any) {
            $.confirm({
                text: "Are you sure you want to cancel this push?",
                confirm: function (button) {
                    console.log('calling confirm for ' + pushid);
                    $.getJSON(
                        "<%= Url.BuildFor<CancelController>().ToCancel() %>?id=" + pushid, // TODO: NEEDS TO COME FROM JOB SUMMARY
                        function (result) {
                            console.log('confirm returning status = ' +
                                result.HttpStatusCode +
                                ', message = ' +
                                result.Message);
                            if (result.HttpStatusCode !== 200) {
                                $('#nationbuilderpushes')
                                    .prepend(
                                        '<div class="alert alert-danger" style="display: none; margin-bottom: 20px;">' +
                                        result.Message +
                                        '</div>');
                            } else {
                                this.renderNationBuilderInprocessGrid();
                                this.renderNationBuilderCompleteGrid();
                                $('#nationbuilderpushes .alert').remove();
                            }
                        });
                },
                cancel: function (button) {
                    // do something
                },
                confirmButton: "Yes",
                cancelButton: "Close"
            });
        }
    }
}