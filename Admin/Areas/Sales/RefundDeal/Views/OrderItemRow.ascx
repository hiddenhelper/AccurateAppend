<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderItemModel>" %>
<%@ Import Namespace="AccurateAppend.Sales.Contracts.ViewModels" %>
<%
  var item = Model ?? new OrderItemModel();
  var index = Guid.NewGuid().ToString();
  var productName = "Items[" + index + "].ProductName";
  var descriptionName = "Items[" + index + "].Description";
  var unitcostName = "Items[" + index + "].Cost";
  var qtyName = "Items[" + index + "].Quantity";
%>
<tr>
  <% = Html.Hidden("Items.Index", index) %>
  <% = Html.Hidden("Items[" + index + "].Id", item.Id) %>
  <td>
    <%= Html.TextBox(productName, item.ProductName, new {@readonly="readonly"}) %>
  </td>
  <td>
    <input type="text" name="<%= descriptionName %>" id="<%= index + "_description" %>" value="<%= item.Description %>" class="form-control" style="width: 260px;"/>
  </td>
  <%
    if (item.Maximum != null)
    {
  %>
  <td>
    <input type="text" name="<%= qtyName %>" id="<%= index + "_qty" %>" value="<%= item.Quantity %>" class="form-control" size="5"/> / <%= item.Maximum %>
  </td>
  <td>
    <input type="text" name="<%= unitcostName %>" id="<%= index + "_unitcost" %>" value="<%= item.Cost %>" readonly="readonly" class="form-control"/>
  </td>
    <%
    }
    else
    {
    %>
    <td>
      <input type="text" name="<%= qtyName %>" id="<%= index + "_qty" %>" value="<%= item.Quantity %>" class="form-control" size="5"/>
    </td>
    <td>
      <input type="text" name="<%= unitcostName %>" id="<%= index + "_unitcost" %>" value="<%= item.Cost %>" class="form-control"/>
    </td>
    <%
    }
    %>
  <td>
    <%= this.Html.TextBox("cost", item.Total(), new { @class ="form-control" }) %>
  </td>
</tr>
<script type="text/javascript">
  $(function () {
      
    //bind quantity change
    $('#<%= index + "_qty" %>').bind('change', function () {
      var unitcost = $('#<%= index + "_unitcost" %>');
      var qty = $('#<%= index + "_qty" %>');

      var cost = $(this).closest('tr').find('input[name=cost]');

      $(cost).val(parseFloat(unitcost.val() * $(qty).val()).toFixed(2));
          updateOrder();
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
