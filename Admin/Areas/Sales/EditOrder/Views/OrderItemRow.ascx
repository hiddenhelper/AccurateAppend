<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderItemModel>" %>
<%@ Import Namespace="AccurateAppend.Sales.Contracts.ViewModels" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<%
    var item = this.Model ?? new OrderItemModel();
    var index = Guid.NewGuid().ToString();
    var userId = item.UserId;
    var productName = "Items[" + index + "].ProductName";
    var descriptionName = "Items[" + index + "].Description";
    var unitcostName = "Items[" + index + "].Cost";
    var qtyName = "Items[" + index + "].Quantity";
  %>
<tr>
  <% = this.Html.Hidden("Items.Index", index) %>
  <% = this.Html.Hidden("Items[" + index + "].Id", item.Id) %>
  <% = this.Html.Hidden("Items[" + index + "].UserId", item.UserId) %>
  <td>
    <%= this.Html.ProductDropDown(item.ProductName, productName, index + "_product") %>
  </td>
  <td>
    <input type="text" name="<%= descriptionName %>" id="<%= index + "_description" %>" value="<%= item.Description %>" class="form-control" style="width: 260px;"/>
  </td>
  <td>
    <input type="text" name="<%= qtyName %>" id="<%= index + "_qty" %>" value="<%= item.Quantity %>" class="form-control"/>
  </td>
  <td>
    <input type="text" name="<%= unitcostName %>" id="<%= index + "_unitcost" %>" value="<%= item.Cost %>" class="form-control"/>
  </td>
  <td>
    <%= this.Html.TextBox("cost", item.Total(), new { @class ="form-control" }) %>
  </td>
  <td>
    <a href="#" id="deleteRow" onclick="$(this).closest('tr').remove(); updateOrder();" class="btn btn-danger btn-xs">
     Delete
    </a>
  </td>
  <script type="text/javascript">
    $(function () {
      //bind product change
      $('#<%= index + "_product" %>').bind('change', function () {
        var selected = $('#<%= index + "_product" %> option:selected');
        var description = $('#<%= index + "_description" %>');

        description.val(selected.text());
      });
      
      //bind quantity change
      $('#<%= index + "_qty" %>').bind('change', function () {
        var selected = $('#<%= index + "_product" %> option:selected');
        var unitcost = $('#<%= index + "_unitcost" %>');
        var qty = $('#<%= index + "_qty" %>');
        var cost = $(this).closest('tr').find('input[name=cost]');

        $.getJSON("<%= this.Url.Action("Index", "ClientCost", new {Area = "Sales"}) %>", {
            name: $(selected).val(),
            qty: $(qty).val(),
            userid: "<%: userId %>",
            cost: $(unitcost).val()
          },
          function(json) {
            $(unitcost).val(json.unitcost);
            $(cost).val(parseFloat(json.unitcost * $(qty).val()).toFixed(2));
            updateOrder();
          });
      });
      
      //bind cost change
      $('#<%= index + "_unitcost" %>').bind('change', function () {
        var qty = $('#<%= index + "_qty" %>');
        var unitcost = $('#<%= index + "_unitcost" %>');
        var cost = $(this).closest('tr').find('input[name=cost]');
        
        $(cost).val(parseFloat($(unitcost).val() * $(qty).val()).toFixed(2));
        updateOrder();
      });
    });
  </script>
</tr>

