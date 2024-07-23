<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<ClientContacts>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Controllers" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.Clients.EditContact.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.EditContact" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.UserDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Client Contacts
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    <script type="text/javascript">
        function insertContactRow() {
            $.get('<% = this.Url.BuildFor<EditContactController>().AddRow() %>', function(data) {
                $('#contacts tr:last').after(data);
            });
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3 style="margin-top: 0;">
        Account Contacts: <%: Model.Name %>
    </h3>
    <div class="row" style="padding: 0 0 20px 20px;">
        <%
            var message = ViewData["Message"] as String;
            if (!String.IsNullOrWhiteSpace(message))
            {
        %>
            <div>
                <div id="succeed" class="alert alert-success">
                    <%: message %>
                    <%: this.Html.NavigationFor<UserDetailController>().Detail("Return to client", Model.UserId) %>
                </div>
            </div>
        <%
            } %>
        <a href="#" id="btn_addorderitem" class="btn btn-default" style="margin: 20px 0 20px 0;" onclick="javascript:insertContactRow();">
                Add New
            </a>
        <%: this.Html.NavigationFor<UserDetailController>().Detail("Return to Client", Model.UserId, new { @class="btn btn-default", style="margin-left: 10px;" }) %>
        <% using (Html.BeginForm()) %>
        <%
           { %>
            
            <table id="contacts" class="table table-striped">
                <tr>
                    <th><%: Html.LabelFor(m => m.Contacts.First().EmailAddress) %></th>
                    <th><%: Html.LabelFor(m => m.Contacts.First().Name) %></th>
                    <th>Categories</th>
                    <th></th>
                </tr>
                <% foreach (var item in this.Model.Contacts)
                   {
                       this.Html.RenderPartial("~/Areas/Clients/EditContact/Views/ContactRow.ascx", item);
                   }
                %>
            </table>

           <a class="btn btn-primary" href="#" onclick="$(this).closest('form').submit()">
                    Save
                </a>
                <a class="btn btn-warning" href="<%= Url.BuildFor<UserDetailController>().ToDetail(this.Model.UserId) %>">
                    Cancel
                </a>
            <input type="hidden" name="model.Id" value="<%= this.Model.Id %>"/>
            <input type="hidden" name="model.UserId" value="<%= this.Model.UserId %>"/>
            <input type="hidden" name="model.Name" value="<%= this.Model.Name %>"/>
        <% } %>
    </div>

</asp:Content>