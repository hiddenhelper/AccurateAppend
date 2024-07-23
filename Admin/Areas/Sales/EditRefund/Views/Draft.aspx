<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BillViewModel>" MasterPageFile="~/Views/Shared/bootstrap3.Master" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models" %>
<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
    Create Refund
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <h3 style="margin-top: 0;">Refund</h3>
    <div class="row" style="padding: 0 0 20px 20px;">
        <% using (this.Html.BeginForm("Process", "RefundDeal", new { Area = "Sales" }, FormMethod.Post, null)) { %>
        <% = this.Html.Hidden("model.OrderId", this.Model.OrderId)%>
        <% = this.Html.Hidden("model.DealId", this.Model.DealId)%>
        <% = this.Html.Hidden("model.UserId", this.Model.UserId)%>
        <% = this.Html.Hidden("model.Content.SendFrom", this.Model.Content.SendFrom)%>
        <% = this.Html.Hidden("model.Content.IsHtml", this.Model.Content.IsHtml) %>
        <table class="table table-condensed">
            <tr>
                <th>To</th>
                <td>
                    <%= this.Html.TextBox("To", this.Model.Content.SendTo.ToArray().Join(","), new { @class = "form-control" })%>&nbsp;<span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
            <tr>
                <th>Bcc</th>
                <td>
                    <%= this.Html.TextBox("Bcc", this.Model.Content.BccTo.ToArray().Join(","), new { @class = "form-control" })%>&nbsp;<span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
            <tr>
                <th>
                    Subject
                </th>
                <td>
                    <%= this.Html.TextBox("model.Content.Subject", this.Model.Content.Subject, new { @class = "form-control" })%>
                </td>
            </tr>
            <tr>
                <th>
                    Message
                </th>
                <td>
                 <%= this.Html.TextArea("model.Content.Body", this.Model.Content.Body, new { cols = 110, rows = 500, id = "editor", @class = "form-control", style="height: 500px;" })%>
                </td>
            </tr>
        </table>
        
        <p><input type="submit" value="Process Refund" title="This refund the order and transmit the notice to the client." class="btn btn-primary" /></p>
        <%
        }
        %>

    </div>
 <% if (this.Model.Content.IsHtml)
       { %>
    <script>
        
        $(document).ready(function() {
            // create Editor from textarea HTML element with default set of tools
         $("#editor").kendoEditor({
            resizable: {
                    content: true,
                    toolbar: true
            },
            tools: [
                "bold",
                "italic",
                "underline",
                "strikethrough",
                "justifyLeft",
                "justifyCenter",
                "justifyRight",
                "justifyFull",
                "insertUnorderedList",
                "insertOrderedList",
                "indent",
                "outdent",
                "createLink",
                "unlink",
                "insertImage",
                "insertFile",
                "subscript",
                "superscript",
                "tableWizard",
                "createTable",
                "addRowAbove",
                "addRowBelow",
                "addColumnLeft",
                "addColumnRight",
                "deleteRow",
                "deleteColumn",
                "viewHtml",
                "formatting",
                "cleanFormatting",
                "fontName",
                "fontSize",
                "foreColor",
                "backColor",
                "print"
             ]
            });
        });
    </script>
    <% } %>
</asp:Content>

<script runat="server">
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.UtcNow);
    }
</script>