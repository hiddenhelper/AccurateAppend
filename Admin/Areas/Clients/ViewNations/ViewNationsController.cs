using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainModel.ActionResults;
using DomainModel.ReadModel;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ViewNations
{
    /// <summary>
    /// Controller to view NationBuilder data.
    /// </summary>
    [Authorize()]
    public class ViewNationsController : ContextBoundController
    {

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNationsController" /> class.
        /// </summary>
        /// <param name="context">The <see cref="T:AccurateAppend.Data.ISessionContext" /> to use for this controller instance.</param>
        public ViewNationsController(ISessionContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns Nation Builder registrations for a specific user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Query(Guid userid, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var query = this.Context.SetOf<NationBuilderRegistration>()
                        .Where(n => n.UserId == userid)
                        .Select(r => new {r.Slug, r.AccessToken})
                        .OrderBy(n => n.Slug);
                
                var data = (await query.ToArrayAsync(cancellation));

                return new JsonNetResult
                {
                    Data = data
                };
            }
        }
    }
}