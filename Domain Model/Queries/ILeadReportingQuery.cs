using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    public interface ILeadReportingQuery
    {
        /// <summary>
        /// Returns lead metrics for use in reporting
        /// </summary>
        Task<IList<LeadMetric>> CalculateLeadMetrics(DateTime startdate, DateTime enddate, Guid applicationId, CancellationToken cancellation);
    }
}
