using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Simple value object used to represent a file attachment.
    /// </summary>
    public class FileAttachment
    {
        #region Fields

        private readonly String contentType;
        private readonly String filePath;
        private readonly String sendFileName;

        #endregion

        #region Constructors

        internal FileAttachment(XElement xml)
        {
            if (xml == null) throw new ArgumentNullException(nameof(xml));
            Contract.EndContractBlock();

            this.filePath = xml.Value;

            var attribute = xml.Attribute("type");
            this.contentType = (attribute?.Value ?? String.Empty).Trim();

            attribute = xml.Attribute("name");
            this.sendFileName = (attribute?.Value ?? String.Empty).Trim();

            attribute = xml.Attribute("zip");
            this.Compression = Boolean.Parse(attribute?.Value ?? "false");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachment"/> from the supplied file name path.
        /// </summary>
        /// <param name="filePath">The full path to the file to be attached. Must use UNC/HTTP paths.</param>
        public FileAttachment(Uri filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (!filePath.IsUnc && filePath.Scheme != Uri.UriSchemeHttps && filePath.Scheme != Uri.UriSchemeHttp) throw new ArgumentOutOfRangeException(nameof(filePath), filePath, "Only Unc/Http paths are supported");
            Contract.EndContractBlock();

            this.filePath = filePath.IsUnc ? filePath.LocalPath : filePath.ToString();
            this.contentType = String.Empty;
            this.sendFileName = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachment"/> from the supplied file name path and the indicated mime type.
        /// </summary>
        /// <param name="filePath">The full path to the file to be attached. Must use UNC paths.</param>
        /// <param name="contentType">The optional mime type of the attachment.</param>
        public FileAttachment(Uri filePath, String contentType) : this(filePath)
        {
            contentType = (contentType ?? String.Empty).Trim();

            this.contentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttachment"/> from the supplied file name path, the indicated mime type, and the name the file should be attached as.
        /// </summary>
        /// <param name="filePath">The full path to the file to be attached. Try to use UNC paths when possible.</param>
        /// <param name="contentType">The optional mime type of the attachment.</param>
        /// <param name="sendFileName">The optional name the file attachment should be sent as.</param>
        public FileAttachment(Uri filePath, String contentType, String sendFileName) : this(filePath, contentType)
        {
            sendFileName = (sendFileName ?? String.Empty).Trim();

            this.sendFileName = sendFileName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content mimetype for the attachment.
        /// </summary>
        /// <value>The content mimetype for the attachment.</value>
        public String ContentType
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);

                return this.contentType;
            }
        }

        /// <summary>
        /// Gets the fully qualitifed name of the file to attach to a <see cref="BillContent"/>.
        /// </summary>
        /// <value>The fully qualitifed name of the file to attach to a <see cref="BillContent"/>.</value>
        public String FilePath
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);

                return this.filePath;
            }
        }

        /// <summary>
        /// Gets the name the attachment should be attached as.
        /// </summary>
        /// <remarks>
        /// Files can be attached with alternative names from the source content.
        /// </remarks>
        /// <value>The name the attachment should be attached as.</value>
        public String SendFileName
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);

                return this.sendFileName;
            }
        }

        /// <summary>
        /// Indicates whether the mail attachment should be compressed before being sent.
        /// </summary>
        /// <value>True if the mail attachment should be compressed; otherwise false.</value>
        public Boolean Compression { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the current instance into an XML form.
        /// </summary>
        /// <returns>The XML equivalent for of the current instance.</returns>
        public virtual XElement ToXml()
        {
            Contract.Ensures(Contract.Result<XElement>() != null);
            Contract.EndContractBlock();

            var value = new XElement("file") { Value = this.FilePath };
            if (this.ContentType.Length > 0) value.SetAttributeValue("type", this.ContentType);
            if (this.SendFileName.Length > 0) value.SetAttributeValue("name", this.SendFileName);
            if (this.Compression) value.SetAttributeValue("zip", true);

            return value;
        }

        #endregion
    }
}
