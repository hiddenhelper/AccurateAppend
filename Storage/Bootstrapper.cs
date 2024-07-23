using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Plugin.Storage;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using EventLogger;

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// Single location for all initialization code for the entire application.
    /// </summary>
    public class Bootstrapper : AbstractBootstrapper
    {
        #region Overrides

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public override void BeginInit()
        {
            if (this.IsInitialized) return;

            // Override to use the new Azure queue
            Logger.Loggers.Clear();
            Logger.Loggers.Add(new EventLogger.AzureQueue.QueueLogger());

#if DEBUG
            if (Debugger.IsAttached)
            {
                // Clear out loggers if we're debugging so we don't cause crap alerts
                Logger.Loggers.Clear();
            }
#endif
            try
            {
                Logger.GlobalOverride(Application.Storage);

                AreaRegistration.RegisterAllAreas();
                ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(this.CreateContainer));

                RouteConfig.RegisterRoutes(RouteTable.Routes);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, TraceEventType.Critical, Severity.Fatal, "Exception encountered on Azure startup");
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

        #region Methods

        /// <summary>
        /// Factory methods used to interact with the core configuration and will be responsible for creating the shared configuration
        /// regardless of runtime.
        /// </summary>
        /// <returns>The new <see cref="IWindsorContainer"/> instance.</returns>
        protected virtual IWindsorContainer CreateContainer()
        {
            Contract.Ensures(Contract.Result<IWindsorContainer>() != null);

            try
            {
                // Instantiate a container, taking configuration from web.config
                var windsor = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

                // Azure components
                StandardFileLocationsBootstrapper.Create(windsor);

                return windsor;
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, TraceEventType.Critical, Severity.Fatal, "Exception encountered on container configuration");
                throw;
            }
        }

        #endregion
    }
}