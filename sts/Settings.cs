using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace AccurateAppend.SecurityTokenServer
{
    public static class Settings
    {
        // Issuer name placed into issued tokens
        internal const String StsName = "Stub STS";

        // Statics for location of certs
        internal static readonly StoreName CertStoreName = StoreName.My;
        internal static readonly StoreLocation CertStoreLocation = StoreLocation.LocalMachine;

        // Statics initialized from app.config
        internal static readonly string CertThumbprint = LoadAppSetting("certThumbprint");
        internal static readonly string IssuerAddress = LoadAppSetting("issuerAddress");
        internal static readonly string EncryptionThumbprint = LoadAppSetting("rpThumbprint");

        #region Helper functions to load app settings from config

        /// <summary>
        /// Helper function to load Application Settings from config
        /// </summary>
        private static string LoadAppSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            CheckIfLoaded(value);
            return value;
        }

        /// <summary>
        /// Helper function to check if a required Application Setting has been specified in config.
        /// Throw if some Application Setting has not been specified.
        /// </summary>
        private static void CheckIfLoaded(String s)
        {
            if (String.IsNullOrEmpty(s))
                throw new ConfigurationErrorsException("Required Configuration Element(s) missing at Stub STS. Please check the STS configuration file.");
        }

        #endregion
    }
}
