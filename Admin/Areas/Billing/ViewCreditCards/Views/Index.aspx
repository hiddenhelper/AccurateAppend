<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<ViewCreditCardsModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.DeleteCreditCard" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Billing.ChangePrimaryCard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Credit Card Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3 style="margin-top: 0;">
        Credit Card Details -
        <%: this.Model.UserName %>
    </h3>
    <div class="row" style="padding: 0 0 20px 20px;">
        <div class="col-md-5">
          <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId); %>
        </div>
      <div class="col-md-3">
        <div class="panel panel-default">
          <div class="panel-heading">Account Functions</div>
          <div class="panel-body">
            <%= this.Html.NavigationFor<CreateCreditCardController>().Create("Add new card", this.Model.UserId, new {@class = "btn btn-default"}) %>
            <%= this.Html.ActionLink("Send update email", "ForUser", "SendPaymentUpdate", new {Area = "Sales", this.Model.UserId}, new {@class = "btn btn-default"}) %>
            <% if (!this.Model.HasCard)
               { %>
              <br />
              <br />
              <div class="alert alert-info">
                No cards present for this client.
              </div>
            <% } %>
          </div>
        </div>
      </div>
    </div>
  <div class="row" style="padding: 0 0 20px 20px;">
    <div class="col-md-5">
      <%
      foreach (var card in this.Model.Cards)
      {
      %>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Card Details - <%: card.DisplayValue %><% if (card.IsPrimary) this.Response.Write(" (PRIMARY)"); %>
                        </div>
                        <div class="panel-body">
                            <table class="table table-striped">
                                <tr>
                                    <th style="width: 125px;">
                                        Business Name
                                    </th>
                                    <td>
                                        <%: card.BillTo.BusinessName %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Name
                                    </th>
                                    <td>
                                        <%: card.BillTo.ToDisplayName() %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Phone
                                    </th>
                                    <td>
                                        <%: card.BillTo.PhoneNumber %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Address
                                    </th>
                                    <td>
                                        <%: card.Address.Street %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        City
                                    </th>
                                    <td>
                                        <%: card.Address.City %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        State
                                    </th>
                                    <td>
                                        <%: card.Address.State %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Zip
                                    </th>
                                    <td>
                                        <%: card.Address.PostalCode %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Country
                                    </th>
                                    <td>
                                        <%: card.Address.Country %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Card Number
                                    </th>
                                    <td>
                                        <%: card.DisplayValue %>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        Card Exp
                                    </th>
                                    <td>
                                        <%: card.Expiration %>
                                    </td>
                                </tr>
                             <% if (card.ExternalProfileId != null)
                                { %>
                                <tr>
                                    <th>
                                      <a href="https://account.authorize.net/UI/themes/anet/CustomerProfile/ViewCustomerProfile.aspx?ProfileID=<%: card.ExternalProfileId %>" target="_new" title="Requires Existing AuthNet Session">AuthNET</a>
                                    </th>
                                </tr>
                             <% } %>
                            </table>
                            <div style="margin-bottom: 20px;">
                                <% if (card.CanMakePrimary)
                                   { %>
                                <%= this.Html.NavigationFor<ChangePrimaryCardController>().MakePrimary("Make Primary", card.BillTo.Id, new {@class = "btn btn-default", @style = "margin-right: 3px;"}) %>
                                <% } %>
                                <% if (card.CanUpdateBilling)
                                   { %>
                                <%= this.Html.NavigationFor<EditCreditCardController>().EditBillingAddress("Edit Address", card.BillTo.Id, new {@class = "btn btn-default", @style = "margin-right: 3px;"}) %>
                                <% } %>
                                <%= this.Html.NavigationFor<DeleteCreditCardController>().Remove("Remove", card.BillTo.Id, new {id = "removeLink", @class = "btn btn-danger"}) %>
                            </div>
                        </div>
                    </div>
                
      <%
      }
      %>
    </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>