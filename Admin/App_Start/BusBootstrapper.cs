using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts.Configuration;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Configuration;
using AccurateAppend.Websites.Admin.Views;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using EventLogger;
using Microsoft.Win32;
using NServiceBus;
using IBus = AccurateAppend.Data.IBus;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Configures the system level Bus.
    /// </summary>
    /// <remarks>
    /// 1. Creates a new child container from the supplied root (if handlers are found).
    /// 2. Uses the <see cref="ConcreteBusChannelBootstrapper"/> to create the <see cref="IEndpointInstance"/> for the application.
    /// 3. Sets up any new <see cref="IBus"/> endpoints in the PARENT container.
    /// 4. Sets up any <see cref="IBusHandler"/> endpoints in the CHILD container.
    /// 5. Registers all endpoints types as components with Transient lifestyle.
    /// </remarks>
    public class BusBootstrapper : ConcreteBusBootstrapper
    {
        #region Constants

        /// <summary>
        /// Holds the application wide NService Bus endpoint name
        /// </summary>
        public const String EndpointName = "AdminWebsite";

        #endregion

        #region Fields

        private static readonly Lazy<IWindsorContainer> ContainerFactory = new Lazy<IWindsorContainer>(() =>
        {
            IResource resource = new FileResource(@"Configuration\Bus.config");
            var busContainer = new WindsorContainer(new XmlInterpreter(resource));

            // A bit of a hack here since the order of initialization is a bit unknown we'll force it on the safest container
            // Setup data caches

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            if (!SiteCache.IsInitialized)
            {
                SiteCache.FactoryMethod(() => LoadSites(busContainer));
                
                SiteCache.Cache.ToString(); // Force creation
            }

            if (!ProductCache.IsInitialized)
            {
                ProductCache.FactoryMethod(()=> LoadProducts(busContainer));

                ProductCache.Cache.ToString(); // Force creation
            }

            if (!AdminUserCache.IsInitialized)
            {
                AdminUserCache.FactoryMethod(() => LoadUsers(busContainer));

                ProductCache.Cache.ToString(); // Force creation
            }
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed

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
            if (containerFactory == null) throw new ArgumentNullException(nameof(containerFactory));
            if (channelFactory == null) throw new ArgumentNullException(nameof(channelFactory));
            Contract.EndContractBlock();

            #region Accounts

            // Reset histogram
            this.InitiatorConfigurations.Add(new ResetClientHistorgramCommandBootstrapper());
            this.HandlerConfigurations.Add(new ResetClientHistorgramCommandHandlerBootstrapper(containerFactory));

            // Configure FTP Account
            this.InitiatorConfigurations.Add(new ConfigureFtpAccountCommandBootstrapper());
            this.HandlerConfigurations.Add(new ConfigureFtpAccountCommandHandlerBootstrapper(containerFactory));

            #endregion

            #region Billing
            
            // Charge Card
            this.HandlerConfigurations.Add(new ProcessChargeCommandHandlerBootstrapper(containerFactory));

            // Post billing service charges to deals
            this.HandlerConfigurations.Add(new ChargeProcessedEventHandlerBootstrapper(containerFactory));

#if DEBUG
            // Enable subscriptions for Charge Processing Events
            this.InitiatorConfigurations.Add(new ChargeProcessedEventBootstrapper());
            this.InitiatorConfigurations.Add(new RefundProcessedEventBootstrapper());
            this.InitiatorConfigurations.Add(new PaymentProfileDeletedEventBootstrapper());
#endif

            #endregion

            #region Job Processing

#if DEBUG
            // Enable subscriptions for Job Processing Events
            this.InitiatorConfigurations.Add(new JobCompletedEventBootstrapper());
            this.InitiatorConfigurations.Add(new JobRequiresAdministrativeActionEventBootstrapper());
            this.InitiatorConfigurations.Add(new JobCreatedEventBootstrapper());
#endif

            // Alert Users About Job Issues
            this.HandlerConfigurations.Add(new UserAlertForJobRequiresAdministrativeActionEventHandlerBootstrapper(containerFactory));

            // Alert users for large jobs and client jobs
            this.HandlerConfigurations.Add(new MonitorJobSubmittedEventHandlerBootstrapper(containerFactory));

            // Dispatch build order commands for JobCompleted events when needed
            this.HandlerConfigurations.Add(new CreateDealsForJobCompletedEventHandlerBootstrapper(containerFactory));
            
            // Send all Job commands to the right place
            this.InitiatorConfigurations.Add(new CreateAdminJobCommandBoostrapper());
            this.InitiatorConfigurations.Add(new ResetJobCommandBoostrapper());
            this.InitiatorConfigurations.Add(new DeleteJobCommandBoostrapper());
            this.InitiatorConfigurations.Add(new ReassignJobCommandBoostrapper());

            #endregion

            #region Sales

            this.HandlerConfigurations.Add(new CreateUsageBillCommandHandlerBootstrapper(containerFactory));

#if DEBUG
            // Enable SubscriptionBilling saga to react to cancellations
            this.InitiatorConfigurations.Add(new DealCanceledEventBootstrapper());

            // Used for sending emails for deals
            this.InitiatorConfigurations.Add(new DealCompletedEventBootstrapper());
            this.InitiatorConfigurations.Add(new DealRefundedEventBootstrapper());

            // Used for syncing with JobProcessing
            this.InitiatorConfigurations.Add(new DealPublicKeyChangedEventBootstrapper());
            this.InitiatorConfigurations.Add(new DealCreatedEventBootstrapper());

            // Subscription account events
            this.InitiatorConfigurations.Add(new SubscriptionCreatedEventBootstrapper());
            this.InitiatorConfigurations.Add(new SubscriptionCanceledEventBootstrapper());

            // Allow auto-billing on Approved event
            this.InitiatorConfigurations.Add(new DealApprovedEventBootstrapper());
#endif

            // Bill Updates
            this.InitiatorConfigurations.Add(new UpdateDealFromProcessingReportCommandBootstrapper());

            #endregion

            #region Operations

            // Send emails
            this.InitiatorConfigurations.Add(new SendEmailCommandBootstrapper());
            this.InitiatorConfigurations.Add(new ResendEmailCommandBootstrapper());

            // Store files
            this.InitiatorConfigurations.Add(new StoreFileCommandBootstrapper());
            this.InitiatorConfigurations.Add(new UpdateFileCorrelationIdCommandBootstrapper());
            this.HandlerConfigurations.Add(new StoreFileCommandHandlerBootstrapper(containerFactory));

            #endregion

            #region Internal

            // Log Action
            this.InitiatorConfigurations.Add(new LogUserActionCommandBootstrapper());
            this.HandlerConfigurations.Add(new LogUserActionCommandHandlerBootstrapper(containerFactory));

#if DEBUG
            // Alert Users About Logon Events
            this.InitiatorConfigurations.Add(new InteractiveLogonEventBootstrapper());
#endif

            this.HandlerConfigurations.Add(new PushNotificationForInteractiveLogonEventHandlerBootstrapper(containerFactory));

            #endregion
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convenient abstraction over this component to run everything in one call.
        /// </summary>
        /// <param name="container">The <see cref="IWindsorContainer"/> that will be further configured.</param>
        public static void Create(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Contract.EndContractBlock();

#if DEBUG
            const String ChannelName = EndpointName;

            var key = Registry.CurrentUser.CreateSubKey(@"Software\ParticularSoftware");
            Debug.Assert(key != null);
            key.SetValue("TrialStart", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"));
            key.Close();
#else
            const String ChannelName = EndpointName;
#endif
            var channelBuilder = new ConcreteBusChannelBootstrapper(ChannelName, () => Config.AccurateAppendDb);
            channelBuilder.EnableAuditQueue = Debugger.IsAttached;
            channelBuilder.OutgoingMutators.Add(() => new PrincipleMutator());
            channelBuilder.EnableSagaSupport = true;
            channelBuilder.EnableDialogs = false;
            channelBuilder.ErrorMonitors.Add(m => m.MessageSentToErrorQueue += OnMessageSentToErrorQueue);
            channelBuilder.CriticalErrorAction += CriticalErrorAction;
            channelBuilder.EnableAutosubscribe = true;

            var boostrapper = new BusBootstrapper(container, ContainerFactory, channelBuilder);
            boostrapper.Run();
        }

        #endregion

        #region Bus Logging

        private static Task CriticalErrorAction(ICriticalErrorContext arg)
        {
            if (Debugger.IsAttached) Debugger.Break();

            var exception = arg.Exception;
            var errorMessage = arg.Error;

            Logger.LogEvent(exception, Severity.Fatal, $"Fatal Bus exception: {errorMessage}");

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
                Logger.LogEvent(exception, Severity.Medium, $"Bus exception on queue {targetQueue} processing {messageType?.Name}");
            }
        }

        #endregion

        #region Caches

        private static IEnumerable<SiteCache.SiteInfo> LoadSites(IWindsorContainer busContainer)
        {
            var context = busContainer.Resolve<ISessionContext>();
            try
            {
                var results = context.SetOf<SiteDetail>().Include(site => site.Application).ToArray();
                return results.Select(site =>
                {
                    var i = new SiteCache.SiteInfo(site);
                    return i;
                }).ToArray();
            }
            finally
            {
                busContainer.Release(context);
            }
        }

        private static IList<ProductCache.ProductInfo> LoadProducts(IWindsorContainer busContainer)
        {
            var context = busContainer.Resolve<Sales.DataAccess.DefaultContext>();
            try
            {
                var results = context.SetOf<Sales.Product>().Where(p => p.Usage != Sales.ProductUsage.Legacy).Include(p => p.Category).ToArray();
                return results.Select(p =>
                {
                    var i = new ProductCache.ProductInfo(p);
                    return i;
                }).ToArray();
            }
            finally
            {
                busContainer.Release(context);
            }
        }

        private static IList<AdminUserCache.UserInfo> LoadUsers(IWindsorContainer busContainer)
        {
            var context = busContainer.Resolve<ISessionContext>();
            try
            {
                var results = context.SetOf<User>()
                    .Where(u => u.Application.Id == ApplicationExtensions.AdminId)
                    .Where(u => !u.UserName.StartsWith("_")).ToArray();
                return results.Select(u =>
                {
                    var i = new AdminUserCache.UserInfo(u);
                    return i;
                }).OrderBy(u => u, AdminUserCache.Ordering).ToArray();
            }
            finally
            {
                busContainer.Release(context);
            }
        }

        #endregion
    }
}