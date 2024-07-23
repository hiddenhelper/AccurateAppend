using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IDealMetricQuery" /> and <see cref="IUserDealMetricQuery"/> components.
    /// </summary>
    public class DealMetricQuery : IDealMetricQuery, IUserDealMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public DealMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IDealMetricQuery Members

        async Task<IEnumerable<DealMetricRecord>> IDealMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateDealMetrics] @ApplicationId=@p0";

            return await this.context.Database.SqlQuery<DealMetricRecord>(Sql, applicationId)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }

        #endregion

        #region IUserDealMetricQuery Members

        async Task<IEnumerable<UserDealMetricRecord>> IUserDealMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateUserDealMetrics] @ApplicationId=@p0";

            return await this.context.Database.SqlQuery<UserDealMetricRecord>(Sql, applicationId)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }

        #endregion
    }
}