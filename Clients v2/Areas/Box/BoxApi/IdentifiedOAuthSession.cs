using System;
using Box.V2.Auth;

namespace AccurateAppend.Websites.Clients.Areas.Box.BoxApi
{
    /// <summary>
    /// Used to link an oAuth session key to an AA stored <see cref="BoxRegistration"/>.
    /// </summary>
    public class IdentifiedOAuthSession : OAuthSession
    {
        #region Properties

        /// <summary>
        /// Contains the unique key that this session instance matching exactly one
        /// <see cref="BoxRegistration"/> instance.
        /// </summary>
        public Int32 Id { get; }

        #endregion

        #region Constructors

        public IdentifiedOAuthSession(Int32 id, String access_token, String refresh_token, Int32 expires_in, String token_type) : base(access_token, refresh_token, expires_in, token_type)
        {
            this.Id = id;
        }

        public IdentifiedOAuthSession(Int32 id, String access_token, String refresh_token, Int32 expires_in, String token_type, AuthVersion authVersion) : base(access_token, refresh_token, expires_in, token_type, authVersion)
        {
            this.Id = id;
        }

        #endregion
    }
}