using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A single transaction event for a specific <see cref="Order"/>. Works as an Event Source (Fowler, PoEA) for billing.
    /// </summary>
    public class TransactionEvent : IKeyedObject<Int32?>
    {
        #region Fields

        private Order order;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionEvent"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected TransactionEvent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionEvent"/> class.
        /// </summary>
        public TransactionEvent(Order order, TransactionResult status, Guid publicKey, Decimal amountRefunded)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (amountRefunded > 0) throw new ArgumentOutOfRangeException(nameof(amountRefunded), amountRefunded, $"{nameof(amountRefunded)} must not be greater than 0");
            Contract.EndContractBlock();

            this.order = order;
            this.Status = status;
            this.PublicKey = publicKey;
            this.AmountProcessed = amountRefunded;
            this.AmountCharged = amountRefunded;
            this.DisplayValue = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionEvent"/> class.
        /// </summary>
        public TransactionEvent(PendingTransaction originalRequest, TransactionResult status, Decimal amountCharged, Decimal? amountProcessed, String displayValue)
        {
            if (originalRequest == null) throw new ArgumentNullException(nameof(originalRequest));
            if (amountCharged < 0) throw new ArgumentOutOfRangeException(nameof(amountCharged), amountCharged, $"{nameof(amountCharged)} must be at least 0");
            if (status == TransactionResult.Approved && amountProcessed == null) throw new ArgumentOutOfRangeException(nameof(amountProcessed), null, $"{nameof(amountProcessed)} cannot be null when {nameof(status)} is {TransactionResult.Approved}");
            Contract.EndContractBlock();

            this.order = originalRequest.Order;
            this.Status = status;
            this.PublicKey = originalRequest.PublicKey;
            this.AmountProcessed = amountProcessed;
            this.AmountCharged = amountCharged;
            this.DisplayValue = (displayValue ?? String.Empty).Trim().Left(4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public Int32? Id { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Order"/> the charge was applied to.
        /// </summary>
        public virtual Order Order
        {
            get => this.order;
            protected set => this.order = value;
        }

        /// <summary>
        /// Gets the status of the charge event.
        /// </summary>
        public virtual TransactionResult Status { get; protected set; }

        /// <summary>
        /// Gets or sets the unique public key for the current instance.
        /// </summary>
        /// <remarks>
        /// Public key values are aligned to other entities on other contexts via this common identifier.
        /// In example, a charge related to a auth_capture via a matching value.
        /// </remarks>
        /// <value>The unique public key for the current instance.</value>
        public virtual Guid PublicKey { get; protected set; }

        /// <summary>
        /// Gets or sets the total amount to be charged.
        /// </summary>
        /// <value>The total amount to be charged.</value>
        public virtual Decimal AmountCharged { get; protected set; }

        /// <summary>
        /// Gets or sets the total amount actually charged.
        /// </summary>
        /// <value>The total amount actually charged. A null value means no amount was processed.</value>
        public virtual Decimal? AmountProcessed { get; protected set; }

        /// <summary>
        /// Gets the display value for the card (last 4 digits).
        /// </summary>
        /// <value>The display value for the card (last 4 digits).</value>
        public virtual String DisplayValue { get; protected set; }

        #endregion
    }
}