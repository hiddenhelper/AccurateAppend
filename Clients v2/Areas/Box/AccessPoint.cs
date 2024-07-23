using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Web;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Encapsulates the logic used to initiate a request to Box.com for an OAUTH handshake request.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// var accessPoint = new AccessPoint(ClientId, Secret);
    /// var uri = accessPoint.BuildAuthUri(HttpContext.Current);
    /// 
    /// // redirect the current user browser to the generated uri value
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    [Serializable()]
    public class AccessPoint
    {
        #region Fields

        private static Func<HttpContextBase, String, String> PostbackGeneratorValue = BuildTokenHandlerUri;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessPoint"/> class (for serialization purposes only)
        /// </summary>
        public AccessPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessPoint"/> class using the supplied values.
        /// </summary>
        /// <param name="clientId">The identifier that uniquely identifies the AA Box.com application.</param>
        /// <param name="secret">The shared secret key used to secure and guarantee authenticity of the generated request.</param>
        /// <param name="identifier">Uniquely identifies this access point instance.</param>
        public AccessPoint(String clientId, String secret, Guid identifier)
        {
            this.ClientId = clientId;
            this.Secret = secret;
            this.Identifier = identifier;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Uniquely identifies this request configuration instance.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The shared secret used by AA and Box.com to secure and guarantee authenticity of the generated request.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(64)]
        public String Secret { get; set; }

        /// <summary>
        /// The identifier that uniquely represents the AA Box.com application.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(64)]
        public String ClientId { get; set; }

        /// <summary>
        /// Contains the AA user name identifier value for debugging purposes.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public String EmailAddress { get; set; }

        /// <summary>
        /// Contains the function used to build a postback uri to the AA system that should handle the generated authorization.
        /// </summary>
        /// <remarks>
        /// This value will never be null. Setting this value to null will automatically reset the default handler generation
        /// function to <see cref="BuildTokenHandlerUri"/>. 
        /// </remarks>
        public static Func<HttpContextBase, String, String> PostbackGenerator
        {
            get => PostbackGeneratorValue;
            set
            {
                value = value ?? BuildTokenHandlerUri;
                Interlocked.Exchange(ref PostbackGeneratorValue, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Using the supplied <paramref name="context"/>, create a FQDN and path to the 
        /// Box.com OAUTH access point to begin the handshake process to generate and acquire
        /// and access key bearer token for use by AA to provide access to the stored data.
        /// </summary>
        /// <param name="context">The <see cref="HttpContextBase"/> to the current request runtime.</param>
        /// <param name="redirectTo">The optional url (relative or absolute) that should be used to redirect the client to after authorization occurs.</param>
        /// <returns>An absolute path to the url that can be used to direct a client browser to begin the authorization handshake with.</returns>
        public virtual String BuildAuthUri(HttpContextBase context, String redirectTo)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            var uri = "https://account.box.com/api/oauth2/authorize";
            var builder = new QueryStringBuilder("response_type=code")
                .AddTo("client_id", this.ClientId)
                .AddTo("redirect_uri", PostbackGenerator(context, redirectTo));

            uri = builder.ToString(uri);
            return uri;
        }

        /// <summary>
        /// Generates the callback url on our system that will be used to capture and process the generated token
        /// using our private client identifier information already configured at Box.com system.
        /// </summary>
        /// <param name="context">The <see cref="HttpContextBase"/> to the current request runtime.</param>
        /// <param name="redirectTo">The optional url (relative or absolute) that should be used to redirect the client to after authorization occurs.</param>
        /// <returns>An absolute path to the url that will be used by NationBuilder to direct a client browser back to our system after authorization has been granted.</returns>
        public static String BuildTokenHandlerUri(HttpContextBase context, String redirectTo)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Request.Url == null) throw new ArgumentNullException(nameof(context), $"{nameof(context.Request)}.{nameof(context.Request.Url)} on supplied {nameof(context)} is null");
            Contract.EndContractBlock();

            var scheme = context.Request.Url.Scheme;
            var host = context.Request.Url.Host;
            var port = context.Request.Url.IsDefaultPort ? String.Empty : ":" + context.Request.Url.Port;

            // override if we're NOT using IIS Express
            if (!host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttps;
            
            var redirectUri = $"{scheme}://{host}{port}/Box/AuthHandler?redirectTo={HttpUtility.UrlEncode(redirectTo)}";

            return redirectUri;
        }

        #endregion
    }
}