using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting.DataAccess;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Security;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Email
{
    [Authorize()]
    public class EmailController : Controller
    {
        #region Fields

        private readonly IMessageQueue queue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="queue">The <see cref="IMessageQueue"/> to use for this controller instance.</param>
        public EmailController(IMessageQueue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            Contract.EndContractBlock();

            this.queue = queue;
        }

        #endregion

        #region Action Methods

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Send(String to, String from, String subject, String body, CancellationToken cancellation)
        {
            if (String.IsNullOrWhiteSpace(to)) return this.Json(new { Success = "False", Message = "Message not sent. Error:email to is empty" });
            if (String.IsNullOrWhiteSpace(from)) return this.Json(new { Success = "False", Message = "Message not sent. Error:email from is empty" });
            if (String.IsNullOrWhiteSpace(subject) && String.IsNullOrWhiteSpace(body)) return this.Json(new { Success = "False", Message = "Message not sent. Error:email subject and body are both empty" });

            try
            {
                var message = new Security.Message(from)
                {
                    Subject = subject,
                    Body = body
                };
                message.SendTo.Add(to);

                await this.queue.EnqueueMessageAsync(message);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Low, Application.AccurateAppend_Admin, description: "Failure sending admin email");
                return this.Json(new { Success = "False", Message = $"Message not sent. Error:{ex.Message}" });
            }

            return this.Json(new { Success = "True", Message = "" });
        }

        #endregion
    }
}