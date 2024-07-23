<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<Guid>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Processing...
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin: 0 0 20px 0;" id="title">Processing...</h2>
    <div class="row" style="padding: 0 0 20px 20px;">
     <div id="processing">
      We're updating the deal from the job processing report. You can wait or continue using the admin. Navigating away from this window will not affect the order being updated.
      <br />
      <img src="~/Images/circular loading.gif" alt="Loading" runat="server"/>
     </div>
        <div id="complete">
            
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
 <script type="text/javascript">
  var timer;

  $(function() {
   console.log("Rendering wait for <%: this.Model %>");
   $("#complete").hide();
   timer = window.setInterval("checkProgress()", 3000);
  });

  function checkProgress() {
   console.log("Checking progress");
   $.ajax(
   {
    type: "GET",
    url: "<%= this.Url.Action("QueryById", "OrdersApi", new {Area="Sales", publicKey = this.Model}) %>",
    success: function (result) {
     console.log("Is complete:" + (result.OrderId === null));

     if (result.OrderId) {
      window.clearInterval(timer);

      document.title = "Processed";
      $("#title").html("Complete");
      $("#processing").hide();
      var div = $("#complete");
      if (result.OrderId) {
       div.addClass("alert alert-success");
      } else {
       div.addClass("alert alert-danger");
      }
      div.html("Order updated<br />" + '<a href="<%= this.Url.Action("Index", "OrderDetail", new {area ="Sales"}) %>?orderId=' + result.OrderId + '">View Order</a>');

      div.show();
     }
    }
   });
  }
 </script>
</asp:Content>