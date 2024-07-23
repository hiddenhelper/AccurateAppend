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
    public class OperationReportMetricQuery : IOperationReportMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationReportMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public OperationReportMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IOperationReportMetricQuery Members

        async Task<IEnumerable<OperationMetricRecord>> IOperationReportMetricQuery.Query(Guid applicationId, Guid? userId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [jobs].[CalculateOperationMetrics] @ApplicationId=@p0,@UserId=@p1";

            return await this.context
                .Database
                .SqlQuery<OperationMetricRecord>(Sql, applicationId, userId)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }
        
        #endregion
    }
}