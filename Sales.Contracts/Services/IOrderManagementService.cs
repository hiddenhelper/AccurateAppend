using System;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales.Contracts.ViewModels;

namespace AccurateAppend.Sales.Contracts.Services
{
    /// <summary>
    /// High level API for sales order content management.
    /// </summary>
    /// <remarks>
    /// Provides an abstraction that allows the simplified access, use, and logic required to create and edit
    /// <see cref="BillableOrder"/>. Order processing workflow is encapsulated
    /// in another component allowing this to focus on just content management.
    /// </remarks>
    public interface IOrderManagementService
    {
        /// <summary>
        /// Stores the updated <see cref="OrderDetail"/> content.
        /// </summary>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task Update(OrderDetail model, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Locates the indicated editable order model by identifier, if exists.
        /// </summary>
        /// <param name="orderId">The identifier of the editable order to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="OrderDetail"/> information; Otherwise null.</returns>
        Task<OrderDetail> FindOrder(Int32 orderId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Locates the indicated editable order model by public key, if exists.
        /// </summary>
        /// <param name="publicKey">The public key of the editable order to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="OrderDetail"/> information; Otherwise null.</returns>
        Task<OrderDetail> FindOrder(Guid publicKey, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Stores the updated <see cref="BillableOrder"/> content.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as a temporary workaround for the Update Order From Job use case to put the updated order values.</note>
        /// </remarks>
        /// <param name="orderId">The identifier of the <see cref="BillableOrder"/> to update.</param>
        /// <param name="manifest">The <see cref="ManifestBuilder"/> describing the job instructions.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task Update(Int32 orderId, ManifestBuilder manifest, ProcessingReport report, CancellationToken cancellation = default(CancellationToken));
        
        /// <summary>
        /// Stores the updated <see cref="BillableOrder"/> content.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as a temporary workaround for the Update Order From Job use case to put the updated order values.</note>
        /// </remarks>
        /// <param name="orderId">The identifier of the <see cref="BillableOrder"/> to update.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task Update(Int32 orderId, ProcessingReport report, CancellationToken cancellation = default(CancellationToken));
    }
}
