using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Queries;
using DomainModel.ReadModel;
using Kendo.Mvc;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary
{
    /// <summary>
    /// Controller for displaying lead summary information.
    /// </summary>
    [Authorize()]
    public class LeadSummaryController : Controller
    {
        #region Fields

        private readonly ILeadsViewQuery dal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadSummaryController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="ILeadsViewQuery"/> component that provides data access.</param>
        public LeadSummaryController(ILeadsViewQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Index()
        {
            return this.View();
        }

        public virtual async Task<ActionResult> Query([DataSourceRequest] DataSourceRequest request, Guid applicationid, DateTime startdate, DateTime enddate, LeadSource? source, LeadStatus? status, LeadQualification? qualified, bool displayUnworkedLeads, CancellationToken cancellation)
        {
            if (request.Sorts == null || !request.Sorts.Any()) request.Sorts = new List<SortDescriptor> { new SortDescriptor(nameof(LeadView.LastUpdate), ListSortDirection.Descending) };

            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var query = this.dal.ActiveDuring(applicationid, startdate, enddate);

            if (source != null) query = query.Where(l => l.LeadSource == source.Value);
            if (status != null) query = query.Where(l => l.Status == status.Value);
            if (qualified != null) query = query.Where(l => l.Qualified == qualified.Value);

            var records = (await query.ToArrayAsync(cancellation)).Select(l => new
            {
                l.Address,
                l.ApplicationId,
                l.ApplicationTitle,
                l.BusinessName,
                l.City,
                l.CompositeName,
                l.ContactMethod,
                DateAdded = l.DateAdded.Coerce().ToUserLocal(),
                l.Email,
                l.FirstName,
                FollowUpDate = l.FollowUpDate.HasValue
                    ? new DateTime?(l.FollowUpDate.Value.Coerce().ToUserLocal())
                    : null,
                l.LeadSource,
                l.LastName,
                LastUpdate = l.LastUpdate.Coerce().ToUserLocal(),
                l.LastUpdateDescription,
                l.LeadId,
                l.LeadStatusDescription,
                l.NoteCount,
                l.Phone,
                l.Qualified,
                l.QualifiedDescription,
                l.Status,
                l.State,
                l.Website,
                l.Score,
                l.Zip,
                Owner = new
                {
                    UserId = l.OwnerId,
                    UserName = l.OwnerUserName
                },
                DetailUrl = this.Url.BuildFor<LeadDetailController>().ToDetail(l.LeadId)
            }).ToArray();

            if (displayUnworkedLeads)
                records = records.Where(a => a.NoteCount == 0 && a.Status != LeadStatus.ConvertedToCustomer).ToArray();

            var data = Kendo.Mvc.Extensions.QueryableExtensions.ToDataSourceResult(records, request, o => o);

            data.Total = records.Length;

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local) {Data = data};

            return jsonNetResult;
        }

        /// <summary>
        /// Downloads list of leads from database
        /// </summary>
        public virtual async Task<ActionResult> Download(DateTime startdate, DateTime enddate, Guid applicationid, CancellationToken cancellation)
        {
            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var report = new StringBuilder();

            #region Header

            report.AppendLine(
                    "LeadId," +
                    "DateAdded," +
                    "Status," +
                    "Address," +
                    "City," +
                    "State," +
                    "Zip," +
                    "Phone," +
                    "Email," +
                    "BusinessName," +
                    "FirstName," +
                    "LastName," +
                    "WebSite," +
                    "Score," +
                    "ContactMethod," +
                    "LeadSourceCategory," +
                    "Qualified," +
                    "Site," +
                    "LeadUrl," +
                    "DNM," +
                    "LeadSource,");

            #endregion

            var builder = this.Url.BuildFor<LeadDetailController>();
            var query = this.dal.ActiveDuring(applicationid, startdate, enddate).OrderByDescending(l => l.DateAdded);

            await query.ForEachAsync(lead =>
            {
                #region Row

                // TODO: Change this over to a string writer and csv writer

                report.AppendLine("\"" + lead.LeadId + "\"" + ',' + "\"" +
                                  lead.DateAdded + "\"" + ',' + "\"" +
                                  lead.Status.GetDescription() + "\"" + ',' + "\"" +
                                  lead.Address + "\"" + ',' + "\"" +
                                  lead.City + "\"" + ',' + "\"" +
                                  lead.State + "\"" + ',' + "\"" +
                                  lead.Zip + "\"" + ',' + "\"" +
                                  lead.Phone + "\"" + ',' + "\"" +
                                  lead.Email + "\"" + ',' + "\"" +
                                  lead.BusinessName + "\"" + ',' + "\"" +
                                  lead.FirstName + "\"" + ',' + "\"" +
                                  lead.LastName + "\"" + ',' + "\"" +
                                  lead.Website + "\"" + ',' + "\"" +
                                  lead.Score + "\"" + ',' + "\"" +
                                  lead.ContactMethod.GetDescription() + "\"" + ',' + "\"" +
                                  lead.LeadSource.GetDescription() + "\"" + ',' + "\"" +
                                  lead.Qualified.GetDescription() + "\"" + ',' + "\"" +
                                  lead.ApplicationTitle + "\"" + ',' + "\"" +
                                  builder.ToDetail(lead.LeadId, Uri.UriSchemeHttps) + "\"" + ',' + "\"" +
                                  lead.DoNotMarketTo + "\"" + ',' + "\"" +
                                  lead.LeadSource.GetDescription() + "\"");

                #endregion
            }, cancellation);

            // download to browser
            return this.File(Encoding.UTF8.GetBytes(report.ToString()), "text/csv", "Leads.csv");
        }

        [OutputCache(Duration = 20, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetLeadQualificationStatuses(DateTime startdate, DateTime enddate, Guid applicationid, LeadSource? source, LeadStatus? status, CancellationToken cancellation)
        {
            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var query = this.dal.ActiveDuring(applicationid, startdate, enddate);
            if (source != null) query = query.Where(l => l.LeadSource == source);
            if (status != null) query = query.Where(l => l.Status == status);

            var final = query.GroupBy(l => l.Qualified).Select(g => new {Description = g.Key, Cnt = g.Count()}).OrderBy(l => l.Description).Distinct();

            var jsonNetResult = new JsonNetResult
            {
                Data = await final.ToArrayAsync(cancellation)
            };

            return jsonNetResult;
        }

        [OutputCache(Duration = 20, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetLeadSources(DateTime startdate, DateTime enddate, Guid applicationid, CancellationToken cancellation)
        {
            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var query = this.dal.ActiveDuring(applicationid, startdate, enddate);

            var final = query.GroupBy(l => l.LeadSource).Select(g => new { Description = g.Key, Cnt = g.Count() }).OrderBy(l => l.Description).Distinct();

            var jsonNetResult = new JsonNetResult
            {
                Data = await final.ToArrayAsync(cancellation)
            };

            return jsonNetResult;
        }

        [OutputCache(Duration = 20, VaryByParam = "*", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> GetLeadStatuses(DateTime startdate, DateTime enddate, Guid applicationid, LeadSource? source, CancellationToken cancellation)
        {
            // Leads table is UTC so we ned to convert start/end dates
            startdate = startdate.ToStartOfDay().FromUserLocal().Coerce();
            enddate = enddate.ToEndOfDay().FromUserLocal().Coerce();

            var query = this.dal.ActiveDuring(applicationid, startdate, enddate);
            if (source != null) query = query.Where(l => l.LeadSource == source);

            var final = query.GroupBy(l => l.Status).Select(g => new { Description = g.Key, Cnt = g.Count() }).OrderBy(l => l.Description).Distinct();

            var jsonNetResult = new JsonNetResult
            {
                Data = await final.ToArrayAsync(cancellation)
            };

            return jsonNetResult;
        }

        #endregion
    }
}