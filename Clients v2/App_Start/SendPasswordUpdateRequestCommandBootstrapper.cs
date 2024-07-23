using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="SendPasswordUpdateRequestCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class SendPasswordUpdateRequestCommandBootstrapper : BusCommandInitiatorConfiguration<SendPasswordUpdateRequestCommand>
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