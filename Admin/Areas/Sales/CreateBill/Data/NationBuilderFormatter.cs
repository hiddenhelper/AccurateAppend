using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Security;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// Creates a bill appropriate for a <see cref="PushRequest"/>.
    /// </summary>
    /// <remarks>
    /// All use of this formatter assumes Accurate Append formatting and customization. 
    /// </remarks>
    public class NationBuilderFormatter : IBillFormatter
    {
        #region Fields

        private readonly PushRequest push;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderFormatter"/> class.
        /// </summary>
        /// <param name="push">The <see cref="PushRequest"/> to a receipt for.</param>
        public NationBuilderFormatter(PushRequest push)
        {
            if (push == null) throw new ArgumentNullException(nameof(push));

            this.push = push;
        }

        #endregion

        #region IBillFormatter Members

        /// <inheritdoc />
        /// <remarks>
        /// Always returns true.
        /// </remarks>
        public Boolean IsHtml => true;

        /// <inheritdoc />
        public virtual Task<MailAddress> SendFrom(BillModel bill)
        {
            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            return Task.FromResult(new MailAddress(site.MailboxSupport));
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MailAddress>> CreateTo(BillModel bill)
        {
            var userName = this.push.For.Owner.UserName;

            return Task.FromResult<IEnumerable<MailAddress>>(new[] { new MailAddress(userName) });
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MailAddress>> CreateBcc(BillModel bill)
        {
            return Task.FromResult(Enumerable.Empty<MailAddress>());
        }

        /// <inheritdoc />
        public virtual Task<String> CreateSubject(BillModel bill)
        {
            var subject = $"Order Complete - {bill.Title}";

            return Task.FromResult(subject);
        }

        /// <inheritdoc />
        public virtual Task<String> CreateHeader(BillModel bill)
        {
            var value = @"
<!DOCTYPE html>

<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta charset='utf-8' />
    <title></title>
</head>
<body style='font-family: 'opensans-regular', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 1.2; background-color: #fff; margin: 10px;' bgcolor='#fff'>";

            return Task.FromResult(value);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Only the <see cref="CommonAttachments.NationBuilderProcessingOptions"/> option is supported. Anything else is ignored.
        /// </remarks>
        public virtual Task<String> CreateBody(Order bill, CommonAttachments attachments = null)
        {
            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            var sb = new StringBuilder();

            sb.AppendLine($@"
    <div style='width: 600px; border: solid silver 1px; padding: 10px;'>
        <div style='background-color: #5CB4E3; padding: 10px;'>
            <img src='https://clients.accurateappend.com/content/AccurateAppend_v7/images/Receipts/logo.png' alt='Accurate Append' style='width:195px;height:34px;' />
        </div>
        <p>Your list {this.push.List.Name} has been processed and information regarding your order is detailed below.</p>");

            sb.AppendLine(this.BuildListInformationBlock());

            sb.AppendLine(this.BuildProcessingReportBlock(bill, attachments?.NationBuilderProcessingOptions ?? false));

            sb.AppendLine($@"<p>For any questions please contact your Account Executive or Customer Support at {site.PrimaryPhone} 9-4PM PST, or email {site.MailboxSupport} any time.</p>");

            sb.AppendLine($@"<p>
Accurate Append Inc <br />
{site.Address}<br />
{site.City}, {site.State} {site.Zip}<br />
{site.PrimaryPhone}<br />
{site.MailboxSupport}<br />
{site.Website}
</p>");

            sb.AppendLine(this.BuildDisclaimerBlock());

            sb.AppendLine(@"
    </div>");

            return Task.FromResult(sb.ToString());
        }

        /// <inheritdoc />
        public virtual Task<String> CreateFooter(BillModel bill)
        {
            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            return Task.FromResult(DomainModel.EmailTemplates.AccurateAppend.CreateFooter(site));
        }

        #endregion

        #region Helpers

        protected virtual String BuildListInformationBlock()
        {
            var sb = new StringBuilder();

            sb.AppendLine($@"
   <table style='border-spacing: 0;border-collapse: collapse;'>
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>List Name</td>
      <td style='padding: 5px;border: solid silver 1px;'>{this.push.List.Name}</td>
     </tr>
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>Input Records</td>
      <td style='padding: 5px;border: solid silver 1px;'>{this.push.List.TotalRecords.ToString("G", CultureInfoHelper.SystemCulture)}</td>
    </table>");

            return sb.ToString();
        }

        protected virtual String BuildProcessingReportBlock(Order bill, Boolean processingLink)
        {
            var sb = new StringBuilder();

            if (
                // If the order exceeds the minimums
                (bill.Total() > bill.OrderMinimum && bill.OrderMinimum > 0)
                ||
                // Or just has and optional discount, then proceed normally
                bill.OrderMinimum <= 0
                )
            {
                sb.AppendLine(@"
    <h4>Order Details</h4>
    <table style='border-spacing: 0;border-collapse: collapse;'>
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>Description</td>
      <td style='padding: 5px;border: solid silver 1px;'>Matches</td>
      <td style='padding: 5px;border: solid silver 1px;'>Cost/Total (USD)</td>
     </tr>");

                foreach (var line in bill.Lines.GroupBy(i => new { i.Description, i.Price }))
                {
                    sb.AppendLine($@"
    <tr>
    <td style='padding: 5px;border: solid silver 1px;'>{line.Key.Description}</td>
    <td style='padding: 5px;border: solid silver 1px;'>{line.Sum(l => l.Quantity).ToString("G", CultureInfoHelper.SystemCulture)}</td>
    <td style='padding: 5px;border: solid silver 1px;'>{line.Key.Price.ToString("C4", CultureInfoHelper.SystemCulture)} / {line.Sum(l => l.Total()).RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)}</td>
    </tr>");
                }
            }
            else
            {
                sb.AppendLine(@"
    <h4>Order Details</h4>
    <table style='border-spacing: 0;border-collapse: collapse;'>
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>Description</td>
      <td style='padding: 5px;border: solid silver 1px;'>Matches</td>
     </tr>");

                foreach (var line in bill.Lines.GroupBy(i => i.Description))
                {
                    sb.AppendLine($@"
    <tr>
     <td style='padding: 5px;border: solid silver 1px;'>{line.Key}</td>
     <td style='padding: 5px;border: solid silver 1px;'>{line.Sum(l => l.Quantity).ToString("G", CultureInfoHelper.SystemCulture)}</td>
    </tr>");
                }
            }

            // Include a credit if needed
            if (bill.OrderMinimum < 0)
            {
                sb.AppendLine($@"
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>[CREDIT]</td>
      <td style='padding: 5px;border: solid silver 1px;'></td>
      <td style='padding: 5px;border: solid silver 1px;'>{bill.OrderMinimum.RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)}</td>
     </tr>");
            }

            // Include a minimum if needed
            if (bill.OrderMinimum > bill.Lines.Total())
            {
                sb.AppendLine($@"
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>[ORDER MINIMUM]</td>
      <td style='padding: 5px;border: solid silver 1px;'></td>
      <td style='padding: 5px;border: solid silver 1px;'>{bill.OrderMinimum.RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)}</td>
     </tr>");
            }

            sb.AppendLine($@"
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>Total</td>
      <td style='padding: 5px;border: solid silver 1px;'></td>
      <td style='padding: 5px;border: solid silver 1px;'>{bill.Total().RoundFractionalPennies().ToString("C2", CultureInfoHelper.SystemCulture)}</td>
     </tr>");

            // Include attachment link if indicated
            if (processingLink)
            {
                sb.AppendLine($@"
     <tr>
      <td style='padding: 5px;border: solid silver 1px;'>Download Files</td>
      <td style='padding: 5px;border: solid silver 1px;' colspan='2'>
<a href='https://clients.accurateappend.com/content/files/Processing options for NationBuilder users.pdf'>Processing options for NationBuilder</a>
      </td>
     </tr>");

                
            }

            sb.AppendLine(@"
    </table>");
            
            return sb.ToString();
        }

        protected virtual String BuildDisclaimerBlock()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
<h4>TERMS AND CONDITIONS</h4>");

            sb.AppendLine(@"
<p>1. CONFIDENTIALITY<br />
Accurate Append Inc and Customer mutually agree to keep all business dealings confidential, including but not limited to pricing, products, services, systems used by Accurate Append Inc to perform or provide services to Customer. Customer further agrees to keep confidential Accurate Append Inc as Customer's source for any services performed and provided to Customer. All Customer files, data, and other communication by Customer to Accurate Append Inc will be kept strictly confidential by Accurate Append Inc, except as necessary to perform or provide services ordered by Customer.
</p>");

            sb.AppendLine(@"
<p>2. PRIVACY OF CUSTOMER DATA<br />
Accurate Append Inc does not cache any customer data and customer data is never made available to any third party or used for any purpose other than to complete the services ordered by Customer. All Customer files, both input and output are deleted 30 (thirty) days after files are completed, except Customer may request and Accurate Append Inc shall set an automatic or manual deletion of all files after completion to comply with secure information or HIPPA information processing guidelines.
</p>");

            sb.AppendLine(@"
<p>3. NO WARRANTY<br />
Accurate Append Inc provides all data, content, information and services on an “as is” basis, with no warranties, including without limitation, merchantability and quality, fitness for a particular purpose, title and non-infringement. Accurate Append Inc does not warrant that access to its data, content, information, and services will be uninterrupted or error-free, or that any errors will be corrected.
</p>");

            sb.AppendLine(@"
<p>4. LIMITATION OF LIABILITY<br />
In no event shall either party, its officers, directors, employees or agents be liable for any indirect, special, incidental, exemplary or consequential damages (including damages for loss of business, loss of profits, or the like), whether based on breach of contract, tort (including negligence), strict liability, breach of warranty or otherwise, even if the other party has been advised of the possibility of such damages and even if a remedy set forth herein is found to have failed of its essential purpose either party’s aggregate liability shall not exceed the total fee paid or payable to Accurate Append Inc
</p>");

            sb.AppendLine(@"
<p>5. INDEMNIFICATION<br />
Customer shall indemnify and hold harmless Accurate Append Inc, its officers, directors, employees and agents from any and all third party claims, liabilities, losses, damages, expenses or costs (including but not limited to defense costs and attorney’s fees) arising out of Customers use of any data, content, information or services provided to Customer by Accurate Append Inc.Accurate Append Inc shall indemnify and hold harmless Customer and its affiliates and suppliers against all claims, losses, damages, liabilities, costs and expenses, including reasonable attorneys’ fees, which Customer may incur as a result of claims relating to the infringement by Accurate Append Inc of any third party patent, copyright, trademark or trade secret.
</p>");

            sb.AppendLine(@"
<p>6. LAWFUL USE<br />
Customer acknowledges their sole responsibility to comply with any and all laws or regulations, Federal, State or otherwise, for the lawful use of any data, content, information or services provided to Customer by Accurate Append Inc, including but not limited to any Mobile telephone numbers provided to Customer.
</p>");

            sb.AppendLine(@"
<p>7. DO NOT CALL/DO NOT MARKET<br />
Accurate Append Inc does not process its databases, appended data or other content provisioned or provided to Customer, including no processing of Customer data against the National Do Not Call Registry, State Phone Suppression files and DMA Phone Suppression files, herein collectively called “Telephone Number Suppression files” and any Do Not Market suppression files. Customer acknowledges their sole responsibility to comply with all Do Not Call and / or Do Not Market laws and regulations, including that Customer will either obtain access to the Do Not Call Registry and other Telephone Number Suppression files on its own behalf, or if Customer will not obtain access to Telephone Number Suppression files, Customer will only make calls for purposes permitted by law. Customer also acknowledges their sole responsibility to comply with the CAN SPAM ACT for all electronic messaging.
</p>");

            return sb.ToString();
        }

        #endregion
    }
}