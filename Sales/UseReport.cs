using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains an aggregated usage report.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Source) + ("}, Matched {" + nameof(MatchCount) + ("} of {" + nameof(RecordCount) + "}")))]
    public class UseReport
    {
        /// <summary>
        /// Contains a named source of usage. Most commonly a operation but could contain other values/meanings.
        /// See the returning method documentation for understanding this value.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// The input records for the usage.
        /// </summary>
        public Int32 RecordCount { get; set; }

        /// <summary>
        /// The matched records for the usage.
        /// </summary>
        public Int32 MatchCount { get; set; }

        /// <summary>
        /// Using the supplied <paramref name="rateCard"/>, locate a <see cref="Cost"/> based on the <see cref="RateCard.PricingModel"/>
        /// using the current <see cref="MatchCount"/> or <see cref="RecordCount"/> as is appropriate, and return the rate.
        /// </summary>
        /// <remarks>
        /// Really a convenience method to standardize the correct logic for selecting the <seealso cref="MatchCount"/> or <seealso cref="RecordCount"/>
        /// from the current instance to use with the <see cref="RateCard.FindCost"/> method.
        /// </remarks>
        /// <param name="rateCard">The <see cref="RateCard"/> that should have a <see cref="Cost"/> looked up from and the the appropriate rate returned.</param>
        /// <returns>The rate and count from the located <see cref="Cost"/> that should be used, based on the <see cref="RateCard"/> <see cref="RateCard.PricingModel">pricing model</see>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The supplied <paramref name="rateCard"/> is for a <see cref="Product"/> that does not match this usage report <see cref="Source"/>.</exception>
        public virtual Tuple<Decimal, Int32> FindCostFromCard(RateCard rateCard)
        {
            if (rateCard.ForProduct.Key != this.Source) throw new ArgumentOutOfRangeException($"{nameof(UseReport)} is for source: {this.Source} but supplied rate card was for {rateCard.ForProduct.Key}");

            Contract.Ensures(Contract.Result<Tuple<Decimal, Int32>>() != null);
            Contract.Ensures(Contract.Result<Tuple<Decimal, Int32>>().Item1 >= 0);
            Contract.Ensures(Contract.Result<Tuple<Decimal, Int32>>().Item2 >= 0);
            Contract.EndContractBlock();

            Trace.TraceInformation($"Looking up cost from rate card for {rateCard.PricingModel} pricing");
            var model = rateCard.PricingModel;
            Trace.TraceInformation($"Pricing model is {model}");

            var amount = model == PricingModel.Match
                ? this.MatchCount
                : this.RecordCount;
            var cost = rateCard.FindCost(amount);

            Trace.TraceInformation($"Located cost with match: {cost.PerMatch}, record: {cost.PerRecord}");

            var rate = model == PricingModel.Match
                ? cost.PerMatch
                : cost.PerRecord;

            Trace.TraceInformation($"Using Rate {rate} for count {amount}");

            return Tuple.Create(rate, amount);
        }
    }
}
