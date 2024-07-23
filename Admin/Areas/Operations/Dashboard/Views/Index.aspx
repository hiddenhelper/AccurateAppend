<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Sales.Pricing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Operations
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row">

        <div class="col-md-4">

            <div class="panel panel-default">
                <div class="panel-heading">Job Management Functions</div>
                <div class="panel-body">
                    <div id="Suppression"><%= this.Html.ActionLink("Upload Suppression", "Index", "UploadSuppression", new {Area="Operations"}, null) %></div>
                    <div id="JobSchedulingMode"><%= this.Html.ActionLink("Job Scheduling", "Scheduling", "JobConfiguration", new {Area="Operations"}, null) %></div>
                    <div id="CheckManifest"><%= this.Html.ActionLink("Validate Manifest", "Validate", "JobConfiguration", new {Area="Operations"}, null) %></div>
                </div>
            </div>

        </div>

        <div class="col-md-4">
            
            <div class="panel panel-default">
                <div class="panel-heading">Billing Functions</div>
                <div class="panel-body">
                    <div id="RateCards"><%= this.Html.NavigationFor<PricingController>().ToIndex("System Rate Cards") %></div>
                </div>
            </div>

        </div>

    </div>

    <div class="row">

        <div class="col-md-5">

            <div class="panel panel-default">
                <div class="panel-heading">System Management</div>
                <div class="panel-body">
                    <div id="CreateUser"><%= this.Html.ActionLink("Create User", "Index", "CreateAdminUser", new {Area="Operations"}, null) %></div>
                    <div id="Systems"><%= this.Html.ActionLink("Systems Status", "Index", "Systems", new {Area="Operations"}, null) %></div>
                    <div id="Bus"><%= this.Html.ActionLink("Failed Bus", "Failed", "Bus", new {Area="Operations"}, null) %></div>
                </div>
            </div>

        </div>

        <div class="col-md-3">

            <div class="panel panel-default">
                <div class="panel-heading">Misc. Functions</div>
                <div class="panel-body">
                    <div id="Decrypt Pgp"><%= this.Html.ActionLink("Decrypt PGP file", "Upload", "Pgp", new {Area="Operations"}, null) %></div>
                    <div id="TFS"><a href="https://accurateappend.visualstudio.com/Core/Core%20Team/_dashboards/Core%20Team"></a></div>
                  <div id="Slack"><a href="https://accurateappend.slack.com">Slack Channels</a></div>
                  <div id="JSlogger"><a href="http://jslogger.com/manage#logs">JSLogger</a></div>
                </div>
            </div>

        </div>

    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">


</asp:Content>
