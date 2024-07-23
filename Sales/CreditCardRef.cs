using System;
using System.Diagnostics;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Provides a cached client reference from the security context.
    /// </summary>
    [DebuggerDisplay("Card {" + nameof(DisplayValue) + "}")]
    public class CreditCardRef
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardRef"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected CreditCardRef()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the card.
        /// </summary>
        public Int32 Id { get; protected set; }

        /// <summary>
        /// Gets the alternate public key of the card.
        /// </summary>
        public Guid PublicKey { get; protected set; }

        /// <summary>
        /// Indicates whether the card instance if the primary card for the client.
        /// </summary>
        public Boolean IsPrimary { get; protected set; }

        /// <summary>
        /// Gets the display value (last four) digits of the card number.
        /// </summary>
        public String DisplayValue { get; protected set; }

        /// <summary>
        /// Gets the expiration date of the card (MMyyyy format).
        /// </summary>
        public virtual String CardExpiration { get; protected set; }
        
        /// <summary>
        /// Gets the <see cref="ClientRef"/> that owns the card instance.
        /// </summary>
        /// <value>The <see cref="ClientRef"/> that owns the card instance.</value>
        public virtual ClientRef Client{ get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the card is still a valid charge account.
        /// </summary>
        /// <returns>True if the card is still valid; otherwise false.</returns>
        public virtual Boolean IsValid()
        {
            var year = Int32.Parse(this.CardExpiration.Right(4));
            
            if (year > DateTime.Today.Year) return true; // If the year is in the future it is automatically ok
            if (year < DateTime.Today.Year) return false; // Likewise, if the year is in the past it is automatically not

            // If the years are the same, check for current or later month of this current year
            var month = Int32.Parse(this.CardExpiration.Left(2));
            return month >= DateTime.Today.Month;
        }

        #endregion
    }
}