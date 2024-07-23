<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<Guid>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Ledger
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <script type="text/x-kendo-template" id="templateLedger">
 
        <table class="table table-condensed">
            <tr>
                <th>Id</th>
                <th>Type</th>
                <th>Effective Through</th>
                <th>Deal</th>
            </tr>
            # for (var i = 0; i < data.length; i++) { #
           <tr>
                <td>${ data[i].Id }</td>
                <td>${ data[i].EntryType }</td>
                <td>${ kendo.toString(kendo.parseDate(data[i].PeriodStart), 'yyyy-MM-dd') }-${ kendo.toString(kendo.parseDate(data[i].PeriodEnd), 'yyyy-MM-dd') }</td>
                <td>
 #if(data[i].Links.Deal != null)
    {#
                  <a href="${ data[i].Links.Deal }">Deal</a>
    # }#
                </td>
           </tr>
          # } #
        </table>
    </script>

  <div>

    <div class="row" style="padding: 0 20px 0 20px;">

      <div class="col-lg-12">
        <div class="panel panel-default">
          <div class="panel-heading">
            <h3 class="panel-title">Ledger Report</h3>
          </div>
          <div class="panel-body" style="line-height: 1em">
            <div id="ledgerList"></div>
          </div>
        </div>
      </div>

    </div>

  </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

  <script type="text/javascript">

      $(function () {
        viewModel.refreshView();
      });

    var viewModel = {
      refreshView: function() {
        $.ajax({
          url: "<%: this.Url.Action("Query", "LedgerReport", new {area = "Sales", userId = this.Model }) %>",
          dataType: 'json',
          type: 'GET',
          success: function(result) {
            $("#ledgerList").html(kendo.template($("#templateLedger").html())(result));
          },
          error: function() {
            $("#ledgerList")
              .html(
                '<div id="notice" class="alert alert-warning" style="margin: 20px 0 20px 0;"><strong>Unable to render ledger</strong></div>');
          }
        });
      }
    };

  </script>
  
</asp:Content>