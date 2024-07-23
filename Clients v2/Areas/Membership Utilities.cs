using System;
using System.Web.Security;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Temporary type used to locate some common logic and routines in membership signup
    /// </summary>
    internal static class MembershipUtilities
    {
        internal static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "The e-mail address provided already has an account. Please contact customer support.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact support.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact support.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact support.";
            }
        }

        internal static String ErrorCodeToModelMember(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                case MembershipCreateStatus.DuplicateEmail:
                case MembershipCreateStatus.InvalidEmail:
                case MembershipCreateStatus.InvalidUserName:
                    return nameof(CreateAccountModelBase.Email);

                case MembershipCreateStatus.InvalidPassword:
                    return nameof(CreateAccountModelBase.Password);

                default:
                    return String.Empty;
            }
        }
    }
}