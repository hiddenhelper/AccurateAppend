using System;
using System.Diagnostics.Contracts;
using System.IO;
using AccurateAppend.Core.Utilities;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Clients.Utilities
{
    /// <summary>
    /// Used to create <see cref="ResourceFileProxy"/> instances at a relative location on disk from the application root.
    /// </summary>
    internal sealed class ResourceFileProxyFactory : IFileLocation
    {
        #region Fields

        private readonly String pathRoot;

        #endregion

        #region Constructor

        public ResourceFileProxyFactory(String pathRoot)
        {
            if (String.IsNullOrWhiteSpace(pathRoot)) throw new ArgumentNullException(nameof(pathRoot));

            Contract.EndContractBlock();

            this.pathRoot = pathRoot;
        }

        #endregion

        #region IFileLocation Members

        public FileProxy CreateInstance(String fileName)
        {
            var resource = new FileResource(Path.Combine(this.pathRoot, fileName));

            return new ResourceFileProxy(resource);
        }

        #endregion
    }
}