using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.DataAccess;
using DomainModel.ActionResults;
using DomainModel.JsonNET;
using DomainModel.Queries;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Admin.Areas.Reporting.Controllers
{
    /// <summary>
    /// Controller for supplying <see cref="Job"/> processing metric reports.
    /// </summary>
    [Authorize()]
    public class JobMetricsController : Controller
    {
        #region Fields

        private readonly IMatchCountsMetricReport matchCountQuery;
        private readonly IJobQueueMetricsQuery queueQuery;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JobMetricsController"/> class.
        /// </summary>
        /// <param name="matchCountQuery">The <see cref="IMatchCountsMetricReport"/> to use for this controller instance.</param>
        /// <param name="queueQuery">The <see cref="IJobQueueMetricsQuery"/> to use for this controller instance.</param>
        public JobMetricsController(IMatchCountsMetricReport matchCountQuery, IJobQueueMetricsQuery queueQuery)
        {
            if (matchCountQuery == null) throw new ArgumentNullException(nameof(matchCountQuery));
            if (queueQuery == null) throw new ArgumentNullException(nameof(queueQuery));
            Contract.EndContractBlock();

            this.matchCountQuery = matchCountQuery;
            this.queueQuery = queueQuery;
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 5 * 60, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetGraphForLast24Hours(Guid applicationid, CancellationToken cancellation)
        {
            var data = await this.queueQuery.Query(applicationid, DateTime.Now.AddDays(-1), DateTime.Now, cancellation);

            var result = new JsonNetResult(DateTimeKind.Local);
            result.Data = new { Data = data };

            return result;
        }

        [OutputCache(Duration = 30 * 60, VaryByParam = "*")]
        public virtual async Task<ActionResult> SubscriberJobMatchCounts(DateGrain aggregate, DateTime startdate, DateTime enddate, Guid applicationId, Source source)
        {
            startdate = startdate.Date;
            enddate = enddate.Date;
            var sources = new List<String>();
            if (source.HasFlag(Source.Batch)) sources.Add(Source.Batch.ToString());
            if (source.HasFlag(Source.Api)) sources.Add(Source.Api.ToString());
            
            var query = await this.matchCountQuery.ExecuteAsync(new DateSpan(startdate, enddate), applicationId);
            query = query.Where(m => sources.Contains(m.Source)).ToList();

            List<MatchCountsMetric> data;

            switch (aggregate)
            {
                case DateGrain.Day:
                    data = query.GroupBy(d => d.Date).Select(g =>
                    {
                        var result = new MatchCountsMetric
                        {
                            Source = Source.All.ToString(),
                            Date = g.Key,
                            EmailAppend = g.Sum(i => i.EmailAppend),
                            OtherAppends = g.Sum(i => i.OtherAppends),
                            PremiumPhoneAppend = g.Sum(i => i.PremiumPhoneAppend),
                            StandardPhoneAppend = g.Sum(i => i.StandardPhoneAppend)
                        };

                        return result;
                    }).ToList();
                    break;
                case DateGrain.Month:
                    data = query.GroupBy(d => new {d.Date.Year, d.Date.Month}).Select(g =>
                    {
                        var result = new MatchCountsMetric
                        {
                            Source = Source.All.ToString(),
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            EmailAppend = g.Sum(i => i.EmailAppend),
                            OtherAppends = g.Sum(i => i.OtherAppends),
                            PremiumPhoneAppend = g.Sum(i => i.PremiumPhoneAppend),
                            StandardPhoneAppend = g.Sum(i => i.StandardPhoneAppend)
                        };

                        return result;
                    }).ToList();
                    break;
                default:
                    throw new NotSupportedException($"{aggregate} is not supported");
            }

            var jsonNetResult = new JsonNetResult
            {
                Data = new {Data = data}
            };

            return jsonNetResult;
        }

        [OutputCache(Duration = 60 * 60, VaryByParam = "*")]
        public virtual async Task<ActionResult> SubscriberJobMatchCountsComparison(DateGrain compare, Guid applicationId)
        {
            var startdate = DateTime.Now.ToFirstOfMonth().AddMonths(-2);
            var enddate = DateTime.Now.AddDays(1);

            var current = DateTime.UtcNow.ToFirstOfMonth();
            var previous = current.AddMonths(-1);

            // create Day dimmension
            var currentNumberOfDays = DateTime.DaysInMonth(current.Year, current.Month);
            var previousNumberOfDays = DateTime.DaysInMonth(previous.Year, previous.Month);
            var numberOfDays = Math.Max(currentNumberOfDays, previousNumberOfDays);

            var dayDimmension = new List<int>();
            for (var i = 1; i < numberOfDays; i++)
            {
                dayDimmension.Add(i);
            }

            //            const String Sql = @"
            //DECLARE @Table table ([Date] date, [Source] varchar(50), EmailAppend int, PremiumPhoneAppend int, StandardPhoneAppend int, OtherAppends int)

            //INSERT INTO @Table
            //execute jobs.CalculateSubscriberReportMetrics @ApplicationId=@p0, @FromDate=@p1, @ToDate=@p2
            //SELECT * FROM @Table";
            //            var db = ((DbContext)this.matchCountQuery).Database;
            //            var query = db.SqlQuery<MatchCountsMetric>(Sql, applicationId, startdate, enddate);

            var query = await this.matchCountQuery.ExecuteAsync(new DateSpan(startdate, enddate), applicationId);
            var data = query.ToList();

            var modelCurrent =
                from a in data
                where a.Date.Between(current, current.AddMonths(1))
                group a by a.Date.Day
                    into g
                    select new { DayOfMonth = g.Key, Total = g.Sum(a => a.Count) };

            var modelPrevious =
                from a in data
                where a.Date.Between(previous, previous.AddMonths(1))
                group a by a.Date.Day
                    into g
                    select new { DayOfMonth = g.Key, Total = g.Sum(a => a.Count) };

            var acitvityModel =
                from d in dayDimmension
                join p in modelPrevious on d equals p.DayOfMonth into cs1
                from subcs1 in cs1.DefaultIfEmpty()
                join c in modelCurrent on d equals c.DayOfMonth into cs2
                from subcs2 in cs2.DefaultIfEmpty()
                select new
                {
                    Day = d,
                    Current = (subcs2 == null ? 0 : subcs2.Total),
                    Previous = (subcs1 == null ? 0 : subcs1.Total)
                };

            var jsonNetResult = new JsonNetResult
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new JsonEnumConvertor(), new DateTimeConverter() },
                    DefaultValueHandling = DefaultValueHandling.Include,
                    NullValueHandling = NullValueHandling.Include
                },
                Formatting = Formatting.Indented,
                Data = new { Data = acitvityModel }
            };

            return jsonNetResult;
        }

        public virtual async Task<ActionResult> NationBuilderSalesMetrics(CancellationToken cancellation)
        {
            const String Sql = @"
select 1 [Level], 'Total Nations' [Description],count(*) [Count] from integration.NationBuilderRegistration (nolock)
union
select 2 [Level], 'Active Nations' [Description],count(*) [Count] from integration.NationBuilderRegistration (nolock) where IsActive=1
union
select 3,'Started Order', count(*) from (select distinct IntegrationId from sales.Cart (nolock) where Source=1) g
union
select 4,'Completed Order', count(*) from (select distinct IntegrationId from sales.Cart (nolock) where Source=1 and IsActive=0) g
";

            var db = ((DbContext)this.matchCountQuery).Database;
            var query = db.SqlQuery<NationBuilderOrderMetric>(Sql);
            var data = await query.ToArrayAsync(cancellation);

            var jsonNetResult = new JsonNetResult
            {
                Data = data
            };

            return jsonNetResult;
        }

        #endregion

        #region Nested Types
        
        public class NationBuilderOrderMetric
        {
            public String Description { get; set; }

            public Int32 Level { get; set; }

            public Int32 Count { get; set; }
        }

        /// <summary>
        /// Indicates one or more sources for match reporting.
        /// </summary>
        [Serializable()]
        [Flags()]
        public enum Source
        {
            /// <summary>
            /// A match from a <see cref="Job"/> source, irrespective of what channel it was delivered from (FTP, SMTP, ADMIN, NB)
            /// </summary>
            Batch = 1,
            /// <summary>
            /// A match from a real-time API call.
            /// </summary>
            Api = Batch * 2,
            /// <summary>
            /// A match from any source.
            /// </summary>
            All = Batch + Api
        }

        #endregion
    }
}