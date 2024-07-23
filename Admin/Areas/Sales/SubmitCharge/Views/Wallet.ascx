<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Guid>" %>
<div id="walletDetail_<%: this.UniqueID %>">
 <div class="alert alert-info" >No cards in wallet</div>
</div>

<script type="text/javascript">
 $(function() {
  console.log("Rendering wallet details at walletDetail_<%: this.UniqueID %>");

  $.ajax(
   {
    type: "GET",
    url: "<%= this.Url.Action("QueryByUser", "ProfileApi", new { Area = "Billing", userId = this.Model })  %>",
    success: function(cards) {
     if (cards.length === 0) return;

     var html = "";
     
     cards.forEach(function(item) {
      html = html +
      "<input type=\"radio\" name=\"cardId\" value=\"" + item.PublicKey + "\" " + (item.IsPrimary ? " checked=\'checked\'" : "") + " />" +
      "<table class=\"table table-condensed\">" +
       "<tr>" +
        "<th>Business Name</th>" +
        "<td>" + item.BusinessName + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Name</th>" +
        "<td>" + item.Name + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Phone</th>" +
        "<td>" + item.PhoneNumber + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Address</th>" +
        "<td>" + item.Address.replace(/(?:\r\n|\r|\n)/g, '<br />') + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>City</th>" +
        "<td>" + item.City + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>State</th>" +
        "<td>" + item.State + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Zip</th>" +
        "<td>" + item.PostalCode + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Card Number</th>" +
        "<td>" + item.DisplayValue + "</td>" +
       "</tr>" +
       "<tr>" +
        "<th>Card Exp</th>" +
       "<td>" + item.Expiration + "</td>" +
      "</tr>" +
     "</table>";
     });
      
     
      $("#walletDetail_<%: this.UniqueID %>").html(html);
    }
   

   });
 });
</script>