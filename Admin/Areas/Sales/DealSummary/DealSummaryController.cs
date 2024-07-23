using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Core;
using AccurateAppend.Core.Collections;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.ReadModel;
using AccurateAppend.Sales.ReadModel.Queries;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.DealSummary.Models;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Enum;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealSummary
{
    /// <summary>
    /// Controller for displaying deal summary information.
    /// </summary>
    [Authorize()]
    public class DealSummaryController : Controller
    {
        #region Fields

        private readonly IDealsViewActiveDuringQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealSummaryController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IDealsViewActiveDuringQuery"/> DAL component.</param>
        public DealSummaryController(IDealsViewActiveDuringQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Main Deals page
        /// </summary>
        public ActionResult Index(Guid? userId, DealStatus? status, String email, DateRange? dateRange, Guid? applicationId)
        {
            if (this.User.Identity.IsLimitedAccess())
            {
                this.TempData["message"] = "Your account does not have access to this feature.";
                return this.View("~/Views/Shared/Error.aspx");
            }
            
            return this.View(new DealSummaryViewModel
                {
                    Email = email,
                    Status = status,
                    UserId = userId,
                    DateRange = dateRange,
                    ApplicationId = applicationId
                });
        }

        /// <summary>
        /// Returns Json object containing all deals
        /// </summary>
        //[OutputCache(Duration = 1*20, VaryByParam = "*")] // removed because it prevents screen from refreshing when processing billing.
        public ActionResult Query([DataSourceRequest] DataSourceRequest request, DealStatus? status, Guid applicationId, DateTime startDate, DateTime endDate, Guid? userId, String email, Boolean? nonCompleteOnly)
        {
            if (request.Sorts == null || !request.Sorts.Any())
            {
                request.Sorts = new List<SortDescriptor>
                {
                    new SortDescriptor("DateCreated", ListSortDirection.Descending)
                };
            }

            var query = this.CreateQuery(startDate, endDate, applicationId, userId, status, email);

            if (nonCompleteOnly == true)
            {
                query = query.Where(d => d.Status != DealStatus.Complete);
            }

            var data = query
                .Select(
                    item =>
                        new
                        {
                            item.CreatedDate,
                            item.Title,
                            item.DealId,
                            item.BusinessName,
                            item.UserName,
                            item.FirstName,
                            item.UserId,
                            item.LastName,
                            item.Status,
                            item.ApplicationId,
                            item.Amount,
                            item.OwnerId,
                            item.OwnerName
                        }
                )
                .ToArray()
                .Select(
                    item =>
                    {
                        var model = new
                        {

                            DateCreated = item.CreatedDate.Coerce().ToUserLocal(),
                            item.Title,
                            Id = item.DealId,
                            item.BusinessName,
                            Email = item.UserName,
                            item.FirstName,
                            item.UserId,
                            item.LastName,
                            item.Status,
                            item.ApplicationId,
                            AdjustedTotal = item.Amount,
                            Owner = new
                            {
                                UserId = item.OwnerId,
                                UserName = item.OwnerName
                            },
                            Links = new
                            {
                                DetailView = this.Url.BuildFor<DealDetailController>().Detail(item.DealId),
                                ClientView = this.Url.BuildFor<UserDetailController>().ToDetail(item.UserId)
                            }
                        };

                        return model;
                    }
                )
                .OrderByDescending(a => a.DateCreated)
                .ToArray()
                .ToDataSourceResult(request);

            data.Total = data.Data.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Returns Json object containing count grouped by Status
        /// </summary>
        [OutputCache(Duration = 1*20, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetDealStatuses(DateTime startDate, DateTime endDate, Guid applicationId, Guid? userId, String email, CancellationToken cancellation)
        {
            var query = this.CreateQuery(startDate, endDate, applicationId, userId, email: email);
            
            var grouped = await query.GroupBy(d => d.Status)
                .Select(g => new {Cnt = g.Count(), Description = g.Key})
                .Distinct()
                .OrderBy(g => g.Description)
                .ToArrayAsync(cancellation);

            var jsonNetResult = new JsonNetResult
            {
                Data = grouped
            };
            return jsonNetResult;
        }

        #endregion

        #region Query Builder

        protected virtual IQueryable<DealView> CreateQuery(DateTime startDate, DateTime endDate, Guid applicationId, Guid? userId = null, AccurateAppend.Sales.DealStatus? status = null, String email = null)
        {
            // Deals table is UTC so we ned to convert start/end dates
            startDate = startDate.ToStartOfDay().FromUserLocal().Coerce();
            endDate = endDate.ToEndOfDay().FromUserLocal().Coerce();

            var query = this.dal.ActiveDuring(startDate, endDate).ForApplication(applicationId);

            if (status != null)
            {
                query = query.InStatus(status.Value);
            }

            if (userId != null)
            {
                query = query.ForUser(userId.Value);
            }

            if (!String.IsNullOrWhiteSpace(email))
            {
                query = query.ForUser(email.Trim());
            }

            return query;
        }

        #endregion
    }
}
