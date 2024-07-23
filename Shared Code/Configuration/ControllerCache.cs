using System;
using System.Collections.ObjectModel;

namespace AccurateAppend.Websites.Configuration
{
    /// <summary>
    /// Provides a cache of <see cref="Type"/> instances keyed to their <see cref="Type.FullName"/> value.
    /// </summary>
    /// <remarks>
    /// Useful to simplify use of a set of <seealso cref="Type"/>, usually all implementing a particular
    /// interface, to the unique key of their full name. All names are compared using a culture neutral
    /// case insensitive manner.
    /// </remarks>
    public class ControllerCache : KeyedCollection<String, Type>
    {
        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see cref="ControllerCache"/> class.
        /// </summary>
        public ControllerCache() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override String GetKeyForItem(Type item)
        {
            return item.FullName;
        }

        #endregion
    }
}