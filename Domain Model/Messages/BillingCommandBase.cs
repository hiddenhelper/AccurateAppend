using System;
using NServiceBus;

namespace DomainModel.Messages
{
    [Obsolete("This type will soon be removed when usage billing is fully automated")]
    public class BillingCommandBase : ICommand
    {
        /// <summary>
        /// Gets or sets the period that the command should be processed for.
        /// </summary>
        public AccurateAppend.Core.DateSpan Period { get; set; }

        /// <summary>
        /// Gets or sets the email address that the notification should be sent to.
        /// </summary>
        public String EmailWhenDone { get; set; }

        /// <summary>
        /// Gets or sets the user identifier that initiated the request.
        /// </summary>
        public Guid Creator { get; set; }
    }
}