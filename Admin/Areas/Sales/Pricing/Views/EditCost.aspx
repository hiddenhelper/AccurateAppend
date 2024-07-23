<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<RateCardModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Edit Rate Card
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  <script type="text/javascript">

    $(function() {

      updateView();
      <% if (this.Model.UserId.HasValue)
         { %>
        
      $("#copyExistingButton").show();
      $("#copyExistingButton")
        .click(function() {
          window.location.replace(
            '<%= this.Url.BuildFor<PricingController>().ToCopyExisting(this.Model.Product, this.Model.UserId.Value) %>');
        });
      <% } %>
    });

    function insertCostRow() {
      $.get('<%= this.Url.Action("CostRow", "Pricing", new {area = "Sales"}) %>?floor=0&ceiling=0',
        function(data) {
          var lastCeiling = parseInt($($('#costs tr:last td input')[1]).attr("value"));
          $('#costs tr:last').after(data);
          if ($('#costs tr').length > 2) {
            $($('#costs tr:last td input')[0]).val(lastCeiling + 1);
          }
          updateView();
        });
    };

    function resetCostRows() {
      $('#costs').find("tr:gt(0)").remove();
      var rows = [
        [0, 25000],
        [25001, 50000],
        [50001, 100000],
        [100001, 250000],
        [250001, 500000],
        [500001, 1000000],
        [1000001, 2147483647]
      ];
      $.each(rows,
        function(i, v) {
          $.ajax({
            url: '<%= this.Url.Action("CostRow", "Pricing", new {area = "Sales"}) %>?floor=' +
              v[0] +
              '&ceiling=' +
              v[1],
            type: "get",
            async: false,
            success: function(data) {
              $('#costs tbody').append(data);
            }
          });
        });
      updateView();
    };

    function updateView() {
      if ($("#costs tr").length > 1) {
        $("#costs").show();
        $("#message").hide();
      } else {
        console.log("hiding table ");
        $("#costs").hide();
        $("#message").text("No rate card found").show();
      }
    };


  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="row" style="padding: 0 20px;">

    <div class="row">
      <div class="col-md-5">
        <h3 style="margin-bottom: 20px;">
          Edit Rate Card
        </h3>
        <% if (this.Model.UserId != null)
           { %>
          <% this.Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", this.Model.UserId ?? Guid.Empty); %>
        <% }
           else
           { %>
          <p>Category: <%: Model.CardName %></p>
        <% } %>
        <p>Card Name: <%: Model.Product %></p>
      </div>
    </div>
    <%
      var message = ViewData["Message"] as String;
      if (!string.IsNullOrWhiteSpace(message))
      {
    %>
      <div class="alert alert-success"><%: message %></div>
    <% } %>

    <% using (Html.BeginForm()) %>
    <%
       { %>
      <%: Html.ValidationSummary(false) %>
      <a href="#" class="btn btn-default" id="copyExistingButton">Copy Existing</a>
      <a href="<%= this.Url.BuildFor<PricingController>().ToIndex() %>?userid=<%= Model.UserId %>" class="btn btn-default">View All Cards</a>
      <a href="#" class="btn btn-default" onclick="javascript:insertCostRow();">Add Row</a>
      <a href="#" class="btn btn-default" onclick="javascript:resetCostRows();">Reset Rows</a>
      <a href="<%= this.Url.BuildFor<PricingController>().ToIndex() %>?userid=<%= Model.UserId %>" class="btn btn-default">Cancel</a>
      <a href="#" class="btn btn-default" onclick="$(this).closest('form').submit()">Save</a>
      <table id="costs" class="table table-striped">
        <thead>
        <tr>
          <th>Floor</th>
          <th>Ceiling</th>
          <th>Per Record</th>
          <th>Per Match</th>
          <th></th>
        </tr>
        </thead>
        <tbody>
        <% foreach (var item in this.Model.Costs)
           {
             Html.RenderPartial("CostRow", item);
           } %>
        </tbody>

      </table>

      <div id="message" class="alert alert-info" style="margin-top: 20px;">No rate card found</div>


      <input type="hidden" name="model.Product" value="<%= Model.Product %>"/>
      <input type="hidden" name="model.CardName" value="<%= Model.CardName %>"/>
      <input type="hidden" name="model.Userid" value="<%= Model.UserId %>"/>
      <input type="hidden" name="model.UserName" value="<%= Model.UserName %>"/>

    <% } %>

  </div>

</asp:Content>