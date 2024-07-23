using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Controllers
{
    /// <summary>
    /// Responsible for dispatching the current request into the public or NationBuilder root.
    /// </summary>
    [AllowAnonymous()]
    public class DefaultController : Controller
    {
        public ActionResult Index(String sourceId, String slug)
        {
            if (String.Equals(sourceId, "nbimports", StringComparison.OrdinalIgnoreCase))
            {
                return this.RedirectToAction("Index", "SignUp", new {area = "NationBuilder", slug});
            }

            return this.RedirectToAction("Index", "SignUp", new {area = "Public"});
        }

        [HttpGet()]
        public ActionResult Test()
        {
            return this.View();
        }

        [HttpPost()]
        public ActionResult Test(TestViewModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            return new LiteralResult(true) {Data = "Valid"};
        }
    }

    public class TestViewModel
    {
        [Display(Name = "Full Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Full name is required")]
        public String FullName { get; set; }

        [Display(Name = "Email Address")]
        [RegularExpression(@"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$")]
        public String EmailAddress { get; set; }

        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }

        [Display(Name = "Comments")]
        [Required(AllowEmptyStrings = true)]
        public String Comments { get; set; }
    }
}