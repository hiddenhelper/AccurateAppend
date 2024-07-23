using System;
using NServiceBus;

namespace DomainModel.Messages  // AccurateAppend.Websites.Admin.Messages.Operations might be a better home for this. Not sure.
{
    /// <summary>
    /// Bus command to create and/or configure a customer FTP account.
    /// </summary>
    [Serializable()]
    public class ConfigureFtpAccountCommand : ICommand
    {
        /// <summary>
        /// AA UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Password for user
        /// </summary>
        public String Password { get; set; }

        // Since FTPLogonModel doesnt have a path, I'm assuming its to be calculated based on username (a relative path off the root)
        ///// <summary>
        ///// FTP Home directory for user
        ///// </summary>
        //public String Unc { get; set; }

        /// <summary>
        /// Account enablement value
        /// </summary>
        public Boolean Enabled { get; set; }
    }
}