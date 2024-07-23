using System;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SendPaymentUpdate.Messages
{
    /// <summary>
    /// Command to instruct the admin to send a payment update link via the admin for a user.
    /// </summary>
    /// <remarks>
    /// This command is purposely defined internally to this application. It is never
    /// intended that any other system or code base is at all expected to interact
    /// with this command processing.
    /// </remarks>
    [Serializable()]
    public class SendPaymentExpiredLinkCommand : ICommand
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public Guid UserId { get; set; }
    }
}