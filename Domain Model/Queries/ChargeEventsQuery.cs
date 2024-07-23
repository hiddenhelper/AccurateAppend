using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Data;
using ChargeEvent = DomainModel.ReadModel.ChargeEvent;

namespace DomainModel.Queries
{
    public class ChargeEventsQuery : IChargeEventsQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public ChargeEventsQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IChargeEventsQuery members

        public virtual IQueryable<ChargeEvent> ForApplication(Guid applicationId, DateTime startOn, DateTime endBy)
        {
            return this.context.SetOf<ChargeEvent>()
                    .Where(e => e.ApplicationId == applicationId)
                    .Where(e => e.EventDate >= startOn && e.EventDate < endBy);
        }

        public virtual IQueryable<ChargeEvent> ForId(Int32 id)
        {
            return this.context.SetOf<ChargeEvent>().Where(e => e.Id == id);
        }
        
        public virtual IQueryable<ChargeEvent> ForDeal(Int32 id)
        {
            return this.context.SetOf<ChargeEvent>().Where(e => e.DealId == id);
        }

        #endregion
    }
}
