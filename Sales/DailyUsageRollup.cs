using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Provides the basic structure for usage rollup report aggregations.
    /// </summary>
    public abstract class DailyUsageRollup : IEquatable<DailyUsageRollup>
    {
        #region Fields

        private DateTime date;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyUsageRollup"/> class.
        /// </summary>
        protected DailyUsageRollup()
        {
            this.date = DateTime.Now.Date;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the user identifier associated with the rollup.
        /// </summary>
        /// <value>The user identifier associated with the rollup.</value>
        public Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> the rollup is for.
        /// </summary>
        /// <remarks>
        /// Only <see cref="DateTime.Date"/> should be evaluated with the rollup. All
        /// return values will set to the Billing Timezone and will only leverage the Date portion of the value.
        /// </remarks>
        /// <value>The <see cref="DateTime"/> the rollup is for.</value>
        public DateTime Date
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Unspecified);
                Contract.Ensures(Contract.Result<DateTime>() == Contract.Result<DateTime>().Date);

                return this.date;
            }
            protected set
            {
                if (value.Kind == DateTimeKind.Utc) value = value.ToBillingZone();
                if (value.Kind == DateTimeKind.Unspecified) value = value.ToSafeLocal().ToBillingZone(); 

                this.date = value.Date;
            }
        }

        /// <summary>
        /// Gets the key value used to categorize the report metric.
        /// </summary>
        /// <value>The key value used to categorize the report metric.</value>
        public String Key { get; protected set; }

        /// <summary>
        /// Gets the value of the count for the report metric for the number of inputs.
        /// </summary>
        /// <value>The value of the count for the report metric for the number of inputs.</value>
        public Int32 Count { get; protected set; }

        /// <summary>
        /// Gets the value of the number of matches for the report metric.
        /// </summary>
        /// <value>The value of the number of matches for the report metric.</value>
        public Int32 Matches { get; protected set; }

        #endregion

        #region IEquatable<DailyUsageRollup> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(DailyUsageRollup other)
        {
            if (other == null) return false;

            if (other.GetType() != this.GetType()) return false;

            return this.UserId == other.UserId &&
                   this.Count == other.Count &&
                   this.Date == other.Date &&
                   this.Key == other.Key &&
                   this.Matches == other.Matches;
        }

        #endregion
    }
}
