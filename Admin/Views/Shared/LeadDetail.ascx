<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<int>" %>

<div class="panel panel-default">
  <div class="panel-heading">Lead Details</div>
  <div class="panel-body" id="leadDetail_<%: UniqueID %>">
  </div>
</div>
<script type="text/javascript">
  $(function() {

    $.ajax(
      {
        type: "GET",
        url: "<%= Url.Action("Read", "LeadDetail", new {Area = "Clients", leadId = Model}) %>",
        success: function(user) {
          var html = "";

          html = user.BusinessName !== "" ? "<strong>" + user.BusinessName + "</strong><br/>" : html;
          var name = (user.FirstName + ' ' + user.LastName).trim();
          html = name !== "" ? html + name + "<br />" : html;
          html = user.Email !== "" ? html + user.Email + "<br />" : html;
          html = user.Phone !== "" ? html + user.Phone + "<br />" : html;

          var contactLink = (user.Links.Detail != null)
            ? "<a class=\"btn btn-default\" href=\"<%= Url.Action("View", "LeadDetail", new {Area = "Clients", leadId = Model}) %>\">View Lead</a>"
            : "";

          html = html + "<div style=\"margin: 10px 0;\">" + contactLink + "</div>";

          $("#leadDetail_<%: UniqueID %>").html(html);
        }
      });

  });
</script>