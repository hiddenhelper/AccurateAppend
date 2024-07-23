using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Messages
{
    /// <summary>
    /// Bus command to initiate a <see cref="AccurateAppend.Security.User"/> password reset request instruction.
    /// </summary>
    [Serializable()]
    public class SendPasswordUpdateRequestCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the user name value.
        /// </summary>
        public String UserName { get; set; }
    }
}