using System;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Models
{
    /// <summary>
    /// Is a wrapper for a <see cref="NewOrderModel"/> and <see cref="PaymentDetailsModel"/> type that instructs the shared payment details view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class OrderPaymentViewPresenter
    {
        /// <summary>
        /// Contains the full MVC action that should receive the posted order content.
        /// </summary>
        public MvcActionModel PostBack { get; set; }

        /// <summary>
        /// Contains the full MVC action that should be used to restart the order process.
        /// </summary>
        public MvcActionModel StartOver { get; set; }

        /// <summary>
        /// Contains the payment information that needs to be entered. (Often null)
        /// </summary>
        public PaymentDetailsModel PaymentDetails { get; set; }

        /// <summary>
        /// Contains the actual order information that needs to be passed along.
        /// </summary>
        public NewOrderModel Order { get; set; }
    }
}
