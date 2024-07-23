using System;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains the detailed information about a NationBuilder cart.
    /// </summary>
    public class NationBuilderCart : Cart
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderCart"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios. Use the factory methods for creating cart instances.</remarks>
        protected internal NationBuilderCart()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the external identifier, if any, of the file in the order.
        /// </summary>
        public virtual Int32? ExternalId { get; set; }

        /// <summary>
        /// Gets the external identifier, if any, of the files
        /// </summary>
        public virtual Int32? IntegrationId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Used to update the list information for the cart.
        /// </summary>
        /// <remarks>
        /// Only used for NationBuilder orders.
        /// </remarks>
        /// <param name="registrationId">The identifier of the Nation integration.</param>
        /// <param name="listId">The identifier of the list in the Nation.</param>
        /// <param name="listName">The name of the list.</param>
        /// <param name="recordCount">The number of records in the list.</param>
        public virtual void UpdateFileInformation(Int32 registrationId, Int32 listId, String listName, Int32 recordCount)
        {
            if (registrationId < 1) throw new ArgumentOutOfRangeException($"{nameof(registrationId)} must be at least 1", registrationId, nameof(registrationId));
            if (listId < 1) throw new ArgumentOutOfRangeException($"{nameof(listId)} must be at least 1", listId, nameof(listId));
            if (String.IsNullOrWhiteSpace(listName)) throw new ArgumentNullException(nameof(listName));
            if (recordCount < 1) throw new ArgumentOutOfRangeException($"{nameof(recordCount)} must be at least 1", recordCount, nameof(recordCount));
            if (!this.IsActive) throw new InvalidOperationException($"Cart {this.Id} is not active");

            this.IntegrationId = registrationId;
            this.ExternalId = listId;
            this.Name = listName;
            this.RecordCount = recordCount;
        }

        #endregion
    }
}