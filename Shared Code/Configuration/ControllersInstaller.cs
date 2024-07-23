using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Configuration
{
    /// <summary>
    /// An <see cref="IWindsorInstaller"/> component that specifically scans the current assembly
    /// for all types that implement <see cref="IController"/>. Any discovered type that is not
    /// already installed in the supplied container instance will be registered. All registration
    /// keys are based on <see cref="Type.FullName"/>.
    /// </summary>
    public class ControllersInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        /// <inheritdoc />
        public virtual void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.
                    FromThisAssembly().
                    BasedOn<IController>().
                    If(c => c.Name.EndsWith(nameof(Controller))).
                    If(c => !container.Kernel.HasComponent(c)).
                    LifestyleTransient());
        }

        #endregion

        #region Locator

        /// <summary>
        /// Helper method to enumerate the current assembly for types that implement <see cref="IController"/>.
        /// </summary>
        /// <remarks>
        /// This method really exists as there's other places that require the the types that are found by this installer.
        /// While this method isn't actually used during registration, the duplicated logic here at least keeps it in
        /// one place.
        /// </remarks>
        /// <returns>A sequence of all types in the current assembly that implement <see cref="IController"/>.</returns>
        public static IEnumerable<Type> ScanForControllers()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IController).IsAssignableFrom(t));
        }

        #endregion
    }
}