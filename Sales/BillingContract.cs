using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains billing data for an <see cref="Order"/>.
    /// </summary>
    /// <remarks>
    /// Currently this information is a small linkage between the order and content (aka email). In the future we will
    /// move draft information from email and place the logic and data into a new table(s) and this entity. It exists to
    /// provide that strong barrier now prior to refactoring.
    /// </remarks>
    [DebuggerDisplay("{" + nameof(OrderId) + "}")]
    public class BillingContract
    {
        #region Fields

        private Order order;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingContract"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected BillingContract()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingContract"/> class.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> instance that this billing information relates to.</param>
        protected internal BillingContract(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Contract.EndContractBlock();

            this.order = order;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Identifies the billing information.
        /// </summary>
        public virtual Int32? OrderId { get; protected set; }

        /// <summary>
        /// Gets the <see cref="Order"/> the bill information is for.
        /// </summary>
        public virtual Order Order
        {
            get { return this.order; }
            protected set { this.order = value; }
        }

        /// <summary>
        /// Gets the payment methodology the current instance should be billed via.
        /// </summary>
        /// <value>The payment methodology the current instance should be billed via.</value>
        public ContractType? ContractType { get; set; }

        #endregion
    }
}
