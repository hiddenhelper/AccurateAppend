using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales.DataAccess;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Pricing
{
    /// <summary>
    /// Controller for providing NationBuilder list product processing information.
    /// </summary>
    [Authorize()]
    public class Controller : PricingControllerBase<Sales.NationBuilderCart>
    {
        #region Fields

        private readonly Sales.ICostService costService;
        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing data access for this controller.</param>
        /// <param name="costService">The <see cref="Sales.ICostService"/> rate card.</param>
        /// <param name="productHelper">The <see cref="IProductHelpService"/> used to provide sales support content.</param>
        public Controller(DefaultContext context, Sales.ICostService costService, IProductHelpService productHelper)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (costService == null) throw new ArgumentNullException(nameof(costService));
            if (productHelper == null) throw new ArgumentNullException(nameof(productHelper));
            Contract.EndContractBlock();

            this.context = context;
            this.costService = costService;
            this.ProductHelper = productHelper;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns collection of available products.
        /// </summary>
        /// <remarks>Override to just set caching. Uses base behavior otherwise.</remarks>
        [OutputCache(Duration = 10 * 60, VaryByParam = "*")]
        public override Task<ActionResult> GetProducts(Guid cartId, CancellationToken cancellation)
        {
            return base.GetProducts(cartId, cancellation);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override PublicProduct[] SupportedProducts =>
            this.costService.SupportedProducts.Any()
                ? this.costService.SupportedProducts
                    .Where(p => Enum.TryParse<PublicProduct>(p, out _))
                    .Select(EnumExtensions.Parse<PublicProduct>)
                    .ToArray()
                : SupportedProductHelper.Products;

        /// <inheritdoc />
        protected override IProductHelpService ProductHelper { get; }

        /// <inheritdoc />
        protected override Task<Sales.ICostService> CreateCostService(CancellationToken cancellation)
        {
            return Task.FromResult(this.costService);
        }

        /// <inheritdoc />
        protected override Decimal GetEstimatedMatchRate(PublicProduct supportedProduct)
        {
            // These are the DEFAULT rates. Comments reflect reality of Processing Report Match Rates 2-12-18
            switch (supportedProduct)
            {
                case PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION:
                    return 0.45M; // 28%
                case PublicProduct.EMAIL_VER_DELIVERABLE:
                    return 0.97M; // 89%
                case PublicProduct.CASS:
                    return 0.95M; // 97%
                case PublicProduct.DEMOGRAHICS:
                    return 0.4M; // 42%
                case PublicProduct.PHONE:
                    return 0.95M; // 70% - this is based on an avg ratio of inputs
                case PublicProduct.NCOA48:
                    return 0.15M; // 54%
                case PublicProduct.PHONE_PREM:
                    return 0.5M; // 58%
                default:
                    return 0;
            }
        }

        /// <inheritdoc />
        protected override async Task<Sales.NationBuilderCart> GetCart(Guid cartId, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var cart = await this.context.SetOf<Sales.Cart>()
                    .ForInteractiveUser()
                    .ForNationBuilder(cartId)
                    .Where(c => c.IsActive)
                    .SingleOrDefaultAsync(cancellation)
                    .ConfigureAwait(false);

                return cart;
            }
        }

        #endregion
    }
}