<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Systems Status
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="row" style="padding: 0 20px 0 20px;">

        <div class="col-lg-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Systems Info</h3>
                </div>
                <div class="panel-body" style="line-height: 1em">
                    <div id="systemInfo"></div>
                </div>
            </div>
        </div>

    </div>

    <script type="text/x-kendo-template" id="templateSystems">
 
        <table class="table table-condensed">
            <tr>
                <th>System</th>
                <th>Host</th>
                <th>Heatbeat</th>
                <th>User</th>
                <th>Id</th>
                <th>Version</th>
            </tr>
            # for (var i = 0; i < data.length; i++) { #
           <tr>
                <td>${ data[i].SystemName }</td>
                <td>${ data[i].Host }</td>
                <td>${ data[i].Heartbeat }</td>
                <td>${ data[i].UserName }</td>
                <td>${ data[i].Id }</td>
                <td>${ data[i].Version }</td>
            </tr>
            # } #
        </table>

    </script>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
    
    <script type="text/javascript" src="<%= this.Url.Content("~/Scripts/loading-overlay/loadingoverlay.js") %>"></script>

    <script type="text/javascript">

        $(function () {
            systemsModel.refreshView();
            setInterval(systemsModel.refreshView, 30000);
        });

        var systemsModel = {
            refreshView: function () {
                $.ajax({
                    url: "<%: this.Url.Action("Query", "Systems", new {area = "Operations"}) %>",
                    dataType: 'json',
                    type: 'GET',
                    success: function (result) {
                        $("#systemInfo").html(kendo.template($("#templateSystems").html())(result));
                    },
                    error: function() {
                        $("#systemInfo").html('<div id="notice" class="alert alert-warning" style="margin: 20px 0 20px 0;"><strong>Unable to render systems</strong></div>');
                    }
                });
            }
        }

    </script>

</asp:Content>