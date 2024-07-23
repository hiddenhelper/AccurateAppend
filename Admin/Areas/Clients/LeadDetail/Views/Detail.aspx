<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<LeadViewModel>" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="DomainModel.Html" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="AccurateAppend.Security" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.DeleteLead" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary" %>
<%@ Import Namespace="AccurateAppend.Accounting" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Lead
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    var isLocked = true;

    $(function() {

      ////////////////////////////////////
      // click handlers
      ////////////////////////////////////

      $("#copyRegLink").click(function() {
        $("#copyMessage").text("");
        $("#registrationLink").select();
        document.execCommand("copy");
        $("#copyMessage").text("Link copied to clipboard");
        return false;
      });

      // if contact method == phone then show checkbox, otherwise hide and set value = false
      $("#ContactMethod").change(function() {
        if ($("#ContactMethod").val() == "<%: (Int32) LeadContactMethod.Phone %>")
          $("#pnlWarmtransferred").show();
        else {
          $("#WarmTransferred").removeAttr("checked");
          $("#pnlWarmtransferred").hide();
        }
      });

      $("#Status").change(function() {
        if ($("#Status").val() == <%: (Int32) LeadStatus.ConvertedToCustomer %>)
          $("#lnkGoToClient").show();
        else
          $("#lnkGoToClient").hide();
      });

      // if not qualified then indicate why
      $("#Qualified").change(function() {
        if ($("#Qualified").val() === "<%: (Int32) LeadQualification.NotQualified %>") {
          $("#disqualificationReasonPanel").show();
          $("#Status").val("<%: (Int32) LeadStatus.NoFurtherAction %>"); // update Status to "No Further Action"
        } else {
          $("#DisqualificationReason").val("<%: (Int32) LeadDisqualificationReason.OrderBelowMinimum %>");
          $("#disqualificationReasonPanel").hide();
        }
      });

      // parses domain from landing page url
      $("[name=Phone],[name=Email],#ApplicationId").change(function() {
        $("#alert").hide();
        if ($("#ApplicationId").val() != '') {
          $.ajax(
            {
              type: "GET",
              url: "<%= this.Url.Action("LeadExists", "LeadDetail", new {area = "Clients"}) %>",
              data: {
                applicationid: $("#ApplicationId").val(),
                email: $("#Email").val(),
                phone: $("#Phone").val(),
                existingLead: 0
              },
              async: true,
              success: function(data) {
                if (data.HttpStatusCodeResult === <%: (Int32) HttpStatusCode.OK %>) {
                  $("#alert").html(data.Message +
                    ' <a href="' +
                    data.DetailUrl +
                    '" class="alert-link">Click here to view the other lead.</a>');
                  $("#alert").show();
                } else if (data.HttpStatusCodeResult === <%: (Int32) HttpStatusCode.InternalServerError %>) {
                  $("#alert").text("Error: Unable to verify if this lead exists.");
                  $("#alert").show();
                }
              }
            });
        }

      });
    
      // delete lead
      $("#delete").click(function() {
        $('#delete-modal').modal('show');
      });

      // cancel edit and reload from database
      $("#cancel").click(function() {
        window.location.replace(
          "<%= this.Url.Action("View", "LeadDetail", new {area = "Clients", leadId = this.Model.LeadId}) %>");
      });

      if ($("#Qualified").val() === "<%: (Int32) LeadQualification.NotQualified %>")
        $("#disqualificationReasonPanel").show();

      if ($("#view").val() == "create") {
        unlockForm();
        $("#cancel").hide();
        $("#delete").hide();
      } else {
        lockForm();
      }

      // if page state = validation error then the form needs to be unlocked so the user can modify the values
      if ($("#error").val() === "True")
        unlockForm();

      // if new lead don't show quote doucment panel
      if ($("#LeadId").val() != 0) {
        $("#quoteDocPanel").show();
        displayNotes();
      }

      // display warm tranfer checkbox if Contact Method = phone
      if ($("#ContactMethod").val() == "<%: LeadContactMethod.Phone %>")
        $("#pnlWarmtransferred").show();

      if ($("#Status").val() == <%: (Int32) LeadStatus.ConvertedToCustomer %>)
        $("#lnkGoToClient").show();
      else
        $("#lnkGoToClient").hide();

      $("[name=ApplicationId]").css({ display: "block" });
    });

    // sets form state to locked
    function lockForm() {
      $(".lockable").prop("readonly", true);
      $("#ApplicationId").prop("disabled", true);
      $("#Qualified").prop("disabled", true);
      $("#DoNotMarketTo").prop("disabled", true);
      $("#Status").prop("disabled", true);
      $("#State").prop("disabled", true);
      $("#ProductInterest").prop("disabled", true);
      $("#ContactMethod").prop("disabled", true);
      $("#LeadSource").prop("disabled", true);
      $("#DisqualificationReason").prop("disabled", true);
      $("#OwnerId").prop("disabled", true);
      $("#Score").prop("disabled", true);
      $("#save").hide();
      $("#edit").show();
      $("#cancel").hide();
      isLocked = true;
    };

    // sets form state to unlocked
    function unlockForm() {
      $(".lockable").prop("readonly", false);
      $("#ApplicationId").prop("disabled", false);
      $("#Qualified").prop("disabled", false);
      $("#DoNotMarketTo").prop("disabled", false);
      $("#Status").prop("disabled", false);
      $("#State").prop("disabled", false);
      $("#ProductInterest").prop("disabled", false);
      $("#ContactMethod").prop("disabled", false);
      $("#LeadSource").prop("disabled", false);
      $("#DisqualificationReason").prop("disabled", false);
      $("#Score").prop("disabled", false);
      $("#save").show();
      $("#edit").hide();
      $("#cancel").show();
      isLocked = false;
    };

    // locks or unlocks controls depending on current state
    function toggleLockState() {
      if (isLocked) {
        unlockForm();
      } else {
        lockForm();
      }
    }

    // deletes current lead
    function deleteLead() {
      $.ajax(
        {
          type: "GET",
          async: false,
          url: "<%= this.Url.BuildFor<DeleteLeadController>().Delete(this.Model.LeadId) %>",
          success: function(result) {
            window.location.assign("<%= this.Url.BuildFor<LeadSummaryController>().ToIndex() %>");
          },
          error: function(xhr, status, error) {
            $("#error").html("<strong>Error:</strong>Unable to delete lead.").show();
          }
        });
    }

    // adds note to lead
    function addNote() {
      $.ajax(
        {
          type: "POST",
          async: false,
          url: "<%= this.Url.Action("Add", "LeadNotes", new {Area = "Clients"}) %>",
          data: { body: $("#notebody").val(), leadid: $("#LeadId").val() },
          success: function(result) {
            displayNotes();
          },
          error: function(xhr, status, error) {
            $("#error").html("<strong>Error:</strong>Unable to save note." + xhr.statusText).show();
          }
        });
    }

    //displays notes for a lead
    function displayNotes() {

      $("#notesPanel").show();

      var grid = $('#notesgrid').data('kendoGrid');
      if (grid != null) {
        grid.dataSource.read();
      } else {
        $("#notesgrid").kendoGrid({
          autobind: false,
          dataSource: {
            autobind: false,
            type: "json",
            transport: {
              read: function(options) {
                $.ajax({
                  url:
                    "<%= this.Url.Action("Query", "LeadNotes", new {Area = "Clients", leadId = this.Model.LeadId}) %>",
                  dataType: 'json',
                  type: 'GET',
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
            pageSize: 3,
            change: function() {
              if (this.data().length <= 0) {
                $("#notesInfo").show();
                $("#notesgrid").hide();
              } else {
                $("#notesInfo").hide();
                $("#notesgrid").show();
              }
            }
          },
          scrollable: false,
          sortable: true,
          pageable: {
            input: true,
            numeric: false
          },
          columns: [
            { field: "Body", title: "Description", template: kendo.template($("#noteTemplate").html()) }
          ]
        });
      }
    }

  </script>

  <script type="text/x-kendo-template" id="noteTemplate">
    <p>#= Body #</p>
    <p style="margin-bottom: 2px; font-size:.8em;line-height: .8em;"><span style="font-weight: bold;">Added By: </span>#= AddedBy #</p>
    <p style="font-size:.8em;"><span style="font-weight: bold;">Date: </span>#= DateAdded #</p>
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="well well-lg" style="padding: 30px;">

<div class="row">

  <div class="form-group">

    <div>
      <button type="button" class="btn btn-danger" id="delete"><span class="fa fa-warning"></span>Delete Lead</button>
      <button type="button" class="btn btn-default" id="cancel" style="display: none;"><span class="fa fa-undo"></span>Cancel</button>
      <button type="button" class="btn btn-primary" onclick="toggleLockState();" id="edit" style="display: none;"><span class="fa fa-edit"></span>Edit</button>
      <button type="button" class="btn btn-success" onclick="$('#form').submit();" id="save" style="display: none;"><span class="fa fa-check-square"></span>Save</button>
      <% if (this.Model.TrialId != 0)
         { %>
        <%: this.Html.ActionLinkWithFontAwesomIcon("View API Trial", "Index", "ApiTrialDetail", new {Id = this.Model.TrialId}, new {@class = "btn btn-default"}, "") %>
      <% } %>
      <% if (this.Model.CrmLink != null)
         {
      %>
        <button type="button" class="btn btn-success" onclick="window.location.href = '<%: this.Model.CrmLink %>';"><span class="fa fa-check-square"></span>ZenSell</button>
      <%
         }
      %>
    </div>

  </div>

  <div class="alert alert-info" role="alert" id="alert" style="display: none;"></div>

</div>

<div class="row col-md-8">

<% using (this.Html.BeginForm("Edit", "LeadDetail", FormMethod.Post, new {@class = "form-horizontal", id = "form"}))
     // form action = Edit because the submit buttopn is only active when the form is in Edit mode
   { %>

  <div class="row">

    <div class="col-md-4">

      <div class="form-group">
        <label>Site</label>
        <%: Html.SiteDropDown(Model.ApplicationId.ToString()) %>
        <%= Html.ValidationMessageFor(m => m.ApplicationId, "") %>
      </div>

      <div class="form-group">
        <label>Date</label>
        <%= Html.TextBoxFor(a => a.DateAdded, new {@class = "form-control", disabled = "disabled"}) %>
      </div>

      <div class="form-group">
        <label>Status</label>
        <%: Html.DropDownListFor(m => m.Status, EnumExtensions.ToLookup<LeadStatus>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
        <%= Html.ValidationMessageFor(m => m.Status, "") %>
      </div>

      <div class="form-group" id="lnkGoToClient">
        <%= this.Html.NavigationFor<UserDetailController>().Detail("View Client Account", this.Model.LeadId) %>
      </div>

      <div class="form-group">
        <label>Owner</label>
        <%= this.Html.AdminUsersDropDown(id: "OwnerId") %>
        <%= this.Html.ValidationMessageFor(x => x.OwnerId) %>
      </div>

      <div class="form-group">
        <label>Qualification</label>
        <%: Html.DropDownListFor(m => m.Qualified, EnumExtensions.ToLookup<LeadQualification>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
        <%= Html.ValidationMessageFor(m => m.Qualified, "") %>
      </div>

      <div class="form-group">
        <label>Score</label>
        <%: Html.DropDownListFor(m => m.Score, EnumExtensions.ToLookup<LeadScore>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
        <%= Html.ValidationMessageFor(m => m.Score, "") %>
      </div>

      <div class="form-group">
        <div class="checkbox">
          <label><%: Html.CheckBoxFor(m => m.DoNotMarketTo) %> Do Not Market</label>
          <%= Html.ValidationMessageFor(m => m.DoNotMarketTo, "") %>
        </div>
      </div>

      <div class="form-group" id="disqualificationReasonPanel" style="display: none;">

        <div>
          <label class="control-label">Disqualification Reason</label>
          <%

            var reasons = new List<SelectListItem>
            {
              new SelectListItem
              {
                Text = @"-- select ----",
                Value = ""
              },
              new SelectListItem
              {
                Text = LeadDisqualificationReason.OrderBelowMinimum.GetDescription(),
                Value = ((Int32) LeadDisqualificationReason.OrderBelowMinimum).ToString()
              },
              new SelectListItem
              {
                Text = LeadDisqualificationReason.DontHaveProduct.GetDescription(),
                Value = ((Int32) LeadDisqualificationReason.DontHaveProduct).ToString(),
                Selected = true
              },
              new SelectListItem
              {
                Text = LeadDisqualificationReason.Solicitation.GetDescription(),
                Value = ((Int32) LeadDisqualificationReason.Solicitation).ToString()
              },
              new SelectListItem
              {
                Text = LeadDisqualificationReason.Other.GetDescription(),
                Value = ((Int32) LeadDisqualificationReason.Other).ToString()
              }
            };
          %>
          <%: Html.DropDownListFor(a => a.DisqualificationReason, reasons, new {@class = "form-control lockable"}) %>
          <%= Html.ValidationMessageFor(a => a.DisqualificationReason, "", new {@class = "help-block", style = "color:red;"}) %>
        </div>

      </div>

    </div>

    <div class="col-md-8" style="padding: 0 0 0 30px">

      <div class="form-group">

        <div class="col-md-12" id="quoteDocPanel" style="display: none;">

          <% if (this.Model.Status != LeadStatus.ConvertedToCustomer)
             {
          %>

            <label>Registration</label>

            <div class="panel panel-default">
              <div class="panel-body">
                <%
                  if (Model.Status != LeadStatus.ConvertedToCustomer)
                  {
                    var url = "";
                    if (Model.ApplicationId == ApplicationExtensions.AccurateAppendId)
                    {
                      url = "https://clients.accurateappend.com/";
                    }
                    else if (Model.ApplicationId == ApplicationExtensions.TwentyTwentyId)
                    {
                      url = "https://clients.2020connect.net/";
                    }
                    Response.Write("<textarea id='registrationLink' class='form-control' row='2'>" + url + "Public/NewClientRegistration/Create?id=" + Model.PublicKey + "</textarea>");
                  }
                %>
                <a id="copyRegLink" href="#" class="btn btn-default" style="margin-top: 10px;">Copy Link to Clipboard</a><span id="copyMessage" style="color: green; margin-left: 20px;"></span>
              </div>
            </div>
          <% } %>
        </div>

      </div>

    </div>

  </div>

  <div class="row">

  <div class="form-group">

    <div class="col-md-12">
      <label>Business Name</label>
      <%= Html.TextBoxFor(a => a.BusinessName, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.BusinessName, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-6">
      <label>First Name</label>
      <%= Html.TextBoxFor(a => a.FirstName, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.FirstName, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-6">
      <label>Last Name</label>
      <%= Html.TextBoxFor(a => a.LastName, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.LastName, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-6">
      <label>Phone</label>
      <%= Html.TextBoxFor(a => a.Phone, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Phone, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-6">
      <label>Email</label>
      <%= Html.TextBoxFor(a => a.Email, new {@class = "form-control lockable", onclick = "this.select();", type = "email"}) %>
      <%= Html.ValidationMessageFor(a => a.Email, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-12">
      <label>Comments</label>
      <%= Html.TextAreaFor(a => a.Comments, 10, 110, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Comments, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-12">
      <label>Address</label>
      <%= Html.TextBoxFor(a => a.Address, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Address, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-6">
      <label>City</label>
      <%= Html.TextBoxFor(a => a.City, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.City, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-3">
      <label>State</label>
      <%: Html.DropDownListFor(a => a.State, NorthAmericanTerritories.StateSelectList(), new {@class = "form-control"}) %>
      <%= Html.ValidationMessageFor(a => a.State, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-3">
      <label>Zip</label>
      <%= Html.TextBox("Zip", null, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Zip, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group">

    <div class="col-md-12">
      <label>Website Url</label>
      <%= Html.TextBoxFor(a => a.Website, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Website, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <%--<div class="form-group">

    <div class="col-md-12">
      <label>Sector Industry</label>
      <%: Html.DropDownListFor(a => a.SectorIndustry, this.Model.SectorIndustry.Select(a => new SelectListItem {Value = a.Key.ToString(), Text = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.Website, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>--%>

  <div class="form-group">

    <div class="col-md-4">
      <label>Product Interest</label>
      <%
        var items = new List<SelectListItem>
        {
          new SelectListItem
          {
            Text = @"Phone Append",
            Value = "Phone Append"
          },
          new SelectListItem
          {
            Text = @"Email Append",
            Value = "Email Append",
            Selected = true
          },
          new SelectListItem
          {
            Text = @"Phone & Email Append",
            Value = "Phone & Email Append"
          },
          new SelectListItem
          {
            Text = @"Lead Validation",
            Value = "Lead Validation"
          },
          new SelectListItem
          {
            Text = @"List",
            Value = "List"
          },
          new SelectListItem
          {
            Text = @"Other",
            Value = "Other"
          }
        };
      %>
      <%: Html.DropDownListFor(a => a.ProductInterest, items, new {@class = "form-control"}) %>
      <%= Html.ValidationMessageFor(a => a.ProductInterest, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-4">
      <label>Contact Method</label>
      <%: Html.DropDownListFor(a => a.ContactMethod, EnumExtensions.ToLookup<LeadContactMethod>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
      <%= Html.ValidationMessageFor(a => a.ContactMethod, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

    <div class="col-md-4">
      <label>Channel</label>
      <%: Html.DropDownListFor(a => a.LeadSource, EnumExtensions.ToLookup<LeadSource>()
            .Where(a => !(a.Value == (int) LeadSource.EmailMarketing || a.Value == (int) LeadSource.Sem || a.Value == (int) LeadSource.TotalLiveChat)) // don't include EmailMarketing, SEM option in form
            .Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
      <%= Html.ValidationMessageFor(a => a.LeadSource, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

 <%-- <% if (this.Model.Trial != null)
     { %>
    <div class="form-group" style="display: none;">

      <div class="col-md-6">
        <label>Trial Id</label>
        <%= Html.TextBoxFor(a => a.TrialIdentity.AccessId, null, new {@class = "form-control", disabled = "disabled"}) %>
      </div>

    </div>
  <% } %>--%>

  <div class="form-group" style="display: none;">

    <div class="col-md-6">
      <label>IP</label>
      <%= Html.TextBoxFor(a => a.IP, null, new {@class = "form-control lockable"}) %>
      <%= Html.ValidationMessageFor(a => a.IP, "", new {@class = "help-block", style = "color:red;"}) %>
    </div>

  </div>

  <div class="form-group" style="display: none;">

    <div class="col-md-12">
      <label>Landing Page Url</label>
      <%= Html.TextBoxFor(a => a.LandingPageUrl, new {@class = "form-control lockable"}) %>
    </div>

  </div>

  <% if (this.Model.ApiReportAction != null)
     { %>
    <div class="form-group">

      <div class="col-md-12">
        <label>Trial Acccess</label>
        <a href="<%= this.Model.ApiReportAction %>">View API Calls</a>
      </div>

    </div>
  <% } %>
  </div>

  <%= Html.HiddenFor(a => a.LandingPageDomain) %>
  <%= Html.HiddenFor(a => a.FollowUpDate) %>
  <%= Html.HiddenFor(a => a.DisqualificationReason) %>
  <%= Html.HiddenFor(a => a.DateAdded) %>
  <%: Html.HiddenFor(a => a.LeadId) %>
  <%: Html.HiddenFor(a => a.PublicKey) %>
  <%: Html.HiddenFor(a => a.ApiReportAction) %>
  <%: Html.HiddenFor(a => a.SectorIndustry) %>
  <%: Html.Hidden("view", ViewData["view"]) %>
  <%: Html.Hidden("error", ViewData["error"]) %>

<% } %>
</div>

<div class="row col-md-4" style="padding-left: 60px;">

  <div class="form-group" id="notesPanel" style="display: none;">

    <label class="control-label">Notes</label>
    <div class="panel panel-default">
      <div class="panel-body">
        <div>
          <button type="button" class="btn btn-default pull-right" style="margin-bottom: 10px;" id="addNote" onclick="$('#note-modal').modal('show');"><span class="fa fa-plus"></span>New Note</button>
        </div>
        <div class="clearfix"></div>
        <div class="alert-info" style="display: none; padding: 10px;" id="notesInfo">No notes found</div>
        <div id="notesgrid"></div>
      </div>
    </div>

  </div>

</div>

<div class="clearfix"></div>

</div>

<div class="modal fade" id="delete-modal" tabindex="-1" role="dialog" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content" style="width: 190px;">
      <div class="modal-header" style="background-color: #F5F5F5;">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Confirmation</h4>
      </div>
      <div class="modal-body">
        <div class="form-group">
          <h4>Delete this lead?</h4>
          <button class="btn btn-default" onclick="$('#delete-modal').modal('hide');" type="button">Cancel</button>
          <button class="btn btn-danger" onclick="$('#delete-modal').modal('hide');deleteLead();" type="button">Delete</button>
        </div>
      </div>
    </div>
  </div>
</div>

<div class="modal fade" id="note-modal" tabindex="-1" role="dialog" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header" style="background-color: #F5F5F5;">
        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
        <h4 class="modal-title">Add Note</h4>
      </div>
      <div class="modal-body">
        <div class="form-group">
          <textarea id="notebody" cols="70" rows="6"></textarea>
        </div>
        <div class="form-group">
          <button class="btn btn-default" onclick="$('#note-modal').modal('hide');$('#notebody').val('');" type="button">Cancel</button>
          <button class="btn btn-success" onclick="$('#note-modal').modal('hide');addNote();$('#notebody').val('');" type="button">Save</button>
        </div>
      </div>
    </div>
  </div>
</div>

</asp:Content>