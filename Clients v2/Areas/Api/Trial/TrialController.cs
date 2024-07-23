using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Standardization;
using AccurateAppend.Websites.Clients.Areas.Api.Trial.Messages;
using AccurateAppend.Websites.Clients.Areas.Api.Trial.Models;
using DomainModel;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Api.Trial
{
    /// <summary>
    /// Provides API trial key signup access.
    /// </summary>
    [AllowAnonymous()]
    public class TrialController : Controller
    {
        #region Fields
        
        private readonly ISessionContext context;
        private readonly INameStandardizer parser;
        private readonly IMessageSession bus;
        private readonly CaptchaVerifyer verifier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TrialController"/> class.
        /// </summary>
        public TrialController(ISessionContext context, INameStandardizer parser, CaptchaVerifyer verifier, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (verifier == null) throw new ArgumentNullException(nameof(verifier));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.parser = parser;
            this.verifier = verifier;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Form through which prospects can request a free trial.
        /// </summary>
        public virtual ActionResult RequestTrial()
        {
            return this.View();
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> RequestTrial(ApiSignupModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            var passedCaptcha = await this.verifier.Verify(this.Request, cancellation);
            if (!passedCaptcha) return this.View(model);

            if (!String.IsNullOrEmpty(model.Name))
            {
                var response = this.parser.Parse(model.Name);
                model.FirstName = response.FirstName;
                model.LastName = response.LastName;
            }

            try
            {
                if (await this.CheckEmailIsInUse(this.context, model.Email, cancellation))
                {
                    this.TempData["message"] = "This email has been used to request a previous trial. Please try with another email or contact customer support.";
                    this.TempData["messageType"] = "error";
                    return this.View(model);
                }

                if (String.IsNullOrWhiteSpace(model.FirstName)) model.FirstName = "New";
                if (String.IsNullOrWhiteSpace(model.LastName)) model.LastName = "Lead";

                var command = new RequestTrialCommand
                {
                    ApplicationId = ApplicationExtensions.AccurateAppendId,
                    Company = model.Company,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Ip = model.Ip,
                    Referrer = model.Referrer
                };

                await this.bus.Send(command);

                return this.View("ThankYou");
            }
            catch (Exception ex)
            {
                this.TempData["message"] = "An error has occured. Please contact customer support.";
                this.TempData["messageType"] = "error";
                Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress,
                    $"{nameof(TrialController)} failing. Model={model}");
            }

            return this.View(model);
        }

        #endregion

        #region Query Helpers

        protected virtual Task<Boolean> CheckEmailIsInUse(ISessionContext context, String email, CancellationToken cancellation)
        {
            // check email against current users
            var cutoff = DateTime.Now.AddMonths(-6);
            var userQuery = context.SetOf<User>().Where(u => u.UserName == email).Select(u => u.UserName);
            var trialQuery = context.SetOf<Lead>()
                .Where(l => l.Trial.IsEnabled && l.CreatedDate >= cutoff && l.DefaultEmail == email)
                .Select(l => l.DefaultEmail);

            var query = trialQuery.Union(userQuery);

            return query.AnyAsync(cancellation);
        }

        #endregion
    }
}