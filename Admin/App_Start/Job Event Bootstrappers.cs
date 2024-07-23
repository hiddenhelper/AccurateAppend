#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

#if DEBUG
using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="JobCompletedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class JobCompletedEventBootstrapper : BusEventInitiatorConfiguration<JobCompletedEvent>
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
    /// Bootstrapper configuring the <see cref="JobRequiresAdministrativeActionEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class JobRequiresAdministrativeActionEventBootstrapper : BusEventInitiatorConfiguration<JobRequiresAdministrativeActionEvent>
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
    /// Bootstrapper configuring the <see cref="JobCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class JobCreatedEventBootstrapper : BusEventInitiatorConfiguration<JobCreatedEvent>
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
#endif

