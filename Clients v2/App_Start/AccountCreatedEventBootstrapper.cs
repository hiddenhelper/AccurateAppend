using System;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="AccountCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class AccountCreatedEventBootstrapper : BusEventInitiatorConfiguration<AccountCreatedEvent>
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