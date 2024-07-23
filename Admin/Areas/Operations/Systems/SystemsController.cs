using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Systems
{
    [Authorize()]
    public class SystemsController : ContextBoundController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemsController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public SystemsController(ISessionContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            return this.View();
        }

        public virtual async Task<ActionResult> Query(CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var final = (await this.Context.SetOf<DomainModel.ReadModel.DeployedSystem>()
                        .OrderBy(r => r.SystemName)
                        .ThenBy(r => r.Host)
                        .ToArrayAsync(cancellation));

                var data = final
                    .Select(r => new
                    {
                        r.Id,
                        r.SystemName,
                        r.UserName,
                        r.Host,
                        Heartbeat = r.Heartbeat.ToUserLocal(),
                        r.Version
                    });

                var jsonNetResult = new JsonNetResult
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }
    }
}