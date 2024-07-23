using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDeal;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Enum;
using DomainModel.Queries;
using DomainModel.ReadModel;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserSummary
{
    /// <summary>
    /// Controller to view clients list.
    /// </summary>
    [Authorize()]
    public class UserSummaryController : ActivityLoggingController2
    {
        #region Fields

        private readonly IClientsViewQuery dal;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSummaryController"/> class.
        /// </summary>
        /// <param name="query">The <see cref="IClientsViewQuery"/> to use for this controller instance.</param>
        public UserSummaryController(IMessageSession bus, IClientsViewQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            this.bus = bus;
            this.dal = query;
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Index()
        {
            return this.View();
        }

        public virtual async Task<ActionResult> Read([DataSourceRequest()] DataSourceRequest request, Guid applicationid, DateRange dateRange, CancellationToken cancellation)
        {
            request.Sorts = new List<SortDescriptor>()
            {
                new SortDescriptor(nameof(ClientView.LastActivityDate), ListSortDirection.Descending)
            };

            var query = this.CraftQuery(dateRange, applicationid);

            var records = (await query.ToArrayAsync(cancellation)).Select(u =>
                new
                {
                    u.ApplicationId,
                    u.CompositeName,
                    u.FirstName,
                    u.IsSubscriber,
                    u.Location,
                    u.LastActivityDate,
                    u.LastActivityDescription,
                    u.LastName,
                    u.LifeTimeRevenue,
                    u.LifeTimeRevenueDescription,
                    u.Status,
                    StatusDescription = u.Status.GetDescription(),
                    u.UserName,
                    u.UserId,
                    DetailUrl = this.Url.BuildFor<UserDetailController>().ToDetail(u.UserId),
                    NewDealUrl = this.Url.BuildFor<NewDealController>().ToCreate(u.UserId)
                }).ToArray();

            var data = records.ToDataSourceResult(request, o => o);
            data.Total = records.Length;

            var result = new JsonNetResult
            {
                Data = data
            };

            return result;
        }

        [Authorize()]
        public virtual async Task<ActionResult> Download(DateRange dateRange, Guid applicationid, CancellationToken cancellation)
        {
            this.OnEvent("Download customer list");

            var message = new SendEmailCommand();
            message.Subject = "Customer download";
            message.Body = $"Action By: {this.User.Identity.Name}";
            message.Track = false;
            message.IsHtmlContent = false;
            message.To.Add("chris@accurateappend.com");
            message.SendFrom = "support@accurateappend.com";

            await this.bus.Send(message);
            //var query = this.CraftQuery(dateRange, applicationid);

            //var records = (await query.ToArrayAsync(cancellation)).Select(u =>
            //    new
            //    {
            //        u.UserId,
            //        u.UserName,
            //        u.CompositeName,
            //        u.LastActivityDate,
            //        Status = u.Status.GetDescription(),
            //        u.Address,
            //        u.City,
            //        u.State,
            //        u.Zip,
            //        u.Phone,
            //        u.LifeTimeRevenue,
            //        u.LifeTimeRevenueDescription,
            //        LeadSource = u.LeadSource.GetDescription(),
            //        DetailUrl = this.Url.BuildFor<UsersController>().ToDetail(u.UserId, Uri.UriSchemeHttps)
            //    })
            //    .OrderByDescending(a => a.LastActivityDate);

            //var report = new StringBuilder().AppendLine("UserId\tEmail\tName\tLastActivityDate\tStatus\tAddress\tCity\tState\tZip\tPhone\tLifeTimeRevenue\tLifeTimeRevenueDescription\tLeadSource");
            //foreach (var d in records)
            //{
            //    report.AppendLine($"{d.UserId}\t{d.UserName}\t{d.CompositeName}\t{d.LastActivityDate}\t\t{d.Status}\t{d.Address}\t{d.City}\t{d.State}\t{d.Zip}\t{d.Phone}\t{d.LifeTimeRevenue}\t{d.LifeTimeRevenueDescription}\t{d.LeadSource}");
            //}

            return new FileProxyResult(NullFile.Null);

            //return this.File(Encoding.UTF8.GetBytes(report.ToString()), "text/csv", "Users.tsv");
        }

        #endregion

        #region Methods

        protected virtual IQueryable<ClientView> CraftQuery(DateRange dateRange, Guid applicationId)
        {
            // default to Last7Days
            if (dateRange == DateRange.Custom) dateRange = DateRange.Last7Days;

            DateTime startdate;
            DateTime enddate;

            this.DetermineDateRange(dateRange, out startdate, out enddate);

            var query = this.dal.ActiveDuring(startdate, enddate).Where(c => c.ApplicationId == applicationId);
            query = query.Where(u => u.UserId != new Guid("43056364-2161-448e-8bd2-ee1fcebd3492")); // Filter Rob
            query = query.OrderByDescending(a => a.LastActivityDate);

            return query;
        }

        protected virtual void DetermineDateRange(DateRange dateRange, out DateTime start, out DateTime end, Boolean utc = true)
        {
            start = DateTime.Now.ToStartOfDay();
            end = DateTime.Now.ToEndOfDay();

            switch (dateRange)
            {
                case DateRange.Today:
                    start = DateTime.Now.ToStartOfDay();
                    end = DateTime.Now.ToEndOfDay();
                    break;
                case DateRange.LastMonth:
                    start = end.ToFirstOfMonth().AddMonths(-1);
                    end = start.ToLastOfMonth();
                    break;
                case DateRange.Last30Days:
                    start = end.AddDays(-30);
                    break;
                case DateRange.Last60Days:
                    start = end.AddDays(-60);
                    break;
                case DateRange.Last90Days:
                    start = end.AddDays(-90);
                    break;
                case DateRange.All:
                    start = new DateTime(2012, 1, 1);
                    break;
                case DateRange.Last24Hours:
                    end = DateTime.Now;
                    start = end.AddHours(-24);
                    break;
                case DateRange.Yesterday:
                    start = start.AddDays(-1);
                    end = end.AddDays(-1);
                    break;
                case DateRange.Last7Days:
                    start = start.AddDays(-7);
                    break;
                case DateRange.CurrentMonth:
                    start = start.ToFirstOfMonth();
                    break;
                case DateRange.PreviousToLastMonth:
                    start = end.ToFirstOfMonth().AddMonths(-2);
                    end = start.ToLastOfMonth();
                    break;
                case DateRange.Last60Minutes:
                    end = DateTime.Now;
                    start = end.AddMinutes(-60);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dateRange), dateRange, null);
            }

            if (!utc) return;

            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
        }

        #endregion
    }
}