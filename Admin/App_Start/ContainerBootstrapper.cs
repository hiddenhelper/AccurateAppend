using System.Diagnostics.Contracts;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Utilities;
using AccurateAppend.DQS;
using AccurateAppend.Standardization;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper responsible for initialization of the application root <see cref="IWindsorContainer"/>.
    /// </summary>
    public class ContainerBootstrapper : AbstractBootstrapper<IWindsorContainer>
    {
        #region Overrides

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public override void BeginInit()
        {
            // Create root container
            var container = this.CreateContainer();
            this.Result = container;
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
            Contract.EndContractBlock();

            // Instantiate a container, taking configuration from web.config
            var windsor = new WindsorContainer(
                new XmlInterpreter(new ConfigResource("castle"))
            );

            // Set global transient Encryption
            Default<IEncryptor>.Value = windsor.Resolve<IEncryptor>();

            // DQS
            windsor.Register(
                Component
                    .For<IAddressStandardizer>()
                    .LifestyleTransient()
                    .UsingFactoryMethod(() => ParserFactory.NewAddressStandardizer()));
            windsor.Register(
                Component
                    .For<INameStandardizer>()
                    .LifestyleTransient()
                    .UsingFactoryMethod(ParserFactory.NewNameStandardizer));

            return windsor;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convenient abstraction over this component to run everything in one call.
        /// </summary>
        /// <returns>The <see cref="IWindsorContainer"/> that can be further configured or leveraged.</returns>
        public static IWindsorContainer Create()
        {
            Contract.Ensures(Contract.Result<IWindsorContainer>() != null);
            Contract.EndContractBlock();

            var bootstrapper = new ContainerBootstrapper();
            var container = bootstrapper.Run();

            return container;
        }

        #endregion
    }
}