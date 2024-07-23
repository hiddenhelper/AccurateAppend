using System;
using AccurateAppend.Core;
using AccurateAppend.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Security
{
    [Serializable()]
    public class ExternalLoginAddedEvent : IEvent
    {
        #region Fields

        private DateTime eventDate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalLoginAddedEvent"/> class.
        /// </summary>
        public ExternalLoginAddedEvent()
        {
            this.eventDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalLoginAddedEvent"/> class.
        /// </summary>
        /// <param name="logon">The <see cref="MappedIdentityLogon"/> to create an event instance from.</param>
        public ExternalLoginAddedEvent(MappedIdentityLogon logon) : this()
        {
            this.UserId = logon.User.Id;
            this.DisplayName = logon.DisplayName;
            this.Source = logon.Provider;
            this.UserName = logon.User.UserName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The identifier of the user that has a linked external login.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The display name of the mapped identity.
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// The source of authority for the external login.
        /// </summary>
        public IdentityProvider Source { get; set; }

        /// <summary>
        /// The moment (in UTC) when the link occured.
        /// </summary>
        public DateTime EventDate
        {
            get => this.eventDate;
            set => this.eventDate = value.Coerce();
        }

        /// <summary>
        /// THe user name of the account that was linked.
        /// </summary>
        public String UserName { get; set; }

        #endregion
    }
}