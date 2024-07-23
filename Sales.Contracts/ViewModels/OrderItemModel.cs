using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// A viewmodel representation of an existing <see cref="ProductLine"/> detail information.
    /// </summary>
    [Serializable()]
    public class OrderItemModel
    {
        #region Properties
        
        /// <summary>
        /// Gets the identifier of the current model.
        /// </summary>
        [Display(Name = "OrderItemId")]
        public Int32? Id { get; set; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        [Display(Name = "Product")]
        [Required(AllowEmptyStrings = false)]
        public String ProductName { get; set; }

        /// <summary>
        /// Gets the description of the product.
        /// </summary>
        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false)]
        public String Description { get; set; }

        /// <summary>
        /// Gets the quantity of the product.
        /// </summary>
        [Display(Name = "Quantity")]
        [Required()]
        public Int32 Quantity { get; set; }

        /// <summary>
        /// Gets the cost per unit of the product.
        /// </summary>
        [Display(Name = "Unit Cost")]
        [Required()]
        public Decimal Cost { get; set; }

        /// <summary>
        /// Gets the maximum number of the product allowed.
        /// </summary>
        [Display(Name = "Maximum Allowed")]
        public Int32? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user that owns the parent deal.
        /// </summary>
        [Required()]
        public Guid UserId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the current total of the item.
        /// </summary>
        /// <returns>The USD value, as <see cref="Decimal"/>, for the item.</returns>
        public virtual Decimal Total()
        {
            return this.Quantity * this.Cost;
        }

        #endregion
    }
}
