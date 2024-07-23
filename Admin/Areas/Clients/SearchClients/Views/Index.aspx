<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<SearchResultModel>" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.Clients.SearchClients.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Search
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Clients</h3>
    <% var users = this.Model.Users; %>
    <% if (!users.Any()) { %>
    <div class="alert alert-info">No clients found</div>
    <% } else { %>
    <%
     this.Html.RenderPartial("UserSummary", users); %>
    <% } %>
    <h3>Leads</h3>
    <% var leads = this.Model.Leads; %>
    <% if (!leads.Any()) { %>
    <div class="alert alert-info">No leads found</div>
    <% } else { %>
    <%
     this.Html.RenderPartial("LeadsSummary", leads); %>
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>
