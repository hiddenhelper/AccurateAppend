using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail.Models;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Templates;
using DomainModel.Html;

namespace AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail
{
    [Authorize()]
    public class OrderDetailController : ActivityLoggingController2
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IPdfGenerator generator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="OrderDetailController"/> class.
        /// </summary>
        /// <param name="context">The <seealso cref="DefaultContext"/> to use for data access.</param>
        /// <param name="generator">The <see cref="IPdfGenerator"/> component used for PDF creation.</param>
        public OrderDetailController(DefaultContext context, IPdfGenerator generator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            Contract.EndContractBlock();

            this.context = context;
            this.generator = generator;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Performs the action to display details of an order
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(Int32 orderId, CancellationToken cancellation)
        {
            this.OnEvent($"Order viewed: {orderId}");

            var order = await this.Query(orderId, cancellation);

            if (order == null)
            {
                this.TempData["message"] = "The order does not exist ";
                return this.View("~/Views/Shared/Error.aspx");
            }

            return this.View(order);
        }

        /// <summary>
        /// Performs the action to display details of an order in a printer friendly format.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> SaveAsPdf(Int32 orderId, CancellationToken cancellation)
        {
            this.OnEvent($"Order downloaded as pdf: {orderId}");

            var order = await this.context
                .SetOf<Order>()
                .Where(o => o.Id == orderId)
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .Include(p => p.Lines)
                .Include(o => o.Lines.Select(l => l.Product))
                .SingleOrDefaultAsync(cancellation);
            
            var html = await OrderDetailHtml.OrderDetail(order);
            var data = this.generator.FromHtml(html);

            return this.File(data, MediaTypeNames.Application.Pdf, $"Accurate Append - Order {orderId}, {order.Deal.Client.UserName.ToLower()}.pdf");
        }

        #endregion
        
        #region Helpers

        /// <summary>
        /// Centralizes logic for the order query.
        /// </summary>
        protected virtual Task<OrderDetailView> Query(Int32 orderId, CancellationToken cancellation)
        {
            return this.context
                .SetOf<Order>()
                .Where(o => o.Id == orderId)
                .Select(
                    o => new OrderDetailView()
                    {
                        OrderId = orderId,
                        UserId = o.Deal.Client.UserId
                    }
                ).FirstOrDefaultAsync(cancellation);
        }

        #endregion
    }
}