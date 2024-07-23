using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// View Model used for bill content editing and creation.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Bill: {" + nameof(DealId) + "}")]
    public class BillModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BillModel"/> class.
        /// </summary>
        public BillModel()
        {
            this.Attachments = new CommonAttachments();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillModel"/> class.
        /// </summary>
        public BillModel(Order order) : this()
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Id == null || order.Deal.Id == null) throw new ArgumentOutOfRangeException(nameof(order), null, $"Orders must have an {nameof(order.Id)} value prior to using this model");

            this.UserId = order.Deal.Client.UserId;
            this.DealId = order.Deal.Id.Value;
            this.PublicKey = new Guid(order.PublicKey);
            this.OrderId = order.Id.Value;
            this.Title = order.Deal.Title;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier of the deal to build a bill for.
        /// </summary>
        /// <value>The identifier of the deal to build a bill for.</value>
        [Required()]
        //[RequiredGreaterThanZero()]
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the order in the deal to build a bill for.
        /// </summary>
        /// <value>The identifier of the order in the deal to build a bill for.</value>
        [Required()]
        //[RequiredGreaterThanZero()]
        public Int32 OrderId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the owner of the deal.
        /// </summary>
        /// <value>The identifier of the owner of the deal.</value>
        [Required()]
        //[RequiredNotEmptyGuid()]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the title of the deal being billed.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets the public key identifier of the order.
        /// </summary>
        public Guid PublicKey { get; set; }

        /// <summary>
        /// Gets the set of common system attachments that should be included with the bill.
        /// </summary>
        [Required()]
        public CommonAttachments Attachments { get; }

        #endregion
    }
}