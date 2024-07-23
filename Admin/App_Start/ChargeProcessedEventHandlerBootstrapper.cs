using System;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Messaging;
using AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge.Messaging;
using Castle.Windsor;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Bootstrapper configuring the <see cref="ChargeProcessedEventHandler"/> NServiceBus infrastructure to process the <see cref="ChargeProcessedEvent"/>.
    /// </summary>
    public class ChargeProcessedEventHandlerBootstrapper : BusHandlerConfiguration<ChargeProcessedEventHandler, ChargeProcessedEvent>
    {
        #region Fields

        private readonly Lazy<IWindsorContainer> busFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeProcessedEventHandlerBootstrapper"/> class.
        /// </summary>
        /// <param name="busFactory">The one time initialization component of the bus handler level container.</param>
        public ChargeProcessedEventHandlerBootstrapper(Lazy<IWindsorContainer> busFactory)
        {
            if (busFactory == null) throw new ArgumentNullException(nameof(busFactory));

            this.busFactory = busFactory;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override IWindsorContainer CreateChildContainer()
        {
            return this.busFactory.Value;
        }

        #endregion
    }
}