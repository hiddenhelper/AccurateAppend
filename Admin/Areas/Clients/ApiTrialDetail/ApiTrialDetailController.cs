using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ApiTrialDetail
{
    /// <summary>
    /// Controller for displaying API Trial detail information.
    /// </summary>
    [Authorize()]
    public class ApiTrialDetailController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTrialDetailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> used for data access.</param>
        public ApiTrialDetailController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display the trial details view.
        /// </summary>
        public ActionResult Index(Int32 id, CancellationToken cancellation)
        {
            return this.View(id);
        }

        /// <summary>
        /// Action to query the trial detail data.
        /// </summary>
        public async Task<ActionResult> Read(Int32 id, CancellationToken cancellation)
        {
            var lead =  await this.Context.SetOf<Lead>().Where(l => l.Trial.Id == id).Include(l => l.Trial).FirstOrDefaultAsync(cancellation);

            if (lead == null) return new JsonNetResult(DateTimeKind.Local);
            
            var data = new 
            {
                lead.Trial.Id,
                lead.Trial.IsEnabled,
                lead.Trial.AccessId,
                lead.Trial.MaximumCalls,
                lead.DefaultEmail,
                DateCreated = lead.Trial.DateCreated.ToUserLocal(),
                ExpirationDate = lead.Trial.DateCreated.AddDays(30).ToUserLocal(),
                Links = new
                {
                    Extend = Url.Action("Extend", "ApiTrialDetail", new {Area = "Clients", id}),
                    Disable = Url.Action("Disable", "ApiTrialDetail", new {Area = "Clients", id}),
                    MethodCallCounts = Url.Action("MethodCallCounts", "ApiTrialMetrics", new {Area = "Reporting", lead.Trial.AccessId}),
                    OperationMatchCounts = Url.Action("OperationMatchCounts", "ApiTrialMetrics", new {Area = "Reporting", lead.Trial.AccessId}),
                    SourceLead = Url.Action("View", "LeadDetail", new {Area = "Clients", leadid = lead.Id})
                }
            };

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };

            return jsonNetResult;

        }

        /// <summary>
        /// Extends an existing API trial.
        /// </summary>
        public virtual async Task<ActionResult> Extend(Int32 id, Int16 maximumCalls, CancellationToken cancellation)
        {
            var trial = await this.Context
                .SetOf<TrialIdentity>()
                .FirstOrDefaultAsync(l => l.Id == id, cancellation);

            if (trial == null) return this.Json(new {Data = new {Message = $"No trial found for {id}"}}, JsonRequestBehavior.AllowGet);

            using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                trial.Extend(maximumCalls);

                await uow.CommitAsync(cancellation);
            }
            
            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = new
                {
                    Message = $"Trial {trial.AccessId} extended until {trial.DateCreated.AddDays(30)} with {trial.MaximumCalls} calls."
                }
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Disables an existing API Trial
        /// </summary>
        public virtual async Task<ActionResult> Disable(Int32 id, CancellationToken cancellation)
        {
            var trial = await this.Context
                .SetOf<TrialIdentity>()
                .FirstOrDefaultAsync(l => l.Id == id, cancellation);

            if (trial == null) return this.Json(new {Data = new {Message = $"No trial found for {id}"}}, JsonRequestBehavior.AllowGet);

            using (var uow  = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                trial.Disable();

                await uow.CommitAsync(cancellation);
            }

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = new
                {
                    Message = $"Trial {trial.AccessId} disabled."
                }
            };

            return jsonNetResult;
        }

        #endregion
    }
}