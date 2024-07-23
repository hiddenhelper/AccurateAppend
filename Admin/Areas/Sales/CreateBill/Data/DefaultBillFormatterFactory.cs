using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Security;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Admin.Areas.Sales.CreateBill.Data
{
    /// <summary>
    /// Simplifies client use and creation of bills by encapsulating all logic
    /// for what formatter uses what and by enforcing requirements for individual
    /// billing scenarios. Allows this to be dependency injected into any billing
    /// related component that needs them.
    /// </summary>
    public class DefaultBillFormatterFactory : IBillFormatterFactory
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBillFormatterFactory"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> providing data access.</param>
        public DefaultBillFormatterFactory(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region IBillFormatterFactory Members

        /// <inheritdoc />
        public virtual async Task<IBillFormatter> ForSubscription(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            var applicationId = await this.LookupApplication(userId, cancellation).ConfigureAwait(false);

            BillFormatter formatter;
            if (applicationId == WellKnownIdentifiers.AccurateAppendId)
            {
                formatter = new SubscriptionBillFormatterHtml(this.context);
            }
            else
            {
                formatter = new SubscriptionBillFormatterText(this.context);
            }

            return formatter;
        }

        /// <inheritdoc />
        public virtual async Task<IBillFormatter> ForUsage(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            var applicationId = await this.LookupApplication(userId, cancellation).ConfigureAwait(false);

            BillFormatter formatter;
            if (applicationId == WellKnownIdentifiers.AccurateAppendId)
            {
                formatter = new UsageBillFormatterHtml(this.context);
            }
            else
            {
                formatter = new UsageBillFormatterText(this.context);
            }

            return formatter;
        }

        /// <inheritdoc />
        public virtual async Task<IBillFormatter> ForRefund(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            var applicationId = await this.LookupApplication(userId, cancellation).ConfigureAwait(false);

            BillFormatter formatter;
            if (applicationId == WellKnownIdentifiers.AccurateAppendId)
            {
                formatter = new RefundBillFormatterHtml(this.context);
            }
            else
            {
                formatter = new RefundBillFormatterText(this.context);
            }

            return formatter;
        }

        /// <inheritdoc />
        public virtual async Task<IBillFormatter> ForNationBuilder(Guid publicKey, CancellationToken cancellation = default(CancellationToken))
        {
            var request = await this.context
                .SetOf<PushRequest>()
                .Where(p => p.SupressionId == publicKey)
                .Include(p => p.For)
                .Include(p => p.For.Owner)
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (request == null) throw new InvalidOperationException("This order is not for a NationBuilder append and cannot use this template.");

            return new NationBuilderFormatter(request);
        }

        /// <inheritdoc />
        public virtual Task<IBillFormatter> ByMatchLevel(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult<IBillFormatter>(new BasicBillFormatter(this.context));
        }

        /// <inheritdoc />
        public virtual Task<IBillFormatter> ByMatchType(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult<IBillFormatter>(new BasicBillFormatter(this.context));
        }

        /// <inheritdoc />
        public virtual async Task<IBillFormatter> ForPublic(Guid publicKey, CancellationToken cancellation = default(CancellationToken))
        {
            var baseQuery = this.context
                .SetOf<Job>()
                .Where(j => j.Status == JobStatus.Complete && j.PublicKey == publicKey);

            var csvQuery = baseQuery.OfType<ClientJob>();
            var listBuilderQuery = baseQuery.OfType<ListbuilderJob>();

            if (!await csvQuery.Select(j => j.Id).Concat(listBuilderQuery.Select(j => j.Id)).AnyAsync(cancellation))
            {
                throw new InvalidOperationException("This order is not for a public CSV or ListBuilder append and cannot use this template.");
            }

            return new BasicBillFormatter(this.context) {IncludeDownloadLink = true};
        }

        #endregion

        #region Helpers

        private Task<Guid> LookupApplication(Guid userId, CancellationToken cancellation)
        {
            return this.context
                .SetOf<User>()
                .Where(u => u.Id == userId)
                .Select(u => u.Application.Id)
                .FirstAsync(cancellation);
        }

        #endregion
    }
}