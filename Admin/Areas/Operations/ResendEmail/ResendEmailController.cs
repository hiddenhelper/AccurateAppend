using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Operations.DataAccess;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Operations.ResendEmail
{
    /// <summary>
    /// Controller for resending tracked emails.
    /// </summary>
    [Authorize()]
    public class ResendEmailController:Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public ResendEmailController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual async Task<ActionResult> Index(Int32 id, CancellationToken cancellation)
        {
            var message = await this.context
                .Set<AccurateAppend.Operations.Message>()
                .Where(m => m.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (message == null) return this.RedirectToAction("index", "Message", new {Area = "Operations"});

            return this.View(message);
        }

        /// <summary>
        /// Resends a message
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index(Int32 id, String sendTo, String bccTo, CancellationToken cancellation)
        {
            var message = await this.context
                .Set<AccurateAppend.Operations.Message>()
                .FirstOrDefaultAsync(m => m.Id == id, cancellation);
            if (message != null)
            {
                var command = new ResendEmailCommand();
                command.MessageKey = message.Correlation;
                foreach (var address in sendTo.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    command.SendTo.Add(address);
                }

                foreach (var address in bccTo.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    command.BccTo.Add(address);
                }

                Validator.ValidateObject(command, new ValidationContext(command));

                await this.bus.Send(command);
            }

            return this.RedirectToAction("index", "Message", new { Area = "Operations" });
        }

        #endregion
    }
}