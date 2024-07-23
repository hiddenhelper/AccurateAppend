<%@ Control Language="C#" Inherits="ViewUserControl<ContactModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.EditContact.Models" %>
<%
  var item = Model ?? new ContactModel();
  var index = Guid.NewGuid().ToString();
  var emailAddress = "model.Contacts[" + index + "].EmailAddress";
  var name = "model.Contacts[" + index + "].name";
  var billing = "model.Contacts[" + index + "].BillTo";
  var notify = "model.Contacts[" + index + "].ShouldNotify";
  var submit = "model.Contacts[" + index + "].SubmitJobs";
  var admin = "model.Contacts[" + index + "].IsAdmin";
%>
<tr>
  <% = this.Html.Hidden("model.Contacts.Index", index) %>
  <% = this.Html.Hidden("model.Contacts[" + index + "].Id", item.Id) %>
  <td>
    <input type="text" name="<%= emailAddress %>" id="<%= index + "_emailAddress" %>" maxlength="50" value="<%= item.EmailAddress %>" class="form-control"/>
  </td>
  <td>
    <input type="text" name="<%= name %>" id="<%= index + "_name" %>" maxlength="100" value="<%= item.Name %>" class="form-control"/>
  </td>
  <td>
   <input type="checkbox" name="<%= billing %>" id="<%= index + "_billing" %>" value="true" <%= item.BillTo ? "checked" : "" %> />
   <%: this.Html.LabelFor(m => m.BillTo, "Send contact receipts and billing inquiries", new { @for = index + "_billing"}) %>
   <br />
   <input type="checkbox" name="<%= notify %>" id="<%= index + "_NotifyJobs" %>" value="true" <%= item.ShouldNotify ? "checked" : "" %> />
   <%: this.Html.LabelFor(m => m.ShouldNotify, "Send contact job notifications", new { @for = index + "_NotifyJobs"}) %>
   <br />
   <input type="checkbox" name="<%= submit %>" id="<%= index + "_SubmitJobs" %>" value="true" <%= item.SubmitJobs ? "checked" : "" %> />
   <%: this.Html.LabelFor(m => m.SubmitJobs, "Contact is allowed to submit SMTP jobs", new { @for = index + "_SubmitJobs"}) %>
   <br />
   <input type="checkbox" name="<%= admin %>" id="<%= index + "_IsAdmin" %>" value="true" <%= item.IsAdmin ? "checked" : "" %> />
   <%: this.Html.LabelFor(m => m.IsAdmin, "Contact is allowed to answer account management inquires and request password resets", new { @for = index + "_IsAdmin"}) %>
  </td>
  <td>
    <a href="#" id="deleteRow" onclick="$(this).closest('tr').remove();" class="btn btn-danger">
     Delete
    </a>
  </td>
</tr>

