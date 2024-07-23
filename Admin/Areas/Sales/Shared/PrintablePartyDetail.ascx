<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Guid>" %>

<div class="panel panel-default">
  <div class="panel-heading">Client Details</div>
  <div class="panel-body" id="clientDetail_<%: UniqueID %>">
  </div>
</div>
<script type="text/javascript">
  $(function() {
    console.log("Rendering client details at clientDetail_<%: UniqueID %>");

    $.ajax(
      {
        type: "GET",
        url: "<%= Url.Action("Read", "UserDetail", new {Area = "Clients", userId = Model}) %>",
        success: function(user) {
          var html = "";

          if (user.BusinessName != null) {
            html = "<strong>" + user.BusinessName + "</strong><br/>";
          } else {
            html = html + user.CompositeName + "<br />";
          }

          html = user.Address !==  "" ? html + user.Address + "<br />" : "";
          html = user.City !== "" ? html + user.City + ", " + user.State + " " + user.Zip + "<br/>" : "";
          html = user.Email !== "" ? html + user.Email + "<br />" : "";
          html = user.Phone !== "" ? html + user.Phone : "";

          $("#clientDetail_<%: UniqueID %>").html(html);
        }
      });

  });
</script>