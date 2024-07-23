using System;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Contains information about the result of an External logon request made by a client.
    /// </summary>
    [Serializable()]
    public class LogonResult
    {
        /// <summary>
        /// Gets the <see cref="ExternalLogonResult"/> value indicating the response from the external logon provider.
        /// </summary>
        /// <value>The <see cref="ExternalLogonResult"/> value indicating the response from the external logon provider..</value>
        public ExternalLogonResult Type { get; set; }

        /// <summary>
        /// Gets the unique identifier of the user at the external provider as related to a specific application identifier for Accurate Append.
        /// <note type="warning">
        /// Returned identities are unique per user per application. In example, a Facebook user has a differing identifier to each authorized application.
        /// </note>
        /// </summary>
        /// <value>
        /// The unique identifier of the user as defined by the external logon provider.
        /// </value>
        public String Identifier { get; set; }

        /// <summary>
        /// Gets the unique bearer token used to allow access to user information at the external provider as related to a specific application identifier for Accurate Append.
        /// <note type="warning">
        /// Access tokens are unique per user per application and generally are not able to be used except in emphemeral cases.
        /// </note>
        /// </summary>
        /// <value>
        /// The unique bearer token defined by the external logon provider.
        /// </value>
        public String Token { get; set; }
    }
}