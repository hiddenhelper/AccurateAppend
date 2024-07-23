using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IOperatingMetricQuery"/> component.
    /// </summary>
    public class OperatingMetricQuery : IOperatingMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatingMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public OperatingMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IDealReportMetricQuery Members

        async Task<IEnumerable<MetricRecord>> IOperatingMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateOperatingMetrics] @ApplicationId=@p0";

            return await this.context.Database.SqlQuery<MetricRecord>(Sql, applicationId)
                    .ToArrayAsync(cancellation)
                    .ConfigureAwait(false);
        }

        async Task<IEnumerable<RevenueMetric>> IOperatingMetricQuery.GenerateRevenueMetrics(Guid applicationId, DateTime startDate, DateTime endDate, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[GenerateRevenueMetrics] @ApplicationId=@p0, @start=@p1, @end=@p2";

            return await this.context.Database.SqlQuery<RevenueMetric>(Sql, applicationId, startDate, endDate)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }

        #endregion
    }
}