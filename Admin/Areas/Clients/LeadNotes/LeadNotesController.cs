using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadNotes
{
    /// <summary>
    /// Controller managing <see cref="Note">Notes</see> for a <see cref="Lead"/>.
    /// </summary>
    [Authorize()]
    public class LeadNotesController : ActivityLoggingController2
    {
        #region Constructor

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadNotesController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> dal component.</param>
        public LeadNotesController(ISessionContext context)
        {
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Retrieves notes for a given lead
        /// </summary>
        public virtual async Task<ActionResult> Query([DataSourceRequest] DataSourceRequest request, Int32 leadId, CancellationToken cancellation)
        {
            if (request.Sorts == null || !request.Sorts.Any()) request.Sorts = new List<SortDescriptor> { new SortDescriptor("DateAdded", ListSortDirection.Descending) };

            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var notes = (await this.context
                        .SetOf<Lead>()
                        .Where(l => l.Id == leadId)
                        .SelectMany(d => d.Notes)
                        .OrderByDescending(n => n.CreatedDate)
                        .ToArrayAsync(cancellation))
                    .Select(n => new
                    {
                        AddedBy = n.CreatedBy.UserName,
                        Body = n.Content,
                        DateAdded = n.CreatedDate.ToLocalTime()
                    });

                var data = notes.ToDataSourceResult(request, o => o);
                data.Total = data.Data.Count();

                var jsonNetResult = new JsonNetResult (DateTimeKind.Utc)
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }

        /// <summary>
        /// Saves note for a given <see cref="Lead"/>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Add(String body, Int32 leadId, CancellationToken cancellation)
        {
            body = (body ?? String.Empty).Trim().Left(4000);
            if (body.Length == 0) return new JsonResult();

            this.OnEvent($"Lead {leadId} note added");

            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                var lead = await this.context.SetOf<Lead>().FirstOrDefaultAsync(d => d.Id == leadId, cancellation);
                if (lead == null) return new JsonNetResult() { Data = new { Sucess = false, Message = "Lead does not exist" } };

                var user = await this.context.CurrentUserAsync(cancellation);
                lead.Notes.Add(user, body);
                await uow.CommitAsync(cancellation);
            }
            return new JsonNetResult() { Data = new { Sucess = true } };
        }

        #endregion
    }
}