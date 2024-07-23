using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A client usage only services contract. These contracts have no prepayment performed at the 
    /// start of a period. In addition, they may optionally may be allowed a maximum balance to accrue
    /// which will enforce a bill to be created during the incomplete period. Irrespective, at the
    /// close of the period a new bill will be created if any balance exists.
    /// </summary>
    [DebuggerDisplay("ID:{" + nameof(Id) + "}, Usage Only, Recurrence={" + nameof(Recurrence) + "}, Starting {" + nameof(EffectiveDate) + ".ToString(\"d\")}")]
    public class UsageBilling : RecurringBillingAccount
    {
        #region Fields

        private Decimal? maxBalance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageBilling"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected UsageBilling()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageBilling"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientRef"/> the usage billing is for.</param>
        /// <param name="publicKey">The alternate shared identifier of the billing.</param>
        /// <param name="effectiveDate">The date the usage billing becomes active.</param>
        /// <param name="endDate">The optional date the usage billing ends.</param>
        public UsageBilling(ClientRef client, Guid publicKey, DateTime effectiveDate, DateTime? endDate = null) : base(client, publicKey, effectiveDate, endDate)
        {
        }

        #endregion

        #region Properties

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
        /// Indicates the maximum limit, if any, for total usage during a billing period. A
        /// null value indicates an account that has unlimited account balance in a billing period.
        /// </summary>
        public Decimal? MaxBalance
        {
            get { return this.maxBalance; }
            protected set
            {
                if (value.HasValue && value <= 0) throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(this.MaxBalance)} must be null or greater than 0");
                this.maxBalance = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Establishes a new limit, if any, for the contract.
        /// </summary>
        /// <param name="limit">The limit, in currency, of the max allowed usage before billing may occur.</param>
        public virtual void ApplyLimit(Decimal? limit)
        {
            this.MaxBalance = limit;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Determines the appropriate range of unbilled dates that should be used for billing up to the present time.
        /// </summary>
        /// <remarks>
        /// Returns 0 or 1 <see cref="BillingPeriod"/> ranging from (Last Billed + 1 day) or start of account, whichever is later close of period.
        /// </remarks>
        /// <param name="calculator">The <see cref="IClientUsageCalculator"/> that can be used to provide ledger access.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A sequence of <see cref="BillingPeriod"/> indicating each unpaid periods, by type, and indicating if the period should be closed before billing.</returns>
        public override async Task<IEnumerable<BillingPeriod>> DetermineUnpaidBillingPeriods(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken))
        {
            if (this.Id == null) return Enumerable.Empty<BillingPeriod>();

            DateTime calculatedStart;
            DateTime calculatedEnd;

            var billedThrough = await calculator.NewestLedgerDate(this.Id.Value, LedgerType.ForUsage, cancellation).ConfigureAwait(false);

            // Start is +1 day from last time we billed though or start of contract, whichever is later
            if (billedThrough == null)
            {
                calculatedStart = this.EffectiveDate;
            }
            else
            {
                calculatedStart = billedThrough.Value.AddDays(1);
            }

            // If the next unbilled day falls outside the contract end, then we're current
            if (this.EndDate.HasValue && calculatedStart > this.EndDate.Value) return Enumerable.Empty<BillingPeriod>();

            // End is the day the contract ends or the last day of the period, whichever is earlier
            var endOfPeriod = TemporalHelper.CalculateEndOfPeriod(this.EffectiveDate, this.EndDate, calculatedStart, 1, this.Recurrence);

            if (this.EndDate.HasValue && endOfPeriod.EndingOn > this.EndDate.Value)
            {
                calculatedEnd = this.EndDate.Value;
            }
            else
            {
                calculatedEnd = endOfPeriod.EndingOn;
            }

            var period = new BillingPeriod()
            {
                StartingOn = endOfPeriod.StartingOn,
                EndingOn = calculatedEnd,
                PaidThrough = billedThrough ?? endOfPeriod.StartingOn.AddDays(-1),
                Type = LedgerType.ForUsage,
                WaitUntilPeriodClose = (this.MaxBalance == null || calculatedEnd >= DateTime.Today)
            };

            return period.IsLogicalDateRange() ? new[] { period } : Enumerable.Empty<BillingPeriod>();
        }

        #endregion
    }
}
