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
        "<span style='margin-right: 10px;'><strong>Order Date:</strong></span>" +
          order.DateOrdered +
          "<br>" +
          "<span style='margin-right: 10px;'><strong>Order Id:</strong></span>" +
          order.OrderId +
          "<br>";

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
     $("#orderDetail_<%: this.UniqueID %>").html(html);
    }
   });

 });
</script>