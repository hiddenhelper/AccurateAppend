using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.ViewModels;
using DomainModel.Enum;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Models
{
    /// <summary>
    /// View Model used for bill content editing and creation.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Bill: {" + nameof(DealId) + "}")]
    public class BillViewModel : BillModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BillViewModel"/> class.
        /// </summary>
        public BillViewModel()
        {
            this.ReceiptTemplateName = ReceiptTemplateName.IndHh;
            this.AdminFiles = new List<File>();
            this.ClientFiles = new List<File>();
            this.Content = new BillMessage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillViewModel"/> class.
        /// </summary>
        public BillViewModel(Order order) : base(order)
        {
            this.ReceiptTemplateName = ReceiptTemplateName.IndHh;
            this.AdminFiles = new List<File>();
            this.ClientFiles = new List<File>();
            this.Content = new BillMessage();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of billing structure the current instance should be generated as.
        /// </summary>
        /// <value>The type of billing structure the current instance should be generated as.</value>
        [Required()]
        public BillType BillType { get; set; }
        
        /// <summary>
        /// Gets or sets the type of receipt template that is used to generate the bill for.
        /// </summary>
        /// <value>The type of receipt template that is used to generate the bill for.</value>
        [Required()]
        public ReceiptTemplateName ReceiptTemplateName { get; set; }

        /// <summary>
        /// Gets the list of job processing output files.
        /// </summary>
        public IList<File> ClientFiles { get; }

        /// <summary>
        ///  Gets the list of admin uploaded client files.
        /// </summary>
        public IList<File> AdminFiles { get; }

        /// <summary>
        /// Gets the bill message content data.
        /// </summary>
        public BillMessage Content { get; }

        /// <summary>
        /// Indicates whether all attached files should be compressed before attachment.
        /// </summary>
        /// <value>True if the current bill should have all attached files compressed; otherwise false.</value>
        [Required()]
        public Boolean ZipFiles { get; set; }

        #endregion
    }
}