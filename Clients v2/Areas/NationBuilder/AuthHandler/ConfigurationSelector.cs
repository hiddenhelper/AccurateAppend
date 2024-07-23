using System;
using System.Diagnostics.Contracts;
using System.Web;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.AuthHandler
{
    /// <summary>
    /// Component capable of understanding the current request made for oAuth integration with NationBuilder.
    /// </summary>
    /// <remarks>
    /// NationBuilder oAuth application registration is fully based on preconfigured post back url. A single application
    /// has a unique url therefore this type really exists to simplify the use of our local dev, integration, and production
    /// website deployments.
    /// </remarks>
    public class ConfigurationSelector
    {
        /// <summary>
        /// Determines the appropriate settings that should be used for NationBuilder oAuth token acquisition for the current <paramref name="request"/>.
        /// </summary>
        /// <remarks>
        /// Is just a lookup between current host and the oAuth configuration values.
        /// </remarks>
        /// <param name="request">The <see cref="HttpRequestBase"/> to examine.</param>
        /// <returns>The <see cref="NationBuilderConfiguration"/> containing the settings that should be used for oAuth integration.</returns>
        public virtual NationBuilderConfiguration Select(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            Contract.Ensures(Contract.Result<NationBuilderConfiguration>() != null);
            Contract.EndContractBlock();

            // PRODUCTION
            var clientId = @"ef735564944f3a24f4ff7a563162f4a80844882968bf76d7f8440910fcd0f95e";
            var secret = @"11f40c0b81d0bed511f5a1bc3b19e4624f569f205687788e248eec457baf191c";

            var url = request.Url ?? new Uri("http://localhost");

            // if dev.clients then use the test id
            if (url.Host.ToLowerInvariant() == "devclients.accurateappend.com")
            {
                // TEST
                clientId = @"9b4b0a060b5fff96f2c3956dfd4fe05df2d8dd01bf5a612bd4be8e4f90a41374";
                secret = @"12f46bb91edf13f5ed21cd76dd47953485e6a074b2913cbd900e3bc9c6bbc9dc";
            }

#if DEBUG
            if (url.Host.ToLowerInvariant() == "localhost")
            {
                // DEV
                clientId = @"c13408b3be17b0b51eded1ba2d7e2b718d4971c2e8a61e74a4243a8f6f4f329c";
                secret = @"54e77bdc811715121d3f6ab01e8f50baa9df51463f34186425fe70d06072824b";
            }
#endif

            return new NationBuilderConfiguration(clientId, secret);
        }
    }
}