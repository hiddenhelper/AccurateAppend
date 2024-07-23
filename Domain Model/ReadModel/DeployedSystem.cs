using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Core;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// A single registered AA application.
    /// </summary>
    public class DeployedSystem
    {
        #region Fields

        private DateTime heartbeat;

        #endregion

        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected DeployedSystem()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the instance.
        /// </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Gets the identifier of the identity the instance executes as.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the name of the identity the instance executes as.
        /// </summary>
        public virtual String UserName { get; protected set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> of the last heartbeat of the instance.
        /// </summary>
        public virtual DateTime Heartbeat
        {
            get { return this.heartbeat; }
            protected set { this.heartbeat = value.Coerce(); }
        }

        /// <summary>
        /// Gets the name of the system.
        /// </summary>
        public virtual String SystemName { get; protected set; }

        /// <summary>
        /// Gets the name of the server (NetBIOS) the instance is executing on.
        /// </summary>
        public virtual String Host { get; protected set; }

        /// <summary>
        /// Gets the version string of the entry executable of the deployed instance.
        /// </summary>
        public virtual String Version { get; protected set; }
        
        #endregion
    }

    internal class DeployedSystemConfiguration : EntityTypeConfiguration<DeployedSystem>
    {
        public DeployedSystemConfiguration()
        {
            this.ToTable("SystemsView", "admin");

            // Primary Key
            this.HasKey(s => s.Id);

            this.Property(s => s.Id).HasColumnName("InstanceId").IsRequired();
            this.Property(s => s.UserId).IsRequired();
            this.Property(s => s.UserName).IsUnicode(true).HasMaxLength(256).IsRequired();
            this.Property(s => s.SystemName).IsUnicode(false).HasMaxLength(100).IsRequired();
            this.Property(s => s.Host).HasColumnName("ExecutionHost").IsUnicode(false).HasMaxLength(15).IsRequired();
            this.Property(s => s.Heartbeat).IsRequired();
            this.Property(s => s.Version).HasMaxLength(25);
        }
    }
}