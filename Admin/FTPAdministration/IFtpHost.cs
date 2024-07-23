using System;

namespace AccurateAppend.Security.FTPAdministration
{
    /// <summary>
    /// Represents a remote FTP service system management API for use in the specific needs of the AA system.
    /// </summary>
    public interface IFtpHost
    {
        /// <summary>
        /// Creates a user on the FTP server
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        /// <param name="enabled">enabled/disabled</param>
        /// <returns>AccountSetupResult</returns>
        AccountSetupResult CreateUser(String userName, String password, Boolean enabled);

        /// <summary>
        /// Set account status
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="enabled">account enabled value</param>
        void Switch(String userName, Boolean enabled);

        /// <summary>
        /// Set account status
        /// </summary>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        void ResetPassword(String userName, String password);

        /// <summary>
        /// Set account status
        /// </summary>
        /// <param name="userName">username</param>        
        Boolean UserExists(String userName);

        /// <summary>
        /// Set account status
        /// </summary>
        /// <param name="userName">username</param>        
        void DeleteUser(String userName);       
    }
}
