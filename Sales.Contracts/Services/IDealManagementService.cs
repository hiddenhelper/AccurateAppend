using System;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.Contracts.ViewModels;

namespace AccurateAppend.Sales.Contracts.Services
{
    /// <summary>
    /// High level API for deal content management.
    /// </summary>
    /// <remarks>
    /// Provides an abstraction that allows the simplified access, use, and logic required to create and edit
    /// <see cref="DealBinder"/> logic. Order processing workflow is encapsulated in another component allowing
    /// this to focus on just content management.
    /// </remarks>
    public interface IDealManagementService
    {
        #region Deals

        /// <summary>
        /// Creates a <see cref="DealModel"/> for the indicated <see cref="ClientRef"/> using the default rules.
        /// </summary>
        /// <param name="userId">The identifier of the user to create the <see cref="DealModel"/> for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>A new <see cref="DealModel"/> that can be edited or updated to the correct values.</returns>
        Task<DealModel> Default(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Stores a new <see cref="DealModel"/> to the system.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealCreatedEvent"/>.
        /// </remarks>
        /// <param name="model">The <see cref="DealModel"/> that needs to be stored.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The identifier of the new deal.</returns>
        Task<Int32> Create(DealModel model, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Stores a new <see cref="DealBinder"/> to the system.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as an api for the New Deal From Job use case to put the deal and order data in one go.</note>
        /// Publishes the <see cref="DealCreatedEvent"/>.
        /// </remarks>
        /// <param name="publicKey">The public key of the new deal.</param>
        /// <param name="model">The <see cref="DealModel"/> that needs to be stored.</param>
        /// <param name="manifest">The <see cref="ManifestBuilder"/> describing the job instructions.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The identifier of the new deal.</returns>
        Task<Int32> Create(Guid publicKey, DealModel model, ManifestBuilder manifest, ProcessingReport report, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Locates the indicated editable deal model, if exists.
        /// </summary>
        /// <param name="dealId">The identifier of the editable deal to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="DealModel"/> information; Otherwise null.</returns>
        Task<DealModel> FindDeal(Int32 dealId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Stores the updated <see cref="DealModel"/> content.
        /// </summary>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        Task Update(DealModel model, CancellationToken cancellation = default(CancellationToken));

        #endregion
    }
}
