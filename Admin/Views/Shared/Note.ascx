<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<NoteModel>>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.ViewModels.Common" %>
<% if (!this.Model.Any())
   { %>
<div id="notice">
  <strong>No notes found.</strong>
</div>
<% }
   else
   { %>
<table width="100%" cellpadding="0" cellspacing="0">
  <thead>
    <tr>
      <th scope="col" style="width: 140px;"><%= Html.LabelFor(m => m.First().CreatedDate) %>
      </th>
      <th scope="col" style="width: 180px;"><%= Html.LabelFor(m => m.First().CreatedBy) %>
      </th>
      <th scope="col">&nbsp;
      </th>
    </tr>
  </thead>
  <tbody>
    <% foreach (var item in this.Model)
       {
    %>
    <tr>
      <td>
        <%= Html.StandardDateDisplay(item.CreatedDate) %>
      </td>
      <td>
        <%: item.CreatedBy %>
      </td>
      <td>
        <%= item.Content.FormatHtml() %>
      </td>
    </tr>
    <% } %>
  </tbody>
</table>
<% } %>
