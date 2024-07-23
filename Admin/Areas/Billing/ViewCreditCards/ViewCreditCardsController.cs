using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.ChargeProcessing.DataAccess;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards.Models;

namespace AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards
{
    [Authorize()]
    public class ViewCreditCardsController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

#if DEBUG
        private readonly Core.Utilities.IEncryptor encryption;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCreditCardsController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        /// <param name="encryption">Card decryption system.</param>
        public ViewCreditCardsController(DefaultContext context, Core.Utilities.IEncryptor encryption)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
            this.encryption = encryption;
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCreditCardsController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        public ViewCreditCardsController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }
#endif


        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Index(Guid userid, CancellationToken cancellation)
        {
            var query = this.context
                .Set<ClientRef>()
                .Where(c => c.UserId == userid)
                .Include(c => c.ChargePayments)
                .AsNoTracking();
            var client = await query.FirstOrDefaultAsync(cancellation);

            if (client == null) return this.DisplayErrorResult($"Client {userid} does not exist");

            var model = new ViewCreditCardsModel(client);
            
            return this.View(model);
        }

#if DEBUG
        public ActionResult Decrypt(String value, CancellationToken t)
        {
            var q = this.encryption.SymetricDecrypt(value);
            return new DomainModel.ActionResults.LiteralResult() {Data = q};
        }

        public ActionResult Encrypt(String value, CancellationToken t)
        {
            var q = this.encryption.SymetricEncrypt(value);
            return new DomainModel.ActionResults.LiteralResult() { Data = q };
        }
#endif
        #endregion
    }
}