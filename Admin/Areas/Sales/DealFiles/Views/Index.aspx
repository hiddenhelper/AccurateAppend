<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<LinkFilesToDeal>" %>

<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealFiles.Models" %>
<%@ Import Namespace="AccurateAppend.Websites" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Files
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin-top: 0;">Associate With Deal: <%: this.Html.ActionLink(this.Model.Title, "Index", "DealDetail", new {this.Model.DealId}, null) %></h2>
    <div class="row" style="padding: 0 0 20px 20px;">
        <div>
            <h3>Recent Files</h3>
            (Files uploaded during last 7 days)
           
                <table class="table table-condensed">
                    <thead>
                    <tr>
                        <th>
                          File Name
                        </th>
                        <th>
                          Date Created
                        </th>
                        <th style="text-align: right">
                            Size
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                    </thead>
                    <tbody id="files">
                    <tr><td colspan="5">
                    <div id="notice" title="files_no_files" class="alert alert-info">
                      No files found.
                    </div>
                    </td></tr>
                    </tbody>
                </table>
        </div>
    </div>
  
<script type="text/javascript">
  $(function() {
    console.log("Rendering available files");

    $.ajax(
      {
        type: "GET",
        url:
          "<%= this.Url.Action("Uncorrelated", "FilesApi", new { Area = "Operations", userId = this.Model.UserId, startDate = DateTime.UtcNow.ToUserLocal().AddDays(-7)})  %>",
            success: function (files) {
                if (files.Data.length === 0) return;
                var html = '';
                files.Data.forEach(function (file) {
                  html = html +
                          "<tr>" +
                            "<td style=\"white-space: nowrap;\">" + file.CustomerFileName + "</td>" +
                            "<td style=\"text-align: right;\">" + file.CreatedDate + "</td>" +
                            "<td style=\"padding-left: 10px;\">" + file.FileSize + "</td>" +
                            "<td><a href=\"<%= this.Url.Action("Select", "DealFiles", new {Area = "Sales", Deal = this.Model.PublicKey}) %>&File=" + file.Id + "\" class=\"btn btn-default\">Select</a></td>" +
                          "</tr>";
                      });
                    $("#files").html(html);
            }
      });
    });
  </script>
</asp:Content>
  
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>