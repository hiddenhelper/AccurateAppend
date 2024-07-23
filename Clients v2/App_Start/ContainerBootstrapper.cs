using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Configuration;
using AccurateAppend.DQS;
using AccurateAppend.Standardization;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using DAL.Standardization;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Responsible for setting up the core system <see cref="IWindsorContainer"/> instance for
    /// the entire application. Root level services will be registered here. This component does
    /// NOT perform any container management functions for the application, only creating the root
    /// container.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// var aContainerToUse = ContainerBootstrapper.Create();
    /// ]]>
    /// </code>
    /// </example>
    public class ContainerBootstrapper : AbstractBootstrapper<IWindsorContainer>
    {
        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            try
            {
                // Instantiate a container, taking configuration from web.config
                var windsor = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

                // DQS components
                windsor.Register(Component.For<INameStandardizer>()
                    .Named("NameParser")
                    .UsingFactoryMethod(ParserFactory.NewNameStandardizer)
                    .LifestyleTransient());

                windsor.Register(
                    Component.For<IAddressStandardizer>()
                        .Named("AddressParser")
                        .LifestyleTransient()
                        .UsingFactoryMethod(() => ParserFactory.NewAddressStandardizer(new ParserFactory.AddressStandardizerSettings() {SingleThreaded = true, DisableCaching = true})));

                ParserFactory.Register(() => new EmailObject());
                windsor.Register(Component.For<IEmailStandardizer>()
                    .Named("EmailParser")
                    .UsingFactoryMethod(ParserFactory.NewEmailStandardizer)
                    .LifestyleTransient());

                this.Result = windsor;
            }
            catch (Exception)
            {
                if (Debugger.IsAttached) Debugger.Break();
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
        public static IWindsorContainer Create()
        {
            Contract.Ensures(Contract.Result<IWindsorContainer>() != null);

            var boostrapper = new ContainerBootstrapper();
            var container = boostrapper.Run();

            return container;
        }

        #endregion
    }
}