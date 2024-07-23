using System;
using System.Collections.Generic;
using System.Linq;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Extensions for the <see cref="String"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Performs a <see cref="String.Join(String, String[])"/> call on the supplied sequence.
        /// </summary>
        /// <param name="sequence">The source of to join.</param>
        /// <param name="separator">The separator value to use between each element in <paramref name="sequence"/>.</param>
        /// <returns>A new string containing the in order parts of <paramref name="sequence"/> with the <paramref name="separator"/> value between each element.</returns>
        public static string Join(this IEnumerable<String> sequence, String separator)
        {
            var result = String.Join(separator, sequence.ToArray());
            return result;
        }
    }
}
