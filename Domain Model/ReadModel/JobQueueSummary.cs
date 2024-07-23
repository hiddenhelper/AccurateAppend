using System;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// An aggregated job summary view based on <see cref="JobQueueView"/>.
    /// </summary>
    public class JobQueueSummary
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected internal JobQueueSummary()
        {
        }

        #endregion

        #region Properties

        public Guid UserId { get; protected internal set; }

        public Int32 FileCount { get; protected internal set; }

        public DateTime LastActivity { get; protected internal set; }

        public Int32 MatchCount { get; protected internal set; }

        public Int32 RecordCount { get; protected internal set; }

        public String UserName { get; protected internal set; }

        #endregion
    }
}