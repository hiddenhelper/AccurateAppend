<%@ Page Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
  Error
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
  
    <div class="row" style="padding: 0 0 0 20px;" >
        <% if (TempData["message"] != null)
       { %>
    <div class="alert alert-danger">
      <%: TempData["message"] %>
    </div>
    <% } %>

    </div>

</asp:Content>
