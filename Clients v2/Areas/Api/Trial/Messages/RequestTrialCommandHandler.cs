using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Api.Trial.Messages
{
    /// <summary>
    /// Handler designed to process the <see cref="RequestTrialCommand"/> message.
    /// </summary>
    /// <remarks>
    /// Responds to the request by
    /// 1. create a new lead
    /// 2. match against de-duplication services
    /// 3. send an email if the trial identity is new
    ///
    /// <note type="implementnotes">
    /// This handler creates an instance of the <see cref="StandardLeadConsolidationService"/>
    /// for lead de-duplication services. This is due to the difficulties with configuring a
    /// shared <see cref="ISessionContext"/> in a bus container.
    /// </note>
    /// </remarks>
    public class RequestTrialCommandHandler : IHandleMessages<RequestTrialCommand>
    {
        #region Fields

        private readonly ISessionContext dataContext;
        private readonly ILeadConsolidationService leadDeduplication;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTrialCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="ISessionContext"/> providing data access.</param>
        public RequestTrialCommandHandler(ISessionContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.leadDeduplication = new StandardLeadConsolidationService(this.dataContext);
        }

        #endregion

        #region IHandleMessages<RequestTrialCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(RequestTrialCommand message, IMessageHandlerContext context)
        {
            using (var uow = this.dataContext.CreateScope(ScopeOptions.AutoCommit))
            {
                var application = await this.dataContext
                    .SetOf<Application>()
                    .ForAccurateAppend()
                    .FirstAsync()
                    .ConfigureAwait(false);

                var lead = new Lead(application, message.FirstName, message.LastName)
                {
                    DefaultEmail = message.Email,
                    Qualified = LeadQualification.UnknownAtThisTime,
                    BusinessName = message.Company,
                    Status = LeadStatus.TrialKeyRequested,
                    FollowUpDate = DateTime.UtcNow.AddDays(3),
                    ContactMethod = LeadContactMethod.Form,
                    Source = LeadSource.Unknown,
                    IP = message.Ip,
                    Trial = new TrialIdentity()
                };
                lead.Trial.IsEnabled = true; // always enabled
                lead.PrimaryPhone.Value = message.Phone;
                
                var systemUser = await this.dataContext
                    .SetOf<User>()
                    .SystemUser()
                    .FirstAsync();

                lead.ChangeOwner(systemUser.Id);

                lead = await this.leadDeduplication.DeduplicateLead(lead);

                // a de-duplicated lead may not have a trial so recreate if needed
                if (lead.Trial == null)
                {
                    lead.Trial = new TrialIdentity();
                    lead.Trial.IsEnabled = lead.Trial.IsEnabled = true; // always enabled
                }

                // send welcome email
                if (lead.Trial.IsEnabled)
                {
                    lead.Notes.Add(new Note($"API Trial Key Requested. Key={lead.Trial.AccessId}", systemUser));

                    var apiTrialWelcomMessage = await EmailFactory.ApiTrialWelcomMessage(lead.DefaultEmail, lead);
                    this.dataContext.SetOf<Message>().Add(apiTrialWelcomMessage);
                }

                if (lead.Id == null) this.dataContext.SetOf<Lead>().Add(lead);

                await uow.CommitAsync();
            }
        }

        #endregion
    }
}