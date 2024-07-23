using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Super type for all automated  billing account contracts in AA. Each account describes an active period,
    /// the recurrence, and the client the account is applicable to.
    /// </summary>
    public abstract class RecurringBillingAccount : IKeyedObject<Int32?>
    {
        #region Fields

        private DateTime effectiveDate;
        private DateTime? endDate;
        private ClientRef client;
        private Guid publicKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringBillingAccount"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected RecurringBillingAccount()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringBillingAccount"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ClientRef"/> the account is for.</param>
        /// <param name="publicKey">The alternate shared identifier of the account.</param>
        /// <param name="effectiveDate">The date the account becomes active.</param>
        /// <param name="endDate">The optional date the account ends.</param>
        protected RecurringBillingAccount(ClientRef client, Guid publicKey, DateTime effectiveDate, DateTime? endDate = null)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.Ensures(this.client != null);
            Contract.EndContractBlock();

            this.effectiveDate = effectiveDate.Date;
            this.endDate = endDate == null ? null : new DateTime?(endDate.Value.Date);
            this.client = client;
            this.publicKey = publicKey;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public virtual Int32? Id { get; protected set; }

        /// <summary>
        /// Gets the date this subscription becomes active. This value is always in
        /// <see cref="DateTimeExtensions.BillingZone"></see> assumed time.
        /// </summary>
        /// <value>The date this subscription becomes active.</value>
        public virtual DateTime EffectiveDate
        {
            get => this.effectiveDate;
            protected set
            {
                value = value.Date;
                this.effectiveDate = value.AssumeBillingZone();
            }
        }

        /// <summary>
        /// Gets the possible date that the subscription should end. This value is always in
        /// <see cref="DateTimeExtensions.BillingZone"></see> assumed time.
        /// </summary>
        /// <remarks>For subscriptions that do not expire this value should be null.</remarks>
        /// <value>The possible date that the subscription should end.</value>
        public virtual DateTime? EndDate
        {
            get => this.endDate;
            protected set
            {
                value = value?.AssumeBillingZone().Date;

                this.endDate = value;
            }
        }

        /// <summary>
        /// Get the <see cref="ClientRef"/> for the current subscription.
        /// </summary>
        /// <value>The <see cref="ClientRef"/> for the current subscription.</value>
        public virtual ClientRef ForClient
        {
            get => this.client;
            protected set => this.client = value;
        }

        /// <summary>
        /// Indicates whether the service account has a custom processing procedure. Used as
        /// a hint for the automated billing system.
        /// </summary>
        public virtual Boolean SpecialProcessing { get; set; }

        /// <summary>
        /// Provides the alternate public key value used to represent this entity.
        /// </summary>
        /// <value>The alternate public key value used to represent this entity.</value>
        public virtual Guid PublicKey
        {
            get => this.publicKey;
            protected set => this.publicKey = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines the appropriate range of unbilled dates that should be used for billing up to the present time.
        /// </summary>
        /// <param name="calculator">The <see cref="IClientUsageCalculator"/> that can be used to provide ledger access.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A sequence of <see cref="BillingPeriod"/> indicating each unpaid periods, by type, and indicating if the period should be closed before billing.</returns>
        public abstract Task<IEnumerable<BillingPeriod>> DetermineUnpaidBillingPeriods(IClientUsageCalculator calculator, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Marks the current contract to be expired at the close of a billing date, if not already.
        /// </summary>
        /// <remarks>
        /// A contract that already has an <see cref="EndDate"/> value will not be changed.
        /// </remarks>
        /// <param name="expire">
        /// The <see cref="DateTime"/> (at <see cref="DateTime.Date"/> granularity) the contract should end. 
        /// This value is always evaluated as <see cref="DateTimeExtensions.ToBillingZone(DateTime)"></see> converted time.</param>
        public virtual void Expire(DateTime expire)
        {
            if (this.EndDate.HasValue) return;
            if (!this.IsValidForDate(expire)) throw new InvalidOperationException($"New expiration date {expire} for contract {this.Id} must be a valid range for the contract.");

            this.EndDate = expire.ToBillingZone().Date;
        }

        /// <summary>
        /// Evaluates if the current instance is valid for the indicated <paramref name="dateAndTime"/>
        /// </summary>
        /// <returns>
        /// All dates will be evaluated as <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time zone</see> by <see cref="DateTime.Date">date</see> only.
        /// </returns>
        /// <param name="dateAndTime">The <seealso cref="DateTime"/> indicating when to check if the current contract is valid during.</param>
        /// <returns>True if it was valid; otherwise false.</returns>
        public virtual Boolean IsValidForDate(DateTime dateAndTime)
        {
            dateAndTime = dateAndTime.ToBillingZone().Date;

            return this.EffectiveDate <= dateAndTime && (this.EndDate == null || this.EndDate.Value >= dateAndTime);
        }

        /// <summary>
        /// Evaluates if the current instance is valid for the entirety of the indicated <paramref name="period"/>.
        /// </summary>
        /// <returns>
        /// All times will be evaluated as <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time zone</see> by <see cref="DateTime.Date">date</see> only.
        /// </returns>
        /// <param name="period">The <seealso cref="DateSpan"/> indicating when to check if the current subscription is valid during.</param>
        /// <returns>True if it was valid; otherwise false.</returns>
        public virtual Boolean IsValidForPeriod(DateSpan period)
        {
            if (period == null) throw new ArgumentNullException(nameof(period));
            Contract.EndContractBlock();

            var dateAndTimes = new[] { period.StartingOn.ToBillingZone().Date, period.EndingOn.ToBillingZone().Date };
            var result = dateAndTimes.All(this.IsValidForDate);

            return result;
        }

        /// <summary>Attempts to create a bill for usage calculations for the indicated <paramref name="period"/>.</summary>
        /// <param name="currentUser">The identifier of the user creating the billing.</param>
        /// <param name="period">The <see cref="BillingPeriod"/> to craft a bill for. See <see cref="DetermineUnpaidBillingPeriods"/> to determine values.</param>
        /// <param name="calculator">A <see cref="IClientUsageCalculator"/> that understands the client usage and ledger system.</param>
        /// <param name="rates">A <see cref="ICostService"/> that is used to manage the rate cards for the current account.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>If the accrued balance exceeds the allowed amount, the <see cref="LedgerEntry"/> for the account; otherwise null.</returns>
        public virtual async Task<LedgerEntry> CreateUsageBill(Guid currentUser, BillingPeriod period, IClientUsageCalculator calculator, ICostService rates, CancellationToken cancellation = default(CancellationToken))
        {
            if (currentUser == null) throw new ArgumentNullException(nameof(currentUser));
            if (period == null) throw new ArgumentNullException(nameof(period));
            if (period.Type != LedgerType.ForUsage) throw new ArgumentOutOfRangeException(nameof(period), period.Type, $"The supplied billing period {period} must be {LedgerType.ForUsage}");
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            if (rates == null) throw new ArgumentNullException(nameof(rates));
            Contract.EndContractBlock();

            if (this.Id == null) return null;

            // TODO: we really need to create a method to determine if the providing billing period actually crosses temporal periods

            var range = period.ToOutstandingRange();
            if (!range.IsLogicalDateRange()) return null;

            var usage = await calculator.Calculate(this.ForClient.UserId, range, cancellation).ConfigureAwait(false);

            var deal = new LedgerDeal(this, period, currentUser);
            var order = deal.Orders.First();

            foreach (var result in usage)
            {
                var operation = EnumExtensions.Parse<DataServiceOperation>(result.Source);

                var rateCard = await rates.CreateRateCard(operation, cancellation).ConfigureAwait(false);

                var price = result.FindCostFromCard(rateCard);
                var product = rateCard.ForProduct;

                var line = order.CreateLine(product);
                line.Price = price.Item1;
                line.Quantity = price.Item2;
            }

            deal.Amount = deal.Total();

            if (deal.Total() <= 0) return null;

            var ledger = LedgerEntry.ForUsage(this, deal, period);
            return ledger;
        }

        #endregion
    }
}
