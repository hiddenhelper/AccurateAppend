using System;
using System.ComponentModel;

namespace AccurateAppend.Websites.Clients.HtmlHelpers.Styles
{
    /// <summary>
    /// Bootstrap column layout targets.
    /// </summary>
    [Serializable()]
    public enum Target
    {
        /// <summary>
        /// Extra small device.
        /// </summary>
        [Description("xs")]
        Xs,

        /// <summary>
        /// Small device.
        /// </summary>
        [Description("sm")]
        Sm,

        /// <summary>
        /// Medium device.
        /// </summary>
        [Description("md")]
        Md,

        /// <summary>
        /// Large device.
        /// </summary>
        [Description("lg")]
        Lg
    }
}