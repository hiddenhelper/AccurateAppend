<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if(TempData["message"] != null) { %>
<div id="notice">
    <strong>Error:</strong>&nbsp;<%= TempData["message"] %>
</div>

<% } %>
