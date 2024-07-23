using System;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Indicatees one of the possible results of an external logon process (assumes oAuth).
    /// </summary>
    [Serializable()]
    public enum ExternalLogonResult
    {
        /// <summary>
        /// The result is unable to be determined. The external provideder did not return a meaningful repsonse or
        /// was unable to be contacted.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The logon attempt was sucessul.
        /// </summary>
        Accepted,

        /// <summary>
        /// The user explicitly denied access to our application.
        /// </summary>
        UserRejected
    }
}