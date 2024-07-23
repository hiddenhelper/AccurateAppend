using System;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Indicates a <see cref="PricingModel"/> that is used for a single
    /// <see cref="Product"/> for a defined <see cref="Cost.IsSystem">non-system</see>
    /// cost structure.
    /// </summary>
    public class CostPricingModel
    {
        /// <summary>
        /// Not intended for direct creation.
        /// </summary>
        protected CostPricingModel()
        {
        }

        /// <summary>
        /// Gets the <see cref="Product"/> the pricing model is for.
        /// </summary>
        public virtual Product ForProduct { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Product.Id"/> the pricing model is for.
        /// </summary>
        protected internal virtual Int32 ProductId { get; protected set; }

        /// <summary>
        /// Gets the <see cref="PricingModel"/> that should be used.
        /// </summary>
        public virtual PricingModel Model { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Cost.Category"/> the pricing model is for.
        /// </summary>
        public virtual String Category { get; protected set; }
    }
}