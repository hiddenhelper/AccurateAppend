<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AssociateJobModel>" %>

<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal.Models" %>
<%@ Import Namespace="AccurateAppend.Sales" %>
<%@ Import Namespace="AccurateAppend.Core" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Deals
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin-top: 0;">Associate With Deal: <%: this.Model.CustomerFileName %></h2>
    <h3><%: this.Html.ActionLink(this.Model.UserName, "Index", "UserDetail", new {area = "Clients", userId=this.Model.UserId}, null) %></h3>
    <div class="row" style="padding: 0 0 20px 20px;">
        <div>
            <h3>Recent Deals</h3>
            (Deal created during last 5 days that are <%: DealStatus.InProcess.GetDescription() %>
           
                <table class="table table-condensed">
                    <thead>
                    <tr>
                        <th>
                            Id
                        </th>
                        <th>
                            Date Created
                        </th>
                        <th style="text-align: right">
                            Amount
                        </th>
                        <th style="padding-left: 10px">
                            Title
                        </th>
                        <th>
                            &nbsp;
                        </th>
                    </tr>
                    </thead>
                    <tbody id="deals">
                    <tr><td colspan="5">
                    <div id="notice" title="deals_no_deals" class="alert alert-info">
                      No deals found.
                    </div>
                    </td></tr>
                    </tbody>
                </table>
        </div>
    </div>
  
<script type="text/javascript">
  $(function() {
    console.log("Rendering available deals");

    $.ajax(
      {
        type: "GET",
        url:
          "<%= this.Url.Action("ByUser", "DealsApi", new { Area = "Sales", userId = this.Model.UserId, status = DealStatus.InProcess})  %>",
            success: function (deals) {
                if (deals.length === 0) return;

              var formatter = new Intl.NumberFormat("en-US",
                {
                  style: "currency", currency: "USD",
                  minimumFractionDigits: 2
                    });
              
          var html = '';
            deals.forEach(function (deal) {
                html = html +
                    "<tr>" +
                      "<td>" + deal.DealId + "</td>" +
                      "<td style=\"white-space: nowrap;\">" + deal.DateCreated + "</td>" +
                      "<td style=\"text-align: right;\">" + formatter.format(deal.Amount) + "</td>" +
                      "<td style=\"padding-left: 10px;\">" + deal.Title + "</td>" +
                      "<td><a href=\"" + deal.Links.Detail + "\" target=\"_new\", class=\"btn btn-default\">Deal Detail</a> <a href=\"<%= this.Url.Action("Select", new {this.Model.JobId}) %>&dealId=" + deal.DealId + "\" target=\"_new\", class=\"btn btn-default\">Select</a></td>" +
                    "</tr>";
                });
              $("#deals").html(html);
        }
      });
    });
  </script>
</asp:Content>
   
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>