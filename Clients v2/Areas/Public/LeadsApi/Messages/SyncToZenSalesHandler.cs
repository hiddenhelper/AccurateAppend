using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using EventLogger;
using NServiceBus;
using LeadContactMethod = AccurateAppend.CustomerManagement.Contracts.LeadContactMethod;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Messages
{
    /// <summary>
    /// Centralizes and manages all lead to ZenSales sync behavior. Responds to several event publications and
    /// executes logic to determine if this means the lead data should be synced to ZenSales.
    ///
    /// -<see cref="LeadCreatedEvent"/>: If <see cref="LeadContactMethod"/> = Form and a name and email are present,
    /// sync the lead to ZenDesk
    /// -<see cref="LeadAssignedEvent"/>: If the assignment is to Andy, sync to ZenSales
    /// All other conditions are ignored. The actual synchronization logic is handled by dispatching an internally
    /// and reflectively routed <see cref="SyncToZenSalesCommand"/> message.
    /// </summary>
    public class SyncToZenSalesHandler : IHandleMessages<LeadCreatedEvent>, IHandleMessages<LeadAssignedEvent>, IHandleMessages<SyncToZenSalesCommand>
    {
        #region Fields

        private readonly Accounting.DataAccess.DefaultContext dataContext;
        private readonly ZenDesk.Sales.Leads.ILeadsService api;

        #endregion

        #region Constructor

        public SyncToZenSalesHandler(Accounting.DataAccess.DefaultContext dataContext, ZenDesk.Sales.Leads.ILeadsService api)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (api == null) throw new ArgumentNullException(nameof(api));

            this.dataContext = dataContext;
            this.api = api;
        }

        #endregion

        #region IHandleMessages<LeadCreatedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(LeadCreatedEvent message, IMessageHandlerContext context)
        {
            using (new Correlation(message.PublicKey))
            {
                if (message.ContactMethod != LeadContactMethod.Form) return;

                // Validate
                if (String.IsNullOrEmpty($"{message.FirstName} {message.LastName}")) return;
                if (String.IsNullOrEmpty(message.Email)) return;

                var leadId = await this.AvailableLeads()
                    .Where(l => l.PublicKey == message.PublicKey)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (leadId == null) return;

                var command = new SyncToZenSalesCommand();
                command.LeadId = leadId.Value;

                await context.SendLocal(command);
            }
        }

        #endregion

        #region IHandleMessages<LeadAssignedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(LeadAssignedEvent message, IMessageHandlerContext context)
        {
            using (new Correlation(message.PublicKey))
            {
                if (message.AssignedTo.UserId != new Guid("03831F96-0A0F-4ED7-AF52-0D48B3F8B9E2")) return; // Andy

                var leadId = await this.AvailableLeads()
                    .Where(l => l.PublicKey == message.PublicKey)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (leadId == null) return;

                var command = new SyncToZenSalesCommand();
                command.LeadId = leadId.Value;

                await context.SendLocal(command);
            }
        }

        #endregion

        #region IHandleMessages<SyncToZenSalesCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(SyncToZenSalesCommand message, IMessageHandlerContext context)
        {
            var lead = await this.AvailableLeads()
                .Where(l => l.Id == message.LeadId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (lead == null) return;

            using (new Correlation(lead.PublicKey))
            {
                var payload = new ZenDesk.Sales.Leads.Lead()
                {
                    Email = lead.DefaultEmail,
                    FirstName = lead.FirstName,
                    LastName = lead.LastName,
                    Phone = lead.PrimaryPhone?.ToString(),
                    OrganizationName = String.IsNullOrEmpty(lead.BusinessName) ? "none" : lead.BusinessName,
                    Description = lead.Comments ?? String.Empty,
                    OwnerId = this.MapToZenUser(lead.OwnerId)
                };

                var createdLead = await this.api.CreateAsync(payload);

                var externalId = createdLead.Id;
                var ra = await this.dataContext
                    .Database
                    .ExecuteSqlCommandAsync("UPDATE [accounts].[Leads] SET [ExternalId] = @p1 WHERE [PublicKey]=@p0", lead.PublicKey, externalId);
                Logger.LogEvent($"ZenSales {externalId} for lead {lead.PublicKey} updated {ra} records", Severity.None, Application.Clients);
            }
        }

        #endregion

        #region Methods

        protected virtual IQueryable<Accounting.Lead> AvailableLeads()
        {
            return this.dataContext
                .SetOf<Accounting.Lead>()
                .Where(l => l.IsDeleted == null || l.IsDeleted == false)
                .Where(l => l.Status != Accounting.LeadStatus.ConvertedToCustomer);
        }

        protected virtual Int64? MapToZenUser(Guid assignedTo)
        {
            return ZenDesk.Sales.WellKnownUsers.TranslateToZenDeskUser(assignedTo);
        }

        #endregion
    }
}