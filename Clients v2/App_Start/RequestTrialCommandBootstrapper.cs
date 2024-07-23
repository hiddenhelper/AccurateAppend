using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.Api.Trial.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="RequestTrialCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class RequestTrialCommandBootstrapper : BusCommandInitiatorConfiguration<RequestTrialCommand>
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