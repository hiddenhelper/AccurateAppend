<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<ChangeOwnerModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.ChangeOwner.Models" %>
<%@ Import Namespace="AccurateAppend.Accounting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Change Ownership
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <% if (this.ViewData["Message"] != null)
     { %>
    <div class="alert-info"><%: this.ViewData["Message"] %></div>
  <% } %>
  <%= this.Html.ValidationSummary() %>
  
  <h3 style="margin-top: 0;">Change Ownership: <%: PartyExtensions.BuildCompositeName(this.Model.FirstName, this.Model.LastName, String.Empty) %></h3>
    <% using (this.Html.BeginForm()) { %>
    
      <%= this.Html.HiddenFor(m=>m.Id) %>
      <%= this.Html.HiddenFor(m=>m.FirstName) %>
       <div class="row" style="padding-right: 20px;">
         <table class="table table-condensed">
          <tr>
            <th>
              Owner
            </th>
            <td>
              <%= this.Html.AdminUsersDropDown(id: "OwnerId", userId: this.Model.OwnerId) %>
            </td>
          </tr>
           <tr>
             <th>
             </th>
             <td>
              <input type="submit" value="Update" class="btn btn-primary"/>  
             </td>
            </tr>
         </table>
       </div>
     <% } %>

</asp:Content>


