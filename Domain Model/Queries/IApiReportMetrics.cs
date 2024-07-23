using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Accounting.DataAccess;
using AccurateAppend.Core;
using AccurateAppend.Security;

namespace DomainModel.Queries
{
    public interface IApiReportMetrics
    {
        Task<IEnumerable<ServiceCountsByUser>> ServiceCountsByUser(CancellationToken cancellation, Guid? identity, DateTime? startDate = null, DateTime? endDate = null);

        Task<IEnumerable<ServiceCallsStatistics>> GetExecutionStatistics(CancellationToken cancellation, String host = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null);
        
        Task<IEnumerable<ServiceCallByExecutionTime>> GetExecutionTimes(CancellationToken cancellation, String host = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null);

        Task<IEnumerable<ServiceOperationByCount>> GetOperationCounts(CancellationToken cancellation, String server = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null);

        Task<IEnumerable<ServiceCallsByUser>> TotalCallsByUser(CancellationToken cancellation);
    }

    public class ServiceCallsStatistics
    {
        protected ServiceCallsStatistics() { }

        public ServiceCallsStatistics(DateTime date, Int32 calls, Int32 median, Int32 min, Int32 max)
        {
            if (calls < 0) throw new ArgumentOutOfRangeException(nameof(calls), calls, "Calls must be at least 0");
            if (median < 0) throw new ArgumentOutOfRangeException(nameof(median), calls, "Median must be at least 0");
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min), calls, "Min must be at least 0");
            if (max < 0) throw new ArgumentOutOfRangeException(nameof(max), calls, "Max must be at least 0");

            this.TransactionDate = date;
            this.Calls = calls;
            this.Median = median;
            this.Min = min;
            this.Max = max;
        }

        public DateTime TransactionDate { get; private set; }

        public Int32 Calls { get; private set; }

        public Int32 Median { get; private set; }

        public Int32 Min { get; private set; }

        public Int32 Max { get; private set; }
    }

    public class ServiceCountsByUser
    {
        protected ServiceCountsByUser() { }

        public ServiceCountsByUser(String operation, Int32 calls, Guid userId)
        {
            if (String.IsNullOrEmpty(operation)) throw new ArgumentNullException(nameof(operation));
            if (calls < 0) throw new ArgumentOutOfRangeException(nameof(calls), calls, "Calls must be at least 0");

            this.Operation = operation;
            this.Calls = calls;
            this.UserId = userId;
        }

        public Guid UserId { get; private set; }

        public String Operation { get; private set; }

        public Int32 Calls { get; private set; }
    }

    public class ServiceCallsByUser
    {
        protected ServiceCallsByUser() { }

        public ServiceCallsByUser(String userName, Int32 calls, Guid userId)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (calls < 0) throw new ArgumentOutOfRangeException(nameof(calls), calls, "Calls must be at least 0");

            this.UserName = userName;
            this.Calls = calls;
            this.UserId = userId;
        }

        public Guid UserId { get; private set; }

        public String UserName { get; private set; }

        public Int32 Calls { get; private set; }
    }

    public class ServiceCallByExecutionTime
    {
        public ServiceCallByExecutionTime() { }
        public ServiceCallByExecutionTime(Double seconds, Int32 calls)
        {
            if (seconds < 1) throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Seconds must be at least 1");
            if (calls < 0) throw new ArgumentOutOfRangeException(nameof(calls), calls, "Calls must be at least 0");

            this.Seconds = seconds;
            this.Calls = calls;
        }

        public Double Seconds { get; private set; }

        public Int32 Calls { get; private set; }
    }

    public class ServiceOperationByCount
    {
        public ServiceOperationByCount() { }
        public ServiceOperationByCount(String operation, Int32 calls)
        {
            if (String.IsNullOrEmpty(operation)) throw new ArgumentNullException(nameof(operation));
            if (calls < 0) throw new ArgumentOutOfRangeException(nameof(calls), calls, "Calls must be at least 0");

            this.Operation = operation;
            this.Calls = calls;
        }

        public String Operation { get; private set; }

        public Int32 Calls { get; private set; }
    }

    public class ApiReportMetrics : IApiReportMetrics
    {
        #region Constants

        private const String Rob = "43056364-2161-448E-8BD2-EE1FCEBD3492";

        #endregion

        #region Fields

        private readonly DbContext apiContext;
        private readonly DefaultContext salesContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiReportMetrics"/> class.
        /// </summary>
        /// <param name="apiContext"><see cref="DbContext"/> providing access to the real-time API database.</param>
        /// <param name="salesContext"><seealso cref="DbContext"/> providing access to the sales database.</param>
        public ApiReportMetrics(DbContext apiContext, DefaultContext salesContext)
        {
            if (apiContext == null) throw new ArgumentNullException(nameof(apiContext));
            if (salesContext == null) throw new ArgumentNullException(nameof(salesContext));
            Contract.EndContractBlock();

            this.apiContext = apiContext;
            this.salesContext = salesContext;
        }

        #endregion

        #region IApiReportMetrics Members

        public virtual async Task<IEnumerable<ServiceCountsByUser>> ServiceCountsByUser(CancellationToken cancellation, Guid? identity, DateTime? startDate = null, DateTime? endDate = null)
        {
            endDate = endDate.Coerce();
            if (endDate == null) endDate = DateTime.Now.ToEndOfDay().ToUniversalTime();

            startDate = startDate.Coerce();
            if (startDate == null) startDate = endDate.Value.ToLocalTime().ToFirstOfMonth().ToUniversalTime();

            const String Sql = @"SELECT UserId, Method [Operation], Count(*) [Calls] FROM dbo.XMLTransactions WHERE UserId=@p0 AND TransactionDate >= @p1 AND TransactionDate <= @p2 GROUP BY UserId, Method ORDER BY Method DESC";

            var data = await this.apiContext
                .Database
                .SqlQuery<ServiceCountsByUser>(Sql, identity, startDate.Value, endDate.Value)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);

            return data;
        }

        public virtual async Task<IEnumerable<ServiceCallsStatistics>> GetExecutionStatistics(CancellationToken cancellation, String host = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null)
        {
            if (endDate == null) endDate = DateTime.UtcNow;
            if (startDate == null) startDate = endDate.Value.AddDays(-5);

            var sql = @"
            declare @table table(ProcessingTime int, TransactionDate date)
            insert into @table
            select
                processingtime,
                convert(date, transactiondate)
            from dbo.xmltransactions
            where transactiondate between '" + startDate + "' and '" + endDate + "' and ProcessingTime > 0 and userid <> '" + Rob + "'";
            if (host != null)
            {
                sql = sql + " and Server='" + host.Replace("'", "''") + "'";
            }
            if (userId != null)
            {
                sql = sql + " and userid = '" + userId + "'";
            }
            sql = sql + @";

            WITH MedianGroupA
            AS
            (
                SELECT
                ProcessingTime,
                TransactionDate,
                Row_Number() OVER(PARTITION BY TransactionDate ORDER BY TransactionDate) [A]
                FROM @table
            ),
            MedianGroupB
            AS
            (
                SELECT
                ProcessingTime,
                TransactionDate,
                A,
                Row_Number() OVER(PARTITION BY TransactionDate ORDER BY TransactionDate DESC) [B]
                FROM MedianGroupA
            )
            SELECT
                TransactionDate,
                Count(*) [Calls],
                Avg(ProcessingTime) [Median],
                Min(ProcessingTime) [Min],
                Max(ProcessingTime) [Max]
            FROM MedianGroupB
            WHERE Abs(A-B) <=1
            GROUP BY TransactionDate
            ";

            var results = await this.apiContext
                .Database
                .SqlQuery<ServiceCallsStatistics>(sql)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);

            return results;
        }

        public virtual async Task<IEnumerable<ServiceCallByExecutionTime>> GetExecutionTimes(CancellationToken cancellation, String host = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null)
        {
            if (startDate == null) startDate = DateTime.UtcNow.AddDays(-1);

            var sql = @"
              declare @table table(seconds float)
  
              insert into @table
              SELECT floor(processingtime/1000) seconds
              FROM [XMLTransactions].[dbo].[XMLTransactions]
              where TransactionDate >= '" + startDate + @"' AND userid <> '" + Rob + "'";
            if (endDate != null)
            {
                sql = sql + @" AND TransactionDate <= '" + endDate + "'";
            }
            if (host != null)
            {
                sql = sql + @" AND Server = '" + host.Replace("'", "''") + "'";
            }
            if (userId != null)
            {
                sql = sql + @" AND UserId = '" + userId + "'";
            }

            sql = sql + @"
 
               select distinct seconds, count(*) Calls
               from @table
               group by seconds
               order by seconds
            ";
            var results = await this.apiContext
                .Database
                .SqlQuery<ServiceCallByExecutionTime>(sql)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);

            var detail = results.Where(r => r.Seconds <= 6);
            var remainder = new[]
                                {
                                    new ServiceCallByExecutionTime(7, results.Where(r => r.Seconds > 6).Sum(r => r.Calls))
                                };

            return detail.Union(remainder);
        }

        public virtual async Task<IEnumerable<ServiceOperationByCount>> GetOperationCounts(CancellationToken cancellation, String server = null, DateTime? startDate = null, DateTime? endDate = null, Guid? userId = null)
        {
            if (startDate == null) startDate = DateTime.UtcNow.AddDays(-1);

            var sql = @"

              declare @table table(Method varchar(50))
  
              insert into @table
              SELECT Method
              FROM [XMLTransactions].[dbo].[XMLTransactions]
              where TransactionDate >= '" + startDate + @"' AND UserId <> '" + Rob + "'";
                        if (endDate != null)
                        {
                            sql = sql + @" AND TransactionDate <= '" + endDate + "'";
                        }
                        if (userId != null)
                        {
                            sql = sql + @" AND UserId = '" + userId + "'";
                        }
                        if (server != null)
                        {
                            sql = sql + @" AND Server = '" + server.Replace("'", "''") + "'";
                        }
                        sql = sql + @"
 
               select distinct Method [Operation], count(*) Calls
               from @table
               group by Method
               order by Method
            ";
            var results = await this.apiContext
                .Database
                .SqlQuery<ServiceOperationByCount>(sql)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
            return results;
        }

        public virtual async Task<IEnumerable<ServiceCallsByUser>> TotalCallsByUser(CancellationToken cancellation)
        {
            var enddate = DateTime.UtcNow.ToEndOfDay();
            var startdate = enddate.AddDays(-1).ToStartOfDay();

            var rob = new Guid("43056364-2161-448E-8BD2-EE1FCEBD3492");
            
            var transactionQuery = this.salesContext
                .Set<SoapCallsDailyUsageRollup>()
                .Where(t => t.Date >= startdate && t.Date <= enddate && t.UserId != rob)
                .GroupBy(t => t.UserId)
                .Select(g => new {UserId = g.Key, Count = g.Sum(c => c.Count)});

            var usersQuery = this.salesContext
                .SetOf<User>()
                .Where(u => u.LastActivityDate >= startdate);

            var resultsQuery = transactionQuery.Join(usersQuery, t => t.UserId, u => u.Id,
                (t, u) => new {Email = u.UserName, UserId = u.Id, t.Count});
            var data = await resultsQuery.OrderByDescending(a => a.Count).ToArrayAsync(cancellation);

            return data.Select(d => new ServiceCallsByUser(d.Email, d.Count, d.UserId));
        }
        
        #endregion
    }
}