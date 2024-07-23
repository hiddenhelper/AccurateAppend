using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.ListBuilder.Messaging;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="CreateListBuilderCartCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class CreateListBuilderCartCommandBootstrapper : BusCommandInitiatorConfiguration<CreateListBuilderCartCommand>
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