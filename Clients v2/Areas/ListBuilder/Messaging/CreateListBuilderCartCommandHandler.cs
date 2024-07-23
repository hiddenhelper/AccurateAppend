using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Sales;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.Messaging
{
    /// <summary>
    /// Responds to the <see cref="CreateListBuilderCartCommand"/> to create a new List Builder based sales cart for a user.
    /// </summary>
    public class CreateListBuilderCartCommandHandler : IHandleMessages<CreateListBuilderCartCommand>
    {
        #region Fields

        private readonly Sales.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateListBuilderCartCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The <see cref="Sales.DataAccess.DefaultContext"/> providing data access to the handler.</param>
        public CreateListBuilderCartCommandHandler(Sales.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<CreateListBuilderCartCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(CreateListBuilderCartCommand message, IMessageHandlerContext context)
        {
            try
            {

                var userId = message.UserId;
                var cartId = message.CartId;

                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .SingleOrDefaultAsync(u => u.UserId == userId)
                    .ConfigureAwait(false);

                if (await this.dataContext.SetOf<Cart>().AnyAsync(c => c.Id == cartId))
                {
                    Trace.WriteLine($"Cart: {cartId} already exists. Exiting...");
                    return;
                }

                var cart = Cart.ForListbuilder(client, cartId, $"{cartId}.csv", 0);
                this.dataContext.SetOf<Cart>().Add(cart);

                await this.dataContext.SaveChangesAsync();

                var @event = new CartCreatedEvent();
                @event.CartId = cartId;
                @event.ForUser = userId;

                await context.Publish(@event);

            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                throw;
            }
        }

        #endregion
    }
}