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
    /// Bootstrapper configuring the <see cref="ResetClientHistorgramCommandHandler"/> NServiceBus infrastructure to process the <see cref="ResetClientHistorgramCommand"/>.
    /// </summary>
    public class ResetClientHistorgramCommandHandlerBootstrapper : BusHandlerConfiguration<ResetClientHistorgramCommandHandler, ResetClientHistorgramCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetClientHistorgramCommandHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public ResetClientHistorgramCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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
    /// Bootstrapper configuring the <see cref="ResetClientHistorgramCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class ResetClientHistorgramCommandBootstrapper : BusCommandInitiatorConfiguration<ResetClientHistorgramCommand>
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