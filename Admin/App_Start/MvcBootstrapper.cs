using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Websites.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Configures the MVC runtime and routing configuration.
    /// </summary>
    /// <remarks>
    /// 1. Adds AA Style View Location configuration to be enabled.
    /// 2. Registers all MVC areas.
    /// 3. Creates a <see cref="WindsorControllerFactory"/> and provides the factory method for dedicated child <see cref="IWindsorContainer"/>.
    /// 4. Registers routes, filters, and bundles.
    /// 
    /// <note type="warning">
    /// This bootstrapper cannot be used multiple times.
    /// </note>
    /// </remarks>
    public class MvcBootstrapper : AbstractBootstrapper
    {
        #region Fields

        private readonly IWindsorContainer rootFactory;
        private readonly IFacility[] facilities;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcBootstrapper"/> class.
        /// </summary>
        /// <param name="rootFactory">The wrapper to the root factory for the root or parent container for the application.</param>
        /// <param name="facilities">The set of optional <see cref="IFacility"/> instance to be added to the MVC dedicated container. These facilities MUST not already be initialized.</param>
        public MvcBootstrapper(IWindsorContainer rootFactory, params IFacility[] facilities)
        {
            if (rootFactory == null) throw new ArgumentNullException(nameof(rootFactory));
            Contract.EndContractBlock();

            this.rootFactory = rootFactory;
            this.facilities = facilities ?? new IFacility[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a flag indicating this bootstrapper has been run.
        /// </summary>
        protected Boolean IsInitialized { get; set; }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            if (this.IsInitialized) return;

            this.ConfigureViewLocations(ViewEngines.Engines.OfType<WebFormViewEngine>().First());
            this.ConfigureViewLocations(ViewEngines.Engines.OfType<RazorViewEngine>().First());

            AreaRegistration.RegisterAllAreas();
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(this.CreateContainer));

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <inheritdoc />
        public override void EndInit()
        {
            this.IsInitialized = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the view engine support for view and partial view locations.
        /// </summary>
        /// <remarks>
        /// Adds support for the AA style view location support (/Areas/{Area}/{Controller}/Views/{ViewName}.aspx)
        /// It does NOT remove already configured locations.
        /// </remarks>
        /// <param name="engine">The <see cref="WebFormViewEngine"/> to configure.</param>
        protected virtual void ConfigureViewLocations(WebFormViewEngine engine)
        {
            var locations = new HashSet<String>(engine.AreaViewLocationFormats, StringComparer.OrdinalIgnoreCase)
            {
                "~/Areas/{2}/{1}/Views/{0}.aspx",
                "~/Areas/{2}/{1}/Views/{0}.ascx"
            };
            engine.AreaViewLocationFormats = locations.ToArray();

            locations = new HashSet<String>(engine.AreaPartialViewLocationFormats, StringComparer.OrdinalIgnoreCase)
            {
                "~/Areas/{2}/{1}/Views/{0}.ascx"
            };
            engine.AreaPartialViewLocationFormats = locations.ToArray();
        }

        /// <summary>
        /// Configures the view engine support for view and partial view locations.
        /// </summary>
        /// <remarks>
        /// Adds support for the AA style view location support (/Areas/{Area}/{Controller}/Views/{ViewName}.cshtml)
        /// It does NOT remove already configured locations.
        /// </remarks>
        /// <param name="engine">The <see cref="RazorViewEngine"/> to configure.</param>
        protected virtual void ConfigureViewLocations(RazorViewEngine engine)
        {
            var locations = new HashSet<String>(engine.AreaViewLocationFormats, StringComparer.OrdinalIgnoreCase)
            {
                "~/Areas/{2}/{1}/Views/{0}.cshtml"
            };
            engine.AreaViewLocationFormats = locations.ToArray();

            locations = new HashSet<String>(engine.AreaPartialViewLocationFormats, StringComparer.OrdinalIgnoreCase)
            {
                "~/Areas/{2}/{1}/Views/{0}.cshtml"
            };
            engine.AreaPartialViewLocationFormats = locations.ToArray();
        }

        /// <summary>
        /// The dependencies in the controller will be satisfied from the parent containers.
        /// </summary>
        /// <returns>The <see cref="IWindsorContainer"/> instance that is to be used for the MVC runtime.</returns>
        protected virtual IWindsorContainer CreateContainer()
        {
            // access the root system container
            var rootContainer = this.rootFactory;

            // Instantiate a child container just for the controllers
            IResource resource = new FileResource(@"Configuration\Controllers.config");
            var windsor = new WindsorContainer(new XmlInterpreter(resource));
            rootContainer.AddChildContainer(windsor);

            // If we have been provided with any desired facilities then go ahead and add them
            foreach (var facility in this.facilities)
            {
                windsor.AddFacility(facility);
            }

            // Locate all controller types in the current assembly
            var controllerTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IController).IsAssignableFrom(t));

            // Register controller types that we already do not have a registration (Configuration\Controllers.config can have static registrations so skip)
            foreach (var t in controllerTypes.Where(t => !windsor.Kernel.HasComponent(t)))
            {
                windsor.Register(Component.For(t).Named(t.FullName).LifeStyle.Transient);
            }

            return windsor;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convenient abstraction over this component to run everything in one call.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> that will be further configured.</param>
        /// <param name="facilities">THe params array of any <see cref="IFacility"/> instances that are desired to be placed on the MVC level container.</param>
        public static void Create(IWindsorContainer container, params IFacility[] facilities)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Contract.EndContractBlock();

            var boostrapper = new MvcBootstrapper(container, facilities);
            boostrapper.Run();
        }

        #endregion
    }
}