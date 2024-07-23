using System;
using AccurateAppend.Websites.Clients.Areas.Authentication.Shared.Models;

namespace AccurateAppend.Websites.Clients.Areas.Authentication.Direct.Models
{
    /// <summary>
    /// ViewModel to interact with direct AccurateAppend account logon process.
    /// </summary>
    public class DirectLoginModel : LoginModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectLoginModel"/> class.
        /// </summary>
        public DirectLoginModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectLoginModel"/> class with the supplied <paramref name="redirectTo"/> value.
        /// </summary>
        /// <param name="redirectTo">The URL that the user should be redirected to after logon.</param>
        public DirectLoginModel(String redirectTo) : base(redirectTo)
        {
        }

        #endregion
    }
}