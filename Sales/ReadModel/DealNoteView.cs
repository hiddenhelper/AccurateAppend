#pragma warning disable SA1402 // File may only contain a single class
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.ReadModel
{
    /// <summary>
    /// Common view model element for representing a single deal note.
    /// </summary>
    public class DealNoteView
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected DealNoteView()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current model.
        /// </summary>
        /// <value>The identifier for the current model.</value>
        [Required()]
        public Int32 NoteId { get; set; }

        /// <summary>
        /// Gets the identifier for the current model.
        /// </summary>
        /// <value>The identifier for the current model.</value>
        [Required()]
        public Int32 DealId { get; set; }

        /// <summary>
        /// Gets the content for the current model.
        /// </summary>
        /// <value>The content for the current model.</value>
        [Required()]
        [DataType(DataType.MultilineText)]
        [Display(Name = "")]
        public String Content { get; set; }

        /// <summary>
        /// Gets the user name of the account that created the current model.
        /// </summary>
        /// <value>The user name of the account that created the current model.</value>
        [Required()]
        [DataType(DataType.Text)]
        [Display(Name = "Added By")]
        public String CreatedBy { get; set; }

        /// <summary>
        /// Gets the date the current model was created.
        /// </summary>
        /// <value>The date the current model was created.</value>
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime CreatedDate { get; set; }

        #endregion
    }

    internal class DealNoteViewConfiguration : EntityTypeConfiguration<DealNoteView>
    {
        public DealNoteViewConfiguration()
        {
            this.ToTable("DealNotesView", "sales");

            // Primary Key
            this.HasKey(n => n.NoteId);

            this.Property(n => n.Content).HasColumnName("Body").IsUnicode(true).HasMaxLength(4000).IsRequired();
            this.Property(n => n.CreatedBy).HasColumnName("UserName").IsUnicode(true).HasMaxLength(256).IsRequired();
            this.Property(n => n.CreatedDate).HasColumnName("DateAdded").IsRequired();
            this.Property(n => n.DealId).IsRequired();
            this.Property(n => n.NoteId).IsRequired();
        }
    }
}