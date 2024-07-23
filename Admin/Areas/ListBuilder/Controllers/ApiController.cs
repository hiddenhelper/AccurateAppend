using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.ListBuilder.Controllers;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers
{
    [Authorize()]
    public class ApiController : ApiControllerBase
    {
        /// <inheritdoc />
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 1 * 60, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public override Task<ActionResult> GetCounties(Int32[] state, CancellationToken cancellation)
        {
            return base.GetCounties(state, cancellation);
        }

        /// <inheritdoc />
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 1 * 30, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public override Task<ActionResult> GetCities(Int32[] state, CancellationToken cancellation)
        {
            return base.GetCities(state, cancellation);
        }

        /// <inheritdoc />
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 2 * 60, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public override ActionResult GetStates()
        {
            return base.GetStates();
        }

        /// <inheritdoc />
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(Duration = 1 * 60, Location = OutputCacheLocation.Server, VaryByParam = "*")]
        public override Task<ActionResult> GetZips(Int32[] state, CancellationToken cancellation)
        {
            return base.GetZips(state, cancellation);
        }
    }
}