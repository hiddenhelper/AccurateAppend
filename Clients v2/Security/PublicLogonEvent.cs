using System;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Event published when a client logs into the public system.
    /// </summary>
    /// <remarks>
    /// Declared as part of this application as it is a temporary stop-gap until we load all the
    /// user base into ZenSell. This event will be removed at some point.
    /// </remarks>
    [Serializable()]
    public class PublicLogonEvent : IEvent
    {
        /// <summary>
        /// Gets or sets the username of the user that was created.
        /// </summary>
        /// <value>The username of the user that was created.</value>
        public String UserName { get; set; }
    }
}