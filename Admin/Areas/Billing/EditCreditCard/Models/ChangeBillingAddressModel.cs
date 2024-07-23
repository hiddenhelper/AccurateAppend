using System;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models;

namespace AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard.Models
{
    [Serializable()]
    public class ChangeBillingAddressModel
    {
        [Required()]
        public Guid UserId { get; set; }

        [Required()]
        public String UserName { get; set; }

        [Required()]
        public Int32 CardId { get; set; }

        [Required()]
        public BillingAddressModel Address { get; set; }
    }
}