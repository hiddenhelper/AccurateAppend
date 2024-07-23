using System;
using AccurateAppend.Core;
using AccurateAppend.Security;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Admin
{
    /// <summary>
    /// Bus command to process a <see cref="User"/> action perform on the application.
    /// </summary>
    /// <remarks>
    /// This command is purposely defined internally to this application. It is never
    /// intended that any other system or code base is at all expected to interact
    /// with this command processing.
    /// </remarks>
    [Serializable()]
    public class LogUserActionCommand : ICommand
    {
        #region Fields

        private DateTime eventDate;

        #endregion

        #region Properties

        /// <summary>
        /// The textual description for the action performed.
        /// </summary>
        public String Description
        {
            get; set;
        }

        /// <summary>
        /// The identifier of the <see cref="User"/> that performed the action.
        /// </summary>
        public Guid UserId
        {
            get; set;
        }

        /// <summary>
        /// The <see cref="DateTime"/> that the action was performed at (in UTC).
        /// </summary>
        public DateTime EventDate
        
        {
            get { return this.eventDate; }
            set { this.eventDate = value.Coerce(); }
        }

        /// <summary>
        /// The IP address that the user performing the address communicated from.
        /// </summary>
        public String Ip
        {
            get;
            set;
        }

        #endregion
    }
}