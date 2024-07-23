#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ResetJobCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class ResetJobCommandBoostrapper : BusCommandInitiatorConfiguration<ResetJobCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "JobProcessing";
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="DeleteJobCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class DeleteJobCommandBoostrapper : BusCommandInitiatorConfiguration<DeleteJobCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "JobProcessing";
        }

        #endregion
    }
    /// <summary>
    /// Bootstrapper configuring the <see cref="ReassignJobCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class ReassignJobCommandBoostrapper : BusCommandInitiatorConfiguration<ReassignJobCommand>
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