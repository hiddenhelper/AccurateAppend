using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.Security;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Admin
{
    /// <summary>
    /// Handler for the <see cref="InteractiveLogonEvent"/> bus event.
    /// </summary>
    /// <remarks>
    /// Responds to a message by pushing a SignalR message to all connected browser clients about the logon.
    /// </remarks>
    public class PushNotificationForInteractiveLogonEventHandler : IHandleMessages<InteractiveLogonEvent>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNotificationForInteractiveLogonEventHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public PushNotificationForInteractiveLogonEventHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<InteractiveLogonEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(InteractiveLogonEvent message, IMessageHandlerContext context)
        {
            var userId = message.UserId;

            var user = await this.dataContext.SetOf<User>()
                .Where(u => u.Id == userId)
                .Select(u => new {UserId = u.Id, u.UserName})
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (user == null) return;

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
            hubContext.Clients.All.addNewMessageToPage($"{user.UserName} has logged in");
        }

        #endregion
    }
}