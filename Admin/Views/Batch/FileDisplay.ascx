<%@ Control Language="C#" Inherits="ViewUserControl<List<AccurateAppend.Websites.Admin.Entities.BatchJobRequestFile>>" %>
<%
    foreach (var file in Model)
    {
        Response.Write("<p style=\"margin-bottom: 2px;\">" + file.ClientFileName + " (" + file.RecordCount.ToString() + " Records)</p>");
    }
%>