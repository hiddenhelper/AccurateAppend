<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<OrderDetailView>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Order Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h3 style="margin-top: 0">Order Detail</h3>
  <div class="row">
    <div class="col-md-4">
      <% this.Html.RenderPartial("~/Areas/Sales/Shared/OrderDetail.ascx", this.Model.OrderId); %>
    </div>
    <div class="col-md-4">
      <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId); %>
    </div>
    <div class="col-md-4">
        <%--<% if (this.Model.Deal != null && (!String.IsNullOrEmpty(this.Model.Deal.Instructions))) { %>
            <div class="panel panel-default">
                <div class="panel-heading">Deal Instructions</div>
                <div class="panel-body">
                    <%= Html.Encode(this.Model.Deal.Instructions ?? String.Empty).Replace("\r\n", "<br />") %>
                </div>
            </div>
        <% } %>--%>
    </div>
</div>
  <div class="row">
    <div class="col-md-12">
        <div class="panel panel-default">
            <div class="panel-heading">
                Order Items
            </div>
            <div class="panel-body" id="orderItems">
              <div class="alert alert-info">No order items</div>
            </div>
        </div>
    </div>

</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  
  <script type="text/javascript">

    $(function() {
      viewModel.renderLineItems();
    });

        var viewModel = {
            renderLineItems: function() {
                console.log("Rendering order items");

                $.ajax(
                  {
                        type: "GET",
                        url: "<%= this.Url.Action("LinesForOrder", "OrdersApi", new {area ="Sales", orderId = this.Model.OrderId }) %>",
                        success: function(result) {

                          var html = "<table class=\"table table-striped\">" +
                            "<tr><th>Product</th>" +
                            "<th>Description</th>" +
                            "<th style=\"text-align: right;\">Quantity</th>" +
                            "<th style=\"text-align: right;\">Unit Cost</th>" +
                            "<th style=\"text-align: right;\">Cost</th></tr>";

                            $.each(result.Items, function (i, v) {
                              html = html +
                                "<tr>" +
                                "<td style=\"white-space: nowrap;\">" + v.Key + "</td>" +
                                "<td style=\"white-space: nowrap;\">" + v.Description + "</td>" +
                                "<td style=\"text-align: right;\">" + new Intl.NumberFormat("en-US",{}).format(v.Quantity).replace(/\.0+$/, "") + "</td>" +
                                "<td style=\"text-align: right;\">" + (new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 4 })).format(v.Price) + "</td>" +
                                "<td style=\"text-align: right;\">" + (new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 })).format(v.Total) + "</td>" +
                                "</tr>";
                            });

                          html = html + "<tr><td colspan=\"4\">Total</td><td style=\"text-align: right; font-weight: bold;\">" + (new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 })).format(result.Total) + "</td></tr>";
                          html = html + "</table>";

                          $("#orderItems").html(html);
                        }
                  });
            }
        }

    </script>

</asp:Content>