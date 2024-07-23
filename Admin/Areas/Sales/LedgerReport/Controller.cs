using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Sales.DataAccess;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Sales.LedgerReport
{
    /// <summary>
    /// Controller for displaying recent ARB ledger information for a user.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> providing data access to the sales information.</param>
        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual ActionResult Index(Guid userId)
        {
            return this.View(userId);
        }

        public virtual async Task<ActionResult> Query(Guid userId, CancellationToken cancellation)
        {
            const String Sql = @"
declare @results table(LedgerId int, AccountId int, EntryType varchar(50), PeriodStart date, PeriodEnd date, DealId int, AccountStart date, AccountEnd date, Amount decimal, Cycle varchar(50), AccountType varchar(50))

insert into @results
exec sales.LedgerReport @UserId=@p0

select * from @results";

            var data = (await this.context
                    .Database
                    .SqlQuery<LedgerEntry>(Sql, userId)
                    .ToArrayAsync(cancellation))
                .Select(l => new
                {
                    Id = l.LedgerId,
                    l.EntryType,
                    PeriodStart = l.PeriodStart.ToBillingZone(),
                    PeriodEnd = l.PeriodStart.ToBillingZone(),
                    Links = new
                    {
                        Deal = l.DealId == null
                            ? null
                            : this.Url.Action("Index", "DealDetail", new {Area = "Sales", dealId = l.DealId})
                    }
                });

            return new JsonNetResult(DateTimeKind.Local) {Data = data};
        }

        #endregion

        #region Nested Type

        private sealed class LedgerEntry
        {
            public Int32 LedgerId { get; set; }

            public String EntryType { get; set; }

            public DateTime PeriodStart { get; set; }

            public DateTime PeriodEnd { get; set; }

            public Int32? DealId { get; set; }
        }

        #endregion
    }
}