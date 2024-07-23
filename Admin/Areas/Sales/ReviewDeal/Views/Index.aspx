<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<ReviewViewModel>" %>
<%@ Import namespace="AccurateAppend.Sales.Contracts.ViewModels" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="DomainModel.Enum" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Review Bill
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <h2 style="margin-top: 0;">Review Bill</h2>
  <div class="row" style="padding: 0 0 20px 20px;">
    <% using (Html.BeginForm("Index", "ReviewDeal", new {Area = "Sales"}, FormMethod.Post, new {id = "theForm"}))
       { %>
      <%: Html.Hidden("dealid", Model.Id) %>
      <div class="k-grid k-widget" style="margin: 15px 0 15px 0; width: 1290px;">
        <table>
          <tr>
            <th class="k-header"></th>
            <td style="padding: 10px;">
              <input type="submit" value="Approve" onclick="javascript:Approve('<%: ReviewAction.Approve %>');" class="btn btn-primary"/>
              <input type="submit" value="Decline" onclick="javascript:Approve('<%: ReviewAction.Decline %>');" class="btn btn-warning"/>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              <%= Html.LabelFor(m => m.SendFrom) %>
            </th>
            <td>
              <%: Model.SendFrom %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              <%= Html.LabelFor(m => m.SendTo) %>
            </th>
            <td>
              <%: string.Join(", ", Model.SendTo) %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              <%= Html.LabelFor(m => m.BccTo) %>
            </th>
            <td>
              <%: string.Join(", ", Model.BccTo) %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              Deal
            </th>
            <td>
              <%= Html.NavigationFor<DealDetailController>().Detail(Model.Description, Model.Id, new {target = "_new", }) %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              Files
            </th>
            <td>
              <%
                var links = Model.Files.Select(attachment => Html.ActionLink(attachment.SendFileName, "ViewFile", "CreateBill", new {path = attachment.FileName}, new {target = "_new", @class = "btn btn-default"}).ToHtmlString()).ToList();

                Response.Write(string.Join(", ", links));
              %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              <%: Html.LabelFor(m => m.Subject) %>
            </th>
            <td>
              <%: Model.Subject %>
            </td>
          </tr>
          <tr>
            <th class="k-header">
              <%: Html.LabelFor(m => m.Body) %>
            </th>
            <td>
              <iframe src="<%: Url.Action("Content", "ReviewDeal", new {dealId = Model.Id, area = "Sales"}) %>" style="background-color: #ffffff; border: solid #eeeeee 1px; height: 300px; left: 100px; top: 100px; width: 1200px;">

              </iframe>
            </td>
          </tr>
          <tr>
            <th class="k-header"></th>
            <td style="padding: 10px;">
              <label>Reviewer Comments</label>
              <textarea name="approvalnote" id="approvalnote" rows="5" cols="5" class="form-control" style="width: 500px;"></textarea><br/>
              <input type="hidden" name="action" id="action"/>
              <input type="submit" value="Approve" onclick="javascript:Approve('<%: ReviewAction.Approve %>');" class="btn btn-primary"/>
              <input type="submit" value="Decline" onclick="javascript:Approve('<%: ReviewAction.Decline %>');" class="btn btn-warning"/>

            </td>
          </tr>

        </table>
      </div>

    <% } %>
  </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="DocumentStart">
  <script type="text/javascript">
    function Approve(action) {
      $("#action").val(action);
    }
  </script>
  <style>
    .k-header {
      padding: 5px 10px 5px 10px;
    }
  </style>
</asp:Content>