<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<UserSearchResult>>" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="AccurateAppend.Security" %>
<%@ Import Namespace="AccurateAppend.Websites" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.SearchClients.Models" %>

<div class="k-grid k-widget" style="margin-bottom: 40px;">
<table>
    <thead class="k-grid-header">
        <tr>
            <th class="k-header" style="text-align: center;">
                E-mail
            </th>
            <th class="k-header" style="text-align: center;" >
                Company
            </th>
            <th class="k-header" style="text-align: center;">
                Name
            </th>
            <th class="k-header" style="text-align: center;">
                Last Activity
            </th>
            <th class="k-header" style="text-align: center;">
                Site
            </th>
            <th class="k-header" style="text-align: center;">
                
            </th>
        </tr>
    </thead>
    <tbody>
        <% foreach (var item in this.Model) { %>
        <tr>
            <td>
                <%: item.UserName.ToLower() %>
            </td>
            <td>
                <%: item.BusinessName.ToTitleCase() %>
            </td>
            <td>
                <%: item.FirstName.ToTitleCase() + " " + item.LastName.ToTitleCase() %>
            </td>
            <td style="white-space: nowrap; text-align: center;">
                <%: item.LastActivityDate.ToUserLocal() %>
            </td>
            <td style="text-align: center;">
                <%= SiteCache.Cache.Where(s => s.ApplicationId == item.ApplicationId).Select(s => s.Title).FirstOrDefault() %>
            </td>
            <td  style="text-align: center; width: 170px;">
                <a class="btn btn-default" href="<%= item.NewDealUrl %>">New Deal</a>
                <a class="btn btn-primary" href="<%= item.DetailUrl %>">Details</a>            
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
</div>
