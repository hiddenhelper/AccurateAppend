using System;
using System.ComponentModel;

namespace AccurateAppend.Websites.Clients.Areas.Box.AuthHandler
{
    /// <summary>
    /// Data contract for an issued oAuth token.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Token : IGeneratedToken
    {
        #region Properties

#pragma warning disable IDE1006 // Naming Styles
        public String access_token { get; set; }

        public Int32 expires_in { get; set; }

        public String refresh_token { get; set; }

        public String token_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        #endregion

        #region IGeneratedToken Members

        String IGeneratedToken.AccessToken => this.access_token;

        Int32 IGeneratedToken.ExpiresIn => this.expires_in;

        String IGeneratedToken.RefreshToken => this.refresh_token;

        #endregion
    }
}