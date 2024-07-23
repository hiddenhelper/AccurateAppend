using System;
using System.Data.Entity.ModelConfiguration;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// A single failed bus message.
    /// </summary>
    public class FailedBusMessage
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected FailedBusMessage()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the instance.
        /// </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Gets the identifier of used to correlated messages.
        /// </summary>
        public virtual String CorrelationId { get; protected set; }

        /// <summary>
        /// Gets the name of the queue the failed message is from.
        /// </summary>
        public virtual String Queue { get; protected set; }

        /// <summary>
        /// Gets the headers of the failed message as JSON.
        /// </summary>
        public virtual String Headers { get; protected set; }

        /// <summary>
        /// Gets the content of the failed message as JSON.
        /// </summary>
        public virtual String Body { get; protected set; }

        #endregion
    }

    internal class FailedBusMessageConfiguration : EntityTypeConfiguration<FailedBusMessage>
    {
        public FailedBusMessageConfiguration()
        {
            this.ToTable("FailedMessages", "bus");

            // Primary Key
            this.HasKey(s => s.Id);

            this.Property(s => s.Id).IsRequired();
            this.Property(s => s.CorrelationId).IsUnicode(false).HasMaxLength(255).IsRequired();
            this.Property(s => s.Queue).IsUnicode(false).HasMaxLength(255).IsRequired();
            this.Property(s => s.Headers).IsUnicode(false).IsMaxLength().IsRequired();
            this.Property(s => s.Body).IsUnicode(false).IsMaxLength().IsRequired();
        }
    }
}
