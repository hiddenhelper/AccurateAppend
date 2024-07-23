using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A constrained set of <see cref="Cost"/> instances used as part of a unified rate card for a <see cref="Product"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ForProduct) + "." + nameof(Product.Key) + "}:{" + nameof(PricingModel) + "}")]
    public class RateCard
    {
        #region Fields

        private readonly Cost[] costStructure;
        private readonly Product product;
        private readonly PricingModel pricingModel;

        #endregion

        #region Constructors

        private RateCard(Product product, PricingModel pricingModel)
        {
            this.costStructure = new Cost[0];
            this.product = product;
            this.pricingModel = pricingModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateCard"/> class.
        /// </summary>
        /// <param name="costStructure">The set of <see cref="Cost"/> structures to base this rate card from for a single <see cref="Product"/>.</param>
        /// <param name="pricingModel">The <see cref="PricingModel"/> that this instance should be used with.</param>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="costStructure"/> was empty</para>
        /// <para>-OR-</para>
        /// <para><paramref name="costStructure"/> contains more than single <see cref="Cost.Product"/> value</para>
        /// </exception>
        public RateCard(IEnumerable<Cost> costStructure, PricingModel pricingModel)
        {
            if (costStructure == null) throw new ArgumentNullException(nameof(costStructure));
            Contract.EndContractBlock();

            costStructure = costStructure.Where(c => c != null).ToArray();
            if (!costStructure.Any()) throw new InvalidOperationException("The provided cost structure data was empty");
            if (costStructure.Select(c => c.Product).Distinct().Count() != 1) throw new InvalidOperationException($"The supplied {nameof(Cost)} structure data contained more than a single {nameof(Product)} type. {nameof(RateCard)} instances are scoped to a single {nameof(Product)} each.");

            this.costStructure = costStructure.ToArray();
            this.product = this.costStructure.First().Product;
            this.pricingModel = pricingModel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Product"/> the rate card is for.
        /// </summary>
        /// <value>The <see cref="Product"/> the rate card is for.</value>
        public Product ForProduct
        {
            get
            {
                Contract.Ensures(Contract.Result<Product>() != null);

                return this.product;
            }
        }

        /// <summary>
        /// Indicates the appropriate <see cref="PricingModel"/> that should be used with this instance.
        /// </summary>
        /// <remarks>
        /// The <see cref="FindCost"/> method leverages a supplied number (a count) to determine which
        /// <see cref="Cost"/> that value falls within. However the caller is required to understand
        /// which value to aggregate to acquire that number but there's never a universal case possible
        /// as the potential source of that information is context specific. Therefore we can use this
        /// as a "hint" to the caller to determine what to aggregate.
        /// </remarks>
        // ReSharper disable ConvertToAutoProperty
        public PricingModel PricingModel => this.pricingModel;
        // ReSharper restore ConvertToAutoProperty

        #endregion

        #region Methods

        /// <summary>
        /// Performs a query for the appropriate <see cref="Cost"/> that can be used for the provided <see cref="Product"/> and item count.
        /// </summary>
        /// <param name="itemCount">The item count to match to.</param>
        /// <returns>A <see cref="Cost"/> structure appropriate for the <see cref="Product"/>.</returns>
        public virtual Cost FindCost(Int32 itemCount)
        {
            Contract.Ensures(Contract.Result<Cost>() != null);
            Contract.EndContractBlock();

            var match = this.costStructure.FirstOrDefault(c => c.CanHandle(this.ForProduct, itemCount));

            match = match ?? Cost.DefaultCost(this.ForProduct);

            return match;
        }

        /// <summary>
        /// Gets an empty <see cref="RateCard"/> instance. That is, a <seealso cref="RateCard"/> that does not
        /// have any pricing information and provides estimations of 0.
        /// </summary>
        public static RateCard Empty(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            Contract.Ensures(Contract.Result<RateCard>() != null);
            Contract.EndContractBlock();

            return new RateCard(product, default(PricingModel));
        }

        #endregion

        #region Overrides

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode()
        {
            return this.ForProduct.GetHashCode();
        }

        #endregion
    }
}
