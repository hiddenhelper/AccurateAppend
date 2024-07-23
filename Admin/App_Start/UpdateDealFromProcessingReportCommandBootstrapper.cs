using System;
using AccurateAppend.Messaging;
using AccurateAppend.Sales.Contracts.Messages;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="UpdateDealFromProcessingReportCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class UpdateDealFromProcessingReportCommandBootstrapper : BusCommandInitiatorConfiguration<UpdateDealFromProcessingReportCommand>
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