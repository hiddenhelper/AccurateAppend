<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CostModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models" %>

<%
  var item = Model ?? new CostModel();
  var index = Guid.NewGuid().ToString();
  var floor = "model.Costs[" + index + "].Floor";
  var ceiling = "model.Costs[" + index + "].Ceiling";
  var perRecord = "model.Costs[" + index + "].PerRecord";
  var perMatch = "model.Costs[" + index + "].PerMatch";
%>
<tr>
  <td>
    <input type="text" name="<%= floor %>" id="<%= index + "_floor" %>" maxlength="100" value="<%= item.Floor %>" class="form-control" />
  </td>
  <td>
    <input type="text" name="<%= ceiling %>" id="<%= index + "_ceiling" %>" maxlength="100" value="<%= item.Ceiling %>" class="form-control" />
  </td>
  <td>
   <input type="text" name="<%= perRecord %>" id="<%= index + "_perRecord" %>" maxlength="100" value="<%= item.PerRecord %>" class="form-control" onchange="$('#<%= index + "_perMatch" %>').val($('#<%= index + "_perRecord" %>').val())"/>
  </td>
  <td>
   <input type="text" name="<%= perMatch %>" id="<%= index + "_perMatch" %>" maxlength="100" value="<%= item.PerMatch %>" class="form-control"/>
  </td>
  <td>
    <a href="#" id="deleteRow">
      <a href="#" class="btn-sm btn-danger"  onclick="$(this).closest('tr').remove();updateView();">Delete</a>
    </a>
      <% = Html.Hidden("model.Costs.Index", index) %>
  </td>
</tr>

