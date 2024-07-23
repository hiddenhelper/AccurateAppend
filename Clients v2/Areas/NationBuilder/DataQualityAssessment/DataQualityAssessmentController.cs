using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.DataQualityAssessment.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.DataQualityAssessment
{
    /// <summary>
    /// Controller for managing a client request for data consultation.
    /// </summary>
    [Authorize()]
    public class DataQualityAssessmentController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        internal const Int32 ImpressiveListSize = 75000;
        internal const String CookieName = "UserSettings";
        internal const String SettingName = "SuppressDataQualityAssessment";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataQualityAssessmentController"/> class.
        /// </summary>
        public DataQualityAssessmentController(IMessageSession bus)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Display a form for requesting data consultation.
        /// </summary>
        public virtual ActionResult Request(Guid cartId)
        {
            var model = new ConsultationRequestModel();
            model.CartId = cartId;

            return this.View(model);
        }

        /// <summary>
        /// Process the request for data consultation.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Request(ConsultationRequestModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            var userId = this.User.Identity.GetIdentifier();

            var message = new MailMessage();
            message.From = new MailAddress("support@accurateappend.com");
            message.To.Add(new MailAddress("support@accurateappend.com"));
            message.Subject = "Data consultation requested";
            message.Body = $"Name: {model.Name}\r\nPhone: {model.Phone}\r\nComments: {model.Comments}\r\nAccount link: https://admin.accurateappend.com/Users/Detail?userid={userId}";

            var command = new SendEmailCommand(message);
            command.Track = false;

            await this.bus.Send(command);
            
            this.SuppressNotifications();

            // Instead of a redirect, return the named view directly
            return this.RedirectToAction(nameof(this.Thankyou), new {cartId = model.CartId} );
        }

        /// <summary>
        /// Displays the thank you notice.
        /// </summary>
        public virtual ActionResult Thankyou(Guid cartId)
        {
            return this.View(cartId);
        }

        /// <summary>
        /// Performs the suppression cookie.
        /// </summary>
        public virtual ActionResult Suppress(Guid cartId)
        {
            this.SuppressNotifications();

            return this.RedirectToAction("Index", "DisplayLists", new { area = "NationBuilder", cartId });
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Used to add/update a cookie for not displaying the interstitial view.
        /// </summary>
        protected void SuppressNotifications()
        {
            // suppress future solicitations
            var userSettings = new HttpCookie(CookieName);
            userSettings.Values[SettingName] = true.ToString();
            userSettings.Expires = DateTime.Now.AddMonths(3);

            this.Response.Cookies.Add(userSettings);
        }

        #endregion
    }
}