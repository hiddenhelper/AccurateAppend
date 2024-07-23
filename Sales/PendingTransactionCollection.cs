using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Collections.Generic;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A hierarchical collection from <see cref="Order"/> to <see cref="PendingTransaction"/>.
    /// </summary>
    /// <remarks>
    /// This collection is responsible for enforcement of not null entities on a
    /// set of orders and that all orders are part of a singular <see cref="Order"/>.
    /// </remarks>
    public class PendingTransactionCollection : ObservableCollection<PendingTransaction>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingTransactionCollection"/> class.
        /// </summary>
        /// <param name="parent">The owning <see cref="Order"/> instance.</param>
        public PendingTransactionCollection(Order parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            Contract.EndContractBlock();

            this.Parent = parent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Order"/> that owns the items in the collection.
        /// </summary>
        public Order Parent { get; }

        #endregion

        #region Methods

        protected virtual void ValidateObject(PendingTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction.Order == null) return; // Lazy loading scenario
            if (!this.Parent.Equals(transaction.Order)) throw new InvalidOperationException($"The provided {nameof(PendingTransaction)} has a different {nameof(Order)} than the current collection and cannot be added. This: {this.Parent.Id}, Order: {transaction.Order?.Id}");
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ClearItems()
        {
            if (!this.Parent.Status.CanBeEdited()) throw new InvalidOperationException($"A {nameof(PendingTransaction)} can only be removed if the owning {nameof(Order)} is in the {OrderStatus.Open} state");
            this.ForEach(t => t.Order = null);
            
            base.ClearItems();
        }

        /// <inheritdoc />
        protected override void InsertItem(Int32 index, PendingTransaction item)
        {
            this.ValidateObject(item);

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(Int32 index, PendingTransaction item)
        {
            this.ValidateObject(item);

            base.SetItem(index, item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(Int32 index)
        {
            if (!this.Parent.Status.CanBeEdited()) throw new InvalidOperationException($"A {nameof(PendingTransaction)} can only be removed if the owning {nameof(Order)} is in the {OrderStatus.Open} state");
            var transaction = this[index];
            if (transaction != null) transaction.Order = null;

            base.RemoveItem(index);
        }

        #endregion
    }
}
