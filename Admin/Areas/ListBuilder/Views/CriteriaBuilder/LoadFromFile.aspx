<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.ListBuilder.Models.ListCriteria>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Load Defintion From File
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row" style="padding: 20px;">
        <div class="col-md-4">
            <h2>Load List Defintion</h2>
            <% using (Html.BeginForm("LoadFromFile", "CriteriaBuilder", FormMethod.Post, null))
               { %>
                <div class="form-group">
                    <input name="files" id="files" type="file" aria-label="files" class="form-control"/>
                    <p style="padding-top: 1em; text-align: right">
                        <button type="submit" class="btn btn-default">Submit</button>
                    </p>
                </div>
            <% } %>    
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {
            $("#files").kendoUpload();
        });

    </script>

</asp:Content>