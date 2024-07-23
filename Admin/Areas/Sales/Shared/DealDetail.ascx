<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Int32>" %>
<div class="panel panel-default">
 <div class="panel-heading">Deal Details</div>
 <div class="panel-body" id="dealDetail_<%: this.UniqueID %>">
   <div class="alert alert-info" style="margin-bottom: 20px;">Loading deal information</div>
 </div>
</div>
<script type="text/javascript">
 $(function() {
  console.log("Rendering deal details at dealDetail_<%: this.UniqueID %>");

  $.ajax(
   {
    type: "GET",
    url: "<%= Url.Action("ById", "DealsApi", new {Area = "Sales", dealId = this.Model}) %>",
    success: function(deal) {
      var html = "<strong>Title:</strong> " +
        deal.Title +
        "<br>" +
        "<strong>Description:</strong> " +
        deal.Description +
        "<br>" +
        "<strong>Amount (Suggested):</strong> " +
        (deal.Amount === 0
          ? "None"
          : (new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 }))
          .format(deal.Amount)) +
        "<br>" +
        "<strong>Deal Id:</strong> " +
        deal.DealId +
        "<br>" +
        "<strong>Date Added:</strong> " +
        deal.CreatedDate +
        "<br>" +
        "<strong>Status:</strong> " +
        deal.StatusDescription +
        "<br>" +
        "<strong>Auto-Bill:</strong> " + deal.EnableAutoBill + "<br/>" +
        "<strong>Internal Notifications:</strong> Disabled<br>" +
        "<div style=\"margin: 10px 0;\">";

      html = html + "</div>";

      $("#dealDetail_<%: this.UniqueID %>").html(html);
    }
   });

 });
</script>