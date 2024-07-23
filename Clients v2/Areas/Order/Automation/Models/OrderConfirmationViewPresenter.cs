using System;
using AccurateAppend.Websites.Clients.Areas.Profile.Card.Models;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Models
{
    /// <summary>
    /// Is a wrapper for a <see cref="AutomationUploadOrderModel"/> type that instructs the column mapper view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class OrderConfirmationViewPresenter
    {
        public MvcActionModel Postback { get; set; }

        public Guid CartId { get; set; }

        public String DataUrl { get; set; }

        public String WalletUrl { get; set; }

        public PaymentDetailsModel PaymentDetails { get; set; }
    }
}