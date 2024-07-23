<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<ViewAccountsModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.ViewAccounts.Models" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Service Account Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="row" style="padding: 0 0 20px 20px;">
    <div class="row">
      <div class="col-md-5">
        <h3>
          Automatic Recurring Payments
        </h3>
        <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId); %>
      </div>
    </div>

    <div class="row">
      <div class="col-md-5" style="margin-bottom: 20px;">
        <%= this.Html.ActionLink("Create New", "Start", "ContractWizard", new {Area = "Clients", userId = this.Model.UserId}, new {@class = "btn btn-primary"}) %>
      </div>
    </div>

    <div class="row">
      <div class="col-md-5">
        <% if (this.Model.CurrentAccount == null)
           { %>
          <div class="alert alert-info">
            No active service accounts present for this client.
          </div>
        <% } %>
        <%
           foreach (var account in this.Model.Accounts)
           {
        %>
          <div class="panel panel-default">
            <div class="panel-heading">
              Account Details - <%: account.Type.GetDescription() %><% if (account.IsCurrent) this.Response.Write(" (CURRENT)"); %>
            </div>
            <div class="panel-body">
              <table class="table table-striped">
                <tr>
                  <th style="width: 125px;">
                    Effective Dates
                  </th>
                  <td>
                    <%: account.EffectiveDate.ToString("d") %> - <%: account.EndDate.HasValue ? account.EndDate.Value.ToString("d") : " (now)" %>
                  </td>
                </tr>
                <tr>
                  <th style="width: 125px;">
                    Billing Cycle
                  </th>
                  <td>
                    <%: account.Recurrance %>
                  </td>
                </tr>
                <% if (account.Type != AccountType.UsageOnly)
                   { %>
                  <tr>
                    <th>
                      Amount
                    </th>
                    <td>
                      <%: account.Amount.Value.ToString("C") %> <% if (account.IsFixedRate) this.Response.Write(" (FIXED RATE)"); %>
                    </td>
                  </tr>
                <% } %>
                <tr>
                  <th style="width: 125px;">
                    Custom Billing?
                  </th>
                  <td><%: account.SpecialProcessing %></td>
                </tr>
                <% if (account.Limit != null)
                   { %>
                  <tr>
                    <th style="width: 125px;">
                      Balance Allowed
                    </th>
                    <td><%: account.Limit.Value.ToString("C") %></td>
                  </tr>
                <% } %>
                <% if (account.IsCurrent || account.EffectiveDate > DateTime.Today)
                   { %>
                  <tr>
                    <td>
                      <%= this.Html.ActionLink("Cancel", "Index", "CancelAccount", new {Area = "Clients", userId = this.Model.UserId, RedirectTo = this.Url.Action("Index", "ViewAccounts", new {Area = "Clients", userId = this.Model.UserId})}, new {@class = "btn btn-danger"}) %>
                    </td>
                  </tr>
                <% } %>
              </table>
            </div>
          </div>
        <%
           }
        %>
      </div>
    </div>
  </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>