<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Admin.Areas.Tickets.CreateTicket.Models.CreateTicketViewModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.ZenDesk.Support" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.TempDataExtensions" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Create Ticket
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <div class="row" style="padding: 0 0 20px 20px;">
    <div class="col-md-5">
      <div class="panel panel-default">
        <div class="panel-heading">
          <h3 class="panel-title">Create Ticket</h3>
        </div>
        <div class="panel-body">
          <% if (this.TempData.Peek("message") != null)
             { %>
            <div id="notice" class="alert alert-<%: this.TempData.Get<AlertMessage>("message").Type %>" style="margin: 20px 0 20px 0;">
              <%= this.TempData.Get<AlertMessage>("message").Body %>
            </div>
          <% } %>
          <% using (Html.BeginForm("Index", "CreateTicket", FormMethod.Post, new {}))
             { %>
            <%: Html.ValidationSummary() %>

            <div class="form-group" id="recipientHolder">
              <label for="">Recipients</label>
              <%
                var index = 0;
                foreach (var recipient in Model.Recipients)
                { %>
                <div class="checkbox">
                  <label>
                    <input type="checkbox" <%= recipient.Checked ? "checked" : "" %> id="Recipients[<%: index %>]"> <%: recipient.Text %>
                  </label>
                </div>
                <% = Html.Hidden("Recipients[" + index + "].Text", recipient.Text) %>
                <% = Html.Hidden("Recipients[" + index + "].Value", recipient.Value) %>
                <% = Html.Hidden("Recipients[" + index + "].Checked", recipient.Checked) %>
              <% index++;
                }
              %>
            </div>
            <div class="form-group">
              <label for="">Other Recipients</label>
              <%= Html.TextBoxFor(a => a.OtherRecipients, new {@class = "form-control"}) %>
              <p style="font-style: italic">Separate multiple emails with a comma.</p>
            </div>
            <div class="form-group">
              <label for="">Type</label>
              <%: Html.DropDownListFor(m => m.Type, EnumExtensions.ToLookup<TicketType>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
            </div>
            <div class="form-group">
              <label for="">Priority</label>
              <%: Html.DropDownListFor(m => m.Priority, EnumExtensions.ToLookup<TicketPriority>().Select(a => new SelectListItem {Text = a.Key.ToString(), Value = a.Value.ToString()}).AsEnumerable(), new {@class = "form-control"}) %>
            </div>
            <div class="form-group">
              <label for="">Subject</label>
              <%= Html.TextBoxFor(a => a.Subject, new {@class = "form-control"}) %>
            </div>
            <div class="form-group">
              <label for="">Comments</label>
              <%= Html.TextAreaFor(m => m.Comments, new {@class = "form-control", rows = 10, cols = 47}) %>
            </div>
            <p style="font-style: italic">Customer Support signature automatically included in message.</p>
            <a href="<%: Url.BuildFor<UserDetailController>().ToDetail(Model.UserId) %>" class="btn btn-default">Cancel</a>
            <button type="submit" class="btn btn-primary">Submit</button>
            <%: Html.HiddenFor(m => m.UserId) %>
          <% } %>
        </div>
      </div>

    </div>
  </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    $(function() {
      // when a recipient is checked, set the corresponding hidden input to true so the view model can pick it up on the back end
        $("#recipientHolder input[type=checkbox]").click(function () {
          console.log("recipientHolder input[type=hidden][name='" + $(this).attr("id") + ".Checked']");
        $("#recipientHolder input[type=hidden][name='" + $(this).attr("id") + ".Checked']").val($(this).prop('checked'));
      });
    });

  </script>


</asp:Content>