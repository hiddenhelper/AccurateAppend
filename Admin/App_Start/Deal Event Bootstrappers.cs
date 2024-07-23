#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

using System;
using AccurateAppend.Messaging;
using AccurateAppend.Sales.Contracts.Messages;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="DealCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class DealCreatedEventBootstrapper : BusEventInitiatorConfiguration<DealCreatedEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="DealCanceledEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class DealCanceledEventBootstrapper : BusEventInitiatorConfiguration<DealCanceledEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
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
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="DealRefundedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class DealRefundedEventBootstrapper : BusEventInitiatorConfiguration<DealRefundedEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="DealPublicKeyChangedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class DealPublicKeyChangedEventBootstrapper : BusEventInitiatorConfiguration<DealPublicKeyChangedEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="DealApprovedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    /// 
    public class DealApprovedEventBootstrapper : BusEventInitiatorConfiguration<DealApprovedEvent>
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