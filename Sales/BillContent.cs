using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Xml.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.ComponentModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A business communication from Accurate Append to a client.
    /// Communications can have files attached, contain a list of 
    /// recipients public and private.
    /// </summary>
    /// <remarks>
    /// Currently a subset of the older Message entity in the AA system. This is a proper table/entity
    /// where this type contains draft content outside of the tracked messages and then if approved,
    /// can be enqueued for delivery with another service thus isolating delivery and scheduling concerns
    /// here. Likewise the mail queue system can then be isolated from the editable concepts on the
    /// receipt/invoice code.
    /// </remarks>
    [DebuggerDisplay("Id = {" + nameof(Id) + "}")]
    public class BillContent : IKeyedObject<Int32?>, ISupportInitializeNotification
    {
        #region Fields

        private readonly LockableList<MailAddress> sendTo;
        private readonly LockableList<MailAddress> bccTo;
        private readonly LockableList<FileAttachment> attachments;
        private String sendToDb;
        private String bccToDb;
        private String attachmentsDb;
        private readonly XElement sendToInternal;
        private readonly XElement bccToInternal;
        private readonly XElement attachmentsInternal;
        private String sendFrom;
        private String body;
        private Boolean isHtml;
        private String subject;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BillContent"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected BillContent()
        {
            var sendTo = new ObservableCollection<MailAddress>();
            sendTo.CollectionChanged += (sender, e) => this.OnCollectionChanged(this.sendToInternal, this.SendTo);
            this.sendTo = new LockableList<MailAddress>(sendTo);
            this.SendToDb = "<emails/>";
            this.sendToInternal = XElement.Parse(this.sendToDb);
            this.sendToInternal.Changed += (sender, e) => this.sendToDb = this.sendToInternal.ToString();

            var attachments = new ObservableCollection<FileAttachment>();
            attachments.CollectionChanged +=
                (sender, e) => this.OnCollectionChanged(this.attachmentsInternal, this.Attachments);
            this.attachments = new LockableList<FileAttachment>(attachments);
            this.AttachmentsDb = "<files/>";
            this.attachmentsInternal = XElement.Parse(this.attachmentsDb);
            this.attachmentsInternal.Changed += (sender, e) => this.attachmentsDb = this.attachmentsInternal.ToString();

            var bccTo = new ObservableCollection<MailAddress>();
            bccTo.CollectionChanged += (sender, e) => this.OnCollectionChanged(this.bccToInternal, this.BccTo);
            this.bccTo = new LockableList<MailAddress>(bccTo);
            this.BccToDb = "<emails/>";
            this.bccToInternal = XElement.Parse(this.bccToDb);
            this.bccToInternal.Changed += (sender, e) => this.bccToDb = this.bccToInternal.ToString();

            this.subject = String.Empty;
            this.body = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillContent"/> class.
        /// </summary>
        /// <param name="sendFrom">The email address to send from.</param>
        public BillContent(String sendFrom) : this()
        {
            if (sendFrom == null) throw new ArgumentNullException(nameof(sendFrom));
            Contract.EndContractBlock();

            this.SendFrom = sendFrom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillContent"/> class.
        /// </summary>
        /// <param name="sendFrom">The email address to send from.</param>
        public BillContent(MailAddress sendFrom) : this()
        {
            if (sendFrom == null) throw new ArgumentNullException(nameof(sendFrom));
            Contract.EndContractBlock();

            this.SendFrom = sendFrom.Address;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier for the current instance.
        /// </summary>
        /// <value>The identifier for the current instance.</value>
        public virtual Int32? Id { get; private set; }

        /// <summary>
        /// Used to marshall the XML into and out of EF.
        /// </summary>
        private String SendToDb
        {
            // ReSharper disable UnusedMember.Local
            get { return this.sendToDb; }
            // ReSharper restore UnusedMember.Local
            set
            {
                this.sendToDb = value;

                if (value != null && this.sendTo.Count == 0)
                {
                    var xml = XElement.Parse(value);
                    xml.Descendants().Select(n => new MailAddress(n.Value)).ToList().ForEach(a => this.SendTo.Add(a));
                }
            }
        }

        /// <summary>
        /// Used to marshall the XML into and out of EF.
        /// </summary>
        private String BccToDb
        {
            // ReSharper disable UnusedMember.Local
            get { return this.bccToDb; }
            // ReSharper restore UnusedMember.Local
            set
            {
                this.bccToDb = value;

                if (value != null && this.bccTo.Count == 0)
                {
                    var xml = XElement.Parse(value);
                    xml.Descendants().Select(n => new MailAddress(n.Value)).ToList().ForEach(a => this.BccTo.Add(a));
                }
            }
        }

        /// <summary>
        /// Used to marshall the XML into and out of EF.
        /// </summary>
        private String AttachmentsDb
        {
            // ReSharper disable UnusedMember.Local
            get { return this.attachmentsDb; }
            // ReSharper restore UnusedMember.Local
            set
            {
                this.attachmentsDb = value;

                if (value != null && this.attachments.Count == 0)
                {
                    var xml = XElement.Parse(value);
                    xml.Descendants()
                        .Select(n => new FileAttachment(n))
                        .ToList()
                        .ForEach(a => this.Attachments.Add(a));
                }
            }
        }

        /// <summary>
        /// Gets the list of <see cref="MailAddress"/> recipients to publicly send the <see cref="BillContent"/> to.
        /// </summary>
        /// <value>The list of <see cref="MailAddress"/> recipients to publicly send the <see cref="BillContent"/> to.</value>
        public virtual ICollection<MailAddress> SendTo
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<MailAddress>>() != null);
                return this.sendTo;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="MailAddress"/> recipients to privately send the <see cref="BillContent"/> to.
        /// </summary>
        /// <value>The list of <see cref="MailAddress"/> recipients to privately send the <see cref="BillContent"/> to.</value>
        public virtual ICollection<MailAddress> BccTo
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<MailAddress>>() != null);
                return this.bccTo;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="FileAttachment"/> to send with the <see cref="BillContent"/>.
        /// </summary>
        /// <value>The list of <see cref="FileAttachment"/> to send with the <see cref="BillContent"/>.</value>
        public virtual ICollection<FileAttachment> Attachments
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<FileAttachment>>() != null);
                return this.attachments;
            }
        }

        /// <summary>
        /// Gets the email box to sent the <see cref="BillContent"/> from.
        /// </summary>
        /// <value>The email box to sent the <see cref="BillContent"/> from.</value>
        public virtual String SendFrom
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);

                return this.sendFrom;
            }
            private set { this.sendFrom = value; }
        }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        /// <value>The content of the message.</value>
        public virtual String Body
        {
            get { return this.body; }
            set
            {
                this.CheckEditable();

                value = value ?? String.Empty;
                this.body = value;
            }
        }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        /// <value>The subject of the message.</value>
        public virtual String Subject
        {
            get { return this.subject; }
            set
            {
                this.CheckEditable();

                value = value ?? String.Empty;
                this.subject = value;
            }
        }

        /// <summary>
        /// Indicates whether the current <see cref="Body"/> content should be evaluated as HTML content.
        /// </summary>
        /// <value>True if the current <see cref="Body"/> content should be evaluated as HTML content; otherwise false.</value>
        public virtual Boolean IsHtml
        {
            get { return this.isHtml; }
            set
            {
                this.CheckEditable();

                this.isHtml = value;
            }
        }

        public virtual Order Order { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Performs validation that the current instance is editable.
        /// </summary>
        /// <remarks>The base version is safe to not be invoked.</remarks>
        protected virtual void CheckEditable()
        {
            if (!this.IsInitialized) return;

            if (this.Order.Status.CanBeEdited()) return;

            throw new InvalidOperationException($"The order is currently in the {this.Order.Status} state. Only {OrderStatus.Open} order bills may be edited.");
        }

        /// <summary>
        /// Called when a <see cref="DealBinder"/> is declined.
        /// </summary>
        protected internal virtual void Clear()
        {
            this.OnDraftCleared();
        }

        /// <summary>
        /// Marks the current instance as ready to be sent.
        /// </summary>
        public virtual void Complete()
        {
            if (!this.SendTo.Concat(this.BccTo).Any()) throw new InvalidOperationException("At least one mail recipient must be specified before being able to send");
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when a draft bill content is cleared.
        /// </summary>
        public event EventHandler DraftCleared;

        /// <summary>
        /// Raises the <see cref="DraftCleared"/> event.
        /// </summary>
        protected virtual void OnDraftCleared()
        {
            this.DraftCleared?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Responds to the <see cref="ObservableCollection{T}.CollectionChanged"/> events on the Send To and BCC collections.
        /// </summary>
        protected virtual void OnCollectionChanged(XElement target, ICollection<MailAddress> source)
        {
            var emails = source.Select(m => new XElement("email", m.Address)).OfType<Object>().ToArray();
            target.ReplaceAll(emails);
        }

        /// <summary>
        /// Responds to the <see cref="ObservableCollection{T}.CollectionChanged"/> events on the Attachments collection.
        /// </summary>
        protected virtual void OnCollectionChanged(XElement target, ICollection<FileAttachment> source)
        {
            var files = source.Select(f => f.ToXml()).OfType<Object>().ToArray();
            target.ReplaceAll(files);
        }

        #endregion

        #region ISupportInitialize Members

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            this.IsInitialized = false;

            this.sendTo.UnLock();
            this.bccTo.UnLock();
            this.attachments.UnLock();
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            this.IsInitialized = true;
            if (this.Order.Status.CanBeEdited()) return;

            this.sendTo.Lock();
            this.bccTo.Lock();
            this.attachments.Lock();

            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ISupportInitializeNotification Members

        /// <summary>
        /// Gets a value indicating whether the component is initialized.
        /// </summary>
        /// <returns>
        /// true to indicate the component has completed initialization; otherwise, false. 
        /// </returns>
        private Boolean IsInitialized { get; set; }

        /// <summary>
        /// Gets a value indicating whether the component is initialized.
        /// </summary>
        /// <returns>
        /// true to indicate the component has completed initialization; otherwise, false. 
        /// </returns>
        Boolean ISupportInitializeNotification.IsInitialized => this.IsInitialized;

        public event EventHandler Initialized;

        #endregion
    }
}