using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Sales;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Views
{
    /// <summary>
    /// Since <see cref="User"/> information almost never changes and the primary purpose is
    /// to support orthogonal code, such as emails and alerts and contact information, it's a prime
    /// candidate for a simple system to load and cache a form of the data.
    ///  </summary>
    public static class AdminUserCache
    {
        #region Fields

        private static Lazy<IList<UserInfo>> CacheInstance;

        /// <summary>
        /// Provides the default ordering of admin users.
        /// </summary>
        public static readonly IComparer<UserInfo> Ordering = new UserInfoComparer();

        #endregion

        /// <summary>
        /// Indicates whether the <see cref="AdminUserCache"/> has been primed with a <see cref="FactoryMethod"/>
        /// to produce the data to be cached.
        /// </summary>
        /// <remarks>
        /// Interested applications should set up this type as part of the application start process.
        /// </remarks>
        /// <returns>True if there's a callback to produce data; otherwise false. This does not mean the data has been materialized, just that it can be.</returns>
        public static Boolean IsInitialized { get; private set; }

        /// <summary>
        /// Gets the cached data.
        /// </summary>
        /// <exception cref="InvalidOperationException">The data has not been initialized.</exception>
        public static IList<UserInfo> Cache
        {
            get
            {
                if (!IsInitialized) throw new InvalidOperationException($"The {nameof(AdminUserCache)} is not been initialized with a factory callback. This usually occurs during the application start. Did you forget to do this?");

                return CacheInstance.Value;
            }
        }

        /// <summary>
        /// Primes the cache to be able to lazily initialize and cache site information.
        /// </summary>
        /// <param name="factory">The factory method that is used to produce the cached elements.</param>
        public static void FactoryMethod(Func<IList<UserInfo>> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            Contract.Ensures(IsInitialized);
            Contract.EndContractBlock();

            var asyncCache = new Lazy<IList<UserInfo>>(() => factory().ToArray().AsReadOnly(), LazyThreadSafetyMode.ExecutionAndPublication);

            Interlocked.Exchange(ref CacheInstance, asyncCache);
            IsInitialized = true;
        }

        /// <summary>
        /// Cached <see cref="Product"/> information.
        /// </summary>
        [DebuggerDisplay("{" + nameof(UserName) + "}")]
        public class UserInfo
        {
            /// <summary>
            /// Holds the <see cref="User.UserName"/> value for the user.
            /// </summary>
            public String UserName { get; set; }

            /// <summary>
            /// Holds the <see cref="User.Id"/> value for the user.
            /// </summary>
            public Guid UserId { get; set; }

            /// <summary>
            /// Initializes a new empty instance of the <see cref="UserInfo"/> class.
            /// </summary>
            public UserInfo()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="UserInfo"/> class from the supplied <see cref="User"/>.
            /// </summary>
            /// <param name="user">The source to load cached data from.</param>
            public UserInfo(User user)
            {
                this.UserId = user.Id;
                this.UserName = user.UserName;
            }
        }

        /// <summary>
        /// Standard ordering for admin users.
        /// </summary>
        public sealed class UserInfoComparer : IComparer<UserInfo>
        {
            #region IComparer<in UserInfo>

            public Int32 Compare(UserInfo x, UserInfo y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                if (x.UserId == y.UserId) return 0;

                //steve
                if (x.UserId == new Guid("74A0CC9B-DE78-40E3-A556-0732AADF4C46")) return -1;
                if (y.UserId == new Guid("74A0CC9B-DE78-40E3-A556-0732AADF4C46")) return 1;

                //matt
                if (x.UserId == new Guid("F23486EB-08D5-4A82-9031-206CE816F4F8")) return -1;
                if (y.UserId == new Guid("F23486EB-08D5-4A82-9031-206CE816F4F8")) return 1;

                //system
                if (x.UserId == UserExtensions.SystemUserId) return -1;
                if (y.UserId == UserExtensions.SystemUserId) return 1;

                //adriel
                if (x.UserId == new Guid("0D47D959-C386-4ADC-86E8-7C67FEE77784")) return -1;
                if (y.UserId == new Guid("0D47D959-C386-4ADC-86E8-7C67FEE77784")) return 1;

                // sort everything else alphabetical
                return StringComparer.OrdinalIgnoreCase.Compare(x.UserName, y.UserName);
            }

            #endregion
        }
    }
}