using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainModel.ActionResults;
using DomainModel.Queries;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ApiUsage
{
    /// <summary>
    /// Controller for handling realtime API customer usage information.
    /// </summary>
    [Authorize()]
    public class ApiUsageController : Controller
    {
        private readonly IApiReportMetrics dal;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUsageController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IApiReportMetrics"/> to use for this controller instance.</param>
        public ApiUsageController(IApiReportMetrics dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        public virtual ActionResult Index(Guid id)
        {
            return this.View(id);
        }

        public virtual async Task<ActionResult> Query(Guid id, DateTime startDate, DateTime endDate, CancellationToken cancellation)
        {
            startDate = startDate.FromUserLocal();
            endDate = endDate.FromUserLocal();

            var data = await this.dal.ServiceCountsByUser(cancellation, id, startDate, endDate);

            return new JsonNetResult() {Data = data};
        }
    }
}