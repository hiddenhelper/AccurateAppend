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
    /// Default implementation of the <see cref="IAgentMetricQuery"/> component.
    /// </summary>
    public class AgentMetricQuery : IAgentMetricQuery
    {
        #region Fields

        private readonly DbContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MrrMetricQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> providing access to the data store.</param>
        public AgentMetricQuery(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IAgentMetricQuery Members

        async Task<IEnumerable<AgentMetricRecord>> IAgentMetricQuery.Query(Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "EXEC [sales].[CalculateAgentMetrics] @ApplicationId=@p0";

            return await this.context.Database.SqlQuery<AgentMetricRecord>(Sql, applicationId)
                    .ToArrayAsync(cancellation)
                    .ConfigureAwait(false);
        }
      
        #endregion
    }
}