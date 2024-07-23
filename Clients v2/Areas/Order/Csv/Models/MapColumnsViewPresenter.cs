using System;
using System.Collections.Generic;
using AccurateAppend.JobProcessing.Manifest;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Order.Csv.Models
{
    /// <summary>
    /// Is a wrapper for a <see cref="ClientUploadOrderModel"/> type that instructs the column mapper view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class MapColumnsViewPresenter
    {
        #region Fields

        private Dictionary<Int32, List<String>> columnSamples;
        private List<Field> inputFields;
        private List<String> automappedFields;

        #endregion

        #region Constructor

        public MapColumnsViewPresenter()
        {
            this.inputFields = new List<Field>();
            this.columnSamples = new Dictionary<Int32, List<String>>();
        }

        #endregion

        #region Properties

        public MvcActionModel Postback { get; set; }

        /// <summary>
        /// Contains the actual order information that needs to be passed along.
        /// </summary>
        public ClientUploadOrderModel Order { get; set; }

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
        /// Columns preferred by manifest for append.
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

        /// <summary>
        /// Columns potentially identified for manifest to map.
        /// </summary>
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
        
        #endregion
    }
}