using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Security;
using NServiceBus;
using LeadSource = AccurateAppend.Accounting.LeadSource;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Messages
{
    /// <summary>
    /// Processes the <see cref="CreateFormstackLeadCommand"/> amd <see cref="CreateChatLeadCommand"/> commands.
    /// </summary>
    /// <remarks>
    /// Normalized the inbound data, verifies email address, deduplicate lead, and then stores the new or updated lead.
    /// </remarks>
    public class CreateLeadCommandHandler : IHandleMessages<CreateFormstackLeadCommand>, IHandleMessages<CreateChatLeadCommand>
    {
        #region Fields

        private readonly Accounting.DataAccess.DefaultContext dataContext;
        private readonly ILeadConsolidationService consolidator;

        private static readonly Guid Steve = WellKnownIdentifiers.Steve;
        private static readonly Guid Chris = WellKnownIdentifiers.Chris;
        private static readonly Guid Andy = WellKnownIdentifiers.Andy;
        private static readonly Guid Max = WellKnownIdentifiers.Max;

        private static readonly Guid System = WellKnownIdentifiers.SystemUserId;

        #endregion

        #region Constructor

        public CreateLeadCommandHandler(Accounting.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.consolidator = new StandardLeadConsolidationService(this.dataContext);
        }

        #endregion

        #region IHandleMessages<CreateFormstackLeadCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateFormstackLeadCommand message, IMessageHandlerContext context)
        {
            var phone = message.Phone;
            var comments = (message.Comments ?? String.Empty).Left(1000);
            var ip = message.Ip;
            var companyname = message.Company;
            var firstname = message.FirstName;
            var lastname = message.LastName;
            var requestType = message.RequestType;

            // bail - ZenDesk integration handles this
            if (requestType == RequestType.Service) return;

            var lead = await this.CreateBaseLead();
            lead.BusinessName = companyname;
            lead.FirstName = firstname;
            lead.LastName = lastname;
            lead.PrimaryPhone.Value = PhoneNumber.CleanFormat(phone);
            lead.DefaultEmail = message.Email;
            lead.ContactMethod = Accounting.LeadContactMethod.Form;
            lead.Comments = comments?.Left(2000);
            lead.IP = ip;
            lead.Source = LeadSource.Unknown;
            lead.Qualified = LeadQualification.UnknownAtThisTime;
            lead.Status = LeadStatus.RequestForInfo;
            lead.ProductInterest = message.ProductInterest;

            this.RouteToBestSalesMember(lead, message);

            await this.SaveLead(lead, context);
        }

        #endregion

        #region IHandleMessages<CreateChatLeadCommand> Members

        /// <inheritdoc />
        public async Task Handle(CreateChatLeadCommand message, IMessageHandlerContext context)
        {
            var phone = message.Phone;
            var comments = (message.Comments ?? String.Empty).Left(1000);
            var firstname = message.FirstName;
            var lastname = message.LastName;

            var lead = await this.CreateBaseLead();
            lead.ChangeOwner(Andy);
            lead.FirstName = firstname;
            lead.LastName = lastname;
            lead.PrimaryPhone.Value = PhoneNumber.CleanFormat(phone);
            lead.DefaultEmail = message.Email;
            lead.ContactMethod = Accounting.LeadContactMethod.Chat;
            lead.Comments = comments?.Left(2000) ?? String.Empty;
            lead.Source = LeadSource.Unknown;
            lead.Qualified = LeadQualification.UnknownAtThisTime;
            lead.Status = LeadStatus.RequestForInfo;
            lead.ProductInterest = "Unknown";

            await this.SaveLead(lead, context);
        }

        #endregion

        #region Helpers
        
        /// <summary>
        /// Common logic that all <see cref="Lead"/> entities have for creation.
        /// </summary>
        /// <returns>A basic <see cref="Lead"/> entity.</returns>
        protected virtual async Task<Lead> CreateBaseLead()
        {
            var application = await this.dataContext
                .SetOf<Application>()
                .ForAccurateAppend()
                .FirstAsync()
                .ConfigureAwait(false);

            var lead = new Lead(application);
            lead.ChangeOwner(WellKnownIdentifiers.SystemUserId); // start with system ownership

            return lead;
        }

        private void RouteToBestSalesMember(Lead lead, CreateFormstackLeadCommand message)
        {
            if (lead == null) return;

            if (message.EstimatedCount == RecordCount.LessThan4K)
            {
                lead.ChangeOwner(Max);
                return;
            }

            var productInterest = lead.ProductInterest ?? String.Empty;
            if (message.EstimatedCount == RecordCount.LessThan10K || productInterest.Contains("API Access"))
            {
                lead.ChangeOwner(Andy);
                return;
            }
            
            if (productInterest.Contains("Phone Append", StringComparison.OrdinalIgnoreCase) ||
                productInterest.Contains("Email Append", StringComparison.OrdinalIgnoreCase) ||
                productInterest.Contains("Email Verification", StringComparison.OrdinalIgnoreCase))
            {
                lead.ChangeOwner(Andy);
                return;
            }

            if (productInterest.Contains("Other", StringComparison.OrdinalIgnoreCase))
            {
                lead.ChangeOwner(Andy);
            }
        }
        
        /// <summary>
        /// Commits the supplied <paramref name="lead"/> to the datastore.
        /// </summary>
        /// <param name="lead">The <see cref="Lead"/> to deduplicate, if needed, and to store.</param>
        /// <param name="context">The NServiceBus handling context. (Used for publishing events)</param>
        protected virtual async Task SaveLead(Lead lead, IMessageProcessingContext context)
        {
            // dedup against existing leads
            lead = await this.consolidator.DeduplicateLead(lead).ConfigureAwait(false);
            var newLead = lead.Id == null;

            if (newLead) this.dataContext.SetOf<Lead>().Add(lead);

            await this.dataContext.SaveChangesAsync();

            if (newLead)
            {
                var @event = new LeadCreatedEvent();
                @event.ApplicationId = lead.Application.Id;
                @event.PublicKey = lead.PublicKey;
                @event.Email = lead.DefaultEmail;
                @event.Phone = lead.PrimaryPhone.Value;
                @event.FirstName = lead.FirstName;
                @event.LastName = lead.LastName;
                @event.SourceChannel = (CustomerManagement.Contracts.LeadSource) (Int32) lead.Source;
                @event.ContactMethod = (CustomerManagement.Contracts.LeadContactMethod) (Int32) lead.ContactMethod;
                @event.AssignedTo.UserId = lead.OwnerId;

                if (lead.OwnerId == Steve)
                {
                    @event.AssignedTo.UserName = "steve@accurateappend.com";
                }
                else if (lead.OwnerId == Chris)
                {
                    @event.AssignedTo.UserName = "chris@accurateappend.com";
                }
                else if (lead.OwnerId == Andy)
                {
                    @event.AssignedTo.UserName = "andy@accurateappend.com";
                }
                else if (lead.OwnerId == Max)
                {
                    @event.AssignedTo.UserName = "max@accurateappend.com";
                }
                else
                {
                    @event.AssignedTo.UserId = WellKnownIdentifiers.SystemIdentity.GetIdentifier();
                    @event.AssignedTo.UserName = WellKnownIdentifiers.SystemIdentity.Name;
                }

                await context.Publish(@event);
            }
        }

        #endregion
    }
}