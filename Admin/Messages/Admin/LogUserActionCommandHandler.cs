using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Security;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Admin
{
    /// <summary>
    /// Handler for the <see cref="LogUserActionCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by creating a new <see cref="ActivityEntry"/>
    /// entity based on the information and commiting it with an externally
    /// supplied <see cref="ISessionContext"/> data component.
    /// </remarks>
    public class LogUserActionCommandHandler : IHandleMessages<LogUserActionCommand>
    {
        #region Fields

        private readonly ISessionContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUserActionCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="ISessionContext"/> component.</param>
        public LogUserActionCommandHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<UserActionEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(LogUserActionCommand message, IMessageHandlerContext context)
        {
            using (new Correlation(context.DefaultCorrelation()))
            {

                var description = message.Description;
                var userId = message.UserId;
                var eventDate = message.EventDate;
                var ip = message.Ip;

                using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
                {
                    var performedBy = await this.dataContext.SetOf<User>().FirstAsync(u => u.Id == userId);

                    var log = new ActivityEntry(description, performedBy, eventDate)
                    {
                        Ip = ip
                    };

                    this.dataContext.SetOf<ActivityEntry>().Add(log);

                    await uow.CommitAsync();
                }

            }
        }

        #endregion
    }
}