#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.HtmlHelpers
{
    /// <summary>
    /// Extension methods for the <see cref="TagBuilder"/> class.
    /// </summary>
    public static class TagBuilderExtensions
    {
        /// <summary>
        /// Merges a 'style' attribute with the supplied <paramref name="value"/>.
        /// </summary>
        public static void AddStyle(this TagBuilder builder, String value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.MergeAttribute("style", value);
        }

        /// <summary>
        /// Merges an 'id' attribute with the supplied <paramref name="value"/>.
        /// </summary>
        public static void AddIdentifier(this TagBuilder builder, String value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.MergeAttribute("id", value);
        }
    }
}