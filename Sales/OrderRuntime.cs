using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains the processing results information for a specific <see cref="BillableOrder"/> instance.
    /// </summary>
    /// <remarks>
    /// Isolates the processing report details (which are not always needed and in the case of
    /// XML data, rather large) from the rest of the core order behaviors. Every <seealso cref="OrderRuntime"/>
    /// has exactly one <see cref="BillableOrder"/> instance but each order instance may refer to exactly 0 or 1
    /// <seealso cref="BillableOrder"/>.
    /// </remarks>
    public class OrderRuntime
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRuntime"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected OrderRuntime()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRuntime"/> class.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> instance that this processing extends.</param>
        protected internal OrderRuntime(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            Contract.EndContractBlock();

            this.source = order;
        }

        #endregion

        #region Fields

        private String reportInternal;
        private XElement report;
        private Order source;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value uniquely identifying the object instance. 
        /// </summary>
        /// <value>The value uniquely identifying the object instance. </value>
        public Int32? Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> instance the current processing information is for.
        /// </summary>
        /// <value>The <typeparamref name="T"/> instance the current processing information is for.</value>
        public virtual Order Source
        {
            get { return this.source; }
            protected set { this.source = value; }
        }

        /// <summary>
        /// Required for database. Do not directly use.
        /// </summary>
        protected virtual String ReportInternal
        {
            get { return this.reportInternal; }
            set
            {
                this.reportInternal = value;

                if (value == null)
                {
                    this.Report = null;
                }
                else
                {
                    var xml = XElement.Parse(value);
                    this.Report = xml;
                }
            }
        }

        /// <summary>
        /// Gets the processing report for the current <see cref="Source"/>.
        /// </summary>
        /// <value>The processing report for the current <see cref="Source"/>.</value>
        public virtual XElement Report
        {
            get { return this.report; }
            protected set
            {
                this.report = value;
                if (this.report == null)
                {
                    if (this.ReportInternal != null) this.ReportInternal = null;
                    return;
                }

                if (this.reportInternal == null)
                {
                    this.ReportInternal = this.report.ToString();
                }
                this.report.Changed += (s, e) => this.reportInternal = this.report.ToString();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the <see cref="Report"/> to the content of the <paramref name="newReport"/>.
        /// </summary>
        /// <param name="newReport">The XML processing report content. If null, the existing <see cref="Report"/> will be removed.</param>
        public virtual void AssociateReport(XElement newReport)
        {
            //Contract.Ensures(newReport == null && this.Report == null || ReferenceEquals(this.Report, newReport));
            Contract.EndContractBlock();

            this.Report = newReport;
        }

        #endregion
    }
}
