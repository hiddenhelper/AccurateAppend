using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IMrrMetricQuery"/> component.
    /// </summary>
    public class MrrMetricQuery : IMrrMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MrrMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public MrrMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IMrrMetricQuery Members

        async Task<IEnumerable<MrrMetricRecord>> IMrrMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateMrrMetrics] @ApplicationId=@p0";

            return await this.context.Database.SqlQuery<MrrMetricRecord>(Sql, applicationId)
                    .ToArrayAsync(cancellation)
                    .ConfigureAwait(false);
        }
      
        #endregion
    }
}