<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<Message>" %>

<%@ Import Namespace="AccurateAppend.Operations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Messages
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row" style="padding: 0 20px 20px 20px;">
        <% using (this.Html.BeginForm()) { %>
        <%= this.Html.Hidden("id", this.Model.Id) %>

        <table class="table table-bordered">
            <tr>
                <th>To</th>
                <td>
                    <%= this.Html.TextBox("sendTo", String.Join(",", this.Model.SendTo.Select(a => a.Address).ToArray()), new { @class = "form-control" })%>&nbsp;<span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
            <tr>
                <th>Bcc</th>
                <td>
                    <%= this.Html.TextBox("bccTo", String.Join(",", this.Model.BccTo.Select(a => a.Address).ToArray()), new { @class = "form-control" })%>&nbsp;<span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
        </table>
        
        <p><input class="btn btn-primary" type="submit" value="Resend" /></p>
        <%
                                      }
        %>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">


</asp:Content>
