#pragma warning disable SA1402 // File may only contain a single class
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Represents a component that can interact with a particular rate card that allows a correct <see cref="ProductLine"/> to be built
    /// and for product rate card acquisition.
    /// </summary>
    [ContractClass(typeof(ICostServiceContracts))]
    public interface ICostService
    {
        /// <summary>
        /// Every <see cref="ICostService"/> may support just a subset of products. This property can contain the limited set of
        /// product keys that are supported for this component. Implementors that do not restrict use to specific products can
        /// instead return an empty array to indicate there's no restrictions.
        /// </summary>
        String[] SupportedProducts { get; }

        /// <summary>
        /// Builds the correct cost structure for the indicated <paramref name="product"/>
        /// </summary>
        /// <param name="product">The <see cref="DataServiceOperation"/> to get a cost off the rate card.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>A populated <see cref="RateCard"/> for the <paramref name="product"/>.</returns>
        Task<RateCard> CreateRateCard(DataServiceOperation product, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Builds the correct cost structure for the indicated <paramref name="productKey"/>
        /// </summary>
        /// <param name="productKey">The <see cref="Product.Key"/> value to get a cost off the rate card.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>A populated <see cref="RateCard"/> for the <paramref name="productKey"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// <para>The supplied <paramref name="productKey"/> value is not supported</para>
        /// <para>-OR-</para>
        /// <para>This overload is not supported</para>
        /// </exception>
        Task<RateCard> CreateRateCard(String productKey, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Determines the cost value for the indicated <paramref name="product" /> off the rate card using a specific pricing model.
        /// </summary>
        /// <param name="product">The <see cref="T:AccurateAppend.Core.Definitions.DataServiceOperation" /> to get a cost off the rate card.</param>
        /// <param name="pricing">Indicated the type of <see cref="T:AccurateAppend.Core.Definitions.PricingModel" /> to be estimated.</param>
        /// <param name="itemCount">The number of items to base off the rate card.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The cost of the product for the indicated matches off the rate card.</returns>
        Task<Decimal> Estimate(DataServiceOperation product, PricingModel pricing, Int32 itemCount, CancellationToken cancellation = default(CancellationToken));
    }

    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(ICostService))]
    internal abstract class ICostServiceContracts : ICostService
    {
        String[] ICostService.SupportedProducts
        {
            get
            {
                Contract.Ensures(Contract.Result<String[]>() != null);
                return default(String[]);
            }
        }

        Task<RateCard> ICostService.CreateRateCard(DataServiceOperation product, CancellationToken cancellation)
        {
            return default(Task<RateCard>);
        }

        Task<RateCard> ICostService.CreateRateCard(String productKey, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(productKey), nameof(productKey));
            Contract.EndContractBlock();

            return default(Task<RateCard>);
        }

        Task<Decimal> ICostService.Estimate(DataServiceOperation product, PricingModel pricing, Int32 itemCount, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentOutOfRangeException>(product != DataServiceOperation.UNKNOWN, nameof(product));
            Contract.Requires<ArgumentOutOfRangeException>(itemCount >= 0, nameof(itemCount));

            return default(Task<Decimal>);
        }
    }
    // ReSharper restore InconsistentNaming
}
