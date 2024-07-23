using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Models;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;
using DomainModel.JsonNET;
using DomainModel.MvcModels;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order
{
    /// <summary>
    /// Controller for providing NationBuilder order processing.
    /// </summary>
    [Authorize()]
    public class Controller : OrderControllerBase
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller(Sales.DataAccess.DefaultContext context, IEncryptor encryption, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentOutOfRangeException(nameof(encryption), encryption.SupportsSymmetry, $"{nameof(encryption)} must support symmetric encryption");
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.encryption = encryption;
            this.bus = bus;
        }

        #endregion

        #region Action Methods
        
        /// <summary>
        /// Step 1 in the order process. Create a new cart for the order.
        /// </summary>
        public virtual async Task<ActionResult> Start()
        {
            if (!this.User.Identity.IsNationBuilderAccount()) return this.RedirectToAction("Index", "Nation", new {Area = "Profile"});

            var command = new CreateNationBuilderCartCommand();
            command.CartId = Guid.NewGuid();
            command.UserId = this.User.Identity.GetIdentifier();

            await this.bus.SendLocal(command);

            return this.RedirectToAction("Index", "DisplayLists", new {area = "NationBuilder", cartId = command.CartId });
        }

        /// <summary>
        /// Step 2 in the order process. Select the list data.
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public virtual async Task<ActionResult> SelectList(Guid cartId, Int32 listId, String listName, Int32 regId, Int32 recordCount, CancellationToken cancellation)
        {
            var command = new SelectListForCartCommand()
            {
                CartId = cartId,
                ListId = listId,
                RegistrationId = regId,
                ListName = listName,
                RecordCount = recordCount
            };

            await this.bus.SendLocal(command);

            var checkUrl = this.Url.Action(nameof(CartReady), new {cartId});
            var nextUrl = this.Url.Action(nameof(SelectProducts), new { cartId });

            var model = new NewCartModel()
            {
                OrderId = cartId,
                CheckUrl = checkUrl,
                NextUrl = nextUrl
            };

            return this.View(model);
        }

        /// <summary>
        /// Step 2.5 in the order process. Wait for the sales cart.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> CartReady(Guid cartId, CancellationToken cancellation)
        {
            var exists = await this.context
                .SetOf<Sales.Cart>()
                .ForInteractiveUser()
                .ForNationBuilder(cartId)
                .Where(c => c.IsActive)
                .Where(c => c.IntegrationId != null)
                .AnyAsync(cancellation);

            return this.Json(new {Ready = exists}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Step 3 in the order process. Displays product options for a specific list.
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public virtual async Task<ActionResult> SelectProducts(Guid cartId, CancellationToken cancellation)
        {
            // redirect user to lists if not their cart is inactive or does not exist
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var cart = await this.context
                    .SetOf<Sales.Cart>()
                    .ForInteractiveUser()
                    .ForNationBuilder(cartId)
                    .Where(c => c.IsActive)
                    .FirstOrDefaultAsync(cancellation);
                if (cart == null) return this.RedirectToAction("Start");

                var model = new NationBuilderOrderModel
                {
                    RegId = cart.IntegrationId.Value,
                    ListId = cart.ExternalId.Value,
                    ListName = cart.Name,
                    OrderId = cart.Id,
                    RecordCount = cart.RecordCount
                };

                return this.View("DisplayProducts", new OrderProductsViewPresenter()
                {
                    Postback = new MvcActionModel() { AreaName = "NationBuilder", ControllerName = "Order", ActionName = nameof(Confirmation) },
                    Order = model,
                    HelpText = "Results are automatically uploaded to your nation and also available as a separate.csv download so they can be used outside of NationBuilder.",
                    SpecialNotice = String.Empty,
                    OrderViewPath = "~/Areas/NationBuilder/Order/Views/Products.cshtml"
                });
            }
        }

        /// <summary>
        /// Step 4 in the order process. Confirm that list information is valid and if required, require and/or update payment information based on client and then review the quote.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Confirmation([ModelBinder(typeof(FormCollectionJsonBinder))] NationBuilderOrderModel orderModel, CancellationToken cancellation)
        {
            orderModel = orderModel ?? new NationBuilderOrderModel();

            var command = new EnterQuoteForNationBuilderOrderCommand();
            command.CartId = orderModel.OrderId;
            command.OrderMinimum = orderModel.OrderMinimum;
            foreach (var product in orderModel.Products)
            {
                var quote = new ProductQuote();
                quote.Product = product.ProductKey;
                quote.EstimatedMatches = (Int32)Math.Round(product.EstMatches);
                quote.QuotedRate = product.Cost;

                command.Products.Add(quote);
            }

            await this.bus.SendLocal(command);

            #region Valid Payment?

            {
                var view = await this.ForcePaymentScreenIfRequired(this.context, orderModel, null, cancellation);

                if (view != null) return view;
            }

            #endregion

            var model = new OrderConfirmationViewPresenter()
            {
                Postback = new MvcActionModel()
                {
                    AreaName = "NationBuilder",
                    ActionName = nameof(Submit),
                    ControllerName = "Order"
                },
                Order = orderModel
            };

            // display order details

            return this.View(model);
        }

        /// <summary>
        /// Step 5 in the order process. Confirm that valid payment information and then submit.
        /// </summary>
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Submit(
            [ModelBinder(typeof(FormCollectionJsonBinder))] NationBuilderOrderModel orderModel,
            [ModelBinder(typeof(FormCollectionJsonBinder))] PaymentDetailsModel paymentModel,
            CancellationToken cancellation)
        {
            orderModel = orderModel ?? new NationBuilderOrderModel();

            if (!this.TryValidateModel(orderModel)) return this.View("Error"); // probably need to return to prior view
            
            #region Valid Payment?

            {
                var view = await this.ForcePaymentScreenIfRequired(this.context, orderModel, paymentModel, cancellation);

                if (view != null) return view;
            }

            #endregion

            using (new Correlation(orderModel.OrderId))
            { 
                try
                {

                    #region Submit Job

                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (paymentModel != null)
                        {
                            await this.UpdatePayment(this.context, this.bus, paymentModel, cancellation);
                        }

                        var command = new SubmitNationBuilderOrderCommand()
                        {
                            CartId = orderModel.OrderId
                        };

                        await this.bus.SendLocal(command);

                        transaction.Complete();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, TraceEventType.Warning, Severity.High, Application.Clients.ToString(), this.Request.UserHostAddress, $"NationBuilder order exception {this.User.Identity.Name}");
                    return this.View("Error");
                }
            }

            return this.RedirectToAction("OrderReceived",
                new
                {
                    Area = "NationBuilder",
                    orderModel.RegId,
                    orderModel.ListId,
                    orderModel.ListName
                });
        }
        
        /// <summary>
        /// Informational view displayed after an order is submitted.
        /// </summary>
        /// <remarks>
        /// Called from JavaScript in order page. View calls Google Analytics with revenue data.
        /// </remarks>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult OrderReceived([ModelBinder(typeof(FormCollectionJsonBinder))] NationBuilderOrderModel selectedList)
        {
            return this.View(selectedList);
        }

        /// <summary>
        /// Informational view displayed after an order is submitted. View calls Google Analytics with revenue data.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Called from inside method on PaymentController
        /// </remarks>
        public virtual ActionResult OrderReceived(Int32 listId, Int32 regId, String listName)
        {
            var selectedList = new NationBuilderOrderModel();
            selectedList.ListId = listId;
            selectedList.RegId = regId;
            selectedList.ListName = listName;

            return this.View(selectedList);
        }

        #endregion

        #region Helper Methods

        protected override String EncryptPayload(String secureThisValue)
        {
            return this.encryption.SymetricEncrypt(secureThisValue);
        }

        protected override OrderPaymentViewPresenter CreatePaymentViewPresenter(NewOrderModel orderModel, PaymentDetailsModel paymentModel)
        {
            var model = new OrderPaymentViewPresenter()
            {
                PostBack = new MvcActionModel()
                {
                    AreaName = "NationBuilder",
                    ActionName = nameof(this.Submit),
                    ControllerName = "Order"
                },
                StartOver = new MvcActionModel()
                {
                    AreaName = "NationBuilder",
                    ActionName = nameof(DisplayLists.Controller.Index),
                    ControllerName = "DisplayLists"
                },
                Order = orderModel,
                PaymentDetails = paymentModel ??
                    new PaymentDetailsModel()
                    {
                        ApplicationId = this.User.Identity.GetApplication(),
                        UserId = this.User.Identity.GetIdentifier()
                    }
            };
            return model;
        }

        #endregion
    }
}