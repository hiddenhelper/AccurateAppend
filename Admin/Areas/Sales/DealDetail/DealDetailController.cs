using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.ReadModel.Queries;
using AccurateAppend.Websites.Admin.Areas.Sales.DealDetail.Models;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel;

namespace AccurateAppend.Websites.Admin.Areas.Sales.DealDetail
{
    /// <summary>
    /// Controller performing detail operation of <see cref="DealBinder"/> entities.
    /// </summary>
    [Authorize()]
    public class DealDetailController : ActivityLoggingController2
    {
        #region Fields

        private readonly IDealsViewByIdQuery dal;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealDetailController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IDealsViewByIdQuery"/> DAL component.</param>
        /// <param name="encryption">The <see cref="IEncryptor"/> configured to use the shared key for communication with the storage system.</param>
        public DealDetailController(IDealsViewByIdQuery dal, IEncryptor encryption)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            Contract.EndContractBlock();

            this.dal = dal;
            this.encryption = encryption;
        }

        #endregion
        
        #region Actions

        /// <summary>
        /// Performs the action to display details of a deal.
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult> Index(Int32 dealId, CancellationToken cancellation)
        {
            this.OnEvent($"Deal viewed: {dealId}");

            var deal = await this.dal.ForId(dealId)
                .Select(d => new
                    {
                        d.DealId,
                        d.UserId,
                        d.PublicKey,
                        d.Status
                    }
                )
                .FirstOrDefaultAsync(cancellation);

            if (deal == null) return this.DisplayErrorResult("The deal does not exist ");

            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif

            var request = new UploadRequest(this.Url.Action("InvokePostback", "UserFiles", new { Area = "Clients", deal.UserId }, scheme));
            request.CorrelationId = deal.PublicKey;

            var uri = request.CreateRequest(this.encryption);

#if DEBUG
            //uri = request.CreateRequest(this.encryption, UploadRequest.Local); // Uncomment to switch to a local VS instance of the STORAGE app           
#endif
            var model = new DealDetailView()
            {
                DealId = deal.DealId,
                UserId = deal.UserId,
                PublicKey = deal.PublicKey,
                UploadFileLink = deal.Status == DealStatus.Complete ? null : uri.ToString(),
                AssociateFileLink = deal.Status == DealStatus.Complete ? null : this.Url.Action("Index", "DealFiles", new {Area = "Sales", publicKey = deal.PublicKey})
            };
            
            return this.View(model);
        }

        /// <summary>
        /// Performs the action to display details of a deal.
        /// </summary>
        [HttpGet()]
        public async Task<ActionResult> PublicKey(Guid id, CancellationToken cancellation)
        {
            var dealId = await this.dal.ForId(id).Select(d => (Int32?)d.DealId).FirstOrDefaultAsync(cancellation);
            if (dealId == null) return this.DisplayErrorResult("The deal does not exist ");

            return this.RedirectToAction("Index", "DealDetail", new {Area = "Sales", dealId});
        }

        #endregion
    }
}
