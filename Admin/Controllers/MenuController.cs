using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary;
using AccurateAppend.Websites.Admin.Areas.Clients.UserFiles;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers;
using AccurateAppend.Websites.Admin.Areas.Operations.Controllers;
using AccurateAppend.Websites.Admin.Areas.Operations.Dashboard;
using AccurateAppend.Websites.Admin.Areas.Operations.EventLog;
using AccurateAppend.Websites.Admin.Areas.Operations.Message;
using AccurateAppend.Websites.Admin.Areas.Operations.UserStatus;
using AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary;
using AccurateAppend.Websites.Admin.Areas.Sales.DealSummary;
using AccurateAppend.Websites.Admin.Navigator;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;

namespace AccurateAppend.Websites.Admin.Controllers
{
    /// <summary>
    /// Transitive controller used with menu links specific to record user actions.
    /// </summary>
    [Authorize()]
    public class MenuController : ActivityLoggingController2
    {
        #region Actions

        /// <summary>
        /// Directs interactive caller to the Users screen.
        /// </summary>
        public ActionResult ToUsers()
        {
            this.OnEvent("Opening users from sidebar menu");
            return this.NavigationFor<UserSummaryController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Leads screen.
        /// </summary>
        public ActionResult ToLeads()
        {
            this.OnEvent("Opening leads from sidebar menu");
            return this.NavigationFor<LeadSummaryController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Deals screen.
        /// </summary>
        public ActionResult ToDeals()
        {
            this.OnEvent("Opening deals from sidebar menu");
            return this.NavigationFor<DealSummaryController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Files screen.
        /// </summary>
        public ActionResult ToFiles()
        {
            this.OnEvent("Opening files from sidebar menu");
            return this.NavigationFor<UserFilesController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Jobs screen.
        /// </summary>
        public ActionResult ToJobs()
        {
            this.OnEvent("Opening jobs from sidebar menu");
            return this.NavigationFor<SummaryController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Messages screen.
        /// </summary>
        public ActionResult ToMessages()
        {
            this.OnEvent("Opening messages from sidebar menu");
            return this.NavigationFor<MessageController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Event Log screen.
        /// </summary>
        public ActionResult ToEventLog()
        {
            this.OnEvent("Opening event log from sidebar menu");
            return this.NavigationFor<EventLogController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the List Builder App.
        /// </summary>
        public ActionResult ToListBuilder()
        {
            this.OnEvent("Opening list builder from sidebar menu");
            return this.NavigationFor<BuildListController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Charge events screen.
        /// </summary>
        public ActionResult ToChargeEvents()
        {
            this.OnEvent("Opening charge events reporting from sidebar menu");
            return this.NavigationFor<ChargeEventSummaryController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Operations dashboard screen.
        /// </summary>
        public ActionResult ToOperations()
        {
            this.OnEvent("Opening operations from sidebar menu");
            return this.NavigationFor<DashboardController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the User status screen.
        /// </summary>
        public ActionResult ToUserActivity()
        {
            this.OnEvent("Opening user status from sidebar menu");
            return this.NavigationFor<UserStatusController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Reporting screen.
        /// </summary>
        public ActionResult ToReporting()
        {
            this.OnEvent("Opening reporting from sidebar menu");
            return this.NavigationFor<Areas.Reporting.Controllers.DashboardController>().ToIndex();
        }

        /// <summary>
        /// Directs interactive caller to the Support Tickets screen.
        /// </summary>
        public ActionResult ToTickets()
        {
            this.OnEvent("Opening tickets from sidebar menu");
            return this.RedirectToAction("Index", "ListTickets", new {area = "Tickets"});
        }

        /// <summary>
        /// Directs interactive caller to the Support Tickets screen.
        /// </summary>
        public ActionResult ToSystemConfiguration()
        {
            this.OnEvent("Opening system configuration from sidebar menu");
            return this.RedirectToAction("Index", "Dashboard", new {area = "Operations"});
        }

        #endregion
    }
}
