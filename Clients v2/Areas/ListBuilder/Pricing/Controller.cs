using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Pricing
{
    /// <summary>
    /// Controller for providing ListBuilder based file product processing information.
    /// </summary>
    [Authorize()]
    public class Controller : PricingControllerBase<Cart>
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> component providing entity access.</param>
        /// <param name="productHelper">The <see cref="IProductHelpService"/> used to provide sales support content.</param>
        public Controller(DefaultContext context, IProductHelpService productHelper)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (productHelper == null) throw new ArgumentNullException(nameof(productHelper));
            Contract.EndContractBlock();

            this.context = context;
            this.ProductHelper = productHelper;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the set of <see cref="PublicProduct"/> available in this area.
        /// </summary>
        protected override PublicProduct[] SupportedProducts => SupportedProductHelper.Products.Except(new[] {PublicProduct.PHONE_MOB, PublicProduct.EMAIL_VER_DELIVERABLE, PublicProduct.ACCUSEND_W_INPUT_EMAIL_VERIFICATION}).ToArray();

        /// <summary>
        /// Gets the <see cref="IProductHelpService"/> that is used to determine marketing spiel for clients ordering services.
        /// </summary>
        protected override IProductHelpService ProductHelper { get; }

        /// <summary>
        /// Creates a new instance of a <see cref="ICostService"/>. Since this area is priced off the client rates
        /// cards, we must understand the interactive client and have to dynamically create a new service at runtime.
        /// </summary>
        /// <returns>The <see cref="ICostService"/> that should be used for this controller when calculating order prices and rates.</returns>
        protected override Task<ICostService> CreateCostService(CancellationToken cancellation)
        {
            ICostService costService = new ContextBasedCostService(this.context, PricingModel.Match);

            return Task.FromResult(costService);
        }

        /// <summary>
        /// Returns collection of available products.
        /// </summary>
        /// <remarks>Override to just set caching. Uses base behavior otherwise.</remarks>
        [OutputCache(Duration = 10 * 60, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public override Task<ActionResult> GetProducts(Guid cartId, CancellationToken cancellation)
        {
            return base.GetProducts(cartId, cancellation);
        }

        /// <summary>
        /// We assume that client supplied CSV files are pretty much filled and complete.
        /// </summary>
        /// <param name="supportedProduct">The product to calculate an estimate for. - not used</param>
        /// <returns>The estimated rate, as a percentage, for matches.</returns>
        protected override Decimal GetEstimatedMatchRate(PublicProduct supportedProduct)
        {
            switch (supportedProduct)
            {
                case PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION:
                    return 0.40M;
                case PublicProduct.CASS:
                    return 0.95M;
                case PublicProduct.DEMOGRAHICS:
                    return 0.4M;
                case PublicProduct.PHONE:
                    return 0.95M;
                case PublicProduct.NCOA48:
                    return 0.05M;
                case PublicProduct.PHONE_PREM:
                    return 0.5M;
                case PublicProduct.PHONE_MOB:
                    return 0.4M;
                default:
                    return 0;
            }
        }

        /// <inheritdoc />
        protected override async Task<Cart> GetCart(Guid cartId, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var cart = await this.context.SetOf<Cart>()
                    .ForListBuilder(cartId)
                    .Where(c => c.IsActive)
                    .SingleOrDefaultAsync(cancellation)
                    .ConfigureAwait(false);

                return cart;
            }
        }

        #endregion
    }
}