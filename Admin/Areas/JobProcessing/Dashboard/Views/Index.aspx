<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Int32>" %>
<%@ Import Namespace="AccurateAppend.Core.Definitions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Job Slice Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row" style="padding: 0 20px 0 20px;">

        <div class="col-md-7">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Job Info</h3>
                </div>
                <div class="panel-body" style="line-height: 1em">
                    <div id="jobInfo2"></div>
                </div>
            </div>
        </div>
        <div class="col-md-5">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Slice Status</h3>
                </div>
                <div class="panel-body">
                    <div id="slicesByStatus2"></div>
                </div>
            </div>
        </div>

    </div>

    <div class="row" style="padding: 0 20px 0 35px;">

          <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active"><a href="#jobEvents" aria-controls="jobEvents" role="tab" data-toggle="tab">Job Events</a></li>
            <li role="presentation"><a href="#sliceEvents" aria-controls="sliceEvents" role="tab" data-toggle="tab">Slice Events</a></li>
          </ul>

          <!-- Tab panes -->
          <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="jobEvents" style="padding-top: 20px;">
                <div class="alert alert-info"></div>
                <div id="jobEventSummary"></div>
            </div>
            <div role="tabpanel" class="tab-pane" id="sliceEvents" style="padding-top: 20px;">
                <div class="alert alert-info"></div>
                <div id="sliceEventSummary"></div>
            </div>
          </div>

    </div>

    <input type="hidden" id="jobId" value="<%: this.Model %>"/>

    <script type="text/x-kendo-template" id="templateSlicesByStatus2">

        <table class="table table-condensed">
            <tr>
                <th>Status</th>
                <th>Host</th>
                <th>System Errors</th>
                <th>Slices</th>
                <th>Last Active</th>
            </tr>
            # for (var i = 0; i < data.length; i++) { #
           <tr>
                <td>${ data[i].Processor }</td>
                <td>${ data[i].Status }</td>
                <td>${ data[i].SystemErrors }</td>
                <td>${ data[i].Count }</td>
                <td>${ data[i].LastActive }</td>
            </tr>
            # } #
        </table>

    </script>

    <script type="text/x-kendo-template" id="templateEventSummary">

          # for (var i = 0; i < data.length; i++) { #
          <table class="table table-bordered">
            <tr>
                <th style="width: 100px;">Count</th>
                <th style="width: 170px;">First Reported</th>
                <th style="width: 170px;">Last Reported</th>
                <th>Message</th>
            </tr>
            <tr>
                <td>${ data[i].Count }</td>
                <td>${ data[i].FirstTime }</td>
                <td>${ data[i].LastTime }</td>
                <td>${ data[i].Message }</td>
            </tr>
            # if (data[i].StackTrace) { #
            <tr>
                <td colspan="4"><pre>${ data[i].StackTrace }</pre></td>
            </tr>
            # } #
            # if (data[i].EventByHost) { #
            <tr>
                <td colspan="4">
                    <table class="table table-bordered">
                        <tr>
                            <th>Host</th>
                            <th>Event Count</th>
                        </tr>
                    # for (var h = 0; h < data[i].EventByHost.length; h++) { #
                        <tr>
                            <td>${ data[i].EventByHost[h].Host }</td>
                            <td>${ data[i].EventByHost[h].Count }</td>
                        </tr>
                    # } #
                    </table>
                </td>
            </tr>
            # } #
        </table>
        # } #

    </script>

    <script type="text/x-kendo-template" id="templateJobInfo2">

          <div class="col-md-6">
            <div>
                <label>Job Id</label>
                <p><a href="/JobProcessing/Summary/?jobid=${ JobId }">${ JobId }</a></p>

            </div>
            <div>
                <label>Customer File Name</label>
                <p>${ CustomerFileName }</p>
            </div>
            <div>
                <label>System File Name</label>
                <p>${ InputFileName }</p>
            </div>
            <div>
                <label>Date Submitted</label>
                <p>${ DateSubmitted }</p>
            </div>
            # if(Status=="<%: JobStatus.Complete %>") { #
            <div>
                <label>Date Complete</label>
                <p>${ DateComplete }</p>
            </div>
            # } #
        </div>
        <div class="col-md-6">
            <div>
                <label>Status</label>
                <p>${ Status }</p>
            </div>
            <div>
                <label>Total Records</label>
                <p>${ TotalRecords }</p>
            </div>
            <div>
                <label>Match Records</label>
                <p>${ MatchRecords }</p>
            </div>
            <div>
                <label>System Errors</label>
                <p>${ SystemErrors }</p>
            </div>
        </div>

    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript" src="<%= this.Url.Content("~/Scripts/loading-overlay/loadingoverlay.js") %>"></script>

    <script type="text/javascript">

        $(function() {
            $("#eventAccordion").collapse();
            viewModel.refreshView();
            jobModel.refreshView();
            sliceModel.refreshView();
            setInterval(viewModel.refreshView, 30000);
        });

        var jobModel = {
            refreshView: function() {
                $.ajax({
                    url: "/JobProcessing/Detail",
                    dataType: 'json',
                    type: 'GET',
                    data: { jobid: $('#jobId').val() },
                    success: function (result) {
                        $("#jobInfo2").html(kendo.template($("#templateJobInfo2").html())(result));
                    }
                });
            }
        }

        var sliceModel = {
            refreshView: function () {
                $.ajax({
                    url: "/JobProcessing/Dashboard/SliceStatus",
                    dataType: 'json',
                    type: 'GET',
                    data: { jobid: $('#jobId').val() },
                    success: function (result) {
                        $("#slicesByStatus2").html(kendo.template($("#templateSlicesByStatus2").html())(result));
                    }
                });
            }
        }

        var viewModel = {
            refreshView: function () {
                $.ajax({
                    url: "/JobProcessing/DashBoard/EventSummary",
                    dataType: 'json',
                    type: 'GET',
                    data: { jobid: $('#jobId').val() },
                    success: function (result) {
                        $('a[href="#jobEvents"]').text("Job Events (" + result.JobEventSummary.length + ")");
                        $('a[href="#sliceEvents"]').text("Slice Events (" + result.SliceEventSummary.length + ")");

                        viewModel.refreshSliceEventSummary(result.SliceEventSummary);
                        viewModel.refreshJobEventSummary(result.JobEventSummary);
                        if (result.SliceEventSummary.length > 0) $('a[href="#sliceEvents"]').tab('show');
                    }
                });
            },
            refreshSliceEventSummary: function (data) {
                if (data.length > 0) {
                    $("#sliceEventSummary").show();
                    $("#sliceEvents .alert").hide();
                    $("#sliceEventSummary").html(kendo.template($("#templateEventSummary").html())(data));
                } else {
                    $("#sliceEventSummary").hide();
                    $("#sliceEvents .alert").text("No Events Found").show();
                }
            },
            refreshJobEventSummary: function (data) {
                if (data.length > 0) {
                    $("#jobEventSummary").show();
                    $("#jobEvents .alert").hide();
                    $("#jobEventSummary").html(kendo.template($("#templateEventSummary").html())(data));
                } else {
                    $("#jobEventSummary").hide();
                    $("#jobEvents .alert").text("No Events Found").show();
                }
            }
        }

    </script>

</asp:Content>