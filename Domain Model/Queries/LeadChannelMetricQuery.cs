using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="ILeadChannelMetricQuery"/> component.
    /// </summary>
    public class LeadChannelMetricQuery : ILeadChannelMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadChannelMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public LeadChannelMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region ILeadChannelMetricQuery Members

        async Task<IEnumerable<LeadChannelMetricRecord>> ILeadChannelMetricQuery.Query(Guid applicationId, int leadSource, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateLeadChannelMetrics] @ApplicationId=@p0, @LeadSource=@p1";

            return await this.context.Database.SqlQuery<LeadChannelMetricRecord>(Sql, applicationId, leadSource)
                    .ToArrayAsync(cancellation)
                    .ConfigureAwait(false);
        }
      
        #endregion
    }
}