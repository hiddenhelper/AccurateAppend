using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Entity used to determine a cost structure for a <see cref="Product"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Floor) + "}-{" + nameof(Ceiling) + "}: match={" + nameof(PerMatch) + "},record={" + nameof(PerRecord) + "}")]
    public class Cost : IKeyedObject<Int32?>, IEquatable<Cost>
    {
        #region Fields

        private Product product;
        private String category;

        #endregion

        #region Constants

        /// <summary>
        /// Contains the string value "Default" that is used as the <see cref="Category"/> for default cost structures.
        /// </summary>
        public const String DefaultCategory = "Default";

        /// <summary>
        /// Contains the string value "NationBuilder" that is used as the <see cref="Category"/> for NationBuilder cost structures.
        /// </summary>
        public const String NationBuilderCategory = "NationBuilder";

        /// <summary>
        /// Contains the string value "ListBuilder" that is used as the <see cref="Category"/> for ListBuilder cost structures.
        /// </summary>
        public const String ListBuilderCategory = "ListBuilder";

        /// <summary>
        /// Contains the string value "CSV" that is used as the <see cref="Category"/> for CSV uploaded file cost structures.
        /// </summary>
        public const String CsvUploadCategory = "CSV";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Cost"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected Cost()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cost"/> class.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> to create costs for.</param>
        /// <param name="category">The name of the rate card the cost is for.</param>
        public Cost(Product product, String category)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (String.IsNullOrWhiteSpace(category)) throw new ArgumentNullException(nameof(category));
            Contract.EndContractBlock();

            this.product = product;
            this.category = category.Trim();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public virtual Int32? Id { get; protected set; }

        /// <summary>
        /// Gets the lower count value for this cost entry.
        /// </summary>
        /// <value>The lower count value for this cost entry.</value>
        public virtual Int32 Floor { get; set; }

        /// <summary>
        /// Gets the upper count value for this cost entry.
        /// </summary>
        /// <value>The upper count value for this cost entry.</value>
        public virtual Int32 Ceiling { get; set; }

        /// <summary>
        /// Gets the cost per record for this cost entry.
        /// </summary>
        /// <value>The cost per record for this cost entry.</value>
        public virtual Decimal PerRecord { get; set; }

        /// <summary>
        /// Gets the cost per match for this cost entry.
        /// </summary>
        /// <value>The cost per match for this cost entry.</value>
        public virtual Decimal PerMatch { get; set; }

        /// <summary>
        /// Gets the <see cref="Product"/> this cost entry is established for.
        /// </summary>
        /// <value>The <see cref="Product"/> this cost entry is established for.</value>
        public virtual Product Product
        {
            get => this.product;
            protected set => this.product = value;
        }

        /// <summary>
        /// Gets the name this cost entry is established for.
        /// </summary>
        /// <value>The name this cost entry is established for.</value>
        public virtual String Category
        {
            get => this.category;
            protected set => this.category = (value ?? String.Empty).Trim();
        }

        /// <summary>
        /// Gets the minimum, if any, this cost entry should have.
        /// </summary>
        /// <value>The minimum, if any, this cost entry should have.</value>
        public virtual Decimal? FileMinimum { get; set; }

        #endregion

        #region Equality

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(Cost other)
        {
            if (other == null) return false;
            if (this.Id == null && other.Id == null)
            {
                return ReferenceEquals(this, other);
            }

            if (other.Id == null) return false;

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
            return this.Equals(obj as Cost);
        }

        #endregion

        #region Overrides

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return $"{this.Product.Key}, {this.Floor}-{this.Ceiling}";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates whether the current cost structure element is appropriate for the indicated product and count.
        /// </summary>
        /// <param name="forProduct">The <see cref="Product"/> to match.</param>
        /// <param name="itemCount">The item count to find a cost match for.</param>
        /// <returns>True if the current instance is a match; otherwise false.</returns>
        public virtual Boolean CanHandle(Product forProduct, Int32 itemCount)
        {
            if (forProduct == null) throw new ArgumentNullException(nameof(forProduct));
            Contract.EndContractBlock();

            return this.Product.Equals(forProduct) && this.Floor <= itemCount && this.Ceiling >= itemCount;
        }

        /// <summary>
        /// Gets the default pricing structure for a <see cref="Product"/>.
        /// </summary>
        /// <param name="forProduct">The <see cref="Product"/> to get the default cost for.</param>
        /// <returns>A <see cref="Cost"/> containing the default pricing appropriate with the <paramref name="forProduct"/> value.</returns>
        public static Cost DefaultCost(Product forProduct)
        {
            if (forProduct == null) throw new ArgumentNullException(nameof(forProduct));
            Contract.Ensures(Contract.Result<Cost>() != null);
            Contract.Ensures(ReferenceEquals(Contract.Result<Cost>().Product, forProduct));
            Contract.EndContractBlock();

            return new Cost
            {
                Ceiling = Int32.MaxValue,
                Floor = 0,
                Id = null,
                PerMatch = 0,
                PerRecord = 0,
                Product = forProduct
            };
        }

        /// <summary>
        /// Indicates whether the current instance is part of a cost structure that is a system defined one.
        /// That is, not attached to a specific client.
        /// </summary>
        /// <remarks>
        /// Wraps the <see cref="IsSystem(String)"/> overload.
        /// </remarks>
        /// <returns>True if the current instance is part of a system defined structure; otherwise false.</returns>
        public virtual Boolean IsSystem()
        {
            return IsSystem(category);
        }

        /// <summary>
        /// Indicates whether the provided <paramref name="category"/> is part of a cost structure that is a system defined one.
        /// That is, not attached to a specific client.
        /// </summary>
        /// <remarks>
        /// Evaluation of <paramref name="category"/> is performed in a culture neutral case insensitive fashion.
        /// </remarks>
        /// <param name="category">The value to check if it is a system defined one.</param>
        /// <returns>True if the <paramref name="category"/> is part of a system defined structure; otherwise false.</returns>
        public static Boolean IsSystem(String category)
        {
            var categories = new[]
            {
                DefaultCategory,
                NationBuilderCategory,
                ListBuilderCategory,
                CsvUploadCategory
            };

            return categories.Any(c => String.Equals(c, category, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
