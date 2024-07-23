using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using AccurateAppend.Sales.Contracts.ViewModels;

namespace AccurateAppend.Sales.Contracts.Services
{
    /// <summary>
    /// Represents a component that can create <see cref="BillContent"/> values for a specific use case.
    /// </summary>
    /// <remarks>
    /// This component is designed at the lowest level of granularity so that the individual bill creation
    /// UI steps can access just the content needed.
    /// </remarks>
    public interface IBillFormatter
    {
        /// <summary>
        /// Indicates whether the current formatter instance supplies html or pre-formatted text.
        /// </summary>
        /// <returns>True if the content is HTML; Otherwise false.</returns>
        Boolean IsHtml { get; }

        /// <summary>
        /// Determines the appropriate mail address to send from.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>The <see cref="MailAddress"/> that the bill should be send from.</returns>
        Task<MailAddress> SendFrom(BillModel bill);

        /// <summary>
        /// Determines the appropriate mail addresses to send to.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>The set of email addresses to send the bill to.</returns>
        Task<IEnumerable<MailAddress>> CreateTo(BillModel bill);

        /// <summary>
        /// Determines the appropriate mail addresses to send to.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>The set of email addresses that should have a BCC.</returns>
        Task<IEnumerable<MailAddress>> CreateBcc(BillModel bill);

        /// <summary>
        /// Determines the appropriate subject line for the bill.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>The subject title.</returns>
        Task<String> CreateSubject(BillModel bill);

        /// <summary>
        /// Allows a specific formatter to wrap the body content with a header or intro, if desired.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>Any header or intro text.</returns>
        Task<String> CreateHeader(BillModel bill);

        /// <summary>
        /// Determines the appropriate bill content.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to create from.</param>
        /// <param name="attachments">The optional <see cref="CommonAttachments"/> that are included in the bill.</param>
        /// <returns>The bill content.</returns>
        Task<String> CreateBody(Order order, CommonAttachments attachments = null);

        /// <summary>
        /// Allows a specific formatter to wrap the body content with a footer or outro, if desired.
        /// </summary>
        /// <param name="bill">The <see cref="BillModel"/> to create from.</param>
        /// <returns>Any footer or outro text.</returns>
        Task<String> CreateFooter(BillModel bill);
    }
}