using System;
using System.Diagnostics.Contracts;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.AuthHandler
{
    /// <summary>
    /// Provides the runtime oAuth configuration that should be used for this environment.
    /// </summary>
    /// <remarks>
    /// Simple holder type for the integration application settings for NationBuilder. Allows the configuration
    /// to use constructor injection of these values in a simple fashion.
    /// </remarks>
    public class NationBuilderConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NationBuilderConfiguration"/> class.
        /// </summary>
        /// <param name="clientId">The <see cref="ClientId"/> value.</param>
        /// <param name="secret">The <see cref="Secret"/> key value.</param>
        public NationBuilderConfiguration(String clientId, String secret)
        {
            if (String.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException(nameof(clientId));
            if (String.IsNullOrWhiteSpace(secret)) throw new ArgumentNullException(nameof(secret));
            Contract.EndContractBlock();

            this.ClientId = clientId;
            this.Secret = secret;
        }

        /// <summary>
        /// Gets the value used to identify this application runtime instance for oAuth registration with NationBuilder.
        /// </summary>
        /// <value>The value used to identify this application runtime instance for oAuth registration with NationBuilder.</value>
        public String ClientId { get; protected set; }

        /// <summary>
        /// Gets the shared secrete key value used to secure token request information for this application runtime instance.
        /// </summary>
        /// <value>The shared secrete key value used to secure token request information for this application runtime instance.</value>
        public String Secret { get; protected set; }
    }
}