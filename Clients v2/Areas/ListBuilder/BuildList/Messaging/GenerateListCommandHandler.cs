using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.ListBuilder.DataSources.ConsumerProfile;
using AccurateAppend.ListBuilder.Models;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.BuildList.Messaging
{
    /// <summary>
    /// Handler designed to process the <see cref="GenerateListCommand"/> pair.
    /// </summary>
    /// <remarks>
    /// Responds to the request by
    /// 1. validating input criteria
    /// 2. producing the requested data
    /// 3. converting the data to CSV format to be stored in the configured location.
    /// 4. Start a new sales cart
    /// </remarks>
    public class GenerateListCommandHandler : IHandleMessages<GenerateListCommand>
    {
        #region Fields

        private readonly IDataAccess dal;
        private readonly IFileLocation temp;
        private readonly Sales.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateListCommandHandler"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IDataAccess"/> component providing list builder data access.</param>
        /// <param name="temp">The location where to write generated lists to.</param>
        /// <param name="dataContext">The <see cref="Sales.DataAccess.DefaultContext"/> providing sales data.</param>
        public GenerateListCommandHandler(IDataAccess dal, IFileLocation temp, Sales.DataAccess.DefaultContext dataContext)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            if (temp == null) throw new ArgumentNullException(nameof(temp));
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dal = dal;
            this.temp = temp;
            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<GenerateListCommand> Members

        /// <inheritdoc />
        public async Task Handle(GenerateListCommand message, IMessageHandlerContext context)
        {
            try
            {
                var userId = context.InitiatingUserId();
                var cartId = message.Criteria.RequestId;

                if (await this.dataContext.SetOf<Cart>().AnyAsync(c => c.Id == cartId))
                {
                    Trace.WriteLine($"Cart: {cartId} already exists. Exiting...");
                    if (Debugger.IsAttached) Debugger.Break();
                    return;
                }

                var recordCount = await this.BuildFile(message.Criteria).ConfigureAwait(false);

                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .SingleOrDefaultAsync(u => u.UserId == userId)
                    .ConfigureAwait(false);

                var cart = Cart.ForListbuilder(client, cartId, $"{cartId}.csv", recordCount);
                this.dataContext.SetOf<Cart>().Add(cart);

                await this.dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                if (Debugger.IsAttached) Debugger.Break();

                throw;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the actual work of generating the list content.
        /// </summary>
        /// <param name="criteria">The <see cref="ListCriteria"/> that describes the desired list.</param>
        protected virtual async Task<Int32> BuildFile(ListCriteria criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            
            var writeTo = this.temp.CreateInstance(criteria.RequestId.ToString());

            var records = await this.dal.GetRecordsAsync(criteria, CancellationToken.None).ConfigureAwait(false);

            var count = 0;

            using (var stream = writeTo.OpenStream(FileAccess.Write, true))
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var item in records.Select((r, i) => new {Record = r, Index = i}))
                    {
                        if (item.Index == 0) // used to know when to print header
                        {
                            // this odd loop idiom avoids needing to multiple enumerate while keeping the low memory profile of the enumerator pattern.
                            await writer.WriteLineAsync(item.Record.GetHeaderRow());
                        }

                        await writer.WriteLineAsync(item.Record.ToString());

                        count = item.Index;
                    }

                    await writer.FlushAsync();
                    if (stream.CanSeek) stream.SetLength(stream.Position);
                }
            }

            return count;
        }

        #endregion
    }
}