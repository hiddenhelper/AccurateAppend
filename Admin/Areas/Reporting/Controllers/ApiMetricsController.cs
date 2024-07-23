using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainModel.ActionResults;
using DomainModel.Queries;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    [Authorize()]
    public class ApiMetricsController : Controller
    {
        #region Fields

        private readonly IApiReportMetrics reports;

        #endregion
        
        #region Constructor

        public ApiMetricsController(IApiReportMetrics reports)
        {
            if (reports == null) throw new ArgumentNullException(nameof(reports));
            Contract.EndContractBlock();

            this.reports = reports;
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 5 * 60, VaryByParam = "host;startDate;endDate;userId")]
        public virtual async Task<ActionResult> WebServiceStatistics(String host, DateTime? startDate, DateTime? endDate, Guid? userId, CancellationToken cancellation)
        {
            var data = await this.reports.GetExecutionStatistics(cancellation, host, startDate, endDate, userId);

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = new { Data = data.OrderByDescending(a => a.TransactionDate) }
            };
            return jsonNetResult;
        }

        [OutputCache(Duration = 5 * 60, VaryByParam = "host;startDate;endDate;userId")]
        public virtual async Task<ActionResult> WebServiceByResponseTime(String host, DateTime? startDate, DateTime? endDate, Guid? userId, CancellationToken cancellation)
        {
            var data = await this.reports.GetExecutionTimes(cancellation, host, startDate, endDate, userId);

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = data.OrderByDescending(a => a.Seconds)
            };
            return jsonNetResult;
        }

        [OutputCache(Duration = 5 * 60, VaryByParam = "host;startDate;endDate;userId")]
        public virtual async Task<ActionResult> WebServiceByOperation(String host, DateTime? startDate, DateTime? endDate, Guid? userId, CancellationToken cancellation)
        {
            var data = await this.reports.GetOperationCounts(cancellation, host, startDate, endDate, userId);

            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = data.OrderByDescending(a => a.Calls) 
            };
            return jsonNetResult;
        }

        [OutputCache(Duration = 5 * 60, VaryByParam = "none")]
        public virtual async Task<ActionResult> GetCallsByUserJson(CancellationToken cancellation)
        {
            var data = await this.reports.TotalCallsByUser(cancellation);
            var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            {
                Data = new {Data = data.Select(u => new {Email = u.UserName, u.UserId, Count = u.Calls})}
            };

            //var enddate = DateTime.UtcNow.ToEndOfDay();
            //var startdate = enddate.AddDays(-1).ToStartOfDay();
            //var rob = new Guid("43056364-2161-448E-8BD2-EE1FCEBD3492");

            //using (var db = new UnifiedDataContext(Config.AccurateAppendDb))
            //{
            //    var transactionQuery = db.Set<SoapCallsDailyUsageRollup>()
            //        .Where(t => t.Date >= startdate && t.Date <= enddate && t.UserId != rob)
            //        .GroupBy(t => t.UserId)
            //        .Select(g => new {UserId = g.Key, Count = g.Sum(c => c.Count)});

            //    var usersQuery = db.SetOf<User>().Where(u => u.LastActivityDate >= startdate);

            //    var resultsQuery = transactionQuery.Join(usersQuery, t => t.UserId, u => u.Id,
            //        (t, u) => new {Email = u.UserName, UserId = u.Id, t.Count});
            //    var data = await resultsQuery.OrderByDescending(a => a.Count).ToArrayAsync(cancellation);

            //    var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
            //    {
            //        Data = new {Data = data}
            //    };

                return jsonNetResult;
            //}
        }

        #endregion
    }
}