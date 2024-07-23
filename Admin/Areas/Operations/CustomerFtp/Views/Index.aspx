<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<FtpLogonModel>" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.Operations.Controllers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  FTP
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% using (this.Html.BeginForm())
     { %>
    <%: this.Html.HiddenFor(m => m.UserId) %>
    <%: this.Html.HiddenFor(m => m.Id) %>
    <div class="row">

      <div class="col-md-4">

        <div class="panel panel-default">
          <div class="panel-heading">Create Ftp</div>
          <div class="panel-body">
            <%: this.Html.LabelFor(m => m.UserName) %> <%= this.Html.TextBoxFor(m =>m.UserName, new {id = "userName"}) %>
            <%: this.Html.ValidationMessageFor(m => m.UserName) %>
            <br/>
            <%: this.Html.LabelFor(m => m.Password) %> <%= this.Html.TextBoxFor(m => m.Password, new {id = "password"}) %><button type="button" onclick="generatePassword()">Generate</button><br/>
            <%: this.Html.ValidationMessageFor(m => m.Password) %>
            <br/>
            Enabled: <%: this.Html.RadioButtonFor(m => m.IsActive, true) %> Disabled: <%: this.Html.RadioButtonFor(m => m.IsActive, false) %>
          </div>
        </div>

      </div>

    </div>
    <input type="submit" value="Create" id="submit" disabled="disabled"/>
  <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">
    function generatePassword() {
      $.ajax(
        {
          type: "GET",
          url: "<%: this.Url.Action("Generate") %>",
          success: function (data) {
            $('#password').val(data);
          }
        });
    }
  </script>
  <% if (this.Request.HttpMethod == "GET")
      { %>
     <script type="text/javascript">
       $(function() {
         $.ajax(
           {
             type: "GET",
             url: "<%: this.Url.Action("Query") %>",
             data: {
               userId: '<%: this.Model.UserId %>'
             },
             success: function(data) {
               $('#userName').val(data.UserName),
                 $('#password').val(data.Password),
                 $('#submit').removeAttr("disabled")
             }
           }
         );
       })
     </script>
  <% }
     else
     { %>
<script type="text/javascript">
  $(function() {
    $('#submit').removeAttr("disabled");
  });
</script>
    <%
     }%>
</asp:Content>
