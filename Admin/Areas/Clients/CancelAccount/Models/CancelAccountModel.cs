using System;

namespace AccurateAppend.Websites.Admin.Areas.Clients.CancelAccount.Models
{
    /// <summary>
    /// View model for the cancel automated recurring billing account use case.
    /// </summary>
    public class CancelAccountModel
    {
        /// <summary>
        /// Gets the automated recurring payment account identifier.
        /// </summary>
        public Int32 AccountId { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> (at Date grain) that is the earliest date that can be canceled.
        /// </summary>
        public DateTime FirstAvailableDate { get; set; }

        /// <summary>
        /// Gets the optional Uri that the user should be redirected to after completing the action.
        /// </summary>
        public String RedirectTo { get; set; }
    }
}