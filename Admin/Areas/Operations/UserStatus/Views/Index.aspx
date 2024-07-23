<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    User Activity
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- user activity for mobile view-->

    <div>
        
        <h2>User Activity</h2>
        <ul style="padding-left: 0" class="activity userStatus"></ul>

    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">

    <script type="text/javascript">

        $(document).ready(function() {

            AccurateAppend.Bootstrap.updateUserStatus();

        });
        
    </script>

</asp:Content>