using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A hierarchical collection from <see cref="Order"/> to <see cref="ProductLine"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// This collection is responsible for enforcement of not null entities on a
    /// set of order lines and that all lines are part of a singluar <see cref="Order"/>.
    /// </remarks>
    public class ProductLineCollection : ObservableCollection<ProductLine>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductLineCollection"/> class.
        /// </summary>
        /// <param name="parent">The owning <see cref="Order"/> instance.</param>
        public ProductLineCollection(Order parent)
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

        /// <summary>
        /// Called whenever an <see cref="ProductLine"/> is inserted or added to the collection.
        /// Will always confirm that the <see cref="Order.Validate"/> method of the <see cref="Parent"/>
        /// is called.
        /// </summary>
        /// <param name="orderLine">The <see cref="ProductLine"/> that is added or inserted.</param>
        protected virtual void ValidateObject(ProductLine orderLine)
        {
            if (orderLine == null) throw new ArgumentNullException(nameof(orderLine));
            if (orderLine.Order == null) return; // Lazy loading scenario
            if (!this.Parent.Equals(orderLine.Order)) throw new InvalidOperationException($"The provided {nameof(ProductLine)} has a different {nameof(Order)} than the current collection and cannot be added. This: {this.Parent.Id}, Order: {orderLine.Order?.Id}");
            
            this.Parent.Validate(orderLine);

            //orderLine.PriceChanged += (s, e) => this.OnPriceChanged(s as OrderLine);
            //orderLine.QuantityChanged += (s, e) => this.OnPriceChanged(s as OrderLine);
        }

        #endregion

        #region Events

        ///// <summary>
        ///// Raised when any contained <see cref="OrderLine"/> instances have the <see cref="OrderLine.Price"/> value changed.
        ///// </summary>
        //public event EventHandler<EventArgs<OrderLine>> PriceChanged;

        ///// <summary>
        ///// Raises the <see cref="PriceChanged"/> event.
        ///// </summary>
        ///// <param name="orderLine">The <see cref="OrderLine"/> instance that changed.</param>
        //protected virtual void OnPriceChanged(OrderLine orderLine)
        //{
        //    if (orderLine == null) throw new ArgumentNullException("orderLine");
        //    Contract.EndContractBlock();

        //    if (this.PriceChanged == null) return;
        //    this.PriceChanged(this, new EventArgs<OrderLine>(orderLine));
        //}

        ///// <summary>
        ///// Raised when any contained <see cref="OrderLine"/> instances have the <see cref="OrderLine.Quantity"/> value changed.
        ///// </summary>
        //public event EventHandler<EventArgs<OrderLine>> QuantityChanged;

        ///// <summary>
        ///// Raises the <see cref="QuantityChanged"/> event.
        ///// </summary>
        ///// <param name="orderLine">The <see cref="OrderLine"/> instance that changed.</param>
        //protected virtual void OnQuantityChanged(OrderLine orderLine)
        //{
        //    if (orderLine == null) throw new ArgumentNullException("orderLine");
        //    Contract.EndContractBlock();

        //    if (this.QuantityChanged == null) return;
        //    this.QuantityChanged(this, new EventArgs<OrderLine>(orderLine));
        //}

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void ClearItems()
        {
            this.ToArray().ForEach(o => o.Order = null);
            base.ClearItems();
        }

        /// <inheritdoc />
        protected override void InsertItem(Int32 index, ProductLine item)
        {
            this.ValidateObject(item);
            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void SetItem(Int32 index, ProductLine item)
        {
            this.ValidateObject(item);
            base.SetItem(index, item);
        }

        /// <inheritdoc />
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var line in e.OldItems.OfType<ProductLine>())
                {
                    line.Order = null;
                }
            }
            base.OnCollectionChanged(e);
        }
        
        #endregion
    }
}
