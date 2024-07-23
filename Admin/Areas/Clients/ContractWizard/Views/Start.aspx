<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<Party<Guid>>" %>

<%@ Import Namespace="AccurateAppend.Websites.Admin.ViewModels.Common" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Create Recurring Billing
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% using (Html.BeginForm("Details", "ContractWizard", FormMethod.Post))
     {
  %>
    <%: Html.Hidden("userId", Model.Id) %>
    <div class="row" style="padding: 0 0 0px 20px;">
      <div class="row">
        <div class="col-md-4">
          <% Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", Model.Id); %>
        </div>
      </div>
      <div class="row">
        <div class="col-md-6">
          <h4>Create New Contract</h4>
          <table class="table table-bordered">
            <tr>
              <th></th>
              <th>Type</th>
              <th>Description</th>
            </tr>
            <tr>
              <td>
                <input type="radio" name="type" id="type_subscription" value="<%: ContractWizardController.ContractType.Subscription %>" checked="checked"/>
              </td>
              <td style="white-space: nowrap">Pre-payment</td>
              <td>Accounts prepay a fixed amount at the start of the period. Throughout the period the tally of all matches/inputs for billing which are totaled and if exceeding the prepayment, will be billed at the close of the period or optionally after an indicated amount.</td>
            </tr>
            <tr>
              <td>
                <input type="radio" name="type" id="type_fixedrate" value="<%: ContractWizardController.ContractType.FixedRate %>"/>
              </td>
              <td style="white-space: nowrap">Fixed-rate</td>
              <td>Accounts prepay a fixed amount at the start of the period regardless of use.</td>
            </tr>
            <tr>
              <td>
                <input type="radio" name="type" id="type_usage" value="<%: ContractWizardController.ContractType.Usage %>"/>
              </td>
              <td style="white-space: nowrap">Usage Only</td>
              <td>Accounts tally all matches/inputs for billing which is totaled and billed at the end of a period or after an indicated amount</td>
            </tr>
            <tr>
              <td>
                <input type="radio" name="type" id="type_accrual" value="<%: ContractWizardController.ContractType.Accrual %>"/>
              </td>
              <td style="white-space: nowrap">Accruing Total</td>
              <td>Accounts will tally and total all matches/inputs until the balance exceeds an indicated amount OR the end of the contract is met, whichever occurs first.</td>
            </tr>
            <tr>
              <td>
                <input type="radio" name="type" id="type_paidtest" value="<%: ContractWizardController.ContractType.PaidTest %>"/>
              </td>
              <td style="white-space: nowrap">Paid Test</td>
              <td>Same as an Accruing Total account but with a required end of the trial period</td>
            </tr>
          </table>
          <input type="submit" value="Next->" title="Next" class="btn btn-primary"/>
        </div>
      </div>
    </div>

  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

</asp:Content>