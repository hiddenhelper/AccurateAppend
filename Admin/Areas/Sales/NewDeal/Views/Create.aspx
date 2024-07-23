<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<DealModel>" %>
<%@ Import namespace="AccurateAppend.Sales.Contracts.ViewModels" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Views" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= this.Html.ValidationSummary() %>

    <h3 style="margin-top: 0;">Create New Deal</h3>
    <div class="row" style="padding: 0 0 20px 20px;">
        <% using (this.Html.BeginForm())
           { %>
            <% = this.Html.HiddenFor(m => m.UserId) %>
            <table class="table table-condensed">
                <tr>
                    <th>
                        Title / File Name
                    </th>
                    <td>
                        <%= this.Html.TextBoxFor(m => m.Title, new {@class = "form-control", cols = 40}) %>
                    </td>
                </tr>
                <tr>
                    <th>
                        Description
                    </th>
                    <td>
                        <%= this.Html.TextBoxFor(m => m.Description, new {@class = "form-control", cols = 40}) %>
                    </td>
                </tr>
                <tr>
                    <th>
                        <%= this.Html.LabelFor(m => m.Instructions) %>
                    </th>
                    <td>
                        <%= this.Html.TextAreaFor(m => m.Instructions, new {@class = "form-control", rows = 10, cols = 47}) %>
                    </td>
                </tr>
                <tr>
                    <th>
                        <%= this.Html.LabelFor(m => m.Amount) %>
                    </th>
                    <td>
                        <%= this.Html.TextBoxFor(m => m.Amount, new {@class = "form-control", rows = 10, cols = 40}) %>
                    </td>
                </tr>
                <tr>
                  <th>
                    Deal Owner
                  </th>
                  <td>
                    <%= this.Html.AdminUsersDropDown(id: "OwnerId", style:"width: 175px;display: inline;") %> 
                  </td>
                </tr>
                <tr>
                    <th>
                        Options
                    </th>
                    <td>
                        <%= this.Html.LabelFor(m => m.AutoBill) %> <%= this.Html.CheckBoxFor(m => m.AutoBill) %> / <%= this.Html.LabelFor(m => m.SuppressNotifications) %> <%= this.Html.CheckBoxFor(m => m.SuppressNotifications) %>
                    </td>
                </tr>
                <tr>
                    <th>
                    </th>
                    <td>
                        <input type="submit" value="Create" class="btn btn-primary"/>
                    </td>
                </tr>
            </table>
        <%= this.Html.Hidden("Identifier", ViewData["identifier"]) %>
        <% } %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

</asp:Content>