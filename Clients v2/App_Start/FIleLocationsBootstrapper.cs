using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Utilities;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Orchestration.LocalData;
using AccurateAppend.Plugin.Storage;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Performs configuration related to build file system locations.
    /// </summary>
    /// <remarks>
    /// This component sets up the locations and related locations.
    /// -Uses the <see cref="StandardFileLocationsBootstrapper"/> for default configs
    /// -Proceeds to acquire and set up the Operation Definitions manager for use
    /// -Builds and registers a singleton instance of <see cref="StandardFileLocations"/> component
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// FileLocationsBootstrapper.Create(aContainerToConfigure);
    /// ]]>
    /// </code>
    /// </example>
    public class FileLocationsBootstrapper : AbstractBootstrapper
    {
        #region Fields

        private readonly IWindsorContainer container;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLocationsBootstrapper"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> instance that is uses as the target of component registrations.</param>
        public FileLocationsBootstrapper(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Contract.EndContractBlock();

            this.container = container;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        /// <remarks>
        /// This method initializes the container configuration and then resolved instances of many locations. Normally
        /// proper usage of Windsor requires the caller to properly release them back the container. However as we don't
        /// know the lifetime (or better put, shouldn't know) we purposely leave them active and referenced on the FileContext
        /// component we manually created a singleton instance of. Even though we know the lifetime of the locations will be
        /// Singleton, it is too easy for a configuration change to cause an issue by disposing of a location without knowing
        /// we're doing this.
        /// </remarks>
        public override void BeginInit()
        {
            if (this.IsInitialized) return;

            // Configure Operation Definitions for Local Data
            OperationDefinitionDataBootstrapper.Configure();
            container.Register(
                Component
                    .For<IFileLocation>()
                    .Named("Operation Definitions")
                    .LifestyleSingleton()
                    .Instance(OperationManager.DefaultManifestLocation));

            // Azure components
            StandardFileLocationsBootstrapper.Create(container);

            // Column Mapper data
            ColumnMapper.LocalData.ColumnMapperDataBootstrapper.Configure();

            // Add the shortcut
            {
                var rawCustomerFiles = this.container.Resolve<IFileLocation>("Raw Customer Files");
                var inbox = this.container.Resolve<IFileLocation>("inbox");
                var outbox = this.container.Resolve<IFileLocation>("outbox");
                var temp = this.container.Resolve<IFileLocation>("temp");
                var operations = this.container.Resolve<IFileLocation>("Operation Definitions");
                var assisted = this.container.Resolve<IFileLocation>("Assisted Files");

                var context = new StandardFileLocations(inbox, outbox, assisted, rawCustomerFiles, temp, operations);
                this.container.Register(Component.For<StandardFileLocations>().Instance(context).LifestyleSingleton());
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
        /// <param name="rootContainer">The <see cref="IWindsorContainer"/> instance that is uses as the parent of the container this component will create.</param>
        public static void Create(IWindsorContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException(nameof(rootContainer));
            Contract.EndContractBlock();

            var boostrapper = new FileLocationsBootstrapper(rootContainer);
            boostrapper.Run();
        }

        #endregion
    }
}