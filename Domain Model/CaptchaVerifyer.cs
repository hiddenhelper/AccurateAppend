using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using RestSharp;

namespace DomainModel
{
    /// <summary>
    /// Provides logic for interacting with reCAPTCHA to verify that a request was found to be verified by Google.
    /// </summary>
    /// <remarks>
    /// https://developers.google.com/recaptcha/docs/verify
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public async Task<ActionResult> YourAction(CancellationToken cancellation)
    /// {
    ///   var captcha = new CaptchaVerifyer(YOURRSECRETKEY);
    ///   Boolean isVerified = await captcha.Verify(this.Request, cancellation);
    /// 
    ///   // do verification logic
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    /// <threadsafety instance="true" static="true"/>
    public class CaptchaVerifyer
    {
        #region Nested Type

        // ReSharper disable ClassNeverInstantiated.Local
        [DebuggerDisplay("{" + nameof(success) + "}")]
        private sealed class CaptchaVerificationResponse
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable MemberCanBePrivate.Local
            public Boolean success { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore InconsistentNaming
        }
        // ReSharper restore ClassNeverInstantiated.Local

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaVerifyer"/> class.
        /// </summary>
        /// <param name="secret">The site or registration specific secret code that is used as authorization to Google. This value MUST be matched to the sitekey that was used with the generated reCAPTCHA challenge.</param>
        public CaptchaVerifyer(String secret)
        {
            if (String.IsNullOrWhiteSpace(secret)) throw new ArgumentNullException(nameof(secret));

            this.Secret = secret.Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the secret proof used to validate a reCAPTCHA pverification request.
        /// </summary>
        /// <value>The secret proof used to validate a reCAPTCHA pverification request.</value>
        protected String Secret { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to verify the current <paramref name="request"/> has a verified challenge.
        /// </summary>
        /// <remarks>
        /// Use of this method on a request without reCAPTCHA content does not generate an error.
        /// </remarks>
        /// <param name="request">The current <see cref="HttpRequestBase"/> (usually from <see cref="HttpContext.Request"/>).</param>
        /// <param name="cancellation">Used to signal that cancellation of an asynchronous request is desired.</param>
        /// <returns>True if the reCAPTCHA information found on the current request is verified by Google; Otherwise false.</returns>
        public virtual async Task<Boolean> Verify(HttpRequestBase request, CancellationToken cancellation)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var captcha = request.Form["g-recaptcha-response"] ?? String.Empty;

            var client = new RestClient("https://www.google.com");
            var captchaRequest = new RestRequest("recaptcha/api/siteverify", Method.POST);
            captchaRequest.AddParameter("secret", this.Secret);
            captchaRequest.AddParameter("response", captcha);

            var captchaVerification = await client.ExecuteTaskAsync<CaptchaVerificationResponse>(captchaRequest, cancellation).ConfigureAwait(false);
            var response = captchaVerification.Data?.success ?? false;

            return response;
        }

        #endregion
    }
}