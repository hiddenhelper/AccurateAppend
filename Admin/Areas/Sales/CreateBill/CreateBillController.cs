using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Navigator;
using AccurateAppend.Websites.Templates;
using DomainModel;
using DomainModel.Enum;
using DomainModel.Html;
using BillableOrder = AccurateAppend.Sales.BillableOrder;
using BillType = AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models.BillType;
using File = AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models.File;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill
{
    /// <summary>
    /// Controller used to craft a bill for an open deal.
    /// </summary>
    [Authorize()]
    public class CreateBillController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IBillFormatterFactory billFormatterFactory;
        private readonly FileContext files;
        private readonly IPdfGenerator generator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateBillController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing access to the sales context data.</param>
        /// <param name="billFormatterFactory">The <see cref="IBillFormatterFactory"/> used to determine what formatter should be created under which scenarios.</param>
        /// <param name="files">The <see cref="FileContext"/> providing access to the system file locations.</param>
        /// <param name="generator">The <see cref="IPdfGenerator"/> component used for PDF creation.</param>
        public CreateBillController(DefaultContext context, IBillFormatterFactory billFormatterFactory, FileContext files, IPdfGenerator generator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (billFormatterFactory == null) throw new ArgumentNullException(nameof(billFormatterFactory));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            this.context = context;
            this.billFormatterFactory = billFormatterFactory;
            this.files = files;
            this.generator = generator;
        }

        #endregion

        #region Action Methods

        #region Interstitial

        /// <summary>
        /// Bounces the user to the order id review
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> CreateBillFromDeal(Int32 dealId, CancellationToken cancellation)
        {
            var orderId = await this.context
                .SetOf<BillableOrder>()
                .AreEditable()
                .Where(o => o.Deal.Id == dealId)
                .Select(o => o.Id.Value)
                .FirstAsync(cancellation);

            return this.RedirectToAction("ReviewOrder", new {Area = "Sales", id = orderId});
        }

        #endregion

        #region Step 1

        /// <summary>
        /// Action to review the current order information and select billing type.
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult> ReviewOrder(Int32 id, CancellationToken cancellation)
        {
            var order = await this.context
                .SetOf<DealBinder>()
                .AreEditable()
                .Where(d => d.Orders.Any(o => o.Id == id))
                .SelectMany(d => d.Orders)
                .OfType<BillableOrder>()
                .AreEditable()
                .Select(o => new ReviewOrderModel
                {
                    OrderId = o.Id.Value,
                    UserId = o.Deal.Client.UserId,
                    DealId = o.Deal.Id.Value
                })
                .FirstOrDefaultAsync(cancellation);
            if (order == null) return this.DisplayErrorResult($"The order {id} does not exist or cannot have a bill created for it.");

            return this.View(order);
        }

        /// <summary>
        /// Action to process the reviewed order information.
        /// </summary>
        [HttpPost()]
        public virtual ActionResult ReviewOrder(ReviewOrderModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            return this.RedirectToAction("AttachFiles", "CreateBill", new {Area = "Sales", mode = model.BillType, model.DealId});
        }

        #endregion

        #region Step 2

        /// <summary>
        /// Action to select a template and any attached job or user files that should be added to the bill.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> AttachFiles(BillType mode, Int32 dealId, CancellationToken cancellation)
        {
            var model = await this.context
                .SetOf<DealBinder>()
                .AreEditable()
                .Where(d => d.Id == dealId)
                .Select(d => new
                {
                    DealId = d.Id.Value,
                    OrderId = d.Orders.OfType<BillableOrder>().Select(o => o.Id.Value).FirstOrDefault(),
                    PublicKey = d.Orders.OfType<BillableOrder>().Select(o => o.PublicKey).FirstOrDefault(),
                    d.Client.UserId,
                    d.Title
                })
                .FirstAsync(cancellation)
                .ContinueWith(t => new BillViewModel()
                {
                    UserId = t.Result.UserId,
                    BillType = mode,
                    DealId = t.Result.DealId,
                    OrderId = t.Result.OrderId,
                    PublicKey = Guid.Parse(t.Result.PublicKey),
                    Title = t.Result.Title,
                    ReceiptTemplateName = AcquireReceiptTemplateName(t.Result.Title)
                }, cancellation);

            return this.View(model);
        }

        /// <summary>
        /// Action to process the selected file attachments.
        /// </summary>
        [HttpPost()]
        public virtual ActionResult AttachFiles(BillViewModel model)
        {
            if (!this.ModelState.IsValid) return this.View(model);

            // Clear out the unselected files to reduce payload size
            model.AdminFiles.RemoveAll(f => !f.Selected);
            model.ClientFiles.RemoveAll(f => !f.Selected);

            this.Session["Bill"] = model;

            return this.RedirectToAction("Draft");
        }

        #endregion

        #region Step 3

        /// <summary>
        /// Action to display the bill content editor.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Draft(CancellationToken cancellation)
        {
            var model = this.Session["Bill"] as BillViewModel;
            if (model == null) return this.DisplayErrorResult("Bill creation has timed out");

            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var deal = await this.context
                    .SetOf<DealBinder>()
                    .AreEditable()
                    .Where(d => d.Id == model.DealId)
                    .Include(d => d.Orders.Select(o => o.Lines))
                    .Include(d => d.Orders)
                    .Include(d => d.Client)
                    .FirstOrDefaultAsync(cancellation);
                if (deal == null) return this.DisplayErrorResult($"Deal {model.DealId} does not exist.");

                await this.PopulateContent(model, deal);
                
                return this.View(model);
            }
        }

        /// <summary>
        /// Action to process the bill content from the editor.
        /// </summary>
        [HttpPost()]
        public virtual async Task<ActionResult> Draft(BillViewModel bill, String to, String bcc, CancellationToken cancellation)
        {
            try
            {
                if (!this.ModelState.IsValid) return this.DisplayErrorResult();

                var order = await this.context
                    .SetOf<BillableOrder>()
                    .FirstOrDefaultAsync(o => o.Id == bill.OrderId, cancellation);
                if (order == null) throw new InvalidOperationException($"Order {bill.OrderId} does not exist");

                var communication = new BillContent(bill.Content.SendFrom);
                communication.Subject = bill.Content.Subject;

                if (bill.Content.IsHtml)
                {
                    var formatter = await this.AcquireBillFormatterForTemplate(bill);

                    communication.Body = $"{await formatter.CreateHeader(bill)}{HttpUtility.HtmlDecode(bill.Content.Body)}{await formatter.CreateFooter(bill)}";
                }
                else
                {
                    communication.Body = bill.Content.Body;
                }

                communication.IsHtml = bill.Content.IsHtml;

                foreach (var recipient in to.Split(",").Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                {
                    communication.SendTo.Add(new MailAddress(recipient));
                }
                foreach (var recipient in bcc.Split(",").Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                {
                    communication.BccTo.Add(new MailAddress(recipient));
                }

                // attach files
                var attachements = await this.ProcessAttachedFiles(bill.ClientFiles, this.files.Outbox, this.files.Temp, bill.ZipFiles, cancellation);
                communication.Attachments.AddRange(attachements);

                attachements = await this.ProcessAttachedFiles(bill.AdminFiles, this.files.Assisted, this.files.Temp, bill.ZipFiles, cancellation);
                communication.Attachments.AddRange(attachements);

                communication.Attachments.Add(await BuildOrderPdfAttachment(order, cancellation));

                if (bill.BillType == BillType.Invoice)
                {
                    order.DraftInvoice(communication);
                }
                else
                {
                    order.DraftReceipt(communication);
                }

                await this.context.SaveChangesAsync(cancellation);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);
                this.TempData["Error"] = ex.Message;

                return this.RedirectToAction("TransmitOrderError", "CreateBill", new { Area = "Accounting", userid = bill.UserId });
            }

            return this.NavigationFor<DealDetailController>().Detail(bill.DealId);
        }

        #endregion

        #endregion

        #region Helpers

        private Task<IBillFormatter> AcquireBillFormatterForTemplate(BillViewModel model)
        {
            switch (model.ReceiptTemplateName)
            {
                case ReceiptTemplateName.Subscription:
                    return this.billFormatterFactory.ForSubscription(model.UserId);
                case ReceiptTemplateName.Usage:
                    return this.billFormatterFactory.ForUsage(model.UserId);
                case ReceiptTemplateName.Refund:
                    return this.billFormatterFactory.ForRefund(model.UserId);
                case ReceiptTemplateName.NationBuilder:
                    return this.billFormatterFactory.ForNationBuilder(model.PublicKey);
                case ReceiptTemplateName.IndHh:
                    return this.billFormatterFactory.ByMatchType(model.UserId);
                case ReceiptTemplateName.MatchLevel:
                    return this.billFormatterFactory.ByMatchLevel(model.UserId);
                case ReceiptTemplateName.Public:
                    return this.billFormatterFactory.ForPublic(model.PublicKey);
                default:
                    throw new NotSupportedException($"{model.ReceiptTemplateName} type is not supported");
            }
        }

        private async Task PopulateContent(BillViewModel model, DealBinder deal)
        {
            var formatter = await this.AcquireBillFormatterForTemplate(model).ConfigureAwait(false);
            foreach (var address in await formatter.CreateTo(model).ConfigureAwait(false))
            {
                model.Content.SendTo.Add(address.Address);
            }

            foreach (var address in await formatter.CreateBcc(model).ConfigureAwait(false))
            {
                model.Content.BccTo.Add(address.Address);
            }

            model.Content.Subject = await formatter.CreateSubject(model).ConfigureAwait(false);
            model.Content.SendFrom = (await formatter.SendFrom(model).ConfigureAwait(false)).Address;
            model.Content.Body = await formatter.CreateBody(deal.OriginatingOrder(), model.Attachments).ConfigureAwait(false);
            model.Content.IsHtml = formatter.IsHtml;
        }

        private ReceiptTemplateName AcquireReceiptTemplateName(String title)
        {
            if (title.ToLower().Contains("usage")) return ReceiptTemplateName.Usage;

            if (title.ToLower().Contains("subscription")) return ReceiptTemplateName.Subscription;

            if (title.ToLower().Contains("nationbuilder")) return ReceiptTemplateName.NationBuilder;
            
            return ReceiptTemplateName.IndHh;
        }

        private async Task<IEnumerable<FileAttachment>> ProcessAttachedFiles(IEnumerable<File> postedFiles, IFileLocation source, IFileLocation destination, Boolean zip, CancellationToken cancellation)
        {
            var attachments = new List<FileAttachment>();

            // find selected files
            foreach (var f in postedFiles.Where(f => f.Selected))
            {
                var customerfilename = f.DetermineFileName();
                var systemFileName = f.Filename;

                var readFrom = source.CreateInstance(systemFileName);
                var writeTo = destination.CreateInstance(systemFileName);

                await readFrom.CopyToAsync(writeTo, cancellation);

                var attachment = new FileAttachment(writeTo.Path, MimeTypeHelper.ConvertMimeType(customerfilename), customerfilename);

                if (zip)
                {
                    attachment.Compression = true;
                }
                attachments.Add(attachment);
            }

            return attachments;
        }

        private async Task<FileAttachment> BuildOrderPdfAttachment(Order order, CancellationToken cancellation)
        {
            var html = await OrderDetailHtml.OrderDetail(order).ConfigureAwait(false);
            var pdfReport = this.generator.FromHtml(html);
            var tempPdfFile = this.files.Temp.CreateInstance(Guid.NewGuid().ToString());
            using (var destination = tempPdfFile.OpenStream(FileAccess.Write))
            {
                await new MemoryStream(pdfReport).CopyToAsync(destination, cancellation).ConfigureAwait(false);
            }
            var fileName = $"Accurate Append - Order {order.Id}, {order.Deal.Client.UserName.ToLower()}.pdf";
            return new FileAttachment(tempPdfFile.Path, MimeTypeHelper.ConvertMimeType(fileName), fileName);;
        }
        
        #endregion
    }
}