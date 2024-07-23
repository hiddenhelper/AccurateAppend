using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.ZenDesk.Support;
using DomainModel.ActionResults;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Tickets.TicketsApi
{
    /// <summary>
    /// Controller for listing Zendesk Tickets
    /// </summary>
    [Authorize]
    public class TicketsApiController : Controller
    {
        #region Fields

        private readonly ITicketService service;
        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketsApiController"/> class.
        /// </summary>
        public TicketsApiController(ITicketService service, ISessionContext context)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
            this.service = service;
        }

        #endregion

        #region Action Methods
        
        public virtual async Task<ActionResult> ListAllTickets(DataSourceRequest request, DateTime start, DateTime end, TicketStatus? status, TicketPriority? priority, CancellationToken cancellation)
        {
            request = request ?? new DataSourceRequest();
            request.PageSize = request.PageSize < 1 ? ListOptions.Default.PerPage : request.PageSize;

            var options = new ListOptions
            {
                Page = request.Page,
                PerPage = request.PageSize,
                Sort = TicketSort.CreatedAt,
                SortOrder = SortDirection.Descending
            };
            options.CreatedDuring(start, end);
            if (status != null) options.InStatus(status.Value);
            if (priority != null) options.WithPriority(priority.Value);

            var tickets = await service.ListAsync(options, cancellation);
            var data = this.Transform(tickets.Tickets, request);
            var result = new JsonNetResult(DateTimeKind.Local) {Data = data};

            return result;
        }

        /// <summary>
        ///     Gets all ZenDesk tickets for an account
        /// </summary>
        public virtual async Task<ActionResult> GetTicketsForUser(DataSourceRequest request, Guid userid, TicketStatus? status, TicketPriority? priority, CancellationToken cancellation)
        {
            request = request ?? new DataSourceRequest();
            request.PageSize = request.PageSize < 1 ? ListOptions.Default.PerPage : request.PageSize;

            using (context.CreateScope(ScopeOptions.NoTracking))
            {
                var baseQuery = this.context
                    .SetOf<Client>()
                    .Where(c => c.Logon.Id == userid);

                var contactsQuery = baseQuery.SelectMany(c => c.Contacts.Select(i => i.EmailAddress));
                var clientQuery = baseQuery.Select(c => c.DefaultEmail);
                var query = contactsQuery.Concat(clientQuery).Distinct();

                var results = new List<Task<IResultSet>>();
                var contacts = await query.ToArrayAsync(cancellation);

                foreach (var contact in contacts)
                {
                    var filter = TicketFilterFactory.CreateFilter(TicketFilterOption.EqualToRequester, new Requester { Email = contact });
                    var options = new ListOptions
                    {
                        Page = request.Page,
                        PerPage = request.PageSize,
                        Sort = TicketSort.CreatedAt,
                        SortOrder = SortDirection.Descending,
                        Filter = filter
                    };
                    if (status != null) options.InStatus(status.Value);
                    if (priority != null) options.WithPriority(priority.Value);

                    results.Add(service.ListAsync(options, cancellation));
                }

                await Task.WhenAll(results);

                var tickets = results.SelectMany(a => a.Result.Tickets).ToArray();
                var data = this.Transform(tickets, request);
                var result = new JsonNetResult(DateTimeKind.Local) { Data = data };

                return result;
            }
        }

        #endregion

        #region Helpers

        protected virtual DataSourceResult Transform(IEnumerable<Ticket> tickets, DataSourceRequest request)
        {
            var data = tickets.Select(t => new
                {
                    CreatedAt = t.CreatedAt.Value.ToUserLocal(),
                    t.Description,
                    Type = t.Type.ToString(),
                    t.Id,
                    t.HasIncidents,
                    Status = t.Status.GetDescription(),
                    t.Subject,
                    ZendeskDetail = $"https://accurateappend.zendesk.com/agent/tickets/{t.Id}",
                    SearchUrl = Url.Action("Index", "SearchClients",
                        new {Area = "Clients", searchterm = t.Recipient}),
                    t.Recipient
                })
                .ToDataSourceResult(request);
            data.Total = tickets.Count();

            return data;
        }

        #endregion
    }
}