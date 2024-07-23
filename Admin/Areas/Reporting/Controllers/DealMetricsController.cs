using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Controller for supplying <see cref="T:AccurateAppend.Accounting.Deal" /> revenue metric reports.
    /// </summary>
    [Authorize()]
    public class DealMetricsController : Controller
    {
        #region Fields

        private readonly IDealMetricQuery query1;
        private readonly IUserDealMetricQuery query2;

        #endregion

        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <seealso cref="T:AccurateAppend.Websites.Admin.Areas.Reporting.Controllers.DealMetricsController" /> class.
        /// </summary>
        /// <param name="query1">The <seealso cref="T:DomainModel.Queries.IDealMetricQuery" /> to use for data access.</param>
        /// <param name="query2">The <seealso cref="T:DomainModel.Queries.IUserDealMetricQuery" /> to use for data access.</param>
        public DealMetricsController(IDealMetricQuery query1, IUserDealMetricQuery query2)
        {
            if (query1 == null) throw new ArgumentNullException(nameof(query1));
            if (query2 == null) throw new ArgumentNullException(nameof(query2));
            Contract.EndContractBlock();

            this.query1 = query1;
            this.query2 = query2;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Returns data aggregated by performance metrics in Json format
        /// </summary>
        [OutputCache(Duration = 5 * 60, VaryByParam = "applicationId")]
        public async Task<ActionResult> RecentDeals([DataSourceRequest] DataSourceRequest request, Guid applicationId, CancellationToken cancellation)
        {
            var data = (await this.query1.Query(applicationId, cancellation)).ToArray();
            var result = data.ToDataSourceResult(request);

            result.Total = data.Count();
            var jsonNetResult = new JsonNetResult
            {
                Data = result
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Returns data aggregated by performance metrics in Json format
        /// </summary>
        [OutputCache(Duration = 5 * 60, VaryByParam = "applicationId")]
        public async Task<ActionResult> ClientDeals([DataSourceRequest] DataSourceRequest request, Guid applicationId, CancellationToken cancellation)
        {
            var data = (await this.query2.Query(applicationId, cancellation)).ToArray();
            var result = data.ToDataSourceResult(request);

            result.Total = data.Count();
            var jsonNetResult = new JsonNetResult
            {
                Data = result
            };
            return jsonNetResult;
        }

        #endregion

    }
}
