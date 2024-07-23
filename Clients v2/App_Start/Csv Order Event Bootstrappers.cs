using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="FileUploadedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class FileUploadedEventBootstrapper : BusEventInitiatorConfiguration<FileUploadedEvent>
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
    public class CsvQuoteCreatedEventBootstrapper : BusEventInitiatorConfiguration<QuoteCreatedEvent>
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
    /// Bootstrapper configuring the <see cref="CsvOrderPlacedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class CsvOrderPlacedEventBootstrapper : BusEventInitiatorConfiguration<CsvOrderPlacedEvent>
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
    /// Bootstrapper configuring the <see cref="CsvOrderExpiredEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class CsvOrderExpiredEventBootstrapper : BusEventInitiatorConfiguration<CsvOrderExpiredEvent>
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