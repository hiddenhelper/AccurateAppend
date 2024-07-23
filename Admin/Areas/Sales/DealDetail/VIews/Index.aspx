<%@ Page Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<DealDetailView>" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealDetail.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.AddNoteToDeal" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.DealNotes" %>
<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
  Deal Detail
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

  <h3 style="margin-top: 0;">Deal Detail</h3>
  <div class="row" style="padding: 0 0 20px 20px;">

    <div class="alert alert-danger" id="error"></div>
    <div class="alert alert-info" id="notice" style="display: none"></div>

    <div class="row">
      <div class="col-md-6">
        <div class="row">
          <div class="col-md-6">
            <div class="panel panel-default">
              <div class="panel-heading">
                Deal Details
              </div>
              <div class="panel-body" id="dealDetails">
                <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="dealsInfo">Loading deal information</div>
              </div>
            </div>
          </div>
          <div class="col-md-6">
            <% Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", Model.UserId); %>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12">
            <div class="panel panel-default" id="dealInstructions" style="display: none;">
            </div>
          </div>
          <div class="col-md-12">
            <div class="panel panel-default">
              <div class="panel-heading">Orders</div>
              <div class="panel-body">
                <div id="ordersMessage" class="alert alert-info" style="display: none;"></div>
                <div class="k-grid k-widget">
                  <table id="ordersTable">
                    <thead class="k-grid-header">
                    <tr>
                      <th class="k-header">Order Date</th>
                      <th class="k-header">Order Id</th>
                      <th class="k-header">Status</th>
                      <th class="k-header">Amount</th>
                      <th class="k-header"></th>
                    </tr>
                    </thead>
                    <tbody></tbody>
                  </table>
                </div>
                <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="ordersInfo">No orders found</div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="panel panel-default">
          <div class="panel-heading">Files</div>
          <div class="panel-body">
            <% if (this.Model.UploadFileLink != null)
               { %>
            <button type="button" class="btn btn-default" onclick="viewModel.uploadFile('<%: this.Model.UploadFileLink %>')" id="uploadButton" style="margin-bottom: 10px;">Upload File</button>
              <% } %>
            <% if (this.Model.AssociateFileLink != null)
               { %>
              <button type="button" class="btn btn-default" onclick="viewModel.existingFile('<%: this.Model.AssociateFileLink %>')" id="associateButton" style="margin-bottom: 10px;">Existing File</button>
            <% } %>
            <div class="alert alert-info" style="display: none; margin-bottom: 20px;" id="filesInfo">No files found</div>
            <div id="filesGrid"></div>
          </div>
        </div>
        <div class="panel panel-default">
          <div class="panel-heading">Notes & Events</div>
          <div class="panel-body">
            <div id="notesMessage" class="alert alert-info" style="display: none;"></div>
            <button class="btn btn-default" id="linkaddnote" style="margin-bottom: 10px;">Add Note</button>
            <div class="k-grid k-widget">
              <table id="notesTable">
                <thead class="k-grid-header">
                <tr>
                  <th class="k-header" style="width: 200px;">Date</th>
                  <th class="k-header" style="width: 200px;">Added By</th>
                  <th class="k-header"></th>
                </tr>
                </thead>
                <tbody>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>

  <%= Html.Hidden("dealid", Model.DealId) %>
  <%= Html.Hidden("userid", Model.UserId) %>

  <div class="modal fade" tabindex="-1" role="dialog" id="noteinputform">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header" style="background-color: #f0f0f0;">
          <button type="button" class="close" data-dismiss="modal" aria-label="Close">
            <span aria-hidden="true">&times;</span>
          </button>
          <h4 class="modal-title">Add Note</h4>
        </div>
        <div class="modal-body" style="padding: 25px;">
          <div class="form-horizontal">
            <div class="form-group">
              <%= Html.TextArea("notebody", null, 7, 120, new {@class = "form-control"}) %>
            </div>
            <div class="form-group">
              <button class="btn btn-default pull-right" id="saveNote">Submit</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div class="modal fade" id="uploadModal">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal">
            <span aria-hidden="true">&times;</span><span class="sr-only">Close</span>
          </button>
          <h4 class="modal-title"></h4>
        </div>
        <div class="modal-body">
          <input name="files" id="files" type="file"/>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>

</asp:Content>
<asp:Content ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

    $(function() {
      window.setInterval("viewModel.updateViews()", 10000);
      $("#error").hide();
      $("#linkaddnote").click(function() { $("#noteinputform").modal("show"); });
      $("#saveNote").click(function() {
        $("#noteinputform").modal("hide");
        $.ajax(
          {
            type: "POST",
            url: "<%= Url.BuildFor<AddNoteToDealController>().AddNoteToDeal() %>",
            data: { body: $("#notebody").val(), dealid: '<%: Model.DealId %>' },
            success: function(result) {
              $("#noteinputform").modal("hide");
              viewModel.renderNotes();
              $("#notebody").val("");
            }
          });
      });
      viewModel.renderNotes();
      viewModel.renderFilesGrid();
      viewModel.renderOrdersGrid();
      viewModel.renderDealDetails();
    });

    var viewModel = {
      updateViews: function() {
        this.renderFilesGrid();
        this.renderNotes();
      },
      newJob: function(uri) {
        window.location.replace(uri);
      },
      viewOrder: function(uri) {
        window.location.replace(uri);
      },
      renderNotes: function() {
        $.ajax(
          {
            type: "GET",
            url: "<%= Url.BuildFor<DealNotesController>().GetNotesForDeal(Model.DealId) %>",
            success: function(result) {
              if (result.length > 0) {
                $("#notesMessage").hide();
                $("#notesTable").show();
                $("#notesTable tbody tr").remove();
                $.each(result,
                  function(i, v) {
                    $("#notesTable tbody").append("<tr><td>" +
                      v["CreatedDate"] +
                      "</td><td>" +
                      v["CreatedBy"] +
                      "</td><td>" +
                      v["Content"] +
                      "</td></tr>");
                  });
              } else {
                $("#notesTable").hide();
                $("#notesMessage").show().html("No notes found");
              }
            }
          });
        },
        existingFile: function(url) {
          window.location.href = url;
        },
        uploadFile: function(url) {
        // clear files from Upload control
        if ($("#files").data("kendoUpload") != null) {
          $("#files").data("kendoUpload").destroy();
          $("#files").closest(".k-upload").remove();
          $(".modal-body").append('<input name="files" id="files" type="file" />');
        }
        $("#files").kendoUpload({
          async: {
            saveUrl: url,
            autoUpload: true,
            withCredentials: false
          },
            success: function (e) {
              if (e.response.status === <%: (Int32)HttpStatusCode.OK %>) {
                  $.ajax(e.response.data,
                    {
                      contentType: 'application/json',
                      type: 'POST'
                    });
            }
            else {
              $("#errorMessage").text("The was an error uploading your file. Please contact customer support.").show();
              console.log("Failed to upload ");
            }
          },
          error: function(e) {
            var err = $.parseJSON(e.XMLHttpRequest.responseText);

            alert(err.Message);

            $.map(e.files,
              function(file) {
                alert("Could not upload " + file.name);
              });
          }
        });
        $("#uploadModal .modal-title").text('Upload File');
        $("#uploadModal").modal('show');
      },
      renderFilesGrid: function() {
        console.log("Rendering files");
        var grid = $("#filesGrid").data("kendoGrid");
        if (grid !== undefined && grid !== null) {
          grid.dataSource.read();
        } else {
          $("#filesGrid").kendoGrid({
            dataSource: {
              type: "json",
              transport: {
                read: function(options) {
                  $.ajax({
                    url: '<%= Url.Action("ForCorrelationId", "FilesApi", new{area="Operations", id=this.Model.PublicKey}) %>',
                    dataType: 'json',
                    type: 'GET',
                    data: options.data,
                    success: function(result) {
                      options.success(result);
                    }
                  });
                }
              },
              schema: {
                type: 'json',
                data: "Data",
                total: function(response) {
                  return response.Data.length;
                }
              },
              pageSize: 20,
              change: function() {
                if (this.data().length <= 0) {
                  $("#filesInfo").show();
                  $("#filesGrid").hide();
                } else {
                  $("#filesInfo").hide();
                  $("#filesGrid").show();
                }
              }
            },
            scrollable: false,
            sortable: true,
            pageable: false,
            filterable: false,
            columns: [
              {
                field: "CreatedDate",
                title: "Date Added",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" }
              },
              {
                field: "CustomerFileName",
                title: "File Name",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" }
              },
              {
                field: "FileSize",
                title: "Size",
                headerAttributes: { style: "text-align: center;" },
                attributes: { style: "text-align: center;" }
              },
              {
                template: kendo.template(
                  "<a class=\"btn btn-default\" onclick=\"viewModel.newJob('#= Actions.NewJob #')\">New Job</a>"),
                width: "150px",
                attributes: { style: "text-align: center;" }
              }
            ]
          });
        }
      },
      renderOrdersGrid: function() {
        $.ajax(
          {
            type: "GET",
            url:
              "<%= Url.Action("QueryByDeal", "OrdersApi", new {Area = "Sales", dealId = Model.DealId}) %>",
            success: function(result) {
              if (result.length > 0) {
                $("#ordersMessage").hide();
                $("#ordersTable").show();
                $("#ordersTable tbody tr").remove();
                $.each(result,
                  function(i, v) {
                    var row = "<td>" +
                      v.DateOrdered +
                      "</td><td>" +
                      v.Id +
                      "</td><td>" +
                      v.Status +
                      "</td><td>" +
                      (new Intl.NumberFormat("en-US",
                        { style: "currency", currency: "USD", minimumFractionDigits: 2 })).format(v.Amount) +
                      "</td>";

                    var viewButton = (v.Links.View != null)
                      ? "<a class=\"btn btn-default\" href=\"" + v.Links.View + "\">Details</a>"
                      : "";
                    var editButton = (v.Links.Edit != null)
                      ? "<a class=\"btn btn-default\" href=\"" + v.Links.Edit + "\">Edit</a>"
                      : "";
                    var chargeButton = (v.Links.Process != null)
                      ? "<a class=\"btn btn-primary\" href=\"" + v.Links.Process + "\">Charge</a>"
                          : "";
                    var pdfButton = "<a class=\"btn btn-default\" href=\"" + v.Links.DownloadPdf + "\">PDF</a>";
                      
                    row = "<tr>" +
                      row +
                      "<td style=\"text-align: right;\">" +
                      pdfButton + 
                      " " +
                      editButton +
                      " " +
                      viewButton +
                      " " +
                      chargeButton +
                      "</td></tr>";

                    $("#ordersTable tbody").append(row);
                  });
              } else {
                $("#ordersTable").hide();
                $("#ordersMessage").show().html("No orders found");
              }
            }
          });
      },
      renderDealDetails: function() {
        console.log("Rendering deal details");

        $.ajax(
          {
            type: "GET",
            url: "<%= Url.Action("ById", "DealsApi", new {Area = "Sales", dealId = Model.DealId}) %>",
            success: function(result) {

              var html = "<strong>Title:</strong> " +
                result.Title +
                "<br>" +
                "<strong>Description:</strong> " +
                result.Description +
                "<br>" +
                "<strong>Sales Rep:</strong> " +
                result.Owner.UserName +
                "<br>" +
                "<strong>Amount (Suggested):</strong> " +
                (result.Amount === 0
                  ? "None"
                  : (new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2 }))
                  .format(result.Amount)) +
                "<br>" +
                "<strong>Deal Id:</strong> " +
                result.DealId +
                "<br>" +
                "<strong>Date Added:</strong> " +
                result.CreatedDate +
                "<br>" +
                "<strong>Status:</strong> " +
                result.StatusDescription +
                "<br>" +
                "<strong>Auto-Bill:</strong> " + (result.EnableAutoBill ? "Enabled" : "Disabled") + "<br/>" +
                "<strong>Internal Notifications:</strong> Disabled<br>" +
                "<div style=\"margin: 10px 0;\">";

              if (result.Links.Edit != null) {
                html = html +
                  " <button class=\"btn btn-default\" onclick=\"window.location = '" +
                  result.Links.Edit +
                  "'\">Edit</button>";
              }

              if (result.Links.Bill != null) {
                html = html +
                  " <button class=\"btn btn-primary\" title=\"Will create a new bill (invoice or receipt) for the deal and send it for sales approval\" onclick=\"window.location = '" +
                  result.Links.Bill +
                  "'\">Create Bill</button>";
              }

              if (result.Links.Review != null) {
                html = html +
                  " <button class=\"btn btn-primary\" title=\"Review this deal for sales approval\" onclick=\"window.location = '" +
                  result.Links.Review +
                  "'\">Review Bill</button>";
              }

              if (result.Links.Charges != null) {
                html = html +
                  " <button class=\"btn btn-default\" title=\"View Charges\" onclick=\"window.location = '" +
                  result.Links.Charges +
                  "'\">View Charges</button>";
              }

              if (result.Links.Expire != null) {
                html = html +
                  " <button class=\"btn btn-danger\" title=\"Reset Deal and Delete Bill\" onclick=\"window.location = '" +
                  result.Links.Expire +
                  "'\">Reset</button>";
              }

              if (result.Links.Refund != null) {
                html = html +
                  " <button class=\"btn btn-warning\" title=\"Process a refund deal\" onclick=\"window.location = '" +
                  result.Links.Refund +
                  "'\">Process Refund</button>";
              }

              if (result.Actions.SendPaymentUpdate != null) {
                html = html +
                  " <button class=\"btn btn-warning\" title=\"Send payment email\" onclick=\"window.location = '" +
                  result.Actions.SendPaymentUpdate +
                  "'\">Send payment email</button>";
              }

              html = html + "</div>";

              $("#dealDetails").html(html);

              if (result.ProcessingInstructions) {
                html = '<div class="panel-heading">Instructions</div>' +
                  '<div class="panel-body">' +
                  result.ProcessingInstructions.replace("\r\n", "<br />") +
                  '</div>';

                $("#dealInstructions").html(html).show();
              }
            }
          });
      }
    };

  </script>

</asp:Content>