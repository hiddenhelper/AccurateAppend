#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class
using System;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Messages.Admin;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="LogUserActionCommandHandler"/> NServiceBus infrastructure to process the <see cref="LogUserActionCommand"/>.
    /// </summary>
    public class LogUserActionCommandHandlerBootstrapper : BusHandlerConfiguration<LogUserActionCommandHandler, LogUserActionCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUserActionCommandHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public LogUserActionCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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
    /// Bootstrapper configuring the <see cref="LogUserActionCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class LogUserActionCommandBootstrapper : BusCommandInitiatorConfiguration<LogUserActionCommand>
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