using System;
using System.Web.Mvc;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Controllers
{
    /// <summary>
    /// Controller for managing load balancer monitor.
    /// </summary>
    [AllowAnonymous()]
    public class StatusController : Controller
    {
        public ActionResult Index()
        {
            return new LiteralResult(true) { Data = $"Server: {Environment.MachineName}\r\nStatus: Online" };
        }
    }
}