using System;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Admin
{
    /// <summary>
    /// Event raised when a user logs onto the Admin application interatively (that is, via the log in form).
    /// </summary>
    /// <remarks>
    /// This event is completely internal to the admin application for is defined here.
    /// </remarks>
    [Serializable()]
    public class InteractiveLogonEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the user that logged on.
        /// </summary>
        public Guid UserId { get; set; }
    }
}