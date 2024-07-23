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
    public class OperationMetricsController : Controller
    {
        #region Fields

        private readonly IOperationReportMetricQuery dal;

        #endregion

        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <seealso cref="T:AccurateAppend.Websites.Admin.Areas.Reporting.Controllers.OperationMetricsController" /> class.
        /// </summary>
        /// <param name="dal">The <seealso cref="T:DomainModel.Queries.IOperationReportMetricQuery" /> to use for data access.</param>
        public OperationMetricsController(IOperationReportMetricQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions
        
        /// <summary>
        /// Returns Operation usage metrics in Json format
        /// </summary>
        [OutputCache(Duration = 5*60, VaryByParam = "applicationId;userId")]
        public async Task<ActionResult> OverviewReport([DataSourceRequest] DataSourceRequest request, Guid applicationId, Guid? userId, CancellationToken cancellation)
        {
            var data = (await this.dal.Query(applicationId, userId, cancellation)).ToArray();
            var result = data.ToDataSourceResult(request);

            result.Total = data.Length;
            var jsonNetResult = new JsonNetResult
            {
                Data = result
            };
            return jsonNetResult;
        }

        #endregion
    }
}
