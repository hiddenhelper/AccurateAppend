using System;
using System.ComponentModel.DataAnnotations;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Profile.SocialMediaLogin.Messages
{
    /// <summary>
    /// Command to send a linked invite email to a user.
    /// </summary>
    [Serializable()]
    public class InviteUserCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the identifier of the user account sending the invite.
        /// </summary>
        [RequiredNotEmptyGuid()]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the email address that should be sent the invite.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Your email address is not correctly formatted.")]
        [MaxLength(250)]
        public String EmailAddress { get; set; }
    }
}