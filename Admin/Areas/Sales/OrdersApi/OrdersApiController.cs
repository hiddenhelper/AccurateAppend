using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.EditOrder;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.OrdersApi
{
    /// <summary>
    /// Controller to provide query access to <see cref="Order"/> data.
    /// </summary>
    [Authorize()]
    public class OrdersApiController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersApiController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> dal component.</param>
        public OrdersApiController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to query <see cref="Order"/> data for a single indicated <see cref="DealBinder"/> by identifier.
        /// </summary>
        public async Task<ActionResult> QueryByDeal(Int32 dealId, CancellationToken cancellation)
        {
            var orders = await this.context
                .SetOf<Order>()
                .Where(o => o.Deal.Id == dealId)
                .Include(o => o.Lines)
                .Include(o => o.Deal)
                .OrderBy(o => o.CreatedDate)
                .ToArrayAsync(cancellation);

            var data = orders.Select(o =>
                new
                {
                    o.Id,
                    DealId = o.Deal.Id,
                    DateOrdered = o.CreatedDate.ToUserLocal(),
                    o.Status,
                    Amount = o.Total(),
                    Links = new
                    {
                        Deal = this.Url.BuildFor<DealDetailController>().Detail(dealId),
                        View = this.CreateViewLink(o),
                        Edit = this.CreateEditLink(o),
                        Process = this.CreateChargeLink(o),
                        DownloadPdf = this.CreatePdfLink(o)
                    }
                }).ToArray();

            return new JsonNetResult(DateTimeKind.Local) {Data = data};
        }

        /// <summary>
        /// Action to query <see cref="Order"/> data for a single indicated identifier.
        /// </summary>
        public async Task<ActionResult> QueryById(Int32? orderId, Guid? publicKey, CancellationToken cancellation)
        {
            var publicKeyString = publicKey?.ToString();

            var order = await this.context
                .SetOf<Order>()
                .Where(o => o.Id == orderId || o.PublicKey == publicKeyString)
                .Include(o => o.Lines)
                .Include(o => o.Deal)
                .FirstOrDefaultAsync(cancellation);

            if (order == null) return new JsonNetResult(DateTimeKind.Local) {Data = null};

            var messageId = await this.context.Database.SqlQuery<Int32?>(
                    "SELECT [MessageId] FROM [operations].[Messages] WHERE [Correlation] = @p0",
                    order.PublicKey)
                .FirstOrDefaultAsync(cancellation);

            var data = 
                new
                {
                    OrderId = order.Id,
                    DealId = order.Deal.Id,
                    DateOrdered = order.CreatedDate.ToUserLocal(),
                    order.Status,
                    CostAdjustment = order.OrderMinimum,
                    Amount = order.Total(),
                    Links = new
                    {
                        Deal = this.Url.BuildFor<DealDetailController>().Detail(order.Deal.Id.Value),
                        View = this.Url.BuildFor<OrderDetailController>().Detail(order.Id.Value),
                        Edit =  this.CreateEditLink(order),
                        Process = this.CreateChargeLink(order),
                        Bill = this.CreateBillLink(order, messageId),
                        DownloadPdf = this.CreatePdfLink(order)
                    }
                };

            return new JsonNetResult(DateTimeKind.Local) { Data = data };
        }

        /// <summary>
        /// Action to query <see cref="ProductLine"/> data for a single indicated order.
        /// </summary>
        public async Task<ActionResult> LinesForOrder(Int32 orderId, CancellationToken cancellation)
        {
            var order = await this.context
                .SetOf<Order>()
                .Where(o => o.Id == orderId)
                .Include(p => p.Lines)
                .Include(o => o.Lines.Select(l => l.Product))
                .SingleOrDefaultAsync(cancellation);

            if (order == null) return new JsonNetResult() { Data = null };

            var data = new
            {
                OrderId = orderId,
                OrderStatus = order.Status,
                CostAdjustment = order.OrderMinimum,
                Total = order.Total(),
                Items = order
                    .Lines
                    .Select(l =>
                        new
                        {
                            OrderItemId = l.Id.Value,
                            l.Product.Key,
                            l.Description,
                            l.Quantity,
                            l.Price,
                            Total = l.Total()
                        })
            };

            return new JsonNetResult(DateTimeKind.Local) { Data = data };
        }

        /// <summary>
        /// Query api that allows the status of a transaction matching the provided public key.
        /// </summary>
        /// <param name="publicKey">The public key identifier relating charges to order transactions.</param>
        /// <param name="cancellation">Signals the need to cancel an asynchronous process.</param>
        [HttpGet()]
        public virtual async Task<ActionResult> CheckTransaction(Guid publicKey, CancellationToken cancellation)
        {
            var charge = await this.context
                .SetOf<TransactionEvent>()
                .SingleOrDefaultAsync(e => e.PublicKey == publicKey, cancellation);
            if (charge == null)
            {
                return this.Json(new { Complete = false }, JsonRequestBehavior.AllowGet);
            }

            return this.Json(new { Complete = true, Success = this.IsSuccess(charge.Status), Message = this.StatusToMessage(charge.Status) }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helpers

        protected virtual String CreateViewLink(Order order)
        {
            return !order.Deal.Status.CanBeEdited()
                    ? this.Url.BuildFor<OrderDetailController>().Detail(order.Id.Value)
                    : null;
        }

        protected virtual String CreateEditLink(Order order)
        {
            if (order is RefundOrder && order.Status.CanBeEdited())
            {
                return this.Url.Action("Index", "RefundDeal", new { Area = "Sales", orderId = order.Id });
            }

            if (!order.Deal.Status.CanBeEdited()) return null;

            return this.Url.BuildFor<EditOrderController>().Edit(order.Id.Value);
        }

        protected virtual String CreateChargeLink(Order order)
        {
            if (order is RefundOrder) return null;

            if (!order.Status.CanBeEdited() || order.Deal.Status != DealStatus.Billing) return null;
            
            return this.Url.BuildFor<SubmitChargeController>().ViewOrder(order.Id.Value);
        }

        protected virtual String CreateBillLink(Order order, Int32? messageId)
        {
            if (messageId != null)
            {
                return this.Url.Action("Index", "Message", new { area = "Operations", id = messageId });
            }

            if (order.Status == OrderStatus.Canceled || order.Deal.Status.CanBeEdited()) return null;

            if (order.Status == OrderStatus.Open)
            {
                return this.Url.Action("Content", "ViewBill", new {area = "Sales", orderId = order.Id});
            }

            return null;
        }

        protected virtual String CreatePdfLink(Order order)
        {
            return this.Url.Action("SaveAsPdf", "OrderDetail", new { Area = "Sales", orderId = order.Id });
        }

        /// <summary>
        /// Used to simplify the mapping of <see cref="TransactionResult"/> to a simple Success|Fail value.
        /// </summary>
        protected virtual Boolean IsSuccess(TransactionResult status)
        {
            switch (status)
            {
                case TransactionResult.Approved:
                case TransactionResult.Refunded:
                case TransactionResult.Voided:
                case TransactionResult.Processed:
                    return true;
                case TransactionResult.Declined:
                case TransactionResult.Fraud:
                case TransactionResult.CardExpired:
                case TransactionResult.InvalidCardNumber:
                case TransactionResult.ProcessorError:
                case TransactionResult.LocalSystemError:
                    return false;
                default:
                    throw new NotSupportedException($"Status {status} is not supported");
            }
        }

        /// <summary>
        /// Used to simplify the mapping of <see cref="TransactionResult"/> to a human readable message.
        /// </summary>
        protected virtual String StatusToMessage(TransactionResult status)
        {
            switch (status)
            {
                case TransactionResult.Approved:
                case TransactionResult.Refunded:
                case TransactionResult.Voided:
                case TransactionResult.Processed:
                    return "Charge successfully transmitted.";
                case TransactionResult.Declined:
                case TransactionResult.Fraud:
                case TransactionResult.CardExpired:
                case TransactionResult.InvalidCardNumber:
                    return "The transaction was declined.";
                case TransactionResult.ProcessorError:
                    return "The remote processor has encountered an issue attempting to process the charge. Check AuthNet.";
                case TransactionResult.LocalSystemError:
                    return "The local processor has encountered an issue attempting to process the charge. Check the event log.";
                default:
                    throw new NotSupportedException($"Status {status} is not supported");
            }
        }

        #endregion
    }
}