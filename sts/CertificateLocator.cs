using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace AccurateAppend.SecurityTokenServer
{
    /// <summary>
    /// A utility class which helps to retrieve an x509 certificate
    /// </summary>
    public static class CertificateLocator
    {
        [SecurityPermission( SecurityAction.Demand, Action = SecurityAction.LinkDemand)]
        public static X509Certificate2 GetCertificate( StoreName name, StoreLocation location, String thumbprint )
        {
            var store = new X509Store( name, location );
            X509Certificate2Collection certificates = null;
            store.Open( OpenFlags.ReadOnly );

            try
            {
                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                X509Certificate2 result = (from certificate in store.Certificates.Cast<X509Certificate2>()
                                           where certificate.Thumbprint != null &&
                                                 certificate.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase)
                                           select certificate).FirstOrDefault();

                if ( result == null )
                {
                    throw new CryptographicException( string.Format( CultureInfo.InvariantCulture, "No certificate was found for thumbprint '{0}'", thumbprint ) );
                }
                return result;
            }
            finally
            {
                if ( certificates != null )
                {
                    for ( int i = 0; i < certificates.Count; i++ )
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }
    }
}
