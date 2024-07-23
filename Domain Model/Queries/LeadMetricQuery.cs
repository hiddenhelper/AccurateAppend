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
    /// Default implementation of the <see cref="T:DomainModel.Queries.ILeadMetricQuery" /> component.
    /// </summary>
    public class LeadMetricQuery : ILeadMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public LeadMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region ILeadMetricQuery Members

        Task<IEnumerable<LeadMetricRecord>> ILeadMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateLeadMetrics] @ApplicationId=@p0";

            return this.context.Database.SqlQuery<LeadMetricRecord>(Sql, applicationId)
                .ToArrayAsync(cancellation)
                .ContinueWith(t => (IEnumerable<LeadMetricRecord>) t.Result,
                    TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion
    }
}