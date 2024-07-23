using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ContactsApi
{
    /// <summary>
    /// Controller for supplying a REST Api for <see cref="Contact"/> data.
    /// </summary>
    [Authorize()]
    public class ContactsApiController : Controller
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsApiController"/> class.
        /// </summary>
        public ContactsApiController(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Returns list of client contacts emails.
        /// </summary>
        public virtual async Task<ActionResult> Parties(Guid userId, CancellationToken cancellation)
        {
            var client = await this.context
                .SetOf<Client>()
                .Where(c => c.Logon.Id == userId)
                .Include(c => c.Contacts)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (client == null) return this.NavigationFor<UserSummaryController>().ToIndex();

            var data = new
            {
                UserId = userId,
                Name = client.CompositeName,
                EmailAddress = client.DefaultEmail,
                Contacts = client.Contacts.Select(c => new
                {
                    Name = c.Name ?? String.Empty,
                    c.EmailAddress,
                    ShouldNotify = c.NotifyJobs,
                    c.SubmitJobs,
                    IsAdmin = c.Admin,
                    BillTo = c.Billing
                }).ToArray()
            };

            var result = new JsonNetResult {Data = data};

            return result;
        }

        #endregion
    }
}