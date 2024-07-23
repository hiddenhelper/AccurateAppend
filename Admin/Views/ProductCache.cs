using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Sales;

namespace AccurateAppend.Websites.Admin.Views
{
    /// <summary>
    /// Since <see cref="Product"/> information almost never changes and the primary purpose is
    /// to support orthogonal code, such as emails and alerts and contact information, it's a prime
    /// candidate for a simple system to load and cache a form of the data.
    ///  </summary>
    public static class ProductCache
    {
        #region Fields

        private static Lazy<IList<ProductInfo>> CacheInstance;

        #endregion

        /// <summary>
        /// Indicates whether the <see cref="ProductCache"/> has bee primed with a <see cref="FactoryMethod"/>
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
        public static IList<ProductInfo> Cache
        {
            get
            {
                if (!IsInitialized) throw new InvalidOperationException($"The {nameof(ProductCache)} is not been initialized with a factory callback. This usually occurs during the application start. Did you forget to do this?");

                return CacheInstance.Value;
            }
        }

        /// <summary>
        /// Primes the cache to be able to lazily initialize and cache site information.
        /// </summary>
        /// <param name="factory">The factory method that is used to produce the cached elements.</param>
        public static void FactoryMethod(Func<IList<ProductInfo>> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            Contract.Ensures(IsInitialized);
            Contract.EndContractBlock();

            var asyncCache = new Lazy<IList<ProductInfo>>(() => factory().ToArray().AsReadOnly(), LazyThreadSafetyMode.ExecutionAndPublication);

            Interlocked.Exchange(ref CacheInstance, asyncCache);
            IsInitialized = true;
        }

        /// <summary>
        /// Cached <see cref="Product"/> information.
        /// </summary>
        public class ProductInfo
        {
            /// <summary>
            /// Holds the <see cref="Product.Key"/> value for the product.
            /// </summary>
            public String Key { get; set; }

            /// <summary>
            /// Holds the <see cref="Product.Title"/> value for the product.
            /// </summary>
            public String Title { get; set; }

            /// <summary>
            /// Holds the <see cref="Product.Category"/> value for the product.
            /// </summary>
            public String Category { get; set; }

            /// <summary>
            /// Initializes a new empty instance of the <see cref="ProductInfo"/> class.
            /// </summary>
            public ProductInfo()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ProductInfo"/> class from the supplied <see cref="Product"/>.
            /// </summary>
            /// <param name="product">The source to load cached data from.</param>
            public ProductInfo(Product product)
            {
                this.Title = product.Title;
                this.Key = product.Key;
                this.Category = product.Category.Name;
            }
        }
    }
}