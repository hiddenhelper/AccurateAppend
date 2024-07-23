using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Order.Models
{
    /// <summary>
    /// Order model for AA system generated files. At first glance the presence of the column
    /// mapping  on this type seems odd, however while this system operates on a fixed schema,
    /// product enhancements selection will drive the column mapping. The automatically
    /// generated order identifier will be used to create the customer file name and will be
    /// mapped to <see cref="NewOrderModel.ListName"/>.
    /// </summary>
    public class GeneratedFileOrderModel : NewOrderModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedFileOrderModel"/> class.
        /// </summary>
        public GeneratedFileOrderModel()
        {
            this.OrderMinimum = 75;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The input file name as stored on our storage app after generation from our system.
        /// This value is generated and manipulated by us.
        /// </summary>
        [Required()]
        [MinLength(1)]
        public String SystemFileName { get; set; }

        /// <summary>
        /// Contains the client supplied / picked column map. If this value is null, it means no mapping is yet performed.
        /// </summary>
        [Required()]
        [MinLength(1)]
        public String ColumnMap { get; set; }

        #endregion

        #region Overrides

        /// <summary>
        /// Since each concrete <see cref="NewOrderModel"/> type has additional data (generally set and required
        /// but afterwards read-only), this allows generic and shared code to operate with the data but not needing
        /// the knowledge of the hierarchy. Most often this is handled in a UI where the data needs to be present
        /// but otherwise not used (e.g. javascript object or json). Each additional data point should match the
        /// name of the property (using the <see cref="KeyValuePair{TKey,TValue}.Key"/> property) and the current value
        /// supplied without any coercion. Elements from the super type MUST NOT be returned, only values unique
        /// TO THIS TYPE.
        /// </summary>
        /// <remarks>Returns a single element for the <see cref="SystemFileName"/> property.</remarks>
        /// <returns>A sequence of additional data values indexed with the property name matching the property on the concrete type.</returns>
        public override IEnumerable<KeyValuePair<String, Object>> ExtensionData()
        {
            yield return new KeyValuePair<String, Object>(nameof(this.SystemFileName), this.SystemFileName);
        }

        #endregion
    }
}