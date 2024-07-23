using System;
using System.Diagnostics;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Definitions;
using Castle.Windsor;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Single location for all initialization code for the entire application.
    /// </summary>
    public class Bootstrapper : AbstractBootstrapper<IWindsorContainer>
    {
        #region Overrides

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
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

                // Add ZenDesk support
                ZenDesk.Configuration.DefaultBootstrapper.Create(container);

                // Configure bus
                BusBootstrapper.Create(container);

                // Create the user logging event facility
                var bus = container.Resolve<IMessageSession>();
                var facility = new EventManagerBridgeFacility(bus);

                // MVC registrations
                MvcBootstrapper.Create(container, facility);
                
                // Custom model binders
                ModelBinderBootstrapper.Execute();
                
                base.Result = container;
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, TraceEventType.Critical, Severity.Fatal, Application.AccurateAppend_Admin.ToString(), description: "Exception encountered on Admin startup");
                throw;
            }
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public override void EndInit()
        {

        }

        #endregion

        #region Temp

        /// <summary>
        /// Added temporarily to work around a design issue.
        /// </summary>
        public new IWindsorContainer Result => base.Result;

        #endregion
    }
}