<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<EventsRequest>" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.Operations.EventLog.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Event Log
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="row">
    <div class="col-md-6">
      <div id="chartLast30"></div>
    </div>
    <div class="col-md-6" style="padding-right: 25px;">
      <div id="chartLast24"></div>
    </div>
  </div>

  <div class="row" id="controls">
    <div class="pull-right" style="padding: 15px 20px;">
      <button type="button" class="btn btn-default" id="refresh" onclick=" javascript:reset(); "><span class="fa fa-refresh"></span>Reset</button>
      <input type="checkbox" data-toggle="toggle" data-size="small" id="includeInformation"><span style="margin-left: 10px;">Information</span>
      <select class="form-control" id="dateRange" style="display: inline; width: 160px;">
        <option value="Last60Minutes">Last 60 Minutes</option>
        <option value="Last24Hours">Last 24 Hours</option>
        <option value="Yesterday">Yesterday</option>
      </select>
      <select class="form-control" id="eventType" style="display: inline; width: 160px;"></select>
      <select class="form-control" id="hours" style="display: inline; width: 175px;"></select>
      <select class="form-control" id="severity" style="display: inline; width: 160px;"></select>
      <select class="form-control" id="applications" style="display: inline; width: 250px;"></select>
      <select class="form-control" id="hosts" style="display: inline; width: 150px;"></select>
    </div>
  </div>

  <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="gridInfo"></div>
  <div id="grid" style="display: none; margin-bottom: 20px; margin-top: 10px;"></div>

  <div class="modal fade" id="email-modal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #F5F5F5;">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
          <h4 class="modal-title">Email notification</h4>
        </div>
        <div class="modal-body" id="notification-body">
          <form>
            <div class="form-group">
              <%: Html.DropDownList("adminEmails", Model.Email.Select(e => new SelectListItem {Text = e}), new {@class = "form-control", id = "adminEmails", style = "margin-bottom: 10px;"}) %>
            </div>
            <div class="form-group">
              <input class="form-control" id="email" name="email" placeholder="Email" type="email" required/>
            </div>
            <div class="form-group">
              <input class="form-control" id="subject" name="subject" placeholder="Subject"/>
            </div>
            <div class="form-group">
              <textarea class="form-control" id="message" name="message" placeholder="Message" rows="5"></textarea>
            </div>
            <div class="form-group">
              <button class="btn btn-primary" type="button" onclick=" javascript:sendMessage(); ">Send</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>

  <script type="text/x-kendo-template" id="detail-template">
			
			<div style="margin-bottom: 20px;">
				<p style="margin-top: 10px;">
					<button type="button" class="btn btn-default" onclick="javascript:openEmailModal(#= id #);"><span class="fa fa-envelope"></span>Email</button>
				</p>        
				 <table>
					<tr>
						<th>Id</th>
						<td>#= id #</td>
					</tr>
					<tr>
						<th>EventDate</th>
						<td>#= EventDate #</td>
					</tr>
					<tr>
						<th>Application</th>
						<td>#= Application #</td>
					</tr>
					<tr>
						<th>Host</th>
						<td>#= Host #</td>
					</tr>
					<tr>
						<th>Severity</th>
						<td>#= Severity #</td>
					</tr>
					<tr>
						<th>EventType</th>
						<td>#= EventType #</td>
					</tr>
					<tr>
						<th>Message</th>
						<td>#= Message #</td>
					</tr>
					<tr>
						<th>ThreadIdentity</th>
						<td>#= ThreadIdentity #</td>
					</tr>
					<tr>
						<th>CorrelationId</th>
						<td>#= CorrelationId #</td>
					</tr>
					<tr>
						<th>StackTrace</th>
						<td><pre>#= StackTrace #</pre></td>
					</tr>
					<tr>
						<th>Source</th>
						<td>#= Source #</td>
					</tr>
					<tr>
						<th>Target</th>
						<td>#= Target #</td>
					</tr>
					<tr>
						<th>Description</th>
						<td>#= Description #</td>
					</tr>
					<tr>
						<th>IP</th>
						<td>#= IP #</td>
					</tr>
					<tr>
						<th>Username</th>
						<td>#= Username #</td>
					</tr>
					<tr>
						<th>RelatedEvents</th>
						<td>#= RelatedEvents #</td>
					</tr>
				</table>
		  </div>

	</script>

  <script type="text/x-kendo-template" id="command-template">
		#if(RelatedEvents > 2){#
			<a href="/Operations/EventLog/Index?correlationId=#= CorrelationId #">#= id #</a>
		#} else {#
			#= id #
		#}#

	</script>

  <input type="hidden" id="eventid"/>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <style type="text/css">
	   
    .k-loading-mask { display: none; }

  </style>

  <!-- http://www.bootstraptoggle.com/ -->
  <link href="//gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
  <script src="//gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>

  <script type="text/javascript">

    var pEventId = <%= Model.EventId != null ? "'" + Model.EventId + "'" : "null" %>;
    var correlationid = <%= Model.CorrelationId != null ? "'" + Model.CorrelationId + "'" : "null" %>;
    // filter Information events
    var information;

    $(function() {

      information = $("#includeInformation").prop("checked");

      $("#includeInformation").change(function() {
        information = $("#includeInformation").prop("checked");
        loadDropDowns();
      }); 

      loadDropDowns();

      // if a specific event id is called then hide the controls
      if (pEventId != null) {
        $("#dateRange").hide();
        $("#applications").hide();
        $("#hosts").hide();
      } else
        window.setInterval(renderEventSummaryGrid, 60000);

      window.setInterval(function() {
          renderCharts();
        },
        60000);

      // clicks events for filters
      if (correlationid == null) {
        $("#dateRange").bind('change',
          function() {
            if (this.value === "Last60Minutes" || this.value === "Last24Hours") {
              $("#hours").show();
              $("#hours").bind('change',
                function() {
                  console.log("#hours change firing");
                  $("#severity option").remove();
                  $.getJSON('/Operations/EventLog/GetSeverity',
                    {
                      dateRange: $("#dateRange").val(),
                      hour: $("#hours").val(),
                      eventType: $("#eventType").val(),
                      correlationId: correlationid,
                      information: information
                    }).done(function(data) {
                    $("#severity").append("<option value='' selected=\"selected\">- All Severity ------</option>");
                    $.each(data,
                      function(i, o) {
                        $("#severity").append("<option value=" +
                          o.Description +
                          ">" +
                          o.Description +
                          " (" +
                          o.Cnt +
                          ")" +
                          "</option>");
                      });
                    $("#applications option").remove();
                    $.getJSON('/Operations/EventLog/GetApplications',
                      {
                        dateRange: $("#dateRange").val(),
                        hour: $("#hours").val(),
                        eventType: $("#eventType").val(),
                        severity: $("#severity").val(),
                          correlationId: correlationid,
                        information: information
                      }).done(function(data) {
                      $("#applications").append("<option value=''>- All Applications ------</option>");
                      $.each(data,
                        function(i, o) {
                          $("#applications").append("<option value=" +
                            o.Description +
                            ">" +
                            o.Description +
                            " (" +
                            o.Cnt +
                            ")" +
                            "</option>");
                        });
                      $("#hosts option").remove();
                      $.getJSON('/Operations/EventLog/GetHosts',
                        {
                          dateRange: $("#dateRange").val(),
                          hour: $("#hours").val(),
                          eventType: $("#eventType").val(),
                          severity: $("#severity").val(),
                          application: $("#applications").val(),
                            correlationId: correlationid,
                          information: information
                        }).done(function(data) {
                        $("#hosts").append("<option value=''>- All Hosts ------</option>");
                        $.each(data,
                          function(i, o) {
                            $("#hosts").append("<option value=" +
                              o.Description +
                              ">" +
                              o.Description +
                              " (" +
                              o.Cnt +
                              ")" +
                              "</option>");
                          });
                        renderEventSummaryGrid();
                      });
                    });
                  });
                });
            } else {
              $("#hours").hide();
              $("#hours").unbind("click");
            }
            loadDropDowns();
          });
      } else {
        $("#dateRange").hide();
        $("#hours").hide();
      }

      $("#eventType").bind('change',
        function() {
          $("#hours option").remove();
          $.getJSON('/Operations/EventLog/GetHours', { eventType: $("#eventType").val() }).done(function(data) {
            $("#hours").append("<option value='' selected=\"selected\">- All Hours ------</option>");
            $.each(data,
              function(i, o) {
                $("#hours").append("<option value='" + o.Hour + "'>" + o.Hour + " (" + o.Cnt + ")" + "</option>");
              });
            $("#severity option").remove();
            $.getJSON('/Operations/EventLog/GetSeverity',
              {
                dateRange: $("#dateRange").val(),
                hour: $("#hours").val(),
                eventType: $("#eventType").val(),
                correlationId: correlationid,
                information: information
              }).done(function(data) {
              $("#severity").append("<option value='' selected=\"selected\">- All Severity ------</option>");
              $.each(data,
                function(i, o) {
                  $("#severity").append("<option value=" +
                    o.Description +
                    ">" +
                    o.Description +
                    " (" +
                    o.Cnt +
                    ")" +
                    "</option>");
                });
              $("#applications option").remove();
              $.getJSON('/Operations/EventLog/GetApplications',
                {
                  dateRange: $("#dateRange").val(),
                  hour: $("#hours").val(),
                  eventType: $("#eventType").val(),
                  severity: $("#severity").val(),
                    correlationId: correlationid,
                  information: information
                }).done(function(data) {
                $("#applications").append("<option value=''>- All Applications ------</option>");
                $.each(data,
                  function(i, o) {
                    $("#applications").append("<option value=" +
                      o.Description +
                      ">" +
                      o.Description +
                      " (" +
                      o.Cnt +
                      ")" +
                      "</option>");
                  });
                $("#hosts option").remove();
                $.getJSON('/Operations/EventLog/GetHosts',
                  {
                    dateRange: $("#dateRange").val(),
                    hour: $("#hours").val(),
                    eventType: $("#eventType").val(),
                    severity: $("#severity").val(),
                    application: $("#applications").val(),
                      correlationId: correlationid,
                    information: information
                  }).done(function(data) {
                  $("#hosts").append("<option value=''>- All Hosts ------</option>");
                  $.each(data,
                    function(i, o) {
                      $("#hosts").append("<option value=" +
                        o.Description +
                        ">" +
                        o.Description +
                        " (" +
                        o.Cnt +
                        ")" +
                        "</option>");
                    });
                  renderEventSummaryGrid();
                });
              });
            });
          });
        });
      $("#hours").bind('change',
        function() {
          $("#severity option").remove();
          $.getJSON('/Operations/EventLog/GetSeverity',
            {
              dateRange: $("#dateRange").val(),
              hour: $("#hours").val(),
              eventType: $("#eventType").val(),
              correlationId: correlationid,
              information: information
            }).done(function(data) {
            $("#severity").append("<option value='' selected=\"selected\">- All Severity ------</option>");
            $.each(data,
              function(i, o) {
                $("#severity").append("<option value=" +
                  o.Description +
                  ">" +
                  o.Description +
                  " (" +
                  o.Cnt +
                  ")" +
                  "</option>");
              });
            $("#applications option").remove();
            $.getJSON('/Operations/EventLog/GetApplications',
              {
                dateRange: $("#dateRange").val(),
                hour: $("#hours").val(),
                eventType: $("#eventType").val(),
                severity: $("#severity").val(),
                  correlationId: correlationid,
                information: information
              }).done(function(data) {
              $("#applications").append("<option value=''>- All Applications ------</option>");
              $.each(data,
                function(i, o) {
                  $("#applications").append("<option value=" +
                    o.Description +
                    ">" +
                    o.Description +
                    " (" +
                    o.Cnt +
                    ")" +
                    "</option>");
                });
              $("#hosts option").remove();
              $.getJSON('/Operations/EventLog/GetHosts',
                {
                  dateRange: $("#dateRange").val(),
                  hour: $("#hours").val(),
                  eventType: $("#eventType").val(),
                  severity: $("#severity").val(),
                  application: $("#applications").val(),
                    correlationId: correlationid,
                  information: information
                }).done(function(data) {
                $("#hosts").append("<option value=''>- All Hosts ------</option>");
                $.each(data,
                  function(i, o) {
                    $("#hosts").append("<option value=" +
                      o.Description +
                      ">" +
                      o.Description +
                      " (" +
                      o.Cnt +
                      ")" +
                      "</option>");
                  });
                renderEventSummaryGrid();
              });
            });
          });
        });
      $("#severity").bind('change',
        function() {
          $("#applications option").remove();
          $.getJSON('/Operations/EventLog/GetApplications',
            {
              dateRange: $("#dateRange").val(),
              hour: $("#hours").val(),
              eventType: $("#eventType").val(),
              severity: $("#severity").val(),
              correlationId: correlationid,
              information: information
            }).done(function(data) {
            $("#applications").append("<option value=''>- All Applications ------</option>");
            $.each(data,
              function(i, o) {
                $("#applications").append("<option value=" +
                  o.Description +
                  ">" +
                  o.Description +
                  " (" +
                  o.Cnt +
                  ")" +
                  "</option>");
              });
            $("#hosts option").remove();
            $.getJSON('/Operations/EventLog/GetHosts',
              {
                dateRange: $("#dateRange").val(),
                hour: $("#hours").val(),
                eventType: $("#eventType").val(),
                severity: $("#severity").val(),
                application: $("#applications").val(),
                  correlationId: correlationid,
                information: information
              }).done(function(data) {
              $("#hosts").append("<option value=''>- All Hosts ------</option>");
              $.each(data,
                function(i, o) {
                  $("#hosts").append("<option value=" +
                    o.Description +
                    ">" +
                    o.Description +
                    " (" +
                    o.Cnt +
                    ")" +
                    "</option>");
                });
              renderEventSummaryGrid();
            });
          });
        });
      $("#applications").bind('change',
        function() {
          $("#hosts option").remove();
          $.getJSON('/Operations/EventLog/GetHosts',
            {
              dateRange: $("#dateRange").val(),
              hour: $("#hours").val(),
              eventType: $("#eventType").val(),
              severity: $("#severity").val(),
              application: $("#applications").val(),
              correlationId: correlationid,
              information: information
            }).done(function(data) {
            $("#hosts").append("<option value=''>- All Hosts ------</option>");
            $.each(data,
              function(i, o) {
                $("#hosts").append("<option value=" +
                  o.Description +
                  ">" +
                  o.Description +
                  " (" +
                  o.Cnt +
                  ")" +
                  "</option>");
              });
            renderEventSummaryGrid();
          });
        });
      $("#hosts").bind('change',
        function() {
          renderEventSummaryGrid();
        });

      $("#adminEmails").bind('change',
        function() {
          if ($("#adminEmails").val() != '') {
            $("#email").val($("#adminEmails").val());
            $("#message").focus();
          }
        });

      if (correlationid == null) {
        createChartLast24Hours();
        createChartLast30Minutes();
      }
    });

    function renderAll() {
      renderEventSummaryGrid();
      renderCharts();
    }

    function renderCharts() {
      var chartLast30 = $("#chartLast30").data("kendoChart");
      chartLast30.dataSource.read();
      chartLast30.refresh();
      var chartLast24 = $("#chartLast24").data("kendoChart");
      chartLast24.dataSource.read();
      chartLast24.refresh();
    }

    function loadDropDowns() {
      $("#eventType option").remove();
      $.getJSON('/Operations/EventLog/GetEventTypes',
        { dateRange: $("#dateRange").val(), correlationId: correlationid, information: information }).done(
        function(data) {
          $("#eventType").append("<option value='' selected=\"selected\">- All Types ------</option>");
          $.each(data,
            function(i, o) {
              $("#eventType").append("<option value='" +
                o.Description +
                "'>" +
                o.Description +
                " (" +
                o.Cnt +
                ")" +
                "</option>");
            });
          $("#hours option").remove();
          $.getJSON('/Operations/EventLog/GetHours', { eventType: $("#eventType").val(), information: information })
            .done(function(data) {
              $("#hours").append("<option value='' selected=\"selected\">- All Hours ------</option>");
              $.each(data,
                function(i, o) {
                  $("#hours").append("<option value='" + o.Hour + "'>" + o.Hour + " (" + o.Cnt + ")" + "</option>");
                });
              $("#severity option").remove();
              $.getJSON('/Operations/EventLog/GetSeverity',
                {
                  dateRange: $("#dateRange").val(),
                  hour: $("#hours").val(),
                  eventType: $("#eventType").val(),
                  correlationId: correlationid,
                  information: information
                }).done(function(data) {
                $("#severity").append("<option value='' selected=\"selected\">- All Severity ------</option>");
                $.each(data,
                  function(i, o) {
                    $("#severity").append("<option value='" +
                      o.Description +
                      "'>" +
                      o.Description +
                      " (" +
                      o.Cnt +
                      ")" +
                      "</option>");
                  });
                $("#applications option").remove();
                $.getJSON('/Operations/EventLog/GetApplications',
                  {
                    dateRange: $("#dateRange").val(),
                    hour: $("#hours").val(),
                    eventType: $("#eventType").val(),
                    severity: $("#severity").val(),
                    correlationId: correlationid,
                    information: information
                  }).done(function(data) {
                  $("#applications").append("<option value=''>- All Applications ------</option>");
                  $.each(data,
                    function(i, o) {
                      $("#applications").append("<option value='" +
                        o.Description +
                        "'>" +
                        o.Description +
                        " (" +
                        o.Cnt +
                        ")" +
                        "</option>");
                    });
                  $("#hosts option").remove();
                  $.getJSON('/Operations/EventLog/GetHosts',
                    {
                      dateRange: $("#dateRange").val(),
                      hour: $("#hours").val(),
                      eventType: $("#eventType").val(),
                      severity: $("#severity").val(),
                      application: $("#applications").val(),
                      correlationId: correlationid,
                      information: information
                    }).done(function(data) {
                    $("#hosts").append("<option value=''>- All Hosts ------</option>");
                    $.each(data,
                      function(i, o) {
                        $("#hosts").append("<option value='" +
                          o.Description +
                          "'>" +
                          o.Description +
                          " (" +
                          o.Cnt +
                          ")" +
                          "</option>");
                      });
                    renderAll();
                  });
                });
              });
            });
        });
    }

    function createChartLast30Minutes() {
      $("#chartLast30").kendoChart({
        chartArea: {
          height: 200
        },
        dataSource: {
          transport: {
            read: function(options) {
              var data = {
                application: $('#applications').val(),
                host: $('#hosts').val(),
                severity: $("#severity").val(),
                correlationId: correlationid
              };
              $.ajax({
                url: "/Operations/EventLog/GetGraphForLast30Minutes",
                dataType: 'json',
                type: 'GET',
                data: data,
                success: function(result) {
                  options.success(result);
                }
              });
            }
          }
        },
        legend: {
          visible: false
        },
        series: [
          {
            type: "line",
            style: "step",
            field: "Cnt",
            markers: {
              visible: false
            }
          }
        ],
        valueAxis: {
          //type: "log",
          labels: {
            format: "N0"
          },
          title: {
            text: "Errors - Last 30 min"
          }
        },
        categoryAxis: {
          field: "Minute",
          visible: false
        },
        tooltip: {
          visible: true,
          format: "{0}",
          template: "<span style='color: white;'>Errors:#= value #</span>"
        }
      });
    }

    function createChartLast24Hours() {
      $("#chartLast24").kendoChart({
        chartArea: {
          height: 200
        },
        dataSource: {
          transport: {
            read: function(options) {
              var data = {
                application: $('#applications').val(),
                host: $('#hosts').val(),
                severity: $("#severity").val(),
                correlationId: correlationid
              };
              $.ajax({
                url: "/Operations/EventLog/GetGraphForLast24Hours",
                dataType: 'json',
                type: 'GET',
                data: data,
                success: function(result) {
                  options.success(result);
                }
              });
            }
          }
        },
        legend: {
          visible: false
        },
        series: [
          {
            type: "line",
            style: "step",
            field: "Cnt",
            markers: {
              visible: false
            }
          }
        ],
        valueAxis: {
          //type: "log",
          labels: {
            format: "N0"
          },
          title: {
            text: "Errors - Last 24 Hours"
          }
        },
        categoryAxis: {
          labels: {
            rotation: -45,
          },
          field: "Hour",
          visible: false
        },
        seriesClick: function(e) {
          // if error count > 0 then set event type to Error
          if (e.value > 0) {
            $('#eventType').val('Error');
            $("#hours option").remove();
            $.getJSON('/Operations/EventLog/GetHours', { eventType: $("#eventType").val(), information: information }).done(function(data) {
              $("#hours").append("<option value=''>- All Hours ------</option>");
              $.each(data,
                function(i, o) {
                  $("#hours").append("<option value='" + o.Hour + "'>" + o.Hour + " (" + o.Cnt + ")" + "</option>");
                });
              console.log("updating #hours to " + e.category);
              $('#hours').val(e.category);
              $("#severity option").remove();
              $.getJSON('/Operations/EventLog/GetSeverity',
                {
                  dateRange: $("#dateRange").val(),
                  hour: $("#hours").val(),
                  eventType: $("#eventType").val(),
                    correlationId: correlationid,
                  information: information
                }).done(function(data) {
                $("#severity").append("<option value='' selected=\"selected\">- All Severity ------</option>");
                $.each(data,
                  function(i, o) {
                    $("#severity").append("<option value=" +
                      o.Description +
                      ">" +
                      o.Description +
                      " (" +
                      o.Cnt +
                      ")" +
                      "</option>");
                  });
                $("#applications option").remove();
                $.getJSON('/Operations/EventLog/GetApplications',
                  {
                    dateRange: $("#dateRange").val(),
                    hour: $("#hours").val(),
                    eventType: $("#eventType").val(),
                    severity: $("#severity").val(),
                      correlationId: correlationid,
                    information: information
                  }).done(function(data) {
                  $("#applications").append("<option value=''>- All Applications ------</option>");
                  $.each(data,
                    function(i, o) {
                      $("#applications").append("<option value=" +
                        o.Description +
                        ">" +
                        o.Description +
                        " (" +
                        o.Cnt +
                        ")" +
                        "</option>");
                    });
                  $("#hosts option").remove();
                  $.getJSON('/Operations/EventLog/GetHosts',
                    {
                      dateRange: $("#dateRange").val(),
                      hour: $("#hours").val(),
                      eventType: $("#eventType").val(),
                      severity: $("#severity").val(),
                      application: $("#applications").val(),
                        correlationId: correlationid,
                      information: information
                    }).done(function(data) {
                    $("#hosts").append("<option value=''>- All Hosts ------</option>");
                    $.each(data,
                      function(i, o) {
                        $("#hosts").append("<option value=" +
                          o.Description +
                          ">" +
                          o.Description +
                          " (" +
                          o.Cnt +
                          ")" +
                          "</option>");
                      });
                    renderEventSummaryGrid();
                  });
                });
              });
            });
          }
        },
        yAxis: {
          axisCrossingValue: [0]
        },
        tooltip: {
          visible: true,
          format: "{0}",
          template: "<span style='color: white;'>Errors:#= value #</span>"
        }
      });
    }

    function reset() {
      history.pushState(null, "EventLog", "/Operations/EventLog/Index");
      window.location.replace("/Operations/EventLog/Index");
    }

    function openEmailModal(id) {
      $("#notification-body .alert").remove();
      $("#adminEmails").val('');
      $("#eventid").val(id);
      $("#subject").val('Please take a look at EventId: ' + id);
      $("#email").val('');
      $("#message").val('https://admin.accurateappend.com/Operations/EventLog/Index?eventid=' + id);
      $("#email-modal").modal('show');
    }

    function sendMessage() {
      $("#notification-body .alert").remove();
      if ($("#email").val() == '') {
        $("#notification-body").prepend("<div class=\"alert alert-danger\">Email address required</div>");
        return false;
      }
      if ($("#subject").val() == '') {
        $("#notification-body").prepend("<div class=\"alert alert-danger\">Subject required</div>");
        return false;
      }
      if ($("#message").val() == '') {
        $("#notification-body").prepend("<div class=\"alert alert-danger\">Message required</div>");
        return false;
      }
      $.ajax({
        url: "/Operations/Email/Send",
        dataType: 'json',
        type: 'POST',
        data: {
          to: $("#email").val(),
          from: "<%: User.Identity.Name %>",
          subject: $("#subject").val(),
          body: $("#message").val(),
        },
        success: function() {
          $("#notification-body")
            .prepend(
              "<div class=\"alert alert-success\">Email successfuly sent. Window will close in 5 seconds.</div>");
          sleep(50000);
          $("#email-modal").modal('hide');
        },
        error: function() {
          $("#notification-body").prepend("<div class=\"alert alert-danger\">Error. " + result + ".</div>");
        }
      });
    }

    function renderEventSummaryGrid() {

      var expandedRow;
      var grid = $('#grid').data('kendoGrid');
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#grid").kendoGrid({
          dataSource: {
            type: "json",
            serverPaging: true,
            serverSorting: true,
            pageSize: 25,
            transport: {
              read: function(options) {
                options.data.dateRange = $('#dateRange').val();
                options.data.correlationId = correlationid;
                options.data.application = $('#applications').val();
                options.data.host = $('#hosts').val();
                options.data.eventid = pEventId;
                options.data.severity = $('#severity').val();
                options.data.hour = $('#hours').val();
                options.data.eventType = $("#eventType").val();
                options.data.information = information;

                //var data = { page: 2, dateRange: $('#dateRange').val(), correlationId: correlationid, application: $('#applications').val(), host: $('#hosts').val(), eventid: pEventId, severity: $('#severity').val(), hour: $('#hours').val(), eventType: $("#eventType").val() };
                $.ajax({
                  url: "/Operations/EventLog/EventSumamry_Read",
                  dataType: 'json',
                  type: 'GET',
                  data: options.data,
                  success: function(result) {
                    options.success(result);
                  }
                });
              }
            },
            schema: {
              type: 'json',
              data: "Data",
              total: "Total"
            },
            change: function() {
              if (this.data().length <= 0) {
                $("#gridInfo").text('No events found').show();
                $("#grid").hide();
                //$("#controls").hide();
              } else {
                $("#gridInfo").hide();
                $("#grid").show();
                //$("#controls").show();
              }
            }
          },
          scrollable: false,
          sortable: true,
          pageable: true,
          detailTemplate: kendo.template($("#detail-template").html()),
          filterable: {
            extra: false,
            operators: {
              string: {
                eq: "Is equal to",
                neq: "Is not equal to"
              }
            }
          },
          columns: [
            {
              field: "id",
              title: "Id",
              filterable: false,
              template: kendo.template($("#command-template").html()),
              headerAttributes: { style: "text-align: center;" },
              attributes: { style: "text-align: center;" }
            },
            { field: "EventDate", title: "Date", filterable: false, width: 175 },
            { field: "Application", title: "Application", filterable: false },
            { field: "IP", title: "IP", filterable: true },
            { field: "Host", title: "Host", filterable: false },
            { field: "Severity", title: "Severity", filterable: false },
            { field: "EventType", title: "EventType", filterable: false },
            { field: "Message", title: "Message", filterable: true }
          ],
          dataBound: function() {
            if (this.dataSource.total() == 1) {
              var grid = $('#grid').data('kendoGrid');
              grid.expandRow($('#grid tbody > tr:first'));
            }
            var grid = $('#grid').data('kendoGrid');
            // expand detail row that was previosuly expanded before datasource was refreshed
            var state = sessionStorage.getItem("eventgrid");
            if (state) {
              state = JSON.parse(state);
              for (var id in state) {
                var dataItem =
                  grid.dataSource.get(id); // "get" method requires model to be set in dataSourc with field name "id"
                if (dataItem != null)
                  grid.expandRow("tr[data-uid=" + dataItem.uid + "]");
              }
            }
          },
          detailExpand: function(e) {
            // collapse all expanded rows before expanding this one
            if (expandedRow != null && expandedRow[0] != e.masterRow[0]) {
              var grid = $('#grid').data('kendoGrid');
              grid.collapseRow(expandedRow);
            }
            expandedRow = e.masterRow;
            // save grid state so expanded rows don't collapse when the dataSource is refreshed
            var state = sessionStorage.getItem("eventgrid");
            if (!state) {
              state = {};
            } else {
              state = JSON.parse(state);
            }
            state[this.dataItem(e.masterRow).id] = true;
            sessionStorage.setItem("eventgrid", JSON.stringify(state));
          },
          detailCollapse: function(e) {
            // save grid state so expanded rows don't collapse when the dataSource is refreshed
            var state = sessionStorage.getItem("eventgrid");
            if (state) {
              state = JSON.parse(state);
              delete state[this.dataItem(e.masterRow).id];
              sessionStorage.setItem("eventgrid", JSON.stringify(state));
            }
          }
        });
      }
    }

    function sleep(milliseconds) {
      var start = new Date().getTime();
      for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
          break;
        }
      }
    }

  </script>

</asp:Content>