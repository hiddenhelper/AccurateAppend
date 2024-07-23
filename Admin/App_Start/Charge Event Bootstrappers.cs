using System;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Messaging;

#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ChargeProcessedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class ChargeProcessedEventBootstrapper : BusEventInitiatorConfiguration<ChargeProcessedEvent>
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
    /// Bootstrapper configuring the <see cref="RefundProcessedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class RefundProcessedEventBootstrapper : BusEventInitiatorConfiguration<RefundProcessedEvent>
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
    /// Bootstrapper configuring the <see cref="PaymentProfileDeletedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class PaymentProfileDeletedEventBootstrapper : BusEventInitiatorConfiguration<PaymentProfileDeletedEvent>
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