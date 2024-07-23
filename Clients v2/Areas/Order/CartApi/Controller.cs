using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Areas.Order.CartApi
{
    /// <summary>
    /// Controller providing an API into <see cref="Cart"/> data.
    /// </summary>
    [Authorize()]
    public class CartApiController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        public CartApiController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Accesses the quote for given cart.
        /// </summary>
        public virtual async Task<ActionResult> Quote(Guid cartId, CancellationToken cancellation)
        {
            var cart = await this.context
                .SetOf<Cart>()
                .ForInteractiveUser()
                .Where(c => c.Id == cartId)
                .FirstOrDefaultAsync(cancellation);

            if (cart == null) return new JsonNetResult();

            var quote = new
            {
                CartId = cartId,
                OrderMinimum = cart.OrderMinimum(),
                SubTotal = cart.QuotedTotal(),
                Products = cart.QuotedProducts().Select(p => new
                {
                    Product = p.Item1,
                    Description = p.Item1.GetDescription(),
                    Price = p.Item3,
                    EstimatedMatches = p.Item2
                })
            };

            return new JsonNetResult()
            {
                Data = quote
            };
        }

        #endregion
    }
}