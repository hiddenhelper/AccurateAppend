using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// An Accurate Append deal composing a set of <see cref="Order">Orders</see> for a client.
    /// </summary>
    [DebuggerDisplay("Id = {" + nameof(Id) + ("}, Status = {" + nameof(Status) + "}"))]
    public abstract class DealBinder : IEquatable<DealBinder>
    {
        #region Fields

        private DateTime followupDate;
        private DateTime createdDate;
        private DealStatus status;
        private readonly OrderCollection orders;
        private String description;
        private String title;
        private ISet<Audit> notes;
        private String instructions;
        private Decimal amount;
        private Guid ownerId;
        private ClientRef client;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DealBinder"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected DealBinder()
        {
            this.orders = new OrderCollection(this);
            this.orders.OrdersComplete += this.OnOrdersComplete;
            this.notes = new WriteOnlySet<Audit>();
            this.createdDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DealBinder"/> class.
        /// </summary>`
        /// <param name="client">The <see cref="Client"/> the deal is for.</param>
        protected DealBinder(ClientRef client) : this()
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.EndContractBlock();

            this.client = client;
            this.followupDate = this.createdDate.AddDays(14);
            this.description = $"New Deal for {client.UserName}, {DateTime.Today.ToShortDateString()}";
            this.title = this.description;
            this.instructions = String.Empty;
            this.status = DealStatus.InProcess;
            this.ownerId = client.OwnerId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DealBinder"/> class.
        /// </summary>`
        /// <param name="client">The <see cref="Client"/> the deal is for.</param>
        /// <param name="owner">The security identifier of the user that owns the deal.</param>
        protected DealBinder(ClientRef client, Guid owner) : this(client)
        {
            this.OwnerId = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current instance.
        /// </summary>
        /// <value>The identifier for the current instance.</value>
        public virtual Int32? Id { get; protected set; }

        /// <summary>
        /// Gets or sets the deal actual current total. (When initially created, the estimated amount).
        /// </summary>
        /// <value>The current deal amount total.</value>
        public virtual Decimal Amount
        {
            get
            {
                Contract.Ensures(Contract.Result<Decimal>() >= 0);
                return this.amount;
            }
            set
            {
                if (value < 0) value = 0;

                this.amount = value;
            }
        }

        /// <summary>
        /// Gets the status of the current instance.
        /// </summary>
        /// <value>The status of the current instance.</value>
        public virtual DealStatus Status
        {
            get => this.status;
            protected set => this.status = value;
        }

        /// <summary>
        /// Gets or sets the title of the current instance.
        /// </summary>
        /// <value>The title of the current instance.</value>
        public virtual String Title
        {
            get => this.title;
            set
            {
                value = (value ?? String.Empty);
                this.title = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional description of the current instance.
        /// </summary>
        /// <value>The optional description of the current instance.</value>
        public virtual String Description
        {
            get => this.description;
            set => this.description = value ?? String.Empty;
        }

        /// <summary>
        /// Gets or sets the optional processing instructions of the current instance.
        /// </summary>
        /// <value>The optional processing instructions of the current instance.</value>
        public virtual String Instructions
        {
            get => this.instructions;
            set => this.instructions = value ?? String.Empty;
        }

        /// <summary>
        /// Gets the date the current instance was created.
        /// </summary>
        /// <value>The date the current instance was created.</value>
        public virtual DateTime CreatedDate
        {
            get => this.createdDate;
            protected set => this.createdDate = value.Coerce();
        }

        /// <summary>
        /// Gets the date the current instance needs to be followed up by.
        /// </summary>
        /// <value>The date the current instance needs to be followed up by.</value>
        public virtual DateTime FollowupDate
        {
            get => this.followupDate;
            protected set
            {
                value = value.Coerce();
                this.followupDate = value;
            }
        }

        /// <summary>
        /// Gets the identifier of the user that owns the deal.
        /// </summary>
        /// <value>The identifier of the user that owns the deal.</value>
        public virtual Guid OwnerId
        {
            get => this.ownerId;
            private set => this.ownerId = value;
        }

        /// <summary>
        /// Gets the <see cref="ClientRef"/> that the deal is for.
        /// </summary>
        /// <value>The <see cref="ClientRef"/> that the deal is for.</value>
        public virtual ClientRef Client
        {
            get => this.client;
            protected set => this.client = value;
        }

        /// <summary>
        /// Gets the list of <see cref="Order">Orders</see> that the deal contains.
        /// </summary>
        /// <value>The list of <see cref="Order">Orders</see> that the deal contains.</value>
        public virtual IList<Order> Orders => this.orders;

        /// <summary>
        /// Gets the list of <see cref="Audit"/> entries that describe the deal history.
        /// </summary>
        /// <value>The list of <see cref="Audit"/> entries that describe the deal history.</value>
        public virtual ICollection<Audit> Notes
        {
            get => this.notes;
            protected set
            {
                value = value ?? new Audit[0];

                this.notes = new WriteOnlySet<Audit>(new HashSet<Audit>(value));
            }
        }

        /// <summary>
        /// Indicates whether the current deal should raise administrative notifications
        /// of status changes.
        /// </summary>
        /// <value>True if notifications should be suppressed; otherwise false.</value>
        public virtual Boolean SuppressNotifications { get; set; }

        /// <summary>
        /// Gets the date the current instance was completed (that is billed).
        /// </summary>
        /// <value>The date the current instance was completed, in UTC.</value>
        public virtual DateTime? CompletedDate { get; protected set; }

        #endregion

        #region Methods

        ///// <summary>
        ///// Indicates whether the current instance can be issued a refund.
        ///// </summary>
        ///// <remarks>
        ///// In order to be refunded the deal must be complete and have orders that are billable in excess of
        ///// orders that have been refunded (open refunds are not included).
        ///// </remarks>
        ///// <returns>True if the instance can be refunded still; otherwise false.</returns>
        //public virtual Boolean CanIssueRefund()
        //{
        //    if (this.Status != DealStatus.Complete) return false;
        //    if (!this.Orders.Any()) return false;

        //    return this.TotalRefunded() + this.TotalBilled() > 0;
        //}

        /// <summary>
        /// Transitions the current instance into a permanently Canceled state.
        /// </summary>
        /// <param name="history">The <see cref="Audit"/> explaining the cancellation of the current deal.</param>
        /// <exception cref="InvalidOperationException">The current instance is not in a status that can be canceled.</exception>
        public virtual void Cancel(Audit history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            Contract.Ensures(this.Notes.Contains(history));
            Contract.EndContractBlock();

            if (!this.Status.CanBeEdited()) throw new InvalidOperationException("Only editable deals may be canceled");

            Trace.TraceInformation($"Deal '{this.Id}' with {this.Orders.Count} orders being canceled");

            this.status = DealStatus.Canceled;
            this.Orders.ForEach(o => o.Cancel());
            this.Notes.Add(history);
            this.Amount = 0M;
            this.FollowupDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Transitions the current instance into the <see cref="DealStatus">InProcess</see> state due to state timeout.
        /// </summary>
        /// <param name="history">The <see cref="Audit"/> explaining the reason the current deal is being expired.</param>
        /// <exception cref="InvalidOperationException">The current instance is not in a status that can be expired.</exception>
        public virtual void Expire(Audit history)
        {
            if (this.Status != DealStatus.Billing) throw new InvalidOperationException("Only deals that are waiting in billing may be expired");

            Trace.TraceInformation($"Deal '{this.Id}' with {this.Orders.Count} orders being expired");
            
            this.status = DealStatus.InProcess;
            this.Notes.Add(history);
            this.FollowupDate = DateTime.UtcNow.AddDays(14);

            this.OnExpired();
        }

        /// <summary>
        /// Determines if the current instance requires review.
        /// </summary>
        /// <remarks>Always returns true.</remarks>
        /// <returns>True if the current instance will require review; otherwise false.</returns>
        public virtual Boolean RequiresReview()
        {
            Trace.TraceInformation($"Deal '{this.Id}' {nameof(this.RequiresReview)}() called. Returning true");
            return true;
        }

        /// <summary>
        /// Readies the current instance for being in a state ready to be send to review status.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The current instance has no <see cref="Order">orders</see></para>
        /// <para>-OR-</para>
        /// <para>The deal status is not currently <see cref="DealStatus">In Process</see></para>
        /// </exception>
        public virtual void SubmitForReview(Audit history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            if (this.Orders.Count == 0) throw new InvalidOperationException("A deal cannot be submitted for review without containing at least one order");
            if (this.Status != DealStatus.InProcess) throw new InvalidOperationException($"A deal cannot be submitted for review while in the {this.Status} state.");
            Contract.EndContractBlock();

            if (this.Orders.Any(o => o.Status != OrderStatus.Open)) throw new InvalidOperationException($"No orders that have not been billed exist on deal '{this.Id}'");

            Trace.TraceInformation($"Deal '{this.Id}' {nameof(this.SubmitForReview)}() called.");

            this.Status = DealStatus.Approval;
            this.Notes.Add(history);
            this.FollowupDate = DateTime.UtcNow.AddDays(7);
            this.Amount = this.Total();

            if (!this.RequiresReview())
            {
                this.Approve(new Audit("Deal was automatically approved.", history.CreatedBy));
            }
        }

        /// <summary>
        /// Readies the current instance for being an approved deal.
        /// </summary>
        /// <param name="history">The <see cref="Audit"/> describing who and why the deal is approved.</param>
        /// <exception cref="InvalidOperationException">The deal status is not currently <see cref="DealStatus">In Approval</see>.</exception>
        public virtual void Approve(Audit history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            Contract.Ensures(this.Notes.Contains(history));
            Contract.EndContractBlock();

            if (this.Status != DealStatus.Approval) throw new InvalidOperationException($"A deal cannot be approved in review while in the {this.Status} state.");

            Trace.TraceInformation($"Deal '{this.Id}' {nameof(this.Approve)}() called. Note: {history.Content}");

            this.Notes.Add(history);
            this.FollowupDate = DateTime.UtcNow.AddDays(4);

            this.Status = DealStatus.Billing;

            this.OnReviewed();
        }

        /// <summary>
        /// Readies the current instance for being an declined deal.
        /// </summary>
        /// <remarks>
        /// Raises the <see cref="Reviewed"/> event.
        /// When the ability to pull ambient security identifiers from the runtime is present, this method will only take textual reasons.
        /// </remarks>
        /// <param name="history">The <see cref="Audit"/> describing why the deal is declined.</param>
        /// <exception cref="InvalidOperationException">The deal status is not currently <see cref="DealStatus">In Approval</see>.</exception>
        public virtual void Decline(Audit history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            Contract.Ensures(this.Notes.Contains(history));
            Contract.EndContractBlock();

            if (this.Status != DealStatus.Approval) throw new InvalidOperationException($"A deal cannot be declined in review while in the {this.Status} state.");

            // NOTE: this will confirm all Orders are loaded before raising event
            Trace.TraceInformation($"Deal '{this.Id}' with {this.Orders.Count} orders being declined");
            
            this.Notes.Add(history);
            this.Status = DealStatus.InProcess;
            this.FollowupDate = DateTime.UtcNow.AddDays(7);

            this.OnReviewed();
        }

        /// <summary>
        /// Gets the calculated total for all the current orders.
        /// </summary>
        /// <returns>The calculated total for all the current orders.</returns>
        public virtual Decimal Total()
        {
            return this.Orders.Sum(o => o.Total());
        }

        /// <summary>
        /// Changes the current instance to have a new admin creator owner.
        /// </summary>
        /// <param name="newOwnerId">The new identifier of the admin user that owns and is responsible for the deal.</param>
        public virtual void ChangeOwner(Guid newOwnerId)
        {
            if (!this.Status.CanBeEdited()) throw new InvalidOperationException($"Deal {this.Id} is not editable");

            this.OwnerId = newOwnerId;
        }

        ///// <summary>
        ///// Gets the calculated total for all the current orders.
        ///// </summary>
        ///// <returns>The calculated total for all the current orders.</returns>
        //public virtual Decimal TotalBilled()
        //{
        //    var billedTotal = this.Orders.OfType<BillableOrder>().Where(o => o.Status == OrderStatus.Billed).Sum(o => o.Total());
        //    return billedTotal;
        //}

        ///// <summary>
        ///// Gets the calculated total for all the current orders.
        ///// </summary>
        ///// <returns>The calculated total for all the current orders.</returns>
        //public virtual Decimal TotalRefunded()
        //{
        //    var refundedTotal = this.Orders.OfType<RefundOrder>().Where(o => o.Status == OrderStatus.Refunded).Sum(o => o.Total());
        //    return refundedTotal;
        //}

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the current instance has been reviewed.
        /// </summary>
        public event EventHandler Reviewed;

        /// <summary>
        /// Raises the <see cref="Reviewed"/> event.
        /// </summary>
        protected virtual void OnReviewed()
        {
            // Makes sure the Orders are loaded so fired events will be handled by them
            Trace.WriteLine($"Deal {this.Id} with {this.Orders.Count} orders reviewed");

            this.Reviewed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the current instance has been expired.
        /// </summary>
        public event EventHandler<EventArgs> Expired;

        /// <summary>
        /// Raises the <see cref="Expired"/> event.
        /// </summary>
        protected virtual void OnExpired()
        {
            // Makes sure the Orders are loaded so fired events will be handled by them
            Trace.WriteLine($"Deal {this.Id} with {this.Orders.Count} orders expired");

            this.Expired?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Event Sinks

        /// <summary>
        /// Responds to the <see cref="OrderCollection.OrdersComplete"/> event.
        /// </summary>
        protected virtual void OnOrdersComplete(Object source, EventArgs e)
        {
            this.Status = DealStatus.Complete;

            var billableOrders = this.Orders.OfType<BillableOrder>().Where(o => o.Status == OrderStatus.Billed).ToArray();
            var refundOrders = this.Orders.OfType<RefundOrder>().Where(o => o.Status == OrderStatus.Refunded);

            // Invoices are always at the full rate 
            var sum1 = billableOrders.Where(o => o.Bill.ContractType == ContractType.Invoice).Sum(o => o.Total());

            // Charges are only at the value they actually billed which may be up to the full rate, but may be less
            var sum2 = billableOrders
                .Where(o => o.Bill.ContractType == ContractType.Receipt)
                .SelectMany(o => o.Transactions.Where(c => c.Status.IsCaptured()))
                .Sum(c => c.AmountProcessed ?? 0);

            // Gets the set of refunds to remove from the total
            var sum3 = refundOrders.Sum(o => o.Total());
            
            // Amount Invoiced + Amount Charged - Refunds = New Total
            this.Amount = sum1 + sum2 + sum3;

            // Completed refund orders call this method as well so don't bump the date if already set
            if (this.CompletedDate == null) this.CompletedDate = DateTime.UtcNow;
        }

        #endregion

        #region IEquatable<DealBinder> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(DealBinder other)
        {
            if (other == null) return false;
            if (this.Id == null && other.Id == null)
            {
                return ReferenceEquals(this, other);
            }

            if (other.Id == null) return false;

            return this.Id == other.Id;
        }

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as DealBinder);
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (this.Id ?? 0).GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion
    }
}