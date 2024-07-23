using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Automation.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Automation.Models;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Shared.Models;
using DomainModel;
using DomainModel.ActionResults;
using DomainModel.JsonNET;
using DomainModel.MvcModels;
using EventLogger;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation
{
    /// <summary>
    /// Controller to support client automation sales file processing.
    /// </summary>
    [Authorize()]
    [ValidateInput(false)]
    public class AutomationController : Controller
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext context;
        private readonly IFileLocation tempFolder;
        private readonly IFileLocation rawCustomerFiles;
        private readonly IEncryptor encryption;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationController"/> class.
        /// </summary>
        public AutomationController(Sales.DataAccess.DefaultContext context, StandardFileLocations files, IEncryptor encryption, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentOutOfRangeException(nameof(encryption), encryption.SupportsSymmetry, $"{nameof(encryption)} must support symmetric encryption");
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.tempFolder = files.Temp;
            this.rawCustomerFiles = files.RawCustomerFiles;
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
            if (!this.User.Identity.HasAutoProcessingRules()) return this.RedirectToAction("Index", "Current", new {Area = "Order"});

            var command = new CreateAutomationCartCommand
            {
                CartId = Guid.NewGuid(),
                UserId = this.User.Identity.GetIdentifier()
            };

            await this.bus.SendLocal(command);

            return this.RedirectToAction(nameof(this.SelectRule), new { cartId = command.CartId });
        }

        /// <summary>
        /// Step 2.a in the order process. Display the list of automation definitions for the interactive user.
        /// </summary>
        /// <remarks>
        /// While this view is sent the the client we're able to allow the cart saga to spin up asynchronously.
        /// We've got the shared cart identifier to relate with the Automation Rule selection which can happen in parallel.
        /// </remarks>
        public virtual ActionResult SelectRule(Guid cartId)
        {
            var selectUrl = this.Url.Action(nameof(SelectRule), new { cartId });
            var nextUrl = this.Url.Action(nameof(Upload), new { cartId });

            var model = new DisplayAutomationsModel
            {
                CartId = cartId,
                QueryUrl = this.Url.Action("ForCurrentUser", "AutomationRules", new {Area = "JobProcessing"}),
                SelectUrl = selectUrl,
                NextUrl = nextUrl
            };

            return this.View(model);
        }

        /// <summary>
        /// Step 2.b in the order process. Enter the selected automation definitions for the order.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> SelectRule(Guid cartId, String manifestContent)
        {
            var manifest = XElement.Parse(manifestContent);
            var command = new SelectManifestForCartCommand()
            {
                CartId = cartId,
                Manifest = manifest
            };

            await this.bus.SendLocal(command);

            return new JsonNetResult() {Data = new {Status = (Int32) HttpStatusCode.OK}};
        }

        /// <summary>
        /// Step 2.c in the order process. Wait for the sales cart.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> CartReady(Guid cartId, CancellationToken cancellation)
        {
            var exists = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .ForCsv(cartId)
                .Where(c => c.IsActive)
                .Where(c => c.ManifestId != null) // Ensures we've got the Manifest entered
                .AnyAsync(cancellation);

            return this.Json(new { Ready = exists }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Step 3.a in the order process. Craft a request to the storage application using a bearer token.
        /// </summary>
        public virtual ActionResult Upload(Guid cartId)
        {
            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif
            var request = new UploadRequest(cartId, this.Url.Action(nameof(this.ProcessFile), "Automation", new { Area = "Order" }, scheme))
            {
                ConvertToCsv = true
            };

            Logger.LogEvent($"Client {this.User.Identity.Name} started Automation order {request.Identifier}", Severity.None, Application.Clients);

            var model = new UploadRequestModel();

            var uri = request.CreateRequest(this.encryption);
#if DEBUG
            //uri = request.CreateRequest(this.encryption, UploadRequest.Local); // Uncomment to switch to a local VS instance of the STORAGE app
#endif
            model.UploadTo = uri;

            return this.View(model);
        }

        public virtual async Task<ActionResult> ProcessFile(CancellationToken cancellation)
        {
            var result = UploadResult.HandleFromPostback(this.Request.QueryString, this.encryption);
            var identifier = result.Identifier;
            var systemFileName = result.SystemFileName;
            var clientFileName = JobPipeline.CleanFileName(result.ClientFileName);

            using (new Correlation(identifier))
            {
                var rawFile = this.tempFolder.CreateInstance(systemFileName);

                // Kendo upload control validates the file extension so we shouldn't hit this but defensive programming
                if (!JobPipeline.IsSupported(rawFile))
                {
                    Logger.LogEvent($"File type is not supported: {rawFile}", Severity.Low, Application.Clients);
                    return this.DisplayErrorResult($"File type is not supported: {Path.GetExtension(rawFile.Name)}");
                }

                // Handle Zips
                if (JobPipeline.IsArchive(rawFile))
                {
                    rawFile = (await JobPipeline.HandleZip(rawFile, identifier, this.tempFolder, this.rawCustomerFiles, cancellation)).First();
                    clientFileName = JobPipeline.CleanFileName(rawFile.Name);

                    rawFile = await rawFile.Rename(Path.ChangeExtension(systemFileName, Path.GetExtension(rawFile.Name)), this.tempFolder, cancellation);
                    systemFileName = rawFile.Name;
                }

                // Handle Excel
                if (JobPipeline.IsExcel(rawFile))
                {
                    var excelFile = new ExcelFile(ExcelFile.DetermineExcelFormat(rawFile), rawFile);
                    rawFile = await JobPipeline.HandleExcel(excelFile, identifier, this.tempFolder, this.rawCustomerFiles, cancellation);

                    clientFileName = rawFile.Name;
                    rawFile = await rawFile.Rename(Path.ChangeExtension(systemFileName, Path.GetExtension(rawFile.Name)), this.tempFolder, cancellation);
                    systemFileName = rawFile.Name;
                }

                var cart = await this.context
                    .SetOf<Cart>()
                    .ForInteractiveUser()
                    .ForCsv(identifier)
                    .Where(c => c.IsActive)
                    .Include(c => c.Client)
                    .FirstOrDefaultAsync(cancellation);

                if (cart == null)
                {
                    Logger.LogEvent($"This cart {identifier} does not exist", Severity.Low, Application.Clients);
                    return this.DisplayErrorResult($"We're sorry but we're having trouble with processing your your order right now. Please contact support and provide this error id: '{identifier}'.");
                }

                if (!cart.IsActive)
                {
                    Logger.LogEvent($"Cart {identifier} is expired", Severity.Low, Application.Clients);
                    return this.DisplayErrorResult($"We're sorry but your order: '{identifier}' is expired or has already been completed.");
                }

                var @event = new FileUploadedEvent()
                {
                    CartId = cart.Id,
                    CustomerFileName = clientFileName,
                    RecordCount = cart.RecordCount,
                    UserId = this.User.Identity.GetIdentifier()
                };

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await this.bus.Publish(@event);
                    await this.context.SaveChangesAsync(cancellation);

                    transaction.Complete();
                }

                return this.View(new ProcessFileModel()
                {
                    CartId = identifier,
                    SubmitUrl = this.Url.Action("AnalyzeFile", new { cartId = identifier, systemFileName, clientFileName }),
                    NextUrl = this.Url.Action("IdentifyColumns", new { cartId = identifier })
                });
            }
        }

        /// <summary>
        /// Step 3.b in the order process. Handle the redirect from the storage application and start analysis.
        /// </summary>
        public virtual async Task<ActionResult> AnalyzeFile(Guid cartId, String systemFileName, String clientFileName, Guid requestId, CancellationToken cancellation)
        {
            using (new Correlation(cartId))
            {
                try
                {
                    var command = new AnalyzeManifestCsvFileCommand
                    {
                        CartId = cartId,
                        SystemFileName = systemFileName,
                        RequestId = requestId,
                        ClientFileName = clientFileName
                    };

                    await this.bus.SendLocal(command);

                    return this.Json(new { Status = (Int32)HttpStatusCode.Accepted });
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.High, nameof(this.AnalyzeFile));
                    throw;
                }
            }
        }

        /// <summary>
        /// Step 4.a in the order process. Based on the selected products, allow the client to map input columns to our product fields.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> IdentifyColumns(Guid cartId, CancellationToken cancellation)
        {
            var cart = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .ForCsv(cartId)
                .FirstOrDefaultAsync(cancellation);

            if (cart?.Quote == null || cart?.Manifest == null)
            {
                // Spin wait due to race condition
                await Task.Delay(TimeSpan.FromSeconds(2), cancellation);
                return this.RedirectToAction(nameof(IdentifyColumns), new {cartId});
            }

            if (!cart.IsActive)
            {
                Logger.LogEvent($"Cart {cartId} is expired", Severity.Low, Application.Clients);
                return this.DisplayErrorResult($"We're sorry but your order: '{cartId}' is expired or has already been completed.");
            }

            var model = new MapColumnsModel();
            model.CartId = cartId;
            model.Postback = new MvcActionModel() { ActionName = "IdentifyColumns", AreaName = "Order", ControllerName = "Automation" };

            #region Read Top Rows

            var rawFile = this.tempFolder.CreateInstance(cart.SystemFileName);

            var manifest = new ManifestBuilder(cart.Manifest);
            var sample = await FileSampler.Perform(rawFile, manifest, cancellation: cancellation);

            model.HasHeaderRow = sample.HasHeaderRow;
            model.AutomappedFields.AddRange(sample.AutomappedFields);

            foreach (var item in sample.ColumnSamples)
            {
                model.ColumnSamples.Add(item.Key, item.Value.ToList());
            }

            #endregion

            // collection of fields is built into select > options in View
            model.InputFields = manifest.DetermineRequiredInputFields().Where(a => !a.MetaFieldName.StartsWith("_")).ToList();  // fields with underscore should not be mapped
            // collection needs to be serialized because it's consumed as a JavaScript array in the view
            var serializer = new JavaScriptSerializer();
            model.RequiredFields = serializer.Serialize(manifest.DetermineRequiredInputFields().Where(a => a.Required).Select(a => a.MetaFieldName));

            return this.View(model);
        }

        /// <summary>
        /// Step 4.b in the order process. Store the client mapped input columns.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> IdentifyColumns([ModelBinder(typeof(FormCollectionJsonBinder))] MappedColumnsModel orderModel, CancellationToken cancellation)
        {
            orderModel = orderModel ?? new MappedColumnsModel();
            var cartId = orderModel.CartId;

            var cart = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .ForCsv(cartId)
                .FirstAsync(cancellation);

            cart.Manifest.ColumnMap(orderModel.ColumnMap);
            cart.Manifest.HasHeaderRow(orderModel.HasHeaderRow);

            await this.context.SaveChangesAsync(cancellation);

            return this.RedirectToAction("Confirmation", "Automation", new {Area = "Order", cartId});
        }

        /// <summary>
        /// Confirm that mapped columns are present and if required, require and/or update payment information based on client and then review the quote.
        /// </summary>
        [HttpGet()]
        public virtual ActionResult Confirmation(Guid cartId)
        {
            var model = new OrderConfirmationViewPresenter()
            {
                Postback = new MvcActionModel()
                {
                    AreaName = "Order",
                    ActionName = "Submit",
                    ControllerName = "Automation"
                },
                CartId = cartId,
                WalletUrl = this.Url.Action("PrimaryCard", "WalletApi", new { Area = "Profile" }),
                DataUrl = this.Url.Action("Quote", "CartApi", new { Area = "Order" }),
                PaymentDetails = new Profile.Card.Models.PaymentDetailsModel()
            };

            return this.View(model);
        }

        public virtual async Task<ActionResult> Submit(Guid cartId, Profile.Card.Models.PaymentDetailsModel paymentDetails, CancellationToken cancellation)
        {
            var cart = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .ForCsv(cartId)
                .FirstAsync(cancellation);

            if (cart == null)
            {
                Logger.LogEvent($"This cart {cartId} does not exist", Severity.Low, Application.Clients);
                return this.DisplayErrorResult($"We're sorry but we're having trouble with processing your your order right now. Please contact support and provide this error id: '{cartId}'.");
            }

            if (!cart.IsActive)
            {
                Logger.LogEvent($"Cart {cartId} is expired", Severity.Low, Application.Clients);
                return this.DisplayErrorResult($"We're sorry but your order: '{cartId}' is expired or has already been completed.");
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                #region Payment

                if (await this.RequiresPayment(cancellation))
                {
                    paymentDetails = paymentDetails ?? new Profile.Card.Models.PaymentDetailsModel();

                    if (!this.ModelState.IsValid) return this.RedirectToAction("Confirmation", "Automation", new {cartId});

                    var address = new BillingAddressPayload();
                    address.FirstName = paymentDetails.CardHolderFirstName;
                    address.LastName = paymentDetails.CardHolderLastName;
                    address.BusinessName = paymentDetails.CardHolderBusinessName;
                    address.PostalCode = paymentDetails.CardPostalCode;
                    address.PhoneNumber = paymentDetails.CardHolderPhone;

                    var card = new CreditCardPayload(this.encryption.SymetricEncrypt(paymentDetails.CardNumber), paymentDetails.GetExpirationDate(), paymentDetails.CardCvv);

                    var command = new CreatePaymentProfileCommand
                    {
                        UserId = this.User.Identity.GetIdentifier(),
                        Card = card,
                        BillingAddress = address,
                        RequestId = Guid.NewGuid()
                    };

                    await this.bus.Send(command);
                }

                #endregion

                #region Submit

                {
                    var command = new SubmitAutomationOrderCommand {CartId = cartId};

                    await this.bus.SendLocal(command);
                }

                #endregion

                transaction.Complete();
            }

            return this.RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou()
        {
            return this.View("~/Areas/Order/Shared/Views/ThankYou.cshtml");
        }

        #endregion

        #region Helper Methods

        protected virtual async Task<Boolean> RequiresPayment(CancellationToken cancellation)
        {
            var accounts = await context
                .SetOf<CreditCardRef>()
                .CardsForInteractiveUserAsync()
                .AsNoTracking()
                .OrderByDescending(c => c.IsPrimary)
                .ThenByDescending(c => c.Id)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);

            if (!accounts.Any()) return true;

            return !accounts.Any(c => c.IsValid());
        }

        #endregion
    }
}