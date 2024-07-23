using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Models
{
    /// <summary>
    /// Order model for NationBuilder lists. The List Name at NationBuilder
    /// will be mapped to <see cref="NewOrderModel.ListName"/>. In addition,
    /// the <see cref="RegId"/> will uniquely identify what
    /// <see cref="DomainModel.ReadModel.NationBuilderRegistration"/> for the interactive user
    /// and the <see cref="ListId"/> will identify the remote object.
    /// </summary>
    public class NationBuilderOrderModel : NewOrderModel
    {
        #region Constructor

        /// <summary>
        /// Intializes a new instance of the <see cref="NationBuilderOrderModel"/> class.
        /// </summary>
        public NationBuilderOrderModel()
        {
            this.OrderMinimum = 15; //CostService.FileMinimum;
        }

        #endregion

        /// <summary>
        /// The NationBuilder generated list identifier. Used to access
        /// the remote data.
        /// </summary>
        [Required()]
        [Range(1, Int32.MaxValue)]
        public Int32 ListId { get; set; }

        /// <summary>
        /// The registered integration information for a Nation generated 
        /// during their signup/approval process. Used by our system as the
        /// alternate key to a a Nation SLUG to access the remote data.
        /// </summary>
        [Required()]
        [Range(1, Int32.MaxValue)]
        public Int32 RegId { get; set; }

        /// <summary>
        /// Since each concrete <see cref="NewOrderModel"/> type has additional data (generally set and required
        /// but afterwards read-only), this allows generic and shared code to operate with the data but not needing
        /// the knowledge of the hierarchy. Most often this is handled in a UI where the data needs to be present
        /// but otherwise not used (e.g. javascript object or json). Each additional data point should match the
        /// name of the property (using the <see cref="KeyValuePair{TKey,TValue}.Key"/> property) and the current value
        /// supplied without any coersion. Elements from the super type MUST NOT be returned, only values unique
        /// TO THIS TYPE.
        /// </summary>
        /// <remarks>Returns an element for the <see cref="ListId"/> and <see cref="RegId"/> properties.</remarks>
        /// <returns>A sequence of additional data values indexed with the property name matching the property on the concrete type.</returns>
        public override IEnumerable<KeyValuePair<String, Object>> ExtensionData()
        {
            yield return new KeyValuePair<String, Object>(nameof(this.ListId), this.ListId);
            yield return new KeyValuePair<String, Object>(nameof(this.RegId), this.RegId);
        }
    }
}