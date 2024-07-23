using System;
using AccurateAppend.Messaging;
using DomainModel.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="ResetClientHistorgramCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class ResetClientHistorgramCommandBootstrapper : BusCommandInitiatorConfiguration<ResetClientHistorgramCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "AdminWebsite";
        }

        #endregion
    }
}