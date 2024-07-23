<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Controllers" %>
<%@ Import Namespace="AccurateAppend.Websites.Admin.Navigator" %>
<%
    var url = Page.Request.RawUrl;
    Response.Write("<ul class=\"nav nav-pills nav-stacked visible-md visible-lg \" id=\"sidenav\">");
    Response.Write("<li" + (url.ToLower().Contains("leads") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToLeads() + "\"><span class=\"fa fa-book\"></span> Leads</a>");
    Response.Write("</li>");

    if (!SecurityExtensions.IsLimitedAccess())
    {
      Response.Write("<li" + (url.ToLower().Contains("tickets") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<ListTicketsController>().ToIndex() + "\"><span class=\"fa fa-tasks\"></span> Tickets & Tasks</a>");
      Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("users") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToUsers() + "\"><span class=\"fa fa-users\"></span> Users</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("deals") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToDeals() + "\"><span class=\"fa fa-money\"></span> Deals</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower() == "/" || url.ToLower().Contains("jobs") || url.ToLower().Contains("batch") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToJobs() + "\"><span class=\"fa fa-gears\"></span> Jobs</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("chargeeventsummary") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToChargeEvents() + "\"><span class=\"fa fa-cc-mastercard\"></span> Charge Events</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("/userfiles") || url.ToLower().Contains("rawinputfiles") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToFiles() + "\"><span class=\"fa fa-files-o\"></span> Files</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("message") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToMessages() + "\"><span class=\"fa fa-envelope-o\"></span> Messages</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower()== "/operations/eventlog" ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToEventLog() + "\"><span class=\"fa fa-exclamation-circle\"></span> Event Log</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("reporting") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToReporting() + "\"><span class=\"fa fa-line-chart\"></span> Reporting</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("documents") ? " class='active'" : string.Empty) + "><a href=\"https://accurateappendazure.sharepoint.com/sites/AccurateAppend\" target=\"_new\"><span class=\"fa fa-files-o\"></span> Sharepoint</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("customerftp") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.Action("Index", "CustomerFtp", new { Area = "JobProcessing" }) + "\"><span class=\"fa fa-folder-o\"></span> Customer FTP</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower() == "/operations/dashboard" ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToOperations() + "\"><span class=\"fa fa-bank\"></span> System Configuration</a>");
     Response.Write("</li>");

     Response.Write("<li" + (url.ToLower().Contains("listbuilder") ? " class='active'" : string.Empty) + "><a href=\"" + this.Url.BuildFor<MenuController>().ToListBuilder() + "\"><span class=\"fa fa-list\"></span> List Builder</a>");
     Response.Write("</li>");

    }
    Response.Write("</ul>");
%>



