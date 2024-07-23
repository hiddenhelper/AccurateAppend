using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Messages.Admin;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using NServiceBus;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Custom windsor facility that monitors for created <see cref="ActivityLoggingController2"/> instances
    /// in a specific container. When the controller is created the <see cref="ActivityLoggingController2.UserActivity"/>
    /// event will be registered. Events will be dispatched as <see cref="LogUserActionCommand"/> messages to
    /// an <see cref="IMessageSession"/> instance provided as a constructor parameter. Bus interaction is NOT awaited.
    /// </summary>
    internal sealed class EventManagerBridgeFacility : IFacility
    {
        #region Fields

        private IKernel parentKernel;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        internal EventManagerBridgeFacility(IMessageSession bus)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.bus = bus;
        }

        #endregion

        #region IFacility Members

        /// <inheritdoc />
        void IFacility.Init(IKernel kernel, IConfiguration facilityConfig)
        {
            this.parentKernel = kernel;
            this.parentKernel.ComponentCreated += this.OnComponentCreated;
            this.parentKernel.ComponentDestroyed += this.OnComponentDestroyed;
        }

        /// <inheritdoc />
        void IFacility.Terminate()
        {
            if (this.parentKernel == null) return;

            this.parentKernel.ComponentCreated -= this.OnComponentCreated;
            this.parentKernel = null;
        }

        #endregion

        #region Event Sinks

        private void OnComponentDestroyed(ComponentModel model, Object instance)
        {
            var controller = instance as ActivityLoggingController2;
            if (controller == null) return;

            controller.UserActivity -= this.OnUserActivity;
        }

        private void OnComponentCreated(ComponentModel model, Object instance)
        {
            var controller = instance as ActivityLoggingController2;
            if (controller == null) return;

            controller.UserActivity += this.OnUserActivity;
        }

        private void OnUserActivity(Object sender, UserActvityEventArgs e)
        {
            var message = new LogUserActionCommand
            {
                Description = e.ActivityDescription,
                EventDate = DateTime.UtcNow,
                UserId = e.UserId,
                Ip = e.Ip.ToString()
            };

#pragma warning disable NSB0001 // Await or assign Task
            this.bus.Send(message);
#pragma warning restore NSB0001 // Await or assign Task
        }

        #endregion
    }
}