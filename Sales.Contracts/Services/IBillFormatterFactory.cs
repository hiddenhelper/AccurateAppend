using System;
using System.Threading;
using System.Threading.Tasks;

namespace AccurateAppend.Sales.Contracts.Services
{
    /// <summary>
    /// Provides an abstraction to hide the concrete formatter instances used in this
    /// application as well as their specific initialization and construction/usage 
    /// logic. Instead of the application controller itself understanding and being
    /// responsible for this logic, it simply defers to an implementation of this type
    /// to create bills from.
    /// </summary>
    /// <remarks>
    /// This abstraction is be necessity an application scenario specific one and therefore
    /// cannot really be generic not can it be defined in the general purpose code.
    /// </remarks>
    public interface IBillFormatterFactory
    {
        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate subscription based bills.
        /// </summary>
        /// <param name="userId">The identifier of the user to create a subscription formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for subscription billing.</returns>
        Task<IBillFormatter> ForSubscription(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate usage based bills.
        /// </summary>
        /// <param name="userId">The identifier of the user to create a usage formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for usage billing.</returns>
        Task<IBillFormatter> ForUsage(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate refund based bills.
        /// </summary>
        /// <param name="userId">The identifier of the user to create a refund formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for refund billing.</returns>
        Task<IBillFormatter> ForRefund(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate NationBuilder order based bills.
        /// </summary>
        /// <param name="publicKey">The identifier of the request to create a NationBuilder formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for NationBuilder billing.</returns>
        Task<IBillFormatter> ForNationBuilder(Guid publicKey, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate match rate based bills.
        /// </summary>
        /// <param name="userId">The identifier of the user to create a standard formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for standard billing.</returns>
        Task<IBillFormatter> ByMatchLevel(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate match type based bills.
        /// </summary>
        /// <param name="userId">The identifier of the user to create a standard formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for standard billing.</returns>
        Task<IBillFormatter> ByMatchType(Guid userId, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Provides access to a <see cref="IBillFormatter"/> used to generate public job based bills.
        /// </summary>
        /// <param name="publicKey">The identifier of the request to create a public job formatter for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="IBillFormatter"/> for public job billing.</returns>
        Task<IBillFormatter> ForPublic(Guid publicKey, CancellationToken cancellation = default(CancellationToken));
    }
}