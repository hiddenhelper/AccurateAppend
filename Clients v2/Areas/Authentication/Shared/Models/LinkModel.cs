using System;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Validation;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Shared.Models
{
    /// <summary>
    /// ViewModel to interact with external logon to AccurateAppend account process.
    /// </summary>
    public class LinkModel : LoginModel
    {
        public MvcActionModel Postback { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String ExternalIdentifier { get; set; }

        [Required()]
        public IdentityProvider Provider { get; set; }

        [Boolean(RequiredValue = true, ErrorMessage = "You must accept the terms and conditions")]
        public Boolean AggreeToTerms { get; set; }

        public String DisplayName { get; set; }
    }
}