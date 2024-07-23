using System;
using System.IO;
using System.Linq;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Security.FTPAdministration
{
    /// <summary>
    /// Provides the implementation of the <see cref="IFtpHost"/> api for interacting with the Titan FTP server software.
    /// </summary>
    /// <threadsafety instance="true" static="true"/>
    public class TitanFacade : IFtpHost
    {
        #region Fields

        private readonly String host; // the alternative to storing these in fields would be to create the SrxComWrapper here and connect now, then disconnect in the destructor, but that's both too error prone and long lived.
        private readonly UInt16 port;
        private readonly String server;
        private readonly String credential;
        private readonly String passKey;
        private readonly NtfsFileLocation ftpRoot;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TitanFacade"/> class.
        /// </summary>
        /// <param name="host">host titan service is running on</param>
        /// <param name="credential">Service login username</param>
        /// <param name="passKey">Service login password</param>
        /// <param name="ftpRoot">Location of FTP root</param>
        /// <param name="port">port titan service is running on</param>
        /// <param name="server">Name of FTP server instance on service.</param>
        /// <remarks>
        /// Service credential and passKey are the credentials for administration of the server, not the credentials for the user account.
        /// </remarks>
        public TitanFacade(String host, String credential, String passKey, NtfsFileLocation ftpRoot, String server, UInt16 port)
        {
            this.host = host;
            this.port = port; 
            this.server = server; // Our service houses a single server called "Client FTP" 
            this.credential = credential;
            this.passKey = passKey;
            this.ftpRoot = ftpRoot;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a user account on the FTP server, sets homedir, set enabled
        /// </summary>
        /// <param name="userName">user account name</param>
        /// <param name="password">user account password</param>
        /// <param name="enabled">enabled/disabled</param>
        /// <remarks>Will replace any preexisting directory</remarks>
        /// <returns>Account setup result</returns>
        public AccountSetupResult CreateUser(String userName, String password, Boolean enabled)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            if (String.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            var cleaned = new String(userName.Trim().Where(c => !Path.GetInvalidPathChars().Contains(c)).ToArray());

            if (!String.Equals(userName, cleaned, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(userName), userName, $"{nameof(CreateUser)} cannot create directory {userName} due to invalid characters");
            }

            var userFtpDir = this.ftpRoot.Rebase(userName).AsDirectoryInfo();

            // We must give the user a blank dir for security reasons.  Even if we 
            // think there cant be another user by this name so it's safe, if that assumption
            // ever fails it will be a data breach in the current climate. So this is critical.
            //
            // On the other hand if we delete a customer's directory that might be one pissed off customer. 
            //
            // Weighing these two, and the desire for restartability of processes, I decided the better thing to do 
            // is document this method as nuking any preexisting directory.
            
            if (userFtpDir.Exists) Directory.Delete(userFtpDir.ToString(),true); 
            
            userFtpDir.Create(); 

            using (var ftpService = new SrxComWrapper())
            {
                ftpService.SRX_Connect(host, port, credential, passKey);

                //TODO: What if it already exists? Perhaps we can check and throw an exception?
                //^might be a good idea^ the message handler, in actual use here as the caller, will stick the request on the error queue for admin correction so we're kinda golden if you take that approach. Interactive code, if every done, would know to expect the exception and can react appropriately. Good thinking.
                ftpService.USR_Create(server, userName, password);
                ftpService.USR_SetAttr(server, userName, "HomeDir", userFtpDir.FullName); // this "HomeDir" attr name is not documented in titan's instructions
                ftpService.USR_SetAttr(server, userName, "Enabled", (enabled ? 2 : 0).ToString()); // 2 = default "enabled" value. 0 = disabled. 1 seems to be enabled, but i'm not sure what it means.
                ftpService.SRX_Disconnect();

                return new AccountSetupResult() {Location = userFtpDir.FullName, Username = userFtpDir.Name, IsActive = true};
            }
        }

        public Boolean UserExists(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            var users = Enumerable.Empty<String>();

            using (var ftpService = new SrxComWrapper())
            {
                ftpService.SRX_Connect(host, port, credential, passKey);
                users = ftpService.USR_Enum(server);
                ftpService.SRX_Disconnect();
            }

            return users.Contains(userName);
        }

        public void DeleteUser(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var ftpService = new SrxComWrapper())
            {
                ftpService.SRX_Connect(host, port, credential, passKey);
                ftpService.USR_Delete(server, userName);
                ftpService.SRX_Disconnect();
            }            
        }

        /// <summary>
        /// Enables/disables a user account
        /// </summary>        
        /// <param name="userName">user account name</param>
        /// <param name="enabled">enabled true/false</param>
        public void Switch(String userName, Boolean enabled)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            using (var ftpService = new SrxComWrapper())
            {
                ftpService.SRX_Connect(host, port, credential, passKey);
                ftpService.USR_SetAttr(server, userName, "Enabled", (enabled ? 2 : 0).ToString()); // 2 = enabled. 0 = disabled.  1 =.. not sure.
                ftpService.SRX_Disconnect();
            }
        }

        /// <summary>
        /// Enables/disables a user account
        /// </summary>        
        /// <param name="userName">user account name</param>
        /// <param name="password">new password</param>
        public void ResetPassword(String userName, String password)
        {
            if (String.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            if (String.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            using (var ftpService = new SrxComWrapper())
            {
                ftpService.SRX_Connect(host, port, credential, passKey);
                ftpService.USR_SetAttr(server, userName, "Password", password);
                ftpService.SRX_Disconnect();
            }
        }
    }
   
    #endregion
}