using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;

namespace DomainModel
{
    [Serializable()]
    [DebuggerDisplay("Result:{" + nameof(Status) + "}")]
    public class UploadResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadResult"/> class.
        /// </summary>
        protected UploadResult()
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadResult"/> class. This only works with <see cref="UploadStatus">successful</see> uploads; Use the <see cref="Canceled"/> and <see cref="Failed"/> factories otherwise.
        /// </summary>
        /// <param name="identifier">The identifier of the original request (system file name).</param>
        /// <param name="systemFileName">The system generated file name placed into the storage application.</param>
        /// <param name="path">The full path to the stored file.</param>
        /// <param name="clientFileName">The original name of the uploaded file.</param>
        public UploadResult(Guid identifier, String systemFileName, Uri path, String clientFileName)
        {
            if (systemFileName == null) throw new ArgumentNullException(nameof(systemFileName));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrWhiteSpace(clientFileName)) throw new ArgumentNullException(nameof(clientFileName));
            Contract.EndContractBlock();

            this.Status = UploadStatus.Success;
            this.Identifier = identifier;
            this.SystemFileName = systemFileName;
            this.Path = path;
            this.ClientFileName = clientFileName.Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique system identifier for the current model.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UploadStatus"/> indicating the result of the request.
        /// </summary>
        public UploadStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the uploaded file original (non-system) name.
        /// </summary>
        public String ClientFileName { get; set; }

        /// <summary>
        /// Gets or sets the system generated file name.
        /// </summary>
        public String SystemFileName { get; set; }

        /// <summary>
        /// Gets or sets the full path to the stored (and potentially modified) file instance.
        /// </summary>
        public Uri Path { get; set; }

        /// <summary>
        /// Gets or sets the remote system message describing the result (if any).
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Value used to correlated this upload with other uploaded instances.
        /// </summary>
        public Guid? CorrelationId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a post back containing the results from a bearer token request based on the supplied <see cref="UploadResult"/>.
        /// </summary>
        /// <param name="originalRequest">The original request.</param>
        /// <param name="encryption">The symmetric <see cref="IEncryptor"/> shared with the remote application used to craft secure communications with.</param>
        /// <returns>The <see cref="Uri"/> that can be used to return the client browser to in order to continue the processing with at the remote application.</returns>
        public virtual Uri CreatePostback(UploadRequest originalRequest, IEncryptor encryption)
        {
            if (originalRequest == null) throw new ArgumentNullException(nameof(originalRequest));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentException("The supplied encryption system does not support symmetry", nameof(encryption));
            if (originalRequest.Identifier != this.Identifier) throw new InvalidOperationException($"The supplied request identifier '{originalRequest.Identifier}' does not match the result identifier '{this.Identifier}'");
            Contract.Ensures(Contract.Result<Uri>() != null);
            Contract.EndContractBlock();

            var builder = new QueryStringBuilder()
                .AddTo(nameof(this.Status), this.Status.ToString(), true)
                .AddTo(nameof(this.Identifier), this.Identifier.ToString(), true)
                .AddTo(nameof(this.ClientFileName), this.ClientFileName, true)
                .AddTo(nameof(this.SystemFileName), this.SystemFileName, true)
                .AddTo(nameof(this.Path), this.Path.ToString(), true)
                .AddTo(nameof(this.Message), this.Message, true)
                .AddTo(nameof(this.CorrelationId), this.CorrelationId?.ToString(), true);

            var rawValue = builder.ToString();
            rawValue = encryption.SymetricEncrypt(rawValue);
            rawValue = HttpUtility.UrlEncode(rawValue);

            builder = new QueryStringBuilder(new Uri(originalRequest.ReturnUrl));
            builder = builder.AddTo("Result", rawValue, true);
            
            var postback = new Uri(builder.ToString(originalRequest.ReturnUrl));
            return postback;
        }

        /// <summary>
        /// Handles the post back results from a bearer token request.
        /// </summary>
        /// <param name="querystring">The deconstructed request collection. Generally this is the <see cref="HttpRequestBase.QueryString"/> collection.</param>
        /// <param name="encryption">The symmetric <see cref="IEncryptor"/> shared with the storage application used to craft secure communications with.</param>
        /// <returns>The <see cref="UploadResult"/> explaining the result of the request.</returns>
        public static UploadResult HandleFromPostback(NameValueCollection querystring, IEncryptor encryption)
        {
            if (querystring == null) throw new ArgumentNullException(nameof(querystring));
            if (querystring.Count == 0) throw new ArgumentException("The querystring collection is empty", nameof(querystring));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (!encryption.SupportsSymmetry) throw new ArgumentException("The supplied encryption system does not support symmetry", nameof(encryption));
            Contract.Ensures(Contract.Result<UploadResult>() != null);
            Contract.EndContractBlock();

            if (!querystring.AllKeys.Any(k => k.Equals("Result", StringComparison.OrdinalIgnoreCase))) throw new ArgumentException("The querystring collection lacks a 'Result' element", nameof(querystring));

            var rawValue = HttpUtility.UrlDecode(querystring["Result"]);
            rawValue = encryption.SymetricDecrypt(rawValue);

            var builder = new QueryStringBuilder(rawValue);
            var status = EnumExtensions.Parse<UploadStatus>(builder[nameof(Status)]);
            var identifier = Guid.Parse(builder[nameof(Identifier)]);
            var systemFileName = builder[nameof(SystemFileName)];
            var clientFileName = builder[nameof(ClientFileName)];
            var path = new Uri(builder[nameof(Path)]);
            var correlationId = String.IsNullOrEmpty(builder[nameof(CorrelationId)])
                ? new Guid?()
                : Guid.Parse(builder[nameof(CorrelationId)]);

            var message = builder[nameof(Message)] ?? String.Empty;

            switch (status)
            {
                case UploadStatus.Success:
                    var result = new UploadResult(identifier, systemFileName, path, clientFileName)
                    {
                        Message = message,
                        CorrelationId = correlationId
                    };
                    return result;
                case UploadStatus.Cancel:
                    return Canceled(identifier, correlationId, message);
                case UploadStatus.Fail:
                    return Failed(identifier, correlationId, message);
                default:
                    throw new InvalidOperationException($" status: {status} is unsupported");
            }
        }

        /// <summary>
        /// Factory to create a canceled request.
        /// </summary>
        /// <param name="identifier">The original request identifier.</param>
        /// <param name="correlationId">The value used to correlate this instance with other requests.</param>
        /// <param name="message">The optional message to return to the remote application.</param>
        public static UploadResult Canceled(Guid identifier, Guid? correlationId, String message = "")
        {
            Contract.Ensures(Contract.Result<UploadResult>() != null);
            Contract.EndContractBlock();

            return new UploadResult() {Status = UploadStatus.Cancel, Identifier = identifier, Message = message, CorrelationId = correlationId};
        }

        /// <summary>
        /// Factory to create a failed request.
        /// </summary>
        /// <param name="identifier">The original request identifier.</param>
        /// <param name="correlationId">The value used to correlate this instance with other requests.</param>
        /// <param name="message">The optional message to return to the remote application.</param>
        public static UploadResult Failed(Guid identifier, Guid? correlationId, String message = "")
        {
            Contract.Ensures(Contract.Result<UploadResult>() != null);
            Contract.EndContractBlock();

            return new UploadResult() { Status = UploadStatus.Fail, Identifier = identifier, Message = message, CorrelationId = correlationId};
        }

        #endregion
    }

    /// <summary>
    /// Indicates one of the possible result statuses from a client upload interaction.
    /// </summary>
    [Serializable()]
    public enum UploadStatus
    {
        Success,
        Fail,
        Cancel
    }
}
