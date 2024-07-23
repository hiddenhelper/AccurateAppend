using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for displaying API Trial metrics information.
    /// </summary>
    [Authorize()]
    public class ApiTrialMetricsController : Controller
    {
        #region Fields

        private readonly IApiTrailMetricsQuery query;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTrialMetricsController"/> class.
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiTrialMetricsController(IApiTrailMetricsQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            this.query = query;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns trail API usage counts grouped by Method
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessId">Trial key used by customer to access API</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [OutputCache(Duration = 5 * 60, VaryByParam = "accessId")]
        public virtual async Task<ActionResult> MethodCallCounts([DataSourceRequest] DataSourceRequest request, Guid accessId, CancellationToken cancellation)
        {
            var metrics = await this.query.QueryMethodCallsCounts(accessId, cancellation);
            var data = metrics.ToDataSourceResult(request);
            data.Total = metrics.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = data
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Returns trail API usage counts grouped by Method
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accessId">Trial key used by customer to access API</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [OutputCache(Duration = 5 * 60, VaryByParam = "accessId")]
        public virtual async Task<ActionResult> OperationMatchCounts([DataSourceRequest] DataSourceRequest request, Guid accessId, CancellationToken cancellation)
        {
            var metrics = await this.query.QueryOperationCounts(accessId, cancellation);
            var data = metrics.ToDataSourceResult(request);
            data.Total = metrics.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = data
            };
            return jsonNetResult;
        }
       
        #endregion
    }
}