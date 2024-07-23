using System;
using System.ComponentModel.DataAnnotations;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Api.Models
{
    /// <summary>
    /// View model for updating a password to a new one.
    /// </summary>
    [Serializable()]
    public class ApiDetailsModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the API access key.
        /// </summary>
        [Display(Name = "Your Api Key")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the status of the current API Access.
        /// </summary>
        public Boolean ApiEnabled { get; set; }

        /// <summary>
        /// Gets or sets the status of the current FTP Access.
        /// </summary>
        public Boolean BatchEnabled { get; set; }

        /// <summary>
        /// Gets or sets the FTP user name access.
        /// </summary>
        public String FtpUser { get; set; }

        /// <summary>
        /// Gets or sets the FTP user password credential.
        /// </summary>
        public String FtpPassword { get; set; }

        #endregion
    }
}