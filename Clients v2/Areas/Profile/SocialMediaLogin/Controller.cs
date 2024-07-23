using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin.Messages;
using AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin.Models;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin
{
    /// <summary>
    /// Controller to render view for managing social logins
    /// </summary>
    [Authorize()]
    public class SocialMediaLoginController : Controller
    {
        #region Fields

        private readonly ISessionContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing data access to this instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> component providing access to the bus.</param>
        public SocialMediaLoginController(ISessionContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Display the list of linked social logins.
        /// </summary>
        public virtual ActionResult Index(CancellationToken cancellation)
        {
            return this.View();
        }

        /// <summary>
        /// Returns the list of linked social logins.
        /// </summary>
        public virtual async Task<ActionResult> Read(CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var model = (await this.context
                        .SetOf<MappedIdentityLogon>()
                        .ForInteractiveUser()
                        .OrderBy(l => l.DisplayName)
                        .ToArrayAsync(cancellation))
                    .Select(x => new
                    {
                        Name = x.DisplayName,
                        x.Id,
                        ProviderName = x.Provider.GetDescription(),
                        Actions = new
                        {
                            Remove = this.Url.Action("Remove", "SocialMediaLogin", new {area = "Profile", x.Id})
                        }
                    }).ToList();

                return this.Json(model,JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet()]
        public virtual ActionResult Invite()
        {
            if (this.User.Identity.GetAuthenticationMethod() != System.Security.Claims.AuthenticationTypes.Password) return this.RedirectToAction("Index", "SocialMediaLogin", new { Area = "Profile" });

            return this.View(new InviteUserModel());
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Invite(InviteUserModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);
            if (this.User.Identity.GetAuthenticationMethod() != System.Security.Claims.AuthenticationTypes.Password) return this.RedirectToAction("Index", "SocialMediaLogin", new {Area = "Profile"});

            var command = new InviteUserCommand
            {
                UserId = this.User.Identity.GetIdentifier(),
                EmailAddress = model.EmailAddress
            };

            await this.bus.SendLocal(command);

            return this.RedirectToAction("Index", "SocialMediaLogin", new {Area = "Profile"});
        }

        /// <summary>
        /// Action to process a remove action on an identity.
        /// </summary>
        /// <param name="id">The identity of the external login to remove from our system.</param>
        [HttpDelete()]
        public virtual async Task<ActionResult> Remove(Int32 id, CancellationToken cancellation)
        {
            using (new Correlation(this.User.Identity.GetIdentifier()))
            {
                try
                {
                    using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                    {
                        var selectedLogin = await this.context
                            .SetOf<MappedIdentityLogon>()
                            .ForInteractiveUser()
                            .Where(l => l.Id == id)
                            .FirstOrDefaultAsync(cancellation);

                        if (selectedLogin == null)
                        {
                            return Json(new
                            {
                                Message = "Social media login not found",
                                Success = true
                            });
                        }

                        this.context.SetOf<MappedIdentityLogon>().Remove(selectedLogin);
                        await uow.CommitAsync(cancellation);

                        return this.Json(new
                        {
                            Message = "Social media login successfully removed",
                            Success = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.Medium,Application.Clients,this.Request.UserHostAddress, $"Exception deleting external login {id}");
                    return this.Json(new
                    {
                        Message = "Error encountered removing login",
                        Success = false
                    });
                }
            }
        }

        #endregion
    }
}
