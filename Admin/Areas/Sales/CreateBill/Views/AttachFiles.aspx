<%@ Page Title="Attach Files" Language="C#" Inherits="System.Web.Mvc.ViewPage<BillViewModel>" MasterPageFile="~/Views/Shared/bootstrap3.Master" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models" %>
<%@ Import Namespace="AccurateAppend.Core" %>
<%@ Import Namespace="DomainModel.Enum" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Attach Files
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <%
    using (Html.BeginForm())
    { %>

    <h2 style="margin-top: 0;">Include Files in Bill</h2>
    <div class="row">
      <div class="col-md-6">
        <div class="panel panel-default">
          <div class="panel-body">
            <div class="form-group col-md-6">
              <label>Receipt Template</label>
              <%= Html.Kendo().DropDownList()
                    .Name("ReceiptTemplateName")
                    .DataTextField("Text")
                    .DataValueField("Value")
                    .BindTo(EnumExtensions.ToLookup<ReceiptTemplateName>().Select(a => new SelectListItem { Text = ((ReceiptTemplateName)a.Value).GetDescription() , Value = ((ReceiptTemplateName)a.Value).ToString() }))
                    .Value(Model.ReceiptTemplateName.ToString())
              %>
              <label style="margin-top: 10px;">Attach Legends</label>
              <div class="checkbox">
                <label>
                  <%: this.Html.CheckBoxFor(m => m.Attachments.CommonProcessingCodes) %>
                  <%: this.Html.LabelFor(m => m.Attachments.CommonProcessingCodes) %>
                </label>
              </div>
              </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <% Html.RenderPartial("~/Areas/Sales/Shared/DealDetail.ascx", Model.DealId); %>
      </div>
    </div>
    <div class="panel panel-default">
      <div class="panel-heading">Batch Jobs</div>
      <div class="panel-body">
        <div id="files_batch_message" style="display: none;" class="alert alert-info">
          No jobs found.
        </div>
        <div id="files_batch" style="margin: 15px 0 15px 0;" class="k-grid k-widget">
          <table>
            <thead class="k-grid-header">
            <tr>
              <th class="k-header" style="width: 20px;">
              </th>
              <th class="k-header">
                Date
              </th>
              <th class="k-header">
                Records
              </th>
              <th class="k-header">
                File Name
              </th>
              <th class="k-header">
                Product
              </th>
            </tr>
            </thead>
            <tbody>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    <div class="panel panel-default">
      <div class="panel-heading">Deal Files</div>
      <div class="panel-body">
        <div id="files_admin_no_files" style="display: none; margin-top: 15px;" class="alert alert-info">
          No files found.
        </div>
        <div class="k-grid k-widget" style="margin: 15px 0 15px 0;" id="files_admin">
          <table>
            <thead class="k-grid-header">
            <tr>
              <th class="k-header" style="width: 20px;">
              </th>
              <th class="k-header">
                Date
              </th>
              <th class="k-header">
                Size
              </th>
              <th class="k-header">
                File Name
              </th>
            </tr>
            </thead>
            <tbody>

            </tbody>
          </table>
        </div>
      </div>
    </div>
    <input id="ProceedBtn" type="submit" value="Proceed to Next Step >" class="btn btn-primary"/>
  
    <%: this.Html.HiddenFor(m => m.BillType) %>
    <%: this.Html.HiddenFor(m => m.UserId) %>
    <%: this.Html.HiddenFor(m => m.DealId) %>
    <%: this.Html.HiddenFor(m => m.OrderId) %>
    <%: this.Html.HiddenFor(m => m.PublicKey) %>
    <%: this.Html.HiddenFor(m => m.Title) %>
  
  <% } %>
  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    var viewModel = {
      updateFilesView: function() {
        $.ajax(
          {
            type: "GET",
            async: false,
            url: "<%: this.Url.Action("ForCorrelationId", "FilesApi", new { Area = "Operations", id = this.Model.PublicKey}) %>",
            success: function(files) {
              $("#files_admin tbody tr").remove();
              if (files.Data.length === 0) {
                $("#files_admin_no_files").show();
                $("#files_admin").hide();
              } else {
                $("#files_admin").show();
                $("#files_admin_no_files").hide();
                $(files.Data).each(function(i, file) {
                  $('<tr><td>' +
                    '<input type="hidden" name="AdminFiles.Index" value="' +
                    i +
                    '" />' +
                    '<input type="hidden" name="AdminFiles[' +
                    i +
                    '].Filename" value="' +
                    file.SystemFileName +
                    '" />' +
                    '<input type="hidden" name="AdminFiles[' +
                    i +
                    '].CustomerFilename" value="' +
                    file.CustomerFileName +
                    '" />' +
                    '<input type="checkbox" name="AdminFiles[' +
                    i +
                    '].Selected" value="true"/>' +
                    '</td><td>' +
                    file.CreatedDate +
                    '</td><td>' +
                    file.FileSize +
                    '</td><td colspan=2>' +
                    file.CustomerFileName +
                    '</td></tr>').appendTo("#files_admin tbody");
                });
              }
            },
            error: function(xhr, status, error) {
              $("#error").html("<strong>Error:</strong>" + xhr.responseText).show();
            }
          });
      },
      updateJobsView: function() {
        $.ajax(
          {
            type: "GET",
            async: false,
            url:
              "/JobProcessing/Queue/Complete?userid=<%: Model.UserId %>&startdate=<%: DateTime.Now.AddDays(-3).ToShortDateString() %>&enddate=<%: DateTime.Now.ToShortDateString() %>",
            success: function(files) {
              console.log(files.Total);
              $("#files_batch tbody tr").remove();
              if (files.Total === 0) {
                $("#files_batch_message").show();
                $("#files_batch").hide();
              } else {
                $(files.Data).each(function(i, file) {
                  $('<tr><td>' +
                    '<input type="hidden" name="ClientFiles.Index" value="' +
                    i +
                    '" />' +
                    '<input type="hidden" name="ClientFiles[' +
                    i +
                    '].Filename" value="' +
                    file.InputFileName + ".csv" +
                    '" />' +
                    '<input type="hidden" name="ClientFiles[' +
                    i +
                    '].CustomerFilename" value="' +
                    file.CustomerFileName +
                    '" />' +
                    '<input type="checkbox" name="ClientFiles[' +
                    i +
                    '].Selected" value="true"/>' +
                    '</td><td>' +
                    file.DateComplete +
                    '</td><td>' +
                    file.TotalRecords +
                    '</td><td>' +
                    file.CustomerFileName +
                    '</td><td style="width: 400px;">' +
                    file.Product +
                    '</td></tr>').appendTo("#files_batch tbody");
                });
                $("#files_batch").show();
                $("#files_batch_message").hide();
              }
            },
            error: function(xhr, status, error) {
              $("#error").html("<strong>Error:</strong>" + xhr.responseText).show();
            }
          });
      }
    };

    var clientFiles = new Array();
    var adminFiles = new Array();

    $(function() {
      viewModel.updateFilesView();
      viewModel.updateJobsView();
      
      $("#ProceedBtn").click(function () {
          $("#ProceedBtn").prop("disabled", true);
          $("form").submit();
        });

    });

  </script>

</asp:Content>