using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// The basic standard <see cref="Order"/> that can be added to a <see cref="DealBinder"/> and charged.
    /// </summary>
    public class BillableOrder : Order
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BillableOrder"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected BillableOrder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillableOrder"/> class using a randomly generated key.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="DealBinder"/> that contains this order instance.</param>
        public BillableOrder(DealBinder deal) : this(deal, Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class using the provided <paramref name="publicKey"/>.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="DealBinder"/> that contains this order instance.</param>
        /// <param name="publicKey">The unique public key value.</param>
        public BillableOrder(DealBinder deal, Guid publicKey) : base(deal, publicKey)
        {
            if (deal.Status.CanBeEdited()) return;

            deal.Orders.Remove(this);
            throw new InvalidOperationException($"The deal {deal.Id} is not editable and cannot have billable orders added to it");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the current order should allow automatic billing processes.
        /// </summary>
        /// <value>True if auto-billing is enabled; otherwise false.</value>
        public virtual Boolean PerformAutoBilling { get; set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the calculated total for all the current order items.
        /// </summary>
        /// <remarks>
        /// This value is affected by the presence of a non zero value in the
        /// <see cref="Order.OrderMinimum"/> property. If <seealso cref="Order.OrderMinimum"/> is greater than
        /// zero and greater than calculated total, the minimum adjusted value will be returned
        /// instead. If <seealso cref="Order.OrderMinimum"/> is a negative value then the total will
        /// be the difference of the values or 0, whichever is greater.
        /// </remarks>
        /// <returns>The calculated total for all the current order items.</returns>
        public override Decimal Total()
        {
            if (this.Status == OrderStatus.WriteOff) return 0;

            return base.Total();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the alteration of the <see cref="Order.PublicKey"/> to a new value on an open order.
        /// </summary>
        /// <param name="newKey">The new unique identifier for the public key.</param>
        public virtual void ResetKey(Guid newKey)
        {
            if (this.Status != OrderStatus.Open) throw new InvalidOperationException($"Only {OrderStatus.Open} orders can alter their public key. Order {this.Id} has a status = {this.Status}");
            Contract.EndContractBlock();

            this.PublicKey = newKey.ToString();
        }

        /// <summary>
        /// Crafts a new <see cref="PendingTransaction"/> that is used to track the outstanding requests made to a payment processing system.
        /// Ensures that the <paramref name="amountRequested"/> is within the outstanding total of the current order.
        /// </summary>
        /// <param name="creditCard">The <see cref="CreditCardRef"/> to apply the charge request to.</param>
        /// <param name="amountRequested">The amount to create a request for.</param>
        /// <returns>A new <see cref="PendingTransaction"/> that can be used to track a request to a payment processor.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="amountRequested"/> exceeds the outstanding total of the current order.</exception>
        public virtual PendingTransaction CreateRequest(CreditCardRef creditCard, Decimal amountRequested)
        {
            if (creditCard == null) throw new ArgumentNullException(nameof(creditCard));
            if (amountRequested < 0) throw new ArgumentOutOfRangeException(nameof(amountRequested), amountRequested, $"{nameof(amountRequested)} must be at least 0");
            Contract.Ensures(Contract.Result<PendingTransaction>() != null);
            Contract.EndContractBlock();

            // Remove any partial payments from total
            var remaining = this.OutstandingTotal();

            // Can't charge more than is left
            if (amountRequested > remaining) throw new InvalidOperationException($"Total charge of {amountRequested:C} exceeds remaining order total of {remaining:C}");

            var request = new PendingTransaction(this, Guid.NewGuid(), creditCard, amountRequested);
            this.PendingTransactions.Add(request);

            return request;
        }

        /// <summary>
        /// Posts the provided set of <paramref name="charges"/> to the current instance. Once the total amount
        /// posted (<see cref="TransactionEvent.AmountCharged"/>) matches the <see cref="Total"/>, the order
        /// will be Complete and the <see cref="Order.Complete"/> event is raised. All <paramref name="charges"/>
        /// must be part of the same <see cref="PendingTransaction"/>, which is removed when this method completes.
        /// </summary>
        /// <param name="charges">The sequence of <see cref="TransactionEvent"/> to post to this order.</param>
        public virtual void PostCharge(params TransactionEvent[] charges)
        {
            if (this.Status != OrderStatus.Open) throw new InvalidOperationException($"{nameof(BillableOrder)} is not in the {OrderStatus.Open} status");
            if (charges == null) throw new ArgumentNullException(nameof(charges));
            Contract.EndContractBlock();

            if (!charges.Any()) return;

            // Apply the transactions
            foreach (var charge in charges)
            {
                if (!charge.Order.Equals(this)) throw new ArgumentOutOfRangeException(nameof(charges), charge.Order.PublicKey, $"{nameof(TransactionEvent)} {charge.PublicKey} does not belong to {nameof(BillableOrder)} {this.PublicKey}");
                if (charge.Status == TransactionResult.Refunded) throw new ArgumentOutOfRangeException(nameof(charges), charges, $"Posted charges cannot be type {TransactionResult.Refunded}");

                var originalRequest = this.PendingTransactions.FirstOrDefault(t => t.PublicKey == charge.PublicKey);
                if (originalRequest == null) throw new InvalidOperationException($"The posted transactions {charge.PublicKey} lacks a matching pending transaction request. Was this posted to the correct order?");

                this.Transactions.Add(charge);
            }

            // Now remove pending transactions
            this.PendingTransactions.RemoveRange(this.PendingTransactions.Where(p => charges.Select(c => c.PublicKey).Contains(p.PublicKey)));
            
            var capturedEvents = this.Transactions.Where(c => c.Status.IsCaptured());
            var amountCharged = capturedEvents.Sum(c => c.AmountCharged); // we sum the amount charged vs captured as void should cause this to go over the total to close
            var total = this.Total();

            if (amountCharged >= total)
            {
                this.OnComplete(OrderStatus.Billed);
            }
        }

        #endregion
    }
}