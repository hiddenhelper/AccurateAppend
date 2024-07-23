using System;
using System.Diagnostics;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A single Accurate Append Product that we support.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Id) + ("}:{" + nameof(Key) + "}"))]
    public class Product : IKeyedObject<Int32?>, IEquatable<Product>, IEquatable<String>
    {
        #region Constructor

        /// <summary>
        /// Product creation is not directly supported.
        /// </summary>
        protected Product()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current instance.
        /// </summary>
        /// <value>The identifier for the current instance.</value>
        public virtual Int32? Id { get; protected set; }

        /// <summary>
        /// Gets or sets the unique product name value.
        /// </summary>
        /// <value>The unique product name value.</value>
        public virtual String Key { get; protected set; }

        /// <summary>
        /// Gets or sets the friendly product name value.
        /// </summary>
        /// <value>The friendly product name value.</value>
        public virtual String Title { get; protected set; }

        /// <summary>
        /// Gets or sets the product description value.
        /// </summary>
        /// <value>The product description value.</value>
        public virtual String Description { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProductUsage"/> for the current instance.
        /// </summary>
        /// <value>The <see cref="ProductUsage"/> for the current instance.</value>
        public virtual ProductUsage Usage { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ProductCategory"/> for the current instance.
        /// </summary>
        /// <value>The <see cref="ProductCategory"/> for the current instance.</value>
        public virtual ProductCategory Category { get; protected set; }

        /// <summary>
        /// Gets the default <see cref="PricingModel"/> for the product.
        /// </summary>
        /// <value>The default <see cref="PricingModel"/> for the product.</value>
        public virtual PricingModel DefaultPricingModel
        {
            get;
            protected set;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            if (obj is String)
            {
                return this.Equals(obj as String);
            }
            if (obj is Product)
            {
                return this.Equals(obj as Product);
            }
            return false;
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion

        #region IEquatable<Product> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(Product other)
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

        #region IEquatable<String> Members

        /// <summary>
        /// Indicates whether the current object has the same <see cref="Key"/> as the provided value.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(String other)
        {
            other = (other ?? String.Empty).Trim();

            return this.Key.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}