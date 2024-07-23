using System;
namespace AccurateAppend.Security.FTPAdministration
{
    /// <summary>
    /// POCO describing result of account setup procedure
    /// </summary>
    public class AccountSetupResult
    {
        /// <summary>
        /// Internal path to customer file transfer directory
        /// </summary>
        public String Location;

        /// <summary>
        /// Selected username
        /// </summary>
        public String Username;

        /// <summary>
        /// Account enabled indicator
        /// </summary>
        public Boolean IsActive;
    }
}
