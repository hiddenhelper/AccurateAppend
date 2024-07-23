using System;
using System.Diagnostics.Contracts;
using System.Web;

namespace AccurateAppend.Websites.Clients.Areas.Box.AuthHandler
{
    /// <summary>
    /// Component capable of understanding the current request made for oAuth integration with Box.com system.
    /// </summary>
    /// <remarks>
    /// Box.com oAuth application registration is fully based on preconfigured post back url. A single application
    /// has a unique url therefore this type really exists to simplify the use of our local dev, integration, and production
    /// website deployments.
    /// </remarks>
    public class ConfigurationSelector
    {
        /// <summary>
        /// Determines the appropriate settings that should be used for Box.com oAuth token acquisition for the current <paramref name="request"/>.
        /// </summary>
        /// <remarks>
        /// Is just a lookup between current host and the oAuth configuration values.
        /// </remarks>
        /// <param name="request">The <see cref="HttpRequestBase"/> to examine.</param>
        /// <returns>The <see cref="BoxConfiguration"/> containing the settings that should be used for oAuth integration.</returns>
        public virtual BoxConfiguration Select(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            Contract.Ensures(Contract.Result<BoxConfiguration>() != null);
            Contract.EndContractBlock();

            // PRODUCTION
            var clientId = @"gz0w0uvlrj0tkxw1ra1mn6474gszs98c";
            var secret = @"N6OobNczf3Pyzwh0Rmz23AKg3PkQEeBp";

            var url = request.Url ?? new Uri("http://localhost");

#if DEBUG
            if (url.Host.ToLowerInvariant() == "localhost")
            {
                // DEV
                clientId = @"926a36fzx8xoxq01tq6c1xqbdoepie3g";
                secret = @"KHNIz9MKmdMNz8TaCvaI5aNDSpNToTQY";
            }
#endif

            return new BoxConfiguration(clientId, secret);
        }
    }
}