using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    public class LeadReportingQuery : ILeadReportingQuery
    {
        #region Fields

        private readonly ReadContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new isntance of the <see cref="LeadReportingQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ReadContext"/> data access component.</param>
        public LeadReportingQuery(ReadContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region ILeadReportingQuery Members
        
        /// <summary>
        /// Returns lead metrics for use in reporting
        /// </summary>
        public virtual Task<IList<LeadMetric>> CalculateLeadMetrics(DateTime startdate, DateTime enddate, Guid applicationId, CancellationToken cancellation)
        {
            const String Sql = "exec [accounts].[CalculateLeadMetrics] @start=@p0, @end=@p1, @applicationId=@p2";

            return this.context.Database.SqlQuery<LeadMetric>(Sql, startdate, enddate, applicationId)
                        .ToListAsync(cancellation)
                        .ContinueWith(t => (IList<LeadMetric>)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion
    }
}
