using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A single product - cost - quantity entry for an <see cref="Order"/>.
    /// </summary>
    [DebuggerDisplay("{Product.Key}, Total = {Total()}")]
    public class ProductLine : IKeyedObject<Int32?>
    {
        #region Fields

        private String description;
        private Int32 quantity;
        private Decimal price;
        private Order order;
        private Int32? maximum;
        private Product product;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductLine"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected ProductLine()
        {
            this.description = "New item";
            this.price = 0.0M;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductLine"/> class.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="Sales.Order.Lines"/> collection.</remarks>
        /// <param name="order">The <see cref="Order"/> that contains this instance.</param>
        /// <param name="product">The <see cref="Product"/> the current item is for. Used to prepopulate initial values.</param>
        /// <param name="maximum">The <see cref="Maximum"/> value.</param>
        public ProductLine(Order order, Product product, Int32? maximum = null)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (maximum.HasValue && maximum < 1) throw new ArgumentOutOfRangeException(nameof(maximum), maximum.Value, $"{nameof(this.Maximum)} must be at least 1");

            Contract.EndContractBlock();

            this.order = order;
            this.product = product;
            this.description = product.Description;
            this.maximum = maximum;

            order.Lines.Add(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public virtual Int32? Id { get; protected set; }
        
        /// <summary>
        /// Gets or sets the product description of the order item.
        /// </summary>
        /// <value>The product description of the order item.</value>
        public virtual String Description
        {
            get { return this.description; }
            set { this.description = value ?? String.Empty; }
        }

        /// <summary>
        /// Gets or sets the price per unit of the order item.
        /// </summary>
        /// <value>The price per unit of the order item.</value>
        public virtual Decimal Price
        {
            get { return this.price; }
            set
            {
                var raiseEvent = this.price != value;

                this.price = value;

                if (raiseEvent) this.OnPriceChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of product units of the order item.
        /// </summary>
        /// <value>The number of product units of the order item.</value>
        public virtual Int32 Quantity
        {
            get { return this.quantity; }
            set
            {
                if (value < 0) value = 0;

                var raiseEvent = this.quantity != value;

                this.quantity = value;

                if (raiseEvent) this.OnQuantityChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of product units of the order item that are allowed.
        /// </summary>
        /// <value>The number of product units of the order item that are allowed.</value>
        public virtual Int32? Maximum
        {
            get { return this.maximum; }
            set
            {
                if (value != null && value < 1) value = 1;
                this.maximum = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Order"/> the current instance is attached to.
        /// </summary>
        /// <value>The <see cref="Order"/> the current instance is attached to.</value>
        public virtual Order Order
        {
            get { return this.order; }
            protected internal set { this.order = value; }
        }

        /// <summary>
        /// Gets the <see cref="Product"/> the current instance is attached to.
        /// </summary>
        /// <value>The <see cref="Product"/> the current instance is attached to.</value>
        public virtual Product Product
        {
            get { return this.product; }
            protected set { this.product = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the calculated total for the current order item.
        /// </summary>
        /// <returns>The calculated total for the current order item.</returns>
        public virtual Decimal Total()
        {
            if (!this.IsBillable()) return Decimal.Zero;

            var amount = this.Quantity * this.Price;
            return amount;
        }

        /// <summary>
        /// Determines if the current instance should be considered a billable line item.
        /// That is, a line that should be part of a <see cref="BillableOrder.Total"/>.
        /// </summary>
        /// <returns>True if it is considered a billable item; otherwise false.</returns>
        public virtual Boolean IsBillable()
        {
            var productKey = this.Product.Key;

            return !ProductExtensions.NonBillableProductKeys.Contains(productKey);
        }

        /// <summary>
        /// Determines if the current instance should be considered a line item that has restricted
        /// refund cases. Restrictions are to limit refunds to original amounts and quantities billed.
        /// </summary>
        /// <returns>True if it has restricted refunds; otherwise false.</returns>
        public virtual Boolean HasRestrictedRefund()
        {
            var productKey = this.Product.Key;

            return ProductExtensions.RestrictedRefundProductKeys.Contains(productKey);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as ProductLine);
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion

        #region IEquatable<OrderLine> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(ProductLine other)
        {
            if (other == null) return false;
            if (this.Id == null && other.Id == null)
            {
                return ReferenceEquals(this, other);
            }

            if (other.Id == null) return false;

            return this.Id == other.Id;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the <see cref="Price"/> value changes.
        /// </summary>
        public event EventHandler PriceChanged;

        /// <summary>
        /// Raises the <see cref="PriceChanged"/> event.
        /// </summary>
        protected virtual void OnPriceChanged()
        {
            if (this.PriceChanged == null) return;

            this.PriceChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when the <see cref="Quantity"/> value changes.
        /// </summary>
        public event EventHandler QuantityChanged;

        /// <summary>
        /// Raises the <see cref="QuantityChanged"/> event.
        /// </summary>
        protected virtual void OnQuantityChanged()
        {
            if (this.QuantityChanged == null) return;

            this.QuantityChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}