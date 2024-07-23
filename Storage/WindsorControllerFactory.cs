using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// Controller factory bridging the MVC runtime to the <see cref="IWindsorContainer"/> environment. This
    /// controller factory will automatically register all <see cref="Controller"/> types in the current
    /// assembly as part of the initialization routine.
    /// </summary>
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> container;

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactory"/> class.
        /// </summary>
        /// <param name="coreFactory">The factory that creates the core application components shared across that application.</param>
        public WindsorControllerFactory(Func<IWindsorContainer> coreFactory)
        {
            if (coreFactory == null) throw new ArgumentNullException(nameof(coreFactory));
            Contract.EndContractBlock();

            // ReSharper disable ConvertToLocalFunction
            Func<IWindsorContainer> creator = () =>
            {
                // access the root system container
                var rootContainer = coreFactory();

                // Now register all the controller types in the current assembly as transient 
                var controllerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where typeof (IController).IsAssignableFrom(t)
                    select t;
                foreach (var t in controllerTypes.Where(t => !rootContainer.Kernel.HasComponent(t)))
                {
                    rootContainer.Register(Component.For(t).Named(t.FullName).LifeStyle.Transient);
                }

                return rootContainer;
            };
            // ReSharper restore ConvertToLocalFunction

            this.container = new Lazy<IWindsorContainer>(creator, LazyThreadSafetyMode.PublicationOnly);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;
            try
            {
                controller = (IController) this.container.Value.Resolve(controllerType);
            }
            catch (ArgumentNullException)
            {
                throw new Exception("GetControllerInstance: File Missing?=" + String.Join(",", requestContext.RouteData.Values.Select(a => a.ToString()).ToArray()));
            }
            catch (Exception ex)
            {
                throw new Exception($"GetControllerInstance: type={controllerType.Name} Message: '{ex.Message}'");
            }
            return controller;
        }

        #endregion
    }
}
