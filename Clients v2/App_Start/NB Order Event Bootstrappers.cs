using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ListSelectedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class ListSelectedEventBootstrapper : BusEventInitiatorConfiguration<ListSelectedEvent>
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
    /// Bootstrapper configuring the <see cref="QuoteCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class NbQuoteCreatedEventBootstrapper : BusEventInitiatorConfiguration<QuoteCreatedEvent>
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
    /// Bootstrapper configuring the <see cref="NationBuilderOrderPlacedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class NationBuilderOrderPlacedEventBootstrapper : BusEventInitiatorConfiguration<NationBuilderOrderPlacedEvent>
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
    /// Bootstrapper configuring the <see cref="NationBuilderOrderExpiredEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class NationBuilderOrderExpiredEventBootstrapper : BusEventInitiatorConfiguration<NationBuilderOrderExpiredEvent>
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