using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Box.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Box.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Box
{
    /// <summary>
    /// Controller to support Box.com integration based file processing. Operates as an
    /// interstitial process for another controller .
    /// </summary>
    [Authorize()]
    [ValidateInput(false)]
    public class BoxController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxController"/> class.
        /// </summary>
        public BoxController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods
        
        [HttpGet()]
        public virtual ActionResult SelectList(Guid cartId)
        {
            var data = this.Url.Action("ForCurrentUser", "BoxApi", new {Area = "Box"});

            var model = new SelectListModel();
            model.CartId = cartId;
            model.DataUrl = new Uri(data, UriKind.RelativeOrAbsolute);

            return this.View(model);
        }

        [HttpPost()]
        public virtual async Task<ActionResult> SelectList(Guid cartId, Int64 nodeId, CancellationToken cancellation)
        {
            var command = new TransferBoxFileCommand();
            command.PublicKey = cartId;
            command.NodeId = nodeId;

            await this.bus.SendLocal(command);

            return this.Json(new {Status = (Int32) HttpStatusCode.Accepted});
        }

        public virtual ActionResult ProcessFile(Guid cartId)
        {
            var model = new ProcessFileModel();
            model.CartId = cartId;
            model.CheckUrl = this.Url.Action("CartReady", new {Area = "Order", cartId});
            model.NextUrl = this.Url.Action("SelectProducts", "Csv", new {Area = "Order", cartId});

            return this.View(model);
        }

        [HttpGet()]
        public virtual async Task<ActionResult> CartReady(Guid cartId, CancellationToken cancellation)
        {
            var exists = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .ForCsv(cartId)
                .WithCompletedAnalysis() // Ensures we've got the File downloaded
                .Where(c => c.IsActive)
                .AnyAsync(cancellation);

            return this.Json(new { Ready = exists }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}