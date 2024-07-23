<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<Guid>" %>

<div class="panel panel-default">
    <div class="panel-heading">Client Details</div>
    <div class="panel-body" id="clientDetail_<%: this.UniqueID %>">
    </div>
</div>
<script type="text/javascript">
 $(function() {
  console.log("Rendering client details at clientDetail_<%: this.UniqueID %>");

  $.ajax(
   {
    type: "GET",
    url: "<%= this.Url.Action("Read", "UserDetail", new { Area = "Clients", userId = this.Model })  %>",
    success: function(user) {
     var html = "";

     html = (user.BusinessName || "") !== "" ? "<strong>" + user.BusinessName + "</strong><br/>" : html;
     html = user.CompositeName !== "" ? html + user.CompositeName + "<br />" : html;
     html = user.Email !== "" ? html + user.Email + "<br />" : html;
     html = user.Phone !== "" ? html + user.Phone + "<br />" : html;

     var contactLink = (user.Links.Detail != null) ? "<a class=\"btn btn-default\" href=\"" + user.Links.Detail + "\">View Contact</a>" : "";
     var filesLink = (user.Links.Files != null) ? "<a class=\"btn btn-default\" href=\"" + user.Links.Files + "\">View Files</a>" : "";
     
     html = html + "<div style=\"margin: 10px 0;\">" + contactLink + " " + filesLink + "</div>";

     $("#clientDetail_<%: this.UniqueID %>").html(html);
    }
   });

 });
</script>