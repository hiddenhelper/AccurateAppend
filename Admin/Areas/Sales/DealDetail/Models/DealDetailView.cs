using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealDetail.Models
{
    /// <summary>
    /// View model for displaying the details for a single deal.
    /// </summary>
    [Serializable()]
    public class DealDetailView
    {
        /// <summary>
        /// The identifier for the deal instance to view.
        /// </summary>
        public Int32 DealId { get; set; }

        /// <summary>
        /// The identifier for the owner of the deal.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The alternate public key identifier for the deal.
        /// </summary>
        public Guid PublicKey { get; set; }

        public String UploadFileLink { get; set; }

        public String AssociateFileLink { get; set; }
    }
}