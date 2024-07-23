using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using AccurateAppend.Core.Utilities;

namespace DomainModel
{
    /// <summary>
    /// Used to provide a standard methodology for securely interacting with the remote
    /// storage web application with a custom bearer token for proof. A requesting application
    /// can supply an action / url that is able to accept and understand the response
    /// from the remote storage application and an optional UUID to identify this request.
    /// Proof is handled via a symmetric encryption system and key that is shared between
    /// the two systems.
    /// Once crafted, the Uri to the remote application is generated and is expected to be used
    /// to redirect the remote browser to. When the interaction is complete, the client browser
    /// is redirected to the originally supplied location (generally resident on the current
    /// application though chaining and back-channel solutions are possible). The results of
    /// the operation, again using the same shared symmetric encryption system and key, are
    /// processed by the current application leveraging the <see cref="UploadResult"/> class.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Request {" + nameof(Identifier) + ("} handled at {" + nameof(ReturnUrl) + "}"))]
    public class UploadRequest
    {
        #region Constants

        /// <summary>
        /// Contains the path to the local VS instance of the STORAGE app.
        /// </summary>
        public const String Local = "http://localhost:2315/Upload/Save";

        #endregion

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="UploadRequest"/> class with the supplied <paramref name="returnUrl"/>
        /// location where the results of the upload request (success or failure) should be passed back to the requesting application.
        /// This overload will generate a new unique request identifier.
        /// </summary>
        /// <param name="returnUrl">The uri that the results of the storage operation should be returned to residing in the requesting application.</param>
        public UploadRequest(String returnUrl) : this(Guid.NewGuid(), returnUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadRequest"/> class with the supplied <paramref name="identifier"/> and
        /// <paramref name="returnUrl"/> location where the results of the upload request (success or failure) should be passed back
        /// to the requesting application.
        /// </summary>
        /// <param name="identifier">The <see cref="Guid"/> value that is used to uniquely represent this request.</param>
        /// <param name="returnUrl">The uri that the results of the storage operation should be returned to residing in the requesting application.</param>
        public UploadRequest(Guid identifier, String returnUrl)
        {
            if (String.IsNullOrWhiteSpace(returnUrl)) throw new ArgumentNullException(nameof(returnUrl));
            Contract.EndContractBlock();

            this.Identifier = identifier;
            this.ReturnUrl = returnUrl.Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique system identifier for the current model.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Gets or sets the uri to redirect after upload.
        /// </summary>
        [Required()]
        [MinLength(1)]
        public String ReturnUrl { get; set; }

        /// <summary>
        /// Indicates whether the storage application should perform unpackaging and conversion of supported types to CSV format.
        /// </summary>
        public Boolean ConvertToCsv { get; set; }
        
        /// <summary>
        /// Value used to correlated this upload with other uploaded instances.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Crafts the uri to redirect the client to the remote upload location.
        /// </summary>
        /// <example>
        /// <p>Shows basic usage</p>
        /// <code>
        /// <![CDATA[
        /// var request = new UploadRequest("http://yourappurl");
        /// var redirectClientTo = request.CreateRequest(theEncryptionystem, "http://thisapplication/path/to/handle/response");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="encryption">The symmetric <see cref="IEncryptor"/> shared with the storage application used to craft secure communications with.</param>
        /// <param name="storageApplication">The absolute uri for the storage application request.</param>
        /// <returns>A <see cref="Uri"/> containing the encoded request values that an uploaded file should be posted to for storage.</returns>
        public virtual Uri CreateRequest(IEncryptor encryption, String storageApplication = "https://storage.accurateappend.com/Upload/Save")
        {
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentException("The supplied encryption system does not support symmetry", nameof(encryption));
            Contract.Ensures(Contract.Result<Uri>() != null);
            Contract.EndContractBlock();

            var builder = new QueryStringBuilder()
                .AddTo(nameof(this.Identifier), this.Identifier.ToString(), true)
                .AddTo(nameof(this.ReturnUrl), this.ReturnUrl, true);
            if (this.CorrelationId != null) builder.AddTo(nameof(this.CorrelationId), this.CorrelationId.ToString(), true);
            if (this.ConvertToCsv) builder.AddTo(nameof(this.ConvertToCsv), this.ConvertToCsv.ToString(), true);

            var rawalue = builder.ToString();

            var token = HttpUtility.UrlEncode(encryption.SymetricEncrypt(rawalue));
            
            builder = new QueryStringBuilder(new Uri(storageApplication))
                .AddTo("Token", token, true);

            var result = builder.ToString(storageApplication);
            return new Uri(result);
        }

        /// <summary>
        /// Handles the initial request from a bearer token.
        /// </summary>
        /// <param name="querystring">The deconstructed request collection. Generally this is the <see cref="HttpRequestBase.QueryString"/> collection.</param>
        /// <param name="encryption">The symmetric <see cref="IEncryptor"/> shared with the storage application used to craft secure communications with.</param>
        /// <returns>The <see cref="UploadRequest"/> received.</returns>
        public static UploadRequest HandleToken(NameValueCollection querystring, IEncryptor encryption)
        {
            if (querystring == null) throw new ArgumentNullException(nameof(querystring));
            if (querystring.Count == 0) throw new ArgumentException("The querystring collection is empty", nameof(querystring));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentException("The supplied encryption system does not support symmetry", nameof(encryption));
            Contract.Ensures(Contract.Result<UploadRequest>() != null);
            Contract.EndContractBlock();

            if (!querystring.AllKeys.Any(k => k.Equals("Token", StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("The querystring collection lacks a 'Token' element", nameof(querystring));

            var rawValue = HttpUtility.UrlDecode(querystring["Token"]);
            rawValue = encryption.SymetricDecrypt(rawValue);

            var builder = new QueryStringBuilder(rawValue);
            var identifier = Guid.Parse(builder[nameof(Identifier)]);
            var returnUrl = builder[nameof(ReturnUrl)] ?? String.Empty;
            var convertToCsv = Boolean.Parse(builder[nameof(ConvertToCsv)] ?? false.ToString());
            var correlationId = String.IsNullOrEmpty(builder[nameof(CorrelationId)])
                ? new Guid?()
                : Guid.Parse(builder[nameof(CorrelationId)]);

            var result = new UploadRequest(identifier, returnUrl)
            {
                ConvertToCsv = convertToCsv,
                CorrelationId = correlationId
            };

            return result;
        }

        #endregion
    }
}