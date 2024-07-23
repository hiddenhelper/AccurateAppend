using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary.Models;
using AccurateAppend.Websites.Admin.Areas.Sales.OrderDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ChargeEvent = DomainModel.ReadModel.ChargeEvent;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary
{
    /// <summary>
    /// Controller displaying <see cref="ChargeEvent"/> model.
    /// </summary>
    [Authorize()]
    public class ChargeEventSummaryController : Controller
    {
        #region Fields

        private readonly IChargeEventsQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeEventSummaryController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IChargeEventsQuery"/> component.</param>
        public ChargeEventSummaryController(IChargeEventsQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Main summary view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(String email, Guid? userId, Int32? dealId)
        {
            if ((!String.IsNullOrEmpty(email) || userId != null) && dealId != null)
            {
                this.TempData["message"] = "You cannot search by both user/email and for transactions for a specific deal simultaneously.";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var model = new ClientModel {UserId = userId, Email = email, DealId = dealId, };

            return this.View(model);
        }

        /// <summary>
        /// Returns Json object containing ChargeEvent transactions
        /// </summary>
        [OutputCache(Duration = 15*0, VaryByParam = "*")]
        public virtual ActionResult Query([DataSourceRequest] DataSourceRequest request,
            Guid applicationid, DateTime startdate, DateTime enddate, TransactionResult? status, Guid? userid,
            String email, Int32? dealId)
        {
            if (request.Sorts == null || !request.Sorts.Any()) request.Sorts = new List<SortDescriptor> { new SortDescriptor(nameof(ChargeEvent.EventDate), ListSortDirection.Descending) };

            IQueryable<ChargeEvent> query;

            if (dealId == null)
            {
                // Charges table is UTC so we ned to convert start/end dates
                startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
                enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

                query = this.dal.ForApplication(applicationid, startdate, enddate);

                if (!String.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(e => e.UserName == email);
                }
                if (userid != null)
                {
                    query = query.Where(e => e.UserId == userid.Value);
                }
                if (status != null)
                {
                    query = query.Where(e => e.Status == status.Value.ToString());
                }
            }
            else
            {
                query = this.dal.ForDeal(dealId.Value);
            }

            query = query.OrderByDescending(e => e.EventDate);
            
            var data = query.With().ToDataSourceResult(request, a => new
            {
                a.Address,
                a.Amount,
                a.City,
                a.FullName,
                EventDate = a.EventDate.ToUserLocal(),
                a.UserName,
                a.Id,
                a.OrderId,
                a.Status,
                a.State,
                a.UserId,
                a.TransactionId,
                a.TransactionType,
                a.AuthorizationCode,
                a.Message,
                a.ZipCode,
                a.DisplayValue,
                a.ExpirationDate,
                Links = new { UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(a.UserId) },
                Data = new { TransactionDetail = this.Url.Action("GetChargeEventDetailJson", new { id = a.Id }) }
            });
            data.Total = data.Data.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }
        
        /// <summary>
        /// Returns Json object for single ChargeEvent
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual ActionResult GetChargeEventDetailJson(Int32 id)
        {
            var query = this.dal.ForId(id);

            var data = query.With().Select(a => new
            {
                a.Address,
                a.Amount,
                a.City,
                a.FullName,
                EventDate = a.EventDate.ToUserLocal(),
                a.UserName,
                a.Id,
                a.OrderId,
                a.Status,
                a.State,
                a.UserId,
                a.TransactionId,
                a.TransactionType,
                a.AuthorizationCode,
                a.Message,
                a.ZipCode,
                a.DisplayValue,
                a.ExpirationDate,
                UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(a.UserId),
                OrderDetail = this.Url.BuildFor<OrderDetailController>().Detail(a.OrderId)
            }).FirstOrDefault();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Returns Json object containing grouped Status
        /// </summary>
        [OutputCache(Duration = 10, VaryByParam = "*")]
        public ActionResult GetChargeEventsStatusesJson(Guid applicationid, DateTime startdate, DateTime enddate,
            TransactionResult? status, Guid? userid, String email)
        {
            startdate = startdate.FromUserLocal().Coerce().ToStartOfDay();
            enddate = enddate.FromUserLocal().Coerce().ToEndOfDay();

            var query = this.dal.ForApplication(applicationid, startdate, enddate);

            if (!String.IsNullOrWhiteSpace(email))
            {
                query = query.Where(e => e.UserName == email);
            }
            if (userid != null)
            {
                query = query.Where(e => e.UserId == userid.Value);
            }
            if (status != null)
            {
                query = query.Where(e => e.Status == status.Value.ToString());
            }

            query = query.OrderByDescending(e => e.EventDate);

            var data = query.ToArray()
                .GroupBy(d => d.Status)
                .Select(g => new {Cnt = g.Count(), Description = g.Key});

            var jsonNetResult = new JsonNetResult
            {
                Data = data
            };
            return jsonNetResult;
        }

        #endregion
    }
}
