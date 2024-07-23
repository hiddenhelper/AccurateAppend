using System;
using AccurateAppend.Messaging;
using AccurateAppend.Sales.Contracts.Messages;

#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="SubscriptionCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class SubscriptionCreatedEventBootstrapper : BusEventInitiatorConfiguration<SubscriptionCreatedEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="SubscriptionCanceledEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class SubscriptionCanceledEventBootstrapper : BusEventInitiatorConfiguration<SubscriptionCanceledEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }
}