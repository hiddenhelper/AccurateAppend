using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Websites.Configuration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Configures the MVC runtime and routing configuration settings for the hosting
    /// application. Encapsulates all logic related to bootstrapping the ASP.Net MVC
    /// runtime configuration.
    /// </summary>
    /// <remarks>
    /// 1. Registers all MVC areas.
    /// 2. Creates a new <see cref="WindsorControllerFactory"/> and registers it with MVC.
    /// 3. Registers routes.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// MvcBootstrapper.Run(rootApplicationController);
    /// ]]>
    /// </code>
    /// </example>
    public class MvcBootstrapper : AbstractBootstrapper
    {
        #region Fields

        private readonly IWindsorContainer controllerContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcBootstrapper"/> class.
        /// </summary>
        /// <param name="controllerContainer">The <see cref="IWindsorContainer"/> instance configured to support the MVC runtime.</param>
        public MvcBootstrapper(IWindsorContainer controllerContainer)
        {
            if (controllerContainer == null) throw new ArgumentNullException(nameof(controllerContainer));
            Contract.EndContractBlock();

            this.controllerContainer = controllerContainer;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            AreaRegistration.RegisterAllAreas();
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(this.controllerContainer));
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            foreach (var thisEngine in ViewEngines.Engines.OfType<RazorViewEngine>())
            {
                RazorConfig.RegisterPaths(thisEngine);
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
        /// <remarks>
        /// This component performs no component registration logic. It is assumed the caller has properly configure the application
        /// prior to calling this method.
        /// </remarks>
        /// <param name="controllerContainer">The <see cref="IWindsorContainer"/> instance configured to support the MVC runtime.</param>
        public static void Run(IWindsorContainer controllerContainer)
        {
            if (controllerContainer == null) throw new ArgumentNullException(nameof(controllerContainer));
            Contract.EndContractBlock();

            var boostrapper = new MvcBootstrapper(controllerContainer);
            boostrapper.Run();
        }

        #endregion
    }
}