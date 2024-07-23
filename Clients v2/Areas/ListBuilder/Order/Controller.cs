using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Security;
using AccurateAppend.Websites.Clients.Areas.ListBuilder.Order.Models;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;
using DomainModel.JsonNET;
using DomainModel.MvcModels;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using OrderConfirmationViewPresenter = AccurateAppend.Websites.Clients.Areas.ListBuilder.Order.Models.OrderConfirmationViewPresenter;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Order
{
    [Authorize()]
    public class Controller : OrderControllerBase
    {
        #region Fields

        private readonly ISessionContext context;
        private readonly StandardFileLocations files;
        private readonly IMessageSession bus;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller(Sales.DataAccess.DefaultContext context, StandardFileLocations files, IEncryptor encryption, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentOutOfRangeException(nameof(encryption), encryption.SupportsSymmetry, $"{nameof(encryption)} must support symmetric encryption");
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.files = files;
            this.bus = bus;
            this.encryption = encryption;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Step 1 in the order process: Start job from previously generated ListBuilder file and create new request
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FromListBuilder(Guid id)
        {
            var model = new GenerateListModel()
            {
                PublicKey = id,
                CheckUrl = this.Url.Action("CheckFileStatus", "BuildList", new { area = "ListBuilder", id }),
                NextUrl = this.Url.Action(nameof(this.SelectProducts), new {id})
            };

            return this.View(model);
        }

        /// <summary>
        /// Step 2 in the order process: Handle the redirect from the storage application and display product selection cart.
        /// </summary>
        public virtual async Task<ActionResult> SelectProducts(CancellationToken cancellation, Guid id)
        {
            var identifier = id;
            var customerFileName = id.ToString(); // customer should be able to name list
            var systemFileName = id.ToString();
            var file = this.files.Temp.CreateInstance(systemFileName);

            if (!file.Exists()) return this.DisplayErrorResult($"No list builder file with id {id} exists");

            using (new Correlation(id)) 
            {
                try
                {
                    // Kendo upload control validates the file extension so we shouldn't hit this but defensive programming
                    if (!JobPipeline.IsSupported(file))
                    {
                        Logger.LogEvent($"File type is not supported: {file}", Severity.Low, Application.Clients);
                        return this.View("Error");
                    }

                    var csvFile = new Plugin.Storage.CsvFile(file);
                    var recordCount = csvFile.CountRecords();

                    using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                    {
                        var client = await this.context.SetOf<Sales.ClientRef>().ForInteractiveUser().SingleAsync(cancellation);
                        var cart = await this.context.SetOf<Sales.Cart>().SingleOrDefaultAsync(c => c.Id == identifier, cancellation);
                        if (cart == null)
                        {
                            cart = Sales.Cart.ForListbuilder(client, identifier, customerFileName, recordCount);
                            this.context.SetOf<Sales.Cart>().Add(cart);
                        }

                        // is this cart expired?
                        if (!cart.IsActive)
                        {
                            Logger.LogEvent($"Cart {identifier} is expired", Severity.Low, Application.Clients);
                            return this.View("Error");
                        }

                        await uow.CommitAsync(cancellation);
                    }

                    var model = new GeneratedFileOrderModel
                    {
                        ListName = customerFileName,
                        SystemFileName = systemFileName,
                        OrderId = identifier,
                        RecordCount = recordCount
                    };

                    return this.View("DisplayProducts", new OrderProductsViewPresenter()
                    {
                        Postback = new MvcActionModel() { AreaName = "ListBuilder", ControllerName = "Order", ActionName = nameof(this.Confirmation) },
                        Order = model,
                        HelpText = "Please note that estimated match rates are based on historical averages of lists processed that contain: first name, last name, (full name may be used in place of first name and last name), street address, city, state (province), and zip code.",
                        OrderViewPath = "~/Areas/ListBuilder/Order/Views/Products.cshtml"
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.High, Application.Clients, description: nameof(this.SelectProducts));
                    throw;
                }
            }
        }

        /// <summary>
        /// Step 3 in the order process. Confirm order before submitting
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Confirmation([ModelBinder(typeof(FormCollectionJsonBinder))] GeneratedFileOrderModel orderModel, CancellationToken cancellation)
        {
            orderModel = orderModel ?? new GeneratedFileOrderModel();

            #region Valid Payment?

            {
                var view = await this.ForcePaymentScreenIfRequired(this.context, orderModel, null, cancellation);

                if (view != null) return view;
            }

            #endregion

            var model = new OrderConfirmationViewPresenter() { Postback = new MvcActionModel() { AreaName = "ListBuilder", ActionName = "Submit", ControllerName = "Order" }, Order = orderModel };

            // display order details

            return this.View(model);
        }

        /// <summary>
        /// Step 4 in the order process. If required, require and/or update payment information based on client and then submit the job.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Submit(
            [ModelBinder(typeof(FormCollectionJsonBinder))] GeneratedFileOrderModel orderModel,
            [ModelBinder(typeof(FormCollectionJsonBinder))] PaymentDetailsModel paymentModel,
            CancellationToken cancellation)
        {
            orderModel = orderModel ?? new GeneratedFileOrderModel();

            //if (!this.TryValidateModel(orderModel)) return await this.IdentifyColumns(orderModel, cancellation);

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

                        await this.SubmitJob(orderModel, cancellation);

                        transaction.Complete();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, TraceEventType.Warning, Severity.High, Application.Clients.ToString(), this.Request.UserHostAddress, $"ListBuilder order unhanded exception {this.User.Identity.Name}");
                    return this.View("Error");
                }
            }

            return this.RedirectToAction("Thankyou"); // show the order submitted
        }

        /// <summary>
        /// Step 5 in the order process.
        /// </summary>
        /// <returns></returns>
        public ActionResult Thankyou()
        {
            return this.View();
        }

        #endregion

        #region Helper Methods

        protected virtual async Task SubmitJob(GeneratedFileOrderModel order, CancellationToken cancellation)
        {
            var filename = $"List Built: {Path.GetFileName(order.ListName)}";
            var clientFile = this.files.Temp.CreateInstance(order.SystemFileName);

            #region Copy this into Inbox

            await JobPipeline.BackupClientInputFileAsync(clientFile, this.files.RawCustomerFiles, order.OrderId, cancellation).ConfigureAwait(false);

            var inboxFile = this.files.Inbox.CreateInstance(order.OrderId.ToString());
            await clientFile.CopyToAsync(inboxFile, cancellation);

            #endregion

            #region Create Manifest

            var products = order.Products.Select(p => p.ProductKey).Normalize().Select(p => p.Convert()).ToArray();

            var manifest = new ManifestBuilder();
            foreach (var product in products)
            {
                var operation = OperationDefinition.LoadFromDefinition(product);
                manifest.Operations.Add(operation);
            }
            manifest.ColumnMap = new ColumnMap($"{FieldName.FirstName};{FieldName.Unknown};{FieldName.LastName};{FieldName.StreetAddress};{FieldName.City};{FieldName.State};{FieldName.PostalCode}");
            manifest.HasHeaderRow = true;

            var m = manifest.ToXml();
            m.InputDelimiter(CsvFileContent.DefaultDelimiter);
            m.OutputDelimiter(CsvFileContent.DefaultDelimiter);

            #endregion

            #region Submit the Job

            var inputFile = new JobInputFile(new CsvFile(inboxFile) { Delimiter = CsvFileContent.DefaultDelimiter }, filename);

            using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
            {
                var cart = await this.context.SetOf<Sales.Cart>().SingleAsync(c => c.Id == order.OrderId, cancellation).ConfigureAwait(false);
                var po = cart.Complete();

                this.context.SetOf<Sales.ProductOrder>().Add(po);

                var user = await this.context.SetOf<User>().InteractiveUser().Include(u => u.Application).FirstAsync(cancellation);

                var job = new ListbuilderJob(user, m, inputFile);

                if (Debugger.IsAttached) Debugger.Break(); // Skip next lines if you REALLLY don't want to submit
                this.context.SetOf<Job>().Add(job);

                await uow.CommitAsync(cancellation);
            }

            #endregion
        }

        protected override String EncryptPayload(String secureThisValue)
        {
            return this.encryption.SymetricEncrypt(secureThisValue);
        }

        protected override OrderPaymentViewPresenter CreatePaymentViewPresenter(NewOrderModel orderModel, PaymentDetailsModel paymentModel)
        {
            return new OrderPaymentViewPresenter()
            {
                PostBack = new MvcActionModel()
                {
                    AreaName = "ListBuilder",
                    ActionName = nameof(this.Submit),
                    ControllerName = "Order"
                },
                StartOver = new MvcActionModel()
                {
                    AreaName = "ListBuilder",
                    ActionName = nameof(this.FromListBuilder),
                    ControllerName = "Order"
                },
                Order = orderModel,
                PaymentDetails = paymentModel ??
                                 new PaymentDetailsModel()
                                 {
                                     ApplicationId = this.User.Identity.GetApplication(),
                                     UserId = this.User.Identity.GetIdentifier()
                                 }
            };
        }

        #endregion
    }
}