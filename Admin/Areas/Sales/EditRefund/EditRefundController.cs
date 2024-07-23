using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.DataAccess;
using DomainModel.Enum;

namespace AccurateAppend.Websites.Admin.Areas.Sales.EditRefund
{
    /// <summary>
    /// Performs refund order item editing.
    /// </summary>
    [Authorize()]
    public class EditRefundController : Controller
    {
        #region Fields

        private readonly IBillFormatterFactory factory;
        private readonly DefaultContext context;

        #endregion

        #region Constructor

        public EditRefundController(DefaultContext context, IBillFormatterFactory factory)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            Contract.EndContractBlock();

            this.context = context;
            this.factory = factory;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Draft(Int32 orderId, CancellationToken cancellation)
        {
            var deal = await this.context.SetOf<RefundOrder>()
                .AreEditable()
                .Where(o => o.Id == orderId)
                .Select(o => o.Deal)
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(cancellation);

            if (deal == null)
            {
                return this.DisplayErrorResult($"Order {orderId} does not exist or the deal does not have an open refund order currently in process.");
            }

            var refund = deal.Orders.OfType<RefundOrder>().FirstOrDefault(o => o.Status == OrderStatus.Open);
            
            var model = new CreateBill.Models.BillViewModel(refund);
            var formatter = await this.factory.ForRefund(model.UserId, cancellation);

            foreach (var address in await formatter.CreateTo(model))
            {
                model.Content.SendTo.Add(address.Address);
            }

            foreach (var address in await formatter.CreateBcc(model))
            {
                model.Content.BccTo.Add(address.Address);
            }

            model.Content.Subject = await formatter.CreateSubject(model);
            model.Content.SendFrom = (await formatter.SendFrom(model)).Address;
            model.Content.Body = await formatter.CreateBody(refund);
            model.Content.IsHtml = formatter.IsHtml;
            model.ReceiptTemplateName = ReceiptTemplateName.Refund;

            return this.View(model);
        }

        #endregion
    }
}