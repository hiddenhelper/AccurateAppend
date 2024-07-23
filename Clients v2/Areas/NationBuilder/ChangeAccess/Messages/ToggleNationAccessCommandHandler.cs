using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using Integration.NationBuilder.Data;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.ChangeAccess.Messages
{
    /// <summary>
    /// Handler responsible for processing the <see cref="ToggleNationAccessCommand"/> command.
    /// </summary>
    public class ToggleNationAccessCommandHandler : IHandleMessages<ToggleNationAccessCommand>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleNationAccessCommandHandler" /> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="ISessionContext" /> component providing entity access.</param>
        public ToggleNationAccessCommandHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<ToggleNationAccessCommand> Members

        /// <inheritdoc />
        public async Task Handle(ToggleNationAccessCommand message, IMessageHandlerContext context)
        {
            var id = message.NationId;
            var connectionId = message.RequestId;

            using (context.Alias())
            {
                using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
                {
                    var registration = await this.dataContext.SetOf<Registration>()
                        .RegistrationsForInteractiveUser()
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (registration != null)
                    {
                        if (message.Enable)
                        {
                            registration.Reactivate();
                        }
                        else
                        {
                            registration.Deactivate();
                        }
                    }

                    await uow.CommitAsync();
                }
            }

            var callback = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
            callback.Clients.Client(connectionId.ToString()).callbackComplete();
        }

        #endregion
    }
}