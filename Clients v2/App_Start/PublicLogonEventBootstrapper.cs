using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Security;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="PublicLogonEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class PublicLogonEventBootstrapper : BusEventInitiatorConfiguration<PublicLogonEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "ClientsWebsite";
        }

        #endregion
    }
}