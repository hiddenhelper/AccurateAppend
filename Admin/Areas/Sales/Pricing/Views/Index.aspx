<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<ClientModel>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Sales" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Edit Rate Cards
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <% var title = "Edit Rate Cards " + (String.IsNullOrEmpty(Model.UserName) ? ": System" : ""); %>

  <div class="row" style="padding: 20px;">
    <div class="row">
      <div class="col-md-5">
        <h3 style="margin-top: 0;"><%: title %></h3>
        <div class="form-group" id="categoryControl">
          <label for="category">Rate Card</label>
          <select class="form-control" id="category" style="width: 150px;">
            <option value="<%: Cost.DefaultCategory %>" selected="selected"><%: Cost.DefaultCategory %></option>
            <option value="<%: Cost.NationBuilderCategory %>"><%: Cost.NationBuilderCategory %></option>
            <option value="<%: Cost.CsvUploadCategory %>"><%: Cost.CsvUploadCategory %></option>
            <option value="<%: Cost.ListBuilderCategory %>"><%: Cost.ListBuilderCategory %></option>
          </select>
        </div>
        <% if (this.Model.UserId != null) { %>
        <div id="partyDetail">
          <% Html.RenderPartial("~/Views/Shared/PartyDetail2.ascx", Model.UserId.Value); %>
        </div>
        <% } %>
      </div>
    </div>
    <div class="row">
      <div class="col-md-12">
        <div class="form-group" id="productsControl">
          <label for="products">Create New Rate Card</label>
          <select class="form-control" id="products" name="product" style="width: 350px;"></select>
        </div>
        <button class="btn btn-primary" id="editButton" style="margin-bottom: 20px;">Next</button>
        <% if (this.Model.UserId != null) { %>
        <p style="font-weight: bold;">Existing Rate Cards</p>
        <div id="existingCardsGrid" style="display: none;"></div>
        <div id="existingCardsMessage" class="alert alert-info" style="display: none;"></div>
        <button class="btn btn-warning" id="nukeButton" style="display: none; margin-top: 20px;">Delete all existing rate cards for this User</button>
        <button class="btn btn-default" id="downloadButton" style="display: none; margin-top: 20px;">Download all rate cards</button>
        <% } %>
      </div>
    </div>
  </div>

  <script type="text/javascript">

    var userid = '<%: Model.UserId %>';

    $(function() {

      updateProductSelect();
      loadUserOperatingMetricReport();

      $("#editButton").click(function() {
        history.pushState(null, 'Rate Cards', '<%= Url.BuildFor<PricingController>().ToIndex() %>');
        window.location.replace('<%= Url.BuildFor<PricingController>().ToEdit() %>' +
          '?product=' +
          $("#products").val() +
          '&category=' +
          (userid ? userid : $("#category").val()));
      });

      $("#nukeButton").click(function() {
        window.location.replace('<%= Url.Action("DeleteAll", new {Model.UserId}) %>');
      });

      $("#downloadButton").click(function() {
        window.location.replace('<%= this.Model.DownloadLink %>');
      });

      if (userid) $("#categoryControl").hide();
        if (!userid) {
          $("#nukeButton").hide();
          $("#downloadButton").hide();
        }
    });

    function updateProductSelect() {
      $.getJSON('<%= Url.Action("AvailableProducts", "Pricing", new {area = "Sales"}) %>',
        { category: userid ? userid : $("#category").val() }).done(function(data) {
        if (!data.length) {
          $("#productsControl").hide();
          $("#editButton").hide();
          $("#noRecordFoundMessage").show();
        } else {
          $("#productsControl").show();
          $("#editButton").show();
          $("#noRecordFoundMessage").hide();
        }
        $.each(data,
          function(i, o) {
            $("#products").append("<option value=" + o.Name + ">" + o.Name + " (" + o.Title + ")</option>");
          });
      });
    };

    function loadUserOperatingMetricReport() {
      var grid = $("#existingCardsGrid").data("kendoGrid");
      if (grid !== undefined && grid !== null) {
        grid.dataSource.read();
      } else {
        $("#existingCardsGrid").kendoGrid({
          dataSource: {
            type: "json",
            transport: {
              read(options) {
                $.ajax({
                  url: '<%= Url.Action("ExistingCards", "Pricing", new {area = "Sales", userid = Model.UserId}) %>',
                  dataType: "json",
                  type: "GET",
                  success(result) {
                    options.success(result);
                  }
                });
              },
              cache: false
            },
            schema: {
              type: "json",
              data: "Data",
              total(response) {
                return response.Data.length;
              }
            },
            change: function() {
              if (this.data().length <= 0) {
                $("#existingCardsMessage").text('No existing rate cards for this user.').show();
                $("#existingCardsGrid").hide();
                $("#nukeButton").hide();
                $("#downloadButton").hide();
              } else {
                $("#existingCardsGrid").show();
                $("#nukeButton").show();
                $("#downloadButton").show();
              }
            }
          },
          columns: [
            {
              field: "Name",
              title: "Product"
            },
            {
              field: "Description",
              title: "Description"
            },
            {
              width: 140,
              attributes: { style: "text-align: center;" },
              template: kendo.template(
                "<a href=\"#=Links.DeleteCard#\" class=\"btn btn-danger\">Delete</a><a href=\"#=Links.EditCost#\" class=\"btn btn-primary\" style=\"margin-left: 5px;\">Edit</a>")
            }
          ],
          scrollable: false
        });
      }
    }

  </script>

</asp:Content>