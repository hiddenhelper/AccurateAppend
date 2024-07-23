using System;
using System.Linq;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Cost service for NationBuilder products. Operates by switching the rate card to <see cref="Cost.NationBuilderCategory"/>
    /// and restricts costing products to <see cref="PublicProduct"/> and <see cref="DataServiceOperation.EMAIL_VERIFICATION"/> only.
    /// </summary>
    public class NationBuilderCostService : ContextBasedCostService
    {
        #region Fields

        /// <summary>
        /// While the <see cref="PublicProduct"/> values support the listing of products available to order/execute,
        /// the processing reports on ACCUSEND_W_INPUT_EMAIL_VERIFICATION will produce additional operations to
        /// distinguish the input validation and therefore required to be added to this type.
        /// </summary>
        private static readonly String[] Products = new[]
            {
                PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION,
                PublicProduct.EMAIL_VER_DELIVERABLE,
                PublicProduct.PHONE_PREM,
                PublicProduct.DEMOGRAHICS,
                PublicProduct.NCOA48,
                PublicProduct.CASS,
                PublicProduct.PHONE,
#if DEBUG
                PublicProduct.EMAIL_BASIC_REV,
                PublicProduct.PHONE_REV_PREM
#endif
        }
            .Select(p => p.ToString())
            .Concat(new[] {DataServiceOperation.EMAIL_VERIFICATION.ToString()})
            .ToArray();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderCostService"/> class.
        /// </summary>
        /// <remarks>
        /// Used the default <see cref="PricingModel"/> on all created <see cref="RateCard"/> instances.
        /// </remarks>
        /// <param name="context">The <see cref="DefaultContext"/> to be used by the cost service.</param>
        public NationBuilderCostService(DefaultContext context) : base(context)
        {
            this.RateCard = Cost.NationBuilderCategory;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// While the subset of <see cref="PublicProduct"/> values we support. As the processing reports on
        /// ACCUSEND_W_INPUT_EMAIL_VERIFICATION product will produce additional operations to distinguish the
        /// input email validation, therefore EMAIL_VERIFICATION is also required to be supported by this type.
        /// </summary>
        public sealed override String[] SupportedProducts => Products;

        /// <inheritdoc />
        protected sealed override String RateCard { get; set; }

        /// <inheritdoc />
        protected override IQueryable<Cost> CreateBaseQuery(String productKey)
        {
            if (!SupportedProducts.Contains(productKey, StringComparer.OrdinalIgnoreCase)) throw new NotSupportedException($"{productKey} is not a supported product with {Cost.NationBuilderCategory} rate cards");

            return base.CreateBaseQuery(productKey);
        }

        #endregion
    }
}
