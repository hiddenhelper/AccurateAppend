using System;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Order.Models
{
    /// <summary>
    /// Is a wrapper for a <see cref="GeneratedFileOrderModel"/> type that instructs the column mapper view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class OrderConfirmationViewPresenter
    {
        public MvcActionModel Postback { get; set; }

        /// <summary>
        /// Contains the actual order information that needs to be passed along.
        /// </summary>
        public GeneratedFileOrderModel Order { get; set; }
    }
}