using System;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Clients.Messages.Accounts
{
    /// <summary>
    /// Implementation of <see cref="IReverseFileProxyFactory"/> that always returns <see cref="NullFile.Null"/>
    /// </summary>
    internal sealed class NullReverseFileProxyFactory : IReverseFileProxyFactory
    {
        internal static readonly NullReverseFileProxyFactory Instance = new NullReverseFileProxyFactory();

        private NullReverseFileProxyFactory()
        {
        }

        public FileProxy FromAbsoluteUri(Uri uri)
        {
            return NullFile.Null;
        }
    }
}