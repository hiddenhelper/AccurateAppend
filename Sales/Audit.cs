using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A textual content entry that can be appeneded to various entities.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Content) + "}")]
    public class Audit : IKeyedObject<Int32?>, IEquatable<Audit>
    {
        #region Fields

        private String content;
        private Guid createdBy;
        private DateTime createdDate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected Audit()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        /// <param name="content">The textual content of the note.</param>
        /// <param name="createdBy">The identifier of the logon that caused the created the audit instance.</param>
        public Audit(String content, Guid createdBy) : this(content, createdBy, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        /// <param name="content">The textual content of the note.</param>
        /// <param name="createdBy">The identifier of the logon that caused the created the audit instance.</param>
        /// <param name="onDate">The <see cref="DateTime"/> the audit event occured.</param>
        public Audit(String content, Guid createdBy, DateTime onDate)
        {
            if (String.IsNullOrWhiteSpace(content)) throw new ArgumentNullException(nameof(content));
            if (createdBy == null) throw new ArgumentNullException(nameof(createdBy));
            Contract.EndContractBlock();

            this.content = content.Trim();
            this.createdBy = createdBy;
            this.CreatedDate = onDate.Coerce();
        }

        #endregion

        #region IEquatable<T> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean Equals(Audit other)
        {
            if (other == null) return false;
            if (this.Id == null && other.Id == null)
            {
                return ReferenceEquals(this, other);
            }

            if (other.Id == null) return false;

            return this.Id == other.Id;
        }


        /// <inheritdoc />
        public override Boolean Equals(Object other)
        {
            return this.Equals(other as Audit);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override String ToString()
        {
            return this.Content;
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current instance.
        /// </summary>
        /// <value>The identifier for the current instance.</value>
        public Int32? Id { get; private set; }

        /// <summary>
        /// Gets or sets the textual content of the note.
        /// </summary>
        /// <value>The textual content of the note.</value>
        /// <exception cref="InvalidOperationException">The current instance is persisted and therefore read-only.</exception>
        public virtual String Content
        {
            get { return this.content; }
            protected set
            {
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
                
                this.content = value.Trim();
            }
        }

        /// <summary>
        /// Gets the idetnifier of the logong that caused the audit.
        /// </summary>
        /// <value>The idetnifier of the logong that caused the audit.</value>
        public virtual Guid CreatedBy
        {
            get
            {
                return this.createdBy;
            }
            protected set { this.createdBy = value; }
        }

        /// <summary>
        /// Gets the date and time (in UTC) that the current instance was created on.
        /// </summary>
        /// <value>The date and time (in UTC) that the current instance was created on.</value>
        public DateTime CreatedDate
        {
            get { return this.createdDate; }
            protected set { this.createdDate = value.Coerce(); }
        }

        #endregion
    }
}
