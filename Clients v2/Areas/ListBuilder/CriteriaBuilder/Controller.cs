using System;
using System.Web.Mvc;
using AccurateAppend.ListBuilder.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.CriteriaBuilder
{
    /// <summary>
    /// Controller used for managing the List Builder criteria collection.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public Controller(IMessageSession bus)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Generates a unique list identifier session.
        /// </summary>
        public virtual ActionResult Start()
        {
            //var cartId = Guid.NewGuid();
            //var command = new CreateListBuilderCartCommand();
            //command.UserId = this.User.Identity.GetIdentifier();
            //command.CartId = cartId;


            //await this.bus.Send(command, new SendOptions());

            return this.RedirectToAction(nameof(this.Index), new {id = Guid.NewGuid()});
        }

        /// <summary>
        /// Actually displays the end user view.
        /// </summary>
        public ActionResult Index(Guid id)
        {
            var listCriteria = new ListCriteria {RequestId = id};

            return this.View(listCriteria);
        }

        #endregion
    }
}