<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<Guid>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Processing...
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin: 0 0 20px 0;" id="title">Processing...</h2>
    <div class="row" style="padding: 0 0 20px 20px;">
     <div id="processing">
      We're are currently contacting the remote payment gateway to process this charge. You can wait or continue using the admin. Navigating away from this window will not affect the charge being completed.
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
    url: "<%= this.Url.Action("CheckTransaction", "OrdersApi", new {Area="Sales", publicKey = this.Model}) %>",
    success: function (result) {
     console.log("Is complete:" + result.Complete);

     if (result.Complete) {
      window.clearInterval(timer);

      document.title = "Processed";
      $("#title").html("Complete");
      $("#processing").hide();
      var div = $("#complete");
      if (result.Success) {
       div.addClass("alert alert-success");
      } else {
       div.addClass("alert alert-danger");
      }
      div.html(result.Message + "<br />" + '<a href="<%= this.Url.BuildFor<DealSummaryController>().ToIndex() %>">Return to deals</a>');

      div.show();
     }
    }
   });
  }
 </script>
</asp:Content>