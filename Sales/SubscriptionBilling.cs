using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A client subscription services contract. These contracts have a minimum prepayment performed at the 
    /// start of a period. In addition, they may optionally have a fixed rate OR are allowed a optional 
    /// balance above the amount of prepayment to be enforced. If fixed rates are not required, either at the
    /// close of the period OR when the balance exceeds the indicated value, a new bill will be created.
    /// </summary>
    [DebuggerDisplay("ID:{" + nameof(Id) + "}, Prepayment, Amount={" + nameof(PrepaymentAmount) + ".ToString(\"C\")}, Recurrence={" + nameof(Recurrence) + "}, Starting {" + nameof(EffectiveDate) + ".ToString(\"d\")}")]
    public class SubscriptionBilling : RecurringBillingAccount
    {
        #region Fields

        private Decimal amount;
        private Decimal? maxOverageLimit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBilling"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected SubscriptionBilling()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBilling"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientRef"/> the subscription is for.</param>
        /// <param name="publicKey">The alternate shared identifier of the subscription.</param>
        /// <param name="amount">The amount the subscription costs.</param>
        /// <param name="effectiveDate">The date the subscription becomes active.</param>
        /// <param name="endDate">The optional date the subscription ends.</param>
        public SubscriptionBilling(ClientRef client, Guid publicKey, Decimal amount, DateTime effectiveDate, DateTime? endDate = null) : base(client, publicKey, effectiveDate, endDate)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), amount, $"{nameof(amount)} must be at least 0");
            Contract.EndContractBlock();

            this.amount = amount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the amount of the subscription.
        /// </summary>
        /// <remarks>This value is always a positive number.</remarks>
        /// <returns>The amount of the subscription.</returns>
        public virtual Decimal PrepaymentAmount
        {
            get
            {
                Contract.Ensures(Contract.Result<Decimal>() > 0);

                return this.amount;
            }
            set
            {
                if (value <= 0) throw new ArgumentException($"Value must be greater than {0}.", nameof(value));
                Contract.EndContractBlock();

                this.amount = value;
            }
        }

        /// <summary>
        /// Get or sets the billing schedule for the current subscription.
        /// </summary>
        /// <value>The billing schedule for the current subscription.</value>
        public virtual DateGrain Recurrence
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum limit, if any, for total usage during a billing period
        /// that exceeds the <see cref="PrepaymentAmount"/>. A null value indicates an
        /// account that has unlimited possible balance in a billing period.
        /// </summary>
        public Decimal? MaxOverageLimit
        {
            get { return this.maxOverageLimit; }
            protected set
            {
                if (value.HasValue && value <= 0) throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(this.MaxOverageLimit)} must be null or greater than 0");
                this.maxOverageLimit = value;
            }
        }

        /// <summary>
        /// Indicates where the current account is a fixed rate or not. Fixed rate accounts so not incur usage bills period.
        /// </summary>
        /// <value>True if the current instance is a fixed rate account; Otherwise false.</value>
        public virtual Boolean FixedRate { get; set; }

        #endregion

        #region Overrides

        /// <inheritdoc />
        /// <remarks>
        /// Returns 0, 1, or 2 <see cref="T:AccurateAppend.Sales.BillingPeriod" /> instances, though only one per <see cref="T:AccurateAppend.Sales.LedgerType" />. Each will range from (Last Billed + 1 day) or
        /// start of account, whichever is later and either last closed business day (Usage) or till close of period (Subscription). If <see cref="P:AccurateAppend.Sales.Subscription.FixedRate" /> is
        /// set, there will NEVER be usage based periods created.
        /// </remarks>
        public override async Task<IEnumerable<BillingPeriod>> DetermineUnpaidBillingPeriods(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken))
        {
            var periods = new List<BillingPeriod>();
            var period = await this.DetermineNextSubscriptionPeriod(calculator, cancellation).ConfigureAwait(false);
            if (period != null) periods.Add(period);

            period = await this.DetermineNextUsagePeriod(calculator, cancellation);
            if (period != null) periods.Add(period);

            return periods;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Extends the base behavior by first checking for the presence of <see cref="FixedRate"/>. If found, short circuit as there's never usage.
        /// Then leverages the base implementation to generate a usage based deal on ALL covered usage and current <paramref name="rates"/>. Then the
        /// <see cref="PrepaymentAmount"/> is applied as a credit adjustment to the <see cref="Order"/>. It the recalculated total has a value only then
        /// is the generated <see cref="LedgerEntry"/> returned.
        /// </remarks>
        public override async Task<LedgerEntry> CreateUsageBill(Guid currentUser, BillingPeriod period, IClientUsageCalculator calculator, ICostService rates, CancellationToken cancellation = default(CancellationToken))
        {
            if (this.FixedRate) return null;

            var ledger = await base.CreateUsageBill(currentUser, period, calculator, rates, cancellation).ConfigureAwait(false);
            if (ledger == null) return null;

            // If we've got a deal created then apply the prepayment amount to the adjustment (sans any prior adjustments)
            var deal = ledger.WithDeal;
            var order = deal.Orders.First();
            order.OrderMinimum = -1 * this.PrepaymentAmount;

            // Recalculate the total and see if we've gone to nill or not
            if (deal.Total() <= 0) return null;

            return ledger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <also cref="LedgerEntry"/> fully populated for the billing cycle containing the provided <paramref name="billingPeriod"/>.
        /// </summary>
        /// <param name="currentUser">The identifier of the user creating the billing.</param>
        /// <param name="onProduct">The <see cref="Product"/> for the "Subscription" sku.</param>
        /// <param name="billingPeriod">The <see cref="BillingPeriod"/> used to evaluate the billing cycle.</param>
        /// <returns>The appropriate <see cref="LedgerEntry"/> instance.</returns>
        public virtual LedgerEntry CreateSubscriptionBill(Guid currentUser, Product onProduct, BillingPeriod billingPeriod)
        {
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
            if (onProduct == null) throw new ArgumentNullException(nameof(onProduct));
            Contract.EndContractBlock();

            // TODO: we really need to create a method to determine if the providing billing period actually crosses temporal periods

            if (!this.IsValidForPeriod(billingPeriod)) throw new InvalidOperationException($"The subscription {this.Id} is not valid for the billing period covering {billingPeriod}");

            var deal = new LedgerDeal(this, billingPeriod, currentUser);

            var order = deal.Orders.First();
            var line = order.CreateLine(onProduct);
            line.Quantity = 1;
            line.Price = this.PrepaymentAmount;
            deal.Amount = deal.Total();

            deal.Description = $"Subscription Billing : {billingPeriod:MMMM}";
            deal.Title = deal.Description;

            var ledger = LedgerEntry.ForSubscription(this, deal, billingPeriod);
            return ledger;
        }

        /// <summary>
        /// Performs logic to calculate the next unpaid <see cref="BillingPeriod"/> for an account, if any, for subscription billing.
        /// </summary>
        /// <param name="calculator">The <see cref="IClientUsageCalculator"/> able to provide access to a billing ledger.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The next <see cref="BillingPeriod"/> that should be billed. This period may or may not be ready to be billed.</returns>
        protected virtual async Task<BillingPeriod> DetermineNextSubscriptionPeriod(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken))
        {
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            Contract.EndContractBlock();

            if (this.Id == null) return null;

            var lastPaidSubscription = await calculator.NewestLedgerDate(this.Id.Value, LedgerType.ForSubscriptionPeriod, cancellation).ConfigureAwait(false);

            DateTime floor;
            DateTime ceiling;

            // Start is +1 day from last time we billed though or start of contract, whichever is later
            if (lastPaidSubscription == null)
            {
                floor = this.EffectiveDate;
            }
            else
            {
                floor = lastPaidSubscription.Value.AddDays(1);
            }

            // If the next unbilled day falls outside the contract end, then we're current
            if (this.EndDate.HasValue && floor > this.EndDate.Value) return null;

            // End is the day the contract ends or the provided through date or the last day of the period, whichever is earlier
            var endOfPeriod = TemporalHelper.CalculateEndOfPeriod(this.EffectiveDate, this.EndDate, floor, 1, this.Recurrence).EndingOn;
            if (this.EndDate.HasValue && endOfPeriod > this.EndDate.Value)
            {
                ceiling = this.EndDate.Value;
            }
            else
            {
                ceiling = endOfPeriod;
            }

            var period = new BillingPeriod()
            {
                StartingOn = floor,
                PaidThrough = floor,
                EndingOn = ceiling,
                Type = LedgerType.ForSubscriptionPeriod,
                WaitUntilPeriodClose = floor >= DateTime.Today
            };


            return period.IsLogicalDateRange() ? period : null;
        }

        /// <summary>
        /// Performs logic to calculate the next unpaid <see cref="BillingPeriod"/> for an account, if any, for any usage billing.
        /// </summary>
        /// <param name="calculator">The <see cref="IClientUsageCalculator"/> able to provide access to a billing ledger.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The next <see cref="BillingPeriod"/> that should be billed. This period may or may not be ready to be billed.</returns>
        protected virtual async Task<BillingPeriod> DetermineNextUsagePeriod(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken))
        {
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            Contract.EndContractBlock();

            if (this.Id == null) return null;
            if (this.FixedRate) return null;

            var billedThrough = await calculator.NewestLedgerDate(this.Id.Value, LedgerType.ForUsage, cancellation).ConfigureAwait(false);

            DateTime floor;
            DateTime ceiling;

            // Start is +1 day from last time we billed though or start of contract, whichever is later
            if (billedThrough == null)
            {
                floor = this.EffectiveDate;
            }
            else
            {
                floor = billedThrough.Value.AddDays(1);
            }

            // If the next unbilled day falls outside the contract end, then we're current
            if (this.EndDate.HasValue && floor > this.EndDate.Value) return null;

            // End is the day the contract ends or the last day of the period, whichever is earlier
            var endOfPeriod = TemporalHelper.CalculateEndOfPeriod(this.EffectiveDate, this.EndDate, floor, 1, this.Recurrence);

            if (this.EndDate.HasValue && endOfPeriod.EndingOn > this.EndDate.Value)
            {
                ceiling = this.EndDate.Value;
            }
            else
            {
                ceiling = endOfPeriod.EndingOn;
            }

            var period = new BillingPeriod()
            {
                StartingOn = endOfPeriod.StartingOn,
                EndingOn = ceiling,
                PaidThrough = billedThrough ?? endOfPeriod.StartingOn.AddDays(-1),
                Type = LedgerType.ForUsage,
                WaitUntilPeriodClose = this.MaxOverageLimit == null
            };

            return period.IsLogicalDateRange() ? period : null;
        }

        /// <summary>
        /// Establishes a new limit, if any, for the subscription.
        /// </summary>
        /// <param name="limit">The limit, in currency, of the max allowed usage before billing may occur.</param>
        public virtual void ApplyLimit(Decimal? limit)
        {
            this.MaxOverageLimit = limit;
        }

        #endregion

        #region Overrides

        // Keep this as we've not added partial usage back into the new context

        ///// <summary>Attempts to create a bill for usage calculations for the indicated <paramref name="period"/>.</summary>
        ///// <param name="currentUser">The interactive <see cref="User"/> creating the billing.</param>
        ///// <param name="period">The <see cref="BillingPeriod"/> to craft a bill for. See <see cref="DetermineUnpaidBillingPeriods"/> to determine values.</param>
        ///// <param name="calculator">A <see cref="IClientUsageCalculator"/> that understands the client usage and ledger system.</param>
        ///// <param name="rates">A <see cref="ICostService"/> that is used to manage the rate cards for the current account.</param>
        ///// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        ///// <returns>If the accured balance exceeds the allowed amount, the <see cref="LedgerEntry"/> for the account; otherwise null.</returns>
        //public override async Task<LedgerEntry> CreateUsageBill(User currentUser, BillingPeriod period, IClientUsageCalculator calculator, ICostService rates, CancellationToken cancellation)
        //{
        //    if (this.FixedRate) return null;

        //    // TODO: we really need to create a method to determine if the providing billing period actually crosses temporal periods

        //    var ledger = await base.CreateUsageBill(currentUser, period, calculator, rates, cancellation).ConfigureAwait(false);
        //    if (ledger == null) return null;

        //    // Technically this could be a landmine. We need to understand how much of the subscription amount is already handled, is any, before subtracting
        //    var v = (await calculator.LedgersForPeriod(this.ForClient.Logon.Id, period, cancellation)).Where(l => l.Classification == LedgerType.ForUsage)
        //        .SelectMany(l => l.WithDeal.Orders.OfType<BillableOrder>().Select(o => o.Adjustment)).Sum();

        //    // If we've got a deal created then apply the prepayment amount to the adjustment (sans any prior adjustments)
        //    var deal = ledger.WithDeal;
        //    var order = deal.Orders.First();
        //    order.Adjustment = -1 * this.PrepaymentAmount + (-1 * v);

        //    // Recalculate the total and see if we've gone to nill or not
        //    if (ledger.WithDeal.Total() <= 0) return null;

        //    return ledger;
        //}

        #endregion
    }
}
