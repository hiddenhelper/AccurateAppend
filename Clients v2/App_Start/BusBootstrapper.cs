using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Configuration;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts.Configuration;
using AccurateAppend.Security;
using AccurateAppend.ZenDesk.Contracts.Support.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using EventLogger;
using Microsoft.Win32;
using NServiceBus;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Configures the system level Bus.
    /// </summary>
    /// <remarks>
    /// 1. Creates a new child container from the supplied root.
    /// 2. Sets up any new <see cref="AccurateAppend.Data.IBus"/> endpoints in the PARENT container.
    /// 3. Sets up any <see cref="IBusHandler"/> endpoints in the CHILD container.
    /// 4. Registers all endpoints types as components with Transient lifestyle.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// // Just execute the static entrypoint
    /// BusBootstrapper.Create(aContainerToConfigure);
    /// ]]>
    /// </code>
    /// </example>
    public class BusBootstrapper : ConcreteBusBootstrapper
    {
        #region Fields

        private static readonly Lazy<IWindsorContainer> ContainerFactory = new Lazy<IWindsorContainer>(() =>
            {
                IResource resource = new FileResource(@"Configuration\Bus.config");
                var busContainer = new WindsorContainer(new XmlInterpreter(resource));

                // Setup site cache
                if (!SiteCache.IsInitialized)
                {
                    SiteCache.FactoryMethod(() =>
                    {
                        var context = busContainer.Resolve<ISessionContext>();
                        try
                        {
                            var results = context.SetOf<SiteDetail>().Include(s => s.Application).ToArray();
                            return results.Select(s =>
                            {
                                var i = new SiteCache.SiteInfo(s);
                                return i;
                            }).ToArray();
                        }
                        finally
                        {
                            busContainer.Release(context);
                        }
                    });
                }

                return busContainer;
            },
            LazyThreadSafetyMode.PublicationOnly);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BusBootstrapper"/> class.
        /// </summary>
        /// <param name="rootFactory">The wrapper to the root factory for the root or parent container for the application.</param>
        /// <param name="containerFactory">The shared bus container routine for handler initialization.</param>
        /// <param name="channelFactory">Responsible for creating the appropriate </param>
        public BusBootstrapper(IWindsorContainer rootFactory, Lazy<IWindsorContainer> containerFactory, EndpointConfigurationBase channelFactory) : base(rootFactory, containerFactory, channelFactory)
        {
            if (rootFactory == null) throw new ArgumentNullException(nameof(rootFactory));
            if (channelFactory == null) throw new ArgumentNullException(nameof(channelFactory));
            Contract.EndContractBlock();

            // Trial Api
            this.InitiatorConfigurations.Add(new RequestTrialCommandBootstrapper());

            #region Event Subscriptions

            // Send Email For Create User
            this.InitiatorConfigurations.Add(new AccountCreatedEventBootstrapper());

            // Sync Logon to ZenSell
            this.InitiatorConfigurations.Add(new PublicLogonEventBootstrapper());

            // Handle automated lead capture
            this.InitiatorConfigurations.Add(new LeadCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new LeadAssignedEventBootstrapper());

            // Create payment
            this.InitiatorConfigurations.Add(new PaymentProfileCreatedEventBootstrapper());
            
            #endregion

            // Reset Histogram
            this.InitiatorConfigurations.Add(new ResetClientHistorgramCommandBootstrapper());

            // Create Payment Profile
            this.InitiatorConfigurations.Add(new CreatePaymentProfileCommandBootstrapper());

            // Send Password Update Request
            this.InitiatorConfigurations.Add(new SendPasswordUpdateRequestCommandBootstrapper());

            // List Builder Generate
            this.InitiatorConfigurations.Add(new GenerateListCommandBootstrapper());
            this.HandlerConfigurations.Add(new GenerateListCommandHandlerBootstrapper(containerFactory));
            
            // List Builder Sales Saga
            this.InitiatorConfigurations.Add(new CreateListBuilderCartCommandBootstrapper());

            // De/Reactivate Nations
            this.InitiatorConfigurations.Add(new ToggleNationAccessCommandBootstrapper());

            // Job Processing
            //==========================================
            this.InitiatorConfigurations.Add(new CreateCsvJobCommandBootstrapper());

            // Handle jobs for public orders
            this.InitiatorConfigurations.Add(new JobCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new JobCompletedEventBootstrapper());
            this.InitiatorConfigurations.Add(new JobDeletedEventBootstrapper());

            // Handle bills for public orders
            this.InitiatorConfigurations.Add(new DealCompletedEventBootstrapper());

            // Handle CSV Order Events
            //this.InitiatorConfigurations.Add(new CsvCartCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new FileUploadedEventBootstrapper());
            this.InitiatorConfigurations.Add(new CsvQuoteCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new CsvOrderPlacedEventBootstrapper());
            this.InitiatorConfigurations.Add(new CsvOrderExpiredEventBootstrapper());

            // Handle Automation Order Events
            //this.InitiatorConfigurations.Add(new AutomationCartCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new AutomationQuoteCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new AutomationOrderPlacedEventBootstrapper());
            this.InitiatorConfigurations.Add(new AutomationOrderExpiredEventBootstrapper());

            // Handle NB Order Events
            //this.InitiatorConfigurations.Add(new NationBuilderCartCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new ListSelectedEventBootstrapper());
            this.InitiatorConfigurations.Add(new NbQuoteCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new NationBuilderOrderPlacedEventBootstrapper());
            this.InitiatorConfigurations.Add(new NationBuilderOrderExpiredEventBootstrapper());
            //==========================================

            // Zendesk
            this.InitiatorConfigurations.Add(new CreateTicketCommandBootstrapper()
            {
                Environment = Debugger.IsAttached ? DeploymentEnvironment.Development : DeploymentEnvironment.Production
            });

            #region Operations

            // Send emails
            this.InitiatorConfigurations.Add(new SendEmailCommandBootstrapper());
            this.InitiatorConfigurations.Add(new StoreFileCommandBootstrapper());

            #endregion
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convenient abstraction over this component to run everything in one call.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> that will be further configured.</param>
        /// <returns>
        /// A configured <see cref="IWindsorContainer"/> instance that should be used by the hosting application.
        /// This may or may not be the same instance in <paramref name="container"/>.
        /// </returns>
        public static void Create(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Contract.EndContractBlock();

            const String ChannelName = "ClientsWebsite";
#if DEBUG
            var key = Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware");
            key.SetValue("TrialStart", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"));
            key.Close();
#endif

            var channelBuilder = new ConcreteBusChannelBootstrapper(ChannelName, () => ConfigurationManager.ConnectionStrings["ASPMembershipDB"].ConnectionString);
            channelBuilder.EnableAuditQueue = Debugger.IsAttached;
            channelBuilder.OutgoingMutators.Add(() => new PrincipleMutator());
            channelBuilder.EnableDialogs = false;
            channelBuilder.ErrorMonitors.Add(m => m.MessageSentToErrorQueue += OnMessageSentToErrorQueue);
            channelBuilder.EnableAutosubscribe = true;
            channelBuilder.EnableSagaSupport = true;
            channelBuilder.CriticalErrorAction = CriticalErrorAction;
            channelBuilder.EnableAutosubscribe = true;

            var boostrapper = new BusBootstrapper(container, ContainerFactory, channelBuilder);
            boostrapper.Run();
        }

        private static Task CriticalErrorAction(ICriticalErrorContext arg)
        {
            if (Debugger.IsAttached) Debugger.Break();

            MvcApplication.Log(arg.Exception, arg.Error, Severity.Fatal);

            return Task.CompletedTask;
        }

        private static void OnMessageSentToErrorQueue(Object sender, NServiceBus.Faults.FailedMessage e)
        {
            if (Debugger.IsAttached) Debugger.Break();

            var exception = e.Exception;
            var targetQueue = e.ProcessingEndpoint();
            var messageType = e.EnclosedMessageType();
            var correlationId = e.DefaultCorrelation();

            using (new Correlation(correlationId))
            {
                MvcApplication.Log(exception, $"Bus exception on queue {targetQueue} processing {messageType?.Name}");
            }
        }

        #endregion
    }
}