<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Int32>" %>
<div class="panel panel-default">
 <div class="panel-heading">Order Details</div>
 <div class="panel-body" id="orderDetail_<%: this.UniqueID %>">
 </div>
</div>
<script type="text/javascript">
 $(function() {
  console.log("Rendering order details at orderDetail_<%: this.UniqueID %>");

  $.ajax(
   {
    type: "GET",
    url: "<%= this.Url.Action("QueryById", "OrdersApi", new { Area = "Sales", orderId = this.Model })  %>",
    success: function(order) {
     var html =
      "<strong>Order Date:</strong>" + order.DateOrdered + "<br>" +
      "<strong>Order Id:</strong>" + order.OrderId + "<br>" +
      "<strong>Deal Id:</strong>" + order.DealId + "<br>" +
      "<strong>Status:</strong>" + order.Status + "<br>";

      var formatter = new Intl.NumberFormat("en-US",
                        {
                         style: "currency", currency: "USD",
                         minimumFractionDigits: 2
                        });
     if (order.CostAdjustment < 0) {
      html = html + "<strong>Credit:</strong>" + formatter.format(order.CostAdjustment) + "<br/>";
     }
     else if (order.CostAdjustment > 0) {
      html = html + "<strong>Minimum:</strong>" + formatter.format(order.CostAdjustment) + "<br/>";
     }

     var editLink = (order.Links.Edit != null) ? "<a class=\"btn btn-default\" href=\"" + order.Links.Edit + "\">Edit Order</a>" : "";
     var dealLink = (order.Links.Deal != null) ? "<a class=\"btn btn-default\" href=\"" + order.Links.Deal + "\">View Deal</a>" : "";
     var messageLink = (order.Links.Bill != null) ? "<a class=\"btn btn-default\" href=\"" + order.Links.Bill + "\">View Bill</a>" : "";

     html = html + "<div style=\"margin: 10px 0;\">" + editLink + " " + dealLink + " " + messageLink + "</div>";
     
     $("#orderDetail_<%: this.UniqueID %>").html(html);
    }
   });

 });
</script>