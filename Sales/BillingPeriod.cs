using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A specialized version of a <see cref="DateSpan"/> specifically used with billing. It not only describes
    /// a period of time but also qualifies this as belonging to a <see cref="LedgerType"/>. In essence it will
    /// allow a caller to specify a range and target for the type of billing to perform on a <see cref="RecurringBillingAccount"/>
    /// while allowing calculation and logic related to the closing of periods, accounting for partial billing, and other
    /// concerns to be gathered.
    /// </summary>
    [DebuggerDisplay("{AccurateAppend.Core.EnumExtensions.GetDescription(Type)}, {StartingOn.ToShortDateString()}-{EndingOn.ToShortDateString()}")]
    public class BillingPeriod : DateSpan
    {
        /// <summary>
        /// Used in a period to flag that this type of billing must be a past period (closed) prior to billing. It does NOT
        /// indicate if the period has actually closed (use the <see cref="IsClosed"/> method).
        /// </summary>
        /// <value>True if this <see cref="Type"/> of period billing should wait for the period to close prior to being run.</value>
        public Boolean WaitUntilPeriodClose { get; set; }

        /// <summary>
        /// Classifies the current period for a specific type of billing.
        /// </summary>
        public LedgerType Type { get; set; }

        /// <summary>
        /// Provides a watermark <see cref="DateTime.Date">Date</see> that billing has occured through. This value may be the
        /// same as the actual <see cref="DateSpan.StartingOn"/> value, however, interim billing may bump this through this value.
        /// </summary>
        public DateTime PaidThrough { get; set; }

        /// <summary>
        /// Calculates if the current <see cref="BillingPeriod"/> is completed, as in occuring completely in the past.
        /// </summary>
        /// <returns>True if the period is past; Otherwise false.</returns>
        public virtual Boolean IsClosed()
        {
            return this.EndingOn.ToBillingZone().Date < DateTime.Now.ToBillingZone().Date;
        }

        /// <inheritdoc />
        public override String ToString()
        {
            return $"{this.StartingOn.ToShortDateString()} - {this.EndingOn.ToShortDateString()}";
        }

        /// <summary>
        /// Determines the outstanding portion of the <see cref="BillingPeriod"/>. That is, the remaining
        /// portion of the period after taking any <see cref="PaidThrough"/> marker into account. Use the
        /// <see cref="DateSpan.IsLogicalDateRange"/> method to evaluate if there is any outstanding portion.
        /// </summary>
        /// <returns>The portion of the current period that is outstanding. This value is never null. It is the responsibility of the caller to determine if the result is <see cref="DateSpan.IsLogicalDateRange">cogent</see>.</returns>
        public virtual DateSpan ToOutstandingRange()
        {
            Contract.Ensures(Contract.Result<DateSpan>() != null);

            var floor = this.PaidThrough.Date.AddDays(1);
            floor = DateTimeExtensions.Max(floor, this.StartingOn);

            return new DateSpan(floor, this.EndingOn);
        }

        /// <summary>
        /// Factory method to quickly craft a <seealso cref="BillingPeriod"/> from a <seealso cref="DateSpan"/>.
        /// </summary>
        /// <remarks>
        /// Assumes the <seealso cref="BillingPeriod.PaidThrough"/> value is current up to the <seealso cref="DateSpan.StartingOn"/> but not including.
        /// Any <seealso cref="DateTime.TimeOfDay"/> portion of the range, if any, is maintained.
        /// The <seealso cref="DateTime.Kind"/> value will be maintained.
        /// </remarks>
        /// <param name="span">The <seealso cref="DateSpan"/> to base the range from.</param>
        /// <param name="ledgerType">Indicates the type of billing period to generate.</param>
        /// <returns>A new <seealso cref="BillingPeriod"/> for the ledger type covering the same range as <param name="span"/>.</returns>
        public static BillingPeriod FromDateSpan(DateSpan span, LedgerType ledgerType)
        {
            if (span == null) throw new ArgumentNullException(nameof(span));
            Contract.Ensures(Contract.Result<BillingPeriod>() != null);
            Contract.Ensures(Contract.Result<BillingPeriod>().Type == ledgerType);
            Contract.Ensures(Contract.Result<BillingPeriod>().StartingOn == span.StartingOn);
            Contract.Ensures(Contract.Result<BillingPeriod>().StartingOn.Kind == span.StartingOn.Kind);
            Contract.Ensures(Contract.Result<BillingPeriod>().EndingOn == span.EndingOn);
            Contract.Ensures(Contract.Result<BillingPeriod>().EndingOn.Kind == span.EndingOn.Kind);
            Contract.EndContractBlock();

            var period = new BillingPeriod();
            period.Type = ledgerType;
            period.WaitUntilPeriodClose = false;
            period.StartingOn = span.StartingOn;
            period.EndingOn = span.EndingOn;
            period.PaidThrough = span.StartingOn.AddDays(-1);

            return period;
        }
    }
}