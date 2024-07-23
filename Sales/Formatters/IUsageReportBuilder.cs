using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace AccurateAppend.Sales.Formatters
{
    /// <summary>
    /// Represents a component that can produce a fully formed report content for usage over a given period.
    /// </summary>
    /// <remarks>
    /// Generally an implementation will be human readable but that is not required. The generated content can
    /// be stored or transmitted as appropriate for the use case.
    /// </remarks>
    [ContractClass(typeof(IUsageReportBuilderContracts))]
    public interface IUsageReportBuilder
    {
        /// <summary>
        /// Produces the generated report content for this type of builder.
        /// </summary>
        /// <param name="userId">The identifier of the user to produce the report for.</param>
        /// <param name="during">The starting and ending (inclusive) that the report content should contain (at Date granularity).</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The fully formatted usage report.</returns>
        Task<String> GenerateUsageReport(Guid userId, DateSpan during, CancellationToken cancellation = default(CancellationToken));
    }

    [ContractClassFor(typeof(IUsageReportBuilder))]
    internal abstract class IUsageReportBuilderContracts : IUsageReportBuilder
    {
        Task<String> IUsageReportBuilder.GenerateUsageReport(Guid userId, DateSpan during, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(during != null, nameof(during));
            return default(Task<String>);
        }
    }
}
