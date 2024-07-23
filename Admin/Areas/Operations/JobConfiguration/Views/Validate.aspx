<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ValidationResult>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Operations
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (this.Html.BeginForm())
       { %>
    <div class="alert alert-warning alert-dismissible fade show">
      <% foreach (var error in this.Model)
         { %>
        <li><%:error.ErrorMessage %></li>
         <% }%>
    </div>
    <div class="row">

        <div class="col-md-8">

            <div class="panel panel-default">
                <div class="panel-heading">Validate Manifest</div>
                <div class="panel-body">
                  <textarea name="manifest" rows="10" cols="100"></textarea>
                </div>
            </div>

        </div>

    </div>
    <input type="submit" value="Update"/>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">


</asp:Content>
