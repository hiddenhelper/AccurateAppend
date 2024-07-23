using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Websites.Admin.Areas.Sales.EditOrder.Models;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;

namespace AccurateAppend.Websites.Admin.Areas.Sales.EditOrder
{
    /// <summary>
    /// Controller to edit an <see cref="Order"/> item.
    /// </summary>
    [Authorize()]
    public class EditOrderController : ActivityLoggingController2
    {
        #region Fields

        private readonly IOrderManagementService service;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditOrderController"/> class.
        /// </summary>
        /// <param name="service">The <see cref="IOrderManagementService"/> that provides deal and order content management logic.</param>
        public EditOrderController(IOrderManagementService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            this.service = service;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Performs the action to render the edit form.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(Int32 orderId, CancellationToken cancellation)
        {
            try
            {
                var order = await this.service.FindOrder(orderId, cancellation);
                if (order == null)
                {
                    this.TempData["message"] = $"Order: {orderId} does not exist";
                    return this.View("~/Views/Shared/Error.aspx");
                }

                var model = new EditViewModel();
                model.Id = orderId;
                model.CanUpdateFromJob = false;
                model.UserId = order.UserId;
                model.OrderMinimum = order.OrderMinimum;
                model.PublicKey = order.PublicKey;

                foreach (var item in order.Items)
                {
                    model.Items.Add(item);
                }

                return this.View(model);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();

                this.TempData["message"] = ex.Message;
                return this.View("~/Views/Shared/Error.aspx");
            }
        }

        /// <summary>
        /// Performs the action to persist an order after editing.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Index(EditViewModel order, CancellationToken cancellation)
        {
            order = order ?? new EditViewModel();

            if (!this.ModelState.IsValid) return this.View(order);

            try
            {
                var model = new AccurateAppend.Sales.Contracts.ViewModels.OrderDetail();
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

                this.TempData["message"] = ex.Message;
                return this.View("~/Views/Shared/Error.aspx");
            }

            return this.NavigationFor<OrderDetailController>().Detail(order.Id);
        }

        /// <summary>
        /// Action to render an order line row.
        /// </summary>
        public virtual ActionResult OrderItemRow(Guid userId)
        {
            return this.View(new OrderItemModel() { UserId = userId });
        }

        #endregion
    }
}