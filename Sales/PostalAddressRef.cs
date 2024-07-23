using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Provides a cached client party address reference from the security context.
    /// </summary>
    [DebuggerDisplay("Address for: {" + nameof(Id) + "}")]
    public class PostalAddressRef
    {
        #region Fields

        private ClientRef client;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostalAddressRef"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected PostalAddressRef()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostalAddressRef"/> class.
        /// </summary>
        /// <param name="client">The <see cref="PostalAddressRef"/> instance that this processing describes.</param>
        protected internal PostalAddressRef(ClientRef client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            Contract.EndContractBlock();

            this.client = client;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the client.
        /// </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Gets or set the postal address, if any, of the <see cref="Client"/>.
        /// </summary>
        public String StreetAddress { get; protected set; }

        /// <summary>
        /// Gets or set the city, if any, of the <see cref="Client"/>.
        /// </summary>
        public String City { get; protected set; }

        /// <summary>
        /// Gets or set the state or province, if any, of the <see cref="Client"/>.
        /// </summary>
        public String State { get; protected set; }

        /// <summary>
        /// Gets or set the postal code, if any, of the <see cref="Client"/>.
        /// </summary>
        public String PostalCode { get; protected set; }

        /// <summary>
        /// Gets or set the country, if any, of the <see cref="Client"/>. Uses ISO 3166-1 alpha-2
        /// values. Null assumes "US".
        /// </summary>
        public String Country { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ClientRef"/> this party address pertains to.
        /// </summary>
        public virtual ClientRef Client
        {
            get => this.client;
            protected set => this.client = value;
        }

        #endregion
    }
}