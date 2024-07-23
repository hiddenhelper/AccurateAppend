<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bus Status
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
 <div id="window"></div>

    <div class="row" style="padding: 0 20px 0 20px;">

        <div class="col-lg-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Failed Bus Messages</h3>
                </div>
                <div class="panel-body" style="line-height: 1em">
                    <div id="messageList"></div>
                </div>
            </div>
        </div>

    </div>

    <script type="text/x-kendo-template" id="templateMessages">
 
        <table class="table table-condensed">
            <tr>
                <th>Id</th>
                <th>Queue</th>
                <th>Header</th>
                <th>Body</th>
            </tr>
            # for (var i = 0; i < data.length; i++) { #
           <tr>
                <td><a href="${ data[i].EventLog }">${ data[i].Id }</a></td>
                <td>${ data[i].Queue }</td>
                <td><a href="javascript: showWindow('${ data[i].Headers }');">Show</a></td>
                <td><a href="javascript: showWindow('${ data[i].Body }');">Show</a></td>
            </tr>
            # } #
        </table>

    </script>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    
    <script type="text/javascript" src="<%= this.Url.Content("~/Scripts/loading-overlay/loadingoverlay.js") %>"></script>

    <script type="text/javascript">

       var displayWindow;

        $(function () {
            systemsModel.refreshView();
            setInterval(systemsModel.refreshView, 60000);

            displayWindow = $("#window");

        });

        var systemsModel = {
            refreshView: function () {
                $.ajax({
                    url: "<%: this.Url.Action("Query", "Bus", new {area = "Operations"}) %>",
                    dataType: 'json',
                    type: 'GET',
                    success: function (result) {
                     $("#messageList").html(kendo.template($("#templateMessages").html())(result));
                    },
                    error: function() {
                     $("#messageList").html('<div id="notice" class="alert alert-warning" style="margin: 20px 0 20px 0;"><strong>Unable to render bus</strong></div>');
                    }
                });
            }
        }

     function showWindow(content) {
      
      displayWindow.html("<pre>" + content + "</pre>");

      displayWindow.kendoWindow({
        width: "1200px",
        title: "",
        visible: false,
        actions: [
            "Close"
        ]
       }).data("kendoWindow").center().open();
      }

    </script>

</asp:Content>