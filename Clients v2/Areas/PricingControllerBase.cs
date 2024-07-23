using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Sales;
using DomainModel.ActionResults;
using EventLogger;
using Newtonsoft.Json;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Controller for managing and defining the API for various product and price estimation controllers.
    /// There's expected to be one concrete implementation per ordering area (NationBuilder, Batch Book, CSV, etc).
    /// </summary>
    [ContractClass(typeof(PricingControllerBaseContracts))]
    public abstract class PricingControllerBase<T> : Controller where T : Cart
    {
        #region Properties

        /// <summary>
        /// Gets the set of <see cref="PublicProduct"/> available in this area.
        /// </summary>
        protected virtual PublicProduct[] SupportedProducts
        {
            get
            {
                Contract.Ensures(Contract.Result<PublicProduct[]>() != null);
                return SupportedProductHelper.Products;
            }
        }

        /// <summary>
        /// Gets the <see cref="IProductHelpService"/> that is used to determine marketing spiel for clients ordering services.
        /// </summary>
        protected abstract IProductHelpService ProductHelper { get; }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns collection of available products.
        /// </summary>
        /// <remarks>
        /// It is anticipated that the default implementation of this action will suffice for most areas. However
        /// overrides to apply an MVC filter and then delegating to the base implementation may be desired.
        /// 
        /// The action MUST return the following JSON entity as a result array.
        /// <list type="table">
        /// <listheader>
        ///  <term>field</term>
        ///  <description>type</description>
        /// </listheader>
        /// <item>
        ///  <term>OperationName</term>
        ///  <description>string. The <see cref="PublicProduct"/> value.</description>
        /// </item>
        /// <item>
        ///  <term>Title</term>
        ///  <description>string.</description>
        /// </item>
        /// <item>
        ///  <term>Description</term>
        ///  <description>string.</description>
        /// </item>
        /// <item>
        ///  <term>EstMatchRate</term>
        ///  <description>decimal. Percentage of input records that will match (expressed 0 to 1).</description>
        /// </item>
        /// <item>
        ///  <term>SuitableRecords</term>
        ///  <description>integer. Number of records deemed to meet the requirements of the product.</description>
        /// </item>
        /// <item>
        ///  <term>Cost</term>
        ///  <description>decimal. The PPU for each match.</description>
        /// </item>
        /// <item>
        ///  <term>Category</term>
        ///  <description>string. The <see cref="SupportedProductCategory"/> value.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public virtual async Task<ActionResult> GetProducts(Guid cartId, CancellationToken cancellation)
        {
            using (new Correlation(cartId))
            {
                try
                {
                    var availableProducts = this.SupportedProducts;
                    var costService = await this.CreateCostService(cancellation);

                    var cart = await this.GetCart(cartId, cancellation);
                    var recordCount = cart?.RecordCount ?? 0;

                    var productMatrix = availableProducts.Select(p => new Tuple<PublicProduct, Int32>(p, recordCount)).ToArray();
                    var costMatrix = new List<Tuple<PublicProduct, Decimal>>();

                    foreach (var analysis in cart.AnalyzedProducts().Where(a => a.Item2 > 0))
                    {
                        var replaceAt = productMatrix.FindIndex(p => p.Item1 == analysis.Item1);
                        if (replaceAt == -1) continue;

                        productMatrix[replaceAt] = analysis;
                    }

                    foreach (var m in productMatrix)
                    {
                        var cost = await this.GetCost(costService, m.Item1, m.Item2, cancellation);
                        costMatrix.Add(new Tuple<PublicProduct, Decimal>(m.Item1, cost));
                    }

                    var jsonNetResult = new JsonNetResult()
                    {
                        Data = productMatrix.Select(m =>
                            new
                            {
                                OperationName = m.Item1.ToString(),
                                Title = this.ProductHelper.GetTitle(m.Item1),
                                Description = this.ProductHelper.GetDescription(m.Item1),
                                EstMatchRate = this.GetEstimatedMatchRate(m.Item1),
                                SuitableRecords = productMatrix.First(i => i.Item1 == m.Item1).Item2,
                                Cost = costMatrix.First(i => i.Item1 == m.Item1).Item2,
                                Category = this.ProductHelper.GetCategory(m.Item1)
                            }).ToArray()
                    };
                    jsonNetResult.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
                    return jsonNetResult;
                }
                catch (Exception exception)
                {
                    Logger.LogEvent(exception, Severity.High);
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns product help text describing the product and how it is used.
        /// </summary>
        /// <remarks>
        /// It is anticipated that the default implementation of this action will suffice for most areas. However
        /// overrides to apply an MVC filter and then delegating to the base implementation may be desired.
        /// </remarks>
        [OutputCache(Duration = 5 * 60, Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult GetHelpText(PublicProduct product)
        {
            try
            {
                var content = this.ProductHelper.GetHelpText(product);
                return this.Json(new { HttpStatus = HttpStatusCode.OK, Text = content }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, TraceEventType.Warning, Severity.High, Application.Clients.ToString(), this.Request.UserHostAddress, $"GetHelpText, {product} {this.User.Identity.Name}");
            }

            return this.Json(new { HttpStatus = HttpStatusCode.InternalServerError, Message = $"Product description for {product} is erroring." }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Used to access the indicated cart.
        /// </summary>
        /// <param name="cartId">The identifier of the cart to acquire record counts for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the cancellation of an asynchronous operation.</param>
        /// <returns>If found, the cart; Otherwise null.</returns>
        protected abstract Task<T> GetCart(Guid cartId, CancellationToken cancellation);

        /// <summary>
        /// Used to create the concrete implementation of the <see cref="ICostService"/> that's used to determine
        /// match rates, prices, and other order related information. It is called once as part of the <see cref="GetProducts"/>
        /// action method and cached thereafter. However subclasses may change this behavior.
        /// </summary>
        /// <remarks>
        /// As this has a widely differing set of creation semantics (based on dynamic calling user all the way to simple
        /// constructor parameters or even static types) this is required to be implemented by subclasses. If the value
        /// is static or available at constructor time, simply returning the value as part of <see cref="Task.FromResult{T}(T)"/>
        /// will suffice.
        /// </remarks>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the cancellation of an asynchronous operation.</param>
        /// <returns>The <see cref="ICostService"/> that should be used for this controller when calculating order prices and rates.</returns>
        protected abstract Task<ICostService> CreateCostService(CancellationToken cancellation);

        /// <summary>
        /// Using a supplied <see cref="ICostService"/> (as acquired from <see cref="CreateCostService"/>) and <paramref name="product"/>
        /// value, calculate the cost for the supplied <paramref name="recordCount"/>.
        /// </summary>
        /// <remarks>
        /// Estimated match count is based on the highest integer for the product of <see cref="GetEstimatedMatchRate"/> and <paramref name="recordCount"/>.
        /// Using the <see cref="RateCard"/> created by the <paramref name="costService"/>, the <see cref="RateCard.PricingModel"/> and estimated
        /// match count is supplied to the <paramref name="costService"/> to price.
        /// </remarks>
        /// <param name="costService">The <see cref="ICostService"/> that will calculate the costs and rates for the indicated product.</param>
        /// <param name="product">The product that is to be calculated. This value MUST be supplied from the <see cref="SupportedProducts"/> implementation.</param>
        /// <param name="recordCount">The number of records to calculate a cost for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the cancellation of an asynchronous operation.</param>
        /// <returns>The cost, in currency value (assumed USD). This value will always be positive.</returns>
        protected virtual async Task<Decimal> GetCost(ICostService costService, PublicProduct product, Int32 recordCount, CancellationToken cancellation)
        {
            if (costService == null) throw new ArgumentNullException(nameof(costService));
            if (recordCount < 0) throw new ArgumentOutOfRangeException(nameof(recordCount), recordCount, $"{nameof(recordCount)} must be at least 0");
            Contract.Ensures(Contract.Result<Decimal>() >= 0);
            Contract.EndContractBlock();

            var matchRate = this.GetEstimatedMatchRate(product);

            var estimatedRecordCount = (Int32)Math.Floor(matchRate * recordCount);
            var rateCard = await costService.CreateRateCard(product.Convert(), cancellation).ConfigureAwait(false);
            var model = rateCard.PricingModel;

            return await costService.Estimate(product.Convert(), model, estimatedRecordCount, cancellation);
        }

        /// <summary>
        /// Used to implement the needed logic for product match rate estimations. Each context has it's own logic based on prior use.
        /// </summary>
        /// <param name="supportedProduct">The product to calculate an estimate for.</param>
        /// <returns>The estimated rate, as a percentage, for matches.</returns>
        protected abstract Decimal GetEstimatedMatchRate(PublicProduct supportedProduct);

        #endregion
    }

    [ContractClassFor(typeof(PricingControllerBase<>))]
    internal abstract class PricingControllerBaseContracts : PricingControllerBase<Cart>
    {
        protected override PublicProduct[] SupportedProducts => default(PublicProduct[]);

        protected override IProductHelpService ProductHelper
        {
            get
            {
                Contract.Ensures(Contract.Result<IProductHelpService>() != null);
                return default(IProductHelpService);
            }
        }

        protected override Task<ICostService> CreateCostService(CancellationToken cancellation)
        {
            return default(Task<ICostService>);
        }

        protected override Task<Decimal> GetCost(ICostService costService, PublicProduct product, Int32 recordCount, CancellationToken cancellation)
        {
            return default(Task<Decimal>);
        }

        protected override Decimal GetEstimatedMatchRate(PublicProduct supportedProduct)
        {
            Contract.Ensures(Contract.Result<Decimal>() >= 0 && Contract.Result<Decimal>() <= 1);

            return default(Decimal);
        }
    }
}