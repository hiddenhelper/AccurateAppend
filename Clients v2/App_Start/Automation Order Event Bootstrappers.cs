using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="QuoteCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class AutomationQuoteCreatedEventBootstrapper : BusEventInitiatorConfiguration<QuoteCreatedEvent>
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
    /// Bootstrapper configuring the <see cref="AutomationOrderPlacedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class AutomationOrderPlacedEventBootstrapper : BusEventInitiatorConfiguration<AutomationOrderPlacedEvent>
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
    /// Bootstrapper configuring the <see cref="AutomationOrderExpiredEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class AutomationOrderExpiredEventBootstrapper : BusEventInitiatorConfiguration<AutomationOrderExpiredEvent>
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