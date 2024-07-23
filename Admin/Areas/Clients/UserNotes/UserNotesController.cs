using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserNotes
{
    /// <summary>
    /// Controller for managing <see cref="Note"/> entities for a specific <see cref="Client"/> .
    /// </summary>
    [Authorize()]
    public class UserNotesController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotesController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public UserNotesController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Read(Guid userId, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var notes = await this.Context.SetOf<Client>()
                        .Where(c => c.Logon.Id == userId)
                        .SelectMany(c => c.Notes)
                        .OrderByDescending(n => n.CreatedDate)
                        .Select(n =>
                            new
                            {
                                n.Id,
                                n.CreatedBy.UserName,
                                n.Content,
                                n.CreatedDate
                            })
                        .ToArrayAsync(cancellation);

                var result = new JsonNetResult();
                result.Data = notes.Select(n =>
                            new
                            {
                                n.Id,
                                AddedBy = n.UserName,
                                Body = n.Content,
                                DateAdded = n.CreatedDate.ToUserLocal()
                            });

                return result;
            }
        }

        public virtual async Task<ActionResult> Add(String body, Guid userId, CancellationToken cancellation)
        {
            body = (body ?? String.Empty).Trim();
            if (String.IsNullOrWhiteSpace(body)) return this.Json(new { Sucess = false, Message = "A note requires a content value" });

            using (var uow = this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                var client = await this.Context
                    .SetOf<Client>()
                    .Where(c => c.Logon.Id == userId)
                    .Include(c => c.Notes)
                    .FirstOrDefaultAsync(cancellation);
                if (client == null) return this.Json(new {Sucess = false, Message = "Client does not exist"});

                var user = await this.Context.CurrentUserAsync(cancellation);

                client.Notes.Add(new Note(body, user));

                await uow.CommitAsync(cancellation);
            }

            return this.Json(new { Sucess = true, Message = "Note added" });
        }

        #endregion
    }
}