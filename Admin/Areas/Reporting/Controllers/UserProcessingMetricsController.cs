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
    /// <summary>
    /// Controller for supplying Operation metric reports.
    /// </summary>
    [Authorize()]
    public class UserProcessingMetricsController : Controller
    {
        #region Fields

        private readonly IUserProcessingMetricQuery dal;

        #endregion

        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <seealso cref="T:AccurateAppend.Websites.Admin.Areas.Reporting.Controllers.OperationMetricsController" /> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="T:DomainModel.Queries.IOperationReportMetricQuery" /> to use for data access.</param>
        public UserProcessingMetricsController(IUserProcessingMetricQuery dal)
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
        [OutputCache(Duration = 5*60, VaryByParam = "applicationId;source")]
        public async Task<ActionResult> OverviewReport([DataSourceRequest] DataSourceRequest request, Guid applicationId, Source source, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(applicationId, source, cancellation)).ToArray();
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
