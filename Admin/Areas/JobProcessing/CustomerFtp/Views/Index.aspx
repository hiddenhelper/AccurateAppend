<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Customer FTP
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <div class="row" style="padding: 0 20px 0 20px;">

    <iframe src="<%: this.Url.Action("Root") %>" width="825" height="700" style="border: none"></iframe>

  </div>

</asp:Content>