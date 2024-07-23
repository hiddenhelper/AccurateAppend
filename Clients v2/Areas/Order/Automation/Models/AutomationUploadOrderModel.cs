using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Models
{
    /// <summary>
    /// Order model for client supplied files. At first glance the presence of the file delimiter
    /// on this type seems odd, however only client supplied files have variable delimiters, similar
    /// to how they only have column mappings as other systems have a fixed schema. The input
    /// customer file name will be mapped to <see cref="NewOrderModel.ListName"/>.
    /// </summary>
    [Serializable()]
    public class AutomationUploadOrderModel : NewOrderModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationUploadOrderModel"/> class.
        /// </summary>
        public AutomationUploadOrderModel()
        {
            this.OrderMinimum = 75;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Input file delimiter. This value indicates the character used to delimit individual columns.
        /// </summary>
        [Required()]
        [RegularExpression("[\t|,]")]
        public Char FileDelimiter { get; set; }

        /// <summary>
        /// The input file name as stored on our storage app after uploading from a customer browser.
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

        /// <summary>
        /// Denotes if first row in file is a header row
        /// </summary>
        public Boolean HasHeaderRow { get; set; }

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
        /// <remarks>Returns a single element for the <see cref="FileDelimiter"/> property.</remarks>
        /// <returns>A sequence of additional data values indexed with the property name matching the property on the concrete type.</returns>
        public override IEnumerable<KeyValuePair<String, Object>> ExtensionData()
        {
            yield return new KeyValuePair<String, Object>(nameof(this.FileDelimiter), this.FileDelimiter);
            yield return new KeyValuePair<String, Object>(nameof(this.SystemFileName), this.SystemFileName);
        }

        #endregion
    }
}