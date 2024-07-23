using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IApiTrailMetricsQuery" /> components.
    /// </summary>
    public class ApiTrialMetricsQuery : IApiTrailMetricsQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiTrialMetricsQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public ApiTrialMetricsQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IApiTrailMetricsQuery Members

        async Task<IEnumerable<ApiTrailMetricsRecord>> IApiTrailMetricsQuery.QueryMethodCallsCounts(Guid accessId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateAPIMethodMetricsForUser] @accessId=@p0";

            return await this.context.Database.SqlQuery<ApiTrailMetricsRecord>(Sql, accessId)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }

        async Task<IEnumerable<ApiTrailMetricsRecord>> IApiTrailMetricsQuery.QueryOperationCounts(Guid accessId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateAPIOperationMetricsForUser] @accessId=@p0";

            return await this.context.Database.SqlQuery<ApiTrailMetricsRecord>(Sql, accessId)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);
        }

        #endregion
        
    }
}