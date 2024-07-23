using AccurateAppend.Core;
using DomainModel.ActionResults;
using DomainModel.Queries;
using DomainModel.ReadModel;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    [Authorize()]
    public class LeadReportsController : Controller
    {
        #region Fields

        private readonly ILeadReportingQuery reporting;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadReportsController"/> class.
        /// </summary>
        /// <param name="reporting">The <see cref="ILeadReportingQuery"/> component.</param>
        public LeadReportsController(ILeadReportingQuery reporting)
        {
            if (reporting == null) throw new ArgumentNullException(nameof(reporting));
            Contract.EndContractBlock();

            this.reporting = reporting;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Used to populate chart on Leads/Index
        ///  </summary>
        [OutputCache(Duration = 30 * 60, VaryByParam = "applicationid")]
        public virtual async Task<ActionResult> LeadMetricSummary(Guid applicationid, CancellationToken cancellation)
        {
            var enddate = DateTime.UtcNow.ToEndOfDay();
            var startdate = new DateTime(DateTime.Now.AddMonths(-2).Year, DateTime.Now.AddMonths(-2).Month, 1);

            var leadMetrics = await this.reporting.CalculateLeadMetrics(startdate, enddate, applicationid, cancellation);
            var data = (from a in leadMetrics
                        select new LeadMetric
                            {
                                Date = a.Date,
                                Total = a.Total,
                                Qualified = a.Qualified,
                                Converted = a.Converted
                            }).ToArray();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = new { Data = data }
            };
            return jsonNetResult;
        }

        #endregion
    }
}