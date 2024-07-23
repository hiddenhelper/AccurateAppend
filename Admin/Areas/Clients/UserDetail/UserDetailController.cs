using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.AdminFile;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail;
using AccurateAppend.Websites.Admin.Areas.SecurityManagement.LogInAsClient;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserDetail
{
    /// <summary>
    /// Controller to access details about a <see cref="Client"/>.
    /// </summary>
    [Authorize()]
    public class UserDetailController : ActivityLoggingController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public UserDetailController(ISessionContext context) : base(context)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Index(Guid userid, CancellationToken cancellation)
        {
            var client = await this.Context.SetOf<Client>()
                .Where(c => c.Logon.Id == userid)
                .Select(c => new {ApplicationId = c.Logon.Application.Id, c.Logon.UserName, UserId = c.Logon.Id, c.Id}).AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (client == null) return this.NavigationFor<UserSummaryController>().ToIndex();

            await this.LogEventAsync($"Opened user details: {userid}, {client.UserName}");

            var appId = client.ApplicationId;
            var userName = client.UserName;

            ViewData["UserId"] = userid;
            ViewData["ApplicationId"] = appId;
            ViewData["UserName"] = userName;
            ViewData["UserDetailId"] = client.Id;
            ViewData["userDetailuri"] = this.Url.Action("Read", "UserDetail", new {Area = "Clients", userid});

            return View();
        }

        /// <summary>
        /// Provides the ability to use the source lead id to access user information.
        /// Bounces the user back to the user screen by looking up the matching user id.
        /// </summary>
        public virtual async Task<ActionResult> FromLead(Int32 leadId, CancellationToken cancellation)
        {
            var client = await this.Context.SetOf<Client>()
                .Where(c => c.SourceLead.Id == leadId)
                .Select(c => (Guid?)c.Logon.Id)
                .FirstOrDefaultAsync(cancellation);

            if (client == null) return this.NavigationFor<UserSummaryController>().ToIndex();

            return this.RedirectToAction("Index", new {userId = client});
        }

        [HandleErrorWithAjaxFilter()]
        public virtual ActionResult Read(Guid userid)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var client = this.Context.SetOf<Client>()
                    .Where(c => c.Logon.Id == userid)
                    .Include(c => c.Logon)
                    .Include(c => c.Logon.Application).FirstOrDefault();

                if (client == null) return this.Json(new { sucess = false, messaage = "Client does not exist" });

                var nationBuilders =
                    this.Context.SetOf<Integration.NationBuilder.Data.Registration>()
                        .Where(r => r.Owner.UserId == userid)
                        .Select(r => new { r.NationName, r.LatestAccessToken })
                        .ToList();

                var access = this.Context.SetOf<Access>().First(a => a.Id == userid);
                var xmlUser = access.IsXmlUser();
                var batchUser = access.IsBatchUser();

                return this.Json(
                    new
                    {
                        UserId = client.Logon.Id,
                        ApplicationId = client.Logon.Application.Id,
                        client.Logon.UserName,
                        LastActivityDate = client.Logon.LastActivityDate.ToUserLocal().ToStandardDisplay(),
                        
                        client.BusinessName,
                        client.FirstName,
                        client.LastName,

                        Address = client.Address?.Address ?? String.Empty,
                        City = client.Address?.City ?? String.Empty,
                        State = client.Address?.State?.ToUpper() ?? String.Empty,
                        Zip = client.Address?.Zip ?? String.Empty,

                        Email = client.DefaultEmail,
                        Phone = client.PrimaryPhone?.ToString() ?? String.Empty,
                        Fax = client.Fax?.ToString() ?? String.Empty,
                        DateAdded = client.Logon.CreatedDate.ToUserLocal().ToStandardDisplay(),

                        client.Logon.IsLockedOut,
                        client.Logon.IsApproved,
                        client.CompositeName,
                        client.DefaultProduct,
                        client.DefaultColumnMap,

                        SiteName = client.Logon.Application.Details?.Title ?? String.Empty,
                        StoreData = client.AllowDataRetention,
                        xmlUser,
                        batchUser,
                        SourceLeadId = client.SourceLead.Id,
                        client.SourceLead.OwnerId,
                        client.SourceLead.PublicKey,
                        NationName = (nationBuilders.Any()
                                ? String.Empty
                                : String.Join(", ", nationBuilders.Select(n => n.NationName).ToArray())),
                        AccessToken = (nationBuilders.Any()
                                ? String.Empty
                                : String.Join(", ", nationBuilders.Select(n => n.LatestAccessToken).ToArray())),
                        LeadDetail = this.Url.BuildFor<LeadDetailController>().ToDetail(client.SourceLead.Id.Value),
                        Links = new
                        {
                            Detail = this.Url.BuildFor<UserDetailController>().ToDetail(client.Logon.Id),
                            Edit = this.Url.Action("Index", "EditUser", new { Area = "Clients"}),
                            Files = this.Url.Action("Index", "UserFiles", new { Area = "Clients", email = client.DefaultEmail}),
                            Cards = this.Url.Action("Index", "ViewCreditCards", new { Area = "Billing", userid}),
                            AdminFiles = this.Url.BuildFor<AdminFileController>().Summary(userid),
                            AdminFileDelete = this.Url.BuildFor<AdminFileController>().Delete(),
                            AdminFileAddNote = this.Url.BuildFor<AdminFileController>().AddNote(),
                            AdminFileDownload = this.Url.BuildFor<AdminFileController>().Download(),
                            AdminFileUpload = this.Url.BuildFor<AdminFileController>().Save(userid),
                            UserNotes = this.Url.Action("Read", "UserNotes", new { Area = "Clients", userid }),
                            AddUserNote = this.Url.Action("Add", "UserNotes", new { Area = "Clients" }),
                            Jobs = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", email = client.DefaultEmail }),
                            JobsNew = this.Url.Action("UploadFile", "Batch", new { Area = "", userid }),
                            LogInAsUser = this.Url.BuildFor<LogInAsClientController>().LoginAsClient(userid),
                            AutoProcessorRules = this.Url.Action("Index", "Summary", new { Area = "SmtpRules", userid }),
                            Contacts = this.Url.Action("Index", "EditContact", new { Area = "Clients", userid }),
                            UserMustChangePassword = this.Url.Action("Index", "UserMustChangePassword", new { Area = "SecurityManagement", userid }),
                            ServiceAccounts = this.Url.Action("Index", "ViewAccounts", new { Area = "Clients", userid }),
                            Deals = this.Url.Action("Index", "DealSummary", new { Area = "Sales", email = client.DefaultEmail, applicationId = client.Logon.Application.Id }),
                            NewDeal = this.Url.Action("Create", "NewDeal", new { Area = "Sales", userid }),
                            Charges = this.Url.Action("Index", "ChargeEventSummary", new { Area = "Sales", email = client.DefaultEmail }),
                            Messages = this.Url.Action("Index", "Message", new { Area = "Operations", email = client.DefaultEmail }),
                            RateCards = this.Url.Action("Index", "Pricing", new { Area = "Sales", userid }),
                            APIReporting = this.Url.Action("Index", "ApiUsage", new { Area = "Clients", id = userid }),
                            DownloadUsage = this.Url.Action("Index", "DownloadUsage", new { Area = "Sales", userid }),
                            SaveUsage = this.Url.Action("SaveUsageToUserFiles", "DownloadUsage", new { Area = "Sales", userid }),
                            LeadDetail = this.Url.Action("View", "LeadDetail", new { Area = "Clients" }),
                            ViewNations = this.Url.Action("Query", "ViewNations", new { Area = "Clients", userid }),
                            SourceLead = this.Url.Action("View", "LeadDetail", new { Area = "Clients", leadId = client.SourceLead.Id }),
                            PaymentUpdateLink = client.Logon.Application.Id == WellKnownIdentifiers.AccurateAppendId
                                ? "https://clients.accurateappend.com/Profile/Card"
                                : "https://clients.accurateappend.com/Profile/Card",
                            UserOperatingMetric = this.Url.Action("Read", "UserOperatingMetric", new { Area = "Reporting", userid }),
                            UserProductUsageMetric = this.Url.Action("OverviewReport", "OperationMetrics", new { Area = "Reporting", userid, applicationId = client.Logon.Application.Id }),
                            TicketSummary = this.Url.Action("GetTicketsForUser", "TicketsApi", new { Area = "Tickets", userid }),
                            CreateTicket = this.Url.Action("Index", "CreateTicket", new { Area = "Tickets", userid }),
                            ChangeOwner = this.Url.Action("Lead2", "ChangeOwner", new { Area = "Clients" })
                        }
                    }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}