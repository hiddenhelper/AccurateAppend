using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Sales;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Clients.Data
{
    public static class OrderExtensions
    {
        /// <summary>
        /// Crafts a query predicate that can acquire the Order for the current interactive user.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Order"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the Orders for the current interactive user.</returns>
        public static IQueryable<T> ForInteractiveUser<T>(this IQueryable<T> queryable) where T : Order
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Order>>() != null);
            Contract.EndContractBlock();

            var userId = Thread.CurrentPrincipal.Identity.GetIdentifier();
            return queryable.Where(o => o.UserId == userId);
        }

        public static String DescribeStatus(this BatchOrder order)
        {
            if (order.JobStatus == JobStatus.Complete) return "Complete";
            if (order.JobStatus == JobStatus.InQueue || order.JobStatus ==  JobStatus.BeingSliced) return "Accepted (Step 1 of 3)";
            if (order.JobStatus == JobStatus.WaitingToBeRecombined || order.JobStatus == JobStatus.EmailVerifyComplete) return "Packaging  (Step 3 of 3)";

            return "Processing  (Step 2 of 3)";
        }

        public static String DescribeStatus(this DirectClientOrder order)
        {
            if (order.JobStatus == JobStatus.Complete && order.OrderStatus == ProcessingStatus.Available) return "Complete";

            if (order.OrderStatus == ProcessingStatus.Billing) return "Billing  (Step 3 of 3)";

            if (order.JobStatus == JobStatus.WaitingToBeRecombined || order.JobStatus == JobStatus.EmailVerifyComplete) return "Packaging  (Step 2 of 3)";

            if (order.JobStatus == JobStatus.InQueue || order.JobStatus == JobStatus.BeingSliced) return "Accepted (Step 1 of 3)";

            return "Processing  (Step 2 of 3)";
        }
        
        public static String DescribeStatus(this NationBuilderOrder order)
        {
            switch (order.PushStatus)
            {
                case PushStatus.Pending:
                    if (order.CurrentPage == 0) return "Accepted";
                    return "Downloading (Step 1 0f 4)";
                case PushStatus.Acquired:
                    return "Downloaded (Step 1 0f 4)";
                case PushStatus.Processing:
                    return "Processing (Step 2 0f 4)";
                case PushStatus.Pushing:
                    if (order.CurrentPage == 0) return "Quality Assurance (Step 3 0f 4)";
                    return "Uploading (Step 4 0f 4)";
                case PushStatus.Failed:
                    return "In Review";
                case PushStatus.Review:
                    return "Processing (Step 2 0f 4)";
                default:
                    return order.PushStatus.GetDescription();
            }
        }
    }
}