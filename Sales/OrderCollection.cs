using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A hierarchical collection from <see cref="DealBinder"/> to <see cref="Order"/>.
    /// </summary>
    /// <remarks>
    /// This collection is responsible for enforcement of not null entities on a
    /// set of orders and that all orders are part of a singular <see cref="DealBinder"/>.
    /// 
    /// In addition is mediates events between the enclosing <see cref="DealBinder"/> to
    /// the contained <see cref="Order"/> items. Likewise, <seealso cref="Order"/> events
    /// will be aggregated and mediated for the <seealso cref="DealBinder"/>.
    /// </remarks>
    public class OrderCollection : ObservableCollection<Order>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCollection"/> class.
        /// </summary>
        /// <param name="parent">The owning <see cref="DealBinder"/> instance.</param>
        public OrderCollection(DealBinder parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            Contract.EndContractBlock();

            this.Parent = parent;
            this.Parent.Reviewed += this.OnReviewed;
            this.Parent.Expired += this.OnReviewed;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DealBinder"/> that owns the items in the collection.
        /// </summary>
        public DealBinder Parent { get; }

        #endregion

        #region Methods

        protected virtual void ValidateObject(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Deal == null) return; // Lazy loading scenario
            if (!this.Parent.Equals(order.Deal)) throw new InvalidOperationException($"The provided {nameof(Order)} has a different {nameof(DealBinder)} than the current collection and cannot be added. This: {this.Parent.Id}, Order: {order.Deal?.Id}");
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ClearItems()
        {
            if (this.Any(o => !o.Status.CanBeEdited())) throw new InvalidOperationException($"An {nameof(Order)} can only be removed if it is in the {OrderStatus.Open} state");
            this.ForEach(o =>
            {
                o.Deal = null;
                o.Complete -= this.OnOrderComplete;

            });

            base.ClearItems();
        }

        /// <inheritdoc />
        protected override void InsertItem(Int32 index, Order item)
        {
            this.ValidateObject(item);
            item.Complete += this.OnOrderComplete;

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(Int32 index, Order item)
        {
            this.ValidateObject(item);
            item.Complete += this.OnOrderComplete;

            base.SetItem(index, item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(Int32 index)
        {
            var order = this[index];
            if (order != null)
            {
                if (order.Status != OrderStatus.Open) throw new InvalidOperationException($"An {nameof(Order)} can only be removed if it is in the {OrderStatus.Open} state");
                order.Deal = null;
                order.Complete -= this.OnOrderComplete;
            }

            base.RemoveItem(index);
        }

        #endregion

        #region Event Sinks

        /// <summary>
        /// Responds to the <see cref="DealBinder.Reviewed"/> event.
        /// </summary>
        protected virtual void OnReviewed(Object sender, EventArgs e)
        {
            // Decline
            if (this.Parent.Status == DealStatus.InProcess)
            {
                foreach (var order in this)
                {
                    order.OnDealDeclined();
                }
            }
            else
            {
                foreach (var order in this)
                {
                    order.OnDealApproved();
                }
            }
        }

        /// <summary>
        /// Responds to the <see cref="Order.Complete"/> event.
        /// </summary>
        protected virtual void OnOrderComplete(Object sender, EventArgs e)
        {
            var allComplete = this.All(o => o.Status.IsComplete());
            if (allComplete)
            {
                OrdersComplete?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the collection of <see cref="Order"/> instances have all been completed (Billed or Processed).
        /// </summary>
        public event EventHandler OrdersComplete;

        #endregion
    }
}
