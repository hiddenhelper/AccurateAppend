using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Models
{
    /// <summary>
    /// Model that contains the column mapping data.
    /// </summary>
    [Serializable()]
    public class MappedColumnsModel
    {
        #region Properties

        /// <summary>
        /// Contains the client supplied / picked column map. If this value is null, it means no mapping is yet performed.
        /// </summary>
        public String ColumnMap { get; set; }
        
        /// <summary>
        /// Denotes if first row in file is a header row
        /// </summary>
        public Boolean HasHeaderRow { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the current cart.
        /// </summary>
        public Guid CartId { get; set; }

        #endregion
    }
}