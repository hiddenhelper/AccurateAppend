using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing.Mapping;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IJobQueueMetricsQuery"/> query.
    /// </summary>
    public class JobQueueMetricsQuery : IJobQueueMetricsQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        public JobQueueMetricsQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IJobQueueMetricsQuery Members

        /// <summary>
        /// Crafts a reqult for <see cref="JobQueueMetric"/> entities for the indicated user that have been processed during the indicated time frame.
        /// </summary>
        public virtual async Task<IEnumerable<JobQueueMetric>> Query(Guid applicationId, DateTime start, DateTime end, CancellationToken cancellation)
        {
            #region sql statement
            var sql = @"set nocount on
                        declare @dimm table( [Year] int, [Month] int, [Day] int, [Hour] int)
                        declare @nb table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)
                        declare @ftp table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)
                        declare @email table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)
                        declare @admin table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)
                        declare @clients table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)
                        declare @lb table( [Year] int, [Month] int, [Day] int, [Hour] int, [Users] int,  [Count] int, [Records] int)

                        declare @application uniqueidentifier = @p0
                        declare @startdate datetime = @p1
                        declare @enddate datetime = @p2

                        declare @completeStatus INT = " + (Int32)JobStatus.Complete + @"
                        declare @sourceFtp INT = " + FtpJobConfiguration.Source + @"
                        declare @sourceSmtp INT = " + SmtpJobConfiguration.Source + @"
                        declare @sourceAdmin INT = " + ManualJobConfiguration.Source + @"
                        declare @sourceNb INT = " + IntegrationJobConfiguration.Source + @"
                        declare @sourceClient INT = " + ClientJobConfiguration.Source + @"
                        declare @sourceLb INT = " + ListbuilderJobConfiguration.Source + @"

                        DECLARE @date datetime = @startdate
                        WHILE (@date < dateadd(hh, 1, @enddate))
                        BEGIN
                        insert @dimm values (datepart(year,@date), datepart(month,@date), datepart(day,@date),datepart(hour,@date))
                        set @date = dateadd(hh,1,@date)
                        END;

                        insert into @clients
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceClient and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        insert into @ftp
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceFtp and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        insert into @nb
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceNb and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        insert into @email
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceSmtp and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        insert into @admin
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceAdmin and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        insert into @lb
                        select distinct datepart(year, DateSubmitted) [Year], datepart(month, DateSubmitted) [Month], datepart(day, DateSubmitted) [Day], datepart(hour, DateSubmitted) [Hour]
                                , count(distinct userid) [Users],  count(*) [Count], sum(RecordCount) Records
                        from [admin].[JobQueueView]
                        where DateSubmitted between @startdate and @enddate 
                            and Status = @completeStatus and Source = @sourceLb and ApplicationId = @application
                        group by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)
                        order by datepart(year, DateSubmitted), datepart(month, DateSubmitted), datepart(day, DateSubmitted), datepart(hour, DateSubmitted)

                        select
                            d.Year, d.Month, d.Day, d.Hour,
                            coalesce(f.Count,0) FTPCount, coalesce(f.Users,0) [FTPUsers], coalesce(f.Records,0) FTPRecords,
                            coalesce(e.Count,0) EmailCount, coalesce(e.Users,0) [EmailUsers], coalesce(e.Records,0) EmailRecords,
                            coalesce(a.Count,0) AdminCount, coalesce(a.Users,0) [AdminUsers], coalesce(a.Records,0) AdminRecords,
                            coalesce(n.Count,0) NationBuilderCount, coalesce(n.Users,0) [NationBuilderUsers], coalesce(n.Records,0) NationBuilderRecords,
                            coalesce(c.Count,0) ClientCount, coalesce(c.Users,0) [ClientUsers], coalesce(c.Records,0) ClientRecords,
                            coalesce(l.Count,0) ListbuilderCount, coalesce(l.Users,0) [ListBuilderUsers], coalesce(l.Records,0) ListbuilderRecords
                        from @dimm d
                        left join @ftp f on d.Year = f.Year and d.Month = f.Month and d.Day = f.Day and d.Hour = f.Hour
                        left join @email e on d.Year = e.Year and d.Month = e.Month and d.Day = e.Day and d.Hour = e.Hour
                        left join @admin a on d.Year = a.Year and d.Month = a.Month and d.Day = a.Day and d.Hour = a.Hour
                        left join @nb n on d.Year = n.Year and d.Month = n.Month and d.Day = n.Day and d.Hour = n.Hour
                        left join @clients c on d.Year = c.Year and d.Month = c.Month and d.Day = c.Day and d.Hour = c.Hour
                        left join @lb l on d.Year = l.Year and d.Month = l.Month and d.Day = l.Day and d.Hour = l.Hour
                        order by d.Year, d.Month, d.Day, d.Hour";
            #endregion

            var data = await this.context.Database.SqlQuery<JobQueueMetric>(
                sql,
                applicationId,
                start,
                end)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
            return data;
        }

        #endregion
    }
}