using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Models
{
    /// <summary>
    /// Base type containing the core ordering logic and data.
    /// Individual ordering systems will each have a concrete
    /// type that contains additional logic for the type of order.
    /// </summary>
    public abstract class NewOrderModel
    {
        #region Fields

        private ICollection<ProductModel> products;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NewOrderModel"/> class.
        /// </summary>
        protected NewOrderModel()
        {
            this.products = new List<ProductModel>();
            this.OrderId = Guid.NewGuid();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of records in the order. Used for payment estimation. (may be moved into view presenter instead)
        /// </summary>
        [Required()]
        [Range(1,Int32.MaxValue)]
        public Int32 RecordCount { get; set; }

        /// <summary>
        /// Gets the set of <see cref="ProductModel"/> that are part of the order.
        /// </summary>
        public ICollection<ProductModel> Products
        {
            get => this.products ?? (this.products = new HashSet<ProductModel>());
            protected set
            {
                value = value ?? new List<ProductModel>();
                this.products = value;
            }
        }

        /// <summary>
        /// Gets the name of list or file or other client data identifier value (specific orders have different meanings for this value).
        /// </summary>
        [Required()]
        [MinLength(1)]
        public String ListName { get; set; }

        /// <summary>
        /// A unique value that represents the order, start to finish.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Display property displaying an estimated total.
        /// </summary>
        public Decimal Total
        {
            get { return Math.Max(this.Products.Sum(a => a.Total), this.OrderMinimum); }
        }

        /// <summary>
        /// Display property displaying any minimum order total.
        /// </summary>
        public Decimal OrderMinimum { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Since each concrete <see cref="NewOrderModel"/> type has additional data (generally set and required
        /// but afterwards read-only), this allows generic and shared code to operate with the data but not needing
        /// the knowledge of the hierarchy. Most often this is handled in a UI where the data needs to be present
        /// but otherwise not used (e.g. javascript object or json). Each additional data point should match the
        /// name of the property (using the <see cref="KeyValuePair{T,K}.Key"/> property) and the current value
        /// supplied without any coercion. Elements from the super type MUST NOT be returned, only values unique
        /// TO THIS TYPE.
        /// </summary>
        /// <returns>A sequence of additional data values indexed with the property name matching the property on the concrete type.</returns>
        public abstract IEnumerable<KeyValuePair<String, Object>>  ExtensionData();

        #endregion
    }
}