#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class
using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Messages.Admin;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="PushNotificationForInteractiveLogonEventHandler"/> NServiceBus infrastructure to process the <see cref="InteractiveLogonEvent"/>.
    /// </summary>
    public class PushNotificationForInteractiveLogonEventHandlerBootstrapper : BusHandlerConfiguration<PushNotificationForInteractiveLogonEventHandler, InteractiveLogonEvent>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNotificationForInteractiveLogonEventHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public PushNotificationForInteractiveLogonEventHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
        {
            if (busFactory == null) throw new ArgumentNullException(nameof(busFactory));

            this.busFactory = busFactory;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override IWindsorContainer CreateChildContainer()
        {
            return this.busFactory.Value;
        }

        #endregion
    }

#if DEBUG
    /// <summary>
    /// Bootstrapper configuring the <see cref="InteractiveLogonEvent"/> subscription via NServiceBus infrastructure.
    /// </summary>
    public class InteractiveLogonEventBootstrapper : BusEventInitiatorConfiguration<InteractiveLogonEvent>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return BusBootstrapper.EndpointName;
        }

        #endregion
    }
#endif
}