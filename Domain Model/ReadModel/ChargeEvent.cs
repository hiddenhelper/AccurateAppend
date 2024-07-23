using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Core;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// A single credit card charge event.
    /// </summary>
    public class ChargeEvent
    {
        #region Fields

        private DateTime eventDate;

        #endregion

        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected ChargeEvent()
        {
        }

        #endregion

        #region Properties

        public virtual Int32 Id { get; protected set; }

        public virtual Decimal Amount { get; protected set; }

        public virtual Guid UserId { get; protected set; }

        public virtual Guid ApplicationId { get; protected set; }

        public virtual String UserName { get; protected set; }

        public virtual DateTime EventDate
        {
            get { return this.eventDate; }
            protected set { this.eventDate = value.Coerce(); }
        }

        public virtual String DisplayValue { get; protected set; }

        public virtual String ExpirationDate { get; protected set; }

        public virtual String Status { get; protected set; }

        public virtual String FullName { get; protected set; }

        public virtual Int32 OrderId { get; protected set; }

        public virtual Int32 DealId { get; protected set; }

        public virtual String TransactionId { get; protected set; }

        public virtual String TransactionType { get; protected set; }

        public virtual String AuthorizationCode { get; protected set; }

        public virtual String Message { get; protected set; }

        public virtual String Address { get; protected set; }

        public virtual String City { get; protected set; }

        public virtual String State { get; protected set; }

        public virtual String ZipCode { get; protected set; }
        
        #endregion
    }

    internal class ChargeEventConfiguration : EntityTypeConfiguration<ChargeEvent>
    {
        public ChargeEventConfiguration()
        {
            this.ToTable("ChargeEvents", "admin");

            // Primary Key
            this.HasKey(c => c.Id);

            this.Property(c => c.Amount).HasPrecision(19, 4);
            this.Property(c => c.Address).IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.City).IsUnicode(false).HasMaxLength(50);
            this.Property(c => c.FullName).IsUnicode(false).HasMaxLength(200);
            this.Property(c => c.State).IsUnicode(false).HasMaxLength(50);
            this.Property(c => c.Status).HasMaxLength(25);
            this.Property(c => c.ZipCode).IsUnicode(false).HasMaxLength(10);
            this.Property(c => c.AuthorizationCode).IsUnicode(false).HasMaxLength(15);
            this.Property(c => c.Message).IsUnicode(false).HasMaxLength(250);
            this.Property(c => c.TransactionId).IsUnicode(false).HasMaxLength(50);
            this.Property(c => c.TransactionType).IsUnicode(false).HasMaxLength(25);
            this.Property(c => c.DisplayValue).IsUnicode(false).IsFixedLength().HasMaxLength(4);
            this.Property(c => c.ExpirationDate).IsUnicode(false).HasMaxLength(8);
            this.Property(c => c.OrderId);
            this.Property(c => c.DealId);
        }
    }
}