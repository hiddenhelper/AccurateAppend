using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Messages;
using AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword.Models;
using DomainModel;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.ResetPassword
{
    [AllowAnonymous()]
    public class ResetPasswordController : Controller
    {
        #region Fields

        private readonly IMembershipService ms;
        private readonly IFormsAuthentication fa;
        private readonly ISessionContext context;
        private readonly IMessageSession bus;
        private readonly CaptchaVerifyer verifier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordController"/> class.
        /// </summary>
        /// <param name="ms">The <see cref="IMembershipService"/> component that provides membership functionality.</param>
        /// <param name="fa">The <see cref="IFormsAuthentication"/> component that provides logon functionality.</param>
        /// <param name="context">The <see cref="ISessionContext"/> component providing entity access.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> providing access to the bus.</param>
        /// <param name="verifier"></param>
        public ResetPasswordController(IMembershipService ms, IFormsAuthentication fa, ISessionContext context, IMessageSession bus, CaptchaVerifyer verifier)
        {
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (fa == null) throw new ArgumentNullException(nameof(fa));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));
            Contract.EndContractBlock();

            this.ms = ms;
            this.fa = fa;
            this.context = context;
            this.bus = bus;
            this.verifier = verifier;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Sends email with link to user
        /// </summary>
        public virtual new ActionResult Request()
        {
            return this.View(new ResetPasswordModel());
        }

        /// <summary>
        /// Sends email with link to user
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Request(ResetPasswordModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            var passedCaptcha = await this.verifier.Verify(base.Request, cancellation);
            if (!passedCaptcha) return this.View(model);

            var message = new SendPasswordUpdateRequestCommand();
            message.UserName = model.UserName;

            await this.bus.Send(message);

            return this.View("RequestSent");
        }

        /// <summary>
        /// Reset user password.
        /// </summary>
        public virtual async Task<ActionResult> Update(Guid id, CancellationToken cancellation)
        {
            var model = new LocalPasswordModel() { PublicKey = id };

            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var request = await this.context.SetOf<PasswordResetRequest>().FirstOrDefaultAsync(r => r.PublicKey == id, cancellation);
                if (request == null || !request.IsValid())
                {
                    model.IsValid = false;
                }
                else
                {
                    model.IsValid = true;
                }

                return this.View(model);
            }
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Update(LocalPasswordModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            String userName;

            using (var trans = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                {
                    var request = await this.context
                        .SetOf<PasswordResetRequest>()
                        .Include(r => r.Logon)
                        .Include(r => r.Logon.Application)
                        .FirstOrDefaultAsync(r => r.PublicKey == model.PublicKey, cancellation);
                    if (request == null || !request.IsValid()) return this.RedirectToAction(nameof(Update), new { area = "Authentication", id = model.PublicKey });

                    if (!await this.ms.ChangePasswordAsync(request.Logon.UserName, model.NewPassword, request.Logon.Application.Id, cancellation))
                    {
                        this.TempData["message"] = "We're sorry but we were unable to reset your password. Please try again.";
                        return this.View(model);
                    }

                    request.MarkReset();
                    await uow.CommitAsync(cancellation);

                    userName = request.Logon.UserName;
                    trans.Complete();
                }
            }

            var principal = await this.ms.GetPrincipalAsync(userName, System.Security.Claims.AuthenticationTypes.Password, cancellation);
            this.fa.CreateAuthenticationToken(principal, false);

            // We use a redirect so the site menu refreshes
            return this.RedirectToAction(nameof(ResetPasswordSuccess));
        }

        [Authorize()]
        public virtual ActionResult ResetPasswordSuccess()
        {
            return this.View();
        }

        #endregion
    }
}