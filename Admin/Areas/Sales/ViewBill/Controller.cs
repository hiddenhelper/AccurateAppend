using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.ViewBill
{
    /// <summary>
    /// Controller to display a bill, i
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        [HttpGet()]
        public virtual async Task<ActionResult> Content(Int32 orderId, CancellationToken cancellation)
        {
            try
            {
                var bill = await this.context
                    .SetOf<BillContent>()
                    .Where(b => b.Order.Id == orderId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellation);
                if (bill == null) return new EmptyResult();

                if (bill.IsHtml) return this.Content(bill.Body, MediaTypeNames.Text.Html);

                return new LiteralResult(true) { Data = HttpUtility.HtmlEncode(bill.Body) };
            }
            catch (Exception ex)
            {
                return this.DisplayErrorResult(ex.Message);
            }
        }

        #endregion
    }
}