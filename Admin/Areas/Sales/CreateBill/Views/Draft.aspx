<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BillViewModel>" MasterPageFile="~/Views/Shared/bootstrap3.Master" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create <%: this.Model.BillType.GetDescription() %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2 style="margin-top: 0;"><%: this.Model.BillType.GetDescription()%></h2>
        <div class="row" style="padding: 0 20px 20px 20px;">
        <% using (this.Html.BeginForm()) { %>
        <%= this.Html.Hidden("bill.BillType", this.Model.BillType) %>
        <%= this.Html.Hidden("bill.UserId", this.Model.UserId) %>
        <%= this.Html.Hidden("bill.DealId", this.Model.DealId) %>
        <%= this.Html.Hidden("bill.OrderId", this.Model.OrderId) %>
        <%= this.Html.Hidden("bill.PublicKey", this.Model.PublicKey) %>
        <%= this.Html.Hidden("bill.Title", this.Model.Title) %>
        
        <%= this.Html.Hidden("bill.Attachments.ConsumerMatchCodes", this.Model.Attachments.CommonProcessingCodes) %>
        
        <%: this.Html.Hidden("bill.Content.SendFrom", this.Model.Content.SendFrom)%>
        <%: this.Html.Hidden("bill.Content.IsHtml", this.Model.Content.IsHtml) %>
        
        <%= this.Html.Hidden("bill.ReceiptTemplateName", this.Model.ReceiptTemplateName) %>
        <%= this.Html.Hidden("bill.IsHtml", this.Model.Content.IsHtml) %>
        <%
         
               foreach (var file in Model.AdminFiles.Select((file, i) => new { File = file, Index = i }))
               {
                   Response.Write(Html.Hidden("bill.AdminFiles.Index", file.Index));
                   Response.Write(Html.Hidden("bill.AdminFiles[" + file.Index + "].Filename", file.File.Filename));
                   Response.Write(Html.Hidden("bill.AdminFiles[" + file.Index + "].CustomerFilename", file.File.CustomerFilename));
                   Response.Write(Html.Hidden("bill.AdminFiles[" + file.Index + "].Selected", file.File.Selected));
               }

               foreach (var file in Model.ClientFiles.Select((file, i) => new { File = file, Index = i }))
               {
                   Response.Write(Html.Hidden("bill.ClientFiles.Index", file.Index));
                   Response.Write(Html.Hidden("bill.ClientFiles[" + file.Index + "].Filename", file.File.Filename));
                   Response.Write(Html.Hidden("bill.ClientFiles[" + file.Index + "].CustomerFilename", file.File.CustomerFilename));
                   Response.Write(Html.Hidden("bill.ClientFiles[" + file.Index + "].Selected", file.File.Selected));
               }
        %>
        <div class="k-grid k-widget" style="margin: 15px 0 15px 0;">
          <table>
            <tr>
                <th class="k-header">To</th>
                <td>
                  <div class="input-group" style="width: 600px;">
                    <%= Html.TextBox("To", Model.Content.SendTo.ToArray().Join(","), new { @class = "form-control" })%>
                    <div class="input-group-btn">
                      <input type="button" class="btn btn-default" value="Don't notify customer" name="dontNotify">
                    </div>
                  </div>
                   <span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
            <tr>
                <th class="k-header">Bcc</th>
                <td>
                  <%= Html.TextBox("Bcc", Model.Content.BccTo.ToArray().Join(","), new { @class = "form-control", style="width: 600px;" })%>&nbsp;<span style="font-size: 10px;">Separate multiple email addresses with a comma</span>
                </td>
            </tr>
            <%
               if (this.Model.AdminFiles.Concat(this.Model.ClientFiles).Any())
               {
                   var filesdisplay = this.Model.AdminFiles.Concat(this.Model.ClientFiles).Where(f => f.Selected).Select(f => f.CustomerFilename).ToArray().Join(",");
            %>
            <tr>
                <th class="k-header">
                    Files
                </th>
                <td>
                    <span class="textile"><%: filesdisplay %></span>
                </td>
            </tr>
            <tr>
                <th class="k-header">
                    Zip Files
                </th>
                <td>
                    <input id="zipfiles" type="checkbox" name="bill.ZipFiles" value="true" />
                </td>
            </tr>

            <% 
               }
            %>
            
            <tr>
                <th class="k-header">
                    Subject
                </th>
                <td>
                    <%= Html.TextBox("bill.Content.Subject", this.Model.Content.Subject, new { @class = "form-control", style="width: 600px;" })%>
                </td>
            </tr>
            <tr>
                <th class="k-header">
                    Message
                </th>
                <td>
                    <%= this.Html.TextArea("bill.Content.Body", this.Model.Content.Body, new { cols = 110, rows = 500, id = "editor", @class = "form-control", style="height: 500px;" })%>
                </td>
            </tr>
        </table>
        </div>
        
        
        <p><input class="btn btn-primary" type="submit" value="Send <%: Model.BillType.GetDescription() %> For Review" title="This will lock the deal and order information for Sales review." /></p>
        <%
        }
        %>

    </div>
    <% if (this.Model.Content.IsHtml)
       { %>
    <script>

      var To;

      $(document).ready(function() {

        $("input[name=dontNotify]").click(function() {
            if ($(this).val() === "Don't notify customer") {
              To = $("input[name=To]").val();
                $("input[name=To]").val("support@accurateappend.com");
              $(this).val("Notify customer");
            } else {
              $(this).val("Don't notify customer");
              $("input[name=To]").val(To);
            }
        });

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

