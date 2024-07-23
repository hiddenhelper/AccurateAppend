using System;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Messaging;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Default bootstrapper configuring the <see cref="CreatePaymentProfileCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class CreatePaymentProfileCommandBootstrapper : BusCommandInitiatorConfiguration<CreatePaymentProfileCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "AdminWebsite";
        }

        #endregion
    }

    /// <summary>
    /// Bootstrapper configuring the <see cref="PaymentProfileCreatedEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class PaymentProfileCreatedEventBootstrapper : BusEventInitiatorConfiguration<PaymentProfileCreatedEvent>
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