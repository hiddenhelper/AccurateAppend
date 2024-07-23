<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Auto Processing Rules
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="row" style="padding: 0 0 30px 35px;">

    <h3 style="margin: 0 0 20px 0;">Auto Processing Rules - <%= Model.Username ?? "" %></h3>

    <div style="margin-bottom: 20px;">
      <a class="btn btn-default" href="#" id="newFixedMapRule">New Fixed-map Rule</a>
      <a class="btn btn-default" href="#" id="newAutoMapRule">New Auto-Map Rule</a>
    </div>

    <div id="globalMessage" style="display: none;"></div>

    <div class="alert alert-info" data-bind="visible: rules().length === 0">No Rules Found</div>

    <div class="k-grid k-widget" data-bind="visible: rules().length > 0">

      <table class="table table-bordered">
        <thead class="k-grid-header">
        <tr>
          <th class="k-header">Date added</th>
          <th class="k-header">Description</th>
          <th class="k-header"`>Terms</th>
          <th class="k-header">Run Order</th>
          <th class="k-header"></th>
          <th class="k-header" style="width: 275px;"></th>
        </tr>
        </thead>
        <tbody data-bind="template: { name: 'ruleRowTemplate', foreach: rules }">
        </tbody>
      </table>

    </div>

  </div>

  <script type="text/x-kendo-template" id="ruleRowTemplate">
    <tr>
      <td data-bind="text: dateAdded" class="text-left"></td>
      <td><span data-bind="text: description, valueUpdate: 'afterchange'"></td>
      <td><span data-bind="text: terms"></td>
      <td><span data-bind="text: order"></td>
      <td>
      <div class="form-check">
        <label class="form-check-label">
          <input type="checkbox" class="form-check-input" value="" data-bind="checked: subject" disabled>Subject Line
        </label>
      </div>
      <div class="form-check">
        <label class="form-check-label">
          <input type="checkbox" class="form-check-input" value="" data-bind="checked: body" disabled>Body
        </label>
      </div>
      <div class="form-check">
        <label class="form-check-label">
          <input type="checkbox" class="form-check-input" value="" data-bind="checked: fileName" disabled>File Name
        </label>
      </div>
      </td>
      <td style="text-align: center;">
    <a href="#" class="btn btn-warning" data-bind="click: $root.openDeleteConfirmationModal">Delete</a>
      <a href="#" class="btn btn-default" data-bind="click: $root.editRule">Edit</a>
    <a href="#" class="btn btn-default" data-bind="click: $root.downloadManifest">Download Manifest</a>    
    <%--<div class="btn-group" role="group">
        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
          Manifest
          <span class="caret"></span>
        </button>
        <ul class="dropdown-menu">
          <li>
            <a href="#" data-bind="click: $root.downloadManifest">Download</a>
          </li>
         <%-- <li>
            <a href="#" data-bind="click: $root.edit">Edit</a>
          </li>
        </ul>
      </div>--%>
      </td>
    </tr>
</script>

  <div class="modal" id="edit-rule-modal" tabindex="-1" role="dialog" aria-hidden="true" data-bind="with: currentRule">
    <div class="modal-dialog modal-lg" style="width: 600px;">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #5CB4E3;">
          <h5 class="modal-title" style="color: #ffffff">Edit Rule</h5>
          <button type="button" class="close" data-dismiss="modal">&times;</button>
        </div>
        <div class="modal-body" style="font-family: Arial;">
          <div id="message" style="display: none;"></div>
          <form>
            <div class="form-group">
              <label>Description</label>
              <input type="text" class="form-control" data-bind="value: description">
            </div>
            <div class="form-group">
              <label>Terms</label>
              <input type="text" class="form-control" data-bind="value: terms">
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" class="form-check-input" value="" data-bind="checked: subject">Subject Line
              </label>
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" class="form-check-input" value="" data-bind="checked: body">Body
              </label>
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" class="form-check-input" value="" data-bind="checked: fileName">File Name
              </label>
            </div>
           <%-- <div class="form-group">
              <input name="files" id="files" type="file"/>
            </div>--%>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
          <button type="button" class="btn btn-primary" data-bind="click: $root.save">Save</button>
        </div>
      </div>
    </div>
  </div>

  <div class="modal" id="delete-rule-modal" tabindex="-1" role="dialog" aria-hidden="true" data-bind="with: currentRule">
    <div class="modal-dialog modal-lg" style="width: 600px;">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #5CB4E3;">
          <h5 class="modal-title" style="color: #ffffff">Delete Rule</h5>
          <button type="button" class="close" data-dismiss="modal">&times;</button>
        </div>
        <div class="modal-body" style="font-family: Arial;">
          <p>Are you sure you want to delete this rule?</p>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
          <button type="button" class="btn btn-danger" data-bind="click: $root.delete">Confirm</button>
        </div>
      </div>
    </div>
  </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script src="<%= Url.Content("~/scripts/knockout-3.4.2.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/Areas/SmtpRules/Summary/Scripts/Index.js") %>"></script>

  <script type="text/javascript">

    var userId = '<%= Model.UserId.ToString() %>';
    var manifestId = '<%= Model.ManifestId ?? "" %>';
    var smtpRuleLinks = <%= Json.Encode(Model.Links) %>;

  </script>

</asp:Content>