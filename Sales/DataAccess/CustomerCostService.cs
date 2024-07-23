using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Extension of the <see cref="ContextBasedCostService"/> used for determining rates for a specific <see cref="ClientRef"/>.
    /// </summary>
    public class CustomerCostService : ContextBasedCostService
    {
        #region Fields

        private readonly ClientRef client;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerCostService"/>.
        /// </summary>
        /// <param name="client">The <see cref="ClientRef"/> to create cost information for.</param>
        /// <param name="context">The <see cref="DefaultContext"/> to be used by the cost service.</param>
        public CustomerCostService(ClientRef client, DefaultContext context) : base(context)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.EndContractBlock();

            this.client = client;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override IQueryable<Cost> CreateBaseQuery(String productKey)
        {
            var userId = this.client.UserId.ToString();
            var categories = new[] { userId, Cost.DefaultCategory };

            var baseQuery = this.Context
                .SetOf<Cost>()
                .Where(c => c.Product.Key == productKey)
                .Where(c => categories.Contains(c.Category));

            return baseQuery;
        }

        /// <inheritdoc />
        protected override async Task<IEnumerable<Cost>> CreateFilteredRateCard(IQueryable<Cost> baseQuery, CancellationToken cancellation)
        {
            var costs = await baseQuery.GroupBy(c => c.Category).ToDictionaryAsync(g => g.Key, g => g.ToArray(), cancellation).ConfigureAwait(false);

            if (!costs.Any()) return Enumerable.Empty<Cost>();

            var defaultRates = costs.ContainsKey(Cost.DefaultCategory) ? costs[Cost.DefaultCategory] : Enumerable.Empty<Cost>();

            var key = costs.Keys.FirstOrDefault(k => k != Cost.DefaultCategory);
            return key == null ? defaultRates : costs[key];
        }

        /// <inheritdoc />
        protected override Task<PricingModel> DeterminePricingModel(IEnumerable<Cost> costStructure, CancellationToken cancellation)
        {
            costStructure = costStructure.ToArray();
            if (!costStructure.Any()) return Task.FromResult(default(PricingModel));

            var category = costStructure.First().Category;
            var product = costStructure.First().Product.Key;

            return this.Context.SetOf<CostPricingModel>()
                .Where(c => c.Category == category && c.ForProduct.Key == product)
                .Select(c => c.Model)
                .FirstOrDefaultAsync(cancellation);
        }

        /// <inheritdoc />
        public override Task<PricingModel> DeterminePricingModel(DataServiceOperation operation, CancellationToken cancellation = default(CancellationToken))
        {
            var category = this.client.UserId.ToString();
            var product = operation.ToString();

            return this.Context.SetOf<CostPricingModel>()
                .Where(c => c.Category == category && c.ForProduct.Key == product)
                .Select(c => c.Model)
                .FirstOrDefaultAsync(cancellation);
        }

        #endregion
    }
}
