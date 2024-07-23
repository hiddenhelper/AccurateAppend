using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Areas.Operations.EventLog.Models;
using AccurateAppend.Websites.Admin.Configuration;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using DomainModel.Enum;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Operations.EventLog
{
    /// <summary>
    /// Controller for presenting system event information
    /// </summary>
    [Authorize()]
    public class EventLogController : ActivityLoggingController
    {
        #region Constructor

        public EventLogController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods
        public async Task<ActionResult> Index(string correlationId, Int32? eventId)
        {
            await this.LogEventAsync("Event log viewed");

            var model = new EventsRequest() {CorrelationId = correlationId, EventId = eventId};
            model.Email.AddRange(new[] {"chris@accurateappend.com", "jimmy@accurateappend.com"});

            return this.View(model);
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> EventSumamry_Read(
            [DataSourceRequest] DataSourceRequest request,
            DateRange dateRange,
            DateTime? hour,
            TraceEventType? eventType,
            Severity? severity,
            String application,
            String host,
            Int32? eventid,
            Guid? correlationId,
            Boolean information,
            CancellationToken cancellation)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;

            GetStartEndDate(dateRange, now, out startdate, out enddate);

            using (var cc = new EventLogger.DefaultContext())
            { 
                var query = information ? cc.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate) 
                    : cc.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate && e.EventType != TraceEventType.Information);

                if (correlationId != null)
                {
                    query = query.Where(e => e.CorrelationId == correlationId.Value);
                }
                if (!String.IsNullOrEmpty(application))
                {
                    query = query.Where(e => e.Application == application);
                }
                if (!String.IsNullOrEmpty(host))
                {
                    query = query.Where(e => e.Host == host);
                }
                if (severity != null)
                {
                    query = query.Where(e => e.Severity == severity.Value);
                }
                if (eventType != null)
                {
                    query = query.Where(e => e.EventType == eventType.Value);
                }
                if (eventid != null)
                {
                    query = query.Where(e => e.Id == eventid);
                }
                if (!String.IsNullOrEmpty(host))
                {
                    query = query.Where(e => e.Host == host);
                }
                if (hour != null)
                {
                    var limit = hour.Value.AddHours(1);
                    query = query.Where(e => e.EventDate >= hour.Value && e.EventDate <= limit);
                }

                query = query.OrderByDescending(e => e.EventDate);

                var final = await query.ToArrayAsync(cancellation);
                var count = final.Count();

                var data = final.ToDataSourceResult(request, o => new
                        {
                            id = o.Id,
                            EventDate = o.EventDate.ToUserLocal(),
                            o.Application,
                            o.Host,
                            o.Severity,
                            o.EventType,
                            o.Message,
                            o.ThreadIdentity,
                            o.CorrelationId,
                            o.StackTrace,
                            o.Source,
                            o.Target,
                            o.Description,
                            o.IP,
                            o.Username,
                            o.RelatedEvents
                        });

                data.Total = count;

                var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
                {
                    Data = data
                };

                return jsonNetResult;
            }
        }

        
        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetEventTypes(DateRange dateRange, Guid? correlationId, Boolean information, CancellationToken cancellation)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;
            GetStartEndDate(dateRange, now, out startdate, out enddate);

            using (var context = new EventLogger.DefaultContext())
            {
                var baseQuery = information ? context.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate) 
                    : context.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate && e.EventType != TraceEventType.Information);

                if (correlationId != null)
                {
                    var ids = (await this.GetInputFileNames(correlationId.ToString(), cancellation)).ToArray();
                    baseQuery = baseQuery.Where(e => ids.Contains(e.CorrelationId.Value));
                }

                var query = baseQuery
                    .GroupBy(e => e.EventType)
                    .Select(g => new { Description = g.Key, Cnt = g.Count()})
                    .Distinct()
                    .OrderBy(e => e.Description);
                
                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Data = await query.ToArrayAsync(cancellation);

                return jsonNetResult;
            }
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public ActionResult GetHours(String eventType, Boolean information)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;
            GetStartEndDate(DateRange.Last24Hours, now, out startdate, out enddate);

            // todo: don't return hours with 0
            string @sql = @"CREATE TABLE #dimm ( [Year] int, [Month] int, [Day] int, [Hour] int)
                        DECLARE @date datetime;set @date = dateadd(hh,-24,getdate())
                        WHILE (@date < getdate())
                        BEGIN
                        insert #dimm values (datepart(year,@date), datepart(month,@date), datepart(day,@date),datepart(hour,@date))
                        set @date = dateadd(hour,1,@date)
                        END;
            
                        select distinct datepart(year, dateadd(hour,-7,EventDate)) [Year], datepart(month, dateadd(hour,-7,EventDate)) [Month], datepart(day, dateadd(hour,-7,EventDate)) [Day], datepart(hour, dateadd(hour,-7,EventDate)) [Hour],  count(*) Cnt
                        into #stats 
                        from [logging].[LogCore] (nolock)
                        WHERE [EventDate] BETWEEN @p0 AND @p1";
            
            if (!String.IsNullOrEmpty(eventType))
            {
                var s = (TraceEventType) System.Enum.Parse(typeof (TraceEventType), eventType);
                @sql += " and [EventType] = " + (int) s + "";
            }

            if (!information)
            {
                var s = TraceEventType.Information;
                @sql += " and [EventType] = " + (int) s + "";
            }

            @sql +=
                @"  group by datepart(year, dateadd(hour,-7,EventDate)), datepart(month, dateadd(hour,-7,EventDate)), datepart(day, dateadd(hour,-7,EventDate)), datepart(hour, dateadd(hour,-7,EventDate));
                        select (cast(#dimm.[Year] as varchar) + '-' + cast(#dimm.[Month] as varchar) + '-' + cast(#dimm.[Day] as varchar)+ ' ' + cast(#dimm.[Hour] as varchar) + ':00') [Hour], isnull(#stats.[Cnt],0) Cnt from #dimm left join #stats on #dimm.[Hour] = #stats.[Hour] order by #dimm.[Year], #dimm.[Month], #dimm.[Day], #dimm.[Hour];";

            using (var context = new System.Data.Linq.DataContext(Config.EventLogDb))
            {
                var data = context.ExecuteQuery<DtoHours>(sql, startdate, enddate).ToArray();

                var jsonNetResult = new JsonNetResult
                {
                    Data = data.Where(a => a.Cnt > 0) // don't return hours with no errors
                };
                return jsonNetResult;
            }
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetSeverity(
            DateRange dateRange, 
            DateTime? hour,
            TraceEventType? eventType,
            Guid? correlationId,
            Boolean information,
            CancellationToken cancellation)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;
            GetStartEndDate(dateRange, now, out startdate, out enddate);

            using (var context = new EventLogger.DefaultContext())
            {
                var query = information ? context.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate) 
                    : context.Events.Where(e => e.EventDate >= startdate && e.EventDate <= enddate && e.EventType != TraceEventType.Information);;

                if (correlationId != null)
                {
                    var ids = (await this.GetInputFileNames(correlationId.ToString(), cancellation)).ToArray();
                    query = query.Where(e => ids.Contains(e.CorrelationId.Value));
                }
                if (hour != null)
                {
                    var limit = hour.Value.AddHours(1);
                    query = query.Where(e => e.EventDate >= hour.Value && e.EventDate <= limit);
                }
                if (eventType != null)
                {
                    query = query.Where(e => e.EventType == eventType.Value);
                }

                var final = query.GroupBy(e => e.Severity)
                    .Select(g => new {Description = g.Key, Cnt = g.Count()})
                    .OrderBy(e => e.Description);

                var data = await final.ToArrayAsync(cancellation);

                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Data = data;
                return jsonNetResult;
            }
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetApplications(
            DateRange dateRange,
            DateTime? hour,
            TraceEventType? eventType,
            Severity? severity,
            Guid? correlationId,
            Boolean information,
            CancellationToken cancellation)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;
            GetStartEndDate(dateRange, now, out startdate, out enddate);

            using (var context = new EventLogger.DefaultContext())
            {
                var query = information ? context.Events : context.Events.Where(e => e.EventType != TraceEventType.Information);

                if (hour != null)
                {
                    var limit = hour.Value.AddHours(1);
                    query = query.Where(e => e.EventDate >= hour && e.EventDate <= limit);
                }
                else
                {
                    query = query.Where(e => e.EventDate >= startdate && e.EventDate <= enddate);
                }

                if (severity != null)
                {
                    query = query.Where(e => e.Severity == severity.Value);
                }

                if (eventType != null)
                {
                    query = query.Where(e => e.EventType == eventType.Value);
                }

                if (correlationId != null)
                {
                    var ids = (await this.GetInputFileNames(correlationId.ToString(), cancellation)).ToArray();
                    query = query.Where(e => ids.Contains(e.CorrelationId.Value));
                }

                var final = query.GroupBy(e => e.Application)
                    .Select(g => new {Description = g.Key, Cnt = g.Count()})
                    .Distinct()
                    .OrderBy(e => e.Description);

                var data = await final.ToArrayAsync(cancellation);

                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Data = data;

                return jsonNetResult;
            }
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetHosts(
            DateRange dateRange,
            DateTime? hour,
            TraceEventType? eventType,
            Severity? severity,
            String application,
            Guid? correlationId,
            Boolean information,
            CancellationToken cancellation)
        {
            DateTime startdate;
            DateTime enddate;
            var now = DateTime.UtcNow;
            GetStartEndDate(dateRange, now, out startdate, out enddate);

            using (var context = new EventLogger.DefaultContext())
            {
                var query = information ? context.Events : context.Events.Where(e => e.EventType != TraceEventType.Information);

                if (hour != null)
                {
                    var limit = hour.Value.AddHours(1);
                    query = query.Where(e => e.EventDate >= hour && e.EventDate <= limit);
                }
                else
                {
                    query = query.Where(e => e.EventDate >= startdate && e.EventDate <= enddate);
                }

                if (severity != null)
                {
                    query = query.Where(e => e.Severity == severity.Value);
                }

                if (eventType != null)
                {
                    query = query.Where(e => e.EventType == eventType.Value);
                }

                if (!String.IsNullOrEmpty(application))
                {
                    query = query.Where(e => e.Application == application);
                }

                if (correlationId != null)
                {
                    var ids = (await this.GetInputFileNames(correlationId.ToString(), cancellation)).ToArray();
                    query = query.Where(e => ids.Contains(e.CorrelationId.Value));
                }

                var final = query.GroupBy(e => e.Host)
                    .Select(g => new {Description = g.Key, Cnt = g.Count()})
                    .Distinct()
                    .OrderBy(e => e.Description);

                var data = await final.ToArrayAsync(cancellation);

                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Data = data;

                return jsonNetResult;
            }
        }

        #endregion

        #region helpers

        /// <summary>
        /// Returns collection of InputFileNames for a given job using InputFileName or JobId
        /// </summary>
        private async Task<IEnumerable<Guid>> GetInputFileNames(String value, CancellationToken cancellation)
        {
            IEnumerable<String> files;

            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                // if numeric then assume it's a JobId, otherwise assume it's a Guid
                Int32 jobid;
                Guid correlationId;

                if (Int32.TryParse(value, out jobid))
                {
                    var jobs = this.Context.SetOf<Job>();
                    var slices = this.Context.SetOf<Slice>();
                    files = await jobs.Where(j => j.Id == jobid)
                            .Select(j => j.InputFileName)
                            .Concat(slices.Where(s => s.Job.Id == jobid).Select(s => s.InputFileName))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
                }
                else if (Guid.TryParse(value, out correlationId))
                {
                    var jobs = this.Context.SetOf<Job>();
                    var slices = this.Context.SetOf<Slice>();

                    files = await jobs.Where(j => j.InputFileName == value)
                            .Select(j => j.InputFileName)
                            .Concat(slices.Where(s => s.Job.InputFileName == value).Select(s => s.InputFileName))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
                }
                else
                {
                    files = Enumerable.Empty<String>();
                }
            }

            return files.Select(f => new Guid(f));
        }

        
        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetGraphForLast30Minutes(String application, String host, String severity,
            String correlationId, CancellationToken cancellation)
        {
            var @sql = "create table #dimm ( [Hour] int, [Minute] int)" +
                          " declare @date datetime;set @date = dateadd(minute,-30,getdate())" +
                          " WHILE (@date < getdate())" +
                          " BEGIN" +
                          "  set @date = dateadd(minute,1,@date)" +
                          "   insert #dimm values (datepart(hour,@date),datepart(minute,@date))" +
                          "END;";

            @sql +=
                @"select distinct datepart(hour, dateadd(hour,-7,EventDate)) [Hour], datepart(minute, dateadd(hour,-7,EventDate)) [Minute],  count(*) Cnt" +
                " into #stats from [logging].[LogCore] (nolock)" +
                " where EventType = 2 and EventDate between dateadd(hh,-1,getutcdate()) and getutcdate() #correlationid#";

            // if correlationid is present then insert it in the where clause
            if (!String.IsNullOrEmpty(correlationId))
            {
                var ids = (await this.GetInputFileNames(correlationId, CancellationToken.None)).Select(id => $"'{id}'");
                @sql = @sql.Replace("#correlationid#",
                    " AND [CorrelationId] in (" +
                    string.Join<string>(",", ids) + ")");
            }
            else
            {
                @sql = @sql.Replace("#correlationid#", String.Empty);
            }

            if (!String.IsNullOrEmpty(application)) @sql += " and [Application] = '" + application + "'";
            if (!String.IsNullOrEmpty(host)) @sql += " and [Host] = '" + host + "'";
            if (!String.IsNullOrEmpty(severity)) @sql += " and [Severity] = '" + (int) System.Enum.Parse(typeof (Severity), severity) + "'";

            @sql += " group by datepart(hour, dateadd(hour,-7,EventDate)), datepart(minute, dateadd(hour,-7,EventDate));";

            @sql += "select #dimm.[Hour], #dimm.[Minute], isnull(#stats.[Cnt],0) Cnt from #dimm left join #stats on #dimm.[Hour] = #stats.[Hour] and #dimm.[Minute] = #stats.[Minute] order by #dimm.[Hour], #dimm.[Minute];";

            using (var context = new System.Data.Linq.DataContext(Config.EventLogDb))
            {
                DtoGraph[] data = context.ExecuteQuery<DtoGraph>(sql).ToArray();
                var jsonNetResult = new JsonNetResult
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }

        [OutputCache(Duration = 0, Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetGraphForLast24Hours(String correlationId)
        {
            DateTime startdate;
            DateTime enddate;
            DateTime now = DateTime.Now.ToUniversalTime();
            GetStartEndDate(DateRange.Last24Hours, now, out startdate, out enddate);

            string @sql = @"CREATE TABLE #dimm ( [Year] int, [Month] int, [Day] int, [Hour] int)
                        DECLARE @date datetime;set @date = dateadd(hh,-24,getdate())
                        WHILE (@date < getdate())
                        BEGIN
                        insert #dimm values (datepart(year,@date), datepart(month,@date), datepart(day,@date),datepart(hour,@date))
                        set @date = dateadd(hour,1,@date)
                        END;
            
                        select distinct datepart(year, dateadd(hour,-7,EventDate)) [Year], datepart(month, dateadd(hour,-7,EventDate)) [Month], datepart(day, dateadd(hour,-7,EventDate)) [Day], datepart(hour, dateadd(hour,-7,EventDate)) [Hour],  count(*) Cnt
                        into #stats 
                        from [logging].[LogCore] (nolock)
                        WHERE [EventDate] BETWEEN '#startdate#' AND '#enddate#' and [EventType] = 2 #correlationid#
                        group by datepart(year, dateadd(hour,-7,EventDate)), datepart(month, dateadd(hour,-7,EventDate)), datepart(day, dateadd(hour,-7,EventDate)), datepart(hour, dateadd(hour,-7,EventDate));
                        
                        select (cast(#dimm.[Year] as varchar) + '-' + cast(#dimm.[Month] as varchar) + '-' + cast(#dimm.[Day] as varchar)+ ' ' + cast(#dimm.[Hour] as varchar) + ':00') [Hour], isnull(#stats.[Cnt],0) Cnt from #dimm left join #stats on #dimm.[Hour] = #stats.[Hour] order by #dimm.[Year], #dimm.[Month], #dimm.[Day], #dimm.[Hour];";

            // if correlationid is present then insert it in the where clause
            if (!String.IsNullOrEmpty(correlationId))
            {
                var ids = (await this.GetInputFileNames(correlationId, CancellationToken.None)).Select(id => $"'{id}'");
                @sql = @sql.Replace("#correlationid#",
                    " AND [CorrelationId] in (" +
                    string.Join<string>(",", ids) + ")");
            }
            else
            {
                @sql = @sql.Replace("#correlationid#", String.Empty);
            }

            @sql = @sql.Replace("#startdate#", startdate.ToString());
            @sql = @sql.Replace("#enddate#", enddate.ToString());

            using (var context = new System.Data.Linq.DataContext(Config.EventLogDb))
            {
                var data = context.ExecuteQuery<DtoHours>(sql).ToArray();
                var jsonNetResult = new JsonNetResult
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }

        public ActionResult GetEventDetail(Int32 eventId)
        {
            using (var context = new EventLogger.DefaultContext())
            {
                const String Sql =
                    @"SELECT FormattedData [data] FROM logging.Exceptions WHERE Id={0}
UNION ALL
SELECT AdditionalInfo [data] FROM logging.Traces WHERE Id={0}";
                var data = context.Database.SqlQuery<String>(Sql, eventId).FirstOrDefault();
                var result = new LiteralResult()
                {
                    ContentType = "application/xml",
                    Data = data,
                    Title = "Event " + eventId
                };
                return result;
            }
        }

        #endregion

        #region Dto objects

        private class DtoGraph
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
            public int Cnt { get; set; }
        }
        
        private class DtoHours
        {
            public string Hour { get; set; }
            public int Cnt { get; set; }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns start and end for a specific dateRange
        /// </summary>
        private static void GetStartEndDate(DateRange dateRange, DateTime now, out DateTime start, out DateTime end)
        {
            var startOfDay = now.ToStartOfDay();
            end = startOfDay;
            start = startOfDay.AddDays(-1);

            switch (dateRange)
            {
                case DateRange.Last60Minutes:
                    start = now.AddHours(-1);
                    end = now;
                    break;
                case DateRange.Last24Hours:
                    start = now.AddHours(-24);
                    end = now;
                    break;
                case DateRange.Yesterday:
                    start = now.AddHours(-24);
                    end = now;
                    break;
                case DateRange.Today:
                    start = startOfDay;
                    end = startOfDay.AddDays(1);
                    break;
                case DateRange.Last7Days:
                    end = startOfDay.AddDays(1);
                    start = startOfDay.AddDays(-7);
                    break;
                case DateRange.Last30Days:
                    end = startOfDay.AddDays(1);
                    start = startOfDay.AddDays(-30);
                    break;
                case DateRange.Last60Days:
                    end = startOfDay.AddDays(1);
                    start = startOfDay.AddDays(-60);
                    break;
                case DateRange.Last90Days:
                    end = startOfDay.AddDays(1);
                    start = startOfDay.AddDays(-90);
                    break;
                case DateRange.CurrentMonth:
                    end = startOfDay.AddDays(1);
                    start = now.ToFirstOfMonth();
                    break;
                case DateRange.LastMonth:
                    start = now.ToFirstOfMonth().AddMonths(-1);
                    end = start.AddMonths(1);
                    break;
                case DateRange.PreviousToLastMonth:
                    start = now.ToFirstOfMonth().AddMonths(-2);
                    end = start.AddMonths(1);
                    break;
            }
        }

        #endregion
    }
}