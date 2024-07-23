using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// A viewmodel representation of an existing <see cref="RefundOrder"/> detail information.
    /// </summary>
    public class RefundDetail
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundDetail"/> class.
        /// </summary>
        public RefundDetail()
        {
            this.Items = new Collection<OrderItemModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current model.
        /// </summary>
        [Display(Name = "Order Id")]
        [Required()]
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user that owns the parent deal.
        /// </summary>
        [Required()]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets the set of <see cref="OrderItemModel"/> for the current model.
        /// </summary>
        public ICollection<OrderItemModel> Items { get; protected set; }

        /// <summary>
        /// Gets the adjusted minimum cost of the order total.
        /// </summary>
        [Display(Name = "Minimum")]
        public Decimal OrderMinimum { get; set; }

        /// <summary>
        /// Gets the public key used for this order.
        /// </summary>
        public String PublicKey { get; set; }

        #endregion
    }
}