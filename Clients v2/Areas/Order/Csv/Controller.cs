using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.ColumnMapper.Tests;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Messages;
using AccurateAppend.Websites.Clients.Areas.Order.Csv.Models;
using AccurateAppend.Websites.Clients.Areas.Order.Shared.Models;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;
using DomainModel;
using DomainModel.JsonNET;
using DomainModel.MvcModels;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv
{
    /// <summary>
    /// Controller to support client uploaded file processing.
    /// </summary>
    [Authorize()]
    [ValidateInput(false)]
    public class CsvController : Controller
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
        /// Initializes a new instance of the <see cref="CsvController"/> class.
        /// </summary>
        public CsvController(Sales.DataAccess.DefaultContext context, StandardFileLocations files, IEncryptor encryption, IMessageSession bus)
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
            var command = new CreateCsvCartCommand
            {
                CartId = Guid.NewGuid(),
                UserId = this.User.Identity.GetIdentifier()
            };

            await this.bus.SendLocal(command);

            return this.RedirectToAction(nameof(this.Upload), new {cartId = command.CartId});
        }

        /// <summary>
        /// Step 2.a in the order process. Craft a request to the storage application using a bearer token.
        /// </summary>
        /// <remarks>
        /// While this view is sent the the client we're able to allow the cart saga to spin up asynchronously.
        /// We've got the shared cart identifier to relate with the upload which can happen in parallel.
        /// </remarks>
        public virtual ActionResult Upload(Guid cartId)
        {
            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif
            var request = new UploadRequest(cartId, this.Url.Action(nameof(this.ProcessFile), "Csv", new { Area = "Order" }, scheme))
            {
                ConvertToCsv = true
            };

            Logger.LogEvent($"Client {this.User.Identity.Name} started CSV order {request.Identifier}", Severity.None, Application.Clients);

            var model = new UploadRequestModel();

            var uri = request.CreateRequest(this.encryption);
#if DEBUG
            //uri = request.CreateRequest(this.encryption, UploadRequest.Local); // Uncomment to switch to a local VS instance of the STORAGE app           
#endif

            model.UploadTo = uri;
            var redirectTo = this.Url.Action("SelectList", "Box", new {Area = "Order", cartId});
            model.BoxUpload = new Uri(this.Url.Action("Initiate", "AuthHandler", new {Area = "Box", redirectTo, publicKey = cartId}), UriKind.RelativeOrAbsolute);
            
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

                var recordCount = new CsvFile(rawFile).CountRecords();

                var cart = await this.context
                    .SetOf<Sales.Cart>()
                    .ForInteractiveUser()
                    .ForCsv(identifier)
                    .Where(c => c.IsActive)
                    .Include(c => c.Client)
                    .FirstOrDefaultAsync(cancellation);

                if (cart == null)
                {
                    // Race condition. Enter spin wait
                    Logger.LogEvent($"This cart {identifier} does not exist", Severity.High, Application.Clients);
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellation);

                    return this.Redirect(this.Request.Url.ToString());
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
                    RecordCount = recordCount,
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
                    SubmitUrl = this.Url.Action(nameof(this.AnalyzeFile), new {cartId = identifier, systemFileName, clientFileName}),
                    NextUrl = this.Url.Action(nameof(this.SelectProducts), new {cartId = identifier})
                });
            }
        }

        /// <summary>
        /// Step 2.b in the order process. Handle the redirect from the storage application and start analysis.
        /// </summary>
        public virtual async Task<ActionResult> AnalyzeFile(Guid cartId, String systemFileName, String clientFileName, Guid requestId, CancellationToken cancellation)
        {
            using (new Correlation(cartId))
            {
                try
                {
                    var command = new AnalyzeRawCsvFileCommand
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
        /// Step 3.a in the order process. Displays product options for a specific list.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> SelectProducts(Guid cartId, CancellationToken cancellation)
        {
            var identifier = cartId;

            using (new Correlation(identifier))
            {
                try
                {
                    var cart = await this.context
                        .SetOf<Sales.Cart>()
                        .ForInteractiveUser()
                        .ForCsv(identifier)
                        .Where(c => c.IsActive)
                        .Include(c => c.Client)
                        .FirstOrDefaultAsync(cancellation);

                    if (cart == null)
                    {
                        Logger.LogEvent($"This cart {cartId} does not exist", Severity.Low, Application.Clients);
                        return this.DisplayErrorResult($"We're sorry but we're having trouble with processing your your order right now. Please contact support and provide this error id: '{identifier}'.");
                    }

                    if (!cart.IsActive)
                    {
                        Logger.LogEvent($"Cart {identifier} is expired", Severity.Low, Application.Clients);
                        return this.DisplayErrorResult($"We're sorry but your order: '{identifier}' is expired or has already been completed.");
                    }

                    if (cart.Analysis == null)
                    {
                        if (Debugger.IsAttached) Debugger.Break();

                        // Race condition -SpinWait
                        await Task.Delay(TimeSpan.FromSeconds(2), cancellation);
                        return this.Redirect(this.Request.RawUrl);
                    }

                    var systemFileName = cart.SystemFileName;
                    var csvFile = new CsvFile(this.tempFolder.CreateInstance(systemFileName));
                    var firstLine = await csvFile.ReadHeaderRow(cancellation);
                    var hasHeaderRow = await Header.IsHeaderLine(firstLine);
                    var delimiter = (await CsvFileContent.DiscoverDelimiterAsync(csvFile, cancellation)) ??
                                    CsvFileContent.DefaultDelimiter;

                    var model = new ClientUploadOrderModel
                    {
                        ListName = cart.Name,
                        SystemFileName = systemFileName,
                        OrderId = identifier,
                        RecordCount = cart.RecordCount,
                        FileDelimiter = delimiter,
                        HasHeaderRow = hasHeaderRow
                    };

                    return this.View("DisplayProducts", new OrderProductsViewPresenter()
                    {
                        Postback = new MvcActionModel()
                        {
                            AreaName = "Order",
                            ControllerName = "Csv",
                            ActionName = nameof(this.SelectProducts)
                        },
                        Order = model,
                        HelpText = "Please note that estimated match rates are based on historical averages of lists processed that contain: first name, last name, (full name may be used in place of first name and last name), street address, city, state (province), and zip code.",
                        OrderViewPath = "~/Areas/Order/Csv/Views/Products.cshtml"
                    });

                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.High, nameof(this.SelectProducts));
                    throw;
                }
            }
        }

        /// <summary>
        /// Step 3.b in the order process. Handle selection of product options for a specific list.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> SelectProducts([ModelBinder(typeof(FormCollectionJsonBinder))] ClientUploadOrderModel orderModel, CancellationToken cancellation)
        {
            orderModel = orderModel ?? new ClientUploadOrderModel();

            var command = new EnterQuoteForCsvOrderCommand();
            command.CartId = orderModel.OrderId;
            command.OrderMinimum = orderModel.OrderMinimum;

            var manifest = new ManifestBuilder();
            manifest.UserId = this.User.Identity.GetIdentifier();
            manifest.InputFieldDelimiter = orderModel.FileDelimiter.ToString();
            manifest.HasHeaderRow = orderModel.HasHeaderRow;

            foreach (var product in orderModel.Products)
            {
                var operation = OperationDefinition.LoadFromDefinition(product.ProductKey.Convert());
                manifest.Operations.Add(operation);

                var quote = new ProductQuote();
                quote.Product = product.ProductKey;
                quote.EstimatedMatches = (Int32)Math.Round(product.EstMatches);
                quote.QuotedRate = product.Cost;

                command.Products.Add(quote);
            }
            command.Manifest = manifest.ToXml();

            await this.bus.SendLocal(command);

            return this.RedirectToAction("IdentifyColumns", new {cartId = orderModel.OrderId});
        }

        /// <summary>
        /// Step 4.a in the order process. Based on the selected products, allow the client to map input columns to our product fields.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> IdentifyColumns(Guid cartId, CancellationToken cancellation)
        {
            using (new Correlation(cartId))
            {
                try
                {
                    var cart = await this.context
                    .SetOf<Sales.Cart>()
                    .ForInteractiveUser()
                    .ForCsv(cartId)
                    .Where(c => c.IsActive)
                    .Include(c => c.Client)
                    .FirstOrDefaultAsync(cancellation);

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

                    if (cart.Manifest == null)
                    {
                        if (Debugger.IsAttached) Debugger.Break();

                        // Race condition -SpinWait
                        await Task.Delay(TimeSpan.FromSeconds(2), cancellation);
                        return this.Redirect(this.Request.RawUrl);
                    }

                    var orderModel = new ClientUploadOrderModel();
                    orderModel.OrderId = cartId;
                    orderModel.ListName = cart.Name;
                    orderModel.FileDelimiter = CsvFileContent.DefaultDelimiter;
                    orderModel.RecordCount = cart.RecordCount;
                    orderModel.SystemFileName = cart.SystemFileName;

                    var model = new MapColumnsViewPresenter()
                    {
                        Postback = new MvcActionModel()
                        {
                            AreaName = "Order",
                            ActionName = nameof(this.IdentifyColumns),
                            ControllerName = "Csv"
                        },
                        Order = orderModel
                    };

                    #region Read Top Rows

                    var rawFile = this.tempFolder.CreateInstance(orderModel.SystemFileName);

                    var manifest = new ManifestBuilder(cart.Manifest);
                    var sample = await FileSampler.Perform(rawFile, manifest, cancellation: cancellation);

                    orderModel.FileDelimiter = sample.CsvFile.Delimiter;
                    orderModel.HasHeaderRow = sample.HasHeaderRow;

                    model.AutomappedFields.AddRange(sample.AutomappedFields);

                    foreach (var item in sample.ColumnSamples)
                    {
                        model.ColumnSamples.Add(item.Key, item.Value.ToList());
                    }

                    #endregion

                    // collection of fields is built into select > options in View
                    model.InputFields = manifest.DetermineRequiredInputFields().Where(a => !a.MetaFieldName.StartsWith("_")).ToList(); // fields with underscore should not be mapped

                    // collection needs to be serialized because it's consumed as a JavaScript array in the view
                    var serializer = new JavaScriptSerializer();
                    model.RequiredFields = serializer.Serialize(manifest.DetermineRequiredInputFields().Where(a => a.Required).Select(a => a.MetaFieldName));

                    return this.View(nameof(this.IdentifyColumns), model);
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.High, nameof(this.IdentifyColumns));
                    throw;
                }
            }
        }

        /// <summary>
        /// Step 4.b in the order process. Store the client mapped input columns.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> IdentifyColumns([ModelBinder(typeof(FormCollectionJsonBinder))] ClientUploadOrderModel orderModel, CancellationToken cancellation)
        {
            var command = new ColumnMapCsvOrderCommand();
            command.CartId = orderModel.OrderId;
            command.HasHeaderRow = orderModel.HasHeaderRow;
            command.ColumnMap = orderModel.ColumnMap;

            await this.bus.SendLocal(command);

            return this.RedirectToAction(nameof(Confirmation), new {cartId = orderModel.OrderId});
        }

        /// <summary>
        /// Confirm that mapped columns are present and if required, require and/or update payment information based on client and then review the quote.
        /// </summary>
        [HttpGet()]
        public virtual ActionResult Confirmation(Guid cartId)
        {
            var model = new OrderConfirmationViewPresenter2()
            {
                Postback = new MvcActionModel()
                {
                    AreaName = "Order",
                    ActionName = "Submit",
                    ControllerName = "Csv"
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
                .SetOf<Sales.Cart>()
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

                    if (!this.ModelState.IsValid) return this.RedirectToAction(nameof(Confirmation), "Automation", new { cartId });

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
                    var command = new SubmitCsvOrderCommand { CartId = cartId };

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
                .SetOf<Sales.CreditCardRef>()
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