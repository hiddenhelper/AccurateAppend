using AccurateAppend.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Admin.Areas.Clients.DeleteLead
{
    [Authorize()]
    public class DeleteLeadController : ActivityLoggingController2
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteLeadController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public DeleteLeadController(ISessionContext context)
        {
            this.context = context;
        }

        #endregion

        public async Task<ActionResult> Index(Int32 leadid, CancellationToken cancellation)
        {
            try
            {
                using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                {
                    if (await this.context.SetOf<Client>().Where(c => c.SourceLead.Id == leadid).AnyAsync(cancellation))
                    {
                        return new JsonNetResult {Data = true};
                    }

                    var lead = await this.context
                        .SetOf<Lead>()
                        .FirstOrDefaultAsync(l => l.Id == leadid && !l.IsDeleted.Value, cancellation);
                    if (lead == null) return new JsonNetResult {Data = true};

                    var user = await this.context.SetOf<User>().InteractiveUser().FirstAsync(cancellation);
                    var note = new Note("Lead deleted by user", user);

                    lead.MarkDeleted(note);

                    await uow.CommitAsync(cancellation);
                }

                this.OnEvent($"Deleted lead: {leadid}");
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                EventLogger.Logger.LogEvent(ex, Severity.Low, Application.AccurateAppend_Admin, description: $"Lead {leadid}");

                return new JsonNetResult
                {
                    Data = false
                };
            }
            
            var result = new JsonNetResult
            {
                Data = true
            };

            return result;
        }
    }
}