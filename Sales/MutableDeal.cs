using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// The default <see cref="DealBinder"/> type for use in the AA system. Supports editing and
    /// minimal restrictions on content.
    /// </summary>
    public class MutableDeal : DealBinder
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableDeal"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected MutableDeal()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableDeal"/> class.
        /// </summary>`
        /// <param name="client">The <see cref="ClientRef"/> the deal is for.</param>
        public MutableDeal(ClientRef client) : base(client)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableDeal"/> class.
        /// </summary>`
        /// <param name="client">The <see cref="ClientRef"/> the deal is for.</param>
        /// <param name="creator">The security identifier of the user that created the deal.</param>
        public MutableDeal(ClientRef client, Guid creator) : base(client, creator)
        {
        }

        #endregion
    }
}