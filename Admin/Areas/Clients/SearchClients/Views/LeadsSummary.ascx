<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<DomainModel.ReadModel.LeadView>>" %>
<%@ Import Namespace="AccurateAppend.Accounting" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<% if (!this.Model.Any())
   { %>
<div id="notice">
    <strong>No leads found.</strong></div>
<% }
   else
   { %>
  <div class="k-grid k-widget" style="margin-bottom: 40px;">
<table>
    <thead class="k-grid-header">
        <tr>
            <th class="k-header" style="text-align: center;">
                Email
            </th>
            <th class="k-header"  style="text-align: center;">
                Name
            </th>
            <th class="k-header" style="text-align: center;">
                Status
            </th>
            <th class="k-header" style="text-align: center;">
                Age (Days)
            </th>
            <th class="k-header" style="text-align: center;">
                Note Count
            </th>
            <th class="k-header" style="white-space: nowrap; text-align: center;">
                Last Update
            </th>
            <th class="k-header" style="white-space: nowrap;text-align: center;">
                Site
            </th>
            <th class="k-header" ></th>
        </tr>
    </thead>
    <tbody>
    <% foreach (var item in this.Model)
       { %>
        <tr>
        <% %>
            <td>
                <%= item.Email %>
            </td>
            <td>
                <%= PartyExtensions.BuildCompositeName(item.FirstName, item.LastName, item.BusinessName) %>
            </td>
            <td style="text-align: center;">
                <% var status = item.Status;
                   this.Response.Write(AccurateAppend.Core.EnumExtensions.GetDescription(status));
                %>
            </td>
            <td  style="text-align: right;">
                <%= this.Html.Encode(item.AgeInDays)%>
            </td>
            <td style="text-align: center;">
                <%= this.Html.Encode(item.NoteCount)%>
            </td>
            <td>
                <%= this.Html.Encode(String.Format("{0:g}", item.LastUpdate))%>
            </td>
            <td style="text-align: center;">
                <%= this.Html.Encode(item.ApplicationTitle)%>
            </td>
            <td style="text-align: center; width: 80px;"><%= this.Html.NavigationFor<LeadDetailController>().ToDetail(item.LeadId, "Details", new { @class= "btn btn-primary"}) %></td>
        </tr>
    
    <% } %>
    </tbody>
    </table>
  </div>
<% } %>

