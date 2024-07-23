using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Dashboard
{
    /// <summary>
    /// Controller for handling job processing detail analysis.
    /// </summary>
    [Authorize()]
    public class DashboardController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public DashboardController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Index(Int32 jobId)
        {
            return this.View(jobId);
        }

        public virtual async Task<ActionResult> SliceStatus(Int32 jobId, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var query = this.Context.SetOf<Slice>()
                    .Where(s => s.Job.Id == jobId)
                    .GroupBy(s => new {s.Status, s.Processor})
                    .Select(g =>
                        new
                        {
                            g.Key.Status,
                            g.Key.Processor,
                            Count = g.Count(),
                            SystemErrors = g.Sum(gg => gg.Processing.SystemErrors),
                            DateCompleted = g.Max(gg => gg.DateCompleted),
                            DateUpdated = g.Max(gg => gg.DateUpdated),
                        });

                var final = await query.ToArrayAsync(cancellation);

                var result = new JsonNetResult(DateTimeKind.Local)
                {
                    Data = final.Select(s =>
                        new
                        {
                            s.Count,
                            s.Processor,
                            s.Status,
                            s.SystemErrors,
                            LastActive = s.Status == Core.Definitions.SliceStatus.Complete 
                                ? s.DateCompleted?.DescribeDifference(DateTime.Now) 
                                : s.DateUpdated.DescribeDifference(DateTime.Now)
                        })
                };

                return result;
            }
        }

        //[OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public virtual async Task<ActionResult> EventSummary(Int32 jobId, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var jobCorrelation = new Guid(await this.Context.SetOf<Job>().Where(j => j.Id == jobId).Select(j => j.InputFileName).FirstAsync(cancellation));
                var sliceCorrelations = await this.Context.SetOf<Slice>().Where(s => s.Job.Id == jobId).Select(s => s.InputFileName).ToArrayAsync(cancellation);

                var ids = sliceCorrelations.Select(id => new Guid(id)).ToList();

                using (var context = new EventLogger.DefaultContext())
                {
                    var jobQuery = context.Events.Where(e => e.CorrelationId.Value == jobCorrelation);
                    var jobFinal = await jobQuery
                        .GroupBy(e => new {e.Message, e.Application, e.StackTrace, e.Severity})
                        .Select(g =>
                            new
                            {
                                g.Key.Application,
                                g.Key.Message,
                                g.Key.Severity,
                                g.Key.StackTrace,
                                Count = g.Count(),
                                FirstTime = g.Min(gg => gg.EventDate),
                                LastTime = g.Max(gg => gg.EventDate)
                            })
                        .OrderBy(d => d.FirstTime)
                        .ToArrayAsync(cancellation);
                    
                    var sliceQuery = context.Events.Where(e => ids.Contains(e.CorrelationId.Value));
                    var sliceFinal = await sliceQuery
                        .GroupBy(e => new {e.Message, e.Application, e.StackTrace, e.Severity})
                        .Select(g =>
                            new
                            {
                                g.Key.Application,
                                g.Key.Message,
                                g.Key.Severity,
                                g.Key.StackTrace,
                                Count = g.Count(),
                                FirstTime = g.Min(gg => gg.EventDate),
                                LastTime = g.Max(gg => gg.EventDate)
                            })
                        .OrderBy(d => d.FirstTime)
                        .ToArrayAsync(cancellation);

                    return new JsonNetResult(DateTimeKind.Local)
                    {
                        Data = new
                        {
                            SliceEventSummary = sliceFinal,
                            JobEventSummary = jobFinal
                        }
                    };
                }
            }
        }

        #endregion
    }
}