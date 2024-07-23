using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using AccurateAppend.Core.ComponentModel;

// ReSharper disable CheckNamespace
namespace AccurateAppend.Security.FTPAdministration
{
    // ReSharper disable CommentTypo

    /// <summary>
    /// Calls to the SrxCOM which administers the Titan FTP server offered offered South River Technologies
    /// </summary>
    /// <remarks>
    /// regsvr32 srxCOM.dll # make sure to run copy in path, not the copy in SysWow64.
    /// 
    /// srxCOM.dll is 64 bit, so this project must be hosted in a 64bit process.
    /// 
    /// Documentation is available <see href="http://southrivertech.com/support/cornerstone/v10/webhelp/srxcom_overview.htm">here</see>
    /// 
    /// The following commands are available in COM but are not implemented here because I doubt we'll need them. They can be if needed:
    ///     GRP_Create()
    ///     GRP_Delete()
    ///     GRP_Enum()
    ///     GRP_GetMembers()
    ///     GRP_SetMembers()
    ///     GRP_GetAttr()
    ///     GRP_SetAttr()
    /// </remarks>
    public sealed class SrxComWrapper : DisposeableObject
    {
        #region Fields

        private static readonly Type srx = Type.GetTypeFromProgID("SrxCom.SrxTitan");
        private Object instance;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SrxComWrapper"/> class.
        /// </summary>
        public SrxComWrapper()
        {
            Debug.Assert(srx != null, "Titan COM is not registered");

            instance = Activator.CreateInstance(srx);
        }

        #endregion

        #region Global methods

        /// <summary>
        /// Connect to a service
        /// </summary>
        /// <param name="ipAddress">ip address or host name</param>
        /// <param name="port">port number, usually 31000 or 31001</param>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        public void SRX_Connect(String ipAddress, UInt16 port, String username, String password)
        {
            // SRX_Connect2( LPCTSTR szMachineIP, USHORT usPort, LPCTSTR szUsername, LPCTSTR szPassword, LONG* lError)  

            var args = new Object[] {ipAddress, port, username, password, 0};

            var pMod = new ParameterModifier(5);
            pMod[4] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SRX_Connect2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[4];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Disconnect from service
        /// </summary>
        public void SRX_Disconnect()
        {
            // SRX_Disconnect2( LONG* lError )

            var args = new Object[] {0};

            // set param 4 to be passed by ref
            var pMod = new ParameterModifier(1);
            pMod[0] = true;
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SRX_Disconnect2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[0];
            if (lError != 0) ThrowException(lError);
        }

        #endregion

        #region Server methods

        /// <summary>
        /// Starts a server
        /// </summary>        
        /// <remarks>
        /// 1) Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.
        /// 
        /// 2) Issuing SVR_Start on a running server does nothing.
        /// 
        /// 3) Titan has a concept of a "service" and a "server". The service can house any number of servers. This command stops the server, not the service.
        /// </remarks>
        /// <param name="serverName">server name</param>
        public void SVR_Start(String serverName)
        {
            // SVR_Start2(LPCTSTR szServerName, LONG* lError);

            var args = new Object[] {serverName, 0};

            var pMod = new ParameterModifier(2);
            pMod[1] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Start2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[1];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Stops the server
        /// </summary>        
        /// <remarks>        
        /// 1) Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.
        /// 
        /// 2) Also, issuing SVR_Stop on a stopped server does nothing.
        /// 
        /// 3) Titan has a concept of a "service" and a "server". The service can house any number of servers. This command stops the server, not the service.
        /// </remarks>
        /// <param name="serverName">server name to stop (note: Not the same as stopping the service)</param>
        public void SVR_Stop(String serverName)
        {
            // SVR_Stop2(LPCTSTR szServerName, LONG* lError);

            var args = new Object[] {serverName, 0};

            var pMod = new ParameterModifier(2);
            pMod[1] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Stop2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[1];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Restarts the specified server
        /// </summary>
        /// <remarks>
        /// Titan has a concept of a "service" and a "server". The service can house any number of servesr. This command stops the server, not the service.
        /// </remarks>
        /// <param name="serverName">Server name</param>
        public void SVR_Restart(String serverName)
        {
            // SVR_Restart2(LPCTSTR szServerName, LONG* lError);            

            var args = new Object[] {serverName, 0};

            var pMod = new ParameterModifier(5);
            pMod[1] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Restart2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[1];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Gets a list of server names on the service
        /// </summary>
        /// <returns>An enumeration of server names</returns>
        public IEnumerable<String> SVR_Enum()
        {
            // SVR_Enum2 ( BSTR* pszServerList, LONG* lError );

            var args = new Object[] {"", 0};

            // set param 4 to be passed by ref
            var pMod = new ParameterModifier(2);
            pMod[0] = true; // 0 pszServerList
            pMod[1] = true; // lError
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Enum2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[1];

            if (lError != 0) ThrowException(lError);


            var servers = ((String) args[0]) // eg "|server1|server2|server3|"
                .Split('|')
                .Where(server => !String.IsNullOrWhiteSpace(server))
                .ToList();

            return servers;
        }

        /// <summary>
        /// Returns list of active user sessions.
        /// </summary>
        /// <remarks>See the <see cref="TitanXml"/> for helpers.</remarks>
        /// <param name="serverName">name of server (not the host name)</param>        
        /// <returns>XML describing connected user sessions</returns>
        public XElement SVR_GetSessions(String serverName)
        {
            // xml format (from here http://southrivertech.com/support/cornerstone/v10/webhelp/svr_getsessions.htm ):
            // <Session>
            //  <SessionId>XXX</SessionId>
            //      <Username>users name</Username>
            //      <ClientIP>xx.yy.zz.aa</ClientIP>
            //      <Protocol>FTP or FTPS or SFTP</Protocol>
            //  </Session>
            //</SessionList>                

            // SVR_GetSessions(LPCTSTR szServerName, BSTR* pszSessionList, LONG* lError);

            var args = new Object[] {serverName, "", 0}; // last two params are placeholders for output

            var pMod = new ParameterModifier(3);
            pMod[1] = true; // pszSessionList
            pMod[2] = true; // lError
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_GetSessions2", // Titan's document incorrectly omits the "2", but it is required.
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[2];
            if (lError != 0) ThrowException(lError);

            var xml = XElement.Parse((String) args[1]);
            return xml;
        }

        /// <summary>
        /// Returns server attribute.
        /// </summary>
        /// <remarks>
        /// examples: 
        /// "IdleTimeout" (0 = disabled, 1 = enabled)
        /// "MaxDownloadNumMsg" (a the string that the server will return if the max number of downloads are reached)
        /// 
        /// note: the documentation has some errors. For example "SSLEnabled" and "SSLRequired" listed in the document are not actually valid.
        /// note: attribute names are not case sensitive
        /// </remarks>
        /// <param name="serverName">name of FTP server (not the host name)</param>
        /// <param name="attrName">Attribute name, IE "IdleTimeout"</param>
        /// <returns>The value of the requested attribute</returns>
        public String SVR_GetAttr(String serverName, String attrName)
        {
            // SVR_GetAttr2(LPCTSTR szServerName, LPCTSTR szAttrName, BSTR* pszAttrValue, LONG* lError);

            // attrNames are listed the Titan-Print.pdf, included in the solution                    

            var args = new Object[] {serverName, attrName, "", 0}; // last two params are placeholders for output

            // set param 4 to be passed by ref
            var pMod = new ParameterModifier(4);
            pMod[2] = true; // pszAttrValue
            pMod[3] = true; // lError
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_GetAttr2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var attrValue = (String) args[2];
            var lError = (int) args[3];

            if (lError != 0) ThrowException(lError);

            return attrValue;
        }

        /// <summary>
        /// Returns server attribute.
        /// </summary>
        /// <remarks>
        /// examples: 
        /// "IdleTimeout" (0 = disabled, 1 = enabled)
        /// "MaxDownloadNumMsg" (a the string that the server will return if the max number of downloads are reached)
        /// 
        /// note: the documentation has some errors. For example "SSLEnabled" and "SSLRequired" listed in the document are not actually valid.
        /// note: attribute names are not case sensitive
        /// </remarks>
        /// <param name="serverName">name of FTP server (not the host name)</param>
        /// <param name="attrName">Attribute name, IE "IdleTimeout"</param>
        /// <param name="attrValue">Attribute value</param>
        /// <returns>The value of the requested attribute</returns>
        public void SVR_SetAttr(String serverName, String attrName, String attrValue)
        {
            // SVR_SetAttr2(LPCTSTR szServerName, LPCTSTR szAttrName, LPCTSTR szAttrValue, LONG* lError);

            // attrNames are listed the Titan-Print.pdf, included in the solution                    

            var args = new Object[] {serverName, attrName, attrValue, 0}; // last two params are placeholders for output

            // set param 4 to be passed by ref
            var pMod = new ParameterModifier(4);
            pMod[3] = true; // lError
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_SetAttr2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int16) args[3];

            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Create a FTP server on the service.
        /// </summary>
        /// <remarks>
        /// Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.        
        /// </remarks>
        /// <param name="name">Server name</param>
        /// <param name="ip">local ip address</param>
        /// <param name="port">local port</param>
        /// <param name="baseDir">root directory of server</param>
        /// <param name="authType">0 = Titan user auth, 1 = NT user auth</param>
        public void SVR_Create(String name, String ip, UInt16 port, String baseDir, UInt16 authType)
        {
            // SVR_Create2(LPCTSTR szServerName, LPCTSTR szServerIP, unsigned short usServerPort, LPCTSTR szBaseDataDir, unsigned short usAuthType, LONG* lError)

            var args = new Object[] {name, ip, port, baseDir, authType, 0};

            var pMod = new ParameterModifier(6);
            pMod[5] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Create2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (int) args[5];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Delete an FTP server on the service.
        /// </summary>
        /// <remarks>        
        /// Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.
        /// </remarks>
        /// <param name="name">Server name</param>
        public void SVR_Delete(String name)
        {
            if (String.Equals(name, "Client FTP")) throw new ApplicationException("TitanFtpCOM refuses to delete production FTP server"); // To protect against accidents. 

            // SVR_Delete2(LPCTSTR szServerName, LONG* lError);

            var args = new Object[] {name, 0};

            var pMod = new ParameterModifier(2);
            pMod[1] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SVR_Delete2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[1];
            if (lError != 0) ThrowException(lError);
        }

        #endregion

        #region User methods

        /// <summary>
        /// Create a user account.
        /// </summary>
        /// <remarks>
        /// 1) Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.
        ///
        /// 2) If userName already exists is specified the error message incorrectly says "Error: 5006, A group by this name already exists" 
        /// </remarks>
        /// <param name="serverName">Server name</param>
        /// <param name="userName">username</param>
        /// <param name="password">password</param>
        public void USR_Create(String serverName, String userName, String password)
        {
            // USR_Create2(LPCTSTR szServerName, LPCTSTR szUserName, LPCTSTR szPassword, LONG* lError );

            var args = new Object[] {serverName, userName, password, 0};

            var pMod = new ParameterModifier(4);
            pMod[3] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "USR_Create2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[3];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Delete a user account
        /// </summary>
        /// <remarks>        
        /// Open copies of Titan Administration Console will not reflect updates (even if they are disconnected/reconnected). The application must be closed/reopened.
        /// </remarks>
        /// <param name="serverName">Server name</param>
        /// <param name="userName">User name</param>
        public void USR_Delete(String serverName, String userName)
        {
            // USR_Delete2(LPCTSTR szServerName, LPCTSTR szUserName, LONG* lError );

            var args = new Object[] {serverName, userName, 0};

            var pMod = new ParameterModifier(3);
            pMod[2] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "USR_Delete2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[2];
            if (lError != 0) ThrowException(lError);
        }

        /// <summary>
        /// Lists user accounts on the server.
        /// </summary>
        /// <param name="serverName">Server name</param>
        public IEnumerable<String> USR_Enum(String serverName)
        {
            // USR_Enum2 (LPCTSTR szServerName, BSTR* pszUserList, LONG* lError);

            var args = new Object[] {serverName, "", 0};

            var pMod = new ParameterModifier(3);
            pMod[1] = true; // pszUserList          
            pMod[2] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "USR_Enum2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[2];
            if (lError != 0) ThrowException(lError);

            var users = ((String) args[1]) // eg "user1|user2|". 
                .Split('|')
                .Where(server => !String.IsNullOrWhiteSpace(server)) // the IsNullOrWhiteSpace check isn't necessary with this method, but it is for SVR_Enum, so lets keep it.
                .ToArray();

            return users;
        }

        /// <summary>
        /// Returns user attribute
        /// </summary>
        /// <remarks>        
        /// attrNames are listed on page 122 of Titan docs\Titan-Print.pdf
        /// "AcctDisabledMsg" (default is blank)
        /// "AllowMDTM" (default is 2)
        /// attribute names are case insensitive.
        /// </remarks>
        /// <param name="serverName">name of FTP server (not the host name)</param>
        /// <param name="userName">User name</param>
        /// <param name="attrName">name of attribute to retrieve</param>
        /// <returns></returns>
        public String USR_GetAttr(String serverName, String userName, String attrName)
        {
            // USR_GetAttr2(LPCTSTR szServerName, LPCTSTR szUserName, LPCTSTR szAttrName, BSTR* pszAttrValue, LONG* lError);

            var args = new Object[] {serverName, userName, attrName, "", 0}; // last two params are placeholders for output

            var pMod = new ParameterModifier(5);
            pMod[3] = true; // pszAttrValue
            pMod[4] = true; // lError
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "USR_GetAttr2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var attrValue = (String) args[3];
            var lError = (int) args[4];

            if (lError != 0) ThrowException(lError);

            return attrValue;
        }

        /// <summary>
        /// Set a user attribute
        /// </summary>
        /// <remarks>
        /// If a non-existant user is specified the error message incorrectly says "Error: 5007, The specified attribute does not exist"
        /// </remarks>        
        /// <param name="serverName">Server name</param>
        /// <param name="userName">user name</param>
        /// <param name="attrName">attribute name</param>
        /// <param name="attrValue">value to set attribute to</param>
        public void USR_SetAttr(String serverName, String userName, String attrName, String attrValue)
        {
            // USR_SetAttr2(LPCTSTR szServerName, LPCTSTR szUserName, LPCTSTR szAttrName, LPCTSTR szAttrValue, LONG* lError);

            var args = new Object[] {serverName, userName, attrName, attrValue, 0};

            var pMod = new ParameterModifier(5);
            pMod[4] = true; // lError          
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "USR_SetAttr2",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var lError = (Int32) args[4];
            if (lError != 0) ThrowException(lError);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Converts SrxCOM lERROR code into a human friendly message.
        /// </summary>
        /// <param name="rc">The COM return code.</param>
        /// <returns>The human readable error message.</returns>
        public String SRX_GetErrStr(Int32 rc)
        {
            // void SRX_GetErrStr ( long lRetCode, BSTR* pszErrStr );

            //var args = new Object[] { rc, default(IntPtr) }; // above signature seems incorrect. If output is BSTR* we want an IntPtr then to decode it
            var args = new Object[] {rc, ""};

            // set param 4 to be passed by ref
            var pMod = new ParameterModifier(2);
            pMod[1] = true;
            ParameterModifier[] mods = {pMod};

            srx.InvokeMember(
                "SRX_GetErrStr",
                BindingFlags.InvokeMethod,
                null,
                instance,
                args,
                mods,
                null,
                null);

            var reply = (String) args[1];
            return reply;
        }

        private void ThrowException(Int32 lError)
        {
            String message;

            try
            {
                // attempt to contact the object to replace the error code with a meaningful message
                message = this.SRX_GetErrStr(lError);
                throw new COMException(message); // look at the previous method in the stack trace
            }
            catch (COMException)
            {
                throw;
            }
            catch (Exception ex)
            {
                message = $"SrxCom lError={lError}"; // if it fails for some reason at least return the error code.
                throw new COMException(message, ex); // look at the previous method in the stack trace
            }
        }

        #endregion

        #region Disposable

        /// <inheritdoc />
        ~SrxComWrapper()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        protected override void Dispose(Boolean disposing)
        {
            if (!disposing || this.instance == null) return;

            // free managed resources
            Marshal.ReleaseComObject(instance);
            this.instance = null;
        }

        #endregion
    }

    // ReSharper restore CommentTypo
}
// ReSharper restore CheckNamespace