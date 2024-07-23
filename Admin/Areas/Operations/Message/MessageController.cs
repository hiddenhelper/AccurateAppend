using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Operations.Message.Models;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Message
{
    /// <summary>
    /// Controller for presenting email communication information
    /// </summary>
    [Authorize()]
    public class MessageController : ContextBoundController
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component.</param>
        public MessageController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Int32? id, String email, CancellationToken cancellation)
        {
            var model = new MessagesView();
            if (id != null) model.MessageDetail = this.Url.Action("GetMessageDetailJson", new {id = id.Value});

            var user = await this.Context
                .SetOf<Client>()
                .ClientsWithMatchingContact(email)
                .Select(c => (Guid?) c.Logon.Id)
                .FirstOrDefaultAsync(cancellation);

            model.UserId = user;
            if (user != null)
            {
                model.UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(user.Value);
            }
            model.Email = email;

            return this.View(model);
        }

        /// <summary>
        /// Returns Json Message collection
        /// </summary>
        public virtual async Task<ActionResult> GetMessagesJson([DataSourceRequest] DataSourceRequest request, Guid applicationid, DateTime startdate, DateTime enddate, String email, int? messageid, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                // Messages table is UTC so we ned to convert start/end dates
                startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
                enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

                // limit messages to site using the SentFrom email address
                var site = await this.Context.SetOf<Application>().Where(a => a.Id == applicationid).Select(a => a.Details).FirstOrDefaultAsync(cancellation);
                var query = (await this.Context.SetOf<Security.Message>().ActiveDuring(startdate, enddate).ToArrayAsync(cancellation)).Where(a => a.SendFrom == site.Mail.SupportAddress);

                if (messageid != null)
                {
                    query = query.Where(m => m.Id == messageid);
                }
                else if (!String.IsNullOrEmpty(email))
                {
                    var messages = query;

                    // determine if input email belongs to a client, if it does then pull all messages associated with the account, otherwise pull messages for this email
                    var detail = await this.Context.SetOf<Client>().ClientsWithMatchingContact(email).Include(c => c.Contacts).FirstOrDefaultAsync(cancellation);
                    if (detail != null)
                    {
                        var emails = detail.Contacts.Select(c => c.EmailAddress).ToList();
                        emails.Add(email);
                        messages = messages.Where(m => m.SendTo.Any(e => emails.Contains(e.Address, StringComparer.OrdinalIgnoreCase)));
                    }
                    else
                    {
                        messages = messages.Where(m => m.SendTo.Select(s => s.Address).Contains(email, StringComparer.OrdinalIgnoreCase));
                    }
                    query = messages.AsQueryable();
                }

                var data = query.ToArray().Select(m => new
                {
                    m.Id,
                    m.SendTo,
                    m.Subject,
                    m.Status,
                    CreatedDate = m.CreatedDate.ToUserLocal(),
                    ModifiedDate = m.ModifiedDate.ToUserLocal(),
                    Detail = this.Url.Action("GetMessageDetailJson", new {m.Id}),
                    Resend = m.Status == MessageStatus.Sent ? this.Url.Action("Index", "ResendEmail", new {Area = "Operations", m.Id}) : null,
                    ClearPoison = m.Status == MessageStatus.Posion ? this.Url.Action("Index", "ClearPoisonEmail", new {Area = "Operations", m.Id }) : null
                }).OrderByDescending(a => a.ModifiedDate).ToDataSourceResult(request);
                data.Total = data.Data.Count();
                
                var jsonNetResult = new JsonNetResult(DateTimeKind.Utc)
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }
       
        /// <summary>
        /// Returns Json representation of a message
        /// </summary>
        [OutputCache(Duration = 20*60, VaryByParam = "*")]
        public virtual async Task<ActionResult> GetMessageDetailJson(Int32 id, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var set = this.Context.SetOf<Security.Message>();
                var message = await set.Where(m => m.Id == id).AsNoTracking().FirstAsync(cancellation);

                var dto = new MessageDetail(message);
                return this.Json(new
                        {
                            dto.Attachments,
                            dto.BccTo,
                            dto.CreatedDate,
                            dto.CanClear,
                            dto.CanResend,
                            dto.Id,
                            dto.ModifiedDate,
                            dto.Status,
                            dto.SendFrom,
                            dto.SendTo,
                            dto.Subject,
                            Detail = this.Url.Action("Index", "Message", new { dto.Id }, Uri.UriSchemeHttps),
                            View = this.Url.Action("Index", "ViewEmail", new { dto.Id }),
                            Resend = message.Status == MessageStatus.Sent || message.Status == MessageStatus.Posion ? this.Url.Action("Index", "ResendEmail", new { Area = "Operations", message.Id }) : null,
                            ClearPoison = message.Status == MessageStatus.Posion ? this.Url.Action("Index", "ClearPoisonEmail", new { Area = "Operations", message.Id }) : null
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Returns list of recent messages recipients
        /// </summary>
        public virtual async Task<ActionResult> GetRecentMessageRecipientsJson(Guid applicationid, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                // user for entire site
                var users = await this.Context.SetOf<User>().Where(a => a.Application.Id == applicationid).Select(a => a.EmailAddress.ToLower()).ToArrayAsync(cancellation);
                // users who have been sent a message, includes Alt emails
                var supportEmail = await this.Context.SetOf<Application>().Where(a => a.Id == applicationid).Select(a => a.Details.Mail.SupportAddress.ToLower()).FirstAsync(cancellation);
                var query = this.Context.SetOf<Security.Message>().ActiveDuring(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.ToEndOfDay()).Where(a => a.SendFrom == supportEmail);
                var messages = await query.OrderByDescending(m => m.ModifiedDate).Take(500).ToArrayAsync(cancellation);
                var recipients = messages.SelectMany(s => s.SendTo.Select(a => a.Address)).Distinct().OrderBy(e => e);
                return this.Json(recipients.Concat(users).Distinct(), // merge both user lists so we have a complete list of users plus alt email addresses
                    JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion
    }
}
