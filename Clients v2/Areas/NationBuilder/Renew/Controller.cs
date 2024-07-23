using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Renew
{
    /// <summary>
    /// Handles logic for renewing an existing NationBuilder integration by requesting an updated token.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        /// <summary>
        /// Handles oAuth for a user who has an expired token.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(String slug)
        {
            slug = (slug ?? String.Empty).Trim();

            if (String.IsNullOrEmpty(slug)) return this.View(slug);

            // if slug was passed in then initiate pre-checks for connection process
            var tokenService = new DefaultTokenService(AccessPoint.BuildTokenHandlerUri(this.HttpContext));
            if (await tokenService.CheckNationExistsAsync(slug) == NationCheckStatus.NotFound)
            {
                this.ModelState.AddModelError(nameof(slug), "The Nation you entered is reported to not exist by NationBuilder. Make sure you've typed it in correctly and are using your actual registered Nation name.");
            }
            if (!this.ModelState.IsValid) return this.View(slug);
            
            // Start renewal process
            return this.RedirectToAction("Initiate", "AuthHandler", new {slug, area = "NationBuilder"});
        }

        [HttpPost()]
        [ActionName("Index")]
        public virtual async Task<ActionResult> RenewPostBack(String slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                this.ModelState.AddModelError(nameof(slug), "Please supply your Nation name.");
                return this.View((Object)slug);
            }

            // ensure nation exists
            var tokenService = new DefaultTokenService(AccessPoint.BuildTokenHandlerUri(this.HttpContext));
            if (await tokenService.CheckNationExistsAsync(slug) == NationCheckStatus.NotFound)
            {
                this.ModelState.AddModelError(nameof(slug), "The Nation you entered is reported to not exist by NationBuilder. Make sure you've typed it in correctly and are using your actual registered Nation name.");
            }

            if (!this.ModelState.IsValid) return this.View((Object) slug);

            return this.RedirectToAction("Initiate", "AuthHandler", new {slug, area = "NationBuilder"});
        }
    }
}