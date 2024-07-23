using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Standardization;
using AccurateAppend.Websites.Clients.Areas.Public.OptOut.Messaging;
using AccurateAppend.Websites.Clients.Areas.Public.OptOut.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.OptOut
{
    /// <summary>
    /// Controller to capture opt out requests.
    /// </summary>
    [AllowAnonymous()]
    public class OptOutController : Controller
    {
        #region Fields

        private readonly IAddressStandardizer addresStandardizer;
        private readonly IMessageSession bus;
        private readonly INameStandardizer nameStandardizer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptOutController"/> class.
        /// </summary>
        /// <param name="bus">The message bus used to interact with the support service.</param>
        /// <param name="addresStandardizer">An <see cref="IAddressStandardizer"/> used for address standardization.</param>
        /// <param name="nameStandardizer">An <see cref="INameStandardizer"/> used for name standardization.</param>
        public OptOutController(IMessageSession bus, IAddressStandardizer addresStandardizer, INameStandardizer nameStandardizer)
        {
            this.bus = bus;
            this.addresStandardizer = addresStandardizer;
            this.nameStandardizer = nameStandardizer;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display an opt-out form.
        /// </summary>
        [HttpGet()]
        public virtual ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Current", new {Area = "Order"});

            return this.View(new OptOutModel());
        }

        /// <summary>
        /// Action to process a submitted opt-out form.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(OptOutModel model)
        {
            model = model ?? new OptOutModel();
            if (!this.ModelState.IsValid) return this.View(model);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                //Craft email
                await SendEmail(model);

                //Send update command
                await SendCommand(model);

                transaction.Complete();
            }

            this.TempData["message"] = "Your information has been submitted.";
            this.TempData["messageType"] = "success";

            return this.RedirectToAction("Index");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Helper to craft a support email for an opt out request.
        /// </summary>
        /// <param name="model">The <see cref="OptOutModel"/> to process.</param>
        protected virtual Task SendEmail(OptOutModel model)
        {
            var command = new SendEmailCommand();
            command.To.Add("jimmy@accurateappend.com");
            command.SendFrom = "support@accurateappend.com";
            command.IsHtmlContent = false;
            command.Subject = "Opt-out Request";
            command.Body = $@"
{model.FirstName} {model.LastName}
{model.Address}
{model.City} {model.StatePlainText} {model.PostalCode}
{model.Phone}
{model.Email}

{model.Comments}
";
            return bus.Send(command);
        }

        /// <summary>
        /// Helper to craft a bus command for an opt out request.
        /// </summary>
        /// <param name="model">The <see cref="OptOutModel"/> to process.</param>
        protected Task SendCommand(OptOutModel model)
        {
            var command = new OptOutCommand
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Email = model.Email,
                Phone = model.Phone
            };
            command.Standardize(nameStandardizer, addresStandardizer);

            return bus.SendLocal(command);
        }

        #endregion
    }
}