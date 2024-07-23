using System;
using AccurateAppend.Websites.Clients.Areas.Profile.Card.Models;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Models
{
    /// <summary>
    /// Provides the model for the order confirmation screen.
    /// </summary>
    [Serializable()]
    public class OrderConfirmationViewPresenter2
    {
        public MvcActionModel Postback { get; set; }

        public Guid CartId { get; set; }

        public String DataUrl { get; set; }

        public String WalletUrl { get; set; }

        public PaymentDetailsModel PaymentDetails { get; set; }
    }
}