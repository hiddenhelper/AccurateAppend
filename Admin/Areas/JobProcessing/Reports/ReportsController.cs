using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core.Definitions;
using DomainModel.ActionResults;
using DomainModel.Queries;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Reports
{
    [Authorize()]
    public class ReportsController : Controller
    {
        #region Fields

        private readonly IBatchUsageQuery batchUsageReport;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController"/> class.
        /// </summary>
        /// <param name="batchUsageReport">The <see cref="IBatchUsageQuery"/> component.</param>
        public ReportsController(IBatchUsageQuery batchUsageReport)
        {
            if (batchUsageReport == null) throw new ArgumentNullException(nameof(batchUsageReport));
            Contract.EndContractBlock();

            this.batchUsageReport = batchUsageReport;
        }

        #endregion

        #region Actions

        [OutputCache(Duration = 60 * 1, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetAvailableOperationsForJob(Int32 jobid, CancellationToken cancellation)
        {
            var data = await this.batchUsageReport.AvailableOperationsForJob(jobid, cancellation);
            var result = new JsonNetResult
            {
                Data = new {Data = data.OrderBy(d => d.ToString())}
            };
            return result;
        }

        [OutputCache(Duration = 60 * 1, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetMatchLevelReportForJob(Int32 jobid, DataServiceOperation operationName, CancellationToken cancellation)
        {
            var data = await this.batchUsageReport.MatchLevelReportForJob(jobid, operationName, cancellation);
            var result = new JsonNetResult
            {
                Data = new
                    {
                        Data = data
                            .Select(m => new {MatchLevel = m.Metric, m.File, m.User, m.System})
                            .OrderBy(d => d.MatchLevel)
                    }
            };
            return result;
        }

        [OutputCache(Duration = 60 * 1, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetMaxValidationLevelReportForJob(Int32 jobid, DataServiceOperation operationName, CancellationToken cancellation)
        {
            var data = await this.batchUsageReport.MatchLevelReportForJob(jobid, operationName, cancellation);
            var result = new JsonNetResult
            {
                Data = new
                {
                    Data = data
                            .Select(m => new { MatchLevel = m.Metric, m.File, m.User, m.System })
                            .OrderBy(d => d.MatchLevel)
                }
            };
            return result;
        }

        [OutputCache(Duration = 60 * 1, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetQualityLevelReportForJob(Int32 jobid, DataServiceOperation operationName, CancellationToken cancellation)
        {
            var data = await this.batchUsageReport.QualityLevelReportForJob(jobid, operationName, cancellation);
            var result = new JsonNetResult
            {
                Data = new
                {
                    Data = data
                            .Select(m => new { QualityLevel = m.Metric, m.File, m.User, m.System })
                            .OrderBy(d => d.QualityLevel.ToString())
                }
            };
            return result;
        }

        [OutputCache(Duration = 60 * 1, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetCassReportForJob(Int32 jobid, CancellationToken cancellation)
        {
            var data = (await this.batchUsageReport.CassReportForJob(jobid, cancellation))
                .SelectMany(m => new[]
                {
                    new {m.Metric, Type = "File", Rate = m.File},
                    new {m.Metric, Type = "User", Rate = m.User},
                    new {m.Metric, Type = "System", Rate = m.System}
                }).ToArray();

            var groupedData = data.GroupBy(d => d.Type);
            var final = groupedData.Select(g => new
            {
                Type = g.Key,
                F = g.Where(gg => gg.Metric == CassStatus.F).Sum(gg => gg.Rate),
                P = g.Where(gg => gg.Metric == CassStatus.P).Sum(gg => gg.Rate),
                S = g.Where(gg => gg.Metric == CassStatus.S).Sum(gg => gg.Rate),
                U = g.Where(gg => gg.Metric == CassStatus.U).Sum(gg => gg.Rate),
                C = g.Where(gg => gg.Metric == CassStatus.C).Sum(gg => gg.Rate)
            });

            var result = new JsonNetResult
            {
                Data = new
                {
                    Data = final
                }
            };
            return result;
        }

        #endregion
    }
}
