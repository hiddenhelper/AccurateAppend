#pragma warning disable SA1402 // File may only contain a single class
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Represents a component that can interact with a particular rate card that allows correct <see cref="ProductLine"/> to be built
    /// for a <see cref="ProcessingReport"/> that resulted from a <see cref="ManifestBuilder"/> definition, if available.
    /// </summary>
    /// <remarks>goal: to create/update an order from a processing report while using the pricing info from the rate cards/manifest</remarks>
    [ContractClass(typeof(IOrderCalculationServiceContracts))]
    public interface IOrderCalculationService
    {
        /// <summary>
        /// Populates the provided <paramref name="order"/> with the supplied <paramref name="manifest"/> and <paramref name="processingReport"/> information.
        /// </summary>
        /// <remarks>
        /// This overload will populate the order based on the distinct intersection of the <see cref="Core.Definitions.DataServiceOperation"/> values in <paramref name="manifest"/> and <paramref name="processingReport"/>.
        /// Any existing <see cref="ProductLine"/> values in the <see cref="BillableOrder"/> will be removed.
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to populate.</param>
        /// <param name="costService">The <see cref="ICostService"/> providing the appropriate cost structures for the <paramref name="order"/>.</param>
        /// <param name="manifest">The manifest to populate the billing from.</param>
        /// <param name="processingReport">The processing report to populate from.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task FillFromRateCard(BillableOrder order, ICostService costService, ManifestBuilder manifest, ProcessingReport processingReport, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Populates the provided <paramref name="order"/> just from the supplied <paramref name="processingReport"/> information.
        /// </summary>
        /// <remarks>
        /// This overload will populate the order based on only the <see cref="Core.Definitions.DataServiceOperation"/> values in <paramref name="processingReport"/>.
        /// Any existing <see cref="ProductLine"/> values in the <see cref="BillableOrder"/> will be removed.
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to populate.</param>
        /// <param name="costService">The <see cref="ICostService"/> providing the appropriate cost structures for the <paramref name="order"/>.</param>
        /// <param name="processingReport">The processing report to populate from.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task FillFromRateCard(BillableOrder order, ICostService costService, ProcessingReport processingReport, CancellationToken cancellation = default(CancellationToken));
    }

    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IOrderCalculationService))]
    internal abstract class IOrderCalculationServiceContracts : IOrderCalculationService
    {
        Task IOrderCalculationService.FillFromRateCard(BillableOrder order, ICostService costService, ManifestBuilder manifest, ProcessingReport processingReport, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(order != null, nameof(order));
            Contract.Requires<InvalidOperationException>(order.Status == OrderStatus.Open);
            Contract.Requires<ArgumentNullException>(costService != null, nameof(costService));
            Contract.Requires<ArgumentNullException>(manifest != null, nameof(manifest));
            Contract.Requires<ArgumentNullException>(processingReport != null, nameof(processingReport));

            return default(Task);
        }

        Task IOrderCalculationService.FillFromRateCard(BillableOrder order, ICostService costService, ProcessingReport processingReport, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(order != null, nameof(order));
            Contract.Requires<InvalidOperationException>(order.Status == OrderStatus.Open);
            Contract.Requires<ArgumentNullException>(costService != null, nameof(costService));
            Contract.Requires<ArgumentNullException>(processingReport != null, nameof(processingReport));

            return default(Task);
        }
    }
    // ReSharper restore InconsistentNaming
}
