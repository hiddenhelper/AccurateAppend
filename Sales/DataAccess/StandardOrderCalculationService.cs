using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides the universal implementation of the <see cref="IOrderCalculationService"/> that
    /// all components will use by default. Context specific behavior and logic is generally managed
    /// by leveraging various concrete implementations of <see cref="ICostService"/> to interact
    /// with as opposed to changing this component (though no parts of the system assume this is the case).
    /// </summary>
    public class StandardOrderCalculationService : IOrderCalculationService
    {
        #region IOrderCalculationService Members

        /// <inheritdoc />
        public virtual async Task FillFromRateCard(BillableOrder order, ICostService costService, ManifestBuilder manifest, ProcessingReport processingReport, CancellationToken cancellation = default(CancellationToken))
        {
            order.Lines.Clear();

            // consolidate operations in processing report
            // create new order items using the consolidated items
            // Note the use of Composite Operation Expansion
            var operations = manifest.Operations.FilterPreferences().ExpandComposites().ToArray();

            foreach (var grouping in operations.GroupBy(o => o.Name))
            {
                var product = grouping.Key;
                var rateCard = await costService.CreateRateCard(product, cancellation).ConfigureAwait(false);

                this.LoadOrderItem(grouping.First(), order, rateCard, processingReport);
            }
        }

        /// <inheritdoc />
        public virtual async Task FillFromRateCard(BillableOrder order, ICostService costService, ProcessingReport processingReport, CancellationToken cancellation = default(CancellationToken))
        {
            order.Lines.Clear();

            foreach (var operation in processingReport.Operations.Where(r => !r.Name.IsPreference()).Select(o => o.Name).Distinct())
            {
                var rateCard = await costService.CreateRateCard(operation, cancellation).ConfigureAwait(false);

                this.LoadOrderItem(order, rateCard, processingReport);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the actual work of loading a single <see cref="ProductLine"/> into the provided <paramref name="order"/> derived
        /// from the <paramref name="rateCard"/> based on the supplied <paramref name="report"/> to generate the <see cref="ProductLine"/> for.
        /// </summary>
        /// <remarks>
        /// This overload supports population on <see cref="BillableOrder"/> from only report.
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to populate.</param>
        /// <param name="rateCard">The <see cref="RateCard"/> to generate a <see cref="ProductLine"/> for the <see cref="Product"/>.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> used to supply match and record counts for.</param>
        protected virtual void LoadOrderItem(BillableOrder order, RateCard rateCard, ProcessingReport report)
        {
            var product = rateCard.ForProduct;
            var operation = EnumExtensions.Parse<DataServiceOperation>(product.Key);
            var pricingModel = rateCard.PricingModel;

            // figure out the range based on the rate card pricing model (per-match vs per-record)
            var count = rateCard.PricingModel == PricingModel.Match
                ? report.CalculateMatchCount(operation)
                : report.TotalRecords;

            var costStructure = rateCard.FindCost(count);

            // select rate on rate card pricing model
            var cost = rateCard.PricingModel == PricingModel.Match ? costStructure.PerMatch : costStructure.PerRecord;

            var minimum = (costStructure.FileMinimum ?? 0m);

            // if per record then we need to create order items using the total record count and the operations name
            if (pricingModel == PricingModel.Record)
            {
                var item = new ProductLine(order, product);
                item.Price = Convert.ToDecimal(cost);
                item.Quantity = count;
            }
            else
            {
                // if default (match) then we need to create order items using the match count and the operations in the report

                // if null then there were no matches for this operation, in this case add an order item with 0 quantity using operation name
                if (report.Operations.All(a => a.Name != operation))
                {
                    var item = order.CreateLine(product);
                    item.Price = cost;
                    item.Quantity = 0;
                }
                else
                {
                    count = report.CalculateMatchCount(operation);

                    var item = order.CreateLine(product);
                    item.Price = cost;
                    item.Quantity = count;
                }
            }

            if (minimum != 0) order.OrderMinimum = order.OrderMinimum + minimum;
        }

        /// <summary>
        /// Performs the actual work of loading a single <see cref="ProductLine"/> into the provided <paramref name="order"/> derived
        /// from the <paramref name="rateCard"/> based on the supplied <paramref name="report"/> against the <paramref name="operation"/> ran.
        /// </summary>
        /// <remarks>
        /// This overload supports population on <see cref="BillableOrder"/> from a manifest and report.
        /// </remarks>
        /// <param name="operation">The <see cref="OperationDefinition"/> that was run.</param>
        /// <param name="order">The <see cref="Order"/> instance to add <see cref="ProductLine"/> to..</param>
        /// <param name="rateCard">The set of <see cref="Cost">Cost Structures</see> for a <see cref="Product"/>.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> containing the results of the job.</param>
        protected virtual void LoadOrderItem(OperationDefinition operation, BillableOrder order, RateCard rateCard, ProcessingReport report)
        {
            var product = rateCard.ForProduct;
            var cost = operation.Cost;
            var minimum = 0m;
            Int32 count;
            var pricingModel = operation.PricingModel;

            // if we lack a cost look up the appropriate cost structure from the rate card
            if (cost == 0M)
            {
                pricingModel = rateCard.PricingModel;

                // figure out the range based on the rate card pricing model (per-match v per-record)
                count = rateCard.PricingModel == PricingModel.Match
                    ? report.CalculateMatchCount(operation.Name)
                    : report.TotalRecords;

                var costStructure = rateCard.FindCost(count);
                
                // select rate on rate card pricing model
                cost = rateCard.PricingModel == PricingModel.Match ? costStructure.PerMatch : costStructure.PerRecord;

                minimum = (costStructure.FileMinimum ?? 0m);
            }
            else
            {
                // figure out the count based on the manifest pricing model (per-match v per-record)
                count = operation.PricingModel == PricingModel.Match
                    ? report.CalculateMatchCount(operation.Name)
                    : report.TotalRecords;
            }

            // if per record then we need to create order items using the total record count and the operations in the manifest
            if (pricingModel == PricingModel.Record)
            {
                var item = new ProductLine(order, product);
                item.Price = Convert.ToDecimal(cost);
                item.Quantity = count;
            }
            else
            {
                // if default (match) then we need to create order items using the match count and the operations in the manifest

                // if null then there were no matches for this operation, in this case add an order item with 0 quantity using operation name from manifest
                if (report.Operations.All(a => a.Name != operation.Name))
                {
                    var item = order.CreateLine(product);
                    item.Price = cost;
                    item.Quantity = 0;
                }
                else
                {
                    count = report.CalculateMatchCount(operation.Name);

                    var item = order.CreateLine(product);
                    item.Price = cost;
                    item.Quantity = count;
                }
            }

            if (minimum != 0) order.OrderMinimum = order.OrderMinimum + minimum;
        }

        #endregion
    }
}
