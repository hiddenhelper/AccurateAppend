using System;
using System.Diagnostics;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Provides a cached client reference from the security context.
    /// </summary>
    [DebuggerDisplay("Client {" + nameof(UserName) + "}:{" + nameof(UserId) + "}")]
    public class ClientRef
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRef"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected ClientRef()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the client.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the key of the application the client is bound to.
        /// </summary>
        public virtual Guid ApplicationId { get; protected set; }

        /// <summary>
        /// Gets the public user name of the client.
        /// </summary>
        public virtual String UserName { get; protected set; }

        /// <summary>
        /// Gets or sets the first name of the client contact.
        /// </summary>
        /// <remarks>The first name of the client contact.</remarks>
        public virtual String FirstName { get; protected set; }

        /// <summary>
        /// Gets or sets the last name of the client contact.
        /// </summary>
        /// <remarks>The last name of the client contact.</remarks>
        public virtual String LastName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the business the client represents.
        /// </summary>
        /// <value>The name of the business the client represents.</value>
        public virtual String BusinessName { get; protected set; }

        /// <summary>
        /// Gets the phone information for a user, if any.
        /// </summary>
        public String PrimaryPhone { get; protected set; }

        /// <summary>
        /// Gets the identifier of the user that is responsible for the client billing.
        /// </summary>
        /// <value>The identifier of the user that is responsible for the client billing.</value>
        public virtual Guid OwnerId { get; protected set; }

        /// <summary>
        /// Gets the <see cref="PostalAddressRef"/> for client billing.
        /// </summary>
        public virtual PostalAddressRef Address
        {
            get;
            protected set;
        }

        #endregion
    }
}
