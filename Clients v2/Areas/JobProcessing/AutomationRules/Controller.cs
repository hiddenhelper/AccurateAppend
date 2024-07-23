using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest.Xml;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Areas.JobProcessing.AutomationRules
{
    /// <summary>
    /// Controller for getting and interacting with saved automation Manifest content.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public Controller(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Queries a list of automation rules for the current identity.
        /// </summary>
        public virtual async Task<ActionResult> ForCurrentUser(CancellationToken cancellation)
        {
            using (context.CreateScope(ScopeOptions.ReadOnly))
            {
                var userId = this.User.Identity.GetIdentifier();

                var rules = await context.SetOf<SmtpAutoprocessorRule>()
                    .Where(r => r.Logon.Id == userId)
                    .Include(r => r.ManifestToRun)
                    .OrderByDescending(r => r.IsDefault)
                    .ThenByDescending(r => r.CreatedDate)
                    .ToListAsync(cancellation);

                var data = rules.Select(r =>
                    new
                    {
                        UserId = userId,
                        Name = r.Terms,
                        Terms = r.Terms == "*" ? "Default" : r.Terms,
                        Description = r.Description,
                        ManifestId = r.ManifestToRun.Id,
                        LastUsed = DateTime.UtcNow.ToShortDateString(), // TODO: add last used data
                        Products = r.ManifestToRun.Manifest.Operations()
                            .Select(o =>
                                new
                                {
                                    Name = o.OperationName(),
                                    Desciption = o.OperationName().GetDescription()
                                })
                            .Where(o => !o.Name.IsPreference()).ToArray(),
                        Links = new
                        {
                            Detail = this.Url.Action("Detail", new { ManifestId = r.ManifestToRun.Id})
                        }

                    }).ToArray();

                return new JsonNetResult { Data = new { Data = data, Total = data.Length } };
            }
        }

        /// <summary>
        /// Action to acquire the manifest content details of a specific definition.
        /// </summary>
        public virtual async Task<ActionResult> Detail(Guid manifestId, CancellationToken cancellation)
        {
            using (context.CreateScope(ScopeOptions.ReadOnly))
            {
                var userId = this.User.Identity.GetIdentifier();

                var rule = await context.SetOf<SmtpAutoprocessorRule>()
                    .Where(r => r.Logon.Id == userId)
                    .Where(r => r.ManifestToRun.Id == manifestId)
                    .FirstOrDefaultAsync(cancellation);
                if (rule == null) return new JsonNetResult();

                var manifest = rule.ManifestToRun.Manifest;
                manifest.ManifestId(manifestId);
                manifest.UserId(this.User.Identity.GetIdentifier());

                return new JsonNetResult {Data = new {ManifestId = rule.ManifestToRun.Id, Manifest = manifest.ToString()}};
            }
        }

        #endregion
    }
}