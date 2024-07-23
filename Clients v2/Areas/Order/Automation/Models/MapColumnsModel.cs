using System;
using System.Collections.Generic;
using AccurateAppend.JobProcessing.Manifest;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Order.Automation.Models
{
    /// <summary>
    /// Model that instructs the column mapper view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class MapColumnsModel
    {
        #region Fields

        private Dictionary<Int32, List<String>> columnSamples;
        private List<Field> inputFields;
        private List<String> automappedFields;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MapColumnsModel"/> class.
        /// </summary>
        public MapColumnsModel()
        {
            this.inputFields = new List<Field>();
            this.columnSamples = new Dictionary<Int32, List<String>>();
        }

        #endregion

        #region Properties

        public MvcActionModel Postback { get; set; }

        /// <summary>
        /// Contains the client supplied / picked column map. If this value is null, it means no mapping is yet performed.
        /// </summary>
        public String ColumnMap { get; set; }

        /// <summary>
        /// Sample of records from each column in file
        /// </summary>
        public Dictionary<Int32, List<String>> ColumnSamples
        {
            get => this.columnSamples ?? (this.columnSamples = new Dictionary<Int32, List<String>>());
            set
            {
                if (value == null) value = new Dictionary<Int32, List<String>>();
                this.columnSamples = value;
            }
        }

        /// <summary>
        /// Columns preferred by manifest for append
        /// </summary>
        public List<Field> InputFields
        {
            get => this.inputFields ?? (this.inputFields = new List<Field>());
            set
            {
                if (value == null) value = new List<Field>();
                this.inputFields = value;
            }
        }

        public List<String> AutomappedFields
        {
            get => this.automappedFields ?? (this.automappedFields = new List<String>());
            set
            {
                if (value == null) value = new List<String>();
                this.automappedFields = value;
            }
        }

        /// <summary>
        /// Columns required by manifest for append
        /// </summary>
        public String RequiredFields { get; set; }

        /// <summary>
        /// Denotes if first row in file is a header row
        /// </summary>
        public Boolean HasHeaderRow { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the current cart.
        /// </summary>
        public Guid CartId { get; set; }

        #endregion
    }
}