using System;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Messages.Sales;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="CreateDealsForJobCompletedEventHandler"/> NServiceBus infrastructure to process the <see cref="JobCompletedEvent"/>.
    /// </summary>
    public class CreateDealsForJobCompletedEventHandlerBootstrapper : BusHandlerConfiguration<CreateDealsForJobCompletedEventHandler, JobCompletedEvent>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDealsForJobCompletedEventHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public CreateDealsForJobCompletedEventHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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