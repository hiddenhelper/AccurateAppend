<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<Uri>" %>
<%@ Import Namespace="AccurateAppend.Security" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  User Detail
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="globalMessage" style="display: none; margin-left: 20px;"></div>
<div class="row" style="padding: 0 0 30px 35px;">
  <div class="btn-group" role="group" aria-label="" id="buttonBar">
    <a href="#" id="createTicket" class="btn btn-default">Create Ticket</a>
    <div class="btn-group" role="group">
      <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        Payment & Account
        <span class="caret"></span>
      </button>
      <ul class="dropdown-menu">
        <li>
          <a href="#" id="DownloadUsage">Usage</a>
        </li>
        <li>
          <a href="#" id="Cards">View Cards</a>
        </li>
        <li>
          <a href="#" id="CopyPaymentLinkToClipboard">Copy Card Update Link to Clipboard</a>
        </li>
        <li>
          <a href="#" id="Charges">View Charges</a>
        </li>
        <li>
          <a href="#" id="RateCards">Rate Cards</a>
        </li>
        <li>
          <a href="#" id="ServiceAccounts">Automatic Recurring Billing</a>
        </li>

        <li>
          <a href="#" id="SourceLead">View Source Lead</a>
        </li>
      </ul>
    </div>
    <div class="btn-group" role="group">
      <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        Deals
        <span class="caret"></span>
      </button>
      <ul class="dropdown-menu">
        <li>
          <a href="#" id="NewDeal">New Deal</a>
        </li>
        <li>
          <a href="#" id="Deals">View Deals</a>
        </li>
      </ul>
    </div>
    <div class="btn-group" role="group">
      <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        Jobs
        <span class="caret"></span>
      </button>
      <ul class="dropdown-menu">
        <li>
          <a href="#" id="JobsNew">New Job</a>
        </li>
        <li>
          <a href="#" id="Jobs">View Jobs</a>
        </li>
      </ul>
    </div>
    <div class="btn-group" role="group">
      <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        Login
        <span class="caret"></span>
      </button>
      <ul class="dropdown-menu">
        <li>
          <a href="#" id="LogInAsUser" target="_blank">Login As User</a>
        </li>
        <%--<li>
          <a href="#" id="">(NI)Send Password Reset Notification</a>
        </li>--%>
        <li>
          <a href="#" id="UserMustChangePassword">Force Password Change on Next Login</a>
        </li>
      </ul>
    </div>
    <a href="#" id="Contacts" class="btn btn-default">Contacts</a>
    <a href="#" id="Files" class="btn btn-default">Files</a>
    <a href="#" id="Messages" class="btn btn-default">Messages</a>
    <a href="#" id="AutoProcessorRules" class="btn btn-default">Custom Products</a>
    <a href="#" id="APIReporting" class="btn btn-default">API</a>
    <a href="#" id="ViewNations" class="btn btn-default">Nations</a>
  </div>
</div>

<div class="row" style="padding: 0 0 20px 20px;">

<div class="row">
<!-- Account Details -->
<div class="col-md-4">
  <div class="panel panel-default">
    <div class="panel-heading">
      <h3 class="panel-title">
        Account Details
        <span id="LastActivityDate" class="pull-right" style="color: #A0A0A0; font-size: 10px;"></span>
      </h3>
    </div>
    <div class="panel-body" id="userDetails">
      <form class="form-horizontal" id="user_edit_form">
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-6">
            <input type="checkbox" checked data-toggle="toggle" data-size="mini" id="IsLockedOut"><span style="margin-left: 10px;">Account Locked</span>
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-6">
            <span id="DateAdded"></span>
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Email/Username</label>
          <div class="col-sm-8">
            <% if (!User.Identity.IsSuperUser())
               { %>
              <input class="form-control" id="Email" disabled>
            <% }
               else
               { %>
              <input class="form-control" id="Email">
            <% } %>
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Business Name</label>
          <div class="col-sm-8">
            <input class="form-control" id="BusinessName">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">First Name</label>
          <div class="col-sm-8">
            <input class="form-control" id="FirstName">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Last Name</label>
          <div class="col-sm-8">
            <input class="form-control" id="LastName">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Address</label>
          <div class="col-sm-8">
            <input class="form-control" id="Address">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">City</label>
          <div class="col-sm-8">
            <input class="form-control" id="City">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">State</label>
          <div class="col-sm-8">
            <!-- need select here-->
            <input class="form-control" id="State">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Postal Code</label>
          <div class="col-sm-8">
            <input class="form-control" id="Zip">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Phone Number</label>
          <div class="col-sm-8">
            <input class="form-control" id="Phone">
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">Sales Rep</label>
          <div class="col-sm-8">
            <select id="accountOwner" class="form-control" <% if (!User.Identity.IsSuperUser())
                                                              { %> disabled <% } %>></select>
            <input type="hidden" id="publicKey"/>
          </div>
        </div>
        <div class="form-group">
          <label for="" class="col-sm-4 control-label">UserId/API Key</label>
          <div class="col-sm-8">
            <div class="input-group">
              <input type="text" class="form-control" aria-label="" id="userid">
              <div class="input-group-btn">
                <button type="button" class="btn btn-default" id="copyUserIdToClipboard">Copy</button>
              </div>
            </div>
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-6">
            <input type="checkbox" checked data-toggle="toggle" data-size="mini" id="StoreData"><span style="margin-left: 10px;">Retain Data</span>
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-6">
            <input type="checkbox" checked data-toggle="toggle" data-size="mini" id="batchUser"><span style="margin-left: 10px;">Batch User</span>
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-6">
            <input type="checkbox" checked data-toggle="toggle" data-size="mini" id="xmlUser"><span style="margin-left: 10px;">API Access</span>
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-offset-4 col-sm-4">
            <button type="button" class="btn btn-success" onclick="userDetailViewModel.save()">Update</button>
          </div>
        </div>
      </form>
    </div>
  </div>
</div>

<div class="col-md-8">

  <!-- Nav tabs -->
  <ul class="nav nav-tabs" role="tablist">
    <li role="presentation" class="active">
      <a href="#reporting" aria-controls="reporting" role="tab" data-toggle="tab">Reporting</a>
    </li>
    <li role="presentation">
      <a href="#tickets" aria-controls="tickets" role="tab" data-toggle="tab">Tickets</a>
    </li>
    <li role="presentation">
      <a href="#notes" aria-controls="notes" role="tab" data-toggle="tab">Notes</a>
    </li>
    <li role="presentation">
      <a href="#documents" aria-controls="documents" role="tab" data-toggle="tab">Documents</a>
    </li>
  </ul>
  <!-- Tab panes -->
  <div class="tab-content">
    <div role="tabpanel" class="tab-pane active" id="reporting" style="padding-top: 20px;">
      <h4>Activity Overview</h4>
      <div id="UserOperatingMetricsOverviewGrid" class="k-grid k-widget" style="margin-bottom: 20px;"></div>
      <h4>Product Usage Overview</h4>
      <div id="UserProductUsageOverviewGridMessage" class="alert alert-info" style="display: none;">No product usage found</div>
      <div id="UserProductUsageOverviewGrid" class="k-grid k-widget" style="margin-bottom: 20px;"></div>
    </div>
    <div role="tabpanel" class="tab-pane" id="tickets" style="padding-top: 20px;">
      <a id="createTicket" href="#" class="btn btn-primary btn-sm" style="margin-bottom: 20px;"><span class="glyphicon glyphicon-plus" aria-hidden="true"></span>New Ticket</a>
      <div class="alert alert-info" style="display: none; margin: 20px 0 20px 0;" id="ticketsGridMessage"></div>
      <div id="ticketsGrid" style="margin-bottom: 20px; margin-top: 10px;"></div>
    </div>
    <div role="tabpanel" class="tab-pane" id="notes" style="padding-top: 20px;">
      <a href="#" id="addNoteButton" class="btn btn-primary btn-sm" style="margin-bottom: 20px;"><span class="glyphicon glyphicon-plus" aria-hidden="true"></span>New Note</a>
      <div class="alert alert-info" style="display: none;" id="message">No notes found</div>
      <div style="margin: 0 0 15px 0;" class="k-grid k-widget" id="notes_table">
        <table>
          <thead class="k-grid-header">
          <tr>
            <th style="width: 140px;" class="k-header">
              Date
            </th>
            <th style="width: 140px;" class="k-header">
              Added By
            </th>
            <th class="k-header">
              Note
            </th>
          </tr>
          </thead>
          <tbody></tbody>
        </table>
      </div>
    </div>
    <div role="tabpanel" class="tab-pane" id="documents" style="padding-top: 20px;">
      <a href="#" id="addDocumentButton" class="btn btn-primary btn-sm" style="margin-bottom: 20px;"><span class="glyphicon glyphicon-plus" aria-hidden="true"></span>New Document</a>
      <div class="alert alert-info" style="display: none;" id="message">No documents found</div>
      <div style="margin: 0 0 15px 0;" class="k-grid k-widget" id="documents_table">
        <table>
          <thead class="k-grid-header">
          <tr>
            <th style="width: 140px;" class="k-header">
              Date
            </th>
            <th class="k-header">
              File name
            </th>
            <th class="k-header">
              Note
            </th>
            <th class="k-header" style="width: 215px;"></th>
          </tr>
          </thead>
          <tbody></tbody>
        </table>
        <input type="hidden" id="documentId" value=""/>
      </div>

    </div>

  </div>

</div>

</div>
</div>

<%= Html.Hidden("ApplicationId", ViewData["ApplicationId"]) %>
<%: Html.Hidden("userDetailUri", ViewData["userDetailuri"]) %>
<%= Html.Hidden("adminusername", User.Identity.Name) %>
<!-- used for clipboard copy -->
<input id="clipboard" value="" style="left: -1000px; position: absolute; top: -1000px"/>

<!-- ADD NOTE -->
<div class="modal fade" tabindex="-1" role="dialog" id="accountNoteModal">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #f0f0f0;">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
        <h4 class="modal-title">Add Note to Account</h4>
      </div>
      <div class="modal-body">
        <div>
          <%= Html.TextArea("notebody", null, 7, 120, new {@class = "form-control"}) %>
        </div>
      </div>
      <div class="modal-footer" style="background-color: #f0f0f0;">
        <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
        <button type="button" class="btn btn-primary" onclick="notesViewModel.save()">Save changes</button>
      </div>
    </div>
  </div>
</div>

<!-- ADD NOTE TO ADMIN FILE -->
<div class="modal fade" tabindex="-1" role="dialog" id="adminFileNoteModal">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #f0f0f0;">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
        <h4 class="modal-title">Add Note to Admin File</h4>
      </div>
      <div class="modal-body">
        <div class="form-horizontal">
          <div class="form-group">
            <label class="col-sm-4 control-label">Note</label>
            <div class="col-sm-8">
              <%= Html.TextArea("documentNoteBody", null, 7, 120, new {@class = "form-control"}) %>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-footer" style="background-color: #f0f0f0;">
        <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
        <button type="button" class="btn btn-primary" onclick="documentsViewModel.addNote()">Save changes</button>
      </div>
    </div>
  </div>
</div>

<!-- DOWNLOAD USAGE -->
<div class="modal fade" tabindex="-1" role="dialog" id="downloadUsageModal">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #f0f0f0;">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
        <h4 class="modal-title">Download Usage</h4>
      </div>
      <div class="modal-body">
        <form class="form-horizontal">
          <div class="form-group">
            <label class="col-sm-4 control-label">Date Range</label>
            <div class="col-sm-8">
              <span id="usageDateRangeWidget"></span>
            </div>
          </div>
          <div class="form-group">
            <div class="col-sm-offset-4 col-sm-8">
              <a href="#"class="btn btn-default" onclick="userDetailViewModel.saveUsage()">Save To Files</a>
              <a href="#"class="btn btn-primary" onclick="userDetailViewModel.downloadUsage()">Download</a>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</div>

<!-- UPLOAD ADMIN FILE -->
<div class="modal fade" tabindex="-1" role="dialog" id="documentUploadModal">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #f0f0f0;">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
        <h4 class="modal-title">Upload Document</h4>
      </div>
      <div class="modal-body" style="padding: 25px;">
        <input name="files" id="adminFilesToUpload" type="file"/>
      </div>
      <div class="modal-footer" style="background-color: #f0f0f0;">
        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

<!-- NATION BUILDER REGISTRATIONS MODAL -->
<div class="modal fade" tabindex="-1" role="dialog" id="nationBuilderRegistrations">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #f0f0f0;">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
        <h4 class="modal-title">Nation Builder Registrations</h4>
      </div>
      <div class="modal-body">
        <a href="https://apiexplorer.nationbuilder.com/nationbuilder" target="_new" class="btn btn-default" style="margin-bottom: 20px;">Nation Builder API Explorer</a>
        <div id="nationsMessage" class="alert alert-info" style="display: none;"></div>
        <div style="margin: 0 0 15px 0;" class="k-grid k-widget" id="nations">
          <table>
            <thead class="k-grid-header">
            <tr>
              <th style="width: 140px;" class="k-header">
                Slug
              </th>
              <th style="width: 140px;" class="k-header">
                Token
              </th>
            </tr>
            </thead>
            <tbody></tbody>
          </table>
        </div>
      </div>
      <div class="modal-footer" style="background-color: #f0f0f0;">
        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

<div class="modal" tabindex="-1" role="dialog" id="detailsModal">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"></button>
        <h4 class="modal-title"></h4>
      </div>
      <div class="modal-body">
        <pre></pre>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>
  </div>
</div>

<div class="modal" tabindex="-1" role="dialog" id="confirmationForcePasswordReset">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"></button>
        <h4 class="modal-title">Confirm</h4>
      </div>
      <div class="modal-body">
        <p>You are are about to force the user to change their password the next time they login.</p>
        <p>Do you want to proceed?</p>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
        <button class="btn btn-danger" id="btnForcePasswordReset" onclick="userDetailViewModel.forcePasswordChange()">Confirm</button>
      </div>
    </div>
  </div>
</div>


<script id="responsive-column-template-complete" type="text/x-kendo-template">
<strong>Date Created</strong>
<p class="col-template-val">#= data.CreatedAt #</p>

<strong>Ticket Type</strong>
<p class="col-template-val">#= data.Type #</p>

<strong>Status</strong>
<p class="col-template-val">#= data.Status #</p>

<strong>Subject</strong>
<p class="col-template-val">#=data.Subject #</p>
  
<a href="\\#" class="btn btn-default" onclick="ticketsApiViewModel.viewDetail('#= uid #')">View Details</a>
<a href="#= data.Url #" class="btn btn-default">View In Zendesk</a>

</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script src="<%= Url.Content("~/Areas/Clients/UserDetail/Scripts/UserDetail.js") %>" type="text/javascript">
  </script>

  <!-- http://www.bootstraptoggle.com/ -->
  <link href="//gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css" rel="stylesheet">
  <script src="//gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>

  <script type="text/javascript">

    var usageDateRangeWidget;

    adminUsers = <%= Json.Encode(AdminUserCache.Cache.Select(u => new
                     {
                       u.UserId,
                       u.UserName
                     })
                       ) %>;

    $(function() {

      usageDateRangeWidget = new AccurateAppend.Ui.DateRangeWidget("usageDateRangeWidget",
        new AccurateAppend.Ui.DateRangeWidgetSettings(
          [
            AccurateAppend.Ui.DateRangeValue.ThisMonth,
            AccurateAppend.Ui.DateRangeValue.LastMonth,
            AccurateAppend.Ui.DateRangeValue.Last24Hours,
            AccurateAppend.Ui.DateRangeValue.Last7Days,
            AccurateAppend.Ui.DateRangeValue.Last30Days,
            AccurateAppend.Ui.DateRangeValue.Custom
          ],
          AccurateAppend.Ui.DateRangeValue.LastMonth,
          []));
    });

  </script>

  <script id="operating-metrics-responsive-template" type="text/x-kendo-template">
  
  <strong>Description</strong>  
  <p class="col-template-val">#=data.MetricNameDescription#</p>

  <strong>Today</strong>
  <p class="col-template-val"># switch (data.MetricName) {
          case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #: kendo.toString(data.Today, "c0")#
            # break; #
        # default:#
            #: kendo.toString(data.Today, "n0")#
        # }#</p> 

  <strong>Last 7</strong>
  <p class="col-template-val"># switch (MetricName) {
         case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(Last7, "c0")#
            # break; #
        # default:#
            #= kendo.toString(Last7, "n0")#
        # }#</p>
  
  <strong>Current Month</strong>
  <p class="col-template-val"># switch (MetricName) {
          case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(CurrentMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(CurrentMonth, "n0")#
        # }#</p>
  
  <strong>Same Period Last Month</strong>
  <p class="col-template-val"> # switch (MetricName) {
              case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(SamePeriodLastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(SamePeriodLastMonth, "n0")#
        # }#</p>
  
  <strong>Last Month</strong>
  <p class="col-template-val"> # switch (MetricName) {
             case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(LastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(LastMonth, "n0")#
        # }#</p>
  
  <strong>Previous To Last Month</strong>
  <%--<p class="col-template-val">#=kendo.render(kendo.template($("\\#previousToLastMonthTemplate").html()),data)#</p>--%>
  <p class="col-template-val"># switch (MetricName) {
      case 'SelfServiceRevenue': #
      # case 'ChargeEventsRevenue': #
      # case 'TotalRevenue': #
          #= kendo.toString(PreviousToLastMonth, "c0")#
          # break; #
      # default:#
          #= kendo.toString(PreviousToLastMonth, "n0")#
      # }#</p>

</script>
  <script id="todayTemplate" type="text/x-kendo-tmpl">
        # switch (data.MetricName) {
          case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #: kendo.toString(data.Today, "c0")#
            # break; #
        # default:#
            #: kendo.toString(data.Today, "n0")#
        # }#
  
  </script>
  <script id="last7Template" type="text/x-kendo-tmpl">
        # switch (MetricName) {
         case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(Last7, "c0")#
            # break; #
        # default:#
            #= kendo.toString(Last7, "n0")#
        # }#
    </script>
  <script id="currentMonthTemplate" type="text/x-kendo-tmpl">
        # switch (MetricName) {
          case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(CurrentMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(CurrentMonth, "n0")#
        # }#
    </script>
  <script id="samePeriodLastMonthTemplate" type="text/x-kendo-tmpl">
       # switch (MetricName) {
              case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(SamePeriodLastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(SamePeriodLastMonth, "n0")#
        # }#
    </script>
  <script id="LastMonthTemplate" type="text/x-kendo-tmpl">
       # switch (MetricName) {
             case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(LastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(LastMonth, "n0")#
        # }#
    </script>
  <script id="previousToLastMonthTemplate" type="text/x-kendo-tmpl">
       # switch (MetricName) {
        case 'SelfServiceRevenue': #
        # case 'ChargeEventsRevenue': #
        # case 'TotalRevenue': #
            #= kendo.toString(PreviousToLastMonth, "c0")#
            # break; #
        # default:#
            #= kendo.toString(PreviousToLastMonth, "n0")#
        # }#
    </script>

</asp:Content>