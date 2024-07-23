using System;
using System.Linq;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail.Models;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using NServiceBus;
using Application = AccurateAppend.Security.Application;
using LeadSource = AccurateAppend.Accounting.LeadSource;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail
{
    /// <summary>
    /// Controller performing detail operation of <see cref="Lead"/> entities.
    /// </summary>
    [Authorize()]
    public class LeadDetailController : ActivityLoggingController2
    {
        #region Fields

        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadDetailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public LeadDetailController(AccurateAppend.Accounting.DataAccess.DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Determines if a lead submitted in the last 30 days matches on email or phone
        /// </summary>
        public async Task<ActionResult> LeadExists(Guid applicationid, String email, String phone, Int32? existingLead, CancellationToken cancellation)
        {
            email = (email ?? String.Empty).Trim();
            phone = (phone ?? String.Empty).Trim();
            phone = PhoneNumber.CleanFormat(phone);

            if (!String.IsNullOrWhiteSpace(email) || !String.IsNullOrWhiteSpace(phone))
            {
                var limit = DateTime.UtcNow.AddDays(-30);

                try
                {
                    var query = this.context
                        .SetOf<Lead>()
                        .Where(l => l.Application.Id == applicationid && l.CreatedDate >= limit)
                        .Where(l => l.IsDeleted == null || l.IsDeleted == false)
                        .Where(l => l.Status != LeadStatus.ConvertedToCustomer);
                    if (existingLead != null) query = query.Where(l => l.Id.Value != existingLead);
                    if (!String.IsNullOrWhiteSpace(email)) query = query.Where(l => l.DefaultEmail == email);
                    if (!String.IsNullOrWhiteSpace(phone)) query = query.Where(l => l.PrimaryPhone.Value == phone);

                    var match = await query.OrderByDescending(l => l.CreatedDate)
                            .Select(l => new {Id = l.Id.Value, l.DefaultEmail, Phone = l.PrimaryPhone.Value})
                            .FirstOrDefaultAsync(cancellation);

                    if (match != null)
                    {
                        var detailUrl = this.Url.Action("View", "LeadDetail", new {Area = "Clients", leadId = match.Id});

                        if (String.Equals(email, match.DefaultEmail, StringComparison.OrdinalIgnoreCase))
                        {
                            return new JsonNetResult
                            {
                                Data = new
                                {
                                    HttpStatusCodeResult = (Int32) HttpStatusCode.OK,
                                    Message = $"An existing lead with the email {email.ToLower()} was found.",
                                    ExistingLeadId = match.Id,
                                    DetailUrl = detailUrl
                                }
                            };
                        }

                        if (String.Equals(phone, match.Phone, StringComparison.OrdinalIgnoreCase))
                        {
                            return new JsonNetResult
                            {
                                Data = new
                                {
                                    HttpStatusCodeResult = (Int32) HttpStatusCode.OK,
                                    Message = $"An existing lead with the phone number {phone.ToLower()} was found.",
                                    ExistingLeadId = match.Id,
                                    DetailUrl = detailUrl
                                }
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();

                    EventLogger.Logger.LogEvent(ex, Severity.Low);
                    return new JsonNetResult
                    {
                        Data = new
                            {
                                HttpStatusCodeResult = (Int32) HttpStatusCode.InternalServerError,
                                Message = "Unable to retrieve leads",
                                ExistingLeadId = 0
                            }
                    };
                }
            }

            return new JsonNetResult
            {
                Data = new
                {
                    HttpStatusCodeResult = (Int32)HttpStatusCode.NotFound,
                    Message = "No existing lead found",
                    ExistingLeadId = 0
                }
            };
        }
       
        public ActionResult Create(CancellationToken cancellation)
        {
            var lead = new LeadViewModel()
            {
                ApplicationId = WellKnownIdentifiers.AccurateAppendId,
                PublicKey = Guid.NewGuid(),
                Qualified = LeadQualification.UnknownAtThisTime,
                LeadSource = LeadSource.Unknown,
                FollowUpDate = DateTime.UtcNow.AddDays(3),
                DateAdded = DateTime.UtcNow,
                OwnerId = WellKnownIdentifiers.SystemUserId,
                Score = LeadScore.Unknown
            };
            ViewData["view"] = "create";

            return this.View("Detail", lead);
        }

        public async Task<ActionResult> View(Int32 leadid, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var q = this.context.SetOf<Lead>().Where(l => l.Id == leadid)
                    .Select(l => new LeadViewModel()
                    {
                        LeadId = l.Id.Value,
                        PublicKey = l.PublicKey,
                        ApplicationId = l.Application.Id,
                        OwnerId = l.OwnerId,

                        Address = l.Address.Address,
                        City = l.Address.City,
                        State = l.Address.State,
                        Zip = l.Address.Zip,

                        Phone = l.PrimaryPhone.Value,
                        Email = l.DefaultEmail,

                        DateAdded = l.CreatedDate,
                        FollowUpDate = l.FollowUpDate,

                        BusinessName = l.BusinessName,
                        FirstName = l.FirstName,
                        LastName = l.LastName,

                        ContactMethod = l.ContactMethod,
                        Comments = l.Comments,
                        DisqualificationReason = l.DisqualificationReason,
                        DoNotMarketTo = l.DoNotMarketTo,
                        IP = l.IP,
                        LeadSource = l.Source,
                        ProductInterest = l.ProductInterest,
                        Qualified = l.Qualified,
                        
                        Status = l.Status,
                        Website = l.Website,
                        Score = l.Score,

                        LandingPageDomain = "",
                        LandingPageUrl = "",

                        CrmLink = l.ExternalId,

                        TrialId = l.Trial.Id ?? 0
                    });
                
                var model = await q.FirstOrDefaultAsync(cancellation);
                if (model == null) return this.NavigationFor<LeadSummaryController>().ToIndex();

                //var externalId = await this.context.Database
                //    .SqlQuery<String>("SELECT ExternalId FROM accounts.Leads WHERE LeadId = @p0", leadid)
                //    .FirstOrDefaultAsync(cancellation);
                var externalId = model.CrmLink;
                if (externalId != null)
                {
                    model.CrmLink = $"https://app.futuresimple.com/leads/{externalId}";

                    if (!Debugger.IsAttached) return this.Redirect(model.CrmLink);
                }

                model.FollowUpDate = model.FollowUpDate?.ToUserLocal();
                model.DateAdded = model.DateAdded.ToUserLocal();

                this.OnEvent($"Lead {leadid} viewed");
                
                return this.View("Detail", model);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Edit(LeadViewModel lead, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewData["error"] = true;
                return this.View("Detail", lead);
            }

            var newLead = lead.LeadId == 0;

            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                Lead entity;
                if (lead.LeadId == 0)
                {
                    var application = await this.context.SetOf<Application>().SingleAsync(a => a.Id == lead.ApplicationId, cancellation);
                    entity = new Lead(application);
                    this.context.SetOf<Lead>().Add(entity);
                }
                else
                {
                    entity = await this.context.SetOf<Lead>().FirstAsync(l => l.Id == lead.LeadId, cancellation);
                }

                if (entity == null) return this.NavigationFor<LeadSummaryController>().ToIndex();

                entity.Address.Address = lead.Address;
                entity.Address.City = lead.City;
                entity.Address.State = lead.State;
                entity.Address.Zip = lead.Zip;

                entity.BusinessName = lead.BusinessName;
                entity.FirstName = lead.FirstName;
                entity.LastName = lead.LastName;

                entity.PrimaryPhone.Value = lead.Phone;
                entity.DefaultEmail = lead.Email;

                entity.ContactMethod = lead.ContactMethod;
                entity.Comments = lead.Comments;
                if (entity.Status != LeadStatus.ConvertedToCustomer) entity.DisqualificationReason = lead.DisqualificationReason;
                entity.DoNotMarketTo = lead.DoNotMarketTo;

                entity.IP = lead.IP;
                entity.Source = lead.LeadSource;
                entity.ProductInterest = lead.ProductInterest;
                if (entity.Status != LeadStatus.ConvertedToCustomer) entity.Qualified = lead.Qualified;
                if (entity.Status != LeadStatus.ConvertedToCustomer) entity.Status = lead.Status;
                entity.Website = lead.Website;
                entity.Score = lead.Score;

                var raiseChangedEvent = lead.OwnerId != WellKnownIdentifiers.SystemUserId && lead.OwnerId != entity.OwnerId;
                if (lead.OwnerId != entity.OwnerId)
                {
                    entity.ChangeOwner(lead.OwnerId.Value);

                    var user = await this.context.CurrentUserAsync(cancellation);
                    var userName = await this.context
                        .SetOf<User>()
                        .Where(u => u.Id == entity.OwnerId)
                        .Select(u => u.UserName)
                        .FirstAsync(cancellation);
                    entity.Notes.Add(user, $"Changed owner : {userName}");
                }

                #region Commit

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await uow.CommitAsync(cancellation);

                    lead.LeadId = entity.Id.Value;

                    if (raiseChangedEvent)
                    {
                        var email = await this.context
                            .SetOf<User>()
                            .Where(u => u.Id == lead.OwnerId)
                            .Select(u => u.UserName)
                            .FirstAsync(cancellation);

                        var @event = new LeadAssignedEvent
                        {
                            AssignedTo =
                            {
                                UserId = entity.OwnerId,
                                UserName = email
                            },
                            PublicKey = entity.PublicKey,
                            ApplicationId = lead.ApplicationId
                        };

                        await this.bus.Publish(@event);
                    }

                    transaction.Complete();
                }

                #endregion

            }

            if (newLead)
            {
                this.OnEvent($"Lead {lead.LeadId} created");
            }
            else
            {
                this.OnEvent($"Lead {lead.LeadId} edited");
            }

            if (lead.Status == LeadStatus.NoFurtherAction)
            {
                return this.NavigationFor<LeadSummaryController>().ToIndex();
            }

            return this.RedirectToAction("View", new { leadid = lead.LeadId });
        }

        /// <summary>
        /// Provides the ability to use the alternate public key to access lead information.
        /// Bounces the user back to the lead screen by looking up the matching lead id.
        /// </summary>
        public async Task<ActionResult> PublicKey(Guid id, CancellationToken cancellation)
        {
            var leadId = await this.context
                .SetOf<Lead>()
                .Where(l => l.PublicKey == id)
                .Select(l => l.Id)
                .FirstOrDefaultAsync(cancellation);

            return this.RedirectToAction("View", "LeadDetail", new {leadId});
        }

        public async Task<ActionResult> Read(Int32 leadid, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var data = await this.context.SetOf<Lead>().Where(l => l.Id == leadid)
                    .Select(l => new LeadViewModel()
                    {
                        LeadId = l.Id.Value,
                        PublicKey = l.PublicKey,
                        ApplicationId = l.Application.Id,
                        OwnerId = l.OwnerId,
                        Address = l.Address.Address,
                        City = l.Address.City,
                        State = l.Address.State,
                        Zip = l.Address.Zip,
                        Phone = l.PrimaryPhone.Value,
                        Email = l.DefaultEmail,
                        DateAdded = l.CreatedDate,
                        FollowUpDate = l.FollowUpDate,
                        BusinessName = l.BusinessName,
                        FirstName = l.FirstName,
                        LastName = l.LastName,
                        ContactMethod = l.ContactMethod,
                        Comments = l.Comments,
                        DisqualificationReason = l.DisqualificationReason,
                        DoNotMarketTo = l.DoNotMarketTo,
                        IP = l.IP,
                        LeadSource = l.Source,
                        ProductInterest = l.ProductInterest,
                        Qualified = l.Qualified,
                        Status = l.Status,
                        Website = l.Website,
                        Score = l.Score,
                        TrialId = l.Trial.Id ?? 0
                    }).ToArrayAsync(cancellation);
                
                var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
                {
                    Data = new
                    {
                        Data = data
                    }
                };

                return jsonNetResult;
            }
        }

        #endregion
    }
}