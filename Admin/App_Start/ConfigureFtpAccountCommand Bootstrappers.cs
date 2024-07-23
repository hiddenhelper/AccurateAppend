#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class
using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Messages;
using Castle.Windsor;
using DomainModel.Messages;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ConfigureFtpAccountCommandHandler"/> NServiceBus infrastructure to process the <see cref="ConfigureFtpAccountCommand"/>.
    /// </summary>
    public class ConfigureFtpAccountCommandHandlerBootstrapper : BusHandlerConfiguration<ConfigureFtpAccountCommandHandler, ConfigureFtpAccountCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureFtpAccountCommandHandler"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public ConfigureFtpAccountCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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

    /// <summary>
    /// Bootstrapper configuring the <see cref="ConfigureFtpAccountCommand"/> initiating NServiceBus infrastructure.
    /// </summary>
    public class ConfigureFtpAccountCommandBootstrapper : BusCommandInitiatorConfiguration<ConfigureFtpAccountCommand>
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
