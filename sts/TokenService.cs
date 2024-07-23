using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace AccurateAppend.SecurityTokenServer
{
    public class TokenService : SecurityTokenService
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="config">The <see cref="SecurityTokenServiceConfiguration"/> object to be 
        /// passed to the base class.</param>
        public TokenService(SecurityTokenServiceConfiguration config) : base(config)
        {
        }

        /// <summary>
        /// This methods returns the configuration for the token issuance request. The configuration
        /// is represented by the Scope class. In our case, we are only capable to issue a token for a
        /// single RP identity represented by CN=localhost.
        /// </summary>
        /// <param name="principal">The caller's principal</param>
        /// <param name="request">The incoming RST</param>
        /// <returns>The configuration for the token issuance request.</returns>
        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            // Validate the AppliesTo on the incoming request
            ValidateAppliesTo(request.AppliesTo);

            // Create the scope using the request AppliesTo address and the STS signing certificate
            var scope = new Scope(request.AppliesTo.Uri.ToString(), SecurityTokenServiceConfiguration.SigningCredentials);

            // In this sample app only a single RP identity is shown, which is localhost, and the certificate of that RP is 
            // populated as encryptingCreds
            // If you have multiple RPs for the STS you would select the certificate that is specific to 
            // the RP that requests the token and then use that for encryptingCreds
            EncryptingCredentials encryptingCreds = new X509EncryptingCredentials(
                                                        CertificateLocator.GetCertificate(
                                                                        StoreName.My,
                                                                        StoreLocation.LocalMachine,
                                                                        Settings.EncryptionThumbprint));

            // Set the RP certificate for encryption
            scope.EncryptingCredentials = encryptingCreds;

            return scope;
        }

        /// <summary>
        /// This methods returns the claims to be included in the issued token. 
        /// </summary>
        /// <param name="scope">The scope that was previously returned by GetScope method</param>
        /// <param name="principal">The caller's principal</param>
        /// <param name="request">The incoming RST</param>
        /// <returns>The claims to be included in the issued token.</returns>
        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            var callerIdentity = (IClaimsIdentity)principal.Identity;
            Trace.WriteLine("\nRequest from: " + callerIdentity.Name + "\n");

            IClaimsIdentity outputIdentity = new ClaimsIdentity();

            // Create a 'Name' claim from the incoming identity.
            var nameClaim = new Claim(System.IdentityModel.Claims.ClaimTypes.Name, callerIdentity.Name);
            outputIdentity.Claims.Add(nameClaim);

            // Create an 'Age' claim with a value of 25. In a real scenario, this may likely be looked up from a database.
            var ageClaim = new Claim("http://GenevaSamples/2008/05/AgeClaim", "25", ClaimValueTypes.Integer);
            outputIdentity.Claims.Add(ageClaim);

            // Create a 'Upn' claim from the incoming identity. In a real scenario, this would be pulled from AD.
            var upnClaim = new Claim(System.IdentityModel.Claims.ClaimTypes.Upn, callerIdentity.Name.Split('\\')[1] + "@" + callerIdentity.Name.Split('\\')[0]);
            outputIdentity.Claims.Add(upnClaim);

            // Create a 'Sid' claim from the incoming identity. In a real scenario, this would be pulled from AD.
            String domainName = callerIdentity.Name.Split('\\')[0];
            String ntName = callerIdentity.Name.Split('\\')[1];
            NTAccount account = new NTAccount(domainName, ntName);
            IdentityReference identityReference = account.Translate(typeof(SecurityIdentifier));
            String sid = identityReference.Value;
            Claim sidClaim = new Claim(ClaimTypes.PrimarySid, sid);
            outputIdentity.Claims.Add(sidClaim);

            foreach (var claim in outputIdentity.Claims)
            {
                Console.WriteLine("ClaimType  : " + claim.ClaimType);
                Console.WriteLine("ClaimValue : " + claim.Value);
                Console.WriteLine();
            }

            return outputIdentity;
        }

        /// <summary>
        /// Validates the appliesTo and throws an exception if the appliesTo is null or appliesTo contains some unexpected address.  
        /// </summary>
        private static void ValidateAppliesTo(EndpointAddress appliesTo)
        {
            if (appliesTo == null)
                throw new InvalidRequestException("The appliesTo is null.");

            return;
        }
    }
}
