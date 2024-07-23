using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.ChangeOwner.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ChangeOwner
{
    /// <summary>
    /// Controller for changing lead ownership for AA users.
    /// </summary>
    /// <remarks>
    /// Since users are owned by their source lead, we can get away with a single real operation for change.
    /// We simply bounce the user from the action accepting UserId to the source lead. The lead is always kept in sync
    /// with the User so we can ride on that.
    /// </remarks>
    [Authorize()]
    public class ChangeOwnerController : Controller
    {
        private readonly AccurateAppend.Accounting.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;

        public ChangeOwnerController(AccurateAppend.Accounting.DataAccess.DefaultContext context, IMessageSession bus)
        {
            this.context = context;
            this.bus = bus;
        }

        public virtual async Task<ActionResult> Lead(Int32 leadId, CancellationToken cancellation)
        {
            if (!this.User.Identity.IsSuperUser()) return this.DisplayErrorResult("You must be an admin to use this feature");

            var lead = await this.context
                .SetOf<Lead>()
                .Where(l => l.Id == leadId)
                .Select(l => new ChangeOwnerModel() {Id = l.PublicKey, OwnerId = l.OwnerId, FirstName = l.FirstName, LastName = l.LastName})
                .FirstOrDefaultAsync(cancellation);

            if (lead == null) return this.DisplayErrorResult($"Lead {leadId} does not exist");

            return this.View(lead);
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Lead(ChangeOwnerModel model, CancellationToken cancellation)
        {
            if (!this.User.Identity.IsSuperUser()) return this.DisplayErrorResult("You must be an admin to use this feature");

            var lead = await this.context
                .SetOf<Lead>()
                .Where(l => l.PublicKey == model.Id)
                .Include(l => l.Application)
                .FirstAsync(cancellation);

            var raiseChangedEvent = model.OwnerId != WellKnownIdentifiers.SystemUserId && lead.OwnerId != model.OwnerId;
            lead.ChangeOwner(model.OwnerId);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

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
                            UserId = lead.OwnerId,
                            UserName = email
                        },
                        PublicKey = lead.PublicKey,
                        ApplicationId = lead.Application.Id
                    };

                    await this.bus.Publish(@event);

                    transaction.Complete();
                }
            }

            this.ViewData["Message"] = "Updated";

            return this.View(model);
        }
        
        [HttpPost()]
        public virtual async Task<ActionResult> Lead2(ChangeOwnerModel model, CancellationToken cancellation)
        {
            if (!this.User.Identity.IsSuperUser())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("You must be an admin to use this feature");
            }

            var lead = await this.context
                .SetOf<Lead>()
                .Where(l => l.PublicKey == model.Id)
                .Include(l => l.Application)
                .FirstAsync(cancellation);

            var raiseChangedEvent = model.OwnerId != WellKnownIdentifiers.SystemUserId && lead.OwnerId != model.OwnerId;
            lead.ChangeOwner(model.OwnerId);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

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
                            UserId = lead.OwnerId,
                            UserName = email
                        },
                        PublicKey = lead.PublicKey,
                        ApplicationId = lead.Application.Id
                    };

                    await this.bus.Publish(@event);

                    transaction.Complete();
                }
            }

            return this.Json(new { Success = HttpStatusCode.Accepted, Message = "Account owner has been updated" });
        }

        public virtual async Task<ActionResult> ForUser(Guid userId, CancellationToken cancellation)
        {
            if (!this.User.Identity.IsSuperUser()) return this.DisplayErrorResult("You must be an admin to use this feature");

            var leadId = await this.context
                .Database
                .SqlQuery<Int32?>("SELECT [SourceLeadId] FROM [accounts].[UserDetail] WHERE [UserId]= @p0", userId)
                .FirstOrDefaultAsync(cancellation);

            if (leadId == null) return this.DisplayErrorResult($"User {userId} does not exist");

            return this.RedirectToAction(nameof(Lead), "ChangeOwner", new {leadId = leadId.Value});
        }
    }
}