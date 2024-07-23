using System;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Operations.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="StoreFileCommandHandler"/> NServiceBus infrastructure to process the <see cref="StoreFileCommand"/>.
    /// </summary>
    public class StoreFileCommandHandlerBootstrapper : BusHandlerConfiguration<StoreFileCommandHandler, StoreFileCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreFileCommandHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public StoreFileCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
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

        /// <summary>
        /// Factory method to generate a component registration for the current handler. The default implementation
        /// simply creates registration for the handler with a lifestyle of Transient.
        /// </summary>
        protected override ComponentRegistration<StoreFileCommandHandler> CreateRegistration()
        {
            var configuration = base.CreateRegistration();
            configuration.DependsOn(
                Dependency.OnComponent("temp", "Temp"),
                Dependency.OnComponent("assisted", "Assisted Files"));

            return configuration;
        }

        #endregion
    }
}