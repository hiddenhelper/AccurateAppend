using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.DealFiles.Models;
using NServiceBus;
using Order = AccurateAppend.Sales.Order;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealFiles
{
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        public Controller(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        public virtual async Task<ActionResult> Index(Guid publicKey, CancellationToken cancellation)
        {
            var model = await this.context
                .Set<Order>()
                .Where(o => o.PublicKey == publicKey.ToString())
                .Select(o => o.Deal)
                .Select(d =>
                    new LinkFilesToDeal
                    {
                        UserId = d.Client.UserId,
                        PublicKey = publicKey,
                        Title = d.Title,
                        DealId = d.Id.Value
                    })
                .FirstAsync(cancellation);

            return this.View(model);
        }

        public virtual async Task<ActionResult> Select(Guid deal, Guid File)
        {
            var command = new AccurateAppend.Operations.Contracts.CorrelateFileCommand
            {
                RequestId = Guid.NewGuid(),
                PublicKey = File,
                CorrelationId = deal
            };

            await this.bus.SendLocal(command);

            return this.RedirectToAction("PublicKey", "DealDetail", new {id = deal});
        }
    }
}