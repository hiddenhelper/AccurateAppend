using System;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Message.Models
{
    /// <summary>
    /// View model containing the information used to customize the filtering on the
    /// view data presented.
    /// </summary>
    public class MessagesView
    {
        /// <summary>
        /// Gets the url to the message detail requested, if any.
        /// </summary>
        public String MessageDetail { get; set; }

        /// <summary>
        /// Gets the email address to filter the view data for, if any.
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Gets the customer user identifier to filter the view data for, if any.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets the url to the user detail requested, if any.
        /// </summary>
        public String UserDetail { get; set; }
    }
}