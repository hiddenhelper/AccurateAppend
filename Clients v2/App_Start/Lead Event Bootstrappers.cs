using System;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="LeadAssignedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class LeadAssignedEventBootstrapper : BusEventInitiatorConfiguration<CustomerManagement.Contracts.LeadAssignedEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "ClientsWebsite";
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="LeadCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class LeadCreatedEventBootstrapper : BusEventInitiatorConfiguration<LeadCreatedEvent>
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