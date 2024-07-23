using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Provides a combined view of all the standard file locations used by the application.
    /// </summary>
    /// <remarks>
    /// Sometimes components require large numbers of file locations in their operation. To ease
    /// the overhead and reduce the configuration complexity we can use this composite when needed
    /// to hit the easy button with. Especially good to shorten constructor parameters with.
    /// </remarks>
    public class FileContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContext"/> class.
        /// </summary>
        /// <param name="inbox">The <see cref="Inbox"/> folder location.</param>
        /// <param name="outbox">The <see cref="Outbox"/> folder location.</param>
        /// <param name="assisted">The <see cref="Assisted"/> folder location.</param>
        /// <param name="rawCustomer">The <see cref="RawCustomer"/> folder location.</param>
        /// <param name="temp">The <see cref="Temp"/> folder location.</param>
        /// <param name="legacyManifest">The <see cref="LegacyManifest"/> folder location.</param>
        public FileContext(IFileLocation inbox,
            IFileLocation outbox,
            IFileLocation assisted,
            IFileLocation rawCustomer,
            IFileLocation temp,
            IFileLocation legacyManifest)
        {
            if (inbox == null) throw new ArgumentNullException(nameof(inbox));
            if (outbox == null) throw new ArgumentNullException(nameof(outbox));
            if (assisted == null) throw new ArgumentNullException(nameof(assisted));
            if (rawCustomer == null) throw new ArgumentNullException(nameof(rawCustomer));
            if (temp == null) throw new ArgumentNullException(nameof(inbox));
            if (legacyManifest == null) throw new ArgumentNullException(nameof(legacyManifest));
            Contract.EndContractBlock();

            this.Inbox = inbox;
            this.Outbox = outbox;
            this.Assisted = assisted;
            this.RawCustomer = rawCustomer;
            this.Temp = temp;
            this.LegacyManifest = legacyManifest;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Provides access to the normalized but not appended customer files location.
        /// </summary>
        public IFileLocation Inbox { get; }

        /// <summary>
        /// Provides access to the appended customer files location.
        /// </summary>
        public IFileLocation Outbox { get; }

        /// <summary>
        /// Provides access to the assisted files location.
        /// </summary>
        public IFileLocation Assisted { get; }

        /// <summary>
        /// Provides access to the raw customer files location where unaltered original client files are stored.
        /// </summary>
        public IFileLocation RawCustomer { get; }

        /// <summary>
        /// Provides access to a temporary storage location.
        /// </summary>
        public IFileLocation Temp { get; }

        /// <summary>
        /// Provides access to the legacy compatibility manifest location.
        /// </summary>
        public IFileLocation LegacyManifest { get; }

        #endregion
    }
}