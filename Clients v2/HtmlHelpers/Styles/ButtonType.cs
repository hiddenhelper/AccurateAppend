using System;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// One of the named styled button types
    /// </summary>
    [Serializable()]
    public enum ButtonType
    {
        /// <summary>
        /// The primary style button type.
        /// </summary>
        Primary,

        /// <summary>
        /// The large form of the primary style button type.
        /// </summary>
        PrimaryLarge,

        /// <summary>
        /// The secondary style button type.
        /// </summary>
        Secondary,

        /// <summary>
        /// The text link style button type.
        /// </summary>
        Link,

        /// <summary>
        /// The profile link button type.
        /// </summary>
        Profile,

        /// <summary>
        /// The top-nav link button type.
        /// </summary>
        TopNav
    }
}