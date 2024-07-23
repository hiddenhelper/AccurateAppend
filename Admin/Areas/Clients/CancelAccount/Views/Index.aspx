<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<CancelAccountModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.CancelAccount.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Cancel Account
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% using (Html.BeginForm("Index2", "CancelAccount"))
     {
  %>
    <%= Html.HiddenFor(d => d.AccountId) %>
    <%= Html.HiddenFor(d => d.FirstAvailableDate) %>
    <%= Html.HiddenFor(d => d.RedirectTo) %>
    <%= Html.Hidden("endDate", Model.FirstAvailableDate, new {id = "enddate"}) %>
  
  <div class="row" style="padding: 0 0 0 10px;">
    <div class="col-md-4">
      <div class="panel panel-default">
        <div class="panel-heading">Cancel Account</div>
        <div class="panel-body">
            <div class="form-group">
              <label for="" class="col-sm-5 control-label">Cancellation Date</label>
              <div class="col-sm-7">
                <input id="datepicker" value="<%: Model.FirstAvailableDate.ToShortDateString() %>" title="datepicker"/>
                <% = Html.ValidationMessageFor(m => m.FirstAvailableDate, "") %>
              </div>
            </div>
            <div class="form-group">
              <div class="col-sm-offset-5 col-sm-7">
                <button type="submit" class="btn btn-primary" style="margin-top: 10px;">Cancel Subscription</button>
              </div>
            </div>
        </div>
      </div>
    </div>
  </div>
  <% } %>
  </asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  <script type="text/javascript">
    $(document).ready(function() {
      function onEndDateChange() {
        $("#enddate").val(kendo.toString(this.value(), 'd'));
      }

      $("#datepicker").kendoDatePicker(
        {
          change: onEndDateChange
        });

    });
  </script>
</asp:Content>