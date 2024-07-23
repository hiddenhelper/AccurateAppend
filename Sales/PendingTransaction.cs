using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// An outstanding transaction request for a specific <see cref="Order"/>. Works as an temporary Memento (GoF, Design Patterns) for billing.
    /// A <see cref="PendingTransaction"/> will eventually be converted to a <see cref="TransactionEvent"/> and disappear.
    /// </summary>
    public class PendingTransaction : IKeyedObject<Int32?>
    {
        #region Fields

        private Order order;
        private String displayValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingTransaction"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected PendingTransaction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingTransaction"/> class.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create a request for.</param>
        /// <param name="publicKey">The key that is used to uniquely identify this request.</param>
        /// <param name="creditCard">The <see cref="CreditCardRef"/> to apply the charge request to.</param>
        /// <param name="amountRequested">The amount to create a request for.</param>
        public PendingTransaction(Order order, Guid publicKey, CreditCardRef creditCard, Decimal amountRequested)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (creditCard == null) throw new ArgumentNullException(nameof(creditCard));
            if (amountRequested < 0) throw new ArgumentOutOfRangeException(nameof(amountRequested), amountRequested, $"{nameof(amountRequested)} must be at least 0");
            Contract.EndContractBlock();

            this.order = order;
            this.PublicKey = publicKey;
            this.AmountRequested = amountRequested;
            this.displayValue = creditCard.DisplayValue;
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
            protected internal set => this.order = value;
        }
        
        /// <summary>
        /// Gets or sets the unique public key for the current instance.
        /// </summary>
        /// <remarks>
        /// Public key values are aligned to other entities on other contexts via this common identifier.
        /// In example, a requested charge related to a requested auth_capture via a matching value.
        /// </remarks>
        /// <value>The unique public key for the current instance.</value>
        public Guid PublicKey { get; protected set; }

        /// <summary>
        /// Gets or sets the total amount to be charged.
        /// </summary>
        /// <value>The total amount to be charged.</value>
        public Decimal AmountRequested { get; protected set; }

        /// <summary>
        /// Gets the display value for the card (last 4 digits).
        /// </summary>
        /// <value>The display value for the card (last 4 digits).</value>
        public virtual String DisplayValue
        {
            get => this.displayValue;
            protected set => this.displayValue = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Completes the current request into the recorded <see cref="TransactionEvent"/> values.
        /// </summary>
        /// <param name="status">The <see cref="TransactionResult"/> status that resulted from the original request.</param>
        /// <param name="amountCharged">The amount that was attempted to be charged in the request.</param>
        /// <param name="amountProcessed">The amount that was actually processed (this may be 0).</param>
        /// <returns>The matching <see cref="TransactionEvent"/> for the completed request.</returns>
        public virtual TransactionEvent Complete(TransactionResult status, Decimal amountCharged, Decimal? amountProcessed)
        {
            var chargeEvent = new TransactionEvent(this, status, amountCharged, amountProcessed, this.displayValue);
            return chargeEvent;
        }

        #endregion
    }
}