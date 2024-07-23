using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Messages.JobProcessing;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="MonitorJobSubmittedEventHandler"/> NServiceBus infrastructure to process the <see cref="JobCreatedEvent"/>.
    /// </summary>
    public class MonitorJobSubmittedEventHandlerBootstrapper : BusHandlerConfiguration<MonitorJobSubmittedEventHandler, JobCreatedEvent>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorJobSubmittedEventHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public MonitorJobSubmittedEventHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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