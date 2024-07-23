using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Definitions;
using Castle.Windsor;
using EventLogger;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Single location for all initialization code for the entire application.
    /// </summary>
    /// <remarks>
    /// This component allows the order of start up configuration components, themselves
    /// generally additional <see cref="AbstractBootstrapper"/> types, to be defined and
    /// linked together without the individual sub-configurations needing knowledge of each
    /// other. By wrapping this code up in this component here, it simplifies the overall
    /// start up logic to be a run and forget call by the application entry. No actual
    /// configuration logic is performed here, just the logic of the individual configurations
    /// that should be run. They in turn encapsulate their internal logic from possibly
    /// leaking into any parts of the rest of the code base.
    ///
    /// In addition, bootstrap failure logging and handling is located here freeing
    /// individual sub-configurations from needing to worry about this.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// var aConfiguredContainerHierarchy = Bootstrapper.Configure();
    /// ]]>
    /// </code>
    /// </example>
    public class Bootstrapper : AbstractBootstrapper<IWindsorContainer>
    {
        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            if (this.IsInitialized) return;
            
            try
            {
                // Configure the global app for logging
                LoggingConfig.Execute();

                // Create root container
                var container = ContainerBootstrapper.Create();

                // Set up all files
                FileLocationsBootstrapper.Create(container);

                // Zen Components
                ZenDesk.Configuration.DefaultBootstrapper.Create(container);

                // Create MVC controller container
                var controllers = ControllerRegistrationBootstrapper.Create(container);

                // MVC registrations
                MvcBootstrapper.Run(controllers);

                // Configure bus environment
                BusBootstrapper.Create(container);

                this.Result = controllers;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Logger.LogEvent(ex, TraceEventType.Critical, Severity.Fatal, "Clients", description: "Exception encountered on Client startup");
                throw;
            }
        }

        /// <inheritdoc />
        public override void EndInit()
        {

        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convenient abstraction over this component to run everything in one call.
        /// </summary>
        public static IWindsorContainer Configure(EventHandler onInitialized = null)
        {
            Contract.Ensures(Contract.Result<IWindsorContainer>() != null);

            var boostrapper = new Bootstrapper();
            if (onInitialized != null) boostrapper.Initialized += onInitialized;

            return boostrapper.Run();
        }

        #endregion
    }
}