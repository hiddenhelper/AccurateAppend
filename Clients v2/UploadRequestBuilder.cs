using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Web;
using AccurateAppend.Core.Utilities;
using DomainModel;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Component used to simplify interaction with the <see cref="UploadRequest"/> by crafting a singular dependency.
    /// Basically this exposes the needed client API we originally should of had.
    /// </summary>
    /// <remarks>
    /// This type will be moved to Domain Model assembly soon.
    /// </remarks>
    public class UploadRequestBuilder
    {
        #region Fields

        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadRequestBuilder"/> class.
        /// </summary>
        /// <param name="encryption">The symmetric <see cref="IEncryptor"/> shared with the storage application used to craft secure communications with.</param>
        public UploadRequestBuilder(IEncryptor encryption)
        {
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentException("The supplied encryption system does not support symmetry", nameof(encryption));
            Contract.EndContractBlock();


            this.encryption = encryption;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Crafts the uri to redirect the client to the remote upload location.
        /// </summary>
        /// <param name="returnUrl">The uri that the results of the storage operation should be returned to residing in the requesting application.</param>
        /// <param name="convertToCsv">Indicates whether the posted file should automatically convert to a CSV upon upload.</param>
        /// <returns>A <see cref="Uri"/> containing the encoded request values that an uploaded file should be posted to for storage.</returns>
        public virtual Uri CreateRequest(String returnUrl, Boolean convertToCsv = true)
        {
            if (String.IsNullOrWhiteSpace(returnUrl)) throw new ArgumentNullException(nameof(returnUrl));
            Contract.Ensures(Contract.Result<Uri>() != null);

            return this.CreateRequest(Guid.NewGuid(), returnUrl, convertToCsv);
        }

        /// <summary>
        /// Crafts the uri to redirect the client to the remote upload location.
        /// </summary>
        /// <param name="identifier">The <see cref="Guid"/> value that is used to uniquely represent this request.</param>
        /// <param name="returnUrl">The uri that the results of the storage operation should be returned to residing in the requesting application.</param>
        /// <param name="convertToCsv">Indicates whether the posted file should automatically convert to a CSV upon upload.</param>
        /// <returns>A <see cref="Uri"/> containing the encoded request values that an uploaded file should be posted to for storage.</returns>
        public virtual Uri CreateRequest(Guid identifier, String returnUrl, Boolean convertToCsv = true)
        {
            if (String.IsNullOrWhiteSpace(returnUrl)) throw new ArgumentNullException(nameof(returnUrl));
            Contract.Ensures(Contract.Result<Uri>() != null);

            var request = new UploadRequest(identifier, returnUrl)
            {
                ConvertToCsv = convertToCsv
            };

            var uri = request.CreateRequest(this.encryption);
#if DEBUG
            //uri = request.CreateRequest(this.encryption, UploadRequest.Local); // Uncomment to switch to a local VS instance of the STORAGE app           
#endif

            return uri;
        }

        /// <summary>
        /// Utility method used to determine the appropriate <see cref="Uri.Scheme"/> value for the current request context.
        /// </summary>
        /// <param name="currentRequest">The <see cref="HttpRequestBase"/> instance for teh request.</param>
        /// <returns>The appropriate <see cref="Uri.Scheme"/> that should be used for the current request.</returns>
        public static String DetermineScheme(HttpRequestBase currentRequest)
        {
            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (currentRequest.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif

            return scheme;
        }

        /// <summary>
        /// Handles the post back results from a bearer token request.
        /// </summary>
        /// <param name="querystring">The deconstructed request collection. Generally this is the <see cref="HttpRequestBase.QueryString"/> collection.</param>
        /// <returns>The <see cref="UploadResult"/> explaining the result of the request.</returns>
        public virtual UploadResult HandleFromPostback(NameValueCollection querystring)
        {
            var result = UploadResult.HandleFromPostback(querystring, this.encryption);
            return result;
        }

        #endregion
    }
}