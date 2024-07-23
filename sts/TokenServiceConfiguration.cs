using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.SecurityTokenService;

namespace AccurateAppend.SecurityTokenServer
{
    public class TokenServiceConfiguration : SecurityTokenServiceConfiguration
    {

        private static readonly X509SigningCredentials credentials =
            new X509SigningCredentials(
                CertificateLocator.GetCertificate(
                    Settings.CertStoreName,
                    Settings.CertStoreLocation,
                    Settings.CertThumbprint ) );

        public TokenServiceConfiguration() : base( Settings.IssuerAddress, credentials )
        {
            this.SecurityTokenService = typeof (TokenService);
        }
    }
}
