using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="T:DomainModel.Queries.IOperationReportMetricQuery" /> component.
    /// </summary>
    public class AdminUserActivityMetricQuery : IAdminUserActivityMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminUserActivityMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public AdminUserActivityMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IUserActivityMetricQuery Members

        Task<IEnumerable<AdminUserActivityMetricRecord>> IAdminUserActivityMetricQuery.Query(Guid userid, CancellationToken cancellation)
        {
            const String Sql = "EXEC [operations].[AdminUserActivityMetrics] @UserId=@p0";

            //// DEBUG
            //var task = this.context.Database.SqlQuery<AdminUserActivityMetricRecord>(Sql, userid)
            //    .ToArrayAsync(cancellation);

            //task.ContinueWith(t =>
            //    {
            //        Console.WriteLine(t.Exception.Message);
            //    },
            //    TaskContinuationOptions.OnlyOnFaulted);

            //return task.ContinueWith(t => (IEnumerable<AdminUserActivityMetricRecord>)t.Result,
            //    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);


            return this.context.Database.SqlQuery<AdminUserActivityMetricRecord>(Sql, userid)
                .ToArrayAsync(cancellation)
                .ContinueWith(t => (IEnumerable<AdminUserActivityMetricRecord>) t.Result);
        }

        #endregion
    }
}