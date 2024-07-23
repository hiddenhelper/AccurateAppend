using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="CreateCsvJobCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class CreateCsvJobCommandBootstrapper : BusCommandInitiatorConfiguration<CreateCsvJobCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "JobProcessing";
        }

        #endregion
    }
}