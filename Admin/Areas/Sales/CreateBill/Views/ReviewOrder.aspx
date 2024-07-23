<%@ Page Title="Title" Language="C#" Inherits="System.Web.Mvc.ViewPage<ReviewOrderModel>" MasterPageFile="~/Views/Shared/bootstrap3.Master" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Bill Order
</asp:Content>

<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
  <% using (Html.BeginForm())
      { %>
    <%: this.Html.HiddenFor(m => m.DealId) %>
    <%: this.Html.HiddenFor(m => m.UserId) %>
    <%: this.Html.HiddenFor(m => m.OrderId) %>
     <h2 style="margin-top: 0;">Bill Order</h2>
     <div class="row">
       <div class="col-md-4">
         <% this.Html.RenderPartial("~/Areas/Sales/Shared/OrderDetail.ascx", this.Model.OrderId); %>
      </div>
      <div class="col-md-6">
        <% Html.RenderPartial("~/Areas/Sales/Shared/DealDetail.ascx", Model.DealId); %>
      </div>
      <div class="col-md-6">
        <div class="panel panel-default">
          <div class="panel-heading">Billing Options</div>
          <div class="panel-body">
            <div class="alert alert-info" id="alertInfo" style="margin-bottom: 20px;">Loading payment options</div>
            Indicate if this order should be invoiced or billed immediately when approved. Customers without active cards can only be invoiced.
            <div class="checkbox">
              <label>
                <%= Html.RadioButtonFor(m => m.BillType, BillType.Invoice, new { id = "invoiceBilling" }) %> <%= Html.LabelFor(m => m.BillType, "Invoice") %>
              </label>
            </div>
            <div class="checkbox">
              <label>
                <%= Html.RadioButtonFor(m => m.BillType, BillType.Receipt, new { id = "chargeBilling", disabled = true }) %> <%= Html.LabelFor(m => m.BillType, "Bill Card") %>
              </label>
            </div>
          </div>
        </div>
        <input class="btn btn-primary" type="submit" id="submit" disabled="disabled" value="Proceed To Next Step"/>
      </div>
     </div>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  <script type="text/javascript">

    $(function() {
        viewModel.loadPaymentOptions();
    });

    var viewModel = {
        loadPaymentOptions() {
        $.ajax(
          {
            type: "GET",
            url: "<%= this.Url.Action("QueryByUser", "ProfileApi", new { Area = "Billing", userId = this.Model.UserId })  %>",
            success: function(cards) {
              console.log("Card count: " + cards.length);

              // If cards on file, enable & select
              if (cards.length > 0) {
                $("#chargeBilling").prop('disabled', false);
                $("#chargeBilling").prop('checked', true);
              }

              // remove notice
              $("#alertInfo").remove();

              // enable form
              $("#submit").prop('disabled', false);
            },
            error: function (xhr, ajaxOptions, thrownError) {
              $("#alertInfo").html("Cannot render payment options");
            }
          });
      }
    };
  </script>

</asp:Content>