using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;
using AccurateAppend.Sales.Contracts.Messages;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="JobCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
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

    /// <summary>
    /// Bootstrapper configuring the <see cref="JobCompletedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
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
    /// Bootstrapper configuring the <see cref="JobDeletedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class JobDeletedEventBootstrapper : BusEventInitiatorConfiguration<JobDeletedEvent>
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
    /// Bootstrapper configuring the <see cref="DealCompletedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class DealCompletedEventBootstrapper : BusEventInitiatorConfiguration<DealCompletedEvent>
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