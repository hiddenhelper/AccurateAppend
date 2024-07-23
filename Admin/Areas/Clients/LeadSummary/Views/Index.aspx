<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail" %>
<%@ Import Namespace="AccurateAppend.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Leads
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div>
    <div class="row" style="padding-right: 20px;">
      <div class="pull-right">
        <%: Html.SiteDropDown(null) %>
      </div>
    </div>

    <div class="row" style="padding: 0 20px 0 20px;">

      <ul class="nav nav-tabs" id="tabs">
        <li class="active">
          <a href="#leads" data-toggle="tab">Leads</a>
        </li>
        <li>
          <a href="#" id="zendeskLeads" data-toggle="tab">Zendesk Sell Leads</a>
        </li>
        <li>
          <a href="#apiTrialActivity" data-toggle="tab">API Trials</a>
        </li>
      </ul>

      <div class="tab-content">

        <div class="tab-pane active" id="leads" style="margin-top: 20px;">

          <div class="row">

            <div class="col-md-1">
              <a href="<%= Url.BuildFor<LeadDetailController>().Create() %>" class="btn btn-default"><span class="fa fa-plus"></span>Add Lead</a>
            </div>
            <div class="col-md-11" style="text-align: right;">
              <span style="margin-right: 7px;">Display Only Un-worked Leads</span><input type="checkbox" checked data-toggle="toggle" data-size="small" id="displayUnworkedLeads"/>
              <button type="button" class="btn btn-default" id="refresh" onclick="javascript:reset();"><span class="fa fa-refresh"></span>Reset</button>
              <a class="btn btn-default" onclick="javascript:downloadLeads();"><span class="fa fa-download"></span>Download</a>
              <%= Html.Kendo().DropDownList()
                    .Name("dateRange")
                    .DataTextField("Text")
                    .DataValueField("Value")
                    .BindTo(EnumExtensions.ToLookup<DateRange>().Select(a => new SelectListItem {Text = a.Key, Value = ((DateRange) a.Value).ToString()}))
                    .Value(DateRange.Last30Days.ToString())
  %>
              <span id="datePickers" style="display: none;">
                <input id="startdate" style="width: 150px;"/>
                <label for="startdate">&nbsp;thru&nbsp;</label>
                <input id="enddate" style="width: 150px;"/>
              </span>
              <select id="source" style="display: inline; width: 150px;" class="form-control"></select>
              <select id="status" style="display: inline; width: 150px;" class="form-control"></select>
              <select id="qualified" style="display: inline; width: 150px;" class="form-control"></select>
            </div>
          </div>
          <div class="alert alert-info" style="display: none; margin: 20px 0 20px 0;" id="leadSummaryGridMessage"></div>
          <div id="grid" style="margin-bottom: 20px; margin-top: 10px;"></div>

        </div>

        <div class="tab-pane" id="apiTrialActivity" style="margin-top: 20px;">

          <div class="row">

            <div class="col-md-12" style="text-align: right;">
              <%= Html.Kendo().DropDownList()
                    .Name("apiTrialActivityDateRange")
                    .DataTextField("Text")
                    .DataValueField("Value")
                    .BindTo(EnumExtensions.ToLookup<DateRange>().Select(a => new SelectListItem {Text = a.Key, Value = ((DateRange) a.Value).ToString()}))
                    .Value("Last30Days")
  %>
              <span id="apiTrialActivityDatePickers" style="display: none;">
                <input id="apiTrialActivityStartdate" style="width: 150px;"/>
                <label>&nbsp;thru&nbsp;</label>
                <input id="apiTrialActivityEnddate" style="width: 150px;"/>
              </span>
            </div>

          </div>
          <div class="alert alert-info" style="display: none; margin: 20px 0 20px 0;" id="apiTrialActivityGridMessage"></div>
          <div id="apiTrialActivityGrid" style="margin-bottom: 20px; margin-top: 10px;"></div>

        </div>

      </div>

    </div>

  </div>

  <script id="responsive-column-template-lead-summary" type="text/x-kendo-template">
  <strong>Received</strong>
  <p class="col-template-val">#=data.LastUpdateDescription#</p>
    
  <strong>Id</strong>
  <p class="col-template-val">#=data.LeadId#</p>

  <strong>Name</strong>
  <p class="col-template-val">#=data.CompositeName#</p>

  <strong>Phone</strong>
  <p class="col-template-val">#=data.Phone#</p>
    
  <strong>Email</strong>
  <p class="col-template-val">#=data.Email#</p>

  <strong>Status</strong>
  <p class="col-template-val">#=data.LeadStatusDescription#</p>
  
  <strong>Qualified</strong>
  <p class="col-template-val">#=data.QualifiedDescription#</p>
    
  <strong>Source</strong>
  <p class="col-template-val">#=data.LeadSource#</p>
    
  <strong>ContactMethod</strong>
  <p class="col-template-val">#=data.ContactMethod#</p>

  <strong>Note Count</strong>
  <p class="col-template-val">#=data.NoteCount#</p>
    
  <strong>Score</strong>
  <p class="col-template-val">#=data.Score#</p>
  
  <p><a class="k-button k-button-icontext k-grid" href="#=data.DetailUrl#">View Details</a></p>
  
  </script>

  <script id="responsive-column-template-api-summary" type="text/x-kendo-template">
  <strong>Name</strong>
  <p class="col-template-val">#=data.FirstName# #=data.LastName#</p>
    
  <strong>Email</strong>
  <p class="col-template-val">#=data.Email#</p>
    
  <strong>Phone</strong>
  <p class="col-template-val">#=data.Phone#</p>

  <strong>Date Created</strong>
  <p class="col-template-val">#=data.DateCreated#</p>
  
  <strong>Is Enabled</strong>
  <p class="col-template-val">#=data.IsEnabled#</p>
    
  <strong>Score</strong>
  <p class="col-template-val">#=data.Score#</p>
  
  <p><a class="k-button k-button-icontext k-grid" href="#=data.DetailUrl#">View Details</a></p>
  
  </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script src="<%= Url.Content("~/Scripts/moment.min.js") %>" type="text/javascript"> </script>
  <!-- http://www.bootstraptoggle.com/ -->
  <link href="//gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
  <script src="//gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>

  <script type="text/javascript">

    var apiActivityView;

    // http://www.kendoui.com/code-library/web/grid/preserve-grid-state-in-a-cookie.aspx

    $(function() {

      var start = $("#startdate").kendoDatePicker({
        format: "yyyy-MM-dd"
      });
      var end = $("#enddate").kendoDatePicker({
        format: "yyyy-MM-dd"
      });
      var apiTrialActivitystart = $("#apiTrialActivityStartdate").kendoDatePicker({
        format: "yyyy-MM-dd"
      });
      var apiTrialActivityend = $("#apiTrialActivityEnddate").kendoDatePicker({
        format: "yyyy-MM-dd"
      });

      loadApplicationId();
      syncDatePickersWithDateRange();
      apiActivityView.apiTrialActivitySyncDatePickersWithDateRange();
      apiActivityView.renderApiTrialGrid();
      updateDateRangeDropDown();
      updateAllFilterSelects();

      $("#dateRange").change(function() {
        if ($("#dateRange").val() === "Custom") {
          $("#datePickers").show();
        } else {
          $("#datePickers").hide();
        }
        syncDatePickersWithDateRange();
        updateAllFilterSelects();
      });
      $("#apiTrialActivityDateRange").change(function() {
        if ($("#apiTrialActivityDateRange").val() === "Custom") {
          $("#apiTrialActivityDatePickers").show();
        } else {
          $("#apiTrialActivityDatePickers").hide();
        }
        apiActivityView.apiTrialActivitySyncDatePickersWithDateRange();
        apiActivityView.renderApiTrialGrid();
      });
      $("#source").change(function() {
        $("#status option").remove();
        $.getJSON('/Clients/LeadSummary/GetLeadStatuses?startdate=' +
          moment($("#startdate").val()).format("YYYY-MM-DD") +
          '&enddate=' +
          moment($("#enddate").val()).format("YYYY-MM-DD") +
          '&applicationid=' +
          $("#ApplicationId").val() +
          '&source=' +
          $("#source").val(),
          function(data) {
            $("#status").append("<option value=''>- All Status ------</option>");
            $.each(data,
              function(index, status) {
                $("#status").append("<option value=" +
                  status.Description +
                  ">" +
                  status.Description +
                  " (" +
                  status.Cnt +
                  ")" +
                  "</option>");
              });
            $("#qualified option").remove();
            $.getJSON('/Clients/LeadSummary/GetLeadQualificationStatuses?startdate=' +
              moment($("#startdate").val()).format("YYYY-MM-DD") +
              '&enddate=' +
              moment($("#enddate").val()).format("YYYY-MM-DD") +
              '&applicationid=' +
              $("#ApplicationId").val() +
              '&source=' +
              $("#source").val(),
              function(data) {
                $("#qualified").append("<option value=''>- All Qualification ------</option>");
                $.each(data,
                  function(index, status) {
                    $("#qualified").append("<option value=" +
                      status.Description +
                      ">" +
                      status.Description +
                      " (" +
                      status.Cnt +
                      ")" +
                      "</option>");
                  });
                renderLeadSummaryGrid();
              });
          });
      });
      $("#status").change(function() {
        $("#qualified option").remove();
        $.getJSON('/Clients/LeadSummary/GetLeadQualificationStatuses?startdate=' +
          moment($("#startdate").val()).format("YYYY-MM-DD") +
          '&enddate=' +
          moment($("#enddate").val()).format("YYYY-MM-DD") +
          '&applicationid=' +
          $("#ApplicationId").val() +
          '&source=' +
          $("#source").val() +
          '&status=' +
          $("#status").val(),
          function(data) {
            $("#qualified").append("<option value=''>- All Qualification ------</option>");
            $.each(data,
              function(index, status) {
                $("#qualified").append("<option value=" +
                  status.Description +
                  ">" +
                  status.Description +
                  " (" +
                  status.Cnt +
                  ")" +
                  "</option>");
              });
            renderLeadSummaryGrid();
          });
      });
      $("#qualified").change(function() {
        renderLeadSummaryGrid();
      });

      $("#ApplicationId").bind('change',
        function() {
          setApplicationId();
          updateAllFilterSelects();
          apiActivityView.renderApiTrialGrid();
          if ($('ul#tabs li.active [href$="apiTrialActivity"]').length !== 0) {
            apiActivityView.renderApiTrialGrid();
          }
        });
      start.bind("change",
        function() {
          renderLeadSummaryGrid();
        });
      end.bind("change",
        function() {
          renderLeadSummaryGrid();
        });
      apiTrialActivitystart.bind("change",
        function() {
          apiActivityView.renderApiTrialGrid();
        });
      apiTrialActivityend.bind("change",
        function() {
          apiActivityView.renderApiTrialGrid();
        });

      $("#displayUnworkedLeads").change(function() {
        renderLeadSummaryGrid();
        });

      $("#zendeskLeads").click(function() {
        window.open("https://app.futuresimple.com/working/leads");
      });

      window.setInterval(renderLeadSummaryGrid, 60000);
      window.setInterval(apiActivityView.renderApiTrialGrid, 60000);
    });

    function updateAllFilterSelects() {
      $("#source option").remove();
      $.getJSON('/Clients/LeadSummary/GetLeadSources?startdate=' +
        moment($("#startdate").val()).format("YYYY-MM-DD") +
        '&enddate=' +
        moment($("#enddate").val()).format("YYYY-MM-DD") +
        '&applicationid=' +
        $("#ApplicationId").val(),
        function(data) {
          $("#source").append("<option value=''>- All Sources ------</option>");
          $.each(data,
            function(index, status) {
              $("#source").append("<option value=" +
                status.Description +
                ">" +
                status.Description +
                " (" +
                status.Cnt +
                ")" +
                "</option>");
            });
          $("#status option").remove();
          $.getJSON('/Clients/LeadSummary/GetLeadStatuses?startdate=' +
            moment($("#startdate").val()).format("YYYY-MM-DD") +
            '&enddate=' +
            moment($("#enddate").val()).format("YYYY-MM-DD") +
            '&applicationid=' +
            $("#ApplicationId").val() +
            '&source=' +
            $("#source").val(),
            function(data) {
              $("#status").append("<option value=''>- All Status ------</option>");
              $.each(data,
                function(index, status) {
                  $("#status").append("<option value=" +
                    status.Description +
                    ">" +
                    status.Description +
                    " (" +
                    status.Cnt +
                    ")" +
                    "</option>");
                });
              $("#qualified option").remove();
              $.getJSON('/Clients/LeadSummary/GetLeadQualificationStatuses?startdate=' +
                moment($("#startdate").val()).format("YYYY-MM-DD") +
                '&enddate=' +
                moment($("#enddate").val()).format("YYYY-MM-DD") +
                '&applicationid=' +
                $("#ApplicationId").val() +
                '&source=' +
                $("#source").val() +
                '&status=' +
                $("#status").val(),
                function(data) {
                  $("#qualified").append("<option value=''>- All Qualification ------</option>");
                  $.each(data,
                    function(index, status) {
                      $("#qualified").append("<option value=" +
                        status.Description +
                        ">" +
                        status.Description +
                        " (" +
                        status.Cnt +
                        ")" +
                        "</option>");
                    });
                  renderLeadSummaryGrid();
                });
            });
        });
    }

    function syncDatePickersWithDateRange() {
      $("#datePickers").hide();
      switch ($("#dateRange").val()) {
      case "Today":
        $("#startdate").val(moment().subtract('days', 1).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last7Days":
        $("#startdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "ThisMonth":
        $("#startdate").val(moment().startOf('month').local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "LastMonth":
        $("#startdate").val(moment().subtract('months', 1).startOf('month').local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().subtract('months', 1).endOf('month').local().format("YYYY-MM-DD"));
        break;
      case "Last30Days":
        $("#startdate").val(moment().subtract('days', 30).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last60Days":
        $("#startdate").val(moment().subtract('days', 60).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last90Days":
        $("#startdate").val(moment().subtract('days', 90).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "All":
        $("#startdate").val(moment().subtract('years', 1).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Custom":
        // display date pickers because we are going to allow the user to enter a custom date
        $("#datePickers").show();
        // set default to 1 week back
        $("#startdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
        $("#enddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      }
      $("#apiTrialActivityDatePickers").hide();
      switch ($("#apiTrialActivityDateRange").val()) {
      case "Today":
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 1).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last7Days":
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "ThisMonth":
        $("#apiTrialActivityStartdate").val(moment().startOf('month').local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "LastMonth":
        $("#apiTrialActivityStartdate")
          .val(moment().subtract('months', 1).startOf('month').local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().subtract('months', 1).endOf('month').local().format("YYYY-MM-DD"));
        break;
      case "Last30Days":
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 30).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last60Days":
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 60).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Last90Days":
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 90).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "All":
        $("#apiTrialActivityStartdate").val(moment().subtract('years', 1).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      case "Custom":
        // display date pickers because we are going to allow the user to enter a custom date
        $("#apiTrialActivityDatePickers").show();
        // set default to 1 week back
        $("#apiTrialActivityStartdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
        $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
        break;
      }
    }

    function reset() {
      $.cookie("leadsGridState", "");
      history.pushState(null, "Leads", "/Clients/LeadSummary");
      window.location.replace("/Clients/LeadSummary");
    }

    function downloadLeads() {
      window.location.replace("/Clients/LeadSummary/Download?applicationid=" +
        $("#ApplicationId").val() +
        "&startdate=" +
        moment($("#startdate").val()).format("YYYY-MM-DD") +
        "&enddate=" +
        moment($("#enddate").val()).format("YYYY-MM-DD"));
    }

    function updateDateRangeDropDown() {
      var value = $.cookie("leadDateRangeState");
      if (value) $("#dateRange").val(value);
    }

    function renderLeadSummaryGrid() {

      $.cookie("dateRangeState", $("#dateRange").val());
      var grid = $("#grid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#grid").kendoGrid({
          autobind: false,
          dataSource: {
            autobind: false,
            type: "json",
            transport: {
              read: function(options) {
                $.ajax({
                  url: "/Clients/LeadSummary/Query",
                  dataType: 'json',
                  type: 'GET',
                  data: {
                    applicationid: $("#ApplicationId").val(),
                    startdate: moment($("#startdate").val()).local().format("YYYY-MM-DD"),
                    enddate: moment($("#enddate").val()).local().format("YYYY-MM-DD"),
                    source: $("#source").val(),
                    status: $("#status").val(),
                    qualified: $("#qualified").val(),
                    displayUnworkedLeads: $("#displayUnworkedLeads").prop("checked")
                  },
                  success: function(result) {
                    options.success(result);
                  }
                });
              }
            },
            schema: {
              type: 'json',
              data: "Data",
              total: function(response) {
                return response.Data.length;
              }
            },
            pageSize: 20,
            change: function() {
              if (this.data().length <= 0) {
                $("#leadSummaryGridMessage").text("No Leads Found").show();
                $("#grid").hide();
              } else {
                $("#leadSummaryGridMessage").hide();
                $("#grid").show();
              }
            }
          },
          scrollable: false,
          sortable: true,
          pageable: {
            input: true,
            numeric: false
          },
          filterable: false,
          dataBound: function() {
            $.cookie("leadsGridState",
              kendo.stringify({
                page: this.dataSource.page(),
                pageSize: this.dataSource.pageSize(),
                sort: this.dataSource.sort(),
                group: this.dataSource.group(),
                filter: this.dataSource.filter()
              }));
          },
          columns: [
            { field: "LeadId", title: "Id", media: "(min-width: 450px)" },
            { field: "CompositeName", title: "Name", media: "(min-width: 450px)" },
            { field: "Owner.UserName", title: "Sales Rep", media: "(min-width: 450px)" },
            { field: "LeadStatusDescription", title: "Status", media: "(min-width: 450px)" },
            { field: "QualifiedDescription", title: "Qualified", media: "(min-width: 450px)" },
            //{ field: "LeadSource", title: "Source", media: "(min-width: 450px)" },
            { field: "Score", title: "Score", media: "(min-width: 450px)" },
            //{ field: "DetailUrl", title: "Age In Days", width: 75 },
            { field: "NoteCount", title: "Note Count", width: 75, media: "(min-width: 450px)" },
            { field: "LastUpdateDescription", title: "Received", media: "(min-width: 450px)" },
            {
              template:
                "<a class=\"k-button k-button-icontext k-grid-ViewDetails\" href=\"#=DetailUrl#\">View Details</a>",
              title: " ",
              width: "140px",
              media: "(min-width: 450px)"
            },
            {
              title: "",
              template: kendo.template($("#responsive-column-template-lead-summary").html()),
              media: "(max-width: 450px)"
            }
          ]
        });
      }

      var state = JSON.parse($.cookie("leadsGridState"));
      if (state) {
        $('#grid').data('kendoGrid').dataSource.query(state);
      } else {
        $('#grid').data('kendoGrid').dataSource.read();
      }
    }

    function viewDetails(e) {
      var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
      history.pushState(null, "Leads", "<%= Url.Action("Index", "LeadSummary", new {Area = "Clients"}) %>");
      window.location.replace(dataItem.DetailUrl);
    }

    function loadApplicationId() {
      var v = $.cookie('ApplicationId');
      if (v != '') {
        $('#ApplicationId option[value=' + $.cookie('ApplicationId') + ']').attr('selected', 'selected');
      }
    }

    function setApplicationId() {
      $.cookie('ApplicationId', $('#ApplicationId option:selected').val());
    }

    apiActivityView = {
      renderApiTrialGrid: function() {

        var grid = $("#apiTrialActivityGrid").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#apiTrialActivityGrid")
            .kendoGrid({
              autobind: false,
              dataSource: {
                autobind: false,
                type: "json",
                transport: {
                  read: function(options) {
                    $.ajax({
                      url: "/Clients/ApiTrialSummary/Query",
                      dataType: 'json',
                      type: 'GET',
                      data: {
                        applicationid: $("#ApplicationId").val(),
                        startdate: moment($("#apiTrialActivityStartdate").val())
                          .local()
                          .format("YYYY-MM-DD"),
                        enddate: moment($("#apiTrialActivityEnddate").val())
                          .local()
                          .format("YYYY-MM-DD")
                      },
                      success: function(result) {
                        options.success(result);
                      }
                    });
                  }
                },
                schema: {
                  type: 'json',
                  data: "Data",
                  total: function(response) {
                    return response.Data.length;
                  }
                },
                pageSize: 20,
                change: function() {
                  if (this.data().length <= 0) {
                    $("#apiTrialActivityGridMessage").text("No Trials Found").show();
                    $("#apiTrialActivityGrid").hide();
                  } else {
                    $("#apiTrialActivityGridMessage").hide();
                    $("#apiTrialActivityGrid").show();
                  }
                }
              },
              scrollable: false,
              sortable: true,
              pageable: {
                input: true,
                numeric: false
              },
              filterable: false,
              columns: [
                { field: "FirstName", title: "First Name", media: "(min-width: 450px)" },
                { field: "LastName", title: "Last Name", media: "(min-width: 450px)" },
                { field: "Email", title: "Email", media: "(min-width: 450px)" },
                { field: "Score", title: "Score", media: "(min-width: 450px)" },
                { field: "DateCreated", title: "Date", media: "(min-width: 450px)" },
                {
                  field: "IsEnabled",
                  title: "Is Enabled",
                  media: "(min-width: 450px)",
                  headerAttributes: { style: "text-align: center;" },
                    attributes: { style: "text-align: center;" },
                  template: kendo.template("#=IsEnabled ? 'Yes' : ''#")
                },
                {
                  field: "",
                  title: "",
                  filterable: false,
                  headerAttributes: { style: "text-align: center;" },
                  attributes: { style: "text-align: center;" },
                  template: kendo.template("<a class=\"btn btn-default\" href=\"#=DetailUrl#\">View Details</a>")
                },
                //{ command: { text: "View Details", click: viewDetails }, title: " ", width: "120px", media: "(min-width: 450px)" },
                {
                  title: "",
                  template: kendo.template($("#responsive-column-template-api-summary").html()),
                  media: "(max-width: 450px)"
                }
              ]
            });

        }

      },
      apiTrialActivitySyncDatePickersWithDateRange: function() {
        $("#apiTrialActivityDatePickers").hide();
        switch ($("#apiTrialActivityDateRange").val()) {
        case "Today":
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 1).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "Last7Days":
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "ThisMonth":
          $("#apiTrialActivityStartdate").val(moment().startOf('month').local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "LastMonth":
          $("#apiTrialActivityStartdate")
            .val(moment().subtract('months', 1).startOf('month').local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate")
            .val(moment().subtract('months', 1).endOf('month').local().format("YYYY-MM-DD"));
          break;
        case "Last30Days":
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 30).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "Last60Days":
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 60).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "Last90Days":
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 90).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "All":
          $("#apiTrialActivityStartdate").val(moment().subtract('years', 1).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        case "Custom":
          // display date pickers because we are going to allow the user to enter a custom date
          $("#apiTrialActivityDatePickers").show();
          // set default to 1 week back
          $("#apiTrialActivityStartdate").val(moment().subtract('days', 7).local().format("YYYY-MM-DD"));
          $("#apiTrialActivityEnddate").val(moment().add('days', 1).local().format("YYYY-MM-DD"));
          break;
        }
      }
    };

  </script>

</asp:Content>