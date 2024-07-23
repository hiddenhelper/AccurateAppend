using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Accounting.Data;
using Contact = AccurateAppend.Accounting.Contact;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// <see cref="BillFormatter"/> used to create "standard" bills.
    /// </summary>
    public class BasicBillFormatter : HtmlBillFormatter
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBillFormatter"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public BasicBillFormatter(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the formatter should add public download links for the file content.
        /// </summary>
        /// <value>True if links to the clients application orders screen should be added; Otherwise false.</value>
        public Boolean IncludeDownloadLink { get; set; }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override async Task<IEnumerable<MailAddress>> CreateTo(BillModel bill)
        {
            var userId = bill.UserId;

            var userQuery = this.context
                .SetOf<User>()
                .Where(u => u.Id == userId)
                .Select(u => u.UserName);

            var contactQuery = this.context
                .SetOf<Contact>()
                .Where(c => c.Client.Logon.Id == userId)
                .Where(c => c.Billing)
                .Select(c => c.EmailAddress);

            var emails = await userQuery.Concat(contactQuery)
                .Distinct()
                .ToArrayAsync()
                .ConfigureAwait(false);

            return emails.Select(e => new MailAddress(e));
        }

        /// <inheritdoc />
        protected override String CreateContentIntroBlock(Order order)
        {
            var sb = new StringBuilder();

            sb.AppendLine(this.IncludeDownloadLink
                ? ReceiptTemplate.ContentIntroBlockWithDownload
                : ReceiptTemplate.ContentIntroBlock);

            var report = order.Processing.Report == null
                ? null
                : new ProcessingReport(order.Processing.Report);

            if (report != null)
            {
                sb.AppendLine(this.BuildFileDetailsBlock(DateTime.UtcNow,
                    report.TotalRecords,
                    order.Deal.Title));

                sb.AppendLine(this.BuildProcessingReportBlock(report));
            }

            return sb.ToString();
        }

        /// <inheritdoc />
        protected override async Task<SiteCache.SiteInfo> GetSite(BillModel bill)
        {
            var applicationId = await this.context
                .SetOf<User>()
                .Where(u => u.Id == bill.UserId)
                .Select(u => u.Application.Id)
                .FirstAsync()
                .ConfigureAwait(false);

            var site = SiteCache.Cache.FirstOrDefault(s => s.ApplicationId == applicationId) ??
                       SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            return site;
        }

        /// <inheritdoc />
        public override async Task<String> CreateFooter(BillModel bill)
        {
            var site = await this.GetSite(bill).ConfigureAwait(false);

            return DomainModel.EmailTemplates.AccurateAppend.CreateFooter(site);
        }

        #endregion

        #region Helpers

        protected virtual String BuildFileDetailsBlock(DateTime dateCompleted, Int32 totalRecords, String customerFileName)
        {
            return String.Format(ReceiptTemplate.ContentFileDetailsBlock,
                FormatDate(dateCompleted),
                totalRecords.ToString("G", CultureInfoHelper.SystemCulture),
                HttpUtility.HtmlEncode(customerFileName));
        }

        protected static String FormatDate(DateTime value)
        {
            return $"{value.ToBillingZone():MM-dd-yy h:mm tt} {Core.DateTimeExtensions.BillingZone().StandardName}";
        }

        protected static String FormatPercentage(Double value)
        {
            return value.ToString("P1", CultureInfoHelper.SystemCulture);
        }

        protected virtual String BuildProcessingReportBlock(ProcessingReport processingReport)
        {
            var sb = new StringBuilder();

            #region Summary

            sb.AppendLine(ReceiptTemplate.ContentProcessingSummaryBlockStart);

            foreach (var operation in processingReport.Operations.Where(o => !o.Name.IsPreference()).Select(o => o.Name).Distinct())
            {
                var matches = processingReport.CalculateMatchCount(operation);
                var rate = processingReport.CalculateMatchRate(operation);

                sb.AppendLine(this.BuildProcessingReportSummaryItem(operation, matches, rate));
            }

            sb.AppendLine(ReceiptTemplate.ContentProcessingSummaryBlockEnd);

            #endregion

            #region Details

            sb.AppendLine(ReceiptTemplate.ContentProcessingDetailBlockStart);

            foreach (var operation in processingReport.Operations.Where(o => !o.Name.IsPreference()).GroupBy(o => o.Name))
            {
                sb.AppendLine(this.BuildProcessingReportDetailGroup(processingReport, operation));
            }

            sb.AppendLine(ReceiptTemplate.ContentProcessingDetailBlockEnd);

            #endregion

            return sb.ToString();
        }

        protected virtual String BuildProcessingReportSummaryItem(DataServiceOperation operation, Int32 matches, Double rate)
        {
            return String.Format(ReceiptTemplate.ContentProcessingSummaryBlockItem,
                operation.GetDescription(),
                matches.ToString("G", CultureInfoHelper.SystemCulture),
                FormatPercentage(rate));
        }

        protected virtual String BuildProcessingReportDetailGroup(ProcessingReport processingReport, IGrouping<DataServiceOperation, OperationReport> grouping)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"
     <tr>
      <td height=""1"" bgcolor=""#dddddd"" colspan=""7"" style=""font-size: 1px; line-height: 1px;""></td>
     </tr>");

            var operation = grouping.Key;

            foreach (var groupedMatchType in grouping.GroupBy(o => o.MatchLevel.GetCategoryDescription()).Select((g, i) => new { Index = i, MatchType = g.Key, Operations = g.ToArray() }))
            {
                var count = groupedMatchType.Operations.Sum(o => o.Count);
                var matchRate = groupedMatchType.Operations.Sum(o => o.CalculateMatchRate(processingReport.TotalRecords)); // you can do this as they have matching denominators and we're grouping by ML

                if (groupedMatchType.Index == 0)
                {
                    sb.AppendLine(this.BuildProcessingReportDetailItemHeader(operation, groupedMatchType.MatchType, count, matchRate));
                }
                else
                {
                    sb.AppendLine(this.BuildProcessingReportDetailItemValue(groupedMatchType.MatchType, count, matchRate));
                }
            }

            var totalMatches = grouping.Sum(o => o.Count);
            var totalMatchRate = grouping.Sum(o => o.CalculateMatchRate(processingReport.TotalRecords));

            if (operation == DataServiceOperation.EMAIL_VER_DELIVERABLE || operation == DataServiceOperation.EMAIL_VERIFICATION)
            {
                var deliverable = grouping.Select(o => o).Where(o => o.QualityLevel == QualityLevel.A).Sum(o => o.Count);
                var undeliverable = grouping.Select(o => o).Where(o => o.QualityLevel == QualityLevel.E).Sum(o => o.Count);

                sb.AppendLine(this.BuildProcessingReportDetailItemEmailDeliverability(deliverable, undeliverable));
            }

            sb.AppendLine(this.BuildProcessingReportDetailItemTotals(totalMatches, totalMatchRate));

            return sb.ToString();
        }

        protected virtual String BuildProcessingReportDetailItemHeader(DataServiceOperation operation, String matchType, Int32 count, Double matchRate)
        {
            return String.Format(ReceiptTemplate.ContentProcessingDetailItemFirst,
                operation.GetDescription(),
                matchType,
                count.ToString("G", CultureInfoHelper.SystemCulture),
                FormatPercentage(matchRate));
        }

        protected virtual String BuildProcessingReportDetailItemValue(String matchType, Int32 count, Double matchRate)
        {
            return String.Format(ReceiptTemplate.ContentProcessingDetailItemSubsequent,
                matchType,
                count.ToString("G", CultureInfoHelper.SystemCulture),
                FormatPercentage(matchRate));
        }

        protected virtual String BuildProcessingReportDetailItemEmailDeliverability(Int32 deliverable, Double undeliverable)
        {
            var sb = new StringBuilder(2);

            sb.AppendLine(String.Format(ReceiptTemplate.ContentProcessingDetailItemDeliverability, VerificationStatus.Verified.GetDeliveryDescription(), deliverable));
            sb.AppendLine(String.Format(ReceiptTemplate.ContentProcessingDetailItemDeliverability, VerificationStatus.Undeliverable.GetDeliveryDescription(), undeliverable));

            return sb.ToString();
        }

        protected virtual String BuildProcessingReportDetailItemTotals(Int32 totalMatches, Double totalMatchRate)
        {
            return String.Format(ReceiptTemplate.ContentProcessingDetailItemTotals, totalMatches, FormatPercentage(totalMatchRate));
        }

        #endregion
    }
}