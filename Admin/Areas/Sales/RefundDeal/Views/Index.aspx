<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<RefundDetail>" %>
<%@ Import Namespace="AccurateAppend.Sales.Contracts.ViewModels" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Order
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">


        $(function() {
            
            updateView();

            $("#CostAdjustment").change(function() {
                updateOrder();
            });

        });
        
        function updateOrder() {
            console.log("updating order");
            var sum = 0;
            
            $.each($("table#orderitems [name=cost]"), function(k, v) {
                var c = parseFloat($(v).val());
                if (!isNaN(c))
                    sum += c;
            });

            var adjustment = parseFloat($("#CostAdjustment").val());
            if (adjustment > 0) {
                sum = Math.max(adjustment, sum);
            } else if (adjustment < 0) {
                sum = sum + adjustment;
                sum = Math.max(sum, 0);
            }

            $("table#orderitems [id=totalcost]").text(sum.toFixed(4));

            updateView();
        };

        function updateView() {
            if ($("table#orderitems tr").length > 2) {
                $("#orderItemsSection").show();
                $("#orderItemAlert").hide();
            } else {
                $("#orderItemsSection").hide();
                $("#orderItemAlert").show();
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% using (this.Html.BeginForm("Index", "RefundDeal", new {Area = "Sales"}, FormMethod.Post))
       { %>
    <%: this.Html.HiddenFor(m => m.Id) %>
    <%: this.Html.HiddenFor(m => m.UserId) %>
    <%: this.Html.HiddenFor(m => m.PublicKey) %>
    <div class="row">
      <div class="col-md-4">
         <% this.Html.RenderPartial("~/Areas/Sales/Shared/OrderDetail.ascx", this.Model.Id); %>
      </div>
      <div class="col-md-4">
        <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId); %>
      </div>
    </div>
    <div class="row" style="padding: 0 0 20px 0;">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">Order Items</div>
                <div class="panel-body">
                    <section id="orderItemsSection" style="display: none;">
                        <table id="orderitems" class="table table-striped">
                            <thead>
                            <tr>
                                <th><%: this.Html.LabelFor(m => m.Items.First().ProductName) %></th>
                                <th><%: this.Html.LabelFor(m => m.Items.First().Description) %></th>
                                <th><%: this.Html.LabelFor(m => m.Items.First().Quantity) %></th>
                                <th><%: this.Html.LabelFor(m => m.Items.First().Cost) %></th>
                                <th>Total</th>
                            </tr>
                            </thead>
                            <tbody>
                            <% foreach (var item in this.Model.Items)
                               {
                                   this.Html.RenderPartial("~/Areas/Sales/RefundDeal/Views/OrderItemRow.ascx", item);
                               }
                            %>
                            </tbody>
                            <tfoot>
                            <tr id="ordertotalrow">
                                <td colspan="4">
                                    Total
                                </td>
                                <td id="totalcost">
                                   <%: this.Model.Items.Sum(i => i.Total()).ToString("C") %>
                                </td>
                            </tr>
                            </tfoot>
                        </table>
                    </section>
                    <div id="orderItemAlert" class="alert alert-info" style="display: none;">No items in this order</div>
                  <a class="btn btn-warning" href="<%= this.Url.BuildFor<OrderDetailController>().Detail(this.Model.Id) %>">
                    Cancel
                  </a>
                  <a class="btn btn-primary" href="#" onclick="$(this).closest('form').submit()">
                    Save
                  </a>
                </div>
            </div>
        </div>
        <% } %>
    </div>
    </asp:Content>