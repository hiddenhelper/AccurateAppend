using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Sales;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for supplying <see cref="DealBinder"/> revenue metric reports.
    /// </summary>
    [Authorize()]
    public class OperatingMetricsController : Controller
    {
        #region Fields

        private readonly IOperatingMetricQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="OperatingMetricsController"/> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="IOperatingMetricQuery"/> to use for data access.</param>
        public OperatingMetricsController(IOperatingMetricQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions
        
        /// <summary>
        /// Returns PPC data aggregated by performance metrics in Json format
        /// </summary>
        [OutputCache(Duration = 5*60, VaryByParam = "applicationId")]
        public async Task<ActionResult> OverviewReport([DataSourceRequest] DataSourceRequest request, Guid applicationId, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(applicationId, cancellation)).ToArray();
            var result = data.ToDataSourceResult(request);

            result.Total = data.Count();
            var jsonNetResult = new JsonNetResult
            {
                Data = result
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Returns revenue graph for an application
        /// </summary>
        [OutputCache(Duration = 60*60, VaryByParam = "applicationId")]
        public async Task<ActionResult> Revenue(Guid applicationid, CancellationToken cancellation)
        {
            var enddate = DateTime.Now.ToEndOfDay().Coerce();
            var startdate = DateTime.Now.AddMonths(-13).ToFirstOfMonth().Coerce();

            var data = (await this.dal.GenerateRevenueMetrics(applicationid, startdate.Date, enddate, cancellation))
                .OrderBy(a => a.Date)
                .ToList();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = new {Data = data}
            };
            return jsonNetResult;
        }
        
        #endregion
    }
}
