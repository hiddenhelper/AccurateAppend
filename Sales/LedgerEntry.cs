using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Indicates an entry for a specific type (aka meaning) relating an <see cref="RecurringBillingAccount"/> to a
    /// <see cref="DealBinder"/> and specifying the date (day only, not time based) the entry is valid through. That
    /// is, effective up through the indicated date as close of business in <see cref="DateTimeExtensions.BillingZone"/>.
    /// </summary>
    [DebuggerDisplay("{this." + nameof(Classification) + ("} {this." + nameof(PeriodStart) + "} through {this." + nameof(PeriodEnd) + "}"))]
    public class LedgerEntry
    {
        #region Fields

        private DateTime periodStart;
        private DateTime periodEnd;
        private DealBinder withDeal;
        private RecurringBillingAccount forAccount;
        private LedgerType classification;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerEntry"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected LedgerEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerEntry"/> class.
        /// </summary>
        /// <param name="forAccount">The <see cref="RecurringBillingAccount"/> the current entry is for.</param>
        /// <param name="period"> The date (time portion is ignored) that the entry is entered for (as COB) being current through.</param>
        /// <param name="classification">A <see cref="LedgerType"/> flag indicating how to classify this entry.</param>
        protected LedgerEntry(RecurringBillingAccount forAccount, DateSpan period, LedgerType classification)
        {
            if (forAccount == null) throw new ArgumentNullException(nameof(forAccount));
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.EndContractBlock();

            // Convert to billing time
            period = period.ToBillingZone();

            if (!forAccount.IsValidForPeriod(period)) throw new ArgumentOutOfRangeException(nameof(period), period, $"{nameof(period)} is outside of account contract effective period of {forAccount.EffectiveDate}-{forAccount.EndDate}");

            this.periodStart = period.StartingOn.Date;
            this.periodEnd = period.EndingOn.Date;
            this.forAccount = forAccount;
            this.classification = classification;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerEntry"/> class.
        /// </summary>
        /// <param name="forAccount">The <see cref="RecurringBillingAccount"/> the current entry is for.</param>
        /// <param name="period"> The date (time portion is ignored) that the entry is entered for (as COB) being current through.</param>
        /// <param name="deal">The <see cref="DealBinder"/> that is added to the account ledger.</param>
        /// <param name="classification">A <see cref="LedgerType"/> flag indicating how to classify this entry.</param>
        protected LedgerEntry(RecurringBillingAccount forAccount, DateSpan period, DealBinder deal, LedgerType classification)
            : this(forAccount, period, classification)
        {
            if (deal == null) throw new ArgumentNullException(nameof(deal));
            Contract.EndContractBlock();

            this.withDeal = deal;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The identifier of the current instance.
        /// </summary>
        public virtual Int32? Id { get; protected internal set; }

        /// <summary>
        /// Gets the <see cref="RecurringBillingAccount"/> the ledger entry is for.
        /// </summary>
        public virtual RecurringBillingAccount ForAccount
        {
            get => this.forAccount;
            protected set => this.forAccount = value;
        }

        /// <summary>
        /// Gets the <see cref="DealBinder"/>, if any, that is posted for the ledger entry.
        /// </summary>
        public virtual DealBinder WithDeal
        {
            get => this.withDeal;
            protected set => this.withDeal = value;
        }

        /// <summary>
        /// The date (time portion is ignored) that the entry is entered for (as OFB) as being current from.
        /// </summary>
        public virtual DateTime PeriodStart
        {
            get => this.periodStart;
            protected set => this.periodStart = value.ToSafeLocal().Date;
        }

        /// <summary>
        /// The date (time portion is ignored) that the entry is entered for (as COB) as being current through.
        /// </summary>
        public virtual DateTime PeriodEnd
        {
            get => this.periodEnd;
            protected set => this.periodEnd = value.ToSafeLocal().Date;
        }

        /// <summary>
        /// Indicates the type of ledger entry this instance is. Will be replaced in the future with polymorphism.
        /// </summary>
        public virtual LedgerType Classification
        {
            get => this.classification;
            protected set => this.classification = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks the current instance as voided. Only applicable for entries with an associated deal.
        /// </summary>
        public virtual void Void()
        {
            this.WithDeal = null;
        }

        /// <summary>
        /// Factory to create a ledger entry for a subscription bill.
        /// </summary>
        /// <param name="subscriptionBilling">The <see cref="SubscriptionBilling"/> billed.</param>
        /// <param name="deal">The <see cref="LedgerDeal"/> that was billed.</param>
        /// <param name="period">The ending period (effective date) of the bill.</param>
        /// <returns>The constructed <see cref="LedgerEntry"/>.</returns>
        public static LedgerEntry ForSubscription(SubscriptionBilling subscriptionBilling, LedgerDeal deal, DateSpan period)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.Ensures(Contract.Result<LedgerEntry>() != null);
            Contract.Ensures(Contract.Result<LedgerEntry>().WithDeal != null);

            return new LedgerEntry(subscriptionBilling, period, deal, LedgerType.ForSubscriptionPeriod);
        }

        /// <summary>
        /// Factory to create a ledger entry for a usage bill.
        /// </summary>
        /// <param name="account">The <see cref="RecurringBillingAccount"/> billed.</param>
        /// <param name="deal">The <see cref="LedgerDeal"/> that was billed.</param>
        /// <param name="period">The ending period (effective date) of the bill.</param>
        /// <returns>The constructed <see cref="LedgerEntry"/>.</returns>
        public static LedgerEntry ForUsage(RecurringBillingAccount account, LedgerDeal deal, DateSpan period)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.Ensures(Contract.Result<LedgerEntry>() != null);
            Contract.Ensures(Contract.Result<LedgerEntry>().WithDeal != null);

            return new LedgerEntry(account, period, deal, LedgerType.ForUsage);
        }

        /// <summary>
        /// Factory to create a ledger entry for a subscription indicating it is current through the ending period.
        /// </summary>
        /// <param name="subscription">The <see cref="SubscriptionBilling"/> billed.</param>
        /// <param name="period">The ending period (effective date) the account is considered current through.</param>
        /// <returns>The constructed <see cref="LedgerEntry"/>.</returns>
        public static LedgerEntry ClosePeriod(SubscriptionBilling subscription, DateSpan period)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.Ensures(Contract.Result<LedgerEntry>() != null);
            Contract.Ensures(Contract.Result<LedgerEntry>().WithDeal == null);

            return new LedgerEntry(subscription, period, LedgerType.GeneralAdjustment);
        }

        /// <summary>
        /// Factory to create a ledger entry for a usage only account indicating it is current through the ending period.
        /// </summary>
        /// <param name="account">The <see cref="UsageBilling"/> billed.</param>
        /// <param name="period">The ending period (effective date) the account is considered current through.</param>
        /// <returns>The constructed <see cref="LedgerEntry"/>.</returns>
        public static LedgerEntry ClosePeriod(UsageBilling account, DateSpan period)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.Ensures(Contract.Result<LedgerEntry>() != null);
            Contract.Ensures(Contract.Result<LedgerEntry>().WithDeal == null);

            return new LedgerEntry(account, period, LedgerType.GeneralAdjustment);
        }

        #endregion
    }
}
