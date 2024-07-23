using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Xml.Linq;
using AccurateAppend.Core.Utilities;
using AccurateAppend.JobProcessing.Manifest.Xml;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains the detailed information about a CSV file cart.
    /// </summary>
    public class CsvCart : Cart
    {
        #region Fields

        private String analysisInternal;
        private XElement analysis;
        private String manifestInternal;
        private XElement manifest;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvCart"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios. Use the factory methods for creating cart instances.</remarks>
        protected internal CsvCart()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Required for database. Do not directly use.
        /// </summary>
        protected internal virtual String AnalysisInternal
        {
            get => this.analysisInternal;
            set
            {
                this.analysisInternal = value;

                if (value == null)
                {
                    this.Analysis = null;
                }
                else
                {
                    var xml = XElement.Parse(value);
                    this.Analysis = xml;
                }
            }
        }

        /// <summary>
        /// Gets the performed analysis report for the current cart, if any.
        /// </summary>
        /// <value>The performed analysis report for the current cart, if any.</value>
        public virtual XElement Analysis
        {
            get => this.analysis;
            set
            {
                this.analysis = value;
                if (this.analysis == null)
                {
                    if (this.AnalysisInternal != null) this.AnalysisInternal = null;
                    return;
                }

                if (this.analysisInternal == null)
                {
                    this.AnalysisInternal = this.analysis.ToString();
                }
                this.analysis.Changed += (s, e) => this.analysisInternal = this.analysis.ToString();
            }
        }

        /// <summary>
        /// Gets the name of the system file that is stored as part of the order.
        /// </summary>
        public String SystemFileName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the identifier of the stored manifest definition that this cart will use.
        /// </summary>
        public Guid? ManifestId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Required for database. Do not directly use.
        /// </summary>
        public virtual String ManifestInternal
        {
            get => this.manifestInternal;
            set
            {
                this.manifestInternal = value;

                if (value == null)
                {
                    this.Manifest = null;
                }
                else
                {
                    var xml = XElement.Parse(value);
                    this.Manifest = xml;
                }
            }
        }

        /// <summary>
        /// Gets the performed analysis report for the current cart, if any.
        /// </summary>
        /// <value>The performed analysis report for the current cart, if any.</value>
        public virtual XElement Manifest
        {
            get => this.manifest;
            protected set
            {
                this.manifest = value;
                if (this.manifest == null)
                {
                    if (this.ManifestInternal != null) this.ManifestInternal = null;
                    return;
                }

                if (this.manifestInternal == null)
                {
                    this.ManifestInternal = this.manifest.ToString();
                }
                else
                {
                    this.manifestInternal = this.manifest.ToString();
                }

                this.manifest.Changed += (s, e) => this.manifestInternal = this.manifest.ToString();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Establishes a new manifest for the cart order.
        /// </summary>
        /// <param name="newManifest">The manifest content that is used to support the cart order process.</param>
        public virtual void LoadManifest(XElement newManifest)
        {
            if (newManifest == null) throw new ArgumentNullException(nameof(newManifest));
            Contract.Ensures(this.ManifestId != null);
            Contract.Ensures(this.Manifest != null);
            Contract.EndContractBlock();

            this.ManifestId = newManifest.ManifestId();
            this.Manifest = new XElement(newManifest);
        }

        /// <summary>
        /// Used to update the file information for the cart.
        /// </summary>
        /// <remarks>
        /// Only used for CSV orders.
        /// </remarks>
        /// <param name="systemFile">The <see cref="CsvFileContent"/> providing access to the system stored content.</param>
        /// <param name="customerFileName">The original customer supplied file name.</param>
        public virtual async Task UpdateFileInformation(CsvFileContent systemFile, String customerFileName)
        {

            systemFile.Delimiter = await CsvFileContent.DiscoverDelimiterAsync(systemFile).ConfigureAwait(false) ??
                                   CsvFileContent.DefaultDelimiter;
            this.Name = customerFileName;
            this.RecordCount = systemFile.CountRecords();
            this.SystemFileName = systemFile.Name;
        }

        #endregion
    }
}