using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AccurateAppend.SecurityTokenServer
{
    /// <summary>
    /// Provides Windows account name translation functions.
    /// </summary>
    public static class NameTranslator
    {
        #region P/Invoke Methods

        [DllImport( "Secur32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern bool TranslateName(
            [In(), MarshalAs( UnmanagedType.LPTStr )] string lpAccountName,
            int accountNameFormat,
            int desiredNameFormat,
            [Out(), MarshalAs( UnmanagedType.LPTStr )] string lpTranslatedName,
            ref uint nSize );

        #endregion

        #region Methods

        private static string TranslateName( string ntAccountName, int format )
        {
            if ( string.IsNullOrEmpty( ntAccountName ) )
                return string.Empty;

            uint size = 255U;
            string name = new string( '\0', (int) size );

            // 3 = Display Name
            // 8 = User Principal Name (UPN)

            if ( !TranslateName( ntAccountName, 2 /* SAM */, format, name, ref size ) )
            {
                // buffer is too small
                name = new string( '\0', (int) size );

                // something else is wrong
                if ( !TranslateName( ntAccountName, 2, format, name, ref size ) )
                    return ntAccountName;
            }

            // trim null-chars
            return name.Substring( 0, (int) ( size - 1U ) );
        }

        /// <summary>
        /// Gets the display name of the specified Windows NT account.
        /// </summary>
        /// <param name="accountName">The Windows NT account to get the display name for.</param>
        /// <returns>The display name for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetDisplayName( string accountName )
        {
            return GetDisplayName( new NTAccount( accountName ) );
        }

        /// <summary>
        /// Gets the display name of the specified Windows NT account.
        /// </summary>
        /// <param name="domainName">The name of the Windows NT domain of the account to translate.</param>
        /// <param name="userName">The name of the user (alias) account to translate.</param>
        /// <returns>The display name for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetDisplayName( string domainName, string userName )
        {
            return GetDisplayName( new NTAccount( domainName, userName ) );
        }

        /// <summary>
        /// Gets the display name of the specified Windows NT account.
        /// </summary>
        /// <param name="account">The <see cref="NTAccount"/> to get the display name for.</param>
        /// <returns>The display name for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetDisplayName( NTAccount account )
        {
            return TranslateName( account.Value, 3 );
        }

        /// <summary>
        /// Gets the User Principal Name (UPN) of the specified Windows NT account.
        /// </summary>
        /// <param name="accountName">The Windows NT account to get the display name for.</param>
        /// <returns>The User Principal Name (UPN) for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetUserPrincipalName( string accountName )
        {
            return GetUserPrincipalName( new NTAccount( accountName ) );
        }

        /// <summary>
        /// Gets the User Principal Name (UPN) of the specified Windows NT account.
        /// </summary>
        /// <param name="domainName">The name of the Windows NT domain of the account to translate.</param>
        /// <param name="userName">The name of the user (alias) account to translate.</param>
        /// <returns>The User Principal Name (UPN) for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetUserPrincipalName( string domainName, string userName )
        {
            return GetUserPrincipalName( new NTAccount( domainName, userName ) );
        }

        /// <summary>
        /// Gets the User Principal Name (UPN) of the specified Windows NT account.
        /// </summary>
        /// <param name="account">The <see cref="NTAccount"/> to get the display name for.</param>
        /// <returns>The User Principal Name (UPN) for the specified account.</returns>
        /// <remarks>If the translation fails, the original name is returned.</remarks>
        public static string GetUserPrincipalName( NTAccount account )
        {
            return TranslateName( account.Value, 8 );
        }

        #endregion
    }

}
