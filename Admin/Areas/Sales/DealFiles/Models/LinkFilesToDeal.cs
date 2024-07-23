using System;
using AccurateAppend.Operations;
using AccurateAppend.Sales;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealFiles.Models
{
    /// <summary>
    /// Model used to relate a <see cref="DealBinder"/> to a <see cref="UserFile"/>.
    /// </summary>
    [Serializable()]
    public class LinkFilesToDeal
    {
        /// <summary>
        /// The identifier of the user the deal is for.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The public key of the deal.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// The title of the deal.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// The identifier of the deal.
        /// </summary>
        public Int32 DealId { get; set; }
    }
}