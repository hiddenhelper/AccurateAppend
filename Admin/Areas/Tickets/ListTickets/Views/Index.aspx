<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/bootstrap3.Master" Inherits="System.Web.Mvc.ViewPage<AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets.Models.PageSettings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tickets
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <div class="row" style="">
    <div class="col-md-3 pull-left"><span style="margin-left: 0;" id="ticketsDateRange"></span></div>
    <div class="col-md-3 pull-right"><a class="btn btn-default pull-right" style="margin-right: 5px;" target="_blank" href="<%= this.Model.ZenDesk %>">Go to Zendesk</a></div>
  </div>
  
  <div class="row" style="padding: 0 20px 0 20px;">
    <div class="alert alert-info" style="display: none; margin: 20px 0 20px 0;" id="gridMessage"></div>
    <div id="grid" style="margin-bottom: 20px; margin-top: 10px;"></div>
  </div>
    
  <div class="modal" tabindex="-1" role="dialog" id="detailsModal">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-label="Close"></button>
          <h4 class="modal-title"></h4>
        </div>
        <div class="modal-body"><pre></pre></div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
        </div>
      </div>
    </div>
  </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DocumentStart" runat="server">
  
  <script src="<%= Url.Content("~/Areas/Tickets/ListTickets/Scripts/ListTickets.js") %>" type="text/javascript"></script>
  <script src="<%= Url.Content("~/Scripts/moment.min.js") %>" type="text/javascript"> </script>
  <script type="text/javascript">

    var queryUrl = '<%= this.Model.Data %>';

  </script>

  <script id="responsive-column-template-complete" type="text/x-kendo-template">
  <strong>Date Created</strong>
  <p class="col-template-val">#= data.CreatedAt #</p>

  <strong>Ticket Type</strong>
  <p class="col-template-val">#= data.Type #</p>

  <strong>Status</strong>
  <p class="col-template-val">#= data.Status #</p>
  
  <strong>Subject</strong>
  <p class="col-template-val">#=data.Subject #</p>
    
  <a href="\\#" class="btn btn-default" onclick="ticketsApiViewModel.viewDetail('#= uid #')">View Details</a>
  <a href="#= data.Url #" target="_blank" class="btn btn-default">View In Zendesk</a>

  </script>
   
</asp:Content>
