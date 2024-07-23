using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A client accrual services contract. These contracts have a maximum balance allowed on them. Once the balance
    /// is equal or exceeded, they will create a new deal created for the total amount of usage off their rate card.
    /// </summary>
    [DebuggerDisplay("ID:{Id}, Accrual, Limit={MaxAccrualAmount.ToString(\"C\")}, Starting {EffectiveDate.ToString(\"d\")}")]
    public class AccrualBilling : RecurringBillingAccount
    {
        #region Fields

        private Decimal maxAccrualAmount;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccrualBilling"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected AccrualBilling()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccrualBilling"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientRef"/> the subscription is for.</param>
        /// <param name="publicKey">The alternate shared identifier of the subscription.</param>
        /// <param name="maxAccrualAmount">The required limit (minimum 50) of unpaid balance before we bill.</param>
        /// <param name="effectiveDate">The date the subscription becomes active.</param>
        /// <param name="endDate">The optional date the subscription ends.</param>
        public AccrualBilling(ClientRef client, Guid publicKey, Decimal maxAccrualAmount, DateTime effectiveDate, DateTime? endDate = null) : base(client, publicKey, effectiveDate, endDate)
        {
            if (maxAccrualAmount < 50) throw new ArgumentOutOfRangeException(nameof(maxAccrualAmount), maxAccrualAmount, $"Balance cannot be less than {50}");
            Contract.EndContractBlock();

            this.maxAccrualAmount = maxAccrualAmount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the total limit of the accrual, if any.
        /// </summary>
        /// <remarks>If not null, this value is always a positive number.</remarks>
        /// <returns>The total limit of the accrual, if any.</returns>
        public virtual Decimal MaxAccrualAmount
        {
            get
            {
                Contract.Ensures(Contract.Result<Decimal>() >= 50);

                return this.maxAccrualAmount;
            }
            set
            {
                if (value < 50) throw new ArgumentOutOfRangeException(nameof(this.MaxAccrualAmount), value, $"Balance cannot be less than {50}");
                Contract.EndContractBlock();

                this.maxAccrualAmount = value;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Determines the appropriate range of unbilled dates that should be used for billing up to the present time.
        /// </summary>
        /// <remarks>
        /// Returns 0 or 1 <see cref="BillingPeriod"/> ranging from (Last Billed + 1 day) or start of account, whichever is later and through
        ///  last closed business day and will never <see cref="BillingPeriod.WaitUntilPeriodClose">wait till close</see>
        /// </remarks>
        /// <param name="calculator">The <see cref="IClientUsageCalculator"/> that can be used to provide ledger access.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A sequence of <see cref="BillingPeriod"/> indicating each unpaid periods, by type, and indicating if the period should be closed before billing.</returns>
        public override async Task<IEnumerable<BillingPeriod>> DetermineUnpaidBillingPeriods(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken))
        {
            if (this.Id == null) return Enumerable.Empty<BillingPeriod>();

            DateTime calculatedEnd;

            var billedThrough = await calculator.NewestLedgerDate(this.Id.Value, LedgerType.ForUsage, cancellation).ConfigureAwait(false);

            // Start is +1 day from last time we billed though or start of contract, whichever is later
            var calculatedStart = billedThrough?.AddDays(1) ?? this.EffectiveDate;

            // End is the day the contract ends or the last full day, whichever is earlier
            if (this.EndDate.HasValue && DateTime.Today.AddDays(-1) > this.EndDate.Value)
            {
                calculatedEnd = this.EndDate.Value;
            }
            else
            {
                calculatedEnd = DateTime.Today.AddDays(-1);
            }

            var period = new BillingPeriod()
            {
                StartingOn = calculatedStart,
                EndingOn = calculatedEnd,
                PaidThrough = billedThrough ?? calculatedStart.AddDays(-1),
                Type = LedgerType.ForUsage,
                WaitUntilPeriodClose = false
            };

            return period.IsLogicalDateRange() ? new[] { period } : Enumerable.Empty<BillingPeriod>();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Extends the base behavior by taking the supplied <paramref name="period"/> as is.
        /// </remarks>
        public override async Task<LedgerEntry> CreateUsageBill(Guid currentUser, BillingPeriod period, IClientUsageCalculator calculator, ICostService rates, CancellationToken cancellation = default(CancellationToken))
        {
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
            if (period == null) throw new ArgumentNullException(nameof(period));
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            if (rates == null) throw new ArgumentNullException(nameof(rates));
            Contract.EndContractBlock();

            // TODO: we really need to create a method to determine if the providing billing period actually crosses temporal periods

            var usage = await calculator.Calculate(this.ForClient.UserId, period, cancellation).ConfigureAwait(false);

            var deal = new LedgerDeal(this, period, currentUser);
            var order = deal.Orders.First();

            foreach (var result in usage)
            {
                var operation = EnumExtensions.Parse<DataServiceOperation>(result.Source);

                var rateCard = await rates.CreateRateCard(operation, cancellation);

                var price = result.FindCostFromCard(rateCard);
                var product = rateCard.ForProduct;

                var line = order.CreateLine(product);
                line.Price = price.Item1;
                line.Quantity = price.Item2;
            }

            var total = order.Total();
            if (this.MaxAccrualAmount >= total) return null;

            var ledger = LedgerEntry.ForUsage(this, deal, period);
            return ledger;
        }

        #endregion
    }
}
