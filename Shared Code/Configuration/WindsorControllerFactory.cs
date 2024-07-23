using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Core.Collections.Generic;
using Castle.Windsor;

namespace AccurateAppend.Websites.Configuration
{
    /// <summary>
    /// Controller factory bridging the MVC runtime to the <see cref="IWindsorContainer"/> environment. This
    /// controller factory will only support requests for <see cref="IController"/> based components that are
    /// registered on the provided container instance. All other requests will be deferred to the base implementation.
    /// </summary>
    /// <remarks>
    /// The container that is supplied to the component will be assumed to previously be configured. No assumption
    /// about the container instances will be made. Callers are expected to properly configure the environment prior
    /// to using this component.
    /// </remarks>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> container;
        private readonly Lazy<ControllerCache> controllers;

        #endregion

        #region Constructors

        private WindsorControllerFactory()
        {
            this.controllers = new Lazy<ControllerCache>(() =>
                {
                    var cache = new ControllerCache();

                    ControllersInstaller.ScanForControllers().ForEach(t => cache.Add(t));

                    return cache;
                },
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactory"/> class.
        /// </summary>
        /// <param name="container">The configured <see cref="IWindsorContainer"/> instance that is used to create <see cref="Controller"/> instances from.</param>
        public WindsorControllerFactory(IWindsorContainer container) : this()
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Contract.EndContractBlock();

            this.container = new Lazy<IWindsorContainer>(() => container, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The factory that creates the MVC application component container for the application.</param>
        public WindsorControllerFactory(Func<IWindsorContainer> containerFactory) : this()
        {
            if (containerFactory == null) throw new ArgumentNullException(nameof(containerFactory));
            Contract.EndContractBlock();

            this.container = new Lazy<IWindsorContainer>(containerFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the factory for the <see cref="WindsorContainer"/> instance used by for MVC.
        /// </summary>
        protected virtual IWindsorContainer Container => this.container.Value;

        #endregion

        #region Overrides

        /// <summary>Retrieves the controller type for the specified name and request context.</summary>
        /// <returns>The controller type.</returns>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerName">The name of the controller.</param>
        protected override Type GetControllerType(RequestContext requestContext, String controllerName)
        {
            var areaName = requestContext.RouteData.DataTokens.Area();
            if (!String.IsNullOrWhiteSpace(areaName))
            {
                var cache = this.controllers.Value;

                var rootNamespace = this.GetType().Assembly.GetName().Name;

                // First try the AA format
                var typeName = $"{rootNamespace}.Areas.{areaName}.{controllerName}.Controller";
                if (cache.Contains(typeName)) return cache[typeName];

                // then try the hybrid format
                typeName = $"{rootNamespace}.Areas.{areaName}.{controllerName}.{controllerName}Controller";
                if (cache.Contains(typeName)) return cache[typeName];
            }

            // fall through to Default MVC format
            return base.GetControllerType(requestContext, controllerName);
        }

        /// <inheritdoc />
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (controllerType != null && this.Container.Kernel.HasComponent(controllerType))
                {
                    return (IController)this.Container.Resolve(controllerType);
                }

                return base.GetControllerInstance(requestContext, controllerType);
            }
            catch (ArgumentNullException ex)
            {
                throw new ConfigurationErrorsException($"{nameof(this.GetControllerInstance)}: File Missing?=" + String.Join(",", requestContext.RouteData.Values.Select(a => a.ToString()).ToArray()), ex);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException($"{nameof(this.GetControllerInstance)}: type={controllerType?.Name} Message: {ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public override void ReleaseController(IController controller)
        {
            this.Container.Release(controller);
        }

        #endregion
    }
}