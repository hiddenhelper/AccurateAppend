using System;
using System.Data.Entity.ModelConfiguration;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// Readmodel for Contact Views.
    /// </summary>
    public class ContactView
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected ContactView()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current contact.
        /// </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary>
        /// Gets the user identifier of the current contact.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the identifier of the contact application.
        /// </summary>
        public virtual Guid ApplicationId { get; protected set; }

        /// <summary>
        /// Gets the email address of the current contact.
        /// </summary>
        public virtual String Email { get; protected set; }

        /// <summary>
        /// Gets the full name of the current contact.
        /// </summary>
        public virtual String Name { get; protected set; }

        #endregion
    }

    internal class ContactViewConfiguration : EntityTypeConfiguration<ContactView>
    {
        public ContactViewConfiguration()
        {
            this.ToTable("ContactsView", "admin");

            // Primary Key
            this.HasKey(c => c.Id);

            this.Property(c => c.UserId);
            this.Property(c => c.ApplicationId);
            this.Property(c => c.Name).HasColumnName("ContactName").IsUnicode().HasMaxLength(100);
            this.Property(c => c.Email).HasColumnName("EmailAddress").IsUnicode(false).HasMaxLength(50);
        }
    }
}
