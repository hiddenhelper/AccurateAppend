using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.ChangeAccess.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="ToggleNationAccessCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class ToggleNationAccessCommandBootstrapper : BusCommandInitiatorConfiguration<ToggleNationAccessCommand>
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