using System;
using System.Diagnostics.Contracts;
using System.IO;
using AccurateAppend.Core.Utilities;
using Castle.Core.Resource;

namespace AccurateAppend.Websites.Clients.Utilities
{
    /// <summary>
    /// <see cref="FileProxy"/> adapter to wrap a Castle <see cref="FileResource"/> resource.
    /// </summary>
    /// <remarks>
    /// The Castle resource component makes using readonly on-disk in application deployed content files trivial to leverage and hard to use inappropriately.
    /// </remarks>
    internal sealed class ResourceFileProxy : FileProxy
    {
        #region Fields

        private readonly FileResource resource;

        #endregion

        #region Constructor

        internal ResourceFileProxy(FileResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            Contract.EndContractBlock();

            this.resource = resource;
        }

        #endregion

        #region Overrides

        public override Stream OpenStream(FileAccess accessLevel, Boolean async = false)
        {
            if (accessLevel != FileAccess.Read) throw new NotSupportedException($"{nameof(ResourceFileProxy)} instances only support reading");

            return this.resource.CreateStream();
        }

        public override Boolean Exists()
        {
            return true;
        }

        public override void Delete()
        {
            throw new NotSupportedException($"{nameof(ResourceFileProxy)} instances only support reading");
        }

        public override void Create()
        {
        }

        public override DateTime CreatedDate()
        {
            return DateTime.UtcNow;
        }

        #endregion
    }
}