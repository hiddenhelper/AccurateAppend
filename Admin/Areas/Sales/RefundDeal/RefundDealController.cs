using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.Services;

namespace AccurateAppend.Websites.Admin.Areas.Sales.RefundDeal
{
    /// <summary>
    /// Controller for processing refunds on completed orders.
    /// </summary>
    /// <summary>
    /// Controller responsible for allowing a <see cref="RefundOrder"/> to have an amount to be refunded
    /// and to be processed via the billing service.
    ///
    /// Step 1- Display order edit details
    /// Step 2- Craft a refund notification
    /// Step 3- Wait for Transaction result to post
    /// Step 4- Display result details to end user
    /// </summary>
    [Authorize()]
    public class RefundDealController : ActivityLoggingController2
    {
        #region Fields

        private readonly DealRefundService service;
        private readonly IBillFormatterFactory factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundDealController"/> class.
        /// </summary>
        // <param name="context">The <see cref="ISessionContext"/> used for the controller instance.</param>
        public RefundDealController(DealRefundService service, IBillFormatterFactory factory)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            Contract.EndContractBlock();

            this.service = service;
            this.factory = factory;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display completed deal order items that have a total value > 0
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 orderId, CancellationToken cancellation)
        {
            var order = await this.service.FindRefund(orderId, cancellation);
            if (order == null) return this.DisplayErrorResult($"Refund {orderId} does not exist.");

            return this.View(order);
        }

        /// <summary>
        /// Performs the action to persist an order refund after editing.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(RefundDetail order, CancellationToken cancellation)
        {
            order = order ?? new RefundDetail();

            if (!this.ModelState.IsValid) return this.View(order);

            try
            {
                var model = new RefundDetail();
                model.OrderMinimum = order.OrderMinimum;
                model.Id = order.Id;
                model.UserId = order.UserId;
                model.PublicKey = order.PublicKey;
                model.Items.AddRange(
                    order.Items.Select(
                        i => new OrderItemModel()
                        {
                            Cost = i.Cost,
                            Description = i.Description,
                            Id = i.Id,
                            Maximum = i.Maximum,
                            ProductName = i.ProductName,
                            Quantity = i.Quantity,
                            UserId = i.UserId
                        }));

                await this.service.Update(model, cancellation);

                this.OnEvent($"Deal {order.Id} edited");
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();

                return this.DisplayErrorResult(ex.Message);
            }

            return this.RedirectToAction("Draft", "EditRefund", new {area = "Sales", orderId = order.Id});
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Process(CreateBill.Models.BillViewModel model, String to, String bcc, CancellationToken cancellation)
        {
            var refund = await this.service.FindRefund(model.OrderId, cancellation);

            if (refund == null) return this.DisplayErrorResult($"Deal {model.DealId} does not have an open refund order currently in process.");

            if (model.Content.IsHtml)
            {
                var formatter = await this.factory.ForSubscription(model.UserId, cancellation);
                var header = await formatter.CreateHeader(model);
                var footer = await formatter.CreateFooter(model);

                model.Content.Body = $"{header}{System.Web.HttpUtility.HtmlDecode(model.Content.Body)}{footer}";
            }

            foreach (var recipient in to.Split(',').Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            {
                model.Content.SendTo.Add(recipient);
            }
            foreach (var recipient in bcc.Split(',').Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            {
                model.Content.BccTo.Add(recipient);
            }
            
            var content = new RefundViewModel();
            content.Id = model.DealId;
            content.UserId = model.UserId;
            content.Subject = model.Content.Subject;
            content.Body = model.Content.Body;
            content.IsHtml = model.Content.IsHtml;
            content.SendFrom = model.Content.SendFrom;
            content.SendTo.AddRange(model.Content.SendTo);
            content.BccTo.AddRange(model.Content.BccTo);
            
            await this.service.Draft(content, cancellation);

            return this.RedirectToAction("Processing", new {Area="sales", refund.PublicKey});
        }

        /// <summary>
        /// View to wait on the results of the refund billing request.
        /// </summary>
        /// <param name="publicKey">The public key identifier relating refund to order transactions.</param>
        [HttpGet()]
        public ActionResult Processing(Guid publicKey)
        {
            return this.View(publicKey);
        }

        #endregion
    }
}
