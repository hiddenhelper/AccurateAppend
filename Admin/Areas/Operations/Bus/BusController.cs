using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Bus
{
    [Authorize()]
    public class BusController : ContextBoundController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public BusController(ISessionContext context) : base(context)
        {
        }

        public ActionResult Failed()
        {
            return this.View();
        }

        public virtual async Task<ActionResult> Query(CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var final = await this.Context.SetOf<DomainModel.ReadModel.FailedBusMessage>()
                        .ToArrayAsync(cancellation);

                var data = final.Select(d =>
                    new
                    {
                        d.Id,
                        d.Queue,
                        d.Headers,
                        d.Body,
                        EventLog = this.Url.Action("Index", "EventLog", new {area = "Operations", d.CorrelationId})
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