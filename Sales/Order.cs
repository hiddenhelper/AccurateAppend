using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A client order for services or payment.
    /// </summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + ("}, Status = {" + nameof(Status) + "}"))]
    public abstract class Order : IKeyedObject<Int32?>, IEquatable<Order>, ISupportInitializeNotification
    {
        #region Fields

        private BillingContract billingInfo;
        private DateTime createdDate;
        private DateTime completedDate;
        private OrderStatus status;
        private String publicKey;
        private readonly ProductLineCollection lines;
        private readonly ObservableCollection<TransactionEvent> transactions;
        private readonly ObservableCollection<PendingTransaction> pendingTransactions;
        private DealBinder deal;
        private OrderRuntime processingData;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected Order()
        {
            this.lines = new ProductLineCollection(this);
            this.createdDate = DateTime.UtcNow;
            this.completedDate = new DateTime(1900, 1, 1);
            this.status = OrderStatus.Open;
            this.publicKey = Guid.NewGuid().ToString();
            this.transactions = new ObservableCollection<TransactionEvent>();
            this.pendingTransactions = new PendingTransactionCollection(this);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="Deal"/> that contains this order instance.</param>
        protected Order(DealBinder deal) : this()
        {
            if (deal == null) throw new ArgumentNullException(nameof(deal));
            Contract.EndContractBlock();

            this.deal = deal;
            this.IsInitialized = true;
            this.processingData = new OrderRuntime(this);
            deal.Orders.Add(this);

            this.billingInfo = new BillingContract(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="Deal"/> that contains this order instance.</param>
        /// <param name="publicKey">The unique <paramref name="publicKey"/> value.</param>
        protected Order(DealBinder deal, Guid publicKey) : this(deal)
        {
            this.publicKey = publicKey.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public virtual Int32? Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="OrderStatus"/> for the current instance.
        /// </summary>
        /// <value>The <see cref="OrderStatus"/> for the current instance.</value>
        public virtual OrderStatus Status
        {
            get => this.status;
            protected set => this.status = value;
        }

        /// <summary>
        /// Gets or sets the unique public key for the current instance.
        /// </summary>
        /// <remarks>
        /// Public key values are aligned to other entities on other contexts via this common identifier.
        /// In example, an order related to a job via a matching value.
        /// </remarks>
        /// <value>The unique public key for the current instance.</value>
        public virtual String PublicKey
        {
            get => this.publicKey;
            protected set => this.publicKey = value;
        }

        /// <summary>
        /// Gets the date the current instance was created.
        /// </summary>
        /// <value>The date the current instance was created.</value>
        public virtual DateTime CreatedDate
        {
            get => this.createdDate;
            protected set => this.createdDate = value;
        }

        /// <summary>
        /// Gets the date the current instance was completed.
        /// </summary>
        /// <value>The date the current instance was completed.</value>
        public virtual DateTime CompletedDate
        {
            get => this.completedDate;
            private set => this.completedDate = value;
        }

        /// <summary>
        /// Gets the <see cref="Deal"/> associated with the current instance.
        /// </summary>
        /// <value>The <see cref="Deal"/> associated with the current instance.</value>
        public virtual DealBinder Deal
        {
            get => this.deal;
            protected internal set => this.deal = value;
        }

        /// <summary>
        /// Gets the <see cref="OrderRuntime"/> extending this order instance.
        /// </summary>
        public virtual OrderRuntime Processing
        {
            get => this.processingData;
            protected set => this.processingData = value;
        }

        /// <summary>
        /// Gets the current set of <see cref="ProductLine"/> instances associated with this order.
        /// </summary>
        /// <value>The current set of <see cref="ProductLine"/> instances associated with this order.</value>
        public virtual IList<ProductLine> Lines => this.lines;

        /// <summary>
        /// Gets or sets the adjustment amount for the current order (default 0.00).
        /// A positive value is calculated as a minimum value for the order.
        /// A negative value is calculated as a discount value on the total.
        /// </summary>
        /// <remarks>
        /// See the <see cref="Total"/> property for understanding how this effects
        /// calculations.
        /// </remarks>
        public virtual Decimal OrderMinimum { get; set; }

        /// <summary>
        /// Gets the collection of completed transactions made against this instance.
        /// </summary>
        /// <value>The collection of completed transactions made against this instance.</value>
        public virtual ICollection<TransactionEvent> Transactions
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<TransactionEvent>>() != null);

                return this.transactions;
            }
        }

        /// <summary>
        /// Gets the collection of requested transactions made for this instance.
        /// </summary>
        /// <value>The collection of requested transactions made for this instance.</value>
        public virtual ICollection<PendingTransaction> PendingTransactions
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<PendingTransaction>>() != null);

                return this.pendingTransactions;
            }
        }

        /// <summary>
        /// Contains the billing contract configuration for the current order.
        /// </summary>
        public virtual BillingContract Bill
        {
            get => this.billingInfo;
            protected set => this.billingInfo = value;
        }

        /// <summary>
        /// Temporary location. Entity Splitting doesn't allow further associations of the dependent type.
        /// </summary>
        public virtual BillContent Content { get; set; }

        #endregion

        #region ISupportInitializeNotification Members

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void ISupportInitialize.BeginInit()
        {
            this.IsInitialized = false;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void ISupportInitialize.EndInit()
        {
            this.IsInitialized = true;

            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a value indicating whether the component is initialized.
        /// </summary>
        /// <returns>
        /// true to indicate the component has completed initialization; otherwise, false. 
        /// </returns>
        Boolean ISupportInitializeNotification.IsInitialized => this.IsInitialized;

        /// <summary>
        /// Gets a value indicating whether the component is initialized.
        /// </summary>
        /// <returns>
        /// true to indicate the component has completed initialization; otherwise, false. 
        /// </returns>
        protected virtual Boolean IsInitialized { get; private set; }

        /// <summary>
        /// Event raised when the component instance signals it has been <see cref="ISupportInitializeNotification.IsInitialized">initialized</see>.
        /// </summary>
        public event EventHandler Initialized;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the calculated total for all the current order items.
        /// </summary>
        /// <remarks>
        /// This value is affected by the presence of a non zero value in the
        /// <see cref="OrderMinimum"/> property. If <seealso cref="OrderMinimum"/> is greater than
        /// zero and greater than calculated total, the minimum adjusted value will be returned
        /// instead. If <seealso cref="OrderMinimum"/> is a negative value then the total will
        /// be the difference of the values or 0, whichever is greater.
        /// </remarks>
        /// <returns>The calculated total for all the current order items.</returns>
        public virtual Decimal Total()
        {
            var subTotal = this.Lines.Sum(i => i.Total());
            if (this.OrderMinimum == 0) return subTotal;

            if (this.OrderMinimum > 0)
            {
                subTotal = Math.Max(subTotal, this.OrderMinimum);
            }
            else
            {
                subTotal = subTotal + this.OrderMinimum;
                subTotal = Math.Max(subTotal, 0);
            }

            return subTotal;
        }

        /// <summary>
        /// Method called when a parent <see cref="Deal"/> is transitioned to the <see cref="DealStatus">Canceled</see> status.
        /// </summary>
        /// <remarks>
        /// When an order is canceled, the <see cref="PublicKey"/> value will be automatically changed to a new value.
        /// </remarks>
        protected internal void Cancel()
        {
            Contract.Ensures(Contract.OldValue(this.Status) == OrderStatus.Open
                ? this.Status == OrderStatus.Canceled
                : this.Status == Contract.OldValue(this.Status));

            if (this.Status == OrderStatus.Open)
            {
                this.PublicKey = Guid.NewGuid().ToString();
                this.Status = OrderStatus.Canceled;
            }
        }

        /// <summary>
        /// Called whenever a new <see cref="ProductLine"/> is added or when an existing instance is changed.
        /// </summary>
        /// <remarks>
        /// The default implementation performs no work. When overriding, the <see cref="IsInitialized"/> property should
        /// be checked. If not initialized, no work should be performed.
        /// </remarks>
        /// <param name="line">The <see cref="ProductLine"/> instance that is new or changed.</param>
        protected internal virtual void Validate(ProductLine line)
        {
            // No op
        }

        #endregion

        #region Event Sinks

        /// <summary>
        /// Called when a <see cref="DealBinder"/> is declined or expired.
        /// </summary>
        /// <remarks>
        /// If the current instance is an in process order with a bill the <see cref="Bill"/> and <see cref="BillingContract.ContractType"/>
        /// will be set to null and the <see cref="BillContent.DraftCleared"/> event will be raised.
        /// </remarks>
        protected internal virtual void OnDealDeclined()
        {
            this.Content.Clear();
            this.Content = null;
            this.Bill.ContractType = null;
        }

        /// <summary>
        /// Called when a <see cref="DealBinder"/> is approved.
        /// </summary>
        /// <remarks>
        /// If the current <see cref="BillingContract.ContractType"/> is invoice, the <see cref="Complete"/> event will be raised.
        /// </remarks>
        protected internal virtual void OnDealApproved()
        {
            if (this.Bill.ContractType == ContractType.Invoice) this.OnComplete(OrderStatus.Billed);
        }

        #endregion

        #region Events

        /// <summary>
        /// Sets the state for the current instance to completed and raises the <see cref="Complete"/> event.
        /// </summary>
        /// <remarks>
        /// Implementors should make sure to set the <see cref="Status"/> property to the appropriate value prior to calling this method.
        /// </remarks>
        protected virtual void OnComplete(OrderStatus statusToSet)
        {
            // Makes sure the Deal is loaded so fired events will be handled by Deal
            Trace.TraceInformation($"Completing order {this.Id} for deal {this.Deal.Id}");

            this.Status = statusToSet;
            this.CompletedDate = DateTime.UtcNow;
            this.Content.Complete();

            this.Complete?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised when the current instance has been completed (Billed or Processed).
        /// </summary>
        public event EventHandler Complete;

        #endregion

        #region IEquatable<Order> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(Order other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!this.Id.HasValue) return false;

            return this.Id == other.Id;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as Order);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion
    }
}
