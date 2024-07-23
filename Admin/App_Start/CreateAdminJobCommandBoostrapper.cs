using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="CreateAdminJobCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class CreateAdminJobCommandBoostrapper : BusCommandInitiatorConfiguration<CreateAdminJobCommand>
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