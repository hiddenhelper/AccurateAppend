using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Sales.ReadModel;
using AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary;
using AccurateAppend.Websites.Admin.Areas.Sales.CreateBill;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.EditDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.ExpireDeal;
using AccurateAppend.Websites.Admin.Areas.Sales.ReviewDeal;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealsApi
{
    /// <summary>
    /// Controller to provide query access to <see cref="DealView"/> data.
    /// </summary>
    [Authorize()]
    public class DealsApiController : Controller
    {
        #region Fields

        private readonly ReadContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealsApiController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> dal component.</param>
        public DealsApiController(ReadContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to query <see cref="DealBinder"/> data for a single user indicated by identifier.
        /// </summary>
        public async Task<ActionResult> ByUser(Guid userId, DealStatus? status, CancellationToken cancellation)
        {
            var dealQuery = this.context
                .SetOf<DealView>()
                .ForUser(userId);
            if (status != null) dealQuery = dealQuery.Where(d => d.Status == status);

            var deals = await dealQuery
                .OrderBy(o => o.CreatedDate)
                .ToArrayAsync(cancellation);

            var data = deals.Select(d =>
                new
                {
                    d.DealId,
                    DateOrdered = d.CreatedDate.ToUserLocal(),
                    d.Title,
                    d.Status,
                    d.Amount,
                    DateCreated = d.CreatedDate,
                    Client = new
                    {
                        d.UserId,
                        d.UserName
                    },
                    Links = new
                    {
                        Detail = this.Url.BuildFor<DealDetailController>().Detail(d.DealId),
                    }
                }).ToArray();

            return new JsonNetResult(DateTimeKind.Local) {Data = data};
        }

        /// <summary>
        /// Query to retrieve the details of a single deal by identifier.
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult> ById(Int32 dealId, CancellationToken cancellation)
        {
            var deal = (await this.context
                    .SetOf<DealView>()
                    .Where(d => d.DealId == dealId)
                    .ToArrayAsync(cancellation))
                .Select(d =>
                    new
                    {
                        d.DealId,
                        d.Title,
                        d.Description,
                        d.Amount,
                        CreatedDate = d.CreatedDate.ToLocalTime(),
                        d.Status,
                        StatusDescription = d.Status.GetDescription(),
                        d.ProcessingInstructions,
                        d.EnableAutoBill,
                        Owner = new
                        {
                            UserId = d.OwnerId,
                            UserName = d.OwnerName
                        },
                        Links = new
                        {
                            Edit = d.Status.CanBeEdited()
                                ? this.Url.BuildFor<EditDealController>().Edit(dealId)
                                : null,
                            Bill = d.Status.CanBeEdited()
                                ? this.Url.BuildFor<CreateBillController>().ForDeal(dealId)
                                : null,
                            Review = d.Status.CanBeReviewed()
                                ? this.Url.BuildFor<ReviewDealController>().Review(dealId)
                                : null,
                            Charges = d.Status == DealStatus.Billing || d.Status == DealStatus.Complete
                                ? this.Url.BuildFor<ChargeEventSummaryController>().ForDeal(dealId)
                                : null,
                            Expire = d.Status == DealStatus.Billing
                                ? this.Url.BuildFor<ExpireDealController>().Expire(dealId)
                                : null,
                            Refund = d.Status == DealStatus.Complete
                                ? this.Url.Action("Index", "CreateRefund", new {Area = "Sales", dealId})
                                : null
                        },
                        Actions = new
                        {
                            SendPaymentUpdate = d.Status == DealStatus.Billing
                                ? this.Url.Action("ForDeal", "SendPaymentUpdate", new {Area = "Sales", dealId})
                                : null
                        }
                    }
                )
                .FirstOrDefault();

            var result = new JsonNetResult(DateTimeKind.Local)
            {
                Data = deal
            };
            return result;
        }

        #endregion
    }
}