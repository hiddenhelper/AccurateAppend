using System;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.ChargeProcessing.Handlers;
using AccurateAppend.Messaging;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ProcessChargeCommandHandler"/> NServiceBus infrastructure to process the <see cref="ProcessChargeCommand"/>.
    /// </summary>
    public class ProcessChargeCommandHandlerBootstrapper : BusHandlerConfiguration<ProcessChargeCommandHandler, ProcessChargeCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessChargeCommandHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public ProcessChargeCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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
}