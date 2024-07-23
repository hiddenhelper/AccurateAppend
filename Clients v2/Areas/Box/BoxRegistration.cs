using System;
using AccurateAppend.Core;
using AccurateAppend.Websites.Clients.Areas.Box.BoxApi;
using Box.V2.Auth;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Entity related to Box.com access integration for an Accurate Append client.
    /// </summary>
    public class BoxRegistration
    {
        #region Fields

        private DateTime dateRegistered;
        private DateTime dateExpires;
        private DateTime dateGranted;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxRegistration"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected BoxRegistration()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        public Int32 Id { get; protected set; }

        /// <summary>
        /// Gets the name identifying this instance.
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Gets the identifier of the client that created the integration.
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        /// The <see cref="DateTime"/> (in UTC) when the registration was made.
        /// </summary>
        public DateTime DateRegistered
        {
            get => this.dateRegistered.Coerce();
            protected set => this.dateRegistered = value;
        }

        /// <summary>
        /// Gets the current access token for use with the Box.com API.
        /// </summary>
        public String AccessToken { get; protected set; }

        /// <summary>
        /// Gets the current refresh token for use with the Box.com API.
        /// </summary>
        public String RefreshToken { get; protected set; }

        /// <summary>
        /// The <see cref="DateTime"/> (in UTC) when the <see cref="AccessToken"/> will expire.
        /// </summary>
        public DateTime DateExpires
        {
            get => this.dateExpires.Coerce();
            protected set => this.dateExpires = value;
        }

        /// <summary>
        /// The <see cref="DateTime"/> (in UTC) when the current registration was last granted.
        /// </summary>
        public DateTime DateGranted
        {
            get => this.dateGranted.Coerce();
            protected set => this.dateGranted = value;
        }

        /// <summary>
        /// The shared public key identifier used to related this entity with another system.
        /// </summary>
        public Guid PublicKey { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the current session access tokens from the supplied session instance.
        /// </summary>
        /// <remarks>
        /// https://github.com/box/box-windows-sdk-v2/issues/31
        /// 
        /// Due to the crazy way Box.com treats access tokens, we're forced to monitor this for
        /// their code automatically updating it for use. This isn't optimal so we can at least
        /// isolate all the code to here for when the session updates.
        /// </remarks>
        /// <param name="session">The <see cref="OAuthSession"/> that we should update our tokens from.</param>
        public void Update(OAuthSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            this.AccessToken = session.AccessToken;
            this.RefreshToken = session.RefreshToken;
            this.DateExpires = DateTime.UtcNow.AddMinutes(session.ExpiresIn);
            this.DateGranted = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a <see cref="OAuthSession"/> that we should use for Box.com API access.
        /// </summary>
        /// <returns>A new <see cref="IdentifiedOAuthSession"/> from this instance.</returns>
        public IdentifiedOAuthSession CreateSession()
        {
            if (this.DateExpires < DateTime.UtcNow) throw new InvalidOperationException($"Box registration: {this.Id} is currently inactive");

            var expires = Convert.ToInt32((this.DateExpires - this.DateGranted).TotalSeconds);
            return new IdentifiedOAuthSession(this.Id, this.AccessToken, this.RefreshToken, expires, "bearer");
        }

        /// <summary>
        /// Factory method to create a new registration from.
        /// </summary>
        public static BoxRegistration Create(Guid userId, Guid publicKey, IGeneratedToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            var registration = new BoxRegistration();
            registration.Name = $"Registration on {DateTime.UtcNow:d}";
            registration.AccessToken = token.AccessToken;
            registration.RefreshToken = token.RefreshToken;
            registration.DateExpires = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
            registration.UserId = userId;
            registration.DateGranted = DateTime.UtcNow;
            registration.DateRegistered = DateTime.UtcNow;
            registration.PublicKey = publicKey;

            return registration;
        }

        #endregion
    }
}