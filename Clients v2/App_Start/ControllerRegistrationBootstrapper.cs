using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Websites.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Responsible for locating and automatically registering all <see cref="System.Web.Mvc.IController"/> types in the current
    /// assembly as part of the initialization routine. All other components are deferred to the parent containers.
    /// </summary>
    /// <remarks>
    /// Will create a distinct child container environment just for the MVC Controllers in the application. The supplied
    /// root container will be used as the parent. Actual registration logic is deferred to the logic found in the
    /// <see cref="ControllersInstaller"/> component.
    ///
    /// This component is safe to call multiple times.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// aContainerToConfigure = ControllerRegistrationBootstrapper.Create(aContainerToConfigure);
    /// ]]>
    /// </code>
    /// </example>
    public class ControllerRegistrationBootstrapper : AbstractBootstrapper<IWindsorContainer>
    {
        #region Fields

        private readonly IWindsorContainer rootContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerRegistrationBootstrapper"/> class.
        /// </summary>
        /// <param name="rootContainer">The <see cref="IWindsorContainer"/> instance that is uses as the parent of the container this component will create.</param>
        public ControllerRegistrationBootstrapper(IWindsorContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException(nameof(rootContainer));
            Contract.EndContractBlock();

            this.rootContainer = rootContainer;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            // Get the predefined static configuration
            IResource resource = new FileResource(@"Configuration\Controllers.config");

            // Create a container based on this initial config
            var childContainer = new WindsorContainer(new XmlInterpreter(resource));
            
            // Now dynamically discover the rest of the controllers
            childContainer.Install(new ControllersInstaller());

            // Link the as part of the graph
            this.rootContainer.AddChildContainer(childContainer);

            this.Result = childContainer;
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
        /// <param name="rootContainer">The <see cref="IWindsorContainer"/> instance that is uses as the parent of the container this component will create.</param>
        public static IWindsorContainer Create(IWindsorContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException(nameof(rootContainer));
            Contract.Ensures(Contract.Result<IWindsorContainer>() != null);
            Contract.EndContractBlock();

            var boostrapper = new ControllerRegistrationBootstrapper(rootContainer);
            return boostrapper.Run();
        }

        #endregion
    }
}