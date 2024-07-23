<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Error | Accurate Append
</asp:Content>
<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="carousel">
        <div class="container">
            <ul class="slides">
                <li>
                    <div class="block fixed error">

                        <h1>We're sorry, something is not quite right.</h1>

                        <h4 style="color: #696969">Please back up and try again or <a href="http://www.accurateappend.com/get-in-touch/" class="blue-link">contact customer</a> to file a support request</h4>

                        <p>
                            <% if (this.TempData["message"] != null) { %>
                            <%: this.TempData["message"] %>
                            <% } %>
                        </p>

                    </div>
                </li>

            </ul>
        </div>
        <!-- container -->
    </div>
    <!-- carousel -->
    
</asp:Content>
