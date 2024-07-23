<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Operations
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (this.Html.BeginForm())
       { %>
    <div class="row">

        <div class="col-md-4">

            <div class="panel panel-default">
                <div class="panel-heading">Create User</div>
                <div class="panel-body">
                    <label for="userName">User Name:</label><%= this.Html.TextBox("userName", "", new {id = "userName"}) %><br/>
                    <label for="password">Password:</label><%= this.Html.TextBox("password", "", new {id = "password"}) %>
                </div>
            </div>

        </div>

    </div>
    <input type="submit" value="Create"/>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">


</asp:Content>
