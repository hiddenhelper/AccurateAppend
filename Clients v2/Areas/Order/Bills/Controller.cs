using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Templates;
using DomainModel.ActionResults;
using DomainModel.Html;

namespace AccurateAppend.Websites.Clients.Areas.Order.Bills
{
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IPdfGenerator generator;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <seealso cref="Controller" /> class.
        /// </summary>
        /// <param name="context">The <seealso cref="DefaultContext" /> to use for data access.</param>
        /// <param name="generator">The <see cref="IPdfGenerator" /> component used for PDF creation.</param>
        public Controller(DefaultContext context, IPdfGenerator generator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            Contract.EndContractBlock();

            this.context = context;
            this.generator = generator;
        }

        #endregion
        
        #region Action Methods

        /// <summary>
        ///     View to access the current bills.
        /// </summary>
        public virtual ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to query the completed orders for the interactive client.
        /// </summary>
        public virtual async Task<ActionResult> ForCurrentUser(CancellationToken cancellation, DateTime? start, DateTime? end)
        {
            start = (start ?? DateTime.Today.ToFirstOfMonth().AddYears(-1)).ToStartOfDay();
            end = (end ?? DateTime.Today).ToEndOfDay();

            var data = await this.context
                .SetOf<BillableOrder>()
                .ForInteractiveUser()
                .AreComplete()
                .Where(o => o.CompletedDate >= start && o.CompletedDate <= end)
                .Select(o =>
                    new
                    {
                        o.Deal.Title,
                        o.Deal.Amount,
                        o.CompletedDate,
                        o.PublicKey,
                        o.Id
                    })
                .ToArrayAsync(cancellation);

            var final = data.Select(o =>
                new
                {
                    o.Id,
                    o.Title,
                    o.Amount,
                    o.CompletedDate,
                    BillId = o.PublicKey,
                    Links = new
                    {
                        Download = Url.Action("Receipt", "Bills", new { Area = "Order"}) + "/" + o.PublicKey
                    }
                }).OrderByDescending(a => a.CompletedDate).ToArray();

            return new JsonNetResult(DateTimeKind.Utc) {Data = new {Data = final, Total = final.Count()}};
        }

        /// <summary>
        ///     Action method to generate a PDF version of a specific bill by order identifier (public key).
        /// </summary>
        public virtual async Task<ActionResult> Receipt(Guid id, CancellationToken cancellation)
        {
            var userId = this.User.Identity.GetIdentifier();

            var order = await this.context
                .SetOf<BillableOrder>()
                .Where(o => o.Deal.Client.UserId == userId)
                .Where(o => o.PublicKey == id.ToString())
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .Include(o => o.Lines)
                .Include(o => o.Lines.Select(l => l.Product))
                .FirstOrDefaultAsync(cancellation);
            if (order == null) return new LiteralResult(true) {Data = "Order is not found"};

            var html = await OrderDetailHtml.OrderDetail(order);
            var data = generator.FromHtml(html);

            return File(data, MediaTypeNames.Application.Pdf, $"Accurate Append - Order {order.Id}, {User.Identity.Name.ToLower()}.pdf");
        }

        #endregion
    }
}