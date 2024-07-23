using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Clients.SearchClients.Models;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDeal;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Filters;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Enum;
using DomainModel.Queries;
using DomainModel.ReadModel;
using Kendo.Mvc.Extensions;

namespace AccurateAppend.Websites.Admin.Areas.Clients.SearchClients
{
    /// <summary>
    /// Provides 
    /// </summary>
    [Authorize()]
    public class SearchClientsController : ActivityLoggingController2
    {
        #region Fields

        private readonly ILeadsViewQuery leads;
        private readonly IClientsViewQuery clients;
        private readonly ISessionContext context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBoundController"/> class.
        /// </summary>
        public SearchClientsController(ILeadsViewQuery leads, IClientsViewQuery clients, ISessionContext context)
        {
            if (leads == null) throw new ArgumentNullException(nameof(leads));
            if (clients == null) throw new ArgumentNullException(nameof(clients));
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.leads = leads;
            this.clients = clients;
            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(String searchterm, CancellationToken cancellation)
        {
            if (String.IsNullOrWhiteSpace(searchterm) || searchterm.Length < 2)
            {
                return this.View(new SearchResultModel());
            }

            searchterm = searchterm.Trim();

            this.OnEvent($"Search users, term:{searchterm}");

            var clientQuery = (this.User.Identity.IsLimitedAccess()
                ? Enumerable.Empty<ClientView>().AsQueryable()
                : this.clients.Search(searchterm));

            var leadQuery = this.leads.Search(searchterm).Where(l => l.Status != LeadStatus.ConvertedToCustomer);

            // Membership filters
            if (!this.User.Identity.IsAccurateAppendAdmin())
            {
                clientQuery = clientQuery.Where(u => u.ApplicationId != WellKnownIdentifiers.AccurateAppendId);
                leadQuery = leadQuery.Where(l => l.ApplicationId != WellKnownIdentifiers.AccurateAppendId);
            }

            if (!this.User.Identity.Is2020ConnectAdmin())
            {
                clientQuery = clientQuery.Where(u => u.ApplicationId != WellKnownIdentifiers.TwentyTwentyId);
                leadQuery = leadQuery.Where(l => l.ApplicationId != WellKnownIdentifiers.TwentyTwentyId);
            }

            var model = new SearchResultModel();

            await clientQuery.OrderByDescending(u => u.LastActivityDate).ForEachAsync(u =>
            {
                model.Users.Add(
                    new UserSearchResult()
                    {
                        ApplicationId = u.ApplicationId,
                        UserId = u.UserId,
                        UserName = u.UserName,
                        BusinessName = u.BusinessName,
                        Email = u.UserName,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        LastActivityDate = u.LastActivityDate,
                        DetailUrl = this.Url.BuildFor<UserDetailController>().ToDetail(u.UserId),
                        NewDealUrl = this.Url.BuildFor<NewDealController>().ToCreate(u.UserId)
                    });
            }, cancellation);

            await leadQuery.OrderByDescending(l => l.DateAdded).ForEachAsync(l => { model.Leads.Add(l); }, cancellation);

            // if there is one client match ONLY, then go directly to the user detail
            if (!this.User.Identity.IsLimitedAccess() && model.Users.Count == 1 && model.Leads.Count == 0)
            {
                return this.Redirect(model.Users.First().DetailUrl);
            }

            return this.View(model);
        }

        [HandleErrorWithAjaxFilter()]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult Query([Kendo.Mvc.UI.DataSourceRequest] Kendo.Mvc.UI.DataSourceRequest request, String searchterm)
        {
            var clientQuery = (this.User.Identity.IsLimitedAccess()
                ? Enumerable.Empty<ClientView>().AsQueryable()
                : this.clients.Search(searchterm));

            // Membership filters
            if (!this.User.Identity.IsAccurateAppendAdmin())
            {
                clientQuery = clientQuery.Where(u => u.ApplicationId != WellKnownIdentifiers.AccurateAppendId);
            }

            if (!this.User.Identity.Is2020ConnectAdmin())
            {
                clientQuery = clientQuery.Where(u => u.ApplicationId != WellKnownIdentifiers.TwentyTwentyId);
            }

            var results = clientQuery.ToArray().Select(u => new
            {
                u.ApplicationId,
                u.UserId,
                u.UserName,
                u.BusinessName,
                Email = u.UserName,
                u.FirstName,
                u.LastName,
                LastActivityDate = u.LastActivityDate.ToUserLocal(),
                SiteName = WellKnownIdentifiers.AdminId == u.ApplicationId ? String.Empty : SiteCache.Cache.First(s => s.ApplicationId == u.ApplicationId).Title
            }).ToArray();

            var data = results.OrderByDescending(u => u.LastActivityDate).ToDataSourceResult(request, o => new
                {
                    o.UserId,
                    Email = o.Email.ToLower(),
                    CompositeName =
                    PartyExtensions.BuildCompositeName(o.FirstName, o.LastName,
                        String.IsNullOrWhiteSpace(o.BusinessName) ? o.UserName : o.BusinessName),
                    LastActivityDescription = o.LastActivityDate.DescribeDifference(DateTime.Now.ToUserLocal()),
                    Links = new
                    {
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(o.UserId)
                    }
            });

            data.Total = results.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };

            return jsonNetResult;
        }

        [HandleErrorWithAjaxFilter()]
        [OutputCache(Duration = 60*5, VaryByParam = "*")]
        public virtual async Task<ActionResult> List(Guid applicationid, DateRange activeWithin, CancellationToken cancellation)
        {
            var period = DateRangeExtensions.CalculatePeriod(DateTime.Now.FromUserLocal(), activeWithin);

            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var query = this.context.SetOf<Client>()
                    .Where(c => c.Logon.Application.Id == applicationid || c.Logon.Application.Id == WellKnownIdentifiers.AdminId)
                    .Where(c => c.Logon.LastActivityDate >= period.StartingOn)
                    .Select(c => new
                    {
                        c.Logon.Id,
                        c.Logon.UserName,
                        c.FirstName,
                        c.LastName,
                        c.BusinessName,
                        c.Logon.LastActivityDate
                    })
                    .OrderBy(u => u.UserName);

                var data = (await query.ToArrayAsync(cancellation)).Select(u => new
                {
                    UserId = u.Id,
                    Email = u.UserName.ToLower(),
                    CompositeName =
                    PartyExtensions.BuildCompositeName(u.FirstName, u.LastName, String.IsNullOrWhiteSpace(u.BusinessName) ? u.UserName : u.BusinessName),
                    LastActivityDescription = u.LastActivityDate.DescribeDifference(DateTime.Now),
                    Links = new
                    {
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(u.Id)
                    }
                });

                var jsonNetResult = new JsonNetResult
                {
                    Data = data
                };
                return jsonNetResult;
            }
        }

        #endregion
    }
}