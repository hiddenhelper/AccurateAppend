using System;
using System.Diagnostics;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A categorization scheme for the <see cref="Product"/> entity.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Name) + ("}:'{" + nameof(Description) + "}'"))]
    public class ProductCategory
    {
        #region Constructor

        /// <summary>
        /// ProductCategory creation is not directly supported.
        /// </summary>
        protected ProductCategory()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current instance.
        /// </summary>
        /// <value>The identifier for the current instance.</value>
        public virtual Int32? Id { get; private set; }

        /// <summary>
        /// Gets or sets the unique product category name value.
        /// </summary>
        /// <value>The unique product category name value.</value>
        public virtual String Name { get; protected set; }

        /// <summary>
        /// Gets or sets the friendly product category description value.
        /// </summary>
        /// <value>The friendly product category description value.</value>
        public virtual String Description { get; protected set; }

        #endregion
    }
}