using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A single shopping cart instance for a client using the public ordering system.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Id) + "}")]
    public abstract class Cart
    {
        #region Fields

        private DateTime dateCreated;
        private String quoteInternal;
        private XElement quote;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current cart instance.
        /// </summary>
        /// <remarks>
        /// This value is used to provided public key continuity between various contexts.
        /// THe identifier will be used throughout the entire lifetime of the order, processing,
        /// billing, and content management.
        /// </remarks>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Gets the client that opened the cart.
        /// </summary>
        public virtual ClientRef Client { get; protected set; }
        
        /// <summary>
        /// Gets the date and time (in UTC) the cart was created.
        /// </summary>
        public virtual DateTime DateCreated
        {
            get => this.dateCreated;
            protected set => this.dateCreated = value.Coerce();
        }
        
        /// <summary>
        /// Gets the name of the client file that is part of the order.
        /// </summary>
        public virtual String Name { get; protected set; }

        /// <summary>
        /// Indicates whether the current cart instance is still active.
        /// </summary>
        public virtual Boolean IsActive { get; protected set; }

        /// <summary>
        /// Gets the number of records in the file.
        /// </summary>
        public virtual Int32 RecordCount { get; protected set; }
        
        /// <summary>
        /// Required for database. Do not directly use.
        /// </summary>
        protected internal virtual String QuoteInternal
        {
            get => this.quoteInternal;
            set
            {
                this.quoteInternal = value;

                if (value == null)
                {
                    this.Quote = null;
                }
                else
                {
                    var xml = XElement.Parse(value);
                    this.Quote = xml;
                }
            }
        }

        /// <summary>
        /// Gets the finalized quote for the current cart, if any.
        /// </summary>
        /// <value>The finalized quote for the current cart, if any.</value>
        public virtual XElement Quote
        {
            get => this.quote;
            set
            {
                this.quote = value;
                if (this.quote == null)
                {
                    if (this.QuoteInternal != null) this.QuoteInternal = null;
                    return;
                }

                if (this.quoteInternal == null)
                {
                    this.QuoteInternal = this.quote.ToString();
                }
                this.quote.Changed += (s, e) => this.quoteInternal = this.quote.ToString();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marks the current cart as complete.
        /// </summary>
        public virtual ProductOrder Complete()
        {
            Contract.Ensures(!this.IsActive);
            Contract.Ensures(Contract.Result<ProductOrder>() != null);
            Contract.EndContractBlock();

            var po = new ProductOrder(this);
            this.IsActive = false;

            return po;
        }

        /// <summary>
        /// Marks the current cart as canceled.
        /// </summary>
        public void Cancel()
        {
            Contract.Ensures(!this.IsActive);
            Contract.EndContractBlock();

            this.IsActive = false;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method for creating a new sales cart for CSV upload orders.
        /// </summary>
        /// <param name="client">The client to create the new order for.</param>
        /// <param name="identifier">The unique identifier for the new cart.</param>
        /// <returns>A <see cref="Cart"/> for the client.</returns>
        public static NationBuilderCart ForNationBuilder(ClientRef client, Guid identifier)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.EndContractBlock();

            var cart = new NationBuilderCart()
            {
                Client = client,
                Id = identifier,
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                Name = String.Empty
            };

            return cart;
        }

        /// <summary>
        /// Factory method for creating a new sales cart for CSV upload orders.
        /// </summary>
        /// <param name="client">The client to create the new order for.</param>
        /// <param name="identifier">The unique identifier for the new cart.</param>
        /// <returns>A <see cref="Cart"/> for the client.</returns>
        public static CsvCart ForClientFile(ClientRef client, Guid identifier)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.EndContractBlock();

            var cart = new CsvCart
            {
                Client = client,
                Id = identifier,
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                Name = String.Empty
            };

            return cart;
        }
        
        public static Cart ForListbuilder(ClientRef client, Guid identifier, String customerFileName, Int32 recordCount)
        {
            throw new NotSupportedException();
            //    if (client == null) throw new ArgumentNullException(nameof(client));
            //    if (String.IsNullOrWhiteSpace(customerFileName)) throw new ArgumentNullException(nameof(customerFileName));
            //    if (recordCount < 1) throw new ArgumentOutOfRangeException($"{nameof(recordCount)} must be at least 1", recordCount, nameof(recordCount));
            //    Contract.EndContractBlock();

            //var cart = new ListbuilderCart
            //{
            //    Client = client,
            //    Id = identifier,
            //    DateCreated = DateTime.UtcNow,
            //    IsActive = true,
            //    Name = String.Empty,
            //    RecordCount = recordCount,
            //};

            //    return cart;
        }

        #endregion
    }
}
