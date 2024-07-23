using System;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Represents a granted oAuth token.
    /// </summary>
    public interface IGeneratedToken
    {
        /// <summary>
        /// Gets the bearer token value that should be used to make requests with.
        /// </summary>
        String AccessToken { get; }

        /// <summary>
        /// The number of seconds till <see cref="AccessToken"/> expires.
        /// </summary>
        Int32 ExpiresIn { get; }

        /// <summary>
        /// Gets the token that should be used to refresh an <see cref="AccessToken"/> to.
        /// </summary>
        String RefreshToken { get; }
    }
}