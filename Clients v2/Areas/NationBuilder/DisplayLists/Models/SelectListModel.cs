using System;
using System.Diagnostics;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.DisplayLists.Models
{
    /// <summary>
    /// View model containing cart identifier and the validation for list selection.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Cart={" + nameof(CartId) + "}")]
    public class SelectListModel
    {
        /// <summary>
        /// Gets or sets the identifier for the current cart.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of records allowed in a NationBuilder list to append.
        /// Defaults to 200k.
        /// </summary>
        public Int32 MaximumListSize { get; set; } = 200000;
    }
}