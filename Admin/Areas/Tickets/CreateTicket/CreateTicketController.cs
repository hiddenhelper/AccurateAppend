using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Websites.Admin.Areas.Tickets.CreateTicket.Models;
using AccurateAppend.Websites.Admin.TempDataExtensions;
using AccurateAppend.ZenDesk.Contracts.Support;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.CreateTicket
{
    /// <summary>
    /// Controller for creating Zendesk Tickets
    /// </summary>
    [Authorize]
    public class CreateTicketController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTicketController"/> class.
        /// </summary>
        public CreateTicketController(IMessageSession bus, ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.bus = bus;
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns list of contacts in view.
        /// </summary>
        public async Task<ActionResult> Index(Guid userId, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var client = await this.context
                    .SetOf<Client>()
                    .Where(c => c.Logon.Id == userId)
                    .Include(c => c.Contacts)
                    .FirstOrDefaultAsync(cancellation);
                if (client == null) return this.NavigationFor<UserSummaryController>().ToIndex();

                var contacts = client
                    .Contacts
                    .OrderBy(c => c.EmailAddress)
                    .Where(c => !c.EmailAddress.Equals(c.Client.DefaultEmail, StringComparison.OrdinalIgnoreCase))
                    .Select(c => new CheckBoxes
                    {
                        Text = $"{c.EmailAddress} ({BuildContactProperties(new ContactModel(c))})",
                        Value = $"{c.EmailAddress}"
                    })
                    .ToList();

                // add primary account holder in first position
                contacts.Insert(0, new CheckBoxes { Text = $"{client.DefaultEmail} (Is Primary)", Value = $"{client.DefaultEmail}" });

                var model = new CreateTicketViewModel
                {
                    UserId = userId
                };
                model.Recipients.AddRange(contacts);

                return this.View(model);
            }
        }
        
        /// <summary>
        /// Creates a Zendesk ticket.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Index(CreateTicketViewModel model,  CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            try
            {
                var createTicketCommand = new CreateTicketCommand
                {
                    RequestedBy = "support@accurateappend.com",
                    Subject = model.Subject,
                    Type = (TicketType)model.Type,
                    Priority = (TicketPriority)model.Priority,
                    Description = model.Comments
                };
                var recipients = model.Recipients.Where(a => a.Checked).Select(a => a.Value);
                var otherRecipients = model.OtherRecipients == null ? new List<String>() : model.OtherRecipients.Split(',').ToList();
                createTicketCommand.EmailAddress.AddRange(recipients.Concat(otherRecipients).Distinct(StringComparer.OrdinalIgnoreCase));

                await this.bus.SendLocal(createTicketCommand);

                this.TempData.Put("message", new AlertMessage { Type = AlertType.success  , Body = "Ticket has been created In Zendesk."});
                return RedirectToAction("Index", new { userId = model.UserId });

            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Create ticket failing");
                return this.DisplayErrorResult(ex.Message);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds description of the contact's properties
        /// </summary>
        private static String BuildContactProperties(ContactModel contact)
        {
            var roles = new List<String>();
            if (contact.IsPrimary) roles.Add("Primary");
            if (contact.IsAdmin) roles.Add("Admin");
            if (contact.BillTo) roles.Add("Bill To");
            if (contact.ShouldNotify) roles.Add("Should Notify");
            if (contact.SubmitJobs) roles.Add("Submit Jobs");

            return String.Join(",", roles);
        }

        #endregion
    }
}