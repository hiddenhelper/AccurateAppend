#pragma warning disable SA1649 // File name must match first type name
#pragma warning disable SA1402 // File may only contain a single class

using System;
using AccurateAppend.Websites.Clients.Areas.ListBuilder.BuildList.Messaging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="GenerateListCommandHandler"/> NServiceBus infrastructure to process the <see cref="GenerateListCommand"/>.
    /// </summary>
    public class GenerateListCommandHandlerBootstrapper : Messaging.BusHandlerConfiguration<GenerateListCommandHandler, GenerateListCommand>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateListCommandHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public GenerateListCommandHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
        {
            if (busFactory == null) throw new ArgumentNullException(nameof(busFactory));

            this.busFactory = busFactory;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override ComponentRegistration<GenerateListCommandHandler> CreateRegistration()
        {
            return Component
                .For<GenerateListCommandHandler>()
                .LifestyleTransient()
                .DependsOn(Dependency.OnComponent("temp", "Temp"));
        }

        /// <inheritdoc />
        protected override IWindsorContainer CreateChildContainer()
        {
            return this.busFactory.Value;
        }

        #endregion
    }
    
    /// <summary>
    /// Default bootstrapper configuring the <see cref="GenerateListCommand"/> initiator NServiceBus infrastructure.
    /// </summary>
    public class GenerateListCommandBootstrapper : Messaging.BusCommandInitiatorConfiguration<GenerateListCommand>
    {
        #region Overrides

        /// <inheritdoc />
        protected override String CreateStandardNameForDestination()
        {
            return "ClientsWebsite";
        }

        #endregion
    }
}