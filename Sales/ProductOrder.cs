using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// The result of a public order cart that has been submitted for appending.
    /// Responsible for tracking the access and delivery of the public order to the
    /// client.
    /// </summary>
    public class ProductOrder
    {
        #region Fields

        private Guid id;
        private DateTime dateSubmitted;
        private ClientRef client;
        private FileSource source;
        private String name;
        private Int32 recordCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrder"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios. Use the factory methods for creating cart instances.</remarks>
        protected ProductOrder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrder"/> class.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> that was submitted.</param>
        protected internal ProductOrder(Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (!cart.IsActive) throw new InvalidOperationException($"Cart: {cart.Id} is not active and cannot create a {nameof(ProductOrder)}");
            Contract.EndContractBlock();

            this.id = cart.Id;
            this.client = cart.Client;
            this.dateSubmitted = DateTime.UtcNow;
            this.name = cart.Name;
            this.recordCount = cart.RecordCount;

            switch (cart)
            {
                case CsvCart _:
                    this.source = FileSource.ClientFile;
                    break;
                case NationBuilderCart _:
                    this.source = FileSource.NationBuilder;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrder"/> class.
        /// </summary>
        /// <param name="cart">The <see cref="CsvCart"/> that was submitted.</param>
        public ProductOrder(CsvCart cart) : this((Cart)cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (!cart.IsActive) throw new InvalidOperationException($"Cart: {cart.Id} is not active and cannot create a {nameof(ProductOrder)}");
            Contract.EndContractBlock();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrder"/> class.
        /// </summary>
        /// <param name="cart">The <see cref="CsvCart"/> that was submitted.</param>
        public ProductOrder(NationBuilderCart cart) : this((Cart)cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (!cart.IsActive) throw new InvalidOperationException($"Cart: {cart.Id} is not active and cannot create a {nameof(ProductOrder)}");
            Contract.EndContractBlock();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current order instance.
        /// </summary>
        /// <remarks>
        /// This value is used to provided public key continuity between various contexts.
        /// THe identifier will be used throughout the entire lifetime of the order, processing,
        /// billing, and content management.
        /// </remarks>
        public virtual Guid Id
        {
            get => this.id;
            protected set => this.id = value;
        }

        /// <summary>
        /// Gets the client that made the order.
        /// </summary>
        public virtual ClientRef Client
        {
            get => this.client;
            protected set => this.client = value;
        }

        /// <summary>
        /// Gets the date and time (in UTC) the order was submitted.
        /// </summary>
        public virtual DateTime DateSubmitted
        {
            get => this.dateSubmitted;
            protected set => this.dateSubmitted = value.Coerce();
        }

        /// <summary>
        /// Gets the source channel the cart that created this order was for.
        /// </summary>
        public virtual FileSource Source
        {
            get => this.source;
            protected set => this.source = value;
        }

        /// <summary>
        /// Gets the current status in completing the client public order.
        /// </summary>
        public virtual ProcessingStatus Status { get; set; }

        /// <summary>
        /// Gets the name of the client file that is part of the order.
        /// </summary>
        public virtual String Name
        {
            get => this.name;
            protected set => this.name = value;
        }

        /// <summary>
        /// Gets the number of records in the file.
        /// </summary>
        public virtual Int32 RecordCount
        {
            get => this.recordCount;
            protected set => this.recordCount = value;
        }

        #endregion
    }
}
