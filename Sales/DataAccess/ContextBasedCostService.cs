using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Implementation of the <see cref="ICostService"/> for use with the default rate card
    /// (unless overriden by a subclass) in real time from the provided <see cref="DefaultContext"/>.
    /// Pricing rates are only determined via an indicated setting. Match rate estimations are not supported.
    /// </summary>
    public class ContextBasedCostService : ICostService
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly PricingModel pricingModel;
        private string rateCard;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBasedCostService"/> class.
        /// </summary>
        /// <remarks>
        /// Used the default <see cref="PricingModel"/> on all created <see cref="RateCard"/> instances.
        /// </remarks>
        /// <param name="context">The <see cref="DefaultContext"/> to be used by the cost service.</param>
        public ContextBasedCostService(DefaultContext context) : this(context, default(PricingModel))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBasedCostService"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to be used by the cost service.</param>
        /// <param name="pricingModel">The <see cref="PricingModel"/> that default rates cards should be returned with.</param>
        public ContextBasedCostService(DefaultContext context, PricingModel pricingModel)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
            this.pricingModel = pricingModel;
            this.rateCard = Cost.DefaultCategory;
        }

        #endregion

        #region ICostService Members

        /// <summary>
        /// This type does not restrict use to specific products and therefore returns an empty array.
        /// </summary>
        public virtual String[] SupportedProducts => Array.Empty<String>();

        /// <summary>
        /// Builds the correct cost structure for the indicated <paramref name="product"/>
        /// </summary>
        /// <remarks>
        /// Just proxies the call to the <see cref="CreateRateCard(String, CancellationToken)"/> overload.
        /// </remarks>
        /// <param name="product">The <see cref="DataServiceOperation"/> to get a cost off the rate card.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>A populated <see cref="RateCard"/> for the <paramref name="product"/>.</returns>
        public Task<RateCard> CreateRateCard(DataServiceOperation product, CancellationToken cancellation = new CancellationToken())
        {
            return this.CreateRateCard(product.ToString(), cancellation);
        }

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
        public virtual async Task<RateCard> CreateRateCard(String productKey, CancellationToken cancellation = default(CancellationToken))
        {
            Trace.TraceInformation($"Creating rate card for product {productKey}");

            var baseQuery = this.CreateBaseQuery(productKey);
            var costs = (await this.CreateFilteredRateCard(baseQuery, cancellation).ConfigureAwait(false)).ToArray();

            if (!costs.Any()) throw new InvalidOperationException($"{productKey} does not have an associated cost structure for it.");

            var model = await this.DeterminePricingModel(costs, cancellation);
            Trace.TraceInformation($"Rate card for product {productKey} has {model} pricing");

            var rateCard = new RateCard(costs, model);
            return rateCard;
        }

        /// <summary>
        /// Determines the cost value for the indicated <paramref name="product" /> off the rate card using a specific pricing model.
        /// </summary>
        /// <param name="product">The <see cref="T:AccurateAppend.Core.Definitions.DataServiceOperation" /> to get a cost off the rate card.</param>
        /// <param name="pricing">Indicated the type of <see cref="T:AccurateAppend.Core.Definitions.PricingModel" /> to be estimated.</param>
        /// <param name="itemCount">The number of items to base off the rate card.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The cost of the product for the indicated matches off the rate card.</returns>
        public virtual async Task<Decimal> Estimate(DataServiceOperation product, PricingModel pricing, Int32 itemCount, CancellationToken cancellation = default(CancellationToken))
        {
            var rateCard = await this.CreateRateCard(product, cancellation).ConfigureAwait(false);

            var cost = rateCard.FindCost(itemCount);
            if (cost == null) return 0m;

            return rateCard.PricingModel == PricingModel.Match ? cost.PerMatch : cost.PerRecord;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DefaultContext"/> used for this cost service.
        /// </summary>
        protected virtual DefaultContext Context => this.context;

        /// <summary>
        /// Gets the name of the rate card to base costs on. Defaults of <see cref="Cost.DefaultCategory"/>.
        /// </summary>
        protected virtual String RateCard
        {
            get
            {
                return this.rateCard;
            }

            set
            {
                this.rateCard = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Since most rate card logic is based on <see cref="Cost.Category"/> filters, this method allows
        /// simply subclassing by only changing the root query. The first of three hook methods called by
        /// <see cref="CreateRateCard(String, CancellationToken)"/>.
        /// </summary>
        /// <param name="productKey">The <see cref="DataServiceOperation"/> to get a cost off the rate card.</param>
        /// <returns>The query to use.</returns>
        protected virtual IQueryable<Cost> CreateBaseQuery(String productKey)
        {
            return this.context.SetOf<Cost>()
                .Where(c => c.Product.Key == productKey)
                .Where(c => c.Category == this.RateCard);
        }

        /// <summary>
        /// Some rate card logic may need to be deferred until realization of the <paramref name="baseQuery"/>.
        /// The second of three hook methods called by <see cref="CreateRateCard(String, CancellationToken)"/>.
        /// </summary>
        /// <param name="baseQuery">The query created by <see cref="CreateBaseQuery"/>.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The realized and optionally filtered (or replaced) query.</returns>
        protected virtual async Task<IEnumerable<Cost>> CreateFilteredRateCard(IQueryable<Cost> baseQuery, CancellationToken cancellation)
        {
            return await baseQuery.ToArrayAsync(cancellation).ConfigureAwait(false);
        }

        /// <summary>
        /// Used internally to determine the <see cref="PricingModel"/> that should be used with the provided <paramref name="costStructure"/>.
        /// In the default cost service this will always be the original value and will run synchronously. However Subclasses may use this 
        /// method to perform additional logic on the actual structure that is provided before construction of a <see cref="RateCard"/>. The last
        /// of three hook methods called by <see cref="CreateRateCard(String, CancellationToken)"/>.
        /// </summary>
        /// <param name="costStructure">The cost structure that will be provided to a <see cref="RateCard"/>.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="PricingModel"/> to be used with a created <see cref="RateCard"/>.</returns>
        protected virtual Task<PricingModel> DeterminePricingModel(IEnumerable<Cost> costStructure, CancellationToken cancellation)
        {
            return Task.FromResult(this.pricingModel);
        }

        /// <summary>
        /// User to determine the <see cref="PricingModel"/> that should be used with the provided <see cref="DataServiceOperation"/>.
        /// In the default cost service this will always be the original value and will run synchronously. However Subclasses may use this 
        /// method to perform additional logic on the actual structure that is provided before construction of a <see cref="RateCard"/>.
        /// </summary>
        /// <param name="forOperation">The <see cref="DataServiceOperation"/> to determine for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The <see cref="PricingModel"/> to be used a specific operation.</returns>
        public virtual Task<PricingModel> DeterminePricingModel(DataServiceOperation forOperation, CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult(this.pricingModel);
        }

        #endregion
    }
}
