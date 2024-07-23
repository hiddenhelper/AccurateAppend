using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <inheritdoc />
    /// <summary>
    /// Default implementation of the <see cref="T:DomainModel.Queries.IUserProcessingMetricQuery" /> component.
    /// </summary>
    public class UserProcessingMetricQuery : IUserProcessingMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProcessingMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public UserProcessingMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IOperationReportMetricQuery Members

        Task<IEnumerable<UserProcessingMetricRecord>> IUserProcessingMetricQuery.Query(Guid applicationId, Source source, CancellationToken cancellation)
        {
            const String Sql = "EXEC [jobs].[CalculateOperationMetricsByClient] @ApplicationId=@p0, @Channel=@p1";

            // DEBUG
            var task = this.context.Database.SqlQuery<UserProcessingMetricRecord>(Sql, applicationId, (int)source)
                .ToArrayAsync(cancellation);

            task.ContinueWith(t =>
                {
                    Console.WriteLine(t.Exception.Message);
                },
                TaskContinuationOptions.OnlyOnFaulted);

            return task.ContinueWith(t => (IEnumerable<UserProcessingMetricRecord>)t.Result,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);


            //return this.context.Database.SqlQuery<UserProcessingMetricRecord>(Sql, applicationId)
            //    .ToArrayAsync(cancellation)
            //    .ContinueWith(t => (IEnumerable<UserProcessingMetricRecord>) t.Result,
            //        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        #endregion
    }

    
    /// <remarks>The UsageRollup table leverages 3 values 0,1,2 </remarks>
    public enum Source
    {
        Batch = 0,
        Api = 1
    }
}