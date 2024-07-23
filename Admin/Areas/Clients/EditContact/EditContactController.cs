using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Clients.EditContact.Models;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Clients.EditContact
{
    /// <summary>
    /// Controller for managing a <see cref="Client"/> contacts.
    /// </summary>
    [Authorize()]
    public class EditContactController : ActivityLoggingController2
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditContactController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public EditContactController(ISessionContext context)
        {
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// View the list of client contacts.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(Guid userId, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var client = await this.context
                    .SetOf<Client>()
                    .Where(c => c.Logon.Id == userId)
                    .Include(c => c.Contacts)
                    .Include(c => c.Logon)
                    .FirstOrDefaultAsync(cancellation);
                if (client == null) return this.NavigationFor<UserSummaryController>().ToIndex();

                var model = new ClientContacts(client);
                return this.View(model);
            }
        }
        
        /// <summary>
        /// Update the set of client contacts after normalizing values by email.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(ClientContacts model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            var userId = model.UserId;

            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                var client = await this.context
                    .SetOf<Client>()
                    .Where(c => c.Logon.Id == userId)
                    .Include(c => c.Contacts)
                    .FirstOrDefaultAsync(cancellation);
                if (client == null)
                {
                    uow.Rollback();
                    return this.NavigationFor<UserSummaryController>().ToIndex();
                }

                var inputs = ContactModel.Consolodate(model.Contacts);

                var removedData = client.Contacts.Where(c => inputs.All(cc => !cc.EmailAddress.Equals(c.EmailAddress, StringComparison.OrdinalIgnoreCase))).ToArray();
                client.Contacts.RemoveRange(removedData);

                foreach (var item in inputs)
                {
                    var thisContact = client.Contacts.FirstOrDefault(c => c.EmailAddress == item.EmailAddress);
                    if (thisContact == null)
                    {
                        thisContact= new Contact(client, item.EmailAddress);
                        client.Contacts.Add(thisContact);
                    }

                    thisContact.Billing = item.BillTo;
                    thisContact.NotifyJobs = item.ShouldNotify;
                    thisContact.SubmitJobs = item.SubmitJobs;
                    thisContact.Admin = item.IsAdmin;
                    thisContact.Name = item.Name;
                }

                await uow.CommitAsync(cancellation);
                model = new ClientContacts(client);
            }

            this.OnEvent($"Edited client {model.Name} contacts");

            this.ViewData["Message"] = "Contacts updated";
            return this.View(model);
        }

        /// <summary>
        /// Action to render a contact row in the UI.
        /// </summary>
        public virtual ActionResult ContactRow()
        {
            return this.View();
        }

        #endregion
    }
}
