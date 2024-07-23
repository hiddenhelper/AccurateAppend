<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<JobConfigurationController.Mode>" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Operations.JobConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Operations
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (this.Html.BeginForm())
       { %>
    <div class="row">

        <div class="col-md-4">

            <div class="panel panel-default">
                <div class="panel-heading">Job Scheduling Mode</div>
                <div class="panel-body">
                    <label for="<%: JobConfigurationController.Mode.Fairness %>">Prefer Fairness</label><%= this.Html.RadioButton("mode", JobConfigurationController.Mode.Fairness, this.Model == JobConfigurationController.Mode.Fairness, new {id = JobConfigurationController.Mode.Fairness.ToString()}) %>
                    <label for="<%: JobConfigurationController.Mode.Priority %>">Strict Priority</label><%= this.Html.RadioButton("mode", JobConfigurationController.Mode.Priority, this.Model == JobConfigurationController.Mode.Priority, new {id = JobConfigurationController.Mode.Priority.ToString()}) %>
                </div>
            </div>

        </div>

    </div>
    <input type="submit" value="Update"/>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">


</asp:Content>
