<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<CreatePaidTestModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Paid Test
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="row" style="padding: 0 0 20px 20px;">
    <div class="col-md-4">
      <div class="panel panel-default">
        <div class="panel-heading">Paid Test Details</div>
        <div class="panel-body">
          <% using (Html.BeginForm("CreateTest", "ContractWizard", FormMethod.Post, new {@class = "form-horizontal"}))
             {
          %>
            <div class="form-group">
              <label for="" class="col-sm-5 control-label">Max balance</label>
              <div class="col-sm-7">
                <%= Html.TextBoxFor(m => m.MaxBalance, null, new {@class = "form-control"}) %>
                <% = Html.ValidationMessageFor(m => m.MaxBalance, "") %>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-5 control-label">Effective Date</label>
              <div class="col-sm-7">
                <input id="startdatepicker" value="<%: this.Model.EffectiveDate.ToShortDateString() %>" title="startdatepicker" />
                <% = Html.ValidationMessageFor(m => m.EffectiveDate, "") %>
              </div>
            </div>
            <div class="form-group">
              <label for="" class="col-sm-5 control-label">End Date</label>
              <div class="col-sm-7">
                <input id="enddatepicker" value="<%: this.Model.EndDate.ToShortDateString() %>" title="enddatepicker" />
                <% = Html.ValidationMessageFor(m => m.EndDate, "") %>
              </div>
            </div>
            <div class="form-group">
              <div class="col-sm-offset-5 col-sm-7">
                <button type="submit" class="btn btn-primary">Create</button>
              </div>
            </div>
            <%= Html.HiddenFor(d => d.UserId) %>
            <%= Html.HiddenFor(d => d.EffectiveDate, new {id = "startdate"}) %>
            <%= Html.HiddenFor(d => d.EndDate, new {id = "enddate"}) %>
            <%= Html.HiddenFor(d => d.FirstAvailableDate) %>
          <% } %>
        </div>
      </div>
    </div>
  </div>
        </asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            function onStartDateChange() {
                $("#startdate").val(kendo.toString(this.value(), 'd'));
            }

            function onEndDateChange() {
                $("#enddate").val(kendo.toString(this.value(), 'd'));
            }

            $("#startdatepicker").kendoDatePicker(
            {
                change: onStartDateChange
            });
            $("#enddatepicker").kendoDatePicker(
            {
                change: onEndDateChange
            });
        });
    </script>
</asp:Content>