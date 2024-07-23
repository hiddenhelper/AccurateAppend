<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<JobPreview>" %>
<%@ Import namespace="AccurateAppend.Websites.Admin.Areas.JobProcessing.PreviewJob" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
File Preview
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="row" style="padding: 0 20px 0 20px; overflow: scroll; max-height: 800px;">

    <h4><%: Model.CustomerFileName.ToUpper() %> </h4>
    <h4>Job Id: <%: Html.ActionLink(Model.JobId.ToString(), "Index", "Summary", new { jobid = Model.JobId, area="JobProcessing" }, null) %></h4>
    <h4>Date Submitted: <%: Model.DateSubmitted %> </h4>
    <h4>File Type: <%: Model.FileType %> </h4>
    <h4 style="margin-bottom: 20px;">Displaying <%: Model.CsvRows.Count.ToString("N0") %> of <%: Model.TotalRecords.ToString("N0") %></h4>
    <button id="download" class="btn btn-default" style="margin-bottom: 20px;">Download</button>
    <table class="table table-condensed table-bordered" id="reports">

        <%
            if (Model.ProspectorGraph != null)
            {
                Response.Write("<tr><th colspan=\"" + Model.CsvRows.First().Count() + "\" style=\"background-color: #e0e0e0; font-size: .8em;\"><a href=\"#\" id=\"linkProspectorGraph\">Column Report<a/></th></tr>");

                Response.Write("<tr data-parent=\"prospectorGraph\">");

                foreach (var t in Model.ProspectorGraph.Columns)
                {
                    Response.Write("<td>");

                    Html.RenderPartial("ColumnReport", t);

                    Response.Write("</td>");
                }

                Response.Write("</tr>");
            }

            Response.Write("<tr><th colspan=\"" + Model.CsvRows.First().Count() + "\" style=\"background-color: #e0e0e0; font-size: .8em;\">Column Mapping</th></tr>");

            Response.Write("<tr>");

            for (var i = 0; i < Model.ColumnMap.Count(); i++)
            {
                Response.Write("<td style=\"color:green;font-weight: bold; font-size: .8em;\">" + Model.ColumnMap[i] + "</td>");
            }

            Response.Write("</tr>");

            Response.Write("<tr><th colspan=\"" + Model.CsvRows.First().Count() + "\" style=\"background-color: #e0e0e0; font-size: .8em;\">File Contents</th></tr>");

            for (var i = 0; i < Model.CsvRows.Count; i++)
            {
                if (i == 0 && Model.HasHeader)
                {
                    Response.Write("<tr>");

                    foreach (var cell in Model.CsvRows[i])
                    {
                        Response.Write("<th style=\"background-color: #e0e0e0; font-size: .8em;\">" + cell + "</th>");
                    }

                    Response.Write("</tr>");
                }
                else
                {
                    Response.Write("<tr>");

                    foreach (var cell in Model.CsvRows[i])
                    {
                        Response.Write("<td style=\"font-size: .8em;\">" + cell + "</td>");
                    }

                    Response.Write("</tr>");
                }
            }

             %>

    </table>

</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script>

        var fileType = '<%: Model.FileType %>';
        var jobid = <%: Model.JobId %>;

        $(function() {

            $("#download").click(function() {
                if (fileType === "Input") {
                    window.location.replace("/JobProcessing/DownloadFiles/Input?jobid=" + jobid);
                } else {
                    window.location.replace("/JobProcessing/DownloadFiles/Output?jobid=" + jobid);
                }
            });

            $("#prospectorGraph").toggle();

            $("#linkProspectorGraph").click(function () {
                $("#prospectorGraph").toggle();
            });

        });

    </script>

</asp:Content>