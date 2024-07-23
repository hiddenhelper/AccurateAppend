using System;

namespace AccurateAppend.Sales
{
    public class DealFile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DealFile"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected DealFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class using the provided <paramref name="publicKey"/>.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="DealBinder"/> that contains this order instance.</param>
        /// <param name="publicKey">The unique public key value.</param>
        public DealFile(DealBinder deal, String customerFileName)
        {
            
        }

        #endregion
    }
}
