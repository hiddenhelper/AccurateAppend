<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Int32>" %>

<div class="panel panel-default">
 <div class="panel-heading">
  Order Items
 </div>
 <div class="panel-body" id="orderItems_<%: this.UniqueID %>">
  <div class="alert alert-info">No order items</div>
 </div>
</div>

<script type="text/javascript">
 $(function() {
  console.log("Rendering order items at orderItems_<%: this.UniqueID %>");

  $.ajax(
   {
    type: "GET",
    url: "<%= this.Url.Action("LinesForOrder", "OrdersApi", new { Area = "Sales", orderId = this.Model })  %>",
    success: function (order) {
     if (order.Items.length === 0) return;

     var html =
      "<tr>" +
        "<th>Product</th>" +
        "<th>Description</th>" +
        "<th style=\"text-align: right;\">Quantity</th>" +
        "<th style=\"text-align: right;\">Unit Cost</th>" +
        "<th style=\"text-align: right;\">Cost</th>" +
       "</tr>";

     var formatter4 = new Intl.NumberFormat("en-US",
                        {
                         style: "currency", currency: "USD",
                         minimumFractionDigits: 4
                        });
     var formatter2 = new Intl.NumberFormat("en-US",
                        {
                         style: "currency", currency: "USD",
                         minimumFractionDigits: 2
                        });

     order.Items.forEach(function(item) {
      html = html +
       "<tr>" +
        "<td style=\"white-space: nowrap;\">" + item.Key + "</td>" +
        "<td style=\"white-space: nowrap;\">" + item.Description + "</td>" +
        "<td style=\"text-align: right;\">" + item.Quantity + "</td>" +
        "<td style=\"text-align: right;\">" + formatter4.format(item.Price) + "</td>" +
        "<td style=\"text-align: right;\">" + formatter2.format(item.Total) + "</td>" +
       "</tr>";
     });
      
     if (order.CostAdjustment < 0) {
      html = html + "<tr><td style=\"text-align: right;\" colspan=\"4\">[Credit]</td><td>" + formatter2.format(order.CostAdjustment) + "</td></tr>";
     }
     else if (order.CostAdjustment > 0) {
      html = html + "<tr><td style=\"text-align: right;\" colspan=\"4\">[Minimum]</td><td style=\"text-align: right;\">" + formatter2.format(order.CostAdjustment) + "</td></tr>";
     }

     html = html + 
      "<tr><td colspan=\"4\"><label>Total</label></td><td style=\"text-align: right; font-weight: bold;\">" + formatter2.format(order.Total) + "</td></tr>";
      
     html = "<table class=\"table table-striped\">" + html + "</table>";

     $("#orderItems_<%: this.UniqueID %>").html(html);
    }
   });

 });
</script>