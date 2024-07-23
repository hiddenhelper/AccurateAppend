using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models
{
    /// <summary>
    /// View model for displaying the available rate cards for a single user or for the base system.
    /// </summary>
    [Serializable()]
    public class ClientModel
    {
        private String userName;

        /// <summary>
        /// The user identifier of the user to filter rate cards for, if any.
        /// </summary>
        /// <remarks>
        /// This value will never be null without <see cref="UserName"/> being null as well.
        /// </remarks>
        public Guid? UserId { get; set; }

        /// <summary>
        /// The user name of the user filtering rate cards for, if any.
        /// </summary>
        /// <remarks>
        /// This value will never be null without <see cref="UserId"/> being null as well.
        /// </remarks>
        public String UserName
        {
            get => this.userName ?? String.Empty;
            set => this.userName = value ?? String.Empty;
        }

        /// <summary>
        /// Indicates whether the view should display only the system level rate cards.
        /// </summary>
        public Boolean DisplaySystemCards => this.UserId == null;

        /// <summary>
        /// If present, indicates the URL that the download action should use to get the rates from.
        /// </summary>
        public String DownloadLink { get; set; }
    }
}