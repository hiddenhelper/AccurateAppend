using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// Provides the basic scaffolding for externally logon provider managers.
    /// </summary>
    [ContractClass(typeof(ExternalLogonConfigurationContracts))]
    public abstract class ExternalLogonConfiguration
    {
        /// <summary>
        /// Gets the unique application id at the External Logon provider for Accurate Append social logon.
        /// </summary>
        /// <value>The unique application id at the External Logon provider for Accurate Append social logon.</value>
        public String AppId { get; protected set; }

        /// <summary>
        /// Gets the unique private key value used to confirm our identity at the external Logon provider for Accurate Append social logon.
        /// </summary>
        /// <value>The unique private key value used to confirm our identity at the external Logon provider for Accurate Append social logon.</value>
        public String Secret { get; protected set; }

        /// <summary>
        /// Gets the relative Uri path that's used for this authentication callback.
        /// </summary>
        protected abstract String PostbackRelativePath { get; }

        /// <summary>
        /// Creates the url that a client should be redirected to in order to use the current External logon provider logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public abstract String CreateLogonUrl(HttpContextBase context);

        /// <summary>
        /// Creates the url that a client should be redirected to in when they've declined authorize our application to access to the logon.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to for logon.</returns>
        public abstract String ReRequestPermissions(HttpContextBase context);

        /// <summary>
        /// Handles the response of a logon request from the External Logon provider.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The <see cref="LogonResult"/> containing the information about the logon response.</returns>
        public abstract Task<LogonResult> HandleAuthPostback(HttpContextBase context);

        /// <summary>
        /// Creates the callback url that should be used for handling the response of the logon request. Inheritors will normally
        /// not need to override this member and can instead implement the <see cref="PostbackRelativePath"/> property which is
        /// used with dynamically viewing the current request/host properties to 'automagically' build a postback Url for the external
        /// logon provider that uses this host (such as Visual Studio IIS Express or IIS host header deployment).
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        /// <returns>The absolute url that the client should be redirected to handle the logon response.</returns>
        protected virtual String CreateCallbackUrl(HttpContextBase context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));
            Contract.EndContractBlock();

            var host = context.Request.Url.Host;
            var scheme = context.Request.Url.Scheme;
            if (host.ToLowerInvariant() != "localhost") scheme = Uri.UriSchemeHttps;
            var port = context.Request.Url.Port;

            var builder = new UriBuilder(scheme, host);
            builder.Path = this.PostbackRelativePath;
            if (port != 443 && port != 80) builder.Port = port;

            return builder.ToString();
        }

        /// <summary>
        /// Call to indicate that the external logon process has completed (regardless of success).
        /// </summary>
        /// <param name="context">The current <see cref="HttpContextBase"/> that the user request is operating under.</param>
        public abstract void FinalizeLogon(HttpContextBase context);

        /// <summary>
        /// Allows user detail information related to this <see cref="LogonResult"/> to be acquired for a sucessful log-on action.
        /// </summary>
        /// <remarks>Only the display name is currently supported.</remarks>
        /// <param name="logonResult">The result to acquire user detail information for.</param>
        /// <returns>The display name of the provided <see cref="LogonResult"/>.</returns>
        public abstract Task<String> LookupUserDetails(LogonResult logonResult);
    }

    [ContractClassFor(typeof(ExternalLogonConfiguration))]
    internal abstract class ExternalLogonConfigurationContracts : ExternalLogonConfiguration
    {
        protected override String PostbackRelativePath
        {
            get
            {
                Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));
                return default(String);
            }
        }

        public override String CreateLogonUrl(HttpContextBase context)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));
            Contract.Ensures(Contract.Result<String>() != null);

            return default(String);
        }

        public override String ReRequestPermissions(HttpContextBase context)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));
            Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));

            return default(String);
        }

        public override Task<LogonResult> HandleAuthPostback(HttpContextBase context)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));
            Contract.Ensures(Contract.Result<Task<LogonResult>>() != null);

            return default(Task<LogonResult>);
        }

        public override void FinalizeLogon(HttpContextBase context)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));
        }

        public override Task<String> LookupUserDetails(LogonResult logonResult)
        {
            Contract.Requires<ArgumentNullException>(logonResult != null, nameof(logonResult));

            return default(Task<String>);
        }
    }
}